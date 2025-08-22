/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using PX.Commerce.Amazon.Amazon.DAC;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Client.Interface;
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Amazon.Sync.Interfaces;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
	public class AmazonPaymentEntityBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary => Payment;
		public IMappedEntity[] Entities => new IMappedEntity[] { Primary };

		public MappedPayment Payment;
		public MappedFBMOrder FBMOrder;
		public MappedFBAOrder FBAOrder;
		public MappedInvoice FBAOrderAsInvoice;
	}

	public class AmazonPaymentsRestrictor : BCBaseRestrictor, IRestrictor
	{
		public FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return null;
		}

		public FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			#region Payments
			return base.Restrict<MappedPayment>(mapped, delegate (MappedPayment obj)
			{
				if (obj.Extern != null)
				{
					BCBindingExt bindingExt = processor.GetBindingExt<BCBindingExt>();
					string orderId = obj.Extern?.ShipmentEvent?.AmazonOrderId;
					if (processor.SelectStatus(BCEntitiesAttribute.Order, orderId, false) == null &&
						processor.SelectStatus(BCEntitiesAttribute.SOInvoice, orderId, false) == null &&
						processor.SelectStatus(BCEntitiesAttribute.OrderOfTypeInvoice, orderId, false) == null)
					{
						//Skip if order not synced
						return new FilterResult(FilterStatus.Invalid,
								PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogPaymentSkippedOrderNotSynced, obj.ExternID, orderId));
					}
				}

				return null;
			});
			#endregion
		}
	}

	[BCProcessor(typeof(BCAmazonConnector), BCEntitiesAttribute.Payment, BCCaptions.Payment, 230,
		IsInternal = false,
		Direction = SyncDirection.Import,
		PrimaryDirection = SyncDirection.Import,
		PrimarySystem = PrimarySystem.Extern,
		PrimaryGraph = typeof(PX.Objects.AR.ARPaymentEntry),
		ExternTypes = new Type[] { typeof(Order) },
		LocalTypes = new Type[] { typeof(Payment) },
		AcumaticaPrimaryType = typeof(PX.Objects.AR.ARPayment),
		AcumaticaPrimarySelect = typeof(Search<PX.Objects.AR.ARPayment.refNbr, Where<PX.Objects.AR.ARPayment.docType, Equal<ARDocType.payment>>>),
		URL = "orders-v3/order/{0}",
		RequiresOneOf = new string[] { BCEntitiesAttribute.Order + "." + BCEntitiesAttribute.SOInvoice + "." + BCEntitiesAttribute.SOInvoice })]
	public class AmazonPaymentProcessor : BCProcessorSingleBase<AmazonPaymentProcessor, AmazonPaymentEntityBucket, MappedPayment>, IProcessor
	{
		private IPaymentMethodFeeTypeHandler _feeTypeHandler;

		private IShipmentEventDataProvider _shipmentEventDataProvider;

		private IOrderDataProvider _orderDataProvider;

		private AmazonHelper _helper = PXGraph.CreateInstance<AmazonHelper>();

		[InjectDependency]
		private IPaymentMethodFeeTypeHandlerFactory FeeTypeHandlerFactory { get; set; }

		[InjectDependency]
		public IOrderFinanceEventsDataProviderFactory FinanceEventsDataProviderFactory { get; set; }

		[InjectDependency]
		private IShipmentEventDataProviderFactory ShipmentEventDataProviderFactory { get; set; }

		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);
			_helper.Initialize(this);
			var binding = base.GetBinding();
			var client = ((BCAmazonConnector)iconnector).GetRestClient(GetBindingExt<BCBindingAmazon>(), binding);
			_feeTypeHandler = this.FeeTypeHandlerFactory.CreateInstance(binding.BindingID.Value, this, true);
			_shipmentEventDataProvider = this.ShipmentEventDataProviderFactory.CreateInstance(this.FinanceEventsDataProviderFactory.CreateInstance(client));
			_orderDataProvider = new OrderDataProvider(client);
		}

		public override async Task<MappedPayment> PullEntity(String externID, String jsonObject, CancellationToken cancellationToken = default)
		{
			return null;
		}

		public override async Task<MappedPayment> PullEntity(Guid? localID, Dictionary<string, object> fields, CancellationToken cancellationToken = default)
		{
			return null;
		}
		public override async Task<PullSimilarResult<MappedPayment>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{
			var externEntity = (ExternalPaymentData)entity;

			string uniqueField = externEntity.ID;
			if (string.IsNullOrEmpty(uniqueField))
				return null;

			List<MappedPayment> result = new List<MappedPayment>();
			foreach (PX.Objects.AR.ARRegister item in GetHelper<AmazonHelper>().PaymentByExternalRef.Select(uniqueField))
			{
				Payment data = new Payment() { SyncID = item.NoteID, SyncTime = item.LastModifiedDateTime };
				result.Add(new MappedPayment(data, data.SyncID, data.SyncTime));
			}

			return new PullSimilarResult<MappedPayment>() { UniqueField = uniqueField, Entities = result };
		}

		public override async Task<PXTransactionScope> WithTransaction(Func<Task> action)
		{
			await action();
			return null;
		}

		#region Import

		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			List<string> ordersToBeProcessed = new List<string>();
			DateTime? latestDate = null;
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			var filter = GenerateFinancialEventFilter(minDateTime, maxDateTime, bindingExt.SyncOrdersFrom);

			await Task.Yield();
			await foreach (var shipment in _shipmentEventDataProvider.GetListOfShipmentEvents(filter, cancellationToken))
			{
				if (!ordersToBeProcessed.Contains(shipment.AmazonOrderId))
				{
					ordersToBeProcessed.Add(shipment.AmazonOrderId);
				}

				//As API has limitation of fetching only 180 days data we need to get last date which is fetched by api to store in LastIncremantalImport date
				if (!latestDate.HasValue)
				{
					latestDate = shipment.PostedDate.ToDate();
				}
				else if (shipment.PostedDate.ToDate() > latestDate)
				{
					latestDate = shipment.PostedDate.ToDate();
				}
			}

			foreach (var order in ordersToBeProcessed)
			{
				int index = 0;
				await foreach (var shipmentEvent in _shipmentEventDataProvider.GetListOfShipmentEventsBy(order, cancellationToken))
				{
					string externID = new object[] { shipmentEvent.AmazonOrderId, index + 1 }.KeyCombine();
					ExternalPaymentData externalOrderData = new ExternalPaymentData(externID, null, shipmentEvent);

					MappedPayment obj = new MappedPayment();
					var descr = string.Format(AmazonMessages.PaymentExternalDescrTemplate, externID, shipmentEvent.PostedDate);
					obj.AddExtern(externalOrderData, externID, descr, shipmentEvent.PostedDate.ToDate());
					EntityStatus status = EnsureStatus(obj, SyncDirection.Import);

					index++;
				}
			}

			//override the SyncTime to latesdate fetched by api in case of incremental so that next fetch will start from that date
			if (latestDate.HasValue && Operation.PrepareMode == PrepareMode.Incremental)
			{
				SyncTime = latestDate.Value;
			}
		}

		public virtual FinancialEventsFilter GenerateFinancialEventFilter(DateTime? minDateTime, DateTime? maxDateTime, DateTime? syncOrdersFrom)
		{
			FinancialEventsFilter filter = new FinancialEventsFilter();
			if (minDateTime.HasValue || maxDateTime.HasValue)
			{
				DateTime? localMinDateTime = minDateTime?.ToDate();
				filter.PostedAfter = localMinDateTime >= syncOrdersFrom ? localMinDateTime : syncOrdersFrom;
				if (maxDateTime.HasValue)
				{
					filter.PostedBefore = maxDateTime.ToDate();
				}
			}
			else if (syncOrdersFrom.HasValue)
			{
				filter.PostedAfter = syncOrdersFrom.Value;
			}

			return filter;
		}

		public override void ControlDirection(AmazonPaymentEntityBucket bucket, BCSyncStatus status, ref bool shouldImport, ref bool shouldExport, ref bool skipSync, ref bool skipForce)
		{
			//TODO: CHECK HOW IT WORKS with new FBA
			var order = bucket.FBAOrder?.Extern;
			var invoice = bucket.FBAOrderAsInvoice?.Extern;

			//Mark payment as Invalied for ReplacementOrder
			if (order?.IsReplacementOrder == true || invoice?.IsReplacementOrder == true)
			{
				skipForce = true;
				skipSync = true;
				SetInvalidStatus(bucket.Primary, PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.PaymentForReplacementOrderIsInvalid, order?.AmazonOrderId ?? invoice?.AmazonOrderId));
				shouldImport = false;
			}

			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			// Mark a payment as filtered for an order that was created before SyncOrdersFrom
			if (Operation.SyncMethod != SyncMode.Force && bindingExt.SyncOrdersFrom != null
				&& ((order != null && order?.PurchaseDate?.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(bindingExt.OrderTimeZone)) < bindingExt.SyncOrdersFrom)
				|| (invoice != null && invoice?.PurchaseDate?.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(bindingExt.OrderTimeZone)) < bindingExt.SyncOrdersFrom)))
			{
				skipSync = true;
				UpdateSyncStatus(bucket.Payment, null, PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.LogPaymentSkippedCreatedBeforeSyncOrdersFrom, order?.AmazonOrderId ?? invoice?.AmazonOrderId, bindingExt.SyncOrdersFrom.Value.Date.ToString("d")), BCSyncStatusAttribute.Filtered);
				shouldImport = false;
			}
		}

		public override async Task<EntityStatus> GetBucketForImport(AmazonPaymentEntityBucket bucket, BCSyncStatus syncStatus, CancellationToken cancellationToken = default)
		{
			(string amazonOrderID, int shipmentEventID) splitedExternalID = SplitExternalID(syncStatus.ExternID);

			Order order = await _orderDataProvider.GetById(splitedExternalID.amazonOrderID);
			if (order?.AmazonOrderId == null) return EntityStatus.None;

			ShipmentEvent shipmentEvent = await _shipmentEventDataProvider.GetShipmentEventBy(splitedExternalID.amazonOrderID, splitedExternalID.shipmentEventID, cancellationToken);

			int? parentId;

			// there can only be FBM or FBA order thus it can only be order or invoice
			if (order.FulfillmentChannel == FulfillmentChannel.MFN)
			{
				MappedFBMOrder mappedFBMOrder = bucket.FBMOrder = bucket.FBMOrder.Set(order, order.AmazonOrderId?.ToString(), order.AmazonOrderId?.ToString(), order.LastUpdateDate.ToDate());
				EnsureStatus(mappedFBMOrder);
				parentId = mappedFBMOrder.SyncID;
			}
			else
			{
				if (FBAOrderImportedAsInvoice(splitedExternalID.amazonOrderID))
				{
					MappedInvoice invoiceObj = bucket.FBAOrderAsInvoice = bucket.FBAOrderAsInvoice.Set(order, order.AmazonOrderId?.ToString(), order.AmazonOrderId?.ToString(), order.LastUpdateDate.ToDate());
					EntityStatus invoiceStatus = EnsureStatus(invoiceObj);
					parentId = invoiceObj.SyncID;
				}
				else
				{
					MappedFBAOrder mappedFBAOrder = bucket.FBAOrder = bucket.FBAOrder.Set(order, order.AmazonOrderId?.ToString(), order.AmazonOrderId?.ToString(), order.LastUpdateDate.ToDate());
					EntityStatus invoiceStatus = EnsureStatus(mappedFBAOrder);
					parentId = mappedFBAOrder.SyncID;
				}
			}

			ExternalPaymentData externalOrderData = new ExternalPaymentData(syncStatus.ExternID, order, shipmentEvent);
			MappedPayment paymentObj = bucket.Payment = bucket.Payment.Set(externalOrderData, syncStatus.ExternID, syncStatus.ExternDescription, shipmentEvent.PostedDate.ToDate()).With(_ => { _.ParentID = parentId; return _; });
			EntityStatus paymentStatus = EnsureStatus(paymentObj, SyncDirection.Import);

			return paymentStatus;
		}

		protected virtual bool FBAOrderImportedAsInvoice(string amazonOrderID) =>
			SelectStatus(BCEntitiesAttribute.SOInvoice, amazonOrderID, false) != null &&
			SelectStatus(BCEntitiesAttribute.OrderOfTypeInvoice, amazonOrderID, false)?.LocalID is null;

		protected virtual (string amazonOrderID, int shipmentEventID) SplitExternalID(string externID) =>
			IsExternalIDValid(externID) ?
				(externID.KeySplit(0), int.Parse(externID.KeySplit(1)))
			: throw new PXException(AmazonMessages.TheExternalPaymentIDIsNotCorrect);

		protected virtual bool IsExternalIDValid(string externID) =>
			externID.Contains(";")
			&& !string.IsNullOrEmpty(externID?.Split(';')[0])
			&& !string.IsNullOrEmpty(externID?.Split(';')[1]);

		public override async Task MapBucketImport(AmazonPaymentEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			MappedPayment obj = bucket.Payment;

			Order externalOrder = obj.Extern.Order;
			ShipmentEvent shipmentEvent = obj.Extern.ShipmentEvent;
			Payment impl = obj.Local = new Payment();

			BCBinding binding = GetBinding();
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			Payment presented = existing?.Local as Payment;

			// process data received from Finances API for charges and fees of the order
			impl.Charges = new List<PaymentCharge>();

			// map appropriate payment method (as well as cash account Id)
			string currencyCode = GetCurrencyCode(externalOrder, shipmentEvent);
			BCPaymentMethods methodMapping = GetHelper<AmazonHelper>().GetPaymentMethodMapping(externalOrder.PaymentMethod, currencyCode, out string cashAcount);
			var paymentMappingId = methodMapping.PaymentMappingID;
			var cashAccountId = methodMapping.CashAccountID ?? 0;
			var storePaymentMethod = methodMapping.StorePaymentMethod;
			// if the payment already exists then we neeed to keep the same payment method and cash account
			if (presented != null && presented.PaymentMethod.Value != methodMapping?.PaymentMethodID?.Trim())
			{
				impl.PaymentMethod = presented.PaymentMethod;
				impl.CashAccount = presented.CashAccount;

				// get paymentMappingId and cashAccountId of existing payment method mapping
				BCPaymentMethods existingPaymentMapping = GetHelper<AmazonHelper>().GetPaymentMethodMapping(presented.PaymentMethod.Value, currencyCode, out _);
				paymentMappingId = existingPaymentMapping.PaymentMappingID;
				cashAccountId = existingPaymentMapping.CashAccountID ?? 0;
				storePaymentMethod = existingPaymentMapping.StorePaymentMethod;
			}
			else
			{
				impl.PaymentMethod = methodMapping?.PaymentMethodID?.Trim()?.ValueField();
				impl.CashAccount = cashAcount?.Trim()?.ValueField();
			}

			decimal? calculatedPaymentAmount = 0;

			PXCache cache = base.Caches[typeof(BCFeeMapping)];
			List<ShipmentItem> shipmentItems = shipmentEvent.ShipmentItemList;

			// iterate through each item in the order to process charges and fees of each item individually
			foreach (var shipmentItem in shipmentItems)
			{
				decimal? sumCharges = shipmentItem.ItemChargeList?.Sum(x => x.ChargeAmount?.CurrencyAmount?.ToDecimal(NumberStyles.Currency) ?? 0m) ?? 0m;
				decimal? sumPromotions = shipmentItem.PromotionList?.Sum(x => x.PromotionAmount?.CurrencyAmount?.ToDecimal(NumberStyles.Currency) ?? 0m) ?? 0m;
				// promotion amounts are negative, thus sum of charges and promotions here will yield payment amount of each item
				calculatedPaymentAmount += sumCharges + sumPromotions;

				// get all fee types of current item
				List<string> itemFeesStr = shipmentItem.ItemFeeList?.Select(x => x.FeeType)?.ToList() ?? new List<string>();
				// then concat it with list of Taxes charged because Taxes are considered charges/fees for Amazon orders
				IEnumerable<ChargeComponent> itemTaxes = shipmentItem.ItemTaxWithheldList?.SelectMany(x => x.TaxesWithheld) ?? Enumerable.Empty<ChargeComponent>();
				itemFeesStr.AddRange(itemTaxes.Select(x => x.ChargeType));

				_feeTypeHandler.AddMissedFeeTypes(itemFeesStr);

				// iterate through list of item fees and taxes to calculate fees/charges for each item
				foreach (var itemFee in shipmentItem.ItemFeeList)
				{
					BCFeeMapping feeMapping = _feeTypeHandler.GetStoredFeeMapping(itemFee.FeeType);
					decimal feeAmount = Math.Abs(itemFee.FeeAmount?.CurrencyAmount?.ToDecimal(NumberStyles.Currency) ?? 0m);

					var feeMappingExt = feeMapping.GetExtension<BCFeeMappingExtAmazon>();

					if (!feeMappingExt.Active.HasValue && feeMappingExt.Active.Value)
					{
						this.LogInfo(this.Operation.LogScope(), AmazonMessages.FeeMappingIsInactive, feeMappingExt.FeeDescription, feeAmount);
						continue;
					}

					if (!impl.Charges.Any(x => x.EntryTypeID.Value == feeMapping.EntryTypeID))
					{
						impl.Charges.Add(new PaymentCharge()
						{
							EntryTypeID = feeMapping.EntryTypeID.ValueField(),
							Amount = 0m.ValueField()
						});
					}

					impl.Charges.FirstOrDefault(x => x.EntryTypeID.Value == feeMapping.EntryTypeID).Amount.Value += feeAmount;
				}

				// iterate through tax list
				foreach (var itemFee in itemTaxes)
				{
					BCFeeMapping feeMapping = _feeTypeHandler.GetStoredFeeMapping(itemFee.ChargeType);
					decimal feeAmount = Math.Abs(itemFee.ChargeAmount?.CurrencyAmount?.ToDecimal(NumberStyles.Currency) ?? 0m);

					var feeMappingExt = feeMapping.GetExtension<BCFeeMappingExtAmazon>();

					if (!feeMappingExt.Active.HasValue && feeMappingExt.Active.Value)
					{
						this.LogInfo(this.Operation.LogScope(), AmazonMessages.FeeMappingIsInactive, feeMappingExt.FeeDescription, feeAmount);
						continue;
					}

					// init charge with amount=0 if it does not yet exist
					if (!impl.Charges.Any(x => x.EntryTypeID.Value == feeMapping.EntryTypeID))
					{
						impl.Charges.Add(new PaymentCharge()
						{
							EntryTypeID = feeMapping.EntryTypeID.ValueField(),
							Amount = 0m.ValueField()
						});
					}

					// calculate and assign charge amount from item's amount
					impl.Charges.FirstOrDefault(x => x.EntryTypeID.Value == feeMapping.EntryTypeID).Amount.Value += feeAmount;
				}
			}
			// remove Charge entries where amount is 0
			impl.Charges.RemoveAll(x => x.Amount.Value == 0);
			cache.Persisted(false);

			// in order to avoid charge entries from being duplicated, we need to map key columns of the coming records with existing ones
			// Side note: the other approach of deleting all existing entries and inserting new ones does not work due to complex field references
			// to multiple tables of accountId (cashAccountId) column
			if (presented != null && presented.Charges.Any())
			{
				List<int> existingLineNbr = new List<int>();
				foreach (var charge in impl.Charges)
				{
					var existingCharge = presented.Charges.FirstOrDefault(x => x.Amount.Value == charge.Amount.Value
						&& x.EntryTypeID.Value == charge.EntryTypeID.Value
						&& !existingLineNbr.Contains((int)x.LineNbr.Value));

					if (existingCharge != null)
					{
						charge.DocType = existingCharge.DocType;
						charge.LineNbr = existingCharge.LineNbr;
						charge.RefNbr = existingCharge.RefNbr;

						existingLineNbr.Add((int)charge.LineNbr.Value);
					}
				}
			}

			ARInvoice invoice = null;
			PX.Objects.SO.SOOrder order = null;
			PX.Objects.AR.Customer customer;
			PX.Objects.CR.Location location;
			bool? anyUnreleasedPayments = false;

			bool FBAImportedAsOrder = bucket.FBMOrder != null ||
				(bucket.FBAOrder != null && SelectStatus(BCEntitiesAttribute.OrderOfTypeInvoice, obj.Extern?.Order.AmazonOrderId, true) != null);

			if (FBAImportedAsOrder)
			{
				PXResult<PX.Objects.SO.SOOrder, PX.Objects.AR.Customer, PX.Objects.CR.Location, BCSyncStatus> result = PXSelectJoin<PX.Objects.SO.SOOrder,
					   InnerJoin<PX.Objects.AR.Customer, On<PX.Objects.AR.Customer.bAccountID, Equal<SOOrder.customerID>>,
					   InnerJoin<PX.Objects.CR.Location, On<PX.Objects.CR.Location.locationID, Equal<SOOrder.customerLocationID>>,
					   InnerJoin<BCSyncStatus, On<PX.Objects.SO.SOOrder.noteID, Equal<BCSyncStatus.localID>>>>>,
					   Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
						   And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
						   And<BCSyncStatus.entityType, In<Required<BCEntity.entityType>>,
						   And<BCSyncStatus.externID, Equal<Required<BCSyncStatus.externID>>>>>>>
					   .Select(this, new object[] { BCEntitiesAttribute.Order, BCEntitiesAttribute.OrderOfTypeInvoice }, externalOrder.AmazonOrderId)
					   .Select(r => (PXResult<PX.Objects.SO.SOOrder, PX.Objects.AR.Customer, PX.Objects.CR.Location, BCSyncStatus>)r).FirstOrDefault();

				if (result == null) throw new PXException(BCMessages.LogPaymentSkippedOrderNotSynced, bucket.Payment.ExternID, externalOrder.AmazonOrderId);

				order = result.GetItem<PX.Objects.SO.SOOrder>();
				customer = result.GetItem<PX.Objects.AR.Customer>();
				location = result.GetItem<PX.Objects.CR.Location>();

				// when order status is Completed, check if it has already been invoiced
				// if yes, proceed adding the payment to corresponding invoice as same as for FBA order
				if (order.Status.IsIn(SOOrderStatus.Completed, SOOrderStatus.Invoiced))
				{
					PX.Objects.SO.SOOrderShipment shipmentResult =
						SelectFrom<PX.Objects.SO.SOOrderShipment>
						.Where<PX.Objects.SO.SOOrderShipment.orderType.IsEqual<@P.AsString>
						.And<PX.Objects.SO.SOOrderShipment.orderNbr.IsEqual<@P.AsString>>>.View.Select(this, order.OrderType, order.OrderNbr);

					if (shipmentResult != null)
					{
						IEnumerable<PXResult<ARInvoice, ARAdjust>> invoiceAndAdjustmentsResult = PXSelectJoin<ARInvoice,
									LeftJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
										And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
										And<ARAdjust.voided, Equal<False>>>>>,
									Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
									And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(this, shipmentResult.InvoiceType, shipmentResult.InvoiceNbr)
									.Cast<PXResult<ARInvoice, ARAdjust>>();

						invoice = invoiceAndAdjustmentsResult?.FirstOrDefault()?.GetItem<ARInvoice>();
						anyUnreleasedPayments = CheckUnreleasedAdjustments(presented, invoiceAndAdjustmentsResult.RowCast<ARAdjust>());
					}
				}
			}
			else
			{
				IEnumerable<PXResult<ARInvoice, PX.Objects.AR.Customer, PX.Objects.CR.Location, BCSyncStatus, ARAdjust>> result = PXSelectJoin<ARInvoice,
					   InnerJoin<PX.Objects.AR.Customer, On<PX.Objects.AR.Customer.bAccountID, Equal<ARInvoice.customerID>>,
					   InnerJoin<PX.Objects.CR.Location, On<PX.Objects.CR.Location.locationID, Equal<ARInvoice.customerLocationID>>,
					   InnerJoin<BCSyncStatus, On<ARInvoice.noteID, Equal<BCSyncStatus.localID>>,
					   LeftJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
						   And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
						   And<ARAdjust.voided, Equal<False>>>>>>>>,
					   Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
						   And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
						   And<BCSyncStatus.entityType, Equal<Required<BCEntity.entityType>>,
						   And<BCSyncStatus.externID, Equal<Required<BCSyncStatus.externID>>>>>>>
					   .Select(this, BCEntitiesAttribute.SOInvoice, externalOrder.AmazonOrderId)
					   .Cast<PXResult<ARInvoice, PX.Objects.AR.Customer, PX.Objects.CR.Location, BCSyncStatus, ARAdjust>>();

				if (result == null) throw new PXException(BCMessages.LogPaymentSkippedOrderNotSynced, bucket.Payment.ExternID, externalOrder.AmazonOrderId);

				invoice = result.FirstOrDefault()?.GetItem<ARInvoice>();
				customer = result.FirstOrDefault()?.GetItem<PX.Objects.AR.Customer>();
				location = result.FirstOrDefault()?.GetItem<PX.Objects.CR.Location>();
				anyUnreleasedPayments = CheckUnreleasedAdjustments(presented, result.RowCast<ARAdjust>());
			}

			if (invoice == null && order == null) throw new PXException(AmazonMessages.OrderNotSynced, externalOrder.AmazonOrderId);
			if (invoice?.Released == false || anyUnreleasedPayments == true)
			{
				throw new PXException(AmazonMessages.InvoiceOrApplicationsAreNotReleased, invoice.RefNbr);
			}

			//Product
			impl.Type = PX.Objects.AR.Messages.Payment.ValueField();
			impl.CustomerID = customer.AcctCD.ValueField();
			impl.CustomerLocationID = location.LocationCD.ValueField();
			impl.CurrencyID = currencyCode.ValueField();
			var appDate = shipmentEvent.PostedDate.ToDate(timeZone: PXTimeZoneInfo.FindSystemTimeZoneById(GetBindingExt<BCBindingExt>()?.OrderTimeZone));
			if (appDate.HasValue)
				impl.ApplicationDate = (new DateTime(appDate.Value.Date.Ticks)).ValueField();

			impl.PaymentAmount = calculatedPaymentAmount.ValueField();
			impl.Hold = false.ValueField();

			impl.NeedRelease = methodMapping?.ReleasePayments ?? false;

			var paymentDesc = PXMessages.LocalizeFormat(AmazonMessages.PaymentDescription,
						GetBinding().BindingName,
						externalOrder.AmazonOrderId,
						externalOrder.PaymentMethod.ToString());
			impl.Description = paymentDesc.ValueField();

			impl.ExternalRef = bucket.Payment.Extern.ID.ValueField();
			impl.PaymentRef = impl.ExternalRef;

			// TODO: refactor, the logic is the same for FBA and FBM
			/// if there is an invoice, we need to link the payment to the invoice
			if (invoice != null)
			{
				if ((existing as MappedPayment) == null || ((MappedPayment)existing).Local == null ||
				((MappedPayment)existing).Local.DocumentsToApply == null ||
				!((MappedPayment)existing).Local.DocumentsToApply.Any(d => d.DocType?.Value == ARDocType.Invoice && d.ReferenceNbr?.Value == invoice.RefNbr))
				{
					// compare unpaid balance with calculated amount and assign the smaller amount to the payment
					decimal unpaidBalance = invoice.CuryDocBal ?? 0m;
					decimal amount = calculatedPaymentAmount > unpaidBalance ? unpaidBalance : (decimal)calculatedPaymentAmount;

					PaymentDetail detail = new PaymentDetail();
					detail.DocType = ARDocType.Invoice.ValueField();
					detail.ReferenceNbr = invoice.RefNbr.ValueField();
					detail.AmountPaid = amount.ValueField();
					impl.DocumentsToApply = new List<PaymentDetail>(new[] { detail });
				}
			}
			else if (externalOrder.OrderTotal?.Amount != null)
			{
				// TODO: curyUnpaidBalance is calculated but it could never be used if the next if statement is true. Maybe we should move this from here.
				//Calculated Unpaid Balance
				decimal curyUnpaidBalance = order.CuryOrderTotal ?? 0m;
				foreach (SOAdjust adj in PXSelect<SOAdjust,
								Where<SOAdjust.adjdOrderType, Equal<Required<SOOrder.orderType>>,
									And<SOAdjust.adjdOrderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(this, order.OrderType, order.OrderNbr))
				{
					curyUnpaidBalance -= adj.CuryAdjdAmt ?? 0m;
				}

				if ((existing as MappedPayment) == null || ((MappedPayment)existing).Local == null ||
					((MappedPayment)existing).Local.OrdersToApply == null ||
					!((MappedPayment)existing).Local.OrdersToApply.Any(d => d.OrderType?.Value == order.OrderType && d.OrderNbr?.Value == order.OrderNbr))
				{
					decimal applicationAmount = 0m;
					var amount = externalOrder.OrderTotal.Amount.ToDecimal(NumberStyles.Currency);
					applicationAmount = amount > curyUnpaidBalance ? curyUnpaidBalance : (decimal)amount;

					//Order to Apply
					PaymentOrderDetail detail = new PaymentOrderDetail();
					detail.OrderType = order.OrderType.ValueField();
					detail.OrderNbr = order.OrderNbr.ValueField();
					detail.AppliedToOrder = applicationAmount.ValueField();
					impl.OrdersToApply = new List<PaymentOrderDetail>(new[] { detail });
				}
			}
		}

		protected virtual bool? CheckUnreleasedAdjustments(Payment presentedPayment, IEnumerable<ARAdjust> adjustments) =>
			presentedPayment != null
				? adjustments
					.Where(x => x.AdjgDocType != presentedPayment.Type.Value && x.AdjgRefNbr != presentedPayment.ReferenceNbr.Value)?
					.Any(x => x.Released == false)
				: adjustments?
					.Any(x => x.Released == false);

		public virtual string GetCurrencyCode(Order externalOrder, ShipmentEvent shipmentEvent)
		{
			//It is possible that the Order Total object doesn't exist, and in this case we have to get the currency code from somewhere else.
			if (string.IsNullOrEmpty(externalOrder?.OrderTotal?.CurrencyCode) == false)
				return externalOrder.OrderTotal.CurrencyCode;

			string currencyCode = shipmentEvent?.ShipmentItemList?
				.SelectMany(x => x?.ItemFeeList)?
				.FirstOrDefault(itemFee => string.IsNullOrEmpty(itemFee?.FeeAmount?.CurrencyCode) != null)?.FeeAmount.CurrencyCode;
			if (string.IsNullOrEmpty(currencyCode))
				throw new PXArgumentException(nameof(currencyCode));

			return currencyCode;
		}

		public override async Task SaveBucketImport(AmazonPaymentEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{
			MappedPayment obj = bucket.Payment;
			Boolean needRelease = obj.Local.NeedRelease;
			BCSyncStatus parentStatus;
			Payment presented = existing?.Local as Payment;

			if (bucket.FBAOrderAsInvoice != null)
			{
				parentStatus = PXSelectJoin<BCSyncStatus,
					InnerJoin<ARInvoice, On<ARInvoice.noteID, Equal<BCSyncStatus.localID>>>,
					Where<BCSyncStatus.syncID, Equal<Required<BCSyncStatus.syncID>>>>.Select(this, bucket.FBAOrderAsInvoice.SyncID);
			}
			else // FBM - order
			{
				int? parentSyncID = bucket.FBMOrder?.SyncID ?? bucket.FBAOrder?.SyncID;
				parentStatus = PXSelectJoin<BCSyncStatus,
					InnerJoin<SOOrder, On<SOOrder.noteID, Equal<BCSyncStatus.localID>,
						And<SOOrder.lastModifiedDateTime, Equal<BCSyncStatus.localTS>>>>,
					Where<BCSyncStatus.syncID, Equal<Required<BCSyncStatus.syncID>>>>.Select(this, parentSyncID);
			}

			Payment impl = null;
			if (existing?.Local == null)
			{
				using (var transaction = await WithTransaction(async () =>
				{
					impl = cbapi.Put<Payment>(obj.Local, obj.LocalID);
					bucket.Payment.AddLocal(impl, impl.SyncID, impl.SyncTime);

				}))
				{
					transaction?.Complete();
				}
			}
			else
			{
				impl = existing?.Local as Payment;
			}


			if (needRelease && impl.Status?.Value == PX.Objects.AR.Messages.Balanced)
			{
				using (var transaction = await WithTransaction(async () =>
				{
					try
					{
						impl = cbapi.Invoke<Payment, ReleasePayment>(null, impl.Id, ignoreResult: !WebConfig.ParallelProcessingDisabled);
						if (impl != null) bucket.Payment.AddLocal(impl, impl.SyncID, impl.SyncTime);
					}
					catch (Exception ex) { LogError(Operation.LogScope(obj), ex); }
				}))
				{
					transaction?.Complete();
				}
			}

			UpdateStatus(obj, operation);

			if (parentStatus?.LocalID != null) //Payment save updates the order, we need to change the saved timestamp.
			{
				parentStatus.LocalTS = BCSyncExactTimeAttribute.SelectDateTime<SOOrder.lastModifiedDateTime>(parentStatus.LocalID.Value);
				parentStatus = (BCSyncStatus)Statuses.Cache.Update(parentStatus);
			}
		}

		#endregion

		#region Export

		public override async Task<EntityStatus> GetBucketForExport(AmazonPaymentEntityBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			Payment impl = cbapi.GetByID<Payment>(syncstatus.LocalID);
			if (impl == null) return EntityStatus.None;

			MappedPayment obj = bucket.Payment = bucket.Payment.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(bucket.Payment, SyncDirection.Export);

			return status;
		}

		public override async Task FetchBucketsForExport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{

		}

		public override async Task SaveBucketExport(AmazonPaymentEntityBucket bucket, IMappedEntity existing, String operation, CancellationToken cancellationToken = default)
		{

		}

		#endregion

	}
}

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

using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Commerce.Shopify.API.REST;
using PX.Data;
using System;
using System.Collections.Generic;
using PX.Objects.SO;
using System.Linq;
using PX.Common;
using PX.Objects.AR;
using PX.Api.ContractBased.Models;
using PX.Objects.CA;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Commerce.Shopify.API.GraphQL;

namespace PX.Commerce.Shopify
{
	public class SPRefundsBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary { get => Refunds; }
		public IMappedEntity[] Entities => new IMappedEntity[] { Refunds };
		public override IMappedEntity[] PostProcessors { get => new IMappedEntity[] { Order }.Concat(Payments).ToArray(); }

		public MappedRefunds Refunds;
		public MappedOrder Order;
		public List<MappedPayment> Payments = new List<MappedPayment>();
	}

	public class SPRefundsRestrictor : BCBaseRestrictor, IRestrictor
	{
		public virtual FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return null;
		}

		public virtual FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return base.Restrict<MappedRefunds>(mapped, delegate (MappedRefunds obj)
			{
				if (obj.Extern != null)
				{
					//If no Transactions in refund, should skip this filter.
					if (obj.Extern.Refunds.Any(x => x.Transactions?.Any() == true) && !obj.Extern.Refunds.Any(x => x.Transactions.Any(a => (a.Kind == TransactionType.Refund || a.Kind == TransactionType.Void) && a.Status == TransactionStatus.Success)))
					{
						return new FilterResult(FilterStatus.Filtered,
							PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogRefundSkippedStatus, obj.Extern.Id));
					}

					var helper = ((SPRefundsProcessor)processor).GetHelper<SPHelper>();
					//If all transactions have kind refund and receipt action as void then with no  item refund then filter
					if (obj.Extern.Refunds.Any(x => x.Transactions?.Any() == true) &&
							obj.Extern.Refunds.All(x => x.Transactions.All(a => a.Kind == TransactionType.Refund && helper?.GetReceiptAction(a) == TransactionType.Void)
							&& !(x.RefundLineItems?.Any() ?? false)))
					{
						return new FilterResult(FilterStatus.Filtered,
							PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogRefundVoidSkippedStatus, obj.Extern.Id));
					}
				}
				var orderStatus = (BCSyncStatus)processor.SelectStatus(BCEntitiesAttribute.Order, obj.Extern.Id.ToString(), false);

				//If the Sales order sync record is not in Synchronized, Pending status, then the Refund Entity should not be processed as it depends on the Sales Order entity.
				bool shouldSkipIfOrderNotSync = orderStatus == null || orderStatus.LocalID == null ||
												(orderStatus.LocalID != null && orderStatus.Status != BCSyncStatusAttribute.Synchronized && orderStatus.Status != BCSyncStatusAttribute.Pending);
				if (shouldSkipIfOrderNotSync)
				{
					return new FilterResult(FilterStatus.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogRefundSkippedOrderNotSynced, obj.Extern.Id));
				}

				bool shouldFilterRefund = false;
				string filterRefundMessage = string.Empty;
				foreach (OrderTransaction transaction in obj.Extern?.Refunds?.SelectMany(r => r.Transactions ?? new List<OrderTransaction>()) ?? Enumerable.Empty<OrderTransaction>())
				{
					if (transaction.Status == TransactionStatus.Success && transaction.ParentId != null && !((SPRefundsProcessor)processor).IsPOSExchangePayment(obj.Extern, transaction.ParentId.ToString()))
					{
						var paymentStatus = (BCSyncStatus)processor.SelectStatus(BCEntitiesAttribute.Payment, new Object[] { obj.Extern.Id, transaction.ParentId }.KeyCombine(), false);
						//If the Payment sync record is not in Synchronized, Pending status, then the Refund Entity should not be processed as it depends on the Payment entity.
						if (paymentStatus != null && (paymentStatus.LocalID == null || (paymentStatus.LocalID != null && paymentStatus.Status != BCSyncStatusAttribute.Synchronized && paymentStatus.Status != BCSyncStatusAttribute.Pending)))
						{
							shouldFilterRefund = true;
							filterRefundMessage = PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.PaymentStatusNotValidForRefund, transaction.ParentId, BCSyncStatusAttribute.Convert(paymentStatus.Status));
						}
						else if (paymentStatus?.LocalID != null && (paymentStatus?.Status == BCSyncStatusAttribute.Synchronized || paymentStatus?.Status == BCSyncStatusAttribute.Pending))
						{
							//if there are multiple refunds in the order, and one of them is synced successfully, don't filter this refund out.
							shouldFilterRefund = false;
							break;
						}
					}
				}
				if (shouldFilterRefund)
				{
					return new FilterResult(FilterStatus.Invalid, filterRefundMessage);
				}

				return null;
			});
		}
	}

	[BCProcessor(typeof(SPConnector), BCEntitiesAttribute.OrderRefunds, BCCaptions.Refunds, 110,
		IsInternal = false,
		Direction = SyncDirection.Import,
		PrimaryDirection = SyncDirection.Import,
		PrimarySystem = PrimarySystem.Extern,
		PrimaryGraph = typeof(PX.Objects.SO.SOOrderEntry),
		ExternTypes = new Type[] { },
		LocalTypes = new Type[] { },
		AcumaticaPrimaryType = typeof(PX.Objects.SO.SOOrder),
		//AcumaticaPrimarySelect = typeof(Search<PX.Objects.SO.SOOrder.orderNbr>), //Entity Requires Parent Selection, which is not possible in Add/Edit Panel now.
		URL = "orders/{0}",
		Requires = new string[] { BCEntitiesAttribute.Order, BCEntitiesAttribute.Payment }
	)]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.OrderLine, EntityName = BCCaptions.OrderLine, AcumaticaType = typeof(PX.Objects.SO.SOLine))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.OrderAddress, EntityName = BCCaptions.OrderAddress, AcumaticaType = typeof(PX.Objects.SO.SOOrder))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.CustomerRefundOrder, EntityName = BCCaptions.CustomerRefundOrder, AcumaticaType = typeof(PX.Objects.SO.SOOrder))]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.Payment, EntityName = BCCaptions.Payment, AcumaticaType = typeof(PX.Objects.AR.ARPayment))]
	[BCProcessorRealtime(PushSupported = false, HookSupported = true,
		 WebHookType = typeof(WebHookMessage),
		WebHooks = new String[]
		{
			"refunds/create"
		})]
	public class SPRefundsProcessor : SPOrderBaseProcessor<SPRefundsProcessor, SPRefundsBucket, MappedRefunds>
	{
		[InjectDependency]
		protected ISPRestDataProviderFactory<IOrderRestDataProvider> orderDataProviderFactory { get; set; }
		[InjectDependency]
		protected ISPRestDataProviderFactory<IParentRestDataProvider<InventoryLocationData>> locationDataProviderFactory { get; set; }
		[InjectDependency]
		protected ISPGraphQLDataProviderFactory<OrderGQLDataProvider> orderDataGQLProviderFactory { get; set; }
		[InjectDependency]
		public ISPGraphQLAPIClientFactory shopifyGraphQLClientFactory { get; set; }
		[InjectDependency]
		public IShopifyRestClientFactory shopifyRestClientFactory { get; set; }

		protected IParentRestDataProvider<InventoryLocationData> inventoryLocationRestDataProvider;
		protected IOrderRestDataProvider orderDataProvider;
		protected IOrderGQLDataProvider orderDataGQLProvider;

		#region Initialization
		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation);

			var client = shopifyRestClientFactory.GetRestClient(GetBindingExt<BCBindingShopify>());
			orderDataProvider = orderDataProviderFactory.CreateInstance(client);
			inventoryLocationRestDataProvider = locationDataProviderFactory.CreateInstance(client);

			var graphQLClient = shopifyGraphQLClientFactory.GetClient(GetBindingExt<BCBindingShopify>());
			orderDataGQLProvider = orderDataGQLProviderFactory.GetProvider(graphQLClient);

			WrapSaveBucketInTransaction  = false;
		}
		#endregion

		#region Pull
		public override async Task<MappedRefunds> PullEntity(Guid? localID, Dictionary<string, object> fields, CancellationToken cancellationToken = default)
		{
			SalesOrder impl = cbapi.GetByID<SalesOrder>(localID);
			if (impl == null) return null;

			var orderStatus = this.SelectStatus(BCEntitiesAttribute.Order, impl.Id.ToString(), false);
			if (orderStatus == null) return null;

			MappedRefunds obj = new MappedRefunds(impl, impl.SyncID, impl.SyncTime).With(_ => { _.ParentID = orderStatus.SyncID; return _; });

			return obj;
		}
		public override async Task<MappedRefunds> PullEntity(string externID, string jsonObject, CancellationToken cancellationToken = default)
		{
			dynamic msg = JsonConvert.DeserializeObject(jsonObject);

			string orderId = (string)msg.order_id;
			if (orderId == null) return null;
			var orderData = await orderDataProvider.GetByID(orderId);
			if (orderData == null) return null;
			if (orderData.Refunds == null || orderData.Refunds.Count == 0) return null;
			var date = orderData.Refunds.FirstOrDefault(x => x.Id.ToString() == externID)?.DateCreatedAt.ToDate(false);
			if (date == null) return null;
			MappedRefunds obj = new MappedRefunds(orderData, orderData.Id.ToString(), orderData.OrderNumber, date);

			var orderStatus = this.SelectStatus(BCEntitiesAttribute.Order, orderData.Id.ToString(), false);
			obj.ParentID = orderStatus?.SyncID;

			return obj;
		}
		#endregion

		public override async Task<PullSimilarResult<MappedRefunds>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{
			string uniqueFieldvalue = ((OrderData)entity)?.Id?.ToString();
			if (string.IsNullOrEmpty(uniqueFieldvalue))
				return null;
			uniqueFieldvalue = APIHelper.ReferenceMake(uniqueFieldvalue, GetBinding().BindingName);
			List<MappedRefunds> result = new List<MappedRefunds>();
			List<string> orderTypes = new List<string>() { GetBindingExt<BCBindingExt>()?.OrderType };
			if (string.Equals(((OrderData)entity)?.SourceName, ShopifyConstants.POSSource, StringComparison.OrdinalIgnoreCase))
			{
				BCBindingShopify bidningShopify = GetBindingExt<BCBindingShopify>();
				//Support POS order type searching
				if (!string.IsNullOrEmpty(bidningShopify.POSDirectOrderType) && !orderTypes.Contains(bidningShopify.POSDirectOrderType))
					orderTypes.Add(bidningShopify.POSDirectOrderType);
				if (!string.IsNullOrEmpty(bidningShopify.POSShippingOrderType) && !orderTypes.Contains(bidningShopify.POSShippingOrderType))
					orderTypes.Add(bidningShopify.POSShippingOrderType);
			}
			GetHelper<SPHelper>().TryGetCustomOrderTypeMappings(ref orderTypes);

			foreach (SOOrder item in GetHelper<SPHelper>().OrderByTypesAndCustomerRefNbr.Select(orderTypes.ToArray(), uniqueFieldvalue))
			{
				SalesOrder data = new SalesOrder() { SyncID = item.NoteID, SyncTime = item.LastModifiedDateTime, ExternalRef = item.CustomerRefNbr?.ValueField() };
				result.Add(new MappedRefunds(data, data.SyncID, data.SyncTime));
			}
			return new PullSimilarResult<MappedRefunds>() { UniqueField = uniqueFieldvalue, Entities = result };
		}

		#region Export

		public override async Task FetchBucketsForExport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{

		}
		public override async Task<EntityStatus> GetBucketForExport(SPRefundsBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			SalesOrder impl = cbapi.GetByID<SalesOrder>(syncstatus.LocalID, GetCustomFieldsForExport());
			if (impl == null) return EntityStatus.None;

			bucket.Order = bucket.Order.Set(impl, impl.SyncID, impl.SyncTime);
			bucket.Refunds = bucket.Refunds.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(bucket.Refunds, SyncDirection.Export);


			return status;
		}

		public override async Task SaveBucketExport(SPRefundsBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
		}
		#endregion

		#region Import
		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			BCBindingExt currentBindingExt = GetBindingExt<BCBindingExt>();
			BCBindingShopify bidningShopify = GetBindingExt<BCBindingShopify>();
			var delaySecs = -bidningShopify.ApiDelaySeconds ?? 0;

			FilterOrders filter = new FilterOrders { Status = OrderStatus.Any };
			filter.Fields = BCRestHelper.PrepareFilterFields(typeof(OrderData), filters, "id", "name", "source_name", "financial_status", "updated_at", "created_at", "cancelled_at", "closed_at", "refunds");

			GetHelper<SPHelper>().SetFilterMinDate(filter, minDateTime, currentBindingExt.SyncOrdersFrom, delaySecs);
			if (maxDateTime != null) filter.UpdatedAtMax = maxDateTime.Value.ToLocalTime();
			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (OrderData orderData in  orderDataProvider.GetAll(filter, cancellationToken))
			{
				if (orderData.Refunds?.Any() != true) continue;

				SPRefundsBucket bucket = CreateBucket();
				var orderStatus = this.SelectStatus(BCEntitiesAttribute.Order, orderData.Id.ToString(), false);

				if (orderStatus == null) continue;
				var date = orderData.Refunds.Max(x => x.DateCreatedAt.ToDate(false));
				MappedRefunds obj = bucket.Refunds = bucket.Refunds.Set(orderData, orderData.Id.ToString(), orderData.OrderNumber, date).With(_ => { _.ParentID = orderStatus.SyncID; return _; });
				EntityStatus status = EnsureStatus(obj, SyncDirection.Import);

			}
		}
		public override async Task<EntityStatus> GetBucketForImport(SPRefundsBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			OrderData orderData = await orderDataProvider.GetByID(syncstatus.ExternID.KeySplit(0).ToString(), includedTransactions: true);
			if (orderData == null) return EntityStatus.None;
			EntityStatus status = EntityStatus.None;
			if (orderData.Refunds == null || orderData.Refunds.Count == 0) return status;
			var orderStatus = (BCSyncStatus)this.SelectStatus(BCEntitiesAttribute.Order, orderData.Id.ToString(), false);
			if (orderStatus == null) return status;

			if (orderStatus.LastOperation == BCSyncOperationAttribute.Skipped)
				throw new PXException(BCMessages.OrderStatusSkipped, orderData.Id);

			//Get the exchange data if it's POS order
			if (ShouldGetPOSExchangeData(orderData))
			{
				var exchangeOrders = await orderDataGQLProvider.GetOrderExchangesAsync(orderData.Id.ToString(), cancellationToken);
				orderData.ExchangeOrders = exchangeOrders?.ToList();
				if(orderData.ExchangeOrders != null && orderData.ExchangeOrders.Any( x => x.Transactions?.Count > 0))
				{
					var exchangeTrans = orderData.ExchangeOrders.SelectMany(x => x.Transactions).ToList();
					foreach(var trans in orderData.Transactions.Where(t => exchangeTrans.Any(e => e != null && e.Id.ConvertGidToId() == t.Id.ToString() && t.Kind != TransactionType.Refund)))
					{
						var lastKind = GetHelper<SPHelper>().PopulateAction(orderData.Transactions, trans);
						MappedPayment paymentObj = new MappedPayment(trans, new Object[] { orderData.Id, trans.Id }.KeyCombine(), orderData.Name ?? orderData.OrderNumber, trans.DateModifiedAt.ToDate(false), trans.CalculateHash())
							.With(_ => { _.ParentID = orderStatus.SyncID; return _; });
						PXResult<BCSyncStatus> pxresult = GetBCSyncStatusResult(paymentObj.EntityType, paymentObj.SyncID, paymentObj.LocalID, paymentObj.ExternID);
						var currentPaymentStatus = pxresult == null ? null : pxresult.GetItem<BCSyncStatus>();

						var hasBeenVoidedTransaction = orderData.Transactions.Any(x => x.ParentId == trans.Id && x.Status == TransactionStatus.Success && x.Kind == TransactionType.Void && x.Amount == trans.Amount);

						//If Payment has been voided and never synced before, then we  skip it.
						if (trans.Status == TransactionStatus.Success && (trans.Kind == TransactionType.Void || hasBeenVoidedTransaction) && currentPaymentStatus == null)
						{
							continue;
						}

						EntityStatus paymentStatus = EnsureStatus(paymentObj, SyncDirection.Import);

						trans.LastKind = lastKind;

						if (paymentStatus == EntityStatus.Pending)
						{
							bucket.Payments.Add(paymentObj);
						}
					}
				}
			}

			VerifyAuthToken(orderData);

			bucket.Order = bucket.Order.Set(orderData, orderData.Id?.ToString(), orderData.OrderNumber, orderData.DateModifiedAt.ToDate(false));

			var date = orderData.Refunds.Max(x => x.DateCreatedAt.ToDate(false));
			MappedRefunds obj = bucket.Refunds = bucket.Refunds.Set(orderData, orderData.Id.ToString(), orderData.OrderNumber, date);
			status = EnsureStatus(obj, SyncDirection.Import);

			BCExtensions.SetSharedSlot<OrderData>(this.GetType(), orderData.Id?.ToString(), orderData);
			return status;
		}

		/// <summary>
		/// Checks the <see cref="OrderTransaction.Authorization"/> field from order transactions and set values if null or empty.
		/// </summary>
		/// <param name="orderData"></param>
		protected virtual void VerifyAuthToken(OrderData orderData)
		{
			//This code only use for TikTok and shop_cash gateway, because there is no Authorization info in the respective payment gateway, it will cause the void paid action failed issue.
			//So, we manually add the TransactionId as Authorization code if Authorization is empty.
			List<string> gateways = new List<string>() { ShopifyConstants.TikTokPayments, ShopifyConstants.ShopCashPayments };

			foreach (var transaction in orderData?.Transactions ?? Enumerable.Empty<OrderTransaction>())
			{
				if (string.IsNullOrEmpty(transaction.Authorization) && gateways.Any(x => string.Equals(transaction.Gateway, x, StringComparison.OrdinalIgnoreCase)))
					transaction.Authorization = transaction.Id.ToString();
			}
		}

		public override async Task MapBucketImport(SPRefundsBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			MappedRefunds obj = bucket.Refunds;
			OrderData orderData = obj.Extern;
			MappedRefunds mappedRefunds = existing as MappedRefunds;
			if (mappedRefunds?.Local == null) throw new PXException(BCMessages.OrderNotSyncronized, orderData.Id);

			bucket.Refunds.Local = new SalesOrder();
			//Create refund payment in all refund scenarios if there is successful transaction in refund part
			CreateRefundPayment(bucket, mappedRefunds);

			//POS exchange order will be handled in RC order
			if (!IsPOSExchangeOrder(orderData) && mappedRefunds.Local.Status?.Value != PX.Objects.SO.Messages.Completed && mappedRefunds.Local.Status?.Value != PX.Objects.SO.Messages.Invoiced)
			{
				bucket.Refunds.Local.EditSO = true;
			}

			if (mappedRefunds.Local.Status?.Value == PX.Objects.SO.Messages.Completed || mappedRefunds.Local.Status?.Value == PX.Objects.SO.Messages.Invoiced || orderData.Refunds.Any(x => x.RefundLineItems?.Any(r => r.RestockType != RestockType.Cancel) == true))
			{
				await CreateRefundOrders(bucket, mappedRefunds, cancellationToken);
			}
		}

		public virtual async Task CreateRefundOrders(SPRefundsBucket bucket, MappedRefunds existing, CancellationToken cancellationToken = default)
		{
			SalesOrder origOrder = bucket.Refunds.Local;
			OrderData orderData = bucket.Refunds.Extern;
			List<OrderRefund> refunds = orderData.Refunds;
			origOrder.RefundOrders = new List<SalesOrder>();

			//Use for calculating how many RC order should be created by line item.
			//The key represents for line item ID, the 1st value represents for the total RC order items should be created, the 2nd one represents for the RC order items have been created,
			//the 3rd one represents for total return type items
			Dictionary<string, Tuple<int, int, int>> rcOrderPerItem = new Dictionary<string, Tuple<int, int, int>>();

			var operation = PXSelectJoin<SOOrderType, InnerJoin<SOOrderTypeOperation, On<SOOrderType.orderType, Equal<SOOrderTypeOperation.orderType>, And<SOOrderType.defaultOperation, Equal<SOOrderTypeOperation.operation>>>>,
			Where<SOOrderType.orderType, Equal<Required<SOOrderType.orderType>>>>.Select(this, GetBindingExt<BCBindingExt>().ReturnOrderType).AsEnumerable().
				Cast<PXResult<SOOrderType, SOOrderTypeOperation>>().FirstOrDefault();
			if (string.IsNullOrWhiteSpace(GetBindingExt<BCBindingExt>().ReasonCode) && operation.GetItem<SOOrderTypeOperation>()?.RequireReasonCode == true)
				throw new PXException(ShopifyMessages.ReasonCodeRequired);

			foreach (OrderRefund data in refunds)
			{
				//When the restock_type = no_restock or return, we should create the RC order
				//When Tansaction kind= Refund and Receipt action = void and refund is not on line item then skip refund order creation.
				if (data.RefundLineItems == null || (data.Transactions.All(a => a.Kind == TransactionType.Refund && helper?.GetReceiptAction(a) == TransactionType.Void)
							&& !(data.RefundLineItems?.Any() ?? false)) ||
					(existing.Local.Status?.Value != PX.Objects.SO.Messages.Completed && existing.Local.Status?.Value != PX.Objects.SO.Messages.Invoiced && data.RefundLineItems.All(x => x.RestockType == RestockType.Cancel))) continue;

				SalesOrder impl = new SalesOrder();
				impl.ExternalRef = APIHelper.ReferenceMake(data.Id, GetBinding().BindingName).ValueField();

				var presentCROrder = GetExistingCROrder(bucket, impl, data, cancellationToken);

				// check if refund is already imported as CRPayment
				if (existing != null && existing?.Details?.Any() == true)
				{
					if (existing.Details.Any(d => d.EntityType == BCEntitiesAttribute.Payment && d.ExternID.KeySplit(0) == data.Id.ToString()) && presentCROrder == null) continue;
				}

				var orderType = GetOrderType(orderData, isRefundOrder: true);
				var orderTypeClass = SOOrderType.PK.Find(this, orderType);
				OrderTypeValidation(orderTypeClass, orderData);
				//If order type is RC, positive qty means return item(Receipt), negative qty means additional item(Issue); This order type is using in the normal refund process, and Shipping POS exchange order.
				//If order type is MO, positive qty means additional item(Issue), negative qty means return item(Receipt), this type will be used in Counter POS Exchange order.
				//So if order type is MO, we should reverse the qty or amount
				decimal reverseQty = orderTypeClass.Behavior == SOBehavior.MO || orderTypeClass.Template == SOBehavior.MO ? -1 : 1;
				impl.Id = presentCROrder?.Id;
				impl.CanHaveRefundPayment = orderTypeClass.CanHaveRefunds;

				origOrder.RefundOrders.Add(impl);

				impl.RefundID = data.Id.ToString();
				impl.OrderType = (presentCROrder?.OrderType?.Value ?? orderType).ValueField();
				impl.Status = (presentCROrder?.Status?.Value ?? (orderTypeClass?.HoldEntry == true ? PX.Objects.SO.Messages.Hold : PX.Objects.SO.Messages.Open)).ValueField();
				impl.CustomerOrder = orderData.Name.ValueField();
				impl.FinancialSettings = new FinancialSettings();
				impl.FinancialSettings.Branch = existing.Local.FinancialSettings.Branch;

				var refundPayment = PXSelectJoin<BCPaymentMethods, InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<BCPaymentMethods.cashAccountID>>>,
					Where<BCPaymentMethods.bindingID, Equal<Required<BCPaymentMethods.bindingID>>, And<BCPaymentMethods.storePaymentMethod, Equal<Required<BCPaymentMethods.storePaymentMethod>>>>>.
					Select(this, GetBinding().BindingID, data.Transactions.FirstOrDefault(x => x.Status == TransactionStatus.Success)?.Gateway).AsEnumerable().
					Cast<PXResult<BCPaymentMethods, CashAccount>>().FirstOrDefault();
				if (refundPayment != null)
				{
					impl.PaymentMethod = refundPayment.GetItem<BCPaymentMethods>()?.PaymentMethodID.ValueField();
					impl.CashAccount = refundPayment.GetItem<CashAccount>()?.CashAccountCD.ValueField();
				}

				var date = data.DateCreatedAt.ToDate(false, PXTimeZoneInfo.FindSystemTimeZoneById(GetBindingExt<BCBindingExt>()?.OrderTimeZone));
				if (date.HasValue)
					impl.Date = (new DateTime(date.Value.Date.Ticks)).ValueField();
				impl.RequestedOn = impl.Date;
				impl.CustomerID = existing.Local.CustomerID;
				impl.CurrencyID = existing.Local.CurrencyID;
				impl.LocationID = existing.Local.LocationID;
				impl.ContactID = existing.Local.ContactID;
				var description = PXMessages.LocalizeFormat(ShopifyMessages.OrderDescription, GetBinding().BindingName, orderData.Name, orderData.FinancialStatus?.ToString());
				impl.Description = description.ValueField();
				impl.Details = new List<SalesOrderDetail>();
				impl.Totals = new Totals();
				impl.Totals.OverrideFreightAmount = existing.Local.Totals?.OverrideFreightAmount;

				decimal shippingrefundAmt = data.OrderAdjustments?.Where(x => x.Kind == OrderAdjustmentType.ShippingRefund)?.Sum(x => (-x.AmountPresentment) ?? 0m) ?? 0m;
				decimal shippingrefundAmtTax = data.OrderAdjustments?.Where(x => x.Kind == OrderAdjustmentType.ShippingRefund)?.Sum(x => (-x.TaxAmountPresentment) ?? 0m) ?? 0m;

				impl.ShipVia = existing.Local.ShipVia;
				impl.ShippingSettings = new ShippingSettings();
				impl.ShippingSettings.ShippingTerms = existing.Local.ShippingSettings?.ShippingTerms;
				impl.ShippingSettings.ShippingZone = existing.Local.ShippingSettings?.ShippingZone;
				if ((existing.Local.Totals?.Freight?.Value == null || existing.Local.Totals?.Freight?.Value == 0) && existing.Local.Totals?.PremiumFreight?.Value > 0)
				{
					impl.Totals.PremiumFreight = shippingrefundAmt.ValueField();
				}
				else
				{
					impl.Totals.Freight = shippingrefundAmt.ValueField();
				}

				#region OrderAdjustments

				AddOrderAdjustments(bucket, impl, data, existing, presentCROrder, reverseQty);

				#endregion

				#region ShipTo & BillTo Addresses

				MapShippingBillingAddress(bucket, impl, data, existing);

				#endregion

				#region SOLine

				decimal? totalDiscount = 0m;
				if (data.RefundLineItems?.Any() == true)
				{
					totalDiscount = AddSOLine(bucket, impl, data, existing, presentCROrder, reverseQty, ref rcOrderPerItem);
					AddExchangeItems(bucket, impl, data, existing, presentCROrder, reverseQty);
				}

				if (impl.Details.Any() == false && shippingrefundAmt == 0m)
				{
					//If Details is empty, that means RefundLineItems should be adjusted in original SO.
					//Should skip creating RC order without details.
					origOrder.RefundOrders.Remove(impl);
					continue;
				}
				#endregion

				#region Discounts

				MapDiscounts(bucket, impl, data, existing, presentCROrder, totalDiscount);

				#endregion

				#region Tax

				await MapTaxes(bucket, impl, data, existing, reverseQty);

				#endregion`

				#region CR Payment

				//The payment cannot create with MO/RC order together if there is an Issue operation in the order type(POS refund order has both Issue and Receipt operations), should be created and link to the MO order after MO order created
				//If order type doesn't allow refund payment, we cannot insert the refund with RC order
				if (impl.CanHaveRefundPayment == true && orderTypeClass.CanHavePayments != true && !IsPOSExchangeOrder(orderData))
					CreateCRPaymentWithOrder(bucket, impl, data, existing, refundPayment?.GetItem<BCPaymentMethods>()?.ReleasePayments ?? false);

				#endregion
			}
		}

		#region Methods for populating sub items to RC order

		/// <summary>
		/// Checks if refund is already imported as CR Order and returns first result or default.
		/// </summary>
		/// <param name="bucket"></param>
		/// <param name="newCROrder"></param>
		/// <param name="data">External Object</param>
		/// <param name="cancellationToken"></param>
		/// <returns>First Sales order or default.</returns>
		/// <exception cref="PXException">Throws if Refund Order is not found.</exception>
		public virtual SalesOrder GetExistingCROrder(SPRefundsBucket bucket, SalesOrder newCROrder, OrderRefund data, CancellationToken cancellationToken = default)
		{
			//Check if refund is already imported as CR Order
			var existingCR = cbapi.GetAll<SalesOrder>(new SalesOrder()
			{
				OrderType = GetOrderType(bucket.Order.Extern, isRefundOrder: true).SearchField(),
				ExternalRef = newCROrder.ExternalRef.Value.SearchField(),
				Details = new List<SalesOrderDetail>() { new SalesOrderDetail() { InventoryID = new StringReturn() } },
				DiscountDetails = new List<SalesOrdersDiscountDetails>() { new SalesOrdersDiscountDetails() { ExternalDiscountCode = new StringReturn() } }
			},
			filters: GetFilter(Operation.EntityType).LocalFiltersRows.Cast<PXFilterRow>(), cancellationToken: cancellationToken);
			if (existingCR?.Count() > 1)
			{
				throw new PXException(BCMessages.MultipleEntitiesWithUniqueField,
					PXMessages.LocalizeNoPrefix(BCCaptions.SyncDirectionImport),
					PXMessages.LocalizeNoPrefix(Connector.GetEntities().First(e => e.EntityType == Operation.EntityType).EntityName),
					data.Id.ToString());
			}

			return existingCR?.FirstOrDefault();
		}

		/// <summary>
		/// Validate the Operation in the orderType whether is active
		/// </summary>
		protected virtual void OrderTypeValidation(SOOrderType orderType, OrderData orderData)
		{
			if (orderType?.Template == BCBindingExt.orderRCType.OrderRCType && IsPOSOrder(orderData))
			{
				//If there is an exchange order and additonal item, we will use the "Issue" operation in the refund order type
				//If Issue operation is inactive, it will cause an error when we create an Issue item later, so we need to check before we import it.
				if (orderData.ExchangeOrders?.Any(x => x.Additions != null) == true)
				{
					foreach (SOOrderTypeOperation operation in PXSelect<SOOrderTypeOperation, Where<SOOrderTypeOperation.orderType, Equal<Required<SOOrderTypeOperation.orderType>>>>.Select(this, orderType.OrderType))
					{
						if (operation.Operation == SOOperation.Issue && operation.Active != true)
						{
							throw new PXSetPropertyException<BCBindingShopify.pOSShippingExchangeOrderType>(BCMessages.OperationInactiveInRefundOrderType, orderType.OrderType);
						}
					}
				}
			}
		}

		public virtual void MapShippingBillingAddress(SPRefundsBucket bucket, SalesOrder newCROrder, OrderRefund data, MappedRefunds existing)
		{
			newCROrder.BillToAddressOverride = existing.Local.BillToAddressOverride;
			newCROrder.BillToAddress = new Core.API.Address();
			newCROrder.BillToAddress.AddressLine1 = existing.Local.BillToAddress.AddressLine1;
			newCROrder.BillToAddress.AddressLine2 = existing.Local.BillToAddress.AddressLine2;
			newCROrder.BillToAddress.City = existing.Local.BillToAddress.City;
			newCROrder.BillToAddress.Country = existing.Local.BillToAddress.Country;
			newCROrder.BillToAddress.PostalCode = existing.Local.BillToAddress.PostalCode;
			newCROrder.BillToAddress.State = existing.Local.BillToAddress.State;

			newCROrder.BillToContactOverride = existing.Local.BillToContactOverride;
			newCROrder.BillToContact = new Core.API.DocContact();
			newCROrder.BillToContact.Attention = existing.Local.BillToContact.Attention;
			newCROrder.BillToContact.BusinessName = existing.Local.BillToContact.BusinessName;
			newCROrder.BillToContact.Email = existing.Local.BillToContact.Email;
			newCROrder.BillToContact.Phone1 = existing.Local.BillToContact.Phone1;

			newCROrder.ShipToAddressOverride = existing.Local.ShipToAddressOverride;
			newCROrder.ShipToAddress = new Core.API.Address();
			newCROrder.ShipToAddress.AddressLine1 = existing.Local.ShipToAddress.AddressLine1;
			newCROrder.ShipToAddress.AddressLine2 = existing.Local.ShipToAddress.AddressLine2;
			newCROrder.ShipToAddress.City = existing.Local.ShipToAddress.City;
			newCROrder.ShipToAddress.Country = existing.Local.ShipToAddress.Country;
			newCROrder.ShipToAddress.PostalCode = existing.Local.ShipToAddress.PostalCode;
			newCROrder.ShipToAddress.State = existing.Local.ShipToAddress.State;

			newCROrder.ShipToContactOverride = existing.Local.ShipToContactOverride;
			newCROrder.ShipToContact = new Core.API.DocContact();
			newCROrder.ShipToContact.Attention = existing.Local.ShipToContact.Attention;
			newCROrder.ShipToContact.BusinessName = existing.Local.ShipToContact.BusinessName;
			newCROrder.ShipToContact.Email = existing.Local.ShipToContact.Email;
			newCROrder.ShipToContact.Phone1 = existing.Local.ShipToContact.Phone1;
		}

		private bool HasTaxes(OrderRefund orderRefund, OrderData externOrder)
		{
			bool isRefundHasTaxes =
				orderRefund.RefundLineItems != null
				&& orderRefund.RefundLineItems.Any(refundLineItem =>
					refundLineItem.TotalTaxPresentment.HasValue
					&& refundLineItem.TotalTaxPresentment > decimal.Zero);

			bool isShippingFeeHasTaxes =
				orderRefund.OrderAdjustments != null
				&& orderRefund.OrderAdjustments.Any(orderAdjustment =>
					orderAdjustment.Kind == OrderAdjustmentType.ShippingRefund
					&& orderAdjustment.TaxAmountPresentment.HasValue
					&& orderAdjustment.TaxAmountPresentment > decimal.Zero);

			bool isPOSExchangeNonTaxableItemToTaxable = IsPOSExchangeOrder(externOrder)
				&& orderRefund.RefundLineItems.All(refundItem => refundItem.TotalTaxPresentment == decimal.Zero)
				&& externOrder.TotalTaxPresentment > decimal.Zero;

			return isRefundHasTaxes || isShippingFeeHasTaxes || isPOSExchangeNonTaxableItemToTaxable;
		}

		public virtual async Task MapTaxes(SPRefundsBucket bucket, SalesOrder newCROrder, OrderRefund data, MappedRefunds existing)
		{
			decimal reverseQty = 1;
			await MapTaxes(bucket, newCROrder, data, existing, reverseQty);
		}

		public virtual async Task MapTaxes(SPRefundsBucket bucket, SalesOrder newCROrder, OrderRefund data, MappedRefunds existing, decimal reverseQty)
		{
			OrderData externOrderData = bucket.Order.Extern;

			if (!this.HasTaxes(data, externOrderData))
				return;

			var salesOrderDetails = PXSelect<BCSyncDetail, Where<BCSyncDetail.syncID, Equal<Required<BCSyncDetail.syncID>>,
				And<BCSyncDetail.entityType, Equal<Required<BCSyncDetail.entityType>>>>>.Select(this, bucket.Refunds.ParentID, BCEntitiesAttribute.TaxSynchronization);
			if (salesOrderDetails.Any()
					&& salesOrderDetails.FirstOrDefault()?.GetItem<BCSyncDetail>().ExternID == BCObjectsConstants.BCSyncDetailTaxSynced)
			{
				var taxes = PXSelect<SOTaxTran, Where<SOTaxTran.orderType, Equal<Required<SOTaxTran.orderType>>,
				And<SOTaxTran.orderNbr, Equal<Required<SOTaxTran.orderNbr>>>>>.Select(this, existing.Local.OrderType.Value, existing.Local.OrderNbr.Value).RowCast<SOTaxTran>();
				if (taxes?.Any() == true)
				{
					newCROrder.TaxDetails = new List<TaxDetail>();
					if (bucket.Refunds.Extern.TaxLines?.Count > 0)
					{
						newCROrder.IsTaxValid = true.ValueField();
						string taxType = GetHelper<SPHelper>().DetermineTaxType(bucket.Refunds.Extern.TaxLines.Select(i => i.TaxName).ToList());
						decimal shippingrefundAmt = data.OrderAdjustments?.Where(x => x.Kind == OrderAdjustmentType.ShippingRefund)?.Sum(x => (-x.AmountPresentment) ?? 0m) ?? 0m;
						decimal shippingrefundAmtTax = data.OrderAdjustments?.Where(x => x.Kind == OrderAdjustmentType.ShippingRefund)?.Sum(x => (-x.TaxAmountPresentment) ?? 0m) ?? 0m;

						var bindingExt = GetBindingExt<BCBindingExt>();

						//For new exchange items
						IEnumerable<OrderLineItem> lineItemsWithTax = Enumerable.Empty<OrderLineItem>();
						HashSet<string> exchangeAdditionLineItemIds = new();
						HashSet<string> exchangeReturnLineItemIds = new();
						if (IsPOSExchangeOrder(externOrderData))
						{
							lineItemsWithTax = externOrderData.LineItems.Where(x => IsRefundWithPOSExchangeNewItem(externOrderData, data.Id.ToString(), x.Id?.ToString()) && x.TaxLines?.Count > 0);
							exchangeAdditionLineItemIds = externOrderData.ExchangeOrders
								.SelectMany(exchange => exchange.Additions.LineItems)
								.Select(lineItem => lineItem.LineItem.Id)
								.ToHashSet();
							exchangeReturnLineItemIds = externOrderData.ExchangeOrders
								.SelectMany(exchange => exchange.Returns.LineItems)
								.Select(lineItem => lineItem.LineItem.Id)
								.ToHashSet();
						}

						HashSet<string> fulfilledExternalRefundLineItemIds = newCROrder.Details
							.Select(detail => detail.ExternalRef.Value)
							.ToHashSet();

						foreach (var tax in bucket.Refunds.Extern.TaxLines)
						{
							string countryCode = externOrderData.ShippingAddress?.CountryCode ?? externOrderData.BillingAddress?.CountryCode;
							string provinceCode = externOrderData.ShippingAddress?.ProvinceCode ?? externOrderData.BillingAddress?.ProvinceCode;

							if (countryCode == null && provinceCode == null)
							{
								if (externOrderData.LocationId != null)
								{
									var orderLocation = await inventoryLocationRestDataProvider.GetByID(externOrderData.LocationId.ToString());
									countryCode = orderLocation?.CountryCode;
									provinceCode = orderLocation?.ProvinceCode;
								}
								else
								{
									var firstLineItem = externOrderData.LineItems?.FirstOrDefault();
									countryCode = firstLineItem?.OriginLocation?.CountryCode;
									provinceCode = firstLineItem?.OriginLocation?.ProvinceCode;
								}
							}
							string mappedTaxName = GetHelper<SPHelper>().SubstituteTaxName(bindingExt, tax.TaxName, countryCode, provinceCode);

							decimal? taxable = decimal.Zero;
							decimal? taxAmount = decimal.Zero;

							if (lineItemsWithTax?.Any() == true)
							{
								var lineItemsWithCurrentTax = lineItemsWithTax.Where(x => x.TaxLines.Any(t => t.TaxAmountPresentment > 0m && t.TaxName == tax.TaxName && t.TaxRate == tax.TaxRate));
								taxable += (-lineItemsWithCurrentTax.Sum(x => (x?.PricePresentment * x?.Quantity) ?? 0m)
											- lineItemsWithCurrentTax.SelectMany(x => x.DiscountAllocations)?.Sum(x => x.DiscountAmountPresentment ?? 0m)) * reverseQty;
								taxAmount += (-lineItemsWithCurrentTax.SelectMany(x => x.TaxLines)
																		.Where(t => t.TaxName == tax.TaxName && t.TaxRate == tax.TaxRate).Sum(x => x.TaxAmountPresentment ?? 0m)) * reverseQty;
							}

							if (tax.TaxRate != decimal.Zero)
							{
								var fulfilledRefundLineItems = data.RefundLineItems.Where(lineItem =>
									lineItem.RestockType != RestockType.Cancel
									&& fulfilledExternalRefundLineItemIds.Contains(lineItem.Id.ToString()));

								var refundItemsWithTaxes = fulfilledRefundLineItems.Where(lineItem =>
									lineItem.TotalTaxPresentment != decimal.Zero
									&& lineItem.OrderLineItem.TaxLines != null
									&& lineItem.OrderLineItem.TaxLines.Any(taxLine => taxLine.TaxAmountPresentment > decimal.Zero && taxLine.TaxName == tax.TaxName));

								//
								// We have to go through all refund line items and already created refund order lines
								// to verify which particular line item was fulfilled or not in case of non_stock refund.
								// This is mainly in order to cover the case when a refund contains more than one fulfilled item with no_restock.
								//
								bool isPOSExchangeMixedOrder = reverseQty == -1;
								HashSet<SalesOrderDetail> processedOrderDetails = new(newCROrder.Details.Count);
								foreach (RefundLineItem refundLineItem in refundItemsWithTaxes)
								{
									string refundLineItemId = refundLineItem.LineItemId.ToString();
									foreach (SalesOrderDetail salesOrderDetail in newCROrder.Details)
									{
										if (!refundLineItem.OrderLineItem.Sku.Equals(salesOrderDetail.InventoryID.Value, StringComparison.OrdinalIgnoreCase)
											|| processedOrderDetails.Contains(salesOrderDetail))
										{
											continue;
										}

										if (this.IsPOSExchangeOrder(externOrderData))
										{
											bool isDetailBelongToPOSExchange = salesOrderDetail.OrderQty.Value.Value < decimal.Zero;

											if (this.IsDirectPosOrder(externOrderData))
											{
												if (isPOSExchangeMixedOrder && !isDetailBelongToPOSExchange)
													continue;
											}
											else
											{
												if (isDetailBelongToPOSExchange)
													continue;
											}

											bool isExchangeToItemHasOrderTax = lineItemsWithTax
												.SelectMany(lineItemWithTax => lineItemWithTax.TaxLines)
												.Any(taxLine => taxLine.TaxName == tax.TaxName);

											bool isReturnWithoutExchange = !exchangeAdditionLineItemIds.Contains(refundLineItemId)
																		&& !exchangeReturnLineItemIds.Contains(refundLineItemId);

											if (!isExchangeToItemHasOrderTax && !isReturnWithoutExchange)
												continue;
										}

										taxable += salesOrderDetail.UnitPrice.Value.Value * salesOrderDetail.OrderQty.Value.Value;
										processedOrderDetails.Add(salesOrderDetail);
									}
								}
								decimal? discountsAmount = newCROrder.DiscountDetails?.Sum(discount => discount.DiscountAmount.Value) ?? decimal.Zero;
								taxable = await helper.RoundToCurrencyPrecision(taxable - discountsAmount, newCROrder.CurrencyID.Value);
								taxAmount = await helper.RoundToCurrencyPrecision(taxable * tax.TaxRate, newCROrder.CurrencyID.Value);

								if (bucket.Refunds.Extern.ShippingLines.Any(x => x.TaxLines?.Count > 0 && x.TaxLines.Any(t => t.TaxAmountPresentment > decimal.Zero && t.TaxName == tax.TaxName)))
								{
									taxAmount += (await helper.RoundToCurrencyPrecision(shippingrefundAmt * tax.TaxRate, newCROrder.CurrencyID.Value)) * reverseQty;
									taxable += shippingrefundAmt * reverseQty;
								}
							}

							if (taxAmount == decimal.Zero || taxable == decimal.Zero)
								continue;

							TaxDetail inserted = newCROrder.TaxDetails.FirstOrDefault(i => i.TaxID.Value?.Equals(mappedTaxName, StringComparison.OrdinalIgnoreCase) == true
																							&& i.TaxRate.Value == (tax.TaxRate * 100));
							if (inserted == null)
							{
								if (String.IsNullOrEmpty(mappedTaxName)) throw new PXException(PX.Commerce.Objects.BCObjectsMessages.TaxNameDoesntExist);

								newCROrder.TaxDetails.Add(new TaxDetail()
								{
									TaxID = mappedTaxName.ValueField(),
									TaxAmount = taxAmount != null ? taxAmount.ValueField() : decimal.Zero.ValueField(),
									TaxRate = (tax.TaxRate * 100).ValueField(),
									TaxableAmount = (taxable).ValueField()
								});
							}
							else
							{
								if (inserted.TaxAmount != null)
								{
									inserted.TaxAmount.Value += taxAmount;
								}
							}
						}
					}
				}
			}

			if (newCROrder.TaxDetails?.Count > 0)
			{
				newCROrder.FinancialSettings.OverrideTaxZone = existing.Local.FinancialSettings.OverrideTaxZone;
				newCROrder.FinancialSettings.CustomerTaxZone = existing.Local.FinancialSettings.CustomerTaxZone;
			}
			//Calculate tax mode
			newCROrder.TaxCalcMode = existing.Local.TaxCalcMode;

			String[] tooLongTaxIDs = ((newCROrder.TaxDetails ?? new List<TaxDetail>()).Select(x => x.TaxID?.Value).Where(x => (x?.Length ?? 0) > PX.Objects.TX.Tax.taxID.Length).ToArray());
			if (tooLongTaxIDs != null && tooLongTaxIDs.Length > 0)
			{
				throw new PXException(PX.Commerce.Objects.BCObjectsMessages.CannotFindSaveTaxIDs, String.Join(",", tooLongTaxIDs), PX.Objects.TX.Tax.taxID.Length);
			}
		}

		public virtual void AddExchangeItems(SPRefundsBucket bucket, SalesOrder newCROrder, OrderRefund data, MappedRefunds existing, SalesOrder presentCROrder, decimal reverseQty)
		{
			//Exchange only uses in POS order.
			if (!IsPOSOrder(bucket.Order.Extern)) return;

			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			var exchangeAdditions = bucket.Order.Extern.ExchangeOrders?.Where(x => x?.Refunds?.Any(r => r.Id.ConvertGidToId() == data.Id.ToString()) == true)?.Select(eo => eo?.Additions).ToList();
			if(exchangeAdditions?.Count <= 0) return;

			foreach (var item in exchangeAdditions?.SelectMany(x => x.LineItems)?.ToList() ?? new List<OrderExchangeLineItemGQL>())
			{
				var orderLineItem = bucket.Order.Extern.LineItems.FirstOrDefault(x => x.Id.ToString() == item.LineItem?.Id.ConvertGidToId());
				if (orderLineItem == null) continue;

				SalesOrderDetail detail = new SalesOrderDetail();
				String inventoryCD = GetHelper<SPHelper>().GetInventoryCDByExternID(
					orderLineItem.ProductId?.ToString(),
					orderLineItem.VariantId.ToString(),
					orderLineItem.Sku,
					orderLineItem.Name,
					orderLineItem.IsGiftCard,
					out string uom,
					out string alternateID,
					out string itemStatus);

				detail.Branch = existing.Local.FinancialSettings.Branch;
				detail.InventoryID = inventoryCD?.TrimEnd().ValueField();
				detail.OrderQty = ((-(decimal)item.Quantity) * reverseQty).ValueField();
				detail.UOM = uom.ValueField();
				detail.UnitPrice = orderLineItem.PricePresentment.ValueField();
				detail.ManualPrice = true.ValueField();
				detail.ReasonCode = bindingExt.ReasonCode?.ValueField();
				detail.ExternalRef = orderLineItem.Id.ToString().ValueField();
				detail.AlternateID = alternateID?.ValueField();

				(string WarehouseCD, string LocationCD) mappedWarehouseLocation = GetWarehouseLocationMappingForImport(orderLineItem.OriginLocation?.LocationId?.ToString(),
					bucket.Order.Extern.LocationId?.ToString());
				detail.WarehouseID = mappedWarehouseLocation.WarehouseCD?.ValueField();
				detail.Location = mappedWarehouseLocation.LocationCD?.ValueField();

				DetailInfo matchedDetail = existing?.Details?.FirstOrDefault(d => d.EntityType == BCEntitiesAttribute.OrderLine && orderLineItem.Id.ToString() == d.ExternID.KeySplit(1) && data.Id.ToString() == d.ExternID.KeySplit(0));
				if (matchedDetail != null) detail.Id = matchedDetail.LocalID; //Search by Details
				else if (presentCROrder?.Details != null && presentCROrder.Details.Count > 0) //Serach by Existing line
				{
					SalesOrderDetail matchedLine = presentCROrder.Details.FirstOrDefault(x => x.ExternalRef?.Value != null && x.ExternalRef?.Value == orderLineItem.Id.ToString());
					if (matchedLine != null) detail.Id = matchedLine.Id;
				}

				if (orderLineItem.DiscountAllocations?.Count > 0)
				{
					var itemDiscount = orderLineItem.DiscountAllocations.Sum(x => x.DiscountAmountPresentment ?? 0m);
					detail.DiscountAmount = itemDiscount.ValueField();
				}

				newCROrder.Details.Add(detail);
			}
		}

		public virtual decimal? AddSOLine(SPRefundsBucket bucket, SalesOrder newCROrder, OrderRefund data, MappedRefunds existing, SalesOrder presentCROrder, decimal reverseQty, ref Dictionary<string, Tuple<int, int, int>> rcOrderPerItem)
		{
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			decimal? totalDiscount = 0m;
			//Get the SOShipLine data in the original order
			var shipLines = PXSelect<SOShipLine, Where<SOShipLine.origOrderType, Equal<Required<SOShipLine.origOrderType>>, And<SOShipLine.origOrderNbr, Equal<Required<SOShipLine.origOrderNbr>>>>>
								.Select(this, existing.Local?.OrderType?.Value, existing.Local?.OrderNbr?.Value).RowCast<SOShipLine>()?.ToList();

			foreach (var item in data.RefundLineItems)
			{
				//Only add items with no_restock/return type to the CR order if order status is not Completed, the cancel items can modify in order directly in this case
				if (item.RestockType == RestockType.Cancel && existing.Local.Status?.Value != PX.Objects.SO.Messages.Completed && existing.Local.Status?.Value != PX.Objects.SO.Messages.Invoiced)
					continue;

				SalesOrderDetail detail = new SalesOrderDetail();
				String inventoryCD = GetHelper<SPHelper>().GetInventoryCDByExternID(
					item.OrderLineItem.ProductId?.ToString(),
					item.OrderLineItem.VariantId.ToString(),
					item.OrderLineItem.Sku,
					item.OrderLineItem.Name,
					item.OrderLineItem.IsGiftCard,
					out string uom,
					out string alternateID,
					out string itemStatus);

				var shipline = shipLines?.FirstOrDefault(x => x.OrigLineNbr == existing.Local.Details.FirstOrDefault(y => y.NoteID.Value == bucket.Order.Details.FirstOrDefault(
								z => z.EntityType == BCEntitiesAttribute.OrderLine && z.ExternID == item.OrderLineItem.Id.ToString())?.LocalID)?.LineNbr?.Value);
				if (shipline == null)
				{
					PX.Objects.IN.InventoryItem inventory = PXSelectReadonly<PX.Objects.IN.InventoryItem,
						  Where<PX.Objects.IN.InventoryItem.inventoryCD, Equal<Required<PX.Objects.IN.InventoryItem.inventoryCD>>>>.Select(this, inventoryCD);
					shipline = shipLines?.FirstOrDefault(x => x.InventoryID == inventory.InventoryID);
				}
				if (shipline != null)
				{
					//should match the Lot serial Nbr and/or Location in original order
					detail.LotSerialNbr = shipline?.LotSerialNbr.ValueField();
					detail.Location = PX.Objects.IN.INLocation.PK.Find(this, shipline?.LocationID)?.LocationCD?.Trim().ValueField();
				}
				detail.Branch = existing.Local.FinancialSettings.Branch;
				detail.InventoryID = inventoryCD?.TrimEnd().ValueField();
				detail.OrderQty = ((decimal)item.Quantity * reverseQty).ValueField();
				detail.UOM = uom.ValueField();
				detail.UnitPrice = item.OrderLineItem.PricePresentment.ValueField();
				detail.ManualPrice = true.ValueField();
				detail.ReasonCode = bindingExt.ReasonCode?.ValueField();
				detail.ExternalRef = item.Id.ToString().ValueField();
				detail.AlternateID = alternateID?.ValueField();
				//get warehouse from original SO Line
				detail.WarehouseID = existing.Local.Details.FirstOrDefault(x => x.ExternalRef.Value == item.OrderLineItem.Id.ToString())?.WarehouseID;
				DetailInfo matchedDetail = existing?.Details?.FirstOrDefault(d => d.EntityType == BCEntitiesAttribute.OrderLine && item.Id.ToString() == d.ExternID.KeySplit(1) && data.Id.ToString() == d.ExternID.KeySplit(0));
				if (matchedDetail != null) detail.Id = matchedDetail.LocalID; //Search by Details
				else if (presentCROrder?.Details != null && presentCROrder.Details.Count > 0) //Serach by Existing line
				{
					SalesOrderDetail matchedLine = presentCROrder.Details.FirstOrDefault(x =>
						(x.ExternalRef?.Value != null && x.ExternalRef?.Value == item.Id.ToString())
						||
						(x.InventoryID?.Value == detail.InventoryID?.Value && (detail.UOM == null || detail.UOM.Value == x.UOM?.Value)));
					if (matchedLine != null) detail.Id = matchedLine.Id;
				}

				var allowedRefundItemQty = GetAllowedRefundItemQty(bucket, item, ref rcOrderPerItem);
				if (allowedRefundItemQty == 0)
					continue;
				else
					detail.OrderQty = ((decimal)allowedRefundItemQty * reverseQty).ValueField();

				if (item.OrderLineItem.DiscountAllocations?.Count > 0)
				{
					decimal? itemDiscount = CalculateRefundItemDiscount(item);
					totalDiscount += itemDiscount;
					if (bindingExt?.PostDiscounts == BCPostDiscountAttribute.LineDiscount)
					{
						detail.DiscountAmount = itemDiscount.ValueField();
					}
					else
					{
						detail.DiscountAmount = 0m.ValueField();
					}

				}

				newCROrder.Details.Add(detail);
			}

			return totalDiscount;
		}

		public virtual void AddOrderAdjustments(SPRefundsBucket bucket, SalesOrder newCROrder, OrderRefund data, MappedRefunds existing, SalesOrder presentCROrder, decimal reverseQty)
		{
			decimal totalOrderRefundAmout = 0m;
			//Skip order adjustment, the Order Adjustment value should be applied directly to the Original Sales Order if order is not completed or cancelled.
			//If order adjustment comes from the fulfilled items only, should import these adjustment in customer refund payment
			if (existing.Local.Status?.Value == PX.Objects.SO.Messages.Completed || existing.Local.Status?.Value == PX.Objects.SO.Messages.Cancelled ||
				existing.Local.Status?.Value == PX.Objects.SO.Messages.Invoiced ||
				(data.RefundLineItems != null && data.RefundLineItems.All(x => x.RestockType != RestockType.Cancel)))
			{
				totalOrderRefundAmout = data.OrderAdjustments?.Where(x => x.Kind == OrderAdjustmentType.RefundDiscrepancy)?.Sum(y => (y.AmountPresentment ?? 0m)) ?? 0m;
				SalesOrderDetail refundLineItem = null;
				//if there are mulitple order adjustments and some of them adjusted after order completed in AC, we should reduce the order adjustment that already applied to order itself.
				if (existing.Local.Details.Any() && refundItem != null && (refundLineItem = existing.Local.Details.FirstOrDefault(x => string.Equals(x.ExternalRef.Value, data.Id.ToString()) &&
					string.Equals(x.InventoryID.Value, refundItem.InventoryCD?.TrimEnd(), StringComparison.OrdinalIgnoreCase))) != null)
				{
					totalOrderRefundAmout -= refundLineItem.UnitPrice.Value ?? 0m;
				}
			}

			//Add orderAdjustments
			//If there is no Transactions in the refund, we should ignore the order adjustments
			if (totalOrderRefundAmout != 0m && data.Transactions?.Any() == true)
			{
				//When Order Adjustments are imported into RC order in ERP, then the symbol (+, -) from Shopify API should be opposite with RC Order Line in ERP.
				var detail = InsertRefundAmountItem(-totalOrderRefundAmout, existing.Local.FinancialSettings.Branch);
				detail.ExternalRef = data.Id?.ToString().ValueField();

				if (presentCROrder?.Details != null)
					presentCROrder?.Details.FirstOrDefault(x => string.Equals(x.ExternalRef.Value, data.Id.ToString()) && string.Equals(x.InventoryID.Value, detail.InventoryID.Value, StringComparison.OrdinalIgnoreCase))
						.With(e => detail.Id = e.Id);
				detail.OrderQty = (detail.OrderQty.Value * reverseQty).ValueField();
				newCROrder.Details.Add(detail);
			}
		}

		public virtual void MapDiscounts(SPRefundsBucket bucket, SalesOrder newCROrder, OrderRefund data, MappedRefunds existing, SalesOrder presentCROrder, decimal? totalDiscount)
		{
			if (GetBindingExt<BCBindingExt>()?.PostDiscounts == BCPostDiscountAttribute.DocumentDiscount && totalDiscount > 0)
			{
				newCROrder.DisableAutomaticDiscountUpdate = true.ValueField();
				newCROrder.DiscountDetails = new List<SalesOrdersDiscountDetails>();

				SalesOrdersDiscountDetails discountDetail = new SalesOrdersDiscountDetails();
				discountDetail.Type = PX.Objects.Common.Discount.DiscountType.ExternalDocument.ValueField();
				discountDetail.DiscountAmount = totalDiscount.ValueField();
				discountDetail.Description = ShopifyMessages.RefundDiscount.ValueField();
				discountDetail.ExternalDiscountCode = ShopifyMessages.RefundDiscount.ValueField();
				newCROrder.DiscountDetails.Add(discountDetail);
				if (presentCROrder != null)
				{
					presentCROrder.DiscountDetails?.ForEach(e => newCROrder.DiscountDetails?.FirstOrDefault(n => n.ExternalDiscountCode.Value == e.ExternalDiscountCode.Value).With(n => n.Id = e.Id));
					newCROrder.DiscountDetails?.AddRange(presentCROrder.DiscountDetails == null ? Enumerable.Empty<SalesOrdersDiscountDetails>()
						: presentCROrder.DiscountDetails.Where(e => newCROrder.DiscountDetails == null || !newCROrder.DiscountDetails.Any(n => e.Id == n.Id)).Select(n => new SalesOrdersDiscountDetails() { Id = n.Id, Delete = true }));
				}
			}
		}

		public virtual void CreateCRPaymentWithOrder(SPRefundsBucket bucket, SalesOrder newCROrder, OrderRefund data, MappedRefunds existing, bool needRelease = false)
		{
			SalesOrder origOrder = bucket.Refunds.Local;
			newCROrder.Payments = new List<SalesOrderPayment>();
			var payments = origOrder.Payment?.Where(x => x.CreateWithRC == true && x.TransactionID.KeySplit(0) == data.Id.ToString())?.ToList();

			//if any restock type is NoRestock, if item has reduced in original SO, the RC order may not equal to transaction amount, should create Payment separately and apply to both RC order and Payment.
			bool allowCRPaymentWithOrder = payments?.Any() == true && ((data.RefundLineItems.Any() == true && data.RefundLineItems.Any(x => x.RestockType == RestockType.NoRestock) == false)
				|| (data.RefundLineItems?.Any() != true && data.OrderAdjustments?.Any() == true && data.OrderAdjustments.All(adj => adj.Kind == OrderAdjustmentType.ShippingRefund))
				|| existing?.Local?.Status?.Value == PX.Objects.SO.Messages.Completed || existing.Local.Status?.Value != PX.Objects.SO.Messages.Invoiced);
			if (allowCRPaymentWithOrder)
			{
				foreach (var transaction in payments)
				{
					SalesOrderPayment payment = new SalesOrderPayment();
					payment.DocType = PX.Objects.AR.Messages.Refund.ValueField();
					payment.ExternalRef = transaction.ExternalRef;
					payment.PaymentRef = transaction.PaymentRef;
					payment.ApplicationDate = transaction.ApplicationDate;
					payment.Description = transaction.Description;
					payment.PaymentAmount = transaction.PaymentAmount;
					payment.Hold = transaction.Hold ?? false.ValueField();
					payment.Refund = false.ValueField();
					payment.ValidateCCRefundOrigTransaction = false.ValueField();
					payment.AppliedToOrder = transaction.PaymentAmount;
					payment.OrigTransactionNbr = transaction.OrigTransaction;
					payment.ProcessingCenterID = transaction.ProcessingCenterID;
					if (transaction.CreditCardTransactionInfo?.Count > 0)
					{
						payment.CreditCardTransactionInfo = new List<SalesOrderCreditCardTransactionDetail>();
						foreach (var detail in transaction.CreditCardTransactionInfo)
						{
							SalesOrderCreditCardTransactionDetail creditCardDetail = new SalesOrderCreditCardTransactionDetail();
							creditCardDetail.TranNbr = detail.TranNbr;
							creditCardDetail.TranDate = detail.TranDate;
							creditCardDetail.TranType = detail.TranType;
							creditCardDetail.ExtProfileId = detail.ExtProfileId;
							payment.CreditCardTransactionInfo.Add(creditCardDetail);
						}
					}
					payment.Currency = transaction.CurrencyID;
					payment.CashAccount = transaction.CashAccount;
					payment.PaymentMethod = transaction.PaymentMethod;
					payment.NoteID = transaction.NoteID;
                    payment.NeedRelease = needRelease;
                    newCROrder.Payments.Add(payment);
					origOrder.Payment.Remove(transaction);
				}
			}
		}

		public virtual int GetAllowedRefundItemQty(SPRefundsBucket bucket, RefundLineItem item, ref Dictionary<string, Tuple<int, int, int>> rcOrderPerItem)
		{
			//if item is not imported and refund type is NoRestock, should check the item is from Unfulfilled item or fulfilled item
			//if it's from unfulfilled item, should modify original SO instead of creating RC order, we should not allow to create refund item with its Qty
			//if it's from fulfilled item, should create refund item with its Qty
			int allowedQty = 0;
			//If data existed, calculate how many item should be created in RC based on current data.
			if (rcOrderPerItem.TryGetValue(item.LineItemId.ToString(), out Tuple<int, int, int> rcData) && rcData != null)
			{
				var totalRCExceptReturn = rcData.Item1;
				var createdRC = rcData.Item2;
				var returnRC = rcData.Item3;

				if (item.RestockType == RestockType.Return)
				{
					allowedQty = item.Quantity ?? 0;
				}
				else if (createdRC >= totalRCExceptReturn)
				{
					allowedQty = 0;
				}
				else
				{
					allowedQty = ((item.Quantity ?? 0) + createdRC) > totalRCExceptReturn ? (totalRCExceptReturn - createdRC) : (item.Quantity ?? 0);
				}
				//Update data
				rcOrderPerItem[item.LineItemId.ToString()] = new Tuple<int, int, int>(totalRCExceptReturn, (createdRC + (item.RestockType == RestockType.Return ? 0 : allowedQty)), returnRC);
			}
			else
			{
				var refundItems = bucket.Refunds.Extern.Refunds?.SelectMany(x => x.RefundLineItems)?.Where(r => r.LineItemId == item.LineItemId);
				//Get the total Cancel type quantity for current item
				var totalCancelledRefundItems = refundItems?.Where(x => x?.RestockType != null && x.RestockType == RestockType.Cancel)?.Sum(x => x.Quantity ?? 0) ?? 0;
				//Get the total No restock type quantity for current item
				var totalNoRestockRefundItems = refundItems?.Where(x => x?.RestockType != null && x.RestockType == RestockType.NoRestock)?.Sum(x => x.Quantity ?? 0) ?? 0;
				//Get the total return type quantity for current item
				var totalReturnRefundItems = refundItems?.Where(x => x?.RestockType != null && x.RestockType == RestockType.Return)?.Sum(x => x.Quantity ?? 0) ?? 0;
				//Get the total fulfilled item quantity from Shopify side
				var totalFulfilledItems = bucket.Refunds.Extern.Fulfillments?.Where(x => x?.Status != null && x?.Status == FulfillmentStatus.Success)?.
					SelectMany(x => x.LineItems)?.Where(x => x?.Id == item.LineItemId)?.Sum(x => x?.Quantity ?? 0) ?? 0;
				//If FulfillableQty + total refund items + totalFulfilledItems equals to original total item qty, that means all refund items are from unfulfilled item, we should not create RC order
				//If FulfillableQty + total refund items + totalFulfilledItems greater than original total item qty, that means refund items are from both unfulfilled and fulfilled items, we should use the discrepancies to create RC order 
				var totalRCExceptReturn = (item.OrderLineItem.FulfillableQuantity ?? 0) + totalCancelledRefundItems + totalNoRestockRefundItems + totalFulfilledItems - (item.OrderLineItem.Quantity ?? 0);

				//return type should be always created RC order item
				if (item.RestockType == RestockType.Return)
					allowedQty = (item.Quantity ?? 0);
				else
					allowedQty = (item.Quantity ?? 0) > totalRCExceptReturn ? totalRCExceptReturn : (item.Quantity ?? 0);

				//Update data
				rcOrderPerItem[item.LineItemId.ToString()] = new Tuple<int, int, int>(totalRCExceptReturn, (item.RestockType == RestockType.Return ? 0 : allowedQty), totalReturnRefundItems);
			}

			return allowedQty;
		}

		[Obsolete("This item has been deprecated and will be removed in Acumatica ERP 2024 R2. Please use SPOrderBaseProcessor.CalculateRefundItemDiscount instead.")]
		public virtual decimal? CalculateRefundItemDisc(SPRefundsBucket bucket, RefundLineItem item)
		{
			return base.CalculateRefundItemDiscount(item);
		}

		#endregion

		/// <summary>
		/// There are 2 refunds created by POS exchange, one refund with transaction only, it will be used to create the refund payment;
		/// another one with refundItems only, it will be used to create the refund order.
		/// To link the refund payment with the correct associated refund order, when we create the refund payment, we will find the refund with refundItems record,
		/// and use its refund ID instead of the ID of itself to create the relationship
		/// </summary>
		protected virtual string FindAssociatedRefundIdForPOSExchangeTransaction(OrderData orderData, string currentRefundId, string transactionId)
		{
			if(IsPOSExchangePayment(orderData, transactionId))
			{
				string anotherRefundId = orderData.ExchangeOrders.Where(x => x.Refunds.Any(r => r?.Id?.ConvertGidToId() == currentRefundId))?
																	.SelectMany(x => x.Refunds)
																	.FirstOrDefault(x => x.Id.ConvertGidToId() != currentRefundId)?.Id?.ConvertGidToId();
				if(anotherRefundId != null
					&& orderData.Refunds.FirstOrDefault(x => string.Equals(x.Id.ToString(), currentRefundId))?.RefundLineItems?.Any() != true
					&& orderData.Refunds.FirstOrDefault(x => string.Equals(x.Id.ToString(), anotherRefundId))?.RefundLineItems?.Any() == true)
				{
					return anotherRefundId;
				}
			}

			return currentRefundId;
		}

		public virtual void CreateRefundPayment(SPRefundsBucket bucket, MappedRefunds existing)
		{
			BCBinding binding = GetBinding();
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			ARSetup arSetup = PXSelect<ARSetup>.Select(this);

			SalesOrder impl = bucket.Refunds.Local;
			OrderData orderData = bucket.Refunds.Extern;
			List<OrderRefund> refunds = orderData.Refunds;
			impl.Payment = new List<Payment>();

			List<PXResult<PX.Objects.AR.ARPayment, BCSyncStatus>> result = PXSelectJoin<PX.Objects.AR.ARPayment,
					InnerJoin<BCSyncStatus, On<PX.Objects.AR.ARPayment.noteID, Equal<BCSyncStatus.localID>>>,
					Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
						And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
						And<BCSyncStatus.entityType, Equal<Required<BCEntity.entityType>>,
						And<BCSyncStatus.parentSyncID, Equal<Required<BCSyncStatus.parentSyncID>>
					>>>>>.Select(this, BCEntitiesAttribute.Payment, bucket.Refunds.ParentID).AsEnumerable().
					Cast<PXResult<PX.Objects.AR.ARPayment, BCSyncStatus>>().ToList();

			int refundsCount = refunds.Count(x => x.Transactions.Any(y => y.Status == TransactionStatus.Success));

			foreach (var refund in refunds)
			{
				//Usually it's Authorized status if no transactions in refund, we will skip it now until we find a solution to solve Authorized payment issue.
				//Without transactions, we cannot know the original payment reference information, we cannot link to a Prepayment.
				if (refund.Transactions == null || refund.Transactions.Any() == false)
					continue;

				foreach (var transaction in refund.Transactions)
				{
					if (transaction?.Status == TransactionStatus.Success)
					{
						//If all transaction have kind refund and receipt action as void then skip
						if (transaction.Kind == TransactionType.Refund && GetHelper<SPHelper>().GetReceiptAction(transaction) == TransactionType.Void) continue;

						var origPayment = orderData.Transactions.FirstOrDefault(x => x.Id == transaction.ParentId);
						Payment refundPayment = new Payment();
						BCPaymentMethods currentPayment = null;
						ARPayment arPayment = null;
						refundPayment.DocumentsToApply = new List<Core.API.PaymentDetail>();

						string refundId = FindAssociatedRefundIdForPOSExchangeTransaction(orderData, refund.Id.ToString(), transaction.Id.ToString());
						refundPayment.TransactionID = new object[] { refundId, transaction.Id.ToString() }.KeyCombine();

						var ccrefundTransactions = orderData.Transactions.Where(x => (x.Kind == TransactionType.Refund || x.Kind == TransactionType.Void) && x.Authorization != null && x.Status == TransactionStatus.Success);
						if ((orderData.FinancialStatus == OrderFinancialStatus.Refunded || orderData.FinancialStatus == OrderFinancialStatus.Voided) && (refundsCount == 1 && ccrefundTransactions?.Count() == 1)
							&& ((existing.Local?.Status?.Value != PX.Objects.SO.Messages.Completed) || (existing.Local?.Status?.Value == PX.Objects.SO.Messages.Completed && transaction.Kind == TransactionType.Void)) && origPayment.Authorization != null)
						{
							/*call voidCardPayment Action
							 * In case fully refunded and open AC order with authorize/Captured(settled/unsettled) CC type payment or
							 * In case fully refunded and completed AC order with authorize cctype payment
							*/
							currentPayment = GetHelper<SPHelper>().GetPaymentMethodMapping(transaction.Gateway.ReplaceEmptyString(BCConstants.NoneGateway), transaction.Currency, out String cashAcount, false);
							if (currentPayment?.ProcessRefunds != true)
							{
								LogInfo(Operation.LogScope(bucket.Refunds.SyncID), BCMessages.LogRefundPaymentSkipped, orderData.Id, refund.Id, transaction.Id, transaction.Gateway.ReplaceEmptyString(BCConstants.NoneGateway));
								continue; // void payment if only ProcessRefunds is checked
							}
							var parentID = (origPayment.Kind == TransactionType.Capture && origPayment.ParentId != null) ? origPayment.ParentId : transaction.ParentId;// to handle seperate capture transaction
							arPayment = result.FirstOrDefault(x => x.GetItem<BCSyncStatus>()?.ExternID.KeySplit(1) == parentID.ToString())?.GetItem<ARPayment>();
							if (transaction.Kind == TransactionType.Refund)
							{
								if (arPayment == null) throw new PXException(BCMessages.OriginalPaymentNotImported, parentID.ToString(), orderData.Id.ToString());
								if (arPayment?.Released != true) throw new PXException(BCMessages.OriginalPaymentNotReleased, parentID.ToString(), orderData.Id.ToString());
								if (existing != null)
								{
									PopulateNoteID(existing, refundPayment, ARPaymentType.VoidPayment, arPayment.RefNbr);
									if (refundPayment.NoteID != null)
									{
										refundPayment.NeedRelease = currentPayment?.ReleasePayments ?? false;
										impl.Payment.Add(refundPayment);
										continue;
									}
								}
							}
							else
							{
								if (arPayment == null) throw new PXException(BCMessages.OriginalPaymentNotImported, parentID.ToString(), orderData.Id.ToString());
								if (arPayment.IsCCCaptured == true) throw new PXException(BCMessages.OriginalPaymentStatusMismatch, parentID.ToString(), orderData.Id.ToString());
								if (arPayment.Voided == true)
								{
									refundPayment.NoteID = arPayment.NoteID.ValueField();
									impl.Payment.Add(refundPayment);
									continue;
								}

							}
							refundPayment.Type = ARDocType.GetDisplayName(arPayment?.DocType ?? ARDocType.Prepayment).ValueField();
							refundPayment.ReferenceNbr = arPayment.RefNbr.ValueField();

							// if that is not CC payment we should not do CC Void.
							// ProcessingCenterID is used in the SaveBucketImport to identify if it is CC Void or Normal Void
							if (arPayment.ProcessingCenterID != null && arPayment.IsCCPayment == true)
							{
								refundPayment.ProcessingCenterID = arPayment.ProcessingCenterID?.ValueField();
							}
							
							refundPayment.VoidCardParameters = new VoidCardPayment();
							if (ccrefundTransactions.FirstOrDefault()?.Kind == TransactionType.Void)
							{
								refundPayment.VoidCardParameters.TranType = CCTranTypeCode.VoidTran.ValueField();
								refundPayment.VoidCardParameters.TranNbr = GetHelper<SPHelper>().ParseTransactionNumber(origPayment, out bool isCreditCardTran).ValueField();
							}
							else
							{
								refundPayment.VoidCardParameters.TranType = CCTranTypeCode.Unknown.ValueField();
								refundPayment.VoidCardParameters.TranNbr = GetHelper<SPHelper>().ParseTransactionNumber(ccrefundTransactions.FirstOrDefault(), out bool isCreditCardTran).ValueField();
							}

							refundPayment.NeedRelease = currentPayment?.ReleasePayments ?? false;
							impl.Payment.Add(refundPayment);

						}
						else// create CR payment
						{
							bool criteriaForCreatingPaymentWithRC = (existing.Local?.Status?.Value == PX.Objects.SO.Messages.Completed
								&& !(existing.Local.ExternalRefundRef?.Value != null && (existing.Local.ExternalRefundRef.Value.Split(new char[] { ';' }).Contains(refund.Id.ToString()))))
								|| (refund.RefundLineItems?.Any() == true && refund.RefundLineItems.All(r => r.RestockType != RestockType.Cancel))
								|| (refund.RefundLineItems?.Any() != true && refund.OrderAdjustments?.Any() == true && refund.OrderAdjustments.All(adj => adj.Kind == OrderAdjustmentType.ShippingRefund));
							if (criteriaForCreatingPaymentWithRC)// then create Cr payment with RC order 
							{
								refundPayment.CreateWithRC = true;
								refundPayment.PaymentAmount = transaction.Amount.ValueField();
							}

							refundPayment.ExternalRef = transaction.Id.ToString().ValueField();
							refundPayment.PaymentRef = GetHelper<SPHelper>().ParseTransactionNumber(transaction, out bool isCreditCardTran).ValueField();
							currentPayment = GetHelper<SPHelper>().GetPaymentMethodMapping(transaction.Gateway.ReplaceEmptyString(BCConstants.NoneGateway), transaction.Currency, out String cashAcount, false);
							refundPayment.NeedRelease = currentPayment?.ReleasePayments ?? false;

							//check if existing CR Payment
							if (existing != null)
							{
								PopulateNoteID(existing, refundPayment, ARPaymentType.Refund, refundPayment.ExternalRef.Value);
								if (refundPayment.NoteID != null)
								{
									impl.Payment.Add(refundPayment);
									continue;
								}
							}

							//mapy summary section
							refundPayment.Type = PX.Objects.AR.Messages.Refund.ValueField();
							refundPayment.CustomerID = existing.Local.CustomerID;
							refundPayment.CustomerLocationID = existing.Local.LocationID;
							var date = refund.DateCreatedAt.ToDate(false, PXTimeZoneInfo.FindSystemTimeZoneById(GetBindingExt<BCBindingExt>().OrderTimeZone));
							if (date.HasValue)
								refundPayment.ApplicationDate = (new DateTime(date.Value.Date.Ticks)).ValueField();
							refundPayment.BranchID = existing.Local.FinancialSettings.Branch;
							var description = PXMessages.LocalizeFormat(ShopifyMessages.PaymentRefundDescription, binding.BindingName, orderData?.Name, refund.Id.ToString(), GetHelper<SPHelper>().FirstCharToUpper(transaction.Kind), transaction.Status?.ToString(),
								transaction.Gateway.ReplaceEmptyString(BCConstants.NoneGateway));
							refundPayment.Description = description.ValueField();

							refundPayment.PaymentAmount = (transaction.Amount ?? 0).ValueField();

							//map paymentmethod
							if (currentPayment?.ProcessRefunds != true)
							{
								LogInfo(Operation.LogScope(bucket.Refunds.SyncID), BCMessages.LogRefundPaymentSkipped, orderData.Id, refund.Id, transaction.Id, transaction.Gateway.ReplaceEmptyString(BCConstants.NoneGateway));
								continue; // create CR payment if only ProcessRefunds is checked
							}
							var parentID = (origPayment.Kind == TransactionType.Capture && origPayment.ParentId != null) ? origPayment.ParentId : transaction.ParentId;// to handle seperate capture transaction
							arPayment = result.FirstOrDefault(x => x.GetItem<BCSyncStatus>().ExternID.KeySplit(1) == parentID.ToString());
							if (currentPayment?.ProcessingCenterID != null && isCreditCardTran)
							{
								GetHelper<SPHelper>().AddCreditCardProcessingInfo(currentPayment, refundPayment, transaction.Kind);
								if (arPayment?.IsCCPayment == true)
								{
									refundPayment.OrigTransaction = ExternalTransaction.PK.Find(this, arPayment?.CCActualExternalTransactionID)?.TranNumber.ValueField();
								}
							}

							refundPayment.CashAccount = cashAcount?.Trim()?.ValueField();
							refundPayment.PaymentMethod = currentPayment?.PaymentMethodID?.ValueField();

							//Payment and Refund should depend on AR Pref.
							refundPayment.Hold = (arSetup?.HoldEntry == true).ValueField();
							
							if (arPayment == null) throw new PXException(BCMessages.OriginalPaymentNotImported, parentID.ToString(), orderData.Id.ToString());
							if (arPayment?.Released != true) throw new PXException(BCMessages.OriginalPaymentNotReleased, parentID.ToString(), orderData.Id.ToString());

							ValidateCRPayment(arPayment);

							AddRefundPaymentDetail(bucket, refundPayment, refund, transaction, arPayment);

							impl.Payment.Add(refundPayment);

						}
					}
				}
			}
		}

		#region Methods for processing refund Payment details

		public virtual void PopulateNoteID(MappedRefunds existing, Payment refundPayment, string docType, string reference)
		{
			if (existing?.Details?.Count() > 0)
			{
				existing?.Details.FirstOrDefault(d => d.EntityType == BCEntitiesAttribute.Payment && d.ExternID == refundPayment.TransactionID).With(p => refundPayment.NoteID = p.LocalID.ValueField());
			}
			if (refundPayment.NoteID?.Value == null)
			{
				GetHelper<SPHelper>().GetExistingRefundPayment(refundPayment, docType, reference);
			}
		}

		public virtual void ValidateCRPayment(ARPayment payment)
		{
			//Try to find 2 kinds of documents without released
			//1.Refund Payment that linked to the Original Payment: adjdRefNbr is the RefNbr of payment, adjdDocType is the DocType of payment, adjgDocType is the DocType of refund payment
			//2.Invoice or other documents that linked to the Original Payment: adjgRefNbr is the RefNbr of payment, adjgDocType is the DocType of payment
			foreach (ARAdjust adjust in SelectFrom<ARAdjust>.
				Where<ARAdjust.released.IsEqual<False>
					.And<Brackets<ARAdjust.adjdRefNbr.IsEqual<@P.AsString>.And<ARAdjust.adjdDocType.IsEqual<@P.AsString>>.And<ARAdjust.adjgDocType.IsEqual<@P.AsString>>>
					.Or<ARAdjust.adjgRefNbr.IsEqual<@P.AsString>.And<ARAdjust.adjgDocType.IsEqual<@P.AsString>>>>>
				.View.Select(this, payment.RefNbr, payment.DocType, ARPaymentType.Refund, payment.RefNbr, payment.DocType))
			{
				if (adjust.AdjdDocType == payment.DocType && adjust.AdjgDocType == ARPaymentType.Refund)
					throw new PXException(BCMessages.UnreleasedCRPayment, payment.RefNbr, adjust.AdjgRefNbr);
				else if (adjust.AdjgDocType == payment.DocType)
					throw new PXException(BCMessages.PaymentAndInvoiceNotReleased, adjust.AdjdRefNbr, payment.RefNbr);
			}
		}

		public virtual void AddRefundPaymentDetail(SPRefundsBucket bucket, Payment refundPayment, OrderRefund refund, OrderTransaction transaction, ARPayment originalPayment)
		{
			//If transaction amount is all from fulfilled item, Payment should create with RC order
			//If transaciton amount is all from unfulfilled item, Payment should create separately and create PaymentDetail and link to Prepayment
			//If transaction amount is from both fulfilled and unfulfilled items, we should create Payment sparately, and use total unfulfilled items amount to create PaymentDetail,
			//and use total fulfilled items amount to create PaymentOrderDetail
			//If there is any order adjustments in the refund, we should check and calculate them as well.
			var totalAmountForCancelItems = refund.RefundLineItems?.Where(r => r.RestockType == RestockType.Cancel)?.Sum(x => (x.SubTotalPresentment ?? 0m) + (x.TotalTaxPresentment ?? 0m)) ?? 0m;
			var totalOrderAdjustments = refund.OrderAdjustments?.Sum(x => x.AmountPresentment ?? 0m) ?? 0m;
			var totalAmountForRCorder = (transaction.Amount ?? 0m) - (totalAmountForCancelItems - totalOrderAdjustments);
			//If there is any order adjustment or refund item is NoRestock type, it may need to apply the refund Payment to original Prepayment
			//If it's a POS Exchange order, Shopify will create a refund without RefundLineItems but with Transaction record, and another refund with RefundLineItems but without Transaction, in this case, we can only link the Refund payment to the original Payment
			if (originalPayment.Status != ARDocStatus.Closed && ((totalAmountForCancelItems - totalOrderAdjustments) != 0m
				|| refund.RefundLineItems?.Any(x => x?.RestockType == RestockType.NoRestock) == true))
			{
				Core.API.PaymentDetail paymentDetail = new Core.API.PaymentDetail();
				paymentDetail.ReferenceNbr = originalPayment?.RefNbr.ValueField();
				paymentDetail.DocType = ARDocType.GetDisplayName(originalPayment?.DocType ?? ARDocType.Prepayment).ValueField();
				paymentDetail.AmountPaid = ((totalAmountForCancelItems - totalOrderAdjustments) == 0m ? (transaction.Amount ?? 0m) : (totalAmountForCancelItems - totalOrderAdjustments)).ValueField();
				refundPayment.DocumentsToApply.Add(paymentDetail);
			}

			if (totalAmountForRCorder != 0m && (totalOrderAdjustments != 0m || refund.RefundLineItems?.Count > 0 || !string.Equals(refundPayment.TransactionID.KeySplit(0), refund.Id.ToString())))
			{
				//Should create the RC order first and then apply RC order info here.
				Core.API.PaymentOrderDetail orderDetail = new PaymentOrderDetail();
				orderDetail.AppliedToOrder = totalAmountForRCorder.ValueField();
				refundPayment.OrdersToApply = new List<PaymentOrderDetail>() { orderDetail };
			}
		}

		#endregion

		public override async Task SaveBucketImport(SPRefundsBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
			MappedRefunds obj = bucket.Refunds;
			// create CR payment and release it
			SalesOrder order = obj.Local;

			try
			{
				obj.ClearDetails();

				await SaveRefundOrders(bucket, order);

				//Add Payment after RC order in case Payment need to apply to both Prepayment and RC order
				await SaveRefundPayments(bucket, order);

				UpdateStatus(obj, operation);

				if (order.EditSO)
				{
					bucket.Order.ExternTimeStamp = DateTime.MaxValue;
					EnsureStatus(bucket.Order, SyncDirection.Import, Conditions.Resync);
				}
				else
					bucket.Order = null;
			}
			catch
			{
				throw;
			}
		}

		#region Methods for saving bucket details

		public virtual async Task SaveRefundOrders(SPRefundsBucket bucket, SalesOrder order)
		{
			foreach (var refundOrder in order.RefundOrders ?? new List<SalesOrder>())
			{
				var localID = refundOrder.Id;
				SalesOrder impl = null;

				if (refundOrder.Id == null)
				{
					#region Taxes
					//Logging for taxes
					GetHelper<SPHelper>().LogTaxDetails(bucket.Refunds.SyncID, refundOrder);
					#endregion

					impl = cbapi.Put<SalesOrder>(refundOrder, localID);
					localID = impl.Id;

					#region Taxes
					await GetHelper<SPHelper>().ValidateTaxes(bucket.Refunds.SyncID, impl, refundOrder);
					#endregion

					if (impl != null && localID != null)
					{
						UpdateRCPaymentWithRCOrder(bucket, impl, refundOrder, order.Payment);
					}
				}

				if (!bucket.Refunds.Details.Any(x => x.LocalID == localID))
				{
					bucket.Refunds.AddDetail(BCEntitiesAttribute.CustomerRefundOrder, localID, refundOrder.RefundID);
				}

				UpdateSyncDetailsWtihOrderDetails(bucket, impl, refundOrder);

				await UpdateSyncDetailsWithOrderPaymentsAsync(bucket, impl, refundOrder);
			}
		}

		public virtual void UpdateRCPaymentWithRCOrder(SPRefundsBucket bucket, SalesOrder insertedRCOrder, SalesOrder refundOrder, List<Payment> PaymentsList)
		{
			//Check Payments whether need to apply to this RC order
			var associatedPayment = PaymentsList?.FirstOrDefault(x => x.TransactionID.KeySplit(0) == refundOrder.RefundID && x.OrdersToApply != null);
			if (associatedPayment != null && associatedPayment.OrdersToApply.Any())
			{
				//Using the RC order info to update PaymentOrderDetail in the Payment
				var orderToApply = associatedPayment.OrdersToApply.First();
				if (refundOrder.CanHaveRefundPayment == true)
				{
					orderToApply.OrderType = insertedRCOrder.OrderType;
					orderToApply.OrderNbr = insertedRCOrder.OrderNbr;
					orderToApply.AppliedToOrder = associatedPayment.PaymentAmount?.Value > Math.Abs(insertedRCOrder.OrderTotal?.Value ?? 0m) ? Math.Abs(insertedRCOrder.OrderTotal?.Value ?? 0m).ValueField() : associatedPayment.PaymentAmount;
				}
				else
				{
					//If the order type in the refund order doesn't allow refund payment, we cannot link the refund payment to the refund order
					associatedPayment.OrdersToApply.Remove(orderToApply);
				}

				//In case there is any order adjustments, adjust the amount that link to prepayment
				var documentToApply = associatedPayment.DocumentsToApply?.FirstOrDefault();

				if (documentToApply != null && documentToApply.AmountPaid?.Value != (associatedPayment.PaymentAmount?.Value - orderToApply.AppliedToOrder.Value))
				{
					documentToApply.AmountPaid = (associatedPayment.PaymentAmount?.Value - orderToApply.AppliedToOrder.Value).ValueField();
				}
				else if (documentToApply != null && associatedPayment.PaymentAmount?.Value == orderToApply.AppliedToOrder.Value)
				{
					//If all amount has applied to RC order, we don't need to apply to original Prepayment
					associatedPayment.DocumentsToApply.Remove(documentToApply);
				}
			}
		}

		public virtual void UpdateSyncDetailsWtihOrderDetails(SPRefundsBucket bucket, SalesOrder insertedRCOrder, SalesOrder refundOrder)
		{
			List<SalesOrderDetail> details = (refundOrder.Id != null ? refundOrder.Details : insertedRCOrder.Details) ?? new List<SalesOrderDetail>();

			foreach (var lineitem in details)
			{
				if (!bucket.Refunds.Details.Any(x => x.LocalID == lineitem.Id))
				{
					if (lineitem.InventoryID.Value.Trim() == refundItem.InventoryCD.Trim())
						continue;
					else if (lineitem.InventoryID.Value.Trim() == giftCertificateItem?.Value?.InventoryCD?.Trim() ||
						bucket.Order.Extern.LineItems.Any(x => x.Id.ToString() == lineitem.ExternalRef?.Value))
						bucket.Refunds.AddDetail(BCEntitiesAttribute.OrderLine, lineitem.Id, new object[] { refundOrder.RefundID, lineitem.ExternalRef.Value }.KeyCombine());
					else
					{
						var detail = bucket.Refunds.Extern.Refunds.FirstOrDefault(x => x.Id.ToString() == refundOrder.RefundID).RefundLineItems.FirstOrDefault(x => !bucket.Refunds.Details.Any(o => x.Id.ToString() == o.ExternID)
							&& GetHelper<SPHelper>().GetInventoryCDByExternID(x.OrderLineItem.ProductId?.ToString(),
							x.OrderLineItem.VariantId.ToString(),
							x.OrderLineItem.Sku,
							x.OrderLineItem.Name,
							x.OrderLineItem.IsGiftCard,
							out string uom,
							out string alternateID,
							out string itemStatus) == lineitem.InventoryID.Value);
						if (detail != null)
							bucket.Refunds.AddDetail(BCEntitiesAttribute.OrderLine, lineitem.Id, new object[] { refundOrder.RefundID, detail.Id }.KeyCombine());
						else
							throw new PXException(BCMessages.CannotMapLines);
					}
				}
			}
		}

		[Obsolete("The method will be removed in the future versions. Instead, use UpdateSyncDetailsWithOrderPaymentsAsync.")]
		public virtual void UpdateSyncDetailsWithOrderPayments(SPRefundsBucket bucket, SalesOrder insertedRCOrder, SalesOrder refundOrder)
		{
			List<SalesOrderPayment> payments = (refundOrder.Id != null ? refundOrder.Payments : insertedRCOrder.Payments) ?? new List<SalesOrderPayment>();

			foreach (var payment in payments)
			{
				if (string.IsNullOrEmpty(payment.ExternalRef?.Value) && !string.IsNullOrEmpty(payment.ReferenceNbr?.Value))
				{
					var arPayment = ARPayment.PK.Find(this, ARPaymentType.Refund, payment.ReferenceNbr.Value);
					payment.ExternalRef = arPayment?.ExternalRef.ValueField();
					payment.NoteID = arPayment?.NoteID.ValueField();
					bool needRelease = (payment.NeedRelease ||
										refundOrder.Payments?.Any(pay => pay.ExternalRef?.Value == payment?.ExternalRef.Value && pay.NeedRelease) == true);
					if (needRelease && payment.NoteID != null && arPayment.Status == ARDocStatus.Balanced)
					{
						try
						{
							cbapi.Invoke<Payment, ReleasePayment>(null, payment.NoteID.Value, ignoreResult: !WebConfig.ParallelProcessingDisabled);
						}
						catch (Exception ex) { LogError(Operation.LogScope(bucket.Refunds), ex); }
					}
				}
				if (payment.NoteID?.Value != null && !bucket.Refunds.Details.Any(x => x.LocalID == payment.NoteID?.Value))
				{
					bucket.Refunds.AddDetail(BCEntitiesAttribute.Payment, payment.NoteID?.Value, new object[] { refundOrder.RefundID, payment.ExternalRef?.Value }.KeyCombine());
				}
			}
		}

		public virtual async Task UpdateSyncDetailsWithOrderPaymentsAsync(SPRefundsBucket bucket, SalesOrder insertedRCOrder, SalesOrder refundOrder)
		{
			List<SalesOrderPayment> payments = (refundOrder.Id != null ? refundOrder.Payments : insertedRCOrder.Payments) ?? new List<SalesOrderPayment>();

			foreach (var payment in payments)
			{
				if (string.IsNullOrEmpty(payment.ExternalRef?.Value) && !string.IsNullOrEmpty(payment.ReferenceNbr?.Value))
				{
					var arPayment = ARPayment.PK.Find(this, ARPaymentType.Refund, payment.ReferenceNbr.Value);
					payment.ExternalRef = arPayment?.ExternalRef.ValueField();
					payment.NoteID = arPayment?.NoteID.ValueField();
					bool needRelease = (payment.NeedRelease ||
										refundOrder.Payments?.Any(pay => pay.ExternalRef?.Value == payment?.ExternalRef.Value && pay.NeedRelease) == true);

					await ReleasePayment(bucket.Refunds, payment.NoteID?.Value, arPayment.Status, needRelease);
				}
				if (payment.NoteID?.Value != null && !bucket.Refunds.Details.Any(x => x.LocalID == payment.NoteID?.Value))
				{
					bucket.Refunds.AddDetail(BCEntitiesAttribute.Payment, payment.NoteID?.Value, new object[] { refundOrder.RefundID, payment.ExternalRef?.Value }.KeyCombine());
				}
			}
		}

		public virtual async Task SaveRefundPayments(SPRefundsBucket bucket, SalesOrder order)
		{
			List<Tuple<string, string>> addedRefNbr = new List<Tuple<string, string>>();
			foreach (var payment in order.Payment ?? new List<Payment>())
			{
				Guid? localId = payment.NoteID?.Value;
				Payment paymentResp = null;
				using (var transaction = await base.WithTransaction(async () =>
				{
					if (payment.VoidCardParameters != null)
					{
						paymentResp = !string.IsNullOrEmpty(payment.ProcessingCenterID?.Value)
							? cbapi.Invoke<Payment, VoidCardPayment>(payment, action: payment.VoidCardParameters)
							: cbapi.Invoke<Payment, VoidPayment>(payment);
						localId = paymentResp.Id;
					}
					else
					{
						foreach (var detail in payment.DocumentsToApply ?? new List<Core.API.PaymentDetail>())
						{
							if (addedRefNbr.Any(x => x.Item1 == detail.ReferenceNbr.Value))
							{
								throw new SetSyncStatusException(BCMessages.UnreleasedCRPayment, detail?.ReferenceNbr?.Value, addedRefNbr.FirstOrDefault(x => x.Item1 == detail.ReferenceNbr.Value).Item2);
							}
						}

						if (payment.NoteID?.Value == null)
						{
							AdjustRefundPaymentDetails(payment);

							paymentResp = cbapi.Put<Payment>(payment);
							localId = paymentResp?.Id;
							foreach (var detail in payment.DocumentsToApply ?? new List<Core.API.PaymentDetail>())
							{
								addedRefNbr.Add(new Tuple<string, string>(detail.ReferenceNbr.Value, paymentResp.ReferenceNbr.Value));
							}
						}
					}
				}))
				{
					transaction?.Complete();
				}

				await ReleasePayment(bucket.Refunds, localId, paymentResp?.Status?.Value, payment.NeedRelease);

				if (!bucket.Refunds.Details.Any(x => x.LocalID == localId))
				{
					bucket.Refunds.AddDetail(BCEntitiesAttribute.Payment, localId, payment.TransactionID.ToString());
				}
			}
		}

		public virtual async Task ReleasePayment(MappedRefunds obj, Guid? paymentId, string paymentStatus, bool needRelease)
		{
			if (needRelease && paymentId != null && (paymentStatus == PX.Objects.AR.Messages.Balanced || paymentStatus == ARDocStatus.Balanced))
			{
				using (var transaction = await base.WithTransaction(async () =>
				{
					try
					{
						cbapi.Invoke<Payment, ReleasePayment>(null, paymentId.Value, ignoreResult: !WebConfig.ParallelProcessingDisabled);
					}
					catch (Exception ex)
					{
						LogError(Operation.LogScope(obj), ex);
					}
				}))
				{
					transaction?.Complete();
				}
			}
		}

		public virtual void AdjustRefundPaymentDetails(Payment refundPayment)
		{
			//if there is refund items in Refund but all of them are used in the original SO, RC order could not be created, there is any records in OrdersToApply but no OrderType and OrderNbr,
			//we should remove it and adjust the amount in DocumentsToApply record as well.
			if (refundPayment.OrdersToApply?.Any(x => x.OrderType?.Value == null && x.OrderNbr?.Value == null) == true)
			{
				refundPayment.OrdersToApply = null;

				//adjust the amount that link to prepayment
				var documentToApply = refundPayment.DocumentsToApply?.FirstOrDefault();

				if (documentToApply != null && documentToApply.AmountPaid?.Value != refundPayment.PaymentAmount?.Value)
				{
					documentToApply.AmountPaid = refundPayment.PaymentAmount;
				}
			}
		}
		#endregion

		#endregion
	}
}

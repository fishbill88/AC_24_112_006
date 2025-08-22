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

using PX.Api.ContractBased.Models;
using PX.Commerce.BigCommerce.API.REST;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data.BQL;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.BigCommerce
{
	public class BCRefundsBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary { get => Refunds; }
		public IMappedEntity[] Entities => new IMappedEntity[] { Refunds };
		public override IMappedEntity[] PostProcessors { get => new IMappedEntity[] { Order }; }

		public MappedRefunds Refunds;
		public MappedOrder Order;
	}

	public class BCRefundsRestrictor : BCBaseRestrictor, IRestrictor
	{
		public virtual FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return null;
		}

		public virtual FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			return base.Restrict(mapped, delegate (MappedRefunds obj)
			{
				if (obj.Extern != null && obj.Extern.Refunds != null)
				{
					if (obj.Extern.Refunds.All(x => x.RefundPayments.All(a => a.IsDeclined)))
					{
						return new FilterResult(FilterStatus.Filtered,
							PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogRefundSkippedStatus, obj.Extern.Id));
					}
				}

				var orderStatus = (BCSyncStatus)processor.SelectStatus(BCEntitiesAttribute.Order, obj.Extern.Id.ToString(), false);

				//If the Sales order sync record is not in Synchronized, Pending status, then the Refund Entity should not be processed as it depends on the Sales Order entity.
				bool shouldSkipIfOrderNotSync = orderStatus == null || orderStatus.LocalID == null ||
												(orderStatus.LocalID != null && orderStatus.Status != BCSyncStatusAttribute.Synchronized && orderStatus.Status != BCSyncStatusAttribute.Pending);
				if (shouldSkipIfOrderNotSync)
				{
					return new FilterResult(FilterStatus.Invalid,
						PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.LogRefundSkippedOrderNotSynced, obj.Extern.Id.ToString()));
				}

				bool shouldFilterRefund = false;
				string filterRefundMessage = string.Empty;
				foreach (RefundPayment transaction in obj.Extern?.Refunds?.SelectMany(r => r.RefundPayments ?? new List<RefundPayment>()) ?? Enumerable.Empty<RefundPayment>())
				{
					if (transaction?.IsDeclined == false)
					{
						var parent = obj.Extern?.Transactions.FirstOrDefault(x => (x.Event == OrderPaymentEvent.Authorization || x.Event == OrderPaymentEvent.Purchase || x.Event == OrderPaymentEvent.Pending) && x.Gateway.Equals(transaction.ProviderId, StringComparison.OrdinalIgnoreCase));
						if (parent != null)
						{
							var paymentStatus = (BCSyncStatus)processor.SelectStatus(BCEntitiesAttribute.Payment, new Object[] { parent.OrderId, parent.Id }.KeyCombine(), false);
							//If the Payment sync record is not in Synchronized, Pending status, then the Refund Entity should not be processed as it depends on the Payment entity.
							if (paymentStatus != null && (paymentStatus.LocalID == null || (paymentStatus.LocalID != null && paymentStatus.Status != BCSyncStatusAttribute.Synchronized && paymentStatus.Status != BCSyncStatusAttribute.Pending)))
							{
								shouldFilterRefund = true;
								filterRefundMessage = PXMessages.LocalizeFormatNoPrefixNLA(BCMessages.PaymentStatusNotValidForRefund, parent.Id, BCSyncStatusAttribute.Convert(paymentStatus.Status));
							}
							else if (paymentStatus?.LocalID != null && (paymentStatus?.Status == BCSyncStatusAttribute.Synchronized || paymentStatus?.Status == BCSyncStatusAttribute.Pending))
							{
								//if there are multiple refunds in the order, and one of them is synced successfully, don't filter this refund out.
								shouldFilterRefund = false;
								break;
							}
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

	[BCProcessor(typeof(BCConnector), BCEntitiesAttribute.OrderRefunds, BCCaptions.Refunds, 150,
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
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.GiftWrapOrderLine, EntityName = BCCaptions.GiftWrapOrderLine, AcumaticaType = typeof(PX.Objects.SO.SOLine))]
	public class BCRefundsProcessor : BCOrderBaseProcessor<BCRefundsProcessor, BCRefundsBucket, MappedRefunds>
	{
		#region Initialization
		public override async Task Initialise(IConnector iconnector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(iconnector, operation, cancellationToken);
			WrapSaveBucketInTransaction = false;
		}
		#endregion

		#region Export

		public override async Task FetchBucketsForExport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
		}

		public override async Task<EntityStatus> GetBucketForExport(BCRefundsBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			SalesOrder impl = cbapi.GetByID<SalesOrder>(syncstatus.LocalID, GetCustomFieldsForExport());
			if (impl == null) return EntityStatus.None;

			bucket.Refunds = bucket.Refunds.Set(impl, impl.SyncID, impl.SyncTime);
			EntityStatus status = EnsureStatus(bucket.Refunds, SyncDirection.Export);

			return status;
		}

		public override async Task SaveBucketExport(BCRefundsBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Import
		public override async Task<PullSimilarResult<MappedRefunds>> PullSimilar(IExternEntity entity, CancellationToken cancellationToken = default)
		{
			var currentBinding = GetBinding();
			var currentBindingExt = GetBindingExt<BCBindingExt>();
			var uniqueFieldValue = ((OrderData)entity)?.Id.ToString();
			if (string.IsNullOrEmpty(uniqueFieldValue))
				return null;
			uniqueFieldValue = APIHelper.ReferenceMake(uniqueFieldValue, currentBinding.BindingName);
			List<MappedRefunds> result = new List<MappedRefunds>();
			List<string> orderTypes = new List<string>() { currentBindingExt?.OrderType };
			if (!string.IsNullOrWhiteSpace(currentBindingExt.OtherSalesOrderTypes))
			{
				//Support exported order type searching
				var exportedOrderTypes = currentBindingExt.OtherSalesOrderTypes?.Split(',').Where(i => i != currentBindingExt.OrderType);
				if (exportedOrderTypes.Count() > 0)
					orderTypes.AddRange(exportedOrderTypes);
			}
			GetHelper<BCHelper>().TryGetCustomOrderTypeMappings(ref orderTypes);

			foreach (SOOrder item in GetHelper<BCHelper>().OrderByTypesAndCustomerRefNbr.Select(orderTypes.ToArray(), uniqueFieldValue))
			{
				SalesOrder data = new SalesOrder() { SyncID = item.NoteID, SyncTime = item.LastModifiedDateTime, ExternalRef = item.CustomerRefNbr?.ValueField() };
				result.Add(new MappedRefunds(data, data.SyncID, data.SyncTime));
			}
			return new PullSimilarResult<MappedRefunds>() { UniqueField = uniqueFieldValue, Entities = result };
		}

		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			BCBindingExt bindingExt = GetBindingExt<BCBindingExt>();
			FilterOrders filter = new FilterOrders { IsDeleted = "false", };

			GetHelper<BCHelper>().SetFilterMinDate(filter, minDateTime, bindingExt.SyncOrdersFrom);
			if (maxDateTime != null) filter.MaxDateModified = maxDateTime;

			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (OrderData data in orderDataProvider.GetAll(filter, cancellationToken))
			{
				if (data.StatusId == (int)OrderStatuses.Refunded || data.StatusId == (int)OrderStatuses.PartiallyRefunded || data.RefundedAmount > 0)
				{
					data.Refunds = new List<OrderRefund>();
					await Task.Yield();
					await foreach (var item in orderRefundsRestDataProvider.GetAll(data.Id.ToString(), cancellationToken))
						data.Refunds.Add(item);
					if (data.Refunds.Any() == false) continue;

					BCRefundsBucket bucket = CreateBucket();
					var orderStatus = this.SelectStatus(BCEntitiesAttribute.Order, data.Id.ToString(), false);

					if (orderStatus == null) continue;

					var date = data.Refunds.Max(x => x.DateCreated.ToDate());
					MappedRefunds obj = bucket.Refunds = bucket.Refunds.Set(data, data.Id.ToString(), data.Id.ToString(), date).With(_ => { _.ParentID = orderStatus.SyncID; return _; });
					EntityStatus status = EnsureStatus(obj, SyncDirection.Import);
				}
			}
		}

		public override async Task<EntityStatus> GetBucketForImport(BCRefundsBucket bucket, BCSyncStatus syncstatus, CancellationToken cancellationToken = default)
		{
			OrderData orderData = await orderDataProvider.GetByID(syncstatus.ExternID.KeySplit(0).ToString());

			if (orderData == null || orderData.IsDeleted == true) return EntityStatus.None;

			EntityStatus status = EntityStatus.None;
			orderData.Refunds = new List<OrderRefund>();
			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			await foreach (var item in orderRefundsRestDataProvider.GetAll(orderData.Id.ToString(), cancellationToken))
				orderData.Refunds.Add(item);
			var orderStatus = (BCSyncStatus)this.SelectStatus(BCEntitiesAttribute.Order, orderData.Id.ToString(), false);
			if (orderStatus == null) return status;

			if (orderStatus.LastOperation == BCSyncOperationAttribute.Skipped)
				throw new PXException(BCMessages.OrderStatusSkipped, orderData.Id);

			bucket.Order = bucket.Order.Set(orderData, orderData.Id?.ToString(), orderData.Id?.ToString(), orderData.DateModifiedUT.ToDate());
			orderData.Refunds = orderData.Refunds.Where(x => !x.RefundPayments.All(p => p.IsDeclined))?.ToList();
			var date = orderData.Refunds.Max(x => x.DateCreated.ToDate());
			MappedRefunds obj = bucket.Refunds = bucket.Refunds.Set(orderData, orderData.Id.ToString(), orderData.Id.ToString(), date);

			// to force the code to run asynchronously and keep UI responsive.
			//In some case it runs synchronously especially when using IAsyncEnumerable
			await Task.Yield();
			orderData.OrderProducts = new List<OrdersProductData>();
			await foreach (var item in orderProductsRestDataProvider.GetAll(syncstatus.ExternID, cancellationToken))
				orderData.OrderProducts.Add(item);
			orderData.OrdersCoupons = new List<OrdersCouponData>();
			await foreach (var item in orderCouponsRestDataProvider.GetAll(syncstatus.ExternID, cancellationToken))
				orderData.OrdersCoupons.Add(item);
			orderData.Taxes = new List<OrdersTaxData>();
			await foreach (var item in orderTaxesRestDataProvider.GetAll(syncstatus.ExternID, cancellationToken))
				orderData.Taxes.Add(item);
			orderData.Transactions = new List<OrdersTransactionData>();
			await foreach (var item in orderTransactionsRestDataProvider.GetAll(syncstatus.ExternID, cancellationToken))
				orderData.Transactions.Add(item);
			status = EnsureStatus(obj, SyncDirection.Import);

			return status;
		}

		public override async Task MapBucketImport(BCRefundsBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			MappedRefunds obj = bucket.Refunds;
			OrderData orderData = obj.Extern;
			MappedRefunds mappedRefunds = existing as MappedRefunds;
			if (mappedRefunds?.Local == null) throw new PXException(BCMessages.OrderNotSyncronized, orderData.Id);
			if (mappedRefunds.Local.Status?.Value == PX.Objects.SO.Messages.Open || mappedRefunds.Local.Status?.Value == PX.Objects.SO.Messages.Hold ||
				mappedRefunds.Local.Status?.Value == BCObjectsMessages.RiskHold)
			{
				bucket.Refunds.Local = new SalesOrder();
				bucket.Refunds.Local.EditSO = true;
				CreateRefundPayment(bucket, mappedRefunds);
			}
			else if (mappedRefunds.Local.Status?.Value == PX.Objects.SO.Messages.Cancelled && orderData.StatusId == (int)OrderStatuses.Refunded)
			{
				bucket.Refunds.Local = new SalesOrder();
				CreateRefundPayment(bucket, mappedRefunds);
			}
			else if (mappedRefunds?.Local?.Status?.Value == PX.Objects.SO.Messages.Completed)
			{
				bucket.Refunds.Local = new SalesOrder();
				CreateRefundPayment(bucket, mappedRefunds);
				await CreateRefundOrders(bucket, mappedRefunds, cancellationToken);

			}
			else
				throw new PXException(BCMessages.OrderStatusNotValid, orderData.Id);

		}

		#region Create CustomerRefund 

		public virtual void CreateRefundPayment(BCRefundsBucket bucket, MappedRefunds existing)
		{
			var currentBinding = GetBinding();
			var currentBindingExt = GetBindingExt<BCBindingExt>();
			SalesOrder impl = bucket.Refunds.Local;
			OrderData orderData = bucket.Refunds.Extern;
			List<OrderRefund> refunds = orderData.Refunds;
			impl.Payment = new List<Payment>();

			List<PXResult<ARPayment, BCSyncStatus>> result = PXSelectJoin<ARPayment,
				InnerJoin<BCSyncStatus, On<PX.Objects.AR.ARPayment.noteID, Equal<BCSyncStatus.localID>>>,
				Where<BCSyncStatus.connectorType, Equal<Current<BCEntity.connectorType>>,
					And<BCSyncStatus.bindingID, Equal<Current<BCEntity.bindingID>>,
					And<BCSyncStatus.entityType, Equal<Required<BCEntity.entityType>>,
					And<BCSyncStatus.parentSyncID, Equal<Required<BCSyncStatus.parentSyncID>>
				>>>>>.Select(this, BCEntitiesAttribute.Payment, bucket.Refunds.ParentID).AsEnumerable().
				Cast<PXResult<ARPayment, BCSyncStatus>>().ToList();

			int refundsCount = refunds.Count(x => x.RefundPayments.Any(y => y.IsDeclined == false));
			List<string> usedTransactions = null;

			foreach (var refund in refunds)
			{
				foreach (var transaction in refund.RefundPayments)
				{
					if (transaction?.IsDeclined == false)
					{
						string transMethod = null;
						ARPayment aRPayment = null;
						BCPaymentMethods currentPayment = null;
						string cashAccount = null;
						Payment refundPayment = new Payment();
						refundPayment.DocumentsToApply = new List<Core.API.PaymentDetail>();
						refundPayment.TransactionID = new object[] { refund.Id, transaction.Id.ToString() }.KeyCombine();
						var parent = orderData.Transactions.FirstOrDefault(x => (x.Event == OrderPaymentEvent.Authorization || x.Event == OrderPaymentEvent.Purchase || x.Event == OrderPaymentEvent.Pending) && x.Gateway.Equals(transaction.ProviderId, StringComparison.InvariantCultureIgnoreCase));
						var ccrefundTransactions = orderData.Transactions.Where(x => x.Event == OrderPaymentEvent.Refund && x.GatewayTransactionId != null && x.Status == BCConstants.BCPaymentStatusOk);
						if (existing.Local?.Status?.Value != PX.Objects.SO.Messages.Completed && orderData.StatusId == (int)OrderStatuses.Refunded && (ccrefundTransactions?.Count() == 1 && refundsCount == 1 && refund.RefundPayments.Sum(x => x.Amount ?? 0) == (orderData.TotalIncludingTax)) && parent != null)
						{
							/*call voidCardPayment Action 
							 * if Ac order open and fully refunded with CC type payment method and captured(settled)
							*/
							#region VoidCardFlow
							refundPayment.Type = PX.Objects.AR.Messages.Prepayment.ValueField();
							if (!ValidateRefundTransaction(parent, bucket.Refunds.SyncID, orderData, refund.Id, transaction, out cashAccount, out currentPayment)) continue;
							var payment = result?.FirstOrDefault(x => x?.GetItem<BCSyncStatus>()?.ExternID.KeySplit(1) == parent.Id.ToString());
							aRPayment = payment?.GetItem<ARPayment>();
							if (aRPayment == null) throw new PXException(BCMessages.OriginalPaymentNotImported, parent.Id.ToString(), orderData.Id.ToString());
							if (aRPayment?.Released != true) throw new PXException(BCMessages.OriginalPaymentNotReleased, parent.Id.ToString(), orderData.Id.ToString());
							if (existing != null)
							{
								PopulateNoteID(existing, refundPayment, ARPaymentType.VoidPayment, aRPayment.RefNbr);
								if (refundPayment.NoteID != null)
								{
									refundPayment.NeedRelease = currentPayment?.ReleasePayments ?? false;
									impl.Payment.Add(refundPayment);
									continue;
								}
							}

							refundPayment.ReferenceNbr = aRPayment.RefNbr.ValueField();
							String paymentTran = GetHelper<BCHelper>().ParseTransactionNumber(ccrefundTransactions.FirstOrDefault(), out bool isCreditCardTran);

							// if that is not CC payment we should not do CC Void.
							// ProcessingCenterID is used in the SaveBucketImport to identify if it is CC Void or Normal Void
							if (aRPayment.ProcessingCenterID != null && aRPayment.IsCCPayment == true)
							{
								refundPayment.ProcessingCenterID = aRPayment.ProcessingCenterID?.ValueField();
							}

							refundPayment.VoidCardParameters = new VoidCardPayment()
							{
								TranType = CCTranTypeCode.Unknown.ValueField(),
								TranNbr = paymentTran.ValueField(),
							};

							impl.Payment.Add(refundPayment);
							#endregion
						}
						else
						{
							bool isCreditCardTran = false;

							refundPayment.ExternalRef = transaction.Id.ToString().ValueField();
							if (existing != null)
							{
								PopulateNoteID(existing, refundPayment, ARPaymentType.Refund, refundPayment.ExternalRef.Value);
								if (refundPayment.NoteID != null)
								{
									refundPayment.NeedRelease = currentPayment?.ReleasePayments ?? false;
									impl.Payment.Add(refundPayment);
									continue;
								}
							}
							OrdersTransactionData ccrefundTransaction = null;

							if (ccrefundTransactions?.Count() > 0)
							{
								var ccrefundTransactionList = ccrefundTransactions?.Where(x => (decimal)x.Amount == transaction.Amount.Value);
								//need this logic to get refund transaction. as there is no link between refund payment and refund transaction
								if (ccrefundTransactionList?.Count() > 1 && usedTransactions == null)
								{
									var ccTransactions = PXSelectJoin<BCSyncDetail, InnerJoin<ARPayment, On<BCSyncDetail.localID, Equal<ARPayment.noteID>>>,
									   Where<BCSyncDetail.syncID, Equal<Required<BCSyncDetail.syncID>>,
									   And<BCSyncDetail.entityType, Equal<Required<BCSyncDetail.entityType>>,
									   And<ARPayment.docType, Equal<ARDocType.refund>,
									   And<ARPayment.isCCPayment, Equal<True>>>>>>.Select(this, bucket.Refunds.SyncID, BCEntitiesAttribute.Payment)?
									   .AsEnumerable().Cast<PXResult<BCSyncDetail, ARPayment>>().ToList();

									usedTransactions = usedTransactions ?? new List<string>();
									usedTransactions.AddRange(ccTransactions.Select(x => x.GetItem<ARPayment>().ExtRefNbr));
									ccrefundTransaction = ccrefundTransactionList.FirstOrDefault(x => usedTransactions != null && !usedTransactions.Contains(GetHelper<BCHelper>().ParseTransactionNumber(x, out bool iscc)));
								}
								else
									ccrefundTransaction = ccrefundTransactionList.FirstOrDefault();
							}

							var reference = GetHelper<BCHelper>().ParseTransactionNumber(ccrefundTransaction, out isCreditCardTran);
							if (reference != null)
							{
								usedTransactions = usedTransactions ?? new List<string>();
								usedTransactions.Add(reference);
							}
							refundPayment.PaymentRef = (reference ?? transaction.Id.ToString()).ValueField();

							if (parent == null)
							{
								// if refund gateway does not match the orignal payment
								transMethod = transaction.ProviderId.Equals("storecredit", StringComparison.InvariantCultureIgnoreCase) ? BCObjectsConstants.StoreCreditCode : transaction.ProviderId;
								currentPayment = GetHelper<BCHelper>().GetPaymentMethodMapping(transMethod, null, orderData.CurrencyCode, out cashAccount, false);
								if (currentPayment?.ProcessRefunds != true)
								{
									LogInfo(Operation.LogScope(bucket.Refunds.SyncID), BCMessages.LogRefundPaymentSkipped, orderData.Id, refund.Id, transaction.Id, currentPayment?.PaymentMethodID ?? transMethod);
									continue; // create CR payment if only ProcessRefunds is checked
								}
								if (ccrefundTransaction == null && currentPayment?.ProcessingCenterID != null) continue;// if there is processing center and no external refund transaction then skip
								if (existing.Local?.Status?.Value == PX.Objects.SO.Messages.Completed && !(existing.Local.ExternalRefundRef?.Value != null && (existing.Local.ExternalRefundRef.Value.Split(new char[] { ';' }).Contains(refund.Id.ToString())))) // do not apply payment just create in on hold status
								{
									refundPayment.CreateWithRC = true;
								}
								else
								{
									decimal? amount = transaction.Amount;

									foreach (var payment in result)
									{
										if (amount == 0) break;
										aRPayment = payment.GetItem<ARPayment>();
										decimal curyAdjdAmt = ((aRPayment.CuryOrigDocAmt ?? 0) - (aRPayment.CuryUnappliedBal ?? 0));
										if (curyAdjdAmt <= 0) continue;
										if (aRPayment == null) throw new PXException(BCMessages.OriginalPaymentNotImported, aRPayment?.ExtRefNbr, orderData.Id.ToString());
										if (aRPayment?.Released != true) throw new PXException(BCMessages.OriginalPaymentNotReleased, aRPayment?.ExtRefNbr, orderData.Id.ToString());
										ValidateCRPayment(aRPayment?.RefNbr);
										if (curyAdjdAmt >= amount)
										{
											CreatePaymentDetail(refundPayment, aRPayment, amount);
											amount = 0;
											break;
										}
										else
										{
											amount = amount - curyAdjdAmt;
											CreatePaymentDetail(refundPayment, aRPayment, amount);
										}
									}
									if (amount != 0) throw new PXException(BCMessages.OriginalPaymentNotReleased, null, orderData.Id.ToString());
								}
								refundPayment.PaymentAmount = transaction.Amount.ValueField();
							}
							else // if refund payment gateway matches original payment 
							{
								if (!ValidateRefundTransaction(parent, bucket.Refunds.SyncID, orderData, refund.Id, transaction, out cashAccount, out currentPayment)) continue;
								if (ccrefundTransaction == null && currentPayment?.ProcessingCenterID != null) continue;// if there is processing center and no external refund transaction then skip
								var payment = result.FirstOrDefault(x => x.GetItem<BCSyncStatus>().ExternID.KeySplit(1) == parent.Id.ToString());
								aRPayment = payment?.GetItem<ARPayment>();
								if (currentPayment?.ProcessingCenterID != null && isCreditCardTran)
								{
									GetHelper<BCHelper>().AddCreditCardProcessingInfo(currentPayment, refundPayment, ccrefundTransaction.Event, ccrefundTransaction.PaymentInstrumentToken, ccrefundTransaction.CreditCard);
									if (aRPayment?.IsCCPayment == true)
									{
										refundPayment.OrigTransaction = ExternalTransaction.PK.Find(this, aRPayment?.CCActualExternalTransactionID)?.TranNumber.ValueField();
									}
								}
								if (existing.Local?.Status?.Value == PX.Objects.SO.Messages.Completed && !(existing.Local.ExternalRefundRef?.Value != null && (existing.Local.ExternalRefundRef.Value.Split(new char[] { ';' }).Contains(refund.Id.ToString()))))  // do not apply payment just create in on hold status
								{
									refundPayment.CreateWithRC = true;
									refundPayment.PaymentAmount = transaction.Amount.ValueField();
								}
								else
								{

									if (aRPayment == null) throw new PXException(BCMessages.OriginalPaymentNotImported, parent.Id.ToString(), orderData.Id.ToString());
									if (aRPayment?.Released != true) throw new PXException(BCMessages.OriginalPaymentNotReleased, parent.Id.ToString(), orderData.Id.ToString());

									ValidateCRPayment(aRPayment?.RefNbr);
									var paymentDetail = CreatePaymentDetail(refundPayment, aRPayment, transaction.Amount);
									refundPayment.PaymentAmount = paymentDetail.AmountPaid;

								}
							}

							//map Sumary section
							refundPayment.Type = PX.Objects.AR.Messages.Refund.ValueField();
							refundPayment.CustomerID = existing.Local.CustomerID;
							refundPayment.CustomerLocationID = existing.Local.LocationID;
							var date = refund.DateCreated.ToDate(PXTimeZoneInfo.FindSystemTimeZoneById(currentBindingExt.OrderTimeZone));
							if (date.HasValue)
								refundPayment.ApplicationDate = (new DateTime(date.Value.Date.Ticks)).ValueField();
							refundPayment.BranchID = existing.Local.FinancialSettings.Branch;
							refundPayment.TransactionID = new object[] { refund.Id, transaction.Id.ToString() }.KeyCombine();
							refundPayment.PaymentMethod = currentPayment?.PaymentMethodID?.ValueField();
							refundPayment.CashAccount = cashAccount?.Trim()?.ValueField();
							refundPayment.NeedRelease = currentPayment.ReleasePayments ?? false;

							var desc = PXMessages.LocalizeFormat(BigCommerceMessages.PaymentRefundDescription, currentBinding.BindingName, orderData.Id, refund.Id, transaction.ProviderId);
							refundPayment.Description = desc.ValueField();
							impl.Payment.Add(refundPayment);
						}

					}
				}
			}
		}

		public virtual void PopulateNoteID(MappedRefunds existing, Payment refundPayment, string docType, string reference, string paymentRef = null)
		{
			if (existing?.Details?.Count() > 0)
			{
				existing?.Details.FirstOrDefault(d => d.EntityType == BCEntitiesAttribute.Payment && d.ExternID == refundPayment.TransactionID).With(p => refundPayment.NoteID = p.LocalID.ValueField());
			}

			if (refundPayment.NoteID?.Value == null)
			{
				GetHelper<BCHelper>().GetExistingRefundPayment(refundPayment, docType, reference);
			}
		}

		public virtual PaymentDetail CreatePaymentDetail(Payment cr, ARPayment aRPayment, decimal? amount)
		{
			Core.API.PaymentDetail paymentDetail = new Core.API.PaymentDetail();
			paymentDetail.DocType = ARDocType.GetDisplayName(aRPayment.DocType ?? ARDocType.Prepayment).ValueField();
			paymentDetail.AmountPaid = (amount ?? 0).ValueField();
			paymentDetail.ReferenceNbr = aRPayment?.RefNbr.ValueField();

			cr.DocumentsToApply.Add(paymentDetail);
			return paymentDetail;
		}

		//validates if existingCR payment is released or not
		public virtual void ValidateCRPayment(string adjgRefNbr)
		{
			var existinCRPayment = PXSelectJoin<PX.Objects.AR.ARPayment, InnerJoin<ARAdjust, On<ARPayment.refNbr, Equal<ARAdjust.adjgRefNbr>, And<ARAdjust.adjdRefNbr, Equal<Required<ARAdjust.adjdRefNbr>>>>>,
							Where<ARPayment.docType, Equal<Required<ARPayment.docType>>>>.Select(this, adjgRefNbr, ARPaymentType.Refund);
			if (existinCRPayment != null && existinCRPayment.Count > 0)
			{
				if (existinCRPayment.Any(x => x.GetItem<ARPayment>().Released == false))
					throw new PXException(BCMessages.UnreleasedCRPayment, adjgRefNbr, existinCRPayment.FirstOrDefault(x => x.GetItem<ARPayment>().Released == false).GetItem<ARPayment>().RefNbr);
			}
		}

		//validates if processrefund checkbox is checked and credit card refund transaction is not offline
		public virtual bool ValidateRefundTransaction(OrdersTransactionData parent, int? syncID, OrderData orderData, int refundId, RefundPayment trans, out string cashAccount, out BCPaymentMethods methodMapping)
		{
			string transMethod = GetHelper<BCHelper>().GetPaymentMethodName(parent);
			methodMapping = GetHelper<BCHelper>().GetPaymentMethodMapping(transMethod, orderData.PaymentMethod, orderData.CurrencyCode, out cashAccount, false);
			if (methodMapping?.ProcessRefunds != true)
			{
				LogInfo(Operation.LogScope(syncID), BCMessages.LogRefundPaymentSkipped, orderData.Id, refundId, trans.Id, methodMapping?.PaymentMethodID ?? transMethod);
				return false; // process refund if only ProcessRefunds is checked
			}
			if (trans.Offline && parent.CreditCard != null)
			{
				LogInfo(Operation.LogScope(syncID), BCMessages.LogRefundCCPaymentSkipped, orderData.Id, refundId, trans.Id);
				return false;
			}
			return true;
		}
		#endregion

		#region CreateRefundOrders

		public virtual async Task CreateRefundOrders(BCRefundsBucket bucket, MappedRefunds existing, CancellationToken cancellationToken = default)
		{
			var currentBinding = GetBinding();
			var currentBindingExt = GetBindingExt<BCBindingExt>();

			var operation = PXSelectJoin<SOOrderType, InnerJoin<SOOrderTypeOperation, On<SOOrderType.orderType, Equal<SOOrderTypeOperation.orderType>, And<SOOrderType.defaultOperation, Equal<SOOrderTypeOperation.operation>>>>,
			Where<SOOrderType.orderType, Equal<Required<SOOrderType.orderType>>>>.Select(this, currentBindingExt.ReturnOrderType).AsEnumerable().
			Cast<PXResult<SOOrderType, SOOrderTypeOperation>>().FirstOrDefault();
			if (string.IsNullOrWhiteSpace(currentBindingExt.ReasonCode) && operation.GetItem<SOOrderTypeOperation>()?.RequireReasonCode == true)
				throw new PXException(BigCommerceMessages.ReasonCodeRequired);

			SalesOrder origOrder = bucket.Refunds.Local;
			OrderData orderData = bucket.Refunds.Extern;
			List<OrderRefund> refunds = orderData.Refunds;
			origOrder.RefundOrders = new List<SalesOrder>();
			var branch = existing.Local.FinancialSettings.Branch;

			var salesOrderDetails = PXSelect<BCSyncDetail, Where<BCSyncDetail.syncID, Equal<Required<BCSyncDetail.syncID>>,
				And<BCSyncDetail.entityType, Equal<Required<BCSyncDetail.entityType>>>>>.Select(this, bucket.Refunds.ParentID, BCEntitiesAttribute.TaxSynchronization);

			foreach (OrderRefund data in refunds)
			{
				if (data.RefundPayments.All(x => x.IsDeclined == true)) continue;
				SalesOrder impl = new SalesOrder();
				impl.ExternalRef = APIHelper.ReferenceMake(data.Id, currentBinding.BindingName).ValueField();
				//Check if refund is already imported as RC Order
				var existingRC = cbapi.GetAll<SalesOrder>(new SalesOrder()
				{
					OrderType = currentBindingExt.ReturnOrderType.SearchField(),
					ExternalRef = impl.ExternalRef.Value.SearchField(),
					Details = new List<SalesOrderDetail>() { new SalesOrderDetail() { InventoryID = new StringReturn() } },
					DiscountDetails = new List<SalesOrdersDiscountDetails>() { new SalesOrdersDiscountDetails() { ExternalDiscountCode = new StringReturn() } }
				},
				filters: GetFilter(Operation.EntityType).LocalFiltersRows.Cast<PXFilterRow>(), cancellationToken: cancellationToken);
				if (existingRC.Count() > 1)
				{
					throw new PXException(BCMessages.MultipleEntitiesWithUniqueField, BCCaptions.SyncDirectionImport,
						  Connector.GetEntities().First(e => e.EntityType == Operation.EntityType).EntityName, data.Id.ToString());
				}
				var presentCROrder = existingRC?.FirstOrDefault();
				// skip refunds that were adjusted  before order completion
				if (OrderRefundShouldBeSkipped(existing, data, presentCROrder))
				{
					continue;
				}

				impl.Id = presentCROrder?.Id;

				origOrder.RefundOrders.Add(impl);

				impl.RefundID = data.Id.ToString();
				impl.OrderType = currentBindingExt.ReturnOrderType.ValueField();
				impl.FinancialSettings = new FinancialSettings();
				impl.FinancialSettings.Branch = branch;

				if (existing != null && existing.Local?.Payments?.Count > 0)
				{
					impl.PaymentMethod = existing.Local?.Payments.First().PaymentMethod;
					impl.CashAccount = existing.Local?.Payments.First().CashAccount;
				}

				var date = data.DateCreated.ToDate(PXTimeZoneInfo.FindSystemTimeZoneById(currentBindingExt.OrderTimeZone));
				if (date.HasValue)
					impl.Date = (new DateTime(date.Value.Date.Ticks)).ValueField();
				impl.RequestedOn = impl.Date;
				impl.CustomerOrder = orderData.Id.ToString().ValueField();
				impl.CustomerID = existing.Local.CustomerID;
				impl.CurrencyID = existing.Local.CurrencyID;
				impl.LocationID = existing.Local.LocationID;
				impl.ExternalRef = APIHelper.ReferenceMake(data.Id, currentBinding.BindingName).ValueField();
				var description = PXMessages.LocalizeFormat(BigCommerceMessages.OrderDescription, currentBinding.BindingName, orderData.Id.ToString(), orderData.Status.ToString());
				impl.Description = description.ValueField();
				impl.Details = new List<SalesOrderDetail>();
				impl.Totals = new Totals();
				impl.Totals.OverrideFreightAmount = existing.Local.Totals?.OverrideFreightAmount;
				List<RefundedItem> refundItems = data.RefundedItems;
				decimal shippingrefundAmt = refundItems?.Where(x => x.ItemType == RefundItemType.Shipping || x.ItemType == RefundItemType.Handling)?.Sum(x => (x.RequestedAmount) ?? 0m) ?? 0m;

				impl.ShipVia = existing.Local.ShipVia;
				impl.ShippingSettings = new ShippingSettings();
				impl.ShippingSettings.ShippingTerms = existing.Local.ShippingSettings?.ShippingTerms;
				impl.ShippingSettings.ShippingZone = existing.Local.ShippingSettings?.ShippingZone;
				if ((existing.Local.Totals?.Freight?.Value == null || existing.Local.Totals?.Freight?.Value == 0) && existing.Local.Totals?.PremiumFreight?.Value > 0)
				{
					if (shippingrefundAmt > existing.Local.Totals?.PremiumFreight?.Value) throw new PXException(BCMessages.RefundShippingFeeInvalid, shippingrefundAmt, existing.Local.Totals?.PremiumFreight?.Value);
					impl.Totals.PremiumFreight = shippingrefundAmt.ValueField();
				}
				else
				{
					if (shippingrefundAmt > existing.Local.Totals?.Freight?.Value) throw new PXException(BCMessages.RefundShippingFeeInvalid, shippingrefundAmt, existing.Local.Totals?.Freight?.Value);
					impl.Totals.Freight = shippingrefundAmt.ValueField();
				}
				var totalOrderRefundAmout = refundItems?.Where(x => x.ItemType == RefundItemType.Order)?.Sum(y => (y.RequestedAmount)) ?? 0;
				//Add orderAdjustments
				if (totalOrderRefundAmout != 0)
				{
					var detail = InsertRefundAmountItem(totalOrderRefundAmout, branch);
					if (presentCROrder?.Details != null)
						presentCROrder?.Details.FirstOrDefault(x => x.InventoryID.Value == detail.InventoryID.Value).With(e => detail.Id = e.Id);
					impl.Details.Add(detail);
				}

				#region ShipTo & BillTo Addresses
				impl.BillToAddressOverride = existing.Local.BillToAddressOverride;
				impl.BillToAddress = new Core.API.Address();
				impl.BillToAddress.AddressLine1 = existing.Local.BillToAddress.AddressLine1;
				impl.BillToAddress.AddressLine2 = existing.Local.BillToAddress.AddressLine2;
				impl.BillToAddress.City = existing.Local.BillToAddress.City;
				impl.BillToAddress.Country = existing.Local.BillToAddress.Country;
				impl.BillToAddress.PostalCode = existing.Local.BillToAddress.PostalCode;
				impl.BillToAddress.State = existing.Local.BillToAddress.State;

				impl.BillToContactOverride = existing.Local.BillToContactOverride;
				impl.BillToContact = new Core.API.DocContact();
				impl.BillToContact.Attention = existing.Local.BillToContact.Attention;
				impl.BillToContact.BusinessName = existing.Local.BillToContact.BusinessName;
				impl.BillToContact.Email = existing.Local.BillToContact.Email;
				impl.BillToContact.Phone1 = existing.Local.BillToContact.Phone1;

				impl.ShipToAddressOverride = existing.Local.ShipToAddressOverride;
				impl.ShipToAddress = new Core.API.Address();
				impl.ShipToAddress.AddressLine1 = existing.Local.ShipToAddress.AddressLine1;
				impl.ShipToAddress.AddressLine2 = existing.Local.ShipToAddress.AddressLine2;
				impl.ShipToAddress.City = existing.Local.ShipToAddress.City;
				impl.ShipToAddress.Country = existing.Local.ShipToAddress.Country;
				impl.ShipToAddress.PostalCode = existing.Local.ShipToAddress.PostalCode;
				impl.ShipToAddress.State = existing.Local.ShipToAddress.State;

				impl.ShipToContactOverride = existing.Local.ShipToContactOverride;
				impl.ShipToContact = new Core.API.DocContact();
				impl.ShipToContact.Attention = existing.Local.ShipToContact.Attention;
				impl.ShipToContact.BusinessName = existing.Local.ShipToContact.BusinessName;
				impl.ShipToContact.Email = existing.Local.ShipToContact.Email;
				impl.ShipToContact.Phone1 = existing.Local.ShipToContact.Phone1;
				#endregion

				#region Taxes	
				impl.TaxDetails = new List<TaxDetail>();

				if (salesOrderDetails.Count() > 0
					&& salesOrderDetails.FirstOrDefault()?.GetItem<BCSyncDetail>().ExternID == BCObjectsConstants.BCSyncDetailTaxSynced)
				{
					if (data.TotalTax > 0 && orderData.Taxes.Count > 0)// if acumatica original SO has tax and Refunds has tax then process tax
					{
						impl.IsTaxValid = true.ValueField();
						string taxType = GetHelper<BCHelper>().DetermineTaxType(orderData.Taxes.Select(i => i.Name).ToList());
						foreach (OrdersTaxData tax in orderData.Taxes)
						{
							string taxName = GetHelper<BCHelper>().ProcessTaxName(currentBindingExt, tax.Name, taxType);
							decimal taxable = 0m;
							TaxDetail inserted = impl.TaxDetails.FirstOrDefault(i => i.TaxID.Value?.Equals(taxName, StringComparison.InvariantCultureIgnoreCase) == true);
							var shippingItems = refundItems.Where(x => (x.ItemType == RefundItemType.Shipping || x.ItemType == RefundItemType.Handling));
							if (tax.LineItemType == BCConstants.giftWrapping)
							{
								// to get taxable amount for giftwrap Note: single orderline with more than one quantity will have same quantity giftwrap and each gift wrap can be same or different
								var origProduct = orderData.OrderProducts.FirstOrDefault(i => i.Id == tax.OrderProductId);
								if (origProduct != null)
									taxable = (origProduct.WrappingCostExcludingTax / origProduct.Quantity) * data.RefundedItems.FirstOrDefault(r => r.ItemType == RefundItemType.GiftWrapping && r.ItemId == origProduct.Id)?.Quantity ?? 0;


							}
							else
							{
								var trefundItems = refundItems.Where(y => y.ItemType == RefundItemType.Product && y.ItemId == tax.OrderProductId);
								var quantity = trefundItems?.Sum(x => x.Quantity) ?? 0;
								var originalOrderProduct = orderData.OrderProducts.FirstOrDefault(i => i.Id == tax.OrderProductId);
								taxable = CalculateTaxableRefundAmount(originalOrderProduct, shippingItems, quantity, tax.LineItemType);

							}
							var taxAmount = await GetHelper<BCHelper>().RoundToStoreSetting(taxable * tax.Rate / 100);
							if (inserted == null)
							{
								impl.TaxDetails.Add(new TaxDetail()
								{
									TaxID = taxName.ValueField(),
									TaxAmount = taxAmount.ValueField(),
									TaxRate = tax.Rate.ValueField(),
									TaxableAmount = taxable.ValueField()
								});
							}
							else if (inserted.TaxAmount != null)
							{
								inserted.TaxAmount.Value += taxAmount;
								inserted.TaxableAmount.Value += taxable;
							}
						}
						// to solve the problem  of rounding
						if (impl.TaxDetails.Sum(x => x.TaxAmount.Value ?? 0) > data.TotalTax && impl.TaxDetails.Count > 0)
						{
							var amounttoAdjust = impl.TaxDetails.Sum(x => x.TaxAmount.Value ?? 0) - data.TotalTax;
							var taxdetails = impl.TaxDetails.FirstOrDefault(x => x.TaxAmount.Value >= amounttoAdjust);
							if (taxdetails != null)
								taxdetails.TaxAmount.Value -= amounttoAdjust;
						}
					}
					else
					{
						if (existing.Local.TaxDetails?.Count > 0 && data.TotalTax > 0 && orderData.Taxes.Count == 0)// In case of full refunds order taxes count become zero
						{
							impl.IsTaxValid = true.ValueField();
							var taxRateTotal = existing.Local.TaxDetails.Sum(x => x.TaxRate.Value);
							foreach (TaxDetail tax in existing.Local.TaxDetails)
							{
								TaxDetail inserted = impl.TaxDetails.FirstOrDefault(i => i.TaxID.Value?.Equals(tax.TaxID.Value, StringComparison.InvariantCultureIgnoreCase) == true);
								var taxable = data.TotalAmount - data.TotalTax;
								var taxAmount = await GetHelper<BCHelper>().RoundToStoreSetting((decimal)(data.TotalTax * tax.TaxRate.Value / taxRateTotal)); // just get tax amount based on totaltax inrefund and taxrate from Acumatica salesorder
								if (inserted == null)
								{
									impl.TaxDetails.Add(new TaxDetail()
									{
										TaxID = tax.TaxID,
										TaxAmount = taxAmount.ValueField(),
										TaxRate = tax.TaxRate,
										TaxableAmount = tax.TaxableAmount
									});
								}
								else if (inserted.TaxAmount != null)
								{
									inserted.TaxAmount.Value += taxAmount;
									inserted.TaxableAmount.Value += taxable;
								}
							}
						}
					}
				}

				if (impl.TaxDetails?.Count > 0)
				{
					impl.FinancialSettings.OverrideTaxZone = existing.Local.FinancialSettings.OverrideTaxZone;
					impl.FinancialSettings.CustomerTaxZone = existing.Local.FinancialSettings.CustomerTaxZone;
				}
				//Set tax calculation to default mode
				impl.TaxCalcMode = existing.Local.TaxCalcMode;

				String[] tooLongTaxIDs = ((impl.TaxDetails ?? new List<TaxDetail>()).Select(x => x.TaxID?.Value).Where(x => (x?.Length ?? 0) > PX.Objects.TX.Tax.taxID.Length).ToArray());
				if (tooLongTaxIDs != null && tooLongTaxIDs.Length > 0)
				{
					throw new PXException(PX.Commerce.Objects.BCObjectsMessages.CannotFindSaveTaxIDs, String.Join(",", tooLongTaxIDs), PX.Objects.TX.Tax.taxID.Length, currentBindingExt.TaxCategorySubstitutionListID);
				}



				#endregion

				#region Discounts
				Dictionary<string, decimal?> totalDiscount = null;
				if (refundItems?.Where(x => x.ItemType == RefundItemType.Product || x.ItemType == RefundItemType.GiftWrapping).Count() > 0)
				{
					totalDiscount = AddSOLine(bucket, impl, data, existing, branch, presentCROrder);
				}



				if (currentBindingExt.PostDiscounts == BCPostDiscountAttribute.DocumentDiscount && totalDiscount != null && totalDiscount?.Count > 0)
				{
					#region Coupons
					impl.DisableAutomaticDiscountUpdate = true.ValueField();
					impl.DiscountDetails = new List<SalesOrdersDiscountDetails>();
					foreach (OrdersCouponData couponData in orderData.OrdersCoupons)
					{
						SalesOrdersDiscountDetails disDetail = new SalesOrdersDiscountDetails();
						disDetail.ExternalDiscountCode = couponData.CouponCode.ValueField();
						disDetail.Description = string.Format(BCMessages.DiscountCouponDesctiption, couponData.CouponType.GetDescription(), couponData.Discount)?.ValueField();

						if (currentBindingExt.PostDiscounts == BCPostDiscountAttribute.DocumentDiscount)
							disDetail.DiscountAmount = totalDiscount.GetValueOrDefault_<decimal>(couponData.CouponCode, 0).ValueField();
						else disDetail.DiscountAmount = 0m.ValueField();

						impl.DiscountDetails.Add(disDetail);
					}
					#endregion


					SalesOrdersDiscountDetails detail = new SalesOrdersDiscountDetails();
					detail.Type = PX.Objects.Common.Discount.DiscountType.ExternalDocument.ValueField();
					detail.ExternalDiscountCode = BCCaptions.Manual.ValueField();
					detail.DiscountAmount = (totalDiscount.GetValueOrDefault_<decimal>(BCCaptions.Manual, 0)).ValueField();
					impl.DiscountDetails.Add(detail);
				}

				#endregion
				#region CR Payment

				impl.Payments = new List<SalesOrderPayment>();
				var payments = origOrder.Payment?.Where(x => x.CreateWithRC == true && x.TransactionID.KeySplit(0) == data.Id.ToString())?.ToList();
				if (payments != null)
					foreach (var transaction in payments)
					{
						SalesOrderPayment refundPayment = new SalesOrderPayment();
						refundPayment.DocType = PX.Objects.AR.Messages.Refund.ValueField();
						refundPayment.ExternalRef = transaction.ExternalRef;
						refundPayment.PaymentRef = transaction.PaymentRef;
						refundPayment.ApplicationDate = transaction.ApplicationDate;
						refundPayment.Description = transaction.Description;
						refundPayment.PaymentAmount = transaction.PaymentAmount;
						refundPayment.Hold = transaction.Hold ?? false.ValueField();
						refundPayment.Refund = false.ValueField();
						refundPayment.ValidateCCRefundOrigTransaction = false.ValueField();
						refundPayment.AppliedToOrder = transaction.PaymentAmount;
						refundPayment.OrigTransactionNbr = transaction.OrigTransaction;
						refundPayment.ProcessingCenterID = transaction.ProcessingCenterID;
						if (transaction.CreditCardTransactionInfo?.Count > 0)
						{
							refundPayment.CreditCardTransactionInfo = new List<SalesOrderCreditCardTransactionDetail>();
							foreach (var detail in transaction.CreditCardTransactionInfo)
							{
								SalesOrderCreditCardTransactionDetail creditCardDetail = new SalesOrderCreditCardTransactionDetail();
								creditCardDetail.TranNbr = detail.TranNbr;
								creditCardDetail.TranDate = detail.TranDate;
								creditCardDetail.TranType = detail.TranType;
								creditCardDetail.ExtProfileId = detail.ExtProfileId;
								refundPayment.CreditCardTransactionInfo.Add(creditCardDetail);
							}
						}
						refundPayment.Currency = transaction.CurrencyID;
						refundPayment.CashAccount = transaction.CashAccount;
						refundPayment.PaymentMethod = transaction.PaymentMethod;
						refundPayment.NoteID = transaction.NoteID;
						refundPayment.NeedRelease = transaction.NeedRelease;
						impl.Payments.Add(refundPayment);
						origOrder.Payment.Remove(transaction);
					}
				#endregion
				#region Adjust for Existing
				if (presentCROrder != null)
				{
					//Keep the same order Type
					impl.OrderType = presentCROrder.OrderType;

					//if Order already exists assign ID's 
					presentCROrder.DiscountDetails?.ForEach(e => impl.DiscountDetails?.FirstOrDefault(n => n.ExternalDiscountCode.Value == e.ExternalDiscountCode.Value).With(n => n.Id = e.Id));

					impl.DiscountDetails?.AddRange(presentCROrder.DiscountDetails == null ? Enumerable.Empty<SalesOrdersDiscountDetails>()
					: presentCROrder.DiscountDetails.Where(e => impl.DiscountDetails == null || !impl.DiscountDetails.Any(n => e.Id == n.Id)).Select(n => new SalesOrdersDiscountDetails() { Id = n.Id, Delete = true }));
				}
				#endregion
			}
		}

		/// <summary>
		/// The method checks whether it needs to skip processing of a given refund. Skips refunds that were adjusted  before order completion
		/// </summary>
		/// <param name="existing"></param>
		/// <param name="currentRefund"></param>
		/// <param name="presentCROrder"></param>
		/// <returns></returns>
		public virtual bool OrderRefundShouldBeSkipped(MappedRefunds existing, OrderRefund currentBigCommerceRefund, SalesOrder alreadyImportedRCOrder)
		{
			if (existing != null)
			{
				bool customerRefundsAreImported = existing?.Details?
					.Any(d => d.EntityType == BCEntitiesAttribute.Payment && d.ExternID.KeySplit(0) == currentBigCommerceRefund.Id.ToString()) == true;

				if (customerRefundsAreImported && alreadyImportedRCOrder == null)
				{
					var NoteIdsOfCustomerRefunds = existing.Details
						.Where(d => d.EntityType == BCEntitiesAttribute.Payment && d.ExternID.KeySplit(0) == currentBigCommerceRefund.Id.ToString())
						.Select(d => d.LocalID);
					// customer refunds can be deleted, information about it will still present in the BCSyncDetail
					bool CustomerRefundDoesNotExist = SelectFrom<ARRegister>.Where<ARRegister.noteID.IsIn<@P.AsGuid>>.View.Select(this, NoteIdsOfCustomerRefunds).Any() == false;

					if (CustomerRefundDoesNotExist)
					{
						return true;
					}
				}
				if (existing.Local.ExternalRefundRef?.Value != null)
				{
					if (existing.Local.ExternalRefundRef.Value.Split(new char[] { ';' }).Contains(currentBigCommerceRefund.Id.ToString())) return true;
				}
			}
			return false;
		}

		public virtual Dictionary<string, decimal?> AddSOLine(BCRefundsBucket bucket, SalesOrder impl, OrderRefund data, MappedRefunds existing, StringValue branch, SalesOrder presentCROrder)
		{
			var currentBinding = GetBinding();
			var currentBindingExt = GetBindingExt<BCBindingExt>();
			//Get the SOShipLine data in the original order
			var shipLines = PXSelect<SOShipLine, Where<SOShipLine.origOrderType, Equal<Required<SOShipLine.origOrderType>>, And<SOShipLine.origOrderNbr, Equal<Required<SOShipLine.origOrderNbr>>>>>
								.Select(this, existing.Local?.OrderType?.Value, existing.Local?.OrderNbr?.Value).RowCast<SOShipLine>()?.ToList();
			OrderData salesOrder = bucket.Refunds.Extern;
			Dictionary<string, decimal?> totaldiscount = new Dictionary<string, decimal?>();

			foreach (var item in data.RefundedItems.Where(x => x.ItemType == RefundItemType.Product || x.ItemType == RefundItemType.GiftWrapping))
			{
				SalesOrderDetail detail = new SalesOrderDetail();
				detail.Branch = branch;
				var productData = salesOrder.OrderProducts.FirstOrDefault(x => x.Id == item.ItemId);
				if (productData == null) throw new PXException(BCMessages.RefundInventoryNotFound, item.ItemId);
				string inventoryCD = null;
				if (item.ItemType == RefundItemType.GiftWrapping)
				{
					var wrapInventoryCD = GetInventoryCDForGiftWrap(out string uom);
					detail.InventoryID = wrapInventoryCD?.ValueField();
					detail.UOM = uom?.ValueField();
					detail.OrderQty = ((decimal)item.Quantity).ValueField();
					detail.UnitPrice = (productData.WrappingCostExcludingTax / productData.Quantity).ValueField();
					detail.ManualPrice = true.ValueField();
					detail.ReasonCode = currentBindingExt.ReasonCode?.ValueField();
					detail.ExternalRef = item.ItemId.ToString().ValueField();
					var soLine = existing.Local.Details.FirstOrDefault(x => x.ExternalRef?.Value != null && x.AssociatedOrderLineNbr?.Value != null && x.ExternalRef?.Value == detail.ExternalRef?.Value.ToString());
					detail.AssociatedOrderLineNbr = soLine?.AssociatedOrderLineNbr;
					detail.GiftMessage = soLine?.GiftMessage;
					SalesOrderDetail matchedLine = presentCROrder?.Details?.FirstOrDefault(x => x.ExternalRef?.Value != null && x.AssociatedOrderLineNbr?.Value != null && x.ExternalRef?.Value == detail.ExternalRef?.Value.ToString());
					impl.Details.Add(detail);

					if (matchedLine != null)
					{
						detail.Id = matchedLine.Id;
					}
				}
				else
				{
					inventoryCD = GetHelper<BCHelper>().GetInventoryCDByExternID(
					   productData.ProductId.ToString(),
					   productData.OptionSetId >= 0 ? productData.VariandId.ToString() : null,
					   productData.Sku,
					   productData.ProductName,
					   productData.ProductType,
					   out string uom,
					   out string alternateID,
					   out string itemStatus);

					var shipline = shipLines?.FirstOrDefault(x => x.OrigLineNbr == existing.Local.Details.FirstOrDefault(y => y.NoteID.Value == bucket.Order.Details.FirstOrDefault(
									d => d.EntityType == BCEntitiesAttribute.OrderLine && item.ItemId.ToString() == d.ExternID)?.LocalID)?.LineNbr?.Value);
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

					decimal discountPerItem = 0;
					if (productData.AppliedDiscounts != null)
					{
						discountPerItem = productData.AppliedDiscounts.Select(p => p.DiscountAmount).Sum() / productData.Quantity;
						detail.DiscountAmount = (discountPerItem * item.Quantity).ValueField();
						foreach (var discount in productData.AppliedDiscounts)
						{
							string key = discount.Code;
							if (discount.Id != "coupon")
								key = BCCaptions.Manual;
							if (totaldiscount.ContainsKey(key))
								totaldiscount[key] = totaldiscount[key].Value + ((discount.DiscountAmount / productData.Quantity) * item.Quantity);
							else
								totaldiscount.Add(key, ((discount.DiscountAmount / productData.Quantity) * item.Quantity));
						}
					}
					if (currentBindingExt.PostDiscounts != BCPostDiscountAttribute.LineDiscount)
						detail.DiscountAmount = 0m.ValueField();
					detail.InventoryID = inventoryCD?.TrimEnd().ValueField();

					if (item.Quantity > productData.Quantity)
						throw new PXException(BCMessages.RefundQuantityGreater);

					detail.OrderQty = ((decimal)item.Quantity).ValueField();
					detail.UOM = uom.ValueField();
					detail.UnitPrice = productData.PriceExcludingTax.ValueField();
					detail.ManualPrice = true.ValueField();
					detail.ReasonCode = currentBindingExt.ReasonCode?.ValueField();
					detail.ExternalRef = item.ItemId.ToString().ValueField();
					detail.AlternateID = alternateID?.ValueField();
					impl.Details.Add(detail);
					DetailInfo matchedDetail = existing?.Details?.FirstOrDefault(d => d.EntityType == BCEntitiesAttribute.OrderLine && item.ItemId.ToString() == d.ExternID.KeySplit(1) && data.Id.ToString() == d.ExternID.KeySplit(0));
					if (matchedDetail != null) detail.Id = matchedDetail.LocalID; //Search by Details
					else if (presentCROrder?.Details != null && presentCROrder.Details.Count > 0) //Serach by Existing line
					{
						SalesOrderDetail matchedLine = presentCROrder.Details.FirstOrDefault(x =>
							(x.ExternalRef?.Value != null && x.ExternalRef?.Value == item.ItemId.ToString())
							||
							(x.InventoryID?.Value == detail.InventoryID?.Value && !impl.Details.Any(y => y.Id == x.Id) && (detail.UOM == null || detail.UOM.Value == x.UOM?.Value)));
						if (matchedLine != null) detail.Id = matchedLine.Id;
					}
				}
			}

			return totaldiscount;
		}
		#endregion

		public override async Task SaveBucketImport(BCRefundsBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
			MappedRefunds obj = bucket.Refunds;
			SalesOrder order = obj.Local;
			try
			{
				obj.ClearDetails();

				if (order.Payment != null)
				{
					List<Tuple<string, string>> addedRefNbr = new List<Tuple<string, string>>();
					foreach (var payment in order.Payment)
					{
						Payment paymentResp = null;
						Guid? localId = payment.NoteID?.Value;
						using (var transaction = await base.WithTransaction(async () =>
						{
							if (payment.VoidCardParameters != null)
							{
								paymentResp = !string.IsNullOrEmpty(payment.ProcessingCenterID?.Value) ? cbapi.Invoke<Payment, VoidCardPayment>(payment, action: payment.VoidCardParameters) : cbapi.Invoke<Payment, VoidPayment>(payment);
								localId = paymentResp.Id;
							}
							else
							{
								foreach (var detail in payment.DocumentsToApply)
								{
									if (addedRefNbr.Any(x => x.Item1 == detail.ReferenceNbr.Value))
									{
										throw new SetSyncStatusException(BCMessages.UnreleasedCRPayment, detail?.ReferenceNbr?.Value, addedRefNbr.FirstOrDefault(x => x.Item1 == detail.ReferenceNbr.Value).Item2);
									}

								}

								if (payment.NoteID?.Value == null)
								{
									paymentResp = cbapi.Put<Payment>(payment);
									localId = paymentResp?.NoteID?.Value;
									foreach (var detail in payment.DocumentsToApply)
									{
										addedRefNbr.Add(new Tuple<string, string>(detail.ReferenceNbr.Value, paymentResp.ReferenceNbr.Value));
									}
								}
							}
						}))
						{
							transaction?.Complete();
						}

						if (payment?.NeedRelease == true && localId.HasValue && paymentResp?.Status?.Value == PX.Objects.AR.Messages.Balanced)
						{
							try
							{
								paymentResp = cbapi.Invoke<Payment, ReleasePayment>(null, localId.Value, ignoreResult: !WebConfig.ParallelProcessingDisabled);
							}
							catch (Exception ex) { LogError(Operation.LogScope(obj), ex); }
						}

						if (!obj.Details.Any(x => x.LocalID == localId))
						{
							obj.AddDetail(BCEntitiesAttribute.Payment, localId, payment.TransactionID.ToString());
						}

					}
				}

				if (order.RefundOrders != null)
				{
					foreach (var refundOrder in order.RefundOrders)
					{
						var details = refundOrder.Details;
						var localID = refundOrder.Id;
						var payments = refundOrder.Payments;
						if (refundOrder.Id == null)
						{
							#region Taxes
							//Logging for taxes
							GetHelper<BCHelper>().LogTaxDetails(obj.SyncID, refundOrder);
							#endregion

							SalesOrder impl = cbapi.Put<SalesOrder>(refundOrder, localID);
							localID = impl.Id;
							payments = SetPaymentsExceptNeedRelease(payments, impl.Payments);
							details = impl.Details;

							#region Taxes
							await GetHelper<BCHelper>().ValidateTaxes(obj.SyncID, impl, refundOrder);
							#endregion
						}

						if (!obj.Details.Any(x => x.LocalID == localID))
						{
							obj.AddDetail(BCEntitiesAttribute.CustomerRefundOrder, localID, refundOrder.RefundID);
						}
						if (details != null)
							foreach (var lineitem in details)
							{
								if (!obj.Details.Any(x => x.LocalID == lineitem.Id))
								{
									RefundedItem detail = null;
									var externRefundItems = obj.Extern.Refunds.FirstOrDefault(x => x.Id.ToString() == refundOrder.RefundID).RefundedItems;
									detail = externRefundItems.FirstOrDefault(x => (x.ItemId.ToString() == lineitem.ExternalRef?.Value && x.ItemType == RefundItemType.Product && lineitem.AssociatedOrderLineNbr?.Value == null)
									|| (lineitem.AssociatedOrderLineNbr?.Value != null && x.ItemId.ToString() == lineitem.ExternalRef?.Value
									&& x.ItemType == RefundItemType.GiftWrapping));
									if (detail?.ItemType == RefundItemType.GiftWrapping)
									{
										if (detail == null) throw new PXException(BCMessages.CannotMapLines);
										obj.AddDetail(BCEntitiesAttribute.GiftWrapOrderLine, lineitem.Id, new object[] { refundOrder.RefundID, detail.ItemId.ToString() }.KeyCombine());
									}
									else
									{
										if (lineitem.InventoryID.Value.Trim() == refundItem.InventoryCD.Trim())
											continue;
										else if (lineitem.InventoryID.Value.Trim() == giftCertificateItem?.Value?.InventoryCD?.Trim())
											obj.AddDetail(BCEntitiesAttribute.OrderLine, lineitem.Id, new object[] { refundOrder.RefundID, lineitem.ExternalRef.Value }.KeyCombine());
										else
										{
											if (detail == null)
												detail = externRefundItems.FirstOrDefault(x => !obj.Details.Any(o => x.ItemId.ToString() == o.ExternID && x.ItemType == RefundItemType.Product)
										&& obj.Extern.OrderProducts.FirstOrDefault(o => o.Id == x.ItemId)?.Sku == lineitem.InventoryID.Value);
											if (detail != null)
												obj.AddDetail(BCEntitiesAttribute.OrderLine, lineitem.Id, new object[] { refundOrder.RefundID, detail.ItemId }.KeyCombine());
											else
												throw new PXException(BCMessages.CannotMapLines);
										}
									}
								}

							}

						if (payments?.Count() > 0)
						{
							foreach (var payment in payments)
							{
								if (string.IsNullOrEmpty(payment.ExternalRef?.Value) && !string.IsNullOrEmpty(payment.ReferenceNbr?.Value))
								{
									var arPayment = ARPayment.PK.Find(this, ARPaymentType.Refund, payment.ReferenceNbr.Value);
									payment.ExternalRef = arPayment?.ExternalRef.ValueField();
									payment.NoteID = arPayment?.NoteID.ValueField();
									if (payment.NeedRelease && payment.NoteID != null && arPayment.Status == ARDocStatus.Balanced)
									{
										try
										{
											cbapi.Invoke<Payment, ReleasePayment>(null, payment.NoteID.Value, ignoreResult: !WebConfig.ParallelProcessingDisabled);
										}
										catch (Exception ex) { LogError(Operation.LogScope(bucket.Refunds), ex); }
									}
								}
								if (payment.NoteID?.Value != null && !obj.Details.Any(x => x.LocalID == payment.NoteID?.Value))
								{
									obj.AddDetail(BCEntitiesAttribute.Payment, payment.NoteID?.Value, new object[] { refundOrder.RefundID, payment.ExternalRef?.Value }.KeyCombine());
								}
							}
						}
					}

				}
				UpdateStatus(obj, operation);
				if (order.EditSO)
				{
					bucket.Order.ExternTimeStamp = DateTime.MaxValue;
					EnsureStatus(bucket.Order, SyncDirection.Import, Conditions.Resync);
				}
				else
					bucket.Order = null;

			}
			catch (SetSyncStatusException)
			{
				throw;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// This method sets <paramref name="existingPayments"/> to <paramref name="cbapiPayments"/> excluding the field <see cref="SalesOrderPayment.NeedRelease"/>.
		/// </summary>
		/// <param name="existingPayments"></param>
		/// <param name="cbapiPayments"></param>
		/// <returns>Updated List of SalesOrderPayment.</returns>
		public virtual List<SalesOrderPayment> SetPaymentsExceptNeedRelease(List<SalesOrderPayment> existingPayments, List<SalesOrderPayment> cbapiPayments)
		{
			var listNeedToRelease = existingPayments.Select(x => x.NeedRelease).ToList();
			existingPayments = cbapiPayments;
			for (int i = 0; i < cbapiPayments?.Count; i++)
				existingPayments[i].NeedRelease = listNeedToRelease[i];

			return existingPayments;
		}
		#endregion

		#region Pull
		public override async Task<MappedRefunds> PullEntity(string externID, string externalInfo, CancellationToken cancellationToken = default)
		{
			return null;
		}

		public override async Task<MappedRefunds> PullEntity(Guid? localID, Dictionary<string, object> externalInfo, CancellationToken cancellationToken = default)
		{
			return null;
		}

		#endregion
	}
}

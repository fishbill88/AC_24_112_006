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

using System;
using System.Linq;
using System.Collections.Generic;
using PX.CCProcessingBase.Interfaces.V2;
using PX.Data;
using PX.Objects.AR;
using PX.Commerce.Shopify.ShopifyPayments.Extensions;
using PX.Commerce.Shopify.API.REST;
using PX.Commerce.Core;
using System.Threading.Tasks;
using PX.Concurrency;

namespace PX.Commerce.Shopify.ShopifyPayments
{
	public class ShopifyTransactionGetter : ShopifyPaymentsDataProvider, ICCTransactionGetter
	{
		public ShopifyTransactionGetter(IShopifyRestClient restClient) : base(restClient)
		{
		}

		public TransactionData GetTransaction(string transactionId)
		{
			try
			{
				PXTrace.WriteVerbose("Get transaction details by TransactionId: {TransactionId}.", transactionId);

				if (string.IsNullOrEmpty(transactionId))
				{
					throw new CCProcessingException(PX.CCProcessingBase.Messages.PaymentTransactionIDEmpty);
				}

				string docType;
				string docRefNbr;
				SlotARPaymentKeys.GetKeys(out docType, out docRefNbr, true);

				if (docType == null || docRefNbr == null)
				{
					throw new PXException(ShopifyPluginMessages.TheExternalOrderIDCouldNotBeCalculatedBecauseTheARPaymentKeysAreMissingInTheMethodX, nameof(ShopifyTransactionGetter.GetTransaction));
				}

				PXGraph graph = PXGraph.CreateInstance<PXGraph>();

				ARPayment arPayment = null;
				BCSyncStatus bcSyncStatus = GetOrderBCSyncStatus(graph, docType, docRefNbr, out arPayment);
				string orderID = bcSyncStatus?.ExternID;

				if (orderID == null)
				{
					throw new PXException(ShopifyPluginMessages.TheExternalOrderIDCouldNotBeFound);
				}

				long longTransactionID;
				if (long.TryParse(transactionId, out longTransactionID) == false)
				{
					throw new PXException(ShopifyPluginMessages.InvalidProcessingCenterTransactionNumberXTheXExpectsItToBeAnIntegerNumber, transactionId, ShopifyPluginMessages.APIPluginDisplayName);
				}

				OrderTransaction transaction = null;
				OrderTransaction newestTransaction = null;
				var key = Guid.NewGuid();
				graph.LongOperationManager.StartAsyncOperation(key, async cancellationToken =>
				{
					// to force the code to run asynchronously and keep UI responsive.
					//In some case it runs synchronously especially when using IAsyncEnumerable
					await Task.Yield();
					foreach (OrderTransaction tran in (await GetOrderTransactions(orderID)).OrderByDescending(t => t.DateModifiedAt))
					{
						if (docType == ARPaymentType.Prepayment
							&& tran.Kind == TransactionType.Refund
							&& tran.Id != longTransactionID)
						{
							continue;
						}

						if (newestTransaction == null
							&& (tran.Id == longTransactionID || tran.ParentId == longTransactionID))
						{
							newestTransaction = tran;
						}

						if (tran.Id == longTransactionID)
						{
							transaction = tran;
							break;
						}
					}
				});
				PXLongOperation.WaitCompletion(key);
				if (transaction == null)
				{
					throw new PXException(ShopifyPluginMessages.TheExternalCardTransactionWithIdXCouldNotBeFound, transactionId);
				}

				transaction = newestTransaction;

				TransactionData result = new TransactionData()
				{
					Amount = transaction.Amount ?? 0,
					CustomerId = null, //tranDetails.profile?.customerProfileId,
					PaymentId = null, //tranDetails.profile?.customerPaymentProfileId,
					CardNumber = null, //((creditCardMaskedType)tranDetails.payment?.Item)?.cardNumber,
					DocNum = transaction.OrderId.ToString(),
					TranID = transaction.Id.ToString(),
					AuthCode = transaction.Authorization,
					SubmitTime = (DateTime)transaction.DateModifiedAt.Value.ToUniversalTime(),
					CcvVerificationStatus = GetCcvVerificationStatusFromErrorCode(transaction.ErrorCode),
					TranStatus = GetCCTranStatus(transaction),
					ResponseReasonText = transaction.Message,
					ResponseReasonCode = 0,
				};

				SetTranType(result, transaction.Kind, transaction.Status);

				//if (result.TranType == PX.CCProcessingBase.Interfaces.V2.CCTranType.Credit 
				//	&& tranDetails.refTransId != null && result.CustomerId == null)
				//{
				//	var refResult = GetTransaction(tranDetails.refTransId);
				//	if (refResult.CustomerId != null && refResult.PaymentId != null)
				//	{
				//		result.PaymentId = refResult.PaymentId;
				//		result.CustomerId = refResult.CustomerId;
				//	}
				//}

				if (result.TranType == CCTranType.Credit
					|| result.TranType == CCTranType.Void
					|| result.TranType == CCTranType.PriorAuthorizedCapture)
				{
					result.RefTranID = transaction.ParentId.ToString();

					if (transaction.Kind == TransactionType.Refund)
					{
						// This is because the Shopify connector doesn't create a CCProcTran record with the Capture transactionId
						// for the case when the payment is captured in Shopify before being imported in Acumatica;
						// it only creates a CCProcTran record with the Tran. Type "Capture Authorized"
						// but with the Authorization transactionId.

						ExternalTransaction externalTransaction = PXSelect<ExternalTransaction,
							Where<ExternalTransaction.docType, Equal<Required<ExternalTransaction.docType>>,
							And<ExternalTransaction.refNbr, Equal<Required<ExternalTransaction.refNbr>>,
							And<ExternalTransaction.tranNumber, Equal<Required<ExternalTransaction.tranNumber>>,
							And<ExternalTransaction.active, Equal<True>>>>>>
							.Select(graph, docType, docRefNbr, result.RefTranID);

						if (externalTransaction == null)
						{
							OrderTransaction capture = GetOrderSingleTransaction(orderID, transaction.ParentId.ToString()).Result;
							if (capture != null)
							{
								// This would match the Authorization transactionId of the record created by the Shopify connector.
								result.RefTranID = capture.ParentId.ToString();
							}
						}
					}
				}

				if (result.TranType == CCTranType.AuthorizeOnly
					&& result.TranStatus != CCTranStatus.Expired)
				{
					result.ExpireAfterDays = ShopifyPluginHelper.AuthorizationValidPeriodDays;
				}

				PXTrace.WriteVerbose("Processing center returns CustomerId: {CustomerId}, Amount: {Amount}.", result.CustomerId, result.Amount);
				return result;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				throw;
			}
		}

		public IEnumerable<TransactionData> GetTransactionsByCustomer(string customerProfileId, TransactionSearchParams searchParams = null)
		{
			throw new PXException(ShopifyPluginMessages.TheMethodXIsNotImplementedInTheX, nameof(ICCTransactionGetter) + "." + nameof(GetTransactionsByCustomer), ShopifyPluginMessages.APIPluginDisplayName);
		}

		public IEnumerable<TransactionData> GetUnsettledTransactions(TransactionSearchParams searchParams = null)
		{
			try
			{
				PXTrace.WriteVerbose("Get unsettled transactions from Processing center.");

				string docType;
				string docRefNbr;
				SlotARPaymentKeys.GetKeys(out docType, out docRefNbr, true);

				if (docType == null || docRefNbr == null)
				{
					throw new PXException(ShopifyPluginMessages.TheExternalOrderIDCouldNotBeCalculatedBecauseTheARPaymentKeysAreMissingInTheMethodX, nameof(ShopifyTransactionGetter.GetUnsettledTransactions));
				}

				PXGraph graph = PXGraph.CreateInstance<PXGraph>();

				ARPayment arPayment = null;
				BCSyncStatus bcSyncStatus = GetOrderBCSyncStatus(graph, docType, docRefNbr, out arPayment);
				string orderID = bcSyncStatus?.ExternID;

				if (orderID == null)
				{
					throw new PXException(ShopifyPluginMessages.TheExternalOrderIDCouldNotBeFound);
				}
				var key = Guid.NewGuid();
				IOrderedEnumerable<OrderTransaction> transactions = null;
				graph.LongOperationManager.StartAsyncOperation(key, async cancellationToken =>
				{
					transactions = (await GetOrderTransactions(orderID)).OrderByDescending(t => t.DateModifiedAt);
				});
				PXLongOperation.WaitCompletion(key);
				IEnumerable<TransactionData> output = GetTransactionList(transactions);

				int cnt = transactions != null ? transactions.ToList().Count : 0;
				PXTrace.WriteVerbose("Processing center returns {TotalRows} records.", cnt);
				return output;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				throw;
			}
		}

		private IEnumerable<TransactionData> GetTransactionList(IOrderedEnumerable<OrderTransaction> transactions)
		{
			if (transactions != null)
			{
				foreach (OrderTransaction transaction in transactions)
				{
					TransactionData data = new TransactionData();
					data.Amount = transaction.Amount ?? 0;
					data.DocNum = transaction.OrderId.ToString();
					data.TranID = transaction.Id.ToString();
					data.CustomerId = null;
					data.PaymentId = null;
					data.SubmitTime = (DateTime)transaction.DateModifiedAt.Value.ToUniversalTime();
					yield return data;
				}
			}
		}
	}
}

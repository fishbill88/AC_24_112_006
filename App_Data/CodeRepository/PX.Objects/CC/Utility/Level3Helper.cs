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

using PX.Data;
using PX.Objects.AR;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Wrappers;
using PX.Objects.CR;
using PX.Objects.Extensions.PaymentTransaction;
using PX.Objects.IN;

using ARCashSale = PX.Objects.AR.Standalone.ARCashSale;

namespace PX.Objects.CC.Utility
{
	internal static class Level3Helper
	{
		internal static void SetL3StatusExternalTransaction(ExternalTransaction externalTransaction, L3TranStatus? l3Status, string l3Error)
		{
			string inputL3Status = V2Converter.GetL3Status(l3Status ?? L3TranStatus.NotApplicable);
			switch (externalTransaction.L3Status)
			{
				case null:
				case ExtTransactionL3StatusCode.NotApplicable:
					externalTransaction.L3Status = ExtTransactionL3StatusCode.Pending;
					break;
				case ExtTransactionL3StatusCode.Pending:
				case ExtTransactionL3StatusCode.Failed:
					externalTransaction.L3Status = l3Status != null ? inputL3Status : ExtTransactionL3StatusCode.Pending;
					externalTransaction.L3Error = l3Error;
					break;
				case ExtTransactionL3StatusCode.ResendFailed:
					if (inputL3Status == ExtTransactionL3StatusCode.Rejected)
					{
						externalTransaction.L3Status = ExtTransactionL3StatusCode.ResendRejected;
						externalTransaction.L3Error = null;
					}
					else if (inputL3Status == ExtTransactionL3StatusCode.Failed)
					{
						externalTransaction.L3Status = ExtTransactionL3StatusCode.ResendFailed;
						externalTransaction.L3Error = l3Error;
					}
					else if (inputL3Status == ExtTransactionL3StatusCode.Sent)
					{
						externalTransaction.L3Status = ExtTransactionL3StatusCode.Sent;
						externalTransaction.L3Error = null;
					}
					break;
				case ExtTransactionL3StatusCode.Sent:
					if (inputL3Status == ExtTransactionL3StatusCode.Rejected)
					{
						externalTransaction.L3Status = ExtTransactionL3StatusCode.Rejected;
					}
					else if (inputL3Status == ExtTransactionL3StatusCode.Sent)
					{
						externalTransaction.L3Status = ExtTransactionL3StatusCode.Sent;
						externalTransaction.L3Error = null;
					}
					else if (inputL3Status == ExtTransactionL3StatusCode.Failed)
					{
						externalTransaction.L3Status = ExtTransactionL3StatusCode.ResendFailed;
						externalTransaction.L3Error = l3Error;
					}
					break;
				case ExtTransactionL3StatusCode.Rejected:
					if (inputL3Status == ExtTransactionL3StatusCode.Sent)
					{
						externalTransaction.L3Status = ExtTransactionL3StatusCode.Sent;
						externalTransaction.L3Error = null;
					}
					break;
				default:
					if (inputL3Status == ExtTransactionL3StatusCode.Sent)
					{
						externalTransaction.L3Status = ExtTransactionL3StatusCode.Sent;
						externalTransaction.L3Error = null;
					}
					else if (inputL3Status == ExtTransactionL3StatusCode.Rejected)
					{
						externalTransaction.L3Status = ExtTransactionL3StatusCode.Rejected;
					}
					break;
			}
		}

		internal static void FillL3Header(ARPaymentEntry arPaymentEntry, Payment payment, decimal taxAmount)
		{
			Customer customer = arPaymentEntry.customer.Current;
			Location customerLocation = arPaymentEntry.location.Current;
			GL.Branch branch = ARPayment.FK.Branch.FindParent(arPaymentEntry, arPaymentEntry.CurrentDocument.Current);

			FillL3Header(arPaymentEntry, payment, taxAmount, customer, customerLocation, branch);
		}

		internal static void FillL3Header(ARCashSaleEntry arCashSaleEntry, Payment payment, decimal taxAmount)
		{
			Customer customer = arCashSaleEntry.customer.Current;
			Location customerLocation = arCashSaleEntry.location.Current;			
			GL.Branch branch = ARCashSale.FK.Branch.FindParent(arCashSaleEntry, arCashSaleEntry.CurrentDocument.Current);

			FillL3Header(arCashSaleEntry, payment, taxAmount, customer, customerLocation, branch);
		}

		private static void FillL3Header(PXGraph graph, Payment payment, decimal taxAmount,
			Customer customer, Location customerLocation, GL.Branch branch)
		{
			Address customerLocationAddress = Location.FK.Address.FindParent(graph, customerLocation);
			payment.L3Data.DestinationCountryCode = customerLocationAddress != null
				? ISO3166.Country.List.FirstOrDefault(c => c.TwoLetterCode.Equals(customerLocationAddress.CountryID)).NumericCode
				: null;

			ExternalTransaction externalTransaction = ExternalTransaction.PK.Find(graph, payment.CCActualExternalTransactionID);
			payment.L3Data.TransactionId = externalTransaction.TranNumber;
			payment.L3Data.CardType = CardType.GetCardTypeEnumByCode(externalTransaction.CardType);

			payment.L3Data.CustomerVatRegistration = customerLocation?.TaxRegistrationID ?? customer?.TaxRegistrationID;

			Address customerAddress = Customer.FK.Address.FindParent(graph, customer);
			payment.L3Data.ShiptoZipCode = customerLocationAddress?.PostalCode ?? customerAddress?.PostalCode;

			BAccount bAccount = BAccount2.PK.Find(graph, branch.BAccountID.Value);
			Address bAddress = Address.PK.Find(graph, bAccount.DefContactID);
			payment.L3Data.ShipfromZipCode = bAddress?.PostalCode;
			payment.L3Data.TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero);
			payment.L3Data.TaxExempt = payment.L3Data.TaxAmount == 0;
			payment.L3Data.MerchantVatRegistration = bAccount?.TaxRegistrationID;
		}

		public static void RetriveInventoryInfo(PXGraph graph, int? inventoryID, TranProcessingL3DataLineItemInput item)
		{
			InventoryItem inventoryItem = InventoryItem.PK.Find(graph, inventoryID);
			item.ProductCode = inventoryItem?.InventoryCD;
			item.CommodityCode = inventoryItem?.HSTariffCode;
		}
	}

	public class PaymentMapping : IBqlMapping
	{
		public Type PMInstanceID = typeof(Payment.pMInstanceID);
		public Type CashAccountID = typeof(Payment.cashAccountID);
		public Type ProcessingCenterID = typeof(Payment.processingCenterID);
		public Type CuryDocBal = typeof(Payment.curyDocBal);
		public Type CuryID = typeof(Payment.curyID);
		public Type DocType = typeof(Payment.docType);
		public Type RefNbr = typeof(Payment.refNbr);
		public Type OrigDocType = typeof(Payment.origDocType);
		public Type OrigRefNbr = typeof(Payment.origRefNbr);
		public Type RefTranExtNbr = typeof(Payment.refTranExtNbr);
		public Type Released = typeof(Payment.released);
		public Type SaveCard = typeof(Payment.saveCard);
		public Type CCTransactionRefund = typeof(Payment.cCTransactionRefund);
		public Type CCPaymentStateDescr = typeof(Payment.cCPaymentStateDescr);
		public Type CCActualExternalTransactionID = typeof(Payment.cCActualExternalTransactionID);

		public Type Table { get; private set; }
		public Type Extension => typeof(Payment);
		public PaymentMapping(Type table)
		{
			Table = table;
		}
	}

	public class ExternalTransactionDetailMapping : IBqlMapping
	{
		public Type TransactionID = typeof(ExternalTransactionDetail.transactionID);
		public Type PMInstanceID = typeof(ExternalTransactionDetail.pMInstanceID);
		public Type DocType = typeof(ExternalTransactionDetail.docType);
		public Type RefNbr = typeof(ExternalTransactionDetail.refNbr);
		public Type OrigDocType = typeof(ExternalTransactionDetail.origDocType);
		public Type OrigRefNbr = typeof(ExternalTransactionDetail.origRefNbr);
		public Type VoidDocType = typeof(ExternalTransactionDetail.voidDocType);
		public Type VoidRefNbr = typeof(ExternalTransactionDetail.voidRefNbr);
		public Type TranNumber = typeof(ExternalTransactionDetail.tranNumber);
		public Type AuthNumber = typeof(ExternalTransactionDetail.authNumber);
		public Type Amount = typeof(ExternalTransactionDetail.amount);
		public Type ProcStatus = typeof(ExternalTransactionDetail.procStatus);
		public Type LastActivityDate = typeof(ExternalTransactionDetail.lastActivityDate);
		public Type Direction = typeof(ExternalTransactionDetail.direction);
		public Type Active = typeof(ExternalTransactionDetail.active);
		public Type Completed = typeof(ExternalTransactionDetail.completed);
		public Type ParentTranID = typeof(ExternalTransactionDetail.parentTranID);
		public Type ExpirationDate = typeof(ExternalTransactionDetail.expirationDate);
		public Type CVVVerification = typeof(ExternalTransactionDetail.cVVVerification);
		public Type NeedSync = typeof(ExternalTransactionDetail.needSync);
		public Type SaveProfile = typeof(ExternalTransactionDetail.saveProfile);
		public Type SyncStatus = typeof(ExternalTransactionDetail.syncStatus);
		public Type SyncMessage = typeof(ExternalTransactionDetail.syncMessage);
		public Type NoteID = typeof(ExternalTransactionDetail.noteID);
		public Type Extension => typeof(ExternalTransactionDetail);
		public Type Table { get; private set; }

		public ExternalTransactionDetailMapping(Type table)
		{
			Table = table;
		}
	}
}

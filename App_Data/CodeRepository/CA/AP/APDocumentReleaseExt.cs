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

using PX.Data;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.CR;
using static PX.Data.PXGraph;
using PX.Objects.GL;
using PX.Objects.CM.Extensions;
using PX.Objects.CA;

namespace PX.Objects.Localizations.CA.AP
{
	public class APReleaseProcessExt : PXGraphExtension<APReleaseProcess>
	{
		public PXSelect<T5018Transactions> Transactions;
		
		#region IsActive

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
		}

		#endregion

		#region Overrides

		//Inserting T5018Transactions for released APPayment documents
		[PXOverride]
		public virtual void ProcessPayment(
			JournalEntry je,
			APRegister doc,
			PXResult<APPayment, CurrencyInfo, Currency, Vendor, CashAccount> res)
		{
			if (doc == null) return;

			APPayment payment = res;

			if (payment == null) return;

			Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID,
				Equal<Required<Vendor.bAccountID>>>>.Select(Base, doc.VendorID);

			if (vendor == null) return;

			T5018VendorExt vendorExt = PXCache<BAccount>.GetExtension<T5018VendorExt>(vendor);

			if (!(vendorExt.VendorT5018.HasValue && vendorExt.VendorT5018.Value.Equals(true))) return;			

			bool exists = T5018Transactions.PK.Find(Base, payment.BranchID.Value, payment.VendorID.Value, payment.DocDate.Value, payment.DocType, payment.RefNbr) != null;

			if (!exists && (payment.DocType.Equals(APDocType.Prepayment)
				|| payment.DocType.Equals(APDocType.QuickCheck) 
				|| payment.DocType.Equals(APDocType.Check)))
			{
				this.Transactions.Insert(new T5018Transactions
				{
					BranchID = payment.BranchID,
					VendorID = payment.VendorID,
					DocDate = payment.DocDate,
					DocType = payment.DocType,
					RefNbr = payment.RefNbr
				});
			}
		}

		//When prepayment is created from prepayment application it should also be added into T5018Transactions
		[PXOverride]
		public virtual void ProcessPrepaymentRequestAppliedToCheck(APRegister prepaymentRequest, APAdjust prepaymentAdj)
		{
			//Selecting prepayment that was generated from prepayment request
			APPayment payment = Base.APPayment_DocType_RefNbr.Select(prepaymentRequest.DocType, prepaymentRequest.RefNbr);
			
			if (payment != null &&
				(T5018Transactions.PK.Find(Base, payment.BranchID.Value, payment.VendorID.Value, payment.DocDate.Value, payment.DocType, payment.RefNbr) == null))
			{
				this.Transactions.Insert(new T5018Transactions
				{
					BranchID = payment.BranchID,
					VendorID = payment.VendorID,
					DocDate = payment.DocDate,
					DocType = payment.DocType,
					RefNbr = payment.RefNbr
				});
			}
		}

		[PXOverride]
		public void PerformPersist(IPersistPerformer persister)
		{
			persister.Insert(Transactions.Cache);
		}
		#endregion
	}
}

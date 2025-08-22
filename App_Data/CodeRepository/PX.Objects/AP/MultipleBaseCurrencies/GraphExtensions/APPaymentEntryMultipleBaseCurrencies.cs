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
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;

namespace PX.Objects.AP
{
	public class APPaymentEntryMultipleBaseCurrencies : PXGraphExtension<APPaymentEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>();
		}

		protected virtual void _(Events.FieldVerifying<APPayment.branchID> e)
		{
			if (e.NewValue == null)
				return;

			Branch branch = PXSelectorAttribute.Select<APPayment.branchID>(e.Cache, e.Row, (int)e.NewValue) as Branch;
			string vendorBaseCuryID = (string)PXFormulaAttribute.Evaluate<APPaymentMultipleBaseCurrenciesRestriction.vendorBaseCuryID>(e.Cache, e.Row);

			if (vendorBaseCuryID != null && branch != null
				&& branch.BaseCuryID != vendorBaseCuryID)
			{
				e.NewValue = branch.BranchCD;
				BAccountR vendor = PXSelectorAttribute.Select<APPayment.vendorID>(e.Cache, e.Row) as BAccountR;
				throw new PXSetPropertyException(Messages.BranchVendorDifferentBaseCury, PXOrgAccess.GetCD(vendor.VOrgBAccountID), vendor.AcctCD);
			}
		}

		protected virtual void _(Events.RowUpdated<APPayment> e)
		{
			Branch branch = PXSelectorAttribute.Select<APPayment.branchID>(e.Cache, e.Row, e.Row.BranchID) as Branch;
			PXFieldState vendorBaseCuryID = e.Cache.GetValueExt<APPaymentMultipleBaseCurrenciesRestriction.vendorBaseCuryID>(e.Row) as PXFieldState;
			if (vendorBaseCuryID?.Value != null && branch != null
				&& branch.BaseCuryID != vendorBaseCuryID.ToString())
			{
				e.Row.BranchID = null;
			}
		}

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[APInvoiceType.AdjdRefNbr(typeof(Search5<APAdjust.APInvoice.refNbr,
			LeftJoin<APAdjust, On<APAdjust.adjdDocType, Equal<APAdjust.APInvoice.docType>,
				And<APAdjust.adjdRefNbr, Equal<APAdjust.APInvoice.refNbr>,
				And<APAdjust.released, Equal<False>,
				And<Where<APAdjust.adjgDocType, NotEqual<Current<APPayment.docType>>,
					Or<APAdjust.adjgRefNbr, NotEqual<Current<APPayment.refNbr>>>>>>>>,
			LeftJoin<APAdjust2, On<APAdjust2.adjgDocType, Equal<APAdjust.APInvoice.docType>,
				And<APAdjust2.adjgRefNbr, Equal<APAdjust.APInvoice.refNbr>,
				And<APAdjust2.released, Equal<False>,
				And<APAdjust2.voided, Equal<False>>>>>,
			LeftJoin<APPayment, On<APPayment.docType, Equal<APAdjust.APInvoice.docType>,
				And<APPayment.refNbr, Equal<APAdjust.APInvoice.refNbr>,
				And<Where<APPayment.docType, Equal<APDocType.prepayment>,
					Or<APPayment.docType, Equal<APDocType.debitAdj>>>>>>,
				LeftJoin<Branch, On<Branch.branchID, Equal<APAdjust.APInvoice.branchID>>>>>>,
			Where<APAdjust.APInvoice.vendorID, Equal<Optional<APPayment.vendorID>>,
				And<APAdjust.APInvoice.docType, Equal<Optional<APAdjust.adjdDocType>>,
				And<Branch.baseCuryID, Equal<Optional<APPaymentMultipleBaseCurrenciesRestriction.branchBaseCuryID>>,
				And2<Where<APAdjust.APInvoice.released, Equal<True>,
					Or<APAdjust.APInvoice.prebooked, Equal<True>>>,
				And<APAdjust.APInvoice.openDoc, Equal<True>,
				And<APAdjust.APInvoice.hold, Equal<False>,
				And2<Where<APAdjust.adjgRefNbr, IsNull, Or<APAdjust.APInvoice.isJointPayees, Equal<True>>>,
				And<APAdjust2.adjdRefNbr, IsNull,
				And2<Where<APPayment.refNbr, IsNull,
					And<Current<APPayment.docType>, NotEqual<APDocType.refund>,
					Or<APPayment.refNbr, IsNotNull,
					And<Current<APPayment.docType>, Equal<APDocType.refund>,
					Or<APPayment.docType, Equal<APDocType.debitAdj>,
					And<Current<APPayment.docType>, Equal<APDocType.check>,
					Or<APPayment.docType, Equal<APDocType.debitAdj>,
					And<Current<APPayment.docType>, Equal<APDocType.voidCheck>>>>>>>>>,
				And2<Where<APAdjust.APInvoice.docDate, LessEqual<Current<APPayment.adjDate>>,
					And<APAdjust.APInvoice.tranPeriodID, LessEqual<Current<APPayment.adjTranPeriodID>>,
					Or<Current<APPayment.adjTranPeriodID>, IsNull,
					Or<Current<APPayment.docType>, Equal<APDocType.check>,
					And<Current<APSetup.earlyChecks>, Equal<True>,
					Or<Current<APPayment.docType>, Equal<APDocType.voidCheck>,
					And<Current<APSetup.earlyChecks>, Equal<True>,
					Or<Current<APPayment.docType>, Equal<APDocType.prepayment>,
					And<Current<APSetup.earlyChecks>, Equal<True>>>>>>>>>>,
				And2<Where<
					Current<APSetup.migrationMode>, NotEqual<True>,
					Or<APAdjust.APInvoice.isMigratedRecord, Equal<Current<APRegister.isMigratedRecord>>>>,
				And<Where<APAdjust.APInvoice.pendingPPD, NotEqual<True>,
					Or<Current<APRegister.pendingPPD>, Equal<True>>>>>>>>>>>>>>>,
				Aggregate<GroupBy<APAdjust.APInvoice.docType, GroupBy<APAdjust.APInvoice.refNbr>>>>),
			Filterable = true)]
		protected virtual void _(Events.CacheAttached<APAdjust.adjdRefNbr> e) { }

	}
}

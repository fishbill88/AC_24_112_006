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
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.TX;
using ARCashSale = PX.Objects.AR.Standalone.ARCashSale;

namespace PX.Objects.AR
{
	public class ARCashSaleTaxAttribute : ARTaxAttribute
	{
		public ARCashSaleTaxAttribute(Type ParentType, Type TaxType, Type TaxSumType, Type parentBranchIDField = null)
			: base(ParentType, TaxType, TaxSumType, parentBranchIDField: parentBranchIDField)
		{
			DocDate = typeof(ARCashSale.adjDate);
			FinPeriodID = typeof(ARCashSale.adjFinPeriodID);
			CuryLineTotal = typeof(ARCashSale.curyLineTotal);
			this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<ARTran.lineType, NotEqual<SO.SOLineType.discount>>, ARTran.curyTranAmt>, Minus<ARTran.curyTranAmt>>), typeof(SumCalc<ARCashSale.curyLineTotal>)));
			this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<ARTran.lineType,
				NotIn3<SO.SOLineType.discount, SO.SOLineType.freight>>, ARTran.curyDiscAmt>,decimal0>),
				typeof(SumCalc<ARCashSale.curyLineDiscTotal>)));
			_Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<
					Case<Where<ARTran.lineType, NotEqual<SO.SOLineType.miscCharge>, And<ARTran.lineType, NotEqual<SO.SOLineType.freight>, And<ARTran.lineType, NotEqual<SO.SOLineType.discount>,
					And<ARTran.lineType, IsNotNull, And<ARTran.lineType, NotEqual<Empty>>>>>>, ARTran.curyExtPrice>,
					decimal0>),
					typeof(SumCalc<ARCashSale.curyGoodsExtPriceTotal>)));
			_Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<
				Case<Where<ARTran.lineType, Equal<SO.SOLineType.miscCharge>, Or<ARTran.lineType, IsNull, Or<ARTran.lineType, Equal<Empty>>>>, ARTran.curyExtPrice>,
				decimal0>),
				typeof(SumCalc<ARCashSale.curyMiscExtPriceTotal>)));
		}

		private bool needTaxRecalculation = true;

		protected override void ParentFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARCashSale doc;
			if (e.Row is ARCashSale)
			{
				base.ParentFieldUpdated(sender, e);
			}
			else if (e.Row is CurrencyInfo && (doc = PXSelect<ARCashSale, Where<ARCashSale.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(sender.Graph, ((CurrencyInfo)e.Row).CuryInfoID)) != null && doc.DocType != ARDocType.CashReturn)
			{
				base.ParentFieldUpdated(sender, e);
			}
		}

		protected override void ZoneUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((ARCashSale)e.Row).DocType != ARDocType.CashReturn)
			{
				base.ZoneUpdated(sender, e);
			}
		}

		protected override void DateUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((ARCashSale)e.Row).DocType != ARDocType.CashReturn)
			{
				base.DateUpdated(sender, e);
			}
		}
		protected override bool IsRetainedTaxes(PXGraph graph)
		{
			return false;
		}

		protected override bool ConsiderEarlyPaymentDiscount(PXCache sender, object parent, Tax tax)
		{
			return
				(tax.TaxCalcLevel == CSTaxCalcLevel.CalcOnItemAmt
				|| tax.TaxCalcLevel == CSTaxCalcLevel.CalcOnItemAmtPlusTaxAmt)
							&&
				tax.TaxApplyTermsDisc == CSTaxTermsDiscount.ToPromtPayment;
		}
		protected override bool ConsiderInclusiveDiscount(PXCache sender, object parent, Tax tax)
		{
			return (tax.TaxCalcLevel == CSTaxCalcLevel.Inclusive && tax.TaxApplyTermsDisc == CSTaxTermsDiscount.ToPromtPayment);
		}

		protected override void _CalcDocTotals(
			PXCache sender,
			object row,
			decimal CuryTaxTotal,
			decimal CuryInclTaxTotal,
			decimal CuryWhTaxTotal,
			decimal CuryTaxDiscountTotal)
		{
			decimal CuryDiscountTotal = (decimal)(ParentGetValue(sender.Graph, _CuryDiscTot) ?? 0m);
			decimal CuryLineTotal = (decimal)(ParentGetValue(sender.Graph, _CuryLineTotal) ?? 0m);

			decimal CuryDocTotal = CuryLineTotal + CuryTaxTotal + CuryTaxDiscountTotal - CuryInclTaxTotal - CuryDiscountTotal;

			decimal doc_CuryTaxTotal = (decimal)(ParentGetValue(sender.Graph, _CuryTaxTotal) ?? 0m);

			if (!Equals(CuryTaxTotal, doc_CuryTaxTotal))
			{
				ParentSetValue(sender.Graph, _CuryTaxTotal, CuryTaxTotal);
			}

			if (!string.IsNullOrEmpty(_CuryTaxDiscountTotal))
			{
				ParentSetValue(sender.Graph, _CuryTaxDiscountTotal, CuryTaxDiscountTotal);
			}

			decimal doc_CuryDocBal = 0;
			if (!string.IsNullOrEmpty(_CuryDocBal))
			{
				doc_CuryDocBal = (decimal)(ParentGetValue(sender.Graph, _CuryDocBal) ?? 0m);
				ParentSetValue(sender.Graph, _CuryDocBal, CuryDocTotal);
			}

			object parent = ParentRow(sender.Graph);
			if (parent == null)
				return;

			//Here we only handle the case with externallCall == false
			//Since in the ParentRowUpdated method of the base attribute, there is a case handling
			//with externallCall == true (when the CuryOrigDiscAmt value is entered by the user manually)
			OrigDiscAmtExtCallDict.TryGetValue(parent, out bool externallCall);
			if (externallCall || CuryDocTotal == 0 || doc_CuryDocBal == CuryDocTotal)
				return;

			bool considerEarlyPaymentDiscount = false;
			foreach (object taxitem in SelectTaxes(sender, parent, PXTaxCheck.RecalcTotals))
			{
				Tax tax = PXResult.Unwrap<Tax>(taxitem);
				if (ConsiderEarlyPaymentDiscount(sender, parent, tax) || ConsiderInclusiveDiscount(sender, parent, tax))
				{
					considerEarlyPaymentDiscount = true;
					break; //as combination of reduce taxable and reduce taxable on early payment is forbidden, we can skip further calculation
				}
			}

			if (considerEarlyPaymentDiscount && needTaxRecalculation)
			{
				needTaxRecalculation = false;
				decimal CuryOrigDiscAmt = (decimal)(ParentGetValue(sender.Graph, _CuryOrigDiscAmt) ?? 0m);
				DiscPercentsDict[parent] = 100 * CuryOrigDiscAmt / CuryDocTotal;

				PXFieldUpdatedEventArgs args = new PXFieldUpdatedEventArgs(parent, doc_CuryDocBal, false);
				ParentFieldUpdated(sender, args);

				OrigDiscAmtExtCallDict.Remove(parent);
				DiscPercentsDict.Remove(parent);
			}
		}
	}
}

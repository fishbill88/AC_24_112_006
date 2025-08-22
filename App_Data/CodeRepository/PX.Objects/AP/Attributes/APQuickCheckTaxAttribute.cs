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
using PX.Objects.TX;
using APQuickCheck = PX.Objects.AP.Standalone.APQuickCheck;

namespace PX.Objects.AP
{
	/// <summary>
	/// Specialized for <see cref="APQuickCheck"/> version of the <see cref="APTaxAttribute"/>(override).<br/>
	/// Provides Tax calculation for <see cref="APTran"/>, by default is attached to <see cref="APTran"/> (details) and <see cref="APQuickCheck"/> (header) <br/>
	/// Normally, should be placed on the TaxCategoryID field. <br/>
	/// Internally, it uses <see cref="APQuickCheckEntry"/> graph, otherwise taxes are not calculated (tax calc Level is set to NoCalc).<br/>
	/// As a result of this attribute work a set of <see cref="APTax"/> tran related to each AP Tran and to their parent will created <br/>
	/// May be combined with other attrbibutes with similar type - for example, APTaxAttribute <br/>
	/// <example>
	///[APQuickCheckTax(typeof(Standalone.APQuickCheck), typeof(APTax), typeof(APTaxTran))]
	/// </example>    
	/// </summary>
	public class APQuickCheckTaxAttribute : APTaxAttribute
	{
		/// <summary>
		/// <param name="ParentType">Type of parent(main) document. Must Be IBqlTable </param>
		/// <param name="TaxType">Type of the TaxTran records for the row(details). Must be IBqlTable</param>
		/// <param name="TaxSumType">Type of the TaxTran recorde for the main documect (summary). Must be iBqlTable</param>		
		/// </summary>

		public APQuickCheckTaxAttribute(Type ParentType, Type TaxType, Type TaxSumType, Type CalcMode = null, Type parentBranchIDField = null)
			: base(ParentType, TaxType, TaxSumType, CalcMode, parentBranchIDField)
		{
			Init();
		}

		private bool needTaxRecalculation = true;

		private void Init()
		{
			DocDate = typeof(APQuickCheck.adjDate);
			FinPeriodID = typeof(APQuickCheck.adjFinPeriodID);
			CuryLineTotal = typeof(APQuickCheck.curyLineTotal);
			CuryTranAmt = typeof(APTran.curyTranAmt);

			_Attributes.Clear();
			_Attributes.Add(new PXUnboundFormulaAttribute(typeof(APTran.curyTranAmt), typeof(SumCalc<APQuickCheck.curyLineTotal>)));
		}

		protected override void ParentFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APQuickCheck doc;
			if (e.Row is APQuickCheck && ((APQuickCheck)e.Row).DocType != APDocType.VoidQuickCheck)
			{
				base.ParentFieldUpdated(sender, e);
			}
			else if (e.Row is CurrencyInfo && (doc = PXSelect<APQuickCheck, Where<APQuickCheck.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(sender.Graph, ((CurrencyInfo)e.Row).CuryInfoID)) != null && doc.DocType != APDocType.VoidQuickCheck)
			{
				base.ParentFieldUpdated(sender, e);
			}
		}

		protected override void ZoneUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((APQuickCheck)e.Row).DocType != APDocType.VoidQuickCheck)
			{
				base.ZoneUpdated(sender, e);
			}
		}

		protected override void DateUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((APQuickCheck)e.Row).DocType != APDocType.VoidQuickCheck)
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

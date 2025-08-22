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
using PX.Objects.AR;
using PX.Objects.CA.MultiCurrency;
using PX.Objects.CM.Extensions;
using PX.Objects.Extensions.MultiCurrency;
using System;

namespace PX.Objects.CA.BankStatementProtoHelpers
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class StatementApplicationBalancesProto : PXGraphExtension<CABankTransactionsMaint>
	{
		private PXSelectBase<CurrencyInfo> curyInfoSelect => Base.CurrencyInfo_CuryInfoID;

		private IPXCurrencyHelper CuryHelper => Base.GetExtension<CABankTransactionsMaintMultiCurrency>();

		private APPaymentBalanceCalculator APPaymentBalanceCalculator => new APPaymentBalanceCalculator(CuryHelper);

		public void UpdateBalance(CABankTran currentDetail, CABankTranAdjustment adj, bool isCalcRGOL)
		{
			if (currentDetail.OrigModule == GL.BatchModule.AP)
			{
				foreach (PXResult<APInvoice, CurrencyInfo> res in PXSelectJoin<APInvoice, InnerJoin<CurrencyInfo,
					On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>>,
					Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
						And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(Base, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					UpdateBalanceFromAPDocument<APInvoice>(res, adj, isCalcRGOL);
					return;
				}

				foreach (PXResult<APPayment, CurrencyInfo> res in PXSelectJoin<APPayment, InnerJoin<CurrencyInfo,
					On<CurrencyInfo.curyInfoID, Equal<APPayment.curyInfoID>>>,
					Where<APPayment.docType, Equal<Required<APPayment.docType>>,
						And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>.Select(Base, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					UpdateBalanceFromAPDocument< APPayment>(res, adj, isCalcRGOL);
				}
			}
			else if (currentDetail.OrigModule == GL.BatchModule.AR)
			{
				foreach (ARInvoice invoice in PXSelect<ARInvoice, Where<ARInvoice.customerID, Equal<Required<ARInvoice.customerID>>,
					And<ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
					And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>>.Select(Base, currentDetail.PayeeBAccountID, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					UpdateBalanceFromARDocument(adj, invoice, isCalcRGOL);
					return;
				}

				foreach (ARPayment invoice in PXSelect<ARPayment, Where<ARPayment.customerID, Equal<Required<ARPayment.customerID>>,
					And<ARPayment.docType, Equal<Required<ARPayment.docType>>,
					And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>>.Select(Base, currentDetail.PayeeBAccountID, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					UpdateBalanceFromARDocument(adj, invoice, isCalcRGOL);
				}
			}
		}

		private void UpdateBalanceFromAPDocument<T>(T invoice, CABankTranAdjustment adj, bool isCalcRGOL)
			where T : APRegister, CM.IInvoice, new()
		{
			APAdjust adjustment = new APAdjust
			{
				AdjdRefNbr = adj.AdjdRefNbr,
				AdjdDocType = adj.AdjdDocType
			};
			CopyToAdjust(adjustment, adj);
			APPaymentBalanceCalculator.CalcBalances(adjustment, invoice, isCalcRGOL, true, null);

			CopyToAdjust(adj, adjustment);
			adj.AdjdCuryRate = adjustment.AdjdCuryRate;
		}

		private void UpdateBalanceFromARDocument<TInvoice>(CABankTranAdjustment adj, TInvoice invoice, bool isCalcRGOL)
			where TInvoice : CM.IInvoice
		{
			ARAdjust adjustment = new ARAdjust
			{
				AdjdRefNbr = adj.AdjdRefNbr,
				AdjdDocType = adj.AdjdDocType
			};
			CopyToAdjust(adjustment, adj);

			CalculateBalancesAR(adjustment, invoice, isCalcRGOL, false);

			CopyToAdjust(adj, adjustment);
			adj.AdjdCuryRate = adjustment.AdjdCuryRate;
		}

		public void PopulateAdjustmentFieldsAP(CABankTran currentDetail, CABankTranAdjustment adj)
		{
			foreach (PXResult<APInvoice, CurrencyInfo> res in PXSelectJoin<APInvoice, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>>,
				Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
					And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(Base, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				PopulateAP<APInvoice>(res, res, currentDetail, adj);
				return;
			}

			foreach (PXResult<APPayment, CurrencyInfo> res in PXSelectJoin<APPayment, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<APPayment.curyInfoID>>>,
				Where<APPayment.docType, Equal<Required<APPayment.docType>>,
					And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>.Select(Base, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				PopulateAP<APPayment>(res, res, currentDetail, adj);
			}
		}

		private void PopulateAP<T>(T invoice, CurrencyInfo info, CABankTran currentDetail, CABankTranAdjustment adj)
			where T : APRegister, CM.IInvoice, new()
		{
			CurrencyInfo info_copy;

			info_copy = PXCache<CurrencyInfo>.CreateCopy(info);
			info_copy.CuryInfoID = adj.AdjdCuryInfoID;
			info_copy = (CurrencyInfo)curyInfoSelect.Cache.Update(info_copy);
			info_copy.CuryEffDate = currentDetail.TranDate;
			info_copy = (CurrencyInfo)curyInfoSelect.Cache.Update(info_copy);
			
			adj.AdjdBranchID = invoice.BranchID;
			adj.AdjdDocDate = invoice.DocDate;
			adj.AdjdFinPeriodID = invoice.FinPeriodID;
			//				adj.AdjgCuryInfoID = currentDetail.CuryInfoID;
			adj.AdjdCuryInfoID = info_copy.CuryInfoID;
			adj.AdjdOrigCuryInfoID = info.CuryInfoID;
			adj.AdjgDocDate = currentDetail.TranDate;
			adj.AdjdAPAcct = invoice.APAccountID;
			adj.AdjdAPSub = invoice.APSubID;
			adj.PaymentsByLinesAllowed = invoice.PaymentsByLinesAllowed;
			adj.CuryOrigDocAmt = invoice.CuryOrigDocAmt;

			APAdjust adjustment = new APAdjust
			{
				AdjdRefNbr = adj.AdjdRefNbr,
				AdjdDocType = adj.AdjdDocType,
				AdjdAPAcct = invoice.APAccountID,
				AdjdAPSub = invoice.APSubID
			};
			CopyToAdjust(adjustment, adj);

			if (currentDetail.DrCr == CADrCr.CACredit)
			{
				adjustment.AdjgDocType = APDocType.Check;
			}
			else
			{
				adjustment.AdjgDocType = APDocType.Refund;
			}

			adj.AdjgBalSign = adjustment.AdjgBalSign;
			APPaymentBalanceCalculator.CalcBalances(adjustment, invoice, false, true, null);

			decimal? CuryApplDiscAmt = (adjustment.AdjgDocType == APDocType.DebitAdj) ? 0m : adjustment.CuryDiscBal;
			decimal? CuryApplAmt = adjustment.CuryDocBal - adjustment.CuryWhTaxBal - CuryApplDiscAmt;
			decimal? CuryUnappliedBal = currentDetail.CuryUnappliedBal;

			if (currentDetail != null && adjustment.AdjgBalSign < 0m)
			{
				if (CuryUnappliedBal < 0m)
				{
					CuryApplAmt = Math.Min((decimal)CuryApplAmt, Math.Abs((decimal)CuryUnappliedBal));
				}
			}
			else if (currentDetail != null && CuryUnappliedBal > 0m && adjustment.AdjgBalSign > 0m && CuryUnappliedBal < CuryApplDiscAmt)
			{
				CuryApplAmt = CuryUnappliedBal;
				CuryApplDiscAmt = 0m;
			}
			else if (currentDetail != null && CuryUnappliedBal > 0m && adjustment.AdjgBalSign > 0m)
			{
				CuryApplAmt = Math.Min((decimal)CuryApplAmt, (decimal)CuryUnappliedBal);
			}
			else if (currentDetail != null && CuryUnappliedBal <= 0m && currentDetail.CuryOrigDocAmt > 0)
			{
				CuryApplAmt = 0m;
			}

			adjustment.CuryAdjgAmt = CuryApplAmt;
			adjustment.CuryAdjgDiscAmt = CuryApplDiscAmt;
			adjustment.CuryAdjgWhTaxAmt = adjustment.CuryWhTaxBal;
			APPaymentBalanceCalculator.CalcBalances(adjustment, invoice, true, true, null);
			CopyToAdjust(adj, adjustment);
			adj.AdjdCuryRate = adjustment.AdjdCuryRate;
		}

		public void PopulateAdjustmentFieldsAR(CABankTran currentDetail, CABankTranAdjustment adj)
		{
			foreach (PXResult<ARInvoice, CurrencyInfo> res in PXSelectJoin<ARInvoice, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<ARInvoice.curyInfoID>>>,
				Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
					And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(Base, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				PopulateAR<ARInvoice>(res, res, currentDetail, adj);
				return;
			}


			foreach (PXResult<ARPayment, CurrencyInfo> res in PXSelectJoin<ARPayment, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<ARPayment.curyInfoID>>>,
				Where<ARPayment.docType, Equal<Required<ARPayment.docType>>,
					And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>.Select(Base, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				PopulateAR<ARPayment>(res, res, currentDetail, adj);
			}
		}

		private void PopulateAR<TInvoice>(TInvoice invoice, CurrencyInfo currencyInfo, CABankTran currentDetail, CABankTranAdjustment adj)
			where TInvoice : ARRegister, CM.IInvoice, new()
		{
			CurrencyInfo info_copy = PXCache<CurrencyInfo>.CreateCopy(currencyInfo);
			info_copy.CuryInfoID = adj.AdjdCuryInfoID;
			info_copy = (CurrencyInfo)curyInfoSelect.Cache.Update(info_copy);
			info_copy.CuryEffDate = currentDetail.TranDate;
			info_copy = (CurrencyInfo)curyInfoSelect.Cache.Update(info_copy);

			//adj.AdjgCuryInfoID = currentDetail.CuryInfoID;
			adj.AdjdCuryInfoID = info_copy.CuryInfoID;
			adj.AdjdOrigCuryInfoID = invoice.CuryInfoID;
			adj.AdjdBranchID = invoice.BranchID;
			adj.AdjdDocDate = invoice.DocDate;
			adj.AdjdFinPeriodID = invoice.FinPeriodID;
			adj.AdjdARAcct = invoice.ARAccountID;
			adj.AdjdARSub = invoice.ARSubID;
			adj.AdjgBalSign = -ARDocType.SignBalance(currentDetail.DocType) * ARDocType.SignBalance(adj.AdjdDocType);
			adj.PaymentsByLinesAllowed = invoice.PaymentsByLinesAllowed;
			adj.CuryOrigDocAmt = invoice.CuryOrigDocAmt;

			ARAdjust adjustment = new ARAdjust
			{
				AdjdRefNbr = adj.AdjdRefNbr,
				AdjdDocType = adj.AdjdDocType,
				AdjdARAcct = invoice.ARAccountID,
				AdjdARSub = invoice.ARSubID
			};
			CopyToAdjust(adjustment, adj);

			CalculateBalancesAR(adjustment, invoice, false, true);

			decimal? CuryApplAmt = adjustment.CuryDocBal - adjustment.CuryDiscBal;
			decimal? CuryApplDiscAmt = adjustment.CuryDiscBal;
			decimal? CuryUnappliedBal = currentDetail.CuryUnappliedBal;


			if (currentDetail != null && adj.AdjgBalSign < 0m)
			{
				if (CuryUnappliedBal < 0m)
				{
					CuryApplAmt = Math.Min((decimal)CuryApplAmt, Math.Abs((decimal)CuryUnappliedBal));
				}
			}
			else if (currentDetail != null && CuryUnappliedBal > 0m && adj.AdjgBalSign > 0m)
			{
				CuryApplAmt = Math.Min((decimal)CuryApplAmt, (decimal)CuryUnappliedBal);

				if (CuryApplAmt + CuryApplDiscAmt < adjustment.CuryDocBal)
				{
					CuryApplDiscAmt = 0m;
				}
			}
			else if (currentDetail != null && CuryUnappliedBal <= 0m && ((CABankTran)currentDetail).CuryOrigDocAmt > 0)
			{
				CuryApplAmt = 0m;
				CuryApplDiscAmt = 0m;
			}

			adjustment.CuryAdjgAmt = CuryApplAmt;
			adjustment.CuryAdjgDiscAmt = CuryApplDiscAmt;
			adjustment.CuryAdjgWOAmt = 0m;

			CalculateBalancesAR(adjustment, invoice, true, true);

			CopyToAdjust(adj, adjustment);
			adj.AdjdCuryRate = adjustment.AdjdCuryRate;
		}

		public void CalculateBalancesAR<TInvoice>(ARAdjust adj, TInvoice invoice, bool isCalcRGOL, bool DiscOnDiscDate) where TInvoice : CM.IInvoice
		{
			Customer currentCustomer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Optional<CABankTran.payeeBAccountID>>>>.Select(Base);
			CM.Extensions.PaymentBalanceCalculator paymentBalanceCalculator = new CM.Extensions.PaymentBalanceCalculator(CuryHelper);
			paymentBalanceCalculator.CalcBalances(adj.AdjgCuryInfoID, adj.AdjdCuryInfoID, invoice, adj);

			if (DiscOnDiscDate)
			{
				CM.PaymentEntry.CalcDiscount(adj.AdjgDocDate, invoice, adj);
			}
			CM.PaymentEntry.WarnDiscount<TInvoice, ARAdjust>(Base, adj.AdjgDocDate, invoice, adj);
			adj.AdjdCuryRate = paymentBalanceCalculator.GetAdjdCuryRate(adj);

			if (currentCustomer != null && currentCustomer.SmallBalanceAllow == true && adj.AdjgDocType != ARDocType.Refund && adj.AdjdDocType != ARDocType.CreditMemo)
			{
				decimal payment_smallbalancelimit = CuryHelper.GetCurrencyInfo(adj.AdjgCuryInfoID).CuryConvCury(currentCustomer.SmallBalanceLimit ?? 0m);
				adj.CuryWOBal = payment_smallbalancelimit;
				adj.WOBal = currentCustomer.SmallBalanceLimit;
			}
			else
			{
				adj.CuryWOBal = 0m;
				adj.WOBal = 0m;
			}

			new CM.Extensions.PaymentBalanceAjuster(CuryHelper).AdjustBalance(adj);

			if (isCalcRGOL && (adj.Voided != true))
			{
				new CM.Extensions.PaymentRGOLCalculator(CuryHelper, adj, adj.ReverseGainLoss).Calculate(invoice);
			}
		}


		public static CABankTranAdjustment CopyToAdjust(CABankTranAdjustment bankAdj, IAdjustment iAdjust)
		{
			bankAdj.AdjgCuryInfoID = iAdjust.AdjgCuryInfoID;
			bankAdj.AdjdCuryInfoID = iAdjust.AdjdCuryInfoID;
			bankAdj.AdjgDocDate = iAdjust.AdjgDocDate;
			bankAdj.DocBal = iAdjust.DocBal;
			bankAdj.CuryDocBal = iAdjust.CuryDocBal;
			bankAdj.CuryDiscBal = iAdjust.CuryDiscBal;
			bankAdj.CuryWhTaxBal = iAdjust.CuryWhTaxBal;
			bankAdj.CuryAdjgAmt = iAdjust.CuryAdjgAmt;
			bankAdj.CuryAdjdAmt = iAdjust.CuryAdjdAmt;
			bankAdj.CuryAdjgDiscAmt = iAdjust.CuryAdjgDiscAmt;
			bankAdj.CuryAdjdDiscAmt = iAdjust.CuryAdjdDiscAmt;
			bankAdj.CuryAdjgWhTaxAmt = iAdjust.CuryAdjgWhTaxAmt;
			bankAdj.AdjdOrigCuryInfoID = iAdjust.AdjdOrigCuryInfoID;
			return bankAdj;
		}

		public static IAdjustment CopyToAdjust(IAdjustment iAdjust, CABankTranAdjustment bankAdj)
		{
			iAdjust.AdjgCuryInfoID = bankAdj.AdjgCuryInfoID;
			iAdjust.AdjdCuryInfoID = bankAdj.AdjdCuryInfoID;
			iAdjust.AdjgDocDate = bankAdj.AdjgDocDate;
			iAdjust.DocBal = bankAdj.DocBal;
			iAdjust.CuryDocBal = bankAdj.CuryDocBal;
			iAdjust.CuryDiscBal = bankAdj.CuryDiscBal;
			iAdjust.CuryWhTaxBal = bankAdj.CuryWhTaxBal;
			iAdjust.CuryAdjgAmt = bankAdj.CuryAdjgAmt;
			iAdjust.CuryAdjdAmt = bankAdj.CuryAdjdAmt;
			iAdjust.CuryAdjgDiscAmt = bankAdj.CuryAdjgDiscAmt;
			iAdjust.CuryAdjdDiscAmt = bankAdj.CuryAdjdDiscAmt;
			iAdjust.CuryAdjgWhTaxAmt = bankAdj.CuryAdjgWhTaxAmt;
			iAdjust.AdjdOrigCuryInfoID = bankAdj.AdjdOrigCuryInfoID;
			return iAdjust;
		}
	}
}

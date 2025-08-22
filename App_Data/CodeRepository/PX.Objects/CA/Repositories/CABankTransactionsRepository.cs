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
using PX.Objects.CS;
using PX.Objects.GL;
using System;
using CATranExt = PX.Objects.CA.BankStatementHelpers.CATranExt;
using CATran2 = PX.Objects.CA.BankStatementHelpers.CATran2;
using IMatchSettings = PX.Objects.CA.BankStatementHelpers.IMatchSettings;
using PX.Objects.CA.BankStatementProtoHelpers;
using PX.Objects.CA.BankStatementHelpers;

namespace PX.Objects.CA.Repositories
{
	public class CABankTransactionsRepository : ICABankTransactionsRepository
	{
		public virtual PXResultset<CATranExt> SearchForMatchingTransactions(PXGraph graph, CABankTran aDetail, IMatchSettings aSettings, Pair<DateTime, DateTime> tranDateRange, string curyID)
		{
			var cmd = new PXSelectReadonly2<CATranExt,
					LeftJoin<Light.BAccount, On<Light.BAccount.bAccountID, Equal<CATranExt.referenceID>>,
					LeftJoin<CATran2, On<CATran2.cashAccountID, Equal<CATranExt.cashAccountID>,
						And<CATran2.voidedTranID, Equal<CATranExt.tranID>,
						And<True, Equal<Required<CASetup.skipVoided>>>>>,
					LeftJoin<CABankTranMatch2, On<CABankTranMatch2.cATranID, Equal<CATranExt.tranID>,
						And<CABankTranMatch2.tranType, Equal<Required<CABankTran.tranType>>>>,
					LeftJoin<CABatchDetailOrigDocAggregate, On<CABatchDetailOrigDocAggregate.origModule, Equal<CATranExt.origModule>,
						And<CABatchDetailOrigDocAggregate.origDocType, Equal<CATranExt.origTranType>,
						And<CABatchDetailOrigDocAggregate.origRefNbr, Equal<CATranExt.origRefNbr>,
						And<CATranExt.isPaymentChargeTran, Equal<False>>>>>,
					LeftJoin<CABankTranMatch, On<CABankTranMatch.docModule, Equal<BatchModule.moduleAP>,
						And<CABankTranMatch.docType, Equal<CATranType.cABatch>,
						And<CABankTranMatch.docRefNbr, Equal<CABatchDetailOrigDocAggregate.batchNbr>,
						And<CABankTranMatch.tranType, Equal<Required<CABankTran.tranType>>>>>>>>>>>,
					Where<CATranExt.cashAccountID, Equal<Required<CABankTran.cashAccountID>>,
						And<CATranExt.tranDate, Between<Required<CATranExt.tranDate>, Required<CATranExt.tranDate>>,
						And<CATranExt.curyID, Equal<Required<CATranExt.curyID>>>>>>(graph);

			if (aDetail.MultipleMatchingToPayments == true && aDetail.MatchReceiptsAndDisbursements != true)
			{
				if (aDetail.CuryTranAmt.Value > 0)
				{
					cmd.WhereAnd<Where<CATranExt.curyTranAmt, LessEqual<Required<CATranExt.curyTranAmt>>,
										And<CATranExt.curyTranAmt, GreaterEqual<Zero>>>>();
				}
				else
				{
					cmd.WhereAnd<Where<CATranExt.curyTranAmt, GreaterEqual<Required<CATranExt.curyTranAmt>>,
										And<CATranExt.curyTranAmt, LessEqual<Zero>>>>();
				}
			}
			else if (aDetail.MatchReceiptsAndDisbursements != true)
			{
				cmd.WhereAnd<Where<CATranExt.curyTranAmt, Equal<Required<CATranExt.curyTranAmt>>>>();
			}

			if (aSettings.SkipVoided == true)
			{
				cmd.WhereAnd<Where<CATranExt.voidedTranID, IsNull, And<CATran2.tranID, IsNull>>>();
			}
			if ((graph.Caches[typeof(CASetup)].Current as CASetup).SkipReconciled == true)
			{
				cmd.WhereAnd<Where<CATranExt.reconciled, Equal<False>>>();
			}

			return cmd.Select(aSettings.SkipVoided, aDetail.TranType, aDetail.TranType, aDetail.CashAccountID, tranDateRange.first, tranDateRange.second, curyID, aDetail.CuryTranAmt.Value);
		}

		public virtual PXResultset<CABatch> SearchForMatchingCABatches(PXGraph graph, CABankTran aDetail, IMatchSettings aSettings, Pair<DateTime, DateTime> tranDateRange, string curyID, bool allowUnreleased)
		{
			var cmd = new PXSelectJoin<CABatch,
							LeftJoin<CABatchDetail, On<CABatchDetail.batchNbr, Equal<CABatch.batchNbr>>,
							LeftJoin<Light.APPayment, On<Light.APPayment.docType, Equal<CABatchDetail.origDocType>,
								And<Light.APPayment.refNbr, Equal<CABatchDetail.origRefNbr>>>,
							LeftJoin<Light.BAccount, On<Light.BAccount.bAccountID, Equal<Light.APPayment.vendorID>>,
							LeftJoin<CABankTranMatch, On<CABankTranMatch.cATranID, Equal<Light.APPayment.cATranID>,
								And<CABankTranMatch.tranType, Equal<Required<CABankTran.tranType>>>>>>>>,
							 Where<CABatch.cashAccountID, Equal<Required<CABatch.cashAccountID>>,
								And2<Where<CABatch.released, Equal<True>, Or<Required<CASetup.allowMatchingToUnreleasedBatch>, Equal<True>>>,
								And<Where<CABatch.tranDate, Between<Required<CABatch.tranDate>, Required<CABatch.tranDate>>,
								And<CABatch.curyID, Equal<Required<CABatch.curyID>>,
								And<Where<CABatch.reconciled, Equal<False>, Or<Required<CASetup.skipReconciled>, Equal<False>>>>>>>>>>(graph);

			if (aDetail.MultipleMatchingToPayments == true && aDetail.MatchReceiptsAndDisbursements != true)
			{
				cmd.WhereAnd<Where<CABatch.curyDetailTotal, LessEqual<Required<CABatch.curyDetailTotal>>>>();
			}
			else if (aDetail.MatchReceiptsAndDisbursements != true)
			{
				cmd.WhereAnd<Where<CABatch.curyDetailTotal, Equal<Required<CABatch.curyDetailTotal>>>>();
			}

			if (aSettings.SkipVoided == true)
			{
				cmd.WhereAnd<Where<CABatch.voided, NotEqual<True>>>();
			}

			return cmd.Select(aDetail.TranType, aDetail.CashAccountID, allowUnreleased, tranDateRange.first, tranDateRange.second,
								 curyID, (graph.Caches[typeof(CASetup)].Current as CASetup).SkipReconciled ?? false, -1 * aDetail.CuryTranAmt.Value);
		}

		public virtual PXResultset<CABatchDetailOrigDocAggregate> SearchForMatchesInCABatches(PXGraph graph, string tranType, string batchNbr)
		{
			return PXSelectJoin<CABatchDetailOrigDocAggregate,
					InnerJoin<CATran, On<CATran.origModule, Equal<CABatchDetailOrigDocAggregate.origModule>,
						And<CATran.origTranType, Equal<CABatchDetailOrigDocAggregate.origDocType>,
						And<CATran.origRefNbr, Equal<CABatchDetailOrigDocAggregate.origRefNbr>,
						And<CATran.isPaymentChargeTran, Equal<False>>>>>,
					InnerJoin<CABankTranMatch, On<CABankTranMatch.cATranID, Equal<CATran.tranID>,
						And<CABankTranMatch.tranType, Equal<Required<CABankTran.tranType>>>>>>,
					Where<CABatchDetailOrigDocAggregate.batchNbr, Equal<Required<CABatch.batchNbr>>>>.Select(graph, tranType, batchNbr);
		}

		public virtual PXResultset<CABankTranMatch> SearchForTranMatchForCABatch(PXGraph graph, string batchNbr)
		{
			return PXSelect<CABankTranMatch, Where<CABankTranMatch.docRefNbr, Equal<Required<CABankTranMatch.docRefNbr>>,
					And<CABankTranMatch.docType, Equal<CATranType.cABatch>,
					And<CABankTranMatch.docModule, Equal<BatchModule.moduleAP>>>>>.Select(graph, batchNbr);
		}

		/// <summary>
		/// Searches in database AR invoices, based on the the information in CABankTran record.
		/// The field used for the search are  - BAccountID and InvoiceInfo. First it is searching a invoice by it RefNbr, 
		/// then (if not found) - by invoiceNbr. 
		/// </summary>
		/// <param name="aRow">parameters for the search. The field used for the search are  - BAccountID and InvoiceInfo.</param>
		///	<returns>Returns null if nothing is found and PXResult<ARInvoice,ARAdjust> in the case of success.
		///		ARAdjust record represents unreleased adjustment (payment), applied to this Invoice
		///	</returns>
		public virtual PXResult<ARInvoice, ARAdjust> FindARInvoiceByInvoiceInfo(PXGraph graph, CABankTran aRow)
		{
			PXResult<ARInvoice, ARAdjust> invResult = (PXResult<ARInvoice, ARAdjust>)PXSelectJoin<
				ARInvoice,
				LeftJoin<ARAdjust,
					On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
					And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
					And<ARAdjust.released, Equal<boolFalse>>>>>,
				Where<ARInvoice.docType, Equal<AR.ARInvoiceType.invoice>,
					And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>
				.Select(graph, aRow.InvoiceInfo);

			if (invResult == null)
			{
				invResult = (PXResult<ARInvoice, ARAdjust>)PXSelectJoin<
					ARInvoice,
					LeftJoin<ARAdjust,
						On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
						And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
						And<ARAdjust.released, Equal<boolFalse>>>>>,
					Where<ARInvoice.docType, Equal<AR.ARInvoiceType.invoice>,
						And<ARInvoice.invoiceNbr, Equal<Required<ARInvoice.invoiceNbr>>>>>
					.Select(graph, aRow.InvoiceInfo);
			}
			return invResult;
		}

		/// <summary>
		/// Searches in database AR invoices, based on the the information in the CABankTran record.
		/// The field used for the search are  - BAccountID and InvoiceInfo. First it is searching a invoice by it RefNbr, 
		/// then (if not found) - by invoiceNbr. 
		/// </summary>
		/// <param name="aRow">Parameters for the search. The field used for the search are  - BAccountID and InvoiceInfo.</param>
		/// <returns>Returns null if nothing is found and PXResult<APInvoice,APAdjust,APPayment> in the case of success.
		/// APAdjust record represents unreleased adjustment (payment), applied to this APInvoice</returns>
		public virtual PXResult<APInvoice, APAdjust, APPayment> FindAPInvoiceByInvoiceInfo(PXGraph graph, CABankTran aRow)
		{

			PXResult<APInvoice, APAdjust, APPayment> invResult = (PXResult<APInvoice, APAdjust, APPayment>)PXSelectJoin<
				APInvoice,
				LeftJoin<APAdjust,
					On<APAdjust.adjdDocType, Equal<APInvoice.docType>,
					And<APAdjust.adjdRefNbr, Equal<APInvoice.refNbr>,
					And<APAdjust.released, Equal<boolFalse>>>>,
				LeftJoin<APPayment,
					On<APPayment.docType, Equal<APInvoice.docType>,
					And<APPayment.refNbr, Equal<APInvoice.refNbr>,
					And<Where<APPayment.docType, Equal<APDocType.prepayment>,
						Or<APPayment.docType, Equal<APDocType.debitAdj>>>>>>>>,
				Where<APInvoice.docType, Equal<AP.APInvoiceType.invoice>,
					And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>
				.Select(graph, aRow.InvoiceInfo);


			if (invResult == null)
			{
				invResult = (PXResult<APInvoice, APAdjust, APPayment>)PXSelectJoin<
					APInvoice,
					LeftJoin<APAdjust,
						On<APAdjust.adjdDocType, Equal<APInvoice.docType>,
						And<APAdjust.adjdRefNbr, Equal<APInvoice.refNbr>,
						And<APAdjust.released, Equal<boolFalse>>>>,
					LeftJoin<APPayment,
						On<APPayment.docType, Equal<APInvoice.docType>,
						And<APPayment.refNbr, Equal<APInvoice.refNbr>,
						And<Where<APPayment.docType, Equal<APDocType.prepayment>,
							Or<APPayment.docType, Equal<APDocType.debitAdj>>>>>>>>,
					Where<APInvoice.docType, Equal<AP.APInvoiceType.invoice>,
						And<APInvoice.invoiceNbr, Equal<Required<APInvoice.invoiceNbr>>>>>
					.Select(graph, aRow.InvoiceInfo);

			}
			return invResult;
		}

		public virtual decimal EvaluateMatching(PXGraph graph, CABankTran aDetail, CABankTranExpenseDetailMatch matchRow, IMatchSettings aSettings)
		{
			return StatementsMatchingProto.EvaluateMatching(graph, aDetail, matchRow, aSettings);
		}

		public virtual decimal EvaluateMatching(PXGraph graph, CABankTran aDetail, CATran aTran, IMatchSettings aSettings)
		{
			return StatementsMatchingProto.EvaluateMatching(graph, aDetail, aTran, aSettings);
		}

		public virtual decimal EvaluateMatching(PXGraph graph, string aStr1, string aStr2, bool aCaseSensitive, bool matchEmpty = true)
		{
			return StatementMatching.EvaluateMatching(aStr1, aStr2, aCaseSensitive, matchEmpty);
		}

		public virtual decimal EvaluateTideMatching(PXGraph graph, string aStr1, string aStr2, bool aCaseSensitive, bool matchEmpty = true)
		{
			return StatementMatching.EvaluateTideMatching(aStr1, aStr2, aCaseSensitive, matchEmpty);
		}

		public virtual decimal CompareDate(PXGraph graph, CABankTran aDetail, CATran aTran, double meanValue, double sigma)
		{
			return StatementsMatchingProto.CompareDate(aDetail, aTran, meanValue, sigma);
		}

		public virtual decimal CompareDate(PXGraph graph, CABankTran aDetail, CABankTranInvoiceMatch aTran, double meanValue, double sigma)
		{
			DateTime? comparedDate = (aTran?.DiscDate >= aDetail.TranDate.Value) ? aTran?.DiscDate : aTran?.DueDate;
			return StatementsMatchingProto.CompareDate(aDetail, comparedDate, meanValue, sigma);
		}

		public virtual decimal CompareDate(PXGraph graph, CABankTran aDetail, CABankTranExpenseDetailMatch aTran, double meanValue, double sigma)
		{
			return StatementsMatchingProto.CompareDate(aDetail, aTran.DocDate, meanValue, sigma);
		}

		public virtual decimal CompareRefNbr(PXGraph graph, CABankTran aDetail, CATran aTran, bool looseCompare, IMatchSettings settings)
		{
			return StatementsMatchingProto.CompareRefNbr(graph, aDetail, aTran, looseCompare, settings);
		}

		public virtual decimal CompareRefNbr(PXGraph graph, CABankTran aDetail, CABankTranInvoiceMatch aTran, bool looseCompare)
		{
			return StatementsMatchingProto.CompareRefNbr(graph, aDetail, aTran, looseCompare);
		}

		public virtual decimal CompareRefNbr(PXGraph graph, CABankTran aDetail, CABankTranExpenseDetailMatch aTran, bool looseCompare, IMatchSettings matchSettings)
		{
			return StatementsMatchingProto.CompareRefNbr(graph, aDetail, aTran.ExtRefNbr, looseCompare, matchSettings);
		}

		public virtual decimal ComparePayee(PXGraph graph, CABankTran aDetail, CATran aTran)
		{
			return StatementsMatchingProto.ComparePayee(graph, aDetail, aTran);
		}

		public virtual decimal ComparePayee(PXGraph graph, CABankTran aDetail, CABankTranInvoiceMatch aTran)
		{
			return StatementsMatchingProto.ComparePayee(graph, aDetail, aTran);
		}

		public virtual decimal CompareExpenseReceiptAmount(PXGraph graph, CABankTran bankTran, CABankTranExpenseDetailMatch receipt, IMatchSettings settings)
		{
			return StatementsMatchingProto.CompareExpenseReceiptAmount(bankTran, receipt.CuryDocAmt.GetValueOrDefault(), settings.CuryDiffThreshold.GetValueOrDefault());
		}

		public virtual decimal EvaluateMatching(PXGraph graph, CABankTran aDetail, CABankTranInvoiceMatch aTran, IMatchSettings aSettings)
		{
			return StatementsMatchingProto.EvaluateMatching(graph, aDetail, aTran, aSettings);
		}
	}
}

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
using System;
using CATranExt = PX.Objects.CA.BankStatementHelpers.CATranExt;
using IMatchSettings = PX.Objects.CA.BankStatementHelpers.IMatchSettings;
using PX.Objects.CA.BankStatementProtoHelpers;

namespace PX.Objects.CA.Repositories
{
	public interface ICABankTransactionsRepository
	{
		PXResultset<CATranExt> SearchForMatchingTransactions(PXGraph graph, CABankTran aDetail, IMatchSettings aSettings, Pair<DateTime, DateTime> tranDateRange, string curyID);
		PXResultset<CABatch> SearchForMatchingCABatches(PXGraph graph, CABankTran aDetail, IMatchSettings aSettings, Pair<DateTime, DateTime> tranDateRange, string curyID, bool allowUnreleased);
		PXResultset<CABatchDetailOrigDocAggregate> SearchForMatchesInCABatches(PXGraph graph, string tranType, string batchNbr);
		PXResultset<CABankTranMatch> SearchForTranMatchForCABatch(PXGraph graph, string batchNbr);

		PXResult<ARInvoice, ARAdjust> FindARInvoiceByInvoiceInfo(PXGraph graph, CABankTran aRow);
		PXResult<APInvoice, APAdjust, APPayment> FindAPInvoiceByInvoiceInfo(PXGraph graph, CABankTran aRow);
		
		decimal EvaluateMatching(PXGraph graph, CABankTran aDetail, CATran aTran, IMatchSettings aSettings);
		decimal EvaluateMatching(PXGraph graph, CABankTran aDetail, CABankTranInvoiceMatch aTran, IMatchSettings aSettings);
		decimal EvaluateMatching(PXGraph graph, CABankTran aDetail, CABankTranExpenseDetailMatch matchRow, IMatchSettings aSettings);
		decimal EvaluateMatching(PXGraph graph, string aStr1, string aStr2, bool aCaseSensitive, bool matchEmpty = true);
		decimal EvaluateTideMatching(PXGraph graph, string aStr1, string aStr2, bool aCaseSensitive, bool matchEmpty = true);
		
		decimal CompareDate(PXGraph graph, CABankTran aDetail, CATran aTran, double meanValue, double sigma);
		decimal CompareDate(PXGraph graph, CABankTran aDetail, CABankTranInvoiceMatch aTran, double meanValue, double sigma);
		decimal CompareDate(PXGraph graph, CABankTran aDetail, CABankTranExpenseDetailMatch aTran, double meanValue, double sigma);

		decimal CompareRefNbr(PXGraph graph, CABankTran aDetail, CATran aTran, bool looseCompare, IMatchSettings settings);
		decimal CompareRefNbr(PXGraph graph, CABankTran aDetail, CABankTranInvoiceMatch aTran, bool looseCompare);
		decimal CompareRefNbr(PXGraph graph, CABankTran aDetail, CABankTranExpenseDetailMatch aTran, bool looseCompare, IMatchSettings matchSettings);

		decimal ComparePayee(PXGraph graph, CABankTran aDetail, CATran aTran);
		decimal ComparePayee(PXGraph graph, CABankTran aDetail, CABankTranInvoiceMatch aTran);

		decimal CompareExpenseReceiptAmount(PXGraph graph, CABankTran bankTran, CABankTranExpenseDetailMatch receipt, IMatchSettings settings);
	}
}

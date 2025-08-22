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

namespace PX.Objects.CA
{
	public interface ICABankTransactionsDataProvider
	{
		PXResultset<CATranExt> SearchForMatchingTransactions(CABankTran aDetail, IMatchSettings aSettings, Pair<DateTime, DateTime> tranDateRange, string curyID);
		PXResultset<CABatch> SearchForMatchingCABatches(CABankTran aDetail, IMatchSettings aSettings, Pair<DateTime, DateTime> tranDateRange, string curyID, bool allowUnreleased);
		PXResultset<CABatchDetailOrigDocAggregate> SearchForMatchesInCABatches(string tranType, string batchNbr);
		PXResultset<CABankTranMatch> SearchForTranMatchForCABatch(string batchNbr);

		PXResult<ARInvoice, ARAdjust> FindARInvoiceByInvoiceInfo(CABankTran aRow);
		PXResult<APInvoice, APAdjust, APPayment> FindAPInvoiceByInvoiceInfo(CABankTran aRow);
		string GetStatus(CATran tran);
	}
}

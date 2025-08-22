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

using PX.Objects.AR.CCPaymentProcessing.Interfaces;
using PX.Objects.Extensions.PaymentTransaction;

namespace PX.Objects.AR.CCPaymentProcessing.Common
{
	public interface ICCTransactionsProcessor
	{
		void ProcessAuthorize(ICCPayment doc, IExternalTransaction tran);
		void ProcessAuthorizeCapture(ICCPayment doc, IExternalTransaction tran);
		void ProcessPriorAuthorizedCapture(ICCPayment doc, IExternalTransaction tran);
		void ProcessVoid(ICCPayment doc, IExternalTransaction tran);
		void ProcessVoidOrCredit(ICCPayment doc, IExternalTransaction tran);
		void ProcessCredit(ICCPayment doc, IExternalTransaction tran);
		void ProcessCaptureOnly(ICCPayment doc, IExternalTransaction tran);
		/// <summary>
		/// Update Level 3 data for transaction.
		/// </summary>
		/// <param name="doc">Document for processing.</param>
		/// <param name="tran">External transaction with Level 3 Data.</param>
		/// <returns></returns>
		void UpdateLevel3Data(Payment doc, IExternalTransaction tran);
	}
}

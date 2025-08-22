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

using PX.Objects.Extensions.PaymentTransaction;

namespace PX.Objects.AR.CCPaymentProcessing.Common
{
	public interface ICCPaymentProcessor
	{
		TranOperationResult Authorize(ICCPayment payment, bool aCapture);
		TranOperationResult Capture(ICCPayment payment, int? transactionId);
		TranOperationResult CaptureOnly(ICCPayment payment, string aAuthorizationNbr);
		TranOperationResult Credit(ICCPayment payment, string aExtRefTranNbr, string procCetnerId);
		TranOperationResult Credit(ICCPayment payment, int? transactionId);
		TranOperationResult Void(ICCPayment payment, int? transactionId);
		TranOperationResult VoidOrCredit(ICCPayment payment, int? transactionId);
		/// <summary>
		/// Update Level 3 data for transaction.
		/// </summary>
		/// <param name="payment">Document for processing.</param>
		/// <param name="transactionId">Transaction ID.</param>
		/// <returns></returns>
		TranOperationResult UpdateLevel3Data(Payment payment, int? transactionId);
	}
}

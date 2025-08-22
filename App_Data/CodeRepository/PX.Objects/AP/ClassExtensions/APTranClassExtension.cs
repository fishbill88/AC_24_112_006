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


namespace PX.Objects.AP
{
	public static class APTranClassExtension
	{
		public static void ClearInvoiceDetailsBalance(this APTran tran)
		{
			tran.CuryCashDiscBal = 0m;
			tran.CashDiscBal = 0m;

			tran.CuryRetainedTaxableAmt = 0m;
			tran.RetainedTaxableAmt = 0m;
			tran.CuryRetainedTaxAmt = 0m;
			tran.RetainedTaxAmt = 0m;

			tran.CuryRetainageBal = 0m;
			tran.RetainageBal = 0m;
			tran.CuryOrigRetainageAmt = 0m;
			tran.OrigRetainageAmt = 0m;

			tran.CuryOrigTranAmt = 0m;
			tran.OrigTranAmt = 0m;
			tran.CuryTranBal = 0m;
			tran.TranBal = 0m;

			tran.CuryOrigTaxableAmt = 0m;
			tran.OrigTaxableAmt = 0m;
			tran.CuryOrigTaxAmt = 0m;
			tran.OrigTaxAmt = 0m;
		}

		public static void RecoverInvoiceDetailsBalance(this APTran tran)
		{
			tran.CuryRetainageBal = tran.CuryOrigRetainageAmt;
			tran.RetainageBal = tran.OrigRetainageAmt;

			tran.CuryTranBal = tran.CuryOrigTranAmt;
			tran.TranBal = tran.OrigTranAmt;
		}
	}
}

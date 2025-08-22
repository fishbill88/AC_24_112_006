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
using PX.Objects.Common.GraphExtensions.Abstract.DAC;

namespace PX.Objects.Common.GraphExtensions.Abstract.Mapping
{
    public class PaidInvoiceMapping : IBqlMapping
    {
        /// <exclude />
        public Type Extension => typeof(PaidInvoice);
        /// <exclude />
        protected Type _table;
        /// <exclude />
        public Type Table => _table;

        public PaidInvoiceMapping(Type table)
        {
            _table = table;
        }

        /// <exclude />
        public Type BranchID = typeof(PaidInvoice.branchID);

        /// <exclude />
        public Type HeaderDocDate = typeof(PaidInvoice.headerDocDate);

        /// <exclude />
        public Type HeaderTranPeriodID = typeof(PaidInvoice.headerTranPeriodID);

		/// <exclude />
		public Type HeaderFinPeriodID = typeof(PaidInvoice.headerFinPeriodID);

		public Type DocType = typeof(PaidInvoice.docType);

        public Type RefNbr = typeof(PaidInvoice.refNbr);

        public Type CuryInfoID = typeof(PaidInvoice.curyInfoID);

        public Type Hold = typeof(PaidInvoice.hold);

        public Type Released = typeof(PaidInvoice.released);

        public Type Printed = typeof(PaidInvoice.printed);

        public Type OpenDoc = typeof(PaidInvoice.openDoc);

        public Type FinPeriodID = typeof(PaidInvoice.finPeriodID);

        public Type InvoiceNbr = typeof(PaidInvoice.invoiceNbr);

        public Type DocDesc = typeof(PaidInvoice.docDesc);

        public Type ContragentID = typeof(PaidInvoice.contragentID);

        public Type ContragentLocationID = typeof(PaidInvoice.contragentLocationID);

        public Type TaxZoneID = typeof(PaidInvoice.taxZoneID);

        public Type TaxCalcMode = typeof(PaidInvoice.taxCalcMode);

        public Type OrigModule = typeof(PaidInvoice.origModule);

	    public Type OrigDocType = typeof(Invoice.origDocType);

		public Type OrigRefNbr = typeof(PaidInvoice.origRefNbr);

        public Type CuryOrigDocAmt = typeof(PaidInvoice.curyOrigDocAmt);

        public Type CuryTaxAmt = typeof(PaidInvoice.curyTaxAmt);

        public Type CuryDocBal = typeof(PaidInvoice.curyDocBal);

        public Type CuryTaxTotal = typeof(PaidInvoice.curyTaxTotal);

        public Type CuryTaxRoundDiff = typeof(PaidInvoice.curyTaxRoundDiff);

        public Type CuryRoundDiff = typeof(PaidInvoice.curyRoundDiff);

        public Type TaxRoundDiff = typeof(PaidInvoice.taxRoundDiff);

        public Type RoundDiff = typeof(PaidInvoice.roundDiff);

        public Type TaxAmt = typeof(PaidInvoice.taxAmt);

        public Type DocBal = typeof(PaidInvoice.docBal);

        public Type Approved = typeof(PaidInvoice.approved);

        public Type CashAccountID = typeof(PaidInvoice.cashAccountID);

        public Type PaymentMethodID = typeof(PaidInvoice.paymentMethodID);

	    public Type CATranID = typeof(PaidInvoice.cATranID);

	    public Type ClearDate = typeof(PaidInvoice.clearDate);

	    public Type Cleared = typeof(PaidInvoice.cleared);

	    public Type ExtRefNbr = typeof(PaidInvoice.extRefNbr);
	}
}

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

using PX.Common;

namespace PX.Objects.Localizations.CA.Messages
{
    [PXLocalizable]
    public static class T5018Messages
    {
        public const string EightCharMax = "The value should not exceed 8 characters.";
        public const string SNFormat = "The SIN must be entered in the format of #########.";
        public const string BNFormat = "The Program Account Number format should be 123456789RT1234.";

		public const string
			T5018IndividualEmptyPrimary = "The primary contact must be specified if the vendor files the T5018 form as an individual.";
    
		public const string TaxYear = "Once you submit the T5018 report with a specific T5018 year type, subsequent returns must be filed using the same T5018 year type unless otherwise authorized in writing by the CRA.";
		public const string TaxYearImmutable = "T5018 Year Type cannot be changed while there is prepared data for this company in the system. Prepared data can be deleted on the Create T5018 E-File (AP507600) form.";
		public const string NoNewRows = "A new revision cannot be created because there are no new transactions for any of the vendors. The last revision will be displayed.";
		public const string NoPreviousSubmissions = "To use the Amendment filing type, mark one of the previous revisions as original submission by selecting the E-File Submitted to CRA check box for that revision.";
		internal const string NotLatestRevision = "Only the last revision for T5018 can be deleted.";

		public const string PrepareButton = "Prepare Data";
		public const string ReportButton = "View Validation Report";
		public const string GenerateButton = "Create E-File";

		public const string CalendarYear = "Calendar Year";
		public const string FiscalYear = "Fiscal Year";
		public const string Corporation = "Corporation";
		public const string Partnership = "Partnership";
		public const string Individual = "Individual";

		public const string Transmitter = "Transmitter";
		public const string T5018Year = "T5018 Tax Year";
		public const string Revision = "Revision";
		public const string T5018MasterTable = "T5018 Master Table";

		public const string NewValue = PX.Objects.AP.Messages.NewKey;

		public const string Original = "Original";
		public const string Amendment = "Amendment";
		public const string Amended = "Amended";
		public const string Canceled = "Canceled";
		internal const string English = "English";
		internal const string French = "French";
		
		public const string OrigDocAmtLessThanTaxTotal = "The original document amount is less than the document tax total. (AP301000) APInvoice type: {0}, reference number: {1}.";
		public const string EnableT5018Vendor = "Enable T5018 Reporting";
		public const string DisableT5018Vendor = "Disable T5018 Reporting";

		public const string ThresholdAmount = "Threshold Amount";
	}
}

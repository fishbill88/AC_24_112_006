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

namespace PX.Objects.CN.Subcontracts.PO.Descriptor
{
    [PXLocalizable(Prefix)]
    public static class Messages
    {
        public const string InvalidAssignmentMap = "Invalid assignment map";
        public const string OnlyPurchaseOrdersAreAllowedMessage = "Only Purchase Orders are allowed.";
        public const string NoteFilesFieldName = "NoteFiles";

        private const string Prefix = "PO Error";

        public static class PoSetup
        {
            public const string SubcontractNumberingName = "SUBCONTR";
            public const string SubcontractNumberingId = "Subcontract Numbering Sequence";
            public const string RequireSubcontractControlTotal = "Validate Total on Entry";
            public const string SubcontractRequireApproval = "Require Approval";
        }
    }
}
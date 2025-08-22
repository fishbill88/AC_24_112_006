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
using System.Linq;
using PX.Data;
using PX.Objects.AR;

namespace PX.Objects.CN.Compliance.CL.Descriptor.Attributes.ComplianceDocumentRefNote.ComplianceDocumentEntityStrategies
{
    public class ArInvoiceStrategy : ComplianceDocumentEntityStrategy
    {
        public ArInvoiceStrategy()
        {
            EntityType = typeof(ARInvoice);
            FilterExpression = typeof(Where<ARInvoice.docType, Equal<ARDocType.invoice>,
                Or<ARInvoice.docType, Equal<ARDocType.debitMemo>,
                    Or<ARInvoice.docType, Equal<ARDocType.creditMemo>,
                        Or<ARInvoice.docType, Equal<ARDocType.finCharge>,
                            Or<ARInvoice.docType, Equal<ARDocType.smallCreditWO>
                            >>>>>);
            TypeField = typeof(ARInvoice.docType);
        }

        public override Guid? GetNoteId(PXGraph graph, string clDisplayName)
        {
            var key = ComplianceReferenceTypeHelper.ConvertToDocumentKey<ARInvoice>(clDisplayName);

            var noteId = new PXSelect<ARInvoice,
                Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
                And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>(graph)
                .Select(key.DocType, key.RefNbr)
                .FirstTableItems
                .ToList()
                .SingleOrDefault()
                ?.NoteID;

            return noteId;
        }
    }
}
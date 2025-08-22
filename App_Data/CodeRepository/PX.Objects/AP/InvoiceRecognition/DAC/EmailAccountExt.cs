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
using PX.Data;
using PX.Data.BQL;
using PX.SM;

namespace PX.Objects.AP.InvoiceRecognition.DAC
{
    [PXInternalUseOnly]
    public sealed class EMailAccountExt : PXCacheExtension<EMailAccount>
    {
        [PXDBBool]
        [PXUIField(DisplayName = "Submit Incoming Documents")]
        public bool? SubmitToIncomingAPDocuments { get; set; }
        public abstract class submitToIncomingAPDocuments : BqlBool.Field<submitToIncomingAPDocuments> { }
    }
}

using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;

namespace CompiledVersion.DAC
{
    [PXCacheName(Messages.ARInvoiceExtension)]
    public sealed class ARInvoiceExt : PXCacheExtension<PX.Objects.AR.ARInvoice>
    {
        public static bool IsActive() => true;

        #region UsrEmail    
        [PXString(200, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.Email, Enabled = true)]
        [PXFormula(typeof(Search<Contact.eMail, Where<Contact.bAccountID, Equal<Current<ARInvoice.customerID>>>>))]
        [PXFormula(typeof(Default<ARInvoice.customerID>))]
        public string UsrEmail { get; set; }
        public abstract class usrEmail : PX.Data.BQL.BqlString.Field<usrEmail> { }
        #endregion
    }
}



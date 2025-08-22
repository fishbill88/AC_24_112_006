using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubspotCustomization
{
    [PXCacheName(Messages.ARInvoiceExtension)]
    public class ARInvoiceExt : PXCacheExtension<PX.Objects.AR.ARInvoice>
    {
        public static bool IsActive() => true;
        #region UsrEmail    
        [PXString(200, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.Email, Enabled = true)]
        [PXFormula(typeof(Search<Contact.eMail,Where<Contact.bAccountID,Equal<Current<ARInvoice.customerID>>>>))]
        [PXFormula(typeof(Default<ARInvoice.customerID>))]
        public virtual string UsrEmail { get; set; }
        public abstract class usrEmail : PX.Data.BQL.BqlString.Field<usrEmail> { }
        #endregion
    }
}

using PX.Data;
using PX.Data.BQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubspotCustomization
{
    public sealed class CRQuoteStandaloneExt : PXCacheExtension<PX.Objects.CR.Standalone.CRQuote>
    {
        public static bool IsActive() => true;

        #region UsrHubspotDealID    
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.HubspotDealID, Enabled = false)]
        public string UsrHubspotDealID { get; set; }
        public abstract class usrHubspotDealID : PX.Data.BQL.BqlString.Field<usrHubspotDealID> { }
        #endregion
    }
    public sealed class CRQuoteExt : PXCacheExtension<PX.Objects.CR.CRQuote>
    {
        public static bool IsActive() => true;

        #region UsrHubspotDealID   
        [PXDBString(BqlField = typeof(CRQuoteStandaloneExt.usrHubspotDealID))]
        [PXUIField(DisplayName = Messages.HubspotDealID, Enabled = false)]
        public string UsrHubspotDealID { get; set; }
        public abstract class usrHubspotDealID : BqlType<IBqlString, String>.Field<usrHubspotDealID>
        #endregion
        {
        }
    }
}
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubspotCustomization
{
    [PXCacheName(Messages.CROpportunityExtension)]
    public sealed class CROpportunityExt : PXCacheExtension<PX.Objects.CR.CROpportunity>
    {
        public static bool IsActive() => true;

        #region UsrHubspotDealID
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.HubspotDealID)]
        public string UsrHubspotDealID { get; set; }
        public abstract class usrHubspotDealID : PX.Data.BQL.BqlString.Field<usrHubspotDealID> { }
        #endregion
    }
    
    [PXCacheName(Messages.CROpportunityExtension)]
    public sealed class CROpportunityStandaloneExt : PXCacheExtension<PX.Objects.CR.Standalone.CROpportunity>
    {
        public static bool IsActive() => true;

        #region UsrHubspotDealID
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.HubspotDealID)]
        public string UsrHubspotDealID { get; set; }
        public abstract class usrHubspotDealID : PX.Data.BQL.BqlString.Field<usrHubspotDealID> { }
        #endregion
    }
}
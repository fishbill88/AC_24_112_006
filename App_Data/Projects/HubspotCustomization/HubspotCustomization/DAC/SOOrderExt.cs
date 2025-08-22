using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubspotCustomization
{
    [PXCacheName(Messages.SOOrderExtension)]
    public class SOOrderExt : PXCacheExtension<PX.Objects.SO.SOOrder>
    {
        public static bool IsActive() => true;

        #region UsrHubspotDealID    
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.HubspotDealID, Enabled = true)]
        public virtual string UsrHubspotDealID { get; set; }
        public abstract class usrHubspotDealID : PX.Data.BQL.BqlString.Field<usrHubspotDealID> { }
        #endregion
    }
}

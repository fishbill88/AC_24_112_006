using PX.Data;
using PX.Objects.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKTechCustomization
{
    public class POFixedDemandExt : PXCacheExtension<POFixedDemand>
    {
        public static bool IsActive() => true;

        #region UsrSWKRTHCost
        [PXDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Cost", Enabled = false)]
        public virtual decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion
    }
}

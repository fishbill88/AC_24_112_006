using PX.Data;
using System;

namespace ItemRequestCustomization
{
    public class InventoryItemExt : PXCacheExtension<PX.Objects.IN.InventoryItem>
    {
        #region UsrSWKRTHCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "SWKRTH Cost")]
        public virtual decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion
    }
}
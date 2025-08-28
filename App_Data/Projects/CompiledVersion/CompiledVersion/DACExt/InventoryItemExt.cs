using PX.Data;
using System;

namespace CompiledVersion.DAC
{
    public sealed class InventoryItemExt : PXCacheExtension<PX.Objects.IN.InventoryItem>
    {
        public static bool IsActive() => true;
        #region UsrSWKRTHCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "SWKRTH Cost")]
        public decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion

        #region UsrItemSpecs
        [PXDBString(2000, IsUnicode = true)]
        [PXUIField(DisplayName = "Item Specs")]
        public string UsrItemSpecs { get; set; }
        public abstract class usrItemSpecs : PX.Data.BQL.BqlString.Field<usrItemSpecs> { }
        #endregion
    }
}
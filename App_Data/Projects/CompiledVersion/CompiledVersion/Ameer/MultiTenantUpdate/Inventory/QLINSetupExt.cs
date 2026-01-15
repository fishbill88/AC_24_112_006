using PX.Data;
using PX.Objects.IN;

namespace MTUInventory
{
    // Phase 3 - Setup Extension for Multi-Tenant Control
    // Adds checkboxes to Inventory Setup to enable/disable multi-tenant sync
    
    public class QLINSetupExt : PXCacheExtension<INSetup>
    {
        public static bool IsActive() => true;

        #region UsrMultiTenantStockItem
        [PXDBBool]
        [PXUIField(DisplayName = "Multi Tenant Stock Item")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrMultiTenantStockItem { get; set; }
        public abstract class usrMultiTenantStockItem : PX.Data.BQL.BqlBool.Field<usrMultiTenantStockItem> { }
        #endregion

        #region UsrMultiTenantNonStockItem
        [PXDBBool]
        [PXUIField(DisplayName = "Multi Tenant Non-Stock Item")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrMultiTenantNonStockItem { get; set; }
        public abstract class usrMultiTenantNonStockItem : PX.Data.BQL.BqlBool.Field<usrMultiTenantNonStockItem> { }
        #endregion
    }
}

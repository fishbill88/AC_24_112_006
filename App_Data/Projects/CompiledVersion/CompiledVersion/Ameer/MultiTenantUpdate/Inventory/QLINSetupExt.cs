using PX.Data;
using PX.Objects.IN;

namespace MTUInventory
{
    // Phase 3 - Setup Extension for Multi-Tenant Control
    // Adds checkboxes to Inventory Setup to enable/disable multi-tenant sync
    
    public class QLINSetupExt : PXCacheExtension<INSetup>
    {
        public static bool IsActive() => true;

        #region UsrMultiTenantStockItemSend
        [PXDBBool]
        [PXUIField(DisplayName = "Send Stock Item Updates")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrMultiTenantStockItemSend { get; set; }
        public abstract class usrMultiTenantStockItemSend : PX.Data.BQL.BqlBool.Field<usrMultiTenantStockItemSend> { }
        #endregion

        #region UsrMultiTenantStockItemReceive
        [PXDBBool]
        [PXUIField(DisplayName = "Receive Stock Item Updates")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrMultiTenantStockItemReceive { get; set; }
        public abstract class usrMultiTenantStockItemReceive : PX.Data.BQL.BqlBool.Field<usrMultiTenantStockItemReceive> { }
        #endregion

        #region UsrMultiTenantNonStockItemSend
        [PXDBBool]
        [PXUIField(DisplayName = "Send Non-Stock Item Updates")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrMultiTenantNonStockItemSend { get; set; }
        public abstract class usrMultiTenantNonStockItemSend : PX.Data.BQL.BqlBool.Field<usrMultiTenantNonStockItemSend> { }
        #endregion

        #region UsrMultiTenantNonStockItemReceive
        [PXDBBool]
        [PXUIField(DisplayName = "Receive Non-Stock Item Updates")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrMultiTenantNonStockItemReceive { get; set; }
        public abstract class usrMultiTenantNonStockItemReceive : PX.Data.BQL.BqlBool.Field<usrMultiTenantNonStockItemReceive> { }
        #endregion

        #region UsrEnableVerboseSyncLogging
        [PXDBBool]
        [PXUIField(DisplayName = "Enable Verbose Sync Logging")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrEnableVerboseSyncLogging { get; set; }
        public abstract class usrEnableVerboseSyncLogging : PX.Data.BQL.BqlBool.Field<usrEnableVerboseSyncLogging> { }
        #endregion

        #region UsrMultiTenantStockItem (Deprecated)
        [PXDBBool]
        [PXUIField(DisplayName = "Multi Tenant Stock Item (Deprecated)", Visible = false)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrMultiTenantStockItem { get; set; }
        public abstract class usrMultiTenantStockItem : PX.Data.BQL.BqlBool.Field<usrMultiTenantStockItem> { }
        #endregion

        #region UsrMultiTenantNonStockItem (Deprecated)
        [PXDBBool]
        [PXUIField(DisplayName = "Multi Tenant Non-Stock Item (Deprecated)", Visible = false)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrMultiTenantNonStockItem { get; set; }
        public abstract class usrMultiTenantNonStockItem : PX.Data.BQL.BqlBool.Field<usrMultiTenantNonStockItem> { }
        #endregion
    }
}

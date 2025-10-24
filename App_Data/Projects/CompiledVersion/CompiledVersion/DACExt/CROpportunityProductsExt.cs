using PX.Data;
using PX.Objects.CR;
using PX.Objects.IN;
using System;
using CompiledVersion.DAC; // for InventoryItemExt (RTH Cost)

namespace CompiledVersion
{
    public sealed class CROpportunityProductsExt : PXCacheExtension<CROpportunityProducts>
    {
        public static bool IsActive() => true;

        #region UsrSWKRTHCost
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "RTH Cost", Enabled = false)]
        // Refresh when Inventory changes
        [PXFormula(typeof(Default<CROpportunityProducts.inventoryID>))]
        // Persist default from InventoryItemExt when InventoryID is set (only if null)
        [PXDefault(
        typeof(Search<InventoryItemExt.usrSWKRTHCost,
        Where<InventoryItem.inventoryID, Equal<Current<CROpportunityProducts.inventoryID>>>>),
        PersistingCheck = PXPersistingCheck.Nothing)]
        public decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion

        #region UsrSWKSPCCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "SPC Cost")]
        public decimal? UsrSWKSPCCost { get; set; }
        public abstract class usrSWKSPCCost : PX.Data.BQL.BqlDecimal.Field<usrSWKSPCCost> { }
        #endregion

        #region UsrSWKSPCCode
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "SPC Code")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public string UsrSWKSPCCode { get; set; }
        public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
        #endregion
    }
}

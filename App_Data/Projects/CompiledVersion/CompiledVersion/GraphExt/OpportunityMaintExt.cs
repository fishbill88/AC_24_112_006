using CompiledVersion.DAC;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.IN;
using System;

namespace CompiledVersion
{
    public class OpportunityMaintExt : PXGraphExtension<OpportunityMaint>
    {
        public static bool IsActive() => true;

        // Default CuryUnitCost to RTH Cost, unless SPC Cost is provided
        protected virtual void _(Events.FieldDefaulting<CROpportunityProducts, CROpportunityProducts.curyUnitCost> e)
        {
            var row = e.Row; if (row == null) return;
            var rowExt = e.Row.GetExtension<CROpportunityProductsExt>();
            decimal spc = rowExt?.UsrSWKSPCCost ?? 0m;
            if (spc > 0m)
            {
                e.NewValue = spc;
                e.Cancel = true;
                return;
            }
            if (row.InventoryID == null) return;
            var item = InventoryItem.PK.Find(Base, row.InventoryID);
            var itemExt = item?.GetExtension<InventoryItemExt>();
            if (item == null) return;
            decimal rth = itemExt?.UsrSWKRTHCost ?? 0m;
            if (rth <= 0m) return;
            // UOM conversion if needed
            if (!string.IsNullOrEmpty(row.UOM) && !string.IsNullOrEmpty(item.BaseUnit) && !string.Equals(row.UOM, item.BaseUnit, StringComparison.OrdinalIgnoreCase))
            {
                INUnit conv = PXSelect<INUnit,
                Where<INUnit.inventoryID, Equal<Required<INUnit.inventoryID>>,
                And<INUnit.fromUnit, Equal<Required<INUnit.fromUnit>>,
                And<INUnit.toUnit, Equal<Required<INUnit.toUnit>>>>>>
                .Select(Base, item.InventoryID, item.BaseUnit, row.UOM);
                if (conv != null && conv.UnitRate != null && conv.UnitRate != 0)
                {
                    // rth is per base unit, convert to row.UOM
                    if (string.Equals(conv.UnitMultDiv, "M", StringComparison.OrdinalIgnoreCase))
                        rth = rth * (conv.UnitRate ?? 1m);
                    else
                        rth = rth / (conv.UnitRate ?? 1m);
                }
            }
            e.NewValue = rth;
            e.Cancel = true;
        }

        protected virtual void _(Events.FieldUpdated<CROpportunityProducts, CROpportunityProducts.inventoryID> e)
        {
            var row = e.Row; if (row == null) return;
            // populate RTH field
            var item = InventoryItem.PK.Find(Base, row.InventoryID);
            var itemExt = item?.GetExtension<InventoryItemExt>();
            var lineExt = row.GetExtension<CROpportunityProductsExt>();
            if (lineExt != null)
            {
                lineExt.UsrSWKRTHCost = itemExt?.UsrSWKRTHCost ?? 0m;
            }
            // reset unit cost to default from RTH/SPC
            e.Cache.SetDefaultExt<CROpportunityProducts.curyUnitCost>(row);
        }

        protected virtual void _(Events.FieldUpdated<CROpportunityProducts, CROpportunityProducts.uOM> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<CROpportunityProducts.curyUnitCost>(e.Row);
        }

        protected virtual void _(Events.FieldUpdated<CROpportunityProducts, CROpportunityProductsExt.usrSWKSPCCost> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<CROpportunityProducts.curyUnitCost>(e.Row);
        }
    }
}

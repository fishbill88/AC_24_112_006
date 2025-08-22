using PX.Data;
using PX.Objects.SO;
using PX.Objects.IN;
using ItemRequestCustomization;

namespace SWKTechCustomization
{
    public class SOOrderEntryExt : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() => true;

        protected virtual void _(Events.FieldDefaulting<SOLine, SOLine.curyUnitCost> e)
        {
            if (e.Row == null) return;

            SOLine line = e.Row;

            // Get the inventory item
            InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
            if (item?.InventoryID != null)
            {
                var itemExt = item.GetExtension<InventoryItemExt>();
                if ((itemExt?.UsrSWKRTHCost ?? 0m) > 0m)
                {
                    // Calculate Unit Cost with UOM conversion
                    decimal calculatedCost = CalculateUnitCostWithUOM(line, item, itemExt.UsrSWKRTHCost.Value);
                    e.NewValue = calculatedCost;
                    e.Cancel = true;
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.inventoryID> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.uOM> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLineExt.usrSWKSPCCost> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        // Recalculate when Warehouse changes
        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.siteID> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        // Recalculate when Subitem changes (often part of cost/stock context)
        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.subItemID> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        // If branch affects pricing context in your setup, keep this. Otherwise you may remove it.
        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.branchID> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        #region Helper Methods

        private void TryRecalculateUnitCost(PXCache cache, SOLine line)
        {
            if (line == null || line.InventoryID == null)
                return;

            InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
            if (item?.InventoryID == null)
                return;

            var itemExt = item.GetExtension<InventoryItemExt>();
            var soLineExt = line.GetExtension<SOLineExt>();
            soLineExt.UsrSWKRTHCost = (itemExt?.UsrSWKRTHCost ?? 0m);
            // If neither RTH nor SPC cost provided, do nothing
            if ((itemExt?.UsrSWKRTHCost ?? 0m) <= 0m && (soLineExt?.UsrSWKSPCCost ?? 0m) <= 0m)
                return;

            decimal rthCost = (itemExt?.UsrSWKRTHCost ?? 0m);
            decimal calculatedCost = CalculateUnitCostWithUOM(line, item, rthCost);

            // Set CuryUnitCost so ExtCost recalculates via PXFormula
            cache.SetValueExt<SOLine.curyUnitCost>(line, calculatedCost);
        }

        protected virtual decimal CalculateUnitCostWithUOM(SOLine line, InventoryItem item, decimal rthCost)
        {
            SOLineExt soLineExt = line.GetExtension<SOLineExt>();
            if ((soLineExt?.UsrSWKSPCCost ?? 0m) > 0m)
                return (soLineExt?.UsrSWKSPCCost ?? 0m);

            if (line?.UOM == null || item?.BaseUnit == null)
                return rthCost;

            // If SO line UOM == item base UOM, return RTH Cost as-is
            if (string.Equals(line.UOM, item.BaseUnit, System.StringComparison.OrdinalIgnoreCase))
            {
                return rthCost;
            }

            // If SO line UOM != item base UOM, apply UOM conversion
            // Find the conversion rate from SO line UOM to base UOM
            INUnit conversion = PXSelect<INUnit,
                Where<INUnit.inventoryID, Equal<Required<INUnit.inventoryID>>,
                    And<INUnit.fromUnit, Equal<Required<INUnit.fromUnit>>,
                    And<INUnit.toUnit, Equal<Required<INUnit.toUnit>>>>>>
                .Select(Base, item.InventoryID, line.UOM, item.BaseUnit);

            if (conversion != null && conversion.UnitRate != null && conversion.UnitRate != 0)
            {
                // Apply UOM conversion based on UnitMultDiv
                if (conversion.UnitMultDiv == "M") // Multiply
                {
                    return rthCost * conversion.UnitRate.Value;
                }
                else if (conversion.UnitMultDiv == "D") // Divide
                {
                    return rthCost / conversion.UnitRate.Value;
                }
            }

            // If no conversion found, return original RTH Cost
            return rthCost;
        }

        #endregion
    }
}
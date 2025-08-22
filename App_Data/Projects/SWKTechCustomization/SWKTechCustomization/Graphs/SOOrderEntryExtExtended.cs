using PX.Data;
using PX.Objects.SO;
using PX.Objects.PO;
using PX.Objects.CS;

namespace SWKTechCustomization
{
    public class SOOrderEntryExtExtended : PXGraphExtension<SOOrderEntryExt, PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() => true;
        public bool SuppressCodeRequired = false;
        #region Event Handlers

        protected virtual void _(Events.FieldUpdated<SOLine, SOLineExt.usrSWKSPCCost> e)
        {
            if (e.Row == null) return;

            var lineExt = e.Row.GetExtension<SOLineExt>();
            if (lineExt?.UsrSWKSPCCost > 0)
            {
                // Automatically check the Manual Cost checkbox when SPC Cost > 0
                lineExt.UsrSWKManualCost = true;
                
                // Force validation of SPC Code field
                try
                {
                    if (!SuppressCodeRequired)
                        e.Cache.RaiseExceptionHandling<SOLineExt.usrSWKSPCCode>(e.Row, lineExt.UsrSWKSPCCode,
                            string.IsNullOrEmpty(lineExt.UsrSWKSPCCode) ?
                            new PXSetPropertyException(Messages.SPCCodeRequired, PXErrorLevel.Error) :
                            null);
                }
                catch { }
            }
            else if (lineExt?.UsrSWKSPCCost == 0)
            {
                // Clear SPC Code when SPC Cost is zero
                lineExt.UsrSWKSPCCode = null;
                lineExt.UsrSWKManualCost = false;
            }

            // Recalculate Extended Cost
            RecalculateExtendedCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldVerifying<SOLine, SOLineExt.usrSWKSPCCode> e)
        {
            if (e.Row == null) return;

            // Skip validation if this is an import scenario
            if (e.Cache.Graph.IsImport || Base.IsImport)
                return;

            var lineExt = e.Row.GetExtension<SOLineExt>();
            if (lineExt?.UsrSWKSPCCost > 0 && string.IsNullOrEmpty((string)e.NewValue))
            {
                if (!SuppressCodeRequired)
                    throw new PXSetPropertyException(Messages.SPCCodeRequired, PXErrorLevel.Error);
            }
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLineExt.usrSWKSPCCode> e)
        {
            if (e.Row == null) return;

            var lineExt = e.Row.GetExtension<SOLineExt>();
            if (!string.IsNullOrEmpty(lineExt?.UsrSWKSPCCode) && (lineExt.UsrSWKSPCCost ?? 0) == 0)
            {
                // If SPC Code is entered but SPC Cost is 0, clear the code
                lineExt.UsrSWKSPCCode = null;
                if (!SuppressCodeRequired)
                    e.Cache.RaiseExceptionHandling<SOLineExt.usrSWKSPCCode>(e.Row, null,
                    new PXSetPropertyException(Messages.SPCCodeOnlyWithCost, PXErrorLevel.Warning));
            }
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.orderQty> e)
        {
            if (e.Row == null) return;

            // Recalculate Extended Cost when quantity changes
            RecalculateExtendedCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.curyUnitCost> e)
        {
            if (e.Row == null) return;

            // Recalculate Extended Cost when unit cost changes
            RecalculateExtendedCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.RowSelected<SOLine> e)
        {
            if (e.Row == null) return;

            var lineExt = e.Row.GetExtension<SOLineExt>();
            
            // Enable/disable SPC Code based on SPC Cost
            PXUIFieldAttribute.SetEnabled<SOLineExt.usrSWKSPCCode>(e.Cache, e.Row, 
                (lineExt?.UsrSWKSPCCost ?? 0) > 0);
                
            // Manual Cost checkbox is always disabled (read-only)
            PXUIFieldAttribute.SetEnabled<SOLineExt.usrSWKManualCost>(e.Cache, e.Row, false);
        }

        protected virtual void _(Events.RowPersisting<SOLine> e)
        {
            if (e.Row == null) return;

            // Skip validation if this is an import scenario
            if (e.Cache.Graph.IsImport || Base.IsImport)
                return;

            var lineExt = e.Row.GetExtension<SOLineExt>();
            
            // Validate SPC Code is provided when SPC Cost > 0
            if ((lineExt?.UsrSWKSPCCost ?? 0) > 0 && string.IsNullOrEmpty(lineExt?.UsrSWKSPCCode))
            {
                if (!SuppressCodeRequired)
                    e.Cache.RaiseExceptionHandling<SOLineExt.usrSWKSPCCode>(e.Row, lineExt?.UsrSWKSPCCode,
                    new PXSetPropertyException(Messages.SPCCodeRequired, PXErrorLevel.Error));
            }
        }

        // Override the Extended Cost calculation
        [PXMergeAttributes]
        [PXFormula(typeof(Switch<
            Case<Where<SOLineExt.usrSWKSPCCost, Greater<decimal0>>, 
                Mult<SOLine.orderQty, SOLineExt.usrSWKSPCCost>>,
            Mult<SOLine.orderQty, SOLine.unitCost>>))]
        protected virtual void _(Events.CacheAttached<SOLine.extCost> e) { }

        #endregion

        #region Helper Methods

        private void RecalculateExtendedCost(PXCache cache, SOLine line)
        {
            if (line == null) return;

            var lineExt = line.GetExtension<SOLineExt>();
            
            // Calculate Extended Cost: 
            // If SPC Cost > 0, use SPC Cost * Quantity (no UOM conversion)
            // Otherwise, use standard Unit Cost * Quantity calculation
            decimal extendedCost = 0m;
            
            if ((lineExt?.UsrSWKSPCCost ?? 0) > 0)
            {
                // Use SPC Cost without UOM calculation
                extendedCost = (lineExt.UsrSWKSPCCost ?? 0) * (line.OrderQty ?? 0);
            }
            else
            {
                // Use standard calculation: Unit Cost * Quantity
                extendedCost = (line.UnitCost ?? 0) * (line.OrderQty ?? 0);
            }

            // Set the Extended Cost
            cache.SetValueExt<SOLine.curyExtCost>(line, extendedCost);
        }

        #endregion
    }
}
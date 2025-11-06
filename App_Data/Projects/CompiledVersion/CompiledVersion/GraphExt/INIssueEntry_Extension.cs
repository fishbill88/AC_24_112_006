using PX.Data;
using PX.Objects.IN;
using PX.Objects.SO;
using System;

namespace CompiledVersion.GraphExt
{
    public class INIssueEntry_Extension : PXGraphExtension<INIssueEntry>
    {
        public static bool IsActive() => true;

      /// <summary>
        /// Event handler that ensures INTran records created from SO Lines 
   /// use the Unit Cost (CuryUnitCost) and Extended Cost (CuryExtCost) from the Sales Order Line.
        /// This is triggered when Update IN is clicked from SOShipmentEntry.
        /// </summary>
        protected virtual void _(Events.RowInserting<INTran> e)
  {
      if (e.Row == null) return;

            INTran tran = e.Row;

 // Only process if this transaction is related to a sales order
  if (string.IsNullOrEmpty(tran.SOOrderType) || string.IsNullOrEmpty(tran.SOOrderNbr))
       return;

      // Find the original SOLine to get the cost
       SOLine soLine = PXSelect<SOLine,
            Where<SOLine.orderType, Equal<Required<INTran.sOOrderType>>,
        And<SOLine.orderNbr, Equal<Required<INTran.sOOrderNbr>>,
     And<SOLine.lineNbr, Equal<Required<INTran.sOOrderLineNbr>>>>>>
         .Select(Base, tran.SOOrderType, tran.SOOrderNbr, tran.SOOrderLineNbr);

    if (soLine == null) return;

 // Override the costs with values from the sales order line
        // Map SOLine.CuryUnitCost to INTran.UnitCost
     if (soLine.CuryUnitCost != null && soLine.CuryUnitCost != 0m)
            {
    tran.UnitCost = soLine.CuryUnitCost;
         }

            // Map SOLine.CuryExtCost to INTran.TranCost (Extended Cost)
            if (soLine.CuryExtCost != null && soLine.CuryExtCost != 0m)
            {
      tran.TranCost = soLine.CuryExtCost;
       }
        }

        /// <summary>
        /// Event handler to prevent the system from recalculating costs 
     /// after we've set them from the sales order.
        /// </summary>
  protected virtual void _(Events.FieldDefaulting<INTran, INTran.unitCost> e)
        {
            if (e.Row == null) return;

            INTran tran = e.Row;

          // If this is from a sales order and unitCost is already set, don't recalculate
            if (!string.IsNullOrEmpty(tran.SOOrderNbr) && tran.UnitCost != null && tran.UnitCost != 0m)
       {
                e.NewValue = tran.UnitCost;
       e.Cancel = true;
            }
        }

        /// <summary>
        /// Event handler to prevent the system from recalculating extended costs 
        /// after we've set them from the sales order.
        /// </summary>
        protected virtual void _(Events.FieldDefaulting<INTran, INTran.tranCost> e)
 {
        if (e.Row == null) return;

            INTran tran = e.Row;

 // If this is from a sales order and tranCost is already set, don't recalculate
if (!string.IsNullOrEmpty(tran.SOOrderNbr) && tran.TranCost != null && tran.TranCost != 0m)
   {
     e.NewValue = tran.TranCost;
      e.Cancel = true;
            }
        }
}
}

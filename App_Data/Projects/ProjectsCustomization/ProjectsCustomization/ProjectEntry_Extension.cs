using PX.Data;
using PX.Objects.PM;
using System;

namespace ProjectsCustomization
{
    public class ProjectEntry_Extension : PXGraphExtension<PX.Objects.PM.ProjectEntry>
    {
        public static bool IsActive() => true; // example only

        protected virtual void _(Events.RowSelecting<PMCostBudget> e)
        {
            var row = e.Row;
            if (row == null)
                return;

            var ext = row.GetExtension<PMCostBudgetExt>();
            if (ext == null)
                return;

            decimal revised = row.CuryRevisedAmount.GetValueOrDefault();
            decimal committed = row.CuryCommittedAmount.GetValueOrDefault();

            ext.UsrPercentCommitted = revised != 0m ? (committed / revised) : 0m;
        }
    }
}

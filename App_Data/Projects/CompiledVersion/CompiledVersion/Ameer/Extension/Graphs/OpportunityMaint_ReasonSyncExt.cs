using CompiledVersion.DAC;
using PX.Data;
using PX.Objects.CR;
using System;

namespace CompiledVersion.GraphExt
{
    public class OpportunityMaint_ReasonSyncExt : PXGraphExtension<OpportunityMaint>
    {
        public static bool IsActive() => false;

        // Sync UsrResolution to Resolution when user changes UsrResolution
        protected virtual void _(Events.FieldUpdated<CROpportunity, CROpportunityReasonExt.usrResolution> e)
        {
            var row = e.Row;
            if (row == null) return;

            var ext = row.GetExtension<CROpportunityReasonExt>();
            if (ext == null) return;

            // Only update if values differ
            if (row.Resolution != ext.UsrResolution)
            {
                e.Cache.SetValueExt<CROpportunity.resolution>(row, ext.UsrResolution);
            }
        }

        // Sync Resolution to UsrResolution when row is selected from database
        protected virtual void _(Events.RowSelecting<CROpportunity> e)
        {
            var row = e.Row;
            if (row == null) return;

            var ext = row.GetExtension<CROpportunityReasonExt>();
            if (ext == null) return;

            // Use PXConnectionScope to ensure we're in a proper database context
            using (new PXConnectionScope())
            {
                // Only update if values differ
                if (ext.UsrResolution != row.Resolution)
                {
                    ext.UsrResolution = row.Resolution;
                }
            }
        }
    }
}

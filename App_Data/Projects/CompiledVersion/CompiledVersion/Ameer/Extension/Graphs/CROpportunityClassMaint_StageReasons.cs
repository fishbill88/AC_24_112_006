using PX.Data;
using PX.Data;
using PX.Objects.CR;
using System.Collections.Generic;
using System;
using PX.Data.BQL.Fluent;
using CompiledVersion.DAC;

namespace CompiledVersion.GraphExt
{
    //skip documentation for this
    public class CROpportunityClassMaint_StageReasons : PXGraphExtension<CROpportunityClassMaint>
    {
        public static bool IsActive() => false;

        [PXViewName("Reasons")]
        public SelectFrom<CROpportunityClassStageReason>
        .Where<CROpportunityClassStageReason.classID.IsEqual<CROpportunityClass.cROpportunityClassID.FromCurrent>>
        .View Reasons;

        protected virtual void _(Events.RowSelected<CROpportunityClass> e)
        {
            // placeholder if any UI enable/disable needed
        }

        protected virtual void _(Events.FieldDefaulting<CROpportunityClassStageReason.stageDescription> e)
        {
            var row = (CROpportunityClassStageReason)e.Row;
            if (row == null || string.IsNullOrEmpty(row.StageCode)) return;
            CROpportunityProbability prob = PXSelect<CROpportunityProbability,
            PX.Data.Where<CROpportunityProbability.stageCode, PX.Data.Equal<PX.Data.Required<CROpportunityProbability.stageCode>>>>
            .Select(Base, row.StageCode);
            e.NewValue = prob?.Name;
            e.Cancel = true;
        }

        // Populate ReasonDescription for CROpportunityClassStageReason on RowSelecting
        protected virtual void _(Events.FieldUpdated<CROpportunityClassStageReason, CROpportunityClassStageReason.reason> e)
        {
            var row = e.Row;
            if (row == null) return;
            var reasonCode = row.Reason;
            if (string.IsNullOrWhiteSpace(reasonCode))
            {
                row.ReasonDescription = "NO DATA FOUND";
            }
            else if (OpportunityMaint_StageReasonExt.ReasonCodeCatalog.TryGetValue(reasonCode.Trim(), out var description))
            {
                row.ReasonDescription = description;
            }
            else
            {
                row.ReasonDescription = "NO DATA FOUND";
            }
        }
        protected virtual void _(Events.RowSelecting<CROpportunityClassStageReason> e)
        {
            var row = e.Row;
            if (row == null) return;

            using (new PXConnectionScope())
            {
                var reasonCode = row.Reason;
                if (string.IsNullOrWhiteSpace(reasonCode))
                {
                    row.ReasonDescription = "NO DATA FOUND";
                }
                else if (OpportunityMaint_StageReasonExt.ReasonCodeCatalog.TryGetValue(reasonCode.Trim(), out var description))
                {
                    row.ReasonDescription = description;
                }
                else
                {
                    row.ReasonDescription = "NO DATA FOUND";
                }
            }
        }
    }
}


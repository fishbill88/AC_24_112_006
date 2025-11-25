//using CompiledVersion.Attributes;
using CompiledVersion.DAC;
using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.CR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompiledVersion
{
    public class OpportunityMaint_StageReasonExt : PXGraphExtension<OpportunityMaint>
    {
        public static bool IsActive() => false;

        // Keep list current on row selected - set list on UsrResolution
        protected virtual void _(Events.RowSelected<CROpportunity> e)
        {
            var row = e.Row; if (row == null) return;
            var ext = row.GetExtension<CROpportunityReasonExt>(); if (ext == null) return;
            var list = BuildReasonList(Base, row.ClassID, row.StageID);

            // Always hide standard Resolution and show custom UsrResolution field
            PXUIFieldAttribute.SetVisible<CROpportunity.resolution>(e.Cache, row, false);
            PXUIFieldAttribute.SetVisible<CROpportunityReasonExt.usrResolution>(e.Cache, row, true);

            string[] values; string[] labels;
            if (list.values.Length >0)
            {
                // Use class/stage specific list
                values = list.values; labels = list.labels;
            }
            else
            {
                // Fallback to status-driven base reasons (StatusToReasonCodes)
                values = GetBaseReasonValues(row.Status);
                labels = GetBaseReasonLabels(row.Status);
            }
            PXStringListAttribute.SetList<CROpportunityReasonExt.usrResolution>(e.Cache, row, values, labels);
        }

        // Clear UsrResolution on stage change and refresh list via RowSelected
        protected virtual void _(Events.FieldUpdated<CROpportunity, CROpportunity.stageID> e)
        {
            var row = e.Row; if (row == null) return;
            var ext = row.GetExtension<CROpportunityReasonExt>(); if (ext == null) return;

            e.Cache.SetValueExt<CROpportunityReasonExt.usrResolution>(row, null);
            // Do NOT touch CROpportunity.resolution; it remains hidden and unmanaged.
            e.Cache.RaiseRowSelected(row);
        }

        private static string GetPendingOrValue(PXCache cache, object data, string fieldName, string currentValue)
        {
            object pending = cache?.GetValuePending(data, fieldName);
            if (pending is PXFieldState fs && fs.Value is string s1) return s1;
            if (pending is string s) return s;
            return currentValue;
        }

        // Render-time override for custom list (or status fallback) on UsrResolution only
        protected virtual void _(Events.FieldSelecting<CROpportunity, CROpportunityReasonExt.usrResolution> e)
        {
            var row = e.Row; if (row == null) return;
            var stage = GetPendingOrValue(e.Cache, row, nameof(CROpportunity.StageID), row.StageID);
            var list = BuildReasonList(Base, row.ClassID, stage);
            string[] values; string[] labels;
            if (list.values.Length >0)
            {
                values = list.values; labels = list.labels;
            }
            else
            {
                values = GetBaseReasonValues(row.Status);
                labels = GetBaseReasonLabels(row.Status);
            }
            e.ReturnState = PXStringState.CreateInstance(
                         e.ReturnValue,
              2,
                    null,
                  nameof(CROpportunityReasonExt.usrResolution),
              false,
               1,
               null,
              values,
                    labels,
                  true,
                null);
        }

        // Do not override standard Resolution field list; keep it hidden
        protected virtual void _(Events.FieldSelecting<CROpportunity, CROpportunity.resolution> e)
        {
            // Intentionally left blank so default attribute logic applies (field hidden in UI)
        }

        // Master mapping of standard reason code -> label (used for base reasons)
        private static readonly Dictionary<string, string> BaseReasonLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { OpportunityReason.Created, "Created" },
            { OpportunityReason.Technology, "Technology" },
            { OpportunityReason.Relationship, "Relationship" },
            { OpportunityReason.Price, "Price" },
            { OpportunityReason.Other, "Other" },
            { OpportunityReason.InProcess, "In Process" },
            { OpportunityReason.CompanyMaturity, "Company Maturity" },
            { OpportunityReason.ConvertedFromLead, "Converted from Lead" },
            { OpportunityReason.Qualified, "Qualified" },
            { OpportunityReason.OrderPlaced, "Order Placed" },
        };

        // Status-driven base reason codes
        private static readonly Dictionary<string, string[]> StatusToReasonCodes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            // Status: New -> CR, FL, QL
            { "N", new [] { OpportunityReason.Created, OpportunityReason.ConvertedFromLead, OpportunityReason.Qualified } },
            // Status: Hold -> CR, FL, QL
            { "H", new [] { OpportunityReason.InProcess, OpportunityReason.Qualified } },
            // Status: Open -> IP, QL
            { "O", new [] { OpportunityReason.InProcess, OpportunityReason.Qualified } },
            // Status: Won -> TH, RL, PR, OT, OP
            { "W", new [] { OpportunityReason.Technology, OpportunityReason.Relationship, OpportunityReason.Price, OpportunityReason.Other, OpportunityReason.OrderPlaced } },
            // Status: Lost -> TH, RL, PR, OT, CM
            { "L", new [] { OpportunityReason.Technology, OpportunityReason.Relationship, OpportunityReason.Price, OpportunityReason.Other, OpportunityReason.CompanyMaturity } },
        };

        public static string[] GetBaseReasonValues(string status)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                var key = status.Trim().ToUpperInvariant();
                if (StatusToReasonCodes.TryGetValue(key, out var codes)) return codes;
            }
            // Strict fallback: no global list, remain status-driven only
            return new string[0];
        }

        public static string[] GetBaseReasonLabels(string status)
        {
            var codes = GetBaseReasonValues(status);
            return codes.Select(c => BaseReasonLabels.TryGetValue(c, out var lbl) ? lbl : c).ToArray();
        }

        // Master catalog of reason codes -> description (fill out with full list)
        public static readonly Dictionary<string, string> ReasonCodeCatalog = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
     {
         // A-series
            {"A1","New inquiry received"},
            {"A2","Inbound interest from divesting company"},
            {"A3","Prospect identified via market research"},
            // B-series
            {"B1","Missing product details (waiting for specs / asset list)"},
            {"B2","Awaiting financial documents"},
            {"B3","Initial contact made, awaiting response"},
            {"B4","Under technical review"},
            // C-series
            {"C1","Product confirmed, pricing under review"},
            {"C2","Compliance/legal checks underway"},
            {"C3","Internal investment committee review"},
            // D-series
            {"D1","Counter-offer from seller pending"},
            {"D2","Pricing under review"},
            {"D3","Terms and conditions discussion ongoing"},
            // E-series
            {"E1","Awaiting PO creation"},
            {"E2","Budget allocated, awaiting execution"},
            {"E3","Legal drafting contract"},
            // F-series
            {"F1","Best commercial terms secured"},
            {"F2","Strategic asset acquired"},
            {"F3","Competitive bid won"},
            {"F4","Timely acquisition (met deadline)"},
            // G-series
            {"G1","Seller withdrew"},
            {"G2","Not commercially viable"},
            {"G3","Already sold elsewhere"},
            {"G4","Did not meet technical requirements"},
            // H-series
            {"H1","Customer Feedback Pending (waiting on divesting company to send docs/specs)"},
            {"H2","Internal Review (finance or technical review before approval)"},
            {"H3","Tender Process (formal bid required, waiting on outcome)"},
            {"H4","Compliance/Legal (contract/legal approvals)"},
            {"H5","Strategic Hold (product interesting but not urgent, parked for later)"},
            // I-series
            {"I1","Targeted Sales"},
            {"I2","Inbound interest from Operator"},
            {"I3","Referral or repeat customer"},
            // J-series
            {"J1","Requirement confirmed, awaiting budget approval"},
            {"J2","Technical specs provided, validation ongoing"},
            {"J3","Customer evaluating options"},
            // K-series
            {"K1","Stock confirmed, awaiting internal approval"},
            {"K2","Stock reserved for customer"},
            {"K3","Availability issue under review"},
            // L-series
            {"L1","Quote sent, awaiting customer review"},
            {"L2","Customer requested revisions"},
            {"L3","Customer evaluating competitor offers"},
            // M-series
            {"M1","Price under discussion"},
            {"M2","Delivery terms under discussion"},
            {"M3","Payment terms under discussion"},
            // N-series
            {"N1","Competitive pricing advantage"},
            {"N2","Fast delivery/availability"},
            {"N3","Strong customer relationship"},
            {"N4","Technical superiority / spec compliance"},
            {"N5","Bundled solution or added value"},
            // O-series
            {"O1","Customer chose competitor"},
            {"O2","Budget constraints"},
            {"O3","Timing did not align"},
            {"O4","Requirement withdrawn"},
            // P-series
            {"P1","Customer Feedback Pending (waiting on client to confirm specs/needs)"},
            {"P2","Tender Process (formal bidding process open)"},
            {"P3","Budget Approval (client waiting for funding/management approval)"},
            {"P4","Technical Review (internal or external technical review required)"},
            {"P5","Pricing Review (internally reviewing margins/discounts)"},
            {"P6","Delivery/Logistics Review (checking feasibility of delivery timelines)"},
            // Q-series
            {"Q1","Customer unable to find item in stock"},
            {"Q2","Urgent sourcing request received"},
            {"Q3","Request logged via account manager"},
            {"Q4","Specs confirmed, passed to sourcing"},
            {"Q5","Urgency confirmed, awaiting Blue review"},
            {"Q6","Supplier identification in progress"},
            {"Q7","Awaiting supplier response"},
            {"Q8","Awaiting pricing confirmation"},
            {"Q9","Quote sent, waiting for confirmation"},
            {"Q0","Quote under customer internal review"},
            // R-series
            {"R0","Customer asked for changes"},
            {"R1","Item sourced successfully"},
            {"R2","Urgent requirement met"},
            {"R3","Customer accepted revised offer"},
            // S-series
            {"S1","Item not available"},
            {"S2","Customer no longer requires item"},
            {"S3","Timeline did not match need"},
            // T-series
            {"T1","Pending Blue Confirmation"},
            {"T2","Customer Review (client deciding if they still need item)"},
            {"T3","Market Availability Review (still looking, no supplier yet)"},
            {"T4","Commercial Review (pricing feasibility check)"},
        };

        // Reverse lookup: normalized description -> code
        private static readonly Dictionary<string, string> ReasonDescriptionToCode = ReasonCodeCatalog
            .ToDictionary(k => Normalize(k.Value), v => v.Key, StringComparer.OrdinalIgnoreCase);

        private static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            return new string(s.Trim().ToLowerInvariant().ToCharArray());
        }

        // Preferred resolver: try code first, then fall back to description
        private static bool TryResolveReason(string input, out string code, out string description)
        {
            code = null; description = null;
            if (string.IsNullOrWhiteSpace(input)) return false;
            var raw = input.Trim();
            var maybeCode = raw.ToUpperInvariant();
            // Prefer direct code lookup (e.g., "Q3")
            if (ReasonCodeCatalog.TryGetValue(maybeCode, out var desc))
            {
                code = maybeCode; description = desc; return true;
            }
            // Fallback: treat input as description and map to code
            var norm = Normalize(raw);
            if (ReasonDescriptionToCode.TryGetValue(norm, out var foundCode)
                && ReasonCodeCatalog.TryGetValue(foundCode, out var foundDesc))
            {
                code = foundCode; description = foundDesc; return true;
            }
            return false;
        }

        // Public so other extensions (workflow) can reuse
        public static (string[] values, string[] labels) BuildReasonList(PXGraph graph, string classID, string stageID)
        {
            var values = new List<string>(); var labels = new List<string>(); if (string.IsNullOrEmpty(classID)|| string.IsNullOrEmpty(stageID)) return (values.ToArray(), labels.ToArray());
            foreach (CROpportunityClassStageReason r in PXSelect<CROpportunityClassStageReason, Where<CROpportunityClassStageReason.classID, Equal<Required<CROpportunityClassStageReason.classID>>, And<CROpportunityClassStageReason.stageCode, Equal<Required<CROpportunityClassStageReason.stageCode>>>>>.Select(graph, classID, stageID))
            {
                var input = r.Reason ?? string.Empty; if (TryResolveReason(input,out var code,out var description)) { values.Add(code); labels.Add(description); }
            }
            return (values.ToArray(), labels.ToArray());
        }
    }
}

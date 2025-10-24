using PX.Data;
using PX.Objects.SO;
using PX.Objects.CR;
using CompiledVersion.DAC; // SOLineExt fields

namespace CompiledVersion.GraphExt
{
    // Carry RTH Cost, SPC Cost, and SPC Code from Opportunity/Quote product to Sales Order line
    public class SOOrderEntryCopyOppFieldsExt : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() => true;

        protected virtual void _(Events.RowInserted<SOLine> e)
        {
            if (e.Row == null)
                return;

            // Source product is available during Create Sales Order operation
            CROpportunityProducts src = PXResult<CROpportunityProducts>.Current;
            if (src == null)
                return;

            var srcCache = Base.Caches[typeof(CROpportunityProducts)];

            object rthObj = srcCache.GetValue(src, "UsrSWKRTHCost");
            object spcObj = srcCache.GetValue(src, "UsrSWKSPCCost");
            object spcCodeObj = srcCache.GetValue(src, "UsrSWKSPCCode");

            decimal? rth = rthObj is decimal r ? (decimal?)r : rthObj as decimal?;
            decimal? spc = spcObj is decimal s ? (decimal?)s : spcObj as decimal?;
            string spcCode = spcCodeObj as string;

            var lineCache = Base.Transactions.Cache;
            if (rth.HasValue)
                lineCache.SetValueExt<SOLineExt.usrSWKRTHCost>(e.Row, rth.Value);
            if (spc.HasValue)
                lineCache.SetValueExt<SOLineExt.usrSWKSPCCost>(e.Row, spc.Value);
            if (!string.IsNullOrEmpty(spcCode))
                lineCache.SetValueExt<SOLineExt.usrSWKSPCCode>(e.Row, spcCode);
        }
    }
}

using CompiledVersion.DAC;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.IN;
using System;

namespace CompiledVersion.Graphs
{
    public class QuoteMaint_Extension : PXGraphExtension<QuoteMaint>
    {
        public static bool IsActive() => true;

        //in fieldupdated of the business account field, get the hubspot deal id form opportunity to quote's hubspot deal id (extended dac)
        protected virtual void CRQuote_BAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated del)
        {
            if (e.Row == null) return;

            del(sender, e);
            CRQuote row = e.Row as CRQuote;
            //CROpportunity opportunity = PXSelect<PX.Objects.CR.CROpportunity,
            //    Where<PX.Objects.CR.CROpportunity.bAccountID, Equal<Required<PX.Objects.CR.CROpportunity.bAccountID>>>>
            //    .Select(Base, row.BAccountID).FirstOrDefault();

            var opportunity = (PX.Objects.CR.Standalone.CROpportunity)PXSelect<PX.Objects.CR.Standalone.CROpportunity,
                        Where<PX.Objects.CR.Standalone.CROpportunity.opportunityID, Equal<Required<PX.Objects.CR.Standalone.CROpportunity.opportunityID>>>>
                        .SelectSingleBound(Base, null, row.OpportunityID);
            CRQuoteExt cRQuoteExt = row.GetExtension<CRQuoteExt>();
            if (opportunity != null)
            {
                string hubspotDealID = opportunity.GetExtension<CROpportunityStandaloneExt>().UsrHubspotDealID;
                Base.Quote.Cache.SetValueExt<CRQuoteExt.usrHubspotDealID>(row, hubspotDealID);
                cRQuoteExt.UsrHubspotDealID = hubspotDealID;
            }
        }
        protected virtual void CRQuote_OpportunityID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated del)
        {
            if (e.Row == null) return;

            del(sender, e);
            CRQuote row = e.Row as CRQuote;
            //CROpportunity opportunity = PXSelect<PX.Objects.CR.CROpportunity,
            //    Where<PX.Objects.CR.CROpportunity.bAccountID, Equal<Required<PX.Objects.CR.CROpportunity.bAccountID>>>>
            //    .Select(Base, row.BAccountID).FirstOrDefault();

            var opportunity = (PX.Objects.CR.Standalone.CROpportunity)PXSelect<PX.Objects.CR.Standalone.CROpportunity,
                        Where<PX.Objects.CR.Standalone.CROpportunity.opportunityID, Equal<Required<PX.Objects.CR.Standalone.CROpportunity.opportunityID>>>>
                        .SelectSingleBound(Base, null, row.OpportunityID);
            CRQuoteExt cRQuoteExt = row.GetExtension<CRQuoteExt>();
            if (opportunity != null)
            {
                string hubspotDealID = opportunity.GetExtension<CROpportunityStandaloneExt>().UsrHubspotDealID;
                Base.Quote.Cache.SetValueExt<CRQuoteExt.usrHubspotDealID>(row, hubspotDealID);
                cRQuoteExt.UsrHubspotDealID = hubspotDealID;
            }
        }

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
            // populate RTH field and persist into cache
            var item = InventoryItem.PK.Find(Base, row.InventoryID);
            var itemExt = item?.GetExtension<InventoryItemExt>();
            var rth = itemExt?.UsrSWKRTHCost ?? 0m;
            e.Cache.SetValueExt<CROpportunityProductsExt.usrSWKRTHCost>(row, rth);
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
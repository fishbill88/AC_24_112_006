using PX.Data;
using PX.Objects.CR;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.CA.PXModule;
using static PX.Objects.GL.FormatDirection;

namespace HubspotCustomization
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
    }
}
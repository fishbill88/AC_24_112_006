using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.SM;
using System;
using PX.Objects.AR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.EP;

namespace PX.Objects.CR
{
    public class SIOpportunityActivityProcess : PXGraph<SIOpportunityActivityProcess>
    {
        #region Views

        public PXFilter<OpportunityActivityFilter> Filter;
        public PXCancel<OpportunityActivityFilter> Cancel;

        [PXFilterable]
        [PXViewDetailsButton(typeof(CROpportunity))]
        public PXFilteredProcessing<CROpportunity, OpportunityActivityFilter>
            Items;

        #endregion

        #region Constructor

        public SIOpportunityActivityProcess()
        {
            Items.SetSelected<CROpportunity.selected>();
            Items.SetProcessDelegate<SIOpportunityActivityProcess>(ProcessRecords);
            
            Items.AllowInsert = false;
            Items.AllowDelete = false;
            Items.AllowUpdate = true;
            
            // Enable the field by default
            PXUIFieldAttribute.SetEnabled<PX.Objects.CR.CROpportunityExt2.usrActivityNote>(Items.Cache, null, true);
            
            // Set default sort order
            Items.Cache.AllowSelect = true;
            Items.OrderByNew<OrderBy<Asc<CROpportunity.closeDate>>>();
        }

        protected virtual IEnumerable items()
        {
            var filter = Filter.Current;
            if (filter == null)
                yield break;

            // Parse comma-delimited ClassIDs
            var classIDs = new List<string>();
            if (!string.IsNullOrWhiteSpace(filter.ClassID))
            {
                classIDs = filter.ClassID.Split(',')
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .ToList();
            }

            // Parse comma-delimited Statuses for exclusion
            var excludeStatuses = new List<string>();
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                excludeStatuses = filter.Status.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
            }

            PXSelectBase<CROpportunity> select = new PXSelectJoinOrderBy<CROpportunity,
                LeftJoin<Contact, On<Contact.contactID, Equal<CROpportunity.contactID>>,
                LeftJoin<BAccount, On<BAccount.bAccountID, Equal<CROpportunity.bAccountID>>,
                LeftJoin<BAccountParent, On<BAccountParent.bAccountID, Equal<BAccount.parentBAccountID>>,
                LeftJoin<CRCampaign, On<CRCampaign.campaignID, Equal<CROpportunity.campaignSourceID>>>>>>,
                OrderBy<Asc<CROpportunity.closeDate>>>(this);

            foreach (PXResult<CROpportunity> result in select.Select())
            {
                CROpportunity opp = result;
                
                // Apply class filter if ClassIDs are specified
                if (classIDs.Count > 0 && !classIDs.Contains(opp.ClassID))
                    continue;

                // Exclude opportunities with matching statuses
                if (excludeStatuses.Count > 0 && excludeStatuses.Contains(opp.Status))
                    continue;

                yield return result;
            }
        }

        #endregion

        #region Event Handlers

        protected virtual void _(Events.RowSelected<OpportunityActivityFilter> e)
        {
            if (e.Row == null) return;

            Items.SetProcessAllCaption("Process All");
            Items.SetProcessCaption("Process");
        }

        protected virtual void _(Events.RowSelected<CROpportunity> e)
        {
            if (e.Row == null) return;

            PXUIFieldAttribute.SetEnabled<PX.Objects.CR.CROpportunityExt2.usrActivityNote>(e.Cache, e.Row, true);
        }

        protected virtual void _(Events.FieldUpdated<CROpportunity, PX.Objects.CR.CROpportunityExt2.usrActivityNote> e)
        {
            if (e.Row == null) return;

            var opportunityExt = e.Row.GetExtension<PX.Objects.CR.CROpportunityExt2>();
            if (opportunityExt != null && !string.IsNullOrWhiteSpace(opportunityExt.UsrActivityNote))
            {
                e.Row.Selected = true;
            }
        }

        #endregion

        #region Processing Methods

        public static void ProcessRecords(SIOpportunityActivityProcess graph, CROpportunity opportunity)
        {
            var opportunityExt = opportunity.GetExtension<PX.Objects.CR.CROpportunityExt2>();
            if (opportunityExt == null)
            {
                throw new PXException("Unable to get opportunity extension.");
            }

            string activityNote = opportunityExt.UsrActivityNote;
            
            if (string.IsNullOrWhiteSpace(activityNote))
            {
                throw new PXException("Activity Note is required.");
            }

            // Create the activity using CRActivityMaint graph
            var activityGraph = PXGraph.CreateInstance<CRActivityMaint>();
            
            var activity = (CRActivity)activityGraph.Activities.Cache.CreateInstance();
            activity.ClassID = CRActivityClass.Activity;
            activity.Type = "N"; // Note type
            activity.RefNoteID = opportunity.NoteID;
            activity.OwnerID = opportunity.OwnerID;
            activity.Subject = activityNote;
            activity.StartDate = graph.Accessinfo.BusinessDate;

            activity = activityGraph.Activities.Insert(activity);
            
            activityGraph.Actions.PressSave();
        }

        #endregion

        #region Filter DAC

        [Serializable]
        [PXHidden]
        public class OpportunityActivityFilter : PX.Data.PXBqlTable, PX.Data.IBqlTable
        {
            #region ClassID
            public abstract class classID : BqlString.Field<classID> { }

            
            [PXString(255, IsUnicode = true)]
            [PXUIField(DisplayName = "Class")]
            [PXSelector(typeof(Search<CROpportunityClass.cROpportunityClassID>),
                        typeof(CROpportunityClass.cROpportunityClassID),
                DescriptionField = typeof(CROpportunityClass.description),ValidateValue = false)]
            public virtual string ClassID { get; set; }
            #endregion

            #region Status
            public abstract class status : BqlString.Field<status> { }

            [PXString(255, IsUnicode = true)]
            [PXUIField(DisplayName = "Not in Status")]
            [PXSelector(typeof(Search4<CROpportunity.status, 
                Aggregate<GroupBy<CROpportunity.status>>>),
                typeof(CROpportunity.status),
                ValidateValue = false)]
            public virtual string Status { get; set; }
            #endregion
        }

        #endregion
    }
}

using System;
using System.Collections;
using PX.Data;
using PX.Objects.CR;
using CompiledVersion.DAC; // for CROpportunityReasonExt extension

namespace CompiledVersion
{
    //skip documentation for this
    /// <summary>
    /// Adds custom workflow action popups (Stage / Reason selection) to replace standard workflow buttons.
    /// This extension intercepts OpenFromNew, Open, CloseAsWon, and CloseAsLost actions to show a custom dialog
    /// that allows users to select both Stage and Reason before executing the underlying workflow action.
    /// </summary>
    public class OpportunityMaint_OpenCustomExt : PXGraphExtension<OpportunityMaint>
    {
        public static bool IsActive() => false;

        // Tracks the intended resulting status during popup rendering so the Reason list
        // can be based on the post-action status. Only used for the popup logic.
        private string _pendingPopupStatus;

        private const string StatusNew = "N";
        private const string StatusHold = "H";
        private const string StatusOpen = "O";
        private const string StatusWon = "W";
        private const string StatusLost = "L";

        #region Data View - Filter

        /// <summary>
        /// Filter view that holds the Stage and Reason selection for custom workflow actions
        /// </summary>
        public PXFilter<OpportunityStatusFilter> StatusChangeFilter;

        #endregion

        #region Filter DAC

        /// <summary>
        /// Filter DAC used to capture Stage and Reason selections in the custom workflow popup dialogs
        /// </summary>
        [Serializable]
        public class OpportunityStatusFilter : PXBqlTable, IBqlTable
        {
            #region FilterStage
            public abstract class filterStage : PX.Data.BQL.BqlString.Field<filterStage> { }

            /// <summary>
            /// The stage to transition to. Dynamically populated based on active stages for the opportunity class.
            /// </summary>
            [PXString]
            [PXUIField(DisplayName = "Stage", Required = true)]
            public virtual string FilterStage { get; set; }
            #endregion

            #region FilterResolution
            public abstract class filterResolution : PX.Data.BQL.BqlString.Field<filterResolution> { }

            /// <summary>
            /// The reason for the status change. Dynamically populated based on the selected stage.
            /// </summary>
            [PXString]
            [PXUIField(DisplayName = "Reason", Required = true)]
            public virtual string FilterResolution { get; set; }
            #endregion
        }

        #endregion

        #region Field Events - Dynamic Lists

        /// <summary>
        /// Builds dynamic list of active stages for the opportunity class
        /// </summary>
        protected virtual void _(Events.FieldSelecting<OpportunityStatusFilter, OpportunityStatusFilter.filterStage> e)
        {
            var opp = Base.Opportunity.Current;
            if (opp == null)
                return;

            // Query active stages for the opportunity class
            var stages = PXSelectJoin<CROpportunityClassProbability,
                InnerJoin<CROpportunityProbability,
                    On<CROpportunityProbability.stageCode, Equal<CROpportunityClassProbability.stageID>>>,

                 Where<CROpportunityClassProbability.classID, Equal<Required<CROpportunity.classID>>>>.Select(Base, opp.ClassID);

            var stageCount = stages.Count;
            if (stageCount ==0)
            {
                e.ReturnState = PXStringState.CreateInstance(e.ReturnValue,2, null,
       nameof(OpportunityStatusFilter.FilterStage), false,1, null, null, null, true, null);
                return;
            }

            string[] values = new string[stageCount];
            string[] labels = new string[stageCount];

            int i =0;
            foreach (PXResult<CROpportunityClassProbability, CROpportunityProbability> result in stages)
            {
                var stage = (CROpportunityProbability)result;
                values[i] = stage.StageCode;
                labels[i] = stage.Name;
                i++;
            }

            e.ReturnState = PXStringState.CreateInstance(e.ReturnValue,2, null,
     nameof(OpportunityStatusFilter.FilterStage), false,1, null, values, labels, true, null);
        }

        /// <summary>
        /// Builds dynamic list of reasons based on the selected stage
        /// </summary>
        protected virtual void _(Events.FieldSelecting<OpportunityStatusFilter, OpportunityStatusFilter.filterResolution> e)
        {
            var opp = Base.Opportunity.Current;
            var filt = e.Row;
            if (opp == null || filt == null)
                return;

            string stage = filt.FilterStage ?? opp.StageID;
            var list = OpportunityMaint_StageReasonExt.BuildReasonList(Base, opp.ClassID, stage);

            // For popup only: when no class-stage list, fallback to base reasons tied to the
            // status that will result AFTER the clicked action is performed.
            string statusForReasons = !string.IsNullOrEmpty(_pendingPopupStatus) ? _pendingPopupStatus : opp.Status;

            string[] values = list.values.Length >0 ? list.values : OpportunityMaint_StageReasonExt.GetBaseReasonValues(statusForReasons);
            string[] labels = list.values.Length >0 ? list.labels : OpportunityMaint_StageReasonExt.GetBaseReasonLabels(statusForReasons);

            e.ReturnState = PXStringState.CreateInstance(e.ReturnValue,10, null,
     nameof(OpportunityStatusFilter.FilterResolution), false,1, null, values, labels, true, null);
        }

        /// <summary>
        /// Resets the resolution field when stage changes to rebuild the reason list
        /// </summary>
        protected virtual void _(Events.FieldUpdated<OpportunityStatusFilter, OpportunityStatusFilter.filterStage> e)
        {
            var row = e.Row;
            if (row == null) return;
            // Reset resolution and let FieldSelecting rebuild the list for the new stage
            e.Cache.SetValueExt<OpportunityStatusFilter.filterResolution>(row, null);
        }

        #endregion

        #region Actions - Custom Workflow Replacements

        #region Open From New Custom

        /// <summary>
        /// Custom replacement for the OpenFromNew workflow action.
        /// Shows a popup to select Stage and Reason, then executes the standard OpenFromNew action.
        /// This action replaces and hides the standard OpenFromNew button.
        /// </summary>
        public PXAction<CROpportunity> OpenFromNewCustom;

        [PXButton(Category = "Processing", Connotation = PX.Data.WorkflowAPI.ActionConnotation.Success)]
        [PXUIField(DisplayName = "Open", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        protected IEnumerable openFromNewCustom(PXAdapter adapter)
        {
            ExecuteStatusChange(() => Base.OpenFromNew.PressImpl(internalCall: true), StatusOpen);
            return adapter.Get();
        }

        #endregion

        #region Open Custom

        /// <summary>
        /// Custom replacement for the Open workflow action.
        /// Shows a popup to select Stage and Reason, then executes the standard Open action.
        /// This action replaces and hides the standard Open button.
        /// </summary>
        public PXAction<CROpportunity> OpenCustom;

        [PXButton(Category = "Processing", Connotation = PX.Data.WorkflowAPI.ActionConnotation.Success)]
        [PXUIField(DisplayName = "Open", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        protected IEnumerable openCustom(PXAdapter adapter)
        {
            ExecuteStatusChange(() => Base.Open.PressImpl(internalCall: true), StatusOpen);
            return adapter.Get();
        }

        #endregion

        #region Close As Won Custom

        /// <summary>
        /// Custom replacement for the CloseAsWon workflow action.
        /// Shows a popup to select Stage and Reason, then executes the standard CloseAsWon action.
        /// This action replaces and hides the standard CloseAsWon button.
        /// </summary>
        public PXAction<CROpportunity> CloseAsWonCustom;

        [PXButton(Category = "Processing", Connotation = PX.Data.WorkflowAPI.ActionConnotation.Success)]
        [PXUIField(DisplayName = "Close as Won", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        protected IEnumerable closeAsWonCustom(PXAdapter adapter)
        {
            ExecuteStatusChange(() => Base.CloseAsWon.PressImpl(internalCall: true), StatusWon);
            return adapter.Get();
        }

        #endregion

        #region Close As Lost Custom

        /// <summary>
        /// Custom replacement for the CloseAsLost workflow action.
        /// Shows a popup to select Stage and Reason, then executes the standard CloseAsLost action.
        /// This action replaces and hides the standard CloseAsLost button.
        /// </summary>
        public PXAction<CROpportunity> CloseAsLostCustom;

        [PXButton(Category = "Processing", Connotation = PX.Data.WorkflowAPI.ActionConnotation.Danger)]
        [PXUIField(DisplayName = "Close as Lost", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        protected IEnumerable closeAsLostCustom(PXAdapter adapter)
        {
            ExecuteStatusChange(() => Base.CloseAsLost.PressImpl(internalCall: true), StatusLost);
            return adapter.Get();
        }

        #endregion

        #endregion

        #region Helper Methods

        /// <summary>
        /// Common logic for all custom status change actions.
        /// Shows the Stage/Reason popup, updates the opportunity, and executes the workflow action.
        /// </summary>
        /// <param name="workflowAction">The underlying workflow action to execute after user confirms</param>
        /// <param name="targetStatusForPopup">The status that will result after the action (used only to build popup reason list)</param>
        private void ExecuteStatusChange(Action workflowAction, string targetStatusForPopup)
        {
            var row = Base.Opportunity.Current;
            if (row == null)
                return;

            // Set the pending status for popup rendering
            _pendingPopupStatus = targetStatusForPopup;

            try
            {
                // Initialize filter only when opening the dialog
                if (StatusChangeFilter.View.Answer == WebDialogResult.None)
                {
                    var ext = row.GetExtension<CROpportunityReasonExt>();
                    string currentResolution = ext?.UsrResolution;

                    StatusChangeFilter.Cache.Clear();
                    StatusChangeFilter.Cache.Insert(new OpportunityStatusFilter
                    {
                        FilterStage = row.StageID,
                        FilterResolution = currentResolution
                    });
                }

                if (StatusChangeFilter.AskExt() != WebDialogResult.OK)
                    return;

                var filter = StatusChangeFilter.Current;

                // Execute the underlying workflow action first
                workflowAction();

                // Refresh current after workflow
                row = Base.Opportunity.Current;
                if (row == null)
                    return;

                // Apply user-selected values after workflow transition
                if (!string.IsNullOrEmpty(filter?.FilterStage) && filter.FilterStage != row.StageID)
                {
                    Base.Opportunity.Cache.SetValueExt<CROpportunity.stageID>(row, filter.FilterStage);
                }

                if (!string.IsNullOrEmpty(filter?.FilterResolution))
                {
                    var oppExt = row.GetExtension<CROpportunityReasonExt>();
                    if (oppExt != null)
                    {
                        Base.Opportunity.Cache.SetValueExt<CROpportunityReasonExt.usrResolution>(row, filter.FilterResolution);
                        Base.Opportunity.Cache.SetValueExt<CROpportunity.resolution>(row, filter.FilterResolution);
                    }
                }

                // Persist and reset dialog state for next use
                Base.Actions.PressSave();
            }
            finally
            {
                // Always clear popup state
                StatusChangeFilter.View.Answer = WebDialogResult.None;
                _pendingPopupStatus = null;
            }
        }

        #endregion

        #region Action Handlers - Hide Standard Buttons

        /// <summary>
        /// Hides the standard workflow buttons and shows our custom replacements
        /// </summary>
        protected virtual void _(Events.RowSelected<CROpportunity> e)
        {
            if (e.Row == null) return;

            // Hide standard workflow buttons
            Base.OpenFromNew.SetVisible(false);
            Base.Open.SetVisible(false);
            Base.CloseAsWon.SetVisible(false);
            Base.CloseAsLost.SetVisible(false);

            // Show custom buttons with same visibility logic as standard buttons
            bool canOpenFromNew = e.Row.Status == OpportunityStatus.New;
            OpenFromNewCustom.SetVisible(canOpenFromNew);
            OpenFromNewCustom.SetEnabled(canOpenFromNew);

            bool isNotNew = e.Row.Status != OpportunityStatus.New;
            bool isNotOpen = e.Row.Status != OpportunityStatus.Open;
            bool isOpen = e.Row.Status == OpportunityStatus.Open;

            // Open button - visible when status is not New and not Open (e.g., Closed, Lost)
            OpenCustom.SetVisible(isNotNew && isNotOpen);
            OpenCustom.SetEnabled(isNotNew && isNotOpen);

            // Close buttons - visible when Open
            CloseAsWonCustom.SetVisible(isOpen);
            CloseAsWonCustom.SetEnabled(isOpen);

            CloseAsLostCustom.SetVisible(isOpen);
            CloseAsLostCustom.SetEnabled(isOpen);
        }

        #endregion
    }
}

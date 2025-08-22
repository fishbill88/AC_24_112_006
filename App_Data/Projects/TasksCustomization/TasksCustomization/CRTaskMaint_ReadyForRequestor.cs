using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.EP;

namespace MyProject
{
    public static class TaskCustomStatuses
    {
        // New status code for CRActivity.uistatus (2-char fixed)
        public const string ReadyForRequestor = "RR";

        public class readyForRequestor : PX.Data.BQL.BqlString.Constant<readyForRequestor>
        {
            public readyForRequestor() : base(ReadyForRequestor) { }
        }

        public const string ReadyForRequestorDisplay = "Ready for Requestor";
    }

    // Graph extension for CRTaskMaint
    public class CRTaskMaintReadyForRequestorExt : PXGraphExtension<CRTaskMaint>
    {
        public static bool IsActive() => true;

        // 1) Replace status list to include the new value between Processing and Completed
        [PXRemoveBaseAttribute(typeof(TaskStatusAttribute))]
        [PXRemoveBaseAttribute(typeof(ActivityStatusAttribute))]
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXStringList(
            new[]
            {
                ActivityStatusListAttribute.Draft,
                ActivityStatusListAttribute.Open,
                ActivityStatusListAttribute.InProcess,
                TaskCustomStatuses.ReadyForRequestor,         // new value - after Processing
                ActivityStatusListAttribute.Completed,
                ActivityStatusListAttribute.Canceled
            },
            new[]
            {
                "Draft",
                "Open",
                "In Process",
                TaskCustomStatuses.ReadyForRequestorDisplay,  // display label
                "Completed",
                "Canceled"
            }
        )]
        protected virtual void _(Events.CacheAttached<CRActivity.uistatus> e) { }

        // 2) New action to set status to "Ready for Requestor"
        public PXAction<CRActivity> MarkReadyForRequestor;
        [PXUIField(DisplayName = "Ready for Requestor", MapEnableRights = PXCacheRights.Update)]
        [PXButton(Tooltip = "Move task to Ready for Requestor")]
        protected virtual void markReadyForRequestor()
        {
            var row = Base.Tasks.Current;
            if (row == null) return;

            // Only from Processing/InProcess
            if (row.UIStatus != ActivityStatusListAttribute.InProcess)
                return;

            var copy = (CRActivity)Base.Tasks.Cache.CreateCopy(row);
            copy.UIStatus = TaskCustomStatuses.ReadyForRequestor;
            Base.Tasks.Cache.Update(copy);
            Base.Actions.PressSave();
        }

        // 3) Enable/disable buttons considering the new status
        protected virtual void _(Events.RowSelected<CRActivity> e)
        {
            var row = e.Row;
            if (row == null) return;

            // Force the dropdown list to include our new status code
            PXStringListAttribute.SetList<CRActivity.uistatus>(e.Cache, row,
                new[]
                {
                    ActivityStatusListAttribute.Draft,
                    ActivityStatusListAttribute.Open,
                    ActivityStatusListAttribute.InProcess,
                    TaskCustomStatuses.ReadyForRequestor,
                    ActivityStatusListAttribute.Completed,
                    ActivityStatusListAttribute.Canceled
                },
                new[]
                {
                    "Draft",
                    "Open",
                    "In Process",
                    TaskCustomStatuses.ReadyForRequestorDisplay,
                    "Completed",
                    "Canceled"
                }
            );

            // Allow completing from Open, Processing, and Ready-for-Requestor
            bool canComplete =
                row.UIStatus == ActivityStatusListAttribute.Open ||
                row.UIStatus == ActivityStatusListAttribute.InProcess ||
                row.UIStatus == TaskCustomStatuses.ReadyForRequestor;

            Base.Complete.SetEnabled(canComplete);
            Base.Complete.SetConnotation(canComplete
                ? PX.Data.WorkflowAPI.ActionConnotation.Success
                : PX.Data.WorkflowAPI.ActionConnotation.None);

            Base.CompleteAndFollowUp.SetEnabled(canComplete);

            // Allow moving to Ready only from Processing
            MarkReadyForRequestor.SetEnabled(row.UIStatus == ActivityStatusListAttribute.InProcess);
        }
    }
}
using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.CR;
using static PX.Data.WorkflowAPI.BoundedTo<PX.Objects.CR.CRTaskMaint, PX.Objects.CR.CRActivity>;

namespace MyProject
{
    // Workflow extension for CRTaskMaint to add "Ready for Requestor" state between Processing and Completed
    public class CRTaskMaint_Workflow : PXGraphExtension<CRTaskMaint>
    {
        public static bool IsActive() => true;

        public sealed override void Configure(PXScreenConfiguration config)
            => Configure(config.GetScreenConfigurationContext<CRTaskMaint, CRActivity>());

        protected static void Configure(WorkflowContext<CRTaskMaint, CRActivity> context)
        {
            var actionMarkReady = context.ActionDefinitions.CreateExisting<CRTaskMaintReadyForRequestorExt>(
                g => g.MarkReadyForRequestor,
                a => a);

            context.AddScreenConfigurationFor(screen =>
            {
                return screen
                    .StateIdentifierIs<CRActivity.uistatus>()
                    .AddDefaultFlow(flow => flow
                        .WithFlowStates(fss =>
                        {
                            fss.Add(ActivityStatusListAttribute.Draft, flowState => flowState.IsInitial());
                            fss.Add(ActivityStatusListAttribute.Open);
                            fss.Add(ActivityStatusListAttribute.InProcess, flowState => flowState
                                .WithActions(actions =>
                                {
                                    actions.Add(actionMarkReady);
                                    actions.Add(g => g.Complete);
                                    actions.Add(g => g.CancelActivity);
                                }));
                            fss.Add(TaskCustomStatuses.ReadyForRequestor, flowState => flowState
                                .WithActions(actions =>
                                {
                                    actions.Add(g => g.Complete, c => c.WithConnotation(ActionConnotation.Success));
                                    actions.Add(g => g.CancelActivity);
                                }));
                            fss.Add(ActivityStatusListAttribute.Completed);
                            fss.Add(ActivityStatusListAttribute.Canceled);
                        })
                        .WithTransitions(transitions =>
                        {
                            transitions.AddGroupFrom(ActivityStatusListAttribute.InProcess, ts =>
                            {
                                ts.Add(t => t
                                    .To(TaskCustomStatuses.ReadyForRequestor)
                                    .IsTriggeredOn(actionMarkReady)
                                    .WithFieldAssignments(fa =>
                                    {
                                        fa.Add<CRActivity.uistatus>(e => e.SetFromValue(TaskCustomStatuses.ReadyForRequestor));
                                    }));
                                ts.Add(t => t
                                    .To(ActivityStatusListAttribute.Completed)
                                    .IsTriggeredOn(g => g.Complete)
                                    .WithFieldAssignments(fa =>
                                    {
                                        fa.Add<CRActivity.uistatus>(e => e.SetFromValue(ActivityStatusListAttribute.Completed));
                                    }));
                                ts.Add(t => t
                                    .To(ActivityStatusListAttribute.Canceled)
                                    .IsTriggeredOn(g => g.CancelActivity)
                                    .WithFieldAssignments(fa =>
                                    {
                                        fa.Add<CRActivity.uistatus>(e => e.SetFromValue(ActivityStatusListAttribute.Canceled));
                                    }));
                            });

                            transitions.AddGroupFrom(TaskCustomStatuses.ReadyForRequestor, ts =>
                            {
                                ts.Add(t => t
                                    .To(ActivityStatusListAttribute.Completed)
                                    .IsTriggeredOn(g => g.Complete)
                                    .WithFieldAssignments(fa =>
                                    {
                                        fa.Add<CRActivity.uistatus>(e => e.SetFromValue(ActivityStatusListAttribute.Completed));
                                    }));
                                ts.Add(t => t
                                    .To(ActivityStatusListAttribute.Canceled)
                                    .IsTriggeredOn(g => g.CancelActivity)
                                    .WithFieldAssignments(fa =>
                                    {
                                        fa.Add<CRActivity.uistatus>(e => e.SetFromValue(ActivityStatusListAttribute.Canceled));
                                    }));
                            });
                        }))
                    .WithActions(actions =>
                    {
                        actions.Add(actionMarkReady);
                    });
            });
        }
    }
}

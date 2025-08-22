using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.Common;
using PX.Objects.SO;

namespace HubspotCustomization
{
    public class SOShipmentEntry_WorkflowExt : PXGraphExtension<SOShipmentEntry_Workflow, SOShipmentEntry>
    {
        public static bool IsActive() => true;

        public override void Configure(PXScreenConfiguration config)
        {
            Configure(config.GetScreenConfigurationContext<SOShipmentEntry, SOShipment>());
        }

        protected static void Configure(WorkflowContext<SOShipmentEntry, SOShipment> context)
        {
            var conditions = context.Conditions.GetPack<SOShipmentEntry_Workflow.Conditions>();
            var commonCategories = CommonActionCategories.Get(context);
            var processingCategory = commonCategories.Processing;

            context.UpdateScreenConfigurationFor(screen =>
                screen.WithActions(actions =>
                {
                    // Add your custom action to the workflow with proper display name
                    actions.Add<SOShipmentEntry_Extension>(e => e.createCombinedInvoice, a => a
                        .DisplayName("Prepare Invoice")
                        .WithCategory(processingCategory)
                        .IsDisabledWhen(conditions.IsNotBillable)
                        .MassProcessingScreen<SOInvoiceShipment>()
                        .InBatchMode());

                    // REMOVE the original actions completely from workflow
                    actions.Remove(g => g.createInvoice);
                    actions.Remove(g => g.createDropshipInvoice);
                }));

            // Add the action to the appropriate workflow states
            context.UpdateScreenConfigurationFor(screen =>
                screen.UpdateDefaultFlow(flow =>
                    flow.WithFlowStates(states =>
                    {
                        // Add to confirmed state (same as createInvoice)
                        states.Update("Confirmed", state =>
                            state.WithActions(actions =>
                            {
                                actions.Add<SOShipmentEntry_Extension>(e => e.createCombinedInvoice,
                                    a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
                            }));

                        // Add to partially invoiced state (same as createInvoice)
                        states.Update("PartiallyInvoiced", state =>
                            state.WithActions(actions =>
                            {
                                actions.Add<SOShipmentEntry_Extension>(e => e.createCombinedInvoice,
                                    a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
                            }));
                    })));
        }
    }
}
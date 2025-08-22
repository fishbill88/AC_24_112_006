/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using System;

using PX.Data;
using PX.Data.WorkflowAPI;

namespace PX.Objects.AR
{
	using State = ARDocStatus;
	using static ARInvoice;

	public class ARInvoiceEntry_ApprovalWorkflow : InvoiceEntry_ApprovalWorkflow<
		ARInvoiceEntry,
		ARInvoiceEntry_Workflow,
		ARInvoiceEntry_ApprovalWorkflow.ARConditions>
	{
		public class ARConditions : Conditions
		{
			public override BoundedTo<ARInvoiceEntry, ARInvoice>.Condition IsApprovalDisabled => GetOrCreate(b => b.FromBql<
				Not<ARSetupApproval.EPSetting.IsDocumentApprovable<docType, status>>
			>());

			public override BoundedTo<ARInvoiceEntry, ARInvoice>.Condition NonEditable => GetOrCreate(b => b.FromBql<
				ARSetupApproval.EPSetting.IsDocumentLockedByApproval<docType, status>
			>());
		}

		[PXWorkflowDependsOnType(typeof(ARSetupApproval))]
		public sealed override void Configure(PXScreenConfiguration config)
		{
			ConfigureBase(config, ctx => ctx.Categories.Get(ARInvoiceEntry_Workflow.CategoryID.Approval));

			config.GetScreenConfigurationContext<ARInvoiceEntry, ARInvoice>()
				.UpdateScreenConfigurationFor(screen => screen
				.UpdateDefaultFlow(flow => flow
				.WithTransitions(transitions => transitions
					.Add(t => t
						.From<State.pendingApproval>()
						.To<State.scheduled>()
						.IsTriggeredOn(g => g.OnConfirmSchedule)
						.WithFieldAssignments(fas =>
						{
							fas.Add<scheduled>(e => e.SetFromValue(true));
							fas.Add<scheduleID>(e => e.SetFromExpression("@ScheduleID"));
						})))));
		}
	}

	public abstract class InvoiceEntry_ApprovalWorkflow<TInvoiceEntry, TWorkflowExtension, TConditionPack> : PXGraphExtension<TWorkflowExtension, TInvoiceEntry>
		where TInvoiceEntry : ARInvoiceEntry
		where TWorkflowExtension : PXGraphExtension<TInvoiceEntry>
		where TConditionPack : InvoiceEntry_ApprovalWorkflow<TInvoiceEntry, TWorkflowExtension, TConditionPack>.Conditions, new()
	{
		public abstract class Conditions : BoundedTo<TInvoiceEntry, ARInvoice>.Condition.Pack
		{
			public virtual BoundedTo<TInvoiceEntry, ARInvoice>.Condition IsApproved => GetOrCreate(b => b.FromBql<
				ARRegister.approved.IsEqual<True>
			>());

			public virtual BoundedTo<TInvoiceEntry, ARInvoice>.Condition IsRejected => GetOrCreate(b => b.FromBql<
				ARRegister.rejected.IsEqual<True>
			>());

			public abstract BoundedTo<TInvoiceEntry, ARInvoice>.Condition IsApprovalDisabled { get; }

			public abstract BoundedTo<TInvoiceEntry, ARInvoice>.Condition NonEditable { get; }
		}

		protected static void ConfigureBase(
			PXScreenConfiguration config,
			Func<
				WorkflowContext<TInvoiceEntry, ARInvoice>,
				BoundedTo<TInvoiceEntry, ARInvoice>.ActionCategory.IConfigured>
			approvalCategorySelector)
		{
			var ctx = config.GetScreenConfigurationContext<TInvoiceEntry, ARInvoice>();
			ConfigureBase(ctx, ctx.Conditions.GetPack<TConditionPack>(), approvalCategorySelector(ctx));
		}

		private static void ConfigureBase(
			WorkflowContext<TInvoiceEntry, ARInvoice> context,
			TConditionPack conditions,
			BoundedTo<TInvoiceEntry, ARInvoice>.ActionCategory.IConfigured approvalCategory)
		{
			context.UpdateScreenConfigurationFor(screen => screen
				.UpdateDefaultFlow(flow => flow
					.WithFlowStates(states =>
					{
						states.UpdateSequence<State.HoldToBalance>(seq =>
							seq.WithStates(sss =>
							{
								sss.Add<State.pendingApproval>(flowState =>
								{
									return flowState
										.IsSkippedWhen(conditions.IsApproved || conditions.IsRejected)
										.WithActions(actions =>
										{
											actions.Add(g => g.approve, a => a.IsDuplicatedInToolbar());
											actions.Add(g => g.reject, a => a.IsDuplicatedInToolbar());
											actions.Add(nameof(ARInvoiceEntry.Approval.ReassignApproval));
											actions.Add(g => g.putOnHold);
										})
										.PlaceAfter<State.hold>();
								});

								sss.Add<State.rejected>(flowState =>
								{
									return flowState
										.IsSkippedWhen(!conditions.IsRejected)
										.WithActions(actions =>
										{
											actions.Add(g => g.putOnHold, a => a.IsDuplicatedInToolbar());
											actions.Add(g => g.printAREdit);
											actions.Add(g => g.customerDocuments);
										})
										.PlaceAfter<State.pendingApproval>();
								});
							}));
					})
					.WithTransitions(transitions =>
					{
						transitions.AddGroupFrom<State.pendingApproval>(ts =>
						{
							ts.Add(t => t
								.To<State.HoldToBalance>()
								.IsTriggeredOn(g => g.OnUpdateStatus));
							ts.Add(t => t
								.ToNext()
								.IsTriggeredOn(g => g.approve)
								.When(conditions.IsApproved));
							ts.Add(t => t
								.To<State.rejected>()
								.IsTriggeredOn(g => g.reject)
								.When(conditions.IsRejected));
						});
						transitions.AddGroupFrom<State.rejected>(ts =>
						{
							ts.Add(t => t
								.To<State.hold>()
								.IsTriggeredOn(g => g.putOnHold)
								.DoesNotPersist()
							);
						});
					}))
				.WithActions(actions =>
				{
					actions.Add(g => g.approve, a => a
						.WithCategory(approvalCategory)
						.PlaceAfter(g => g.releaseFromHold)
						.IsHiddenWhen(conditions.IsApprovalDisabled)
						.WithFieldAssignments(fa => fa.Add<ARRegister.approved>(true)));
					actions.Add(g => g.reject, a => a
						.WithCategory(approvalCategory, g => g.approve)
						.PlaceAfter(g => g.approve)
						.IsHiddenWhen(conditions.IsApprovalDisabled)
						.WithFieldAssignments(fa => fa.Add<ARRegister.rejected>(true)));
					actions.Add(nameof(ARInvoiceEntry.Approval.ReassignApproval), a => a
						.WithCategory(approvalCategory)
						.PlaceAfter(g => g.reject)
						.IsHiddenWhen(conditions.IsApprovalDisabled));

					actions.Update(
						g => g.putOnHold,
						a => a.WithFieldAssignments(fas =>
						{
							fas.Add<ARRegister.approved>(false);
							fas.Add<ARRegister.rejected>(false);
						}));
					actions.Update(
						g => g.releaseFromCreditHold,
						a => a.PlaceAfterInCategory(g => g.reject));

					actions.Update(
						g => g.recalculateDiscountsAction,
						a => a.IsDisabledWhenElse(conditions.NonEditable));

					actions.Update(
						g => g.printInvoice,
						a => a.IsDisabledWhen(conditions.IsRejected));

					actions.Update(
						g => g.sendEmail,
						a => a.IsDisabledWhen(conditions.IsRejected));

					actions.Update(
						g => g.validateAddresses,
						a => a.IsDisabledWhen(conditions.IsRejected));
				}));
		}
	}
}

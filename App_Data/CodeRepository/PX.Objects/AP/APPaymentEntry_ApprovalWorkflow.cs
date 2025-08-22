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

using PX.Data;
using PX.Data.WorkflowAPI;
using System.Collections;

namespace PX.Objects.AP
{
	using State = APDocStatus;
	using static APPayment;
	using static BoundedTo<APPaymentEntry, APPayment>;

	public class APPaymentEntry_ApprovalWorkflow : PXGraphExtension<APPaymentEntry_Workflow, APPaymentEntry>
	{
		[PXWorkflowDependsOnType(typeof(APSetupApproval))]
		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<APPaymentEntry, APPayment>());

		public class Conditions : Condition.Pack
		{
			public Condition IsApproved => GetOrCreate(b => b.FromBql<
				APRegister.approved.IsEqual<True>
			>());

			public Condition IsRejected => GetOrCreate(b => b.FromBql<
				APRegister.rejected.IsEqual<True>
			>());

			public Condition IsApprovalDisabled => GetOrCreate(b => b.FromBql<
				Not<APSetupApproval.EPSetting.IsDocumentApprovable<docType, status>>
			>());
		}
		protected static void Configure(WorkflowContext<APPaymentEntry, APPayment> context)
		{
			var approvalCategory = context.Categories.Get(APPaymentEntry_Workflow.ActionCategory.Approval);
			var conditions = context.Conditions.GetPack<Conditions>();

			var approveAction = context.ActionDefinitions
				.CreateExisting<APPaymentEntry_ApprovalWorkflow>(g => g.approve, a => a
					.WithCategory(approvalCategory)
					.PlaceAfter(g => g.releaseFromHold)
					.IsHiddenWhen(conditions.IsApprovalDisabled)
					.WithFieldAssignments(fa => fa.Add<APRegister.approved>(e => e.SetFromValue(true))));

			var rejectAction = context.ActionDefinitions
				.CreateExisting<APPaymentEntry_ApprovalWorkflow>(g => g.reject, a => a
					.WithCategory(approvalCategory, approveAction)
					.PlaceAfter(approveAction)
					.IsHiddenWhen(conditions.IsApprovalDisabled)
					.WithFieldAssignments(fa => fa.Add<APRegister.rejected>(e => e.SetFromValue(true))));

			var reassignAction = context.ActionDefinitions
				.CreateExisting(nameof(APPaymentEntry.Approval.ReassignApproval), a => a
					.WithCategory(approvalCategory)
					.PlaceAfter(rejectAction)
					.IsHiddenWhen(conditions.IsApprovalDisabled));

			Workflow.ConfiguratorFlow InjectApprovalWorkflow(Workflow.ConfiguratorFlow flow)
			{
				const string initialState = "_";

				return flow
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
											actions.Add(approveAction, a => a.IsDuplicatedInToolbar());
											actions.Add(rejectAction, a => a.IsDuplicatedInToolbar());
											actions.Add(reassignAction);
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
											actions.Add(g => g.printAPEdit);
											actions.Add(g => g.vendorDocuments);
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
								.IsTriggeredOn(approveAction)
								.When(conditions.IsApproved));
							ts.Add(t => t
								.To<State.rejected>()
								.IsTriggeredOn(rejectAction)
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
					});
			}

			context.UpdateScreenConfigurationFor(screen =>
			{
				return screen
					.UpdateDefaultFlow(InjectApprovalWorkflow)
					.WithActions(actions =>
					{
						actions.Add(approveAction);
						actions.Add(rejectAction);
						actions.Add(reassignAction);
					});
			});
		}

		public PXAction<APPayment> approve;

		[PXButton(CommitChanges = true),
		 PXUIField(DisplayName = "Approve", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable Approve(PXAdapter adapter) => adapter.Get();

		public PXAction<APPayment> reject;

		[PXButton(CommitChanges = true),
		 PXUIField(DisplayName = "Reject", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable Reject(PXAdapter adapter) => adapter.Get();
	}
}

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

namespace PX.Objects.CA
{
	using State = CATransferStatus;
	using static CAAdj;
	using static BoundedTo<CATranEntry, CAAdj>;

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class CATranEntry_ApprovalWorkflow : PXGraphExtension<CATranEntry_Workflow, CATranEntry>
	{
		private class SetupApproval : IPrefetchable
		{
			private bool RequestApproval;

			void IPrefetchable.Prefetch()
			{
				using (PXDataRecord CASetup = PXDatabase.SelectSingle<CASetupApproval>(new PXDataField<CASetupApproval.isActive>()))
				{
					if (CASetup != null)
						RequestApproval = (bool)CASetup.GetBoolean(0);
				}
			}
			private static SetupApproval Slot => PXDatabase
				.GetSlot<SetupApproval>(typeof(SetupApproval).FullName, typeof(CASetupApproval));
			public static bool IsRequestApproval =>
				PXAccess.FeatureInstalled<CS.FeaturesSet.approvalWorkflow>() &&
				Slot.RequestApproval;
		}

		private static bool ApprovalIsActive => PXAccess.FeatureInstalled<CS.FeaturesSet.approvalWorkflow>();


		[PXWorkflowDependsOnType(typeof(CA.CASetupApproval))]
		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<CATranEntry, CAAdj>());

		public class Conditions : Condition.Pack
		{
			public Condition IsNotOnHoldAndIsApproved => GetOrCreate(c => c.FromBql<
				hold.IsEqual<False>.And<approved.IsEqual<True>>
			>());

			public Condition IsNotOnHoldAndIsNotApproved => GetOrCreate(c => c.FromBql<
				hold.IsEqual<False>.And<approved.IsEqual<False>>
			>());
			
			public Condition IsRejected => GetOrCreate(c => c.FromBql<
				rejected.IsEqual<True>
         			>());
			public Condition IsApproved => GetOrCreate(c => c.FromBql<
				approved.IsEqual<True>
			>());	
			
			public Condition IsNotApproved => GetOrCreate(c => c.FromBql<
				approved.IsEqual<False>.And<rejected.IsEqual<False>>
			>());	

			public Condition IsApprovalDisabled => GetOrCreate(c => 
				SetupApproval.IsRequestApproval
				? c.FromBql<True.IsEqual<False>>()
				: c.FromBql<status.IsNotIn<State.pending, State.rejected>>()
				);
		}
		protected static void Configure(WorkflowContext<CATranEntry, CAAdj> context)
		{
			var approvalCategory = context.Categories.Get(CATranEntry_Workflow.CategoryID.Approval);
			var conditions = context.Conditions.GetPack<Conditions>();

			var approve = context.ActionDefinitions
				.CreateExisting<CATranEntry_ApprovalWorkflow>(g => g.approve, a => a
				.InFolder(approvalCategory, g => g.releaseFromHold)
				.PlaceAfter(g => g.releaseFromHold)
				.IsHiddenWhen(conditions.IsApprovalDisabled)
				.IsDisabledWhen(context.Conditions.Get("IsNotAdjustment"))
				.WithFieldAssignments(fa => fa.Add<approved>(e => e.SetFromValue(true))));
			
			var reject = context.ActionDefinitions
				.CreateExisting<CATranEntry_ApprovalWorkflow>(g => g.reject, a => a
				.InFolder(approvalCategory, approve)
				.PlaceAfter(approve)
				.IsHiddenWhen(conditions.IsApprovalDisabled)
				.IsDisabledWhen(context.Conditions.Get("IsNotAdjustment"))
				.WithFieldAssignments(fa => fa.Add<rejected>(e => e.SetFromValue(true))));

			var reassign = context.ActionDefinitions
				.CreateExisting(nameof(CATranEntry.Approval.ReassignApproval), a => a
					.WithCategory(approvalCategory)
					.PlaceAfter(reject)
					.IsHiddenWhen(conditions.IsApprovalDisabled));

			Workflow.ConfiguratorFlow InjectApprovalWorkflow(Workflow.ConfiguratorFlow flow)
			{
				const string initialState = "_";

				return flow
					.WithFlowStates(states =>
					{
						states.Add<State.pending>(flowState =>
						{
							return flowState
								.WithActions(actions =>
								{
									actions.Add(approve, a => a.IsDuplicatedInToolbar());
									actions.Add(reject, a => a.IsDuplicatedInToolbar());
									actions.Add(reassign);
									actions.Add(g => g.putOnHold);
								})
								.WithFieldStates(fields =>
								{
									fields.AddAllFields<CAAdj>(table => table.IsDisabled());
									fields.AddField<adjRefNbr>();
									fields.AddTable<CASplit>(table => table.IsDisabled());
									fields.AddTable<CATaxTran>(table => table.IsDisabled());
								});
						});
						states.Add<State.rejected>(flowState =>
						{
							return flowState
								.WithActions(actions =>
								{
									actions.Add(g => g.putOnHold, a => a.IsDuplicatedInToolbar());
								})
								.WithFieldStates(fields =>
								{
									fields.AddAllFields<CAAdj>(table => table.IsDisabled());
									fields.AddField<adjRefNbr>();
									fields.AddTable<CASplit>(table => table.IsDisabled());
									fields.AddTable<CATaxTran>(table => table.IsDisabled());
								});
						});
					})
					.WithTransitions(transitions =>
					{
						transitions.UpdateGroupFrom(initialState, ts =>
						{
							ts.Add(t => t // New Pending Approval
								.To<State.pending>()
								.IsTriggeredOn(g => g.initializeState)
								.When(conditions.IsNotOnHoldAndIsNotApproved)
								);
							ts.Update(t => t
								.To<State.balanced>()
								.IsTriggeredOn(g => g.initializeState), t => t
								.When(conditions.IsNotOnHoldAndIsApproved)); // IsNotOnHold -> IsNotOnHoldAndIsApproved
						});

						transitions.UpdateGroupFrom<State.hold>(ts =>
						{
							ts.Update(
								t => t
								.To<State.balanced>()
								.IsTriggeredOn(g => g.releaseFromHold), t => t
								.When(conditions.IsApproved));
							ts.Add(t => t
								.To<State.pending>()
								.IsTriggeredOn(g => g.releaseFromHold)
								.DoesNotPersist()
								.When(conditions.IsNotApproved));
							ts.Add(t => t
								.To<State.rejected>()
								.IsTriggeredOn(g => g.releaseFromHold)
								.DoesNotPersist()
								.When(conditions.IsRejected));
							ts.Add(t => t
								.To<State.pending>()
								.IsTriggeredOn(g => g.OnUpdateStatus)
								.DoesNotPersist()
								.When(conditions.IsNotApproved));
							ts.Add(t => t
								.To<State.rejected>()
								.IsTriggeredOn(g => g.OnUpdateStatus)
								.DoesNotPersist()
								.When(conditions.IsRejected));
						});


						transitions.AddGroupFrom<State.pending>(ts =>
						{
							ts.Add(t => t
								.To<State.balanced>()
								.IsTriggeredOn(approve)
								.When(conditions.IsApproved));
							ts.Add(t => t
								.To<State.rejected>()
								.IsTriggeredOn(reject)
								.When(conditions.IsRejected));
							ts.Add(t => t
								.To<State.hold>()
								.IsTriggeredOn(g => g.putOnHold)
								.DoesNotPersist());
						});

						transitions.AddGroupFrom<State.rejected>(ts =>
						{
							ts.Add(t => t
								.To<State.hold>()
								.IsTriggeredOn(g => g.putOnHold)
								.DoesNotPersist());
						});
					});
			}

			context.UpdateScreenConfigurationFor(screen =>
			{
				return screen
					.UpdateDefaultFlow(InjectApprovalWorkflow)
					.WithActions(actions =>
					{
						actions.Add(approve);
						actions.Add(reject);
						actions.Add(reassign);
						actions.Update(
							g => g.putOnHold,
							a => a.WithFieldAssignments(fas =>
							{
								fas.Add<approved>(f => f.SetFromValue(false));
								fas.Add<rejected>(f => f.SetFromValue(false));
							}));
					});
			});
		}

		public PXAction<CAAdj> approve;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Approve", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable Approve(PXAdapter adapter) => adapter.Get();

		public PXAction<CAAdj> reject;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Reject", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable Reject(PXAdapter adapter) => adapter.Get();
	}
}

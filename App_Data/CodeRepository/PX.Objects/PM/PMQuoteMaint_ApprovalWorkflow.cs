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

namespace PX.Objects.PM
{
	using static BoundedTo<PMQuoteMaint, PMQuote>;

	public partial class PMQuoteMaint_ApprovalWorkflow : PXGraphExtension<PMQuoteMaint_Workflow, PMQuoteMaint>
	{
		public static bool IsActive() => true;
		private class PMQuoteSetupApproval : IPrefetchable
		{
			public static bool IsActive => PXDatabase.GetSlot<PMQuoteSetupApproval>(nameof(PMQuoteSetupApproval), typeof(PMSetup)).RequestApproval;

			private bool RequestApproval;

			void IPrefetchable.Prefetch()
			{
				using (PXDataRecord setup = PXDatabase.SelectSingle<PMSetup>(new PXDataField<PMSetup.quoteApprovalMapID>()))
				{
					if (setup != null)
						RequestApproval = setup.GetInt32(0).HasValue;
				}
			}
		}

		protected static bool ApprovalIsActive() => PXAccess.FeatureInstalled<CS.FeaturesSet.approvalWorkflow>() && PMQuoteSetupApproval.IsActive;

		[PXWorkflowDependsOnType(typeof(PMSetup))]
		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<PMQuoteMaint, PMQuote>());

		protected static void Configure(WorkflowContext<PMQuoteMaint, PMQuote> context)
		{
			Condition Bql<T>() where T : IBqlUnary, new() => context.Conditions.FromBql<T>();
			var conditions = new
			{
				IsRejected
					= Bql<PMQuote.rejected.IsEqual<True>>(),
				IsNotRejected
					= Bql<PMQuote.rejected.IsNotEqual<True>>(),
				IsApproved
					= Bql<PMQuote.approved.IsEqual<True>>(),
				IsNotApproved
					= Bql<PMQuote.approved.IsEqual<False>>(),
				IsApprovalDisabled
					= ApprovalIsActive()
						? Bql<True.IsEqual<False>>()
						: Bql<PMQuote.status.IsNotIn<PMQuoteStatusAttribute.pendingApproval, PMQuoteStatusAttribute.rejected>>()
			}.AutoNameConditions();

			#region Categories
			var approvalCategory = context.Categories.CreateNew(ToolbarCategory.ActionCategoryNames.Approval,
				category => category.DisplayName(ToolbarCategory.ActionCategory.Approval));
			#endregion

			var approve = context.ActionDefinitions
				.CreateExisting<PMQuoteMaint_ApprovalWorkflow>(g => g.approve, a => a
				.InFolder(approvalCategory, g => g.submit)
				.PlaceAfter(g => g.submit)
				.IsHiddenWhen(conditions.IsApprovalDisabled)
				.WithFieldAssignments(fa => fa.Add<PMQuote.approved>(e => e.SetFromValue(true))));

			var reject = context.ActionDefinitions
				.CreateExisting<PMQuoteMaint_ApprovalWorkflow>(g => g.reject, a => a
				.InFolder(approvalCategory, approve)
				.PlaceAfter(approve)
				.IsHiddenWhen(conditions.IsApprovalDisabled)
				.WithFieldAssignments(fa => fa.Add<PMQuote.rejected>(e => e.SetFromValue(true))));

			var reassign = context.ActionDefinitions
				.CreateExisting(nameof(PMQuoteMaint.Approval.ReassignApproval), a => a
				.WithCategory(approvalCategory)
				.PlaceAfter(reject)
				.IsHiddenWhen(conditions.IsApprovalDisabled));

			context.UpdateScreenConfigurationFor(screen =>
				screen
					.UpdateDefaultFlow(flow => flow
						.WithFlowStates(fss =>
						{
							fss.Add<PMQuoteStatusAttribute.pendingApproval>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(approve, c => c.IsDuplicatedInToolbar());
										actions.Add(reject, c => c.IsDuplicatedInToolbar());
										actions.Add(g => g.editQuote);
										actions.Add(reassign);
										actions.Add(g => g.copyQuote);
										actions.Add(g => g.primaryQuote);
									});
							});
							fss.Add<PMQuoteStatusAttribute.rejected>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.editQuote, c => c.IsDuplicatedInToolbar());
										actions.Add(g => g.copyQuote);
										actions.Add(g => g.primaryQuote);
									});
							});
						})
						.WithTransitions(transitions =>
						{
							transitions.UpdateGroupFrom<PMQuoteStatusAttribute.draft>(ts =>
							{
								ts.Update(t => t
									.To<PMQuoteStatusAttribute.approved>()
									.IsTriggeredOn(g => g.submit), t => t
									.When(conditions.IsApproved));
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.rejected>()
									.IsTriggeredOn(g => g.submit)
									.When(conditions.IsRejected));
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.pendingApproval>()
									.IsTriggeredOn(g => g.submit)
									.When(conditions.IsNotRejected)
									.When(conditions.IsNotApproved));
							});
							transitions.AddGroupFrom<PMQuoteStatusAttribute.pendingApproval>(ts =>
							{
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.approved>()
									.IsTriggeredOn(approve)
									.When(conditions.IsApproved));
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.rejected>()
									.IsTriggeredOn(reject)
									.When(conditions.IsRejected));
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.draft>()
									.IsTriggeredOn(g => g.editQuote));

							});
							transitions.AddGroupFrom<PMQuoteStatusAttribute.rejected>(ts =>
							{
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.draft>()
									.IsTriggeredOn(g => g.editQuote));
							});
						}))
					.WithActions(actions =>
					{
						actions.Add(approve);
						actions.Add(reject);
						actions.Add(reassign);
						actions.Update(
							g => g.editQuote,
							a => a.WithFieldAssignments(fa =>
							{
								fa.Add<PMQuote.approved>(f => f.SetFromValue(false));
								fa.Add<PMQuote.rejected>(f => f.SetFromValue(false));
							}));
					})
					.WithCategories(categories =>
					{
						categories.Add(approvalCategory);
						categories.Update(ToolbarCategory.ActionCategoryNames.Approval, category => category.PlaceAfter(context.Categories.Get(ToolbarCategory.ActionCategoryNames.Processing)));
					}));
		}

		public PXAction<PMQuote> approve;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Approve")]
		protected virtual IEnumerable Approve(PXAdapter adapter) => adapter.Get();

		public PXAction<PMQuote> reject;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Reject")]
		protected virtual IEnumerable Reject(PXAdapter adapter) => adapter.Get();
	}
}

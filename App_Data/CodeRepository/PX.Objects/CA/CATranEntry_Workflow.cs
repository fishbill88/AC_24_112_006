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

namespace PX.Objects.CA
{
	using State = CATransferStatus;
	using static CAAdj;
	using static BoundedTo<CATranEntry, CAAdj>;

	public partial class CATranEntry_Workflow : PXGraphExtension<CATranEntry>
	{
		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<CATranEntry, CAAdj>());
		public class Conditions : Condition.Pack
		{
			public Condition IsOnHold => GetOrCreate(c => c.FromBql<
				hold.IsEqual<True>
			>());

			public Condition IsBalanced => GetOrCreate(c => c.FromBql<
				approved.IsEqual<True>.And<hold.IsEqual<False>>
			>());
			public Condition IsNotAdjustment => GetOrCreate(c => c.FromBql<
				adjTranType.IsNotEqual<CATranType.cAAdjustment>
			>());
		}
		protected static void Configure(WorkflowContext<CATranEntry, CAAdj> context)
		{
			var conditions = context.Conditions.GetPack<Conditions>();
			#region Event Handlers
			WorkflowEventHandlerDefinition.IHandlerConfiguredBase OnReleaseDocument(WorkflowEventHandlerDefinition.INeedEventTarget handler)
			{
				return handler
					.WithTargetOf<CAAdj>()
					.OfEntityEvent<CAAdj.Events>(e => e.ReleaseDocument)
					.Is(g => g.OnReleaseDocument)
					.UsesTargetAsPrimaryEntity();
			}
			WorkflowEventHandlerDefinition.IHandlerConfiguredBase OnUpdateStatus(WorkflowEventHandlerDefinition.INeedEventTarget handler)
			{
				return handler
					.WithTargetOf<CAAdj>()
					.OfFieldUpdated<CAAdj.hold>()
					.Is(g => g.OnUpdateStatus)
					.UsesTargetAsPrimaryEntity();
			}

			#endregion
			#region Categories
			var processingCategory = context.Categories.CreateNew(CategoryID.Processing,
				category => category.DisplayName(CategoryNames.Processing));
			var correctionsCategory = context.Categories.CreateNew(CategoryID.Corrections,
				category => category.DisplayName(CategoryNames.Corrections));
			var approvalCategory = context.Categories.CreateNew(CategoryID.Approval,
				category => category.DisplayName(CategoryNames.Approval));
			#endregion

			const string initialState = "_";

			context.AddScreenConfigurationFor(screen =>
				screen
					.StateIdentifierIs<status>()
					.AddDefaultFlow(flow =>
						flow
						.WithFlowStates(fss =>
						{
							fss.Add(initialState, flowState => flowState.IsInitial(g => g.initializeState));
							fss.Add<State.hold>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.releaseFromHold, a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
									}).WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnUpdateStatus);
									});
							});
							fss.Add<State.balanced>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.Release, a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
										actions.Add(g => g.putOnHold, a => a.IsDuplicatedInToolbar());
									})
									.WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnReleaseDocument);
										handlers.Add(g => g.OnUpdateStatus);
									});
							});
							fss.Add<State.released>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.Reverse, a => a.IsDuplicatedInToolbar());
									})
									.WithFieldStates(fields =>
									{
										fields.AddAllFields<CAAdj>(table => table.IsDisabled());
										fields.AddField<CAAdj.depositAsBatch>();
										fields.AddField<adjRefNbr>();
										fields.AddTable<CASplit>(table => table.IsDisabled());
										fields.AddTable<CATaxTran>(table => table.IsDisabled());
									});;
							});
						})
					.WithTransitions(transitions =>
					{
						transitions.AddGroupFrom(initialState, ts =>
						{
							ts.Add(t => t.To<State.hold>()
								.IsTriggeredOn(g => g.initializeState)
								.When(conditions.IsOnHold)); // New Hold
							ts.Add(t => t.To<State.balanced>()
								.IsTriggeredOn(g => g.initializeState)
								.When(conditions.IsBalanced)); // New Balanced
						});
						transitions.AddGroupFrom<State.hold>(ts =>
						{
							ts.Add(t => t.To<State.balanced>()
								.IsTriggeredOn(g => g.releaseFromHold)
								.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(false))));
							ts.Add(t => t
								.To<State.balanced>()
								.IsTriggeredOn(g => g.OnUpdateStatus)
								.When(conditions.IsBalanced));
						});
						transitions.AddGroupFrom<State.balanced>(ts =>
						{
							ts.Add(t => t
								.To<State.hold>()
								.IsTriggeredOn(g => g.putOnHold)
								.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(true))));
							ts.Add(t => t.To<State.released>()
								.IsTriggeredOn(g => g.OnReleaseDocument));
							ts.Add(t => t
								.To<State.hold>()
								.IsTriggeredOn(g => g.OnUpdateStatus)
								.When(conditions.IsOnHold));
						});
					}
					))
					.WithActions(actions =>
					{
						actions.Add(g => g.initializeState, a => a.IsHiddenAlways());
						actions.Add(g => g.putOnHold, c => c
							.InFolder(processingCategory)
							.WithPersistOptions(ActionPersistOptions.NoPersist)
							.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(true))));
						actions.Add(g => g.releaseFromHold, c => c
							.InFolder(processingCategory)
							.WithPersistOptions(ActionPersistOptions.NoPersist)
							.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(false)))); 
						actions.Add(g => g.Release, c => c
							.InFolder(processingCategory)
							.PlaceAfter(nameof(CATranEntry.Last))
							.IsDisabledWhen(conditions.IsNotAdjustment));
						actions.Add(g => g.Reverse, c => c
							.InFolder(correctionsCategory)
							.IsDisabledWhen(conditions.IsNotAdjustment));
						actions.Add<CATranEntry.CATranEntry_ActivityDetailsExt>(g => g.ViewActivities, c => c
							.InFolder(FolderType.ReportsFolder));
					})
					.WithHandlers(handlers =>
					{
						handlers.Add(handler => handler
							.WithTargetOf<CAAdj>()
							.OfEntityEvent<CAAdj.Events>(e => e.ReleaseDocument)
							.Is(g => g.OnReleaseDocument)
							.UsesTargetAsPrimaryEntity());
						handlers.Add(handler => handler
							.WithTargetOf<CAAdj>()
							.OfFieldUpdated<CAAdj.hold>()
							.Is(g => g.OnUpdateStatus)
							.UsesTargetAsPrimaryEntity());
					})
					.WithCategories(categories =>
					{
						categories.Add(processingCategory);
						categories.Add(correctionsCategory);
						categories.Add(approvalCategory);
					})
			);
		}
		
		public static class CategoryNames
		{
			public const string Processing = "Processing";
			public const string Corrections = "Corrections";
			public const string Approval = "Approval";
		}

		public static class CategoryID
		{
			public const string Processing = "ProcessingID";
			public const string Corrections = "CorrectionsID";
			public const string Approval = "ApprovalID";
		}
	}
}

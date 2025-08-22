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

namespace PX.Objects.GL
{
	using State = GLDocBatchStatus;
	using static GLDocBatch;
	using static BoundedTo<JournalWithSubEntry, GLDocBatch>;

	public partial class JournalWithSubEntry_Workflow : PXGraphExtension<JournalWithSubEntry>
	{
		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<JournalWithSubEntry, GLDocBatch>());

		public class Conditions : Condition.Pack
		{
			public Condition IsOnHold => GetOrCreate(c => c.FromBql<
				hold.IsEqual<True>
			>());

			public Condition IsNotOnHold => GetOrCreate(c => c.FromBql<
				hold.IsEqual<False>
			>());
		}
		protected static void Configure(WorkflowContext<JournalWithSubEntry, GLDocBatch> context)
		{
			var conditions = context.Conditions.GetPack<Conditions>();
			#region Categories
			var processingCategory = context.Categories.CreateNew(ActionCategoryNames.Processing,
				category => category.DisplayName(ActionCategory.Processing));
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
									});;
							});
							fss.Add<State.balanced>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.release, a => a
											.IsDuplicatedInToolbar()
											.WithConnotation(ActionConnotation.Success));
										actions.Add(g => g.putOnHold, a => a
											.IsDuplicatedInToolbar());

									}).WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnUpdateStatus);
										handlers.Add(g => g.OnReleaseVoucher);
									});;
							});
							fss.Add<State.completed>();
							fss.Add<State.released>();
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
								.When(conditions.IsNotOnHold)); // New Hold
						});
						transitions.AddGroupFrom<State.hold>(ts =>
						{
							ts.Add(t => t.To<State.balanced>()
								.IsTriggeredOn(g => g.releaseFromHold)
								.DoesNotPersist()
								.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(false))));
							ts.Add(t => t
								.To<State.balanced>()
								.IsTriggeredOn(g => g.OnUpdateStatus)
								.When(conditions.IsNotOnHold));
						});
						transitions.AddGroupFrom<State.balanced>(ts =>
						{
							ts.Add(t => t.To<State.released>()
								.IsTriggeredOn(g => g.OnReleaseVoucher));
							ts.Add(t => t.To<State.hold>()
								.IsTriggeredOn(g => g.putOnHold)
								.DoesNotPersist()
								.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(true))));
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
						actions.Add(g => g.releaseFromHold, c => c
							.InFolder(processingCategory)
							.PlaceAfter(nameof(JournalWithSubEntry.Last))
							.WithPersistOptions(ActionPersistOptions.NoPersist)
							.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(false))));
						actions.Add(g => g.putOnHold, c => c
							.InFolder(processingCategory)
							.WithPersistOptions(ActionPersistOptions.NoPersist)
							.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(true))));
						actions.Add(g => g.release, c => c
							.InFolder(processingCategory));
					})
					.WithHandlers(handlers =>
					{
						handlers.Add(handler => handler
							.WithTargetOf<GLDocBatch>()
							.OfFieldUpdated<GLDocBatch.hold>()
							.Is(g => g.OnUpdateStatus)
							.UsesTargetAsPrimaryEntity());
						handlers.Add(handler => handler
							.WithTargetOf<GLDocBatch>()
							.OfEntityEvent<GLDocBatch.Events>(e => e.ReleaseVoucher)
							.Is(g => g.OnReleaseVoucher)
							.UsesTargetAsPrimaryEntity());
					})
					.WithCategories(categories =>
					{
						categories.Add(processingCategory);
					})
			);
		}
		public static class ActionCategoryNames
		{
			public const string Processing = "Processing";
		}

		public static class ActionCategory
		{
			public const string Processing = "Processing";
		}
	}
}

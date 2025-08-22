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
using System.Collections.Generic;
using System.Linq;
using PX.Objects.CR.CRCaseMaint_Extensions;

namespace PX.Objects.CR.Workflows
{
	using static PX.Data.WorkflowAPI.BoundedTo<CRCaseMaint, CRCase>;

	/// <summary>
	/// Extensions that used to configure Workflow for <see cref="CRCaseMaint"/> and <see cref="CRCase"/>.
	/// Use Extensions Chaining for this extension if you want customize workflow with code for this graph of DAC.
	/// </summary>
	public class CaseWorkflow : PX.Data.PXGraphExtension<CRCaseMaint>
	{
		#region Conditions
		public class Conditions : Condition.Pack
		{
			public Condition IsActive => GetOrCreate(
				b => b.FromBql<CRCase.isActive.IsEqual<True>>());

			public Condition IsClosureNotesRequired => GetOrCreate(
				b => b.FromBql<CRCaseClass.requireClosureNotes.FromCurrent.IsEqual<True>>());

			public Condition TrackSolutionsInActivities => GetOrCreate(
				b => b.FromBql<CRCaseClass.trackSolutionsInActivities.FromCurrent.IsEqual<True>>());
		}
		#endregion

		#region Actions

		public PXAction<CRCase> openCaseFromProcessing;
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible, DisplayName = "Open")]
		[PXButton]
		protected virtual void OpenCaseFromProcessing() { }

		public PXAction<CRCase> openCaseFromPortal;
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible, DisplayName = "Open")]
		[PXButton]
		protected virtual void OpenCaseFromPortal() { }

		public PXAction<CRCase> closeCaseFromPortal;
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible, DisplayName = "Close")]
		[PXButton]
		protected virtual void CloseCaseFromPortal() { }

		#endregion

		#region Consts

        /// <summary>
        /// Statuses for <see cref="CROpportunity.status"/> used by default in system workflow.
        /// Values could be changed and extended by workflow.
        /// </summary>
        public static class States
        {
            public const string New = "N";
            public const string Open = "O";
            public const string Closed = "C";
            public const string Released = "R";
            public const string PendingCustomer = "P";

			internal class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
						new[]
						{
							New,
							Open,
							Closed,
							Released,
							PendingCustomer,
						},
						new[]
						{
							"New",
							"Open",
							"Closed",
							"Released",
							"Pending Customer",
						}
					)
				{ }
			}
		}

		public static class Reasons
		{
			public const string Rejected = "RJ";
			public const string Resolved = "RD";
			public const string MoreInfoRequested = "MI";
			public const string InProcess = "IP";
			public const string Internal = "IN";
			public const string InEscalation = "ES";
			public const string Duplicate = "DP";
			public const string SolutionProvided = "CR";
			public const string CustomerPostpone = "CP";
			public const string Canceled = "CL";
			public const string PendingClosure = "CC";
			public const string Abandoned = "CA";
			public const string Unassigned = "AS";
			public const string Assigned = "AA";
			public const string Updated = "AD";
			public const string ClosedOnPortal = "PC";
			public const string OpenedOnPortal = "PO";

			public static class Messages
			{
				// ReSharper disable MemberHidesStaticFromOuterClass
				public const string Rejected = "Rejected";
				public const string Resolved = "Resolved";
				public const string MoreInfoRequested = "More Info Requested";
				public const string InProcess = "In Process";
				public const string Internal = "Internal";
				public const string InEscalation = "In Escalation";
				public const string Duplicate = "Duplicate";
				public const string SolutionProvided = "Solution Provided";
				public const string CustomerPostpone = "Customer Postpone";
				public const string Canceled = "Canceled";
				public const string PendingClosure = "Pending Closure";
				public const string Abandoned = "Abandoned";
				public const string Unassigned = "Unassigned";
				public const string Assigned = "Assigned";
				public const string Updated = "Updated";
				public const string ClosedOnPortal = "Closed on Portal";
				public const string OpenedOnPortal = "Opened on Portal";
				// ReSharper restore MemberHidesStaticFromOuterClass
			}
		}

		private static class FieldNames
		{
			public const string Reason = nameof(Reason);
			public const string Owner = nameof(Owner);
			public const string SolutionActivityNoteID = nameof(SolutionActivityNoteID);
			public const string ClosureNotes = nameof(ClosureNotes);
		}

		public static class CategoryNames
		{
			public const string Processing = "Processing";
			public const string Services = "CustomerServices";
			public const string Activities = "Activities";
			public const string Other = "Other";
		}

		public static class CategoryDisplayNames
		{
			public const string Processing = "Processing";
			public const string Services = "Customer Services";
			public const string Activities = "Activities";
			public const string Other = "Other";
		}

		#endregion

		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<CRCaseMaint, CRCase>());

		protected static void Configure(WorkflowContext<CRCaseMaint, CRCase> context)
		{
			var conditions = context.Conditions.GetPack<Conditions>();

			#region forms

			var reasons = new Dictionary<string, string[]>(5)
			{
				[States.New] = new[] { Reasons.Unassigned, Reasons.Assigned },
				[States.Open] = new[] { Reasons.InProcess, Reasons.Updated, Reasons.InEscalation, Reasons.PendingClosure, Reasons.Assigned },
				[States.PendingCustomer] = new[] { Reasons.MoreInfoRequested, Reasons.SolutionProvided, Reasons.PendingClosure },
				[States.Closed] = new[] { Reasons.Resolved, Reasons.Rejected, Reasons.Canceled, Reasons.Abandoned, Reasons.Duplicate },
				[States.Released] = new[] { Reasons.Resolved, Reasons.Canceled },
			};

			var formOpen = context.Forms.Create("Form" + nameof(CRCaseMaint.Open), form => form
				.Prompt("Open")
				.WithFields(fields =>
				{
					AddResolutionFormField(fields, Reasons.InProcess);
					fields.Add(FieldNames.Owner, field => field
						.WithSchemaOf<CRCase.ownerID>()
						.Prompt("Owner")
						.DefaultValueFromSchemaField());
				}));

			var formPendingCustomer = context.Forms.Create("Form" + nameof(CRCaseMaint.PendingCustomer), form => form
				.Prompt("Pending Customer")
				.WithFields(fields =>
				{
					AddResolutionFormField(fields, Reasons.SolutionProvided);
					AddSolutionActivityNoteIDFormField(fields);
				}));

			var formClose = context.Forms.Create("Form" + nameof(CRCaseMaint.Close), form => form
				.Prompt("Close")
				.WithFields(fields =>
				{
					AddResolutionFormField(fields, Reasons.Resolved);
					AddSolutionActivityNoteIDFormField(fields);

					fields.Add(FieldNames.ClosureNotes, field => field
						.WithRichTextEditorField()
						.IsRequiredWhen(conditions.IsClosureNotesRequired)
						.Prompt("Closure Notes")
						.DefaultExpression($"[{nameof(CRCase.closureNotes)}]"));
				}));

			void AddResolutionFormField(FormField.IContainerFillerFields fields, string defaultValue)
			{
				fields.Add(FieldNames.Reason, field => field
					.WithSchemaOf<CRCase.resolution>()
					.IsRequired()
					.Prompt("Reason")
					.ComboBoxValuesSource(ComboBoxValuesSource.TargetState)
					.DefaultValue(defaultValue));
			}

			void AddSolutionActivityNoteIDFormField(FormField.IContainerFillerFields fields)
			{
				fields.Add(FieldNames.SolutionActivityNoteID, field => field
					.WithSchemaOf<CRCase.solutionActivityNoteID>()
					.DefaultValueFromSchemaField()
					.IsHiddenWhen(!conditions.TrackSolutionsInActivities)
					.Prompt("Solution Provided In"));
			}

			#endregion

			#region categories

			var categoryProcessing = context.Categories.CreateNew(CategoryNames.Processing,
				category => category.DisplayName(CategoryDisplayNames.Processing));
			var categoryServices = context.Categories.CreateNew(CategoryNames.Services,
				category => category.DisplayName(CategoryDisplayNames.Services));
			var categoryActivities = context.Categories.CreateNew(CategoryNames.Activities,
				category => category.DisplayName(CategoryDisplayNames.Activities));
			var categoryOther = context.Categories.CreateNew(CategoryNames.Other,
				category => category.DisplayName(CategoryDisplayNames.Other));

			#endregion

			var actionOpen = context.ActionDefinitions.CreateExisting(g => g.Open, a => a
				.WithFieldAssignments(fields =>
				{
					fields.Add<CRCase.isActive>(f => f.SetFromValue(true));
					fields.Add<CRCase.resolution>(f => f.SetFromFormField(formOpen, FieldNames.Reason));
					fields.Add<CRCase.ownerID>(f => f.SetFromFormField(formOpen, FieldNames.Owner));
					fields.Add<CRCase.resolutionDate>(f => f.SetFromValue(null));
				})
				.DisplayName("Open")
				.WithCategory(categoryProcessing)
				.MapEnableToUpdate()
				.WithForm(formOpen)
				.WithPersistOptions(ActionPersistOptions.PersistBeforeAction)
				.IsExposedToMobile(true)
				.MassProcessingScreen<UpdateCaseMassProcess>());

			var gactionTakeCase = context.ActionDefinitions.CreateExisting(g => g.takeCase, a => a
				.DoesNotPersist()
				.WithCategory(categoryProcessing)
				.PlaceAfter(actionOpen));

			var actionClose = context.ActionDefinitions.CreateExisting(g => g.Close, a => a
				.WithFieldAssignments(fields =>
				{
					fields.Add<CRCase.isActive>(f => f.SetFromValue(false));
					fields.Add<CRCase.resolutionDate>(f => f.SetFromNow());
					fields.Add<CRCase.resolution>(f => f.SetFromFormField(formClose, FieldNames.Reason));
					fields.Add<CRCase.solutionActivityNoteID>(f => f.SetFromFormField(formClose, FieldNames.SolutionActivityNoteID));
					fields.Add<CRCase.closureNotes>(f => f.SetFromFormField(formClose, FieldNames.ClosureNotes));
				})
				.DisplayName("Close")
				.WithCategory(categoryProcessing)
				.PlaceAfter(gactionTakeCase)
				.WithForm(formClose)
				.MapEnableToUpdate()
				.WithPersistOptions(ActionPersistOptions.PersistBeforeAction)
				.IsExposedToMobile(true)
				.MassProcessingScreen<UpdateCaseMassProcess>());

			var actionPending = context.ActionDefinitions.CreateExisting(g => g.PendingCustomer, a => a
				.WithFieldAssignments(fields =>
				{
					fields.Add<CRCase.isActive>(f => f.SetFromValue(true));
					fields.Add<CRCase.resolutionDate>(f => f.SetFromValue(null));
					fields.Add<CRCase.resolution>(f => f.SetFromFormField(formPendingCustomer, FieldNames.Reason));
					fields.Add<CRCase.solutionActivityNoteID>(f => f.SetFromFormField(formPendingCustomer, FieldNames.SolutionActivityNoteID));
				})
				.DisplayName("Pending Customer")
				.WithCategory(categoryProcessing)
				.PlaceAfter(actionClose)
				.WithForm(formPendingCustomer)
				.MapEnableToUpdate()
				.WithPersistOptions(ActionPersistOptions.PersistBeforeAction)
				.IsExposedToMobile(true)
				.MassProcessingScreen<UpdateCaseMassProcess>());

			var gactionRelease = context.ActionDefinitions.CreateExisting(g => g.release, a => a
				.WithCategory(categoryProcessing)
				.PlaceAfter(actionPending)
				.WithPersistOptions(ActionPersistOptions.PersistBeforeAction)
				.MassProcessingScreen<CRCaseReleaseProcess>());

			var gactionAssign = context.ActionDefinitions.CreateExisting(g => g.assign, a => a
				.WithCategory(categoryOther));

			var gactionViewInvoice = context.ActionDefinitions.CreateExisting(g => g.viewInvoice, a => a
				.WithCategory(categoryOther)
				.WithPersistOptions(ActionPersistOptions.PersistBeforeAction));

            var gactionOpenFromPortal = context.ActionDefinitions.CreateExisting<CaseWorkflow>(g => g.openCaseFromPortal, a => a
				.WithCategory(categoryOther)
				.DisplayName("Open Case in Portal")
				.WithPersistOptions(ActionPersistOptions.PersistBeforeAction));

            var gactionOpenFromProcessing = context.ActionDefinitions.CreateExisting<CaseWorkflow>(g => g.openCaseFromProcessing, a => a
				.WithCategory(categoryOther)
				.DisplayName("Reopen Case from Email")
				.WithPersistOptions(ActionPersistOptions.PersistBeforeAction));

            var gactionCloseFromPortal = context.ActionDefinitions.CreateExisting<CaseWorkflow>(g => g.closeCaseFromPortal, a => a
				.WithCategory(categoryOther)
				.DisplayName("Close Case from Portal")
				.WithPersistOptions(ActionPersistOptions.PersistBeforeAction));


			var actionCreateEmail = context.ActionDefinitions.CreateExisting(
				CRCaseMaint_ActivityDetailsExt_Actions.ActionNames.NewMailActivity_Workflow,
				a => a.WithCategory(categoryActivities));

			var actionCreateWorkItem = context.ActionDefinitions.CreateExisting(
				CRCaseMaint_ActivityDetailsExt_Actions.ActionNames.NewActivity_Workitem_Workflow,
				a => a.WithCategory(categoryActivities));

			var actionCreateNote = context.ActionDefinitions.CreateExisting(
				CRCaseMaint_ActivityDetailsExt_Actions.ActionNames.NewActivity_Note_Workflow,
				a => a.WithCategory(categoryActivities));

			var actionCreateTask = context.ActionDefinitions.CreateExisting(
				CRCaseMaint_ActivityDetailsExt_Actions.ActionNames.NewTask_Workflow,
				a => a.WithCategory(categoryActivities));

			var actionCreatePhoneCall = context.ActionDefinitions.CreateExisting(
				CRCaseMaint_ActivityDetailsExt_Actions.ActionNames.NewActivity_Phonecall_Workflow,
				a => a.WithCategory(categoryActivities));

			var actionCreateReturnOrder =
				context.ActionDefinitions.CreateExisting<CRCaseMaint_CRCreateReturnOrder>(g => g.CreateReturnOrder, a => a.WithCategory(categoryServices));

			context.AddScreenConfigurationFor(screen =>
			{
				return screen
					.StateIdentifierIs<CRCase.status>()
					.AddDefaultFlow(DefaultCaseFlow)
					.WithActions(actions =>
					{
						actions.Add(gactionTakeCase);
						actions.Add(actionOpen);
						actions.Add(actionPending);
						actions.Add(actionClose);
						actions.Add(gactionRelease);

						actions.Add(actionCreateEmail);
						actions.Add(actionCreateWorkItem);
						actions.Add(actionCreateNote);
						actions.Add(actionCreateTask);
						actions.Add(actionCreatePhoneCall);

						actions.Add(gactionAssign);
						actions.Add(gactionViewInvoice);
                        actions.Add(gactionOpenFromPortal);
                        actions.Add(gactionOpenFromProcessing);
                        actions.Add(gactionCloseFromPortal);

						actions.Add(actionCreateReturnOrder);
					})
					.WithForms(forms =>
					{
						forms.Add(formOpen);
						forms.Add(formPendingCustomer);
						forms.Add(formClose);
					})
					.WithFieldStates(fields =>
					{
						fields.Add<CRCase.resolution>(field => field
							.SetComboValues(
								(Reasons.Rejected, Reasons.Messages.Rejected),
								(Reasons.Resolved, Reasons.Messages.Resolved),
								(Reasons.MoreInfoRequested, Reasons.Messages.MoreInfoRequested),
								(Reasons.InProcess, Reasons.Messages.InProcess),
								(Reasons.Internal, Reasons.Messages.Internal),
								(Reasons.InEscalation, Reasons.Messages.InEscalation),
								(Reasons.Duplicate, Reasons.Messages.Duplicate),
								(Reasons.SolutionProvided, Reasons.Messages.SolutionProvided),
								(Reasons.CustomerPostpone, Reasons.Messages.CustomerPostpone),
								(Reasons.Canceled, Reasons.Messages.Canceled),
								(Reasons.PendingClosure, Reasons.Messages.PendingClosure),
								(Reasons.Abandoned, Reasons.Messages.Abandoned),
								(Reasons.Unassigned, Reasons.Messages.Unassigned),
								(Reasons.Assigned, Reasons.Messages.Assigned),
								(Reasons.Updated, Reasons.Messages.Updated),
								(Reasons.ClosedOnPortal, Reasons.Messages.ClosedOnPortal),
								(Reasons.OpenedOnPortal, Reasons.Messages.OpenedOnPortal)));
						fields.Add<CRCase.resolutionDate>(f => f.IsHiddenWhen(conditions.IsActive));
					})
					.WithCategories(categories =>
					{
						categories.Add(categoryProcessing);
						categories.Add(categoryServices);
						categories.Add(categoryActivities);
						categories.Add(categoryOther);
					});
			});

			Workflow.IConfigured DefaultCaseFlow(Workflow.INeedStatesFlow flow)
			{
				#region states

				var newState = context.FlowStates.Create(States.New, state => state
					.IsInitial()
					.WithFieldStates(fields =>
					{
						fields.AddField<CRCase.resolution>(field => field
							.DefaultValue(Reasons.Unassigned)
							.ComboBoxValues(reasons[States.New].Union(new[] { Reasons.OpenedOnPortal }).ToArray()));
						fields.AddField<CRCase.isActive>(field => field.IsDisabled());
					})
					.WithActions(actions =>
					{
						AddOpenAction(actions, isDuplicationInToolbar: true, withConnotation: true);
						actions.Add(g => g.takeCase, a => a.IsDuplicatedInToolbar());
						actions.Add(g => g.assign);
						AddPendingCustomerAction(actions);
						AddCloseAction(actions);
                        actions.Add<CaseWorkflow>(g=>g.openCaseFromPortal);
                        actions.Add<CaseWorkflow>(g=>g.closeCaseFromPortal);
					}));

				var openState = context.FlowStates.Create(States.Open, state => state
					.WithFieldStates(fields =>
					{
						fields.AddField<CRCase.resolution>(field =>
							field.ComboBoxValues(reasons[States.Open].Union(new[] { Reasons.OpenedOnPortal }).ToArray()));
						fields.AddField<CRCase.isActive>(field => field.IsDisabled());
					})
					.WithActions(actions =>
					{
						actions.Add(g => g.takeCase, a => a.IsDuplicatedInToolbar());
						actions.Add(g => g.assign);
						AddPendingCustomerAction(actions);
						AddCloseAction(actions, isDuplicationInToolbar: true, withConnotation: true);
						actions.Add<CaseWorkflow>(g=>g.openCaseFromProcessing);
                        actions.Add<CaseWorkflow>(g=>g.openCaseFromPortal);
                        actions.Add<CaseWorkflow>(g=>g.closeCaseFromPortal);
					}));

				var pendingState = context.FlowStates.Create(States.PendingCustomer, state => state
					.WithFieldStates(fields =>
					{
						fields.AddField<CRCase.resolution>(field => field
							.ComboBoxValues(reasons[States.PendingCustomer])
							.IsDisabled());
						fields.AddField<CRCase.isActive>(field => field.IsDisabled());
					})
					.WithActions(actions =>
					{
						actions.Add(g => g.takeCase, a => a.IsDuplicatedInToolbar());
						AddOpenAction(actions);
						AddCloseAction(actions, isDuplicationInToolbar: true);
                        actions.Add<CaseWorkflow>(g=>g.openCaseFromProcessing);
                        actions.Add<CaseWorkflow>(g=>g.openCaseFromPortal);
                        actions.Add<CaseWorkflow>(g=>g.closeCaseFromPortal);
					}));

				var closedState = context.FlowStates.Create(States.Closed, state => state
					.WithFieldStates(fields =>
					{
						fields.AddField<CRCase.resolution>(field => field
							.ComboBoxValues(reasons[States.Closed].Union(new[] { Reasons.ClosedOnPortal }).ToArray())
							.IsDisabled());

						DisableFieldsForFinalStates(fields);

						fields.AddField<CRCase.isBillable>();
						fields.AddField<CRCase.manualBillableTimes>();
						fields.AddField<CRCase.timeBillable>();
						fields.AddField<CRCase.overtimeBillable>();
					})
					.WithActions(actions =>
					{
						AddOpenAction(actions);
						AddPendingCustomerAction(actions);
						actions.Add(g => g.release, action => action.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
                        actions.Add<CaseWorkflow>(g=>g.openCaseFromProcessing);
                        actions.Add<CaseWorkflow>(g=>g.openCaseFromPortal);
					}));

				var releasedState = context.FlowStates.Create(States.Released, state => state
					.WithFieldStates(fields =>
					{
						fields.AddField<CRCase.resolution>(field => field
							.ComboBoxValues(reasons[States.Released])
							.IsDisabled());

						DisableFieldsAndAttributesForFinalStates(fields);
					})
					.WithActions(actions =>
					{
						actions.Add(g => g.viewInvoice);
					}));


				#endregion

				return flow
					.WithFlowStates(states =>
					{
						states.Add(newState);
						states.Add(openState);
						states.Add(pendingState);
						states.Add(closedState);
						states.Add(releasedState);
					})
					.WithTransitions(transitions =>
					{
						#region new

						transitions.Add(transition => transition
							.From(newState)
							.To(newState)
							.IsTriggeredOn(gactionTakeCase)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.Assigned));
							})
							.DoesNotPersist());

						transitions.Add(transition => transition
							.From(newState)
							.To(openState)
							.IsTriggeredOn(actionOpen));

						transitions.Add(transition => transition
							.From(newState)
							.To(pendingState)
							.IsTriggeredOn(actionPending));

						transitions.Add(transition => transition
							.From(newState)
							.To(closedState)
							.IsTriggeredOn(actionClose));

						transitions.Add(transition => transition
							.From(newState)
							.To(openState)
							.IsTriggeredOn<CaseWorkflow>(e => e.openCaseFromPortal)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(true));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.OpenedOnPortal));
							}));

						transitions.Add(transition => transition
							.From(newState)
							.To(closedState)
							.IsTriggeredOn<CaseWorkflow>(e => e.closeCaseFromPortal)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(false));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.ClosedOnPortal));
							}));

                        #endregion
                        #region open

						transitions.Add(transition => transition
							.From(openState)
							.To(openState)
							.IsTriggeredOn(gactionTakeCase)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.Assigned));
							})
							.DoesNotPersist());

						transitions.Add(transition => transition
							.From(openState)
							.To(pendingState)
							.IsTriggeredOn(actionPending));

						transitions.Add(transition => transition
							.From(openState)
							.To(closedState)
							.IsTriggeredOn(actionClose));

						transitions.Add(transition => transition
							.From(openState)
							.To(openState)
							.IsTriggeredOn<CaseWorkflow>(e => e.openCaseFromProcessing)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(true));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.Updated));
							}));

						transitions.Add(transition => transition
							.From(openState)
							.To(openState)
							.IsTriggeredOn<CaseWorkflow>(e => e.openCaseFromPortal)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(true));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.OpenedOnPortal));
							}));

						transitions.Add(transition => transition
							.From(openState)
							.To(closedState)
							.IsTriggeredOn<CaseWorkflow>(e => e.closeCaseFromPortal)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(false));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.ClosedOnPortal));
							}));

						#endregion
						#region pending

						transitions.Add(transition => transition
							.From(pendingState)
							.To(openState)
							.IsTriggeredOn(actionOpen)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.solutionActivityNoteID>(f => f.SetFromExpression("=null"));
							}));

						transitions.Add(transition => transition
							.From(pendingState)
							.To(closedState)
							.IsTriggeredOn(actionClose));

						transitions.Add(transition => transition
							.From(pendingState)
							.To(openState)
							.IsTriggeredOn<CaseWorkflow>(e => e.openCaseFromProcessing)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(true));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.Updated));
								fields.Add<CRCase.solutionActivityNoteID>(f => f.SetFromExpression("=null"));
							}));

						transitions.Add(transition => transition
							.From(pendingState)
							.To(openState)
							.IsTriggeredOn<CaseWorkflow>(e => e.openCaseFromPortal)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(true));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.OpenedOnPortal));
								fields.Add<CRCase.solutionActivityNoteID>(f => f.SetFromExpression("=null"));
							}));

						transitions.Add(transition => transition
							.From(pendingState)
							.To(closedState)
							.IsTriggeredOn<CaseWorkflow>(e => e.closeCaseFromPortal)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(false));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.ClosedOnPortal));
							}));

						#endregion
						#region closed

						transitions.Add(transition => transition
							.From(closedState)
							.To(openState)
							.IsTriggeredOn(actionOpen)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.solutionActivityNoteID>(f => f.SetFromExpression("=null"));
							}));

						transitions.Add(transition => transition
							.From(closedState)
							.To(pendingState)
							.IsTriggeredOn(actionPending));

						transitions.Add(transition => transition
							.From(closedState)
							.To(releasedState)
							.IsTriggeredOn(g => g.release)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(false));
							}));

						transitions.Add(transition => transition
							.From(closedState)
							.To(openState)
							.IsTriggeredOn<CaseWorkflow>(e => e.openCaseFromProcessing)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(true));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.Updated));
								fields.Add<CRCase.solutionActivityNoteID>(f => f.SetFromExpression("=null"));
							}));

						transitions.Add(transition => transition
							.From(closedState)
							.To(openState)
							.IsTriggeredOn<CaseWorkflow>(e => e.openCaseFromPortal)
							.WithFieldAssignments(fields =>
							{
								fields.Add<CRCase.isActive>(f => f.SetFromValue(true));
								fields.Add<CRCase.resolution>(f => f.SetFromValue(Reasons.OpenedOnPortal));
								fields.Add<CRCase.solutionActivityNoteID>(f => f.SetFromExpression("=null"));
							}));

						#endregion
					});

				void AddOpenAction(ActionState.IContainerFillerActions filler, bool isDuplicationInToolbar = false, bool withConnotation = false)
				{
					filler.Add(actionOpen, a => a.IsDuplicatedInToolbar(isDuplicationInToolbar).WithSuccessConnotation(withConnotation));
				}

				void AddPendingCustomerAction(ActionState.IContainerFillerActions filler, bool isDuplicationInToolbar = false)
				{
					filler.Add(actionPending, a => a.IsDuplicatedInToolbar(isDuplicationInToolbar));
				}

				void AddCloseAction(ActionState.IContainerFillerActions filler, bool isDuplicationInToolbar = false, bool withConnotation = false)
				{
					filler.Add(actionClose, a => a.IsDuplicatedInToolbar(isDuplicationInToolbar).WithSuccessConnotation(withConnotation));
				}

				void DisableFieldsForFinalStates(FieldState.IContainerFillerFields fields)
				{
					fields.AddTable<CRCase>(field => field.IsDisabled());
					fields.AddTable<CRPMTimeActivity>(field => field.IsDisabled());
					fields.AddField<CRCase.caseCD>();
					fields.AddField<CRCase.closureNotes>();
				}

				void DisableFieldsAndAttributesForFinalStates(FieldState.IContainerFillerFields fields)
				{
					DisableFieldsForFinalStates(fields);
					fields.AddTable<CS.CSAnswers>(field => field.IsDisabled());
				}
			}
		}

	}

	internal static partial class ActionConnotations
	{
		public static BoundedTo<CRCaseMaint, CRCase>.ActionState.IAllowOptionalConfig WithSuccessConnotation(
			this BoundedTo<CRCaseMaint, CRCase>.ActionState.IAllowOptionalConfig actionConfig,
			bool applyConnotation)
		{
			return applyConnotation
						? actionConfig.WithConnotation(ActionConnotation.Success)
						: actionConfig;
		}
	}
}

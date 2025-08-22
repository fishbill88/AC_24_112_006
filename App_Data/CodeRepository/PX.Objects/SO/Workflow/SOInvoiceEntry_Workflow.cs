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
using PX.Common;
using PX.Data.WorkflowAPI;
using PX.Objects.Common;
using PX.Objects.AR;
using PX.Objects.SO.GraphExtensions.SOInvoiceEntryExt;

namespace PX.Objects.SO
{
	using State = ARDocStatus;
	using static ARInvoice;
	using static BoundedTo<SOInvoiceEntry, ARInvoice>;

	public class SOInvoiceEntry_Workflow : PXGraphExtension<SOInvoiceEntry>
	{
		public class Conditions : Condition.Pack
		{
			public Condition IsOnHold => GetOrCreate(b => b.FromBql<
				released.IsEqual<False>.And<hold.IsEqual<True>>
			>());

			public Condition IsNotOnHold => GetOrCreate(b => b.FromBql<
				hold.IsEqual<False>.And<released.IsEqual<False>>
			>());

			public Condition IsCreditHoldChecked => GetOrCreate(b => b.FromBql<
				creditHold.IsEqual<False>
			>());

			public Condition IsPrinted => GetOrCreate(b => b.FromBql<
				ARSetup.printBeforeRelease.FromSetup.IsEqual<False>.
				Or<printInvoice.IsEqual<False>>.
				Or<printed.IsEqual<True>>
			>());

			public Condition IsEmailed => GetOrCreate(b => b.FromBql<
				ARSetup.emailBeforeRelease.FromSetup.IsEqual<False>.
				Or<dontEmail.IsEqual<True>>.
				Or<emailed.IsEqual<True>>
			>());

			public Condition IsCCProcessed => GetOrCreate(b => b.FromBql<
				pendingProcessing.IsEqual<False>
			>());

			public Condition IsOnCreditHold => GetOrCreate(b => b.FromBql<
				released.IsEqual<False>.And<creditHold.IsEqual<True>>
			>());

			public Condition IsReleased => GetOrCreate(b => b.FromBql<
				released.IsEqual<True>.And<openDoc.IsEqual<True>>
			>());

			public Condition IsClosed => GetOrCreate(b => b.FromBql<
				released.IsEqual<True>.And<openDoc.IsEqual<False>>
			>());

			public Condition IsPendingProcessingPure => GetOrCreate(b => b.FromBql<
				pendingProcessing.IsEqual<True>.And<origModule.IsEqual<GL.BatchModule.moduleSO>>
			>());

			public Condition IsCreditType => GetOrCreate(b => b.FromBql<
				docType.IsIn<ARDocType.creditMemo, ARDocType.cashReturn, ARDocType.cashSale>
			>());

			public Condition IsSOInvoice => GetOrCreate(b => b.FromBql<
				origModule.IsEqual<GL.BatchModule.moduleSO>
			>());
			public Condition IsPaymentsTransferPostponed => GetOrCreate(b => b.FromBql<
				isPaymentsTransferred.IsEqual<False>
			>());
		}

		[PXWorkflowDependsOnType(typeof(ARSetup))]
		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<SOInvoiceEntry, ARInvoice>());

		protected static void Configure(WorkflowContext<SOInvoiceEntry, ARInvoice> context)
		{
			var conditions = context.Conditions.GetPack<Conditions>();

			#region Categories
			var commonCategories = CommonActionCategories.Get(context);
			var processingCategory = commonCategories.Processing;
			var correctionsCategory = context.Categories.CreateNew(ActionCategories.CorrectionsCategoryID,
					category => category.DisplayName(ActionCategories.DisplayNames.Corrections));
			var approvalCategory = commonCategories.Approval;
			var printingEmailingCategory = commonCategories.PrintingAndEmailing;
			var otherCategory = commonCategories.Other;
			#endregion

			const string initialState = "_";
			context.AddScreenConfigurationFor(screen =>
			{
				return screen
					.StateIdentifierIs<status>()
					.AddDefaultFlow(flow =>
					{
						return flow
							.WithFlowStates(flowStates =>
							{
								flowStates.Add(initialState, flowState => flowState.IsInitial(g => g.initializeState));
								flowStates.Add<State.incomplete>(flowState =>
								{
									return flowState
										.WithActions(actions =>
										{
											actions.Add(g => g.completeProcessing, a => a.IsDuplicatedInToolbar());
										})
										.WithEventHandlers(handlers =>
										{
											handlers.Add(g => g.OnCompleteProcessing);
										});
								});
								flowStates.AddSequence<State.HoldToBalance>(seq =>
									seq.WithStates(sss =>
									{
										sss.Add<State.hold>(flowState =>
										{
											return flowState
												.IsSkippedWhen(conditions.IsNotOnHold)	
												.WithActions(actions =>
												{
													actions.Add(g => g.releaseFromHold, act => act.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
													actions.Add(g => g.validateAddresses);
													actions.Add(g => g.recalculateDiscountsAction);
												});
										});
										sss.Add<State.cCHold>(flowState =>
										{
											return flowState
												.IsSkippedWhen(conditions.IsCCProcessed)
												.WithActions(actions =>
												{
													actions.Add(g => g.putOnHold);
													actions.Add(g => g.putOnCreditHold);
													actions.Add(g => g.printInvoice);
													actions.Add(g => g.emailInvoice);
													actions.Add(g => g.validateAddresses);
													actions.Add(g => g.recalculateDiscountsAction);
													actions.Add<CreatePaymentExt>(e => e.createAndCapturePayment);
												});
										});
										sss.Add<State.creditHold>(flowState =>
										{
											return flowState
												.IsSkippedWhen(conditions.IsCreditHoldChecked)
												.WithActions(actions =>
												{
													actions.Add(g => g.releaseFromCreditHold, act => act.IsDuplicatedInToolbar());
													actions.Add(g => g.putOnHold, act => act.IsDuplicatedInToolbar());
													actions.Add(g => g.validateAddresses);
													actions.Add(g => g.emailInvoice);
													actions.Add<CreatePaymentExt>(e => e.createAndCapturePayment);
												});
										});
										sss.Add<State.pendingPrint>(flowState =>
										{
											return flowState
												.IsSkippedWhen(conditions.IsPrinted)
												.WithActions(actions =>
												{
													actions.Add(g => g.printInvoice, act => act.IsDuplicatedInToolbar());
													actions.Add(g => g.emailInvoice);
													actions.Add(g => g.putOnHold);
													actions.Add(g => g.putOnCreditHold);
													actions.Add(g => g.validateAddresses);
													actions.Add(g => g.recalculateDiscountsAction);
												});
											;
										});
										sss.Add<State.pendingEmail>(flowState =>
										{
											return flowState
												.IsSkippedWhen(conditions.IsEmailed)
												.WithActions(actions =>
												{
													actions.Add(g => g.emailInvoice, act => act.IsDuplicatedInToolbar());
													actions.Add(g => g.printInvoice);
													actions.Add(g => g.putOnHold);
													actions.Add(g => g.putOnCreditHold);
													actions.Add(g => g.validateAddresses);
													actions.Add(g => g.recalculateDiscountsAction);
												});
										});
										sss.Add<State.balanced>(flowState =>
										{
											return flowState
												.WithActions(actions =>
												{
													actions.Add(g => g.release, act => act.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
													actions.Add(g => g.putOnHold, act => act.IsDuplicatedInToolbar());
													actions.Add(g => g.putOnCreditHold);
													actions.Add(g => g.printInvoice);
													actions.Add(g => g.emailInvoice);
													actions.Add(g => g.arEdit);
													actions.Add(g => g.validateAddresses);
													actions.Add(g => g.recalculateDiscountsAction);
													actions.Add<CreatePaymentExt>(e => e.createAndCapturePayment);
												});
											;
										});
									})
									.WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnUpdateStatus);
									})
								);

								flowStates.Add<State.open>(flowState =>
								{
									return flowState
										.WithActions(actions =>
										{
											actions.Add(g => g.post);
											actions.Add(g => g.writeOff);
											actions.Add(g => g.payInvoice);
											actions.Add(g => g.printInvoice);
											actions.Add(g => g.emailInvoice);
											actions.Add(g => g.validateAddresses);
											actions.Add(g => g.reclassifyBatch);
											actions.Add<Correction>(g => g.cancelInvoice);
											actions.Add<Correction>(g => g.correctInvoice);
											actions.Add<Correction>(g => g.reverseDirectInvoice);
										});
								});
								flowStates.Add<State.closed>(flowState =>
								{
									return flowState
										.WithActions(actions =>
										{
											actions.Add(g => g.post);
											actions.Add(g => g.printInvoice);
											actions.Add(g => g.emailInvoice);
											actions.Add(g => g.validateAddresses);
											actions.Add(g => g.reclassifyBatch);
											actions.Add<Correction>(g => g.cancelInvoice);
											actions.Add<Correction>(g => g.correctInvoice);
											actions.Add<Correction>(g => g.reverseDirectInvoice);
										})
										.WithFieldStates(states =>
										{
											states.AddTable<ARInvoice>(state => state.IsDisabled());
										});
								});
								flowStates.Add<State.canceled>(flowState =>
								{
									return flowState
										.WithActions(actions =>
										{
											actions.Add(g => g.printInvoice);
											actions.Add(g => g.emailInvoice);
										})
										.WithFieldStates(states =>
										{
											states.AddTable<ARInvoice>(state => state.IsDisabled());
										});
								});
							})
							.WithTransitions(transitions =>
							{
								transitions.AddGroupFrom(initialState, ts =>
								{
									ts.Add(t => t.To<State.incomplete>()
										.IsTriggeredOn(g => g.initializeState)
										.When(conditions.IsPaymentsTransferPostponed));
									ts.Add(t => t.To<State.HoldToBalance>()
										.IsTriggeredOn(g => g.initializeState)); // To default sequence
								});
								transitions.AddGroupFrom<State.incomplete>(ts =>
								{
									ts.Add(t => t
										.To<State.HoldToBalance>()
										.IsTriggeredOn(g => g.OnCompleteProcessing)
										.WithFieldAssignments(fass => fass.Add<isPaymentsTransferred>(true))
										.DoesNotPersist());
								});
								transitions.AddGroupFrom<State.HoldToBalance>(ts =>
								{
									ts.Add(t => t
										.To<State.HoldToBalance>()
										.IsTriggeredOn(g => g.OnUpdateStatus)
										.When(conditions.IsSOInvoice));
								});

								transitions.AddGroupFrom<State.balanced>(ts =>
								{
									ts.Add(t => t.To<State.open>().IsTriggeredOn(g => g.release).When(conditions.IsReleased));
									ts.Add(t => t.To<State.closed>().IsTriggeredOn(g => g.release).When(conditions.IsClosed));
								});

								transitions.AddGroupFrom<State.open>(ts =>
								{
									//ts.Add(t => t.To<State.closed>().IsTriggeredOn(g => g.OnApplicationReleased).When(conditions.IsClosed));
								});
								transitions.AddGroupFrom<State.closed>(ts =>
								{
									// terminal status
								});
							});
					})
					.WithActions(actions =>
					{
						actions.Add(g => g.initializeState, a => a.IsHiddenAlways());
						#region Processing
						actions.Add(g => g.completeProcessing, c => c
							.WithCategory(processingCategory)
							.MassProcessingScreen<SOReleaseInvoice>());
						actions.Add(g => g.releaseFromHold, c => c
							.WithCategory(processingCategory)
							.WithFieldAssignments(fass => fass.Add<hold>(false)));
						actions.Add(g => g.putOnHold, c => c
							.WithCategory(processingCategory, g => g.releaseFromHold)
							.PlaceAfter(g => g.release)
							.WithFieldAssignments(fass => fass.Add<hold>(true)));
						actions.Add(g => g.release, c => c
							.WithCategory(processingCategory)
							.MassProcessingScreen<SOReleaseInvoice>()
							.InBatchMode());
						actions.Add(g => g.payInvoice, c => c
							.WithCategory(processingCategory));
						#endregion
						#region Corrections
						actions.Add<Correction>(g => g.correctInvoice, c => c
							.WithCategory(correctionsCategory));
						actions.Add<Correction>(g => g.cancelInvoice, c => c
							.WithCategory(correctionsCategory));
						actions.Add<Correction>(g => g.reverseDirectInvoice, c => c
							.WithCategory(correctionsCategory));
						actions.Add(g => g.writeOff, c => c
							.WithCategory(correctionsCategory));
						actions.Add(g => g.reclassifyBatch, c => c
							.WithCategory(correctionsCategory));
						#endregion
						#region Approval
						actions.Add(g => g.releaseFromCreditHold, c => c
							.WithCategory(approvalCategory)
							.IsHiddenWhen(conditions.IsCreditType)
							.MassProcessingScreen<SOReleaseInvoice>()
							.WithFieldAssignments(fass =>
							{
								fass.Add<creditHold>(false);
							}));
						actions.Add(g => g.putOnCreditHold, c => c
							.WithCategory(approvalCategory)
							.IsHiddenWhen(conditions.IsCreditType)
							.WithFieldAssignments(fass =>
							{
								fass.Add<hold>(false);
								fass.Add<creditHold>(true);
								fass.Add<approvedCredit>(false);
								fass.Add<approvedCreditAmt>(0);
								fass.Add<approvedCaptureFailed>(false);
								fass.Add<approvedPrepaymentRequired>(false);
							}));
						#endregion
						#region Printing and Emailing
						actions.Add(g => g.printInvoice, c => c
							.WithCategory(printingEmailingCategory)
							.WithPersistOptions(ActionPersistOptions.PersistBeforeAction)
							.MassProcessingScreen<SOReleaseInvoice>()
							.InBatchMode()
							.WithFieldAssignments(fass => fass.Add<printed>(true)));
						actions.Add(g => g.emailInvoice, c => c
							.WithCategory(printingEmailingCategory)
							.MassProcessingScreen<SOReleaseInvoice>()
							.WithPersistOptions(ActionPersistOptions.PersistBeforeAction)
							.WithFieldAssignments(fass => fass.Add<emailed>(true)));
						#endregion
						#region Other
						actions.Add(g => g.recalculateDiscountsAction, c => c
							.WithCategory(otherCategory));
						actions.Add(g => g.post, c => c
							.WithCategory(otherCategory)
							.MassProcessingScreen<SOReleaseInvoice>()
							.InBatchMode());
						actions.Add(g => g.validateAddresses, c => c
							.WithCategory(otherCategory));
						#endregion
						#region Reports
						actions.Add(g => g.arEdit, c => c
							.WithCategory(PredefinedCategory.Reports));
						actions.Add(g => g.printAREdit, c => c
							.WithCategory(PredefinedCategory.Reports)
							.IsHiddenAlways());
						actions.Add(g => g.printARRegister, c => c
							.WithCategory(PredefinedCategory.Reports)
							.IsHiddenAlways());
						#endregion

						actions.Add<CreatePaymentExt>(e => e.createAndCapturePayment, c => c
							.WithCategory(PredefinedCategory.Actions)
							.IsHiddenAlways() // only for mass processing
							.MassProcessingScreen<SOReleaseInvoice>());
						actions.Add(g => g.reverseInvoiceAndApplyToMemo, c=> c
							.WithCategory(PredefinedCategory.Actions)
							.IsHiddenAlways());
						actions.Add(g => g.reverseInvoice, c => c
							.WithCategory(PredefinedCategory.Actions)
							.IsHiddenAlways());
						actions.Add(g => g.sendEmail, c => c
							.WithCategory(PredefinedCategory.Actions)
							.IsHiddenAlways());
					})
					.WithCategories(categories =>
					{
						categories.Add(processingCategory);
						categories.Add(correctionsCategory);
						categories.Add(approvalCategory);
						categories.Add(printingEmailingCategory);
						categories.Add(otherCategory);
						categories.Update(FolderType.ReportsFolder, category => category.PlaceAfter(otherCategory));
					})
					.WithHandlers(handlers =>
					{
						handlers.Add(handler => handler
							.WithTargetOf<ARInvoice>()
							.OfEntityEvent<ARInvoice.Events>(e => e.ProcessingCompleted)
							.Is(g => g.OnCompleteProcessing)
							.UsesTargetAsPrimaryEntity());
						handlers.Add(handler => handler
							.WithTargetOf<ARInvoice>()
							.OfFieldsUpdated<ARInvoiceEntry_Workflow.OnUpdateStatusFields>()
							.Is(g => g.OnUpdateStatus)
							.UsesTargetAsPrimaryEntity()
							.DisplayName("Invoice Updated"));
					});
			});
		}

		public static class ActionCategories
		{
			public const string CorrectionsCategoryID = "Corrections Category";

			[PXLocalizable]
			public static class DisplayNames
			{
				public const string Corrections = "Corrections";
			}
		}
	}
}

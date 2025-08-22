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
using PX.Objects.AR.GraphExtensions;
using PX.Objects.CS;

namespace PX.Objects.AR
{
	using State = ARDocStatus;
	using static ARPayment;
	using static BoundedTo<ARPaymentEntry, ARPayment>;
	using static PX.Objects.AR.ARPaymentEntry_Workflow.Constants;

	public class ARPaymentEntry_Workflow : PXGraphExtension<ARPaymentEntry>
	{
		public static class Constants
		{
			public const string NoDocNbr = " <SELECT>";

			public class noDocNbr : PX.Data.BQL.BqlString.Constant<noDocNbr>
			{
				public noDocNbr() : base(NoDocNbr) {; }
			}
		}

		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<ARPaymentEntry, ARPayment>());

		public class Conditions : Condition.Pack
			{
			private readonly ARSetupDefinition _Definition = ARSetupDefinition.GetSlot();

			public Condition IsNotOnHold => GetOrCreate(c => c.FromBql<
				hold.IsEqual<False>.And<released.IsEqual<False>>
			>());
			public Condition IsOpen => GetOrCreate(c => c.FromBql<
				openDoc.IsEqual<True>.And<released.IsEqual<True>>
			>());
				
			public Condition IsClosed => GetOrCreate(c => c.FromBql<
				openDoc.IsEqual<False>.And<released.IsEqual<True>>
			>());
			public Condition IsNotVoidable => GetOrCreate(c => c.FromBql<
				docType.IsIn<ARDocType.voidPayment, ARDocType.voidRefund, ARDocType.creditMemo, ARDocType.prepaymentInvoice>
				.Or<docType.IsEqual<ARDocType.smallBalanceWO>.And<status.IsEqual<ARDocStatus.reserved>>>
			>());
			public Condition IsNotRefundable => GetOrCreate(c => c.FromBql<
				docType.IsNotIn<ARDocType.payment, ARDocType.prepayment, ARDocType.creditMemo>
						.Or<curyUnappliedBal.IsLessEqual<decimal0>>
				.Or<released.IsEqual<False>>
         >());
			public Condition IsCCIntegrated => GetOrCreate(c => 
				PXAccess.FeatureInstalled<FeaturesSet.integratedCardProcessing>() &&
					_Definition.IntegratedCCProcessing == true && _Definition.MigrationMode != true
					? c.FromBql<status.IsEqual<ARDocStatus.cCHold>>()
					: c.FromBql<True.IsEqual<False>>()
			);
			public Condition IsCCProcessed => GetOrCreate(c => c.FromBql<
				ARRegister.pendingProcessing.IsEqual<False>
			>());
			public Condition IsVoided => GetOrCreate(c => c.FromBql<
				voided.IsEqual<True>
			>());

			public Condition IsNotCapturable => GetOrCreate(c => c.FromBql<
				docType.IsIn<ARDocType.voidPayment, ARDocType.refund, ARDocType.voidRefund,
					ARDocType.cashReturn>
			>());
			public Condition IsCreditMemo => GetOrCreate(c => c.FromBql<
				docType.IsEqual<ARDocType.creditMemo>
			>());
			public Condition IsBalanceWO => GetOrCreate(c => c.FromBql<
				docType.IsEqual<ARDocType.smallBalanceWO>
			>());

			public Condition IsPrepaymentInvoice => GetOrCreate(c => c.FromBql<
				docType.IsEqual<ARDocType.prepaymentInvoice>
			>());

			public Condition IsNoDoc => GetOrCreate(c => c.FromBql<
				refNbr.IsEqual<noDocNbr>
			>());
		}
				
		[PXWorkflowDependsOnType(typeof(ARSetup))]
		protected static void Configure(WorkflowContext<ARPaymentEntry, ARPayment> context)
		{
			var _Definition = ARSetupDefinition.GetSlot();
			var conditions = context.Conditions.GetPack<Conditions>();
				
			#region Categories

			var processingCategory = context.Categories.CreateNew(CategoryID.Processing,
				category => category.DisplayName(CategoryNames.Processing));
			var cardProcessingCategory = context.Categories.CreateNew(CategoryID.CardProcessing,
				category => category.DisplayName(CategoryNames.CardProcessing));
			var correctionsCategory = context.Categories.CreateNew(CategoryID.Corrections,
				category => category.DisplayName(CategoryNames.Corrections));
			var approvalCategory = context.Categories.CreateNew(CategoryID.Approval,
				category => category.DisplayName(CategoryNames.Approval));
			var otherCategory = context.Categories.CreateNew(CategoryID.Other,
				category => category.DisplayName(CategoryNames.Other));

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
									fss.AddSequence<State.HoldToBalance>(seq =>
										seq.WithStates(sss =>
											{
												sss.Add<State.hold>(flowState =>
							{
								return flowState
														.IsSkippedWhen(conditions.IsNotOnHold)
									.WithActions(actions =>
									{
															actions.Add(g => g.releaseFromHold,
																a => a.IsDuplicatedInToolbar()
																	.WithConnotation(ActionConnotation
																		.Success));
										actions.Add(g => g.printAREdit);
										actions.Add(g => g.customerDocuments);
															actions.Add<ARPaymentEntryPaymentTransaction>(a => a.voidCCPayment);
														}).WithEventHandlers(handlers => { handlers.Add(g => g.OnVoidDocument); });
							});
												sss.Add<State.cCHold>(flowState =>
							{
								return flowState
														.IsSkippedWhen(conditions.IsCCProcessed)
									.WithActions(actions =>
									{
										actions.Add(a=>a.putOnHold);
															actions.Add<ARPaymentEntryPaymentTransaction>(a => a.captureCCPayment,
																a => a.WithConnotation(ActionConnotation.Success));
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.authorizeCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.voidCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.creditCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.recordCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.captureOnlyCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.validateCCPayment);
										actions.Add(g => g.release);
										actions.Add(a=>a.voidCheck);
									}).WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnUpdateStatus);
										handlers.Add(g => g.OnReleaseDocument);
										handlers.Add(g => g.OnVoidDocument);
									});
							});
												sss.Add<State.balanced>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
															actions.Add(g => g.release,
																a => a.IsDuplicatedInToolbar()
																	.WithConnotation(ActionConnotation.Success));
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.captureCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.authorizeCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.voidCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.creditCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.recordCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.captureOnlyCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.validateCCPayment);
										actions.Add(g => g.putOnHold);
										actions.Add(g => g.printAREdit);
										actions.Add(g => g.customerDocuments);
														});
												});
											})
											.WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnUpdateStatus);
										handlers.Add(g => g.OnReleaseDocument);
										handlers.Add(g => g.OnVoidDocument);
											}));
							fss.Add<State.pendingPayment>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.printARRegister);
										actions.Add(g => g.customerDocuments);
									});
							});
							fss.Add<State.open>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
												actions.Add(g => g.release,
													a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
										actions.Add(g => g.voidCheck, a => a.IsDuplicatedInToolbar());
										actions.Add(g => g.putOnHold);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.captureCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.authorizeCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.voidCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.creditCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.recordCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.captureOnlyCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.validateCCPayment);
										actions.Add(g => g.reverseApplication);
												actions.Add(g => g.printARRegister);
										actions.Add(g => g.printAREdit);
										actions.Add(g => g.customerDocuments);
										actions.Add(g => g.initializeState, act => act.IsAutoAction());
									}).WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnCloseDocument);
										handlers.Add(g => g.OnVoidDocument);
									});
							});
							fss.Add<State.unapplied>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.release,
											a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
										actions.Add(g => g.voidCheck, a => a.IsDuplicatedInToolbar());
										actions.Add(g => g.putOnHold);
										actions.Add<ARPaymentEntryPaymentTransaction>(a => a.captureCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a => a.authorizeCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a => a.voidCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a => a.creditCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a => a.recordCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a => a.captureOnlyCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a => a.validateCCPayment);
										actions.Add(g => g.reverseApplication);
										actions.Add(g => g.printARRegister);
										actions.Add(g => g.printAREdit);
										actions.Add(g => g.customerDocuments);
										actions.Add(g => g.initializeState, act => act.IsAutoAction());
									}).WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnCloseDocument);
										handlers.Add(g => g.OnVoidDocument);
									});
							});
							fss.Add<State.reserved>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
												actions.Add(g => g.releaseFromHold,
													a => a.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
										actions.Add(g => g.printARRegister);
										actions.Add(g => g.customerDocuments);
										actions.Add(g => g.voidCheck);
											}).WithEventHandlers(handlers => { handlers.Add(g => g.OnVoidDocument); });
							});
							fss.Add<State.closed>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.voidCheck, a => a.IsDuplicatedInToolbar());
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.captureCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.authorizeCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.voidCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.creditCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.recordCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.captureOnlyCCPayment);
										actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.validateCCPayment);
										actions.Add(g => g.reverseApplication);
										actions.Add(g => g.printARRegister);
										actions.Add(g => g.customerDocuments);
									}).WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnOpenDocument);
										handlers.Add(g => g.OnVoidDocument);
									});
							});
							fss.Add<State.voided>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.printARRegister);
										actions.Add(g => g.customerDocuments);
									});
							});
						}
						)
					.WithTransitions(transitions =>
					{
						transitions.AddGroupFrom(initialState, ts =>
						{
										ts.Add(t => t.To<State.HoldToBalance>()
											.IsTriggeredOn(g => g.initializeState)); // To default sequence
						});
									transitions.AddGroupFrom<State.HoldToBalance>(ts =>
						{
							ts.Add(t => t
											.To<State.HoldToBalance>()
											.IsTriggeredOn(g => g.OnUpdateStatus));
							ts.Add(t => t
								.To<State.open>()
								.IsTriggeredOn(g => g.OnReleaseDocument)
								.When(conditions.IsOpen && !conditions.IsPrepaymentInvoice));
							ts.Add(t => t
								.To<State.unapplied>()
								.IsTriggeredOn(g => g.OnReleaseDocument)
								.When(conditions.IsOpen && conditions.IsPrepaymentInvoice));
							ts.Add(t => t
								.To<State.closed>()
								.IsTriggeredOn(g => g.OnReleaseDocument)
								.When(conditions.IsClosed));
									});
									transitions.AddGroupFrom<State.hold>(ts =>
									{
										ts.Add(t => t
											.To<State.voided>()
											.IsTriggeredOn(g => g.OnVoidDocument));
									});
									transitions.AddGroupFrom<State.cCHold>(ts =>
									{
							ts.Add(t => t
								.To<State.voided>()
								.IsTriggeredOn(g=>g.OnVoidDocument));
						});
						transitions.AddGroupFrom<State.balanced>(ts =>
						{
							ts.Add(t => t
								.To<State.voided>()
								.IsTriggeredOn(g=>g.OnVoidDocument));
						});
						transitions.AddGroupFrom<State.open>(ts =>
						{
							ts.Add(t => t
								.To<State.reserved>()
								.IsTriggeredOn(g => g.putOnHold)
								.WithFieldAssignments(fas =>
								{
									fas.Add<ARRegister.rejected>(f => f.SetFromValue(false));
								}));
							ts.Add(t => t.To<State.voided>()
								.IsTriggeredOn(g => g.OnVoidDocument));
							ts.Add(t => t
								.To<State.closed>()
								.IsTriggeredOn(g => g.OnCloseDocument));
							ts.Add(t => t
								.To<State.closed>()
								.IsTriggeredOn(g => g.initializeState)
								.When(conditions.IsClosed));
							ts.Add(t => t
								.To<State.closed>()
								.IsTriggeredOn(g => g.OnReleaseDocument)
								.When(conditions.IsClosed));
							ts.Add(t => t
								.To<State.voided>()
								.IsTriggeredOn(g => g.OnReleaseDocument)
								.When(conditions.IsVoided));
						});
						transitions.AddGroupFrom<State.unapplied>(ts =>
						{
							ts.Add(t => t
								.To<State.reserved>()
								.IsTriggeredOn(g => g.putOnHold)
								.WithFieldAssignments(fas =>
								{
									fas.Add<ARRegister.rejected>(f => f.SetFromValue(false));
								}));
							ts.Add(t => t.To<State.voided>()
								.IsTriggeredOn(g => g.OnVoidDocument));
							ts.Add(t => t
								.To<State.closed>()
								.IsTriggeredOn(g => g.OnCloseDocument));
							ts.Add(t => t
								.To<State.closed>()
								.IsTriggeredOn(g => g.initializeState)
								.When(conditions.IsClosed));
							ts.Add(t => t
								.To<State.closed>()
								.IsTriggeredOn(g => g.OnReleaseDocument)
								.When(conditions.IsClosed));
							ts.Add(t => t
								.To<State.voided>()
								.IsTriggeredOn(g => g.OnReleaseDocument)
								.When(conditions.IsVoided));
						});
						transitions.AddGroupFrom<State.reserved>(ts =>
						{
							ts.Add(t => t
								.To<State.open>()
								.IsTriggeredOn(g => g.releaseFromHold)
								.When(!conditions.IsPrepaymentInvoice));
							ts.Add(t => t
								.To<State.unapplied>()
								.IsTriggeredOn(g => g.releaseFromHold)
								.When(conditions.IsPrepaymentInvoice));
							ts.Add(t => t
								.To<State.voided>()
								.IsTriggeredOn(g => g.OnVoidDocument));
							ts.Add(t => t
								.To<State.closed>()
								.IsTriggeredOn(g => g.OnReleaseDocument)
								.When(conditions.IsClosed));
						});
						transitions.AddGroupFrom<State.closed>(ts =>
						{
							ts.Add(t => t
								.To<State.voided>()
								.IsTriggeredOn(g => g.OnVoidDocument));
							ts.Add(t => t
								.To<State.open>()
								.IsTriggeredOn(g => g.OnOpenDocument)
								.When(!conditions.IsPrepaymentInvoice));
							ts.Add(t => t
								.To<State.unapplied>()
								.IsTriggeredOn(g => g.OnOpenDocument)
								.When(conditions.IsPrepaymentInvoice));
							ts.Add(t => t
								.To<State.open>()
								.IsTriggeredOn(g => g.reverseApplication)
								.When(!conditions.IsPrepaymentInvoice)
								.DoesNotPersist());
							ts.Add(t => t
								.To<State.unapplied>()
								.IsTriggeredOn(g => g.reverseApplication)
								.When(conditions.IsPrepaymentInvoice)
								.DoesNotPersist());
							ts.Add(t => t
								.To<State.voided>()
								.IsTriggeredOn(g => g.OnReleaseDocument)
								.When(conditions.IsVoided));
						});
					}
					))
					.WithActions(actions =>
					{
						actions.Add(g => g.initializeState, a => a.IsHiddenAlways());
						actions.Add(g => g.releaseFromHold, c => c
							.WithCategory(processingCategory)
							.WithPersistOptions(ActionPersistOptions.NoPersist)
							.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(false))));
						actions.Add(g => g.putOnHold, c => c
							.WithCategory(processingCategory)
							.WithPersistOptions(ActionPersistOptions.NoPersist)
							.WithFieldAssignments(fas => fas.Add<hold>(f => f.SetFromValue(true)))
							.IsDisabledWhen(conditions.IsNoDoc));
						actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.authorizeCCPayment, a=>a
							.WithCategory(cardProcessingCategory)
							.IsHiddenWhen(conditions.IsCreditMemo || conditions.IsBalanceWO || conditions.IsPrepaymentInvoice));
						actions.Add<ARPaymentEntryPaymentTransaction>(a => a.captureCCPayment, a => a
							.WithCategory(cardProcessingCategory)
							.IsHiddenWhen(conditions.IsNotCapturable || conditions.IsCreditMemo || conditions.IsBalanceWO || conditions.IsPrepaymentInvoice));
						actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.voidCCPayment, a=>a
							.WithCategory(cardProcessingCategory)
							.IsHiddenWhen(conditions.IsCreditMemo || conditions.IsBalanceWO || conditions.IsPrepaymentInvoice));
						actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.creditCCPayment, a=>a
							.WithCategory(cardProcessingCategory)
							.IsHiddenWhen(conditions.IsCreditMemo || conditions.IsBalanceWO || conditions.IsPrepaymentInvoice));
						actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.recordCCPayment, a=>a
							.WithCategory(cardProcessingCategory)
							.IsHiddenWhen(conditions.IsCreditMemo || conditions.IsBalanceWO || conditions.IsPrepaymentInvoice));
						actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.captureOnlyCCPayment, a=>a
							.WithCategory(cardProcessingCategory)
							.IsHiddenWhen(conditions.IsCreditMemo || conditions.IsBalanceWO || conditions.IsPrepaymentInvoice));
						actions.Add<ARPaymentEntryPaymentTransaction>(a=>a.validateCCPayment, a=>a
							.WithCategory(cardProcessingCategory)
							.IsHiddenWhen(conditions.IsCreditMemo || conditions.IsBalanceWO || conditions.IsPrepaymentInvoice));
						actions.Add(g => g.release, c => c
							.WithCategory(processingCategory)
							.IsDisabledWhen(conditions.IsCCIntegrated)
						);
						actions.Add(g => g.refund, c => c
							.WithCategory(processingCategory)
							.IsDisabledWhen(conditions.IsNotRefundable)
							.IsHiddenWhen(conditions.IsPrepaymentInvoice));
						actions.Add(g => g.voidCheck, c => c
							.WithCategory(correctionsCategory)
							.IsHiddenWhen(conditions.IsNotVoidable));
						actions.Add(g => g.reverseApplication, g => g
							.WithPersistOptions(ActionPersistOptions.NoPersist)
						);
						actions.Add(g => g.customerDocuments, c => c
							.WithCategory(PredefinedCategory.Inquiries)
							.IsDisabledWhen(conditions.IsNoDoc));
						actions.Add(g => g.printAREdit, c => c
							.WithCategory(PredefinedCategory.Reports)
							.IsDisabledWhen(conditions.IsNoDoc)
							.IsHiddenWhen(conditions.IsPrepaymentInvoice));
						actions.Add(g => g.printARRegister, c => c.WithCategory(PredefinedCategory.Reports));
					})
					.WithHandlers(handlers =>
					{
						handlers.Add(handler => handler
							.WithTargetOf<ARPayment>()
							.OfEntityEvent<ARPayment.Events>(e => e.ReleaseDocument)
							.Is(g => g.OnReleaseDocument)
							.UsesTargetAsPrimaryEntity()
							.WithUpcastTo<ARRegister>());
						handlers.Add(handler => handler
							.WithTargetOf<ARPayment>()
							.OfEntityEvent<ARPayment.Events>(e => e.OpenDocument)
							.Is(g => g.OnOpenDocument)
							.UsesTargetAsPrimaryEntity()
							.WithUpcastTo<ARRegister>());
						handlers.Add(handler => handler
							.WithTargetOf<ARPayment>()
							.OfEntityEvent<ARPayment.Events>(e => e.CloseDocument)
							.Is(g => g.OnCloseDocument)
							.UsesTargetAsPrimaryEntity()
							.WithUpcastTo<ARRegister>());
						handlers.Add(handler => handler
							.WithTargetOf<ARPayment>()
							.OfEntityEvent<ARPayment.Events>(e => e.VoidDocument)
							.Is(g => g.OnVoidDocument)
							.UsesTargetAsPrimaryEntity()
							.WithUpcastTo<ARRegister>());
						handlers.Add(handler => handler
							.WithTargetOf<ARPayment>()
							.OfFieldsUpdated<BqlFields.FilledWith<ARPayment.hold, ARPayment.pendingProcessing>>()
							.Is(g => g.OnUpdateStatus)
							.UsesTargetAsPrimaryEntity());
					})
					.WithCategories(categories =>
					{
						categories.Add(processingCategory);
						categories.Add(cardProcessingCategory);
						categories.Add(correctionsCategory);
						categories.Add(approvalCategory);
						categories.Add(otherCategory);
						categories.Update(FolderType.InquiriesFolder, category => category.PlaceAfter(otherCategory));
						categories.Update(FolderType.ReportsFolder,
							category => category.PlaceAfter(FolderType.InquiriesFolder));
					})
				);
		}

		public static class CategoryNames
		{
			public const string Processing = "Processing";
			public const string CardProcessing = "Card Processing";
			public const string Corrections = "Corrections";
			public const string Approval = "Approval";			
			public const string Other = "Other";
		}

		public static class CategoryID
		{
			public const string Processing = "ProcessingID";
			public const string CardProcessing = "CardProcessingID";
			public const string Corrections = "CorrectionsID";
			public const string Approval = "ApprovalID";
			public const string Other = "OtherID";
		}
	}
}

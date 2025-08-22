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
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Objects.PM;

namespace PX.Objects.PR
{
	using static PRPTOAdjustment;
	using static BoundedTo<PRPTOAdjustmentMaint, PRPTOAdjustment>;

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public partial class PRPTOAdjustmentMaint_Workflow : PXGraphExtension<PRPTOAdjustmentMaint>
	{
		public override void Configure(PXScreenConfiguration config)
		{
			Configure(config.GetScreenConfigurationContext<PRPTOAdjustmentMaint, PRPTOAdjustment>());
		}

		public class Conditions : Condition.Pack
		{
			public Condition IsVoidingAdjustment => GetOrCreate(c => c.FromBql<type.IsEqual<PTOAdjustmentType.voidingAdjustment>>());
		}

		protected virtual void Configure(WorkflowContext<PRPTOAdjustmentMaint, PRPTOAdjustment> context)
		{
			var conditions = context.Conditions.GetPack<Conditions>();

			var processingCategory = context.Categories.CreateNew(ToolbarCategory.ActionCategoryNames.Processing,
				category => category.DisplayName(ToolbarCategory.ActionCategory.Processing));
			var correctionsCategory = context.Categories.CreateNew(ToolbarCategory.ActionCategoryNames.Corrections,
				category => category.DisplayName(ToolbarCategory.ActionCategory.Corrections));

			void DisableHeaderFields(FieldState.IContainerFillerFields fields)
			{
				fields.AddField<PRPTOAdjustment.description>(field => field.IsDisabled());
			}

			void DisableGridFields(FieldState.IContainerFillerFields fields)
			{
				fields.AddField<PRPTOAdjustmentDetail.bAccountID>(field => field.IsDisabled());
				fields.AddField<PRPTOAdjustmentDetail.bankID>(field => field.IsDisabled());
				fields.AddField<PRPTOAdjustmentDetail.adjustmentHours>(field => field.IsDisabled());
			}

			context.AddScreenConfigurationFor(screen =>
				screen
					.StateIdentifierIs<PRPTOAdjustment.status>()
					.AddDefaultFlow(flow => flow
						.WithFlowStates(fss =>
						{
							fss.Add<PTOAdjustmentStatus.newStatus>(flowState =>
							{
								return flowState
									.IsInitial()
									.WithActions(actions =>
									{
										actions.Add(g => g.Release, c => c.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
									});
							});
							fss.Add<PTOAdjustmentStatus.released>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.VoidPTOAdjustment);
									})
									.WithEventHandlers(handlers =>
									{
										handlers.Add(g => g.OnVoidingAdjustmentReleased);
									})
									.WithFieldStates(fields =>
									{
										DisableHeaderFields(fields);
										DisableGridFields(fields);
									});
							});
							fss.Add<PTOAdjustmentStatus.voided>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										
									})
									.WithFieldStates(fields =>
									{
										DisableHeaderFields(fields);
										DisableGridFields(fields);
									});
							});
						})
						.WithTransitions(transitions =>
						{
							transitions.AddGroupFrom<PTOAdjustmentStatus.newStatus>(ts =>
							{
								ts.Add(t => t
									.To<PTOAdjustmentStatus.released>()
									.IsTriggeredOn(g => g.Release));
							});
							transitions.AddGroupFrom<PTOAdjustmentStatus.released>(ts =>
							{
								ts.Add(t => t
									.To<PTOAdjustmentStatus.voided>()
									.IsTriggeredOn(g => g.OnVoidingAdjustmentReleased));
							});
							transitions.AddGroupFrom<PTOAdjustmentStatus.voided>(ts =>
							{
							});
						}))
						.WithActions(actions =>
						{
							actions.Add(g => g.Release, c => c.WithCategory(processingCategory));
							actions.Add(g => g.VoidPTOAdjustment, c => c.WithCategory(correctionsCategory).IsHiddenWhen(conditions.IsVoidingAdjustment));
						})
					.WithHandlers(handlers =>
					{
						handlers.Add(handler => handler
							.WithTargetOf<PRPTOAdjustment>()
							.OfEntityEvent<PRPTOAdjustment.Events>(e => e.VoidingAdjustmentReleased)
							.Is(g => g.OnVoidingAdjustmentReleased)
							.UsesPrimaryEntityGetter<SelectFrom<PRPTOAdjustment>
								.Where<type.IsEqual<PTOAdjustmentType.adjustment>
									.And<refNbr.IsEqual<refNbr.FromCurrent>>>>());
					})
					.WithCategories(categories =>
					{
						categories.Add(processingCategory);
						categories.Add(correctionsCategory);
					}));
		}
	}
}

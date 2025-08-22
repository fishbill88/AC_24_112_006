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
using PX.Data.BQL;
using PX.Data.WorkflowAPI;

namespace PX.Objects.PM
{
	using static BoundedTo<PMQuoteMaint, PMQuote>;
	using static PX.Objects.PM.PMQuoteMaint;

	public partial class PMQuoteMaint_Workflow : PXGraphExtension<PMQuoteMaint>
	{
		public static bool IsActive() => true;

		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<PMQuoteMaint, PMQuote>());

		protected static void Configure(WorkflowContext<PMQuoteMaint, PMQuote> context)
		{
			#region Conditions
			Condition Bql<T>() where T : IBqlUnary, new() => context.Conditions.FromBql<T>();
			var conditions = new
			{
				IsPrimaryQuoteEnabled
					= Bql<PMQuote.opportunityID.IsNotNull.And<PMQuote.opportunityID.IsNotEqual<StringEmpty>>.And<PMQuote.isPrimary.IsEqual<False>>>(),
				IsCancelDisabled
					= Bql<PMChangeRequest.costChangeOrderNbr.IsNotNull>(),
				IsCloseDisabled
					= Bql<PMChangeRequest.costChangeOrderReleased.IsNotEqual<True>>(),
				IsConvertToProjectEnabled
					= Bql<Brackets<PMQuote.opportunityID.IsNull.Or<PMQuote.isPrimary.IsEqual<True>>>.And<PMQuote.quoteProjectID.IsNull>>()
			}.AutoNameConditions();
			#endregion

			#region Categories
			var processingCategory = context.Categories.CreateNew(ToolbarCategory.ActionCategoryNames.Processing,
				category => category.DisplayName(ToolbarCategory.ActionCategory.Processing));
			var printingAndEmailingCategory = context.Categories.CreateNew(ToolbarCategory.ActionCategoryNames.PrintingAndEmailing,
				category => category.DisplayName(ToolbarCategory.ActionCategory.PrintingAndEmailing));
			var otherCategory = context.Categories.CreateNew(ToolbarCategory.ActionCategoryNames.Other,
				category => category.DisplayName(ToolbarCategory.ActionCategory.Other));
			#endregion

			context.AddScreenConfigurationFor(screen =>
				screen
					.StateIdentifierIs<PMQuote.status>()
					.AddDefaultFlow(flow => flow
						.WithFlowStates(fss =>
						{
							fss.Add<PMQuoteStatusAttribute.draft>(flowState =>
							{
								return flowState
									.IsInitial()
									.WithActions(actions =>
									{
										actions.Add(g => g.submit, c => c.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
										actions.Add(g => g.copyQuote);
										actions.Add(g => g.primaryQuote);
										actions.Add(g => g.GetExtension<PMDiscount>().graphRecalculateDiscountsAction);
									});
							});
							fss.Add<PMQuoteStatusAttribute.approved>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.convertToProject, c => c.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
										actions.Add(g => g.editQuote);
										actions.Add(g => g.sendQuote, c => c.IsDuplicatedInToolbar());
										actions.Add(g => g.printQuote, c => c.IsDuplicatedInToolbar());
										actions.Add(g => g.copyQuote);
										actions.Add(g => g.primaryQuote);
									});
							});
							fss.Add<PMQuoteStatusAttribute.sent>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.convertToProject, c => c.IsDuplicatedInToolbar().WithConnotation(ActionConnotation.Success));
										actions.Add(g => g.editQuote);
										actions.Add(g => g.sendQuote);
										actions.Add(g => g.printQuote, c => c.IsDuplicatedInToolbar());
										actions.Add(g => g.copyQuote);
										actions.Add(g => g.primaryQuote);
									});
							});
							fss.Add<PMQuoteStatusAttribute.closed>(flowState =>
							{
								return flowState
									.WithActions(actions =>
									{
										actions.Add(g => g.sendQuote);
										actions.Add(g => g.printQuote, c => c.IsDuplicatedInToolbar());
										actions.Add(g => g.copyQuote);
										actions.Add(g => g.primaryQuote);
									});
							});
						})
						.WithTransitions(transitions =>
						{
							transitions.AddGroupFrom<PMQuoteStatusAttribute.draft>(ts =>
							{
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.approved>()
									.IsTriggeredOn(g => g.submit));
							});
							transitions.AddGroupFrom<PMQuoteStatusAttribute.approved>(ts =>
							{
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.draft>()
									.IsTriggeredOn(g => g.editQuote));
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.sent>()
									.IsTriggeredOn(g => g.sendQuote));
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.closed>()
									.IsTriggeredOn(g => g.convertToProject));
							});
							transitions.AddGroupFrom<PMQuoteStatusAttribute.sent>(ts =>
							{
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.draft>()
									.IsTriggeredOn(g => g.editQuote));
								ts.Add(t => t
									.To<PMQuoteStatusAttribute.closed>()
									.IsTriggeredOn(g => g.convertToProject));
							});
						}))
					.WithActions(actions =>
					{
						actions.Add(g => g.submit, c => c.InFolder(processingCategory)
							.WithFieldAssignments(fa => fa.Add<PMQuote.hold>(f => f.SetFromValue(false))));
						actions.Add(g => g.editQuote, c => c.InFolder(processingCategory)
							.WithFieldAssignments(fa => fa.Add<PMQuote.hold>(f => f.SetFromValue(true))));
						actions.Add(g => g.convertToProject, c => c.InFolder(processingCategory)
							.IsDisabledWhen(!conditions.IsConvertToProjectEnabled));
						actions.Add(g => g.printQuote, c => c.InFolder(printingAndEmailingCategory));
						actions.Add(g => g.sendQuote, c => c.InFolder(printingAndEmailingCategory));
						actions.Add(g => g.copyQuote, c => c.InFolder(otherCategory));
						actions.Add(g => g.primaryQuote, c => c.InFolder(otherCategory)
							.IsDisabledWhen(!conditions.IsPrimaryQuoteEnabled));
						actions.Add(g => g.validateAddresses, c => c.InFolder(otherCategory));
						actions.Add(g => g.GetExtension<PMDiscount>().graphRecalculateDiscountsAction, c => c.InFolder(otherCategory));
					})
					.WithCategories(categories =>
					{
						categories.Add(processingCategory);
						categories.Add(printingAndEmailingCategory);
						categories.Add(otherCategory);
					}));
		}
	}
}

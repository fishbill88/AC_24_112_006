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
using PX.SM;
using PX.TM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Objects.CR;
using PX.Data.BQL.Fluent;
using PX.Objects.EP.DAC;
using System.Activities.Expressions;

namespace PX.Objects.EP
{
	public class EPApprovalMapMaint : EPApprovalAndAssignmentMapBase<EPApprovalMapMaint>, ICaptionable
	{
		public EPApprovalMapMaint()
		{
			base.AssigmentMap.View = new PXView(this, false, BqlCommand.CreateInstance(BqlCommand.Compose(
				typeof(Select<,>), typeof(EPAssignmentMap),
				typeof(Where<,>), typeof(EPAssignmentMap.mapType), typeof(Equal<>), typeof(EPMapType.approval))));
		}

		public string Caption()
		{
			EPAssignmentMap currentItem = this.AssigmentMap.Current;
			if (currentItem == null) return "";

			if (currentItem.Name != null && currentItem.GraphType != null)
			{
				var graphType = Definitions.SiteMapNodes.FirstOrDefault(x => x.GraphType == currentItem.GraphType);
				if (graphType != null)
				{
					return $"{currentItem.Name} - {graphType.Title}";
				}
			}
			return "";
		}

		public PXSelect<EPRuleEmployeeCondition, Where<EPRuleEmployeeCondition.ruleID, Equal<Current<EPRule.ruleID>>>>
			EmployeeCondition;

		public SelectFrom<EPRuleApprover>
					.InnerJoin<Contact>
						.On<Contact.contactID.IsEqual<EPRuleApprover.ownerID>>
					.InnerJoin<CREmployee>
						.On<CREmployee.defContactID.IsEqual<Contact.contactID>>
					.Where<EPRuleApprover.ruleID.IsEqual<EPRule.ruleID.FromCurrent>>.View
			RuleApprovers;

		protected virtual IEnumerable nodes([PXDBGuid] Guid? ruleID)
		{
			List<EPRule> list = new List<EPRule>();

			PXResultset<EPRule> resultSet;
			if (ruleID == null)
			{
				resultSet = PXSelect
					<EPRule, 
						Where<EPRule.assignmentMapID, Equal<Current<EPAssignmentMap.assignmentMapID>>,
						And<EPRule.stepID, IsNull>>>.Select(this);
			}
			else
			{
				var rule = (EPRule)PXSelect
					<EPRule,
						Where<EPRule.assignmentMapID, Equal<Current<EPAssignmentMap.assignmentMapID>>,
						And<EPRule.ruleID, Equal<Required<EPRule.ruleID>>>>>.Select(this, ruleID);
				if (rule == null || rule.StepID != null)
					return list;
				else
					resultSet = PXSelect
					<EPRule,
						Where<EPRule.assignmentMapID, Equal<Current<EPAssignmentMap.assignmentMapID>>,
						And<EPRule.stepID, Equal<Required<EPRule.stepID>>>>>.Select(this, rule.RuleID);
			}

			foreach (PXResult<EPRule> record in resultSet)
			{
				EPRule route = record;

				if(route.StepID != null)
					route.Icon = PX.Web.UI.Sprite.Tree.GetFullUrl(PX.Web.UI.Sprite.Tree.Leaf);
				else
					route.Icon = PX.Web.UI.Sprite.Main.GetFullUrl(PX.Web.UI.Sprite.Main.Folder);
				list.Add(route);
			}
			return list;
		}

		[PXUIField(MapEnableRights = PXCacheRights.Update, Enabled = true)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.ArrowUp)]
		public virtual IEnumerable Up(PXAdapter adapter)
		{
			IList<EPRule> routes = UpdateSequence();
			int currentItemIndex = (int)Nodes.Current.Sequence - 1;

			if (currentItemIndex > 0)
			{
				routes[currentItemIndex].Sequence--;
				routes[currentItemIndex - 1].Sequence++;

				Nodes.Update(routes[currentItemIndex]);
				Nodes.Update(routes[currentItemIndex - 1]);
				Nodes.Cache.ActiveRow = routes[currentItemIndex];
			}
			else if (Nodes.Current.StepID != null)
			{
				var previous = GetPreviousStep(Nodes.Current.StepID);
				if (previous != null)
				{
					Nodes.Current.StepID = previous.RuleID;
					Nodes.Current.Sequence = int.MaxValue;
					Nodes.Update(Nodes.Current);
					UpdateSequence();
					Nodes.Cache.ClearQueryCacheObsolete();
				}
			}

			return adapter.Get();
		}

		[PXUIField(MapEnableRights = PXCacheRights.Update, Enabled = true)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.ArrowDown)]
		public virtual IEnumerable Down(PXAdapter adapter)
		{
			IList<EPRule> routes = UpdateSequence();
			int currentItemIndex = (int)Nodes.Current.Sequence - 1;

			if (currentItemIndex < routes.Count - 1)
			{
				routes[currentItemIndex].Sequence++;
				routes[currentItemIndex + 1].Sequence--;

				Nodes.Update(routes[currentItemIndex]);
				Nodes.Update(routes[currentItemIndex + 1]);
				Nodes.Cache.ActiveRow = routes[currentItemIndex];
			}
			else if (Nodes.Current.StepID != null)
			{
				var next = GetNextStep(Nodes.Current.StepID);
				if (next != null)
				{
					Nodes.Current.StepID = next.RuleID;
					Nodes.Current.Sequence = 0;
					Nodes.Update(Nodes.Current);
					UpdateSequence();
					Nodes.Cache.ClearQueryCacheObsolete();
				}
			}

			return adapter.Get();
		}

		private EPRule GetPreviousStep(Guid? currentStepId)
		{
			return GetNextStep(currentStepId, Nodes.Select().OrderByDescending(x => ((EPRule)x).Sequence).ToList());
		}

		private EPRule GetNextStep(Guid? currentStepId)
		{
			return GetNextStep(currentStepId, Nodes.Select().ToList());
		}

		private EPRule GetNextStep(Guid? currentStepId, IEnumerable<PXResult<EPRule>> nodes)
		{
			bool isCurrent = false;
			foreach (PXResult<EPRule> node in nodes)
			{
				if (isCurrent)
					return node;
				if (((EPRule)node).RuleID == currentStepId)
					isCurrent = true;
			}
			return null;
		}

		public PXAction<EPAssignmentMap> addStep;
		[PXUIField(DisplayName = "Add Step", MapEnableRights = PXCacheRights.Update, Enabled = true)]
		[PXButton()]
		public virtual void AddStep()
		{
			var isNodes = Nodes.Current != null;

			var step = Nodes.Insert();
			step.Name = Messages.Step;
			if (!isNodes && !IsImport)
			{
				var rule = Nodes.Insert();
				rule.StepID = step.RuleID;
				rule.Name = Messages.Rule;
			}
			Nodes.Cache.ActiveRow = step;
		}
		
		[PXUIField(DisplayName = "Add Rule", MapEnableRights = PXCacheRights.Update, Enabled = true)]
		[PXButton()]
		public override void AddRule()
		{
			EPRule rule = null;
			if (Nodes.Current == null)
			{
				var step = Nodes.Insert();
				step.Name = Messages.Step;

				rule = Nodes.Insert();
				rule.StepID = step.RuleID;
			}
			else if (Nodes.Current.StepID == null)
			{
				var stepID = Nodes.Current.RuleID;
				rule = Nodes.Insert();
				rule.StepID = stepID;
			}
			else
			{
				var stepID = Nodes.Current.StepID;
				rule = Nodes.Insert();
				rule.StepID = stepID;
			}
			rule.Name = Messages.Rule;
			Nodes.Cache.ActiveRow = rule;
		}

		#region Event Handlers
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[EPAssignmentMapSelector(MapType = EPMapType.Approval)]
		protected virtual void EPAssignmentMap_AssignmentMapID_CacheAttached(PXCache sender)
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXDefault(EPMapType.Approval)]
		protected virtual void EPAssignmentMap_MapType_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXUIField(DisplayName = "Workgroup")]
		[PXCompanyTreeSelector]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<EPRuleTree.ruleType>))]
		protected virtual void EPRuleTree_WorkgroupID_CacheAttached(PXCache sender)
		{
		}

		[Owner(null, typeof(Search2<
				OwnerAttribute.Owner.contactID,
			LeftJoin<EPCompanyTreeMember,
				On<EPCompanyTreeMember.contactID, Equal<OwnerAttribute.Owner.contactID>,
				And<EPCompanyTreeMember.workGroupID, Equal<Current<EPRule.workgroupID>>>>>,
			Where2<Where<
					Current<EPRule.workgroupID>, IsNull,
					Or<EPCompanyTreeMember.contactID, IsNotNull>>,
				And<OwnerAttribute.Owner.contactType, Equal<ContactTypesAttribute.employee>>>>),
			headerList: new[] {
				"Employee Name",
				"Job Title",
				"Email",
				"Phone 1",
				"Department",
				"Employee ID",
				"Status",
				"Branch",
				"Reports To",
				"Type",
				"User ID"
			}, DisplayName = "Employee Name")]
		[PXMergeAttributes(Method = MergeMethod.Replace)]
		protected virtual void _(Events.CacheAttached<EPRuleApprover.ownerID> e) { }

		protected override void EPAssignmentMap_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			EPAssignmentMap row = e.Row as EPAssignmentMap;
			addStep.SetEnabled(row != null && row.EntityType != null);
			
			PXUIFieldAttribute.SetVisible(Rules.Cache, null, CurrentNode.Current != null);
			PXUIFieldAttribute.SetVisible(this.EmployeeCondition.Cache, null, false);
			
			base.EPAssignmentMap_RowSelected(sender, e);
		}
		
		protected virtual IEnumerable<string> GetEntityTypeScreens()
		{
			return new string[]
			{
				"AR301000",//Invoice and Memos
				"AP301000",//Bills and Adjustments
				"AP302000",//Checks and Payments
				"AP304000",//Cash Purchases
				"AR302000",//Payments and Applications
				"AR304000",//Cash Sales
				"CA304000",//Cash Transactions
				"EP305000",//Employee Time Card
				"EP308000",//Equipment Time Card
				"EP301000",//Expense Claim
				"EP301020",//Expense Receipt
				"PM301000",//Projects
				"PM307000",//Proforma
				"PM308000",//Change Order
				"PM308500",//Change Request
				"PO301000",//Purchase Order
				"RQ301000",//Purchase Request
				"RQ302000",//Purchase Requisition
				"SO301000",//Sales Order
				"SO303000",//SO Invoices
				"CR304500",//Sales Quote
				"PM304500",//Project Quote
				"PM305000",//Cost Projection
				"GL301000",//Journal Transactions
			};
		}

		protected virtual void EPAssignmentMap_GraphType_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			EPAssignmentMap row = e.Row as EPAssignmentMap;
			if (row != null)
			{
				var list = Definitions.SiteMapNodes.OrderBy(x => x.Title);
				PXStringListAttribute.SetLocalizable<EPAssignmentMap.graphType>(sender, null, false);
				PXStringListAttribute.SetList<EPAssignmentMap.graphType>(sender, row,
					list.Select(x => x.GraphType).ToArray(), list.Select(x => x.Title).ToArray());
			}
		}
		#region GraphTypes Cache
		private Definition Definitions
		{
			get
			{
				Definition defs = PXContext.GetSlot<Definition>();
				if (defs == null)
				{
					defs = PXContext.SetSlot<Definition>(PXDatabase.GetSlot<Definition, EPApprovalMapMaint>(typeof(Definition).FullName, this, typeof(SiteMap)));
				}
				return defs;
			}
		}
		private class Definition : IPrefetchable<EPApprovalMapMaint>
		{
			public List<PXSiteMapNode> SiteMapNodes = new List<PXSiteMapNode>();

			public void Prefetch(EPApprovalMapMaint graph)
			{
				SiteMapNodes = PXSiteMap.Provider.GetNodes()
					.Where(x => graph.GetEntityTypeScreens().Contains(x.ScreenID) && graph.IsEpApproval(x))
					.GroupBy(x => x.ScreenID).Select(x => x.First()).ToList();
			}
		}
		#endregion

		private bool IsEpApproval(PXSiteMapNode node)
		{
			var views = GraphHelper.GetGraphViews(node.GraphType, false);
			return views.Any(x => x.Cache != null && x.Cache.CacheType == typeof(EPApproval));
		}

		protected virtual void EPRule_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			EPRule row = e.Row as EPRule;
			if (row != null)
			{
				var rules = UpdateSequence();
				if (row.StepID != null)
				{
					var isCurrent = false;
					foreach (var rule in rules)
					{
						if (isCurrent)
						{
							rule.Sequence++;
							Nodes.Update(rule);
						}
						else if (rule.RuleID == Nodes.Current.RuleID)
						{
							isCurrent = true;
							row.Sequence = rule.Sequence + 1;
						}
					}
				}
				else
					row.Sequence = rules.Count + 1;
			}
		}

		protected virtual void EPRule_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			EPRule row = e.Row as EPRule;
			if (row == null)
				return;
			row.OwnerID = null;
			if (row.OwnerSource == null && row.StepID != null && row.RuleType == EPRuleType.Document)
				throw new PXSetPropertyException<EPRule.ownerSource>(Messages.EmployeeCannotBeEmpty, PXErrorLevel.Error);
		}

		protected virtual void EPRule_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			EPRule row = e.Row as EPRule;
			if (row == null)
				return;

			if (row.StepID == null)
			{
				foreach (PXResult<EPRule> rule in PXSelect<EPRule, Where<EPRule.stepID, Equal<Required<EPRule.ruleID>>>>
					.Select(this, row.RuleID))
				{
					if (Nodes.Cache.GetStatus((EPRule)rule) == PXEntryStatus.Inserted)
						Nodes.Cache.SetStatus((EPRule)rule, PXEntryStatus.InsertedDeleted);
					else
						Nodes.Delete(rule);
				}
			}
		}

		protected override void EPRule_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			base.EPRule_RowSelected(sender, e);

			EPRule row = e.Row as EPRule;
			if (row == null)
				return;

			PXUIFieldAttribute.SetVisible(RuleApprovers.Cache, null, row.StepID != null && row.RuleType == EPRuleType.Direct);
			PXUIFieldAttribute.SetVisible<EPRule.ownerSource>(this.Nodes.Cache, this.Nodes.Current, row.StepID != null && row.RuleType == EPRuleType.Document);
			PXUIFieldAttribute.SetVisible(EmployeeCondition.Cache, null, row.StepID != null && row.RuleType == EPRuleType.Filter);

			PXUIFieldAttribute.SetVisible<EPRule.emptyStepType>(this.Nodes.Cache, this.Nodes.Current, row.StepID == null);
			PXUIFieldAttribute.SetVisible<EPRule.executeStep>(this.Nodes.Cache, this.Nodes.Current, row.StepID == null);
			PXUIFieldAttribute.SetVisible(Rules.Cache, null, null, row.StepID != null);
			PXUIFieldAttribute.SetVisible<EPRule.ruleType>(this.Nodes.Cache, this.Nodes.Current, row.StepID != null);
			PXUIFieldAttribute.SetVisible<EPRule.workgroupID>(this.Nodes.Cache, this.Nodes.Current, row.StepID != null);
			PXUIFieldAttribute.SetVisible<EPRule.approveType>(this.Nodes.Cache, this.Nodes.Current, row.StepID != null);
			PXUIFieldAttribute.SetVisible<EPRule.waitTime>(this.CurrentNode.Cache, this.CurrentNode.Current, row.StepID != null);
			PXUIFieldAttribute.SetVisible<EPRule.allowReassignment>(this.CurrentNode.Cache, this.CurrentNode.Current, row.StepID != null);

			bool isOfSupportedType = GetSupportedTypes().Contains(AssigmentMap.Current?.GraphType, new PX.Data.CompareIgnoreCase());

			PXUIFieldAttribute.SetVisible<EPRule.reasonForApprove>(this.Nodes.Cache, this.Nodes.Current, isOfSupportedType && row.StepID != null);
			PXUIFieldAttribute.SetVisible<EPRule.reasonForReject>(this.Nodes.Cache, this.Nodes.Current, isOfSupportedType && row.StepID != null);

			Exception ex = null;
			if (row.RuleType == EPRuleType.Filter)
				ex = new PXSetPropertyException(Messages.AllEmployeesByFilterRequired, PXErrorLevel.Warning);
			else if (row.RuleType == EPRuleType.Direct)
				ex = new PXSetPropertyException(Messages.AllEmployeesInTableRequired, PXErrorLevel.Warning);
			sender.RaiseExceptionHandling<EPRule.ruleType>(row, row.RuleType, ex);
		}		

		protected virtual void EPRuleEmployeeCondition_RuleID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = CurrentNode.Current?.RuleID;
		}

		protected virtual void EPRuleEmployeeCondition_FieldName_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			EPRuleEmployeeCondition row = e.Row as EPRuleEmployeeCondition;
			if (row != null)
				e.ReturnState = CreateFieldStateForFieldName(e.ReturnState, row.Entity, null, row.Entity);
		}

		protected virtual void EPRuleEmployeeCondition_FieldName_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			EPRuleEmployeeCondition row = e.Row as EPRuleEmployeeCondition;
			if (row == null) return;

			Type cachetype = GraphHelper.GetType(row.Entity);
			if (cachetype == null)
				return;

			PXCache cache = this.Caches[cachetype];
			PXDBAttributeAttribute.Activate(cache);
			PXFieldState state = cache.GetStateExt(null, e.NewValue.ToString()) as PXFieldState;
			if (state == null)
				throw new PXException(Messages.FieldCannotBeFound);
		}

		protected virtual void _(Events.RowUpdated<EPRuleEmployeeCondition> e) 
		{
			EPRuleEmployeeCondition oldRow = e.OldRow as EPRuleEmployeeCondition;
			EPRuleEmployeeCondition newRow = e.Row as EPRuleEmployeeCondition;
			if (oldRow != null && newRow != null)
			{
				if (!String.Equals(newRow.Entity, oldRow.Entity, StringComparison.OrdinalIgnoreCase)) newRow.FieldName = newRow.Value = newRow.Value2 = null;
				if (!String.Equals(newRow.FieldName, oldRow.FieldName, StringComparison.OrdinalIgnoreCase)) newRow.Value = newRow.Value2 = null;
				EPRuleEmployeeCondition row = e.Row as EPRuleEmployeeCondition;

				if (row.Condition == null || (PXCondition)row.Condition == PXCondition.ISNULL || (PXCondition)row.Condition == PXCondition.ISNOTNULL)
				{
					newRow.Value = newRow.Value2 = null;
				}
				if (newRow.Value == null)
				{
					PXFieldState state = e.Cache.GetStateExt<EPRuleEmployeeCondition.value>(newRow) as PXFieldState;
					newRow.Value = state != null && state.Value != null ? state.Value.ToString() : null;
				}
			}
		}

		protected virtual void EPRuleEmployeeCondition_Value_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			EPRuleEmployeeCondition row = e.Row as EPRuleEmployeeCondition;
			if (row == null) return;

			if (row.IsField == true)
			{
				e.ReturnState = CreateFieldStateForFieldName(e.ReturnState,
					AssigmentMap.Current.EntityType,
					null,
					AssigmentMap.Current.EntityType);
				return;
			}

			if (!string.IsNullOrEmpty(row.FieldName) && row.Condition != null &&
				(PXCondition)row.Condition != PXCondition.ISNULL && (PXCondition)row.Condition != PXCondition.ISNOTNULL &&
				(PXCondition)row.Condition != PXCondition.NOTLIKE && (PXCondition)row.Condition != PXCondition.LIKE &&
				(PXCondition)row.Condition != PXCondition.LLIKE && (PXCondition)row.Condition != PXCondition.RLIKE)
			{
				var fieldState = CreateFieldStateForFieldValue(e.ReturnState, row.Entity, row.Entity, row.FieldName);

				if (fieldState != null)
					e.ReturnState = fieldState;
			}
		}
		protected virtual void EPRuleEmployeeCondition_Value2_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			EPRuleEmployeeCondition row = e.Row as EPRuleEmployeeCondition;
			if (row == null) return;

			if (row.IsField == true)
			{
				e.ReturnState = CreateFieldStateForFieldName(e.ReturnState,
					AssigmentMap.Current.EntityType,
					null,
					AssigmentMap.Current.EntityType);
				return;
			}

			if (row != null && !string.IsNullOrEmpty(row.FieldName) && row.Condition != null &&
				(PXCondition)row.Condition != PXCondition.ISNULL && (PXCondition)row.Condition != PXCondition.ISNOTNULL &&
				(PXCondition)row.Condition != PXCondition.NOTLIKE && (PXCondition)row.Condition != PXCondition.LIKE &&
				(PXCondition)row.Condition != PXCondition.LLIKE && (PXCondition)row.Condition != PXCondition.RLIKE)
			{
				var fieldState = CreateFieldStateForFieldValue(e.ReturnState, row.Entity, row.Entity, row.FieldName);

				if (fieldState != null)
				{
					fieldState.Value = null;
					e.ReturnState = fieldState;
				}
			}
		}

		protected virtual void _(Events.RowSelected<EPRuleApprover> e)
		{
			EPRuleApprover ruleApprover = e.Row;
			if (CurrentNode.Current?.RuleType != EPRuleType.Direct)
			{
				PXUIFieldAttribute.SetError<EPRuleApprover.ownerID>(e.Cache, ruleApprover, null);
				return;
			}
			var employee = EmployeeInactive.SelectSingle(ruleApprover.OwnerID);
			if (employee != null)
				PXUIFieldAttribute.SetWarning<EPRuleApprover.ownerID>(e.Cache, ruleApprover, Messages.EmployeeIsInactive);
		}

		protected virtual void _(Events.FieldUpdated<EPRule, EPRule.workgroupID> e)
		{
			if (e.Row == null)
				return;

			this.RuleApprovers.SelectMain().ForEach(delegate(EPRuleApprover approver)
			{
				object owner = approver.OwnerID;

				try
				{
					this.RuleApprovers.Cache.RaiseFieldVerifying<EPRuleApprover.ownerID>(approver, ref owner);
				}
				catch (Exception)
				{
					this.RuleApprovers.Delete(approver);
				}
			});
		}

		#endregion

		#region Public Methods
		public virtual List<string> GetSupportedTypes()
		{
			return new List<string> {
				typeof(EP.TimeCardMaint).FullName,
				typeof(EP.EquipmentTimeCardMaint).FullName,
				typeof(EP.ExpenseClaimEntry).FullName,
				typeof(PM.ProformaEntry).FullName,
				typeof(PM.ChangeOrderEntry).FullName,
				typeof(CR.QuoteMaint).FullName,
				typeof(PM.PMQuoteMaint).FullName,
				typeof(GL.JournalEntry).FullName,
				typeof(PM.ProgressWorksheetEntry).FullName,
				typeof(AP.APInvoiceEntry).FullName,
				typeof(AR.ARInvoiceEntry).FullName,
				typeof(AP.APPaymentEntry).FullName,
				typeof(AR.ARPaymentEntry).FullName,
				typeof(CA.CATranEntry).FullName,
				typeof(AR.ARCashSaleEntry).FullName,
				typeof(AP.APQuickCheckEntry).FullName
			};
		}

		protected virtual void _(Events.FieldUpdated<EPRuleCondition, EPRuleCondition.fieldName> e)
		{
			EPRuleCondition row = e.Row as EPRuleCondition;
			if (row == null) return;

			if (e.NewValue != null && (e.NewValue as string).Equals(GetWorkflowStatePropertyName(), StringComparison.OrdinalIgnoreCase))
			{
				throw new PXSetPropertyException<EPRuleCondition.fieldName>(Messages.FieldCannotBeFound, PXErrorLevel.Error);
			}
		}
		#endregion

		public virtual string GetWorkflowStatePropertyName()
		{
			var screenID = PXSiteMap.Provider.FindSiteMapNodeByGraphType(AssigmentMap.Current.GraphType)?.ScreenID;

			return
				!String.IsNullOrEmpty(screenID)
					? WorkflowService.GetWorkflowStatePropertyName(screenID)
					: String.Empty;
		}

		public override bool CheckFieldValid(KeyValuePair<string, string> item) =>
			!item.Key.Equals(GetWorkflowStatePropertyName(), StringComparison.OrdinalIgnoreCase)
			&& base.CheckFieldValid(item);
	}
}

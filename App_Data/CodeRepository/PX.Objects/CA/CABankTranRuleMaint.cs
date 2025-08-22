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

using System.Collections;
using PX.Data;

namespace PX.Objects.CA
{
	public class CABankTranRuleMaint : PXGraph<CABankTranRuleMaint, CABankTranRule>
	{
		public PXSelect<CABankTranRule> Rule;

		protected virtual void CABankTranRule_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			if (string.IsNullOrWhiteSpace(rule.BankTranDescription)
			    && rule.CuryTranAmt == null
			    && string.IsNullOrWhiteSpace(rule.TranCode)
			    && string.IsNullOrWhiteSpace(rule.PayeeName))
			{
				throw new PXException(Messages.BankRuleTooLoose);
			}

			PXDefaultAttribute.SetPersistingCheck<CABankTranRule.documentModule>(cache, rule, rule.Action == RuleAction.CreateDocument ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CABankTranRule.documentEntryTypeID>(cache, rule, rule.Action == RuleAction.CreateDocument ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CABankTranRule.curyTranAmt>(cache, rule, rule.AmountMatchingMode != MatchingMode.None ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CABankTranRule.maxCuryTranAmt>(cache, rule, rule.AmountMatchingMode == MatchingMode.Between ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);

			if (rule.BankTranCashAccountID == null || rule.DocumentEntryTypeID == null)
			{
				return;
			}

			CashAccountETDetail entryTypeForAcct = PXSelect<CashAccountETDetail,
				Where<CashAccountETDetail.cashAccountID, Equal<Required<CashAccountETDetail.cashAccountID>>,
					And<CashAccountETDetail.entryTypeID, Equal<Required<CashAccountETDetail.entryTypeID>>>>>
				.Select(this, rule.BankTranCashAccountID, rule.DocumentEntryTypeID);

			if (entryTypeForAcct == null)
			{
					cache.RaiseExceptionHandling<CABankTranRule.documentEntryTypeID>(rule,
						rule.DocumentEntryTypeID,
					new PXSetPropertyException<CABankTranRule.documentEntryTypeID>(Messages.BankRuleEntryTypeDoesntSuitCashAccount));
			}
		}

		protected virtual void CABankTranRule_RowDeleting(PXCache cache, PXRowDeletingEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			CABankTran referer = PXSelect<CABankTran, Where<CABankTran.ruleID, Equal<Required<CABankTran.ruleID>>>>.Select(this, rule.RuleID);

			if (referer != null)
			{
				throw new PXException(Messages.BankRuleInUseCantDelete);
			}
		}

		protected virtual void CABankTranRule_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			if (rule == null)
				return;

			bool isCreateDoc = rule.Action == RuleAction.CreateDocument;

			PXUIFieldAttribute.SetRequired<CABankTranRule.documentModule>(cache, isCreateDoc);
			PXUIFieldAttribute.SetRequired<CABankTranRule.documentEntryTypeID>(cache, isCreateDoc);

			PXUIFieldAttribute.SetVisible<CABankTranRule.documentModule>(cache, rule, isCreateDoc);
			PXUIFieldAttribute.SetVisible<CABankTranRule.documentEntryTypeID>(cache, rule, isCreateDoc);

			var minMaxAmtNeeded = rule.AmountMatchingMode == MatchingMode.Between;
			var amtNeeded = rule.AmountMatchingMode != MatchingMode.None && !minMaxAmtNeeded;

			PXUIFieldAttribute.SetVisible<CABankTranRule.curyTranAmt>(cache, rule, amtNeeded);
			PXUIFieldAttribute.SetVisible<CABankTranRule.curyMinTranAmt>(cache, rule, minMaxAmtNeeded);
			PXUIFieldAttribute.SetVisible<CABankTranRule.maxCuryTranAmt>(cache, rule, minMaxAmtNeeded);

			PXUIFieldAttribute.SetRequired<CABankTranRule.curyTranAmt>(cache, amtNeeded);
			PXUIFieldAttribute.SetRequired<CABankTranRule.curyMinTranAmt>(cache, minMaxAmtNeeded);
			PXUIFieldAttribute.SetRequired<CABankTranRule.maxCuryTranAmt>(cache, minMaxAmtNeeded);

			bool explicitCurrencyAllowed = rule.BankTranCashAccountID == null;
			PXUIFieldAttribute.SetEnabled<CABankTranRule.tranCuryID>(cache, rule, explicitCurrencyAllowed);
		}

		protected virtual void CABankTranRule_BankTranCashAccountID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			if (rule == null)
				return;

			if (rule.BankTranCashAccountID != null)
			{
				var cashAcct = (CashAccount)PXSelectorAttribute.Select<CABankTranRule.bankTranCashAccountID>(cache, rule);
				cache.SetValueExt<CABankTranRule.tranCuryID>(rule, cashAcct.CuryID);
			}
			else
			{
				cache.SetDefaultExt<CABankTranRule.tranCuryID>(rule);
			}
		}

		protected virtual void CABankTranRule_AmountMatchingMode_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			if (rule == null)
				return;

			switch (rule.AmountMatchingMode)
			{
				case MatchingMode.None:
					cache.SetValueExt<CABankTranRule.curyTranAmt>(rule, null);
					cache.SetValueExt<CABankTranRule.maxCuryTranAmt>(rule, null);
					break;
				case MatchingMode.Equal:
					cache.SetValueExt<CABankTranRule.maxCuryTranAmt>(rule, null);
					break;
				case MatchingMode.Between:
					if (rule.MaxCuryTranAmt == null)
					{
						cache.SetValueExt<CABankTranRule.maxCuryTranAmt>(rule, rule.CuryTranAmt);
					}
					break;
			}
		}

		protected virtual void CABankTranRule_Action_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var rule = e.Row as CABankTranRule;

			if (rule.Action == RuleAction.CreateDocument)
			{
				cache.SetDefaultExt<CABankTranRule.documentModule>(rule);
				cache.SetDefaultExt<CABankTranRule.documentEntryTypeID>(rule);
			}
			else
			{
				rule.DocumentModule = null;
				rule.DocumentEntryTypeID = null;
			}
		}
	}

	public class CABankTranRuleMaintPopup : CABankTranRuleMaint
	{
		public PXAction<CABankTranRule> saveAndApply;
		[PXUIField(DisplayName = "Apply to All", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Save, ClosePopup = true)]
		public virtual IEnumerable SaveAndApply(PXAdapter adapter)
		{
			base.Actions.PressSave();
			DoMatch();
			base.Actions.PressSave();
			return adapter.Get();
		}

		public PXAction<CABankTranRule> saveClose;
		[PXUIField(DisplayName = "Apply", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Save, ClosePopup = true)]
		public virtual IEnumerable SaveClose(PXAdapter adapter)
		{
			base.Actions.PressSave();
			return adapter.Get();
		}
		protected virtual void DoMatch()
		{
			CABankTranRule rule = Rule.Current;
			var caBankTransactionsMaint = PXGraph.CreateInstance<CABankTransactionsMaint>();
			CABankTransactionsMaint.Filter filter = new CABankTransactionsMaint.Filter()
			{
				CashAccountID = rule.BankTranCashAccountID,
				TranType = CABankTranType.Statement
			};
			object newValue;
			caBankTransactionsMaint.TranFilter.Current = filter;
			caBankTransactionsMaint.TranFilter.Cache.RaiseFieldDefaulting<CABankTransactionsMaint.Filter.tranType>(filter, out newValue);
			caBankTransactionsMaint.cancel.Press();
			if (rule.IsActive == true)
			{
				caBankTransactionsMaint.ApplyRule(rule);
				caBankTransactionsMaint.Actions.PressSave();
			}
		}

		protected override void CABankTranRule_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			base.CABankTranRule_RowSelected(cache, e);

			var rule = e.Row as CABankTranRule;
			if (rule == null)
				return;

			bool isActive = rule.IsActive == true;

			this.saveAndApply.SetEnabled(isActive);
		}
	}
}

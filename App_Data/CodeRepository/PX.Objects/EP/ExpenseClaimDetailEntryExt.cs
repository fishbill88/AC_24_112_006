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

using System;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.Common.Extensions;
using PX.Objects.CS;

namespace PX.Objects.EP
{
	/// <summary>
	/// Event Handlers
	/// </summary>
	/// <typeparam name="TGraph"></typeparam>
	public abstract class ExpenseClaimDetailEntryExt<TGraph> : ExpenseClaimDetailGraphExtBase<TGraph>
		where TGraph : PXGraph
	{
		public virtual bool UseClaimStatus => false;

		[PXCopyPasteHiddenView]
		public PXSetup<EPSetup> epsetup;

		#region Event hanlders

		protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails, EPExpenseClaimDetails.inventoryID> e)
		{
			decimal? curyStdCost = GetUnitCostByExpenseItem(e.Cache, e.Row);

			if (curyStdCost != null && curyStdCost != 0m)
			{
				e.Cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(e.Row, curyStdCost);
			}
		}

		protected virtual void _(Events.FieldDefaulting<EPExpenseClaimDetails, EPExpenseClaimDetails.curyUnitCost> e)
		{
			e.NewValue = GetUnitCostByExpenseItem(e.Cache, e.Row);
		}

		public virtual void _(Events.RowSelected<EPExpenseClaimDetails> e)
		{
			if (e.Row is EPExpenseClaimDetails row)
			{
				EPExpenseClaim claim = GetParentClaim(row.RefNbr);

				bool enabledApprovalReceipt = PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>() && epsetup.Current.ClaimDetailsAssignmentMapID != null;
				bool legacyClaim = row.LegacyReceipt == true && !String.IsNullOrEmpty(row.RefNbr);
				bool enabledEditReceipt = (row.Hold == true || !enabledApprovalReceipt || (UseClaimStatus && claim?.Hold == true)) && !legacyClaim;

				if (claim != null)
				{
					bool enabledEditClaim = (claim.Hold == true);
					enabledEditReceipt = enabledEditReceipt && enabledEditClaim;
				}

				bool notMatchedToBankTran = row.BankTranDate == null;

				e.Cache.Adjust<PXUIFieldAttribute>()
					.For<EPExpenseClaimDetails.expenseDate>(ui =>
						ui.Enabled = notMatchedToBankTran && enabledEditReceipt)
					.SameFor<EPExpenseClaimDetails.qty>()
					.SameFor<EPExpenseClaimDetails.uOM>()
					.SameFor<EPExpenseClaimDetails.curyUnitCost>()
					.SameFor<EPExpenseClaimDetails.curyEmployeePart>()
					.SameFor<EPExpenseClaimDetails.curyExtCost>()
					.SameFor<EPExpenseClaimDetails.paidWith>();

				PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.curyTipAmt>(e.Cache, row, notMatchedToBankTran 
																								&& enabledEditReceipt 
																								&& row.PaidWith != EPExpenseClaimDetails.paidWith.CardPersonalExpense);

				var corpCardIsEnabled = row.PaidWith != EPExpenseClaimDetails.paidWith.PersonalAccount && notMatchedToBankTran && row.Released != true;
				PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.corpCardID>(e.Cache, row, corpCardIsEnabled);

				bool isProjectEnabled = row.PaidWith != EPExpenseClaimDetails.paidWith.CardPersonalExpense && row.Released != true;
				PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.contractID>(e.Cache, row, isProjectEnabled);
				PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.taskID>(e.Cache, row, isProjectEnabled);
				PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.costCodeID>(e.Cache, row, isProjectEnabled);

				UIState.RaiseOrHideError<EPExpenseClaimDetails.claimDetailCD>(e.Cache, row, row.BankTranDate != null,
					Messages.SomeBoxesAndActionsOnTheFormAreUnavailableExpenseReceiptIsMatchedToBankStatement, PXErrorLevel.Warning,
					row.BankTranDate?.ToShortDateString());

				e.Cache.VerifyFieldAndRaiseException<EPExpenseClaimDetails.curyEmployeePart>(row);
			}
		}

		public virtual void _(Events.RowPersisting<EPExpenseClaimDetails> e)
		{
			if (e.Operation != PXDBOperation.Delete)
			{
				if (e.Row.PaidWith != EPExpenseClaimDetails.paidWith.PersonalAccount && e.Row.CorpCardID == null)
				{
					var fieldName = typeof(EPExpenseClaimDetails.corpCardID).Name;
					throw new PXRowPersistingException(fieldName, e.Row.CorpCardID, ErrorMessages.FieldIsEmpty, $"[{fieldName}]");
				}

				EPExpenseClaim claim = GetParentClaim(e.Row.RefNbr);

				VerifyEmployeePartIsZeroForCorpCardReceipt(e.Row.PaidWith, e.Row.CuryEmployeePart);
				VerifyEmployeePartNotExceedTotal(e.Row.CuryExtCost, e.Row.CuryEmployeePart);
				VerifyEmployeePartSign(e.Row.CuryExtCost, e.Row.CuryEmployeePart);
				VerifyEmployeeAndClaimCurrenciesForCash(e.Row, e.Row.PaidWith, claim);
				VerifyClaimAndCorpCardCurrencies(e.Row.CorpCardID, claim);
				VerifyExpenseRefNbrIsNotEmpty(e.Row);
			}
		}

		public virtual void _(Events.RowInserted<EPExpenseClaimDetails> e)
		{
			if (e.Row is EPExpenseClaimDetails row)
			{
				DefaultCardCurrencyInfo(e.Cache, row);

				SetClaimCuryWhenNotInClaim(row, row.RefNbr, row.CorpCardID);

				SetCardCurrencyData(e.Cache, row, row.CorpCardID);

				ClearFieldsIfNeeded(e.Cache, row);
			}
		}

		public virtual void _(Events.RowUpdated<EPExpenseClaimDetails> e)
		{
			AmtFieldUpdated(e.OldRow, e.Row);
		}

		public virtual void _(Events.FieldUpdated<EPExpenseClaimDetails, EPExpenseClaimDetails.paidWith> e)
		{
			var receipt = e.Row;
			if (receipt == null)
				return;

			ClearFieldsIfNeeded(e.Cache, receipt);

			PXUIFieldAttribute.SetError<EPExpenseClaimDetails.curyEmployeePart>(e.Cache, receipt, null);
			var value = PXFormulaAttribute.Evaluate<EPExpenseClaimDetails.curyEmployeePart>(e.Cache, receipt);
			e.Cache.SetValueExt<EPExpenseClaimDetails.curyEmployeePart>(receipt, value ?? receipt.CuryEmployeePart);
			e.Cache.SetValuePending<EPExpenseClaimDetails.curyEmployeePart>(receipt, value ?? receipt.CuryEmployeePart);
			e.Cache.VerifyFieldAndRaiseException<EPExpenseClaimDetails.curyEmployeePart>(receipt);
		}

		public virtual void _(Events.FieldDefaulting<EPExpenseClaimDetails.paidWith> e)
		{
			if (e.Row is EPExpenseClaimDetails row)
			{
				if (row.EmployeeID != null)
				{
					var thereIsCreditCardForEmployee = GetFirstCreditCardForEmployeeAlphabeticallySorted(row.EmployeeID.Value) != null;

					e.NewValue = thereIsCreditCardForEmployee 
						? EPExpenseClaimDetails.paidWith.CardCompanyExpense
						: EPExpenseClaimDetails.paidWith.PersonalAccount;
				}
			}
		}

		public virtual void _(Events.FieldDefaulting<EPExpenseClaimDetails.corpCardID> e)
		{
			if (!(e.Row is EPExpenseClaimDetails row))
			{
				return;
			}
			if (row.EmployeeID == null)
			{
				e.NewValue = null;
				return;
			}
			if (row.PaidWith == EPExpenseClaimDetails.paidWith.PersonalAccount)
			{
				e.NewValue = null;
				return;
			}
			var firstCreditCard = GetFirstCreditCardForEmployeeAlphabeticallySorted(row.EmployeeID.Value);
			var thereIsCreditCardForEmployee = firstCreditCard != null;
			if (!thereIsCreditCardForEmployee)
			{
				e.NewValue = null;
				return;
			}
			var lastUsedCreditCard = GetLastUsedCreditCardForEmployee(row.EmployeeID.Value);

			var corpCardId = lastUsedCreditCard != null
				? lastUsedCreditCard.CorpCardID
				: firstCreditCard.CorpCardID;

			if (row.RefNbr != null)
			{
				EPExpenseClaim claim = PXParentAttribute.SelectParent<EPExpenseClaim>(e.Cache, row);

				CashAccount cashAccount = CACorpCardsMaint.GetCardCashAccount(Base, corpCardId);

				if (cashAccount.CuryID != claim.CuryID)
				{
					corpCardId = null;
				}
			}
			e.NewValue = corpCardId;
			e.Cache.SetValueExt<EPExpenseClaimDetails.corpCardID>(row, corpCardId);
		}

		public virtual void _(Events.FieldVerifying<EPExpenseClaimDetails.curyExtCost> e)
		{
			var receipt = (EPExpenseClaimDetails)e.Row;
			if (receipt == null || Base.IsCopyPasteContext)
				return;

			decimal? newValue = (decimal?)e.NewValue;

			if (newValue != null)
			{
				string paidWith = (string)BqlHelper.GetValuePendingOrRow<EPExpenseClaimDetails.paidWith>(e.Cache, receipt);

				VerifyIsPositiveForCorpCardReceipt(paidWith, newValue);
			}
		}

		public virtual void _(Events.FieldVerifying<EPExpenseClaimDetails.curyEmployeePart> e)
		{
			EPExpenseClaimDetails receipt = (EPExpenseClaimDetails)e.Row;
			if (receipt == null || Base.IsCopyPasteContext)
				return;

			decimal? newValue = (decimal?)e.NewValue;

			if (newValue != null)
			{
				string paidWith = (string)BqlHelper.GetValuePendingOrRow<EPExpenseClaimDetails.paidWith>(e.Cache, receipt);

				VerifyEmployeePartIsZeroForCorpCardReceipt(paidWith, newValue);

				decimal? total = (decimal?)BqlHelper.GetValuePendingOrRow<EPExpenseClaimDetails.curyExtCost>(e.Cache, receipt);

				VerifyEmployeePartNotExceedTotal(total, newValue);
				VerifyEmployeePartSign(total, newValue);
			}
		}

		public virtual void _(Events.FieldVerifying<EPExpenseClaimDetails.refNbr> e)
		{
			EPExpenseClaimDetails receipt = (EPExpenseClaimDetails)e.Row;
			string newClaimRefNbr = (string)e.NewValue;

			object pendingCorpCard = e.Cache.GetValuePending<EPExpenseClaimDetails.corpCardID>(receipt);
			int? corpCardID = null;
			if (pendingCorpCard == PXCache.NotSetValue)
			{
				corpCardID = receipt.CorpCardID;
			}
			else
			{
				if (pendingCorpCard is string corpCardCD)
				{
					corpCardID = CACorpCard.UK.Find(e.Cache.Graph, corpCardCD)?.CorpCardID;
				}
				else
				{
					corpCardID = (int?)pendingCorpCard;
				}
			}

			EPExpenseClaim newClaim = GetParentClaim(newClaimRefNbr);

			VerifyClaimAndCorpCardCurrencies(corpCardID, newClaim);
			VerifyEmployeeAndClaimCurrenciesForCash(receipt, receipt.PaidWith, newClaim);
		}

		protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails.corpCardID> e)
		{
			EPExpenseClaimDetails document = (EPExpenseClaimDetails)e.Row;
			int? newCorpCardID = (int?)e.NewValue;

			SetCardCurrencyData(e.Cache, document, newCorpCardID);
			SetClaimCuryWhenNotInClaim(document, document.RefNbr, newCorpCardID);
			RecalcAmountInClaimCury(document);
		}

		protected virtual void EPTaxTran_CuryTaxableAmt_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			RecalcAmountInClaimCury(Receipts.Current);
		}

		protected virtual void EPTaxTran_CuryTaxAmt_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			RecalcAmountInClaimCury(Receipts.Current);
		}

		protected virtual void EPTaxTran_ClaimCuryTaxAmt_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			RecalcAmountInClaimCury(Receipts.Current);
		}

		protected virtual void EPTaxTran_CuryExpenseAmt_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			RecalcAmountInClaimCury(Receipts.Current);
		}

		protected virtual void _(Events.RowUpdated<CurrencyInfo> e)
		{
			EPExpenseClaimDetails receipt = Receipts.Current;

			if (receipt != null)
			{
				RecalcAmountInClaimCury(receipt);
			}
		}
		#endregion
	}
}

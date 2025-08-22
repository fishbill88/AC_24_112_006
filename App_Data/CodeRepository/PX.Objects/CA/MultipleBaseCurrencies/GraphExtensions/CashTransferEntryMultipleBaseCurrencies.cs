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
using PX.Objects.CS;

namespace PX.Objects.CA
{
	public class CashTransferEntryMultipleBaseCurrencies : PXGraphExtension<CashTransferEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>();
		}

		protected virtual void _(Events.FieldVerifying<CATransfer, CATransfer.outAccountID> e)
		{
			if (e.NewValue == null)
				return;

			CashAccount inAccount = PXSelectorAttribute.Select<CATransfer.inAccountID>(e.Cache, e.Row) as CashAccount;
			CashAccount outAccount = PXSelectorAttribute.Select<CATransfer.outAccountID>(e.Cache, e.Row, e.NewValue) as CashAccount;
			if (e.Row.InAccountID != null 
				&& inAccount.BaseCuryID != outAccount.BaseCuryID)
			{
				e.NewValue = ((CashAccount)PXSelectorAttribute.Select<CATransfer.outAccountID>(e.Cache, e.Row, e.NewValue)).CashAccountCD;
				throw new PXSetPropertyException(Messages.CashAccountDifferentBaseCury, 
					PXAccess.GetBranchCD(inAccount.BranchID), 
					inAccount.CashAccountCD,
					PXAccess.GetBranchCD(outAccount.BranchID), 
					outAccount.CashAccountCD);
			}
		}

		protected virtual void _(Events.FieldVerifying<CATransfer, CATransfer.inAccountID> e)
		{
			if (e.NewValue == null)
				return;

			CashAccount inAccount = PXSelectorAttribute.Select<CATransfer.inAccountID>(e.Cache, e.Row, e.NewValue) as CashAccount;
			CashAccount outAccount = PXSelectorAttribute.Select<CATransfer.outAccountID>(e.Cache, e.Row) as CashAccount;
			if (e.Row.OutAccountID != null
				&& inAccount.BaseCuryID != outAccount.BaseCuryID)
			{
				e.NewValue = ((CashAccount)PXSelectorAttribute.Select<CATransfer.outAccountID>(e.Cache, e.Row, e.NewValue)).CashAccountCD;
				throw new PXSetPropertyException(Messages.CashAccountDifferentBaseCury,
					PXAccess.GetBranchCD(inAccount.BranchID),
					inAccount.CashAccountCD,
					PXAccess.GetBranchCD(outAccount.BranchID),
					outAccount.CashAccountCD);
			}
		}

		protected virtual void VerifyBaseCuryWithExpenseAccounts()
		{
			foreach (CAExpense expense in Base.Expenses.Select())
			{
				CashAccount expenseAccount = PXSelectorAttribute.Select<CAExpense.cashAccountID>(Base.Expenses.Cache, expense) as CashAccount;
				CashAccount outAccount = PXSelectorAttribute.Select<CATransfer.outAccountID>(Base.Transfer.Cache, Base.Transfer.Current) as CashAccount;

				string existingError = PXUIFieldAttribute.GetErrorOnly<CAExpense.cashAccountID>(Base.Expenses.Cache, expense);
				if (string.IsNullOrEmpty(existingError))
				{
					if (expenseAccount != null && outAccount != null && expenseAccount.BaseCuryID != outAccount.BaseCuryID)
					{
						Base.Expenses.Cache.RaiseExceptionHandling<CAExpense.cashAccountID>(expense, expenseAccount.CashAccountCD, new PXSetPropertyException(
							Messages.CashAccountDifferentBaseCury,
							PXErrorLevel.Error,
							PXAccess.GetBranchCD(expenseAccount.BranchID), expenseAccount.CashAccountCD, PXAccess.GetBranchCD(outAccount.BranchID), outAccount.CashAccountCD));
					}
					else
					{
						Base.Expenses.Cache.RaiseExceptionHandling<CAExpense.cashAccountID>(expense, expenseAccount.CashAccountCD, null);
					}
				}
			}
		}

		protected virtual void _(Events.RowPersisting<CATransfer> e)
		{
			VerifyBaseCuryWithExpenseAccounts();
		}

		protected virtual void CATransfer_OutAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			VerifyBaseCuryWithExpenseAccounts();
		}
	}
}

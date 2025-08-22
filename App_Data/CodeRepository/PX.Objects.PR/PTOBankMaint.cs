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

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.PR
{
	public class PTOBankMaint : PXGraph<PTOBankMaint, PRPTOBank>
	{
		private const int _NonLeapYear = 1900;
		private const string _All = "<ALL>";
		private const string _None = "<NONE>";

		#region Views
		public SelectFrom<PRPTOBank>.View Bank;
		public SelectFrom<PRPTOBank>.Where<PRPTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>>.View CurrentBank;
		public SelectFrom<PRPaymentPTOBank>
			.InnerJoin<PRPayment>.On<PRPayment.refNbr.IsEqual<PRPaymentPTOBank.refNbr>
				.And<PRPayment.docType.IsEqual<PRPaymentPTOBank.docType>>>
			.LeftJoin<PRPTODetail>.On<PRPTODetail.paymentDocType.IsEqual<PRPayment.docType>
					.And<PRPTODetail.paymentRefNbr.IsEqual<PRPayment.refNbr>>
					.And<PRPTODetail.bankID.IsEqual<PRPaymentPTOBank.bankID>>>
			.Where<PRPayment.paid.IsEqual<False>
				.And<PRPayment.released.IsEqual<False>>
				.And<PRPayment.docType.IsNotEqual<PayrollType.voidCheck>>
				.And<PRPaymentPTOBank.bankID.IsEqual<P.AsString>>>.View EditablePaymentPTOBanks;

		// These views are necessary to persist PRPTODetail and PRPayment records when saving
		public SelectFrom<PRPTODetail>.View DummyPTODetailView;
		public SelectFrom<PRPayment>.View DummyPaymentView;

		public SelectFrom<PREmployeeClassPTOBank>
			.Where<PREmployeeClassPTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>>
			.OrderBy<PREmployeeClassPTOBank.employeeClassID.Asc, PREmployeeClassPTOBank.startDate.Asc>.View EmployeeClassPTOBanks;

		public SelectFrom<PREmployeeClassPTOBank>
			.Where<PREmployeeClassPTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
				.And<PREmployeeClassPTOBank.unassigned.IsEqual<False>>>.View EmployeeClassWithSameID;

		public SelectFrom<PREmployeePTOBank>
			.InnerJoin<PREmployee>.On<PREmployee.bAccountID.IsEqual<PREmployeePTOBank.bAccountID>>
			.Where<PREmployee.useCustomSettings.IsEqual<False>
				.And<PREmployeePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>>
				.And<PREmployeePTOBank.startDate.IsEqual<P.AsDateTime.UTC>>>.View EmployeePTOBankWithSameKey;

		public SelectFrom<PRPaymentPTOBank>
			.InnerJoin<PRPayment>.On<PRPaymentPTOBank.FK.Payment>
			.InnerJoin<PREmployee>.On<PREmployee.bAccountID.IsEqual<PRPayment.employeeID>>
			.Where<PRPaymentPTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
				.And<PRPayment.docType.IsNotEqual<PayrollType.voidCheck>>
				.And<PRPayment.status.IsNotEqual<PaymentStatus.voided>>
				.And<PREmployee.employeeClassID.IsEqual<P.AsString>>>.View NonVoidedPaychecksUsingCurrentPTOBankWithSameClassID;

        public SelectFrom<PRBandingRulePTOBank>
            .Where<PRBandingRulePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>>
            .OrderBy<PRBandingRulePTOBank.employeeClassID.Asc, PRBandingRulePTOBank.yearsOfService.Asc>.View BandingRulePTOBanks;

        public PXFilter<UpdateBandingRuleWarning> BandingRulePopupView;

		public SelectFrom<PREmployee>
			.Where<PREmployee.useCustomSettings.IsEqual<False>>.View EmployeesNotUsingCustomSettings;

		public SelectFrom<PREmployeePTOBank>.View EmployeePTOBanks;

		public SelectFrom<PREmployeePTOBank>
			.Where<PREmployeePTOBank.bAccountID.IsEqual<P.AsInt>
				.And<PREmployeePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>>>.View EmployeePTOBanksWithSameBankID;
		#endregion

		#region Events
		protected virtual void _(Events.FieldUpdated<PRPTOBank, PRPTOBank.isActive> e)
		{
			PRPTOBank row = e.Row;
			if (row == null)
			{
				return;
			}

			if (!e.NewValue.Equals(true))
			{
				PXCache paymentCache = this.Caches<PRPayment>();
				PXCache paymentPTOBankCache = this.Caches<PRPaymentPTOBank>();
				PXCache ptoDetailCache = this.Caches<PRPTODetail>();
				foreach (PXResult<PRPaymentPTOBank, PRPayment, PRPTODetail> result in EditablePaymentPTOBanks.Select(row.BankID))
				{
					PRPayment payment = result;
					PRPaymentPTOBank paymentPTOBank = result;
					PRPTODetail ptoDetail = result;

					paymentPTOBank.IsActive = false;
					paymentPTOBank.AccrualAmount = 0m;
					paymentPTOBank.AccrualMoney = 0m;
					paymentPTOBankCache.Update(paymentPTOBank);

					ptoDetailCache.Delete(ptoDetail);

					payment.Calculated = false;
					paymentCache.Update(payment);
				}
			}
		}

		protected virtual void _(Events.FieldUpdating<PRPTOBank, PRPTOBank.createFinancialTransaction> e)
		{
			PRPTOBank row = e.Row;
			if (row == null || e.OldValue == null || e.NewValue.Equals(e.OldValue))
			{
				return;
			}

			if (BankHasBeenUsed(row.BankID))
			{
				throw new PXSetPropertyException<PRPTOBank.createFinancialTransaction>(Messages.PTOBankInUse);
			}
		}

		protected virtual void _(Events.FieldUpdated<PRPTOBank, PRPTOBank.createFinancialTransaction> e)
		{
			PRPTOBank row = e.Row;
			if (row == null)
			{
				return;
			}

			foreach (PXResult<PRPaymentPTOBank, PRPayment> result in EditablePaymentPTOBanks.Select(row.BankID))
			{
				PRPayment payment = result;
				payment.Calculated = false;
				Caches[typeof(PRPayment)].Update(payment);
			}
		}

		protected virtual void _(Events.FieldUpdating<PRPTOBank, PRPTOBank.startDateMonth> e)
		{
			if(e.NewValue == null)
			{
				e.NewValue = e.OldValue;
				e.Cancel = true;
			}
		}

		protected virtual void _(Events.RowPersisting<PRPTOBank> e)
		{
			PRPTOBank row = e.Row;
			if (row == null || e.Cache.GetStatus(e.Row) == PXEntryStatus.Deleted)
			{
				return;
			}

			if (row.ApplyBandingRules == true)
			{
				if (!EmployeeClassPTOBanks.Select().Any())
				{
					throw new PXException(Messages.PTOBankHasNoEmployeeClassSettings);
				}
			}
			else
			{
				if (BandingRulePTOBanks.Select().Any())
				{
					var answer = BandingRulePopupView.Ask(Messages.Warning,
						Messages.DisablingBandingRuleWarning, MessageButtons.YesNoCancel, MessageIcon.Warning);

					switch (answer)
					{
						case WebDialogResult.Yes:
							BandingRulePTOBanks.Select().ForEach(x => BandingRulePTOBanks.Delete(x));
							break;
						case WebDialogResult.No:
							e.Cache.SetValue<PRPTOBank.applyBandingRules>(e.Row, true);
							break;
						case WebDialogResult.Cancel:
							e.Cancel = true;
							break;
						default:
							break;
					}
				}
			}
		}

		protected virtual void _(Events.RowDeleting<PRPTOBank> e)
		{
			if (e.Row != null)
			{
				bool employeePayrollSettingsUsingCurrentPTOBank = SelectFrom<PREmployeePTOBank>
					.InnerJoin<PRPTOBank>.On<PREmployeePTOBank.FK.PTOBank>
						.Where<PREmployeePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>>.View.Select(this).Any();

				if (employeePayrollSettingsUsingCurrentPTOBank)
				{
					throw new PXException(Messages.ProhibitPTOBankDeletion);
				}
			}
		}

		protected virtual void _(Events.RowUpdating<PRPTOBank> e)
		{
			PRPTOBank newRow = e.NewRow;

			EnsureExclusiveNegativeBalanceAndDisburseCarryOver(newRow);

			if (newRow == null || IsContractBasedAPI || newRow?.StartDateMonth == null || newRow?.StartDateDay == null)
			{
				return;
			}

			try
			{
				var newDate = new DateTime(_NonLeapYear, newRow.StartDateMonth.Value, newRow.StartDateDay.Value);
				Bank.SetValueExt<PRPTOBank.startDate>(Bank.Current, newDate);
			}
			catch (ArgumentOutOfRangeException)
			{
				var errorMessage = PXMessages.LocalizeFormat(Messages.InvalidStartDate, nameof(PRPTOBank.StartDate));
				PXUIFieldAttribute.SetWarning<PRPTOBank.startDate>(Bank.Cache, Bank.Current, errorMessage);
			}
		}

		public virtual void _(Events.FieldVerifying<PRPTOBank, PRPTOBank.bandingRuleRoundingMethod> e)
		{
			PRPTOBank row = e.Row;

			if (row == null || e.OldValue != null && e.OldValue == e.NewValue)
			{
				return;
			}

			if (EmployeePayrollSettingsUsingCurrentPTOBank())
			{
				throw new PXSetPropertyException(Messages.BandingRuleRoundingCannotBeChanged);
			}
		}

		protected virtual void _(Events.FieldVerifying<PREmployeeClassPTOBank, PREmployeeClassPTOBank.frontLoadingAmount> e)
		{
			PREmployeeClassPTOBank row = e.Row;

			if (row == null || (e.OldValue != null && (decimal?)e.OldValue == (decimal?)e.NewValue))
			{
				return;
			}

			ValidateFrontLoading((decimal?)e.OldValue, (decimal?)e.NewValue, row.AccrualLimit);
		}

		protected virtual void _(Events.FieldVerifying<PREmployeeClassPTOBank, PREmployeeClassPTOBank.accrualLimit> e)
		{
			PREmployeeClassPTOBank row = e.Row;

			if (row == null || (e.OldValue != null && (decimal?)e.OldValue == (decimal?)e.NewValue))
			{
				return;
			}

			ValidateAccrualLimit((decimal?)e.OldValue, (decimal?)e.NewValue, row.FrontLoadingAmount);
		}

		protected virtual void _(Events.FieldSelecting<PREmployeeClassPTOBank, PREmployeeClassPTOBank.employeeClassID> e)
		{
			PREmployeeClassPTOBank row = e.Row;

			if (row == null)
			{
				return;
			}

			if (row.EmployeeClassID == null)
			{
				if (row.Unassigned == true)
				{
					e.ReturnValue = _None;
				}
				else
				{
					e.ReturnValue = _All;
				}
			}
		}

		protected virtual void _(Events.FieldUpdated<PREmployeeClassPTOBank, PREmployeeClassPTOBank.allowNegativeBalance> e)
		{
			var row = e.Row;
			if (row == null || e.NewValue.Equals(false))
			{
				return;
			}

			ClearDisburseFromCarryOverSelection(row, e.Cache);
		}

		protected virtual void _(Events.FieldUpdated<PREmployeeClassPTOBank, PREmployeeClassPTOBank.disburseFromCarryover> e)
		{
			var row = e.Row;
			if (row == null || e.NewValue.Equals(false))
			{
				return;
			}

			ClearAllowNegativeBalanceSelection(row, e.Cache);
		}

		protected virtual void _(Events.RowSelected<PREmployeeClassPTOBank> e)
		{
			if (e.Row == null)
			{
				return;
			}

			if(e.Row.Unassigned == true)
			{
				PXUIFieldAttribute.SetEnabled(e.Cache, e.Row, false);
			}
		}

		protected virtual void _(Events.RowPersisting<PREmployeeClassPTOBank> e)
		{
			PREmployeeClassPTOBank row = e.Row;
			if (row == null || row.StartDate == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted)
			{
				return;
			}

			PreventCreationOfEmployeeClassDuplicate(row, e.Cache);
		}

		protected virtual void _(Events.RowUpdating<PREmployeeClassPTOBank> e)
		{
			PREmployeeClassPTOBank newRow = e.NewRow;
			PREmployeeClassPTOBank oldRow = e.Row;

			if (newRow == null)
			{
				return;
			}

			if (!e.Cache.ObjectsEqual<PREmployeeClassPTOBank.startDate>(newRow, oldRow))
			{
				PXResult<PRPaymentPTOBank, PRPayment, PREmployee> nonVoidedPaychecksUsingCurrentPTOBank = NonVoidedPaychecksUsingCurrentPTOBankWithSameClassID.Select(newRow.EmployeeClassID)
				.Select(x => (PXResult<PRPaymentPTOBank, PRPayment, PREmployee>)x)
					.Where(x => ((PRPayment)x).EndDate.Value.Date >= newRow.StartDate)
						.OrderBy(x => ((PRPayment)x).TransactionDate).LastOrDefault();

				if (nonVoidedPaychecksUsingCurrentPTOBank != null)
				{
					PRPayment payment = nonVoidedPaychecksUsingCurrentPTOBank;
					e.Cache.RaiseExceptionHandling<PREmployeeClassPTOBank.startDate>(newRow, newRow.StartDate,
						new PXSetPropertyException(PXMessages.LocalizeFormat(Messages.PaychecksCreatedAfterEffectiveDate, payment.EndDate.Value.Date.ToShortDateString())));
					e.Cancel = true;
				}
			}

			EmployeeClassLinkedToBandingRuleNotification(e.Cache, newRow, oldRow);
		}

		protected virtual void _(Events.RowDeleting<PREmployeeClassPTOBank> e)
		{
			if (e.Row != null && e.Cache.GetStatus(e.Row) != PXEntryStatus.InsertedDeleted)
			{
				bool employeeClassSettingIsUsed = EmployeePTOBankWithSameKey
					.Select(e.Row.StartDate).FirstTableItems
					.Where(x => x.EmployeeClassID == e.Row.EmployeeClassID && x.BandingRule == null)
					.Any();

				if (employeeClassSettingIsUsed)
				{
					throw new PXException(Messages.CannotModifyOrDeleteEmployeeClassSettings);
				}

				PreventDeletionOfEmployeeClassLinkedToBandingRules(e.Row);
			}
		}

		protected virtual void _(Events.FieldVerifying<PRBandingRulePTOBank, PRBandingRulePTOBank.frontLoadingAmount> e)
		{
			PRBandingRulePTOBank row = e.Row;

			if (row == null || (e.OldValue != null && (decimal?)e.OldValue == (decimal?)e.NewValue))
			{
				return;
			}

			ValidateFrontLoading((decimal?)e.OldValue, (decimal?)e.NewValue, row.AccrualLimit);
		}

		protected virtual void _(Events.FieldVerifying<PRBandingRulePTOBank, PRBandingRulePTOBank.accrualLimit> e)
		{
			PRBandingRulePTOBank row = e.Row;

			if (row == null || (e.OldValue != null && (decimal?)e.OldValue == (decimal?)e.NewValue))
			{
				return;
			}

			ValidateAccrualLimit((decimal?)e.OldValue, (decimal?)e.NewValue, row.FrontLoadingAmount);
		}

		protected virtual void _(Events.FieldSelecting<PRBandingRulePTOBank, PRBandingRulePTOBank.employeeClassID> e)
		{
			PRBandingRulePTOBank row = e.Row;

			if (row == null)
			{
				return;
			}

			if (row.EmployeeClassID == null)
			{
				e.ReturnValue = _All;
			}
		}

		protected virtual void _(Events.FieldUpdating<PRBandingRulePTOBank, PRBandingRulePTOBank.yearsOfService> e)
		{
			PRBandingRulePTOBank row = e.Row;
			if (row == null || (e.NewValue == null && e.OldValue == null))
			{
				return;
			}

			if (!e.NewValue.Equals(e.OldValue) && (int?)e.NewValue < 1 || (int?)e.NewValue > 99)
			{
				e.NewValue = e.OldValue;
			}
		}

		protected virtual void _(Events.RowDeleting<PRBandingRulePTOBank> e)
		{
			if (e.Row != null && e.Cache.GetStatus(e.Row) != PXEntryStatus.InsertedDeleted)
			{
				if (EmployeePayrollSettingsUsingCurrentPTOBank(e.Row.EmployeeClassID))
				{
					throw new PXException(Messages.CannotModifyBandingRules);
				}
			}
		}

		protected virtual void _(Events.RowPersisting<PRBandingRulePTOBank> e)
		{
			PRBandingRulePTOBank row = e.Row;
			if (row == null)
			{
				return;
			}

			PreventCreationOfBandingRuleDuplicate(row, e.Cache);
			PreventCreationOfBandingRuleWithUndefinedEmployeeClass(row, e.Cache);
		}

		public override void Persist()
		{
			List<PRBandingRulePTOBank> bandingRules =
				new List<PRBandingRulePTOBank>((Caches[typeof(PRBandingRulePTOBank)].Inserted as IEnumerable<PRBandingRulePTOBank>)
				.OrderByDescending(x => x.EmployeeClassID).ThenByDescending(x => x.YearsOfService).ToList());
			List<PREmployeeClassPTOBank> classBanks =
				new List<PREmployeeClassPTOBank>((Caches[typeof(PREmployeeClassPTOBank)].Inserted as IEnumerable<PREmployeeClassPTOBank>)
				.OrderByDescending(x => x.EmployeeClassID).ThenByDescending(x => x.StartDate).ToList());

			base.Persist();

			foreach (PRBandingRulePTOBank bandingRule in bandingRules)
			{
				InsertBandingRuleWithEmployeeBank(bandingRule);
			}

			foreach (PREmployeeClassPTOBank classBank in classBanks)
			{
				InsertClassBankWithEmployeeBank(classBank);
			}

			EmployeePTOBanks.Cache.Persist(PXDBOperation.Update);
			EmployeePTOBanks.Cache.Clear();
		}
		#endregion

		#region Helpers
		protected virtual bool BankHasBeenUsed(string bankID)
		{
			return SelectFrom<PRPaymentPTOBank>
				.InnerJoin<PRPayment>.On<PRPaymentPTOBank.FK.Payment>
				.Where<PRPaymentPTOBank.bankID.IsEqual<P.AsString>
					.And<PRPayment.paid.IsEqual<True>
						.Or<PRPayment.released.IsEqual<True>>>
					.And<PRPayment.docType.IsNotEqual<PayrollType.voidCheck>>
					.And<PRPayment.status.IsNotEqual<PaymentStatus.voided>>>.View.Select(this, bankID).Any();
		}

		public virtual bool EmployeePayrollSettingsUsingCurrentPTOBank(string employeeClassID = null)
		{
			if (employeeClassID != null)
			{
				return SelectFrom<PREmployeePTOBank>
				.InnerJoin<PRPTOBank>.On<PREmployeePTOBank.FK.PTOBank>
				.InnerJoin<PREmployee>.On<PREmployeePTOBank.FK.Employee>
					.Where<PREmployeePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
						.And<PREmployeePTOBank.employeeClassID.IsEqual<P.AsString>>
						.And<PREmployee.useCustomSettings.IsEqual<False>>>.View.Select(this, employeeClassID).Any();
			}

			return SelectFrom<PREmployeePTOBank>
				.InnerJoin<PRPTOBank>.On<PREmployeePTOBank.FK.PTOBank>
				.InnerJoin<PREmployee>.On<PREmployeePTOBank.FK.Employee>
					.Where<PREmployeePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
						.And<PREmployeePTOBank.employeeClassID.IsNull>
						.And<PREmployee.useCustomSettings.IsEqual<False>>>.View.Select(this).Any();
		}

		protected virtual void InsertClassBank(PREmployee employee, PRPTOBank bank, PREmployeeClassPTOBank classBank)
		{
			bool employeeHasNonGenericPTOBanks = EmployeePTOBanksWithSameBankID
				.Select(employee.BAccountID).FirstTableItems
				.Where(x => x.EmployeeClassID != null)
				.Any();

			PREmployeePTOBank activeEmployeeBank = EmployeePTOBanksWithSameBankID.Select(employee.BAccountID).FirstTableItems
				.Where(x => x.StartDate <= Accessinfo.BusinessDate).OrderByDescending(x => x.StartDate).FirstOrDefault();

			var currentActiveClassBank = EmployeeClassPTOBanks.Select().FirstTableItems
				.Where(x => x.EmployeeClassID == classBank.EmployeeClassID && x.StartDate <= Accessinfo.BusinessDate).OrderByDescending(x => x.StartDate).FirstOrDefault();

			if ((activeEmployeeBank == null || (activeEmployeeBank.EmployeeClassID == null || classBank.EmployeeClassID != null) && activeEmployeeBank.BandingRule == null &&
				(classBank.StartDate > activeEmployeeBank.StartDate)) &&
				(classBank.EmployeeClassID != null || !employeeHasNonGenericPTOBanks))
			{
				if (bank.ApplyBandingRules == false || currentActiveClassBank == null || (currentActiveClassBank.StartDate == classBank.StartDate))
				{
					PREmployeePTOBank newBank = new PREmployeePTOBank();
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.bAccountID>(newBank, employee.BAccountID);
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.isActive>(newBank, classBank.IsActive);
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.bankID>(newBank, classBank.BankID);
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.employeeClassID>(newBank, classBank.EmployeeClassID);
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.startDate>(newBank, classBank.StartDate);
					UpdateEmployeePTOBankNonAmountRuleFields(bank, newBank, classBank);
					UpdateEmployeePTOBankAmountFieldsFromClassSettings(newBank, classBank);
					EmployeePTOBanks.Insert(newBank);
				}
			}
		}

		protected virtual void InsertBandingRule(PREmployee employee, PRPTOBank bank, PREmployeeClassPTOBank classBank, PRBandingRulePTOBank bandingRule)
		{
			bool employeeHasNonGenericBandingRules = EmployeePTOBanksWithSameBankID
				.Select(employee.BAccountID).FirstTableItems
				.Where(x => x.EmployeeClassID != null && x.BandingRule != null)
				.Any();

			PREmployeePTOBank activeEmployeeBank = EmployeePTOBanksWithSameBankID.Select(employee.BAccountID).FirstTableItems
				.Where(x => x.StartDate <= Accessinfo.BusinessDate).OrderByDescending(x => x.StartDate).FirstOrDefault();

			if (!employeeHasNonGenericBandingRules || bandingRule.EmployeeClassID != null)
			{
				DateTime? hireDate = EmploymentHistoryHelper.GetEmploymentDates(this, employee.BAccountID, Accessinfo.BusinessDate).ContinuousHireDate;
				DateTime transferDate = PTOHelper.CalculatePTOTransferDate(bank, hireDate);
				DateTime effectiveDate = PTOHelper.GetPTOEffectiveDate(hireDate.Value, transferDate, bandingRule.YearsOfService.Value, bank.BandingRuleRoundingMethod);

				if (activeEmployeeBank == null || activeEmployeeBank.BandingRule == null || effectiveDate > activeEmployeeBank.StartDate)
				{
					PREmployeePTOBank newBank = new PREmployeePTOBank();
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.bAccountID>(newBank, employee.BAccountID);
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.bankID>(newBank, bandingRule.BankID);
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.employeeClassID>(newBank, bandingRule.EmployeeClassID);
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.transferDate>(newBank, transferDate);
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.bandingRule>(newBank, bandingRule.YearsOfService);
					EmployeePTOBanks.SetValueExt<PREmployeePTOBank.startDate>(newBank, effectiveDate);
					UpdateEmployeePTOBankNonAmountRuleFields(bank, newBank, classBank);
					UpdateEmployeePTOBankAmountFieldsFromBandingRule(newBank, bandingRule);
					newBank = EmployeePTOBanks.Insert(newBank);
				}
			}
		}

		protected virtual void UpdateEmployeePTOBankNonAmountRuleFields(PRPTOBank bank, PREmployeePTOBank employeeBank, PREmployeeClassPTOBank classBank)
		{
			employeeBank.AccrualMethod = bank.AccrualMethod;
			employeeBank.CarryoverType = bank.CarryoverType;
			employeeBank.AllowNegativeBalance = classBank.AllowNegativeBalance;
			employeeBank.ProbationPeriodBehaviour = classBank.ProbationPeriodBehaviour;
			employeeBank.SettlementBalanceType = bank.SettlementBalanceType;
			employeeBank.DisburseFromCarryover = classBank.DisburseFromCarryover;
		}

		protected virtual void UpdateEmployeePTOBankAmountFieldsFromClassSettings(PREmployeePTOBank employeeBank, PREmployeeClassPTOBank classBank)
		{
			employeeBank.AccrualRate = classBank.AccrualRate;
			employeeBank.HoursPerYear = classBank.HoursPerYear;
			employeeBank.AccrualLimit = classBank.AccrualLimit;
			employeeBank.CarryoverAmount = classBank.CarryoverAmount;
			employeeBank.FrontLoadingAmount = classBank.FrontLoadingAmount;
		}

		protected virtual void UpdateEmployeePTOBankAmountFieldsFromBandingRule(PREmployeePTOBank employeeBank, PRBandingRulePTOBank bandingRule)
		{
			employeeBank.AccrualRate = bandingRule.AccrualRate;
			employeeBank.HoursPerYear = bandingRule.HoursPerYear;
			employeeBank.AccrualLimit = bandingRule.AccrualLimit;
			employeeBank.CarryoverAmount = bandingRule.CarryoverAmount;
			employeeBank.FrontLoadingAmount = bandingRule.FrontLoadingAmount;
		}

		protected virtual void ClearDisburseFromCarryOverSelection(PREmployeeClassPTOBank row, PXCache cache)
		{
			if (row.DisburseFromCarryover == true)
			{
				row.DisburseFromCarryover = false;
				PXUIFieldAttribute.SetWarning<PREmployeeClassPTOBank.disburseFromCarryover>(cache, row, Messages.CantUseSimultaneously);
			}
		}

		protected virtual void ClearAllowNegativeBalanceSelection(PREmployeeClassPTOBank row, PXCache cache)
		{
			if (row.AllowNegativeBalance == true)
			{
				row.AllowNegativeBalance = false;
				PXUIFieldAttribute.SetWarning<PREmployeeClassPTOBank.allowNegativeBalance>(cache, row, Messages.CantUseSimultaneously);
			}
		}

		protected virtual void PreventCreationOfBandingRuleDuplicate(PRBandingRulePTOBank bandingRule, PXCache cache)
		{
			bool bandingRulesDuplicate = SelectFrom<PRBandingRulePTOBank>
			.Where<PRBandingRulePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
				.And<PRBandingRulePTOBank.yearsOfService.IsEqual<P.AsInt>>
				.And<PRBandingRulePTOBank.recordID.IsNotEqual<P.AsInt>>>.View
				.Select(this, bandingRule.YearsOfService, bandingRule.RecordID)
				.Where(x => ((PRBandingRulePTOBank)x).EmployeeClassID == bandingRule.EmployeeClassID).Any();

			if (bandingRulesDuplicate)
			{
				cache.RaiseExceptionHandling<PRBandingRulePTOBank.employeeClassID>(bandingRule, bandingRule.EmployeeClassID,
						new PXSetPropertyException(Messages.BandingRulesDuplicate, PXErrorLevel.RowError));
			}
		}

		protected virtual void PreventCreationOfBandingRuleWithUndefinedEmployeeClass(PRBandingRulePTOBank bandingRule, PXCache cache)
		{
			bool isEmployeeClassAlreadyDefined;

			if (bandingRule.EmployeeClassID != null)
			{
				 isEmployeeClassAlreadyDefined = SelectFrom<PREmployeeClassPTOBank>
				.Where<PREmployeeClassPTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
					.And<PREmployeeClassPTOBank.employeeClassID.IsEqual<P.AsString>>>.View
					.Select(this, bandingRule.EmployeeClassID).ToList().Any();
			}
			else
			{
				 isEmployeeClassAlreadyDefined = SelectFrom<PREmployeeClassPTOBank>
					.Where<PREmployeeClassPTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
						.And<PREmployeeClassPTOBank.employeeClassID.IsNull>
						.And<PREmployeeClassPTOBank.unassigned.IsEqual<False>>>.View.Select(this).Any();
			}

			if (!isEmployeeClassAlreadyDefined)
			{
				cache.RaiseExceptionHandling<PRBandingRulePTOBank.employeeClassID>(bandingRule, bandingRule.EmployeeClassID,
					new PXSetPropertyException(Messages.EmployeeClassNotDefinedInEmployeeClassSettings, PXErrorLevel.RowError));
			}
		}

		protected virtual void EmployeeClassLinkedToBandingRuleNotification(PXCache cache, PREmployeeClassPTOBank newRow, PREmployeeClassPTOBank oldRow)
		{
			bool employeeClassIsLinkedToABandingRule;

			if (newRow.EmployeeClassID == null)
			{
				// for <ALL> employee class
				employeeClassIsLinkedToABandingRule = SelectFrom<PRBandingRulePTOBank>
					.Where<PRBandingRulePTOBank.bankID.IsEqual<P.AsString>>.View
					.Select(this, newRow.BankID).ToList().Any();
			}
			else
			{
				// for specific employee class
				employeeClassIsLinkedToABandingRule = SelectFrom<PRBandingRulePTOBank>
					.Where<PRBandingRulePTOBank.bankID.IsEqual<P.AsString>
						.And<PRBandingRulePTOBank.employeeClassID.IsEqual<P.AsString>
							.Or<PRBandingRulePTOBank.employeeClassID.IsNull>>>.View
						.Select(this, newRow.BankID, newRow.EmployeeClassID).ToList().Any();
			}

			if (CurrentBank.Current.ApplyBandingRules == true && employeeClassIsLinkedToABandingRule &&
				!cache.ObjectsEqual<PREmployeeClassPTOBank.hoursPerYear, PREmployeeClassPTOBank.accrualLimit,
				PREmployeeClassPTOBank.accrualRate, PREmployeeClassPTOBank.carryoverAmount,
				PREmployeeClassPTOBank.frontLoadingAmount>(newRow, oldRow))
			{
				cache.RaiseExceptionHandling<PRBandingRulePTOBank.employeeClassID>(newRow, newRow.EmployeeClassID,
					new PXSetPropertyException(Messages.EmployeeClassSettingsChanged, PXErrorLevel.RowWarning));
			}
		}

		protected virtual void PreventCreationOfEmployeeClassDuplicate(PREmployeeClassPTOBank employeeClassPTOBank, PXCache cache)
		{
			bool employeeClassDuplicate = SelectFrom<PREmployeeClassPTOBank>
				.Where<PREmployeeClassPTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
					.And<PREmployeeClassPTOBank.recordID.IsNotEqual<P.AsInt>>
					.And<PREmployeeClassPTOBank.unassigned.IsEqual<False>>>
				.View.Select(this, employeeClassPTOBank.RecordID)
				.Where(x => ((PREmployeeClassPTOBank)x).EmployeeClassID == employeeClassPTOBank.EmployeeClassID).Any();

			if (employeeClassDuplicate)
			{
				cache.RaiseExceptionHandling<PREmployeeClassPTOBank.employeeClassID>(employeeClassPTOBank, employeeClassPTOBank.EmployeeClassID,
						new PXSetPropertyException(Messages.EmployeeClassDuplicate, PXErrorLevel.RowError));
			}
		}

		protected virtual void PreventDeletionOfEmployeeClassLinkedToBandingRules(PREmployeeClassPTOBank employeeClassPTOBank)
		{
			PRPTOBank bank = CurrentBank.Current;

			// Prevent triggering the validation if the parent PTO bank is being deleted
			if (CurrentBank.Cache.GetStatus(bank) != PXEntryStatus.Deleted)
			{
				if (employeeClassPTOBank.Unassigned == false)
				{
					// We disallow deleting an employee class if:
					// A banding rule exists with the same employee Class ID
					// or a banding rule exists with the <ALL> employee class ID
					bool bandingRulesLinkedToEmployeeClass = SelectFrom<PRBandingRulePTOBank>
					.Where<PRBandingRulePTOBank.bankID.IsEqual<PRPTOBank.bankID.FromCurrent>
						.And<PRBandingRulePTOBank.employeeClassID.IsEqual<P.AsString>
							.Or<PRBandingRulePTOBank.employeeClassID.IsNull>>>.View.Select(this, employeeClassPTOBank.EmployeeClassID).Any();

					if (bandingRulesLinkedToEmployeeClass)
					{
						throw new PXException(Messages.EmployeeClassCannotBeDeleted);
					}
				}
			}
		}

		public virtual void InsertClassBankWithEmployeeBank(PREmployeeClassPTOBank classBank)
		{
			if (classBank == null || classBank.BankID == null || classBank.StartDate == null || classBank.Unassigned == true)
			{
				return;
			}

			PRPTOBank bank = CurrentBank.Current;

			if (bank == null)
			{
				return;
			}

			IEnumerable<PREmployee> employees = EmployeesNotUsingCustomSettings.Select().FirstTableItems;

			if (classBank.EmployeeClassID != null)
			{
				employees = employees.Where(x => x.EmployeeClassID == classBank.EmployeeClassID);
			}

			foreach (PREmployee employee in employees)
			{
				InsertClassBank(employee, bank, classBank);
			}

			foreach (PREmployeePTOBank newBank in EmployeePTOBanks.Cache.Inserted)
			{
				EmployeePTOBanks.Cache.PersistInserted(newBank);
			}
		}

		public virtual void InsertBandingRuleWithEmployeeBank(PRBandingRulePTOBank bandingRule)
		{
			if (bandingRule == null || bandingRule.BankID == null || bandingRule.YearsOfService == null)
			{
				return;
			}

			PRPTOBank bank = CurrentBank.Current;

			if (bank == null)
			{
				return;
			}

			PREmployeeClassPTOBank classBank = EmployeeClassPTOBanks.Select().FirstTableItems
				.Where(x => x.EmployeeClassID == bandingRule.EmployeeClassID && x.StartDate <= Accessinfo.BusinessDate).OrderByDescending(x => x.StartDate).FirstOrDefault();

			if (classBank == null)
			{
				return;
			}

			IEnumerable<PREmployee> employees = EmployeesNotUsingCustomSettings.Select().FirstTableItems;

			if (bandingRule.EmployeeClassID != null)
			{
				employees = employees.Where(x => x.EmployeeClassID == bandingRule.EmployeeClassID);
			}

			foreach (PREmployee employee in employees)
			{
				InsertBandingRule(employee, bank, classBank, bandingRule);
			}

			foreach (PREmployeePTOBank newBank in EmployeePTOBanks.Cache.Inserted)
			{
				EmployeePTOBanks.Cache.PersistInserted(newBank);
			}
		}

		protected virtual void ValidateFrontLoading(decimal? oldValue, decimal? newValue, decimal? accrualLimit)
		{
			if (oldValue == newValue || accrualLimit == 0)
			{
				return;
			}

			if (newValue > accrualLimit)
			{
				throw new PXSetPropertyException(Messages.FrontLoadingGreaterThanBalanceLimit);
			}
		}

		protected virtual void ValidateAccrualLimit(decimal? oldValue, decimal? newValue, decimal? frontloadingAmount)
		{
			if (oldValue == newValue || frontloadingAmount == 0 || newValue == 0)
			{
				return;
			}

			if (newValue < frontloadingAmount)
			{
				throw new PXSetPropertyException(Messages.BalanceLimitLowerThanFrontLoading);
			}
		}
		#endregion

		#region API

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		protected virtual void EnsureExclusiveNegativeBalanceAndDisburseCarryOver(PRPTOBank bank)
		{
			if (this.IsContractBasedAPI && bank.AllowNegativeBalance == true && bank.DisburseFromCarryover == true)
			{
				throw new Exception(Messages.CantUseSimultaneously);
			}
		}
		#endregion
	}

	[Serializable]
	[PXHidden]
	public class UpdateBandingRuleWarning : PXBqlTable, IBqlTable
    {
		#region Message
		public abstract class message : BqlString.Field<message> { }
		[PXString]
		public string Message { get; set; }
		#endregion
	}
}

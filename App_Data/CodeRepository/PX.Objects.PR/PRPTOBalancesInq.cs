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
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.PR
{
	public class PRPTOBalancesInq : PXGraph<PRPTOBalancesInq>
	{
		public PXCancel<PTOBalanceFilter> Cancel;
		public PXFilter<PTOBalanceFilter> Filter;

		public SelectFrom<PRPTOBalance>.View PTOBalances;

		public PRPTOBalancesInq()
		{
			PTOBalances.AllowInsert = false;
			PTOBalances.AllowUpdate = false;
			PTOBalances.AllowDelete = false;
		}

		public IEnumerable ptoBalances()
		{
			PTOBalanceFilter filter = Filter.Current;
			if (filter == null || filter.PeriodStartDate == null || filter.PeriodEndDate == null)
			{
				yield break;
			}

			// Dictionary<Tuple<EmployeeID, BankID>, PRPTOBalance>
			Dictionary<Tuple<int, string>, PRPTOBalance> employeePTOBalances = new Dictionary<Tuple<int, string>, PRPTOBalance>();

			var paymentPTOBanksQuery = SelectFrom<PRPaymentPTOBank>
				.InnerJoin<PRPayment>
					.On<PRPaymentPTOBank.FK.Payment>
				.Where<PRPayment.docType.IsNotEqual<PayrollType.voidCheck>
					.And<PRPayment.voided.IsEqual<False>>
					.And<PRPayment.released.IsEqual<True>>
					.And<PRPayment.transactionDate.IsGreaterEqual<PTOBalanceFilter.periodStartDate.FromCurrent>>
					.And<PRPayment.transactionDate.IsLessEqual<PTOBalanceFilter.periodEndDate.FromCurrent>>
					.And<P.AsInt.IsNull
						.Or<PRPayment.employeeID.IsEqual<P.AsInt>>>
					.And<P.AsString.IsNull
						.Or<PRPaymentPTOBank.bankID.IsEqual<P.AsString>>>>
				.View.Select(this, filter.EmployeeID, filter.EmployeeID, filter.BankID, filter.BankID);

			var ptoAdjustmentDetails = SelectFrom<PRPTOAdjustmentDetail>
				.InnerJoin<PRPTOAdjustment>
					.On<PRPTOAdjustmentDetail.FK.PTOAdjustment>
				.LeftJoin<PRPayment>
					.On<PRPTOAdjustmentDetail.FK.Payment>
				.Where<PRPTOAdjustment.type.IsEqual<PTOAdjustmentType.adjustment>
					.And<PRPTOAdjustment.status.IsEqual<PTOAdjustmentStatus.released>>
					.And<PRPTOAdjustment.date.IsGreaterEqual<PTOBalanceFilter.periodStartDate.FromCurrent>>
					.And<PRPTOAdjustment.date.IsLessEqual<PTOBalanceFilter.periodEndDate.FromCurrent>>
					.And<PRPTOAdjustmentDetail.paymentDocType.IsNull
						.And<PRPTOAdjustmentDetail.paymentRefNbr.IsNull>
							.Or<PRPayment.transactionDate.IsGreater<PTOBalanceFilter.periodEndDate.FromCurrent>>
							.Or<PRPayment.released.IsEqual<False>>>
					.And<P.AsInt.IsNull
						.Or<PRPTOAdjustmentDetail.bAccountID.IsEqual<P.AsInt>>>
					.And<P.AsString.IsNull
						.Or<PRPTOAdjustmentDetail.bankID.IsEqual<P.AsString>>>>
				.View
				.Select(this, filter.EmployeeID, filter.EmployeeID, filter.BankID, filter.BankID)
				.FirstTableItems
				.ToArray();

			foreach (PXResult<PRPaymentPTOBank, PRPayment> record in paymentPTOBanksQuery)
			{
				PRPaymentPTOBank paymentPTOBank = record;
				PRPayment payment = record;

				if (payment.EmployeeID == null || string.IsNullOrWhiteSpace(paymentPTOBank.BankID))
				{
					continue;
				}

				PRPTOBalance ptoBalance = GetPTOBalanceRecord(employeePTOBalances, payment.EmployeeID.Value, paymentPTOBank.BankID);

				if (ptoBalance.EffectiveStartDate != null && ptoBalance.EffectiveStartDate > paymentPTOBank.EffectiveStartDate)
				{
					continue;
				}

				if (ptoBalance.LatestPaymentDate != null && ptoBalance.LatestPaymentDate > payment.TransactionDate)
				{
					continue;
				}

				ptoBalance.EffectiveStartDate = paymentPTOBank.EffectiveStartDate;
				ptoBalance.LatestPaymentDate = payment.TransactionDate;

				ptoBalance.TotalHoursAccumulated = paymentPTOBank.AccumulatedAmount;
				ptoBalance.TotalHoursUsed = paymentPTOBank.UsedAmount;
				ptoBalance.TotalHoursAvailable = paymentPTOBank.AvailableAmount;

				if (PXAccess.FeatureInstalled<FeaturesSet.payrollCAN>())
				{
					ptoBalance.AccumulatedAmount = paymentPTOBank.AccumulatedMoney;
					ptoBalance.UsedAmount = paymentPTOBank.UsedMoney;
					ptoBalance.AvailableAmount = paymentPTOBank.AvailableMoney;
				}
			}

			foreach (PRPTOAdjustmentDetail ptoAdjustmentDetail in ptoAdjustmentDetails)
			{
				if (ptoAdjustmentDetail.BAccountID == null || string.IsNullOrWhiteSpace(ptoAdjustmentDetail.BankID))
				{
					continue;
				}

				PRPTOBalance ptoBalance = GetPTOBalanceRecord(employeePTOBalances, ptoAdjustmentDetail.BAccountID.Value, ptoAdjustmentDetail.BankID);
				ptoBalance.TotalHoursAccumulated += ptoAdjustmentDetail.AdjustmentHours;
				ptoBalance.TotalHoursAvailable += ptoAdjustmentDetail.AdjustmentHours;
			}

			foreach (KeyValuePair<Tuple<int, string>, PRPTOBalance> kvp in employeePTOBalances)
			{
				yield return kvp.Value;
			}
		}

		#region CacheAttached

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "PTO Bank Description")]
		protected virtual void _(Events.CacheAttached<PRPTOBank.description> e) { }

		#endregion CacheAttached

		public PXAction<PTOBalanceFilter> ViewEmployeePTODetailsReport;
		[PXUIField(DisplayName = "View Employee PTO Details Report", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual void viewEmployeePTODetailsReport()
		{
			PTOBalanceFilter filter = Filter.Current;
			PRPTOBalance ptoBalances = PTOBalances.Current;

			if (filter == null || filter.PeriodStartDate == null || filter.PeriodEndDate == null ||
				ptoBalances == null || ptoBalances.EmployeeID == null || string.IsNullOrWhiteSpace(ptoBalances.BankID))
			{
				return;
			}

			PREmployee payrollEmployee = PREmployee.PK.Find(this, PTOBalances.Current.EmployeeID);
			CR.BAccount businessAccount = CR.BAccount.PK.Find(this, payrollEmployee.ParentBAccountID);

			var parameters = new Dictionary<string, string>();
			if (!string.IsNullOrWhiteSpace(businessAccount?.AcctCD))
			{
				parameters["OrgBAccountID"] = businessAccount.AcctCD;
			}
			parameters["DSelect"] = "TDATE";
			parameters["DateFrom"] = filter.PeriodStartDate.ToString();
			parameters["DateTo"] = filter.PeriodEndDate.ToString();
			parameters["EmplID"] = payrollEmployee.AcctCD;
			parameters["BankID"] = PTOBalances.Current.BankID.ToString();
			parameters["InclUnrel"] = false.ToString();
			parameters["ShowDet"] = true.ToString();

			throw new PXReportRequiredException(parameters, "PR641050", PXBaseRedirectException.WindowMode.New, Messages.EmployeePTODetailsReport);
		}

		protected virtual PRPTOBalance GetPTOBalanceRecord(Dictionary<Tuple<int, string>, PRPTOBalance> employeePTOBalances, int employeeID, string bankID)
		{
			Tuple<int, string> employeeIDAndBankID = new Tuple<int, string>(employeeID, bankID);

			if (!employeePTOBalances.TryGetValue(employeeIDAndBankID, out PRPTOBalance ptoBalance))
			{
				ptoBalance = new PRPTOBalance
				{
					EmployeeID = employeeID,
					BankID = bankID,
					TotalHoursAccumulated = 0m,
					TotalHoursUsed = 0m,
					TotalHoursAvailable = 0m,
				};

				employeePTOBalances[employeeIDAndBankID] = ptoBalance;
			}

			return ptoBalance;
		}
	}

	/// <summary>
	/// A filter for the Employee PTO Balances screen.
	/// </summary>
	[Serializable]
	[PXHidden]
	public class PTOBalanceFilter : PXBqlTable, IBqlTable
	{
		#region PeriodStartDate
		public abstract class periodStartDate : PX.Data.BQL.BqlDateTime.Field<periodStartDate> { }
		/// <summary>
		/// Period Start Date.
		/// </summary>
		[PXDate]
		[PXUIField(DisplayName = "Start Date", Required = true)]
		public virtual DateTime? PeriodStartDate { get; set; }
		#endregion

		#region PeriodEndDate
		public abstract class periodEndDate : PX.Data.BQL.BqlDateTime.Field<periodEndDate> { }
		/// <summary>
		/// Period End Date.
		/// </summary>
		[PXDate]
		[PXUIField(DisplayName = "End Date", Required = true)]
		public virtual DateTime? PeriodEndDate { get; set; }
		#endregion

		#region EmployeeID
		public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }
		/// <summary>
		/// The unique identifier of the employee business account.
		/// </summary>
		[EmployeeActive]
		public virtual int? EmployeeID { get; set; }
		#endregion

		#region BankID
		public abstract class bankID : PX.Data.BQL.BqlString.Field<bankID> { }
		/// <summary>
		/// The unique identifier of the PTO bank to be used for the paid-time-off calculation.
		/// </summary>
		[PXString]
		[PXUIField(DisplayName = "PTO Bank")]
		[PXSelector(typeof(SearchFor<PRPTOBank.bankID>), DescriptionField = typeof(PRPTOBank.description))]
		public virtual string BankID { get; set; }
		#endregion
	}

	/// <summary>
	/// An unbound DAC to display Employee PTO balances.
	/// </summary>
	[Serializable]
	[PXHidden]
	public class PRPTOBalance : PXBqlTable, IBqlTable
	{
		#region EmployeeID
		public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }
		/// <summary>
		/// The unique identifier of the employee business account.
		/// </summary>
		[PXInt(IsKey = true)]
		[PXUIField(DisplayName = "Employee", Enabled = false)]
		[PXSelector(typeof(SearchFor<PREmployee.bAccountID>), SubstituteKey = typeof(PREmployee.acctCD), DescriptionField = typeof(PREmployee.acctName))]
		public virtual int? EmployeeID { get; set; }
		#endregion

		#region BankID
		public abstract class bankID : PX.Data.BQL.BqlString.Field<bankID> { }
		/// <summary>
		/// The unique identifier of the PTO bank to be used for the paid-time-off calculation.
		/// </summary>
		[PXString(IsKey = true)]
		[PXUIField(DisplayName = "PTO Bank", Enabled = false)]
		[PXSelector(typeof(SearchFor<PRPTOBank.bankID>), DescriptionField = typeof(PRPTOBank.description))]
		public virtual string BankID { get; set; }
		#endregion

		#region EffectiveStartDate
		public abstract class effectiveStartDate : PX.Data.BQL.BqlDateTime.Field<effectiveStartDate> { }
		/// <summary>
		/// The start date when the settings of the PTO bank are applied to the employee.
		/// </summary>
		[PXDBDate]
		[PXUIField(DisplayName = "Effective From", Enabled = false)]
		public virtual DateTime? EffectiveStartDate { get; set; }
		#endregion

		#region LatestPaymentDate
		public abstract class latestPaymentDate : PX.Data.BQL.BqlDateTime.Field<latestPaymentDate> { }
		/// <summary>
		/// Date of the latest payment in the specified period.
		/// </summary>
		[PXDate]
		public virtual DateTime? LatestPaymentDate { get; set; }
		#endregion

		#region TotalHoursAccumulated
		public abstract class earnedHours : PX.Data.BQL.BqlDecimal.Field<earnedHours> { }
		/// <summary>
		/// PTO hours earned during the specified period.
		/// </summary>
		[PXDecimal]
		[PXUIField(DisplayName = "Total Hours Accumulated", Enabled = false)]
		public virtual decimal? TotalHoursAccumulated { get; set; }
		#endregion

		#region TotalHoursUsed
		public abstract class totalHoursUsed : PX.Data.BQL.BqlDecimal.Field<totalHoursUsed> { }
		/// <summary>
		/// PTO hours used during the specified period.
		/// </summary>
		[PXDecimal]
		[PXUIField(DisplayName = "Total Hours Used", Enabled = false)]
		public virtual decimal? TotalHoursUsed { get; set; }
		#endregion

		#region TotalHoursAvailable
		public abstract class totalHoursAvailable : PX.Data.BQL.BqlDecimal.Field<totalHoursAvailable> { }
		/// <summary>
		/// PTO hours available by the end of the specified period.
		/// </summary>
		[PXDecimal]
		[PXUIField(DisplayName = "Total Hours Available", Enabled = false)]
		public virtual decimal? TotalHoursAvailable { get; set; }
		#endregion

		#region AccumulatedAmount
		public abstract class accumulatedAmount : PX.Data.BQL.BqlDecimal.Field<accumulatedAmount> { }
		/// <summary>
		/// The amount of money accrued for the bank.
		/// </summary>
		[PXDecimal]
		[PXUIField(DisplayName = "Accumulated Amount", Enabled = false, Visibility = PXUIVisibility.Visible, Visible = false, FieldClass = nameof(FeaturesSet.PayrollCAN))]
		public virtual decimal? AccumulatedAmount { get; set; }
		#endregion

		#region UsedAmount
		public abstract class usedAmount : PX.Data.BQL.BqlDecimal.Field<usedAmount> { }
		/// <summary>
		/// The amount of money currently used by the employee.
		/// </summary>
		[PXDecimal]
		[PXUIField(DisplayName = "Used Amount", Enabled = false, Visibility = PXUIVisibility.Visible, Visible = false, FieldClass = nameof(FeaturesSet.PayrollCAN))]
		public virtual decimal? UsedAmount { get; set; }
		#endregion

		#region AvailableAmount
		public abstract class availableAmount : PX.Data.BQL.BqlDecimal.Field<availableAmount> { }
		/// <summary>
		/// The amount of money accrued for the employee through all released paychecks.
		/// </summary>
		[PXDecimal]
		[PXUIField(DisplayName = "Available Amount", Enabled = false, Visibility = PXUIVisibility.Visible, Visible = false, FieldClass = nameof(FeaturesSet.PayrollCAN))]
		public virtual decimal? AvailableAmount { get; set; }
		#endregion
	}
}

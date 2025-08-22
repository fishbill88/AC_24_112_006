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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
using System;
using System.Diagnostics;

namespace PX.Objects.PR
{
	/// <summary>
	/// Stores the paid time off information related to the configuration of a specific employee. The information will be displayed on the Employee Payroll Settings (PR203000) form.
	/// </summary>
	[Serializable]
	[PXCacheName(Messages.PREmployeePTOBank)]
	[DebuggerDisplay("{GetType().Name,nq}: BAccountID = {BAccountID,nq}, BankID = {BankID,nq}, StartDate = {StartDate,nq}")]
	public class PREmployeePTOBank : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<PREmployeePTOBank>.By<bAccountID, bankID, startDate>
		{
			public static PREmployeePTOBank Find(PXGraph graph, int? bAccountID, string bankID, DateTime? startDate) =>
				FindBy(graph, bAccountID, bankID, startDate);
		}

		public static class FK
		{
			public class Employee : PREmployee.PK.ForeignKeyOf<PREmployeePTOBank>.By<bAccountID> { }
			public class PTOBank : PRPTOBank.PK.ForeignKeyOf<PREmployeePTOBank>.By<bankID> { }
		}
		#endregion

		#region BAccountID
		/// <summary>
		/// The unique identifier of the business account to which the employee bank belongs.
		/// </summary>
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(PREmployee.bAccountID))]
		[PXParent(typeof(SelectFrom<PREmployee>.Where<PREmployee.bAccountID.IsEqual<bAccountID.FromCurrent>>))]
		public int? BAccountID { get; set; }
		public abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
		#endregion

		#region BankID
		/// <summary>
		/// The unique identifier of the PTO bank to be used for the paid-time-off calculation.
		/// </summary>
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "PTO Bank")]
		[PXUIEnabled(typeof(Where<bankID.IsNull>))]
		[PXSelector(typeof(SearchFor<PRPTOBank.bankID>), DescriptionField = typeof(PRPTOBank.description))]
		[PXRestrictor(typeof(Where<PRPTOBank.isActive.IsEqual<True>>), Messages.InactivePTOBank, typeof(PRPTOBank.bankID))]
		[PXForeignReference(typeof(FK.PTOBank))]
		public virtual string BankID { get; set; }
		public abstract class bankID : PX.Data.BQL.BqlString.Field<bankID> { }
		#endregion

		#region EmployeeClassID
		/// <summary>
		/// The unique identifier of the employee class from which the employee bank gets some of its settings' values.
		/// </summary>
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Class ID")]
		public virtual string EmployeeClassID { get; set; }
		public abstract class employeeClassID : PX.Data.BQL.BqlString.Field<employeeClassID> { }
		#endregion

		#region IsActive
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that the employee bank is active.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Active")]
		[PXDefault(true)]
		public virtual bool? IsActive { get; set; }
		public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
		#endregion

		#region AccrualMethod
		/// <summary>
		/// The method of PTO hours accrual that defines whether PTO hours should be calculated 
		/// as a percentage or a specific number should be used for every pay period.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="PTOAccrualMethod.ListAttribute"/>.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Accrual Method")]
		[PTOAccrualMethod.List]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>>))]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual string AccrualMethod { get; set; }
		public abstract class accrualMethod : PX.Data.BQL.BqlString.Field<accrualMethod> { }
		#endregion

		#region AccrualRate
		/// <summary>
		/// An accrual rate to be used to accumulate hours.
		/// </summary>
		[PXDBDecimal(6, MinValue = 0)]
		[PXUIField(DisplayName = "Accrual %")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>
			.And<accrualMethod.IsEqual<PTOAccrualMethod.percentage>
				.Or<accrualMethod.IsEqual<PTOAccrualMethod.frontLoadingAndPercentage>>>>))]
		[ShowValueWhen(typeof(Where<accrualMethod.IsEqual<PTOAccrualMethod.percentage>
			.Or<accrualMethod.IsEqual<PTOAccrualMethod.frontLoadingAndPercentage>>>))]
		[PXDefault(TypeCode.Decimal, "0")]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual Decimal? AccrualRate { get; set; }
		public abstract class accrualRate : PX.Data.BQL.BqlDecimal.Field<accrualRate> { }
		#endregion

		#region HoursPerYear
		/// <summary>
		/// The number of hours that an employee may accrue throughout the year.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[PXUIField(DisplayName = "Hours per Year")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>
			.And<accrualMethod.IsEqual<PTOAccrualMethod.totalHoursPerYear>
				.Or<accrualMethod.IsEqual<PTOAccrualMethod.frontLoadingAndHoursPerYear>>>>))]
		[ShowValueWhen(typeof(Where<accrualMethod.IsEqual<PTOAccrualMethod.totalHoursPerYear>
			.Or<accrualMethod.IsEqual<PTOAccrualMethod.frontLoadingAndHoursPerYear>>>))]
		[PXDefault(TypeCode.Decimal, "0")]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual Decimal? HoursPerYear { get; set; }
		public abstract class hoursPerYear : PX.Data.BQL.BqlDecimal.Field<hoursPerYear> { }
		#endregion

		#region AccrualLimit
		/// <summary>
		/// The upper limit for the employee bank. Once the hours accumulated in the employee bank reach the limit, the system stops accruing the hours.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[PXUIField(DisplayName = "Balance Limit")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>>))]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual Decimal? AccrualLimit
		{
			get => _AccrualLimit != 0 ? _AccrualLimit : null;
			set => _AccrualLimit = value;
		}
		private decimal? _AccrualLimit;
		public abstract class accrualLimit : PX.Data.BQL.BqlDecimal.Field<accrualLimit> { }
		#endregion

		#region CarryoverType
		/// <summary>
		/// The way accruals are to be carried over from year to year starting the date specified in the Start Date box.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="CarryoverType.ListAttribute"/>.
		/// </value>
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Carryover Type")]
		[PXDefault(typeof(CarryoverType.none))]
		[CarryoverType.List]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>>))]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual string CarryoverType { get; set; }
		public abstract class carryoverType : PX.Data.BQL.BqlString.Field<carryoverType> { }
		#endregion

		#region CarryoverAmount
		/// <summary>
		/// The number of hours the system carries over to the following year. 
		/// This box is available only if Partial is selected in the Carryover Type box.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[PXUIField(DisplayName = "Carryover Hours")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>
			.And<carryoverType.IsEqual<CarryoverType.partial>>>))]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual Decimal? CarryoverAmount { get; set; }
		public abstract class carryoverAmount : PX.Data.BQL.BqlDecimal.Field<carryoverAmount> { }
		#endregion

		#region FrontLoadingAmount
		/// <summary>
		/// The number of hours the system adds to the employee bank each year on the date specified in the Start Date box.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[PXUIField(DisplayName = "Front Loading Hours")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>
			.And<accrualMethod.IsEqual<PTOAccrualMethod.frontLoading>
				.Or<accrualMethod.IsEqual<PTOAccrualMethod.frontLoadingAndPercentage>>
				.Or<accrualMethod.IsEqual<PTOAccrualMethod.frontLoadingAndHoursPerYear>>>>))]
		[ShowValueWhen(typeof(Where<accrualMethod.IsEqual<PTOAccrualMethod.frontLoading>
			.Or<accrualMethod.IsEqual<PTOAccrualMethod.frontLoadingAndPercentage>>
			.Or<accrualMethod.IsEqual<PTOAccrualMethod.frontLoadingAndHoursPerYear>>>))]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual Decimal? FrontLoadingAmount { get; set; }
		public abstract class frontLoadingAmount : PX.Data.BQL.BqlDecimal.Field<frontLoadingAmount> { }
		#endregion

		#region StartDate
		/// <summary>
		/// The date at which the employee bank becomes active. 
		/// </summary>
		[PXDBDate(IsKey = true)]
		[PXUIField(DisplayName = "Start Date")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>>))]
		[PXDefault]
		public virtual DateTime? StartDate { get; set; }
		public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
		#endregion

		#region PTOYearStartDate
		/// <summary>
		/// The start date of the PTO year. 
		/// </summary>
		[PXDate]
		[PXDBScalar(typeof(SearchFor<PRPTOBank.startDate>.Where<PRPTOBank.bankID.IsEqual<bankID>>))]
		[PXUnboundDefault(typeof(SearchFor<PRPTOBank.startDate>.Where<PRPTOBank.bankID.IsEqual<bankID.FromCurrent>>))]
		public virtual DateTime? PTOYearStartDate { get; set; }
		public abstract class pTOYearStartDate : PX.Data.BQL.BqlDateTime.Field<pTOYearStartDate> { }
		#endregion

		#region BandingRule
		/// <summary>
		/// The banding rule which indicates the years of service after which the employee bank will become active.
		/// </summary>
		[PXDBInt]
		[PXUIField(DisplayName = "Banding Rule", Enabled = false)]
		public virtual int? BandingRule { get; set; }
		public abstract class bandingRule : PX.Data.BQL.BqlInt.Field<bandingRule> { }
		#endregion

		#region AllowNegativeBalance
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that the system does not put restrictions on the disbursing amount.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Allow Negative Balance")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>>))]
		[PXDefault(false)]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual bool? AllowNegativeBalance { get; set; }
		public abstract class allowNegativeBalance : PX.Data.BQL.BqlBool.Field<allowNegativeBalance> { }
		#endregion

		#region TransferDate
		/// <summary>
		/// The date (day and month) at which the system adds the frontloading amount of hours to the employee bank when banding rules apply.
		/// </summary>
		[PXDBDate(InputMask = "m")]
		[PXUIField(DisplayName = "Transfer Date", Enabled = false)]
		public virtual DateTime? TransferDate { get; set; }
		public abstract class transferDate : PX.Data.BQL.BqlDateTime.Field<transferDate> { }
		#endregion

		#region ProbationPeriodBehaviour
		/// <summary>
		/// The probation period behaviour.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="ProbationPeriodBehaviour.ListAttribute"/>.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[ProbationPeriodBehaviour.List]
		[PXUIField(DisplayName = "During Probation Period")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>>))]
		[PXDefault(typeof(ProbationPeriodBehaviour.accruedAndAvailable))]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual string ProbationPeriodBehaviour { get; set; }
		public abstract class probationPeriodBehaviour : PX.Data.BQL.BqlString.Field<probationPeriodBehaviour> { }
		#endregion

		#region SettlementBalanceType
		/// <summary>
		/// The rule that will be applied to the employee bank when a final paycheck is 
		/// calculated for the employee.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="SettlementBalanceType.ListAttribute"/>.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[SettlementBalanceType.List]
		[PXUIField(DisplayName = "On Settlement")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>>))]
		[PXDefault(typeof(SettlementBalanceType.pay))]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual string SettlementBalanceType { get; set; }
		public abstract class settlementBalanceType : PX.Data.BQL.BqlString.Field<settlementBalanceType> { }
		#endregion

		#region DisburseFromCarryover
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that you can only use the carryover hours from the previous year.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Can Only Disburse from Carryover")]
		[PXUIEnabled(typeof(Where<PREmployee.useCustomSettings.FromParent.IsEqual<True>>))]
		[PXDefault(false)]
		[PTOEmployeeSettingsEditRestriction(typeof(bAccountID), typeof(startDate))]
		public virtual bool? DisburseFromCarryover { get; set; }
		public abstract class disburseFromCarryover : PX.Data.BQL.BqlBool.Field<disburseFromCarryover> { }
		#endregion

		#region CreateFinancialTransaction
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that money calculation and creation of the general ledger transaction for PTO on Paychecks and Adjustments will be enabled.
		/// </summary>
		[PXBool]
		[PXUnboundDefault(typeof(Selector<bankID, PRPTOBank.createFinancialTransaction>))]
		public virtual bool? CreateFinancialTransaction { get; set; }
		public abstract class createFinancialTransaction : PX.Data.BQL.BqlBool.Field<createFinancialTransaction> { }
		#endregion

		#region AllowViewAvailablePTOPaidHours
		/// <summary>
		/// Allow (if set to <see langword="true" />) to view the available PTO paid hours.
		/// </summary>
		[PXBool]
		[PXDependsOnFields(typeof(createFinancialTransaction))]
		public virtual bool? AllowViewAvailablePTOPaidHours => CreateFinancialTransaction == true;
		public abstract class allowViewAvailablePTOPaidHours : PX.Data.BQL.BqlBool.Field<allowViewAvailablePTOPaidHours> { }
		#endregion

		#region System Columns
		#region TStamp
		public abstract class tStamp : PX.Data.BQL.BqlByteArray.Field<tStamp> { }
		[PXDBTimestamp]
		public byte[] TStamp { get; set; }
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		[PXDBCreatedByID]
		public Guid? CreatedByID { get; set; }
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		[PXDBCreatedByScreenID]
		public string CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		[PXDBCreatedDateTime]
		public DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		[PXDBLastModifiedByID]
		public Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		[PXDBLastModifiedByScreenID]
		public string LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		[PXDBLastModifiedDateTime]
		public DateTime? LastModifiedDateTime { get; set; }
		#endregion
		#endregion
	}
}

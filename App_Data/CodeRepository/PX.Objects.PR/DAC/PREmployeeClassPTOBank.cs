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
using System.Diagnostics;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.EP;
using PX.Objects.PR.Descriptor;
using PX.Payroll.Data;

namespace PX.Objects.PR
{
	/// <summary>
	/// Stores the information related to the employee class settings of a specific group of employees. The information will be displayed on the PTO Banks (PR204000) form.
	/// </summary>
	[Serializable]
	[PXCacheName(Messages.PREmployeeClassPTOBank)]
	[DebuggerDisplay("{GetType().Name,nq}: EmployeeClassID = {EmployeeClassID,nq}, BankID = {BankID,nq}, StartDate = {StartDate,nq}")]
	public class PREmployeeClassPTOBank : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<PREmployeeClassPTOBank>.By<recordID>
		{
			public static PREmployeeClassPTOBank Find(PXGraph graph, int? recordID) => FindBy(graph, recordID);
		}

		public static class FK
		{
			public class EmployeeClass : PREmployeeClass.PK.ForeignKeyOf<PREmployeeClassPTOBank>.By<employeeClassID> { }
			public class PTOBank : PRPTOBank.PK.ForeignKeyOf<PREmployeeClassPTOBank>.By<bankID> { }
		}

		[Obsolete("This foreign key is obsolete and is going to be removed in 2021R1. Use FK.PTOBank instead.")]
		public class PTOBankFK : FK.PTOBank { }
		#endregion

		#region RecordID
		/// <summary>
		/// The unique identifier of a banding rule.
		/// </summary>
		[PXDBIdentity(IsKey = true)]
		public virtual Int32? RecordID { get; set; }
		public abstract class recordID : PX.Data.BQL.BqlInt.Field<recordID> { }
		#endregion

		#region EmployeeClassID
		/// <summary>
		/// The unique identifier of an Employee Class.
		/// </summary>
		[PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCC")]
		[PXUIField(DisplayName = "Employee Class")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(SearchFor<PREmployeeClass.employeeClassID>))]
		[PXReferentialIntegrityCheck]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual string EmployeeClassID { get; set; }
		public abstract class employeeClassID : PX.Data.BQL.BqlString.Field<employeeClassID> { }
		#endregion

		#region BankID
		/// <summary>
		/// The unique identifier of a PTO bank to be used for the paid time off calculation.
		/// </summary>
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "PTO Bank")]
		[PXDBDefault(typeof(PRPTOBank.bankID))]
		[PXParent(typeof(Select<PRPTOBank, Where<PRPTOBank.bankID, Equal<Current<bankID>>>>))]
		[PXForeignReference(typeof(FK.PTOBank))]
		[PXReferentialIntegrityCheck]
		public virtual string BankID { get; set; }
		public abstract class bankID : PX.Data.BQL.BqlString.Field<bankID> { }
		#endregion

		#region IsActive
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that the Employee Class Setting is active.
		/// </summary>
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? IsActive { get; set; }
		public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
		#endregion

		#region AllowNegativeBalance
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that the system does not put restrictions on the disbursing amount.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Allow Negative Balance")]
		[PXDefault(false)]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual bool? AllowNegativeBalance { get; set; }
		public abstract class allowNegativeBalance : PX.Data.BQL.BqlBool.Field<allowNegativeBalance> { }
		#endregion

		#region DisburseFromCarryover
		///// <summary>
		///// Indicates (if set to <see langword="true" />) that you can use only the carryover hours from the previous year.
		///// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Disburse Only from Carryover")]
		[PXDefault(false)]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual bool? DisburseFromCarryover { get; set; }
		public abstract class disburseFromCarryover : PX.Data.BQL.BqlBool.Field<disburseFromCarryover> { }
		#endregion

		#region AccrualRate
		/// <summary>
		/// An accrual rate to be used to accumulate hours.
		/// </summary>
		[PXDBDecimal(6, MinValue = 0)]
		[PXUIField(DisplayName = "Accrual %")]
		[PXDefault(TypeCode.Decimal, "0")]
		[PXUIEnabled(typeof(Where<Parent<PRPTOBank.accrualMethod>, Equal<PTOAccrualMethod.percentage>,
			Or<Parent<PRPTOBank.accrualMethod>, Equal<PTOAccrualMethod.frontLoadingAndPercentage>>>))]
		[HidePTOSettings(typeof(accrualRate))]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual Decimal? AccrualRate { get; set; }
		public abstract class accrualRate : PX.Data.BQL.BqlDecimal.Field<accrualRate> { }
		#endregion

		#region HoursPerYear
		/// <summary>
		/// The number of hours that an employee may accrue throughout the year.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[PXUIField(DisplayName = "Hours per Year")]
		[PXDefault(TypeCode.Decimal, "0")]
		[PXUIEnabled(typeof(Where<Parent<PRPTOBank.accrualMethod>, Equal<PTOAccrualMethod.totalHoursPerYear>,
			Or<Parent<PRPTOBank.accrualMethod>, Equal<PTOAccrualMethod.frontLoadingAndHoursPerYear>>>))]
		[HidePTOSettings(typeof(hoursPerYear))]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual Decimal? HoursPerYear { get; set; }
		public abstract class hoursPerYear : PX.Data.BQL.BqlDecimal.Field<hoursPerYear> { }
		#endregion

		#region AccrualLimit
		/// <summary>
		/// The upper limit for the bank. Once the hours accumulated in the bank reach the limit, the system stops accruing the hours.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[PXUIField(DisplayName = "Balance Limit")]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual Decimal? AccrualLimit
		{
			get => _AccrualLimit != 0 ? _AccrualLimit : null;
			set => _AccrualLimit = value;
		}
		private decimal? _AccrualLimit;
		public abstract class accrualLimit : PX.Data.BQL.BqlDecimal.Field<accrualLimit> { }
		#endregion

		#region StartDate
		/// <summary>
		/// The effective date of the Employee Class Settings.
		/// </summary>
		[PXDBDate]
		[PXDefault]
		[PXUIField(DisplayName = "Effective Date")]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual DateTime? StartDate { get; set; }
		public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
		#endregion

		#region PTOYearStartDate
		/// <summary>
		/// The year of the PTO start date.
		/// </summary>
		[PXDate]
		[PXDBScalar(typeof(SearchFor<PRPTOBank.startDate>.Where<PRPTOBank.bankID.IsEqual<bankID>>))]
		[PXUnboundDefault(typeof(SearchFor<PRPTOBank.startDate>.Where<PRPTOBank.bankID.IsEqual<bankID.FromCurrent>>))]
		public virtual DateTime? PTOYearStartDate { get; set; }
		public abstract class pTOYearStartDate : PX.Data.BQL.BqlDateTime.Field<pTOYearStartDate> { }
		#endregion

		#region CarryoverAmount
		/// <summary>
		/// The number of hours the system carries over to the following year. 
		/// This box is available only if Partial is selected in the Carryover Type box.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[PXUIField(DisplayName = "Carryover Hours")]
		[PXUIEnabled(typeof(Where<Parent<PRPTOBank.carryoverType>, Equal<CarryoverType.partial>>))]
		[HidePTOSettings(typeof(carryoverAmount))]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual Decimal? CarryoverAmount { get; set; }
		public abstract class carryoverAmount : PX.Data.BQL.BqlDecimal.Field<carryoverAmount> { }
		#endregion

		#region FrontLoadingAmount
		/// <summary>
		/// The number of hours the system adds to the bank each year on a date specified in the Start Date box.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[PXUIField(DisplayName = "Front Loading Hours")]
		[PXUIEnabled(typeof(Where<Parent<PRPTOBank.accrualMethod>, Equal<PTOAccrualMethod.frontLoading>,
			Or<Parent<PRPTOBank.accrualMethod>, Equal<PTOAccrualMethod.frontLoadingAndPercentage>,
			Or<Parent<PRPTOBank.accrualMethod>, Equal<PTOAccrualMethod.frontLoadingAndHoursPerYear>>>>))]
		[HidePTOSettings(typeof(frontLoadingAmount))]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual Decimal? FrontLoadingAmount { get; set; }
		public abstract class frontLoadingAmount : PX.Data.BQL.BqlDecimal.Field<frontLoadingAmount> { }
		#endregion

		#region CreateFinancialTransaction
		///// <summary>
		///// Indicates (if set to <see langword="true" />) that it will enable the money calculation and the creation of general ledger transaction for paid time off on Paychecks and Adjustments.
		///// </summary>
		[PXBool]
		[PXUnboundDefault(typeof(Selector<bankID, PRPTOBank.createFinancialTransaction>))]
		public virtual bool? CreateFinancialTransaction { get; set; }
		public abstract class createFinancialTransaction : PX.Data.BQL.BqlBool.Field<createFinancialTransaction> { }
		#endregion

		#region ProbationPeriodBehaviour
		/// <summary>
		/// The probation period behaviour.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="ProbationPeriodBehaviour.List"/>.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "During Probation Period")]
		[PXDefault(typeof(ProbationPeriodBehaviour.accruedAndAvailable))]
		[ProbationPeriodBehaviour.List]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual string ProbationPeriodBehaviour { get; set; }
		public abstract class probationPeriodBehaviour : PX.Data.BQL.BqlString.Field<probationPeriodBehaviour> { }
		#endregion

		#region Unassigned
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that the row was recovered during upgrade and won't be assigned to any employee.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Unassigned", Enabled = false, Visible = false)]
		[PXDefault(false)]
		public virtual bool? Unassigned { get; set; }
		public abstract class unassigned : PX.Data.BQL.BqlBool.Field<unassigned> { }
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

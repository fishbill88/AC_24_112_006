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
using PX.Data.ReferentialIntegrity.Attributes;
using System;
using PX.Data.BQL.Fluent;
using PX.Objects.PR.Descriptor;

namespace PX.Objects.PR
{
	/// <summary>
	/// Stores the information related to the banding rules of a specific group of employees. The information will be displayed on the PTO Banks (PR204000) form.
	/// </summary>
	[Serializable]
	[PXCacheName(Messages.PRBandingRulePTOBank)]
	public class PRBandingRulePTOBank : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<PRBandingRulePTOBank>.By<recordID>
		{
			public static PRBandingRulePTOBank Find(PXGraph graph, int? recordID) => FindBy(graph, recordID);
		}

		public static class FK
		{
			public class EmployeeClass : PREmployeeClass.PK.ForeignKeyOf<PREmployeeClassPTOBank>.By<employeeClassID> { }
			public class PTOBank : PRPTOBank.PK.ForeignKeyOf<PREmployeeClassPTOBank>.By<bankID> { }
		}
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

		#region YearsOfService
		/// <summary>
		/// The years of service after which the new banding rules will apply.
		/// </summary>
		[PXDBInt(MinValue = 1, MaxValue = 99)]
		[PXDefault]
		[PXUIField(DisplayName = "Years of Service", Required = true)]
		[PTOBanksEditRestriction(typeof(employeeClassID))]
		public virtual int? YearsOfService { get; set; }
		public abstract class yearsOfService : PX.Data.BQL.BqlInt.Field<yearsOfService> { }
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

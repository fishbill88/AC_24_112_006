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
using System;

namespace PX.Objects.PR
{
	/// <summary>
	/// Stores the details of the PTO adjustment
	/// </summary>
	[PXCacheName(Messages.PRPTOAdjustmentDetail)]
	[Serializable]
	public class PRPTOAdjustmentDetail : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<PRPTOAdjustmentDetail>.By<type, refNbr, bAccountID, bankID>
		{
			public static PRPTOAdjustmentDetail Find(PXGraph graph, string type, string refNbr, string bAccountID, string bankID) =>
				FindBy(graph, type, refNbr, bAccountID, bankID);
		}

		public static class FK
		{
			public class PTOAdjustment : PRPTOAdjustment.PK.ForeignKeyOf<PRPTOAdjustmentDetail>.By<type, refNbr> { }
			public class Employee : PREmployee.PK.ForeignKeyOf<PRPTOAdjustmentDetail>.By<bAccountID> { }
			public class PTOBank : PRPTOBank.PK.ForeignKeyOf<PRPTOAdjustmentDetail>.By<bankID> { }
			public class Payment : PRPayment.PK.ForeignKeyOf<PRPTOAdjustmentDetail>.By<paymentDocType, paymentRefNbr> { }
		}
		#endregion

		#region Type
		public abstract class type : PX.Data.BQL.BqlString.Field<type> { }
		/// <summary>
		/// The type of the PTO adjustment.
		/// The field is included in <see cref="FK.PTOAdjustment"/>.
		/// </summary>
		/// <value>
		/// The value of this field corresponds to the value of the <see cref="PRPTOAdjustment.Type"/> field.
		/// </value>
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(PRPTOAdjustment.type))]
		public virtual string Type { get; set; }
		#endregion

		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		/// <summary>
		/// The user-friendly unique identifier of the PTO Adjustment.
		/// The field is included in <see cref="FK.PTOAdjustment"/>.
		/// </summary>
		/// <value>
		/// The value of this field corresponds to the value of the <see cref="PRPTOAdjustment.RefNbr"/> field.
		/// </value>
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDBDefault(typeof(PRPTOAdjustment.refNbr))]
		[PXParent(typeof(FK.PTOAdjustment))]
		public virtual string RefNbr { get; set; }
		#endregion

		#region BAccountID
		public abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
		/// <summary>
		/// The unique identifier of the business account to which the employee bank belongs.
		/// The field is included in <see cref="FK.Employee"/>.
		/// </summary>
		[EmployeeActiveInPayGroup(IsKey = true, Required = true)]
		[PXForeignReference(typeof(FK.Employee))]
		[PXDefault]
		public int? BAccountID { get; set; }
		#endregion

		#region BankID
		public abstract class bankID : PX.Data.BQL.BqlString.Field<bankID> { }
		/// <summary>
		/// The unique identifier of the PTO bank to be used for the paid-time-off calculation.
		/// The field is included in <see cref="FK.PTOBank"/>.
		/// </summary>
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "PTO Bank", Required = true)]
		[PXSelector(typeof(SelectFrom<PRPTOBank>
			.InnerJoin<PREmployeePTOBank>.On<PRPTOBank.bankID.IsEqual<PREmployeePTOBank.bankID>>
			.Where<PRPTOBank.isActive.IsEqual<True>
				.And<PREmployeePTOBank.isActive.IsEqual<True>>
				.And<PREmployeePTOBank.bAccountID.IsEqual<bAccountID.FromCurrent>>>
			.SearchFor<PRPTOBank.bankID>), DescriptionField = typeof(PRPTOBank.description))]
		[PXForeignReference(typeof(FK.PTOBank))]
		[PXDefault]
		public virtual string BankID { get; set; }
		#endregion

		#region EffectiveStartDate
		public abstract class effectiveStartDate : PX.Data.BQL.BqlDateTime.Field<effectiveStartDate> { }
		/// <summary>
		/// The start date when the settings of the PTO bank are affected by the current PTO adjustment detail.
		/// </summary>
		[PXDBDate]
		[PXUIField(DisplayName = "PTO Bank Effective From", Enabled = false)]
		public virtual DateTime? EffectiveStartDate { get; set; }
		#endregion

		#region PaymentDocType
		public abstract class paymentDocType : PX.Data.BQL.BqlString.Field<paymentDocType> { }
		/// <summary>
		/// The type of the payment.
		/// The field is included in <see cref="FK.Payment"/>.
		/// </summary>
		/// <value>
		/// The value of this field corresponds to the value of the <see cref="PRPayment.DocType"/> field.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Payment Doc. Type")]
		public string PaymentDocType { get; set; }
		#endregion

		#region PaymentRefNbr
		public abstract class paymentRefNbr : PX.Data.BQL.BqlString.Field<paymentRefNbr> { }
		/// <summary>
		/// The unique identifier of the payment.
		/// The field is included in <see cref="FK.Payment"/>.
		/// </summary>
		/// <value>
		/// The value of this field corresponds to the value of the <see cref="PRPayment.RefNbr"/> field.
		/// </value>
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Payment Ref. Number")]
		public string PaymentRefNbr { get; set; }
		#endregion

		#region InitialBalance
		public abstract class initialBalance : PX.Data.BQL.BqlDecimal.Field<initialBalance> { }
		/// <summary>
		/// Year to date available hours for the selected Employee and PTO Bank on the date of adjustment.
		/// </summary>
		[PXDBDecimal]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Initial Balance", Enabled = false)]
		public virtual decimal? InitialBalance { get; set; }
		#endregion

		#region AdjustmentHours
		public abstract class adjustmentHours : PX.Data.BQL.BqlDecimal.Field<adjustmentHours> { }
		/// <summary>
		/// Adjustment hours (positive or negative).
		/// </summary>
		[PXDBDecimal]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Adjustment Hours", Required = true)]
		public virtual decimal? AdjustmentHours { get; set; }
		#endregion

		#region AdjustmentReason
		public abstract class adjustmentReason : PX.Data.BQL.BqlString.Field<adjustmentReason> { }
		/// <summary>
		/// The reason of the PTO adjustment.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="PTOAdjustmentReason.ListAttribute"/>.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Adjustment Reason", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		[PXDefault]
		[PTOAdjustmentReason.List]
		public virtual string AdjustmentReason { get; set; }
		#endregion

		#region ReasonDetails
		public abstract class reasonDetails : PX.Data.BQL.BqlString.Field<reasonDetails> { }
		/// <summary>
		/// The description of the PTO adjustment.
		/// </summary>
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Reason Details")]
		[PXUIEnabled(typeof(Where<adjustmentReason.IsEqual<PTOAdjustmentReason.other>>))]
		public virtual string ReasonDetails { get; set; }
		#endregion

		#region NewBalance
		public abstract class newBalance : PX.Data.BQL.BqlDecimal.Field<newBalance> { }
		/// <summary>
		/// Updated year to date available hours calculated as 'Initial Balance' + 'Adjustment Hours'.
		/// </summary>
		[PXDBDecimal]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "New Balance", Enabled = false)]
		[PXFormula(typeof(initialBalance.When<adjustmentHours.IsNull>.Else<initialBalance.Add<adjustmentHours>>))]
		public virtual decimal? NewBalance { get; set; }
		#endregion

		#region BalanceLimit
		public abstract class balanceLimit : PX.Data.BQL.BqlDecimal.Field<balanceLimit> { }
		/// <summary>
		/// The upper limit for the bank. Once the hours accumulated in the bank reach the limit, the system stops accruing the hours.
		/// </summary>
		[PXDBDecimal]
		[PXUIField(DisplayName = "Balance Limit", Enabled = false)]
		public virtual decimal? BalanceLimit { get; set; }
		#endregion

		#region System Columns
		#region TStamp
		[PXDBTimestamp]
		public virtual byte[] TStamp { get; set; }
		public abstract class tStamp : PX.Data.BQL.BqlByteArray.Field<tStamp> { }
		#endregion

		#region CreatedByID
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID { get; set; }
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		#endregion

		#region CreatedByScreenID
		[PXDBCreatedByScreenID()]
		public virtual string CreatedByScreenID { get; set; }
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		#endregion

		#region CreatedDateTime
		[PXDBCreatedDateTime()]
		[PXUIField(DisplayName = "Created On", Enabled = false)]
		public virtual DateTime? CreatedDateTime { get; set; }
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		#endregion

		#region LastModifiedByID
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID { get; set; }
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		#endregion

		#region LastModifiedByScreenID
		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID { get; set; }
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		#endregion

		#region LastModifiedDateTime
		[PXDBLastModifiedDateTime()]
		[PXUIField(DisplayName = "Last Modified On", Enabled = false)]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		#endregion
		#endregion System Columns
	}
}

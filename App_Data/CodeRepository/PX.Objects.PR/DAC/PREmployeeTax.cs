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
	/// Stores the taxes which apply to a specific employee according to their home and work addresses. The information will be displayed on the Employee Payroll Settings (PR203000) form.
	/// </summary>
	[PXCacheName(Messages.PREmployeeTax)]
	[Serializable]
	public class PREmployeeTax : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<PREmployeeTax>.By<bAccountID, taxID>
		{
			public static PREmployeeTax Find(PXGraph graph, int? bAccountID, int? taxID, PKFindOptions options = PKFindOptions.None) =>
				FindBy(graph, bAccountID, taxID, options);
		}

		public static class FK
		{
			public class Employee : PREmployee.PK.ForeignKeyOf<PREmployeeTax>.By<bAccountID> { }
			public class TaxCode : PRTaxCode.PK.ForeignKeyOf<PREmployeeTax>.By<taxID> { }
		}
		#endregion

		#region BAccountID
		public abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(PREmployee.bAccountID))]
		[PXParent(typeof(Select<PREmployee, Where<PREmployee.bAccountID, Equal<Current<PREmployeeTax.bAccountID>>>>))]
		public virtual int? BAccountID { get; set; }
		#endregion
		#region TaxID
		public abstract class taxID : PX.Data.BQL.BqlInt.Field<taxID> { }
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Tax Code")]
		[PXSelector(typeof(
			SearchFor<PRTaxCode.taxID>
				.Where<PRTaxCode.countryID.IsEqual<employeeCountryID.FromCurrent>
					.And<PRTaxCode.isDeleted.IsEqual<False>>>),
			DescriptionField = typeof(PRTaxCode.description),
			SubstituteKey = typeof(PRTaxCode.taxCD))]
		public virtual int? TaxID { get; set; }
		#endregion
		#region IsActive
		public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
		[PXDBBool]
		[PXUIField(DisplayName = "Active")]
		[PXDefault(true)]
		public virtual bool? IsActive { get; set; }
		#endregion
		#region State
		public abstract class state : PX.Data.BQL.BqlString.Field<state> { }
		[PXString]
		[PXUIField(DisplayName = "State", Visible = false)]
		[PXFormula(typeof(Selector<PREmployeeTax.taxID, PRTaxCode.taxState>))]
		public virtual string State { get; set; }
		#endregion
		#region CountryID
		public abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }
		[PXString]
		[PXUnboundDefault(typeof(SearchFor<PRTaxCode.countryID>
			.Where<PRTaxCode.taxID.IsEqual<taxID.FromCurrent>
				.And<PRTaxCode.isDeleted.IsEqual<False>>>))]
		[PXDBScalar(typeof(SearchFor<PRTaxCode.countryID>
			.Where<PRTaxCode.taxID.IsEqual<taxID>
				.And<PRTaxCode.isDeleted.IsEqual<False>>>))]
		public virtual string CountryID { get; set; }
		#endregion
		#region EmployeeCountryID
		[PXString(2)]
		[PXUnboundDefault(typeof(Parent<PREmployee.countryID>))]
		public virtual string EmployeeCountryID { get; set; }
		public abstract class employeeCountryID : PX.Data.BQL.BqlString.Field<employeeCountryID> { }
		#endregion
		#region ErrorLevel
		public abstract class errorLevel : PX.Data.BQL.BqlInt.Field<errorLevel> { }
		[PXInt]
		public virtual int? ErrorLevel { get; set; }
		#endregion

		#region System Columns
		#region TStamp
		public abstract class tStamp : IBqlField { }
		[PXDBTimestamp]
		public byte[] TStamp { get; set; }
		#endregion
		#region CreatedByID
		public abstract class createdByID : IBqlField { }
		[PXDBCreatedByID]
		public Guid? CreatedByID { get; set; }
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : IBqlField { }
		[PXDBCreatedByScreenID]
		public string CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : IBqlField { }
		[PXDBCreatedDateTime]
		public DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : IBqlField { }
		[PXDBLastModifiedByID]
		public Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : IBqlField { }
		[PXDBLastModifiedByScreenID]
		public string LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : IBqlField { }
		[PXDBLastModifiedDateTime]
		public DateTime? LastModifiedDateTime { get; set; }
		#endregion
		#endregion
	}
}

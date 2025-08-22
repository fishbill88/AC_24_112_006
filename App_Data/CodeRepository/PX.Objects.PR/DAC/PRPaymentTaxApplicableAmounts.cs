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
using PX.Payroll.Data;
using System;

namespace PX.Objects.PR
{
	/// <summary>
	/// Stores the information about the amount used to calculate each tax for a specific paycheck. The information will be displayed on the Paychecks and Adjustments (PR302000) form.
	/// </summary>
	[PXCacheName(Messages.PRPaymentTaxApplicableAmounts)]
	[Serializable]
	public class PRPaymentTaxApplicableAmounts : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<PRPaymentTaxApplicableAmounts>.By<docType, refNbr, taxID, wageTypeID, isSupplemental>
		{
			public static PRPaymentTaxApplicableAmounts Find(PXGraph graph, string docType, string refNbr, int? taxID, int? wageTypeID, bool? isSupplemental, PKFindOptions options = PKFindOptions.None) =>
				FindBy(graph, docType, refNbr, taxID, wageTypeID, isSupplemental, options);
		}

		public static class FK
		{
			public class Payment : PRPayment.PK.ForeignKeyOf<PRPaymentTaxApplicableAmounts>.By<docType, refNbr> { }
			public class Tax : PRTaxCode.PK.ForeignKeyOf<PRPaymentTaxApplicableAmounts>.By<taxID> { }
		}
		#endregion

		#region DocType
		[PXDBString(3, IsFixed = true, IsKey = true)]
		[PXDBDefault(typeof(PRPayment.docType))]
		public string DocType { get; set; }
		public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
		#endregion

		#region RefNbr
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(PRPayment.refNbr))]
		[PXParent(typeof(FK.Payment))]
		public string RefNbr { get; set; }
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		#endregion

		#region TaxID
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Code")]
		[PXSelector(typeof(SearchFor<PRTaxCode.taxID>
			.Where<PRTaxCode.countryID.IsEqual<paymentCountryID.FromCurrent>
				.And<PRTaxCode.isDeleted.IsEqual<False>>>),
			DescriptionField = typeof(PRTaxCode.description),
			SubstituteKey = typeof(PRTaxCode.taxCD))]
		[PXUIEnabled(typeof(Where<taxID.IsNull>))]
		public virtual int? TaxID { get; set; }
		public abstract class taxID : PX.Data.BQL.BqlInt.Field<taxID> { }
		#endregion

		#region WageTypeID
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Wage Type")]
		[PRWebServiceTypeFromDatabaseSelector(PRTaxWebServiceDataSlot.DataType.WageTypes, LocationConstants.CanadaCountryCode, false)]
		public virtual int? WageTypeID { get; set; }
		public abstract class wageTypeID : PX.Data.BQL.BqlInt.Field<wageTypeID> { }
		#endregion

		#region IsSupplemental
		[PXDBBool(IsKey = true)]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Is Supplemental")]
		public virtual bool? IsSupplemental { get; set; }
		public abstract class isSupplemental : PX.Data.BQL.BqlBool.Field<isSupplemental> { }
		#endregion

		#region AmountAllowed
		[PRCurrency]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Allowed")]
		public virtual decimal? AmountAllowed { get; set; }
		public abstract class amountAllowed : PX.Data.BQL.BqlDecimal.Field<amountAllowed> { }
		#endregion

		#region PaymentCountryID
		[PXString(2)]
		[PXUnboundDefault(typeof(Parent<PRPayment.countryID>))]
		public virtual string PaymentCountryID { get; set; }
		public abstract class paymentCountryID : PX.Data.BQL.BqlString.Field<paymentCountryID> { }
		#endregion

		#region System Columns
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
		public virtual DateTime? LastModifiedDateTime { get; set; }
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		#endregion
		#endregion System Columns
	}
}

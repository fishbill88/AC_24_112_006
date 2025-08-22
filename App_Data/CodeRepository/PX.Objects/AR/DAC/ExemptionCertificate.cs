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

namespace PX.Objects.AR
{
	/// <summary>
	/// An unbound DAC that is used to collect the information that is related to the exemption certificate.
	/// </summary>
	[PXHidden]
	public partial class ExemptionCertificate : PXBqlTable, IBqlTable
	{
		#region CertificateID
		public abstract class certificateID : PX.Data.BQL.BqlInt.Field<certificateID> { }
		/// <summary>
		/// The exemption ceritifcate ID.
		/// </summary>
		[PXString(IsKey =true)]
		[PXUIField(DisplayName = "Certificate ID")]
		public virtual string CertificateID
		{
			get;
			set;
		}
		#endregion

		#region ECMCompanyID
		public abstract class eCMCompanyID : PX.Data.BQL.BqlString.Field<eCMCompanyID> { }
		/// <summary>
		/// The company ID in the exemption certificate.
		/// </summary>
		[PXString()]
		public virtual string ECMCompanyID
		{
			get;
			set;
		}
		#endregion

		#region State
		public abstract class state : PX.Data.BQL.BqlString.Field<state> { }
		/// <summary>
		/// The state or region of the exemption certificate.
		/// </summary>
		[PXString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "State", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual string State
		{
			get;
			set;
		}
		#endregion

		#region ExemptionReason
		public abstract class exemptionReason : PX.Data.BQL.BqlString.Field<exemptionReason> { }
		/// <summary>
		///The reason of the exemption certificate.
		/// </summary>
		[PXString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Reason For Exemption", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual string ExemptionReason
		{
			get;
			set;
		}
		#endregion

		#region EffectiveDate
		public abstract class effectiveDate : PX.Data.BQL.BqlDateTime.Field<effectiveDate> { }
		/// <summary>
		/// The effective date of the exemption certificate.
		/// </summary>
		[PXDate()]
		[PXUIField(DisplayName = "Effective Date", Enabled = false)]
		public virtual DateTime? EffectiveDate
		{
			get;
			set;
		}
		#endregion

		#region ExpirationDate
		public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
		/// <summary>
		/// The expiration date of the exemption certificate.
		/// </summary>
		[PXDate()]
		[PXUIField(DisplayName = "Expiration Date", Enabled = false)]
		public virtual DateTime? ExpirationDate
		{
			get;
			set;
		}
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
		/// <summary>
		/// The status of the exemption certificate.
		/// </summary>
		[PXString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Certificate Status", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual string Status
		{
			get;
			set;
		}
		#endregion

		#region CompanyCode
		public abstract class companyCode : PX.Data.BQL.BqlString.Field<companyCode> { }
		/// <summary>
		/// The company code of the exemption certificate.
		/// </summary>
		[PXString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Company Code", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual string CompanyCode
		{
			get;
			set;
		}
		#endregion
	}
}

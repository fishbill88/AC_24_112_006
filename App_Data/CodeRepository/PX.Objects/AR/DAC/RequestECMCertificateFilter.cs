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
using PX.Objects.CR;
using PX.Objects.TX;
using System;

namespace PX.Objects.AR
{
	/// <summary>
	/// An unbound DAC that is used for the dialog box that is displayed when a certificate is requested on the Customers (AR303000) form.
	/// </summary>
	[Serializable]
	[PXHidden]
	public class RequestECMCertificateFilter : PXBqlTable, IBqlTable
	{
		#region CompanyCode
		public abstract class companyCode : PX.Data.BQL.BqlString.Field<companyCode> { }
		/// <summary>
		/// The company code for which the exemption certificate is requested.
		/// </summary>
		[PXDBString()]
		[PXUIField(DisplayName = "Company Code", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search5<TaxPluginMapping.companyCode,
			InnerJoin<TaxPlugin, On<TaxPluginMapping.taxPluginID, Equal<TaxPlugin.taxPluginID>>,
			InnerJoin<TXSetup, On<TaxPlugin.taxPluginID, Equal<TXSetup.eCMProvider>>>>,
			Aggregate<GroupBy<TaxPluginMapping.companyCode>>>))]
		[PXDefault(typeof(Search2<TaxPluginMapping.companyCode,
			InnerJoin<TaxPlugin, On<TaxPluginMapping.taxPluginID, Equal<TaxPlugin.taxPluginID>>,
				InnerJoin<TXSetup, On<TaxPlugin.taxPluginID, Equal<TXSetup.eCMProvider>>>>,
			Where<TaxPluginMapping.branchID, Equal<Current<AccessInfo.branchID>>>>))]
		public virtual string CompanyCode
		{
			get; set;
		}
		#endregion

		#region Email
		public abstract class emailId : PX.Data.BQL.BqlString.Field<emailId> { }
		/// <summary>
		/// The email ID of the recipient.
		/// </summary>
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Email")]
		[PXDefault(typeof(Search<CR.Contact.eMail,
			Where<Contact.contactID, Equal<Current<Customer.primaryContactID>>,
				And<Contact.bAccountID, Equal<Current<Customer.bAccountID>>>>>))]
		public virtual string EmailId
		{
			get;
			set;
		}
		#endregion

		#region Template
		public abstract class template : PX.Data.BQL.BqlString.Field<template> { }
		/// <summary>
		/// The email template for a certificate request.
		/// </summary>
		[PXDBString(100, IsUnicode = true)]
		[PXUIField(DisplayName = "Certificate Request Template")]
		[ECMCertificateTemplateSelector(typeof(CertificateTemplate.templateName), ValidateValue = false)]
		[PXDefault()]
		public virtual string Template { get; set; }
		#endregion

		public class ECMDefaultCountry
		{
			public const string US = "US";
			public class uSCountry : PX.Data.BQL.BqlString.Constant<uSCountry>
			{
				public uSCountry() : base(US) { }
			}
		}

		#region CountryID
		public abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }
		/// <summary>
		/// The country for which the certificate is requested.
		/// </summary>
		[PXDBString(2, IsUnicode = true)]
		[PXUIField(DisplayName = "Country")]
		[PXSelector(typeof(PX.Objects.CS.Country.countryID), DescriptionField = typeof(PX.Objects.CS.Country.description))]
		[PXDefault(typeof(Search<CS.Country.countryID,
			Where<CS.Country.countryID, Equal<ECMDefaultCountry.uSCountry>>>))]
		public virtual string CountryID { get; set; }

		#endregion
		#region State
		public abstract class state : PX.Data.BQL.BqlString.Field<state> { }
		/// <summary>
		/// The state of the country for which the certificate is requested.
		/// </summary>
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "State")]
		[State(typeof(RequestECMCertificateFilter.countryID))]
		[PXDefault()]
		public virtual String State
		{
			get;
			set;
		}
		#endregion
		#region ExemptReason
		public abstract class exemptReason : PX.Data.BQL.BqlString.Field<exemptReason> { }
		/// <summary>
		/// The reason for the requested exemption certificate.
		/// </summary>
		[PXDBString(100, IsUnicode = true)]
		[PXUIField(DisplayName = "Reason for Exemption")]
		[ECMExemptionReasonSelector(DescriptionField = typeof(CertificateReason.reasonName))]
		public virtual string ExemptReason { get; set; }
		#endregion
	}
}

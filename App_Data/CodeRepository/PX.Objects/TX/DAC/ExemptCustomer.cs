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
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.CR;

namespace PX.Objects.TX
{
	/// <summary>
	/// A projection over <see cref="Customer"/> that selects all active customer records with the parimary contact and default address information.
	/// </summary>
	[PXProjection(typeof(SelectFrom<Customer>.
		InnerJoin<CR.BAccount>.On<CR.BAccount.bAccountID.IsEqual<Customer.bAccountID>>.
		InnerJoin<CR.Address>.On<CR.Address.bAccountID.IsEqual<Customer.bAccountID>.
			And<CR.Address.addressID.IsEqual<BAccount.defAddressID>>>.
		InnerJoin<CR.Contact>.On<CR.Contact.bAccountID.IsEqual<Customer.bAccountID>.
			And<CR.Contact.contactID.IsEqual<CR.BAccount.primaryContactID>>>.
		InnerJoin<CR.Location>.On<CR.Location.bAccountID.IsEqual<Customer.bAccountID>.
			And<CR.Location.locationID.IsEqual<CR.BAccount.defLocationID>>>.
		Where<CR.BAccount.status.IsEqual<CustomerStatus.active>>), Persistent = true)]
	[PXHidden]
	[Serializable]
	public partial class ExemptCustomer : Customer
	{
		#region Keys
		public new class PK : PrimaryKeyOf<ExemptCustomer>.By<bAccountID>
		{
			public static ExemptCustomer Find(PXGraph graph, int? bAccountID, PKFindOptions options = PKFindOptions.None)
				=> FindBy(graph, bAccountID, options);
		}

		public new static class FK
		{
			public class Address : CR.Address.PK.ForeignKeyOf<Customer>.By<defAddressID> { }
			public class ContactInfo : CR.Contact.PK.ForeignKeyOf<Customer>.By<defContactID> { }
			public class DefaultLocation : CR.Location.PK.ForeignKeyOf<Customer>.By<bAccountID, defLocationID> { }
		}
		#endregion

		public new abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
		public new abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
		public new abstract class defAddressID : PX.Data.BQL.BqlInt.Field<defAddressID> { }
		public new abstract class primaryContactID : PX.Data.BQL.BqlInt.Field<primaryContactID> { }
		public new abstract class defLocationID : PX.Data.BQL.BqlInt.Field<defLocationID> { }
		public new abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }


		#region CustomerID
		public abstract class customerID : PX.Data.BQL.BqlString.Field<customerID> { }

		/// <summary>
		/// The human-readable identifier of the customer account.
		/// </summary>
		[PXDBString(30, IsUnicode = true, BqlField = typeof(BAccount.acctCD))]
		[PXUIField(DisplayName = "Customer ID")]
		public virtual string CustomerID { get; set; }
		#endregion

		#region CustomerName
		public abstract class customerName : PX.Data.BQL.BqlString.Field<customerName> { }

		/// <summary>
		/// The full name of the customer.
		/// </summary>
		[PXDBString(30, IsUnicode = true, BqlField = typeof(BAccount.acctName))]
		[PXUIField(DisplayName = "Customer Name")]
		public virtual string CustomerName { get; set; }
		#endregion

		#region CustomerTaxRegistrationID
		public abstract class customerTaxRegistrationID : PX.Data.BQL.BqlString.Field<customerTaxRegistrationID> { }

		/// <summary>
		/// The tax registration ID of the customer.
		/// </summary>
		[PXDBString(30, IsUnicode = true, BqlField = typeof(CR.Location.taxRegistrationID))]
		[PXUIField(DisplayName = "Tax Registration ID")]
		public virtual string CustomerTaxRegistrationID { get; set; }
		#endregion

		#region AddressLine1
		public abstract class addressLine1 : PX.Data.BQL.BqlString.Field<addressLine1> { }

		/// <summary>
		/// The first line of the cutomer address.
		/// </summary>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Address.addressLine1))]
		[PXUIField(DisplayName = "Address Line 1")]
		public virtual string AddressLine1 { get; set; }
		#endregion

		#region AddressLine2
		public abstract class addressLine2 : PX.Data.BQL.BqlString.Field<addressLine2> { }

		/// <summary>
		/// The second line of the customer address.
		/// </summary>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Address.addressLine2))]
		[PXUIField(DisplayName = "Address Line 2")]
		public virtual string AddressLine2 { get; set; }
		#endregion

		#region City
		public abstract class city : PX.Data.BQL.BqlString.Field<city> { }

		/// <summary>
		/// The city of the customer address.
		/// </summary>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Address.city))]
		[PXUIField(DisplayName = "City")]
		public virtual string City { get; set; }
		#endregion

		#region State
		public abstract class state : PX.Data.BQL.BqlString.Field<state> { }

		/// <summary>
		/// The state of the customer address.
		/// </summary>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Address.state))]
		[PXUIField(DisplayName = "State")]
		public virtual string State { get; set; }
		#endregion

		#region PostalCode
		public abstract class postalCode : PX.Data.BQL.BqlString.Field<postalCode> { }

		/// <summary>
		/// The postal code of the customer address.
		/// </summary>
		[PXDBString(20, IsUnicode = true, BqlField = typeof(CR.Address.postalCode))]
		[PXUIField(DisplayName = "Postal Code")]
		public virtual string PostalCode { get; set; }
		#endregion

		#region CountryID
		public abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }

		/// <summary>
		/// The country of the customer address.
		/// </summary>
		[PXDBString(2, IsFixed = true, BqlField = typeof(CR.Address.countryID))]
		[PXUIField(DisplayName = "Country")]
		public virtual string CountryID { get; set; }
		#endregion

		#region PrimaryContact
		public abstract class primaryContact : PX.Data.BQL.BqlString.Field<primaryContact> { }

		/// <summary>
		/// The primary contact of the customer.
		/// </summary>
		[PXDBString(BqlField = typeof(CR.Contact.displayName))]
		[PXUIField(DisplayName = "Primary Contact")]
		public virtual string PrimaryContact { get; set; }
		#endregion

		#region PhoneNumber
		public abstract class phoneNumber : PX.Data.BQL.BqlString.Field<phoneNumber> { }

		/// <summary>
		/// The phone number of the customer account.
		/// </summary>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Contact.phone1))]
		[PXUIField(DisplayName = "Phone Number")]
		public virtual string PhoneNumber { get; set; }
		#endregion

		#region Email
		public abstract class email : PX.Data.BQL.BqlString.Field<email> { }

		/// <summary>
		/// The email address of the customer account.
		/// </summary>
		[PXDBString(BqlField = typeof(CR.Contact.eMail))]
		[PXUIField(DisplayName = "Account Email")]
		public virtual string Email { get; set; }
		#endregion

		#region Fax
		public abstract class fax : PX.Data.BQL.BqlString.Field<fax> { }

		/// <summary>
		/// The fax number of the customer account.
		/// </summary>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Contact.fax))]
		[PXUIField(DisplayName = "Fax")]
		public virtual string Fax { get; set; }
		#endregion

		#region ECMCompanyCode
		public abstract class eCMCompanyCode : PX.Data.BQL.BqlString.Field<eCMCompanyCode> { }

		/// <summary>
		/// The company code for which the customer is created in the exemption certificate management (ECM) system.
		/// </summary>
		[PXDBString(BqlField = typeof(Customer.eCMCompanyCode))]
		[PXUIField(DisplayName = "Company Code")]
		public virtual string ECMCompanyCode { get; set; }
		#endregion

	}
}

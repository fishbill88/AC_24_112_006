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
using PX.Data.BQL;
using PX.Objects.IN;

namespace PX.Objects.PO.DAC.Projections
{
	/// <summary>
	/// A projection that is used in the Purchase Receipt (PO646000) report to display information about the warehouse.
	/// </summary>
	[PXCacheName(Messages.SiteInfo)]
	[PXProjection(typeof(Select2<INSite,
		InnerJoin<CR.Address, On<INSite.FK.Address>,
		InnerJoin<CR.Contact, On<INSite.FK.Contact>>>,
		Where<INSite.siteID.IsNotEqual<SiteAttribute.transitSiteID>>>))]
	public class SiteInfo : PXBqlTable, IBqlTable
	{
		#region SiteID
		/// <inheritdoc cref="INSite.SiteID"/>
		[PXDBInt(BqlField = typeof(INSite.siteID))]
		public virtual Int32? SiteID { get; set; }
		public abstract class siteID : BqlInt.Field<siteID> { }
		#endregion

		#region Descr
		/// <inheritdoc cref="INSite.Descr"/>
		[PXDBString(60, IsUnicode = true, BqlField = typeof(INSite.descr))]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr { get; set; }
		public abstract class descr : BqlString.Field<descr> { }
		#endregion

		#region Address Fields

		#region AddressLine1
		/// <inheritdoc cref="CR.Address.AddressLine1"/>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Address.addressLine1))]
		[PXUIField(DisplayName = "Address Line 1", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String AddressLine1 { get; set; }
		public abstract class addressLine1 : PX.Data.BQL.BqlString.Field<addressLine1> { }
		#endregion

		#region AddressLine2
		/// <inheritdoc cref="CR.Address.AddressLine2"/>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Address.addressLine2))]
		[PXUIField(DisplayName = "Address Line 2")]
		public virtual String AddressLine2 { get; set; }
		public abstract class addressLine2 : PX.Data.BQL.BqlString.Field<addressLine2> { }
		#endregion

		#region AddressLine3
		/// <inheritdoc cref="CR.Address.AddressLine3"/>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Address.addressLine3))]
		[PXUIField(DisplayName = "Address Line 3")]
		public virtual String AddressLine3 { get; set; }
		public abstract class addressLine3 : PX.Data.BQL.BqlString.Field<addressLine3> { }
		#endregion

		#region City
		/// <inheritdoc cref="CR.Address.City"/>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Address.city))]
		[PXUIField(DisplayName = "City", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String City { get; set; }
		public abstract class city : PX.Data.BQL.BqlString.Field<city> { }
		#endregion

		#region CountryID
		/// <inheritdoc cref="CR.Address.CountryID"/>
		[PXDBString(100, BqlField = typeof(CR.Address.countryID))]
		[PXUIField(DisplayName = "Country")]
		[CR.Country]
		public virtual String CountryID { get; set; }
		public abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }
		#endregion

		#region State
		/// <inheritdoc cref="CR.Address.State"/>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Address.state))]
		[PXUIField(DisplayName = "State")]
		[CR.State(typeof(countryID))]
		public virtual String State { get; set; }
		public abstract class state : PX.Data.BQL.BqlString.Field<state> { }
		#endregion

		#region PostalCode
		/// <inheritdoc cref="CR.Address.PostalCode"/>
		[PXDBString(20, BqlField = typeof(CR.Address.postalCode))]
		[PXUIField(DisplayName = "Postal Code")]
		public virtual String PostalCode { get; set; }
		public abstract class postalCode : PX.Data.BQL.BqlString.Field<postalCode> { }
		#endregion
		#endregion

		#region Contact Fields

		#region Phone1
		/// <inheritdoc cref="CR.Contact.Phone1"/>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Contact.phone1))]
		[PXUIField(DisplayName = "Phone 1", Visibility = PXUIVisibility.SelectorVisible)]
        [PXPhone]
		public virtual String Phone1 { get; set; }
		public abstract class phone1 : PX.Data.BQL.BqlString.Field<phone1> { }
		#endregion

		#region Fax
		/// <inheritdoc cref="CR.Contact.Fax"/>
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CR.Contact.fax))]
		[PXUIField(DisplayName = "Fax")]
		public virtual String Fax { get; set; }
		public abstract class fax : PX.Data.BQL.BqlString.Field<fax> { }
		#endregion

		#region Attention
		/// <inheritdoc cref="CR.Contact.Attention"/>
		[PXDBString(255, IsUnicode = true, BqlField = typeof(CR.Contact.attention))]
		[PXUIField(DisplayName = "Attention")]
		public virtual String Attention { get; set; }
		public abstract class attention : PX.Data.BQL.BqlString.Field<attention> { }
		#endregion
		#endregion
	}
}

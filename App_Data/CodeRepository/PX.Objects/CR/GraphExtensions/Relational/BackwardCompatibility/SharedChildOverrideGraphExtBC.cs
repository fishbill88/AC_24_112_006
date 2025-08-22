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
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;

namespace PX.Objects.CR.Extensions.Relational.BC
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class BAccountLocationSharedContactOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		BusinessAccountMaint.BAccountLocationSharedContactOverrideGraphExt,
		BusinessAccountMaint,
		CR.Standalone.Location.isContactSameAsMain,
		CR.Standalone.Location.overrideContact> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class BAccountLocationSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		BusinessAccountMaint.BAccountLocationSharedAddressOverrideGraphExt,
		BusinessAccountMaint,
		CR.Standalone.Location.isAddressSameAsMain,
		CR.Standalone.Location.overrideAddress> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class AccountLocationBAccountSharedContactOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		AccountLocationMaint.LocationBAccountSharedContactOverrideGraphExt,
		AccountLocationMaint,
		Location.isContactSameAsMain,
		Location.overrideContact> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class AccountLocationBAccountSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		AccountLocationMaint.LocationBAccountSharedAddressOverrideGraphExt,
		AccountLocationMaint,
		Location.isAddressSameAsMain,
		Location.overrideAddress> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class CustomerLocationBAccountSharedContactOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		CustomerLocationMaint.LocationBAccountSharedContactOverrideGraphExt,
		CustomerLocationMaint,
		Location.isContactSameAsMain,
		Location.overrideContact>
	{ }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class CustomerLocationBAccountSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		CustomerLocationMaint.LocationBAccountSharedAddressOverrideGraphExt,
		CustomerLocationMaint,
		Location.isAddressSameAsMain,
		Location.overrideAddress>
	{ }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class VendorLocationBAccountSharedContactOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		VendorLocationMaint.LocationBAccountSharedContactOverrideGraphExt,
		VendorLocationMaint,
		Location.isContactSameAsMain,
		Location.overrideContact>
	{ }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class VendorLocationBAccountSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		VendorLocationMaint.LocationBAccountSharedAddressOverrideGraphExt,
		VendorLocationMaint,
		Location.isAddressSameAsMain,
		Location.overrideAddress>
	{ }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class ContactBAccountSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		ContactMaint.ContactBAccountSharedAddressOverrideGraphExt,
		ContactMaint,
		Contact.isAddressSameAsMain,
		Contact.overrideAddress> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class LeadBAccountSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		LeadMaint.LeadBAccountSharedAddressOverrideGraphExt,
		LeadMaint,
		CRLead.isAddressSameAsMain,
		CRLead.overrideAddress> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class CustomerBillSharedContactOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		CustomerMaint.CustomerBillSharedContactOverrideGraphExt,
		CustomerMaint,
		Customer.isBillContSameAsMain,
		Customer.overrideBillContact> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class CustomerBillSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		CustomerMaint.CustomerBillSharedAddressOverrideGraphExt,
		CustomerMaint,
		Customer.isBillSameAsMain,
		Customer.overrideBillAddress> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class CustomerSharedContactOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		CustomerMaint.CustomerDefSharedContactOverrideGraphExt,
		CustomerMaint,
		CR.Standalone.Location.isContactSameAsMain,
		CR.Standalone.Location.overrideContact> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class CustomerSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		CustomerMaint.CustomerDefSharedAddressOverrideGraphExt,
		CustomerMaint,
		CR.Standalone.Location.isAddressSameAsMain,
		CR.Standalone.Location.overrideAddress> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class VendorDefSharedContactOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		VendorMaint.VendorDefSharedContactOverrideGraphExt,
		VendorMaint,
		CR.Standalone.Location.isContactSameAsMain,
		CR.Standalone.Location.overrideContact> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class VendorDefSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		VendorMaint.VendorDefSharedAddressOverrideGraphExt,
		VendorMaint,
		CR.Standalone.Location.isAddressSameAsMain,
		CR.Standalone.Location.overrideAddress> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class VendorRemitSharedContactOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		VendorMaint.VendorRemitSharedContactOverrideGraphExt,
		VendorMaint,
		CR.Standalone.Location.isRemitContactSameAsMain,
		CR.Standalone.Location.overrideRemitContact> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class VendorRemitSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		VendorMaint.VendorRemitSharedAddressOverrideGraphExt,
		VendorMaint,
		CR.Standalone.Location.isRemitAddressSameAsMain,
		CR.Standalone.Location.overrideRemitAddress> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class LocationRemitSharedContactOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		VendorLocationMaint.LocationRemitSharedContactOverrideGraphExt,
		VendorLocationMaint,
		LocationAPPaymentInfo.isRemitContactSameAsMain,
		LocationAPPaymentInfo.overrideRemitContact> { }

	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	[PXInternalUseOnly]
	[Obsolete("To use only for C-b Adapter purposes")]
	public class LocationRemitSharedAddressOverrideGraphExtBC : SharedChildOverrideGraphExtBC<
		VendorLocationMaint.LocationRemitSharedAddressOverrideGraphExt,
		VendorLocationMaint,
		LocationAPPaymentInfo.isRemitAddressSameAsMain,
		LocationAPPaymentInfo.overrideRemitAddress> { }

	[PXInternalUseOnly]
	public abstract class SharedChildOverrideGraphExtBC<Extension, Graph, FieldFrom, FieldTo> : PXGraphExtension<Extension, Graph>
		where Graph : PXGraph
		where Extension : PXGraphExtension<Graph>
		where FieldFrom : class, IBqlField
		where FieldTo : class, IBqlField
	{
		protected void _(Events.FieldUpdating<FieldFrom> e)
		{
			var boolValue = e.NewValue as bool?;
			var stringValue = e.NewValue as string;

			if (boolValue != null)
			{
				e.Cache.SetValueExt<FieldTo>(e.Row, !boolValue);
			}
			else if (stringValue != null && bool.TryParse(stringValue, out bool result))
			{
				e.Cache.SetValueExt<FieldTo>(e.Row, !result);
			}
		}
	}
}

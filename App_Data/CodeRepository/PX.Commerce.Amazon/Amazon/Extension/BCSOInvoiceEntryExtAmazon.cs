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
using PX.Objects.AR;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.TX;
using PX.Common;
using PX.TaxProvider;
using PX.Objects.Common.Extensions;
using PX.Data.EP;
using PX.Data.BQL;
using PX.Objects.CM;
using PX.Commerce.Core;
using PX.Objects.CS;
using PX.Commerce.Objects;

namespace PX.Commerce.Amazon
{
	public class BCSOInvoiceEntryExtAmazon : PXGraphExtension<BCSOInvoiceEntryExt, SOInvoiceEntry>
	{
		public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.amazonIntegration>(); }
		public delegate void UpdateIsEncryptedValueDelegate();
		[PXOverride]
		public virtual void UpdateIsEncryptedValue(UpdateIsEncryptedValueDelegate baseMethod)
		{
			BCAPISyncScope.BCSyncScopeContext context = BCAPISyncScope.GetScoped();
			if (context != null)
			{
				foreach (ARInvoice doc in Base.Document.Cache.Inserted
							   .Concat_(Base.Document.Cache.Updated)
							   .Cast<ARInvoice>())
				{
					SOInvoice soInvoice = Base.SODocument.Select(doc.DocType, doc.RefNbr);

					ARShippingContact shippingContact = Base.Shipping_Contact.Select(doc.ShipContactID);
					var overriden = shippingContact?.OverrideContact;
					var shippingContactCopy = (ARShippingContact)Base.Shipping_Contact.Cache.CreateCopy(shippingContact);
					shippingContactCopy.IsEncrypted = overriden;
					Base.Shipping_Contact.Cache.Update(shippingContactCopy);

					ARContact contact = Base.Billing_Contact.Select(doc.BillContactID);
					overriden = contact?.OverrideContact;
					var contactCopy = (ARContact)Base.Billing_Contact.Cache.CreateCopy(contact);
					contactCopy.IsEncrypted = overriden;
					Base.Billing_Contact.Cache.Update(contactCopy);

					ARShippingAddress shippingAddress = Base.Shipping_Address.Select(doc.ShipAddressID);
					overriden = shippingAddress?.OverrideAddress;
					var shippingAddressCopy = (ARShippingAddress)Base.Shipping_Address.Cache.CreateCopy(shippingAddress);
					shippingAddressCopy.IsEncrypted = overriden;
					Base.Shipping_Address.Cache.Update(shippingAddressCopy);

					ARAddress address = Base.Billing_Address.Select(doc.BillAddressID);
					overriden = address?.OverrideAddress;
					var addressCopy = (ARAddress)Base.Billing_Address.Cache.CreateCopy(address);
					addressCopy.IsEncrypted = overriden;
					Base.Billing_Address.Cache.Update(addressCopy);
				}
			}
			else
				baseMethod();
		}
	}
}

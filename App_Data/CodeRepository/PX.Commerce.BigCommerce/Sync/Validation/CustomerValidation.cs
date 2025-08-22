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
using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Commerce.BigCommerce.API.REST;

namespace PX.Commerce.BigCommerce
{
	public class CustomerValidator : BCBaseValidator, ISettingsValidator, IExternValidator
	{
		public int Priority { get { return 0; } }

		public virtual void Validate(IProcessor iproc)
		{
			Validate<BCCustomerProcessor>(iproc, (processor) =>
			{
				BCBinding binding = processor.GetBinding();
				BCBindingExt storeExt = processor.GetBindingExt<BCBindingExt>();
				if (storeExt.CustomerNumberingID == null && BCDimensionMaskAttribute.GetAutoNumbering(CustomerRawAttribute.DimensionName) == null)
					throw new PXException(BigCommerceMessages.NoCustomerNumbering);

				//Validate  length of segmented key and Number sequence matches
				BCDimensionMaskAttribute.VerifyNumberSequenceLength(storeExt.CustomerNumberingID, storeExt.CustomerTemplate, CustomerRawAttribute.DimensionName, binding.BranchID, processor.Accessinfo.BusinessDate);

				if (storeExt.CustomerClassID == null)
				{
					ARSetup arSetup = PXSelect<ARSetup>.Select(processor);
					if (arSetup.DfltCustomerClassID == null)
						throw new PXException(BigCommerceMessages.NoCustomerClass);
				}

			});
			Validate<BCLocationProcessor>(iproc, (processor) =>
			{
				BCBinding binding = processor.GetBinding();
				BCBindingExt storeExt = processor.GetBindingExt<BCBindingExt>();
				if (storeExt.CustomerNumberingID == null && BCDimensionMaskAttribute.GetAutoNumbering(CustomerRawAttribute.DimensionName) == null)
					throw new PXException(BigCommerceMessages.NoCustomerNumbering);
				if (storeExt.LocationNumberingID == null && BCDimensionMaskAttribute.GetAutoNumbering(LocationIDAttribute.DimensionName) == null)
					throw new PXException(BigCommerceMessages.NoLocationNumbering);

				//Validate  length of segmented key and Number sequence matches
				BCDimensionMaskAttribute.VerifyNumberSequenceLength(storeExt.LocationNumberingID, storeExt.LocationTemplate, LocationIDAttribute.DimensionName, binding.BranchID, processor.Accessinfo.BusinessDate);

			});
		}

		public virtual void Validate(IProcessor iproc, IExternEntity ientity)
		{
			Validate<BCCustomerProcessor, CustomerData>(iproc, ientity, (processor, entity) =>
			{
				if(String.IsNullOrWhiteSpace(entity.Email))
					throw new PXException(BigCommerceMessages.NoRequiredField, PXMessages.LocalizeNoPrefix(BCAPICaptions.Email), PXMessages.LocalizeNoPrefix(BCAPICaptions.Customer));

				if (String.IsNullOrWhiteSpace(entity.FirstName) || String.IsNullOrWhiteSpace(entity.LastName))
					throw new PXException(BigCommerceMessages.NoRequiredField, PXMessages.LocalizeNoPrefix(BCAPICaptions.FullName), PXMessages.LocalizeNoPrefix(BCAPICaptions.Customer));
			});
			Validate<BCCustomerProcessor, CustomerAddressData>(iproc, ientity, (processor, entity) =>
			{
				if (String.IsNullOrWhiteSpace(entity.PostalCode))
					throw new PXException(BigCommerceMessages.NoRequiredField, PXMessages.LocalizeNoPrefix(BCAPICaptions.PostalCode), PXMessages.LocalizeNoPrefix(BCAPICaptions.Customer));
			});
			Validate<BCLocationProcessor, CustomerAddressData>(iproc, ientity, (processor, entity) =>
			{
				if (String.IsNullOrWhiteSpace(entity.PostalCode))
					throw new PXException(BigCommerceMessages.NoRequiredField, PXMessages.LocalizeNoPrefix(BCAPICaptions.PostalCode), PXMessages.LocalizeNoPrefix(BCAPICaptions.Customer));
			});
		}
	}
}

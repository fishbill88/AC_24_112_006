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
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.Objects.CS;

namespace PX.Commerce.Amazon
{
	public class AmazonFBASalesOrderValidation : BCBaseValidator, ISettingsValidator, ILocalValidator
	{
		public int Priority { get { return 0; } }

		public virtual void Validate(IProcessor iproc)
		{
			Validate<AmazonFBASalesOrderProcessor>(iproc, (processor) =>
			{
				BCBinding store = processor.GetBinding();
				if (store.BranchID == null)
					throw new PXException(AmazonMessages.NoBranch);

				ARSetup arSettype = PXSelect<ARSetup>.Select(processor);
				if (arSettype?.MigrationMode == true)
					throw new PXException(BCMessages.MigrationModeOnSO);

				BCBindingAmazon bindingAmazon = processor.GetBindingExt<BCBindingAmazon>();
				if (string.IsNullOrEmpty(bindingAmazon.AmazonFulfilledOrderType))
					throw new PXException(AmazonMessages.CannotProcessBecauseAmazonFulfilledOrderTypeIsEmpty);
			});
		}

		public void Validate(IProcessor iproc, ILocalEntity ilocal)
		{
			Validate<AmazonFBASalesOrderProcessor, SalesOrder>(iproc, ilocal, (processor, entity) =>
			{
				BCBindingExt storeExt = processor.GetBindingExt<BCBindingExt>();
				BCBindingAmazon bindingAmazon = processor.GetBindingExt<BCBindingAmazon>();
				if (storeExt.PostDiscounts == BCPostDiscountAttribute.DocumentDiscount && entity.DiscountDetails?.Count > 0 && PXAccess.FeatureInstalled<FeaturesSet.customerDiscounts>() == false)
					throw new PXException(BCMessages.DocumentDiscountSOMsg);

				if (string.IsNullOrEmpty(bindingAmazon.AmazonFulfilledOrderType))
					throw new PXException(AmazonMessages.CannotProcessBecauseAmazonFulfilledOrderTypeIsEmpty);

				var orderType = SOOrderType.PK.Find(processor, bindingAmazon.AmazonFulfilledOrderType);
				if (orderType?.GetExtension<SOOrderTypeExt>().EncryptAndPseudonymizePII != true)
					throw new PXException(AmazonMessages.PIIDisabled, orderType?.OrderType);
			});
		}
	}
}

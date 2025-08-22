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

using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Data;
using PX.Objects.CS;

namespace PX.Commerce.Shopify
{
	public class ProductValidator : BCBaseValidator, ISettingsValidator
	{
		public int Priority { get { return 0; } }

		public virtual void Validate(IProcessor iproc)
		{
			Validate<SPTemplateItemProcessor>(iproc, (processor) =>
			{
				if (PXAccess.FeatureInstalled<FeaturesSet.matrixItem>() == false)
					throw new PXException(BCMessages.MatrixFeatureRequired);

				if (processor.GetEntity().Direction == BCSyncDirectionAttribute.Export)
					return;

				if (processor.GetBindingExt<BCBindingExt>().InventoryNumberingID is null)
					throw new PXException(ShopifyMessages.PINoInventoryNumbering);
			});

			Validate<SPStockItemProcessor>(iproc, (processor) =>
			{
				if (processor.GetEntity().Direction == BCSyncDirectionAttribute.Export)
					return;

				if (processor.GetBindingExt<BCBindingExt>().StockItemClassID is null)
					throw new PXException(ShopifyMessages.PINoStockItemClass);
			});

			Validate<SPNonStockItemProcessor>(iproc, (processor) =>
			{
				if (processor.GetEntity().Direction == BCSyncDirectionAttribute.Export)
					return;

				if (processor.GetBindingExt<BCBindingExt>().NonStockItemClassID is null)
					throw new PXException(ShopifyMessages.PINoNonStockItemClass);
			});
		}
	}
}

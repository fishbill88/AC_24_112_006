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
using PX.Data;
using PX.Objects.IN;

namespace PX.Commerce.Objects
{
	/// <summary>
	/// Creates settings for product import service from the current processor.
	/// </summary>
	public interface IProductImportSettingsBuilder
	{
		/// <summary>
		/// Gets the settings from the current processor and connector.
		/// </summary>
		/// <param name="processor"></param>
		/// <returns></returns>
		public ProductImportSettings GetSettingsInstance(IProcessor processor);
	}
	///<inheritdoc/>
	public class ProductImportSettingsBuilder : IProductImportSettingsBuilder
	{
		///<inheritdoc/>
		public ProductImportSettings GetSettingsInstance(IProcessor processor)
		{
			var productImportSettings = new ProductImportSettings();
			Objects.BCBindingExt bindingExt = processor.GetBindingExt<Objects.BCBindingExt>();			
			productImportSettings.InventoryNumberingID = bindingExt?.InventoryNumberingID;
			productImportSettings.ItemClassSubstitutionListID = bindingExt?.ProductItemClassSubstitutionListID;
			productImportSettings.TaxCategorySubstitutionListID = bindingExt?.TaxCategorySubstitutionListID;
			productImportSettings.EntityType = processor.Operation.EntityType;
			productImportSettings.ConnectorType = processor.Operation.ConnectorType;
			productImportSettings.BindingId = processor.Operation.Binding;
			productImportSettings.StockItemDefaultItemClassCD = INItemClass.PK.Find(processor as PXGraph, bindingExt?.StockItemClassID)?.ItemClassCD.Trim();
			productImportSettings.NonStockItemDefaultItemClassCD = INItemClass.PK.Find(processor as PXGraph, bindingExt?.NonStockItemClassID)?.ItemClassCD.Trim();
			return productImportSettings;
		}
	}
}

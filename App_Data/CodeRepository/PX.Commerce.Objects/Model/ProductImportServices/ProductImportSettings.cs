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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Objects
{	
	///<inheritdoc/>
	public class ProductImportSettings
	{
		/// <inheritdoc/>
		public int? BindingId { get; set; }

		/// <inheritdoc/>
		public string ConnectorType { get; set; }
		/// <inheritdoc/>
		public string EntityType { get; set; }

		/// <inheritdoc/>
		public string InventoryNumberingID { get; set; }

		/// <inheritdoc/>
		public string ItemClassSubstitutionListID { get; set; }

		/// <inheritdoc/>
		public string TaxCategorySubstitutionListID { get; set; }

		/// <inheritdoc/>
		public string StockItemDefaultItemClassCD { get; set; }

		/// <inheritdoc/>
		public string NonStockItemDefaultItemClassCD { get; set; }

		/// <inheritdoc/>
		public string SubEntityTypeForTemplate { get; set; }
		
		/// <inheritdoc/>
		public string GetDefaultItemClassID()
		{
			if (IsTemplateItem)
			{
				return string.Empty;
			}
			else if (IsStockItem)
			{
				return StockItemDefaultItemClassCD;
			}
			else if (IsNonStockItem)
			{
				return NonStockItemDefaultItemClassCD;
			}

			return null;
		}

		/// <inheritdoc/>
		public bool IsStockItem
		{
			get
			{
				if (IsTemplateItem)
					return SubEntityTypeForTemplate == BCEntitiesAttribute.StockItem;

				return EntityType == BCEntitiesAttribute.StockItem;
			}
		}

		/// <inheritdoc/>
		public bool IsNonStockItem
		{
			get
			{
				if (IsTemplateItem)
					return SubEntityTypeForTemplate == BCEntitiesAttribute.NonStockItem;

				return EntityType == BCEntitiesAttribute.NonStockItem;
			}
		}

		/// <inheritdoc/>
		public bool IsTemplateItem
		{
			get
			{
				return EntityType == BCEntitiesAttribute.ProductWithVariant;
			}
		}
	}
}

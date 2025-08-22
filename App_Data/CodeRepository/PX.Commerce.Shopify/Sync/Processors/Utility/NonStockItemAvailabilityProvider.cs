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

using PX.Api.ContractBased.Models;
using PX.Commerce.Core;
using PX.Commerce.Objects;

namespace PX.Commerce.Shopify.Sync.Processors.Utility
{
	/// <summary>
	/// Provides a functionality to obtain a non-stock item availability.
	/// </summary>
	public sealed class NonStockItemAvailabilityProvider : IAvailabilityProvider
	{
		/// <summary>
		/// Gets the non-stock item availability basing on the <paramref name="inventoryManagement"/>.
		/// </summary>
		/// <param name="inventoryManagement">The Shopify inventory management field.</param>
		/// <returns>The non-stock item availability.</returns>
		public StringValue GetAvailablity(string inventoryManagement)
		{
			string availability = null;

			if (inventoryManagement is null || inventoryManagement.Equals(ShopifyConstants.InventoryManagement_Shopify))
				availability = BCItemAvailabilities.AvailableSkip;

			return availability?.ValueField();
		}
	}
}

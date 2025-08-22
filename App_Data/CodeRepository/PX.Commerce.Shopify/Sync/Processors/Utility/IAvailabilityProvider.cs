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

namespace PX.Commerce.Shopify.Sync.Processors.Utility
{
	/// <summary>
	/// Defines an interface to obtain a product availability.
	/// </summary>
	public interface IAvailabilityProvider
	{
		/// <summary>
		/// Gets the product item availability basing on the <paramref name="inventoryManagement"/>.
		/// </summary>
		/// <param name="inventoryManagement">The Shopify inventory management field.</param>
		/// <returns>The product item availability.</returns>
		StringValue GetAvailablity(string inventoryManagement);
	}
}

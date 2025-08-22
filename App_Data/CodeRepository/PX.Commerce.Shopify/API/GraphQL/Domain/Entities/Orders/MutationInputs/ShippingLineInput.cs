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

using Newtonsoft.Json;

namespace PX.Commerce.Shopify.API.GraphQL
{
	/// <summary>
	/// Specifies the shipping details for the order.
	/// </summary>
	public class ShippingLineInput
	{
		/// <summary>
		/// Price of the shipping rate.
		/// </summary>
		[JsonProperty("price", NullValueHandling = NullValueHandling.Ignore)]
		public decimal? Price { get; set; }

		/// <summary>
		/// A unique identifier for the shipping rate.
		/// </summary>
		[JsonProperty("shippingRateHandle", NullValueHandling = NullValueHandling.Ignore)]
		public string ShippingRateHandle { get; set; }

		/// <summary>
		/// Title of the shipping rate.
		/// </summary>
		[JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
		public string Title { get; set; }
	}
}

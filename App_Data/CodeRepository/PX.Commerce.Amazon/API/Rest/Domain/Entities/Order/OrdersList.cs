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

/* 
 * Created class based on https://developer-docs.amazon.com/sp-api/docs/orders-api-v0-model
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// A list of orders along with additional information to make subsequent API calls.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrdersList : IEntityListResponse<Order>
	{
		/// <summary>
		/// Gets or Sets Orders
		/// </summary>
		[JsonProperty("Orders")]
		public IEnumerable<Order> Data { get; set; }

		/// <summary>
		/// When present and not empty, pass this string token in the next request to return the next response page.
		/// </summary>
		/// <value>When present and not empty, pass this string token in the next request to return the next response page.</value>
		[JsonProperty("NextToken")]
		public string NextToken { get; set; }

		/// <summary>
		/// A date used for selecting orders that were last updated before (or at) a specified time. An update is defined as any change in order status, including the creation of a new order. Includes updates made by Amazon and by the seller. All dates must be in ISO 8601 format.
		/// </summary>
		/// <value>A date used for selecting orders that were last updated before (or at) a specified time. An update is defined as any change in order status, including the creation of a new order. Includes updates made by Amazon and by the seller. All dates must be in ISO 8601 format.</value>
		[JsonProperty("LastUpdatedBefore")]
		public DateTime? LastUpdatedBefore { get; set; }

		/// <summary>
		/// A date used for selecting orders created before (or at) a specified time. Only orders placed before the specified time are returned. The date must be in ISO 8601 format.
		/// </summary>
		/// <value>A date used for selecting orders created before (or at) a specified time. Only orders placed before the specified time are returned. The date must be in ISO 8601 format.</value>
		[JsonProperty("CreatedBefore")]
		public DateTime? CreatedBefore { get; set; }
	}
}

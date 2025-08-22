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
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// The order items list along with the order ID.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public  class OrderItemsList : IEntityListResponse<OrderItem>
	{
        /// <summary>
        /// Gets or Sets OrderItems
        /// </summary>
        [JsonProperty("OrderItems")]
        public IEnumerable<OrderItem> Data { get; set; }

        /// <summary>
        /// When present and not empty, pass this string token in the next request to return the next response page.
        /// </summary>
        /// <value>When present and not empty, pass this string token in the next request to return the next response page.</value>
        [JsonProperty("NextToken")]
        public string NextToken { get; set; }

        /// <summary>
        /// An Amazon-defined order identifier, in 3-7-7 format.
        /// </summary>
        /// <value>An Amazon-defined order identifier, in 3-7-7 format.</value>
        [JsonProperty("AmazonOrderId")]
        public string AmazonOrderId { get; set; }       
    }
}

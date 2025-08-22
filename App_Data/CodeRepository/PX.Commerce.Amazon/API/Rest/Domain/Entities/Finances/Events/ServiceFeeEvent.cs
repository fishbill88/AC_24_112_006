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
using PX.Commerce.Core;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing information related to a service fee on the seller's account.
	/// </summary>
	public class ServiceFeeEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets an Amazon-defined identifier for an order.
		/// </summary>
		/// <value>An Amazon-defined identifier for an order.</value>
		[JsonProperty("AmazonOrderId")]
		public string AmazonOrderId { get; set; }

		/// <summary>
		/// Gets or sets the Amazon Standard Identification Number (ASIN) of the item.
		/// </summary>
		/// <value>The Amazon Standard Identification Number (ASIN) of the item.</value>
		[JsonProperty("ASIN")]
		public string Asin { get; set; }

		/// <summary>
		/// Gets or sets a short description of the service fee event.
		/// </summary>
		/// <value>A short description of the service fee event.</value>
		[JsonProperty("FeeDescription")]
		public string FeeDescription { get; set; }

		/// <summary>
		/// Gets or sets a list of fee components associated with the service fee.
		/// </summary>
		/// <value>A list of fee components associated with the service fee.</value>
		[JsonProperty("FeeList")]
		public List<FeeComponent> FeeList { get; set; }

		/// <summary>
		/// Gets or sets a short description of the service fee reason.
		/// </summary>
		/// <value>A short description of the service fee reason.</value>
		[JsonProperty("FeeReason")]
		public string FeeReason { get; set; }

		/// <summary>
		/// Gets or sets a unique identifier assigned by Amazon to products stored in and fulfilled from an Amazon fulfillment center.
		/// </summary>
		/// <value>A unique identifier assigned by Amazon to products stored in and fulfilled from an Amazon fulfillment center.</value>
		[JsonProperty("FnSKU")]
		public string FnSku { get; set; }

		/// <summary>
		/// Gets or sets the seller SKU of the item. 
		/// </summary>
		/// <value>The seller SKU of the item. </value>
		/// <remarks>The seller SKU is qualified by the seller's seller ID, which is included with every call to the Selling Partner API.</remarks>
		[JsonProperty("SellerSKU")]
		public string SellerSku { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => this.FeeList != null;
	}
}

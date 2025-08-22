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
using System;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing an item in an adjustment to the seller's account.
	/// </summary>
	public class AdjustmentItem
	{
		/// <summary>
		/// Gets or sets the Amazon Standard Identification Number (ASIN) of the item.
		/// </summary>
		/// <value>The Amazon Standard Identification Number (ASIN) of the item.</value>
		[JsonProperty("ASIN")]
		public string Asin { get; set; }

		/// <summary>
		/// Gets or sets a unique identifier assigned to products stored in and fulfilled from a fulfillment center.
		/// </summary>
		/// <value>A unique identifier assigned to products stored in and fulfilled from a fulfillment center.</value>
		[JsonProperty("FnSKU")]
		public string FnSku { get; set; }

		/// <summary>
		/// Gets or sets the per unit value of the item.
		/// </summary>
		/// <value>The per unit value of the item.</value>
		[JsonProperty("PerUnitAmount")]
		public Currency PerUnitAmount { get; set; }

		/// <summary>
		/// Gets or sets a short description of the item.
		/// </summary>
		/// <value>A short description of the item.</value>
		[JsonProperty("ProductDescription")]
		public string ProductDescription { get; set; }

		/// <summary>
		/// Gets or sets the number of units in the seller's inventory when the AdustmentType is FBAInventoryReimbursement.
		/// </summary>
		/// <value>The number of units in the seller's inventory when the AdustmentType is FBAInventoryReimbursement.</value>
		[JsonProperty("Quantity")]
		public int Quantity { get; set; }

		/// <summary>
		/// Gets or sets the seller SKU of the item.
		/// </summary>
		/// <value>The seller SKU of the item.</value>
		/// <remarks>The seller SKU is qualified by the seller's seller ID, which is included with every call to the Selling Partner API.</remarks>
		[JsonProperty("SellerSKU")]
		public string SellerSku { get; set; }

		/// <summary>
		/// Gets or sets the  total value of the item.
		/// </summary>
		/// <value>The  total value of the item.</value>
		[JsonProperty("TotalAmount")]
		public Currency TotalAmount { get; set; }
	}
}

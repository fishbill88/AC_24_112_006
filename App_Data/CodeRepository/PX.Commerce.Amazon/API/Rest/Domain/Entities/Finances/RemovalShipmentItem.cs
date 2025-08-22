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
	/// Represents a dto for storing an item-level information for a removal shipment.
	/// </summary>
	public class RemovalShipmentItem
	{
		/// <summary>
		/// Gets or sets the fee that Amazon charged to the seller for the removal of the item.
		/// </summary>
		/// <value>The fee that Amazon charged to the seller for the removal of the item.</value>
		/// <remarks>The amount is a negative number.</remarks>
		[JsonProperty("FeeAmount")]
		public Currency FeeAmount { get; set; }

		/// <summary>
		/// Gets or sets the Amazon fulfillment network SKU for the item.
		/// </summary>
		/// <value>The Amazon fulfillment network SKU for the item.</value>
		[JsonProperty("FulfillmentNetworkSKU")]
		public string FulfillmentNetworkSku { get; set; }

		/// <summary>
		/// Gets or sets the quantity of the item.
		/// </summary>
		/// <value>The quantity of the item.</value>
		[JsonProperty("Quantity")]
		public int? Quantity { get; set; }

		/// <summary>
		/// Gets or sets an identifier for an item in a removal shipment.
		/// </summary>
		/// <value>An identifier for an item in a removal shipment.</value>
		[JsonProperty("RemovalShipmentItemId")]
		public string RemovalShipmentItemId { get; set; }

		/// <summary>
		/// Gets or sets the total amount paid to the seller for the removed item.
		/// </summary>
		/// <value>The total amount paid to the seller for the removed item.</value>
		[JsonProperty("Revenue")]
		public Currency Revenue { get; set; }

		/// <summary>
		/// Gets or sets the tax collected on the revenue.
		/// </summary>
		/// <value>The tax collected on the revenue.</value>
		[JsonProperty("TaxAmount")]
		public Currency TaxAmount { get; set; }

		/// <summary>
		/// Gets or sets the tax collection model applied to the item.
		/// </summary>
		/// <value>The tax collection model applied to the item.</value>
		[JsonProperty("TaxCollectionModel")]
		public string TaxCollectionModel { get; set; }

		/// <summary>
		/// Gets or sets the tax withheld and remitted to the taxing authority by Amazon on behalf of the seller.
		/// </summary>
		/// <value>The tax withheld and remitted to the taxing authority by Amazon on behalf of the seller.</value>
		/// <remarks>If TaxCollectionModel=MarketplaceFacilitator, then TaxWithheld=TaxAmount (except the TaxWithheld amount is a negative number). Otherwise TaxWithheld=0.</remarks>
		[JsonProperty("TaxWithheld")]
		public Currency TaxWithheld { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.Revenue != null
			&& this.Revenue.IsValid;
	}
}

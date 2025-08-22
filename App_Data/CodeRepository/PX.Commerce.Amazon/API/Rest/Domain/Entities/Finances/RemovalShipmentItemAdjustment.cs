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
	/// Represents a dto for storing an item-level information for a removal shipment item adjustment.
	/// </summary>
	public class RemovalShipmentItemAdjustment
	{
		/// <summary>
		/// Gets or sets the adjusted quantity of removal shipmentItemAdjustment items.
		/// </summary>
		/// <value>The adjusted quantity of removal shipmentItemAdjustment items.</value>
		[JsonProperty("AdjustedQuantity")]
		public int? AdjustedQuantity { get; set; }

		/// <summary>
		/// Gets or sets the Amazon fulfillment network SKU for the item.
		/// </summary>
		/// <value>The Amazon fulfillment network SKU for the item.</value>
		[JsonProperty("FulfillmentNetworkSKU")]
		public string FulfillmentNetworkSku { get; set; }

		/// <summary>
		/// Gets or sets an identifier for an item in a removal shipment.
		/// </summary>
		/// <value>An identifier for an item in a removal shipment.</value>
		[JsonProperty("RemovalShipmentItemId")]
		public string RemovalShipmentItemId { get; set; }

		/// <summary>
		/// Gets or sets the total amount adjusted for disputed items.
		/// </summary>
		/// <value>The total amount adjusted for disputed items.</value>
		[JsonProperty("RevenueAdjustment")]
		public Currency RevenueAdjustment { get; set; }

		/// <summary>
		/// Gets or sets the adjustment on the Tax collected amount on the adjusted revenue.
		/// </summary>
		/// <value>The adjustment on the Tax collected amount on the adjusted revenue.</value>
		[JsonProperty("TaxAmountAdjustment")]
		public Currency TaxAmountAdjustment { get; set; }

		/// <summary>
		/// Gets or sets the tax collection model applied to the item.
		/// </summary>
		/// <value>The tax collection model applied to the item.</value>
		[JsonProperty("TaxCollectionModel")]
		public string TaxCollectionModel { get; set; }

		/// <summary>
		/// Gets or sets the adjustment the tax withheld and remitted to the taxing authority by Amazon on behalf of the seller.
		/// </summary>
		/// <value>The adjustment the tax withheld and remitted to the taxing authority by Amazon on behalf of the seller.</value>
		/// <remarks>If TaxCollectionModel=MarketplaceFacilitator, then TaxWithheld=TaxAmount (except the TaxWithheld amount is a negative number). Otherwise TaxWithheld=0.</remarks>
		[JsonProperty("TaxWithheldAdjustment")]
		public Currency TaxWithheldAdjustment { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.RevenueAdjustment != null
			&& this.RevenueAdjustment.IsValid;
	}
}

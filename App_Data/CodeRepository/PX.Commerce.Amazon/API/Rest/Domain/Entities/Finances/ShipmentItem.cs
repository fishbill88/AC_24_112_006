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
	/// Represents a dto for storing information related to an item of a shipment, refund, guarantee claim, or chargeback.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ShipmentItem
	{
		/// <summary>
		/// Gets or sets the cost of Amazon Points granted for a shipment item.
		/// </summary>
		/// <value>The cost of Amazon Points granted for a shipment item.</value>
		[JsonProperty("CostOfPointsGranted")]
		public Currency CostOfPointsGranted { get; set; }

		/// <summary>
		/// Gets or sets the cost of Amazon Points returned for a shipment item.
		/// </summary>
		/// <value>The cost of Amazon Points returned for a shipment item.</value>
		/// <remarks>This value is only returned for refunds, guarantee claims, and chargeback events.</remarks>
		[JsonProperty("CostOfPointsReturned")]
		public Currency CostOfPointsReturned { get; set; }

		/// <summary>
		/// Gets or sets a list of charge adjustments associated with the shipment item.
		/// </summary>
		/// <value>A list of charge adjustments associated with the shipment item.</value>
		/// <remarks>This value is only returned for refunds, guarantee claims, and chargeback events.</remarks>
		[JsonProperty("ItemChargeAdjustmentList")]
		public List<ChargeComponent> ItemChargeAdjustmentList { get; set; }

		/// <summary>
		/// Gets or sets the list of charges associated with the shipment item.
		/// </summary>
		/// <value>The list of charges associated with the shipment item.</value>
		[JsonProperty("ItemChargeList")]
		public List<ChargeComponent> ItemChargeList { get; set; }

		/// <summary>
		/// Gets or sets a list of fee adjustments associated with the shipment item.
		/// </summary>
		/// <value>A list of fee adjustments associated with the shipment item.</value>
		/// <remarks>This value is only returned for refunds, guarantee claims, and chargeback events.</remarks>
		[JsonProperty("ItemFeeAdjustmentList")]
		public List<FeeComponent> ItemFeeAdjustmentList { get; set; }

		/// <summary>
		/// Gets or sets the list of fees associated with the shipment item.
		/// </summary>
		/// <value>The list of fees associated with the shipment item.</value>
		[JsonProperty("ItemFeeList")]
		public List<FeeComponent> ItemFeeList { get; set; }

		/// <summary>
		/// Gets or sets a list of taxes withheld information for a shipment item.
		/// </summary>
		/// <value>A list of taxes withheld information for a shipment item.</value>
		[JsonProperty("ItemTaxWithheldList")]
		public List<TaxWithheldComponent> ItemTaxWithheldList { get; set; }

		/// <summary>
		/// Gets or sets an Amazon-defined order adjustment identifier defined for refunds, guarantee claims, and chargeback events.
		/// </summary>
		/// <value>An Amazon-defined order adjustment identifier defined for refunds, guarantee claims, and chargeback events.</value>
		[JsonProperty("OrderAdjustmentItemId")]
		public string OrderAdjustmentItemId { get; set; }

		/// <summary>
		/// Gets or sets an Amazon-defined order item identifier.
		/// </summary>
		/// <value>An Amazon-defined order item identifier.</value>
		[JsonProperty("OrderItemId")]
		public string OrderItemId { get; set; }

		/// <summary>
		/// Gets or sets a list of promotion adjustments associated with the shipment item.
		/// </summary>
		/// <value>A list of promotion adjustments associated with the shipment item.</value>
		/// <remarks>This value is only returned for refunds, guarantee claims, and chargeback events.</remarks>
		[JsonProperty("PromotionAdjustmentList")]
		public List<Promotion> PromotionAdjustmentList { get; set; }

		/// <summary>
		/// Gets or sets a list of promotions.
		/// </summary>
		/// <value>A list of promotions.</value>
		[JsonProperty("PromotionList")]
		public List<Promotion> PromotionList { get; set; }

		/// <summary>
		/// Gets or sets the number of items shipped.
		/// </summary>
		/// <value>The number of items shipped.</value>
		[JsonProperty("QuantityShipped")]
		public int? QuantityShipped { get; set; }

		// <summary>
		/// Gets or sets the seller stock keeping unit (SKU) of the item.
		/// </summary>
		/// <value>The seller stock keeping unit (SKU) of the item.</value>
		[JsonProperty("SellerSKU")]
		public string SellerSKU { get; set; }
	}
}

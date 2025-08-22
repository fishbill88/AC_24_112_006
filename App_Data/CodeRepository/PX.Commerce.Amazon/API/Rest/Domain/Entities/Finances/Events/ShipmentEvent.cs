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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing information related to a shipment, refund, guarantee claim, or chargeback.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ShipmentEvent : BCAPIEntity
    {
		/// <summary>
		/// Gets or sets an Amazon-defined order identifier, in 3-7-7 format.
		/// </summary>
		/// <value>An Amazon-defined order identifier, in 3-7-7 format.</value>
		[JsonProperty("AmazonOrderId")]
        public string AmazonOrderId { get; set; }

		/// <summary>
		/// Gets or sets a list of transactions where buyers pay Amazon through one of the credit cards offered by Amazon or where buyers pay a seller directly through COD.
		/// </summary>
		/// <value>A list of transactions where buyers pay Amazon through one of the credit cards offered by Amazon or where buyers pay a seller directly through COD.</value>
		[JsonProperty("DirectPaymentList")]
		public List<DirectPayment> DirectPaymentList { get; set; }

		/// <summary>
		/// Gets or sets the name of the marketplace where the event occurred.
		/// </summary>
		/// <value>The name of the marketplace where the event occurred.</value>
		[JsonProperty("MarketplaceName")]
        public string MarketplaceName { get; set; }

		/// <summary>
		/// Gets or sets a list of order-level charge adjustments. 
		/// </summary>
		/// <value>A list of order-level charge adjustments.</value>
		/// <remarks>These adjustments are applicable to Multi-Channel Fulfillment COD orders.</remarks>
		[JsonProperty("OrderChargeAdjustmentList")]
		public List<ChargeComponent> OrderChargeAdjustmentList { get; set; }

		/// <summary>
		/// Gets or sets a list of order-level charges.
		/// </summary>
		/// <value>A list of order-level charges.</value>
		/// <remarks>These charges are applicable to Multi-Channel Fulfillment COD orders.</remarks>
		[JsonProperty("OrderChargeList")]
		public List<ChargeComponent> OrderChargeList { get; set; }

		/// <summary>
		/// Gets or sets a list of order-level fee adjustments.
		/// </summary>
		/// <value>A list of order-level fee adjustments.</value>
		/// <remarks>These adjustments are applicable to Multi-Channel Fulfillment orders.</remarks>
		[JsonProperty("OrderFeeAdjustmentList")]
		public List<FeeComponent> OrderFeeAdjustmentList { get; set; }

		/// <summary>
		/// Gets or sets a list of order-level fees.
		/// </summary>
		/// <value>A list of order-level fees.</value>
		/// <remarks>These charges are applicable to Multi-Channel Fulfillment orders.</remarks>
		[JsonProperty("OrderFeeList")]
		public List<FeeComponent> OrderFeeList { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
        public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets a seller-defined identifier for an order.
		/// </summary>
		/// <value>A seller-defined identifier for an order.</value>
		[JsonProperty("SellerOrderId")]
		public string SellerOrderId { get; set; }

		/// <summary>
		/// Gets or sets a list of shipment-level fee adjustments.
		/// </summary>
		/// <value>A list of shipment-level fee adjustments.</value>
		[JsonProperty("ShipmentFeeAdjustmentList")]
		public List<FeeComponent> ShipmentFeeAdjustmentList { get; set; }

		/// <summary>
		/// Gets or sets a list of shipment-level fees.
		/// </summary>
		/// <value>A list of shipment-level fees.</value>
		[JsonProperty("ShipmentFeeList")]
		public List<FeeComponent> ShipmentFeeList { get; set; }

		/// <summary>
		/// Gets or sets a list of shipment item adjustments.
		/// </summary>
		/// <value>A list of shipment item adjustments.</value>
		[JsonProperty("ShipmentItemAdjustmentList")]
		public List<ShipmentItem> ShipmentItemAdjustmentList { get; set; }

		/// <summary>
		/// Gets or sets a  list of shipment items.
		/// </summary>
		/// <value>A list of shipment items.</value>
		[JsonProperty("ShipmentItemList")]
        public List<ShipmentItem> ShipmentItemList { get; set; }
    }
}

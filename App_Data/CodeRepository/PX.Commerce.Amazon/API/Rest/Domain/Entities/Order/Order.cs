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
using Newtonsoft.Json.Converters;
using PX.Api.ContractBased.Models;
using PX.Commerce.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Order information.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[CommerceDescription(AmazonCaptions.Order, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
	public class Order : BCAPIEntity
	{

		/// <summary>
		/// The current order status.
		/// </summary>
		/// <value>The current order status.</value>
		[JsonProperty("OrderStatus")]
		[CommerceDescription(AmazonCaptions.OrderStatus, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public string OrderStatus { get; set; }


		/// <summary>
		/// Whether the order was fulfilled by Amazon (AFN) or by the seller (MFN).
		/// </summary>
		/// <value>Whether the order was fulfilled by Amazon (AFN) or by the seller (MFN).</value>
		[JsonProperty("FulfillmentChannel")]
		[CommerceDescription(AmazonCaptions.FulfillmentChannel, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public string FulfillmentChannel { get; set; }

		/// <summary>
		/// The payment method for the order. This property is limited to Cash On Delivery (COD) and Convenience Store (CVS) payment methods. Unless you need the specific COD payment information provided by the PaymentExecutionDetailItem object, we recommend using the PaymentMethodDetails property to get payment method information.
		/// </summary>
		/// <value>The payment method for the order. This property is limited to Cash On Delivery (COD) and Convenience Store (CVS) payment methods. Unless you need the specific COD payment information provided by the PaymentExecutionDetailItem object, we recommend using the PaymentMethodDetails property to get payment method information.</value>
		[JsonProperty("PaymentMethod")]
		[CommerceDescription(AmazonCaptions.PaymentMethod, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string PaymentMethod { get; set; }

		/// <summary>
		/// The type of the order.
		/// </summary>
		/// <value>The type of the order.</value>
		[JsonProperty("OrderType")]
		[CommerceDescription(AmazonCaptions.OrderType, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string OrderType { get; set; }

		/// <summary>
		/// An Amazon-defined order identifier, in 3-7-7 format.
		/// </summary>
		/// <value>An Amazon-defined order identifier, in 3-7-7 format.</value>
		[JsonProperty("AmazonOrderId")]
		[CommerceDescription(AmazonCaptions.AmazonOrderId, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public string AmazonOrderId { get; set; }

		/// <summary>
		/// A seller-defined order identifier.
		/// </summary>
		/// <value>A seller-defined order identifier.</value>
		[JsonProperty("SellerOrderId")]
		[CommerceDescription(AmazonCaptions.SellerOrderId, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string SellerOrderId { get; set; }

		/// <summary>
		/// The date when the order was created.
		/// </summary>
		/// <value>The date when the order was created.</value>
		[JsonProperty("PurchaseDate")]
		[CommerceDescription(AmazonCaptions.PurchaseDate, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public DateTime? PurchaseDate { get; set; }

		/// <summary>
		/// The date when the order was last updated.  Note: LastUpdateDate is returned with an incorrect date for orders that were last updated before 2009-04-01.
		/// </summary>
		/// <value>The date when the order was last updated.  Note: LastUpdateDate is returned with an incorrect date for orders that were last updated before 2009-04-01.</value>
		[JsonProperty("LastUpdateDate")]
		public DateTime? LastUpdateDate { get; set; }

		/// <summary>
		/// The sales channel of the first item in the order.
		/// </summary>
		/// <value>The sales channel of the first item in the order.</value>
		[JsonProperty("SalesChannel")]
		[CommerceDescription(AmazonCaptions.SalesChannel, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string SalesChannel { get; set; }

		/// <summary>
		/// The order channel of the first item in the order.
		/// </summary>
		/// <value>The order channel of the first item in the order.</value>
		[JsonProperty("OrderChannel")]
		[CommerceDescription(AmazonCaptions.OrderChannel, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string OrderChannel { get; set; }

		/// <summary>
		/// The shipment service level of the order.
		/// </summary>
		/// <value>The shipment service level of the order.</value>
		[JsonProperty("ShipServiceLevel")]
		[CommerceDescription(AmazonCaptions.ShipServiceLevel, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string ShipServiceLevel { get; set; }

		/// <summary>
		/// The total charge for this order.
		/// </summary>
		/// <value>The total charge for this order.</value>
		[JsonProperty("OrderTotal")]
		[CommerceDescription(AmazonCaptions.OrderTotal, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money OrderTotal { get; set; }

		/// <summary>
		/// The number of items shipped.
		/// </summary>
		/// <value>The number of items shipped.</value>
		[JsonProperty("NumberOfItemsShipped")]
		public int? NumberOfItemsShipped { get; set; }

		/// <summary>
		/// The number of items unshipped.
		/// </summary>
		/// <value>The number of items unshipped.</value>
		[JsonProperty("NumberOfItemsUnshipped")]
		public int? NumberOfItemsUnshipped { get; set; }

		/// <summary>
		/// Information about sub-payment methods for a Cash On Delivery (COD) order.  Note: For a COD order that is paid for using one sub-payment method, one PaymentExecutionDetailItem object is returned, with PaymentExecutionDetailItem/PaymentMethod &#x3D; COD. For a COD order that is paid for using multiple sub-payment methods, two or more PaymentExecutionDetailItem objects are returned.
		/// </summary>
		/// <value>Information about sub-payment methods for a Cash On Delivery (COD) order.  Note: For a COD order that is paid for using one sub-payment method, one PaymentExecutionDetailItem object is returned, with PaymentExecutionDetailItem/PaymentMethod &#x3D; COD. For a COD order that is paid for using multiple sub-payment methods, two or more PaymentExecutionDetailItem objects are returned.</value>
		[JsonProperty("PaymentExecutionDetail")]
		[CommerceDescription(AmazonCaptions.PaymentExecutionDetail, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public List<PaymentExecutionDetailItem> PaymentExecutionDetail { get; set; }


		/// <summary>
		/// A list of payment methods for the order.
		/// </summary>
		/// <value>A list of payment methods for the order.</value>
		[JsonProperty("PaymentMethodDetails")]
		public List<string> PaymentMethodDetails { get; set; }

		/// <summary>
		/// The identifier for the marketplace where the order was placed.
		/// </summary>
		/// <value>The identifier for the marketplace where the order was placed.</value>
		[JsonProperty("MarketplaceId")]
		[CommerceDescription(AmazonCaptions.MarketplaceId, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string MarketplaceId { get; set; }

		/// <summary>
		/// The shipment service level category of the order.  Possible values: Expedited, FreeEconomy, NextDay, SameDay, SecondDay, Scheduled, Standard.
		/// </summary>
		/// <value>The shipment service level category of the order.  Possible values: Expedited, FreeEconomy, NextDay, SameDay, SecondDay, Scheduled, Standard.</value>
		[JsonProperty("ShipmentServiceLevelCategory")]
		[CommerceDescription(AmazonCaptions.ShipmentServiceLevelCategory, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string ShipmentServiceLevelCategory { get; set; }

		/// <summary>
		/// The status of the Amazon Easy Ship order. This property is included only for Amazon Easy Ship orders.  Possible values: PendingPickUp, LabelCanceled, PickedUp, OutForDelivery, Damaged, Delivered, RejectedByBuyer, Undeliverable, ReturnedToSeller, ReturningToSeller.
		/// </summary>
		/// <value>The status of the Amazon Easy Ship order. This property is included only for Amazon Easy Ship orders.  Possible values: PendingPickUp, LabelCanceled, PickedUp, OutForDelivery, Damaged, Delivered, RejectedByBuyer, Undeliverable, ReturnedToSeller, ReturningToSeller.</value>
		[JsonProperty("EasyShipShipmentStatus")]
		public string EasyShipShipmentStatus { get; set; }

		/// <summary>
		/// Custom ship label for Checkout by Amazon (CBA).
		/// </summary>
		/// <value>Custom ship label for Checkout by Amazon (CBA).</value>
		[JsonProperty("CbaDisplayableShippingLabel")]
		public string CbaDisplayableShippingLabel { get; set; }


		/// <summary>
		/// The start of the time period within which you have committed to ship the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.  Note: EarliestShipDate might not be returned for orders placed before February 1, 2013.
		/// </summary>
		/// <value>The start of the time period within which you have committed to ship the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.  Note: EarliestShipDate might not be returned for orders placed before February 1, 2013.</value>
		[JsonProperty("EarliestShipDate")]
		[CommerceDescription(AmazonCaptions.EarliestShipDate, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public DateTime? EarliestShipDate { get; set; }

		/// <summary>
		/// The end of the time period within which you have committed to ship the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.  Note: LatestShipDate might not be returned for orders placed before February 1, 2013.
		/// </summary>
		/// <value>The end of the time period within which you have committed to ship the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.  Note: LatestShipDate might not be returned for orders placed before February 1, 2013.</value>
		[JsonProperty("LatestShipDate")]
		[CommerceDescription(AmazonCaptions.LatestShipDate, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public DateTime? LatestShipDate { get; set; }

		/// <summary>
		/// The start of the time period within which you have committed to fulfill the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.
		/// </summary>
		/// <value>The start of the time period within which you have committed to fulfill the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.</value>
		[JsonProperty("EarliestDeliveryDate")]
		[CommerceDescription(AmazonCaptions.EarliestDeliveryDate, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public DateTime? EarliestDeliveryDate { get; set; }

		/// <summary>
		/// The end of the time period within which you have committed to fulfill the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders that do not have a PendingAvailability, Pending, or Canceled status.
		/// </summary>
		/// <value>The end of the time period within which you have committed to fulfill the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders that do not have a PendingAvailability, Pending, or Canceled status.</value>
		[JsonProperty("LatestDeliveryDate")]
		public DateTime? LatestDeliveryDate { get; set; }

		/// <summary>
		/// When true, the order is an Amazon Business order. An Amazon Business order is an order where the buyer is a Verified Business Buyer.
		/// </summary>
		/// <value>When true, the order is an Amazon Business order. An Amazon Business order is an order where the buyer is a Verified Business Buyer.</value>
		[JsonProperty("IsBusinessOrder")]
		public bool? IsBusinessOrder { get; set; }

		/// <summary>
		/// When true, the order is a seller-fulfilled Amazon Prime order.
		/// </summary>
		/// <value>When true, the order is a seller-fulfilled Amazon Prime order.</value>
		[JsonProperty("IsPrime")]
		public bool? IsPrime { get; set; }

		/// <summary>
		/// When true, the order has a Premium Shipping Service Level Agreement. For more information about Premium Shipping orders, see \&quot;Premium Shipping Options\&quot; in the Seller Central Help for your marketplace.
		/// </summary>
		/// <value>When true, the order has a Premium Shipping Service Level Agreement. For more information about Premium Shipping orders, see \&quot;Premium Shipping Options\&quot; in the Seller Central Help for your marketplace.</value>
		[JsonProperty("IsPremiumOrder")]
		public bool? IsPremiumOrder { get; set; }

		/// <summary>
		/// When true, the order is a GlobalExpress order.
		/// </summary>
		/// <value>When true, the order is a GlobalExpress order.</value>
		[JsonProperty("IsGlobalExpressEnabled")]
		public bool? IsGlobalExpressEnabled { get; set; }

		/// <summary>
		/// The order ID value for the order that is being replaced. Returned only if IsReplacementOrder &#x3D; true.
		/// </summary>
		/// <value>The order ID value for the order that is being replaced. Returned only if IsReplacementOrder &#x3D; true.</value>
		[JsonProperty("ReplacedOrderId")]
		public string ReplacedOrderId { get; set; }

		/// <summary>
		/// When true, this is a replacement order.
		/// </summary>
		/// <value>When true, this is a replacement order.</value>
		[JsonProperty("IsReplacementOrder")]
		public bool? IsReplacementOrder { get; set; }

		/// <summary>
		/// Indicates the date by which the seller must respond to the buyer with an estimated ship date. Returned only for Sourcing on Demand orders.
		/// </summary>
		/// <value>Indicates the date by which the seller must respond to the buyer with an estimated ship date. Returned only for Sourcing on Demand orders.</value>
		[JsonProperty("PromiseResponseDueDate")]
		public DateTime? PromiseResponseDueDate { get; set; }

		/// <summary>
		/// When true, the estimated ship date is set for the order. Returned only for Sourcing on Demand orders.
		/// </summary>
		/// <value>When true, the estimated ship date is set for the order. Returned only for Sourcing on Demand orders.</value>
		[JsonProperty("IsEstimatedShipDateSet")]
		public bool? IsEstimatedShipDateSet { get; set; }

		/// <summary>
		/// When true, the item within this order was bought and re-sold by Amazon Business EU SARL (ABEU). By buying and instantly re-selling your items, ABEU becomes the seller of record, making your inventory available for sale to customers who would not otherwise purchase from a third-party seller.
		/// </summary>
		/// <value>When true, the item within this order was bought and re-sold by Amazon Business EU SARL (ABEU). By buying and instantly re-selling your items, ABEU becomes the seller of record, making your inventory available for sale to customers who would not otherwise purchase from a third-party seller.</value>
		[JsonProperty("IsSoldByAB")]
		public bool? IsSoldByAB { get; set; }

		/// <summary>
		/// The recommended location for the seller to ship the items from. It is calculated at checkout. The seller may or may not choose to ship from this location.
		/// </summary>
		/// <value>The recommended location for the seller to ship the items from. It is calculated at checkout. The seller may or may not choose to ship from this location.</value>
		[JsonProperty("AssignedShipFromLocationAddress")]
		public Address AssignedShipFromLocationAddress { get; set; }

		/// <summary>
		/// Contains the instructions about the fulfillment like where should it be fulfilled from.
		/// </summary>
		/// <value>Contains the instructions about the fulfillment like where should it be fulfilled from.</value>
		[JsonProperty("FulfillmentInstruction")]
		[CommerceDescription(AmazonCaptions.FulfillmentInstruction, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public FulfillmentInstruction FulfillmentInstruction { get; set; }

		[CommerceDescription(AmazonCaptions.OrderItems, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public List<OrderItem> OrderItems { get; set; }

		[JsonProperty("BuyerInfo")]
		[CommerceDescription(AmazonCaptions.BuyerInfo, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public BuyerInfo BuyerInfo { get; set; }

		[CommerceDescription(AmazonCaptions.ShippingAddress, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		[JsonProperty("ShippingAddress")]
		public Address ShippingAddress { get; set; }

		#region Filters
		[CommerceDescription(AmazonCaptions.BuyerEmail, FieldFilterStatus.Filterable, FieldMappingStatus.Skipped)]
		public string BuyerEmail
		{
			get { return this.BuyerInfo?.BuyerEmail; }
		}

		[CommerceDescription(AmazonCaptions.BuyerName, FieldFilterStatus.Filterable, FieldMappingStatus.Skipped)]
		public string BuyerName
		{
			get { return this.BuyerInfo?.BuyerName; }
		}

		[CommerceDescription(AmazonCaptions.PurchaseOrderNumber, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
		public string PurchaseOrderNumber
		{
			get { return this.BuyerInfo?.PurchaseOrderNumber; }
		}
		#endregion



	}

}

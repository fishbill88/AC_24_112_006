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
	/// A single order item.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[CommerceDescription("Order Item")]
	public class OrderItem : BCAPIEntity
	{
		/// <summary>
		/// The category of deemed reseller. This applies to selling partners that are not based in the EU and is used to help them meet the VAT Deemed Reseller tax laws in the EU and UK.
		/// </summary>
		/// <value>The category of deemed reseller. This applies to selling partners that are not based in the EU and is used to help them meet the VAT Deemed Reseller tax laws in the EU and UK.</value>
		[JsonProperty("DeemedResellerCategory")]
		public string DeemedResellerCategory { get; set; }
		/// <summary>
		/// Initializes a new instance of the <see cref="OrderItem" /> class.
		/// </summary>

		/// <summary>
		/// The Amazon Standard Identification Number (ASIN) of the item.
		/// </summary>
		/// <value>The Amazon Standard Identification Number (ASIN) of the item.</value>
		[JsonProperty("ASIN")]
		[CommerceDescription(AmazonCaptions.ASIN, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string ASIN { get; set; }

		/// <summary>
		/// The seller stock keeping unit (SKU) of the item.
		/// </summary>
		/// <value>The seller stock keeping unit (SKU) of the item.</value>
		[JsonProperty("SellerSKU")]
		[CommerceDescription(AmazonCaptions.SellerSKU, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string SellerSKU { get; set; }

		/// <summary>
		/// An Amazon-defined order item identifier.
		/// </summary>
		/// <value>An Amazon-defined order item identifier.</value>
		[JsonProperty("OrderItemId")]
		public string OrderItemId { get; set; }

		/// <summary>
		/// The name of the item.
		/// </summary>
		/// <value>The name of the item.</value>
		[JsonProperty("Title")]
		[CommerceDescription(AmazonCaptions.Title, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string Title { get; set; }

		/// <summary>
		/// The number of items in the order. 
		/// </summary>
		/// <value>The number of items in the order. </value>
		[JsonProperty("QuantityOrdered")]
		[CommerceDescription(AmazonCaptions.QuantityOrdered, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public int? QuantityOrdered { get; set; }

		/// <summary>
		/// The number of items shipped.
		/// </summary>
		/// <value>The number of items shipped.</value>
		[JsonProperty("QuantityShipped")]
		public int? QuantityShipped { get; set; }

		/// <summary>
		/// Product information for the item.
		/// </summary>
		/// <value>Product information for the item.</value>
		[JsonProperty("ProductInfo")]
		public ProductInfoDetail ProductInfo { get; set; }

		/// <summary>
		/// The number and value of Amazon Points granted with the purchase of an item.
		/// </summary>
		/// <value>The number and value of Amazon Points granted with the purchase of an item.</value>
		[JsonProperty("PointsGranted")]
		public PointsGrantedDetail PointsGranted { get; set; }

		/// <summary>
		/// The selling price of the order item. Note that an order item is an item and a quantity. This means that the value of ItemPrice is equal to the selling price of the item multiplied by the quantity ordered. Note that ItemPrice excludes ShippingPrice and GiftWrapPrice.
		/// </summary>
		/// <value>The selling price of the order item. Note that an order item is an item and a quantity. This means that the value of ItemPrice is equal to the selling price of the item multiplied by the quantity ordered. Note that ItemPrice excludes ShippingPrice and GiftWrapPrice.</value>
		[JsonProperty("ItemPrice")]
		[CommerceDescription(AmazonCaptions.ItemPrice, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money ItemPrice { get; set; }

		/// <summary>
		/// The shipping price of the item.
		/// </summary>
		/// <value>The shipping price of the item.</value>
		[JsonProperty("ShippingPrice")]
		[CommerceDescription(AmazonCaptions.ShippingPrice, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money ShippingPrice { get; set; }

		/// <summary>
		/// The tax on the item price.
		/// </summary>
		/// <value>The tax on the item price.</value>
		[JsonProperty("ItemTax")]
		[CommerceDescription(AmazonCaptions.ItemTax, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money ItemTax { get; set; }

		/// <summary>
		/// The tax on the shipping price.
		/// </summary>
		/// <value>The tax on the shipping price.</value>
		[JsonProperty("ShippingTax")]
		[CommerceDescription(AmazonCaptions.ShippingTax, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money ShippingTax { get; set; }

		/// <summary>
		/// The discount on the shipping price.
		/// </summary>
		/// <value>The discount on the shipping price.</value>
		[JsonProperty("ShippingDiscount")]
		[CommerceDescription(AmazonCaptions.ShippingDiscount, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money ShippingDiscount { get; set; }

		/// <summary>
		/// The tax on the discount on the shipping price.
		/// </summary>
		/// <value>The tax on the discount on the shipping price.</value>
		[JsonProperty("ShippingDiscountTax")]
		public Money ShippingDiscountTax { get; set; }

		/// <summary>
		/// The total of all promotional discounts in the offer.
		/// </summary>
		/// <value>The total of all promotional discounts in the offer.</value>
		[JsonProperty("PromotionDiscount")]
		[CommerceDescription(AmazonCaptions.PromotionDiscount, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money PromotionDiscount { get; set; }

		/// <summary>
		/// The tax on the total of all promotional discounts in the offer.
		/// </summary>
		/// <value>The tax on the total of all promotional discounts in the offer.</value>
		[JsonProperty("PromotionDiscountTax")]
		[CommerceDescription(AmazonCaptions.PromotionDiscountTax, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money PromotionDiscountTax { get; set; }

		/// <summary>
		/// Gets or Sets PromotionIds
		/// </summary>
		[JsonProperty("PromotionIds")]
		public List<string> PromotionIds { get; set; }

		/// <summary>
		/// The fee charged for COD service.
		/// </summary>
		/// <value>The fee charged for COD service.</value>
		[JsonProperty("CODFee")]
		[CommerceDescription(AmazonCaptions.CODFee, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money CODFee { get; set; }

		/// <summary>
		/// The discount on the COD fee.
		/// </summary>
		/// <value>The discount on the COD fee.</value>
		[JsonProperty("CODFeeDiscount")]
		[CommerceDescription(AmazonCaptions.CODFeeDiscount, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public Money CODFeeDiscount { get; set; }

		/// <summary>
		/// When true, the item is a gift.
		/// </summary>
		/// <value>When true, the item is a gift.</value>
		[JsonProperty("IsGift")]
		[CommerceDescription(AmazonCaptions.IsGift, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public bool? IsGift { get; set; }

		/// <summary>
		/// The condition of the item as described by the seller.
		/// </summary>
		/// <value>The condition of the item as described by the seller.</value>
		[JsonProperty("ConditionNote")]
		[CommerceDescription(AmazonCaptions.ConditionNote, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string ConditionNote { get; set; }

		/// <summary>
		/// The condition of the item.  Possible values: New, Used, Collectible, Refurbished, Preorder, Club.
		/// </summary>
		/// <value>The condition of the item.  Possible values: New, Used, Collectible, Refurbished, Preorder, Club.</value>
		[JsonProperty("ConditionId")]
		public string ConditionId { get; set; }

		/// <summary>
		/// The subcondition of the item.  Possible values: New, Mint, Very Good, Good, Acceptable, Poor, Club, OEM, Warranty, Refurbished Warranty, Refurbished, Open Box, Any, Other.
		/// </summary>
		/// <value>The subcondition of the item.  Possible values: New, Mint, Very Good, Good, Acceptable, Poor, Club, OEM, Warranty, Refurbished Warranty, Refurbished, Open Box, Any, Other.</value>
		[JsonProperty("ConditionSubtypeId")]
		public string ConditionSubtypeId { get; set; }

		/// <summary>
		/// The start date of the scheduled delivery window in the time zone of the order destination. In ISO 8601 date time format.
		/// </summary>
		/// <value>The start date of the scheduled delivery window in the time zone of the order destination. In ISO 8601 date time format.</value>
		[JsonProperty("ScheduledDeliveryStartDate")]
		[CommerceDescription(AmazonCaptions.ScheduledDeliveryStartDate, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public DateTime? ScheduledDeliveryStartDate { get; set; }

		/// <summary>
		/// The end date of the scheduled delivery window in the time zone of the order destination. In ISO 8601 date time format.
		/// </summary>
		/// <value>The end date of the scheduled delivery window in the time zone of the order destination. In ISO 8601 date time format.</value>
		[JsonProperty("ScheduledDeliveryEndDate")]
		[CommerceDescription(AmazonCaptions.ScheduledDeliveryEndDate, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public DateTime? ScheduledDeliveryEndDate { get; set; }

		/// <summary>
		/// Indicates that the selling price is a special price that is available only for Amazon Business orders. For more information about the Amazon Business Seller Program, see the [Amazon Business website](https://www.amazon.com/b2b/info/amazon-business).   Possible values: BusinessPrice - A special price that is available only for Amazon Business orders.
		/// </summary>
		/// <value>Indicates that the selling price is a special price that is available only for Amazon Business orders. For more information about the Amazon Business Seller Program, see the [Amazon Business website](https://www.amazon.com/b2b/info/amazon-business).   Possible values: BusinessPrice - A special price that is available only for Amazon Business orders.</value>
		[JsonProperty("PriceDesignation")]
		public string PriceDesignation { get; set; }

		/// <summary>
		/// Information about withheld taxes.
		/// </summary>
		/// <value>Information about withheld taxes.</value>
		[JsonProperty("TaxCollection")]
		public TaxCollection TaxCollection { get; set; }

		/// <summary>
		/// When true, the product type for this item has a serial number.  Returned only for Amazon Easy Ship orders.
		/// </summary>
		/// <value>When true, the product type for this item has a serial number.  Returned only for Amazon Easy Ship orders.</value>
		[JsonProperty("SerialNumberRequired")]
		[CommerceDescription(AmazonCaptions.SerialNumberRequired, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public bool? SerialNumberRequired { get; set; }

		/// <summary>
		/// When true, transparency codes are required.
		/// </summary>
		/// <value>When true, transparency codes are required.</value>
		[JsonProperty("IsTransparency")]
		public bool? IsTransparency { get; set; }

		/// <summary>
		/// The IOSS number of the seller. Sellers selling in the EU will be assigned a unique IOSS number that must be listed on all packages sent to the EU.
		/// </summary>
		/// <value>The IOSS number of the seller. Sellers selling in the EU will be assigned a unique IOSS number that must be listed on all packages sent to the EU.</value>
		[JsonProperty("IossNumber")]
		public string IossNumber { get; set; }

		/// <summary>
		/// A single item's buyer information.
		/// </summary>
		[JsonProperty("BuyerInfo")]
		public ItemBuyerInfo BuyerInfo { get; set; }
	}
}

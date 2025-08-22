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

namespace PX.Commerce.Shopify.API.GraphQL
{
	/// <summary>
	/// Contains information about an item in the exchange.
	/// </summary>
	[JsonObject(Description = "Order Exchange Line Item")]
	[CommerceDescription(ShopifyCaptions.LineItem, FieldFilterStatus.Filterable, FieldMappingStatus.ImportAndExport)]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderExchangeLineItemGQL : BCAPIEntity
	{
		/// <summary>
		/// A list of attributes that represent custom features or special requests.
		/// </summary>
		[JsonProperty("customAttributes")]
		[GraphQLField("customAttributes", GraphQLConstants.DataType.Object, typeof(Attribute))]
		[CommerceDescription(ShopifyCaptions.Properties, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		[BCExternCustomField(BCConstants.OrderItemProperties)]
		public List<Attribute> CustomAttributes { get; set; }

		/// <summary>
		/// The total line price after discounts are applied, in shop and presentment currencies.
		/// </summary>
		[JsonProperty("discountedTotalSet", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("discountedTotalSet", GraphQLConstants.DataType.Object, typeof(MoneyBag))]
		public MoneyBag DiscountedTotalSet { get; set; }

		[JsonIgnore]
		[CommerceDescription(ShopifyCaptions.TotalDiscountPresentment, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public decimal? DiscountedTotalPresentment { get => DiscountedTotalSet?.PresentmentMoney?.Amount; set => DiscountedTotalSet = new MoneyBag { PresentmentMoney = new Money { Amount = value } }; }

		/// <summary>
		/// The approximate split price of a line item unit, in shop and presentment currencies. This value doesn't include discounts applied to the entire order.
		/// </summary>
		[JsonProperty("discountedUnitPriceSet", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("discountedUnitPriceSet", GraphQLConstants.DataType.Object, typeof(MoneyBag))]
		public MoneyBag DiscountedUnitPriceSet { get; set; }

		[JsonIgnore]
		public decimal? DiscountedUnitPricePresentment { get => DiscountedUnitPriceSet?.PresentmentMoney?.Amount; set => DiscountedUnitPriceSet = new MoneyBag { PresentmentMoney = new Money { Amount = value } }; }

		/// <summary>
		/// The name of the product.
		/// </summary>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("name", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.String)]
		[CommerceDescription(ShopifyCaptions.Name, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string Name { get; set; }

		/// <summary>
		/// Indiciates if this line item is a gift card.
		/// </summary>
		[JsonProperty("giftCard")]
		[GraphQLField("giftCard", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.Boolean)]
		public bool? IsGiftCard { get; set; }

		/// <summary>
		/// The total price in shop and presentment currencies, without discounts applied. This value is based on the unit price of the variant x quantity.
		/// </summary>
		[JsonProperty("originalTotalSet", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("originalTotalSet", GraphQLConstants.DataType.Object, typeof(MoneyBag))]
		public MoneyBag OriginalTotalSet { get; set; }

		[JsonIgnore]
		public decimal? OriginalTotalPresentment { get => OriginalTotalSet?.PresentmentMoney?.Amount; set => OriginalTotalSet = new MoneyBag { PresentmentMoney = new Money { Amount = value } };  }

		/// <summary>
		/// The variant unit price without discounts applied, in shop and presentment currencies.
		/// </summary>
		[JsonProperty("originalUnitPriceSet", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("originalUnitPriceSet", GraphQLConstants.DataType.Object, typeof(MoneyBag))]
		public MoneyBag OriginalUnitPriceSet { get; set; }

		[JsonIgnore]
		[CommerceDescription(ShopifyCaptions.PricePresentment, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public decimal? OriginalUnitPricePresentment { get => OriginalUnitPriceSet?.PresentmentMoney?.Amount; set => OriginalUnitPriceSet = new MoneyBag { PresentmentMoney = new Money { Amount = value } }; }

		/// <summary>
		/// The number of products that were purchased.
		/// </summary>
		[JsonProperty("quantity")]
		[GraphQLField("quantity", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.Int)]
		[CommerceDescription(ShopifyCaptions.Quantity, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public int Quantity { get; set; }

		/// <summary>
		/// The line item's quantity, minus the removed quantity.
		/// </summary>
		[JsonProperty("refundableQuantity")]
		[GraphQLField("refundableQuantity", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.Int)]
		public int RefundableQuantity { get; set; }

		/// <summary>
		/// Whether physical shipping is required for the variant.
		/// </summary>
		[JsonProperty("requiresShipping")]
		[GraphQLField("requiresShipping", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.Boolean)]
		[CommerceDescription(ShopifyCaptions.RequiresShipping, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public bool RequiresShipping { get; set; }

		/// <summary>
		/// The variant SKU number.
		/// </summary>
		[JsonProperty("sku", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("sku", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.String)]
		[CommerceDescription(ShopifyCaptions.SKU, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string Sku { get; set; }

		/// <summary>
		/// The taxes charged for this line item.
		/// </summary>
		[JsonProperty("taxLines", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("taxLines", GraphQLConstants.DataType.Object, typeof(OrderTaxLineGQL))]
		[CommerceDescription(ShopifyCaptions.TaxLine, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public List<OrderTaxLineGQL> TaxLines { get; set; }

		/// <summary>
		/// Whether the variant is taxable.
		/// </summary>
		[JsonProperty("taxable")]
		[GraphQLField("taxable", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.Boolean)]
		[CommerceDescription(ShopifyCaptions.Taxable, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public bool Taxable { get; set; }

		/// <summary>
		/// The title of the product or variant. This field only applies to custom line items.
		/// </summary>
		[JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("title", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.String)]
		[CommerceDescription(ShopifyCaptions.Title, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string Title { get; set; }

		/// <summary>
		/// The product variant
		/// </summary>
		[JsonProperty("variant", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("variant", GraphQLConstants.DataType.Object, typeof(ProductVariantGQL))]
		public ProductVariantGQL Variant { get; set; }

		[JsonIgnore]
		[CommerceDescription(ShopifyCaptions.VariantId, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string VariantId { get => Variant?.Id; set => Variant = new ProductVariantGQL { Id = value }; }

		/// <summary>
		/// The name of the variant.
		/// </summary>
		[JsonProperty("variantTitle", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("variantTitle", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.String)]
		public string VariantTitle { get; set; }

		/// <summary>
		/// The name of the vendor who created the product variant.
		/// </summary>
		[JsonProperty("vendor", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("vendor", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.String)]
		public string Vendor { get; set; }

		/// <summary>
		/// The line item associated with this object.
		/// </summary>
		[JsonProperty("lineItem", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("lineItem", GraphQLConstants.DataType.Object, typeof(OrderLineItemGQL))]
		public OrderLineItemGQL LineItem { get; set; }
	}
}

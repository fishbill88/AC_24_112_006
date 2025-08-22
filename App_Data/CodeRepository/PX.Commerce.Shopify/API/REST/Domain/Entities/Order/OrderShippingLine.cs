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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PX.Commerce.Core;

namespace PX.Commerce.Shopify.API.REST
{
	[JsonObject(Description = "Order Shipping Line")]
	[CommerceDescription(ShopifyCaptions.ShippingLine, FieldFilterStatus.Filterable, FieldMappingStatus.Export)]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderShippingLine : BCAPIEntity
	{
		/// <summary>
		/// A reference to the carrier service that provided the rate. Present when the rate was computed by a third-party carrier service.
		/// </summary>
		[JsonProperty("carrier_identifier")]
		[CommerceDescription(ShopifyCaptions.CarrierIdentifier, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public String CarrierIdentifier { get; set; }

		/// <summary>
		/// A reference to the shipping method.
		/// </summary>
		[JsonProperty("code")]
		[CommerceDescription(ShopifyCaptions.Code, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public String Code { get; set; }

		/// <summary>
		/// A list of amounts allocated by discount applications. Each discount allocation is associated to a particular discount application.
		/// </summary>
		[JsonProperty("discount_allocations", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.DiscountAllocation, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public List<OrderDiscountAllocation> DiscountAllocations { get; set; }

		/// <summary>
		/// The price of the shipping method after line-level discounts have been applied. Doesn't reflect cart-level or order-level discounts.
		/// </summary>
		[JsonProperty("discounted_price")]
		[CommerceDescription(ShopifyCaptions.DiscountedPrice, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public decimal? DiscountedPrice { get; set; }

		[CommerceDescription(ShopifyCaptions.DiscountedPricePresentment, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		[ShouldNotSerialize]
		public decimal? DiscountedPricePresentment { get => DiscountedPriceSet?.PresentmentMoney?.Amount; }

		/// <summary>
		/// The price of the shipping method in both shop and presentment currencies after line-level discounts have been applied.
		/// </summary>
		[JsonProperty("discounted_price_set")]
		public PriceSet DiscountedPriceSet { get; set; }

		/// <summary>
		/// The price of the shipping method in shop and presentment currencies.
		/// </summary>
		[JsonProperty("price_set")]
		public PriceSet PriceSet { get; set; }

		[CommerceDescription(ShopifyCaptions.ShippingCostExcludingTaxPresentment, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		[ShouldNotSerialize]
		public decimal? ShippingCostExcludingTaxPresentment { get => PriceSet?.PresentmentMoney?.Amount; }

		/// <summary>
		/// The price of this shipping method in the shop currency. Can't be negative.
		/// </summary>
		[JsonProperty("price")]
		[CommerceDescription(ShopifyCaptions.ShippingCostExcludingTax, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public decimal? ShippingCostExcludingTax { get; set; }

		/// <summary>
		/// The source of the shipping method.
		/// </summary>
		[JsonProperty("source")]
		[CommerceDescription(ShopifyCaptions.SourceName, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public String SourceName { get; set; }

		/// <summary>
		/// A list of tax line objects, each of which details a tax applicable to this shipping line.
		/// </summary>
		[JsonProperty("tax_lines", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.TaxLine, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public List<OrderTaxLine> TaxLines { get; set; }

		/// <summary>
		/// The title of the shipping method.
		/// </summary>
		[JsonProperty("title")]
		[CommerceDescription(ShopifyCaptions.Title, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public String Title { get; set; }
	}

}

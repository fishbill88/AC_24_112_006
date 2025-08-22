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

namespace PX.Commerce.Shopify.API.GraphQL
{
	[JsonObject(Description = "Order Exchange Addition")]
	[CommerceDescription(ShopifyCaptions.OrderExchangeAddition, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderExchangeAdditionGQL : BCAPIEntity
	{
		/// <summary>
		/// The list of new items for the exchange.
		/// </summary>
		[JsonProperty("lineItems", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("lineItems", GraphQLConstants.DataType.Object, typeof(OrderExchangeLineItemGQL))]
		[CommerceDescription(ShopifyCaptions.LineItem, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public List<OrderExchangeLineItemGQL> LineItems { get; set; }

		/// <summary>
		/// The subtotal of the items being added, including discounts.
		/// </summary>
		[JsonProperty("subtotalPriceSet", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("subtotalPriceSet", GraphQLConstants.DataType.Object, typeof(MoneyBag))]
		public MoneyBag SubtotalPriceSet { get; set; }

		[JsonIgnore]
		public decimal? SubtotalPricePresentment { get => SubtotalPriceSet?.PresentmentMoney?.Amount; }

		/// <summary>
		/// The total price of the items being added, including discounts and taxes.
		/// </summary>
		[JsonProperty("totalPriceSet", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("totalPriceSet", GraphQLConstants.DataType.Object, typeof(MoneyBag))]
		public MoneyBag TotalPriceSet { get; set; }

		[JsonIgnore]
		public decimal? TotalPriceSetPresentment { get => TotalPriceSet?.PresentmentMoney?.Amount; }

		/// <summary>
		/// The summary of all taxes of the items being added.
		/// </summary>
		[JsonProperty("taxLines", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("taxLines", GraphQLConstants.DataType.Object, typeof(OrderTaxLineGQL))]
		[CommerceDescription(ShopifyCaptions.TaxLine, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public List<OrderTaxLineGQL> TaxLines { get; set; }
	}
}

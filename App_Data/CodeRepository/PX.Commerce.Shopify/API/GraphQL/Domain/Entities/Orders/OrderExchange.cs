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
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderExchangeResponse : IEntityResponse<OrderExchangeGQL>
	{
		[JsonProperty("exchangeV2s")]
		public OrderExchangeGQL TEntityData { get; set; }
	}

	[GraphQLObject(NodeName = "exchangeV2s")]
	[JsonObject(Description = "Order Exchange")]
	[CommerceDescription(ShopifyCaptions.OrderExchange, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderExchangeGQL : BCAPIEntity, INode
	{
		/// <summary>
		/// The details of the new items in the exchange.
		/// </summary>
		[JsonProperty("additions", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("additions", GraphQLConstants.DataType.Object, typeof(OrderExchangeAdditionGQL))]
		[CommerceDescription(ShopifyCaptions.OrderExchangeAddition, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public OrderExchangeAdditionGQL Additions { get; set; }

		/// <summary>
		/// The date and time when the exchange was completed at Shopify.
		/// </summary>
		[JsonProperty("completedAt")]
		[GraphQLField("completedAt", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.DateTime)]
		public DateTime? CompletedAt { get; set; }

		/// <summary>
		/// The date and time when the exchange was created in Shopify.
		/// </summary>
		[JsonProperty("createdAt")]
		[GraphQLField("createdAt", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.DateTime)]
		[CommerceDescription(ShopifyCaptions.DateCreated, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public DateTime? DateCreatedAt { get; set; }

		/// <summary>
		/// A globally-unique identifier.
		/// </summary>
		[JsonProperty("id")]
		[GraphQLField("id", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.ID)]
		public string Id { get; set; }

		/// <summary>
		/// The location where the exchange happened.
		/// </summary>
		[JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("location", GraphQLConstants.DataType.Object, typeof(LocationGQL))]
		public LocationGQL Location { get; set; }

		/// <summary>
		/// The text of an optional note that a shop owner can attach to the exchange order.
		/// </summary>
		[JsonProperty("note", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("note", GraphQLConstants.DataType.Scalar, GraphQLConstants.ScalarType.String)]
		[CommerceDescription(ShopifyCaptions.Note, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public string Note { get; set; }

		/// <summary>
		/// A list of refunds that have been applied to the exchange order.
		/// </summary>
		[JsonProperty("refunds", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("refunds", GraphQLConstants.DataType.Object, typeof(OrderRefundGQL))]
		[CommerceDescription(ShopifyCaptions.Refund, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public List<OrderRefundGQL> Refunds { get; set; }

		/// <summary>
		/// The details of the returned items in the exchange.
		/// </summary>
		[JsonProperty("returns", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("returns", GraphQLConstants.DataType.Object, typeof(OrderExchangeReturnGQL))]
		[CommerceDescription(ShopifyCaptions.OrderExchangeReturn, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public OrderExchangeReturnGQL Returns { get; set; }

		/// <summary>
		/// The amount of money that was paid or refunded as part of the exchange.
		/// </summary>
		[JsonProperty("totalAmountProcessedSet", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("totalAmountProcessedSet", GraphQLConstants.DataType.Object, typeof(MoneyBag))]
		public MoneyBag TotalAmountProcessedSet { get; set; }

		[JsonIgnore]
		public decimal? TotalAmountProcessedPresentment { get => TotalAmountProcessedSet?.PresentmentMoney?.Amount; }

		/// <summary>
		/// The difference in values of the items that were exchanged.
		/// </summary>
		[JsonProperty("totalPriceSet", NullValueHandling = NullValueHandling.Ignore)]
		[GraphQLField("totalPriceSet", GraphQLConstants.DataType.Object, typeof(MoneyBag))]
		public MoneyBag TotalPriceSet { get; set; }

		[JsonIgnore]
		public decimal? TotalPriceSetPresentment { get => TotalPriceSet?.PresentmentMoney?.Amount; }

		/// <summary>
		/// The transactions associated with the exchange order.
		/// </summary>
		[JsonProperty("transactions")]
		[GraphQLField("transactions", GraphQLConstants.DataType.Object, typeof(OrderTransactionGQL))]
		[CommerceDescription(ShopifyCaptions.Transactions, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public List<OrderTransactionGQL> Transactions { get; set; }

		[JsonIgnore]
		public Guid? LocalID { get; set; }
	}
}

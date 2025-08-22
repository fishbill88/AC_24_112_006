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
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderRefundResponse : IEntityResponse<OrderRefund>
	{
		[JsonProperty("refund")]
		public OrderRefund Data { get; set; }
	}
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderRefundsResponse : IEntitiesResponse<OrderRefund>
	{
		[JsonProperty("refunds")]
		public IEnumerable<OrderRefund> Data { get; set; }
	}

	[JsonObject(Description = "Order Refund")]
	[CommerceDescription(ShopifyCaptions.Refund, FieldFilterStatus.Filterable, FieldMappingStatus.Import)]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class OrderRefund : BCAPIEntity
	{
		/// <summary>
		/// [READ-ONLY] The date and time (ISO 8601 format) when the refund was created.
		/// </summary>
		[JsonProperty("created_at")]
		[ShouldNotSerialize]
		public DateTime? DateCreatedAt { get; set; }

		/// <summary>
		///  [READ-ONLY] The unique identifier for the refund.
		/// </summary>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.Id, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public long? Id { get; set; }

		/// <summary>
		///  [READ-ONLY] The unique identifier for the refund.
		/// </summary>
		[JsonProperty("order_id", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.OrderId, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public long? OrderId { get; set; }

		/// <summary>
		/// An optional note attached to a refund.
		/// </summary>
		[JsonProperty("note", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.Note, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public String Note { get; set; }

		/// <summary>
		/// [READ-ONLY] An optional note attached to a refund.
		/// </summary>
		[JsonProperty("order_adjustments", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.OrderAdjustment, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		[ShouldNotSerialize]
		public List<OrderAdjustment> OrderAdjustments { get; set; }

		/// <summary>
		/// The date and time (ISO 8601 format) when the refund was imported. 
		/// This value can be set to a date in the past when importing from other systems. 
		/// If no value is provided, then it will be auto-generated as the current time in Shopify.
		/// </summary>
		[JsonProperty("processed_at", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.ProcessedAt, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public DateTime? ProcessedAt { get; set; }

		/// <summary>
		/// A list of refunded line items. 
		/// </summary>
		[JsonProperty("refund_line_items", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.RefundItem, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		[ShouldNotSerialize]
		public List<RefundLineItem> RefundLineItems { get; set; }

		/// <summary>
		/// A list of transactions involved in the refund. 
		/// </summary>
		[JsonProperty("transactions", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.Transactions, FieldFilterStatus.Skipped, FieldMappingStatus.Import)]
		public List<OrderTransaction> Transactions { get; set; }

		/// <summary>
		///  The unique identifier of the user who performed the refund.
		/// </summary>
		[JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
		[CommerceDescription(ShopifyCaptions.UserId, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		public long? UserId { get; set; }
	}

}

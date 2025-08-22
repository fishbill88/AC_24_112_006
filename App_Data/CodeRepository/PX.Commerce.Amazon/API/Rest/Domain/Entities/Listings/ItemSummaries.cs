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
using PX.Commerce.Core.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Summary details of a listings item for an Amazon marketplace.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ItemSummaries
	{
		/// <summary>
		/// A marketplace identifier. Identifies the Amazon marketplace for the listings item.
		/// </summary>
		/// <value>A marketplace identifier. Identifies the Amazon marketplace for the listings item.</value>
		[JsonProperty("marketplaceId")]
		[CommerceDescription(AmazonCaptions.MarketplaceId, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		public string MarketplaceId { get; set; }

		/// <summary>
		/// Amazon Standard Identification Number (ASIN) is the unique identifier for an item in the Amazon catalog.
		/// </summary>
		/// <value>Amazon Standard Identification Number (ASIN) is the unique identifier for an item in the Amazon catalog.</value>
		[JsonProperty("asin")]
		[CommerceDescription(AmazonCaptions.ASIN, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		public string ASIN { get; set; }

		/// <summary>
		/// The Amazon product type of the listings item.
		/// </summary>
		/// <value>The Amazon product type of the listings item.</value>
		[JsonProperty("productType")]
		[CommerceDescription(AmazonCaptions.ProductType, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		public string ProductType { get; set; }

		/// <summary>
		/// Identifies the condition of the listings item.
		/// </summary>
		/// <value>Identifies the condition of the listings item.</value>
		//[JsonProperty("conditionType")]
		//[CommerceDescription(AmazonCaptions.ConditionType, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		//public ConditionType ConditionType { get; set; }

		/// <summary>
		/// Statuses that apply to the listings item.
		/// </summary>
		/// <value>Statuses that apply to the listings item.</value>
		//[JsonProperty("status")]
		//[CommerceDescription(AmazonCaptions.Status, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		//public List<Status> Status { get; set; }

		/// <summary>
		/// Fulfillment network stock keeping unit is an identifier used by Amazon fulfillment centers to identify each unique item.
		/// </summary>
		/// <value>Fulfillment network stock keeping unit is an identifier used by Amazon fulfillment centers to identify each unique item.</value>
		[JsonProperty("fnSku")]
		[CommerceDescription(AmazonCaptions.FnSku, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		public string FnSku { get; set; }

		/// <summary>
		/// Name, or title, associated with an Amazon catalog item.
		/// </summary>
		/// <value>Name, or title, associated with an Amazon catalog item.</value>
		[JsonProperty("itemName")]
		[CommerceDescription(AmazonCaptions.ItemName, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		public string ItemName { get; set; }

		/// <summary>
		/// Date the listings item was created, in ISO 8601 format.
		/// </summary>
		/// <value>Date the listings item was created, in ISO 8601 format.</value>
		[JsonProperty("createdDate")]
		[CommerceDescription(AmazonCaptions.CreatedDate, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Date the listings item was last updated, in ISO 8601 format.
		/// </summary>
		/// <value>Date the listings item was last updated, in ISO 8601 format.</value>
		[JsonProperty("lastUpdatedDate")]
		[CommerceDescription(AmazonCaptions.LastUpdatedDate, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		public DateTime? LastUpdatedDate { get; set; }

		/// <summary>
		/// Date the listings item was last updated, in ISO 8601 format.
		/// </summary>
		/// <value>Date the listings item was last updated, in ISO 8601 format.</value>
		//[JsonProperty("mainImage")]
		//[CommerceDescription(AmazonCaptions.MainImage, FieldFilterStatus.Skipped, FieldMappingStatus.Skipped)]
		//public ItemImage MainImage { get; set; }
	}
}

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
 https://developer-docs.amazon.com/sp-api/docs/feeds-api-v2021-06-30-reference#feed
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PX.Commerce.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Detailed information about the feed.
	/// </summary>

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Feed : BCAPIEntity
	{
		/// <summary>
		/// The processing status of the feed.
		/// </summary>
		/// <value>The processing status of the feed.</value>
		[JsonProperty("processingStatus")]
		public string ProcessingStatus { get; set; }

		/// <summary>
		/// The identifier for the feed. This identifier is unique only in combination with a seller ID.
		/// </summary>
		/// <value>The identifier for the feed. This identifier is unique only in combination with a seller ID.</value>
		[JsonProperty("feedId")]
		public string FeedId { get; set; }

		/// <summary>
		/// The feed type.
		/// </summary>
		/// <value>The feed type.</value>
		[JsonProperty("feedType")]
		public string FeedType { get; set; }

		/// <summary>
		/// A list of identifiers for the marketplaces that the feed is applied to.
		/// </summary>
		/// <value>A list of identifiers for the marketplaces that the feed is applied to.</value>
		[JsonProperty("marketplaceIds")]
		public List<string> MarketplaceIds { get; set; }

		/// <summary>
		/// The date and time when the feed was created, in ISO 8601 date time format.
		/// </summary>
		/// <value>The date and time when the feed was created, in ISO 8601 date time format.</value>
		[JsonProperty("createdTime")]
		public DateTime? CreatedTime { get; set; }


		/// <summary>
		/// The date and time when feed processing started, in ISO 8601 date time format.
		/// </summary>
		/// <value>The date and time when feed processing started, in ISO 8601 date time format.</value>
		[JsonProperty("processingStartTime")]
		public DateTime? ProcessingStartTime { get; set; }

		/// <summary>
		/// The date and time when feed processing completed, in ISO 8601 date time format.
		/// </summary>
		/// <value>The date and time when feed processing completed, in ISO 8601 date time format.</value>
		[JsonProperty("processingEndTime")]
		public DateTime? ProcessingEndTime { get; set; }

		/// <summary>
		/// The identifier for the feed document. This identifier is unique only in combination with a seller ID.
		/// </summary>
		/// <value>The identifier for the feed document. This identifier is unique only in combination with a seller ID.</value>
		[JsonProperty("resultFeedDocumentId")]
		public string ResultFeedDocumentId { get; set; }

	}

}
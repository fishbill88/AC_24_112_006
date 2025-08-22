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
 https://developer-docs.amazon.com/sp-api/docs/feeds-api-v2021-06-30-reference#createfeedspecification
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Information required to create the feed.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public  class CreateFeedSpecification
	{
		/// <summary>
		/// The feed type.
		/// </summary>
		/// <value>The feed type.</value>
		[JsonProperty("feedType")]
		public string FeedType { get; set; }

		/// <summary>
		/// A list of identifiers for marketplaces that you want the feed to be applied to.
		/// </summary>
		/// <value>A list of identifiers for marketplaces that you want the feed to be applied to.</value>
		[JsonProperty("marketplaceIds")]
		public List<string> MarketplaceIds { get; set; }

		/// <summary>
		/// The document identifier returned by the createFeedDocument operation. Upload the feed document contents before calling the createFeed operation.
		/// </summary>
		/// <value>The document identifier returned by the createFeedDocument operation. Upload the feed document contents before calling the createFeed operation.</value>
		[JsonProperty("inputFeedDocumentId")]
		public string InputFeedDocumentId { get; set; }

		/// <summary>
		/// Gets or Sets FeedOptions
		/// </summary>
		[JsonProperty("feedOptions")]
		public Dictionary<String, string> FeedOptions { get; set; }
	}

}

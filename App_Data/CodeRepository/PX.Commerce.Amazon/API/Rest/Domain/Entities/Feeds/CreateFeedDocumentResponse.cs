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
Classes created based on https://developer-docs.amazon.com/sp-api/docs/feeds-api-v2021-06-30-reference#createfeeddocumentresponse
 */

using Newtonsoft.Json;
using PX.Commerce.Core;
using System.Text;

namespace PX.Commerce.Amazon.API.Rest
{

	/// <summary>
	/// Information required to upload a feed document&#39;s contents.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public  class CreateFeedDocumentResponse : BCAPIEntity
	{

		/// <summary>
		/// The identifier of the feed document.
		/// </summary>
		/// <value>The identifier of the feed document.</value>
		[JsonProperty("feedDocumentId")]
		public string FeedDocumentId { get; set; }

		/// <summary>
		/// The presigned URL for uploading the feed contents. This URL expires after 5 minutes.
		/// </summary>
		/// <value>The presigned URL for uploading the feed contents. This URL expires after 5 minutes.</value>
		[JsonProperty("url")]
		public string Url { get; set; }

	}
}


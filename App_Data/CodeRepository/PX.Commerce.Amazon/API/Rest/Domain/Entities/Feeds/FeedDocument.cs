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
https://developer-docs.amazon.com/sp-api/docs/feeds-api-v2021-06-30-reference#feeddocument
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Information required for the feed document.
	/// </summary>

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public  class FeedDocument
	{

		/// <summary>
		/// If present, the feed document contents are compressed using the indicated algorithm.
		/// </summary>
		/// <value>If present, the feed document contents are compressed using the indicated algorithm.</value>
		[JsonProperty("compressionAlgorithm")]
		public string CompressionAlgorithm { get; set; }

		/// <summary>
		/// The identifier for the feed document. This identifier is unique only in combination with a seller ID.
		/// </summary>
		/// <value>The identifier for the feed document. This identifier is unique only in combination with a seller ID.</value>
		[JsonProperty("feedDocumentId")]
		public string FeedDocumentId { get; set; }

		/// <summary>
		/// A presigned URL for the feed document. This URL expires after 5 minutes.
		/// </summary>
		/// <value>A presigned URL for the feed document. This URL expires after 5 minutes.</value>
		[JsonProperty("url")]
		public string Url { get; set; }

	}

}

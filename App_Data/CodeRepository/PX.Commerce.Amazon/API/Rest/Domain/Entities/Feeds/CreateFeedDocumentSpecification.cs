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
 https://developer-docs.amazon.com/sp-api/docs/feeds-api-v2021-06-30-reference#createfeeddocumentspecification
 */

using Newtonsoft.Json;
using System.Text;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Specifies the content type for the createFeedDocument operation.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public  class CreateFeedDocumentSpecification
	{
		/// <summary>
		/// The content type of the feed.
		/// </summary>
		/// <value>The content type of the feed.</value>
		[JsonProperty("contentType")]
		public string ContentType { get; set; }

	}

}

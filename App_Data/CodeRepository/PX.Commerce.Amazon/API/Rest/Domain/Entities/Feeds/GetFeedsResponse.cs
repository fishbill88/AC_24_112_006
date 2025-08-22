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
https://developer-docs.amazon.com/sp-api/docs/feeds-api-v2021-06-30-reference#getfeedsresponse
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Response schema.
	/// </summary>

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public  class GetFeedsResponse
	{
		/// <summary>
		/// The feeds.
		/// </summary>
		/// <value>The feeds.</value>
		[JsonProperty("feeds")]
		public List<Feed> Feeds { get; set; }

		/// <summary>
		/// Returned when the number of results exceeds pageSize. To get the next page of results, call the getFeeds operation with this token as the only parameter.
		/// </summary>
		/// <value>Returned when the number of results exceeds pageSize. To get the next page of results, call the getFeeds operation with this token as the only parameter.</value>
		[JsonProperty("nextToken")]
		public string NextToken { get; set; }

	}
}

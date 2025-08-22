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
using PX.Commerce.Amazon.API.Rest.Converters;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// The response schema for the listFinancialEvents operation.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class GetNonOrderFinancialEventsResponse : IEntityPayloadResponse<NonOrderFinancialEventsPage>
    {
        /// <summary>
        /// The payload for the listFinancialEvents operation.
        /// </summary>
        /// <value>The payload for the listFinancialEvents operation.</value>
        [JsonProperty("payload")]
        public NonOrderFinancialEventsPage Payload { get; set; }
	}

    /// <summary>
	/// The response schema for the listFinancialEvents operation.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class NonOrderFinancialEventsPage : IEntityListResponse<NonOrderFinancialEvents>
	{
        /// <summary>
        /// Contains all information related to a financial event.
        /// </summary>
        /// <value>Contains all information related to a financial event.</value>
        [JsonProperty("FinancialEvents")]
		[JsonConverter(typeof(SingleOrArrayConverter<NonOrderFinancialEvents>))]
		public IEnumerable<NonOrderFinancialEvents> Data { get; set; }

		/// <summary>
		/// Gets or sets a string token returned in the response of your previous request.
		/// </summary>
		/// <value>A string token returned in the response of your previous request.</value>
		[JsonProperty("NextToken")]
		public string NextToken { get; set; }
	}
}

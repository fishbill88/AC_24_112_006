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
using System;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing an event related to a financial event group.
	/// </summary>
	public class FinancialEventGroupList : IEntityListResponse<FinancialEventGroup>
	{
		/// <summary>
		/// Gets or sets a list of information related to a financial event group.
		/// </summary>
		/// <value>A list of information related to a financial event group.</value>
		[JsonProperty("FinancialEventGroupList")]
		public IEnumerable<FinancialEventGroup> Data { get; set; }

		/// <summary>
		/// Gets or sets a string token returned in the response of your previous request.
		/// </summary>
		/// <value>A string token returned in the response of your previous request.</value>
		[JsonProperty("NextToken")]
		public string NextToken { get; set; }
	}
}

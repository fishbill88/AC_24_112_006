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

﻿using System;
using System.ComponentModel;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents the filter used to get list of financial events
	/// </summary>
	/// <remarks>GET /finances/v0/financialEvents</remarks>
	public class FinancialEventsFilter : Filter
	{
		/// <summary>
		/// Gets or sets a date used for selecting financial events posted after (or at) a specified time
		/// </summary>
		/// <value>A date used for selecting financial events posted after (or at) a specified time</value>
		/// <remarks>The date-time must be more than two minutes before the time of the request, in ISO 8601 date time format.</remarks>
		[Description("PostedAfter")]
		public DateTime? PostedAfter { get; set; }

		/// <summary>
		/// Gets or sets a date used for selecting financial events posted before (but not at) a specified time.
		/// </summary>
		/// <value>A date used for selecting financial events posted before (but not at) a specified time.</value>
		/// <remarks>The date-time must be later than PostedAfter and no later than two minutes before the request was submitted, in ISO 8601 date time format. 
		/// If PostedAfter and PostedBefore are more than 180 days apart, no financial events are returned. 
		/// You must specify the PostedAfter parameter if you specify the PostedBefore parameter. 
		/// Default: Now minus two minutes.</remarks>
		[Description("PostedBefore")]
		public DateTime? PostedBefore { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of results to return per page.
		/// </summary>
		/// <value>The maximum number of results to return per page.</value>
		/// <remarks>If the response exceeds the maximum number of transactions or 10 MB, the API responds with 'InvalidInput'. 
		/// Minimum: 1, Maximum: 100. Default: 100.</remarks>
		[Description("MaxResultsPerPage")]
		public int? MaxResultsPerPage { get; set; }

		/// <summary>
		/// Gets or sets a string token returned in the response of your previous request.
		/// </summary>
		/// <value>A string token returned in the response of your previous request.</value>
		[Description("NextToken")]
		public string NextToken { get; set; }
	}
}

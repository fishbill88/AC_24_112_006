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

using System;
using System.ComponentModel;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents the filter used to get list of financial groups.
	/// </summary>
	/// <remarks>GET /finances/v0/financialEventGroups</remarks>
	public class FinancialEventGroupsFilter : Filter
	{
		/// <summary>
		/// Gets or sets the date used for selecting financial event groups that opened before (but not at) a specified date and time, in ISO 8601 format.
		/// </summary>
		/// <value>The date used for selecting financial event groups that opened before (but not at) a specified date and time.</value>
		/// <remarks>The date-time must be later than FinancialEventGroupStartedAfter and no later than two minutes before the request was submitted. 
		/// If FinancialEventGroupStartedAfter and FinancialEventGroupStartedBefore are more than 180 days apart, no financial event groups are returned.</remarks>
		[Description("FinancialEventGroupStartedAfter")]
		public DateTime? FinancialEventGroupStartedAfter { get; set; }

		/// <summary>
		/// Gets or sets the date used for selecting financial event groups that opened after (or at) a specified date and time, in ISO 8601 format.
		/// </summary>
		/// <value>The date used for selecting financial event groups that opened after (or at) a specified date and time.</value>
		/// <remarks>The date-time must be no later than two minutes before the request was submitted.</remarks>
		[Description("FinancialEventGroupStartedBefore")]
		public DateTime? FinancialEventGroupStartedBefore { get; set; }

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

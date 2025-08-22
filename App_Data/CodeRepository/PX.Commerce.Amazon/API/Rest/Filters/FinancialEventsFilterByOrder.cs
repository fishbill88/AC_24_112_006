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
	/// Represents the filter used to get list of financial events by order id.
	/// </summary>
	/// <remarks>GET /finances/v0/orders/{orderId}/financialEvents</remarks>
	public class FinancialEventsFilterByOrder : Filter
	{
		/// <summary>
		/// Gets or sets the financial group identifier.
		/// </summary>
		/// <value>The financial group identifier.</value>
		public string OrderId { get; set; }

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

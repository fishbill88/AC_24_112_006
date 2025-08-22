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
Classes created based on https://developer-docs.amazon.com/sp-api/docs/reports-api-v2021-06-30-reference#createreportspecification
*/

using Newtonsoft.Json;
using PX.Commerce.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Information required to create the report.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class CreateReportSpecification : BCAPIEntity
	{
		/// <summary>
		/// Additional information passed to reports. This varies by report type.
		/// </summary>
		/// <value>Additional information passed to reports. This varies by report type.</value>
		[JsonProperty("reportOptions")]
		public Dictionary<string, string> ReportOptions { get; set; }

		/// <summary>
		/// The report type.
		/// </summary>
		/// <value>The report type.</value>
		[JsonProperty("reportType")]
		public string reportType { get; set; }

		/// <summary>
		/// The start of a date and time range, in ISO 8601 date time format, used for selecting the data to report. 
		/// The default is now. The value must be prior to or equal to the current date and time. 
		/// Not all report types make use of this.
		/// </summary>
		/// <value> 
		/// The start of a date and time range, in ISO 8601 date time format, used for selecting the data to report. 
		/// The default is now. The value must be prior to or equal to the current date and time. 
		/// Not all report types make use of this.
		/// </value>
		[JsonProperty("dataStartTime")]
		public DateTime? DataStartTime { get; set; }

		/// <summary>
		/// The end of a date and time range, in ISO 8601 date time format, used for selecting the data to report. 
		/// The default is now. The value must be prior to or equal to the current date and time. 
		/// Not all report types make use of this.
		/// </summary>
		/// <value>         
		/// The end of a date and time range, in ISO 8601 date time format, used for selecting the data to report. 
		/// The default is now. The value must be prior to or equal to the current date and time. 
		/// Not all report types make use of this.
		/// </value>
		[JsonProperty("dataEndTime")]
		public DateTime? DataEndTime { get; set; }

		/// <summary>
		/// A list of marketplace identifiers. 
		/// The report document's contents will contain data for all of the specified marketplaces, unless the report type indicates otherwise.
		/// </summary>
		/// <value> 
		/// A list of marketplace identifiers. 
		/// The report document's contents will contain data for all of the specified marketplaces, unless the report type indicates otherwise.
		/// </value>
		[JsonProperty("marketplaceIds")]
		public string[] MarketplaceIds { get; set; }
	}
}

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
using PX.Commerce.Core;
using System;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing an event related to a financial event group.
	/// </summary>
	public class FinancialEventGroup : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the account tail of the payment instrument.
		/// </summary>
		/// <value>The account tail of the payment instrument.</value>
		[JsonProperty("AccountTail")]
		public string AccountTail { get; set; }

		/// <summary>
		/// Gets or sets the balance at the beginning of the settlement period.
		/// </summary>
		/// <value>The balance at the beginning of the settlement period.</value>
		[JsonProperty("BeginningBalance")]
		public Currency BeginningBalance { get; set; }

		/// <summary>
		/// Gets or sets the total amount in the currency of the marketplace in which the funds were disbursed.
		/// </summary>
		/// <value>The total amount in the currency of the marketplace in which the funds were disbursed.</value>
		[JsonProperty("ConvertedTotal")]
		public Currency ConvertedTotal { get; set; }

		/// <summary>
		/// Gets or sets a unique identifier for the financial event group.
		/// </summary>
		/// <value>A unique identifier for the financial event group.</value>
		[JsonProperty("FinancialEventGroupId")]
		public string FinancialEventGroupId { get; set; }

		/// <summary>
		/// Gets or sets the date and time at which the financial event group is closed.
		/// </summary>
		/// <value>The date and time at which the financial event group is closed.</value>
		/// <remarks>In ISO 8601 date time format.</remarks>
		[JsonProperty("FinancialEventGroupEnd")]
		public DateTime? FinancialEventGroupEnd { get; set; }

		/// <summary>
		/// Gets or sets the date and time at which the financial event group is opened.
		/// </summary>
		/// <value>The date and time at which the financial event group is opened.</value>
		/// <remarks>In ISO 8601 date time format.</remarks>
		[JsonProperty("FinancialEventGroupStart")]
		public DateTime? FinancialEventGroupStart { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the disbursement or charge was initiated. 
		/// </summary>
		/// <value>The date and time when the disbursement or charge was initiated.</value>
		/// <remarks>Only present for closed settlements. In ISO 8601 date time format.</remarks>
		[JsonProperty("FundTransferDate")]
		public DateTime? FundTransferDate { get; set; }

		/// <summary>
		/// Gets or sets the status of the fund transfer.
		/// </summary>
		/// <value>The status of the fund transfer.</value>
		[JsonProperty("FundTransferStatus")]
		public string FundTransferStatus { get; set; }

		/// <summary>
		/// Gets or sets the total amount in the currency of the marketplace in which the transactions occurred.
		/// </summary>
		/// <value>The total amount in the currency of the marketplace in which the transactions occurred.</value>
		[JsonProperty("OriginalTotal")]
		public Currency OriginalTotal { get; set; }

		/// <summary>
		/// Gets or sets the processing status of the financial event group indicates whether the balance of the financial event group is settled.
		/// </summary>
		/// <value>The processing status of the financial event group indicates whether the balance of the financial event group is settled.</value>
		[JsonProperty("ProcessingStatus")]
		public ProcessingStatus? ProcessingStatus { get; set; }

		/// <summary>
		/// Gets or sets the trace identifier used by sellers to look up transactions externally.
		/// </summary>
		/// <value>The trace identifier used by sellers to look up transactions externally.</value>
		[JsonProperty("TraceId")]
		public string TraceId { get; set; }
	}
}

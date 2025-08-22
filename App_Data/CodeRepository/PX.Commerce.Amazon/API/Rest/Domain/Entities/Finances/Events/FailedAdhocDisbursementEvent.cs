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
	/// Represents a dto for storing a failed ad hoc disbursement event.
	/// </summary>
	public class FailedAdhocDisbursementEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the disbursement identifier.
		/// </summary>
		/// <value>The disbursement identifier.</value>
		[JsonProperty("DisbursementId")]
		public string DisbursementId { get; set; }

		/// <summary>
		/// Gets or sets the type of fund transfer.
		/// </summary>
		/// <value>The type of fund transfer.</value>
		[JsonProperty("FundsTransfersType")]
		public string FundsTransfersType { get; set; }

		/// <summary>
		/// Gets or sets the type of payment for disbursement.
		/// </summary>
		/// <value>The type of payment for disbursement.</value>
		[JsonProperty("PaymentDisbursementType")]
		public string PaymentDisbursementType { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the status of the failed AdhocDisbursement.
		/// </summary>
		/// <value>The status of the failed AdhocDisbursement.</value>
		[JsonProperty("Status")]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets the amount of the Adhoc Disbursement.
		/// </summary>
		/// <value>The amount of the Adhoc Disbursement.</value>
		[JsonProperty("TransferAmount")]
		public Currency TransferAmount { get; set; }

		/// <summary>
		/// Gets or sets the transfer identifier.
		/// </summary>
		/// <value>The transfer identifier.</value>
		[JsonProperty("TransferId")]
		public string TransferId { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.TransferAmount != null
			&& this.TransferAmount.IsValid
			&& !string.IsNullOrWhiteSpace(this.FundsTransfersType);
	}
}

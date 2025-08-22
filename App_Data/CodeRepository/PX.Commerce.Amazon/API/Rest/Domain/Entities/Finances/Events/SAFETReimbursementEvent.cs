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
	/// Represents a dto for storing a SAFE-T claim reimbursement on the seller's account.
	/// </summary>
	public class SAFETReimbursementEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets a code that indicates why the seller was reimbursed.
		/// </summary>
		/// <value>A code that indicates why the seller was reimbursed.</value>
		[JsonProperty("ReasonCode")]
		public string ReasonCode { get; set; }

		/// <summary>
		/// Gets or sets the amount of the reimbursement.
		/// </summary>
		/// <value>The amount of the reimbursement.</value>
		[JsonProperty("ReimbursedAmount")]
		public Currency ReimbursedAmount { get; set; }

		/// <summary>
		/// Gets or sets a SAFE-T claim identifier.
		/// </summary>
		/// <value>A SAFE-T claim identifier.</value>
		[JsonProperty("SAFETClaimId")]
		public string SAFETClaimId { get; set; }

		/// <summary>
		/// Gets or sets a list of items from a SAFE-T claim reimbursement.
		/// </summary>
		/// <value>A list of items from a SAFE-T claim reimbursement.</value>
		[JsonProperty("SAFETReimbursementItemList")]
		public List<SAFETReimbursementItem> SAFETReimbursementItemList { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.ReimbursedAmount != null
			&& this.ReimbursedAmount.IsValid
			&& string.IsNullOrWhiteSpace(this.ReasonCode);
	}
}

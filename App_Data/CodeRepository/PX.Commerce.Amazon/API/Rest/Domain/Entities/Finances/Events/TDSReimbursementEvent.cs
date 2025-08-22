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
	/// Represents a dto for storing an event related to a Tax-Deducted-at-Source (TDS) reimbursement.
	/// </summary>
	public class TDSReimbursementEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the amount reimbursed.
		/// </summary>
		/// <value>The amount reimbursed.</value>
		[JsonProperty("ReimbursedAmount")]
		public Currency ReimbursedAmount { get; set; }

		/// <summary>
		/// Gets or sets the Tax-Deducted-at-Source (TDS) identifier.
		/// </summary>
		/// <value>The Tax-Deducted-at-Source (TDS) identifier.</value>
		[JsonProperty("TDSOrderId")]
		public string TDSOrderId { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => this.ReimbursedAmount != null
			&& this.ReimbursedAmount.IsValid
			&& !string.IsNullOrWhiteSpace(this.TDSOrderId);
	}
}

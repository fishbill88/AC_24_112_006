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
	/// Represents a dto for storing an event related to charge refund.
	/// </summary>
	public class ChargeRefundEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the amount of the charge refund credit.
		/// </summary>
		/// <value>The amount of the charge refund credit.</value>
		[JsonProperty("ChargeRefundTransactions")]
		public ChargeRefundTransaction ChargeRefundTransaction { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the reason given for a charge refund.
		/// </summary>
		/// <value>The reason given for a charge refund.</value>
		[JsonProperty("ReasonCode")]
		public string ReasonCode { get; set; }

		/// <summary>
		/// Gets or sets the description of the Reason Code.
		/// </summary>
		/// <value>The description of the Reason Code.</value>
		/// <remarks>Example: SubscriptionFeeCorrection.</remarks>
		[JsonProperty("ReasonCodeDescription")]
		public string ReasonCodeDescription { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.ChargeRefundTransaction != null 
			&& this.ChargeRefundTransaction.IsValid;
	}
}

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

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing information related to an event linked to the payment of a fee related to the specified deal.
	/// </summary>
	public class SellerDealPaymentEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the internal description of the deal.
		/// </summary>
		/// <value>The internal description of the deal.</value>
		[JsonProperty("dealDescription")]
		public string DealDescription { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the deal.
		/// </summary>
		/// <value>The unique identifier of the deal.</value>
		[JsonProperty("dealId")]
		public string DealId { get; set; }

		/// <summary>
		/// Gets or sets the type of event: SellerDealComplete.
		/// </summary>
		/// <value>The type of event: SellerDealComplete.</value>
		[JsonProperty("eventType")]
		public string EventType { get; set; }

		/// <summary>
		/// Gets or sets the monetary amount of the fee.
		/// </summary>
		/// <value>The monetary amount of the fee.</value>
		[JsonProperty("feeAmount")]
		public Currency FeeAmount { get; set; }

		/// <summary>
		/// Gets or sets the type of fee: RunLightningDealFee.
		/// </summary>
		/// <value>The type of fee: RunLightningDealFee.</value>
		[JsonProperty("feeType")]
		public string FeeType { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("postedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the monetary amount of the tax applied.
		/// </summary>
		/// <value>The monetary amount of the tax applied.</value>
		[JsonProperty("taxAmount")]
		public Currency TaxAmount { get; set; }

		/// <summary>
		/// Gets or sets the total monetary amount paid.
		/// </summary>
		/// <value>The total monetary amount paid.</value>
		[JsonProperty("totalAmount")]
		public Currency TotalAmount { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => this.TotalAmount != null
			&& this.TotalAmount.IsValid
			&& !string.IsNullOrWhiteSpace(this.FeeType);
	}
}

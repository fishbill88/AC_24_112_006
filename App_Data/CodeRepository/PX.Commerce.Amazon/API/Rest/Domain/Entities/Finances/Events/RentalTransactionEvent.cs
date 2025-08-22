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
	/// Represents a dto for storing an event related to a rental transaction.
	/// </summary>
	public class RentalTransactionEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets an Amazon-defined identifier for an order.
		/// </summary>
		/// <value>An Amazon-defined identifier for an order.</value>
		[JsonProperty("AmazonOrderId")]
		public string AmazonOrderId { get; set; }

		/// <summary>
		/// Gets or sets the number of days that the buyer extended an already rented item.
		/// </summary>
		/// <value>The number of days that the buyer extended an already rented item.</value>
		/// <remarks>This value is only returned for RentalCustomerPayment-Extension and RentalCustomerRefund-Extension events.</remarks>
		[JsonProperty("ExtensionLength")]
		public int? ExtensionLength { get; set; }

		/// <summary>
		/// Gets or sets the name of the marketplace.
		/// </summary>
		/// <value>The name of the marketplace.</value>
		[JsonProperty("MarketplaceName")]
		public string MarketplaceName { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the type of rental event.
		/// </summary>
		/// <value>The type of rental event.</value>
		[JsonProperty("RentalEventType")]
		public string RentalEventType { get; set; }

		/// <summary>
		/// Gets or sets a list of charges associated with the rental event.
		/// </summary>
		/// <value>A list of charges associated with the rental event.</value>
		[JsonProperty("RentalChargeList")]
		public List<ChargeComponent> RentalChargeList { get; set; }

		/// <summary>
		/// Gets or sets a list of fees associated with the rental event.
		/// </summary>
		/// <value>A list of fees associated with the rental event.</value>
		[JsonProperty("RentalFeeList")]
		public List<FeeComponent> RentalFeeList { get; set; }

		/// <summary>
		/// Gets or sets the amount of money the customer originally paid to rent the item.
		/// </summary>
		/// <value>The amount of money the customer originally paid to rent the item.</value>
		/// <remarks>This value is only returned for RentalChargeFailureReimbursement and RentalLostItemReimbursement events.</remarks>
		[JsonProperty("RentalInitialValue")]
		public Currency RentalInitialValue { get; set; }

		/// <summary>
		/// Gets or sets the amount of money Amazon sends the seller to compensate for a lost item or a failed charge.
		/// </summary>
		/// <value>The amount of money Amazon sends the seller to compensate for a lost item or a failed charge.</value>
		/// <remarks>This value is only returned for RentalChargeFailureReimbursement and RentalLostItemReimbursement events.</remarks>
		[JsonProperty("RentalReimbursement")]
		public Currency RentalReimbursement { get; set; }

		/// <summary>
		/// Gets or sets a list of taxes withheld information for a rental item.
		/// </summary>
		/// <value>A list of taxes withheld information for a rental item.</value>
		[JsonProperty("RentalTaxWithheldList")]
		public List<TaxWithheldComponent> RentalTaxWithheldList { get; set; }
	}
}

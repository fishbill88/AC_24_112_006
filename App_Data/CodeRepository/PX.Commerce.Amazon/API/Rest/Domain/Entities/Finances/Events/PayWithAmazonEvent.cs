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
	/// Represents a dto for storing event information related to the seller's Pay with Amazon account.
	/// </summary>
	public class PayWithAmazonEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets a short description of this payment event.
		/// </summary>
		/// <value>A short description of this payment event.</value>
		[JsonProperty("AmountDescription")]
		public string AmountDescription { get; set; }

		/// <summary>
		/// Gets or sets the type of business object.
		/// </summary>
		/// <value>The type of business object.</value>
		[JsonProperty("BusinessObjectType")]
		public string BusinessObjectType { get; set; }

		/// <summary>
		/// Gets or sets the charge associated with the event.
		/// </summary>
		/// <value>The charge associated with the event.</value>
		[JsonProperty("Charge")]
		public ChargeComponent Charge { get; set; }

		/// <summary>
		/// Gets or sets a list of fees associated with the event.
		/// </summary>
		/// <value>A list of fees associated with the event.</value>
		[JsonProperty("FeeList")]
		public List<FeeComponent> FeeList { get; set; }

		/// <summary>
		/// Gets or sets the fulfillment channel.
		/// </summary>
		/// <value>The fulfillment channel.</value>
		[JsonProperty("FulfillmentChannel")]
		public FulfillmentChannelEnum? FulfillmentChannel { get; set; }

		/// <summary>
		/// Gets or sets the type of payment.
		/// </summary>
		/// <value>The type of payment.</value>
		[JsonProperty("PaymentAmountType")]
		public string PaymentAmountType { get; set; }

		/// <summary>
		/// Gets or sets the sales channel for the transaction.
		/// </summary>
		/// <value>The sales channel for the transaction.</value>
		[JsonProperty("SalesChannel")]
		public string SalesChannel { get; set; }

		/// <summary>
		/// Gets or sets an order identifier that is specified by the seller.
		/// </summary>
		/// <value>An order identifier that is specified by the seller.</value>
		[JsonProperty("SellerOrderId")]
		public string SellerOrderId { get; set; }

		/// <summary>
		/// Gets or sets the store name where the event occurred.
		/// </summary>
		/// <value>The store name where the event occurred.</value>
		[JsonProperty("StoreName")]
		public string StoreName { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the payment transaction is posted.
		/// </summary>
		/// <value>The date and time when the payment transaction is posted.</value>
		/// <remarks>In ISO 8601 date time format.</remarks>
		[JsonProperty("TransactionPostedDate")]
		public DateTime? TransactionPostedDate { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => this.FeeList != null;
	}
}

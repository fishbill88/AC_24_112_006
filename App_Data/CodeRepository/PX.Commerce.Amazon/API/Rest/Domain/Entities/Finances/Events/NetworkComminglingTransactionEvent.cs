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
	/// Represents a dto for storing a network commingling transaction event.
	/// </summary>
	public class NetworkComminglingTransactionEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the Amazon Standard Identification Number (ASIN) of the swapped item.
		/// </summary>
		/// <value>The Amazon Standard Identification Number (ASIN) of the swapped item.</value>
		[JsonProperty("ASIN")]
		public string Asin { get; set; }

		/// <summary>
		/// Gets or sets the marketplace in which the event took place.
		/// </summary>
		/// <value>The marketplace in which the event took place.</value>
		[JsonProperty("MarketplaceId")]
		public string MarketplaceId { get; set; }

		/// <summary>
		/// Gets or sets the identifier for the network item swap.
		/// </summary>
		/// <value>The identifier for the network item swap.</value>
		[JsonProperty("NetCoTransactionID")]
		public string NetCoTransactionID { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the reason for the network item swap.
		/// </summary>
		/// <value>The reason for the network item swap.</value>
		[JsonProperty("SwapReason")]
		public string SwapReason { get; set; }

		/// <summary>
		/// Gets or sets the tax on the network item swap paid by the seller.
		/// </summary>
		/// <value>The tax on the network item swap paid by the seller.</value>
		[JsonProperty("TaxAmount")]
		public Currency TaxAmount { get; set; }

		/// <summary>
		/// Gets or sets the price of the swapped item minus TaxAmount.
		/// </summary>
		/// <value>The price of the swapped item minus TaxAmount.</value>
		[JsonProperty("TaxExclusiveAmount")]
		public Currency TaxExclusiveAmount { get; set; }

		/// <summary>
		/// Gets or sets the type of network item swap.
		/// </summary>
		/// <value>The type of network item swap.</value>
		[JsonProperty("TransactionType")]
		public string TransactionType { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.TaxAmount != null
			&& this.TaxAmount.IsValid
			&& !string.IsNullOrWhiteSpace(this.TransactionType);
	}
}

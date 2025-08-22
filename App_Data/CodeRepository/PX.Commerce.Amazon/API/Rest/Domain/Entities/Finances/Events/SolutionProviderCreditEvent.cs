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
	/// Represents a dto for storing event information for credit given to a solution provider.
	/// </summary>
	public class SolutionProviderCreditEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the two-letter country code of the country associated with the marketplace where the order was placed.
		/// </summary>
		/// <value>The two-letter country code of the country associated with the marketplace where the order was placed.</value>
		[JsonProperty("MarketplaceCountryCode")]
		public string MarketplaceCountryCode { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the marketplace where the order was placed.
		/// </summary>
		/// <value>The identifier of the marketplace where the order was placed.</value>
		[JsonProperty("MarketplaceId")]
		public string MarketplaceId { get; set; }

		/// <summary>
		/// Gets or sets the Amazon-defined identifier of the solution provider.
		/// </summary>
		/// <value>The Amazon-defined identifier of the solution provider.</value>
		[JsonProperty("ProviderId")]
		public string ProviderId { get; set; }

		/// <summary>
		/// Gets or sets the transaction type.
		/// </summary>
		/// <value>The transaction type.</value>
		[JsonProperty("ProviderTransactionType")]
		public string ProviderTransactionType { get; set; }

		/// <summary>
		/// Gets or sets the store name where the payment event occurred.
		/// </summary>
		/// <value>The store name where the payment event occurred.</value>
		[JsonProperty("ProviderStoreName")]
		public string ProviderStoreName { get; set; }

		/// <summary>
		/// Gets or sets the Amazon-defined identifier of the seller.
		/// </summary>
		/// <value>The Amazon-defined identifier of the seller.</value>
		[JsonProperty("SellerId")]
		public string SellerId { get; set; }

		/// <summary>
		/// Gets or sets a seller-defined identifier for an order.
		/// </summary>
		/// <value>A seller-defined identifier for an order.</value>
		[JsonProperty("SellerOrderId")]
		public string SellerOrderId { get; set; }

		/// <summary>
		/// Gets or sets the store name where the payment event occurred.
		/// </summary>
		/// <value>The store name where the payment event occurred.</value>
		[JsonProperty("SellerStoreName")]
		public string SellerStoreName { get; set; }

		/// <summary>
		/// Gets or sets the amount of the credit.
		/// </summary>
		/// <value>The amount of the credit.</value>
		[JsonProperty("TransactionAmount")]
		public Currency TransactionAmount { get; set; }

		/// <summary>
		/// Gets or sets the date and time that the credit transaction was created.
		/// </summary>
		/// <value>The date and time that the credit transaction was created.</value>
		/// <remarks>In ISO 8601 date time format.</remarks>
		[JsonProperty("TransactionCreationDate")]
		public DateTime? TransactionCreationDate { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.TransactionAmount != null
			&& this.TransactionAmount.IsValid
			&& !string.IsNullOrWhiteSpace(this.ProviderTransactionType);
	}
}

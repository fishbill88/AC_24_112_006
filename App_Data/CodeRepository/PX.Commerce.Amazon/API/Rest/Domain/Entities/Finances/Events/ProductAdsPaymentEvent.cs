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
	/// Represents a dto for storing information related to a Sponsored Products payment event.
	/// </summary>
	public class ProductAdsPaymentEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the base amount of the transaction, before tax.
		/// </summary>
		/// <value>The base amount of the transaction, before tax.</value>
		[JsonProperty("baseValue")]
		public Currency BaseValue { get; set; }

		/// <summary>
		/// Gets or sets the identifier for the invoice that the transaction appears in.
		/// </summary>
		/// <value>The identifier for the invoice that the transaction appears in.</value>
		[JsonProperty("invoiceId")]
		public string InvoiceId { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("postedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the tax amount of the transaction.
		/// </summary>
		/// <value>The tax amount of the transaction.</value>
		[JsonProperty("taxValue")]
		public Currency TaxValue { get; set; }

		/// <summary>
		/// Gets or sets the transaction type.
		/// </summary>
		/// <value>The transaction type.</value>
		[JsonProperty("transactionType")]
		public string TransactionType { get; set; }

		/// <summary>
		/// Gets or sets The total amount of the transaction.
		/// </summary>
		/// <value>The total amount of the transaction.</value>
		/// <remarks>Equal to baseValue + taxValue.</remarks>
		[JsonProperty("transactionValue")]
		public Currency TransactionValue { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => 
			this.TransactionValue != null
			&& this.TransactionValue.IsValid
			&& !string.IsNullOrWhiteSpace(this.TransactionType);
	}
}

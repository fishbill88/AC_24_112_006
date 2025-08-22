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
	/// Represents a dto for storing an expense related to an affordability promotion.
	/// </summary>
	public class AffordabilityExpenseEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets an Amazon-defined identifier for an order.
		/// </summary>
		/// <value>An Amazon-defined identifier for an order.</value>
		[JsonProperty("AmazonOrderId")]
		public string AmazonOrderId { get; set; }

		/// <summary>
		/// Gets or sets the amount charged for clicks incurred under the Sponsored Products program.
		/// </summary>
		/// <value>The amount charged for clicks incurred under the Sponsored Products program.</value>
		[JsonProperty("BaseExpense")]
		public Currency BaseExpense { get; set; }

		/// <summary>
		/// Gets or sets an encrypted, Amazon-defined marketplace identifier.
		/// </summary>
		/// <value>An encrypted, Amazon-defined marketplace identifier.</value>
		[JsonProperty("MarketplaceId")]
		public string MarketplaceId { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was created.
		/// </summary>
		/// <value>The date and time when the financial event was created.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the Central Goods and Service Tax, charged and collected by the central government.
		/// </summary>
		/// <value>The Central Goods and Service Tax, charged and collected by the central government.</value>
		[JsonProperty("TaxTypeCGST")]
		public Currency TaxTypeCGST { get; set; }

		/// <summary>
		/// Gets or sets the Integrated Goods and Service Tax, charged and collected by the central government.
		/// </summary>
		/// <value>The Integrated Goods and Service Tax, charged and collected by the central government.</value>
		[JsonProperty("TaxTypeIGST")]
		public Currency TaxTypeIGST { get; set; }

		/// <summary>
		/// Gets or sets the State Goods and Service Tax, charged and collected by the state government.
		/// </summary>
		/// <value>The State Goods and Service Tax, charged and collected by the state government.</value>
		[JsonProperty("TaxTypeSGST")]
		public Currency TaxTypeSGST { get; set; }

		/// <summary>
		/// Gets or sets the total amount charged to the seller.
		/// </summary>
		/// <value>The total amount charged to the seller.</value>
		/// <remarks>TotalExpense = BaseExpense + TaxTypeIGST + TaxTypeCGST + TaxTypeSGST.</remarks>
		[JsonProperty("TotalExpense")]
		public Currency TotalExpense { get; set; }

		/// <summary>
		/// Gets or sets the type of transaction.
		/// </summary>
		/// <value>The type of transaction.</value>
		[JsonProperty("TransactionType")]
		public string TransactionType { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.TotalExpense != null
			&& this.TotalExpense.IsValid
			&& !string.IsNullOrWhiteSpace(this.TransactionType);
	}
}

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
	/// Represents a dto for storing an event related to a capacity reservation billing charge.
	/// </summary>
	public class CapacityReservationBillingEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets a short description of the capacity reservation billing event.
		/// </summary>
		/// <value>A short description of the capacity reservation billing event.</value>
		[JsonProperty("Description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the amount of the capacity reservation billing event.
		/// </summary>
		/// <value>The amount of the capacity reservation billing event.</value>
		[JsonProperty("TransactionAmount")]
		public Currency TransactionAmount { get; set; }

		/// <summary>
		/// Gets or sets the type of transaction.
		/// </summary>
		/// <value>The type of transaction.</value>
		/// <remarks>Example: "FBA Inventory Fee".</remarks>
		[JsonProperty("TransactionType")]
		public string TransactionType { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.TransactionAmount != null 
			&& this.TransactionAmount.IsValid 
			&& !string.IsNullOrWhiteSpace(this.TransactionType);
	}
}

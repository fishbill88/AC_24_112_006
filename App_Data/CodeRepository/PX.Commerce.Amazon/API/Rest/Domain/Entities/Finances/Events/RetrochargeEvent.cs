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
	/// Represents a dto for storing a retrocharge or retrocharge reversal event.
	/// </summary>
	public class RetrochargeEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets os sets an Amazon-defined identifier for an order.
		/// </summary>
		/// <value>An Amazon-defined identifier for an order.</value>
		[JsonProperty("AmazonOrderId")]
		public string AmazonOrderId { get; set; }

		/// <summary>
		/// Gets os sets the base tax associated with the retrocharge event.
		/// </summary>
		/// <value>The base tax associated with the retrocharge event.</value>
		[JsonProperty("BaseTax")]
		public Currency BaseTax { get; set; }

		/// <summary>
		/// Gets os sets the name of the marketplace where the retrocharge event occurred.
		/// </summary>
		/// <value>The name of the marketplace where the retrocharge event occurred.</value>
		[JsonProperty("MarketplaceName")]
		public string MarketplaceName { get; set; }

		/// <summary>
		/// Gets os sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets os sets the type of event.
		/// </summary>
		/// <value>The type of event.</value>
		[JsonProperty("RetrochargeEventType")]
		public string RetrochargeEventType { get; set; }

		/// <summary>
		/// Gets os sets a list of information about taxes withheld.
		/// </summary>
		/// <value>A list of information about taxes withheld.</value>
		[JsonProperty("RetrochargeTaxWithheldList")]
		public List<TaxWithheldComponent> RetrochargeTaxWithheldList { get; set; }

		/// <summary>
		/// Gets os sets the shipping  tax associated with the retrocharge event.
		/// </summary>
		/// <value>The shipping  tax associated with the retrocharge event.</value>
		[JsonProperty("ShippingTax")]
		public Currency ShippingTax { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => this.RetrochargeTaxWithheldList != null;
	}
}

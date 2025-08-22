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
using System;
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing an item from a SAFE-T claim reimbursement.
	/// </summary>
	public class SAFETReimbursementItem
	{
		/// <summary>
		/// Gets or sets a list of charges associated with the item.
		/// </summary>
		/// <value>A list of charges associated with the item.</value>
		[JsonProperty("itemChargeList")]
		public List<ChargeComponent> ItemChargeList { get; set; }

		/// <summary>
		/// Gets or sets the description of the item as shown on the product detail page on the retail website.
		/// </summary>
		/// <value>The description of the item as shown on the product detail page on the retail website.</value>
		[JsonProperty("productDescription")]
		public string ProductDescription { get; set; }

		/// <summary>
		/// Gets or sets the number of units of the item being reimbursed.
		/// </summary>
		/// <value>The number of units of the item being reimbursed.</value>
		[JsonProperty("quantity")]
		public int? Quantity { get; set; }
	}
}

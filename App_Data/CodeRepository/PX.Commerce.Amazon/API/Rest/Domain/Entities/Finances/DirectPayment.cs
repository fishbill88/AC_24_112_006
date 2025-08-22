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
	/// Represents a dto for storing a payment made directly to a seller.
	/// </summary>
	public class DirectPayment
	{
		/// <summary>
		/// Gets or sets the amount of the direct payment.
		/// </summary>
		/// <value>The amount of the direct payment.</value>
		[JsonProperty("DirectPaymentAmount")]
		public Currency DirectPaymentAmount { get; set; }

		/// <summary>
		/// Gets or sets the type of payment.
		/// </summary>
		/// <value>The type of payment.</value>
		[JsonProperty("DirectPaymentType")]
		public string DirectPaymentType { get; set; }
	}
}

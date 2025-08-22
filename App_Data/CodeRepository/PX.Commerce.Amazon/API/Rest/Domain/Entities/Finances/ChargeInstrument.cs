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

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing information related to payment instrument.
	/// </summary>
	public class ChargeInstrument
	{
		/// <summary>
		/// Gets or sets a short description of the charge instrument.
		/// </summary>
		/// <value>A short description of the charge instrument.</value>
		[JsonProperty("Description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the account tail (trailing digits) of the charge instrument.
		/// </summary>
		/// <value>The account tail (trailing digits) of the charge instrument.</value>
		[JsonProperty("Tail")]
		public string Tail { get; set; }

		/// <summary>
		/// Gets or sets the amount charged to this charge instrument.
		/// </summary>
		/// <value>The amount charged to this charge instrument.</value>
		[JsonProperty("Amount")]
		public Currency Amount { get; set; }
	}
}

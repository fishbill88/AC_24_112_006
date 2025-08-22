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

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing a fee associated with an event.
	/// </summary>
	public class FeeComponent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the type of fee.
		/// </summary>
		/// <value>The type of fee.</value>
		[JsonProperty("FeeType")]
		public string FeeType { get; set; }

		/// <summary>
		/// Gets or sets the amount of the fee.
		/// </summary>
		/// <value>The amount of the fee.</value>
		[JsonProperty("FeeAmount")]
		public Currency FeeAmount { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.FeeAmount != null
			&& this.FeeAmount.IsValid
			&& !string.IsNullOrWhiteSpace(this.FeeType);
	}
}

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
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing a charge refund transaction.
	/// </summary>
	public class ChargeRefundTransaction
	{
		/// <summary>
		/// Gets or sets the amount of the charge refund credit.
		/// </summary>
		/// <value>The amount of the charge refund credit.</value>
		[JsonProperty("ChargeAmount")]
		public Currency ChargeAmount { get; set; }

		/// <summary>
		/// Gets or sets the type of charge.
		/// </summary>
		/// <value>The type of charge.</value>
		[JsonProperty("ChargeType")]
		public string ChargeType { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid => this.ChargeAmount != null 
			&& this.ChargeAmount.IsValid
			&& !string.IsNullOrWhiteSpace(this.ChargeType);
	}
}

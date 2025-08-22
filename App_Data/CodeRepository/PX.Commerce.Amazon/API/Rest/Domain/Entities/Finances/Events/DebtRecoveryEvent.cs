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
using System.Collections.Generic;

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing information related to a debt payment or debt adjustment.
	/// </summary>
	public class DebtRecoveryEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets a list of payment instruments.
		/// </summary>
		/// <value>A list of payment instruments.</value>
		[JsonProperty("ChargeInstrumentList")]
		public List<ChargeInstrument> ChargeInstrumentList { get; set; }

		/// <summary>
		/// Gets or sets a list of debt recovery item information.
		/// </summary>
		/// <value>A list of debt recovery item information.</value>
		[JsonProperty("DebtRecoveryItemList")]
		public List<DebtRecoveryItem> DebtRecoveryItemList { get; set; }

		/// <summary>
		/// Gets or sets the debt recovery type.
		/// </summary>
		/// <value>The debt recovery type.</value>
		[JsonProperty("DebtRecoveryType")]
		public string DebtRecoveryType { get; set; }

		/// <summary>
		/// Gets or sets the amount returned for overpayment.
		/// </summary>
		/// <value>The amount returned for overpayment.</value>
		[JsonProperty("OverPaymentCredit")]
		public Currency OverPaymentCredit { get; set; }

		/// <summary>
		/// Gets or sets the amount applied for recovery.
		/// </summary>
		/// <value>The amount applied for recovery.</value>
		[JsonProperty("RecoveryAmount")]
		public Currency RecoveryAmount { get; set; }

		/// <summary>
		/// Defines if the recovery transaction contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsRecoveryValid =>
			this.RecoveryAmount != null
			&& this.RecoveryAmount.IsValid
			&& !string.IsNullOrWhiteSpace(this.DebtRecoveryType);

		/// <summary>
		/// Defines if the overpayment transaction contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsOverPaymentValid =>
			this.OverPaymentCredit != null
			&& this.OverPaymentCredit.IsValid
			&& !string.IsNullOrWhiteSpace(this.DebtRecoveryType);
	}
}

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
	/// Represents a dto for storing information related to an item of a debt payment or debt adjustment.
	/// </summary>
	public class DebtRecoveryItem
	{
		/// <summary>
		/// Gets or sets the beginning date and time of the financial event group that contains the debt.
		/// </summary>
		/// <value>The beginning date and time of the financial event group that contains the debt.</value>
		/// <remarks>In ISO 8601 date time format.</remarks>
		[JsonProperty("GroupBeginDate")]
		public DateTime? GroupBeginDate { get; set; }

		/// <summary>
		/// Gets or sets the ending date and time of the financial event group that contains the debt.
		/// </summary>
		/// <value>The ending date and time of the financial event group that contains the debt.</value>
		/// <remarks>In ISO 8601 date time format.</remarks>
		[JsonProperty("GroupEndDate")]
		public DateTime? GroupEndDate { get; set; }

		/// <summary>
		/// Gets or sets the original debt amount.
		/// </summary>
		/// <value>The original debt amount.</value>
		[JsonProperty("OriginalAmount")]
		public Currency OriginalAmount { get; set; }

		/// <summary>
		/// Gets or sets the amount applied for the recovery item.
		/// </summary>
		/// <value>The amount applied for the recovery item.</value>
		[JsonProperty("RecoveryAmount")]
		public Currency RecoveryAmount { get; set; }
	}
}

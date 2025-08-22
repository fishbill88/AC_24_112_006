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
	/// Represents a dto for storing a payment event for Fulfillment by Amazon (FBA) inventory liquidation.
	/// </summary>
	/// <remarks>This event is used only in the US marketplace.</remarks>
	public class FBALiquidationEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the amount paid by the liquidator for the seller's inventory.
		/// </summary>
		/// <value>The amount paid by the liquidator for the seller's inventory.</value>
		/// <remarks>The seller receives this amount minus LiquidationFeeAmount.</remarks>
		[JsonProperty("LiquidationProceedsAmount")]
		public Currency LiquidationProceedsAmount { get; set; }

		/// <summary>
		/// Gets or sets the fee charged to the seller by Amazon for liquidating the seller's FBA inventory.
		/// </summary>
		/// <value>The fee charged to the seller by Amazon for liquidating the seller's FBA inventory.</value>
		[JsonProperty("LiquidationFeeAmount")]
		public Currency LiquidationFeeAmount { get; set; }

		/// <summary>
		/// Gets or sets the identifier for the original removal order.
		/// </summary>
		/// <value>The identifier for the original removal order.</value>
		[JsonProperty("OriginalRemovalOrderId")]
		public string OriginalRemovalOrderId { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Defines if the instance contains all nesessary information and could be considered as valid.
		/// </summary>
		/// <value>True if the instanse is valid; otherwise - false.</value>
		public bool IsValid =>
			this.LiquidationProceedsAmount != null
			&& this.LiquidationProceedsAmount.IsValid
			&& this.LiquidationFeeAmount != null
			&& this.LiquidationFeeAmount.IsValid
			&& !string.IsNullOrWhiteSpace(this.OriginalRemovalOrderId);
	}
}

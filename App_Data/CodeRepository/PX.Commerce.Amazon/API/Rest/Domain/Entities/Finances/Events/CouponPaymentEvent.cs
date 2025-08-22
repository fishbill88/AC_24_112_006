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

namespace PX.Commerce.Amazon.API.Rest
{
	/// <summary>
	/// Represents a dto for storing an adjustment to the seller's account.
	/// </summary>
	public class CouponPaymentEvent : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets the number of coupon clips or redemptions.
		/// </summary>
		/// <value>The number of coupon clips or redemptions.</value>
		[JsonProperty("ClipOrRedemptionCount")]
		public long? ClipOrRedemptionCount { get; set; }

		/// <summary>
		/// Gets or sets a charge on the seller's account.
		/// </summary>
		/// <value>A charge on the seller's account.</value>
		[JsonProperty("ChargeComponent")]
		public ChargeComponent ChargeComponent { get; set; }

		/// <summary>
		/// Gets or sets a coupon identifier.
		/// </summary>
		/// <value>A coupon identifier.</value>
		[JsonProperty("CouponId")]
		public string CouponId { get; set; }

		/// <summary>
		/// Gets or sets a fee associated with the event.
		/// </summary>
		/// <value>A fee associated with the event.</value>
		[JsonProperty("FeeComponent")]
		public FeeComponent FeeComponent { get; set; }

		/// <summary>
		/// Gets or sets a payment event identifier.
		/// </summary>
		/// <value>A payment event identifier.</value>
		[JsonProperty("PaymentEventId")]
		public string PaymentEventId { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the financial event was posted.
		/// </summary>
		/// <value>The date and time when the financial event was posted.</value>
		[JsonProperty("PostedDate")]
		public DateTime? PostedDate { get; set; }

		/// <summary>
		/// Gets or sets the description provided by the seller when they created the coupon.
		/// </summary>
		/// <value>The description provided by the seller when they created the coupon.</value>
		[JsonProperty("SellerCouponDescription")]
		public string SellerCouponDescription { get; set; }

		/// <summary>
		/// Gets or sets the FeeComponent value plus the ChargeComponent value.
		/// </summary>
		/// <value>The FeeComponent value plus the ChargeComponent value.</value>
		[JsonProperty("TotalAmount")]
		public Currency TotalAmount { get; set; }
	}
}

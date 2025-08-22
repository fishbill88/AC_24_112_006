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

namespace PX.Commerce.Amazon.API.Rest.Constants
{
	/// <summary>
	/// Stores the values for order statuses.
	/// </summary>
	public class OrderStatus
	{
		/// <summary>
		/// Stores a Pending order status.
		/// </summary>
		public static string Pending = "Pending";

		/// <summary>
		/// Stores a Unshipped order status.
		/// </summary>
		public static string Unshipped = "Unshipped";

		/// <summary>
		/// Stores a PartiallyShipped order status.
		/// </summary>
		public static string PartiallyShipped = "PartiallyShipped";

		/// <summary>
		/// Stores a Shipped order status.
		/// </summary>
		public static string Shipped = "Shipped";

		/// <summary>
		/// Stores a Canceled order status.
		/// </summary>
		public static string Canceled = "Canceled";

		/// <summary>
		/// Stores a Unfulfillable order status.
		/// </summary>
		public static string Unfulfillable = "Unfulfillable";

		/// <summary>
		/// Stores a InvoiceUnconfirmed order status.
		/// </summary>
		public static string InvoiceUnconfirmed = "InvoiceUnconfirmed";

		/// <summary>
		/// Stores a PendingAvailability order status.
		/// </summary>
		public static string PendingAvailability = "PendingAvailability";
	}
}

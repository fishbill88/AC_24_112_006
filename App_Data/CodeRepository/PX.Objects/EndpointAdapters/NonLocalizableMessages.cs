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

namespace PX.Objects.EndpointAdapters
{
	public static class NonLocalizableMessages
	{
		#region AR
		public const string PaymentInSOOrderNotFound = "Payment {0} {1} was not found in the list of payments applied to order.";
		#endregion

		#region AP
		public const string MissingPOOrderReference = "Both POOrderType and POOrderNumber must be provided to add a Purchase Order to details.";
		public const string PurchaseOrderDoesNotExist = "Purchase order {0} - {1} was not found.";
		public const string SubcontractNotFound = "Subcontract {0} was not found.";
		public const string SubcontractLineNotFound = "Subcontract {0}, Line Nbr.: {1} was not found.";
		public const string PurchaseOrderLineNotFound = "Order Line: {0} {1}, Line Nbr.: {2} not found.";
		public const string PurchaseReceiptNotFound = "Purchase Receipt {0} was not found.";
		public const string PurchaseReceiptLineNotFound = "Receipt Line {0} - {1} not found.";
		#endregion
	}
}

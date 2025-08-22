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

using System;
using PX.Data;
using PX.Objects.AP;

namespace PX.Objects.PO
{
	[Serializable]
	public partial class POOrderFilter : PXBqlTable, IBqlTable
	{
		#region VendorID
		public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID>
		{
		}

		protected Int32? _VendorID;
		[VendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
		[PXDefault(typeof(APInvoice.vendorID))]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion

		#region OrderNbr
		public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
		[PXDBString(15, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(
			typeof(Search5<POOrder.orderNbr,
				InnerJoin<POLine, On<POLine.orderType, Equal<POOrder.orderType>,
					And<POLine.orderNbr, Equal<POOrder.orderNbr>>>>,
				Where<POOrder.orderType, NotIn3<POOrderType.blanket, POOrderType.standardBlanket>,
					And<POOrder.curyID, Equal<Current<APInvoice.curyID>>,
					And<POOrder.status, In3<POOrderStatus.open, POOrderStatus.completed>,
					And<POLine.cancelled, NotEqual<True>,
					And<POLine.closed, NotEqual<True>,
					And<Where<Current<APInvoice.docType>, Equal<APDocType.prepayment>, Or<POLine.pOAccrualType, Equal<POAccrualType.order>>>>>>>>>,
				Aggregate<GroupBy<POOrder.orderType, GroupBy<POOrder.orderNbr>>>>),
		   typeof(POOrder.orderType),
		   typeof(POOrder.orderNbr),
		   typeof(POOrder.orderDate),
		   typeof(POOrder.vendorID),
		   typeof(POOrder.vendorID_Vendor_acctName),
		   typeof(POOrder.vendorLocationID),
		   typeof(POOrder.curyID),
		   typeof(POOrder.curyOrderTotal), Filterable = true)]
		public virtual string OrderNbr { get; set; }
		#endregion

		#region ShowUnbilled
		public abstract class showBilledLines : PX.Data.BQL.BqlBool.Field<showBilledLines>
		{
		}
		/// <summary>
		/// Get or set CreateBill that mark current document create bill on release.
		/// </summary>
		[PXBool]
		[PXUIField(DisplayName = "Show Billed Lines", Enabled = true)]
		public virtual Boolean? ShowBilledLines
		{
			get;
			set;
		}
		#endregion
	}
}

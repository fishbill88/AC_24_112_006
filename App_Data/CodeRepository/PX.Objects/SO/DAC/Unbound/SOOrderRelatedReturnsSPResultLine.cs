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
using PX.Data.BQL;
using PX.Objects.IN;

namespace PX.Objects.SO.DAC.Unbound
{
	/// <exclude/>
	/// <summary>
	/// The DAC that represent the result line of Return Documents Related to Sales Order side panel inquiry of the sales orders form.
	/// </summary>
	[PXCacheName(Messages.SOOrderRelatedReturnsSPResultLine)]
	public class SOOrderRelatedReturnsSPResultLine : PXBqlTable, IBqlTable
	{
		#region OrderType
		public abstract class orderType : BqlString.Field<orderType> { }
		/// <exclude/>
		[PXString(2, IsFixed = true, IsKey = true)]
		[PXUIField(DisplayName = "Order Type")]
		public virtual String OrderType
		{
			get;
			set;
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : BqlString.Field<orderNbr> { }
		/// <exclude/>
		[PXString(15, IsUnicode = true, IsKey = true)]
		[PXUIField(DisplayName = "Order Nbr.")]
		public virtual String OrderNbr
		{
			get;
			set;
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : BqlInt.Field<lineNbr> { }
		/// <exclude/>
		[PXInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visible = false)]
		public virtual Int32? LineNbr
		{
			get;
			set;
		}
		#endregion

		#region ReturnOrderType
		public abstract class returnOrderType : BqlString.Field<returnOrderType> { }
		/// <exclude/>
		[PXString(2, IsFixed = true, IsKey = true)]
		public virtual String ReturnOrderType
		{
			get;
			set;
		}
		#endregion
		#region ReturnOrderNbr
		public abstract class returnOrderNbr : BqlString.Field<returnOrderNbr> { }
		/// <exclude/>
		[PXString(15, IsUnicode = true, IsKey = true)]
		public virtual String ReturnOrderNbr
		{
			get;
			set;
		}
		#endregion
		#region ReturnLineType
		public abstract class returnLineType : BqlString.Field<returnLineType> { }
		/// <exclude/>
		[PXString(2, IsFixed = true)]
		public virtual String ReturnLineType
		{
			get;
			set;
		}
		#endregion

		#region DisplayReturnOrderType
		public abstract class displayReturnOrderType : BqlString.Field<displayReturnOrderType> { }
		/// <exclude/>
		[PXString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Return Order Type", Visible = false)]
		public virtual String DisplayReturnOrderType
		{
			get;
			set;
		}
		#endregion
		#region DisplayReturnOrderNbr
		public abstract class displayReturnOrderNbr : BqlString.Field<displayReturnOrderNbr> { }
		/// <exclude/>
		[PXString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Return Order Nbr.")]
		[PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType, Equal<Current<displayReturnOrderType>>>>))]
		public virtual String DisplayReturnOrderNbr
		{
			get;
			set;
		}
		#endregion

		#region ReturnInvoiceType
		public abstract class returnInvoiceType : BqlString.Field<returnInvoiceType> { }
		/// <exclude/>
		[PXString(3, IsKey = true, IsFixed = true)]
		public virtual String ReturnInvoiceType
		{
			get;
			set;
		}
		#endregion
		#region ReturnInvoiceNbr
		public abstract class returnInvoiceNbr : BqlString.Field<returnInvoiceNbr> { }
		/// <exclude/>
		[PXString(15, IsUnicode = true, IsKey = true)]
		public virtual String ReturnInvoiceNbr
		{
			get;
			set;
		}
		#endregion
		#region DisplayReturnInvoiceType
		public abstract class displayReturnInvoiceType : BqlString.Field<displayReturnInvoiceType> { }
		/// <exclude/>
		[PXString(3, IsFixed = true)]
		[AR.ARDocType.List()]
		[PXUIField(DisplayName = "Return Invoice Type", Visible = false)]
		public virtual String DisplayReturnInvoiceType
		{
			get;
			set;
		}
		#endregion
		#region DisplayReturnInvoiceNbr
		public abstract class displayReturnInvoiceNbr : BqlString.Field<displayReturnInvoiceNbr> { }
		/// <exclude/>
		[PXString(15, IsUnicode = true)]
		[PXSelector(typeof(Search<SOInvoice.refNbr, Where<SOInvoice.docType, Equal<Current<displayReturnInvoiceType>>>>))]
		[PXUIField(DisplayName = "Return Invoice Nbr.")]
		public virtual String DisplayReturnInvoiceNbr
		{
			get;
			set;
		}
		#endregion

		#region InventoryID
		public abstract class inventoryID : BqlInt.Field<inventoryID> { }
		/// <exclude/>
		[AnyInventory]
		public virtual Int32? InventoryID
		{
			get;
			set;
		}
		#endregion
		#region BaseUnit
		public abstract class baseUnit : BqlString.Field<baseUnit> { }
		/// <exclude/>
		[PXString(6, IsUnicode = true)]
		[PXUIField(DisplayName = "Base UOM")]
		public virtual String BaseUnit
		{
			get;
			set;
		}
		#endregion
		#region ReturnedQty
		public abstract class returnedQty : BqlDecimal.Field<returnedQty> { }
		/// <exclude/>
		[PXDecimal(typeof(Search<CS.CommonSetup.decPlQty>))]
		[PXUIField(DisplayName = "Qty. on Return")]
		public virtual Decimal? ReturnedQty
		{
			get;
			set;
		}
		#endregion
	}
}

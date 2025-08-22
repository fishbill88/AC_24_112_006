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
using PX.Objects.AP;

namespace PX.Objects.SO.DAC.Unbound
{
	/// <summary>
	/// The DAC that represent the result line of Return Documents Related to Sales Order side panel inquiry of the sales orders form.
	/// </summary>
	[PXCacheName(Messages.SOOrderRelatedReturnsSPResultDoc)]
	public class SOOrderRelatedReturnsSPResultDoc : PXBqlTable, IBqlTable
	{
		#region GridLineNbr
		public abstract class gridLineNbr : BqlInt.Field<gridLineNbr> { }
		/// <exclude/>
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual Int32? GridLineNbr
		{
			get;
			set;
		}
		#endregion

		#region OrderType
		public abstract class orderType : BqlString.Field<orderType> { }
		/// <exclude/>
		[PXString(2, IsFixed = true)]
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
		[PXString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Order Nbr.")]
		public virtual String OrderNbr
		{
			get;
			set;
		}
		#endregion

		#region ReturnOrderType
		public abstract class returnOrderType : BqlString.Field<returnOrderType> { }
		/// <exclude/>
		[PXString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Return Order Type", Visible = false)]
		public virtual String ReturnOrderType
		{
			get;
			set;
		}
		#endregion
		#region ReturnOrderNbr
		public abstract class returnOrderNbr : BqlString.Field<returnOrderNbr> { }
		/// <exclude/>
		[PXString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Return Order Nbr.")]
		[PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType, Equal<Current<returnOrderType>>>>))]
		public virtual String ReturnOrderNbr
		{
			get;
			set;
		}
		#endregion

		#region ReturnInvoiceType
		public abstract class returnInvoiceType : BqlString.Field<returnInvoiceType> { }
		/// <exclude/>
		[PXString(3, IsFixed = true)]
		[AR.ARDocType.List()]
		[PXUIField(DisplayName = "Return Invoice Type", Visible = false)]
		public virtual String ReturnInvoiceType
		{
			get;
			set;
		}
		#endregion
		#region ReturnInvoiceNbr
		public abstract class returnInvoiceNbr : BqlString.Field<returnInvoiceNbr> { }
		/// <exclude/>
		[PXString(15, IsUnicode = true)]
		[PXSelector(typeof(Search<SOInvoice.refNbr, Where<SOInvoice.docType, Equal<Current<returnInvoiceType>>>>))]
		[PXUIField(DisplayName = "Return Invoice Nbr.")]
		public virtual String ReturnInvoiceNbr
		{
			get;
			set;
		}
		#endregion

		#region ShipmentType
		public abstract class shipmentType : BqlString.Field<shipmentType> { }
		/// <exclude/>
		[PXDBString(1, IsFixed = true)]
		[SOShipmentType.List]
		[PXUIField(DisplayName = "Shipment Type")]
		public virtual String ShipmentType
		{
			get;
			set;
		}
		#endregion
		#region ShipmentNbr
		public abstract class shipmentNbr : BqlString.Field<shipmentNbr> { }
		/// <exclude/>
		[PXDBString(15, IsUnicode = true)]
		[PXSelector(typeof(Search<Navigate.SOOrderShipment.shipmentNbr,
			Where<Navigate.SOOrderShipment.orderType, Equal<Current<returnOrderType>>,
			And<Navigate.SOOrderShipment.orderNbr, Equal<Current<returnOrderNbr>>,
			And<Navigate.SOOrderShipment.shipmentType, Equal<Current<shipmentType>>>>>>))]
		[PXUIField(DisplayName = "Shipment Nbr.")]
		public virtual String ShipmentNbr
		{
			get;
			set;
		}
		#endregion

		#region ARDocType
		public abstract class aRDocType : BqlString.Field<aRDocType> { }
		/// <exclude/>
		[PXString(3, IsFixed = true)]
		[AR.ARDocType.List()]
		[PXUIField(DisplayName = "AR Doc. Type")]
		public virtual String ARDocType
		{
			get;
			set;
		}
		#endregion
		#region ARRefNbr
		public abstract class aRRefNbr : BqlString.Field<aRRefNbr> { }
		/// <exclude/>
		[PXString(15, IsUnicode = true)]
		[PXSelector(typeof(Search<SOInvoice.refNbr, Where<SOInvoice.docType, Equal<Current<aRDocType>>>>))]
		[PXUIField(DisplayName = "AR Ref. Nbr.")]
		public virtual String ARRefNbr
		{
			get;
			set;
		}
		#endregion

		#region APDocType
		public new abstract class aPDocType : BqlString.Field<aPDocType> { }
		/// <exclude/>
		[PXString(3, IsFixed = true)]
		[APInvoiceType.List]
		[PXUIField(DisplayName = "AP Doc. Type")]
		public virtual string APDocType
		{
			get;
			set;
		}
		#endregion
		#region APRefNbr
		public new abstract class aPRefNbr : BqlString.Field<aPRefNbr> { }
		/// <exclude/>
		[PXDBString(15, IsUnicode = true)]
		[PXSelector(typeof(Search<APInvoice.refNbr, Where<APInvoice.docType, Equal<Current<aPDocType>>>>))]
		[PXUIField(DisplayName = "AP Ref. Nbr.")]
		public virtual String APRefNbr
		{
			get;
			set;
		}
		#endregion
	}
}

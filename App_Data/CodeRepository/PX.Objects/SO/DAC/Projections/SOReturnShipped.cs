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
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.SO.DAC.Unbound;

namespace PX.Objects.SO.DAC.Projections
{
	[PXHidden]
	[PXProjection(typeof(Select5<SOLine,
		InnerJoin<SOOrderType, On<SOLine.FK.OrderType>,
		LeftJoin<SOShipLine, On<SOShipLine.FK.OrderLine>,
		LeftJoin<ARTran, On<ARTran.FK.SOOrderLine>,
		LeftJoin<ARRegister, On<ARTran.FK.Document>,
		LeftJoin<POReceiptLine, On<POReceiptLine.FK.SOLine>,
		LeftJoin<APTran, On<APTran.FK.POReceiptLine>,
		LeftJoin<SOOrderShipment,
			On<SOOrderShipment.orderType, Equal<SOLine.orderType>, And<SOOrderShipment.orderNbr, Equal<SOLine.orderNbr>,
			And<Where<SOShipLine.shipmentNbr, IsNull, And2<Where<SOOrderType.requireShipping, Equal<False>, Or<SOLine.lineType, Equal<SOLineType.miscCharge>>>,
					And<SOOrderShipment.shipmentType, Equal<ARTran.sOShipmentType>, And<SOOrderShipment.shipmentNbr, Equal<ARTran.sOShipmentNbr>,
				Or<SOOrderShipment.shipmentType, Equal<SOShipLine.shipmentType>, And<SOOrderShipment.shipmentNbr, Equal<SOShipLine.shipmentNbr>,
				Or<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>, And<SOOrderShipment.shipmentNbr, Equal<POReceiptLine.receiptNbr>>>>>>>>>>>>>>>>>>>,
		Where<SOLine.origOrderType, Equal<CurrentValue<SOOrderRelatedReturnsSPFilter.orderType>>,
			And<SOLine.origOrderNbr, Equal<CurrentValue<SOOrderRelatedReturnsSPFilter.orderNbr>>>>,
		Aggregate<GroupBy<SOLine.orderType, GroupBy<SOLine.orderNbr,
			GroupBy<SOOrderShipment.shippingRefNoteID,
			GroupBy<ARRegister.docType, GroupBy<ARRegister.refNbr,
			GroupBy<APTran.tranType, GroupBy<APTran.refNbr>>>>>>>>>),
		Persistent = false)]
	public class SOReturnShipped : PXBqlTable, IBqlTable
	{
		#region OrderType
		public abstract class orderType : Data.BQL.BqlString.Field<orderType> { }
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(SOLine.orderType))]
		public virtual String OrderType
		{
			get;
			set;
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : Data.BQL.BqlString.Field<orderNbr> { }
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(SOLine.orderNbr))]
		public virtual String OrderNbr
		{
			get;
			set;
		}
		#endregion
		#region ShippingRefNoteID
		public abstract class shippingRefNoteID : Data.BQL.BqlGuid.Field<shippingRefNoteID> { }
		[PXDBGuid(IsKey = true, BqlField = typeof(SOOrderShipment.shippingRefNoteID))]
		public virtual Guid? ShippingRefNoteID
		{
			get;
			set;
		}
		#endregion

		#region ARDocType
		public abstract class aRDocType : Data.BQL.BqlString.Field<aRDocType> { }
		[PXDBString(3, IsFixed = true, BqlField = typeof(ARRegister.docType))]
		public virtual String ARDocType
		{
			get;
			set;
		}
		#endregion
		#region ARRefNbr
		public abstract class aRRefNbr : Data.BQL.BqlString.Field<aRRefNbr> { }
		[PXDBString(15, IsUnicode = true, BqlField = typeof(ARRegister.refNbr))]
		public virtual String ARRefNbr
		{
			get;
			set;
		}
		#endregion
		#region ARNoteID
		public abstract class aRNoteID : Data.BQL.BqlGuid.Field<aRNoteID> { }
		[PXDBGuid(BqlField = typeof(ARRegister.noteID))]
		public virtual Guid? ARNoteID
		{
			get;
			set;
		}
		#endregion

		#region OrigOrderType
		public abstract class origOrderType : Data.BQL.BqlString.Field<origOrderType> { }
		[PXDBString(2, IsFixed = true, BqlField = typeof(SOLine.origOrderType))]
		public virtual String OrigOrderType
		{
			get;
			set;
		}
		#endregion
		#region OrigOrderNbr
		public abstract class origOrderNbr : Data.BQL.BqlString.Field<origOrderNbr> { }
		[PXDBString(15, IsUnicode = true, BqlField = typeof(SOLine.origOrderNbr))]
		public virtual String OrigOrderNbr
		{
			get;
			set;
		}
		#endregion
		#region ShipmentType
		public abstract class shipmentType : Data.BQL.BqlString.Field<shipmentType> { }
		[PXDBString(1, IsFixed = true, BqlField = typeof(SOOrderShipment.shipmentType))]
		public virtual String ShipmentType
		{
			get;
			set;
		}
		#endregion
		#region ShipmentNbr
		public abstract class shipmentNbr : Data.BQL.BqlString.Field<shipmentNbr> { }
		[PXDBString(15, IsUnicode = true, BqlField = typeof(SOOrderShipment.shipmentNbr))]
		public virtual String ShipmentNbr
		{
			get;
			set;
		}
		#endregion
		#region InvoiceType
		public abstract class invoiceType : Data.BQL.BqlString.Field<invoiceType> { }
		[PXDBString(3, IsFixed = true, BqlField = typeof(SOOrderShipment.invoiceType))]
		public virtual String InvoiceType
		{
			get;
			set;
		}
		#endregion
		#region InvoiceNbr
		public abstract class invoiceNbr : Data.BQL.BqlString.Field<invoiceNbr> { }
		[PXDBString(15, IsUnicode = true, BqlField = typeof(SOOrderShipment.invoiceNbr))]
		public virtual String InvoiceNbr
		{
			get;
			set;
		}
		#endregion
		#region InvoiceReleased
		public abstract class invoiceReleased : Data.BQL.BqlBool.Field<invoiceReleased> { }
		[PXDBBool(BqlField = typeof(SOOrderShipment.invoiceReleased))]
		public virtual Boolean? InvoiceReleased
		{
			get;
			set;
		}
		#endregion
		#region InvtDocType
		public abstract class invtDocType : Data.BQL.BqlString.Field<invtDocType> { }
		[PXDBString(1, IsFixed = true, BqlField = typeof(SOOrderShipment.invtDocType))]
		public virtual String InvtDocType
		{
			get;
			set;
		}
		#endregion
		#region InvtRefNbr
		public abstract class invtRefNbr : Data.BQL.BqlString.Field<invtRefNbr> { }
		[PXDBString(15, IsUnicode = true, BqlField = typeof(SOOrderShipment.invtRefNbr))]
		public virtual String InvtRefNbr
		{
			get;
			set;
		}
		#endregion
		#region APDocType
		public abstract class aPDocType : Data.BQL.BqlString.Field<aPDocType> { }
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(APTran.tranType))]
		public virtual String APDocType
		{
			get;
			set;
		}
		#endregion
		#region APRefNbr
		public abstract class aPRefNbr : Data.BQL.BqlString.Field<aPRefNbr> { }
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(APTran.refNbr))]
		public virtual String APRefNbr
		{
			get;
			set;
		}
		#endregion
	}
}

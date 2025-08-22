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

using PX.Data;
using PX.Data.BQL;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO.Attributes;
using System;

namespace PX.Objects.SO.DAC.Projections
{
	/// <exclude/>
	[PXCacheName(Messages.InvoiceSplit)]
	[InvoiceSplitProjection(true)]
	public class InvoiceSplit : PXBqlTable, IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
		[PXBool()]
		[PXUnboundDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get;
			set;
		}
		#endregion

		#region ARDocType
		public abstract class aRDocType : PX.Data.BQL.BqlString.Field<aRDocType> { }
		[PXDBString(3, IsFixed = true, IsKey = true, BqlField = typeof(ARTran.tranType))]
		[PXUIField(DisplayName = "AR Doc. Type", Visible = false)]
		public virtual String ARDocType
		{
			get;
			set;
		}
		#endregion
		#region ARRefNbr
		public abstract class aRRefNbr : PX.Data.BQL.BqlString.Field<aRRefNbr> { }
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(ARTran.refNbr))]
		[PXUIField(DisplayName = "AR Doc. Nbr.")]
		[PXSelector(typeof(Search<ARInvoice.refNbr, Where<ARInvoice.docType.IsEqual<aRDocType.FromCurrent.NoDefault>>>))]
		public virtual String ARRefNbr
		{
			get;
			set;
		}
		#endregion
		#region ARLineNbr
		public abstract class aRLineNbr : PX.Data.BQL.BqlInt.Field<aRLineNbr> { }
		[PXDBInt(IsKey = true, BqlField = typeof(ARTran.lineNbr))]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual Int32? ARLineNbr
		{
			get;
			set;
		}
		#endregion
		#region ARLineType
		public abstract class aRlineType : PX.Data.BQL.BqlString.Field<aRlineType> { }
		[PXDBString(2, IsFixed = true, BqlField = typeof(ARTran.lineType))]
		public virtual String ARLineType
		{
			get;
			set;
		}
		#endregion
		#region ARTranDate
		public abstract class aRTranDate : PX.Data.BQL.BqlDateTime.Field<aRTranDate> { }
		[PXDBDate(BqlField = typeof(ARTran.tranDate))]
		[PXUIField(DisplayName = "AR Doc. Date")]
		public virtual DateTime? ARTranDate
		{
			get;
			set;
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
		[Customer(BqlField = typeof(ARTran.customerID))]
		public virtual int? CustomerID
		{
			get;
			set;
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
		[Inventory(DisplayName = "Inventory ID", BqlField = typeof(ARTran.inventoryID))]
		public virtual Int32? InventoryID
		{
			get;
			set;
		}
		#endregion
		#region SubItemID
		public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
		[IN.SubItem(typeof(inventoryID), BqlField = typeof(ARTran.subItemID))]
		public virtual Int32? SubItemID
		{
			get;
			set;
		}
		#endregion
		#region DropShip
		public abstract class dropShip : PX.Data.BQL.BqlBool.Field<dropShip> { }

		[PXBool]
		[PXDBCalced(typeof(IIf<Where<ARTran.sOShipmentType, Equal<INDocType.dropShip>>, True, False>), typeof(bool))]
		[PXUIField(DisplayName = Messages.DropShip)]
		public virtual bool? DropShip
		{
			get;
			set;
		}
		#endregion

		#region SOOrderType
		public abstract class sOOrderType : PX.Data.BQL.BqlString.Field<sOOrderType> { }
		[PXDBString(2, IsFixed = true, BqlField = typeof(SOLine.orderType))]
		[PXUIField(DisplayName = "Order Type", Visible = false)]
		[PXSelector(typeof(Search<SOOrderType.orderType>), CacheGlobal = true)]
		public virtual String SOOrderType
		{
			get;
			set;
		}
		#endregion
		#region SOOrderNbr
		public abstract class sOOrderNbr : PX.Data.BQL.BqlString.Field<sOOrderNbr> { }
		[PXDBString(15, IsUnicode = true, InputMask = "", BqlField = typeof(SOLine.orderNbr))]
		[PXUIField(DisplayName = "Order Nbr.")]
		[PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType.IsEqual<sOOrderType.FromCurrent.NoDefault>>>))]
		public virtual String SOOrderNbr
		{
			get;
			set;
		}
		#endregion
		#region SOLineNbr
		public abstract class sOLineNbr : PX.Data.BQL.BqlInt.Field<sOLineNbr> { }
		[PXDBInt(BqlField = typeof(SOLine.lineNbr))]
		[PXUIField(DisplayName = "Line Nbr.", Visible = false)]
		public virtual Int32? SOLineNbr
		{
			get;
			set;
		}
		#endregion
		#region SOLineType
		public abstract class sOlineType : PX.Data.BQL.BqlString.Field<sOlineType> { }
		[PXDBString(2, IsFixed = true, BqlField = typeof(SOLine.lineType))]
		public virtual String SOLineType
		{
			get;
			set;
		}
		#endregion
		#region SOOrderDate
		public abstract class sOOrderDate : PX.Data.BQL.BqlDateTime.Field<sOOrderDate> { }
		[PXDBDate(BqlField = typeof(SOLine.orderDate))]
		[PXUIField(DisplayName = "Order Date")]
		public virtual DateTime? SOOrderDate
		{
			get;
			set;
		}
		#endregion
		#region SalesPersonID
		public abstract class salesPersonID : PX.Data.BQL.BqlInt.Field<salesPersonID> { }
		[PXDBInt(BqlField = typeof(SOLine.salesPersonID))]
		public virtual Int32? SalesPersonID
		{
			get;
			set;
		}
		#endregion

		#region INDocType
		public abstract class iNDocType : PX.Data.BQL.BqlString.Field<iNDocType>
		{
			public const string EmptyDoc = "~";
			public class emptyDoc : BqlString.Constant<emptyDoc>
			{
				public emptyDoc() : base(EmptyDoc) { }
			}
		}
		[PXDBCalced(typeof(IsNull<INTran.docType, iNDocType.emptyDoc>), typeof(string))]
		[PXString(1, IsKey = true, IsFixed = true)]
		public virtual String INDocType
		{
			get;
			set;
		}
		#endregion
		#region INRefNbr
		public abstract class iNRefNbr : PX.Data.BQL.BqlString.Field<iNRefNbr> { }
		[PXDBCalced(typeof(IsNull<INTran.refNbr, iNDocType.emptyDoc>), typeof(string))]
		[PXString(15, IsKey = true, IsUnicode = true)]
		public virtual String INRefNbr
		{
			get;
			set;
		}
		#endregion
		#region INLineNbr
		public abstract class iNLineNbr : PX.Data.BQL.BqlInt.Field<iNLineNbr> { }
		[PXDBCalced(typeof(IsNull<INTran.lineNbr, int0>), typeof(int))]
		[PXInt(IsKey = true)]
		public virtual Int32? INLineNbr
		{
			get;
			set;
		}
		#endregion
		#region INSplitLineNbr
		public abstract class iNSplitLineNbr : PX.Data.BQL.BqlInt.Field<iNSplitLineNbr> { }
		[PXDBCalced(typeof(IsNull<INTranSplit.splitLineNbr, int0>), typeof(int))]
		[PXInt(IsKey = true)]
		public virtual Int32? INSplitLineNbr
		{
			get;
			set;
		}
		#endregion

		#region TranDesc
		public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }
		[PXDBString(256, IsUnicode = true, BqlField = typeof(SOLine.tranDesc))]
		[PXUIField(DisplayName = "Line Description")]
		public virtual string TranDesc
		{
			get;
			set;
		}
		#endregion
		#region ComponentID
		public abstract class componentID : PX.Data.BQL.BqlInt.Field<componentID> { }
		// Acuminator disable once PX1095 NoUnboundTypeAttributeWithPXDBCalced Justification false alert, see jira/ATR-780
		[Inventory(DisplayName = "Component ID", IsDBField = false)]
		[PXDBCalced(typeof(Switch<Case<Where<ARTran.inventoryID, NotEqual<INTran.inventoryID>>, INTran.inventoryID>>), typeof(int))]
		public virtual Int32? ComponentID
		{
			get;
			set;
		}
		#endregion
		#region ComponentDesc
		public abstract class componentDesc : PX.Data.BQL.BqlString.Field<componentDesc> { }
		[PXDBCalced(typeof(Switch<Case<Where<ARTran.inventoryID, NotEqual<INTran.inventoryID>>, INTran.tranDesc>>), typeof(string))]
		[PXString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Component Description")]
		public virtual string ComponentDesc
		{
			get;
			set;
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
		// Acuminator disable once PX1095 NoUnboundTypeAttributeWithPXDBCalced Justification false alert, see jira/ATR-780
		[IN.Site(IsDBField = false, Visible = false)]
		[PXDBCalced(typeof(IsNull<INTranSplit.siteID, IsNull<INTran.siteID, SOLine.siteID>>), typeof(int))]
		public virtual Int32? SiteID
		{
			get;
			set;
		}
		#endregion

		#region LocationID
		public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }
		// Acuminator disable once PX1095 NoUnboundTypeAttributeWithPXDBCalced Justification false alert, see jira/ATR-780
		[IN.Location(typeof(siteID), IsDBField = false, Visible = false)]
		[PXDBCalced(typeof(IsNull<INTranSplit.locationID, IsNull<INTran.locationID, SOLine.locationID>>), typeof(int))]
		public virtual Int32? LocationID
		{
			get;
			set;
		}
		#endregion

		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.BQL.BqlString.Field<lotSerialNbr> { }
		[LotSerialNbr(BqlField = typeof(INTranSplit.lotSerialNbr))]
		public virtual String LotSerialNbr
		{
			get;
			set;
		}
		#endregion

		#region UOM
		public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
		// Acuminator disable once PX1095 NoUnboundTypeAttributeWithPXDBCalced Justification false alert, see jira/ATR-780
		[INUnit(DisplayName = "UOM", Enabled = false, IsDBField = false)]
		[PXDBCalced(typeof(IsNull<INTranSplit.uOM, IsNull<INTran.uOM, ARTran.uOM>>), typeof(string))]
		public virtual String UOM
		{
			get;
			set;
		}
		#endregion

		#region Qty
		public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
		[PXQuantity()]
		[PXUIField(DisplayName = "Original Qty.")]
		[PXDBCalced(typeof(IsNull<INTranSplit.qty, IsNull<INTran.qty,
			IIf<Where<ARTran.tranType.IsIn<ARDocType.creditMemo, ARDocType.cashReturn>>, Data.Minus<ARTran.qty>, ARTran.qty>>>
			), typeof(decimal))]
		public virtual Decimal? Qty
		{
			get;
			set;
		}
		#endregion
		#region BaseQty
		public abstract class baseQty : PX.Data.BQL.BqlDecimal.Field<baseQty> { }
		[PXQuantity]
		[PXDBCalced(typeof(IsNull<INTranSplit.baseQty, IsNull<INTran.baseQty, ARTran.baseQty>>), typeof(decimal))]
		public virtual Decimal? BaseQty
		{
			get;
			set;
		}
		#endregion

		#region QtyAvailForReturn
		public abstract class qtyAvailForReturn : PX.Data.BQL.BqlDecimal.Field<qtyAvailForReturn> { }
		[PXQuantity()]
		[PXUIField(DisplayName = "Available for Return")]
		public virtual Decimal? QtyAvailForReturn
		{
			get;
			set;
		}
		#endregion
		#region QtyReturned
		public abstract class qtyReturned : PX.Data.BQL.BqlDecimal.Field<qtyReturned> { }
		[PXQuantity()]
		[PXUIField(DisplayName = "Qty. Returned", Visible = false)]
		public virtual Decimal? QtyReturned
		{
			get;
			set;
		}
		#endregion
		#region QtyToReturn
		public abstract class qtyToReturn : PX.Data.BQL.BqlDecimal.Field<qtyToReturn> { }
		[PXQuantity(MinValue = 0)]
		[PXUIField(DisplayName = "Qty. to Return")]
		[PXUnboundDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? QtyToReturn
		{
			get;
			set;
		}
		#endregion
		#region SerialIsOnHand
		public abstract class serialIsOnHand : BqlBool.Field<serialIsOnHand> { }
		[PXBool]
		public virtual bool? SerialIsOnHand
		{
			get;
			set;
		}
		#endregion
		#region SerialIsAlreadyReceived
		public abstract class serialIsAlreadyReceived : BqlBool.Field<serialIsAlreadyReceived> { }
		[PXBool]
		public virtual bool? SerialIsAlreadyReceived
		{
			get;
			set;
		}
		#endregion
		#region SerialIsAlreadyReceivedRef
		public abstract class serialIsAlreadyReceivedRef : BqlString.Field<serialIsAlreadyReceivedRef> { }
		[PXString]
		public virtual string SerialIsAlreadyReceivedRef
		{
			get;
			set;
		}
		#endregion
		#region ARTranQty
		public abstract class aRTranQty : PX.Data.BQL.BqlDecimal.Field<aRTranQty> { }
		[PXDBQuantity(BqlField = typeof(ARTran.qty))]
		public virtual Decimal? ARTranQty
		{
			get;
			set;
		}
		#endregion
		#region ARTranUOM
		public abstract class aRTranUOM : PX.Data.BQL.BqlString.Field<aRTranUOM> { }
		[INUnit(BqlField = typeof(ARTran.uOM))]
		public virtual String ARTranUOM
		{
			get;
			set;
		}
		#endregion
		#region ARTranDrCr
		public abstract class aRTranDrCr : PX.Data.BQL.BqlString.Field<aRTranDrCr> { }
		[PXDBString(1, IsFixed = true, BqlField = typeof(ARTran.drCr))]
		public virtual String ARTranDrCr
		{
			get;
			set;
		}
		#endregion
		#region INTranQty
		public abstract class iNTranQty : PX.Data.BQL.BqlDecimal.Field<iNTranQty> { }
		[PXDBQuantity(BqlField = typeof(INTran.qty))]
		public virtual Decimal? INTranQty
		{
			get;
			set;
		}
		#endregion
		#region INTranUOM
		public abstract class iNTranUOM : PX.Data.BQL.BqlString.Field<iNTranUOM> { }
		[INUnit(BqlField = typeof(INTran.uOM))]
		public virtual String INTranUOM
		{
			get;
			set;
		}
		#endregion
		#region IsKit
		public abstract class isKit : PX.Data.BQL.BqlBool.Field<isKit> { }
		[PXBool]
		[PXDBCalced(typeof(IIf<Where<ARTran.inventoryID, Equal<INTran.inventoryID>>, False, True>), typeof(bool))]
		public virtual bool? IsKit
		{
			get;
			set;
		}
		#endregion

	}
}

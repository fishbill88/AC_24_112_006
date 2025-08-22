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
using PX.Objects.Common.DAC;
using PX.Objects.IN;
using System;

namespace PX.Objects.PO
{
	[PXCacheName(Messages.DropShipPOLine)]
	[PXProjection(typeof(Select2<POLine,
		LeftJoin<DropShipLink, On<DropShipLink.FK.POLine>>,
		Where<POLine.orderType, Equal<POOrderType.dropShip>,
			And<POLine.lineType, In3<POLineType.goodsForDropShip, POLineType.nonStockForDropShip>>>>))]
	public class DropShipPOLine : PXBqlTable, IBqlTable
	{
		#region OrderType
		public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }

		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POLine.orderType))]
		[PXUIField(DisplayName = "Order Type")]
		public virtual String OrderType
		{
			get;
			set;
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }

		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POLine.orderNbr))]
		[PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.orderType, Equal<Current<DropShipPOLine.orderType>>>>))]
		[PXUIField(DisplayName = "Order Nbr.")]
		public virtual String OrderNbr
		{
			get;
			set;
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

		[PXDBInt(IsKey = true, BqlField = typeof(POLine.lineNbr))]
		[PXUIField(DisplayName = "Line Nbr.")]
		public virtual Int32? LineNbr
		{
			get;
			set;
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> {
			public class PreventEditIfPOReceiptLineExist : PreventEditOf<InventoryItem.lotSerClassID>.On<InventoryItemMaint>
						.IfExists<Select<POReceiptLine,
							Where<POReceiptLine.inventoryID, Equal<Current<InventoryItem.inventoryID>>,
								And<POReceiptLine.pOType, Equal<POOrderType.dropShip>,
									And<POReceiptLine.iNReleased, Equal<False>>>>>>
			{
				protected override void OnPreventEdit(GetEditPreventingReasonArgs args)
				{
					InventoryItem inventoryItm = (InventoryItem)args.Row;
					if (inventoryItm.OrigLotSerClassID == null || INLotSerClass.PK.Find(Base, inventoryItm.LotSerClassID).RequiredForDropship == false)
						args.Cancel = true;						
				}
				public static bool IsActive() => true;
				protected override string CreateEditPreventingReason(GetEditPreventingReasonArgs arg, object firstPreventingEntity, string fieldName, string currentTableName, string foreignTableName)
				{
					var pRLine = (POReceiptLine)firstPreventingEntity;
					DropShipLink link = PXSelect<DropShipLink,
										Where<DropShipLink.pOOrderType, Equal<Required<POLine.orderType>>,
											And<DropShipLink.pOOrderNbr, Equal<Required<POLine.orderNbr>>,
											And<DropShipLink.pOLineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(Base, pRLine.POType, pRLine.PONbr, pRLine.POLineNbr);
					var invenotryItem = InventoryItem.PK.Find(Base, pRLine.InventoryID);
					if (pRLine.ReceiptType == POReceiptType.POReturn)
					{
						return PXMessages.LocalizeFormat(IN.Messages.ItemLotSerClassCannotChangeDocumentNotFullyProcessed, invenotryItem.InventoryCD, pRLine.SOOrderNbr, SO.Messages.SOOrder);
					}
					else
						return PXMessages.LocalizeFormat(IN.Messages.ItemLotSerClassCannotChangeDocumentNotFullyProcessed, invenotryItem.InventoryCD, link?.SOOrderNbr, SO.Messages.SOOrder);
				}
			}
		}

		[POLineInventoryItem(Filterable = true, BqlField = typeof(POLine.inventoryID))]
		public virtual Int32? InventoryID
		{
			get;
			set;
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }

		[PXDBString(256, IsUnicode = true, BqlField = typeof(POLine.tranDesc))]
		[PXUIField(DisplayName = "Description")]
		public virtual String TranDesc
		{
			get;
			set;
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }

		[INUnit(typeof(DropShipPOLine.inventoryID), DisplayName = "UOM", BqlField = typeof(POLine.uOM))]
		public virtual String UOM
		{
			get;
			set;
		}
		#endregion
		#region OrderQty
		public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }

		[PXDBQuantity(BqlField = typeof(POLine.orderQty))]
		[PXUIField(DisplayName = "Not Linked Qty.")]
		public virtual Decimal? OrderQty
		{
			get;
			set;
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }

		[PXNote(BqlField = typeof(POLine.noteID))]
		public virtual Guid? NoteID
		{
			get;
			set;
		}
		#endregion
		#region SOOrderType
		public abstract class sOOrderType : PX.Data.BQL.BqlString.Field<sOOrderType> { }

		[PXDBString(2, IsFixed = true, BqlField = typeof(DropShipLink.sOOrderType))]
		public virtual string SOOrderType
		{
			get;
			set;
		}
		#endregion
		#region SOOrderNbr
		public abstract class sOOrderNbr : PX.Data.BQL.BqlString.Field<sOOrderNbr> { }

		[PXUIField(DisplayName = "Sales Order Nbr.")]
		[PXSelector(typeof(Search<SO.SOOrder.orderNbr, Where<SO.SOOrder.orderType, Equal<Current<DropShipPOLine.sOOrderType>>>>))]
		[PXDBString(15, IsUnicode = true, BqlField = typeof(DropShipLink.sOOrderNbr))]
		public virtual string SOOrderNbr
		{
			get;
			set;
		}
		#endregion
		#region SOLineNbr
		public abstract class sOLineNbr : PX.Data.BQL.BqlInt.Field<sOLineNbr> { }

		[PXUIField(DisplayName = "Sales Order Line Nbr.")]
		[PXDBInt(BqlField = typeof(DropShipLink.sOLineNbr))]
		public virtual int? SOLineNbr
		{
			get;
			set;
		}
		#endregion
		#region SOLinkActive
		public abstract class sOLinkActive : PX.Data.BQL.BqlBool.Field<sOLinkActive> { }

		[PXDBBool(BqlField = typeof(DropShipLink.active))]
		[PXUIField(DisplayName = "SO Linked")]
		public virtual bool? SOLinkActive
		{
			get;
			set;
		}
		#endregion
	}
}

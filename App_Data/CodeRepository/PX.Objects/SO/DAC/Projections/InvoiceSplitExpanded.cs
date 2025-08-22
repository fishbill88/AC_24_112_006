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
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.SO.Attributes;
using System;

namespace PX.Objects.SO.DAC.Projections
{
	/// <exclude/>
	[PXHidden]
	[InvoiceSplitProjection(false)]
	[PXBreakInheritance]
	public class InvoiceSplitExpanded : InvoiceSplit
	{
		#region ARDocType
		public abstract new class aRDocType : PX.Data.BQL.BqlString.Field<aRDocType> { }
		#endregion
		#region ARRefNbr
		public abstract new class aRRefNbr : PX.Data.BQL.BqlString.Field<aRRefNbr> { }
		#endregion
		#region ARLineNbr
		public abstract new class aRLineNbr : PX.Data.BQL.BqlInt.Field<aRLineNbr> { }
		#endregion
		#region ARLineType
		public abstract new class aRlineType : PX.Data.BQL.BqlString.Field<aRlineType> { }
		#endregion
		#region SOOrderNbr
		public abstract new class sOOrderNbr : PX.Data.BQL.BqlString.Field<sOOrderNbr> { }
		#endregion
		#region SOLineType
		public abstract new class sOlineType : PX.Data.BQL.BqlString.Field<sOlineType> { }
		#endregion
		#region DropShip
		public new abstract class dropShip : PX.Data.BQL.BqlBool.Field<dropShip> { }
		[PXBool]
		[PXDBCalced(typeof(IIf<Where<ARTran.sOShipmentType, Equal<INDocType.dropShip>>, True, False>), typeof(bool), BqlTable = typeof(InvoiceSplitExpanded))]
		[PXUIField(DisplayName = Messages.DropShip)]
		public override bool? DropShip
		{
			get => base.DropShip;
			set => base.DropShip = value;
		}
		#endregion
		#region INDocType
		public new abstract class iNDocType : PX.Data.BQL.BqlString.Field<iNDocType> { }
		[PXDBString(1, IsKey = true, IsFixed = true, BqlField = typeof(INTran.docType))]
		public override String INDocType
		{
			get => base.INDocType;
			set => base.INDocType = value;
		}
		#endregion
		#region INRefNbr
		public new abstract class iNRefNbr : PX.Data.BQL.BqlString.Field<iNRefNbr> { }
		[PXDBString(15, IsKey = true, IsUnicode = true, BqlField = typeof(INTran.refNbr))]
		public override String INRefNbr
		{
			get => base.INRefNbr;
			set => base.INRefNbr = value;
		}
		#endregion
		#region INLineNbr
		public new abstract class iNLineNbr : PX.Data.BQL.BqlInt.Field<iNLineNbr> { }
		[PXDBInt(IsKey = true, BqlField = typeof(INTran.lineNbr))]
		public override Int32? INLineNbr
		{
			get => base.INLineNbr;
			set => base.INLineNbr = value;
		}
		#endregion
		#region INSplitLineNbr
		public new abstract class iNSplitLineNbr : PX.Data.BQL.BqlInt.Field<iNSplitLineNbr> { }
		[PXDBInt(IsKey = true, BqlField = typeof(INTranSplit.splitLineNbr))]
		public override Int32? INSplitLineNbr
		{
			get => base.INSplitLineNbr;
			set => base.INSplitLineNbr = value;
		}
		#endregion
		#region ComponentID
		public new abstract class componentID : PX.Data.BQL.BqlInt.Field<componentID> { }
		// Acuminator disable once PX1095 NoUnboundTypeAttributeWithPXDBCalced Justification false alert, see jira/ATR-780
		[Inventory(DisplayName = "Component ID", IsDBField = false)]
		[PXDBCalced(typeof(Switch<Case<Where<ARTran.inventoryID, NotEqual<INTran.inventoryID>>, INTran.inventoryID>>), typeof(int), BqlTable = typeof(InvoiceSplitExpanded))]
		public override Int32? ComponentID
		{
			get => base.ComponentID;
			set => base.ComponentID = value;
		}
		#endregion
		#region ComponentDesc
		public new abstract class componentDesc : PX.Data.BQL.BqlString.Field<componentDesc> { }
		[PXDBCalced(typeof(Switch<Case<Where<ARTran.inventoryID, NotEqual<INTran.inventoryID>>, INTran.tranDesc>>), typeof(string), BqlTable = typeof(InvoiceSplitExpanded))]
		[PXString(256, IsUnicode = true)]
		public override string ComponentDesc
		{
			get => base.ComponentDesc;
			set => base.ComponentDesc = value;
		}
		#endregion
		#region SiteID
		public new abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
		// Acuminator disable once PX1095 NoUnboundTypeAttributeWithPXDBCalced Justification false alert, see jira/ATR-780
		[IN.Site(IsDBField = false)]
		[PXDBCalced(typeof(IsNull<INTranSplit.siteID, IsNull<INTran.siteID, SOLine.siteID>>), typeof(int), BqlTable = typeof(InvoiceSplitExpanded))]
		public override Int32? SiteID
		{
			get => base.SiteID;
			set => base.SiteID = value;
		}
		#endregion

		#region LocationID
		public new abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }
		// Acuminator disable once PX1095 NoUnboundTypeAttributeWithPXDBCalced Justification false alert, see jira/ATR-780
		[IN.Location(typeof(siteID), IsDBField = false)]
		[PXDBCalced(typeof(IsNull<INTranSplit.locationID, IsNull<INTran.locationID, SOLine.locationID>>), typeof(int), BqlTable = typeof(InvoiceSplitExpanded))]
		public override Int32? LocationID
		{
			get => base.LocationID;
			set => base.LocationID = value;
		}
		#endregion
		#region UOM
		public new abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
		// Acuminator disable once PX1095 NoUnboundTypeAttributeWithPXDBCalced Justification false alert, see jira/ATR-780
		[INUnit(DisplayName = "UOM", Enabled = false, IsDBField = false)]
		[PXDBCalced(typeof(IsNull<INTranSplit.uOM, IsNull<INTran.uOM, ARTran.uOM>>), typeof(string), BqlTable = typeof(InvoiceSplitExpanded))]
		public override String UOM
		{
			get => base.UOM;
			set => base.UOM = value;
		}
		#endregion
		#region Qty
		public new abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
		[PXQuantity()]
		[PXUIField(DisplayName = "Quantity")]
		[PXDBCalced(typeof(IsNull<INTranSplit.qty, IsNull<INTran.qty,
			IIf<Where<ARTran.tranType.IsIn<ARDocType.creditMemo, ARDocType.cashReturn>>, Data.Minus<ARTran.qty>, ARTran.qty>>>
			), typeof(decimal), BqlTable = typeof(InvoiceSplitExpanded))]
		public override Decimal? Qty
		{
			get => base.Qty;
			set => base.Qty = value;
		}
		#endregion
		#region BaseQty
		public new abstract class baseQty : PX.Data.BQL.BqlDecimal.Field<baseQty> { }
		[PXQuantity]
		[PXDBCalced(typeof(IsNull<INTranSplit.baseQty, IsNull<INTran.baseQty, ARTran.baseQty>>), typeof(decimal), BqlTable = typeof(InvoiceSplitExpanded))]
		public override Decimal? BaseQty
		{
			get => base.BaseQty;
			set => base.BaseQty = value;
		}
		#endregion
		#region IsKit
		public new abstract class isKit : PX.Data.BQL.BqlBool.Field<isKit> { }
		[PXBool]
		[PXDBCalced(typeof(IIf<Where<ARTran.inventoryID, Equal<INTran.inventoryID>>, False, True>), typeof(bool), BqlTable = typeof(InvoiceSplitExpanded))]
		public override bool? IsKit
		{
			get;
			set;
		}
		#endregion
	}
}

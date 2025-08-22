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

using PX.Data.ReferentialIntegrity.Attributes;

namespace PX.Objects.RQ
{
	using System;
	using PX.Data;
	using PX.Data.BQL;
	using PX.Data.BQL.Fluent;
	using PX.Objects.GL;
	using PX.Objects.IN;
	using PX.Objects.CM;
	using PX.Objects.CR;
	using PX.Objects.CS;
	using PX.Objects.PO;
	using PX.Objects.SO;
	using PX.Objects.AR;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.RQRequisitionLine)]
	public partial class RQRequisitionLine : PXBqlTable, PX.Data.IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<RQRequisitionLine>.By<reqNbr, lineNbr>
		{
			public RQRequisitionLine Find(PXGraph graph, string reqNbr, int? lineNbr) => FindBy(graph, reqNbr, lineNbr);
		}
		public static class FK
		{
			public class Requisition : RQRequisition.PK.ForeignKeyOf<RQRequisitionLine>.By<reqNbr> { }
			public class InventoryItem : IN.InventoryItem.PK.ForeignKeyOf<RQRequisitionLine>.By<inventoryID> { }
			public class Site : INSite.PK.ForeignKeyOf<RQRequisitionLine>.By<siteID> { }
		}
		#endregion

		#region Selected
		public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion		
		#region BranchID
		public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
		protected Int32? _BranchID;
		[Branch(typeof(RQRequisition.branchID))]
		public virtual Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region ReqNbr
		public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
		protected String _ReqNbr;

		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDBDefault(typeof(RQRequisition.reqNbr), DefaultForUpdate = false)]
		[PXParent(typeof(FK.Requisition))]
		[PXUIField(DisplayName = "Req. Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual String ReqNbr
		{
			get
			{
				return this._ReqNbr;
			}
			set
			{
				this._ReqNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
		protected int? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXLineNbr(typeof(RQRequisition.lineCntr))]
		public virtual int? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion				
		#region LineSource
		public abstract class lineSource : PX.Data.BQL.BqlString.Field<lineSource> { }
		[PXDefault("D", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXString(1, IsFixed = true)]		
		[PXStringListAttribute(
			new string[] { "D", "R" },
			new string[] { Messages.Draft, Messages.Request })]
		[PXUIField(DisplayName = "Line Source", Enabled = false)]
		public virtual String LineSource
		{
			[PXDependsOnFields(typeof(byRequest))]
			get
			{
				return this.ByRequest == true ? "R" : "D";					
			}			
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID>
		{
			public class InventoryBaseUnitRule :
				InventoryItem.baseUnit.PreventEditIfExists<
					Select2<RQRequisitionLine,
					InnerJoin<RQRequisition, On<RQRequisition.reqNbr, Equal<reqNbr>>>,
					Where<inventoryID, Equal<Current<InventoryItem.inventoryID>>,
						And2<Where2<POLineType.Goods.Contains<lineType>,
							Or<POLineType.NonStocks.Contains<lineType>>>,
						And<RQRequisition.approved, NotEqual<True>,
						And<cancelled, NotEqual<True>,
						And<RQRequisition.cancelled, NotEqual<True>>>>>>>>
			{ }
		}
		protected Int32? _InventoryID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[RQRequisitionInventoryItem(Filterable = true)]		
		[PXForeignReference(typeof(FK.InventoryItem))]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region LineType
		public abstract class lineType : PX.Data.BQL.BqlString.Field<lineType> { }
		protected String _LineType;
		[PXDBString(2, IsFixed = true)]
		[PXDefault()]
		[POLineTypeList(
			typeof(RQRequisitionLine.inventoryID),
			new string[] { POLineType.GoodsForInventory, POLineType.NonStock, POLineType.Service },
			new string[] { PX.Objects.PO.Messages.GoodsForInventory, PX.Objects.PO.Messages.NonStockItem, PX.Objects.PO.Messages.Service })]
		[PXUIField(DisplayName = "Line Type", Enabled = false, Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual String LineType
		{
			get
			{
				return this._LineType;
			}
			set
			{
				this._LineType = value;
			}
		}
		#endregion
		#region SubItemID
		public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
		protected Int32? _SubItemID;
		[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
			Where<InventoryItem.inventoryID, Equal<Current<RQRequisitionLine.inventoryID>>,
			And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		[SubItem(typeof(RQRequisitionLine.inventoryID))]
		[SubItemStatusVeryfier(typeof(RQRequisitionLine.inventoryID), typeof(RQRequisitionLine.siteID), InventoryItemStatus.Inactive, InventoryItemStatus.NoPurchases, InventoryItemStatus.NoRequest)]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion		
		#region SiteID
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
		protected Int32? _SiteID;
		[PXFormula(typeof(Default<RQRequisitionLine.inventoryID>))]
		[POSiteAvail(typeof(RQRequisitionLine.inventoryID), typeof(RQRequisitionLine.subItemID), typeof(CostCenter.freeStock))]
		[PXForeignReference(typeof(FK.Site))]
        [PXDefault(
			typeof(
			Coalesce<
			Coalesce<
			Coalesce<
			Coalesce<
			Search<RQRequisition.siteID,
			Where<RQRequisition.reqNbr, Equal<Current<RQRequisitionLine.reqNbr>>,
			And<RQRequisition.shipDestType, Equal<POShippingDestination.site>>>>,
			Search<Location.cSiteID,
			Where<Location.locationID, Equal<Current<RQRequisition.customerLocationID>>,
			  And<Location.bAccountID, Equal<Current<RQRequisition.customerID>>>>>>,
			Search<LocationBranchSettings.vSiteID,
				Where<LocationBranchSettings.locationID, Equal<Current<RQRequisition.vendorLocationID>>,
					And<LocationBranchSettings.bAccountID, Equal<Current<RQRequisition.vendorID>>,
					And<LocationBranchSettings.branchID, Equal<Current<RQRequisition.branchID>>>>>>,
			Search<Location.vSiteID,
			Where<Location.locationID, Equal<Current<RQRequisition.vendorLocationID>>,
				And<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>>>>>,
			Search<InventoryItemCurySettings.dfltSiteID,
			Where<InventoryItemCurySettings.inventoryID, Equal<Current<RQRequisitionLine.inventoryID>>,
				And<InventoryItemCurySettings.curyID, EqualBaseCuryID<Current<RQRequisition.branchID>>>>>>,
			Search<RQRequisitionLine.siteID,
			Where<RQRequisitionLine.reqNbr, Equal<Current<RQRequisitionLine.reqNbr>>,
			And<RQRequisitionLine.lineNbr, Equal<Current<RQRequisitionLine.lineNbr>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion		
		#region Description
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
		protected String _Description;
		[PXDBString(Common.Constants.TranDescLength, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<InventoryItem.descr, Where<InventoryItem.inventoryID, Equal<Current<RQRequisitionLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion		
		
		#region UOM
		protected String _UOM;
		[PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<RQRequisitionLine.inventoryID>>>>))]
		[INUnit(typeof(RQRequisitionLine.inventoryID), DisplayName = "UOM")]
		public virtual String UOM
		{
			get => this._UOM;
			set => this._UOM = value;
			}
		public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
		#endregion
		#region AlternateID
		protected String _AlternateID;
		[AlternativeItem(INPrimaryAlternateType.VPN, typeof(inventoryID), typeof(subItemID), typeof(uOM))]
		public virtual String AlternateID
			{
			get => this._AlternateID;
			set => this._AlternateID = value;
		}
		public abstract class alternateID : PX.Data.BQL.BqlString.Field<alternateID> { }
		#endregion

		#region OrderQty
		public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
		protected Decimal? _OrderQty;
		[PXDBQuantity(typeof(RQRequisitionLine.uOM), typeof(RQRequisitionLine.baseOrderQty), HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Order Qty.", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? OrderQty
		{
			get
			{
				return this._OrderQty;
			}
			set
			{
				this._OrderQty = value;
			}
		}
		#endregion
		#region BaseOrderQty
		public abstract class baseOrderQty : PX.Data.BQL.BqlDecimal.Field<baseOrderQty> { }
		protected Decimal? _BaseOrderQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseOrderQty
		{
			get
			{
				return this._BaseOrderQty;
			}
			set
			{
				this._BaseOrderQty = value;
			}
		}
		#endregion
		#region OriginQty
		[Obsolete("Not used anywhere. Consider removing on refactoring.")]
		public abstract class originQty : PX.Data.BQL.BqlDecimal.Field<originQty> { }
		protected Decimal? _OriginQty;
		[PXDBQuantity(typeof(RQRequisitionLine.uOM), typeof(RQRequisitionLine.baseOriginQty), HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Original Qty.", Enabled = false)]
		[Obsolete("Not used anywhere. Consider removing on refactoring.")]
		public virtual Decimal? OriginQty
		{
			get
			{
				return this._OriginQty;
			}
			set
			{
				this._OriginQty = value;
			}
		}
		#endregion
		#region BaseOriginQty
		[Obsolete("Not used anywhere. Consider removing on refactoring.")]
		public abstract class baseOriginQty : PX.Data.BQL.BqlDecimal.Field<baseOriginQty> { }
		protected Decimal? _BaseOriginQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[Obsolete("Not used anywhere. Consider removing on refactoring.")]
		public virtual Decimal? BaseOriginQty
		{
			get
			{
				return this._BaseOriginQty;
			}
			set
			{
				this._BaseOriginQty = value;
			}
		}
		#endregion	

		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
		protected Int64? _CuryInfoID;

		[PXDBLong()]
		[CurrencyInfo(typeof(RQRequisition.curyInfoID))]
		public virtual Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region CuryEstUnitCost
		public abstract class curyEstUnitCost : PX.Data.BQL.BqlDecimal.Field<curyEstUnitCost> { }
		protected Decimal? _CuryEstUnitCost;

		[PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(RQRequisitionLine.curyInfoID), typeof(RQRequisitionLine.estUnitCost))]
		[PXUIField(DisplayName = "Est. Unit Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryEstUnitCost
		{
			get
			{
				return this._CuryEstUnitCost;
			}
			set
			{
				this._CuryEstUnitCost = value;
			}
		}
		#endregion
		#region EstUnitCost
		public abstract class estUnitCost : PX.Data.BQL.BqlDecimal.Field<estUnitCost> { }
		protected Decimal? _EstUnitCost;

		[PXDBPriceCost]
		[PXDefault(typeof(Search<INItemCost.lastCost,
			Where<INItemCost.inventoryID, Equal<Current<RQRequisitionLine.inventoryID>>,
				And<INItemCost.curyID, EqualBaseCuryID<Current2<RQRequisition.branchID>>>>>))]
		[PXUIField(DisplayName = "Est. Unit Cost")]
		public virtual Decimal? EstUnitCost
		{
			get
			{
				return this._EstUnitCost;
			}
			set
			{
				this._EstUnitCost = value;
			}
		}
		#endregion
		#region CuryEstExtCost
		public abstract class curyEstExtCost : PX.Data.BQL.BqlDecimal.Field<curyEstExtCost> { }
		protected Decimal? _CuryEstExtCost;
		[PXDBCurrency(typeof(RQRequisitionLine.curyInfoID), typeof(RQRequisitionLine.estExtCost))]
		[PXUIField(DisplayName = "Est. Ext. Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(Mult<RQRequisitionLine.curyEstUnitCost, RQRequisitionLine.orderQty>), typeof(SumCalc<RQRequisition.curyEstExtCostTotal>), ValidateAggregateCalculation = true)]		
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryEstExtCost
		{
			get
			{
				return this._CuryEstExtCost;
			}
			set
			{
				this._CuryEstExtCost = value;
			}
		}
		#endregion
		#region EstExtCost
		public abstract class estExtCost : PX.Data.BQL.BqlDecimal.Field<estExtCost> { }
		protected Decimal? _EstExtCost;

		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Est. Ext. Cost")]
		public virtual Decimal? EstExtCost
		{
			get
			{
				return this._EstExtCost;
			}
			set
			{
				this._EstExtCost = value;
			}
		}
		#endregion
		#region ManualPrice
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Manual Cost", Visible = false)]
		public virtual Boolean? ManualPrice { get; set; }
		public abstract class manualPrice : PX.Data.BQL.BqlBool.Field<manualPrice> { }
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
		protected Guid? _NoteID;
		[PXNote]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
		#region ExpenseAcctID
		public abstract class expenseAcctID : PX.Data.BQL.BqlInt.Field<expenseAcctID> { }
		protected Int32? _ExpenseAcctID;
		[Account(DisplayName = "Account", 
			Visibility = PXUIVisibility.Visible, Filterable = false, DescriptionField=typeof(Account.description), AvoidControlAccounts = true)]		
		public virtual Int32? ExpenseAcctID
		{
			get
			{
				return this._ExpenseAcctID;
			}
			set
			{
				this._ExpenseAcctID = value;
			}
		}
		#endregion
		#region ExpenseSubID
		public abstract class expenseSubID : PX.Data.BQL.BqlInt.Field<expenseSubID> { }
		protected Int32? _ExpenseSubID;

		[SubAccount(typeof(RQRequisitionLine.expenseAcctID), DisplayName = "Sub.", Visibility = PXUIVisibility.Visible, Filterable = true, DescriptionField=typeof(Sub.description))]		
		public virtual Int32? ExpenseSubID
		{
			get
			{
				return this._ExpenseSubID;
			}
			set
			{
				this._ExpenseSubID = value;
			}
		}
		#endregion

		#region RcptQtyMin
		public abstract class rcptQtyMin : PX.Data.BQL.BqlDecimal.Field<rcptQtyMin> { }
		protected Decimal? _RcptQtyMin;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0)]
		[PXDefault(TypeCode.Decimal, "0.0",
			typeof(Search<Location.vRcptQtyMin,
			Where<Location.locationID, Equal<Current<RQRequisition.vendorLocationID>>,
				And<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>>>>))]
		[PXUIField(DisplayName = "Min. Receipt, %")]
		public virtual Decimal? RcptQtyMin
		{
			get
			{
				return this._RcptQtyMin;
			}
			set
			{
				this._RcptQtyMin = value;
			}
		}
		#endregion
		#region RcptQtyMax
		public abstract class rcptQtyMax : PX.Data.BQL.BqlDecimal.Field<rcptQtyMax> { }
		protected Decimal? _RcptQtyMax;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0)]
		[PXDefault(TypeCode.Decimal, "100.0", typeof(Search<Location.vRcptQtyMax,
			Where<Location.locationID, Equal<Current<RQRequisition.vendorLocationID>>,
				And<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>>>>))]
		[PXUIField(DisplayName = "Max. Receipt, %")]
		public virtual Decimal? RcptQtyMax
		{
			get
			{
				return this._RcptQtyMax;
			}
			set
			{
				this._RcptQtyMax = value;
			}
		}
		#endregion
		#region RcptQtyThreshold
		public abstract class rcptQtyThreshold : PX.Data.BQL.BqlDecimal.Field<rcptQtyThreshold> { }
		protected Decimal? _RcptQtyThreshold;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0)]
		[PXDefault(TypeCode.Decimal, "100.0",
			typeof(Search<Location.vRcptQtyThreshold,
			Where<Location.locationID, Equal<Current<RQRequisition.vendorLocationID>>,
				And<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>>>>))]
		[PXUIField(DisplayName = "Complete On, %")]
		public virtual Decimal? RcptQtyThreshold
		{
			get
			{
				return this._RcptQtyThreshold;
			}
			set
			{
				this._RcptQtyThreshold = value;
			}
		}
		#endregion
		#region RcptQtyAction
		public abstract class rcptQtyAction : PX.Data.BQL.BqlString.Field<rcptQtyAction> { }
		protected String _RcptQtyAction;
		[PXDBString(1, IsFixed = true)]
		[POReceiptQtyAction.List()]
		[PXDefault(POReceiptQtyAction.AcceptButWarn,
			typeof(Search<Location.vRcptQtyAction,
			Where<Location.locationID, Equal<Current<RQRequisition.vendorLocationID>>,
				And<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>>>>))]
		[PXUIField(DisplayName = "Receipt Action")]
		public virtual String RcptQtyAction
		{
			get
			{
				return this._RcptQtyAction;
			}
			set
			{
				this._RcptQtyAction = value;
			}
		}
		#endregion
		#region IsUseMarkup
		public abstract class isUseMarkup : PX.Data.BQL.BqlBool.Field<isUseMarkup> { }
		protected Boolean? _IsUseMarkup;
		[PXDBBool()]
		[PXUIField(DisplayName = "Use Markup", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? IsUseMarkup
		{
			get
			{
				return this._IsUseMarkup;
			}
			set
			{
				this._IsUseMarkup = value;
			}
		}
		#endregion		
		#region MarkupPct
		public abstract class markupPct : PX.Data.BQL.BqlDecimal.Field<markupPct> { }
		protected Decimal? _MarkupPct;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0",
			typeof(Coalesce<
			Search<INItemSite.markupPct, Where<INItemSite.inventoryID, Equal<Current<RQRequisitionLine.inventoryID>>, 
				And<INItemSite.siteID, Equal<Current<RQRequisitionLine.siteID>>>>>,
			Search<InventoryItem.markupPct, Where<InventoryItem.inventoryID, Equal<Current<RQRequisitionLine.inventoryID>>>>>))]
		[PXUIField(DisplayName = "Markup, %")]
		public virtual Decimal? MarkupPct
		{
			get
			{
				return this._MarkupPct;
			}
			set
			{
				this._MarkupPct = value;
			}
		}
		#endregion
		#region RequestedDate
		public abstract class requestedDate : PX.Data.BQL.BqlDateTime.Field<requestedDate> { }
		protected DateTime? _RequestedDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Required Date")]
		public virtual DateTime? RequestedDate
		{
			get
			{
				return this._RequestedDate;
			}
			set
			{
				this._RequestedDate = value;
			}
		}
			#endregion
		#region PromisedDate
		public abstract class promisedDate : PX.Data.BQL.BqlDateTime.Field<promisedDate> { }
		protected DateTime? _PromisedDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Promised Date")]
		public virtual DateTime? PromisedDate
		{
			get
			{
				return this._PromisedDate;
			}
			set
			{
				this._PromisedDate = value;
			}
		}
		#endregion

		#region Approved
		public abstract class approved : PX.Data.BQL.BqlBool.Field<approved> { }
		protected Boolean? _Approved;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Approved", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? Approved
		{
			get
			{
				return this._Approved;
			}
			set
			{
				this._Approved = value;
			}
		}
		#endregion
		#region Cancelled
		public abstract class cancelled : PX.Data.BQL.BqlBool.Field<cancelled> { }
		protected Boolean? _Cancelled;
		[PXDBBool()]
		[PXUIField(DisplayName = "Canceled", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? Cancelled
		{
			get
			{
				return this._Cancelled;
			}
			set
			{
				this._Cancelled = value;
			}
		}
		#endregion
		#region TransferRequest
		public abstract class transferRequest : PX.Data.BQL.BqlBool.Field<transferRequest> { }
		protected bool? _TransferRequest = false;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Transfer")]
		public virtual bool? TransferRequest
		{
			get
			{
				return _TransferRequest;
			}
			set
			{
				_TransferRequest = value;
			}
		}
		#endregion
		#region TransferType
		public abstract class transferType : PX.Data.BQL.BqlString.Field<transferType> { }
		protected string _TransferType = RQTransferType.None;
		[PXDBString(1 , IsFixed=true)]
		[PXDefault(RQTransferType.None)]
		[PXUIField(DisplayName = "Transfer Type", Enabled = false)]
		[RQTransferType.List]
		public virtual string TransferType
		{
			get
			{
				return _TransferType;
			}
			set
			{
				_TransferType = value;
			}
		}
		#endregion

		#region SourceTranReqNbr
		public abstract class sourceTranReqNbr : PX.Data.BQL.BqlString.Field<sourceTranReqNbr> { }
		protected String _SourceTranReqNbr;
		[PXDBString(15, IsUnicode = true, InputMask = "")]		
		[PXUIField(DisplayName = "Source Transfer", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual String SourceTranReqNbr
		{
			get
			{
				return this._SourceTranReqNbr;
			}
			set
			{
				this._SourceTranReqNbr = value;
			}
		}
		#endregion
		#region SourceTranLineNbr
		public abstract class sourceTranLineNbr : PX.Data.BQL.BqlInt.Field<sourceTranLineNbr> { }
		protected int? _SourceTranLineNbr;
		[PXDBInt]
		[PXUIField(DisplayName = "Source Transfer Line", Visibility = PXUIVisibility.Visible, Enabled = false)]		
		public virtual int? SourceTranLineNbr
		{
			get
			{
				return this._SourceTranLineNbr;
			}
			set
			{
				this._SourceTranLineNbr = value;
			}
		}
		#endregion

		#region TransferQty
		public abstract class transferQty : PX.Data.BQL.BqlDecimal.Field<transferQty> { }
		protected Decimal? _TransferQty;
		[PXDBQuantity(typeof(RQRequisitionLine.uOM), typeof(RQRequisitionLine.baseTransferQty), HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Transfer Qty.", Enabled = false)]
		public virtual Decimal? TransferQty
		{
			get
			{
				return this._TransferQty;
			}
			set
			{
				this._TransferQty = value;
			}
		}
		#endregion
		#region BaseTransferQty
		public abstract class baseTransferQty : PX.Data.BQL.BqlDecimal.Field<baseTransferQty> { }
		protected Decimal? _BaseTransferQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseTransferQty
		{
			get
			{
				return this._BaseTransferQty;
			}
			set
			{
				this._BaseTransferQty = value;
			}
		}
		#endregion
		#region OpenQty
		public abstract class openQty : PX.Data.BQL.BqlDecimal.Field<openQty> { }
		protected Decimal? _OpenQty;
		[PXDBQuantity(typeof(RQRequisitionLine.uOM), typeof(RQRequisitionLine.baseOpenQty), HandleEmptyKey = true)]
		[PXFormula(typeof(RQRequisitionLine.orderQty), typeof(SumCalc<RQRequisition.openOrderQty>), ValidateAggregateCalculation = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Open Qty.", Enabled = false)]
		public virtual Decimal? OpenQty
		{
			get
			{
				return this._OpenQty;
			}
			set
			{
				this._OpenQty = value;
			}
		}
		#endregion
		#region BaseOpenQty
		public abstract class baseOpenQty : PX.Data.BQL.BqlDecimal.Field<baseOpenQty> { }
		protected Decimal? _BaseOpenQty;
		[PXDBDecimal(6)]
		public virtual Decimal? BaseOpenQty
		{
			get
			{
				return this._BaseOpenQty;
			}
			set
			{
				this._BaseOpenQty = value;
			}
		}
		#endregion				
		#region BiddingQty
		public abstract class biddingQty : PX.Data.BQL.BqlDecimal.Field<biddingQty> { }
		protected Decimal? _BiddingQty;
		[PXDBQuantity(typeof(RQRequisitionLine.uOM), typeof(RQRequisitionLine.baseBiddingQty), HandleEmptyKey = true)]		
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Bidding Qty.", Enabled = false)]
		public virtual Decimal? BiddingQty
		{
			get
			{
				return this._BiddingQty;
			}
			set
			{
				this._BiddingQty = value;
			}
		}
		#endregion
		#region BaseBiddingQty
		public abstract class baseBiddingQty : PX.Data.BQL.BqlDecimal.Field<baseBiddingQty> { }
		protected Decimal? _BaseBiddingQty;
		[PXDBDecimal(6)]
		public virtual Decimal? BaseBiddingQty
		{
			get
			{
				return this._BaseBiddingQty;
			}
			set
			{
				this._BaseBiddingQty = value;
			}
		}
		#endregion				
		#region ByRequest
		public abstract class byRequest : PX.Data.BQL.BqlBool.Field<byRequest> { }
		protected Boolean? _ByRequest;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? ByRequest
		{
			get
			{
				return this._ByRequest;
			}
			set
			{
				this._ByRequest = value;
			}
		}
		#endregion
		#region QTOrderNbr
		public abstract class qTOrderNbr : PX.Data.BQL.BqlString.Field<qTOrderNbr> { }
		protected String _QTOrderNbr;
		[PXDBString(15, IsUnicode = true, InputMask = "")]		
		public virtual String QTOrderNbr
		{
			get
			{
				return this._QTOrderNbr;
			}
			set
			{
				this._QTOrderNbr = value;
			}
		}
		#endregion
		#region QTLineNbr
		public abstract class qTLineNbr : PX.Data.BQL.BqlInt.Field<qTLineNbr> { }
		protected Int32? _QTLineNbr;
		[PXDBInt]		
		public virtual Int32? QTLineNbr
		{
			get
			{
				return this._QTLineNbr;
			}
			set
			{
				this._QTLineNbr = value;
			}
		}
		#endregion
		#region Availability
		public abstract class availability : PX.Data.BQL.BqlString.Field<availability> { }
		protected String _Availability;
		[PXString]
		[PXUIField(DisplayName = "Availability", Enabled = false, Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual String Availability
		{
			get
			{
				return this._Availability;
			}
			set
			{
				this._Availability = value;
			}
		}
		#endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
	}

	public class RQRequisitionLineReceived : RQRequisitionLine
	{
		#region ReqNbr
		public new abstract class reqNbr : BqlString.Field<reqNbr> { }
		#endregion
		#region LineNbr
		public new abstract class lineNbr : BqlInt.Field<lineNbr> { }
		#endregion		

		#region InventoryID
		public new abstract class inventoryID : BqlInt.Field<inventoryID> { }
		#endregion
		#region SubItemID
		public new abstract class subItemID : BqlInt.Field<subItemID> { }
		#endregion		
		#region Description
		public new abstract class description : BqlString.Field<description> { }
		#endregion		
		#region UOM
		public new abstract class uOM : BqlString.Field<uOM> { }
		#endregion		

		#region OrderQty
		[PXDBDecimal]
		[PXUIField(DisplayName = "Requested Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public override Decimal? OrderQty
			{
			get => base.OrderQty;
			set => base.OrderQty = value;
		}
		public new abstract class orderQty : BqlDecimal.Field<orderQty> { }
		#endregion

		#region POOrderQty
		[PXDecimal, PXDBScalar(typeof(
			SelectFrom<POLine>.
			Where<
				POLine.rQReqNbr.IsEqual<RQRequisitionLine.reqNbr>.
				And<POLine.rQReqLineNbr.IsEqual<RQRequisitionLine.lineNbr>>>.
			AggregateTo<
				GroupBy<POLine.rQReqNbr>,
				GroupBy<POLine.rQReqLineNbr>,
				Sum<POLine.orderQty>>.
			SearchFor<POLine.orderQty>))]
		[PXUIField(DisplayName = "Ordered Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? POOrderQty
		{
			get => _POOrderQty;
			set => _POOrderQty = value;
		}
		public abstract class pOOrderQty : BqlDecimal.Field<pOOrderQty> { }
		protected Decimal? _POOrderQty;
		#endregion
		#region POReceivedQty
		[PXDecimal, PXDBScalar(typeof(
			SelectFrom<POLine>.
			Where<
				POLine.rQReqNbr.IsEqual<RQRequisitionLine.reqNbr>.
				And<POLine.rQReqLineNbr.IsEqual<RQRequisitionLine.lineNbr>>>.
			AggregateTo<
				GroupBy<POLine.rQReqNbr>,
				GroupBy<POLine.rQReqLineNbr>,
				Sum<POLine.receivedQty>>.
			SearchFor<POLine.receivedQty>))]
		[PXUIField(DisplayName = "Received Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? POReceivedQty
		{
			get => _POReceivedQty;
			set => _POReceivedQty = value;
		}
		public abstract class pOReceivedQty : BqlDecimal.Field<pOReceivedQty> { }
		protected Decimal? _POReceivedQty;
		#endregion		
		#region Status
		[PXString]
		[RQRequisitionReceivedStatus.List]		
		[PXFormula(typeof(
			RQRequisitionReceivedStatus.closed.When<orderQty.IsLessEqual<pOReceivedQty>>.
			Else<RQRequisitionReceivedStatus.partially>.When<pOReceivedQty.IsGreater<decimal0>>.
			Else<RQRequisitionReceivedStatus.ordered>.When<pOOrderQty.IsGreater<decimal0>>.
			Else<RQRequisitionReceivedStatus.open>))]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public string Status
		{
			get;
			set;
		}
		public abstract class status : BqlString.Field<status> { }
		protected string _Status;
		#endregion
	}

	public static class RQTransferType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute() : base(
				new[]
				{
					Pair(None, Messages.None),
					Pair(Split, Messages.Split),
					Pair(Transfer, Messages.Transfer),
				}) {}
		}

		public const string None = "N";
		public const string Split = "S";
		public const string Transfer = "T";

	}

	public static class RQRequisitionReceivedStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute() : base(
					Pair(Open, Messages.Open),
					Pair(Partially, Messages.PartiallyReceived),
					Pair(Closed, Messages.Received),
				Pair(Ordered, Messages.Canceled)
			) { }
		}

		public const string Open = "O";
		public const string Partially = "P";		
		public const string Closed = "C";		
		public const string Ordered = "B";

		public class open : BqlString.Constant<open> { public open() : base(Open) { } }
		public class partially : BqlString.Constant<partially> { public partially() : base(Partially) { } }
		public class closed : BqlString.Constant<closed> { public closed() : base(Closed) { } }
		public class ordered : BqlString.Constant<ordered> { public ordered() : base(Ordered) { } }
	}
	

	[PXProjection(typeof(Select5<RQRequisitionContent,
		LeftJoin<RQRequest,
					On<RQRequest.orderNbr, Equal<RQRequisitionContent.orderNbr>>,
		LeftJoin<RQRequestClass, On<RQRequestClass.reqClassID, Equal<RQRequest.reqClassID>>>>,
		Where<RQRequestClass.customerRequest, Equal<CS.boolTrue>>,
		Aggregate<
			GroupBy<RQRequisitionContent.reqNbr,
			GroupBy<RQRequisitionContent.reqLineNbr,
			GroupBy<RQRequestClass.customerRequest,
			Sum<RQRequisitionContent.reqQty>>>>>>),
	Persistent=false)]
    [Serializable]
    [PXHidden]
	public partial class RQRequisitionLineCustomers : PXBqlTable, IBqlTable
	{
		#region ReqNbr
		public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
		protected String _ReqNbr;
		[PXDBString(15, IsUnicode = true, InputMask = "", BqlField = typeof(RQRequisitionContent.reqNbr))]		
		public virtual String ReqNbr
		{
			get
			{
				return this._ReqNbr;
			}
			set
			{
				this._ReqNbr = value;
			}
		}
		#endregion
		#region ReqLineNbr
		public abstract class reqLineNbr : PX.Data.BQL.BqlInt.Field<reqLineNbr> { }
		protected int? _ReqLineNbr;
		[PXDBInt(BqlField = typeof(RQRequisitionContent.reqLineNbr))]
		public virtual int? ReqLineNbr
		{
			get
			{
				return this._ReqLineNbr;
			}
			set
			{
				this._ReqLineNbr = value;
			}
		}
		#endregion
		#region CustomerRequest
		public abstract class customerRequest : PX.Data.BQL.BqlBool.Field<customerRequest> { }
		protected Boolean? _CustomerRequest;
		[PXDBBool(BqlField=typeof(RQRequestClass.customerRequest))]		
		public virtual Boolean? CustomerRequest
		{
			get
			{
				return this._CustomerRequest;
			}
			set
			{
				this._CustomerRequest = value;
			}
		}
		#endregion		
		#region ReqQty
		public abstract class reqQty : PX.Data.BQL.BqlDecimal.Field<reqQty> { }
		protected Decimal? _ReqQty;
		[PXDBDecimal(2, BqlField = typeof(RQRequisitionContent.reqQty))]		
		public virtual Decimal? ReqQty
		{
			get
			{
				return this._ReqQty;
			}
			set
			{
				this._ReqQty = value;
			}
		}
		#endregion		
	}
   
    [Serializable]
    [PXHidden]
	public partial class RQSOSource : PXBqlTable, IBqlTable      //TODO Weird DAC without table in DB but wtih bound fields. May be a bug, need to clarify.
	{
		#region CustomerID
		public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
		protected Int32? _CustomerID;
		[Customer(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Customer.acctName), Filterable = true)]
		public virtual Int32? CustomerID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		#endregion
		#region CustomerLocationID
		public abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }
		protected Int32? _CustomerLocationID;
		[PXDefault(typeof(Search<Customer.defLocationID, Where<Customer.bAccountID, Equal<Current<RQRequisition.customerID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<RQRequisition.customerID>>>), DescriptionField = typeof(Location.descr))]
		public virtual Int32? CustomerLocationID
		{
			get
			{
				return this._CustomerLocationID;
			}
			set
			{
				this._CustomerLocationID = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
		protected Int32? _InventoryID;
		[RQRequisitionInventoryItem(Filterable = true)]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region SubItemID
		public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
		protected Int32? _SubItemID;
		[SubItem(typeof(RQRequisitionLine.inventoryID))]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion		
		#region UOM
		public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
		protected String _UOM;
		[INUnit(typeof(RQSOSource.inventoryID), DisplayName = "UOM")]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion		
		#region OrderQty
		public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
		protected Decimal? _OrderQty;
		[PXDBQuantity(typeof(RQRequisitionLine.uOM), typeof(RQRequisitionLine.baseOrderQty), HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Order Qty.", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? OrderQty
		{
			get
			{
				return this._OrderQty;
			}
			set
			{
				this._OrderQty = value;
			}
		}
		#endregion
		#region IsUseMarkup
		public abstract class isUseMarkup : PX.Data.BQL.BqlBool.Field<isUseMarkup> { }
		protected Boolean? _IsUseMarkup;
		[PXDBBool()]
		public virtual Boolean? IsUseMarkup
		{
			get
			{
				return this._IsUseMarkup;
			}
			set
			{
				this._IsUseMarkup = value;
			}
		}
		#endregion		
		#region MarkupPct
		public abstract class markupPct : PX.Data.BQL.BqlDecimal.Field<markupPct> { }
		protected Decimal? _MarkupPct;
		[PXDBPriceCost()]				
		public virtual Decimal? MarkupPct
		{
			get
			{
				return this._MarkupPct;
			}
			set
			{
				this._MarkupPct = value;
			}
		}
		#endregion
		#region EstUnitCost
		public abstract class estUnitCost : PX.Data.BQL.BqlDecimal.Field<estUnitCost> { }
		protected Decimal? _EstUnitCost;

		[PXDBDecimal(6)]
		public virtual Decimal? EstUnitCost
		{
			get
			{
				return this._EstUnitCost;
			}
			set
			{
				this._EstUnitCost = value;
			}
		}
		#endregion
		#region CuryEstUnitCost
		public abstract class curyEstUnitCost : PX.Data.BQL.BqlDecimal.Field<curyEstUnitCost> { }
		protected Decimal? _CuryEstUnitCost;

		[PXDBDecimal(6)]		
		public virtual Decimal? CuryEstUnitCost
		{
			get
			{
				return this._CuryEstUnitCost;
			}
			set
			{
				this._CuryEstUnitCost = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
		[PXString(Common.Constants.TranDescLength, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Description
		{
			get;
			set;
		}
		#endregion
	}
}

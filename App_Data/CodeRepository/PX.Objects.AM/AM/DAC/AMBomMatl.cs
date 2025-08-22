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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CM;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.AM.Attributes;
using PX.Data.BQL.Fluent;

namespace PX.Objects.AM
{
	/// <summary>
	/// A BOM material.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
	[Serializable]
	[PXCacheName(Messages.BOMMatl)]
	public class AMBomMatl : PXBqlTable, IBqlTable, ISortOrder, IBomOper, INotable, IBomDetail
	{
		internal string DebuggerDisplay => $"BOMID = {BOMID}, RevisionID = {RevisionID}, OperationID = {OperationID}, LineID = {LineID}, InventoryID = {InventoryID}";

		#region Keys

		public class PK : PrimaryKeyOf<AMBomMatl>.By<bOMID, revisionID, operationID, lineID>
		{
			public static AMBomMatl Find(PXGraph graph, string bOMID, string revisionID, int? operationID, int? lineID, PKFindOptions options = PKFindOptions.None)
				=> FindBy(graph, bOMID, revisionID, operationID, lineID, options);
			public static AMBomMatl FindDirty(PXGraph graph, string bOMID, string revisionID, int? operationID, int? lineID)
				=> PXSelect<AMBomMatl,
					Where<bOMID, Equal<Required<bOMID>>,
						And<revisionID, Equal<Required<revisionID>>,
						And<operationID, Equal<Required<operationID>>,
						And<lineID, Equal<Required<lineID>>>>>>>
					.SelectWindowed(graph, 0, 1, bOMID, revisionID, operationID, lineID);
		}

		public static class FK
		{
			public class BOM : AMBomItem.PK.ForeignKeyOf<AMBomMatl>.By<bOMID, revisionID> { }
			public class Operation : AMBomOper.PK.ForeignKeyOf<AMBomMatl>.By<bOMID, revisionID, operationID> { }
			public class InventoryItem : PX.Objects.IN.InventoryItem.PK.ForeignKeyOf<AMBomMatl>.By<inventoryID> { }
			public class Site : PX.Objects.IN.INSite.PK.ForeignKeyOf<AMBomMatl>.By<siteID> { }
		}

		#endregion

		#region Selected

		public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }

		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected { get; set; }

		#endregion
		#region BOMID
		public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }

		protected string _BOMID;

		/// <summary>
		/// The identifier of the bill of material.
		/// </summary>
		[BomID(IsKey = true, Visible = false, Enabled = false)]
		[PXDBDefault(typeof(AMBomOper.bOMID))]
		public virtual string BOMID
		{
			get
			{
				return this._BOMID;
			}
			set
			{
				this._BOMID = value;
			}
		}
		#endregion
		#region RevisionID
		public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }

		protected string _RevisionID;

		/// <summary>
		/// The identifier of the BOM revision, which is the modification of the bill of material.
		/// </summary>
		[RevisionIDField(IsKey = true, Visibility = PXUIVisibility.SelectorVisible, Visible = false, Enabled = false)]
		[PXDBDefault(typeof(AMBomOper.revisionID))]
		public virtual string RevisionID
		{
			get
			{
				return this._RevisionID;
			}
			set
			{
				this._RevisionID = value;
			}
		}
		#endregion
		#region OperationID
		public abstract class operationID : PX.Data.BQL.BqlInt.Field<operationID> { }

		protected int? _OperationID;

		/// <summary>
		/// The numeric identifier of the operation.
		/// </summary>
		[OperationIDField(IsKey = true, Visible = false, Enabled = false)]
		[PXDefault(typeof(AMBomOper.operationID))]
		[PXParent(typeof(Select<AMBomOper,
			Where<AMBomOper.bOMID, Equal<Current<AMBomMatl.bOMID>>,
				And<AMBomOper.revisionID, Equal<Current<AMBomMatl.revisionID>>,
					And<AMBomOper.operationID, Equal<Current<AMBomMatl.operationID>>>>>>))]
		[PXParent(typeof(Select<AMBomItem,
			Where<AMBomItem.bOMID, Equal<Current<AMBomMatl.bOMID>>,
				And<AMBomItem.revisionID, Equal<Current<AMBomMatl.revisionID>>>>>))]

		public virtual int? OperationID
		{
			get
			{
				return this._OperationID;
			}
			set
			{
				this._OperationID = value;
			}
		}
		#endregion
		#region LineID
		public abstract class lineID : PX.Data.BQL.BqlInt.Field<lineID> { }

		protected Int32? _LineID;

		/// <summary>
		/// The line ID, which is an autoincremental number.
		/// </summary>
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
		[PXLineNbr(typeof(AMBomOper.lineCntrMatl), DecrementOnDelete = false, ReuseGaps = false)]
		public virtual Int32? LineID
		{
			get
			{
				return this._LineID;
			}
			set
			{
				this._LineID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

		protected Int32? _InventoryID;

		/// <summary>
		/// The identifier of the item used as a material.
		/// </summary>
		[Inventory(Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault]
		[PXForeignReference(typeof(Field<inventoryID>.IsRelatedTo<InventoryItem.inventoryID>))]
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

		/// <summary>
		/// The subitem of the inventory item.
		/// </summary>
		[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
			Where<InventoryItem.inventoryID, Equal<Current<AMBomMatl.inventoryID>>,
			And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		[SubItem(typeof(AMBomMatl.inventoryID))]
		[PXFormula(typeof(Default<AMBomMatl.inventoryID>))]
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
		#region Descr
		public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }

		protected String _Descr;

		/// <summary>
		/// The description of the inventory item.
		/// </summary>
		[PXDBString(256, IsUnicode = true)]
		[PXDefault(typeof(Search<InventoryItem.descr, Where<InventoryItem.inventoryID, Equal<Current<AMBomMatl.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Description")]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion
		#region QtyReq
		public abstract class qtyReq : PX.Data.BQL.BqlDecimal.Field<qtyReq> { }

		protected Decimal? _QtyReq;

		/// <summary>
		/// The material quantity needed to produce the item.
		/// </summary>
		[PXDBQuantity(typeof(AMBomMatl.uOM), typeof(AMBomMatl.baseQty))]
		[PXDefault(TypeCode.Decimal, "1.0")]
		[PXUIField(DisplayName = "Qty Required")]
		public virtual Decimal? QtyReq
		{
			get
			{
				return this._QtyReq;
			}
			set
			{
				this._QtyReq = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }

		protected String _UOM;

		/// <summary>
		/// The unit of measure for the quantity specified in <see cref="QtyReq"/>.
		/// </summary>
		[PXDefault(typeof(Search<InventoryItem.baseUnit, Where<InventoryItem.inventoryID, Equal<Current<AMBomMatl.inventoryID>>>>))]
		[INUnit(typeof(AMBomMatl.inventoryID))]
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
		#region BaseQty
		public abstract class baseQty : PX.Data.BQL.BqlDecimal.Field<baseQty> { }

		protected Decimal? _BaseQty;

		/// <summary>
		/// The base quantity.
		/// </summary>
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "1.0")]
		[PXUIField(DisplayName = "Base Qty")]
		public virtual Decimal? BaseQty
		{
			get
			{
				return this._BaseQty;
			}
			set
			{
				this._BaseQty = value;
			}
		}
		#endregion
		#region UnitCost
		public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }

		protected Decimal? _UnitCost;

		/// <summary>
		/// The unit cost for the material.
		/// </summary>
		[PXDBPriceCost]
		[PXUIField(DisplayName = "Unit Cost")]
		[PXDefault]
		[MatlUnitCostDefault(
			typeof(AMBomMatl.inventoryID),
			typeof(AMBomMatl.siteID),
			typeof(AMBomMatl.uOM),
			typeof(AMBomItem),
			typeof(AMBomItem.siteID))]
		[PXFormula(typeof(Default<AMBomMatl.inventoryID, AMBomMatl.siteID, AMBomMatl.uOM>))]
		public virtual Decimal? UnitCost
		{
			get
			{
				return this._UnitCost;
			}
			set
			{
				this._UnitCost = value;
			}
		}
		#endregion
		#region MaterialType
		public abstract class materialType : PX.Data.BQL.BqlInt.Field<materialType> { }

		protected int? _MaterialType;

		/// <summary>
		/// The type of the material.
		/// </summary>
		[PXDBInt]
		[PXDefault(AMMaterialType.Regular)]
		[PXUIField(DisplayName = "Material Type")]
		[AMMaterialType.List]
		public virtual int? MaterialType
		{
			get
			{
				return this._MaterialType;
			}
			set
			{
				this._MaterialType = value;
			}
		}
		#endregion
		#region PhantomRouting
		public abstract class phantomRouting : PX.Data.BQL.BqlInt.Field<phantomRouting> { }

		protected int? _PhantomRouting;

		/// <summary>
		/// The way of adding operations of the material that is a phantom subassembly to production orders based on the BOM.
		/// </summary>
		[PXDBInt]
		[PXDefault(PhantomRoutingOptions.Before)]
		[PXUIField(DisplayName = "Phantom Routing")]
		[PhantomRoutingOptions.List]
		public virtual int? PhantomRouting
		{
			get
			{
				return this._PhantomRouting;
			}
			set
			{
				this._PhantomRouting = value;
			}
		}
		#endregion
		#region BFlush
		public abstract class bFlush : PX.Data.BQL.BqlBool.Field<bFlush> { }

		protected Boolean? _BFlush;

		/// <summary>
		/// A Boolean value that indicates whether the the user needs to release the material manually.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Backflush Materials")]
		public virtual Boolean? BFlush
		{
			get
			{
				return this._BFlush;
			}
			set
			{
				this._BFlush = value;
			}
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

		protected Int32? _SiteID;

		/// <summary>
		/// The warehouse from which the material is issued.
		/// </summary>
		[PXRestrictor(typeof(Where<INSite.active, Equal<True>>), PX.Objects.IN.Messages.InactiveWarehouse, typeof(INSite.siteCD), CacheGlobal = true)]
		[AMSite]
		[PXForeignReference(typeof(Field<siteID>.IsRelatedTo<INSite.siteID>))]
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
		#region CompBOMID
		public abstract class compBOMID : PX.Data.BQL.BqlString.Field<compBOMID> { }

		protected String _CompBOMID;

		/// <summary>
		/// The subassembly's bill of materials used for production.
		/// </summary>
		[BomID(DisplayName = "Comp BOM ID")]
		public virtual String CompBOMID
		{
			get
			{
				return this._CompBOMID;
			}
			set
			{
				this._CompBOMID = value;
			}
		}
		#endregion
		#region CompBOMRevisionID
		public abstract class compBOMRevisionID : PX.Data.BQL.BqlString.Field<compBOMRevisionID> { }

		protected string _CompBOMRevisionID;

		/// <summary>
		/// The revision of the BOM that is specified in <see cref="CompBOMID"/>.
		/// </summary>
		[RevisionIDField(DisplayName = "Comp BOM Revision")]
		[PXRestrictor(typeof(Where<AMBomItem.status, Equal<AMBomStatus.active>, Or<AMBomItem.status, Equal<AMBomStatus.hold>>>), Messages.BomRevisionIsArchived, typeof(AMBomItem.bOMID), typeof(AMBomItem.revisionID), CacheGlobal = true)]
		[PXSelector(typeof(Search<AMBomItem.revisionID,
				Where<AMBomItem.bOMID, Equal<Current<AMBomMatl.compBOMID>>>>)
			, typeof(AMBomItem.revisionID)
			, typeof(AMBomItem.status)
			, typeof(AMBomItem.descr)
			, typeof(AMBomItem.effStartDate)
			, typeof(AMBomItem.effEndDate)
			, DescriptionField = typeof(AMBomItem.descr))]
		[PXForeignReference(typeof(CompositeKey<Field<AMBomMatl.compBOMID>.IsRelatedTo<AMBomItem.bOMID>, Field<AMBomMatl.compBOMRevisionID>.IsRelatedTo<AMBomItem.revisionID>>))]
		[PXFormula(typeof(Default<AMBomMatl.compBOMID>))]
		public virtual string CompBOMRevisionID
		{
			get
			{
				return this._CompBOMRevisionID;
			}
			set
			{
				this._CompBOMRevisionID = value;
			}
		}
		#endregion
		#region CompEffDate

#if DEBUG
		//Leave for Contract Endpoint Support
#endif
		[Obsolete("Use compBOMID & compBOMRevisionID to capture bom used on order")]
		public abstract class compEffDate : PX.Data.BQL.BqlDateTime.Field<compEffDate> { }

		protected DateTime? _CompEffDate;
		[Obsolete("Use CompBOMID & CompBOMRevisionID to capture bom used on order")]
		[PXDBDate]
		[PXUIField(DisplayName = "Obsolete Comp BOM Eff Date", Visible = false, Visibility = PXUIVisibility.Invisible)]
		public virtual DateTime? CompEffDate
		{
			get
			{
				return this._CompEffDate;
			}
			set
			{
				this._CompEffDate = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }

		protected Int32? _LocationID;

		/// <summary>
		/// The warehouse location from which the material is issued.
		/// </summary>
		[MfgLocationAvail(typeof(AMBomMatl.inventoryID), typeof(AMBomMatl.subItemID), typeof(AMBomMatl.siteID),
			IsSalesType: typeof(Where<AMBomMatl.qtyReq, GreaterEqual<decimal0>>),
			IsReceiptType: typeof(Where<AMBomMatl.qtyReq, Less<decimal0>>))]
		[PXForeignReference(typeof(CompositeKey<Field<siteID>.IsRelatedTo<INLocation.siteID>, Field<locationID>.IsRelatedTo<INLocation.locationID>>))]
		public virtual Int32? LocationID
		{
			get
			{
				return this._LocationID;
			}
			set
			{
				this._LocationID = value;
			}
		}
		#endregion
		#region ScrapFactor
		public abstract class scrapFactor : PX.Data.BQL.BqlDecimal.Field<scrapFactor> { }

		protected Decimal? _ScrapFactor;

		/// <summary>
		/// The scrap or shrinkage factor for the material.
		/// </summary>
		[PXDBDecimal(6, MinValue = 0.0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Scrap Factor")]
		public virtual Decimal? ScrapFactor
		{
			get
			{
				return this._ScrapFactor;
			}
			set
			{
				this._ScrapFactor = value;
			}
		}
		#endregion
		#region BubbleNbr
		public abstract class bubbleNbr : PX.Data.BQL.BqlString.Field<bubbleNbr> { }

		protected String _BubbleNbr;

		/// <summary>
		/// The number of the material on the engineering drawing that relates to the BOM.
		/// </summary>
		[PXDBString(10)]
		[PXUIField(DisplayName = "Bubble Nbr")]
		public virtual String BubbleNbr
		{
			get
			{
				return this._BubbleNbr;
			}
			set
			{
				this._BubbleNbr = value;
			}
		}
		#endregion
		#region EffDate
		public abstract class effDate : PX.Data.BQL.BqlDateTime.Field<effDate> { }

		protected DateTime? _EffDate;

		/// <summary>
		/// The date when the system starts using the material for the item production.
		/// </summary>
		[DaysOffsetDate(MinOffsetDays = "0")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Effective Date")]
		public virtual DateTime? EffDate
		{
			get
			{
				return this._EffDate;
			}
			set
			{
				this._EffDate = value;
			}
		}
		#endregion
		#region ExpDate
		public abstract class expDate : PX.Data.BQL.BqlDateTime.Field<expDate> { }

		protected DateTime? _ExpDate;

		/// <summary>
		/// The date when the system stops using the material for the item production.
		/// </summary>
		[DaysOffsetDate(MinOffsetDays = "-1")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Expiration Date")]
		public virtual DateTime? ExpDate
		{
			get
			{
				return this._ExpDate;
			}
			set
			{
				this._ExpDate = value;
			}
		}
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
		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

		protected Byte[] _tstamp;
		[PXDBTimestamp]
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
		[PXDBCreatedByID]
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
		[PXDBCreatedByScreenID]
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
		[PXDBCreatedDateTime]
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
		[PXDBLastModifiedByID]
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
		[PXDBLastModifiedByScreenID]
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
		[PXDBLastModifiedDateTime]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

		/// <summary>
		/// The line number.
		/// </summary>
		[PXInt]
		[PXUIField(DisplayName = "Line Nbr. 2", Visibility = PXUIVisibility.Invisible, Visible = false, Enabled = false)]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineID;
			}
		}
		#endregion
		#region SortOrder
		public abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }

		protected Int32? _SortOrder;

		/// <summary>
		/// The sorting order.
		/// </summary>
		[PXUIField(DisplayName = PX.Objects.AP.APTran.sortOrder.DispalyName, Visible = false, Enabled = false)]
		[PXDBInt]
		public virtual Int32? SortOrder
		{
			get
			{
				return this._SortOrder;
			}
			set
			{
				this._SortOrder = value;
			}
		}
		#endregion
		#region BatchSize
		public abstract class batchSize : PX.Data.BQL.BqlDecimal.Field<batchSize> { }

		protected Decimal? _BatchSize;

		/// <summary>
		/// The additional parameter that you can use to flexibly set up the required material quantity. The system uses this value to calculate the final quantity of the material.
		/// </summary>
		[BatchSize]
		[PXDefault(TypeCode.Decimal, "1.0")]
		public virtual Decimal? BatchSize
		{
			get
			{
				return this._BatchSize;
			}
			set
			{
				this._BatchSize = value;
			}
		}
		#endregion
		#region PlanCost
		public abstract class planCost : PX.Data.BQL.BqlDecimal.Field<planCost> { }


		protected Decimal? _PlanCost;

		/// <summary>
		/// The planned cost of the inventory item.
		/// </summary>
		[PXBaseCury]
		[PXUnboundDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Planned Cost", Enabled = false, Visible = true)]
		[PXFormula(typeof(Mult<Mult<AMBomMatl.qtyReq, Add<decimal1, AMBomMatl.scrapFactor>>, AMBomMatl.unitCost>))]
		public virtual Decimal? PlanCost
		{
			get
			{
				return this._PlanCost;
			}
			set
			{
				this._PlanCost = value;
			}
		}
		#endregion
		#region LineCntrRef
		/// <summary>
		/// <see cref="AMBomRef"/> line counter
		/// </summary>
		public abstract class lineCntrRef : PX.Data.BQL.BqlInt.Field<lineCntrRef> { }

		protected Int32? _LineCntrRef;
		/// <summary>
		/// <see cref="AMBomRef"/> line counter
		/// </summary>
		[PXDBInt]
		[PXDefault(0)]
		public virtual Int32? LineCntrRef
		{
			get
			{
				return this._LineCntrRef;
			}
			set
			{
				this._LineCntrRef = value;
			}
		}
		#endregion
		#region RowStatus
		public abstract class rowStatus : PX.Data.BQL.BqlInt.Field<rowStatus> { }
		protected int? _RowStatus;

		/// <summary>
		/// The change status.
		/// </summary>
		[PXDBInt]
		[PXUIField(DisplayName = "Change Status", Enabled = false)]
		[AMRowStatus.List]
		public virtual int? RowStatus
		{
			get
			{
				return this._RowStatus;
			}
			set
			{
				this._RowStatus = value;
			}
		}
		#endregion
		#region SubcontractSource
		public abstract class subcontractSource : PX.Data.BQL.BqlInt.Field<subcontractSource> { }

		protected int? _SubcontractSource;

		/// <summary>
		/// The source of the material.
		/// </summary>
		[PXDBInt]
		[PXDefault(AMSubcontractSource.None)]
		[PXUIField(DisplayName = "Subcontract Source")]
		[AMSubcontractSource.List]
		public virtual int? SubcontractSource
		{
			get
			{
				return this._SubcontractSource;
			}
			set
			{
				this._SubcontractSource = value;
			}
		}
		#endregion

		#region IsStockItem
		public abstract class isStockItem : PX.Data.BQL.BqlBool.Field<isStockItem> { }
		protected Boolean? _IsStockItem;

		/// <summary>
		/// A Boolean value that indicates whether the material is a stock item.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Is stock", Enabled = false)]
		[PXDefault(false, typeof(Search<InventoryItem.stkItem, Where<InventoryItem.inventoryID, Equal<Current<AMBomMatl.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<AMBomMatl.inventoryID>))]
		public virtual bool? IsStockItem
		{
			get
			{
				return this._IsStockItem;
			}
			set
			{
				this._IsStockItem = value;
			}
		}
		#endregion
		#region OriginalTreeNodeID (Unbound)
		public abstract class originalTreeNodeID : PX.Data.BQL.BqlString.Field<originalTreeNodeID> { }

		/// <summary>
		/// The original tree node.
		/// </summary>
		[PXString(IsUnicode = true)]
		[PXUIField(DisplayName = "Original Tree Node", Visibility = PXUIVisibility.Invisible)]
		public virtual string OriginalTreeNodeID { get; set; }

		#endregion
	}

	/// <summary>
	/// Projection of the <see cref="AMBOMCurySettings"/> class for the currency cost data of only the <see cref="AMBomMatl"/> (Material) class.
	/// </summary>
	[Serializable]
	[PXCacheName("AMBomMatlCurrency")]
	[PXProjection(typeof(SelectFrom<AMBOMCurySettings>
		.Where<AMBOMCurySettings.lineType.IsEqual<BOMCurySettingsLineType.material>>), Persistent = true)]
	public class AMBomMatlCury : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<AMBomMatlCury>.By<bOMID, revisionID, operationID, lineID, curyID>
		{
			public static AMBomMatlCury Find(PXGraph graph, string bOMID, string revisionID, int? operationID, int? lineID, string curyID, PKFindOptions options = PKFindOptions.None) =>
				FindBy(graph, bOMID, revisionID, operationID, lineID, curyID, options);
		}
		public static class FK
		{
			public class BomMatl : AMBomMatl.PK.ForeignKeyOf<AMBomMatlCury>.By<bOMID, revisionID, operationID, lineID> { }
		}
		#endregion

		#region BOMID
		public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }
		protected String _BOMID;

		/// <inheritdoc cref="AMBOMCurySettings.BOMID"/>
		[BomID(IsKey = true, Enabled = false, BqlField = typeof(AMBOMCurySettings.bOMID))]
		[PXDBDefault(typeof(AMBomOper.bOMID))]
		[PXParent(typeof(FK.BomMatl))]
		public virtual String BOMID
		{
			get
			{
				return this._BOMID;
			}
			set
			{
				this._BOMID = value;
			}
		}
		#endregion
		#region RevisionID
		public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }
		protected String _RevisionID;

		/// <inheritdoc cref="AMBOMCurySettings.RevisionID"/>
		[PXDBString(10, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCC", BqlField = typeof(AMBOMCurySettings.revisionID))]
		[PXUIField(DisplayName = "Revision", Required = true, Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(AMBomMatl.revisionID))]
		public virtual String RevisionID
		{
			get
			{
				return this._RevisionID;
			}
			set
			{
				this._RevisionID = value;
			}
		}
		#endregion
		#region OperationID
		public abstract class operationID : PX.Data.BQL.BqlInt.Field<operationID> { }

		protected int? _OperationID;

		/// <inheritdoc cref="AMBOMCurySettings.OperationID"/>
		[OperationIDField(IsKey = true, Visible = false, Enabled = false, BqlField = typeof(AMBOMCurySettings.operationID))]
		[PXDBDefault(typeof(AMBomMatl.operationID))]
		public virtual int? OperationID
		{
			get
			{
				return this._OperationID;
			}
			set
			{
				this._OperationID = value;
			}
		}
		#endregion
		#region LineID
		public abstract class lineID : PX.Data.BQL.BqlInt.Field<lineID> { }

		protected Int32? _LineID;

		/// <inheritdoc cref="AMBOMCurySettings.LineID"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMBOMCurySettings.lineID))]
		[PXUIField(DisplayName = "LineID", Visible = false, Enabled = false)]
		[PXDBDefault(typeof(AMBomMatl.lineID))]
		public virtual Int32? LineID
		{
			get
			{
				return this._LineID;
			}
			set
			{
				this._LineID = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }

		protected String _CuryID;

		/// <inheritdoc cref="AMBOMCurySettings.CuryID"/>
		[PXDBString(IsUnicode = true, IsKey = true, BqlField = typeof(AMBOMCurySettings.curyID))]
		[PXUIField(DisplayName = "Currency", Enabled = false)]
		[PXSelector(typeof(Search<CurrencyList.curyID>))]
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region LineType
		public abstract class lineType : PX.Data.BQL.BqlString.Field<lineType> { }
		protected String _LineType;

		/// <inheritdoc cref="AMBOMCurySettings.LineType"/>
		[PXDBString(1, IsFixed = true, IsKey = true, BqlField = typeof(AMBOMCurySettings.lineType))]
		[PXDefault(BOMCurySettingsLineType.Material)]
		[BOMCurySettingsLineType.List()]
		[PXUIField(DisplayName = "Line Type", Enabled = false)]

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
		#region SiteID
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
		protected Int32? _SiteID;

		/// <inheritdoc cref="AMBOMCurySettings.SiteID"/>
		[PXUIField(DisplayName = "Site ID", Visible = false, Enabled = false)]
		[Site(BqlField = typeof(AMBOMCurySettings.siteID))]
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
		#region LocationID
		public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }

		protected Int32? _LocationID;

		/// <inheritdoc cref="AMBOMCurySettings.LocationID"/>
		[PXDBInt(BqlField = typeof(AMBOMCurySettings.locationID))]
		[PXUIField(DisplayName = "Location ID", Visible = false, Enabled = false)]
		public virtual Int32? LocationID
		{
			get
			{
				return this._LocationID;
			}
			set
			{
				this._LocationID = value;
			}
		}
		#endregion
		#region VendorID

		public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
		protected Int32? _VendorID;

		/// <inheritdoc cref="AMBOMCurySettings.VendorID"/>
		[PXDBInt(BqlField = typeof(AMBOMCurySettings.vendorID))]
		[PXUIField(DisplayName = "Vendor ID", Visible = false, Enabled = false)]
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
		#region VendorLocationID

		public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
		protected Int32? _VendorLocationID;

		/// <inheritdoc cref="AMBOMCurySettings.VendorLocationID"/>
		[PXDBInt(BqlField = typeof(AMBOMCurySettings.vendorLocationID))]
		[PXUIField(DisplayName = "Vendor Location ID", Visible = false, Enabled = false)]
		public virtual Int32? VendorLocationID
		{
			get
			{
				return this._VendorLocationID;
			}
			set
			{
				this._VendorLocationID = value;
			}
		}
		#endregion
		#region UnitCost
		public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }

		protected Decimal? _UnitCost;

		/// <inheritdoc cref="AMBOMCurySettings.UnitCost"/>
		[PXDBDecimal(BqlField = typeof(AMBOMCurySettings.unitCost))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Unit Cost")]
		public virtual Decimal? UnitCost
		{
			get
			{
				return this._UnitCost;
			}
			set
			{
				this._UnitCost = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

		protected Byte[] _tstamp;
		[PXDBTimestamp(BqlField = typeof(AMBOMCurySettings.Tstamp))]
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
		[PXDBCreatedByID(BqlField = typeof(AMBOMCurySettings.createdByID))]
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
		[PXDBCreatedByScreenID(BqlField = typeof(AMBOMCurySettings.createdByScreenID))]
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
		[PXDBCreatedDateTime(BqlField = typeof(AMBOMCurySettings.createdDateTime))]
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
		[PXDBLastModifiedByID(BqlField = typeof(AMBOMCurySettings.lastModifiedByID))]
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
		[PXDBLastModifiedByScreenID(BqlField = typeof(AMBOMCurySettings.lastModifiedByScreenID))]
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
		[PXDBLastModifiedDateTime(BqlField = typeof(AMBOMCurySettings.lastModifiedDateTime))]
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
}

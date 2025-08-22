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
using PX.Objects.AM.Attributes;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CS;

namespace PX.Objects.AM
{
	/// <summary>
	/// A projection over the <see cref="AMClockTranSplit"/> class that represents the Line Details dialog box for the <see cref="AMClockItem"/> class on the Clock Entry (AM315000) form (which corresponds to the <see cref="ClockEntry"/> graph).
	/// The parent of the class is <see cref = "AMClockItem"/>.
	/// </summary>
	[PXCacheName("Clock Employee Split")]
    [System.Diagnostics.DebuggerDisplay("EmployeeID = {EmployeeID}, LineNbr = {LineNbr}, SplitLineNbr = {SplitLineNbr}")]
    [PXProjection(typeof(Select<AMClockTranSplit>), Persistent = true)]
    [Serializable]
    public class AMClockItemSplit : PXBqlTable, IBqlTable, ILSDetail, IMoveItemSplit
    {
        #region Selected

        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }

        protected bool? _Selected = false;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
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
        #region EmployeeID
        public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }

        protected Int32? _EmployeeID;

		/// <inheritdoc cref="AMClockTranSplit.EmployeeID"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMClockTranSplit.employeeID))]
        [PXDBDefault(typeof(AMClockItem.employeeID))]
        [PXParent(typeof(Select<AMClockItem, Where<AMClockItem.employeeID, Equal<Current<AMClockItemSplit.employeeID>>,
            And<int0, Equal<Current<AMClockItemSplit.lineNbr>>>>>))]
        [PXUIField(DisplayName = "Employee ID")]
        public virtual Int32? EmployeeID
        {
            get
            {
                return this._EmployeeID;
            }
            set
            {
                this._EmployeeID = value;
            }
        }
        #endregion
        #region TranType

        public abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }

        protected String _TranType;

		/// <inheritdoc cref="AMClockTranSplit.TranType"/>
		[PXDBString(3, IsFixed = true, BqlField = typeof(AMClockTranSplit.tranType))]
        [PXDefault(typeof(AMClockItem.tranType))]
        public virtual String TranType
        {
            get
            {
                return this._TranType;
            }
            set
            {
                this._TranType = value;
            }
        }
        #endregion
        #region LineNbr

        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

        protected Int32? _LineNbr;

		/// <inheritdoc cref="AMClockTranSplit.LineNbr"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMClockTranSplit.lineNbr))]
        [PXDefault(0)]        
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
        #region SplitLineNbr

        public abstract class splitLineNbr : PX.Data.BQL.BqlInt.Field<splitLineNbr> { }

        protected Int32? _SplitLineNbr;

		/// <inheritdoc cref="AMClockTranSplit.SplitLineNbr"/>
		[PXDBInt(IsKey = true, BqlField = typeof(AMClockTranSplit.splitLineNbr))]
        [PXLineNbr(typeof(AMClockItem.lotSerCntr))]
        public virtual Int32? SplitLineNbr
        {
            get
            {
                return this._SplitLineNbr;
            }
            set
            {
                this._SplitLineNbr = value;
            }
        }
        #endregion
        #region TranDate

        public abstract class tranDate : PX.Data.BQL.BqlDateTime.Field<tranDate> { }

        protected DateTime? _TranDate;

		/// <inheritdoc cref="AMClockTranSplit.TranDate"/>
		[PXDBDate(BqlField = typeof(AMClockTranSplit.tranDate))]
        [PXDBDefault(typeof(AMClockItem.tranDate))]
		[PXUIField(DisplayName = "Transaction Date")]
        public virtual DateTime? TranDate
        {
            get
            {
                return this._TranDate;
            }
            set
            {
                this._TranDate = value;
            }
        }
        #endregion
        #region InventoryID

        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

        protected Int32? _InventoryID;

		/// <inheritdoc cref="AMClockTranSplit.InventoryID"/>
		[Inventory(Visible = false, Enabled = false, BqlField = typeof(AMClockTranSplit.inventoryID))]
        [PXDefault(typeof(AMClockItem.inventoryID))]
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

		/// <inheritdoc cref="AMClockTranSplit.SubItemID"/>
		[SubItem(
            typeof(AMClockItemSplit.inventoryID),
            typeof(LeftJoin<INSiteStatusByCostCenter,
                On<INSiteStatusByCostCenter.subItemID, Equal<INSubItem.subItemID>,
                And<INSiteStatusByCostCenter.inventoryID, Equal<Optional<AMClockItemSplit.inventoryID>>,
                And<INSiteStatusByCostCenter.siteID, Equal<Optional<AMClockItemSplit.siteID>>,
				And<INSiteStatusByCostCenter.costCenterID, Equal<CostCenter.freeStock>>>>>>),
			BqlField = typeof(AMClockTranSplit.subItemID))]
        [PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
            Where<InventoryItem.inventoryID, Equal<Current<AMClockItemSplit.inventoryID>>,
            And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>))]
        [PXFormula(typeof(Default<AMClockItemSplit.inventoryID>))]
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

		/// <inheritdoc cref="AMClockTranSplit.SiteID"/>
		[PXRestrictor(typeof(Where<INSite.active, Equal<True>>), PX.Objects.IN.Messages.InactiveWarehouse, typeof(INSite.siteCD), CacheGlobal = true)]
        [Site(BqlField = typeof(AMClockTranSplit.siteID))]
        [PXDefault(typeof(AMClockItem.siteID))]
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

		/// <inheritdoc cref="AMClockTranSplit.LocationID"/>
		[MfgLocationAvail(typeof(AMClockItemSplit.inventoryID), typeof(AMClockItemSplit.subItemID), typeof(AMClockItemSplit.siteID), false, true, null, typeof(AMClockItem), BqlField = typeof(AMClockTranSplit.locationID))]
        [PXDefault]
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
        #region LotSerialNbr

        public abstract class lotSerialNbr : PX.Data.BQL.BqlString.Field<lotSerialNbr> { }

        protected String _LotSerialNbr;

		/// <inheritdoc cref="AMClockTranSplit.LotSerialNbr"/>
		[AMLotSerialNbr(typeof(AMClockItemSplit.inventoryID), typeof(AMClockItemSplit.subItemID),
            typeof(AMClockItemSplit.locationID), typeof(AMClockItem.lotSerialNbr), FieldClass = "LotSerial", BqlField = typeof(AMClockTranSplit.lotSerialNbr))]
        public virtual String LotSerialNbr
        {
            get
            {
                return this._LotSerialNbr;
            }
            set
            {
                this._LotSerialNbr = value;
            }
        }
        #endregion
        #region LotSerClassID

        public abstract class lotSerClassID : PX.Data.BQL.BqlString.Field<lotSerClassID> { }

        protected String _LotSerClassID;

		/// <summary>
		/// Lot serial class
		/// </summary>
		[PXString(10, IsUnicode = true)]
        public virtual String LotSerClassID
        {
            get
            {
                return this._LotSerClassID;
            }
            set
            {
                this._LotSerClassID = value;
            }
        }
        #endregion
        #region AssignedNbr

        public abstract class assignedNbr : PX.Data.BQL.BqlString.Field<assignedNbr> { }

        protected String _AssignedNbr;

		/// <summary>
		/// The assigned number.
		/// </summary>
		[PXString(30, IsUnicode = true)]
        public virtual String AssignedNbr
        {
            get
            {
                return this._AssignedNbr;
            }
            set
            {
                this._AssignedNbr = value;
            }
        }
        #endregion
        #region ExpireDate

        public abstract class expireDate : PX.Data.BQL.BqlDateTime.Field<expireDate> { }

        protected DateTime? _ExpireDate;

		/// <inheritdoc cref="AMClockTranSplit.ExpireDate"/>
		[INExpireDate(typeof(AMClockItemSplit.inventoryID), BqlField = typeof(AMClockTranSplit.expireDate))]
        public virtual DateTime? ExpireDate
        {
            get
            {
                return this._ExpireDate;
            }
            set
            {
                this._ExpireDate = value;
            }
        }
        #endregion
        #region InvtMult

        public abstract class invtMult : PX.Data.BQL.BqlShort.Field<invtMult> { }

        protected Int16? _InvtMult;

		/// <inheritdoc cref="AMClockTranSplit.InvtMult"/>
		[PXDBShort( BqlField = typeof(AMClockTranSplit.invtMult))]
        [PXDefault(typeof(AMClockItem.invtMult))]
        public virtual Int16? InvtMult
        {
            get
            {
                return this._InvtMult;
            }
            set
            {
                this._InvtMult = value;
            }
        }
        #endregion
        #region Released

        public abstract class released : PX.Data.BQL.BqlBool.Field<released> { }

        protected Boolean? _Released;

		/// <inheritdoc cref="AMClockTranSplit.Released"/>
		[PXDBBool( BqlField = typeof(AMClockTranSplit.released))]
        [PXDefault(false)]
        public virtual Boolean? Released
        {
            get
            {
                return this._Released;
            }
            set
            {
                this._Released = value;
            }
        }
        #endregion
        #region UOM

        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }

        protected String _UOM;

		/// <inheritdoc cref="AMClockTranSplit.UOM"/>
		[INUnit(typeof(AMClockItemSplit.inventoryID), DisplayName = "UOM", Enabled = false, BqlField = typeof(AMClockTranSplit.uOM))]
        [PXDefault(typeof(AMClockItem.uOM))]
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
        #region Qty

        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }

        protected Decimal? _Qty;

		/// <inheritdoc cref="AMClockTranSplit.Qty"/>
		[PXDBQuantity(typeof(AMClockItemSplit.uOM), typeof(AMClockItemSplit.baseQty), BqlField = typeof(AMClockTranSplit.qty))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Quantity")]
        public virtual Decimal? Qty
        {
            get
            {
                return this._Qty;
            }
            set
            {
                this._Qty = value;
            }
        }
        #endregion
        #region BaseQty

        public abstract class baseQty : PX.Data.BQL.BqlDecimal.Field<baseQty> { }

        protected Decimal? _BaseQty;

		/// <inheritdoc cref="AMClockTranSplit.BaseQty"/>
		[PXDBQuantity(BqlField = typeof(AMClockTranSplit.baseQty))]
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
        #region CreatedByID

        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }

        protected Guid? _CreatedByID;
        [PXDBCreatedByID(BqlField = typeof(AMClockTranSplit.createdByID))]
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
        [PXDBCreatedByScreenID(BqlField = typeof(AMClockTranSplit.createdByScreenID))]
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
        [PXDBCreatedDateTime(BqlField = typeof(AMClockTranSplit.createdDateTime))]
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
        [PXDBLastModifiedByID(BqlField = typeof(AMClockTranSplit.lastModifiedByID))]
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
        [PXDBLastModifiedByScreenID(BqlField = typeof(AMClockTranSplit.lastModifiedByScreenID))]
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
        [PXDBLastModifiedDateTime(BqlField = typeof(AMClockTranSplit.lastModifiedDateTime))]
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
        #region tstamp

        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

        protected Byte[] _tstamp;
        [PXDBTimestamp(BqlField = typeof(AMClockTranSplit.Tstamp))]
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
        #region ProjectID
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }

        protected Int32? _ProjectID;

		/// <summary>
		/// The project.
		/// </summary>
		[PXInt()]
        [PXUIField(DisplayName = "Project", Visible = false, Enabled = false)]
        public virtual Int32? ProjectID
        {
            get
            {
                return this._ProjectID;
            }
            set
            {
                this._ProjectID = value;
            }
        }
        #endregion
        #region TaskID
        public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }

        protected Int32? _TaskID;

		/// <summary>
		/// The task.
		/// </summary>
		[PXInt()]
        [PXUIField(DisplayName = "Task", Visible = false, Enabled = false)]
        public virtual Int32? TaskID
        {
            get
            {
                return this._TaskID;
            }
            set
            {
                this._TaskID = value;
            }
        }
        #endregion
        #region Is Stock Item
        public abstract class isStockItem : PX.Data.BQL.BqlBool.Field<isStockItem> { }

        protected bool? _IsStockItem;

		/// <inheritdoc cref="AMClockTranSplit.IsStockItem"/>
		[PXDBBool(BqlField = typeof(AMClockTranSplit.isStockItem))]
        [PXUIField(DisplayName = "Stock Item")]
        [PXFormula(typeof(Selector<AMClockItemSplit.inventoryID, InventoryItem.stkItem>))]
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
        #region IsAllocated

        public abstract class isAllocated : PX.Data.BQL.BqlBool.Field<isAllocated> { }

        protected Boolean? _IsAllocated;

		/// <inheritdoc cref="AMClockTranSplit.IsAllocated"/>
		[PXDBBool(BqlField = typeof(AMClockTranSplit.isAllocated))]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Allocated")]
        public virtual Boolean? IsAllocated
        {
            get
            {
                return this._IsAllocated;
            }
            set
            {
                this._IsAllocated = value;
            }
        }
        #endregion

		bool? ILSMaster.IsIntercompany => false;
    }
}

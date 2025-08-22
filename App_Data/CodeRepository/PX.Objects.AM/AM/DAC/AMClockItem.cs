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
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.AM.Attributes;
using PX.Objects.AM.CacheExtensions;
using PX.Objects.AP;
using PX.Objects.EP;

namespace PX.Objects.AM
{
    /// <summary>
    /// A manufacturing clock transaction.
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.ClockItem)]
    [PXPrimaryGraph(typeof(ClockEntry))]
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class AMClockItem : PXBqlTable, IBqlTable, INotable, ILSPrimary, IProdOrder
    {
        internal string DebuggerDisplay => $"EmployeeID = {EmployeeID}";

        #region Keys

        public class PK : PrimaryKeyOf<AMClockItem>.By<employeeID>
        {
            public static AMClockItem Find(PXGraph graph, int? employeeID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, employeeID, options);

            public static AMClockItem FindDirty(PXGraph graph, int? employeeID)
                => PXSelect<AMClockItem, Where<AMClockItem.employeeID, Equal<Required<AMClockItem.employeeID>>>>
                    .SelectWindowed(graph, 0, 1, employeeID);
        }

        #endregion

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
        #region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }

        protected Int32? _BranchID;

		/// <summary>
		/// The branch of the clock transaction.
		/// </summary>
		[Branch]
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
        #region EmployeeID
        public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }

        protected Int32? _EmployeeID;

		/// <summary>
		/// The identifier of the employee for whom the labor time is recorded.
		/// </summary>
		[PXDBInt(IsKey = true)]
		[ProductionEmployeeSelector(typeof(Search2<EPEmployee.bAccountID,
					LeftJoin<EPEmployeePosition,
						On<EPEmployeePosition.employeeID, Equal<EPEmployee.bAccountID>,
						And<EPEmployeePosition.isActive, Equal<True>>>>,
					Where<EPEmployeeExt.amProductionEmployee, Equal<True>,
						And<EPEmployee.vStatus, Equal<VendorStatus.active>,
						And<Where<Current<AMPSetup.restrictClockCurrentUser>, Equal<boolFalse>, Or<Current<AccessInfo.userID>, Equal<EPEmployee.userID>>>>>>>))]
        [PXDefault]
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

		/// <summary>
		/// The type of the transaction.
		/// </summary>
		[PXDBString(3, IsFixed = true)]
        [AMTranType.List]
        [PXDefault(typeof(AMTranType.labor))]
        [PXUIField(DisplayName = "Tran. Type", Enabled = false, Visible = false)]
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
		#region OrderType
		public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }

        protected String _OrderType;

		/// <summary>
		/// The type of the production order for which the labor time is recorded.
		/// </summary>
		[PXDefault(typeof(AMPSetup.defaultOrderType))]
        [AMOrderTypeField(PersistingCheck = PXPersistingCheck.Nothing, Required = false)]
        [PXRestrictor(typeof(Where<AMOrderType.function, NotEqual<OrderTypeFunction.planning>>), Messages.IncorrectOrderTypeFunction)]
        [PXRestrictor(typeof(Where<AMOrderType.active, Equal<True>>), PX.Objects.SO.Messages.OrderTypeInactive)]
        [AMOrderTypeSelector]
        public virtual String OrderType
        {
            get
            {
                return this._OrderType;
            }
            set
            {
                this._OrderType = value;
            }
        }
        #endregion
        #region ProdOrdID
        public abstract class prodOrdID : PX.Data.BQL.BqlString.Field<prodOrdID> { }

        protected String _ProdOrdID;

		/// <summary>
		/// The reference number of the production order that contains the operation for which the labor time is recorded. 
		/// </summary>
		[ProductionNbr(PersistingCheck = PXPersistingCheck.Nothing, Required = false)]
        [PXDefault]
        [ProductionOrderSelector(typeof(orderType), true)]
        [PXFormula(typeof(Validate<orderType>))]
        [PXRestrictor(typeof(Where<AMProdItem.isOpen, Equal<True>>),
            Messages.ProdStatusInvalidForProcess, typeof(AMProdItem.orderType), typeof(AMProdItem.prodOrdID), typeof(AMProdItem.statusID))]
        public virtual String ProdOrdID
        {
            get
            {
                return this._ProdOrdID;
            }
            set
            {
                this._ProdOrdID = value;
            }
        }
        #endregion
        #region OperationID
        public abstract class operationID : PX.Data.BQL.BqlInt.Field<operationID> { }

        protected int? _OperationID;

		/// <summary>
		/// The operation of the production order for which the labor time is recorded.
		/// </summary>
		[OperationIDField(PersistingCheck = PXPersistingCheck.Nothing, Required = false)]
        [PXDefault(typeof(Search<
            AMProdOper.operationID,
            Where<AMProdOper.orderType, Equal<Current<orderType>>,
                And<AMProdOper.prodOrdID, Equal<Current<prodOrdID>>>>,
            OrderBy<
                Asc<AMProdOper.operationCD>>>))]
        [PXSelector(typeof(Search<AMProdOper.operationID,
                Where<AMProdOper.orderType, Equal<Current<orderType>>,
                    And<AMProdOper.prodOrdID, Equal<Current<prodOrdID>>>>>),
            SubstituteKey = typeof(AMProdOper.operationCD))]
        [PXFormula(typeof(Validate<prodOrdID>))]
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
        #region InventoryID
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

        protected Int32? _InventoryID;

		/// <summary>
		/// The item being produced, which is specified in the production order.
		/// </summary>
		[PXDefault(typeof(Search<AMProdItem.inventoryID,
            Where<AMProdItem.orderType, Equal<Current<AMClockItem.orderType>>,
                And<AMProdItem.prodOrdID, Equal<Current<AMClockItem.prodOrdID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [Inventory(Enabled = false, Required = false)]
        [PXFormula(typeof(Default<AMClockItem.prodOrdID>))]
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
		/// The subitem of the item specified in <see cref="InventoryID"/>.
		/// </summary>
		[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
            Where<InventoryItem.inventoryID, Equal<Current<inventoryID>>,
            And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [SubItem(
            typeof(inventoryID),
            typeof(LeftJoin<INSiteStatusByCostCenter,
                On<INSiteStatusByCostCenter.subItemID, Equal<INSubItem.subItemID>,
                And<INSiteStatusByCostCenter.inventoryID, Equal<Optional<inventoryID>>,
                And<INSiteStatusByCostCenter.siteID, Equal<Optional<siteID>>,
				And<INSiteStatusByCostCenter.costCenterID, Equal<CostCenter.freeStock>>>>>>))]
        [PXFormula(typeof(Default<inventoryID>))]
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

		/// <summary>
		/// The warehouse to which the system will move the completed items when the last operation in the routing is completed.
		/// </summary>
		[PXDefault(typeof(Search<AMProdItem.siteID,
            Where<AMProdItem.orderType, Equal<Current<AMClockItem.orderType>>,
                And<AMProdItem.prodOrdID, Equal<Current<AMClockItem.prodOrdID>>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [SiteAvail(typeof(AMClockItem.inventoryID), typeof(AMClockItem.subItemID), typeof(CostCenter.freeStock))]
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
        #region LocationID
        public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }

        protected Int32? _LocationID;

		/// <summary>
		/// The warehouse location to which the system will move the complete items to when the last routing operation is completed.
		/// </summary>
		[MfgLocationAvail(typeof(inventoryID), typeof(subItemID), typeof(siteID), false, true)]
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
        #region TranDate
        public abstract class tranDate : PX.Data.BQL.BqlDateTime.Field<tranDate> { }

        protected DateTime? _TranDate;

		/// <summary>
		/// The date when the clock entry has been created.
		/// </summary>
		[PXDBDate]
        [PXDBDefault(typeof(AccessInfo.businessDate))]
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
        #region Qty
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }

        protected Decimal? _Qty;

		/// <summary>
		/// The quantity of items completed for this operation.
		/// </summary>
		[PXDBQuantity(typeof(uOM), typeof(baseQty), HandleEmptyKey = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
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
        #region UOM
        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }

        protected String _UOM;

		/// <summary>
		/// The unit of measure for the item quantity specified in the <see cref="Qty"/> field.
		/// </summary>
		[PXDefault(typeof(Search<AMProdItem.uOM, Where<AMProdItem.prodOrdID, Equal<Current<AMClockItem.prodOrdID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [INUnit(typeof(AMClockItem.inventoryID), Enabled = false)]
        [PXFormula(typeof(Default<AMClockItem.prodOrdID>))]
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
		[PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
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
		#region StartTime

		public abstract class startTime : PX.Data.BQL.BqlDateTime.Field<startTime> { }

        protected DateTime? _StartTime;

		/// <summary>
		/// The selected employee’s clock-in time for the production operation.
		/// </summary>
		[PXDBDateAndTime(DisplayMask = "t", UseTimeZone = true)]
        [PXUIField(DisplayName = "Start Time", Enabled = false)]
        public virtual DateTime? StartTime
        {
            get
            {
                return this._StartTime;
            }
            set
            {
                this._StartTime = value;
            }
        }
        #endregion
        #region EndTime

        public abstract class endTime : PX.Data.BQL.BqlDateTime.Field<endTime> { }

        protected DateTime? _EndTime;

		/// <summary>
		/// The employee’s clock-out time for the production operation.
		/// </summary>
		[PXDBDateAndTime(DisplayMask = "t", UseTimeZone = true)]
        [PXUIField(DisplayName = "End Time")]
        public virtual DateTime? EndTime
        {
            get
            {
                return this._EndTime;
            }
            set
            {
                this._EndTime = value;
            }
        }
        #endregion
        #region LaborTime
        public abstract class laborTime : PX.Data.BQL.BqlInt.Field<laborTime> { }

        protected Int32? _LaborTime;

		/// <summary>
		/// The automatically calculated amount of time during which the employee was clocked in
		/// for the operation of the production order.
		/// </summary>
		[PXUIField(DisplayName = "Duration", Enabled = false)]
        [PXUnboundDefault(0)]
		[PXFormula(typeof(
			  DateDiff<startTime, IsNull<endTime, TimeZoneNow>, DateDiff.minute>))]
		[PXTimeSpanLong(Format = TimeSpanFormatType.LongHoursMinutes)]
		public virtual Int32? LaborTime
        {
            get
            {
                return this._LaborTime;
            }
            set
            {
                this._LaborTime = value;
            }
        }
        #endregion
        #region LastOper
        public abstract class lastOper : PX.Data.BQL.BqlBool.Field<lastOper> { }

        protected Boolean? _LastOper;

		/// <summary>
		/// A Boolean value that indicates whether the operation is the last operation.
		/// </summary>
		[PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Is Last Oper")]
        public virtual Boolean? LastOper
        {
            get
            {
                return this._LastOper;
            }
            set
            {
                this._LastOper = value;
            }
        }
        #endregion
        #region LotSerCntr
        public abstract class lotSerCntr : PX.Data.BQL.BqlInt.Field<lotSerCntr> { }

        protected Int32? _LotSerCntr;

		/// <summary>
		/// A counter for lot serials.
		/// </summary>
		[PXDBInt]
        [PXDefault(0)]
        [PXUIField(DisplayName = "LotSerCntr")]
        public virtual Int32? LotSerCntr
        {
            get
            {
                return this._LotSerCntr;
            }
            set
            {
                this._LotSerCntr = value;
            }
        }
        #endregion
        #region LotSerialNbr

        public abstract class lotSerialNbr : PX.Data.BQL.BqlString.Field<lotSerialNbr> { }

        protected String _LotSerialNbr;

		/// <summary>
		/// The lot serial number.
		/// </summary>
		[AMLotSerialNbr(typeof(inventoryID), typeof(subItemID), typeof(locationID),
            PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region ShiftCD
		public abstract class shiftCD : PX.Data.BQL.BqlString.Field<shiftCD> { }

        protected String _ShiftCD;

		/// <summary>
		/// The shift in which the employee works.
		/// </summary>
		[ShiftCDField]
        [ShiftCodeSelector]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual String ShiftCD
        {
            get
            {
                return this._ShiftCD;
            }
            set
            {
                this._ShiftCD = value;
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
        #region InvtMult

        public abstract class invtMult : PX.Data.BQL.BqlShort.Field<invtMult> { }

        protected Int16? _InvtMult;

		/// <summary>
		/// A multiplier.
		/// </summary>
		[PXDBShort]
        [PXDefault((short)0)]
        [PXUIField(DisplayName = "Multiplier")]
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
        #region ExpireDate

        public abstract class expireDate : PX.Data.BQL.BqlDateTime.Field<expireDate> { }

        protected DateTime? _ExpireDate;

		/// <summary>
		/// The expiration date of the specified quantity of the stock item.
		/// </summary>
		[INExpireDate(typeof(inventoryID), PersistingCheck = PXPersistingCheck.Nothing, FieldClass = "LotSerial")]
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
        #region TranDesc
        public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }

        protected String _TranDesc;

		/// <summary>
		/// A transaction description.
		/// </summary>
		[PXDBString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Tran Description", Visible = false)]
        public virtual String TranDesc
        {
            get
            {
                return this._TranDesc;
            }
            set
            {
                this._TranDesc = value;
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
        #region ProjectID
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }

        protected Int32? _ProjectID;

		/// <summary>
		/// The project.
		/// </summary>
		[ProjectBase]
        [ProjectDefault(BatchModule.IN, typeof(Search<AMProdItem.projectID, Where<AMProdItem.orderType, Equal<Current<orderType>>,
            And<AMProdItem.prodOrdID, Equal<Current<prodOrdID>>, And<AMProdItem.updateProject, Equal<True>>>>>))]
        [PXRestrictor(typeof(Where<PMProject.isActive, Equal<True>>), PX.Objects.PM.Messages.InactiveContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInIN, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXFormula(typeof(Default<prodOrdID>))]
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
		/// The task of the project.
		/// </summary>
		[PXDefault(typeof(Search<AMProdItem.taskID, Where<AMProdItem.orderType, Equal<Current<orderType>>,
            And<AMProdItem.prodOrdID, Equal<Current<prodOrdID>>, And<AMProdItem.updateProject, Equal<True>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<projectID>))]
        [BaseProjectTask(typeof(projectID))]
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
        #region LineCntr
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }

        protected Int32? _LineCntr;

		/// <summary>
		/// A line counter.
		/// </summary>
		[PXDBInt]
        [PXDefault(0)]
        public virtual Int32? LineCntr
        {
            get
            {
                return this._LineCntr;
            }
            set
            {
                this._LineCntr = value;
            }
        }
		#endregion
		#region UnassignedQty

        public abstract class unassignedQty : PX.Data.BQL.BqlDecimal.Field<unassignedQty> { }

        protected Decimal? _UnassignedQty;

		/// <summary>
		/// The unassigned quantity.
		/// </summary>
		[PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? UnassignedQty
        {
            get
            {
                return this._UnassignedQty;
            }
            set
            {
                this._UnassignedQty = value;
            }
        }
        #endregion
        #region IsClockedIn
        public abstract class isClockedIn : PX.Data.BQL.BqlBool.Field<isClockedIn> { }

		/// <summary>
		/// A Boolean value that indicates whether the item is clocked in.
		/// </summary>
		[PXBool]
        [PXUIField(DisplayName = "Is Clocked In")]
        [PXDependsOnFields(typeof(startTime), typeof(endTime))]
        public Boolean? IsClockedIn => StartTime != null && EndTime == null;
        #endregion

        #region Methods

        public static implicit operator AMClockItemSplit(AMClockItem item)
        {
            AMClockItemSplit ret = new AMClockItemSplit();
            ret.EmployeeID = item.EmployeeID;
            ret.TranType = item.TranType;
            ret.LineNbr = (int)0;
            ret.SplitLineNbr = (int)1;
            ret.InventoryID = item.InventoryID;
            ret.SiteID = item.SiteID;
            ret.SubItemID = item.SubItemID;
            ret.LocationID = item.LocationID;
            ret.LotSerialNbr = item.LotSerialNbr;
            ret.ExpireDate = item.ExpireDate;
            ret.Qty = Math.Abs(item.Qty.GetValueOrDefault());
            ret.BaseQty = Math.Abs(item.BaseQty.GetValueOrDefault());
            ret.UOM = item.UOM;
            ret.TranDate = item.TranDate;
            ret.InvtMult = item.InvtMult;

            return ret;
        }

        public static implicit operator AMClockItem(AMClockItemSplit item)
        {
            AMClockItem ret = new AMClockItem();
            ret.EmployeeID = item.EmployeeID;
            ret.TranType = item.TranType;
            ret.InventoryID = item.InventoryID;
            ret.SiteID = item.SiteID;
            ret.SubItemID = item.SubItemID;
            ret.LocationID = item.LocationID;
            ret.LotSerialNbr = item.LotSerialNbr;

            //Split recs show a positive qty - AMClockItem shows a positive or negative qty
            ret.Qty = Math.Abs(item.Qty.GetValueOrDefault()) * (item.InvtMult * -1);
            ret.BaseQty = Math.Abs(item.BaseQty.GetValueOrDefault()) * (item.InvtMult * -1);

            ret.UOM = item.UOM;
            ret.TranDate = item.TranDate;
            ret.InvtMult = item.InvtMult;

            return ret;
        }

        #endregion

		bool? ILSMaster.IsIntercompany => false;
        bool? ILSPrimary.IsStockItem { set { } }
		int? ILSPrimary.CostCenterID => CostCenter.FreeStock;
    }
}

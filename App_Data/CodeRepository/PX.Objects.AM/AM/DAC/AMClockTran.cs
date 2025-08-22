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

namespace PX.Objects.AM
{
    /// <summary>
    /// A manufacturing clock transaction.
    /// </summary>
	[Serializable]
    [PXCacheName(Messages.ClockLine)]
	[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class AMClockTran : PXBqlTable, IBqlTable, INotable, ILSPrimary
    {
	    internal string DebuggerDisplay => $"EmployeeID = {EmployeeID}, LineNbr = {LineNbr}";

		public class PK : PrimaryKeyOf<AMClockTran>.By<employeeID, lineNbr>
		{
			public static AMClockTran Find(PXGraph graph, int? employeeID, int? lineNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, employeeID, lineNbr, options);
		}
		public static class FK
		{
			public class ClockItem : AMClockItem.PK.ForeignKeyOf<AMClockTran>.By<employeeID> { }
			public class OrderType : AMOrderType.PK.ForeignKeyOf<AMClockTran>.By<AMClockTran.orderType> { }
			public class ProductionOrder : AMProdItem.PK.ForeignKeyOf<AMClockTran>.By<AMClockTran.orderType, prodOrdID> { }
			public class Operation : AMProdOper.PK.ForeignKeyOf<AMClockTran>.By<AMClockTran.orderType, prodOrdID, operationID> { }
			public class Workcenter : AMWC.PK.ForeignKeyOf<AMClockTran>.By<wcID> { }
			public class Employee : PX.Objects.EP.EPEmployee.PK.ForeignKeyOf<AMClockTran>.By<employeeID> { }
			public class Shift : PX.Objects.EP.EPShiftCode.UK.ForeignKeyOf<AMClockTran>.By<shiftCD> { }
			public class Site : INSite.PK.ForeignKeyOf<AMClockTran>.By<siteID> { }
			public class InventoryItem : PX.Objects.IN.InventoryItem.PK.ForeignKeyOf<AMClockTran>.By<inventoryID> { }
		}

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
		/// The branch ID.
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
        [ProductionEmployeeSelector(ValidComboRequired = false)]
        [PXUIField(DisplayName = "Employee ID", Enabled = false, Visible = false)]
        [PXDBDefault(typeof(AMClockItem.employeeID))]
        [PXParent(typeof(Select<AMClockItem, Where<AMClockItem.employeeID, Equal<Current<AMClockTran.employeeID>>>>), LeaveChildren = true)]
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
        #region LineNbr
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

        protected Int32? _LineNbr;

		/// <summary>
		/// The line number.
		/// </summary>
		[PXDBInt(IsKey = true)]
        [PXDefault]
        [PXLineNbr(typeof(AMClockItem.lineCntr))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false, Enabled = false)]
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
        #region TranType

        public abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }

        protected String _TranType;

		/// <summary>
		/// The transaction type.
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
		/// The type of the production order for which the labor time was recorded.
		/// </summary>
		[PXDefault(typeof(AMPSetup.defaultOrderType))]
        [AMOrderTypeField]
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
		/// The reference number of the production order that contains the operation for which the labor time was recorded.
		/// </summary>
		[ProductionNbr]
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
		/// The production operation for which the labor time was recorded.
		/// </summary>
		[OperationIDField]
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
		 Where<AMProdItem.orderType, Equal<Current<AMClockTran.orderType>>,
			 And<AMProdItem.prodOrdID, Equal<Current<AMClockTran.prodOrdID>>>>>))]
		[Inventory(Enabled = false, Required = false)]
		[PXFormula(typeof(Default<AMClockTran.prodOrdID>))]
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
		/// The warehouse to which the system moves the completed items to when the last routing operation is completed.
		/// </summary>
		[PXDefault(typeof(Search<AMProdItem.siteID, Where<AMProdItem.orderType, Equal<Current<orderType>>, And<AMProdItem.prodOrdID, Equal<Current<prodOrdID>>>>>))]
        [SiteAvail(typeof(inventoryID), typeof(subItemID), typeof(CostCenter.freeStock), Enabled = false)]
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
		/// The warehouse location to which the system moves the completed items to when the last routing operation is completed.
		/// </summary>
		[MfgLocationAvail(typeof(inventoryID), typeof(subItemID), typeof(siteID), false, true, Enabled = false)]
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
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Transaction Date", Enabled = false)]
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
        [PXDefault(TypeCode.Decimal,"0.0")]
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
		/// The unit of measure for the item quantity specified in <see cref="Qty"/>.
		/// </summary>
		[PXDefault(typeof(Search<InventoryItem.baseUnit, Where<InventoryItem.inventoryID, Equal<Current<inventoryID>>>>))]
        [INUnit(typeof(inventoryID), Enabled = false)]
		[PXFormula(typeof(Default<AMClockTran.inventoryID>))]
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
        #region Closeflg
        public abstract class closeflg : PX.Data.BQL.BqlBool.Field<closeflg> { }

        protected Boolean? _Closeflg;

		/// <summary>
		/// A Boolean value that indicates whether the clock transaction is approved.
		/// </summary>
		[PXDBBool]
        [PXDefault(false)]
		[PXUIField(Enabled = false, DisplayName = "Approved")]
		public virtual Boolean? Closeflg
        {
            get
            {
                return this._Closeflg;
            }
            set
            {
                this._Closeflg = value;
            }
        }
        #endregion
        #region StartTime

        public abstract class startTime : PX.Data.BQL.BqlDateTime.Field<startTime> { }

        protected DateTime? _StartTime;

		/// <summary>
		/// The selected employee’s clock-in time for the production operation.
		/// </summary>
		[PXDBDateAndTime(UseTimeZone = true, DisplayNameDate = "Start Date", DisplayNameTime = "Clock-In Start Time")]
        [PXUIField(DisplayName = "Start Time", Required = true, Enabled = false)]
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
		[PXDBDateAndTime(UseTimeZone = true, DisplayNameDate = "End Date", DisplayNameTime = "Clock-In End Time")]
        [PXUIField(DisplayName = "End Time", Required = true, Enabled = false)]
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
		/// The employee clock-in time that is automatically calculated for the production order operation.
		/// </summary>
		[PXDBTimeSpanLong(Format = TimeSpanFormatType.LongHoursMinutes)]
        [PXUIField(DisplayName = "Labor Time", Enabled = false)]
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
		/// The last operation.
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
		/// A lot and serial counter.
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
		/// The lot or serial number.
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
		/// The shift in which the employee worked when processing the items.
		/// </summary>
		[ShiftCDField]
        [ShiftCodeSelector]
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
		[PXDefault(Messages.ClockLine, PersistingCheck = PXPersistingCheck.Nothing)]
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
        [ProjectDefault(BatchModule.IN,typeof(Search<AMProdItem.projectID, Where<AMProdItem.orderType, Equal<Current<orderType>>, 
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
		/// The task.
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
		#region IsStockItem
		/// <summary>
		/// A Boolean value that indicates whether the clock transaction is a stock item.
		/// </summary>
		[PXUnboundDefault(true)]
		[PXBool]
		[PXUIField(DisplayName = "Is stock", Enabled = false, Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual bool? IsStockItem { get; set; }

		#endregion
		#region Status
		/// <summary>
		/// The status of the clock transaction.
		/// </summary>
		[PXDefault(typeof(ClockTranStatus.newStatus))]
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		[ClockTranStatus.List]
		public virtual string Status { get; set; }
		public abstract class status : PX.Data.BQL.BqlString.Field<status> { }

		#endregion
		#region IsLotSerialPreassigned
		/// <summary>
		/// A Boolean value that indicates whether the clock transaction has preassigned lot or serial number.
		/// </summary>
		[PXDBBool]
		[PXDefault(typeof(Search<
		  AMProdItem.preassignLotSerial,
		  Where<AMProdItem.orderType, Equal<Current<AMClockTran.orderType>>,
			  And<AMProdItem.prodOrdID, Equal<Current<AMClockTran.prodOrdID>>>>>))]
		[PXUIField(DisplayName = "Lot/Serial Nbr. Preassigned", Enabled = false, FieldClass = "Lot/Serial")]
		[PXFormula(typeof(Default<AMClockTran.prodOrdID>))]
		public virtual bool? IsLotSerialPreassigned { get; set; }
		public abstract class isLotSerialPreassigned : PX.Data.BQL.BqlBool.Field<isLotSerialPreassigned> { }

		#endregion
		#region BaseQtyScrapped
		/// <summary>
		/// The base quantity that is scrapped.
		/// </summary>
		[PXDBQuantity]
		[PXUIField(DisplayName = "Base Qty Scrapped")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? BaseQtyScrapped { get; set; }
		public abstract class baseQtyScrapped : PX.Data.BQL.BqlDecimal.Field<baseQtyScrapped> { }

		#endregion
		#region QtyScrapped
		/// <summary>
		/// The scrapped quantity.
		/// </summary>
		[PXDBQuantity(typeof(AMClockTran.uOM), typeof(AMClockTran.baseQtyScrapped), HandleEmptyKey = true)]
		[PXUIField(DisplayName = "Qty Scrapped")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? QtyScrapped { get; set; }
		public abstract class qtyScrapped : PX.Data.BQL.BqlDecimal.Field<qtyScrapped> { }

		#endregion
		#region ReasonCodeID
		/// <summary>
		/// The reason code.
		/// </summary>
		[PXDBString(20, InputMask = "aaaaaaaaaaaaaaaaaaaa")]
		[PXUIField(DisplayName = "Reason Code")]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID, Where<ReasonCode.usage, Equal<ReasonCodeExt.production>>>))]
		public virtual string ReasonCodeID { get; set; }
		public abstract class reasonCodeID : PX.Data.BQL.BqlString.Field<reasonCodeID> { }

		#endregion
		#region ScrapAction
		/// <summary>
		/// Scrap action.
		/// </summary>
		[PXDBInt]
		[ClockTranScrapActionDefaultAttribute.ClockTranList]
		[PXUIField(DisplayName = "Scrap Action", Enabled = false)]
		[ClockTranScrapActionDefault(typeof(Search<AMProdOper.scrapAction,
		 Where<AMProdOper.orderType, Equal<Current<AMClockTran.orderType>>,
			 And<AMProdOper.prodOrdID, Equal<Current<AMClockTran.prodOrdID>>,
			 And<AMProdOper.operationID, Equal<Current<AMClockTran.operationID>>>>>>))]
		[PXFormula(typeof(Default<AMClockTran.operationID>))]

		public virtual int? ScrapAction { get; set; }
		public abstract class scrapAction : PX.Data.BQL.BqlInt.Field<scrapAction> { }

		#endregion
		#region FinPeriodID
		/// <summary>
		/// The financial period.
		/// </summary>
		[OpenPeriod(null, typeof(tranDate), typeof(branchID))]
		[PXUIField(DisplayName = "Post Period", Enabled = false)]
		public virtual string FinPeriodID { get; set; }
		public abstract class finPeriodID : PX.Data.BQL.BqlInt.Field<finPeriodID> { }


		#endregion

		#region WcID
		/// <summary>
		/// The work center.
		/// </summary>
		[WorkCenterIDField(Enabled = false)]
		[PXUIField(DisplayName = "Work Center", Enabled = false)]
		[PXDefault(typeof(Search<AMProdOper.wcID,
		  Where<AMProdOper.orderType, Equal<Current<AMClockTran.orderType>>,
			  And<AMProdOper.prodOrdID, Equal<Current<AMClockTran.prodOrdID>>,
			  And<AMProdOper.operationID, Equal<Current<AMClockTran.operationID>>,
			  And<AMProdOper.operationID, IsNotNull>>>>>))]

		[PXFormula(typeof(Default<AMClockTran.operationID>))]
		[PXReferentialIntegrityCheck]
		public virtual string WcID { get; set; }
		public abstract class wcID : PX.Data.BQL.BqlString.Field<wcID> { }

		#endregion
		#region AllowMultiClockEntry
		/// <summary>
		/// A Boolean value that indicates whether multiple clock entries are allowed.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Multiple Entries Allowed", Enabled = false, Visible = false)]
		[PXDefault(typeof(Search<AMWC.allowMultiClockEntry,
		  Where<AMWC.wcID, Equal<Current<wcID>>>>))]
		[PXFormula(typeof(Default<wcID>))]
		public virtual bool? AllowMultiClockEntry { get; set; }
		public abstract class allowMultiClockEntry : PX.Data.BQL.BqlBool.Field<allowMultiClockEntry> { }
		protected bool? _AllowMultiClockEntry;

		#endregion

		#region LaborTimeSeconds
		/// <summary>
		/// The labor time in seconds.
		/// </summary>
		[PXDBInt]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Labor Time Seconds", Enabled = false, Visible = false)]
		public virtual int? LaborTimeSeconds { get; set; }
		public abstract class laborTimeSeconds : PX.Data.BQL.BqlInt.Field<laborTimeSeconds> { }
		#endregion
		#region Duration
		public abstract class duration : PX.Data.BQL.BqlInt.Field<duration> { }

		/// <summary>
		/// The duration.
		/// </summary>
		[PXUIField(DisplayName = "Duration")]
		[PXTimeSpanLong(Format = TimeSpanFormatType.LongHoursMinutes)]
		[PXFormula(typeof(
			  DateDiff<AMClockTran.startTime, IsNull<AMClockTran.endTime, TimeZoneNow>, DateDiff.minute>))]
		public virtual int? Duration { get; set; }
		#endregion

		#region Methods

		public static implicit operator AMClockTranSplit(AMClockTran item)
        {
            AMClockTranSplit ret = new AMClockTranSplit();
            ret.TranType = item.TranType;
            ret.LineNbr = item.LineNbr;
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

        public static implicit operator AMClockTran(AMClockTranSplit item)
        {
            AMClockTran ret = new AMClockTran();
            ret.TranType = item.TranType;
            ret.LineNbr = item.LineNbr;
            ret.InventoryID = item.InventoryID;
            ret.SiteID = item.SiteID;
            ret.SubItemID = item.SubItemID;
            ret.LocationID = item.LocationID;
            ret.LotSerialNbr = item.LotSerialNbr;

            //Split recs show a positive qty - AMClockTran shows a positive or negative qty
            ret.Qty = Math.Abs(item.Qty.GetValueOrDefault()) * (item.InvtMult * -1);
            ret.BaseQty = Math.Abs(item.BaseQty.GetValueOrDefault()) * (item.InvtMult * -1);

            ret.UOM = item.UOM;
            ret.TranDate = item.TranDate;
            ret.InvtMult = item.InvtMult;

            return ret;
        }


        #endregion

		bool? ILSMaster.IsIntercompany => false;
		int? ILSPrimary.CostCenterID => CostCenter.FreeStock;
    }
}

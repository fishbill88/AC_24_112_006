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
using PX.Objects.PO;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.CR;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AM.Attributes;
using PX.Objects.IN;
using PX.Data.BQL.Fluent;
using PX.Objects.CM;

namespace PX.Objects.AM
{
    /// <summary>
    /// A BOM operation.
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.BOMOper)]
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class AMBomOper : PXBqlTable, IBqlTable, IOperationMaster, IBomOper, INotable
    {
        internal string DebuggerDisplay => $"BOMID = {BOMID}, RevisionID = {RevisionID}, OperationCD = {OperationCD} ({OperationID}), WcID = {WcID}";

        #region Keys

        public class PK : PrimaryKeyOf<AMBomOper>.By<bOMID, revisionID, operationID>
        {
            public static AMBomOper Find(PXGraph graph, string bOMID, string revisionID, int? operationID, PKFindOptions options = PKFindOptions.None)
                => FindBy(graph, bOMID, revisionID, operationID, options);
            public static AMBomOper FindDirty(PXGraph graph, string bOMID, string revisionID, int? operationID)
                => PXSelect<AMBomOper,
                    Where<bOMID, Equal<Required<bOMID>>,
                        And<revisionID, Equal<Required<revisionID>>,
                        And<operationID, Equal<Required<operationID>>>>>>
                    .SelectWindowed(graph, 0, 1, bOMID, revisionID, operationID);
        }

        public class UK : PrimaryKeyOf<AMBomOper>.By<bOMID, revisionID, operationCD>
        {
            public static AMBomOper Find(PXGraph graph, string bOMID, string revisionID, string operationCD, PKFindOptions options = PKFindOptions.None) => FindBy(graph, bOMID, revisionID, operationCD, options);
        }

        public static class FK
        {
            public class BOM : AMBomItem.PK.ForeignKeyOf<AMBomOper>.By<bOMID, revisionID> { }
            public class Workcenter : AMWC.PK.ForeignKeyOf<AMBomOper>.By<wcID> { }
        }

        #endregion

        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }

        protected bool? _Selected = false;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected", Enabled = true)]
        public virtual bool? Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }
        #endregion
        #region BOMID
        public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }

        protected string _BOMID;

		/// <summary>
		/// The identifier of the bill of material.
		/// </summary>
		[BomID(IsKey = true, Visible = false, Enabled = false)]
        [BOMIDSelector(ValidateValue = false)]
        [PXDBDefault(typeof(AMBomItem.bOMID))]
        [PXParent(typeof(Select<AMBomItem, Where<AMBomItem.bOMID, Equal<Current<AMBomOper.bOMID>>,
            And<AMBomItem.revisionID, Equal<Current<AMBomOper.revisionID>>>>>))]
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
        [PXDBDefault(typeof(AMBomItem.revisionID))]
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
		/// The numeric identifier of the operation, which determines the sequence in which the operation is executed in production orders.
		/// </summary>
		[OperationIDField( Visible = false, Enabled = false)]
        [PXLineNbr(typeof(AMBomItem.lineCntrOperation))]
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
        #region OperationCD
        public abstract class operationCD : PX.Data.BQL.BqlString.Field<operationCD> { }

        protected string _OperationCD;

		/// <summary>
		/// The numeric identifier of the operation, which is displayed in the operation.
		/// </summary>
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [OperationCDField(Visibility = PXUIVisibility.SelectorVisible, IsKey=true)]
        [PXCheckUnique(typeof(AMBomOper.bOMID), typeof(AMBomOper.revisionID))]
        public virtual string OperationCD
        {
            get { return this._OperationCD; }
            set { this._OperationCD = value; }
        }

        #endregion
        #region Descr
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }

        protected String _Descr;

		/// <summary>
		/// A description of the operation.
		/// </summary>
		[PXDBString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Oper Desc", Visibility = PXUIVisibility.SelectorVisible)]
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
        #region WcID
        public abstract class wcID : PX.Data.BQL.BqlString.Field<wcID> { }

        protected String _WcID;

		/// <summary>
		/// The active work center where the operation takes place.
		/// </summary>
		[WorkCenterIDField(Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault(typeof(Search<AMBSetup.wcID>))]
        [PXSelector(typeof(Search<AMWC.wcID>),DescriptionField = typeof(AMWC.descr))]
        [PXForeignReference(typeof(Field<AMBomOper.wcID>.IsRelatedTo<AMWC.wcID>))]
        [PXRestrictor(typeof(Where<AMWC.activeFlg, Equal<True>>), Messages.WorkCenterNotActive)]
        public virtual String WcID
        {
            get
            {
                return this._WcID;
            }
            set
            {
                this._WcID = value;
            }
        }
        #endregion
        #region SetupTime
        public abstract class setupTime : PX.Data.BQL.BqlInt.Field<setupTime> { }

		/// <summary>
		/// The time it takes to prepare to start the operation.
		/// </summary>
		[OperationDBTime]
        [PXDefault(TypeCode.Int32, "0")]
        [PXUIField(DisplayName = "Setup Time")]
        public virtual Int32? SetupTime { get; set; }
        #endregion
        #region RunUnitTime
        public abstract class runUnitTime : PX.Data.BQL.BqlInt.Field<runUnitTime> { }

		/// <summary>
		/// The time required to produce the specified run units of the operation.
		/// </summary>
		[OperationDBTime]
        [PXDefault(TypeCode.Int32, "60")]
        [PXUIField(DisplayName = "Run Time")]
        public virtual Int32? RunUnitTime { get; set; }
        #endregion
        #region RunUnits
        public abstract class runUnits : PX.Data.BQL.BqlDecimal.Field<runUnits> { }

        protected Decimal? _RunUnits;

		/// <summary>
		/// The number of units produced during the specified run time for the operation.
		/// </summary>
		[PXDBQuantity(MinValue = 0.0)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Run Units")]
        public virtual Decimal? RunUnits
        {
            get
            {
                return this._RunUnits;
            }
            set
            {
                this._RunUnits = value;
            }
        }
        #endregion
        #region MachineUnitTime
        public abstract class machineUnitTime : PX.Data.BQL.BqlInt.Field<machineUnitTime> { }

		/// <summary>
		/// The time required to produce the number of machine units specified for the operation.
		/// </summary>
		[OperationDBTime]
        [PXDefault(TypeCode.Int32, "60")]
        [PXUIField(DisplayName = "Machine Time")]
        public virtual Int32? MachineUnitTime { get; set; }
        #endregion
        #region MachineUnits
        public abstract class machineUnits : PX.Data.BQL.BqlDecimal.Field<machineUnits> { }

        protected Decimal? _MachineUnits;

		/// <summary>
		/// The number of units produced during the specified machine time for the operation.
		/// </summary>
		[PXDBQuantity(MinValue = 0.0)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Machine Units")]
        public virtual Decimal? MachineUnits
        {
            get
            {
                return this._MachineUnits;
            }
            set
            {
                this._MachineUnits = value;
            }
        }
        #endregion
        #region QueueTime
        public abstract class queueTime : PX.Data.BQL.BqlInt.Field<queueTime> { }

		/// <summary>
		/// The time a semi-finished item has to wait in the work center before workers can start processing the item.
		/// </summary>
		[OperationDBTime]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Queue Time")]
		[PXFormula(typeof(Selector<AMBomOper.wcID, AMWC.defaultQueueTime>))]
        public virtual Int32? QueueTime { get; set; }
        #endregion
        #region FinishTime
        public abstract class finishTime : PX.Data.BQL.BqlInt.Field<finishTime> { }

		/// <summary>
		/// The time required for the semi-finished item to be prepared for the next operation when the current operation has been finished.
		/// </summary>
		[OperationDBTime]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Finish Time")]
		[PXFormula(typeof(Selector<AMBomOper.wcID, AMWC.defaultFinishTime>))]
		public virtual Int32? FinishTime { get; set; }
        #endregion
        #region BFlush
        public abstract class bFlush : PX.Data.BQL.BqlBool.Field<bFlush> { }

        protected Boolean? _BFlush;

		/// <summary>
		/// A Boolean value that indicates whether a labor transaction is needed to report labor hours spent for the operation.
		/// </summary>
		[PXDBBool]
        [PXDefault(false, typeof(Search<AMWC.bflushLbr, Where<AMWC.wcID, Equal<Current<AMBomOper.wcID>>>>))]
        [PXUIField(DisplayName = "Backflush Labor")]
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
        #region LineCntrMatl
        public abstract class lineCntrMatl : PX.Data.BQL.BqlInt.Field<lineCntrMatl> { }

        protected Int32? _LineCntrMatl;

		/// <summary>
		/// The counter of material lines.
		/// </summary>
		[PXDBInt]
        [PXDefault(0)]
        public virtual Int32? LineCntrMatl
        {
            get
            {
                return this._LineCntrMatl;
            }
            set
            {
                this._LineCntrMatl = value;
            }
        }
        #endregion
        #region LineCntrOvhd
        public abstract class lineCntrOvhd : PX.Data.BQL.BqlInt.Field<lineCntrOvhd> { }

        protected Int32? _LineCntrOvhd;

		/// <summary>
		/// The counter of overhead lines.
		/// </summary>
		[PXDBInt]
        [PXDefault(0)]
        public virtual Int32? LineCntrOvhd
        {
            get
            {
                return this._LineCntrOvhd;
            }
            set
            {
                this._LineCntrOvhd = value;
            }
        }
        #endregion
        #region LineCntrStep
        public abstract class lineCntrStep : PX.Data.BQL.BqlInt.Field<lineCntrStep> { }

        protected Int32? _LineCntrStep;

		/// <summary>
		/// The counter of step lines.
		/// </summary>
		[PXDBInt]
        [PXDefault(0)]
        public virtual Int32? LineCntrStep
        {
            get
            {
                return this._LineCntrStep;
            }
            set
            {
                this._LineCntrStep = value;
            }
        }
        #endregion
        #region LineCntrTool
        public abstract class lineCntrTool : PX.Data.BQL.BqlInt.Field<lineCntrTool> { }

        protected Int32? _LineCntrTool;

		/// <summary>
		/// The counter of tool lines.
		/// </summary>
		[PXDBInt]
        [PXDefault(0)]
        public virtual Int32? LineCntrTool
        {
            get
            {
                return this._LineCntrTool;
            }
            set
            {
                this._LineCntrTool = value;
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
        #region ScrapAction
        public abstract class scrapAction : PX.Data.BQL.BqlInt.Field<scrapAction> { }

        protected int? _ScrapAction;

		/// <summary>
		/// The default scrap action for the operation in new production orders.
		/// </summary>
		[PXDBInt]
        [PXDefault(Attributes.ScrapAction.NoAction, typeof(Search<AMWC.scrapAction, Where<AMWC.wcID,
            Equal<Current<AMBomOper.wcID>>>>))]
        [PXUIField(DisplayName = "Scrap Action")]
        [ScrapAction.List]
        public virtual int? ScrapAction
        {
            get
            {
                return this._ScrapAction;
            }
            set
            {
                this._ScrapAction = value;
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
		#region MoveTime
		public abstract class moveTime : PX.Data.BQL.BqlInt.Field<moveTime> { }

		/// <summary>
		/// The time for a semi-finished item to be moved from the work center where the current operation is performed to the work center where the next operation will be performed.
		/// </summary>
		[OperationDBTime]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Move Time")]
		[PXFormula(typeof(Selector<AMBomOper.wcID, AMWC.defaultMoveTime>))]
		public virtual Int32? MoveTime { get; set; }
		#endregion
		#region ControlPoint
		public abstract class controlPoint : PX.Data.BQL.BqlInt.Field<controlPoint> { }

		protected bool? _ControlPoint;

		/// <summary>
		/// A Boolean value that indicates whether the operation is a control point.
		/// </summary>
		[ControlPoint] 
        [PXFormula(typeof(Selector<AMBomOper.wcID, AMWC.controlPoint>))]
		public virtual bool? ControlPoint
		{
			get
			{
				return this._ControlPoint;
			}
			set
			{
				this._ControlPoint = value;
			}
		}
		#endregion

		#region Raw Times

		#region SetupTimeRaw
		public abstract class setupTimeRaw : PX.Data.BQL.BqlInt.Field<setupTimeRaw> { }

		/// <summary>
		/// The setup time.
		/// </summary>
		[RawTimeField(typeof(setupTime))]
		[PXDependsOnFields(typeof(setupTime))]
		public virtual Int32? SetupTimeRaw
		{
			get
			{
				return this.SetupTime;
			}
			set
			{
				this.SetupTime = value;
			}
		}
		#endregion
		#region RunUnitTimeRaw
		public abstract class runUnitTimeRaw : PX.Data.BQL.BqlInt.Field<runUnitTimeRaw> { }

		/// <summary>
		/// The run unit time.
		/// </summary>
		[RawTimeField(typeof(runUnitTime))]
		[PXDependsOnFields(typeof(runUnitTime))]
		public virtual Int32? RunUnitTimeRaw
		{
			get
			{
				return this.RunUnitTime;
			}
			set
			{
				this.RunUnitTime = value;
			}
		}
		#endregion
		#region MachineUnitTimeRaw
		public abstract class machineUnitTimeRaw : PX.Data.BQL.BqlInt.Field<machineUnitTimeRaw> { }

		/// <summary>
		/// The machine unit time.
		/// </summary>
		[RawTimeField(typeof(machineUnitTime))]
		[PXDependsOnFields(typeof(machineUnitTime))]
		public virtual Int32? MachineUnitTimeRaw
		{
			get
			{
				return this.MachineUnitTime;
			}
			set
			{
				this.MachineUnitTime = value;
			}
		}
		#endregion
		#region QueueTimeRaw
		public abstract class queueTimeRaw : PX.Data.BQL.BqlInt.Field<queueTimeRaw> { }

		/// <summary>
		/// The queue time.
		/// </summary>
		[RawTimeField(typeof(queueTime))]
		[PXDependsOnFields(typeof(queueTime))]
		public virtual Int32? QueueTimeRaw
		{
			get
			{
				return this.QueueTime;
			}
			set
			{
				this.QueueTime = value;
			}
		}
		#endregion
		#region FinishTimeRaw
		public abstract class finishTimeRaw : PX.Data.BQL.BqlInt.Field<finishTimeRaw> { }

		/// <summary>
		/// The finish time.
		/// </summary>
		[RawTimeField(typeof(finishTime))]
		[PXDependsOnFields(typeof(finishTime))]
		public virtual Int32? FinishTimeRaw
		{
			get
			{
				return this.FinishTime;
			}
			set
			{
				this.FinishTime = value;
			}
		}
		#endregion
		#region MoveTimeRaw
		public abstract class moveTimeRaw : PX.Data.BQL.BqlInt.Field<moveTimeRaw> { }

		/// <summary>
		/// The move time.
		/// </summary>
		[RawTimeField(typeof(moveTime))]
		[PXDependsOnFields(typeof(moveTime))]
		public virtual Int32? MoveTimeRaw
		{
			get
			{
				return this.MoveTime;
			}
			set
			{
				this.MoveTime = value;
			}
		}
		#endregion

		#endregion

		#region OutsideProcess
		public abstract class outsideProcess : PX.Data.BQL.BqlBool.Field<outsideProcess> { }

        protected Boolean? _OutsideProcess;

		/// <summary>
		/// The outside process.
		/// </summary>
		[PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Outside Process")]
        public virtual Boolean? OutsideProcess
        {
            get
            {
                return this._OutsideProcess;
            }
            set
            {
                this._OutsideProcess = value;
            }
        }
        #endregion
        #region DropShippedToVendor
        public abstract class dropShippedToVendor : PX.Data.BQL.BqlBool.Field<dropShippedToVendor> { }

        protected Boolean? _DropShippedToVendor;

		/// <summary>
		/// A Boolean value that indicates whether the operation has been drop shipped to the vendor.
		/// </summary>
		[PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Drop Shipped to Vendor")]
        public virtual Boolean? DropShippedToVendor
        {
            get
            {
                return this._DropShippedToVendor;
            }
            set
            {
                this._DropShippedToVendor = value;
            }
        }
        #endregion
        #region VendorID

        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        protected Int32? _VendorID;

		/// <summary>
		/// The vendor.
		/// </summary>
		[POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
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

		/// <summary>
		/// The vendor location.
		/// </summary>
		[LocationActive(typeof(Where<Location.bAccountID, Equal<Current<AMBomOper.vendorID>>,
            And<MatchWithBranch<Location.vBranchID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible,
            DisplayName = "Vendor Location")]
        [PXDefault(typeof(Coalesce<Search2<BAccountR.defLocationID,
            InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>,
            Where<BAccountR.bAccountID, Equal<Current<AMBomOper.vendorID>>,
                And<CRLocation.isActive, Equal<True>,
                And<MatchWithBranch<CRLocation.vBranchID>>>>>,
            Search<CRLocation.locationID,
            Where<CRLocation.bAccountID, Equal<Current<AMBomOper.vendorID>>,
            And<CRLocation.isActive, Equal<True>, And<MatchWithBranch<CRLocation.vBranchID>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<AMBomOper.vendorID>))]
        [PXForeignReference(typeof(Field<vendorLocationID>.IsRelatedTo<Location.locationID>))]
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

		#region NewOperationCD (Unbound)
		public abstract class newOperationCD : PX.Data.BQL.BqlString.Field<newOperationCD> { }

		/// <summary>
		/// The new operation.
		/// </summary>
		[PXString(OperationCDFieldAttribute.OperationFieldLength, IsUnicode = true)]
		[PXUIField(DisplayName = "New Operation ID", Visibility = PXUIVisibility.Invisible)]
		public virtual string NewOperationCD { get; set; }

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
	/// A projection over the <see cref="AMBOMCurySettings"/> class for the currency cost data of the <see cref="AMBomOper"/> class.
	/// </summary>
	[Serializable]
	[PXCacheName("AMBomOperCurrency")]
	[PXProjection(typeof(SelectFrom<AMBOMCurySettings>
		.Where<AMBOMCurySettings.lineType.IsEqual<BOMCurySettingsLineType.operation>>), Persistent = true)]
	public class AMBomOperCury : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<AMBomOperCury>.By<bOMID, revisionID, operationID, curyID>
		{
			public static AMBomOperCury Find(PXGraph graph, string bOMID, string revisionID, int? operationID, string curyID, PKFindOptions options = PKFindOptions.None) =>
				FindBy(graph, bOMID, revisionID, operationID, curyID, options);
		}
		public static class FK
		{
			public class BomOper : AMBomOper.PK.ForeignKeyOf<AMBomOperCury>.By<bOMID, revisionID, operationID> { }
		}
		#endregion

		#region BOMID
		public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }
		protected String _BOMID;

		/// <inheritdoc cref="AMBOMCurySettings.BOMID"/>
		[BomID(IsKey = true, Enabled = false, BqlField = typeof(AMBOMCurySettings.bOMID))]
		[PXDBDefault(typeof(AMBomOper.bOMID))]
		[PXParent(typeof(FK.BomOper))]
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
		[PXDefault(typeof(AMBomOper.revisionID))]
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
		[PXDBDefault(typeof(AMBomOper.operationID))]
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
		[PXDefault(0)]
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
		[PXDefault(BOMCurySettingsLineType.Operation)]
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
		[POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, BqlField = typeof(AMBOMCurySettings.vendorID))]
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
		[LocationActive(typeof(Where<Location.bAccountID, Equal<Current<AMBomOperCury.vendorID>>,
			And<MatchWithBranch<Location.vBranchID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible,
			DisplayName = "Vendor Location", BqlField = typeof(AMBOMCurySettings.vendorLocationID))]
		[PXDefault(typeof(Coalesce<Search2<BAccountR.defLocationID,
			InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>,
			Where<BAccountR.bAccountID, Equal<Current<AMBomOperCury.vendorID>>,
				And<CRLocation.isActive, Equal<True>,
				And<MatchWithBranch<CRLocation.vBranchID>>>>>,
			Search<CRLocation.locationID,
			Where<CRLocation.bAccountID, Equal<Current<AMBomOper.vendorID>>,
			And<CRLocation.isActive, Equal<True>, And<MatchWithBranch<CRLocation.vBranchID>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<AMBomOperCury.vendorID>))]
		[PXForeignReference(typeof(Field<vendorLocationID>.IsRelatedTo<Location.locationID>))]
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

    [Serializable]
    [PXHidden]
    [PXProjection(typeof(Select<AMBomOper>))]
    public class AMBomOperSimple : PXBqlTable, IBqlTable, IBomOper
    {
        #region BOMID
        public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }

        [BomID(IsKey = true, Visible = false, Enabled = false, BqlField = typeof(AMBomOper.bOMID))]
        public virtual string BOMID { get; set; }
        #endregion
        #region RevisionID
        public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }

        [RevisionIDField(IsKey = true, Visibility = PXUIVisibility.SelectorVisible, Visible = false, Enabled = false, BqlField = typeof(AMBomOper.revisionID))]
        public virtual string RevisionID { get; set; }

        #endregion
        #region OperationID
        public abstract class operationID : PX.Data.BQL.BqlInt.Field<operationID> { }

        protected int? _OperationID;
        [OperationIDField(IsKey = true, Visible = false, Enabled = false, BqlField = typeof(AMBomOper.operationID))]
        public virtual int? OperationID { get; set; }
        #endregion
        #region OperationCD
        public abstract class operationCD : PX.Data.BQL.BqlString.Field<operationCD> { }

        protected string _OperationCD;
        [OperationCDField(Visibility = PXUIVisibility.SelectorVisible, BqlField = typeof(AMBomOper.operationCD))]
        public virtual string OperationCD { get; set; }
        #endregion
    }
}

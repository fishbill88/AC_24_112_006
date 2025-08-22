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
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.AM.Attributes;

namespace PX.Objects.AM
{
    /// <summary>
    /// The manufacturing bill of material (BOM) preferences.
    /// </summary>
    [PXPrimaryGraph(typeof(BOMSetup))]
    [PXCacheName(Messages.BOMSetup)]
    [Serializable]
    public class AMBSetup : PXBqlTable, IBqlTable
    {
        #region BOMNumberingID

        public abstract class bOMNumberingID : PX.Data.BQL.BqlString.Field<bOMNumberingID> { }

        protected String _BOMNumberingID;

		/// <summary>
		/// The numbering sequence the system uses for assigning reference numbers to bills of material.
		/// </summary>
		[PXDBString(10, IsUnicode = true)]
        [PXDefault]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "BOM Numbering Sequence", Visibility = PXUIVisibility.Visible)]
        public virtual String BOMNumberingID
        {
            get
            {
                return this._BOMNumberingID;
            }
            set
            {
                this._BOMNumberingID = value;
            }
        }
		#endregion
		#region DuplicateItemOnBOM
		public abstract class duplicateItemOnBOM : PX.Data.BQL.BqlString.Field<duplicateItemOnBOM> { }

		/// <summary>
		/// The option that controls the use of duplicate inventory items as materials in all operations of bills of material.
		/// (Previously DupInvBOM)
		/// </summary>
		[PXDBString(1)]
		[PXDefault(SetupMessage.AllowMsg)]
		[PXUIField(DisplayName = "Duplicates on BOM")]
        [SetupMessage.List]
		public virtual string DuplicateItemOnBOM { get; set; }
		#endregion
		#region DuplicateItemOnOper
		public abstract class duplicateItemOnOper : PX.Data.BQL.BqlString.Field<duplicateItemOnOper> { }

		/// <summary>
		/// The option that controls the use of duplicate inventory items as materials added to each operation of a bill of material.
		/// (Previously DupInvOper)
		/// </summary>
		[PXDBString(1)]
		[PXDefault(SetupMessage.AllowMsg)]
		[PXUIField(DisplayName = "Duplicates on Operation")]
        [SetupMessage.List]
		public virtual string DuplicateItemOnOper { get; set; }
		#endregion
		#region AllowEmptyBOMSubItemID
		public abstract class allowEmptyBOMSubItemID : PX.Data.BQL.BqlInt.Field<allowEmptyBOMSubItemID> { }

        protected Boolean? _AllowEmptyBOMSubItemID;
		/// <summary>
		/// A Boolean value that indicates whether the BOM can be created without a subitem ID.
		/// </summary>
		[PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Allow Empty BOM Item Sub Item ID", FieldClass = "INSUBITEM")]
        public virtual Boolean? AllowEmptyBOMSubItemID
        {
            get
            {
                return this._AllowEmptyBOMSubItemID;
            }
            set
            {
                this._AllowEmptyBOMSubItemID = value;
            }
        }
        #endregion
        #region LastLowLevelCompletedDateTime
        /// <summary>
        /// The last date when the low-level values were calculated.
        /// </summary>
        public abstract class lastLowLevelCompletedDateTime : PX.Data.BQL.BqlDateTime.Field<lastLowLevelCompletedDateTime> { }

        /// <summary>
        /// The last date when the low-level values were calculated.
        /// </summary>
        [PXDBDateAndTime]
        [PXUIField(DisplayName = "Last Low Level Completed At", Enabled = false, IsReadOnly = true, Visible = false)]
        public virtual DateTime? LastLowLevelCompletedDateTime { get; set; }
        
        #endregion
        #region LastMaxLowLevel
        /// <summary>
        /// The maximum low level that was calculated from the last completed low-level process.
        /// </summary>
        public abstract class lastMaxLowLevel : PX.Data.BQL.BqlInt.Field<lastMaxLowLevel> { }

        /// <summary>
        /// The maximum low level that was calculated from the last completed low-level process.
        /// </summary>
        [PXDBInt]
        [PXUIField(DisplayName = "Last Max Low Level", Enabled = false, IsReadOnly = true, Visible = false)]
        public virtual int? LastMaxLowLevel { get; set; }
        
        #endregion
        #region WcID  (Default Work Center)
        public abstract class wcID : PX.Data.BQL.BqlString.Field<wcID> { }

		protected String _WcID;

		/// <summary>
		/// The default work center that is specified for each operation that you add to a bill of material.
		/// </summary>
		[WorkCenterIDField(DisplayName = "Default Work Center")]
        [PXSelector(typeof(Search<AMWC.wcID>))]
        [PXForeignReference(typeof(Field<AMBSetup.wcID>.IsRelatedTo<AMWC.wcID>))]
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

        #region OperationTimeFormat
        public abstract class operationTimeFormat : PX.Data.BQL.BqlInt.Field<operationTimeFormat> { }

        protected int? _OperationTimeFormat;

		/// <summary>
		/// The format that is used in the columns with time settings in the operations.
		/// </summary>
		[PXDBInt]
        [PXDefault(AMTimeFormatAttribute.TimeSpanFormat.ShortHoursMinutesCompact)]
        [PXUIField(DisplayName = "Operation Time Format")]
        [AMTimeFormatAttribute.TimeSpanFormat.List]
        public virtual int? OperationTimeFormat
        {
            get
            {
                return this._OperationTimeFormat;
            }
            set
            {
                this._OperationTimeFormat = value;
            }
        }
        #endregion
        #region ProductionTimeFormat
        public abstract class productionTimeFormat : PX.Data.BQL.BqlInt.Field<productionTimeFormat> { }

        protected int? _ProductionTimeFormat;

		/// <summary>
		/// The time format that is used for total time values.
		/// </summary>
		[PXDBInt]
        [PXDefault(AMTimeFormatAttribute.TimeSpanFormat.LongHoursMinutes)]
        [PXUIField(DisplayName = "Total Time Format")]
        [AMTimeFormatAttribute.TimeSpanFormat.List]
        public virtual int? ProductionTimeFormat
        {
            get
            {
                return this._ProductionTimeFormat;
            }
            set
            {
                this._ProductionTimeFormat = value;
            }
        }
        #endregion

        #region DefaultRevisionID
        public abstract class defaultRevisionID : PX.Data.BQL.BqlString.Field<defaultRevisionID> { }

        protected String _DefaultRevisionID;

		/// <summary>
		/// The default identifier of a revision for new bills of material, which is an alphanumeric string.
		/// </summary>
		[PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCC")]
        [PXUIField(DisplayName = "Default Revision", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault("", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual String DefaultRevisionID
        {
            get
            {
                return this._DefaultRevisionID;
            }
            set
            {
                this._DefaultRevisionID = value;
            }
        }
        #endregion
        #region ECRNumberingID

        public abstract class eCRNumberingID : PX.Data.BQL.BqlString.Field<eCRNumberingID> { }
        protected String _ECRNumberingID;

		/// <summary>
		/// The numbering sequence the system uses for assigning reference numbers to engineering change requests (ECRs).
		/// </summary>
		[PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "ECR Numbering Sequence", FieldClass = Features.ECCFIELDCLASS)]
        public virtual String ECRNumberingID
        {
            get
            {
                return this._ECRNumberingID;
            }
            set
            {
                this._ECRNumberingID = value;
            }
        }
        #endregion
        #region ECONumberingID

        public abstract class eCONumberingID : PX.Data.BQL.BqlString.Field<eCONumberingID> { }
        protected String _ECONumberingID;

		/// <summary>
		/// The numbering sequence the system uses for assigning reference numbers to engineering change orders (ECOs).
		/// </summary>
		[PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "ECO Numbering Sequence", FieldClass = Features.ECCFIELDCLASS)]
        public virtual String ECONumberingID
        {
            get
            {
                return this._ECONumberingID;
            }
            set
            {
                this._ECONumberingID = value;
            }
        }
        #endregion

        #region ECRRequestApproval
        public abstract class eCRRequestApproval : PX.Data.BQL.BqlBool.Field<eCRRequestApproval> { }
        protected bool? _ECRRequestApproval;

		/// <summary>
		/// A Boolean value that indicates whether the ECR needs to be approved.
		/// </summary>
		[EPRequireApproval]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
        [PXUIField(DisplayName = "ECR Require Approval")]
        public virtual bool? ECRRequestApproval
        {
            get
            {
                return this._ECRRequestApproval;
            }
            set
            {
                this._ECRRequestApproval = value;
            }
        }
        #endregion	
        #region ECORequestApproval
        public abstract class eCORequestApproval : PX.Data.BQL.BqlBool.Field<eCORequestApproval> { }
        protected bool? _ECORequestApproval;

		/// <summary>
		/// A Boolean value that indicates whether the ECO needs to be approved.
		/// </summary>
		[EPRequireApproval]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
        [PXUIField(DisplayName = "ECO Require Approval")]
        public virtual bool? ECORequestApproval
        {
            get
            {
                return this._ECORequestApproval;
            }
            set
            {
                this._ECORequestApproval = value;
            }
        }
        #endregion
        #region ForceECR
        public abstract class forceECR : PX.Data.BQL.BqlBool.Field<forceECR> { }
        protected bool? _ForceECR;
		/// <summary>
		/// A Boolean value that indicates whether the ECR or ECO is required for new BOM revisions.
		/// </summary>
		[PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Require ECR/ECO for New BOM Revisions", FieldClass = Features.ECCFIELDCLASS)]
        public virtual bool? ForceECR
        {
            get
            {
                return this._ForceECR;
            }
            set
            {
                this._ForceECR = value;
            }
        }
        #endregion
		#region RequireECRBeforeECO
        public abstract class requireECRBeforeECO : PX.Data.BQL.BqlBool.Field<requireECRBeforeECO> { }
        protected bool? _RequireECRBeforeECO;
		/// <summary>
		/// A Boolean value that indicates whether the ECR is required before creating ECO.
		/// </summary>
		[PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Require ECR Before Creating ECO", FieldClass = Features.ECCFIELDCLASS)]
        public virtual bool? RequireECRBeforeECO
        {
            get
            {
                return this._RequireECRBeforeECO;
            }
            set
            {
                this._RequireECRBeforeECO = value;
            }
        }
        #endregion	

        #region AllowArchiveWithoutUpdatePending
        public abstract class allowArchiveWithoutUpdatePending : PX.Data.BQL.BqlBool.Field<allowArchiveWithoutUpdatePending> { }
        protected bool? _AllowArchiveWithoutUpdatePending;

		/// <summary>
		/// A Boolean value that indicates whether users can archive the cost roll results without updating the pending costs.
		/// </summary>
		[PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Allow Archive without Update Pending")]
        public virtual bool? AllowArchiveWithoutUpdatePending
        {
            get
            {
                return this._AllowArchiveWithoutUpdatePending;
            }
            set
            {
                this._AllowArchiveWithoutUpdatePending = value;
            }
        }
        #endregion	
        #region AutoArchiveWhenUpdatePending
        public abstract class autoArchiveWhenUpdatePending : PX.Data.BQL.BqlBool.Field<autoArchiveWhenUpdatePending> { }
        protected bool? _AutoArchiveWhenUpdatePending;

		/// <summary>
		/// A Boolean value that indicates whether the system will archive the cost roll results when a user updates pending costs.
		/// </summary>
		[PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Auto Archive when Update Pending")]
        public virtual bool? AutoArchiveWhenUpdatePending
        {
            get
            {
                return this._AutoArchiveWhenUpdatePending;
            }
            set
            {
                this._AutoArchiveWhenUpdatePending = value;
            }
        }

		/// <summary>
		/// A Boolean value that indicates whether the new BOM revisions have the On Hold status. 
		/// </summary>
		#endregion

		#region BOM Hold Revisions on Entry
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Hold BOM Revisions on Entry", Enabled = true)]
		public bool? BOMHoldRevisionsOnEntry { get; set; }

		public abstract class bOMHoldRevisionsOnEntry : PX.Data.BQL.BqlInt.Field<bOMHoldRevisionsOnEntry> { }

		#endregion

		#region DefaultMoveTime
		/// <inheritdoc cref="DefaultMoveTime"/>
		public abstract class defaultMoveTime : PX.Data.BQL.BqlInt.Field<defaultMoveTime> { }

		/// <summary>
		/// The time for a semi-finished item to be moved from the work center where the current operation is performed to the work center where the next operation will be performed.
		/// </summary>
		[PXDBInt]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Default Move Time")]
		public virtual Int32? DefaultMoveTime { get; set; }
		#endregion
		#region Default Queue Time
		/// <inheritdoc cref="DefaultQueueTime"/>
		public abstract class defaultQueueTime : PX.Data.BQL.BqlInt.Field<defaultQueueTime> { }

		protected Int32? _DefaultQueueTime;

		/// <summary>
		/// The time a semi-finished item has to wait in the work center before workers can start processing the item.
		/// </summary>
		[PXDBInt]
		[PXUIField(DisplayName = "Default Queue Time")]
		[PXDefault(0)]
		public virtual Int32? DefaultQueueTime
		{
			get
			{
				return this._DefaultQueueTime;
			}
			set
			{
				this._DefaultQueueTime = value;
			}
		}
		#endregion
		#region Default Finish Time
		/// <inheritdoc cref="DefaultFinishTime"/>
		public abstract class defaultFinishTime : PX.Data.BQL.BqlInt.Field<defaultFinishTime> { }

		protected Int32? _DefaultFinishTime;

		/// <summary>
		/// The time required for the semi-finished item to be prepared for the next operation when the current operation has been finished.
		/// </summary>
		[PXDBInt]
		[PXUIField(DisplayName = "Default Finish Time")]
		[PXDefault(0)]
		public virtual Int32? DefaultFinishTime
		{
			get
			{
				return this._DefaultFinishTime;
			}
			set
			{
				this._DefaultFinishTime = value;
			}
		}
		#endregion
	}
}

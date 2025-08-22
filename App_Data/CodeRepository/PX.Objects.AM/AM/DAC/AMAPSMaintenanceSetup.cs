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

namespace PX.Objects.AM
{
	/// <summary>
	/// A single result of processing on the APS Maintenance Process (AM512000) form,
	/// which corresponds to the <see cref="APSMaintenanceProcess"/> graph.
	/// </summary>
	[PXCacheName("APS Maintenance Setup")]
    [Serializable]
    public class AMAPSMaintenanceSetup : PXBqlTable, IBqlTable
    {
        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        protected bool? _Selected = false;
        [PXBool]
        [PXUnboundDefault(false)]
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

        #region WorkCenterCalendarProcessLastRunDateTime
        public abstract class workCenterCalendarProcessLastRunDateTime : PX.Data.BQL.BqlDateTime.Field<workCenterCalendarProcessLastRunDateTime> { }

		/// <summary>
		/// The date when the work center schedule was updated for the last time.
		/// </summary>
		[PXDBDate(InputMask = "g", PreserveTime = true, UseTimeZone = true)]
        [PXUIField(DisplayName = "Work Center Calendar Process Last Run Date Time", Enabled = false)]
        public virtual DateTime? WorkCenterCalendarProcessLastRunDateTime { get; set; }
        #endregion

        #region WorkCenterCalendarProcessLastRunByID
        public abstract class workCenterCalendarProcessLastRunByID : PX.Data.BQL.BqlGuid.Field<workCenterCalendarProcessLastRunByID> { }

		/// <summary>
		/// The user who ran the update of the work center schedule.
		/// </summary>
		[PXDBGuid]
        [PXUIField(DisplayName = "Work Center Calendar Process Last Run By", Enabled = false)]
        public virtual Guid? WorkCenterCalendarProcessLastRunByID { get; set; }
        #endregion

        #region BlockSizeSyncProcessLastRunDateTime
        public abstract class blockSizeSyncProcessLastRunDateTime : PX.Data.BQL.BqlDateTime.Field<blockSizeSyncProcessLastRunDateTime> { }

		/// <summary>
		/// The date and time when the schedule blocks for the work center were last updated.
		/// </summary>
		[PXDBDate(InputMask = "g", PreserveTime = true, UseTimeZone = true)]
        [PXUIField(DisplayName = "Block Size Sync Process Last Run Date Time", Enabled = false)]
        public virtual DateTime? BlockSizeSyncProcessLastRunDateTime { get; set; }
        #endregion

        #region BlockSizeSyncProcessLastRunByID
        public abstract class blockSizeSyncProcessLastRunByID : PX.Data.BQL.BqlGuid.Field<blockSizeSyncProcessLastRunByID> { }

		/// <summary>
		/// The user who updated the schedule block for the work center.
		/// </summary>
		[PXDBGuid]
        [PXUIField(DisplayName = "Block Size Sync Process Last Run By", Enabled = false)]
        public virtual Guid? BlockSizeSyncProcessLastRunByID { get; set; }
        #endregion

        #region LastBlockSize
        public abstract class lastBlockSize : PX.Data.BQL.BqlInt.Field<lastBlockSize> { }

		/// <summary>
		/// The previous schedule block size.
		/// </summary>
		[PXDBInt]
        [PXUIField(DisplayName = "Last Block Size", Enabled = false)]
        [SchdBlockSizeList]
        public virtual int? LastBlockSize { get; set; }
        #endregion

        #region CurrentBlockSize
        public abstract class currentBlockSize : PX.Data.BQL.BqlInt.Field<currentBlockSize> { }

		/// <summary>
		/// The new block size.
		/// </summary>
		[PXInt]
        [PXUIField(DisplayName = "Current Block Size", Enabled = false)]
        [SchdBlockSizeList]
        [PXUnboundDefault(typeof(Search<AMPSetup.schdBlockSize>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? CurrentBlockSize { get; set; }
        #endregion

        #region HistoryCleanupProcessLastRunDateTime
        public abstract class historyCleanupProcessLastRunDateTime : PX.Data.BQL.BqlDateTime.Field<historyCleanupProcessLastRunDateTime> { }

		/// <summary>
		/// The date and time when the history was cleaned up for the last time.
		/// </summary>
		[PXDBDate(InputMask = "g", PreserveTime = true, UseTimeZone = true)]
        [PXUIField(DisplayName = "History Cleanup Process Last Run Date Time", Enabled = false)]
        public virtual DateTime? HistoryCleanupProcessLastRunDateTime { get; set; }
        #endregion

        #region HistoryCleanupProcessLastRunByID
        public abstract class historyCleanupProcessLastRunByID : PX.Data.BQL.BqlGuid.Field<historyCleanupProcessLastRunByID> { }

		/// <summary>
		/// The user who ran the history cleanup.
		/// </summary>
		[PXDBGuid]
        [PXUIField(DisplayName = "History Cleanup Process Last Run By", Enabled = false, Visible = false)]
        public virtual Guid? HistoryCleanupProcessLastRunByID { get; set; }
        #endregion

        #region WorkCalendarProcessLastRunDateTime
        public abstract class workCalendarProcessLastRunDateTime : PX.Data.BQL.BqlDateTime.Field<workCalendarProcessLastRunDateTime> { }

		/// <summary>
		/// The date and time when the work calendar process was lately run.
		/// </summary>
		[PXDBDate(InputMask = "g", PreserveTime = true, UseTimeZone = true)]
        [PXUIField(DisplayName = "Work Calendar Process Last Run Date Time", Enabled = false)]
        public virtual DateTime? WorkCalendarProcessLastRunDateTime { get; set; }
        #endregion

        #region WorkCalendarProcessLastRunByID
        public abstract class workCalendarProcessLastRunByID : PX.Data.BQL.BqlGuid.Field<workCalendarProcessLastRunByID> { }

		/// <summary>
		/// The user who ran the update of the work calendar parameters.
		/// </summary>
		[PXDBGuid]
        [PXUIField(DisplayName = "Work Calendar Process Last Run By", Enabled = false, Visible = false)]
        public virtual Guid? WorkCalendarProcessLastRunByID { get; set; }
        #endregion

        #region LastModifiedByID

        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }

        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion

        #region LastModifiedByScreenID

        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }

        [PXDBLastModifiedByScreenID]
        public virtual String LastModifiedByScreenID { get; set; }
        #endregion

        #region LastModifiedDateTime

        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }

        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion
    }
}

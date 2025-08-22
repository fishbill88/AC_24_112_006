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
using PX.Data.EP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CR.MassProcess;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using System;

namespace PX.Objects.FS
{
    public class InventoryPostBatchMaint : PXGraph<InventoryPostBatchMaint>
    {
		#region Selects
		[PXHidden]
        public PXSelect<BAccount> BAccount;
        [PXHidden]
        public PXSelect<Customer> Customer;
        [PXHidden]
        public PXSelect<FSServiceOrder> FSServiceOrderDummy;

		public PXSelect<FSPostBatch, Where<FSPostBatch.postTo, Equal<ListField_PostTo.IN>>> BatchRecords;

		[PXHidden]
        public PXSelect<FSPostDet>
               BatchDetails;

        public PXSelectReadonly<InventoryPostingBatchDetail,
                Where<
                    InventoryPostingBatchDetail.batchID, Equal<Current<FSPostBatch.batchID>>>>
                BatchDetailsInfo;
        #endregion

		public InventoryPostBatchMaint()
		{
			PXUIFieldAttribute.SetEnabled(BatchRecords.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<FSPostBatch.batchNbr>(BatchRecords.Cache, null, true);
			BatchRecords.Cache.AllowInsert = false;

			Save.SetVisible(false);
		}

        #region CacheAttached
        #region FSPostBatch_BatchNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Batch Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(
                           Search<FSPostBatch.batchNbr, 
                                Where<FSPostBatch.postTo, Equal<ListField_PostTo.IN>,
                                    And<FSPostBatch.status, NotEqual<FSPostBatch.status.temporary>>>>),
                            new Type[]
                                        {
                                            typeof(FSPostBatch.batchNbr),
                                            typeof(FSPostBatch.finPeriodID),
                                            typeof(FSPostBatch.cutOffDate),
                                        })]
        [AutoNumber(typeof(Search<FSSetup.postBatchNumberingID>),
                    typeof(AccessInfo.businessDate))]
        protected virtual void FSPostBatch_BatchNbr_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region FSPostBatch_QtyDoc
        [PXDBInt]
        [PXUIField(DisplayName = "Lines Processed")]
        protected virtual void FSPostBatch_QtyDoc_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region FSPostBatch_CutOffDate
        [PXDBDate]
        [PXUIField(DisplayName = "Up to Date")]
        protected virtual void FSPostBatch_CutOffDate_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region FSPostBatch_FinPeriodID
        [FinPeriodID(BqlField = typeof(FSPostBatch.finPeriodID))]
        [PXUIField(DisplayName = "Document Period", Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void FSPostBatch_FinPeriodID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region FSPostBatch_InvoiceDate
        [PXDBDate]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Document Date", Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void FSPostBatch_InvoiceDate_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region PostingBatchDetail_AppointmentID 
        [PXDBInt(BqlField = typeof(FSAppointment.appointmentID))]
        [PXUIField(DisplayName = "Appointment Nbr.")]
        [PXSelector(typeof(Search<FSAppointment.appointmentID>), SubstituteKey = typeof(FSAppointment.refNbr))]
        protected virtual void InventoryPostingBatchDetail_AppointmentID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region PostingBatchDetail_SOID
        [PXDBInt(BqlField = typeof(FSAppointment.sOID))]
        [PXUIField(DisplayName = "Service Order Nbr.")]
        [PXSelector(typeof(Search<FSServiceOrder.sOID>), SubstituteKey = typeof(FSServiceOrder.refNbr))]
        protected virtual void InventoryPostingBatchDetail_SOID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region PostingBatchDetail_AcctName
        [PXDBString(60, IsUnicode = true, BqlField = typeof(Customer.acctName))]
        [PXDefault]
        [PXFieldDescription]
        [PXMassMergableField]
        [PXUIField(DisplayName = "Billing Customer Name", Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void InventoryPostingBatchDetail_AcctName_CacheAttached(PXCache sender)
        {
        }
		#endregion
		#endregion

		#region Actions
		public PXSave<FSPostBatch> Save;
		public PXCancel<FSPostBatch> Cancel;
		public PXFirst<FSPostBatch> First;
		public PXPrevious<FSPostBatch> Previous;
		public PXNext<FSPostBatch> Next;
		public PXLast<FSPostBatch> Last;

		public PXAction<FSPostBatch> OpenDocument;
        [PXButton]
        [PXUIField(DisplayName = "Open Document", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual void openDocument()
        {
            InventoryPostingBatchDetail inventoryPostingBatchDetailRow = BatchDetailsInfo.Current;

            FSPostDet fsPostDetRow = PXSelectJoin<FSPostDet,
                                     InnerJoin<FSPostInfo,
                                         On<FSPostInfo.postID, Equal<FSPostDet.postID>>,
                                     InnerJoin<FSAppointmentDet,
                                         On<FSAppointmentDet.postID, Equal<FSPostInfo.postID>>>>,
                                     Where<
                                         FSPostDet.batchID, Equal<Current<FSPostBatch.batchID>>,
                                         And<FSAppointmentDet.appDetID, Equal<Required<FSAppointmentDet.appDetID>>>>>
                                     .Select(this, inventoryPostingBatchDetailRow.AppointmentInventoryItemID);

            if (fsPostDetRow != null && fsPostDetRow.INPosted == true)
            {
                if (fsPostDetRow.INDocType.Trim() == INDocType.Receipt)
                {
                    INReceiptEntry graphINReceiptEntry = PXGraph.CreateInstance<INReceiptEntry>();
                    graphINReceiptEntry.receipt.Current = graphINReceiptEntry.receipt.Search<INRegister.refNbr>(fsPostDetRow.INRefNbr);
                    throw new PXRedirectRequiredException(graphINReceiptEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
                }
                else
                {
                    INIssueEntry graphINIssueEntry = PXGraph.CreateInstance<INIssueEntry>();
                    graphINIssueEntry.issue.Current = graphINIssueEntry.issue.Search<INRegister.refNbr>(fsPostDetRow.INRefNbr);
                    throw new PXRedirectRequiredException(graphINIssueEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
                }
            }
        }
        #endregion

        #region Event Handlers

        #region FSPostBatch

        protected virtual void _(Events.RowSelected<FSPostBatch> e)
        {
		}
        #endregion

        #endregion
    }
}

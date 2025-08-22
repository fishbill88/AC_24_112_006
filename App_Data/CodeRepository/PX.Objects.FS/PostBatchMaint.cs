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
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CR.MassProcess;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO;
using static PX.Data.BQL.BqlPlaceholder;

namespace PX.Objects.FS
{
    public class PostBatchMaint : PXGraph<PostBatchMaint>
    {
        #region Selects
        [PXHidden]
        public PXSelect<BAccount> BAccount;
        [PXHidden]
        public PXSelect<Customer> Customer;
        [PXHidden]
        public PXSelect<FSAppointment> Appointment;
        [PXHidden]
        public PXSelect<FSServiceOrder> FSServiceOrderDummy;

        public PXSelect<FSPostBatch, 
               Where<
                   FSPostBatch.postTo, NotEqual<ListField_PostTo.IN>,
                   And<
                       Where<
                           FSPostBatch.status, IsNull,
                           Or<FSPostBatch.status, NotEqual<FSPostBatch.status.temporary>>>>>> BatchRecords;

        [PXHidden]
        public PXSelect<PostingBatchDetail> PostingBatchDetails;

        public PXSelectJoin<FSCreatedDoc,
                InnerJoin<PostingBatchDetail,     
                    On<
                       PostingBatchDetail.postedTO, Equal<FSCreatedDoc.postTo>,
                       And<PostingBatchDetail.postDocType, Equal<FSCreatedDoc.createdDocType>,
                       And<PostingBatchDetail.postRefNbr, Equal<FSCreatedDoc.createdRefNbr>>>>>,
                Where<
                    FSCreatedDoc.batchID, Equal<Current<FSPostBatch.batchID>>>,
                OrderBy<
                    Asc<FSCreatedDoc.recordID,
                    Asc<PostingBatchDetail.sOID,
                    Asc<PostingBatchDetail.appointmentID>>>>>
                BatchDetailsInfo;

		#endregion

		public PostBatchMaint()
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
        [PXSelector(typeof(Search<FSPostBatch.batchNbr, Where<FSPostBatch.postTo, NotEqual<ListField_PostTo.IN>, And<FSPostBatch.status, NotEqual<FSPostBatch.status.temporary>>>>))]
        [AutoNumber(typeof(Search<FSSetup.postBatchNumberingID>),
                    typeof(AccessInfo.businessDate))]
        protected virtual void FSPostBatch_BatchNbr_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region PostingBatchDetail_AppointmentID
        [PXDBInt(BqlField = typeof(FSAppointment.appointmentID))]
        [PXUIField(DisplayName = "Appointment Nbr.")]
        [PXSelector(typeof(Search<FSAppointment.appointmentID>), SubstituteKey = typeof(FSAppointment.refNbr))]
        protected virtual void PostingBatchDetail_AppointmentID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region PostingBatchDetail_SOID
        [PXDBInt(BqlField = typeof(FSServiceOrder.sOID))]
        [PXUIField(DisplayName = "Service Order Nbr.")]
        [PXSelector(typeof(Search<FSServiceOrder.sOID>), SubstituteKey = typeof(FSServiceOrder.refNbr))]
        protected virtual void PostingBatchDetail_SOID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region PostingBatchDetail_AcctName
        [PXDBString(60, IsUnicode = true, BqlField = typeof(Customer.acctName))]
        [PXDefault]
        [PXFieldDescription]
        [PXMassMergableField]
        [PXUIField(DisplayName = "Billing Customer Name", Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void PostingBatchDetail_AcctName_CacheAttached(PXCache sender)
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
            FSCreatedDoc postingBatchDetailRow = BatchDetailsInfo.Current;

            if (postingBatchDetailRow.PostTo == ID.Batch_PostTo.SO)
            {
                if (PXAccess.FeatureInstalled<FeaturesSet.distributionModule>())
                {
                    SOOrderEntry graphSOOrderEntry = PXGraph.CreateInstance<SOOrderEntry>();
                    graphSOOrderEntry.Document.Current = graphSOOrderEntry.Document.Search<SOOrder.orderNbr>(postingBatchDetailRow.CreatedRefNbr, postingBatchDetailRow.CreatedDocType);
                    throw new PXRedirectRequiredException(graphSOOrderEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
                }
            }
            else if (postingBatchDetailRow.PostTo == ID.Batch_PostTo.AR)
            {
                ARInvoiceEntry graphARInvoiceEntry = PXGraph.CreateInstance<ARInvoiceEntry>();
                graphARInvoiceEntry.Document.Current = graphARInvoiceEntry.Document.Search<ARInvoice.refNbr>(postingBatchDetailRow.CreatedRefNbr, postingBatchDetailRow.CreatedDocType);
                throw new PXRedirectRequiredException(graphARInvoiceEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            else if (postingBatchDetailRow.PostTo == ID.Batch_PostTo.SI)
            {
                SOInvoiceEntry graphSOInvoiceEntry = PXGraph.CreateInstance<SOInvoiceEntry>();
                graphSOInvoiceEntry.Document.Current = graphSOInvoiceEntry.Document.Search<ARInvoice.refNbr>(postingBatchDetailRow.CreatedRefNbr, postingBatchDetailRow.CreatedDocType);
                throw new PXRedirectRequiredException(graphSOInvoiceEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            else if (postingBatchDetailRow.PostTo == ID.Batch_PostTo.AP)
            {
                APInvoiceEntry graphAPInvoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
                graphAPInvoiceEntry.Document.Current = graphAPInvoiceEntry.Document.Search<APInvoice.refNbr>(postingBatchDetailRow.CreatedRefNbr, postingBatchDetailRow.CreatedDocType);
                throw new PXRedirectRequiredException(graphAPInvoiceEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            else if (postingBatchDetailRow.PostTo == ID.Batch_PostTo.IN)
            {
                INIssueEntry graphINIssueEntry = PXGraph.CreateInstance<INIssueEntry>();
                graphINIssueEntry.issue.Current = graphINIssueEntry.issue.Search<INRegister.refNbr>(postingBatchDetailRow.CreatedRefNbr, postingBatchDetailRow.CreatedDocType);
                throw new PXRedirectRequiredException(graphINIssueEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            else if (postingBatchDetailRow.PostTo == ID.Batch_PostTo.PM)
            {
                RegisterEntry graphRegisterEntry = PXGraph.CreateInstance<RegisterEntry>();
                graphRegisterEntry.Document.Current = graphRegisterEntry.Document.Search<PMRegister.refNbr>(postingBatchDetailRow.CreatedRefNbr, postingBatchDetailRow.CreatedDocType);
                throw new PXRedirectRequiredException(graphRegisterEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
        }
        #endregion

        #region Events Handlers

        #region FSPostbatch

        #region FieldSelecting
        #endregion
        #region FieldDefaulting
        #endregion
        #region FieldUpdating
        #endregion
        #region FieldVerifying
        #endregion
        #region FieldUpdated
        #endregion

        protected virtual void _(Events.RowSelecting<FSPostBatch> e)
        {
        }

        protected virtual void _(Events.RowSelected<FSPostBatch> e)
        {
		}

        protected virtual void _(Events.RowInserting<FSPostBatch> e)
        {
        }

        protected virtual void _(Events.RowInserted<FSPostBatch> e)
        {
        }

        protected virtual void _(Events.RowUpdating<FSPostBatch> e)
        {
        }

        protected virtual void _(Events.RowUpdated<FSPostBatch> e)
        {
        }

        protected virtual void _(Events.RowDeleting<FSPostBatch> e)
        {
        }

        protected virtual void _(Events.RowDeleted<FSPostBatch> e)
        {
        }

        protected virtual void _(Events.RowPersisting<FSPostBatch> e)
        {
        }

        protected virtual void _(Events.RowPersisted<FSPostBatch> e)
        {
        }

        #endregion
        
        #region PostingBatchDetail

        #region FieldSelecting
        #endregion
        #region FieldDefaulting
        #endregion
        #region FieldUpdating
        #endregion
        #region FieldVerifying
        #endregion
        #region FieldUpdated
        #endregion

        protected virtual void _(Events.RowSelecting<PostingBatchDetail> e)
        {
            if (e.Row == null)
            {
                return;
            }

            PostingBatchDetail postingBatchDetailRow = e.Row as PostingBatchDetail;

            if (postingBatchDetailRow.SOPosted == true)
            {
                using (new PXConnectionScope())
                {
                    var soOrderShipment = (SOOrderShipment)PXSelectReadonly<SOOrderShipment,
                                          Where<
                                              SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>,
                                          And<
                                              SOOrderShipment.orderType, Equal<Required<SOOrderShipment.orderType>>>>>
                                          .Select(e.Cache.Graph, postingBatchDetailRow.SOOrderNbr, postingBatchDetailRow.SOOrderType);

                    postingBatchDetailRow.InvoiceRefNbr = soOrderShipment?.InvoiceNbr;
                }
            }
            else if (postingBatchDetailRow.ARPosted == true || postingBatchDetailRow.SOInvPosted == true)
            {
                postingBatchDetailRow.InvoiceRefNbr = postingBatchDetailRow.Mem_DocNbr;
            }
        }

        protected virtual void _(Events.RowSelected<PostingBatchDetail> e)
        {
        }

        protected virtual void _(Events.RowInserting<PostingBatchDetail> e)
        {
        }

        protected virtual void _(Events.RowInserted<PostingBatchDetail> e)
        {
        }

        protected virtual void _(Events.RowUpdating<PostingBatchDetail> e)
        {
        }

        protected virtual void _(Events.RowUpdated<PostingBatchDetail> e)
        {
        }

        protected virtual void _(Events.RowDeleting<PostingBatchDetail> e)
        {
        }

        protected virtual void _(Events.RowDeleted<PostingBatchDetail> e)
        {
        }

        protected virtual void _(Events.RowPersisting<PostingBatchDetail> e)
        {
        }

        protected virtual void _(Events.RowPersisted<PostingBatchDetail> e)
        {
        }

        #endregion
        
        #endregion
    }
}

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
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.SO;
using System.Collections;
using static PX.Data.BQL.BqlPlaceholder;

namespace PX.Objects.FS
{
    public class ContractPostBatchMaint : PXGraph<ContractPostBatchMaint>
    {
        #region Selects
        [PXHidden]
        public PXSelect<BAccount> BAccount;
        [PXHidden]
        public PXSelect<Customer> Customer;

        public PXSelect<FSContractPostBatch> ContractBatchRecords;

        public PXSelectReadonly<ContractPostBatchDetail,
               Where<
                   ContractPostBatchDetail.contractPostBatchID, Equal<Current<FSContractPostBatch.contractPostBatchID>>>> ContractPostDocRecords;

		#endregion

		public ContractPostBatchMaint()
		{
			PXUIFieldAttribute.SetEnabled(ContractBatchRecords.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<FSContractPostBatch.contractPostBatchNbr>(ContractBatchRecords.Cache, null, true);
			ContractBatchRecords.Cache.AllowInsert = false;

			Save.SetVisible(false);
		}

		#region Actions
		public PXSave<FSContractPostBatch> Save;
		public PXCancel<FSContractPostBatch> Cancel;
		public PXFirst<FSContractPostBatch> First;
		public PXPrevious<FSContractPostBatch> Previous;
		public PXNext<FSContractPostBatch> Next;
		public PXLast<FSContractPostBatch> Last;

		public PXAction<FSContractPostBatch> openDocument;
        [PXButton]
        [PXUIField(DisplayName = "Open Document", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual void OpenDocument()
        {
            ContractPostBatchDetail fsContractPostDocRow = ContractPostDocRecords.Current;

            if (fsContractPostDocRow == null)
            {
                return;
            }

            if (fsContractPostDocRow.PostedTO == ID.Batch_PostTo.SO)
            {
                if (PXAccess.FeatureInstalled<FeaturesSet.distributionModule>())
                {
                    SOOrderEntry graphSOOrderEntry = PXGraph.CreateInstance<SOOrderEntry>();
                    graphSOOrderEntry.Document.Current = graphSOOrderEntry.Document.Search<SOOrder.orderNbr>(fsContractPostDocRow.PostRefNbr, fsContractPostDocRow.PostDocType);
                    throw new PXRedirectRequiredException(graphSOOrderEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
                }
            }
            else if(fsContractPostDocRow.PostedTO == ID.Batch_PostTo.AR)
            {
                ARInvoiceEntry graphARInvoiceEntry = PXGraph.CreateInstance<ARInvoiceEntry>();
                graphARInvoiceEntry.Document.Current = graphARInvoiceEntry.Document.Search<ARInvoice.refNbr>(fsContractPostDocRow.PostRefNbr, fsContractPostDocRow.PostDocType);
                throw new PXRedirectRequiredException(graphARInvoiceEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
        }

        public PXAction<FSContractPostBatch> openContract;
        [PXButton]
        [PXUIField(DisplayName = "Open Contract", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual void OpenContract()
        {
            ContractPostBatchDetail fsContractPostDocRow = ContractPostDocRecords.Current;

            if (fsContractPostDocRow == null)
            {
                return;
            }

            FSServiceContract fsServiceContractRow = FSServiceContract.PK.Find(this, fsContractPostDocRow.ServiceContractID);

            if (fsServiceContractRow == null)
                return;

            if (fsServiceContractRow.RecordType == ID.RecordType_ServiceContract.SERVICE_CONTRACT)
            {
                ServiceContractEntry graphServiceContractEntry = PXGraph.CreateInstance<ServiceContractEntry>();
                graphServiceContractEntry.ServiceContractRecords.Current = graphServiceContractEntry.ServiceContractRecords.Search<FSServiceContract.serviceContractID>(fsServiceContractRow.ServiceContractID, fsServiceContractRow.CustomerID);
                throw new PXRedirectRequiredException(graphServiceContractEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            else
            {
                RouteServiceContractEntry graphRouteServiceContractEntry = PXGraph.CreateInstance<RouteServiceContractEntry>();
                graphRouteServiceContractEntry.ServiceContractRecords.Current = graphRouteServiceContractEntry.ServiceContractRecords.Search<FSServiceContract.serviceContractID>(fsServiceContractRow.ServiceContractID, fsServiceContractRow.CustomerID);
                throw new PXRedirectRequiredException(graphRouteServiceContractEntry, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            
        }
        #endregion

        #region Events

        protected virtual void _(Events.RowSelected<FSContractPostBatch> e)
        {
        }
        #endregion
    }
}

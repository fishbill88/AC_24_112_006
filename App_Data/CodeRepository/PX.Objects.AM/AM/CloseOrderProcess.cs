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
using System.Collections;
using PX.Data;
using System.Collections.Generic;
using PX.Objects.GL;
using PX.Objects.AM.Attributes;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using System.Linq;
using PX.Common;

namespace PX.Objects.AM
{
    /// <summary>
    /// Close production orders process graph
    /// </summary>
public class CloseOrderProcess : PXGraph<CloseOrderProcess>
    {
        public PXCancel<AMProdItem> Cancel;

		[PXFilterable]
		public SelectFrom<AMProdItem>
			.InnerJoin<Branch>
				.On<AMProdItem.branchID.IsEqual<Branch.branchID>>
			.Where<Branch.baseCuryID.IsEqual<AccessInfo.baseCuryID.FromCurrent>
				.And<AMProdItem.closed.IsEqual<False>
				.And<Brackets<AMProdItem.locked.IsEqual<True>
					.Or<Brackets<AMProdItem.completed.IsEqual<True>
						.And<AMPSetup.lockWorkflowEnabled.FromCurrent.IsEqual<False>>>>>>>>
			.ProcessingView CompletedOrders;
		public PXFilter<FinancialPeriod> FinancialPeriod;
        public PXSetup<AMPSetup> ampsetup;

        public CloseOrderProcess()
        {
            FinancialPeriod financialPeriod = FinancialPeriod.Current;
            CompletedOrders.SetProcessDelegate(delegate(List<AMProdItem> list)
            {
                CloseOrders(list, true, financialPeriod);
            });
            
            InquiresDropMenu.AddMenuAction(TransactionsByProductionOrderInq);
        }

        public PXAction<AMProdItem> InquiresDropMenu;
        [PXUIField(DisplayName = Messages.Inquiries)]
        [PXButton(MenuAutoOpen = true)]
        protected virtual IEnumerable inquiresDropMenu(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<AMProdItem> TransactionsByProductionOrderInq;
        [PXUIField(DisplayName = "Unreleased Transactions", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable transactionsByProductionOrderInq(PXAdapter adapter)
        {
            CallTransactionsByProductionOrderGenericInquiry();

            return adapter.Get();
        }

        protected virtual void CallTransactionsByProductionOrderGenericInquiry()
        {
            var gi = new GITransactionsByProductionOrder();
            gi.SetFilterByProductionStatus(ProductionOrderStatus.Completed);
            gi.SetFilterByUnreleasedBatches();
            gi.CallGenericInquiry();
        }

		private enum ProcessStatus
		{
			NotProcessed,
			Processed,
			Error,
			Retry
		}

        public static void CloseOrders(List<AMProdItem> list, bool isMassProcess, FinancialPeriod financialPeriod)
        {
			var prodDetail = CreateInstance<ProdDetail>();
			var wipAdjustmentEntry = CreateInstance<WIPAdjustmentEntry>();
			var productionScheduleEngine = CreateInstance<ProductionScheduleEngine>();
			var hasOverallError = false;
			var tranScopeProcessedOrders = new List<int>();
			var processOrderStatus = new Dictionary<int, ProcessStatus>();
			var nextCloseIndex = 0;
			var isRetry = true;

			while (isRetry)
			{
				isRetry = false;
			wipAdjustmentEntry.Clear();
			wipAdjustmentEntry.ampsetup.Current.RequireControlTotal = false;
			wipAdjustmentEntry.ampsetup.Current.HoldEntry = false;
				var hasTranScopeError = false;
				tranScopeProcessedOrders.Clear();

			using (var SetProdStatusScope = new PXTransactionScope())
			{
					for (var i = nextCloseIndex; i < list.Count; i++)
				{
						nextCloseIndex++;
					var prodItem = list[i];
						var status = processOrderStatus.ContainsKey(i) ? processOrderStatus[i] : ProcessStatus.NotProcessed;
						if (status.IsIn(ProcessStatus.Processed, ProcessStatus.Error))
						{
							continue;
						}

						productionScheduleEngine.Clear();
						prodDetail.Clear();

						if (ProductionTransactionHelper.ProductionOrderHasUnreleasedTransactions(prodDetail, prodItem, out var unreleasedMsg))
						{
							processOrderStatus[i] = ProcessStatus.Error;
							PXProcessing<AMProdItem>.SetError(i, unreleasedMsg);
							hasOverallError = true;
							continue;
						}

					try
					{
						prodDetail.ProdItemRecords.Current = prodDetail.ProdItemRecords.Search<AMProdItem.prodOrdID>(prodItem.ProdOrdID, prodItem.OrderType);
							if (prodDetail.ProdItemRecords.Current == null || prodDetail.ProdItemRecords.Current.Closed == true)
							{
								processOrderStatus[i] = ProcessStatus.Processed;
								continue;
							}
						prodDetail.SetProductionOrderStatus(prodDetail.ProdItemRecords.Current, ProductionOrderStatus.Closed);
						prodDetail.SetAllMaterialStatus(prodDetail.ProdItemRecords.Current, true);
						prodDetail.EndProductionOrder(prodDetail.ProdItemRecords.Current, financialPeriod, wipAdjustmentEntry);
						prodDetail.CloseOrderWorkflow.Press();
						prodDetail.PersistBase();
							prodDetail.DeleteProductionSchedule(prodItem, productionScheduleEngine);
							tranScopeProcessedOrders.Add(i);
							processOrderStatus[i] = ProcessStatus.Processed;
						PXProcessing<AMProdItem>.SetInfo(i, ActionsMessages.RecordProcessed);
					}
					catch (Exception e)
					{
							var exMsg = new System.Text.StringBuilder();
							exMsg.AppendLine(e.Message);
							if (e is PXOuterException oe && oe.InnerMessages != null)
				{
								foreach (var m in oe.InnerMessages)
					{
									exMsg.AppendLine(m);
								}
					}

							processOrderStatus[i] = ProcessStatus.Error;
							PXProcessing<AMProdItem>.SetError(i, exMsg.ToString());
							hasOverallError = true;
							hasTranScopeError = true;

							foreach (var po in tranScopeProcessedOrders)
							{
								processOrderStatus[po] = ProcessStatus.Retry;
								PXProcessing<AMProdItem>.SetWarning(po, string.Empty);
				}

							isRetry = true;
							break;
						}
					}

					if (!hasTranScopeError)
					{
						if (wipAdjustmentEntry.transactions.Select().Count > 0)
				{
					wipAdjustmentEntry.Persist();
					AMDocumentRelease.ReleaseDoc(new List<AMBatch> { wipAdjustmentEntry.batch.Current });
				}

				SetProdStatusScope.Complete();
			}
				}

				if (nextCloseIndex >= list.Count && processOrderStatus.Values.Where(s => s == ProcessStatus.Retry).Any())
				{
					// Pass through all orders at least once. Then swing back around for retries only.
					nextCloseIndex = 0;
					isRetry = true;
				}
			}

			try
			{
				APSMaintenanceProcess.RunHistoryCleanupProcess();
			}
			catch (Exception exception)
			{
				PXTrace.WriteError(exception);
			}

			if (hasOverallError)
			{
				throw new PXOperationCompletedException(ErrorMessages.SeveralItemsFailed);
			}
		}
    }

	/// <summary>
	/// Non-table DAC for passing the financial period information into the manufacturing processes which require such information.
	/// </summary>
	[Serializable]
    [PXCacheName("Financial Period")]
    public class FinancialPeriod : PXBqlTable, IBqlTable
    {
        #region FinancialPeriodID
        public abstract class financialPeriodID : PX.Data.BQL.BqlString.Field<financialPeriodID> { }

        protected String _FinancialPeriodID;
		[OpenPeriod(null,
					typeof(AccessInfo.businessDate),
					typeof(AccessInfo.branchID))]
		[PXUIField(DisplayName = "Period", Visibility = PXUIVisibility.Visible)]
        public virtual String FinancialPeriodID
        {
            get
            {
                return this._FinancialPeriodID;
            }
            set
            {
                this._FinancialPeriodID = value;
            }
        }
        #endregion
    }
}

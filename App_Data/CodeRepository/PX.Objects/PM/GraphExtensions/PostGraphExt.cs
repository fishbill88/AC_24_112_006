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
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.PM.GraphExtensions;
using System;
using System.Collections.Generic;

namespace PX.Objects.PM
{
    public class PostGraphExt : PXGraphExtension<PostGraph>
    {
		public PXSelect<PMRegister> ProjectDocs;
		public PXSelect<PMTran> ProjectTrans;
		public PXSelect<PMHistoryAccum> ProjectHistory;
		Dictionary<string, PMTask> tasksToAutoAllocate = new Dictionary<string, PMTask>();
		List<Batch> created = new List<Batch>();

		[InjectDependency]
		public IProjectMultiCurrency MultiCurrencyService { get; set; }

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<CS.FeaturesSet.projectModule>();
		}

		#region Types

		[PXHidden]
		[Serializable]
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		[PXBreakInheritance]
		public class OffsetAccount : Account
		{
			#region AccountID
			public new abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
			#endregion
			#region AccountCD
			public new abstract class accountCD : PX.Data.BQL.BqlString.Field<accountCD> { }
			#endregion
			#region AccountGroupID
			public new abstract class accountGroupID : PX.Data.BQL.BqlInt.Field<accountGroupID> { }
			#endregion
		}

		[PXHidden]
		[Serializable]
		[PXBreakInheritance]
		public partial class OffsetPMAccountGroup : PMAccountGroup
		{
			#region GroupID
			public new abstract class groupID : PX.Data.BQL.BqlInt.Field<groupID> { }

			#endregion
			#region GroupCD
			public new abstract class groupCD : PX.Data.BQL.BqlString.Field<groupCD> { }
			#endregion
			#region Type
			public new abstract class type : PX.Data.BQL.BqlString.Field<type> { }
			#endregion
		}

		#endregion

		#region Cache Attached Events
		#region PMTran
		#region TranID
		[PXDBLongIdentity(IsKey = true)]
		protected virtual void _(Events.CacheAttached<PMTran.tranID> e)
		{
		}
		#endregion
		#region RefNbr
		[PXDBDefault(typeof(PMRegister.refNbr))]// is handled by the graph
		[PXDBString(15, IsUnicode = true)]
		protected virtual void _(Events.CacheAttached<PMTran.refNbr> e)
		{
		}
		#endregion
		#region BatchNbr
		[PXDBDefault(typeof(Batch.batchNbr), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "BatchNbr")]
		protected virtual void _(Events.CacheAttached<PMTran.batchNbr> e)
		{
		}
		#endregion
		#region Date
		[PXDBDate()]
		[PXDefault(typeof(PMRegister.date))]
		public virtual void _(Events.CacheAttached<PMTran.date> e)
		{
		}
		#endregion
		#region FinPeriodID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[OpenPeriod(
			null,
			typeof(PMTran.date),
			branchSourceType: typeof(PMTran.branchID),
			masterFinPeriodIDType: typeof(PMTran.tranPeriodID),
			redefaultOrRevalidateOnOrganizationSourceUpdated: false,
			redefaultOnDateChanged: false
		   )]
		public virtual void _(Events.CacheAttached<PMTran.finPeriodID> e)
		{
		}
		#endregion
		#region BaseCuryInfoID
		public abstract class baseCuryInfoID : IBqlField { }
		[PXDBLong]
		[CurrencyInfoDBDefault(typeof(CurrencyInfo.curyInfoID))]
		public virtual void _(Events.CacheAttached<PMTran.baseCuryInfoID> e)
		{
		}

		#endregion
		#region ProjectCuryInfoID
		public abstract class projectCuryInfoID : IBqlField { }
		[PXDBLong]
		[CurrencyInfoDBDefault(typeof(CurrencyInfo.curyInfoID))]
		public virtual void _(Events.CacheAttached<PMTran.projectCuryInfoID> e)
		{
		}
		#endregion
		#endregion
		#endregion

		[PXOverride]
		public virtual void UpdateAllocationBalance(Batch b)
		{
			UpdateProjectBalance(b);
		}

		[PXOverride]
		public virtual void ReleaseBatchProc(Batch b, bool unholdBatch, Action<Batch, bool> baseMethod)
        {
			tasksToAutoAllocate.Clear();
			created.Clear();

			baseMethod(b, unholdBatch);
						
			if (tasksToAutoAllocate.Count > 0)
			{
				try
				{
					AutoAllocateTasks(new List<PMTask>(tasksToAutoAllocate.Values));
				}
				catch (Exception ex)
				{
					throw new PXException(ex, PM.Messages.AutoAllocationFailed);
				}
			}

			if (Base.AutoPost)
			{
				foreach(Batch batch in created)
                {
					Base.PostBatchProc(batch);
				}				
			}
		}

		[PXOverride]
		public virtual void CreateProjectTransactions(Batch b)
		{
			ProjectBalance pb = CreateProjectBalance();

			if (b.Module == GL.BatchModule.GL)
			{
				PXSelectBase<GLTran> select = new PXSelectJoin<GLTran,
				InnerJoin<Account, On<GLTran.accountID, Equal<Account.accountID>>,
				InnerJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<Account.accountGroupID>>,
				InnerJoin<PMProject, On<PMProject.contractID, Equal<GLTran.projectID>, And<PMProject.nonProject, Equal<False>>>,
				InnerJoin<PMTask, On<PMTask.projectID, Equal<GLTran.projectID>, And<PMTask.taskID, Equal<GLTran.taskID>>>>>>>,
				Where<GLTran.module, Equal<BatchModule.moduleGL>,
				And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
				And<Account.accountGroupID, IsNotNull,
				And<GLTran.isNonPM, NotEqual<True>>>>>>(Base);

				PXResultset<GLTran> resultset = select.Select(b.BatchNbr);

				if (resultset.Count > 0)
				{
					PMRegister doc = new PMRegister();
					doc.Module = b.Module;
					doc.Date = b.DateEntered;
					doc.Description = b.Description;
					doc.Released = true;
					doc.Status = PMRegister.status.Released;
					SetOrigDocLink(doc, resultset);
					ProjectDocs.Insert(doc);
					ProjectDocs.Cache.Persist(PXDBOperation.Insert);
				}

				List<PMTran> sourceForAllocation = new List<PMTran>();

				foreach (PXResult<GLTran, Account, PMAccountGroup, PMProject, PMTask> res in resultset)
				{
					GLTran tran = (GLTran)res;
					Account acc = (Account)res;
					PMAccountGroup ag = (PMAccountGroup)res;
					PMProject project = (PMProject)res;
					PMTask task = (PMTask)res;

					PMTran pmt = (PMTran)ProjectTrans.Cache.Insert();
					pmt.BranchID = tran.BranchID;
					pmt.AccountGroupID = acc.AccountGroupID;
					pmt.AccountID = tran.AccountID;
					pmt.SubID = tran.SubID;
					pmt.BAccountID = tran.ReferenceID;
					pmt.BatchNbr = tran.BatchNbr;
					pmt.Date = tran.TranDate;
					pmt.Description = tran.TranDesc;
					pmt.FinPeriodID = tran.FinPeriodID;
					pmt.TranPeriodID = tran.TranPeriodID;
					pmt.InventoryID = tran.InventoryID;
					pmt.OrigLineNbr = tran.LineNbr;
					pmt.OrigModule = tran.Module;
					pmt.OrigRefNbr = tran.RefNbr;
					pmt.OrigTranType = tran.TranType;
					pmt.ProjectID = tran.ProjectID;
					pmt.TaskID = tran.TaskID;
					pmt.CostCodeID = tran.CostCodeID;
					pmt.Billable = tran.NonBillable != true;
					pmt.UseBillableQty = true;
					pmt.UOM = tran.UOM;

					MultiCurrencyService.CalculateCurrencyValues(Base, tran, pmt, Base.BatchModule.Current, project, Base.Ledger_LedgerID.Select());

					pmt.Qty = tran.Qty;// pmt.Amount >= 0 ? tran.Qty : (tran.Qty * -1);
					int sign = 1;
					if (acc.Type == AccountType.Income || acc.Type == AccountType.Liability)
					{
						sign = -1;
					}

					if (ProjectBalance.IsFlipRequired(acc.Type, ag.Type))
					{
						sign = sign * -1;

						pmt.ProjectCuryAmount = -pmt.ProjectCuryAmount;
						pmt.TranCuryAmount = -pmt.TranCuryAmount;
						pmt.Amount = -pmt.Amount;
						pmt.Qty = -pmt.Qty;
					}
					pmt.BillableQty = tran.Qty;
					pmt.Released = true;

					PXNoteAttribute.CopyNoteAndFiles(Base.GLTran_Module_BatNbr.Cache, tran, ProjectTrans.Cache, pmt);

					try
					{
						ProjectTrans.Update(pmt);
					}
					catch (PXFieldValueProcessingException e)
					{
						if (e.InnerException is PXTaskIsCompletedException)
						{
							throw new PXSetPropertyException<GLTran.taskID>(PM.Messages.NoPermissionForInactiveTasksWithDetails, task.TaskCD.Trim(), project.ContractCD.Trim(), PXErrorLevel.Error);
						}
						else
						{
							throw;
						}
					}

					Base.CurrencyInfo_ID.Cache.Persist(PXDBOperation.Insert);
					ProjectTrans.Cache.Persist(pmt, PXDBOperation.Insert);

					tran.PMTranID = pmt.TranID;
					Base.Caches[typeof(GLTran)].Update(tran);

					if (pmt.TaskID != null && (pmt.Qty != 0 || pmt.Amount != 0)) //TaskID will be null for Contract
					{
						ProjectBalance.Result balance = pb.Calculate(project, pmt, ag, acc.Type, sign, 1);

						if (balance.Status != null)
						{
							var ps = new PMBudgetAccum();
							ps.ProjectID = balance.Status.ProjectID;
							ps.ProjectTaskID = balance.Status.ProjectTaskID;
							ps.AccountGroupID = balance.Status.AccountGroupID;
							ps.InventoryID = balance.Status.InventoryID;
							ps.CostCodeID = balance.Status.CostCodeID;
							ps.UOM = balance.Status.UOM;
							ps.Type = balance.Status.Type;
							ps.CuryInfoID = balance.Status.CuryInfoID;
							ps.Description = balance.Status.Description;

							ps = (PMBudgetAccum)Base.Caches[typeof(PMBudgetAccum)].Insert(ps);
							ps.ActualQty += balance.Status.ActualQty.GetValueOrDefault();
							ps.CuryActualAmount += balance.Status.CuryActualAmount.GetValueOrDefault();
							ps.ActualAmount += balance.Status.ActualAmount.GetValueOrDefault();

							Base.Views.Caches.Add(typeof(PMBudgetAccum));
						}

						if (balance.ForecastHistory != null)
						{
							PMForecastHistoryAccum forecast = new PMForecastHistoryAccum();
							forecast.ProjectID = balance.ForecastHistory.ProjectID;
							forecast.ProjectTaskID = balance.ForecastHistory.ProjectTaskID;
							forecast.AccountGroupID = balance.ForecastHistory.AccountGroupID;
							forecast.InventoryID = balance.ForecastHistory.InventoryID;
							forecast.CostCodeID = balance.ForecastHistory.CostCodeID;
							forecast.PeriodID = balance.ForecastHistory.PeriodID;

							forecast = (PMForecastHistoryAccum)Base.Caches[typeof(PMForecastHistoryAccum)].Insert(forecast);

							forecast.ActualQty += balance.ForecastHistory.ActualQty.GetValueOrDefault();
							forecast.CuryActualAmount += balance.ForecastHistory.CuryActualAmount.GetValueOrDefault();
							forecast.ActualAmount += balance.ForecastHistory.ActualAmount.GetValueOrDefault();
							forecast.CuryArAmount += balance.ForecastHistory.CuryArAmount.GetValueOrDefault();
							Base.Views.Caches.Add(typeof(PMForecastHistoryAccum));
						}

						if (balance.TaskTotal != null)
						{
							var ta = new PMTaskTotal();
							ta.ProjectID = balance.TaskTotal.ProjectID;
							ta.TaskID = balance.TaskTotal.TaskID;

							ta = (PMTaskTotal)Base.Caches[typeof(PMTaskTotal)].Insert(ta);
							ta.CuryAsset += balance.TaskTotal.CuryAsset.GetValueOrDefault();
							ta.Asset += balance.TaskTotal.Asset.GetValueOrDefault();
							ta.CuryLiability += balance.TaskTotal.CuryLiability.GetValueOrDefault();
							ta.Liability += balance.TaskTotal.Liability.GetValueOrDefault();
							ta.CuryIncome += balance.TaskTotal.CuryIncome.GetValueOrDefault();
							ta.Income += balance.TaskTotal.Income.GetValueOrDefault();
							ta.CuryExpense += balance.TaskTotal.CuryExpense.GetValueOrDefault();
							ta.Expense += balance.TaskTotal.Expense.GetValueOrDefault();

							Base.Views.Caches.Add(typeof(PMTaskTotal));
						}

						RegisterReleaseProcess.AddToUnbilledSummary(Base, pmt);

						sourceForAllocation.Add(pmt);
						if (pmt.Allocated != true && pmt.ExcludedFromAllocation != true && project.AutoAllocate == true)
						{
							if (!tasksToAutoAllocate.ContainsKey(string.Format("{0}.{1}", task.ProjectID, task.TaskID)))
							{
								tasksToAutoAllocate.Add(string.Format("{0}.{1}", task.ProjectID, task.TaskID), task);
							}
						}
					}
				}
				Base.Caches[typeof(PMUnbilledDailySummaryAccum)].Persist(PXDBOperation.Insert);
				Base.Caches[typeof(PMBudgetAccum)].Persist(PXDBOperation.Insert);
				Base.Caches[typeof(PMForecastHistoryAccum)].Persist(PXDBOperation.Insert);
				Base.Caches[typeof(PMTaskTotal)].Persist(PXDBOperation.Insert);
				Base.Caches[typeof(PMTask)].Persist(PXDBOperation.Update);
			}
		}

		public virtual ProjectBalance CreateProjectBalance()
		{
			return new ProjectBalance(Base);
		}

		private void SetOrigDocLink(PMRegister doc, PXResultset<GLTran> glTrans)
		{
			JournalEntryTranRef entryRefGraph = PXGraph.CreateInstance<JournalEntryTranRef>();
			foreach (PXResult<GLTran> res in glTrans)
			{
				GLTran tran = (GLTran)res;
				if (tran.RefNbr != null)
				{
					APInvoice apDoc = PXSelect<APInvoice,
									Where<APRegister.docType, Equal<Required<GLTran.tranType>>,
										And<APRegister.refNbr, Equal<Required<GLTran.refNbr>>>>>.Select(Base, tran.TranType, tran.RefNbr);
					if (apDoc != null)
					{
						doc.OrigDocType = entryRefGraph.GetDocType(apDoc, null, tran);
						doc.OrigNoteID = entryRefGraph.GetNoteID(apDoc, null, tran);
						return;
					}
				}
			}
		}

		protected virtual void UpdateProjectBalance(Batch b)
		{
			PXSelectBase<GLTran> select = new PXSelectJoin<GLTran,
				InnerJoin<PMProject, On<GLTran.projectID, Equal<PMProject.contractID>>,
				InnerJoin<PMTran, On<GLTran.pMTranID, Equal<PMTran.tranID>>,
				InnerJoin<PMAccountGroup, On<PMTran.accountGroupID, Equal<PMAccountGroup.groupID>>,
				LeftJoin<OffsetPMAccountGroup, On<PMTran.offsetAccountGroupID, Equal<OffsetPMAccountGroup.groupID>>>>>>,
				Where<GLTran.module, Equal<Required<GLTran.module>>,
				And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
				And<GLTran.pMTranID, IsNotNull,
				And<GLTran.projectID, NotEqual<Required<GLTran.projectID>>>>>>>(Base);

			ProjectBalance pb = CreateProjectBalance();

			HashSet<long> processed = new HashSet<long>();

			foreach (PXResult<GLTran, PMProject, PMTran, PMAccountGroup, OffsetPMAccountGroup> res in select.Select(b.Module, b.BatchNbr, ProjectDefaultAttribute.NonProject()))
			{
				GLTran tran = (GLTran)res;
				PMProject project = (PMProject)res;
				PMTran pmt = (PMTran)res;
				PMAccountGroup ag = (PMAccountGroup)res;
				OffsetPMAccountGroup offsetAg = (OffsetPMAccountGroup)res;

				if (pmt.RemainderOfTranID != null)
					continue; //skip remainder transactions. 

				if (processed.Contains(tran.PMTranID.Value))
					continue;
				processed.Add(tran.PMTranID.Value);

				var balances = pb.Calculate(project, pmt, ag, offsetAg);

				foreach (ProjectBalance.Result balance in balances)
				{
					foreach (PMHistory item in balance.History)
					{
						PMHistoryAccum hist = new PMHistoryAccum();
						hist.ProjectID = item.ProjectID;
						hist.ProjectTaskID = item.ProjectTaskID;
						hist.AccountGroupID = item.AccountGroupID;
						hist.InventoryID = item.InventoryID;
						hist.CostCodeID = item.CostCodeID;
						hist.PeriodID = item.PeriodID;
						hist.BranchID = item.BranchID;

						hist = ProjectHistory.Insert(hist);
						hist.FinPTDCuryAmount += item.FinPTDCuryAmount.GetValueOrDefault();
						hist.FinPTDAmount += item.FinPTDAmount.GetValueOrDefault();
						hist.FinYTDCuryAmount += item.FinYTDCuryAmount.GetValueOrDefault();
						hist.FinYTDAmount += item.FinYTDAmount.GetValueOrDefault();
						hist.FinPTDQty += item.FinPTDQty.GetValueOrDefault();
						hist.FinYTDQty += item.FinYTDQty.GetValueOrDefault();
						hist.TranPTDCuryAmount += item.TranPTDCuryAmount.GetValueOrDefault();
						hist.TranPTDAmount += item.TranPTDAmount.GetValueOrDefault();
						hist.TranYTDCuryAmount += item.TranYTDCuryAmount.GetValueOrDefault();
						hist.TranYTDAmount += item.TranYTDAmount.GetValueOrDefault();
						hist.TranPTDQty += item.TranPTDQty.GetValueOrDefault();
						hist.TranYTDQty += item.TranYTDQty.GetValueOrDefault();
					}
				}
			}
		}

		protected virtual void AutoAllocateTasks(List<PMTask> tasks)
		{
			PMSetup setup = PXSelect<PMSetup>.Select(Base);
			bool autoreleaseAllocation = setup.AutoReleaseAllocation == true;

			PMAllocator allocator = PXGraph.CreateInstance<PMAllocator>();
			allocator.Clear();
			allocator.TimeStamp = Base.TimeStamp;
			allocator.Execute(tasks);
			allocator.Actions.PressSave();

			if (allocator.Document.Current != null && autoreleaseAllocation)
			{
				List<PMRegister> list = new List<PMRegister>();
				list.Add(allocator.Document.Current);

				List<ProcessInfo<Batch>> batchList;
				bool releaseSuccess = RegisterRelease.ReleaseWithoutPost(list, false, out batchList);
				if (!releaseSuccess)
				{
					throw new PXException(PM.Messages.AutoReleaseFailed);
				}

				foreach (var item in batchList)
				{
					created.AddRange(item.Batches);
				}
			}
		}

		
	}
}

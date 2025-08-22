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

using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.DependencyInjection;
using PX.Data.WorkflowAPI;
using PX.LicensePolicy;
using PX.Objects.Common.Interfaces;
using PX.Objects.Extensions.AddItemLookup;
using PX.Objects.GL;
using PX.Objects.IN.GraphExtensions.INRegisterEntryBaseExt;

namespace PX.Objects.IN
{
	public abstract class INRegisterEntryBase : PXGraph<PXGraph, INRegister>, IGraphWithInitialization, PXImportAttribute.IPXPrepareItems // the first generic parameter is not used by the platform
	{
		public INTranSplitPlan TranSplitPlanExt => FindImplementation<INTranSplitPlan>();

		#region Views

		public PXSetup<INSetup> insetup;

		public abstract PXSelectBase<INRegister> INRegisterDataMember { get; }
		public abstract PXSelectBase<INTran> INTranDataMember { get; }
		public abstract PXSelectBase<INTranSplit> INTranSplitDataMember { get; }
		public abstract PXSelectBase<INTran> LSSelectDataMember { get; }
		#endregion // Views

		protected abstract string ScreenID { get; }

		#region License Limits
		[InjectDependency]
		protected ILicenseLimitsService _licenseLimits { get; set; }

		void IGraphWithInitialization.Initialize()
		{
			if (_licenseLimits != null)
			{
				OnBeforeCommit += _licenseLimits.GetCheckerDelegate<INRegister>(
					new TableQuery(TransactionTypes.LinesPerMasterRecord, typeof(INTran), graph =>
					{
						var inEntry = (INRegisterEntryBase)graph;
						return new PXDataFieldValue[]
						{
							new PXDataFieldValue<INTran.docType>(PXDbType.Char, inEntry.INRegisterDataMember.Current?.DocType),
							new PXDataFieldValue<INTran.refNbr>(inEntry.INRegisterDataMember.Current?.RefNbr),
							new PXDataFieldValue<INTran.createdByScreenID>(PXDbType.Char, inEntry.ScreenID)
						};
					}),
					new TableQuery(TransactionTypes.SerialsPerDocument, typeof(INTranSplit), graph =>
					{
						var inEntry = (INRegisterEntryBase)graph;
						return new PXDataFieldValue[]
						{
							new PXDataFieldValue<INTranSplit.docType>(PXDbType.Char, inEntry.INRegisterDataMember.Current?.DocType),
							new PXDataFieldValue<INTranSplit.refNbr>(inEntry.INRegisterDataMember.Current?.RefNbr),
							new PXDataFieldValue<INTranSplit.createdByScreenID>(PXDbType.Char, inEntry.ScreenID)
						};
					}));
			}
		}
		#endregion

		#region Actions
		public PXInitializeState<INRegister> initializeState;

		public PXAction<INRegister> putOnHold;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Hold")]
		protected virtual IEnumerable PutOnHold(PXAdapter adapter) => adapter.Get();

		public PXAction<INRegister> releaseFromHold;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Remove Hold")]
		protected virtual IEnumerable ReleaseFromHold(PXAdapter adapter) => adapter.Get();

		public PXAction<INRegister> release;
		[PXProcessButton(CommitChanges = true)]
		[PXUIField(DisplayName = Messages.Release, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		protected virtual IEnumerable Release(PXAdapter adapter)
		{
			var list = new List<INRegister>();
			foreach (INRegister indoc in adapter.Get<INRegister>())
			{
				if (indoc.Hold == false && indoc.Released == false)
				{
					list.Add(INRegisterDataMember.Update(indoc));
				}
			}
			if (list.Count == 0)
			{
				throw new PXException(Messages.Document_Status_Invalid);
			}
			Save.Press();
			var quickProcessFlow = adapter.QuickProcessFlow;
			PXLongOperation.StartOperation(this, delegate () { INDocumentRelease.ReleaseDoc(list, false, processFlow: quickProcessFlow); });
			return list;
		}

		public PXAction<INRegister> viewBatch;
		[PXLookupButton(CommitChanges = true)]
		[PXUIField(DisplayName = "Review Batch", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		protected virtual IEnumerable ViewBatch(PXAdapter adapter)
		{
			if (INRegisterDataMember.Current is INRegister doc && !string.IsNullOrEmpty(doc.BatchNbr))
			{
				GL.JournalEntry graph = PXGraph.CreateInstance<GL.JournalEntry>();
				graph.BatchModule.Current = graph.BatchModule.Search<GL.Batch.batchNbr>(doc.BatchNbr, "IN");
				throw new PXRedirectRequiredException(graph, "Current batch record");
			}
			return adapter.Get();
		}

		public PXAction<INRegister> inventorySummary;
		[PXLookupButton(CommitChanges = true, VisibleOnDataSource = false)]
		[PXUIField(DisplayName = "Inventory Summary", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable InventorySummary(PXAdapter adapter)
		{
			PXCache tCache = INTranDataMember.Cache;
			INTran line = INTranDataMember.Current;
			if (line == null) return adapter.Get();

			InventoryItem item = InventoryItem.PK.Find(this, line.InventoryID);
			if (item != null && item.StkItem == true)
			{
				INSubItem sbitem = (INSubItem)PXSelectorAttribute.Select<INTran.subItemID>(tCache, line);
				InventorySummaryEnq.Redirect(item.InventoryID, sbitem?.SubItemCD, line.SiteID, line.LocationID);
			}
			return adapter.Get();
		}

		public PXAction<INRegister> iNEdit;
		[PXLookupButton(CommitChanges = true)]
		[PXUIField(DisplayName = Messages.INEditDetails, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable INEdit(PXAdapter adapter)
		{
			if (INRegisterDataMember.Current is INRegister doc)
			{
				var parameters = new Dictionary<string, string>
				{
					[nameof(INRegister.DocType)] = doc.DocType,
					[nameof(INRegister.RefNbr)] = doc.RefNbr,
					["PeriodTo"] = null,
					["PeriodFrom"] = null
				};
				throw new PXReportRequiredException(parameters, "IN611000",
					PXBaseRedirectException.WindowMode.New, Messages.INEditDetails);
			}
			return adapter.Get();
		}

		public PXAction<INRegister> iNRegisterDetails;
		[PXLookupButton(CommitChanges = true)]
		[PXUIField(DisplayName = Messages.INRegisterDetails, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable INRegisterDetails(PXAdapter adapter)
		{
			if (INRegisterDataMember.Current is INRegister doc)
			{
				var parameters = new Dictionary<string, string>
				{
					[nameof(INRegister.DocType)] = doc.DocType,
					[nameof(INRegister.RefNbr)] = doc.RefNbr,
					["PeriodID"] = (string)INRegisterDataMember.GetValueExt<INRegister.finPeriodID>(doc)
				};
				throw new PXReportRequiredException(parameters, "IN614000",
					PXBaseRedirectException.WindowMode.New, Messages.INRegisterDetails);
			}
			return adapter.Get();
		}
		#endregion

		#region Event Handlers
		#region INRegister
		protected virtual void _(Events.RowUpdated<INRegister> e)
		{
			bool tranDateChanged = !e.Cache.ObjectsEqual<INRegister.tranDate>(e.Row, e.OldRow);
			bool releasedFromHold = !e.Cache.ObjectsEqual<INRegister.hold>(e.Row, e.OldRow) && e.Row.Hold == false;
			if (tranDateChanged || releasedFromHold && e.Row.OrigModule == BatchModule.IN)
			{
				foreach (INTran tran in PXParentAttribute.SelectChildren(INTranDataMember.Cache, e.Row, typeof(INRegister)))
				{
					if (tranDateChanged || IsNotAllowedZeroTran(tran))
					{
						INTranDataMember.Cache.MarkUpdated(tran);
					}
				}
			}
			if (tranDateChanged)
			{
				foreach (INTranSplit split in PXParentAttribute.SelectChildren(INTranSplitDataMember.Cache, e.Row, typeof(INRegister)))
				{
					INTranSplitDataMember.Cache.MarkUpdated(split);
				}
			}
		}
		#endregion

		#region INTran
		protected virtual void _(Events.FieldDefaulting<INTran, INTran.invtMult> e) => e.NewValue = INTranType.InvtMult(e.Row.TranType);

		protected virtual void _(Events.FieldUpdated<INTran, INTran.uOM> e) => DefaultUnitCost(e.Cache, e.Row);
		protected virtual void _(Events.FieldUpdated<INTran, INTran.siteID> e) => DefaultUnitCost(e.Cache, e.Row);
		protected virtual void _(Events.FieldUpdated<INTran, INTran.inventoryID> e)
		{
			e.Cache.SetDefaultExt<INTran.uOM>(e.Row);
			e.Cache.SetDefaultExt<INTran.tranDesc>(e.Row);
		}
		protected virtual void _(Events.RowInserted<INTran> e)
        {
			if (e.Row.SortOrder == null)
				e.Row.SortOrder = e.Row.LineNbr;
        }

		protected virtual void _(Events.RowPersisting<INTran> e)
		{
			if (e.Operation.Command().IsIn(PXDBOperation.Insert, PXDBOperation.Update)
				&& IsNotAllowedZeroTran(e.Row))
			{
				var doc = INRegisterDataMember.Current ?? PXParentAttribute.SelectParent<INRegister>(e.Cache, e.Row);
				if (doc.Hold == false)
				{
					e.Cache.RaiseExceptionHandling<INTran.qty>(e.Row, e.Row.Qty,
						new PXSetPropertyException(CS.Messages.Entry_GT, 0.ToString()));
				}
			}
		}

		protected virtual void SetProjectEnabled(Events.RowSelected<INTran> e)
		{
			if (e.Row == null)
				return;

			var from = IsProjectTaskEnabled(e.Row);

			if (from.Project != null)
				PXUIFieldAttribute.SetEnabled<INTran.projectID>(e.Cache, e.Row, (bool)from.Project);

			if (from.Task != null)
				PXUIFieldAttribute.SetEnabled<INTran.taskID>(e.Cache, e.Row, (bool)from.Task);

			var to = IsToProjectTaskEnabled(e.Row);

			if (to.ToProject != null)
				PXUIFieldAttribute.SetEnabled<INTran.toProjectID>(e.Cache, e.Row, (bool)to.ToProject);

			if (to.ToTask != null)
				PXUIFieldAttribute.SetEnabled<INTran.toTaskID>(e.Cache, e.Row, (bool)to.ToTask);
		}

		protected virtual (bool? Project, bool? Task) IsProjectTaskEnabled(INTran row)
			=> (null, null);

		protected virtual (bool? ToProject, bool? ToTask) IsToProjectTaskEnabled(INTran row)
			=> (null, null);

		protected virtual bool IsNotAllowedZeroTran(INTran row)
		{
			return row?.OrigModule == BatchModule.IN && row.Qty <= 0m
				&& (row.DocType.IsIn(INDocType.Issue, INDocType.Transfer)
				|| row.DocType == INDocType.Receipt && row.OrigDocType != INDocType.Transfer);
		}

		#endregion
		#endregion

		public PXWorkflowEventHandler<INRegister> OnDocumentReleased;

		protected virtual void OnForeignTranInsert(INTran foreignTran)
		{
			INRegister doc = PXParentAttribute.SelectParent<INRegister>(INTranDataMember.Cache, foreignTran);
			if (doc != null)
			{
				PXCache cache = INRegisterDataMember.Cache;
				object copy = cache.CreateCopy(doc);

				doc.SOShipmentType = foreignTran.SOShipmentType;
				doc.SOShipmentNbr = foreignTran.SOShipmentNbr;

				doc.SOOrderType = foreignTran.SOOrderType;
				doc.SOOrderNbr = foreignTran.SOOrderNbr;

				doc.POReceiptType = foreignTran.POReceiptType;
				doc.POReceiptNbr = foreignTran.POReceiptNbr;

				if (object.Equals(doc, cache.Current))
				{
					if (cache.GetStatus(doc).IsIn(PXEntryStatus.Notchanged, PXEntryStatus.Held))
						cache.MarkUpdated(doc, assertError: true);
					cache.RaiseRowUpdated(doc, copy);
				}
				else
				{
					cache.Update(doc);
				}
			}
		}

		protected void FillControlValue<TControlField, TTotalField>(PXCache cache, INRegister document)
			where TControlField : IBqlField, IImplement<IBqlDecimal>
			where TTotalField : IBqlField, IImplement<IBqlDecimal>
		{
			decimal? total = (decimal?)cache.GetValue<TTotalField>(document);
			if (CM.PXCurrencyAttribute.IsNullOrEmpty(total))
				cache.SetValue<TControlField>(document, 0m);
			else
				cache.SetValue<TControlField>(document, total);
		}

		protected void RaiseControlValueError<TControlField, TTotalField>(PXCache cache, INRegister document)
			where TControlField : IBqlField, IImplement<IBqlDecimal>
			where TTotalField : IBqlField, IImplement<IBqlDecimal>
		{
			decimal? control = (decimal?)cache.GetValue<TControlField>(document);
			decimal? total = (decimal?)cache.GetValue<TTotalField>(document);
			if (total != control)
				cache.RaiseExceptionHandling<TControlField>(document, control, new PXSetPropertyException(Messages.DocumentOutOfBalance));
			else
				cache.RaiseExceptionHandling<TControlField>(document, control, null);
		}

		public virtual void DefaultUnitCost(PXCache cache, INTran tran, bool setZero = false)
			=> DefaultUnitAmount<INTran.unitCost>(cache, tran, setZero);

		public virtual void DefaultUnitPrice(PXCache cache, INTran tran)
			=> DefaultUnitAmount<INTran.unitPrice>(cache, tran);

		protected virtual void DefaultUnitAmount<TUnitAmount>(PXCache cache, INTran tran, bool setZero = false)
			where TUnitAmount : IBqlField, IImplement<IBqlDecimal>
		{
			cache.RaiseFieldDefaulting<TUnitAmount>(tran, out object unitAmount);
			if (unitAmount is decimal amount && (amount != 0m || setZero))
			{
				decimal? unitamount = INUnitAttribute.ConvertToBase<INTran.inventoryID>(cache, tran, tran.UOM, amount, INPrecision.UNITCOST);
				cache.SetValueExt<TUnitAmount>(tran, unitamount);
			}
		}

		public CostCenterDispatcher CostCenterDispatcherExt => FindImplementation<CostCenterDispatcher>();

		#region PXImportAttribute.IPXPrepareItems implementation

		public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values) => true;
		public virtual bool RowImporting(string viewName, object row) => row == null;
		public virtual bool RowImported(string viewName, object row, object oldRow) => oldRow == null;
		public virtual void PrepareItems(string viewName, IEnumerable items) { }
		#endregion
	}

	public abstract class INSiteStatusLookupExt<TGraph, TItemInfo> : SiteStatusLookupExt<TGraph, INRegister, INTran, TItemInfo, INSiteStatusFilter>
		where TGraph : INRegisterEntryBase
		where TItemInfo : class, IQtySelectable, IPXSelectable, IBqlTable, new()
	{
		protected PXSelectBase<INRegister> Document => Base.INRegisterDataMember;
		protected PXSelectBase<INTran> Transactions => Base.INTranDataMember;
		protected PXSelectBase<INTranSplit> Splits => Base.INTranSplitDataMember;
		protected PXSelectBase<INTran> LSSelect => Base.LSSelectDataMember;

		protected override INTran CreateNewLine(TItemInfo line)
		{
			INTran newline = PXCache<INTran>.CreateCopy(Transactions.Insert(new INTran()));
			newline = InitTran(newline, line);
			newline.Qty = (decimal?)Base.Caches<TItemInfo>().GetValue(line, QtySelected) ?? 0m;
			return Transactions.Update(newline);
		}

		protected abstract INTran InitTran(INTran newTran, TItemInfo siteStatus);
		protected abstract bool IsAddItemEnabled(INRegister doc);

		protected virtual void _(Events.RowSelected<INRegister> args)
		{
			if (args.Row != null)
			{
				bool isEnabled = IsAddItemEnabled(args.Row);
				showItems.SetEnabled(isEnabled);
				addSelectedItems.SetEnabled(isEnabled);
			}
		}

		protected virtual void _(Events.RowInserted<INSiteStatusFilter> args)
		{
			if (args.Row != null && Document.Current != null)
				args.Row.SiteID = Document.Current.SiteID;
		}
	}

	public abstract class INSiteStatusLookupExt<TGraph> : INSiteStatusLookupExt<TGraph, INSiteStatusSelected>
		where TGraph : INRegisterEntryBase
	{
		protected override INTran InitTran(INTran newTran, INSiteStatusSelected siteStatus)
		{
			newTran.SiteID = siteStatus.SiteID ?? newTran.SiteID;
			newTran.InventoryID = siteStatus.InventoryID;
			newTran.SubItemID = siteStatus.SubItemID;
			newTran.UOM = siteStatus.BaseUnit;
			newTran = PXCache<INTran>.CreateCopy(Transactions.Update(newTran));
			if (siteStatus.LocationID != null)
			{
				newTran.LocationID = siteStatus.LocationID;
				newTran = PXCache<INTran>.CreateCopy(Transactions.Update(newTran));
			}
			return newTran;
		}
	}
}

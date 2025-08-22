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

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated;
using PX.Objects.SO.DAC.Projections;
using PX.Objects.SO.DAC.Unbound;
using System;
using System.Collections;
using System.Linq;

namespace PX.Objects.SO.GraphExtensions.SOOrderEntryExt
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class AddInvoiceExt : PXGraphExtension<SOOrderEntry>
	{
		#region Overrides

		public override void Initialize()
		{
			base.Initialize();

			invoiceSplits.Cache.AdjustUI().ForAllFields(a => a.Enabled = false);
			invoiceSplits.Cache.AdjustUI().For<InvoiceSplit.selected>(a => a.Enabled = true)
				.SameFor<InvoiceSplit.qtyToReturn>();
		}

		#endregion // Overrides

		#region Views

		[PXCopyPasteHiddenView()]
		public SelectFrom<InvoiceSplit>.
			Where<InvoiceSplit.customerID.IsEqual<SOOrder.customerID.FromCurrent.NoDefault>.
				And<InvoiceSplit.aRlineType.IsEqual<InvoiceSplit.sOlineType>>.
				And<InvoiceSplit.sOOrderNbr.IsNotNull>.
				And<AddInvoiceFilter.aRRefNbr.FromCurrent.NoDefault.IsNotNull
					.Or<AddInvoiceFilter.orderNbr.FromCurrent.NoDefault.IsNotNull>
					.Or<AddInvoiceFilter.inventoryID.FromCurrent.NoDefault.IsNotNull>>.
				And<InvoiceSplit.aRDocType.IsEqual<AddInvoiceFilter.aRDocType.FromCurrent.NoDefault>
					.Or<AddInvoiceFilter.aRDocType.FromCurrent.NoDefault.IsNull>>.
				And<InvoiceSplit.aRRefNbr.IsEqual<AddInvoiceFilter.aRRefNbr.FromCurrent.NoDefault>
					.Or<AddInvoiceFilter.aRRefNbr.FromCurrent.NoDefault.IsNull>>.
				And<InvoiceSplit.sOOrderType.IsEqual<AddInvoiceFilter.orderType.FromCurrent.NoDefault>
					.Or<AddInvoiceFilter.orderType.FromCurrent.NoDefault.IsNull>>.
				And<InvoiceSplit.sOOrderNbr.IsEqual<AddInvoiceFilter.orderNbr.FromCurrent.NoDefault>
					.Or<AddInvoiceFilter.orderNbr.FromCurrent.NoDefault.IsNull>>.
				And<InvoiceSplit.inventoryID.IsEqual<AddInvoiceFilter.inventoryID.FromCurrent.NoDefault>
					.Or<AddInvoiceFilter.inventoryID.FromCurrent.NoDefault.IsNull>>.
				And<InvoiceSplit.lotSerialNbr.IsEqual<AddInvoiceFilter.lotSerialNbr.FromCurrent.NoDefault>
					.Or<AddInvoiceFilter.lotSerialNbr.FromCurrent.NoDefault.IsNull>>.
				And<InvoiceSplit.aRTranDate.IsGreaterEqual<AddInvoiceFilter.startDate.FromCurrent.NoDefault>
					.Or<AddInvoiceFilter.startDate.FromCurrent.NoDefault.IsNull>>.
				And<InvoiceSplit.aRTranDate.IsLessEqual<AddInvoiceFilter.endDate.FromCurrent.NoDefault>
					.Or<AddInvoiceFilter.endDate.FromCurrent.NoDefault.IsNull>>>
			.OrderBy<InvoiceSplit.aRTranDate.Desc, InvoiceSplit.inventoryID.Asc, InvoiceSplit.subItemID.Asc>
			.View invoiceSplits;

		public PXFilter<AddInvoiceFilter> AddInvoiceFilter;

		#endregion // Views

		#region Actions

		[PXUIField(DisplayName = Messages.AddInvoice, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton()]
		public virtual IEnumerable AddInvoice(PXAdapter adapter)
		{
			try
			{
				AddInvoiceProc();
			}
			finally
			{
				invoiceSplits.Cache.Clear();
				invoiceSplits.View.Clear();
			}

			return adapter.Get();
		}

		public PXAction<SOOrder> addInvoiceOK;
		[PXUIField(
			DisplayName = "Add",
			MapEnableRights = PXCacheRights.Select,
			MapViewRights = PXCacheRights.Select)]
		[PXLookupButton(DisplayOnMainToolbar = false)]
		public virtual IEnumerable AddInvoiceOK(PXAdapter adapter)
		{
			invoiceSplits.View.Answer = WebDialogResult.OK;

			return AddInvoice(adapter);
		}

		#endregion // Actions

		#region Event Handlers
		#region AddInvoiceFilter
		protected virtual void _(Events.FieldDefaulting<AddInvoiceFilter, AddInvoiceFilter.startDate> e)
		{
			const int DefaultStartDateDays = -90;
			e.NewValue = Base.Accessinfo.BusinessDate?.AddDays(DefaultStartDateDays);
		}

		protected virtual void _(Events.FieldUpdated<AddInvoiceFilter, AddInvoiceFilter.inventoryID> e)
		{
			e.Cache.SetValueExt<AddInvoiceFilter.lotSerialNbr>(e.Row, null);
		}

		protected virtual void _(Events.FieldUpdated<AddInvoiceFilter, AddInvoiceFilter.aRRefNbr> e)
		{
			if (e.Row.ARDocType != null && e.Row.ARRefNbr != null)
			{
				var invoice = ARRegister.PK.Find(Base, e.Row.ARDocType, e.Row.ARRefNbr);

				if (invoice?.DocDate < e.Row.StartDate)
					e.Cache.SetValueExt<AddInvoiceFilter.startDate>(e.Row, null);

				if (invoice?.DocDate > e.Row.EndDate)
					e.Cache.SetValueExt<AddInvoiceFilter.endDate>(e.Row, null);
			}
		}

		protected virtual void _(Events.FieldUpdated<AddInvoiceFilter, AddInvoiceFilter.orderNbr> e)
		{
			if (e.Row.OrderType != null && e.Row.OrderNbr != null)
			{
				if (e.Row.StartDate != null)
				{
					ARTran tranStartDate = SelectFrom<ARTran>
						.InnerJoin<ARRegister>.On<ARTran.tranType.IsEqual<ARRegister.docType>.And<ARTran.refNbr.IsEqual<ARRegister.refNbr>>>
						.Where<ARTran.sOOrderType.IsEqual<AddInvoiceFilter.orderType.FromCurrent>
							.And<ARTran.sOOrderNbr.IsEqual<AddInvoiceFilter.orderNbr.FromCurrent>>
							.And<ARRegister.docDate.IsLess<AddInvoiceFilter.startDate.FromCurrent>>>
						.View.ReadOnly.Select(Base);

					if (tranStartDate != null)
						e.Cache.SetValueExt<AddInvoiceFilter.startDate>(e.Row, null);
				}

				if (e.Row.EndDate != null)
				{
					ARTran tranEndDate = SelectFrom<ARTran>
						.InnerJoin<ARRegister>.On<ARTran.tranType.IsEqual<ARRegister.docType>.And<ARTran.refNbr.IsEqual<ARRegister.refNbr>>>
						.Where<ARTran.sOOrderType.IsEqual<AddInvoiceFilter.orderType.FromCurrent>
							.And<ARTran.sOOrderNbr.IsEqual<AddInvoiceFilter.orderNbr.FromCurrent>>
							.And<ARRegister.docDate.IsGreater<AddInvoiceFilter.endDate.FromCurrent>>>
						.View.ReadOnly.Select(Base);

					if (tranEndDate != null)
						e.Cache.SetValueExt<AddInvoiceFilter.endDate>(e.Row, null);
				}
			}
		}

		protected virtual void _(Events.RowUpdated<AddInvoiceFilter> e)
		{
			invoiceSplits.Cache.Clear();
			invoiceSplits.Cache.ClearQueryCache();
		}

		protected virtual void _(Events.RowSelected<AddInvoiceFilter> e)
		{
			if (e.Row == null)
				return;

			var lotClassID = e.Row.InventoryID == null ? null : InventoryItem.PK.Find(Base, e.Row.InventoryID)?.LotSerClassID;
			var lotEnabled = lotClassID != null && INLotSerClass.PK.Find(Base, lotClassID)?.LotSerTrack != INLotSerTrack.NotNumbered;

			e.Cache.AdjustUI().For<AddInvoiceFilter.lotSerialNbr>(a => a.Enabled = lotEnabled);

			invoiceSplits.Cache.AdjustUI().For<InvoiceSplit.componentID>(a => a.Visible = e.Row.Expand == true)
				.SameFor<InvoiceSplit.componentDesc>();
		}
		#endregion // AddInvoiceFilter
		#region InvoiceSplit
		protected virtual void _(Events.RowSelecting<InvoiceSplit> e)
		{
			if (e.Row?.ARRefNbr == null)
				return;

			// Acuminator disable once PX1042 DatabaseQueriesInRowSelecting The platform's already supported selects in RowSelecting without connection scope.
			// Acuminator disable once PX1075 RaiseExceptionHandlingInEventHandlers MemoCheck is executed with raiseException=false.
			CalculateQtyAvail(e.Row);
		}

		protected virtual void _(Events.FieldVerifying<InvoiceSplit, InvoiceSplit.qtyToReturn> e)
		{
			if ((decimal?)e.NewValue > e.Row.QtyAvailForReturn)
				throw new PXSetPropertyException(Messages.QtyToReturnGreaterQtyAvail);
		}

		protected void _(Events.FieldUpdated<InvoiceSplit, InvoiceSplit.selected> e)
		{
			if (e.Row.Selected == true && e.Row.QtyToReturn == 0m && e.Row.QtyAvailForReturn > 0m)
				e.Cache.SetValueExt<InvoiceSplit.qtyToReturn>(e.Row, e.Row.QtyAvailForReturn ?? 0m);

			if (e.Row.Selected != true && e.Row.QtyToReturn != 0m)
				e.Cache.SetValueExt<InvoiceSplit.qtyToReturn>(e.Row, 0m);
		}

		protected virtual void _(Events.FieldUpdated<InvoiceSplit, InvoiceSplit.qtyToReturn> e)
		{
			if (e.Row.QtyToReturn > 0m && e.Row.Selected != true)
				e.Cache.SetValueExt<InvoiceSplit.selected>(e.Row, true);

			if (e.Row.QtyToReturn == 0m && e.Row.Selected == true)
				e.Cache.SetValueExt<InvoiceSplit.selected>(e.Row, false);
		}

		protected virtual void _(Events.RowSelected<InvoiceSplit> e)
		{
			if (e.Row == null)
				return;

			PXSetPropertyException selectedException = null;
			PXSetPropertyException availException = null;

			var item = InventoryItem.PK.Find(Base, e.Row.ComponentID ?? e.Row.InventoryID);
			if (e.Row.QtyAvailForReturn == 0m)
			{
				var inventoryCD = item?.InventoryCD?.TrimEnd();

				if (e.Row.SerialIsOnHand == true)
				{
					availException = new PXSetPropertyException(Messages.SerialNumberIsAlreadyOnHand,
						PXErrorLevel.Warning, inventoryCD, e.Row.LotSerialNbr);
				}
				else if (e.Row.SerialIsAlreadyReceived == true)
				{
					availException = new PXSetPropertyException(e.Row.SerialIsAlreadyReceivedRef == null ?
						IN.Messages.SerialNumberAlreadyReceived : IN.Messages.SerialNumberAlreadyReceivedIn, PXErrorLevel.Warning,
						inventoryCD,
						e.Row.LotSerialNbr,
						e.Row.SerialIsAlreadyReceivedRef);
				}

				if (e.Row.Selected == true)
				{
					selectedException = availException ??
						new PXSetPropertyException(Messages.TheItemHasBeenFullyReturned, PXErrorLevel.Warning, inventoryCD);
				}
			}

			e.Cache.RaiseExceptionHandling<InvoiceSplit.selected>(e.Row, true, selectedException);
			e.Cache.RaiseExceptionHandling<InvoiceSplit.qtyAvailForReturn>(e.Row, 0, availException);

			if (item?.ItemStatus.IsIn(INItemStatus.Inactive, INItemStatus.ToDelete) == true)
			{
				e.Cache.AdjustUI(e.Row).For<InvoiceSplit.selected>(a => a.Enabled = false)
					.SameFor<InvoiceSplit.qtyToReturn>();
			}
		}

		protected virtual void _(Events.FieldVerifying<InvoiceSplit, InvoiceSplit.inventoryID> e)
		{
			e.Cancel = true;
		}

		#endregion // InvoiceSplit
		#endregion // Event Handlers

		#region Methods

		protected virtual void AddInvoiceProc()
		{
			Base.Transactions.Cache.ForceExceptionHandling = true;
			SOOrder order = Base.Document.Current;
			if ((order?.IsCreditMemoOrder == true || order?.IsRMAOrder == true || order?.IsMixedOrder == true)
				&& Base.Transactions.Cache.AllowInsert && invoiceSplits.AskExt() == WebDialogResult.OK)
			{
				foreach (InvoiceSplit split in invoiceSplits.Cache.Cached.RowCast<InvoiceSplit>().Where(s => s.Selected == true))
				{
					AddInvoiceProc(split);
				}
			}

			ClearInvoiceFilter();
		}

		protected virtual void AddInvoiceProc(InvoiceSplit split)
		{
			SOLine origLine = SOLine.PK.Find(Base, split.SOOrderType, split.SOOrderNbr, split.SOLineNbr);
			ARTran artran = ARTran.PK.Find(Base, split.ARDocType, split.ARRefNbr, split.ARLineNbr);
			ARRegister invoice = ARRegister.PK.Find(Base, split.ARDocType, split.ARRefNbr);

			SOLine existing = FindExistingSOLine(split);

			if (existing != null)
			{
				Base.Transactions.Current = existing;
			}
			else
			{
				INTran tran = split.INDocType == InvoiceSplit.iNDocType.EmptyDoc ? null :
					INTran.PK.Find(Base, split.INDocType, split.INRefNbr, split.INLineNbr);

				SOLine newline = (SOLine)Base.Transactions.Cache.CreateInstance();
				FillSOLine(newline, split);
				FillSOLine(newline, tran, origLine, artran, invoice);
				newline = Base.Transactions.Insert(newline);

				newline.Operation = SOOperation.Receipt;
				existing = newline = Base.Transactions.Update(newline);

				UpdateSOSalesPerTran(split);
				ClearSplits(newline);
			}

			SOLine copy = PXCache<SOLine>.CreateCopy(existing);
			copy = IncreaseQty(split, artran, invoice, copy);
			copy = CopyDiscount(artran, invoice, copy);
		}

		protected virtual void ClearInvoiceFilter()
		{
			if (AddInvoiceFilter.Current != null)
			{
				if (Base.IsImport)
				{
					AddInvoiceFilter.Current.ARRefNbr = null;
					AddInvoiceFilter.Current.InventoryID = null;
					AddInvoiceFilter.Current.LotSerialNbr = null;
					AddInvoiceFilter.Current.OrderNbr = null;
				}
				else
				{
					AddInvoiceFilter.Current = null;
				}
			}
		}

		protected virtual SOLine FindExistingSOLine(InvoiceSplit split)
		{
			return PXSelect<SOLine,
				Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>,
					And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>,
					And<SOLine.origOrderType, Equal<Required<SOLine.origOrderType>>,
					And<SOLine.origOrderNbr, Equal<Required<SOLine.origOrderNbr>>,
					And<SOLine.origLineNbr, Equal<Required<SOLine.origLineNbr>>,
					And<SOLine.inventoryID, Equal<Required<SOLine.inventoryID>>,
					And<SOLine.invoiceType, Equal<Required<SOLine.invoiceType>>,
					And<SOLine.invoiceNbr, Equal<Required<SOLine.invoiceNbr>>,
					And<SOLine.invoiceLineNbr, Equal<Required<SOLine.invoiceLineNbr>>>>>>>>>>>>
				.Select(Base, split.SOOrderType, split.SOOrderNbr, split.SOLineNbr,
					split.ComponentID ?? split.InventoryID, split.ARDocType, split.ARRefNbr, split.ARLineNbr);
		}

		protected virtual void FillSOLine(SOLine newline, InvoiceSplit split)
		{
			newline.InvoiceType = split.ARDocType;
			newline.InvoiceNbr = split.ARRefNbr;
			newline.InvoiceLineNbr = split.ARLineNbr;
			newline.OrigOrderType = split.SOOrderType;
			newline.OrigOrderNbr = split.SOOrderNbr;
			newline.OrigLineNbr = split.SOLineNbr;
			newline.SalesPersonID = split.SalesPersonID;
			newline.InventoryID = split.ComponentID ?? split.InventoryID;
			newline.SubItemID = split.SubItemID;
			newline.SiteID = split.SiteID;
			newline.LotSerialNbr = split.LotSerialNbr;
			newline.UOM = split.UOM;
		}

		protected virtual void FillSOLine(SOLine newline, INTran tran, SOLine origLine, ARTran artran, ARRegister invoice)
		{
			newline.BranchID = origLine.BranchID;
			newline.InvoiceDate = artran.TranDate;
			newline.OrigShipmentType = artran.SOShipmentType;
			newline.SalesAcctID = null;
			newline.SalesSubID = null;
			newline.TaxCategoryID = origLine.TaxCategoryID;
			newline.Commissionable = artran.Commissionable;
			newline.IsSpecialOrder = origLine.IsSpecialOrder;
			newline.CostCenterID = origLine.CostCenterID;

			newline.ManualPrice = true;
			newline.ManualDisc = true;

			newline.UOM = (newline.InventoryID == artran.InventoryID) ? artran.UOM : newline.UOM;
			newline.InvoiceUOM = newline.UOM;
			newline.CuryInfoID = Base.Document.Current.CuryInfoID;

			if (artran?.AvalaraCustomerUsageType != null)
			{
				newline.AvalaraCustomerUsageType = artran.AvalaraCustomerUsageType;
			}

			if (origLine.LineType == SOLineType.MiscCharge || origLine.LineType == SOLineType.NonInventory || tran == null)
			{
				if (newline.InventoryID == artran.InventoryID)
					newline.UnitCost = artran.BaseQty > 0m ? (artran.TranCost / artran.BaseQty) : artran.TranCost;
			}
			else
			{
				if (newline.InventoryID == tran.InventoryID)
					newline.UnitCost = tran.Qty > 0m ? (tran.TranCost / tran.Qty) : tran.TranCost;
			}

			newline.DiscPctDR = artran.DiscPctDR;
			if (artran.CuryUnitPriceDR != null && invoice != null)
			{
				if (Base.Document.Current.CuryID == invoice.CuryID)
				{
					newline.CuryUnitPriceDR = artran.CuryUnitPriceDR;
				}
				else
				{
					decimal unitPriceDR = 0m;
					PXDBCurrencyAttribute.CuryConvBase(Base.Caches[typeof(ARTran)], artran, artran.CuryUnitPriceDR ?? 0m, out unitPriceDR, true);

					decimal orderCuryUnitPriceDR = 0m;
					PXDBCurrencyAttribute.CuryConvCury(Base.Transactions.Cache, newline, unitPriceDR, out orderCuryUnitPriceDR, CommonSetupDecPl.PrcCst);
					newline.CuryUnitPriceDR = orderCuryUnitPriceDR;
				}
			}

			newline.DRTermStartDate = artran.DRTermStartDate;
			newline.DRTermEndDate = artran.DRTermEndDate;

			newline.ReasonCode = origLine.ReasonCode;
			newline.TaskID = artran.TaskID;
			newline.CostCodeID = artran.CostCodeID;

			if (!string.IsNullOrEmpty(artran.DeferredCode))
			{
				DRSchedule drSchedule = null;
				DRSetup dRSetup = new PXSelect<DRSetup>(Base).Select();
				if (PXAccess.FeatureInstalled<FeaturesSet.aSC606>())
				{
					drSchedule = PXSelectReadonly<DRSchedule,
						Where<DRSchedule.module, Equal<BatchModule.moduleAR>,
							And<DRSchedule.docType, Equal<Required<ARTran.tranType>>,
							And<DRSchedule.refNbr, Equal<Required<ARTran.refNbr>>>>>>
						.Select(Base, artran.TranType, artran.RefNbr);
				}
				else
				{
					drSchedule = PXSelectReadonly<DRSchedule,
					Where<DRSchedule.module, Equal<BatchModule.moduleAR>,
						And<DRSchedule.docType, Equal<Required<ARTran.tranType>>,
						And<DRSchedule.refNbr, Equal<Required<ARTran.refNbr>>,
						And<DRSchedule.lineNbr, Equal<Required<ARTran.lineNbr>>>>>>>
					.Select(Base, artran.TranType, artran.RefNbr, artran.LineNbr);
				}
				if (drSchedule != null)
				{
					newline.DefScheduleID = drSchedule.ScheduleID;
				}
			}

			decimal CuryUnitCost;
			PXDBCurrencyAttribute.CuryConvCury(Base.Transactions.Cache, newline, (decimal)newline.UnitCost, out CuryUnitCost, CommonSetupDecPl.PrcCst);
			newline.CuryUnitCost = CuryUnitCost;

			if (invoice != null && newline.InventoryID == artran.InventoryID)
			{
				if (Base.Document.Current.CuryID == invoice.CuryID)
				{
					decimal UnitPrice;
					PXDBCurrencyAttribute.CuryConvBase(Base.Transactions.Cache, newline, (decimal)artran.CuryUnitPrice, out UnitPrice, CommonSetupDecPl.PrcCst);
					newline.CuryUnitPrice = artran.CuryUnitPrice;
					newline.UnitPrice = UnitPrice;
				}
				else
				{
					decimal CuryUnitPrice;
					PXDBCurrencyAttribute.CuryConvCury(Base.Transactions.Cache, newline, (decimal)artran.UnitPrice, out CuryUnitPrice, CommonSetupDecPl.PrcCst);
					newline.CuryUnitPrice = CuryUnitPrice;
					newline.UnitPrice = artran.UnitPrice;
				}
			}

			newline.SkipLineDiscounts = artran.SkipLineDiscounts;
		}

		protected virtual void ClearSplits(SOLine newline)
		{
			Base.LineSplittingExt.RaiseRowDeleted(newline);
		}

		protected virtual void UpdateSOSalesPerTran(InvoiceSplit split)
		{
			var pertran = SOSalesPerTran.PK.Find(Base, split.SOOrderType, split.SOOrderNbr, split.SalesPersonID);
			if (Base.SalesPerTran.Current != null && Base.SalesPerTran.Cache.ObjectsEqual<SOSalesPerTran.salespersonID>(pertran, Base.SalesPerTran.Current))
			{
				SOSalesPerTran salespertran_copy = PXCache<SOSalesPerTran>.CreateCopy(Base.SalesPerTran.Current);
				Base.SalesPerTran.Cache.SetValueExt<SOSalesPerTran.commnPct>(Base.SalesPerTran.Current, pertran.CommnPct);
				Base.SalesPerTran.Cache.RaiseRowUpdated(Base.SalesPerTran.Current, salespertran_copy);
			}
		}

		protected virtual SOLine IncreaseQty(InvoiceSplit split, ARTran artran, ARRegister invoice, SOLine soline)
		{
			INTranSplit inSplit = (split.INDocType == InvoiceSplit.iNDocType.EmptyDoc) ? null :
				INTranSplit.PK.Find(Base, split.INDocType, split.INRefNbr, split.INLineNbr, split.INSplitLineNbr);

			bool processSplits = inSplit != null &&
				(Base.LineSplittingExt.IsLSEntryEnabled || Base.LineSplittingAllocatedExt.IsAllocationEntryEnabled) &&
				(!string.IsNullOrEmpty(split.LotSerialNbr) || Base.LineSplittingExt.IsLocationEnabled);

			if (processSplits && split.QtyToReturn == 0m)
			{
				var item = InventoryItem.PK.Find(Base, soline.InventoryID);
				var lotClass = INLotSerClass.PK.Find(Base, item?.LotSerClassID);

				if (lotClass?.LotSerTrack == INLotSerTrack.SerialNumbered)
					processSplits = false;
			}

			if (!processSplits)
			{
				if (split.UOM == soline.UOM)
				{
					soline.Qty += soline.LineSign * split.QtyToReturn;
				}
				else
				{
					decimal baseQty = INUnitAttribute.ConvertToBase(
						invoiceSplits.Cache, soline.InventoryID, split.UOM, split.QtyToReturn ?? 0m, INPrecision.NOROUND);

					soline.Qty += soline.LineSign * INUnitAttribute.ConvertFromBase<SOLine.inventoryID>(
						Base.Transactions.Cache, soline, soline.UOM, baseQty, INPrecision.QUANTITY);
				}
			}
			else
			{
				if (Base.Document.Current.CuryID == invoice.CuryID)
				{
					decimal LineAmt;
					PXDBCurrencyAttribute.CuryConvBase<SOLine.curyInfoID>(Base.Transactions.Cache, soline, (decimal)artran.CuryTranAmt, out LineAmt);
					soline.CuryLineAmt = artran.CuryTranAmt;
					soline.LineAmt = LineAmt;
				}
				else
				{
					decimal CuryLineAmt;
					PXDBCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(Base.Transactions.Cache, soline, (decimal)artran.TranAmt, out CuryLineAmt);
					soline.CuryLineAmt = CuryLineAmt;
					soline.LineAmt = artran.TranAmt;
				}
			}

			try
			{
				soline = Base.Transactions.Update(soline);
			}
			catch (PXSetPropertyException) {; }

			if (processSplits)
			{
				SOLineSplit newsplit = new SOLineSplit();
				newsplit.SubItemID = inSplit.SubItemID;
				if (Base.LineSplittingExt.IsLocationEnabled)
					newsplit.LocationID = inSplit.LocationID;
				newsplit.LotSerialNbr = inSplit.LotSerialNbr;
				newsplit.ExpireDate = inSplit.ExpireDate;
				newsplit.UOM = inSplit.UOM;

				newsplit = Base.splits.Insert(newsplit);
				newsplit.Qty = split.QtyToReturn;
				newsplit = Base.splits.Update(newsplit);
				string error = PXUIFieldAttribute.GetError<SOLineSplit.qty>(Base.splits.Cache, newsplit);

				if (!string.IsNullOrEmpty(error))
				{
					newsplit.Qty = 0;
					newsplit = Base.splits.Update(newsplit);
				}
			}

			return soline;
		}

		protected virtual SOLine CopyDiscount(ARTran artran, ARRegister invoice, SOLine copy)
		{
			decimal DiscAmt;
			decimal CuryDiscAmt;

			if (Base.Document.Current.CuryID == invoice.CuryID)
			{
				PXDBCurrencyAttribute.CuryConvBase<SOLine.curyInfoID>(Base.Transactions.Cache, copy, (decimal)artran.CuryDiscAmt, out DiscAmt);
				CuryDiscAmt = (decimal)artran.CuryDiscAmt;
			}
			else
			{
				PXDBCurrencyAttribute.CuryConvCury<SOLine.curyInfoID>(Base.Transactions.Cache, copy, (decimal)artran.DiscAmt, out CuryDiscAmt);
				DiscAmt = (decimal)artran.DiscAmt;
			}

			if (artran.Qty != copy.Qty)
			{
				copy.CuryDiscAmt = (CuryDiscAmt / artran.Qty) * copy.Qty;
				copy.DiscAmt = (DiscAmt / artran.Qty) * copy.Qty;
			}
			else
			{
				copy.CuryDiscAmt = CuryDiscAmt;
				copy.DiscAmt = DiscAmt;
			}

			copy.DiscPct = artran.DiscPct;
			copy.FreezeManualDisc = true;

			try
			{
				copy = Base.Transactions.Update(copy);
			}
			catch (PXSetPropertyException) {; }

			if (artran.DiscountsAppliedToLine?.Any() == true)
			{
				Base.Transactions.Cache.RaiseExceptionHandling<SOLine.invoiceNbr>(copy, copy.InvoiceNbr, new PXSetPropertyException(Messages.DiscountsWereNotCopiedToReturnOrder, PXErrorLevel.RowWarning, copy.InvoiceNbr));
			}

			return copy;
		}

		protected virtual void CalculateQtyAvail(InvoiceSplit split)
		{
			if (split.QtyAvailForReturn == null || split.QtyReturned == null)
			{
				var inventoryID = split.ComponentID ?? split.InventoryID;

				var result = Base.ItemAvailabilityExt.MemoCheckQty(inventoryID,
					split.ARDocType, split.ARRefNbr, split.ARLineNbr, split.SOOrderType, split.SOOrderNbr, split.SOLineNbr, split);

				decimal qtyAvailForReturn = (result.Success ? result.QtyAvailForReturn : 0m) ?? 0m;

				if (qtyAvailForReturn > 0)
				{
					CalculateQtyAvail(split, Base.Document.Current, inventoryID, qtyAvailForReturn);
				}
				else
				{
					split.QtyAvailForReturn = 0m;
					split.QtyReturned = split.Qty;
				}
			}
		}

		protected virtual void CalculateQtyAvail(InvoiceSplit split, SOOrder order, int? inventoryID, decimal qtyAvailForReturn)
		{
			if (!string.IsNullOrEmpty(split.LotSerialNbr) && order != null)
			{
				var soline = new SOLine() { OrderType = order.OrderType, OrderNbr = order.OrderNbr, LineNbr = -1 };
				FillSOLine(soline, split);

				var lotResult = Base.ItemAvailabilityExt.MemoCheck(soline, soline, false, false);

				qtyAvailForReturn = Math.Min(lotResult.qtyAvailForReturn, qtyAvailForReturn);
			}

			var qtyAvailInCurrentUOM = INUnitAttribute.ConvertFromBase(Base.Transactions.Cache,
				inventoryID, split.UOM, qtyAvailForReturn, INPrecision.QUANTITY);

			split.QtyAvailForReturn = Math.Min(qtyAvailInCurrentUOM, split.Qty.Value);
			split.QtyReturned = split.Qty - split.QtyAvailForReturn;

			if (!string.IsNullOrEmpty(split.LotSerialNbr))
			{
				var lotSerClassID = InventoryItem.PK.Find(Base, inventoryID)?.LotSerClassID;
				var lotSerClass = INLotSerClass.PK.Find(Base, lotSerClassID);

				if (lotSerClass.LotSerTrack == INLotSerTrack.SerialNumbered && split.QtyAvailForReturn < 1)
					split.QtyAvailForReturn = 0;

				if (lotSerClass.LotSerTrack == INLotSerTrack.SerialNumbered && split.QtyAvailForReturn == 1)
				{
					var lotSerialStatus = INItemLotSerial.PK.Find(Base, inventoryID, split.LotSerialNbr);

					var accum = (ItemLotSerial)Base.Caches<ItemLotSerial>().Locate(
						new ItemLotSerial() { InventoryID = inventoryID, LotSerialNbr = split.LotSerialNbr });

					lotSerialStatus.QtyAvail += accum?.QtyAvail ?? 0m;
					lotSerialStatus.QtyHardAvail += accum?.QtyHardAvail ?? 0m;
					lotSerialStatus.QtyOnReceipt += accum?.QtyOnReceipt ?? 0m;

					bool serialIsOnHand = lotSerClass.LotSerAssign == INLotSerAssign.WhenReceived && lotSerialStatus.QtyOnHand > 0;
					bool serialIsAlreadyReceived = lotSerialStatus.QtyAvail > 0 || lotSerialStatus.QtyHardAvail > 0 || lotSerialStatus.QtyOnReceipt > 0;

					if (serialIsOnHand || serialIsAlreadyReceived)
						split.QtyAvailForReturn = 0m;

					if (!serialIsOnHand && serialIsAlreadyReceived)
					{
						INItemPlan plan = SelectFrom<INItemPlan>
							.Where<INItemPlan.inventoryID.IsEqual<@P.AsInt>.And<INItemPlan.lotSerialNbr.IsEqual<@P.AsString>>>
							.View.Select(Base, inventoryID, split.LotSerialNbr);

						if (plan != null)
						{
							split.SerialIsAlreadyReceivedRef = new EntityHelper(Base).GetEntityRowID(plan.RefNoteID);
						}
					}

					split.SerialIsOnHand = serialIsOnHand;
					split.SerialIsAlreadyReceived = serialIsAlreadyReceived;
				}
			}
		}

		#endregion // Methods
	}
}

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
using System.Collections;
using PX.Objects.IN;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Common;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.AM.Attributes;
using PX.Objects.AM.GraphExtensions;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AM.CacheExtensions;
using PX.Objects.PM;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.CR;
using PX.Objects.GL;

namespace PX.Objects.AM
{
    /// <summary>
    /// Manufacturing critical materials inquiry graph (Page: AM401000)
    /// </summary>
    public class CriticalMaterialsInq : PXGraph<CriticalMaterialsInq>
    {
		public class AMProdMatlSplitPlan : AMProdMatlSplitPlan<CriticalMaterialsInq> { }

        public PXFilter<ProdItemFilter> ProdItemRecs;

        public PXFilteredProcessingOrderBy<SelectedProdMatl, ProdItemFilter,
             OrderBy<Asc<SelectedProdMatl.operationID, Asc<SelectedProdMatl.sortOrder, Asc<SelectedProdMatl.lineID>>>>> ProdMatlRecs;

        [PXHidden]
        public PXSelect<AMProdMatl> MaterialUpdate;

        [PXHidden]
        public PXSelect<AMProdMatlSplit> MatlSplitUpdate;
        public PXFilter<TROrderFilter> TransferOrderFilter;
        public PXFilter<CreateProdFilter> CreateProductionOrderFilter;
        public PXFilter<CreatePurchaseOrdFilter> CreatePurchaseOrderFilter;

        public PXCancel<ProdItemFilter> Cancel;
        public PXAction<ProdItemFilter> FirstOrder;
        public PXAction<ProdItemFilter> PreviousOrder;
        public PXAction<ProdItemFilter> NextOrder;
        public PXAction<ProdItemFilter> LastOrder;

        public PXSetup<POSetup> POSetup;
        public PXSetup<AMPSetup> ProdSetup;
		public PXSetupOptional<SOSetup> SOSetup;

        [PXHidden]
        public PXSelectJoin<Numbering, LeftJoin<AMOrderType, On<AMOrderType.prodNumberingID, Equal<Numbering.numberingID>>>,
                    Where<AMOrderType.orderType, Equal<Current<AMOrderType.orderType>>>> ProductionNumbering;

        [PXHidden]
        public PXSelectJoin<Numbering, LeftJoin<SOOrderType, On<SOOrderType.orderNumberingID, Equal<Numbering.numberingID>>>,
                    Where<SOOrderType.orderType, Equal<Current<TROrderFilter.orderType>>>> TROrderTypeNumbering;

        [PXHidden]
        public PXSelect<Numbering, Where<Numbering.numberingID, Equal<Current<POSetup.regularPONumberingID>>>> PurchaseNumbering;

        public CriticalMaterialsInq()
        {
            var setup = ProdSetup.Current;
            AMPSetup.CheckSetup(setup);

            var poSetup = POSetup.Current;

            var currentFilter = ProdItemRecs.Current;
            var currentTROrderFilter = TransferOrderFilter.Current;
            var currentCreateProdFilter = CreateProductionOrderFilter.Current;
            var currentPurchaseOrderFilter = CreatePurchaseOrderFilter.Current;

            var trNumbering = CurrentTROrderTypeNumbering;
            var prodNumbering = CurrentProductionNumbering;

            ProdMatlRecs.SetProcessDelegate(
                delegate(List<SelectedProdMatl> list)
                {
#pragma warning disable PX1088 // Processing delegates cannot use the data views from processing graphs except for the data views of the PXFilter, PXProcessingBase, or PXSetup types
#if DEBUG
                    // OK to ignore PX1088 per Dmitry 11/19/2019 - Cause is use of MaterialUpdate and MatlSplitUpdate data views
#endif
                    if (currentFilter != null && currentTROrderFilter != null &&
                        currentFilter.RefType == XRefType.Transfer)
                    {
                        ProcessTransferOrder(currentTROrderFilter, list, trNumbering);
                        return;
                    }

                    if (currentFilter != null && currentCreateProdFilter != null &&
                        currentFilter.RefType == XRefType.Manufacture)
                    {
                        ProcessProductionOrders(currentFilter, currentCreateProdFilter, list, prodNumbering);
                        return;
                    }

                    if (currentFilter != null && currentPurchaseOrderFilter != null &&
                        currentFilter.RefType == XRefType.Purchase)
                    {
                        ProcessPOCreate(currentFilter, currentPurchaseOrderFilter, list);
                    }
                }
#pragma warning restore PX1088
            );

            InquiresDropMenu.AddMenuAction(InventorySummary);
            InquiresDropMenu.AddMenuAction(InventoryAllocationDetails);

            var warehouseEnabled = PXAccess.FeatureInstalled<FeaturesSet.warehouse>();

            Transfer.SetVisible(warehouseEnabled);
            Manufacture.SetVisible(warehouseEnabled);
            Purchase.SetVisible(warehouseEnabled && poSetup != null);

            bool orderNbrEnabled = true;

            if (currentFilter.RefType == XRefType.Transfer)
            {
                if (warehouseEnabled && TransferOrderFilter.Current != null && CurrentTROrderTypeNumbering != null &&
                    !CurrentTROrderTypeNumbering.UserNumbering.GetValueOrDefault())
                {
                    TransferOrderFilter.Current.OrderNbr = CurrentTROrderTypeNumbering.NewSymbol;
                    orderNbrEnabled = false;
                }
                PXUIFieldAttribute.SetEnabled<TROrderFilter.orderNbr>(TransferOrderFilter.Cache,
                    TransferOrderFilter.Current, orderNbrEnabled);
            }

            if (currentFilter.RefType == XRefType.Manufacture)
            {
                if (warehouseEnabled && CreateProductionOrderFilter.Current != null &&
                    CurrentProductionNumbering != null &&
                    !CurrentProductionNumbering.UserNumbering.GetValueOrDefault())
                {
                    CreateProductionOrderFilter.Current.ProdOrdID = CurrentProductionNumbering.NewSymbol;
                    orderNbrEnabled = false;
                }
                PXUIFieldAttribute.SetEnabled<CreateProdFilter.prodOrdID>(CreateProductionOrderFilter.Cache,
                    CreateProductionOrderFilter.Current, orderNbrEnabled);
            }

            if (currentFilter.RefType == XRefType.Purchase)
            {
                if (warehouseEnabled && CreatePurchaseOrderFilter.Current != null && CurrentPurchaseNumbering != null &&
                    !CurrentPurchaseNumbering.UserNumbering.GetValueOrDefault())
                {
                    CreatePurchaseOrderFilter.Current.OrderNbr = CurrentPurchaseNumbering.NewSymbol;
                    orderNbrEnabled = false;
                }
                PXUIFieldAttribute.SetEnabled<CreatePurchaseOrdFilter.orderNbr>(CreatePurchaseOrderFilter.Cache,
                    CreatePurchaseOrderFilter.Current, orderNbrEnabled);
            }
        }

        public static void ProcessPOCreate(ProdItemFilter filter, CreatePurchaseOrdFilter poFilter, List<SelectedProdMatl> list)
        {
            var newSelectedProdMatl = new List<SelectedProdMatl>();
            var cmGraph = CreateInstance<CriticalMaterialsInq>();
            cmGraph.CreatePurchaseOrderFilter.Current = poFilter;
            var updated = false;
            foreach (var amProdMatl in list)
            {
                var updatedSelectedProdMatl = (SelectedProdMatl)cmGraph.ProdMatlRecs.Cache.CreateCopy(amProdMatl);

                if (!updatedSelectedProdMatl.POCreate.GetValueOrDefault())
                {
                    AMProdMatl prodMatl = PXSelect<AMProdMatl,
                        Where<AMProdMatl.orderType, Equal<Required<AMProdMatl.orderType>>,
                            And<AMProdMatl.prodOrdID, Equal<Required<AMProdMatl.prodOrdID>>,
                            And<AMProdMatl.operationID, Equal<Required<AMProdMatl.operationID>>,
                            And<AMProdMatl.lineID, Equal<Required<AMProdMatl.lineID>>
                            >>>>>.Select(cmGraph, updatedSelectedProdMatl.OrderType, updatedSelectedProdMatl.ProdOrdID, updatedSelectedProdMatl.OperationID,
                                updatedSelectedProdMatl.LineID);

                    if(prodMatl == null)
                    {
                        continue;
                    }

                    prodMatl.POCreate = true;
                    cmGraph.MaterialUpdate.Update(prodMatl);

                    updated = true;
                }

                AMProdMatlSplit split = cmGraph.MatlSplitUpdate.Cache.LocateElse<AMProdMatlSplit>(PXSelect<AMProdMatlSplit, Where<AMProdMatlSplit.planID,
                    Equal<Required<AMProdMatlSplit.planID>>>>.Select(cmGraph, updatedSelectedProdMatl.PlanID));

                if (split == null)
                {
                    continue;
                }

#if DEBUG
                AMDebug.TraceWriteMethodName($"Material split planid {split.PlanID}. POCreate = {split.POCreate}");
#endif
                newSelectedProdMatl.Add(updatedSelectedProdMatl);

                if (!split.POCreate.GetValueOrDefault())
                {
                    updated = true;
                    split.POCreate = true;
                    cmGraph.MatlSplitUpdate.Update(split);
                }
            }

            //Is dirty is always true
            if (updated)
            {
                cmGraph.Actions.PressSave();

                //Reload list from persisted results...
                var oldList = new List<SelectedProdMatl>(newSelectedProdMatl);
                newSelectedProdMatl.Clear();
                foreach (var selectedProdMatl in oldList)
                {
                    newSelectedProdMatl.Add(cmGraph.ProdMatlRecs.Cache.LocateElse(selectedProdMatl));
                }
            }

            var pograph = PXGraph.CreateInstance<POCreate>();
            pograph.Clear(PXClearOption.ClearAll);
            pograph.Filter.Current.GetExtension<POCreateFilterExt>().AMOrderType = filter?.OrderType;
            pograph.Filter.Current.GetExtension<POCreateFilterExt>().ProdOrdID = filter?.ProdOrdID;
            var demandList = new List<POFixedDemand>();
            foreach(var amProdMatl in newSelectedProdMatl)
            {
                POFixedDemand demand = PXSelectReadonly<POFixedDemand,
                    Where<POFixedDemand.planID, Equal<Required<POFixedDemand.planID>>>>.Select(pograph, amProdMatl.PlanID);
                if (demand?.PlanID == null)
                {
                    continue;
                }
#if DEBUG
                AMDebug.TraceWriteMethodName($"Adding fixed demand material planid {demand.PlanID}");
#endif
                demand.Selected = true;
                demand.VendorID = poFilter.VendorID;
                demand.VendorLocationID = poFilter.VendorLocationID;

                demandList.Add(pograph.FixedDemand.Update(demand));
            }

            cmGraph.POCreateCreatePOOrders(pograph, demandList, pograph.Filter.Current.PurchDate);
        }

        protected virtual void POCreateCreatePOOrders(POCreate poCreateGraph, List<POFixedDemand> list, DateTime? purchaseDate)
        {
            if (CurrentPurchaseNumbering?.UserNumbering != true)
            {
                POCreateAMExtension.POCreatePOOrders(poCreateGraph, list, purchaseDate);
                return;
            }

            POCreateAMExtension.POCreatePOOrders(poCreateGraph, list, purchaseDate, CreatePurchaseOrderFilter.Current.OrderNbr);
        }

        public static void ProcessProductionOrders(ProdItemFilter prodItemFilter, CreateProdFilter filter, List<SelectedProdMatl> list, Numbering numbering)
        {
            if (filter == null)
            {
                throw new PXArgumentException(nameof(filter));
            }

            if (list == null || list.Count == 0)
            {
                PXTrace.WriteInformation("No selected prod material passed to create production orders");
                return;
            }

            var cmGraph = CreateInstance<CriticalMaterialsInq>();
            cmGraph.CreateProductionOrderFilter.Current = filter;

            //Verify filter here...
            var sb = new System.Text.StringBuilder();
            if (filter.OverrideWarehouse == true && filter.SiteID.GetValueOrDefault() == 0)
            {
                sb.AppendLine(string.Format(PXMessages.Localize(ErrorMessages.FieldIsEmpty),
                    PXUIFieldAttribute.GetDisplayName<CreateProdFilter.siteID>(cmGraph.CreateProductionOrderFilter.Cache)));
            }

            if (filter.OverrideWarehouse == true && filter.LocationID.GetValueOrDefault() == 0)
            {
                sb.AppendLine(string.Format(PXMessages.Localize(ErrorMessages.FieldIsEmpty),
                    PXUIFieldAttribute.GetDisplayName<CreateProdFilter.locationID>(cmGraph.CreateProductionOrderFilter.Cache)));
            }

            if (string.IsNullOrWhiteSpace(filter.OrderType))
            {
                sb.AppendLine(string.Format(PXMessages.Localize(ErrorMessages.FieldIsEmpty),
                    PXUIFieldAttribute.GetDisplayName<CreateProdFilter.orderType>(cmGraph.CreateProductionOrderFilter.Cache)));
            }

            if (numbering != null && numbering.UserNumbering.GetValueOrDefault() &&
                (string.IsNullOrWhiteSpace(filter.ProdOrdID) || filter.ProdOrdID.EqualsWithTrim(numbering.NewSymbol)))
            {
                sb.AppendLine(string.Format(PXMessages.Localize(ErrorMessages.FieldIsEmpty),
                    PXUIFieldAttribute.GetDisplayName<CreateProdFilter.prodOrdID>(cmGraph.CreateProductionOrderFilter.Cache)));
            }

            if (list.Count > 1 && numbering != null && numbering.UserNumbering.GetValueOrDefault())
            {
                sb.AppendLine(string.Format(Messages.GetLocal(Messages.OnlyOneProdMatlCanBeSelectedUserNumbering),
                    numbering.NumberingID));
            }

            if (sb.Length != 0)
            {
                throw new PXException(sb.ToString());
            }

            var createdOrderList = new List<AMProdItem>();

            createdOrderList = cmGraph.CreateProductionOrders((AMProdItem)PXSelect<AMProdItem, Where<AMProdItem.orderType,
                Equal<Required<AMProdItem.orderType>>, And<AMProdItem.prodOrdID, Equal<Required<AMProdItem.prodOrdID>>
                >>>.Select(cmGraph, prodItemFilter?.OrderType, prodItemFilter?.ProdOrdID), list, filter, numbering, createdOrderList);

            if (createdOrderList.Count >= list.Count)
            {
				return;
            }
            throw new PXException(Messages.UnableToCreateProductionOrders);
        }

        public static StringBuilder CreateListOfOrders(List<AMProdItem> list)
        {
            var orderList = new StringBuilder();

            if (list == null)
            {
                return orderList;
            }

            foreach (var prodItem in list)
            {
                orderList.AppendLine(Messages.GetLocal(Messages.CreatedProductionOrderForProductionOrder,
													   prodItem.OrderType,
													   prodItem.ProdOrdID.TrimIfNotNullEmpty(),
													   prodItem.ParentOrderType,
													   prodItem.ParentOrdID.TrimIfNotNullEmpty()));
            }

            return orderList;
        }

        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXFirstButton]
        public virtual IEnumerable firstOrder(PXAdapter adapter)
        {
            var filter = ProdItemRecs.Current;
            if (filter == null || string.IsNullOrWhiteSpace(filter.OrderType))
            {
                return adapter.Get();
            }

            AMProdItem prodItem = PXSelectJoin<AMProdItem,
				InnerJoin<Branch, On<AMProdItem.branchID, Equal<Branch.branchID>>>,
				   Where<AMProdItem.orderType, Equal<Required<AMProdItem.orderType>>,
				   And<Branch.baseCuryID, Equal<Current<AccessInfo.baseCuryID>>,
					And<Where<AMProdItem.canceled, Equal<False>,
                    And<AMProdItem.closed, Equal<False>>>>>>,
                OrderBy<Asc<AMProdItem.prodOrdID>>>.SelectWindowed(this, 0, 1, filter.OrderType);
            if (prodItem != null)
            {
                filter.ProdOrdID = prodItem.ProdOrdID;
                ProdMatlRecs.Cache.Clear(); //Required to refresh the details
            }
            return adapter.Get();
        }

        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXPreviousButton]
        public virtual IEnumerable previousOrder(PXAdapter adapter)
        {
            var filter = ProdItemRecs.Current;
            if (filter == null || string.IsNullOrWhiteSpace(filter.OrderType))
            {
                return adapter.Get();
            }

            AMProdItem prodItem = PXSelectJoin<AMProdItem,
				InnerJoin<Branch, On<AMProdItem.branchID, Equal<Branch.branchID>>>,
				Where<AMProdItem.orderType, Equal<Required<AMProdItem.orderType>>,
                    And<AMProdItem.prodOrdID, Less<Required<AMProdItem.prodOrdID>>,
					And<Branch.baseCuryID, Equal<Current<AccessInfo.baseCuryID>>,
					And<Where<AMProdItem.canceled, Equal<False>,                   
                    And<AMProdItem.closed, Equal<False>>>>>>>,
                OrderBy<Desc<AMProdItem.prodOrdID>>>.SelectWindowed(this, 0, 1, filter.OrderType, filter.ProdOrdID);
            if (prodItem != null)
            {
                filter.ProdOrdID = prodItem.ProdOrdID;
                ProdMatlRecs.Cache.Clear(); //Required to refresh the details
            }

            if (prodItem == null || string.IsNullOrWhiteSpace(filter.ProdOrdID))
            {
                return lastOrder(adapter);
            }

            return adapter.Get();
        }

        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXNextButton]
        public virtual IEnumerable nextOrder(PXAdapter adapter)
        {
            var filter = ProdItemRecs.Current;
            if (filter == null || string.IsNullOrWhiteSpace(filter.OrderType))
            {
                return adapter.Get();
            }

			AMProdItem prodItem = PXSelectJoin<AMProdItem,
						  InnerJoin<Branch, On<AMProdItem.branchID, Equal<Branch.branchID>>>,
						  Where<AMProdItem.orderType, Equal<Required<AMProdItem.orderType>>,
							  And<AMProdItem.prodOrdID, Greater<Required<AMProdItem.prodOrdID>>,
							  And<Branch.baseCuryID, Equal<Current<AccessInfo.baseCuryID>>,
							  And<Where<AMProdItem.canceled, Equal<False>,							 							  
							  And<AMProdItem.closed, Equal<False>>>>>>>,
						  OrderBy<Asc<AMProdItem.prodOrdID>>>.SelectWindowed(this, 0, 1, filter.OrderType, filter.ProdOrdID);
			if (prodItem != null)
            {
                filter.ProdOrdID = prodItem.ProdOrdID;
                ProdMatlRecs.Cache.Clear(); //Required to refresh the details
            }

            if (prodItem == null || string.IsNullOrWhiteSpace(filter.ProdOrdID))
            {
                return firstOrder(adapter);
            }

            return adapter.Get();
        }

        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLastButton]
        public virtual IEnumerable lastOrder(PXAdapter adapter)
        {
            var filter = ProdItemRecs.Current;
            if (filter == null || string.IsNullOrWhiteSpace(filter.OrderType))
            {
                return adapter.Get();
            }
			AMProdItem prodItem = PXSelectJoin<AMProdItem,
				InnerJoin<Branch, On<AMProdItem.branchID, Equal<Branch.branchID>>>,
				   Where<AMProdItem.orderType, Equal<Required<AMProdItem.orderType>>,
				   And<Branch.baseCuryID, Equal<Current<AccessInfo.baseCuryID>>,
					And<Where<AMProdItem.isOpen, Equal<False>,
					And<AMProdItem.closed, Equal<False>>>>>>,
				OrderBy<Desc<AMProdItem.prodOrdID>>>.SelectWindowed(this, 0, 1, filter.OrderType);
			if (prodItem != null)
            {
                filter.ProdOrdID = prodItem.ProdOrdID;
                ProdMatlRecs.Cache.Clear(); //Required to refresh the details
            }
            return adapter.Get();
        }

        public override bool CanClipboardCopyPaste()
        {
            return false;
        }

        protected AMProdItem CurrentAMProdItem
        {
            get
            {
                return PXSelect<AMProdItem,
                    Where<AMProdItem.orderType, Equal<Current<ProdItemFilter.orderType>>,
                        And<AMProdItem.prodOrdID, Equal<Current<ProdItemFilter.prodOrdID>>>>>.Select(this);
            }
        }

        protected Numbering CurrentTROrderTypeNumbering
        {
            get
            {
                TROrderTypeNumbering.Current = PXSelectJoin
                    <Numbering, LeftJoin<SOOrderType, On<SOOrderType.orderNumberingID, Equal<Numbering.numberingID>>>,
                        Where<SOOrderType.orderType, Equal<Required<SOOrderType.orderType>>>
                        >.Select(this, TransferOrderFilter.Current.OrderType);
                return TROrderTypeNumbering.Current;
            }
        }

        protected Numbering CurrentProductionNumbering
        {
            get
            {
                ProductionNumbering.Current = PXSelectJoin
                    <Numbering, LeftJoin<AMOrderType, On<AMOrderType.prodNumberingID,
                        Equal<Numbering.numberingID>>>,
                        Where<AMOrderType.orderType, Equal<Required<AMOrderType.orderType>>>
                        >.Select(this, CreateProductionOrderFilter.Current.OrderType);
                return ProductionNumbering.Current;
            }
        }

        protected Numbering CurrentPurchaseNumbering
        {
            get
            {
                PurchaseNumbering.Current = PXSelect
                    <Numbering, Where<Numbering.numberingID, Equal<Required<Numbering.numberingID>>>
                        >.Select(this, POSetup.Current.RegularPONumberingID);
                return PurchaseNumbering.Current;
            }
        }

        protected virtual void TROrderFilter_OrderType_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            TROrderFilter row = (TROrderFilter)e.Row;
            if (row == null)
            {
                return;
            }

            row.OrderNbr = null;

            if (CurrentTROrderTypeNumbering != null && !CurrentTROrderTypeNumbering.UserNumbering.GetValueOrDefault())
            {
                row.OrderNbr = CurrentTROrderTypeNumbering.NewSymbol;
            }
        }

        protected virtual void TROrderFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            TROrderFilter row = (TROrderFilter)e.Row;
            if (row == null)
            {
                return;
            }

            PXUIFieldAttribute.SetEnabled<TROrderFilter.orderNbr>(cache, row,
                CurrentTROrderTypeNumbering != null && CurrentTROrderTypeNumbering.UserNumbering.GetValueOrDefault());
        }

        protected virtual void CreateProdFilter_OrderType_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            CreateProdFilter row = (CreateProdFilter)e.Row;
            if (row == null)
            {
                return;
            }

            row.ProdOrdID = null;

            if (CurrentProductionNumbering != null && !CurrentProductionNumbering.UserNumbering.GetValueOrDefault())
            {
                row.ProdOrdID = CurrentProductionNumbering.NewSymbol;
                return;
            }

            row.CreateLinkedOrders = false;
        }

        protected virtual void CreatePurchaseOrdFilter_OrderType_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            CreatePurchaseOrdFilter row = (CreatePurchaseOrdFilter)e.Row;
            if (row == null)
            {
                return;
            }

            row.OrderNbr = null;

            if (CurrentPurchaseNumbering != null && !CurrentPurchaseNumbering.UserNumbering.GetValueOrDefault())
            {
                row.OrderNbr = CurrentPurchaseNumbering.NewSymbol;
            }
        }

        protected virtual void ProdItemFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            ProdItemFilter row = (ProdItemFilter)e.Row;
            if (row == null || string.IsNullOrWhiteSpace(row.ProdOrdID))
            {
                EnableButtons(false, false);
                return;
            }

            EnableButtons(IsValidTransactionStatus, true);
        }

        protected void EnableButtons(bool actionsEnabled, bool inquiresEnabled)
        {
            var warehouseEnabled = PXAccess.FeatureInstalled<FeaturesSet.warehouse>();
            Manufacture.SetEnabled(actionsEnabled && warehouseEnabled);
            Purchase.SetEnabled(actionsEnabled && warehouseEnabled );
            Transfer.SetEnabled(actionsEnabled && warehouseEnabled);
            InventorySummary.SetEnabled(inquiresEnabled);
            InventoryAllocationDetails.SetEnabled(inquiresEnabled);
        }

		[Api.Export.PXOptimizationBehavior(IgnoreBqlDelegate = true)]
        protected virtual IEnumerable prodMatlRecs()
        {
            ProdItemFilter current = this.ProdItemRecs.Current;

            List<SelectedProdMatl> prodMatlList = new List<SelectedProdMatl>();

            if (current != null)
            {
                bool itVar1 = false;
                IEnumerator enumerator = this.ProdMatlRecs.Cache.Inserted.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SelectedProdMatl itVar2 = (SelectedProdMatl)enumerator.Current;
                    itVar1 = true;
                    yield return itVar2;
                }
                if (!itVar1)
                {
                    foreach (PXResult<AMProdMatl, AMProdOper, AMProdItem, InventoryItem, InventoryItemCurySettings,
                            AMProdMatlSplit, INSiteStatusByCostCenter, INLocationTotals, INItemSite> result in PXSelectJoin<AMProdMatl,
                        InnerJoin<AMProdOper, On<AMProdMatl.orderType, Equal<AMProdOper.orderType>,
                            And<AMProdMatl.prodOrdID, Equal<AMProdOper.prodOrdID>,
                            And<AMProdMatl.operationID, Equal<AMProdOper.operationID>>>>,
                        InnerJoin<AMProdItem, On<AMProdMatl.orderType, Equal<AMProdItem.orderType>,
                              And<AMProdMatl.prodOrdID, Equal<AMProdItem.prodOrdID>>>,
                        InnerJoin<InventoryItem, On<AMProdMatl.inventoryID, Equal<InventoryItem.inventoryID>>,
						LeftJoin<InventoryItemCurySettings, On<InventoryItemCurySettings.inventoryID, Equal<InventoryItem.inventoryID>,
							And<InventoryItemCurySettings.curyID, Equal<Current<AccessInfo.baseCuryID>>>>,
                        LeftJoin<AMProdMatlSplit, On<AMProdMatl.orderType, Equal<AMProdMatlSplit.orderType>,
                            And<AMProdMatl.prodOrdID, Equal<AMProdMatlSplit.prodOrdID>,
                            And<AMProdMatl.operationID, Equal<AMProdMatlSplit.operationID>,
                            And<AMProdMatl.lineID, Equal<AMProdMatlSplit.lineID>>>>>,
                        LeftJoin<INSiteStatusByCostCenter, On<AMProdMatl.inventoryID, Equal<INSiteStatusByCostCenter.inventoryID>,
                            And<AMProdMatl.subItemID, Equal<INSiteStatusByCostCenter.subItemID>,
                            And<AMProdMatl.siteID, Equal<INSiteStatusByCostCenter.siteID>,
							And<INSiteStatusByCostCenter.costCenterID, Equal<CostCenter.freeStock>>>>>,
                        LeftJoin<INLocationTotals, On<INSiteStatusByCostCenter.inventoryID, Equal<INLocationTotals.inventoryID>,
                                And<INSiteStatusByCostCenter.siteID, Equal<INLocationTotals.siteID>,
                                And<INSiteStatusByCostCenter.subItemID, Equal<INLocationTotals.subItemID>>>>,
						LeftJoin<INItemSite, On<AMProdMatl.inventoryID, Equal<INItemSite.inventoryID>,
								And<AMProdMatl.siteID, Equal<INItemSite.siteID>>>>>>>>>>>,
                        Where<AMProdMatl.orderType, Equal<Current<ProdItemFilter.orderType>>,
                            And<AMProdMatl.prodOrdID, Equal<Current<ProdItemFilter.prodOrdID>>>>,
                    OrderBy<Asc<AMProdOper.operationCD, Asc < AMProdMatl.sortOrder, Asc<AMProdMatl.lineID, Desc<AMProdMatlSplit.isAllocated>>>>>>.Select(this))
                    {
                        var row = BuildSelectedProdMatl(prodMatlList, result, result, result, result, result, result, result, result, result);
                        if (row != null)
                        {
                            prodMatlList.Add(row);
                        }
                    }

                    foreach (var prodMatl in prodMatlList)
                    {
						var isShort = prodMatl.QtyShort.GetValueOrDefault() > 0 && prodMatl.StatusID != ProductionOrderStatus.Completed;
						var isAllocated = prodMatl.IsAllocated == true;
						if (isShort || (current.ShowAll.GetValueOrDefault() && !isAllocated) || current.ShowAllocated.GetValueOrDefault() && isAllocated)
						{
							ProdMatlRecs.Insert(prodMatl);
							yield return prodMatl;
						}
                    }
                }
                this.ProdMatlRecs.Cache.IsDirty = false;
            }
        }

		/// <summary>
		/// Builds a displayed production material record in the critical material grid
		/// </summary>
		/// <returns>Processed selected material row</returns>
		///
		[Obsolete]
		protected virtual SelectedProdMatl BuildSelectedProdMatl(List<SelectedProdMatl> prodMatlList, AMProdMatl amProdMatl, AMProdOper amProdOper, AMProdItem amProdItem, 
            InventoryItem inventoryItem, AMProdMatlSplit amProdMatlSplit, INSiteStatusByCostCenter inSiteStatus, INLocationTotals locationTotals, INItemSite itemSite)
        {
			var itemCury = InventoryItemCurySettings.PK.Find(this, inventoryItem.InventoryID, Accessinfo.BaseCuryID);
			return BuildSelectedProdMatl(prodMatlList, amProdMatl, amProdOper, amProdItem,
			inventoryItem, itemCury, amProdMatlSplit, inSiteStatus, locationTotals, itemSite);
		}

		protected virtual SelectedProdMatl BuildSelectedProdMatl(List<SelectedProdMatl> prodMatlList, AMProdMatl amProdMatl, AMProdOper amProdOper, AMProdItem amProdItem, 
            InventoryItem inventoryItem, InventoryItemCurySettings itemCury, AMProdMatlSplit amProdMatlSplit, INSiteStatusByCostCenter inSiteStatus, INLocationTotals locationTotals, INItemSite itemSite)
        {
            if (string.IsNullOrWhiteSpace(amProdMatl?.ProdOrdID) ||
                amProdOper?.OperationID == null ||
                string.IsNullOrWhiteSpace(amProdItem?.ProdOrdID) ||
                inventoryItem?.InventoryID == null || prodMatlList == null)
            {
                return null;
            }
            //it's possible there are fewer available at the site record than the location, so get the lower of the two
            var siteTotals = inSiteStatus;

            var uomConversion = amProdMatl.BaseTotalQtyRequired.GetValueOrDefault() == 0m ? 1m
                : amProdMatl.TotalQtyRequired.GetValueOrDefault() / amProdMatl.BaseTotalQtyRequired.GetValueOrDefault();

            var multiplier = amProdItem.Function == OrderTypeFunction.Disassemble ? -1 : 1;

            var record = new SelectedProdMatl
            {
                InventoryID = amProdMatl.InventoryID,
                SubItemID = amProdMatl.SubItemID,
                OrderType = amProdMatl.OrderType,
                ProdOrdID = amProdMatl.ProdOrdID,
                OperationID = amProdMatl.OperationID,
                LineID = amProdMatl.LineID,
                SortOrder = amProdMatl.SortOrder,
                Descr = amProdMatl.Descr,
                SiteID = amProdMatl.SiteID,
                UOM = amProdMatl.UOM,
                BatchSize = amProdMatl.BatchSize,
                IsStockItem = inventoryItem.StkItem,
                ItemClassID = inventoryItem.ItemClassID,
                IsByproduct2 = amProdMatl.IsByproduct.GetValueOrDefault(),
                RequiredDate = amProdItem.FMLTMRPOrdorOP.GetValueOrDefault() || amProdOper.StartDate == null ?
                    amProdItem.StartDate : amProdOper.StartDate,
                QtyRemaining = amProdMatl.QtyRemaining.GetValueOrDefault() * multiplier,
                TotalQtyRequired = amProdMatl.TotalQtyRequired.GetValueOrDefault() * multiplier,
                QtyOnHand = Math.Min((siteTotals?.QtyOnHand ?? 0m),(locationTotals?.QtyOnHand ?? 0m)) * uomConversion,
                QtyShort = 0m,
                QtyAvail = Math.Min((siteTotals?.QtyAvail ?? 0m), (locationTotals?.QtyAvail ?? 0m)) * uomConversion,
                QtyHardAvail = Math.Min((siteTotals?.QtyHardAvail ?? 0m), (locationTotals?.QtyHardAvail ?? 0m)) * uomConversion,
                QtyProductionSupplyPrepared = Math.Min((siteTotals?.QtyProductionSupplyPrepared ?? 0m), (locationTotals?.QtyProductionSupplyPrepared ?? 0m)) * uomConversion,
                QtyProductionSupply = Math.Min((siteTotals?.QtyProductionSupply ?? 0m), (locationTotals?.QtyProductionSupply ?? 0m)) * uomConversion,
                QtyProductionDemandPrepared = Math.Min((siteTotals?.QtyProductionDemandPrepared ?? 0m), (locationTotals?.QtyProductionDemandPrepared ?? 0m)) * uomConversion,
                QtyProductionDemand = Math.Min((siteTotals?.QtyProductionDemand ?? 0m), (locationTotals?.QtyProductionDemand ?? 0m)) * uomConversion,
                PreferredVendorID = itemCury?.PreferredVendorID,
                TranDate = amProdMatl.TranDate,
                SplitLineCntr = amProdMatl.SplitLineCntr,
                IsAllocated = amProdMatlSplit.IsAllocated,
                POCreate = amProdMatlSplit.POCreate,
                ProdCreate = amProdMatlSplit.ProdCreate,
                MaterialType = amProdMatl.MaterialType,
                SubcontractSource = amProdMatl.SubcontractSource,
				ReplenishmentSourceSiteID = itemSite.ReplenishmentSourceSiteID,
				StatusID = amProdMatl.StatusID
            };

            //if allocated, increase the hard avail by the split qty
            if (record.IsAllocated == true)
            {
                record.QtyHardAvail += amProdMatlSplit.Qty;
                record.QtyAlloc = amProdMatlSplit.Qty;
            }

            record.ReplenishmentSource = InventoryHelper.GetReplenishmentSource(this, record.InventoryID,
                record.SiteID);

            if (record.IsStockItem.GetValueOrDefault() && !amProdMatl.IsByproduct.GetValueOrDefault() 
                && amProdMatl.SubcontractSource != AMSubcontractSource.VendorSupplied && !record.IsAllocated == true)
            {
                var previousQty = GetPreviousMaterialQty(prodMatlList, record.InventoryID, record.SiteID, record.SubItemID);
                var previousAlloc = GetPreviousAllocMaterialQty(prodMatlList, record.InventoryID, record.SiteID, record.SubItemID);
                // Need to account for negative qty on hand
                var adjustedQtyOnHand = Math.Max(record.QtyHardAvail.GetValueOrDefault() - previousQty, 0m);
                var qtyRemain = record.QtyRemaining.GetValueOrDefault() - previousAlloc;
                if (adjustedQtyOnHand < qtyRemain && qtyRemain > 0)
                {
                    record.QtyShort = qtyRemain - adjustedQtyOnHand;
                }
                if (previousAlloc > 0)
                    record.QtyHardAvail += previousAlloc;
            }

            if (amProdMatlSplit.POOrderType != null)
            {
                record.POOrderNbr = amProdMatlSplit.POOrderNbr;
            }
            else if (amProdMatlSplit.AMOrderType != null)
            {
                record.AMOrderType = amProdMatlSplit.AMOrderType;
                record.AMProdOrdID = amProdMatlSplit.AMProdOrdID;
            }
            record.PlanID = amProdMatlSplit.PlanID;

            return record;
        }

        /// <summary>
        /// Find the previous qty for like items to correctly show shortages
        /// </summary>
        protected virtual decimal GetPreviousMaterialQty(List<SelectedProdMatl> prodMatlList, int? inventoryId, int? siteId, int? subItemId)
        {
            var subItemEnabled = PXAccess.FeatureInstalled<FeaturesSet.subItem>();
            var prevunallocated = prodMatlList.Where(prodMatl => prodMatl.InventoryID == inventoryId
                && prodMatl.SiteID == siteId && prodMatl.IsAllocated != true
                && (prodMatl.SubItemID == subItemId || !subItemEnabled)).Sum(prodMatl => prodMatl.Qty.GetValueOrDefault());
            return prevunallocated;
        }

        protected virtual decimal GetPreviousAllocMaterialQty(List<SelectedProdMatl> prodMatlList, int? inventoryId, int? siteId, int? subItemId)
        {
            var subItemEnabled = PXAccess.FeatureInstalled<FeaturesSet.subItem>();
            var prevallocated = prodMatlList.Where(prodMatl => prodMatl.InventoryID == inventoryId
                && prodMatl.SiteID == siteId && prodMatl.IsAllocated == true
                && (prodMatl.SubItemID == subItemId || !subItemEnabled)).Sum(prodMatl => prodMatl.QtyAlloc.GetValueOrDefault());
            return prevallocated;
        }

        protected virtual void SelectedProdMatl_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            var row = (SelectedProdMatl)e.Row;
            if (row == null)
            {
                return;
            }

            bool containsReference = !string.IsNullOrEmpty(row.POOrderNbr) || !string.IsNullOrEmpty(row.AMProdOrdID);

            PXUIFieldAttribute.SetEnabled<SelectedProdMatl.selected>(cache, row, !row.IsAllocated.GetValueOrDefault() && !containsReference);
        }

        protected virtual void SelectedProdMatl_LineID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.Cancel = true;
        }

        //      This resolved the check box for 'ShowAll' not refreshing the page.
        //      If the row has changed then clear cache "Refresh"
        protected virtual void ProdItemFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            if (sender
                .ObjectsEqual<ProdItemFilter.orderType, ProdItemFilter.prodOrdID, ProdItemFilter.showAll,
                    ProdItemFilter.showAllocated>(e.OldRow, e.Row))
            {
                return;
            }

            ProdMatlRecs.Cache.Clear();
        }

        public PXAction<ProdItemFilter> Purchase;
        [PXUIField(DisplayName = Messages.Purchase, MapEnableRights = PXCacheRights.Update,
            MapViewRights = PXCacheRights.Update), PXProcessButton]
        public virtual IEnumerable purchase(PXAdapter adapter)
        {
            if (!IsValidTransactionStatus)
            {
                ThrowExceptionForInvalidTransactionStatus();
            }

            if (CreatePurchaseOrderFilter.Current == null)
            {
                return adapter.Get();
            }

            if (CurrentPurchaseNumbering != null && !CurrentPurchaseNumbering.UserNumbering.GetValueOrDefault())
            {
                CreatePurchaseOrderFilter.Current.OrderNbr = CurrentPurchaseNumbering.NewSymbol;
            }

            if (!CreatePurchaseOrderFilter.Current.VendorDefaulted.GetValueOrDefault())
            {
                SetSharedVendor(ProdMatlRecs.Cache.Cached.Cast<SelectedProdMatl>().Where(row => row.Selected == true).ToList());
            }

            if (CreatePurchaseOrderFilter.AskExt() == WebDialogResult.OK)
            {
                ProdItemRecs.Current.RefType = XRefType.Purchase;
                return Actions[PX.Objects.IN.Messages.Process].Press(adapter);
            }

            //To reset vendor default logic
            CreatePurchaseOrderFilter.Cache.Clear();
            adapter = new PXAdapter(this.ProdItemRecs);
            return adapter.Get();
        }



        public void SetSharedVendor(List<SelectedProdMatl> list)
        {
            int? sharedVendorID = 0;
            bool singleVendorSelected = true;

            foreach (SelectedProdMatl prodMatl in list)
            {
                if ((sharedVendorID ?? 0) == 0
                    && (prodMatl.PreferredVendorID ?? 0) != 0
                    && singleVendorSelected)
                {
                    sharedVendorID = prodMatl.PreferredVendorID;
                }

                if ((sharedVendorID ?? 0) != 0
                    && (prodMatl.PreferredVendorID ?? 0) != 0
                    && sharedVendorID != prodMatl.PreferredVendorID)
                {
                    singleVendorSelected = false;
                }
            }

            CreatePurchaseOrderFilter.Current.VendorID = null;
            CreatePurchaseOrderFilter.Current.VendorLocationID = null;

            if (singleVendorSelected && (sharedVendorID ?? 0) != 0)
            {
				object val = sharedVendorID;

                CreatePurchaseOrderFilter.Current.VendorID = (int?)val;
				try
				{
					CreatePurchaseOrderFilter.Cache.RaiseFieldVerifying<CreatePurchaseOrdFilter.vendorID>(
						CreatePurchaseOrderFilter.Current, ref val);
				}
				catch (Exception)
				{
					CreatePurchaseOrderFilter.Current.VendorID = null;
				}

				CreatePurchaseOrderFilter.Cache.RaiseFieldDefaulting<CreatePurchaseOrdFilter.vendorLocationID>(
                    CreatePurchaseOrderFilter.Current, out object newDefault);
                CreatePurchaseOrderFilter.Current.VendorLocationID = (int?)newDefault;
            }

            CreatePurchaseOrderFilter.Current.VendorDefaulted = true;
        }


        public PXAction<ProdItemFilter> Manufacture;
        [PXUIField(DisplayName = Messages.Manufacture, MapEnableRights = PXCacheRights.Update,
            MapViewRights = PXCacheRights.Update), PXProcessButton]
        public virtual IEnumerable manufacture(PXAdapter adapter)
        {
            if (!IsValidTransactionStatus)
            {
                ThrowExceptionForInvalidTransactionStatus();
            }

            if (CurrentProductionNumbering != null && !CurrentProductionNumbering.UserNumbering.GetValueOrDefault())
            {
                CreateProductionOrderFilter.Current.ProdOrdID = CurrentProductionNumbering.NewSymbol;
            }

            if (CreateProductionOrderFilter.AskExt() == WebDialogResult.OK)
            {
                ProdItemRecs.Current.RefType = XRefType.Manufacture;
                return Actions[PX.Objects.IN.Messages.Process].Press(adapter);
            }
            return adapter.Get();
        }

        public PXAction<ProdItemFilter> Transfer;
        [PXUIField(DisplayName = PX.Objects.IN.Messages.Transfer, MapEnableRights = PXCacheRights.Update,
            MapViewRights = PXCacheRights.Update), PXButton]
        public virtual IEnumerable transfer(PXAdapter adapter)
        {
            if (!IsValidTransactionStatus)
            {
                ThrowExceptionForInvalidTransactionStatus();
            }

            if (TransferOrderFilter.Current == null)
            {
                return adapter.Get();
            }

			int? transferSite = TransferOrderFilter.Current.FromSiteID;
			if (transferSite == null)
			{
				bool sameFromSite = true;
				foreach (var detail in ProdMatlRecs.Cache.Cached.Cast<SelectedProdMatl>().Where(row => row.Selected == true).ToList())
				{
					if (transferSite == null)
						transferSite = detail.ReplenishmentSourceSiteID;
					if (sameFromSite)
					{
						if (transferSite != detail.ReplenishmentSourceSiteID)
							sameFromSite = false;
					}
				}

				if (sameFromSite)
					TransferOrderFilter.Current.FromSiteID = transferSite;
			}

			if (CurrentAMProdItem != null
                && TransferOrderFilter.Current != null
                && TransferOrderFilter.Current.ToSiteID == null)
            {
                TransferOrderFilter.Current.ToSiteID = CurrentAMProdItem.SiteID;
            }

            if (CurrentTROrderTypeNumbering != null && !CurrentTROrderTypeNumbering.UserNumbering.GetValueOrDefault())
            {
                TransferOrderFilter.Current.OrderNbr = CurrentTROrderTypeNumbering.NewSymbol;
            }

            if (TransferOrderFilter.AskExt() == WebDialogResult.OK)
            {
                ProdItemRecs.Current.RefType = XRefType.Transfer;
                return Actions[PX.Objects.IN.Messages.Process].Press(adapter);
            }
            return adapter.Get();
        }

        public PXAction<ProdItemFilter> InquiresDropMenu;
        [PXUIField(DisplayName = Messages.Inquiries)]
        [PXButton(MenuAutoOpen = true)]
        protected virtual IEnumerable inquiresDropMenu(PXAdapter adapter)
        {
            return adapter.Get();
        }

        public PXAction<ProdItemFilter> InventorySummary;
        [PXUIField(DisplayName = "Inventory Summary", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable inventorySummary(PXAdapter adapter)
        {
            if (ProdMatlRecs.Current != null)
            {
                InventorySummaryEnq graph = PXGraph.CreateInstance<InventorySummaryEnq>();
                graph.Filter.Current.InventoryID = ProdMatlRecs.Current.InventoryID;
                graph.Filter.Select();
                throw new PXRedirectRequiredException(graph, true, "Inventory Summary");
            }
            return adapter.Get();
        }

        public PXAction<ProdItemFilter> InventoryAllocationDetails;
        [PXUIField(DisplayName = "Allocation Details", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable inventoryAllocationDetails(PXAdapter adapter)
        {
            if (ProdMatlRecs.Current != null)
            {
                InventoryAllocDetEnq graph = PXGraph.CreateInstance<InventoryAllocDetEnq>();
                graph.Filter.Current.InventoryID = ProdMatlRecs.Current.InventoryID;
                graph.Filter.Select();
                throw new PXRedirectRequiredException(graph, true, "Inventory Allocation Details");
            }
            return adapter.Get();
        }

        private bool ThrowExceptionForInvalidTransactionStatus()
        {
            AMProdItem amProdItem = CurrentAMProdItem;

            if (amProdItem == null)
            {
                throw new PXException(Messages.InvalidProductionNbr);
            }

			string orderStatus = amProdItem.Hold.GetValueOrDefault()
                ? ProductionOrderStatus.Hold
                : amProdItem.StatusID;
            throw new PXException(Messages.GetLocal(Messages.ProdStatusInvalidForProcess), amProdItem.OrderType.TrimIfNotNullEmpty(), amProdItem.ProdOrdID.TrimIfNotNullEmpty(),
                ProductionOrderStatus.GetStatusDescription(orderStatus));
        }

        private bool IsValidTransactionStatus
        {
            get
            {
                var amProdItem = CurrentAMProdItem;
                if (amProdItem == null)
                {
                    return false;
                }

                return ProductionStatus.IsValidTransactionStatus(amProdItem);
            }
        }

        protected virtual List<AMProdMatl> ConvertToProdMatl(List<SelectedProdMatl> selectedList)
        {
            List<AMProdMatl> list = new List<AMProdMatl>();
            foreach (var selectedProdMatl in selectedList)
            {
                AMProdMatl prodMatl = ConvertToProdMatl(selectedProdMatl);
                if (prodMatl == null)
                {
                    continue;
                }
                list.Add(prodMatl);
            }
            return list;
        }

        protected virtual AMProdMatl ConvertToProdMatl(SelectedProdMatl selectedProdMatl)
        {
            if (selectedProdMatl == null || string.IsNullOrWhiteSpace(selectedProdMatl.ProdOrdID))
            {
                return null;

            }
            return PXSelect<AMProdMatl,
                Where<AMProdMatl.prodOrdID, Equal<Required<AMProdMatl.prodOrdID>>,
                    And<AMProdMatl.orderType, Equal<Required<AMProdMatl.orderType>>,
                        And<AMProdMatl.operationID, Equal<Required<AMProdMatl.operationID>>,
                            And<AMProdMatl.lineID, Equal<Required<AMProdMatl.lineID>>>>>
                >>.SelectWindowed(this, 0, 1,
                    selectedProdMatl.ProdOrdID,
                    selectedProdMatl.OrderType,
                    selectedProdMatl.OperationID,
                    selectedProdMatl.LineID);
        }

        public static void ProcessTransferOrder(TROrderFilter filter, List<SelectedProdMatl> list, Numbering numbering)
        {
            if (filter == null)
            {
                throw new PXArgumentException("filter");
            }

            if (list == null || list.Count == 0)
            {
                PXTrace.WriteInformation("No selected prod material passed to process transfer");
                return;
            }

            CriticalMaterialsInq cmGraph = CreateInstance<CriticalMaterialsInq>();
            cmGraph.TransferOrderFilter.Current = filter;

            //Verify filter here...
            var sb = new System.Text.StringBuilder();
            if (filter.ToSiteID.GetValueOrDefault() == 0)
            {
                sb.AppendLine(string.Format(PXMessages.Localize(ErrorMessages.FieldIsEmpty),
                    PXUIFieldAttribute.GetDisplayName<TROrderFilter.toSiteID>(cmGraph.TransferOrderFilter.Cache)));
            }

            if (filter.FromSiteID.GetValueOrDefault() == 0)
            {
                sb.AppendLine(string.Format(PXMessages.Localize(ErrorMessages.FieldIsEmpty),
                    PXUIFieldAttribute.GetDisplayName<TROrderFilter.fromSiteID>(cmGraph.TransferOrderFilter.Cache)));
            }

            if (string.IsNullOrWhiteSpace(filter.OrderType))
            {
                sb.AppendLine(string.Format(PXMessages.Localize(ErrorMessages.FieldIsEmpty),
                    PXUIFieldAttribute.GetDisplayName<TROrderFilter.orderType>(cmGraph.TransferOrderFilter.Cache)));
            }
            else
            {
                var result = (PXResult<Numbering, SOOrderType>)PXSelectJoin<Numbering,
                    LeftJoin<SOOrderType, On<SOOrderType.orderNumberingID, Equal<Numbering.numberingID>>>,
                    Where<SOOrderType.orderType, Equal<Required<TROrderFilter.orderType>>>>.SelectWindowed(cmGraph, 0, 1,
                        filter.OrderType);

                if (result != null)
                {
                    numbering = (Numbering)result;
                    var soOrderType = (SOOrderType)result;
                    if (soOrderType == null || !soOrderType.Active.GetValueOrDefault())
                    {
                        sb.AppendLine(PXMessages.Localize(PX.Objects.SO.Messages.OrderTypeInactive));
                    }
                }
            }

            if (numbering != null && numbering.UserNumbering.GetValueOrDefault()
                && (string.IsNullOrWhiteSpace(filter.OrderNbr) || filter.OrderNbr.EqualsWithTrim(numbering.NewSymbol)))
            {
                sb.AppendLine(string.Format(PXMessages.Localize(ErrorMessages.FieldIsEmpty),
                    PXUIFieldAttribute.GetDisplayName<TROrderFilter.orderNbr>(cmGraph.TransferOrderFilter.Cache)));
            }

            if (sb.Length != 0)
            {
                throw new PXException(sb.ToString());
            }

            SOOrder createdTransfer = null;
            if (cmGraph.TryCreateTransferOrder(list, filter, numbering, out createdTransfer) &&
                createdTransfer != null)
            {
                throw new PXOperationCompletedException(PX.Objects.SO.Messages.TransferOrderCreated, createdTransfer.OrderNbr.TrimIfNotNullEmpty());
            }

            throw new PXException(Messages.UnableToCreateTransfer);
        }

        /// <summary>
        /// Create (persist) a new Transfer order
        /// </summary>
        /// <param name="prodMatlList">list of production material being added to the transfer</param>
        /// <param name="filter">CreatePurchase Order Filter used for order creation</param>
        /// <param name="numbering">Create transfer order Numbering </param>
        /// <param name="newTrOrder">If create process complete this will be the new transfer order record</param>
        /// <returns>true when successful in creation, false otherwise</returns>
        public virtual bool TryCreateTransferOrder(List<SelectedProdMatl> prodMatlList, TROrderFilter filter, Numbering numbering,
            out SOOrder newTrOrder)
        {
            newTrOrder = null;
            if (prodMatlList == null || prodMatlList.Count == 0)
            {
                return false;
            }

            var docgraph = CreateInstance<SOOrderEntry>();

            var prodOrderType = prodMatlList[0].OrderType.TrimIfNotNullEmpty();
            var productionNbr = prodMatlList[0].ProdOrdID.TrimIfNotNullEmpty();

            SOOrder doc = new SOOrder
            {
                OrderType = filter.OrderType,
                OrderNbr = numbering != null && numbering.UserNumbering.GetValueOrDefault() ? filter.OrderNbr : null,
                OrderDate = this.Accessinfo.BusinessDate,
                OrderDesc = string.IsNullOrWhiteSpace(productionNbr)
                        ? string.Empty
                        : Messages.GetLocal(Messages.ProductionOrderMaterialTransfer, prodOrderType, productionNbr)
            };

            SOOrderType orderType = PXSelect<SOOrderType, Where<SOOrderType.orderType, Equal<Required<SOOrderType.orderType>>
                >>.Select(this, filter.OrderType);

            doc.Hold = orderType?.HoldEntry == true;

            doc = docgraph.Document.Insert(doc);
            doc.DestinationSiteID = filter.ToSiteID;
            doc = docgraph.Document.Update(doc);

            var earliestPromiseDate = Common.Dates.EndOfTimeDate;
            var earliestShipDate = Common.Dates.EndOfTimeDate;

            foreach (var selectedProdMatl in prodMatlList)
            {
                var line = InsertSOLine(docgraph, selectedProdMatl, filter);

                if (docgraph.Transactions.Select().Any())
                {
                    // Set the Order Reqested Date
                    earliestPromiseDate = earliestPromiseDate >= line.RequestDate.GetValueOrDefault()
                        ? line.RequestDate.GetValueOrDefault() : earliestPromiseDate;

                    // Set the Order Ship on Date
                    earliestShipDate = earliestShipDate >= line.ShipDate.GetValueOrDefault()
                        ? line.ShipDate.GetValueOrDefault() : earliestShipDate;
                }
            }

            if (!docgraph.Transactions.Cache.Inserted.Any_())
            {
                return false;
            }

			WebDialogResult originalAnswer = docgraph.Document.View.Answer;
			try
			{
				doc = docgraph.Document.Current;
				doc.RequestDate = earliestPromiseDate;
				doc.ShipDate = earliestShipDate;
				// Auto answer PX.Objects.SO.Messages.ConfirmShipDateRecalc
				docgraph.Document.View.Answer = WebDialogResult.No;
				docgraph.Document.Update(doc);
			}
			finally
			{
				docgraph.Document.View.Answer = originalAnswer;
			}

            using (PXTransactionScope scope = new PXTransactionScope())
            {
                docgraph.Actions.PressSave();

                doc = docgraph.Document.Current;
                if (doc == null || doc.OrderNbr == null)
                {
                    return false;
                }
                newTrOrder = doc;
                foreach (var selectedProdMatl in prodMatlList)
                {
                    var prodMatl = ConvertToProdMatl(selectedProdMatl);
                    if (prodMatl == null)
                    {
                        continue;
                    }
                    this.MaterialUpdate.Update(prodMatl);
                }
                this.Actions.PressSave();
                scope.Complete();
            }

			// Mark each row as processed
			for (int i = 0; i < prodMatlList.Count; i++)
			{
				string infoUpdate = $"{Messages.GetLocal(Messages.CreatedTransferOrder)}: {docgraph.Document?.Current?.OrderType} {docgraph.Document?.Current?.OrderNbr}";
				PXProcessing<SelectedProdMatl>.SetInfo(i, infoUpdate);
			}

            this.Clear();
            return true;
        }

        protected virtual SOLine InsertSOLine(SOOrderEntry docgraph, SelectedProdMatl prodMatl, TROrderFilter filter)
        {
            if (docgraph == null
               || prodMatl == null || filter?.FromSiteID == null)
            {
                PXTrace.WriteWarning("Unable to create sales order line");
                return null;
            }

            var orderQty = filter.UseFullQty.GetValueOrDefault()
                ? prodMatl.TotalQtyRequired.GetValueOrDefault()
                : prodMatl.QtyShort.GetValueOrDefault();

            if (orderQty <= 0)
            {
                PXTrace.WriteWarning("Unable to creating sales order line with incorrect qty");
                return null;
            }

            try
            {
                SOLine tran = new SOLine();
                tran = docgraph.Transactions.Insert(tran);
                if (tran == null)
                {
                    return null;
                }

                tran.InventoryID = prodMatl.InventoryID;
                tran.SubItemID = prodMatl.SubItemID;
                tran.TranDesc = prodMatl.Descr;
                tran.Qty = orderQty;
                tran.UOM = prodMatl.UOM;
                tran.SiteID = filter.FromSiteID;
                tran.RequestDate = prodMatl.RequiredDate;

                var transferLeadTime = GetTransferleadTime(tran.InventoryID, tran.SiteID, filter.ToSiteID);

                var requestDate = tran.RequestDate.GetValueOrDefault();
                tran.ShipDate = requestDate.AddDays(-transferLeadTime);

                return docgraph.Transactions.Update(tran);
            }
            catch (Exception e)
            {
                PXTraceHelper.PxTraceException(e);

                InventoryItem inventoryItem = PXSelect<InventoryItem,
                    Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>
                    >.SelectWindowed(this, 0, 1, prodMatl.InventoryID);

                string msg = "Unable to add item to transfer order";
                if (inventoryItem != null)
                {
                    msg = string.Format("Unable to add production order '{0}' material inventory ID '{1}' to transfer",
                        prodMatl.ProdOrdID.TrimIfNotNullEmpty(), inventoryItem.InventoryCD.TrimIfNotNullEmpty());
                }

                throw new PXException($"{msg}: {e.Message}", e);
            }
        }

        /// <summary>
        /// Create Manufacturing production orders
        /// </summary>
        /// <param name="prodCreatedFrom">ProdItem Record that generated critical Material</param>
        /// <param name="prodMatlList">List of production material records</param>
        /// <param name="filter">Create Production Order Filter from Smart panel</param>
        /// <param name="numbering">Production Numbering from the order type</param>
        /// <returns>List of Production orders created</returns>
        public virtual List<AMProdItem> CreateProductionOrders(AMProdItem prodCreatedFrom, List<SelectedProdMatl> prodMatlList,
            CreateProdFilter filter, Numbering numbering, List<AMProdItem> ordersList)
        {
            if (prodMatlList == null || prodMatlList.Count == 0)
            {
                return ordersList;
            }

            var prodMaintGraph = PXGraph.CreateInstance<ProdMaint>();

            using (PXTransactionScope scope = new PXTransactionScope())
            {
                int orderCount = 0;
				int matlIndex = 0;

                foreach (var prodMatl in prodMatlList)
                {
                    ordersList = CreateProductionOrder(prodCreatedFrom, prodMatl, filter, numbering, prodMaintGraph, ordersList);
					var orderCreated = ordersList.Count > orderCount;
					if (!orderCreated)
					{
						var invItem = InventoryItem.PK.Find(this, prodMatl.InventoryID);
						var prodOper = AMProdOper.PK.Find(this, prodMatl.OrderType, prodMatl.ProdOrdID, prodMatl.OperationID);

						var strMsg = Messages.GetLocal(Messages.ProductionOrderNotCreated, invItem.InventoryCD, prodOper.OperationCD);
						PXProcessing.SetError(matlIndex, strMsg);
					}
					else
					{
						var newOrder = ordersList.Count > orderCount ? ordersList[orderCount] : null;
						var orderCreatedMsg = Messages.GetLocal(Messages.CreatedProductionOrderForProductionOrder, newOrder?.OrderType, newOrder?.ProdOrdID, prodCreatedFrom.OrderType, prodCreatedFrom.ProdOrdID);
						PXProcessing.SetInfo(matlIndex, orderCreatedMsg);
					}

                    orderCount = ordersList.Count;
					matlIndex++;
                }

                this.Actions.PressSave();
                scope.Complete();
            }
            Clear();
            return ordersList;
        }

        public virtual List<AMProdItem> CreateProductionOrder(AMProdItem prodCreatedFrom, SelectedProdMatl sourceItemDetail,
            CreateProdFilter filter, Numbering numbering, ProdMaint prodMaintGraph, List<AMProdItem> ordersList)
        {
            var qtyToProd = filter.UseFullQty.GetValueOrDefault()
                ? sourceItemDetail.TotalQtyRequired.GetValueOrDefault()
                : sourceItemDetail.QtyShort.GetValueOrDefault();

            ordersList = CreateProductionOrder(prodCreatedFrom, qtyToProd, ConvertToProdMatl(sourceItemDetail), filter,
                numbering, prodMaintGraph, 0, ordersList, sourceItemDetail.PlanID);

            return ordersList;
        }

        public virtual List<AMProdItem> CreateProductionOrder(AMProdItem prodCreatedFrom, decimal qtyToProduce, AMProdMatl prodMatl,
            CreateProdFilter filter, Numbering numbering, ProdMaint prodMaintGraph, int level, List<AMProdItem> ordersList)
        {
            return CreateProductionOrder(prodCreatedFrom, qtyToProduce, prodMatl, filter, numbering, prodMaintGraph, level, ordersList, null);
        }

        public virtual List<AMProdItem> CreateProductionOrder(AMProdItem prodCreatedFrom, decimal qtyToProduce, AMProdMatl prodMatl,
            CreateProdFilter filter, Numbering numbering, ProdMaint prodMaintGraph, int level, List<AMProdItem> ordersList, long? splitPlanID)
        {
            if (prodMatl == null || prodCreatedFrom == null)
            {
                return ordersList;
            }

            prodMaintGraph.Clear();
            prodMaintGraph.IsImport = true;

            if (qtyToProduce <= 0)
            {
                return ordersList;
            }

            var amProdItem = CreateNewProdItem(prodCreatedFrom, qtyToProduce, prodMatl, filter, numbering, prodMaintGraph);

            if (amProdItem == null)
            {
                return ordersList;
            }

            ordersList.Add(amProdItem);

            prodMaintGraph.Actions.PressSave();

            // Check if Qty to Produce greater than Created Prod Item Qty -- MAX Order Qty Exceeded
            if (qtyToProduce > amProdItem.QtytoProd.GetValueOrDefault())
            {
                CreateProductionOrder(prodCreatedFrom, qtyToProduce - amProdItem.QtytoProd.GetValueOrDefault(),
                    prodMatl, filter, numbering, prodMaintGraph, level, ordersList);
            }

            // Check for SplitPlanID If True Created from Critical Material else ProdMaint Auto Create Linked orders
            if (splitPlanID != null)
            {
                // Set Mark for Production on AMProdMatl 
                prodMatl.ProdCreate = true;
                MaterialUpdate.Update(prodMatl);

                // Set Mark for Production on AMProdMatlSplit
                AMProdMatlSplit split = MatlSplitUpdate.Cache.LocateElse<AMProdMatlSplit>(PXSelect<AMProdMatlSplit, Where<AMProdMatlSplit.planID,
                    Equal<Required<AMProdMatlSplit.planID>>>>.Select(this, splitPlanID));

                if (split != null)
                {
                    split.AMOrderType = amProdItem.OrderType;
                    split.AMProdOrdID = amProdItem.ProdOrdID;
                    split.ProdCreate = true;
                    MatlSplitUpdate.Update(split);
                }
            }

            if (!string.IsNullOrWhiteSpace(amProdItem.BOMID) && filter.CreateLinkedOrders.GetValueOrDefault())
            {
                ordersList = CreateLinkedProductionOrders(amProdItem, filter, prodMaintGraph, numbering, level, ordersList);
            }

            return ordersList;
        }

        /// <summary>
        /// Create/Insert a new Production Order record using the given ProdMaint graph
        /// </summary>
        /// <param name="parentAMProdItem">Parent production record</param>
        /// <param name="qtyToProduce">Qty to produce</param>
        /// <param name="prodMatl">Production material record from the parent order as the base for the child order</param>
        /// <param name="filter">Critical materials create manufactured production order filter</param>
        /// <param name="numbering">Production order type numbering sequence</param>
        /// <param name="prodMaintGraph">Production order maintenance graph for creating the production order</param>
        /// <returns>Current/Inserted production record</returns>
        public virtual AMProdItem CreateNewProdItem(AMProdItem parentAMProdItem, decimal qtyToProduce, AMProdMatl prodMatl, CreateProdFilter filter, Numbering numbering, ProdMaint prodMaintGraph)
        {
            AMProdItem amProdItem = new AMProdItem
            {
                OrderType = filter.OrderType,
                DetailSource = ProductionDetailSource.BOM
            };

            if (numbering.UserNumbering.GetValueOrDefault())
            {
                amProdItem.ProdOrdID = filter.ProdOrdID;
            }

            amProdItem.SupplyType = ProductionSupplyType.Production;
            amProdItem = PXCache<AMProdItem>.CreateCopy(prodMaintGraph.ProdMaintRecords.Insert(amProdItem));

            amProdItem.InventoryID = prodMatl.InventoryID;
            amProdItem.SubItemID = prodMatl.SubItemID;

            if (filter.OverrideWarehouse.GetValueOrDefault())
            {
                amProdItem.SiteID = filter.SiteID;
                amProdItem.LocationID = filter.LocationID;
            }
            else
            {
                amProdItem.SiteID = prodMatl.SiteID ?? parentAMProdItem.SiteID;
                amProdItem.LocationID = prodMatl.LocationID ?? InventoryHelper.DfltLocation.GetDefault(this, InventoryHelper.DfltLocation.BinType.Receipt,
                    amProdItem.InventoryID, amProdItem.SiteID, true);
            }

            amProdItem.QtytoProd = qtyToProduce;

            decimal checkMinMaxLotQty = InventoryHelper.GetMfgReorderQty(prodMaintGraph, amProdItem.InventoryID.GetValueOrDefault(),
                amProdItem.SiteID.GetValueOrDefault(), amProdItem.QtytoProd.GetValueOrDefault());

            // If Check Qty above not equal to current QtytoProd, update orderQty
            if (amProdItem.QtytoProd.GetValueOrDefault() != checkMinMaxLotQty)
            {
                amProdItem.QtytoProd = checkMinMaxLotQty;
            }

            amProdItem = PXCache<AMProdItem>.CreateCopy(prodMaintGraph.ProdMaintRecords.Update(amProdItem));

            amProdItem.ProductOrderType = parentAMProdItem.ProductOrderType;
            amProdItem.ProductOrdID = parentAMProdItem.ProductOrdID;
			if ((prodMatl.CompBOMID != null && amProdItem.BOMID != prodMatl.CompBOMID)
				|| (prodMatl.CompBOMRevisionID != null && amProdItem.BOMRevisionID != prodMatl.CompBOMRevisionID))
			{
				var oldValue = amProdItem.BOMID;
				amProdItem.BOMID = prodMatl.CompBOMID;
				prodMaintGraph.ProdMaintRecords.Cache.RaiseFieldUpdated<AMProdItem.bOMID>(amProdItem, oldValue);
				if (prodMatl.CompBOMRevisionID != null)
				{
					amProdItem.BOMRevisionID = prodMatl.CompBOMRevisionID;
				}
			}

			if (string.IsNullOrWhiteSpace(amProdItem.ProductOrdID))
            {
                amProdItem.ProductOrderType = parentAMProdItem.OrderType;
                amProdItem.ProductOrdID = parentAMProdItem.ProdOrdID;
            }
            amProdItem.ParentOrderType = parentAMProdItem.OrderType;
            amProdItem.ParentOrdID = parentAMProdItem.ProdOrdID;
            amProdItem.UOM = prodMatl.UOM;
            amProdItem.FMLTime = parentAMProdItem.FMLTime;

            switch (parentAMProdItem.SchedulingMethod)
            {
                case ScheduleMethod.FinishOn:
                    amProdItem.SchedulingMethod = ScheduleMethod.FinishOn;
                    amProdItem.ConstDate = parentAMProdItem.StartDate.GetValueOrDefault().AddDays(-1);
                    break;
                case ScheduleMethod.UserDates:
                    amProdItem.SchedulingMethod = ScheduleMethod.UserDates;
                    amProdItem.StartDate = parentAMProdItem.StartDate;
                    amProdItem.EndDate = parentAMProdItem.EndDate;
                    break;
                default:
                    amProdItem.SchedulingMethod = ScheduleMethod.FinishOn;
                    amProdItem.ConstDate = parentAMProdItem.StartDate.GetValueOrDefault().AddDays(-1);
                    break;
            }

            amProdItem.BuildProductionBom = true;
            amProdItem.Reschedule = true;
            amProdItem.ProjectID = parentAMProdItem.ProjectID;
            amProdItem.TaskID = parentAMProdItem.TaskID;
            amProdItem.CostCodeID = parentAMProdItem.CostCodeID;
            amProdItem.UpdateProject = parentAMProdItem.UpdateProject;
            return prodMaintGraph.ProdMaintRecords.Update(amProdItem);
        }

        public static List<AMProdItem> CreateLinkedProductionOrders(AMProdItem prodCreatedFrom, List<AMProdItem> ordersList)
        {
            if (prodCreatedFrom == null)
            {
                throw new PXArgumentException(nameof(prodCreatedFrom));
            }

            var prodMaintGraph = CreateInstance<ProdMaint>();

            Numbering numbering = PXSelectJoin<Numbering,
                LeftJoin<AMOrderType, On<AMOrderType.prodNumberingID, Equal<Numbering.numberingID>>>,
                Where<AMOrderType.orderType, Equal<Required<AMOrderType.orderType>>>
            >.Select(prodMaintGraph, prodCreatedFrom.OrderType);

            if (numbering == null || numbering.UserNumbering.GetValueOrDefault())
            {
                throw new PXException(Messages.CreateLinkedOrdersRequiresAutoNumber, prodCreatedFrom.OrderType, numbering?.NumberingID.TrimIfNotNullEmpty());
            }

            var criticalMaterialGraph = CreateInstance<CriticalMaterialsInq>();

            var createProdFilter = new CreateProdFilter
            {
                OrderType = prodCreatedFrom.OrderType,
                UseFullQty = true,
                CreateLinkedOrders = true,
                OverrideWarehouse = false
            };

            criticalMaterialGraph.CreateProductionOrderFilter.Current = createProdFilter;

            using (var createLinkedOrdersScope = new PXTransactionScope())
            {
                ordersList = criticalMaterialGraph.CreateLinkedProductionOrders(prodCreatedFrom, createProdFilter, prodMaintGraph,
                    numbering, 0, ordersList);
                criticalMaterialGraph.Actions.PressSave();
                createLinkedOrdersScope.Complete();
            }

            return ordersList;
        }

        protected virtual List<AMProdItem> CreateLinkedProductionOrders(AMProdItem prodCreatedFrom, CreateProdFilter filter, ProdMaint prodMaintGraph,
            Numbering numbering, int level, List<AMProdItem> ordersList)
        {
            if (prodCreatedFrom == null || filter == null)
            {
                return ordersList;
            }

            if (numbering == null || numbering.UserNumbering.GetValueOrDefault())
            {
                throw new PXException(Messages.CreateLinkedOrdersRequiresAutoNumber, prodCreatedFrom.OrderType, numbering?.NumberingID.TrimIfNotNullEmpty());
            }

            if (level >= LowLevel.MaxLowLevel)
            {
                throw new PXException(Messages.MaxAutoLinkedOrders, LowLevel.MaxLowLevel);
            }

            foreach (AMProdMatl result in PXSelect<AMProdMatl,
                Where<AMProdMatl.orderType, Equal<Required<AMProdMatl.orderType>>,
                And<AMProdMatl.prodOrdID, Equal<Required<AMProdMatl.prodOrdID>>>>
                >.Select(prodMaintGraph, prodCreatedFrom.OrderType, prodCreatedFrom.ProdOrdID))
            {
                if (result?.ProdOrdID == null || result.IsByproduct.GetValueOrDefault())
                {
                    continue;
                }

                var amProdMatl = (AMProdMatl) prodMaintGraph.ProdMatlRecords.Cache.CreateCopy(result);
                if (amProdMatl == null)
                {
                    continue;
                }

				var split = AMProdMatl.GetSplits(this, amProdMatl)?.FirstOrDefault();
				if (split != null && (split.AMProdOrdID != null || split.POOrderNbr != null))
                {
                    //Contains an existing reference... bail out
                    continue;
                }

                var site = filter.OverrideWarehouse.GetValueOrDefault() ? filter.SiteID : (amProdMatl.SiteID ?? prodCreatedFrom.SiteID);

                if (InventoryHelper.GetReplenishmentSource(this, amProdMatl.InventoryID, site) != INReplenishmentSource.Manufactured)
                {
                    continue;
                }

                var bomid = new PrimaryBomIDManager(this).GetPrimaryAllLevels(amProdMatl.InventoryID, site, amProdMatl.SubItemID);
                if (string.IsNullOrWhiteSpace(bomid))
                {
                    continue;
                }

                if (amProdMatl.QtyRemaining.GetValueOrDefault() <= 0m)
                {
                    continue;
                }

                if (!amProdMatl.ProdCreate.GetValueOrDefault())
                {
                    amProdMatl.ProdCreate = true;
                    amProdMatl = prodMaintGraph.ProdMatlRecords.Update(amProdMatl);
                }

                ordersList = CreateProductionOrder(prodCreatedFrom, amProdMatl.QtyRemaining.GetValueOrDefault(), amProdMatl, filter, numbering,
                    prodMaintGraph, level, ordersList, split.PlanID);
            }
            return ordersList;
        }

        protected virtual void ProdItemFilter_OrderType_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var prodItemFilter = (ProdItemFilter) e.Row;
            if (prodItemFilter == null || prodItemFilter.OrderType == null)
            {
                return;
            }

            prodItemFilter.ProdOrdID = null;
        }

        protected virtual void CreateProdFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            var row = (CreateProdFilter)e.Row;
            if (row == null)
            {
                return;
            }

            PXUIFieldAttribute.SetEnabled<CreateProdFilter.prodOrdID>(cache, row,
                CurrentProductionNumbering != null && CurrentProductionNumbering.UserNumbering.GetValueOrDefault());
            PXUIFieldAttribute.SetEnabled<CreateProdFilter.siteID>(cache, row,
                row.OverrideWarehouse.GetValueOrDefault());
            PXUIFieldAttribute.SetEnabled<CreateProdFilter.locationID>(cache, row,
                row.OverrideWarehouse.GetValueOrDefault());
            PXUIFieldAttribute.SetEnabled<CreateProdFilter.createLinkedOrders>(cache, row,
                CurrentProductionNumbering != null && !CurrentProductionNumbering.UserNumbering.GetValueOrDefault());
        }

        protected virtual void CreatePurchaseOrdFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            var row = (CreatePurchaseOrdFilter)e.Row;
            if (row == null)
            {
                return;
            }

            PXUIFieldAttribute.SetEnabled<CreatePurchaseOrdFilter.orderNbr>(cache, row,
                CurrentPurchaseNumbering != null && CurrentPurchaseNumbering.UserNumbering.GetValueOrDefault());
        }

        protected virtual void CreateProdFilter_ProdOrdID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var createProdOrderFilter = (CreateProdFilter) e.Row;
            if (createProdOrderFilter == null)
            {
                return;
            }

            if (CurrentProductionNumbering.UserNumbering == true)
            {
                AMProdItem amProdItem = PXSelect
                    <AMProdItem, Where<AMProdItem.orderType, Equal<Required<AMProdItem.orderType>>,
                        And<AMProdItem.prodOrdID, Equal<Required<AMProdItem.prodOrdID>>>
                        >>.Select(this, createProdOrderFilter.OrderType, createProdOrderFilter.ProdOrdID);
                if (amProdItem != null)
                {
                    throw new PXSetPropertyException(Messages.GetLocal(Messages.ProductionOrderIDIsAlreadyUsed,
                            createProdOrderFilter.ProdOrdID, createProdOrderFilter.OrderType));
                }
            }
        }

        protected virtual void TROrderFilter_OrderNbr_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var trOrderFilter = (TROrderFilter)e.Row;
            if (trOrderFilter == null)
            {
                return;
            }

            if (CurrentTROrderTypeNumbering.UserNumbering == true)
            {
                SOOrder soOrder = PXSelect
                    <SOOrder, Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                        And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>
                        >>.Select(this, trOrderFilter.OrderType, trOrderFilter.OrderNbr);
                if (soOrder != null)
                {
                    throw new PXSetPropertyException(Messages.GetLocal(Messages.TransferIDIsAlreadyUsed,
                            trOrderFilter.OrderNbr, trOrderFilter.OrderType));
                }
            }
        }

        protected virtual void CreatePurchaseOrdFilter_OrderNbr_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var createPurchaseOrdFilter = (CreatePurchaseOrdFilter)e.Row;
            if (createPurchaseOrdFilter == null)
            {
                return;
            }

            if (CurrentPurchaseNumbering.UserNumbering == true)
            {
                POOrder poOrder = PXSelect
                    <POOrder, Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
                        And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>
                        >>.Select(this, createPurchaseOrdFilter.OrderType, createPurchaseOrdFilter.OrderNbr);
                if (poOrder != null)
                {
                    throw new PXSetPropertyException(Messages.GetLocal(Messages.PurchaseOrderIDIsAlreadyUsed,
                            createPurchaseOrdFilter.OrderNbr, createPurchaseOrdFilter.OrderType));
                }
            }
        }

        protected virtual int GetTransferleadTime(int? inventoryID, int? fromSiteID, int? toSiteID)
        {
            return InventoryHelper.GetTransferLeadTime(this, inventoryID, fromSiteID, toSiteID);
        }
    }

	/// <summary>
	/// Processing or inquiry information based on the <see cref="AMProdMatl"/> class for use on the Critical Materials (AM401000) form (corresponding to the <see cref="CriticalMaterialsInq"/> graph).
	/// </summary>
	[Serializable]
    [PXBreakInheritance]
    [PXCacheName(Messages.ProductionMatl)]
    public class SelectedProdMatl : AMProdMatl
    {
	    #region Keys
	    public new class PK : PrimaryKeyOf<SelectedProdMatl>.By<orderType, prodOrdID, operationID, lineID>
	    {
		    public static SelectedProdMatl Find(PXGraph graph, string orderType, string prodOrdID, int? operationID, int? lineID, PKFindOptions options = PKFindOptions.None) 
			    => FindBy(graph, orderType, prodOrdID, operationID, lineID, options);
	    }

	    public new static class FK
	    {
            public class Operation : AMProdOper.PK.ForeignKeyOf<SelectedProdMatl>.By<orderType, prodOrdID, operationID> { }
            public class ProductionOrder : AMProdItem.PK.ForeignKeyOf<SelectedProdMatl>.By<orderType, prodOrdID> { }
	    }
	    #endregion

        #region OperationID
        public new abstract class operationID : PX.Data.BQL.BqlInt.Field<operationID> { }

        [OperationIDField(IsKey = true, Visible = true, Enabled = false)]
        [PXDBDefault(typeof(AMProdOper.operationID))]
        [PXParent(typeof(FK.Operation))]
        [PXParent(typeof(FK.ProductionOrder))]
        [PXSelector(typeof(Search<AMProdOper.operationID,
                Where<AMProdOper.orderType, Equal<Current<SelectedProdMatl.orderType>>,
                    And<AMProdOper.prodOrdID, Equal<Current<SelectedProdMatl.prodOrdID>>>>>),
            SubstituteKey = typeof(AMProdOper.operationCD))]
        public override int? OperationID
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


        public new abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }
        public new abstract class lineID : PX.Data.BQL.BqlInt.Field<lineID> { }
        public new abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        public new abstract class prodOrdID : PX.Data.BQL.BqlString.Field<prodOrdID> { }

        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }

        protected bool? _Selected;
        [PXBool]
        [PXUnboundDefault(false)]
        [PXUIField(DisplayName = "Selected", Enabled = true)]
        public virtual bool? Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }
        #endregion
        #region QtyOnHand
        public abstract class qtyOnHand : PX.Data.BQL.BqlDecimal.Field<qtyOnHand> { }


        protected Decimal? _QtyOnHand;
        [PXQuantity]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Qty On Hand", Enabled = false)]
        public virtual Decimal? QtyOnHand
        {
            get { return this._QtyOnHand; }
            set { this._QtyOnHand = value; }
        }

        #endregion
        #region QtyShort
        public abstract class qtyShort : PX.Data.BQL.BqlDecimal.Field<qtyShort> { }

        protected Decimal? _QtyShort;

        [PXQuantity]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Qty Short")]
        public virtual Decimal? QtyShort
        {
            get { return this._QtyShort; }
            set { this._QtyShort = value; }
        }
        #endregion

        #region ReplenishmentSource
        public abstract class replenishmentSource : PX.Data.BQL.BqlString.Field<replenishmentSource> { }

        protected String _ReplenishmentSource;

		/// <summary>
		/// Replenishment source
		/// </summary>
		[PXString(1)]
        [PXUIField(DisplayName = "Replenishment Source", Visible = true, Enabled = false)]
        [INReplenishmentSource.List]
        public virtual String ReplenishmentSource
        {
            get { return this._ReplenishmentSource; }
            set { this._ReplenishmentSource = value; }
        }
        #endregion
        #region QtyAvail
        public abstract class qtyAvail : PX.Data.BQL.BqlDecimal.Field<qtyAvail> { }

        protected Decimal? _QtyAvail;
        [PXQuantity]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Available", Enabled = false, Visible = false)]
        public virtual Decimal? QtyAvail
        {
            get
            {
                return this._QtyAvail;
            }
            set
            {
                this._QtyAvail = value;
            }
        }
        #endregion
        #region QtyHardAvail
        public abstract class qtyHardAvail : PX.Data.BQL.BqlDecimal.Field<qtyHardAvail> { }

        protected Decimal? _QtyHardAvail;
        [PXQuantity]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Hard Available", Enabled = false)]
        public virtual Decimal? QtyHardAvail
        {
            get
            {
                return this._QtyHardAvail;
            }
            set
            {
                this._QtyHardAvail = value;
            }
        }
        #endregion
        #region QtyProductionSupplyPrepared
        public abstract class qtyProductionSupplyPrepared : PX.Data.BQL.BqlDecimal.Field<qtyProductionSupplyPrepared> { }

        protected Decimal? _QtyProductionSupplyPrepared;
        [PXQuantity]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Production Supply Prepared", Enabled = false)]
        public virtual Decimal? QtyProductionSupplyPrepared
        {
            get
            {
                return this._QtyProductionSupplyPrepared;
            }
            set
            {
                this._QtyProductionSupplyPrepared = value;
            }
        }
        #endregion
        #region QtyProductionSupply
        public abstract class qtyProductionSupply : PX.Data.BQL.BqlDecimal.Field<qtyProductionSupply> { }

        protected Decimal? _QtyProductionSupply;
        [PXQuantity]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Production Supply", Enabled = false)]
        public virtual Decimal? QtyProductionSupply
        {
            get
            {
                return this._QtyProductionSupply;
            }
            set
            {
                this._QtyProductionSupply = value;
            }
        }
        #endregion
        #region QtyProductionDemandPrepared
        public abstract class qtyProductionDemandPrepared : PX.Data.BQL.BqlDecimal.Field<qtyProductionDemandPrepared> { }

        protected Decimal? _QtyProductionDemandPrepared;
        [PXQuantity]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Production Demand Prepared", Enabled = false)]
        public virtual Decimal? QtyProductionDemandPrepared
        {
            get
            {
                return this._QtyProductionDemandPrepared;
            }
            set
            {
                this._QtyProductionDemandPrepared = value;
            }
        }
        #endregion
        #region QtyProductionDemand
        public abstract class qtyProductionDemand : PX.Data.BQL.BqlDecimal.Field<qtyProductionDemand> { }

        protected Decimal? _QtyProductionDemand;
        [PXQuantity]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Production Demand", Enabled = false)]
        public virtual Decimal? QtyProductionDemand
        {
            get
            {
                return this._QtyProductionDemand;
            }
            set
            {
                this._QtyProductionDemand = value;
            }
        }
        #endregion
        #region RequiredDate
        public abstract class requiredDate : PX.Data.BQL.BqlDateTime.Field<requiredDate> { }

        protected DateTime? _RequiredDate;
        [PXDate]
        [PXUIField(DisplayName = "Required Date", Enabled = false, Visible = false)]
        public virtual DateTime? RequiredDate
        {
            get
            {
                return this._RequiredDate;
            }
            set
            {
                this._RequiredDate = value;
            }
        }
        #endregion
        #region IsByproduct2
        public abstract class isByproduct2 : PX.Data.BQL.BqlBool.Field<isByproduct2> { }

        protected Boolean? _IsByproduct2;
        [PXBool]
        [PXUIField(DisplayName = "By-product", Enabled = false, Visible = false)]
        public virtual Boolean? IsByproduct2
        {
            get
            {
                return this._IsByproduct2;
            }
            set
            {
                this._IsByproduct2 = value;
            }
        }
        #endregion

        #region PreferredVendorID
        public abstract class preferredVendorID : PX.Data.BQL.BqlInt.Field<preferredVendorID> { }

        protected Int32? _PreferredVendorID;
        [PXDefault(typeof(Search<InventoryItem.preferredVendorID, Where<InventoryItem.inventoryID, Equal<Current<SelectedProdMatl.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [Vendor(DisplayName = "Preferred Vendor ID", Visible = true, Enabled = false, Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), IsDBField = false)]
        public virtual Int32? PreferredVendorID
        {
            get
            {
                return this._PreferredVendorID;
            }
            set
            {
                this._PreferredVendorID = value;
            }
        }
        #endregion
        #region Item Class ID
        public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }

        protected int? _ItemClassID;
        [PXInt]
        [PXUIField(DisplayName = "Item Class", Enabled = false, Visible = true)]
        [PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID>), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr))]
        public virtual int? ItemClassID
        {
            get { return this._ItemClassID; }
            set { this._ItemClassID = value; }
        }

        #endregion
        #region PlanID
        public abstract class planID : PX.Data.BQL.BqlLong.Field<planID> { }

        protected Int64? _PlanID;
        [PXLong]
        [PXUIField(DisplayName = "Plan ID", Visible = false, Enabled = false)]
        public virtual Int64? PlanID
        {
            get
            {
                return this._PlanID;
            }
            set
            {
                this._PlanID = value;
            }
        }
        #endregion
        #region IsAllocated
        public abstract class isAllocated : PX.Data.BQL.BqlBool.Field<isAllocated> { }

        protected Boolean? _IsAllocated;
        [PXBool]
        [PXUnboundDefault(false)]
        [PXUIField(DisplayName = "Allocated", Enabled = false)]
        public virtual Boolean? IsAllocated
        {
            get
            {
                return this._IsAllocated;
            }
            set
            {
                this._IsAllocated = value;
            }
        }
        #endregion
        #region POOrderNbr
        public abstract class pOOrderNbr : PX.Data.BQL.BqlString.Field<pOOrderNbr> { }

        protected String _POOrderNbr;
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "PO Order Nbr.", Enabled = false)]
        [PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.orderType, Equal<POOrderType.regularOrder>>>), DescriptionField = typeof(POOrder.orderDesc), ValidateValue = false)]
        public virtual String POOrderNbr
        {
            get
            {
                return this._POOrderNbr;
            }
            set
            {
                this._POOrderNbr = value;
            }
        }
        #endregion
        #region AMOrderType
        public abstract class aMOrderType : PX.Data.BQL.BqlString.Field<aMOrderType> { }

        protected String _AMOrderType;
        [PXString(2, IsFixed = true, InputMask = ">aa")]
        [PXUIField(DisplayName = "Sub. Assy. Order Type")]
        [AMOrderTypeSelector]
        public virtual String AMOrderType
        {
            get
            {
                return this._AMOrderType;
            }
            set
            {
                this._AMOrderType = value;
            }
        }
        #endregion
        #region AMProdOrdID
        public abstract class aMProdOrdID : PX.Data.BQL.BqlString.Field<aMProdOrdID> { }

        protected String _AMProdOrdID;
        [ProductionNbr(Enabled = false, DisplayName = "Sub. Assy. Production Nbr.", IsDBField = false)]
        [ProductionOrderSelector(typeof(SelectedProdMatl.aMOrderType), true, ValidateValue = false)]
        public virtual String AMProdOrdID
        {
            get
            {
                return this._AMProdOrdID;
            }
            set
            {
                this._AMProdOrdID = value;
            }
        }
        #endregion
        #region QtyAlloc
        public abstract class qtyAlloc : PX.Data.BQL.BqlDecimal.Field<qtyAlloc> { }

        protected Decimal? _QtyAlloc;
        [PXQuantity]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Quantity", Enabled = false)]
        public virtual Decimal? QtyAlloc
        {
            get
            {
                return this._QtyAlloc;
            }
            set
            {
                this._QtyAlloc = value;
            }
        }
		#endregion
		#region ReplenishmentSourceSiteID
		public abstract class replenishmentSourceSiteID : PX.Data.BQL.BqlInt.Field<replenishmentSourceSiteID> { }

		protected int? _ReplenishmentSourceSiteID;
		[PXInt]
		[PXUIField(DisplayName = "Replenishment Warehouse", Enabled = false, Visible = true)]		
		public virtual int? ReplenishmentSourceSiteID
		{
			get { return this._ReplenishmentSourceSiteID; }
			set { this._ReplenishmentSourceSiteID = value; }
		}

		#endregion
	}

	/// <summary>
	/// Required filter for displaying the information on the Critical Materials (AM401000) form (corresponding to the <see cref="CriticalMaterialsInq"/> graph).
	/// </summary>
	[Serializable]
    [PXCacheName("Production Filter")]
    public class ProdItemFilter : PXBqlTable, IBqlTable
    {
        #region OrderType
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }


        protected String _OrderType;
        [PXDefault(typeof(AMPSetup.defaultOrderType))]
        [AMOrderTypeField]
        [PXRestrictor(typeof(Where<AMOrderType.active, Equal<True>>), PX.Objects.SO.Messages.OrderTypeInactive)]
        [AMOrderTypeSelector]
        public virtual String OrderType
        {
            get { return this._OrderType; }
            set { this._OrderType = value; }
        }

        #endregion
        #region ProdOrdID

        public abstract class prodOrdID : PX.Data.BQL.BqlString.Field<prodOrdID> { }


        protected String _ProdOrdID;
        [ProductionNbr(Required = true)]
        [ProductionOrderSelector(typeof (ProdItemFilter.orderType), includeAll: true, DescriptionField = typeof(AMProdItem.descr), ValidateValue = false)]
        [PXDefault]
        public virtual String ProdOrdID
        {
            get { return this._ProdOrdID; }
            set { this._ProdOrdID = value; }
        }

        #endregion
        #region ShowAll

        public abstract class showAll : PX.Data.BQL.BqlBool.Field<showAll> { }


        protected Boolean? _ShowAll;

        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Show All Items")]
        public virtual Boolean? ShowAll
        {
            get { return this._ShowAll; }
            set { this._ShowAll = value; }
        }

        #endregion
        #region ShowAllocated

        public abstract class showAllocated : PX.Data.BQL.BqlBool.Field<showAllocated> { }


        protected Boolean? _ShowAllocated;

        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Show Allocated")]
        public virtual Boolean? ShowAllocated
        {
            get { return this._ShowAllocated; }
            set { this._ShowAllocated = value; }
        }

        #endregion
        #region RefType

        public abstract class refType : PX.Data.BQL.BqlString.Field<refType> { }


        protected String _RefType;

        [PXString]
        [PXUIField(DisplayName = "Ref type", Visible = false, Enabled = false)]
        [XRefType.List]
        [PXDefault(XRefType.Purchase, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual String RefType
        {
            get { return this._RefType; }
            set { this._RefType = value; }
        }

		#endregion

		#region BranchID
		public abstract class branchID : PX.Data.BQL.BqlString.Field<branchID> { }

		protected int? _BranchID;

        [Branch]
		[PXDefault(typeof(Search<AMProdItem.branchID, Where<AMProdItem.prodOrdID, Equal<Current<ProdItemFilter.prodOrdID>>,
			And<AMProdItem.orderType, Equal<Current<ProdItemFilter.orderType>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<ProdItemFilter.prodOrdID>))]

		public int? BranchID
		{
			get { return this._BranchID; }
			set { this._BranchID = value; }
		}
		#endregion
	}

	/// <summary>
	/// The filter for the "Create Production Order" pop-up panel that is opened when a user clicks "Manufacture" on the Critical Materials (AM401000) form (corresponding to the <see cref="CriticalMaterialsInq"/> graph).
	/// </summary>
	[Serializable]
    [PXCacheName("Create Production Filter")]
    public class CreateProdFilter : PXBqlTable, IBqlTable
    {
        #region OrderType
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }

        protected String _OrderType;
        [PXDefault(typeof(Search<AMPSetup.defaultOrderType>))]
        [AMOrderTypeField]
        [PXRestrictor(typeof(Where<AMOrderType.function, Equal<OrderTypeFunction.regular>>), Messages.IncorrectOrderTypeFunction)]
        [PXRestrictor(typeof(Where<AMOrderType.active, Equal<True>>), PX.Objects.SO.Messages.OrderTypeInactive)]
        [AMOrderTypeSelector]
        public virtual String OrderType
        {
            get
            {
                return this._OrderType;
            }
            set
            {
                this._OrderType = value;
            }
        }
        #endregion
        #region ProdOrdID
        public abstract class prodOrdID : PX.Data.BQL.BqlString.Field<prodOrdID> { }

        protected String _ProdOrdID;
        [ProductionNbr]
        public virtual String ProdOrdID
        {
            get { return this._ProdOrdID; }
            set { this._ProdOrdID = value; }
        }
        #endregion
        #region SiteID
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

        protected Int32? _SiteID;
        [PXDefault(typeof(Search<InventoryItem.dfltSiteID, Where<InventoryItem.inventoryID, Equal<Current<CreateProdFilter.inventoryID>>>>), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [Site(Visibility = PXUIVisibility.SelectorVisible)]
        [PXForeignReference(typeof(Field<siteID>.IsRelatedTo<INSite.siteID>))]
        public virtual Int32? SiteID
        {
            get { return this._SiteID; }
            set { this._SiteID = value; }
        }
        #endregion
        #region LocationID
        public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }


        protected Int32? _LocationID;
        [MfgLocationAvail(typeof(CreateProdFilter.inventoryID), typeof(CreateProdFilter.subItemID), typeof(CreateProdFilter.siteID), false, true, KeepEntry = false, ResetEntry = true)]
        [PXDefault]
        [PXForeignReference(typeof(CompositeKey<Field<siteID>.IsRelatedTo<INLocation.siteID>, Field<locationID>.IsRelatedTo<INLocation.locationID>>))]
        public virtual Int32? LocationID
        {
            get { return this._LocationID; }
            set { this._LocationID = value; }
        }

        #endregion
        #region UseFullQty
        public abstract class useFullQty : PX.Data.BQL.BqlBool.Field<useFullQty> { }

        protected Boolean? _UseFullQty;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Use Full Qty Required")]
        public virtual Boolean? UseFullQty
        {
            get
            {
                return this._UseFullQty;
            }
            set
            {
                this._UseFullQty = value;
            }
        }
        #endregion
        #region OverrideWarehouse
        public abstract class overrideWarehouse : PX.Data.BQL.BqlBool.Field<overrideWarehouse> { }

        protected Boolean? _OverrideWarehouse;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Override Warehouse")]
        public virtual Boolean? OverrideWarehouse
        {
            get
            {
                return this._OverrideWarehouse;
            }
            set
            {
                this._OverrideWarehouse = value;
            }
        }
        #endregion
        #region CreateLinkedOrders
        public abstract class createLinkedOrders : PX.Data.BQL.BqlBool.Field<createLinkedOrders> { }

        protected Boolean? _CreateLinkedOrders;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Auto Create Linked Orders")]
        public virtual Boolean? CreateLinkedOrders
        {
            get
            {
                return this._CreateLinkedOrders;
            }
            set
            {
                this._CreateLinkedOrders = value;
            }
        }
        #endregion
        #region InventoryID
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

        protected Int32? _InventoryID;
        [StockItem(Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault]
        [PXForeignReference(typeof(Field<inventoryID>.IsRelatedTo<InventoryItem.inventoryID>))]
        public virtual Int32? InventoryID
        {
            get
            {
                return this._InventoryID;
            }
            set
            {
                this._InventoryID = value;
            }
        }
        #endregion
        #region SubItemID

        public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }

        protected Int32? _SubItemID;
        [PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
            Where<InventoryItem.inventoryID, Equal<Current<CreateProdFilter.inventoryID>>,
            And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
            PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [SubItem(typeof(CreateProdFilter.inventoryID))]
        [PXFormula(typeof(Default<CreateProdFilter.inventoryID>))]
        public virtual Int32? SubItemID
        {
            get
            {
                return this._SubItemID;
            }
            set
            {
                this._SubItemID = value;
            }
        }
        #endregion
        #region ProdDate
        public abstract class prodDate : PX.Data.BQL.BqlDateTime.Field<prodDate> { }

        protected DateTime? _ProdDate;
        [PXDBDate]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Order Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? ProdDate
        {
            get
            {
                return this._ProdDate;
            }
            set
            {
                this._ProdDate = value;
            }
        }
        #endregion
        #region QtytoProd
        public abstract class qtytoProd : PX.Data.BQL.BqlDecimal.Field<qtytoProd> { }

        protected Decimal? _QtytoProd;
        [PXDBQuantity(typeof(AMProdItem.uOM), typeof(AMProdItem.baseQtytoProd), HandleEmptyKey = true)]
        [PXDefault(TypeCode.Decimal, "1.0000")]
        [PXUIField(DisplayName = "Qty to Produce")]
        public virtual Decimal? QtytoProd
        {
            get
            {
                return this._QtytoProd;
            }
            set
            {
                this._QtytoProd = value;
            }
        }
        #endregion
        #region ProjectID
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }

        protected Int32? _ProjectID;
        [ProjectDefault]
        [PXRestrictor(typeof(Where<PMProject.isActive, Equal<True>>), PX.Objects.PM.Messages.InactiveContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInIN, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [ProjectBase(Enabled = false)]
        public virtual Int32? ProjectID
        {
            get
            {
                return this._ProjectID;
            }
            set
            {
                this._ProjectID = value;
            }
        }
        #endregion
        #region TaskID
        public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }

        protected Int32? _TaskID;
        [ActiveOrInPlanningProjectTask(typeof(projectID), Required = true)]
        public virtual Int32? TaskID
        {
            get
            {
                return this._TaskID;
            }
            set
            {
                this._TaskID = value;
            }
        }
        #endregion
        #region CostCodeID
        public abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID> { }

        protected Int32? _CostCodeID;
        [CostCode(null, typeof(taskID), null)]
        //[CostCode(typeof(wIPAcctID), typeof(taskID), PX.Objects.GL.AccountType.Asset)]
        public virtual Int32? CostCodeID
        {
            get
            {
                return this._CostCodeID;
            }
            set
            {
                this._CostCodeID = value;
            }
        }
        #endregion

    }

	/// <summary>
	/// The filter for the "Create Transfer" pop-up panel that is opened when a user clicks "Transfer" on the Critical Materials (AM401000) form (corresponding to the <see cref="CriticalMaterialsInq"/> graph).
	/// </summary>
	[Serializable]
    [PXCacheName("Transfer Order Filter")]
    public class TROrderFilter : PXBqlTable, IBqlTable
    {
        #region OrderType

        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }


        protected String _OrderType;

        [PXString(IsFixed = true, InputMask = ">aa")]
        [PXUnboundDefault(typeof(SOSetup.transferOrderType))]
        [PXSelector(typeof (Search5<SOOrderType.orderType, InnerJoin<SOOrderTypeOperation,
                    On<SOOrderTypeOperation.orderType, Equal<SOOrderType.orderType>,
                    And<SOOrderTypeOperation.operation, Equal<SOOrderType.defaultOperation>>>,
                    LeftJoin<SOSetupApproval, On<SOOrderType.orderType, Equal<SOSetupApproval.orderType>>>>,
                    Where<SOOrderTypeOperation.iNDocType, Equal<INTranType.transfer>>,
                    Aggregate<GroupBy<SOOrderType.orderType>>>))]
        [PXRestrictor(typeof (Where<SOOrderType.active, Equal<True>>), PX.Objects.SO.Messages.OrderTypeInactive)]
        [PXUIField(DisplayName = "Order Type", Required = true)]
        public virtual String OrderType
        {
            get { return this._OrderType; }
            set { this._OrderType = value; }
        }

        #endregion
        #region OrderNbr

        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }

        protected String _OrderNbr;
        [PXString(IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Order Nbr.", Required = true)]
        public virtual String OrderNbr
        {
            get { return this._OrderNbr; }
            set { this._OrderNbr = value; }
        }

        #endregion
        #region ToSiteID

        public abstract class toSiteID : PX.Data.BQL.BqlInt.Field<toSiteID> { }


        protected Int32? _ToSiteID;

        [PXDefault]
        [Site(DisplayName = "To Warehouse", DescriptionField = typeof (INSite.descr))]
        public virtual Int32? ToSiteID
        {
            get { return this._ToSiteID; }
            set { this._ToSiteID = value; }
        }

        #endregion
        #region FromSiteID

        public abstract class fromSiteID : PX.Data.BQL.BqlInt.Field<fromSiteID> { }


        protected Int32? _FromSiteID;

        [PXDefault]
        [Site(DisplayName = "From Warehouse", DescriptionField = typeof (INSite.descr))]
        public virtual Int32? FromSiteID
        {
            get { return this._FromSiteID; }
            set { this._FromSiteID = value; }
        }

        #endregion
        #region UseFullQty
        public abstract class useFullQty : PX.Data.BQL.BqlBool.Field<useFullQty> { }

        protected Boolean? _UseFullQty;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Use Full Qty Required")]
        public virtual Boolean? UseFullQty
        {
            get
            {
                return this._UseFullQty;
            }
            set
            {
                this._UseFullQty = value;
            }
        }
        #endregion
    }

	/// <summary>
	/// The filter for the "Create Purchase Order" pop-up panel that is opened when a user clicks "Purchase" on the Critical Materials (AM401000) form (corresponding to the <see cref="CriticalMaterialsInq"/> graph).
	/// </summary>
	[Serializable]
    [PXCacheName("Create Purchase Order Filter")]
    public class CreatePurchaseOrdFilter : PXBqlTable, IBqlTable
    {
        #region OrderType
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }

        protected String _OrderType;
        [PXString(2, IsFixed = true)]
        [PXUnboundDefault(POOrderType.RegularOrder)]
        [POOrderType.List]
        [PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual String OrderType
        {
            get
            {
                return this._OrderType;
            }
            set
            {
                this._OrderType = value;
            }
        }
        #endregion
        #region OrderNbr
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }

        protected String _OrderNbr;
        [PXString(IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Order Nbr.", Required = true)]
        public virtual String OrderNbr
        {
            get { return this._OrderNbr; }
            set { this._OrderNbr = value; }
        }
        #endregion
        #region VendorID
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }

        protected Int32? _VendorID;
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName))]
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
        #region VendorDefaulted
        public abstract class vendorDefaulted : PX.Data.BQL.BqlBool.Field<vendorDefaulted> { }

        protected Boolean? _VendorDefaulted;
        [PXBool]
        [PXUnboundDefault(false)]
        [PXUIField(DisplayName = "Vendor Defaulted")]
        public virtual Boolean? VendorDefaulted
        {
            get
            {
                return this._VendorDefaulted;
            }
            set
            {
                this._VendorDefaulted = value;
            }
        }
        #endregion
        #region VendorLocationID

        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        protected Int32? _VendorLocationID;
        [LocationActive(typeof(Where<Location.bAccountID, Equal<Current<CreatePurchaseOrdFilter.vendorID>>,
            And<MatchWithBranch<Location.vBranchID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible,
            DisplayName = "Vendor Location")]
        [PXDefault(typeof(Coalesce<Search2<BAccountR.defLocationID,
            InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>,
            Where<BAccountR.bAccountID, Equal<Current<CreatePurchaseOrdFilter.vendorID>>,
                And<CRLocation.isActive, Equal<True>,
                And<MatchWithBranch<CRLocation.vBranchID>>>>>,
            Search<CRLocation.locationID,
            Where<CRLocation.bAccountID, Equal<Current<CreatePurchaseOrdFilter.vendorID>>,
            And<CRLocation.isActive, Equal<True>, And<MatchWithBranch<CRLocation.vBranchID>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<CreatePurchaseOrdFilter.vendorID>))]
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
    }

    [System.SerializableAttribute()]
	[PXProjection(typeof(Select5<INLocationStatusByCostCenter,
		InnerJoin<INLocation, On<INLocation.locationID, Equal<INLocationStatusByCostCenter.locationID>,
			And<INLocation.siteID, Equal<INLocationStatusByCostCenter.siteID>>>>,
		Where<INLocation.productionValid, Equal<True>,
			And<INLocationStatusByCostCenter.costCenterID, Equal<CostCenter.freeStock>>>,
		Aggregate<
			GroupBy<INLocationStatusByCostCenter.siteID,
			GroupBy<INLocationStatusByCostCenter.inventoryID,
			GroupBy<INLocationStatusByCostCenter.subItemID,
				Sum<INLocationStatusByCostCenter.qtyOnHand,
				Sum<INLocationStatusByCostCenter.qtyAvail,
				Sum<INLocationStatusByCostCenter.qtyHardAvail,
				Sum<INLocationStatusByCostCenter.qtyProductionSupplyPrepared,
				Sum<INLocationStatusByCostCenter.qtyProductionSupply,
				Sum<INLocationStatusByCostCenter.qtyProductionDemandPrepared,
				Sum<INLocationStatusByCostCenter.qtyProductionDemand>>>>>>>>>>>>), Persistent = false)]
    [PXHidden]
    public partial class INLocationTotals : PXBqlTable, PX.Data.IBqlTable
    {
        #region InventoryID
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        protected Int32? _InventoryID;
        [PXDBInt(IsKey = true, BqlField = typeof(INLocationStatusByCostCenter.inventoryID))]
        [PXDefault()]
        public virtual Int32? InventoryID
        {
            get
            {
                return this._InventoryID;
            }
            set
            {
                this._InventoryID = value;
            }
        }
        #endregion
        #region SubItemID
        public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
        protected Int32? _SubItemID;
        [PXDBInt(IsKey = true, BqlField = typeof(INLocationStatusByCostCenter.subItemID))]
        public virtual Int32? SubItemID
        {
            get
            {
                return this._SubItemID;
            }
            set
            {
                this._SubItemID = value;
            }
        }
        #endregion
        #region SiteID
        public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
        protected Int32? _SiteID;
        [Site(IsKey = true, BqlField = typeof(INLocationStatusByCostCenter.siteID))]
        [PXDefault()]
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
        #region QtyOnHand
        public abstract class qtyOnHand : PX.Data.BQL.BqlDecimal.Field<qtyOnHand> { }
        protected Decimal? _QtyOnHand;
        [PXDBQuantity(BqlField = typeof(INLocationStatusByCostCenter.qtyOnHand))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. On Hand")]
        public virtual Decimal? QtyOnHand
        {
            get
            {
                return this._QtyOnHand;
            }
            set
            {
                this._QtyOnHand = value;
            }
        }
        #endregion
        #region QtyAvail
        public abstract class qtyAvail : PX.Data.BQL.BqlDecimal.Field<qtyAvail> { }
        protected Decimal? _QtyAvail;
        [PXDBQuantity(BqlField = typeof(INLocationStatusByCostCenter.qtyAvail))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Available")]
        public virtual Decimal? QtyAvail
        {
            get
            {
                return this._QtyAvail;
            }
            set
            {
                this._QtyAvail = value;
            }
        }
        #endregion
        #region QtyHardAvail
        public abstract class qtyHardAvail : PX.Data.BQL.BqlDecimal.Field<qtyHardAvail> { }
        protected Decimal? _QtyHardAvail;
        [PXDBQuantity(BqlField = typeof(INLocationStatusByCostCenter.qtyHardAvail))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Hard Available")]
        public virtual Decimal? QtyHardAvail
        {
            get
            {
                return this._QtyHardAvail;
            }
            set
            {
                this._QtyHardAvail = value;
            }
        }
        #endregion
        #region QtyProductionSupplyPrepared
        public abstract class qtyProductionSupplyPrepared : PX.Data.BQL.BqlDecimal.Field<qtyProductionSupplyPrepared> { }
        protected Decimal? _QtyProductionSupplyPrepared;
        /// <summary>
        /// Production / Manufacturing 
        /// Specifies the quantity Production Supply Prepared.  
        /// </summary>
        [PXDBQuantity(BqlField = typeof(INLocationStatusByCostCenter.qtyProductionSupplyPrepared))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty Production Supply Prepared")]
        public virtual Decimal? QtyProductionSupplyPrepared
        {
            get
            {
                return this._QtyProductionSupplyPrepared;
            }
            set
            {
                this._QtyProductionSupplyPrepared = value;
            }
        }
        #endregion
        #region QtyProductionSupply
        public abstract class qtyProductionSupply : PX.Data.BQL.BqlDecimal.Field<qtyProductionSupply> { }
        protected Decimal? _QtyProductionSupply;
        /// <summary>
        /// Production / Manufacturing 
        /// Specifies the quantity Production Supply.  
        /// </summary>
        [PXDBQuantity(BqlField = typeof(INLocationStatusByCostCenter.qtyProductionSupply))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty On Production Supply")]
        public virtual Decimal? QtyProductionSupply
        {
            get
            {
                return this._QtyProductionSupply;
            }
            set
            {
                this._QtyProductionSupply = value;
            }
        }
        #endregion
        #region QtyProductionDemandPrepared
        public abstract class qtyProductionDemandPrepared : PX.Data.BQL.BqlDecimal.Field<qtyProductionDemandPrepared> { }
        protected Decimal? _QtyProductionDemandPrepared;
        /// <summary>
        /// Production / Manufacturing 
        /// Specifies the quantity On Production Demand Prepared.  
        /// </summary>
        [PXDBQuantity(BqlField = typeof(INLocationStatusByCostCenter.qtyProductionDemandPrepared))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty On Production Demand Prepared")]
        public virtual Decimal? QtyProductionDemandPrepared
        {
            get
            {
                return this._QtyProductionDemandPrepared;
            }
            set
            {
                this._QtyProductionDemandPrepared = value;
            }
        }
        #endregion
        #region QtyProductionDemand
        public abstract class qtyProductionDemand : PX.Data.BQL.BqlDecimal.Field<qtyProductionDemand> { }
        protected Decimal? _QtyProductionDemand;
        /// <summary>
        /// Production / Manufacturing 
        /// Specifies the quantity On Production Demand.  
        /// </summary>
        [PXDBQuantity(BqlField = typeof(INLocationStatusByCostCenter.qtyProductionDemand))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty On Production Demand")]
        public virtual Decimal? QtyProductionDemand
        {
            get
            {
                return this._QtyProductionDemand;
            }
            set
            {
                this._QtyProductionDemand = value;
            }
        }
        #endregion
    }
}

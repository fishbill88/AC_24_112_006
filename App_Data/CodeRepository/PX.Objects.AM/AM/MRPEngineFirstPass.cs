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
using System.Collections.Generic;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.AM.Attributes;
using PX.Objects.AM.CacheExtensions;
using PX.Data.BQL.Fluent;
using System.Linq;
using PX.Common;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.PO;
using static PX.Objects.AM.LowLevel;
using PX.Data.BQL;

namespace PX.Objects.AM
{
    /// <summary>
    /// MRP First Pass Process. 
    /// Computes high-level requirements.
    /// </summary>
    public class MRPEngineFirstPass
    {
        public readonly MRPEngine MrpGraph;

		public string PlanTypeForSafetyStock => MrpGraph.Setup.Current?.StockingMethod == AMRPSetup.MRPStockingMethod.SafetyStock ? MRPPlanningType.SafetyStock : MRPPlanningType.ReorderPoint;

        public virtual bool IncludeOnHoldProductionOrder => MrpGraph.Setup.Current != null && MrpGraph.Setup.Current.IncludeOnHoldProductionOrder.GetValueOrDefault();

        public virtual bool IncludeOnHoldPurchaseOrder => MrpGraph.Setup.Current != null && MrpGraph.Setup.Current.IncludeOnHoldPurchaseOrder.GetValueOrDefault();

        public virtual bool IncludeOnHoldSalesOrder => MrpGraph.Setup.Current != null && MrpGraph.Setup.Current.IncludeOnHoldSalesOrder.GetValueOrDefault();

		public virtual bool IncludeOnHoldKitAssembly => MrpGraph.Setup.Current != null && MrpGraph.Setup.Current.IncludeOnHoldKitAssemblies.GetValueOrDefault();

		public MRPEngineFirstPass(MRPEngine mrpGraph)
        {
            MrpGraph = mrpGraph ?? throw new ArgumentException(nameof(mrpGraph));

            if (!Features.MRPOrDRPEnabled())
            {
                throw new PXException(Messages.UnableToProcess, this.GetType().Name);
            }
        }

        /// <summary>
        /// Static call to run all Load methods related to MRP first pass process
        /// </summary>
        /// <param name="mrpGraph">calling mrp graph</param>
        /// <param name="mrpItemDictionary">mrp inventory cache</param>
        public static void LoadAll(MRPEngine mrpGraph, MRPProcessCache.MrpItemDictionary mrpItemDictionary)
        {
            var mrpFirstPass = new MRPEngineFirstPass(mrpGraph);

            mrpFirstPass.LoadAll(mrpItemDictionary);
        }

		private MRPProcessCache.MrpItemDictionary _loadAllMrpItemDictionary;

        /// <summary>
        /// Run all Load methods related to MRP first pass process
        /// </summary>
        /// <param name="mrpItemDictionary">mrp inventory cache</param>
        public virtual void LoadAll(MRPProcessCache.MrpItemDictionary mrpItemDictionary)
        {
			_loadAllMrpItemDictionary = mrpItemDictionary;
            var soSites = new List<INSite>();
            var productionSites = new List<INSite>();
            foreach (INSite inSite in PXSelect<INSite>.Select(MrpGraph))
            {
                var inSiteExtension = inSite.GetExtension<INSiteExt>();
                if (inSiteExtension == null)
                {
                    continue;
                }

                if (inSiteExtension.AMMRPSO.GetValueOrDefault())
                {
                    soSites.Add(inSite);
                }

                if (inSiteExtension.AMMRPProd.GetValueOrDefault())
                {
                    productionSites.Add(inSite);
                }
            }

#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("First Pass: Load All");
            var endProcessMessage = "Process Completed";

            try
            {
#endif
            LoadInventoryStockAdjustments(mrpItemDictionary);
#if DEBUG
                var lastElapsed = sw.Elapsed;
                sb.AppendLine(WriteDebugMessage(lastElapsed, "LoadInventoryStockAdjustments"));
#endif
            LoadInventory(mrpItemDictionary);
#if DEBUG
                var currElapsed = sw.Elapsed;
                sb.AppendLine(WriteDebugMessage(currElapsed - lastElapsed, "LoadInventory"));
                lastElapsed = currElapsed;
#endif
            LoadINItemPlan();
#if DEBUG
                currElapsed = sw.Elapsed;
                sb.AppendLine(WriteDebugMessage(currElapsed - lastElapsed, "LoadINItemPlan"));
                lastElapsed = currElapsed;
#endif
            LoadConsolidatedItemPlan();
#if DEBUG
                currElapsed = sw.Elapsed;
                sb.AppendLine(WriteDebugMessage(currElapsed - lastElapsed, "LoadConsolidatedItemPlan"));
                lastElapsed = currElapsed;
#endif
				LoadBlanketSalesOrders();
#if DEBUG
				currElapsed = sw.Elapsed;
				sb.AppendLine(WriteDebugMessage(currElapsed - lastElapsed, "LoadBlanketSalesOrders"));
				lastElapsed = currElapsed;
#endif
			if (PXAccess.FeatureInstalled<FeaturesSet.manufacturingMRP>())
			{
				LoadMasterProductionSchedule();
#if DEBUG
                currElapsed = sw.Elapsed;
                sb.AppendLine(WriteDebugMessage(currElapsed - lastElapsed, "LoadMasterProductionSchedule"));
                lastElapsed = currElapsed;
#endif
			}

            LoadForecasts();
#if DEBUG
                currElapsed = sw.Elapsed;
                sb.AppendLine(WriteDebugMessage(currElapsed - lastElapsed, "LoadForecasts"));
                lastElapsed = currElapsed;
#endif
            //Perform after all AMRPDetailFP records inserted...
            InsertMRPInventory(mrpItemDictionary);
#if DEBUG
                currElapsed = sw.Elapsed;
                sb.AppendLine(WriteDebugMessage(currElapsed - lastElapsed, "InsertMRPInventory"));
            }
            catch (Exception)
            {
                endProcessMessage = "PROCESS ERROR; END OF PROCESS";
                throw;
            }
            finally
            {
                sw.Stop();
                sb.AppendLine(PXTraceHelper.CreateTimespanMessage(sw.Elapsed, endProcessMessage));
                PXTraceHelper.WriteInformation(sb.ToString());
            }
#endif
        }
#if DEBUG
        private string WriteDebugMessage(TimeSpan elapsedTime, string processName)
        {
            var msg = PXTraceHelper.CreateTimespanMessage(elapsedTime, processName);
            AMDebug.TraceWriteLine(msg);
            return msg;
        }
#endif

        private void InsertMRPInventory(MRPProcessCache.MrpItemDictionary mrpItemDictionary)
        {
            foreach (AMRPDetailFP fpRow in MrpGraph.MrpDetailFirstPassRecs.Cache.Inserted)
            {
                MrpGraph.InsertMRPInventory(fpRow, mrpItemDictionary);
            }            
        }

        /// <summary>
        /// Insert the given Detail FP record into the cache
        /// </summary>
        /// <param name="row">row to insert</param>
        /// <returns>The inserted row</returns>
        public virtual AMRPDetailFP InsertDetailFP(AMRPDetailFP row)
        {
            if (row == null || row.Qty.GetValueOrDefault() <= 0)
            {
                return null;
            }

            //Starting in 6.1 a new hidden transit warehouse was added from the installer...
            //  We need to exclude records referencing this warehouse
            if (MrpGraph.InventorySetup != null && MrpGraph.InventorySetup.Current.TransitSiteID == row.SiteID)
            {
                return null;
            }

            try
            {
				if (_loadAllMrpItemDictionary != null && _loadAllMrpItemDictionary.HasTransferSites && row.SiteSequence == null)
				{
					var itemCache = _loadAllMrpItemDictionary.GetCurrentItemCache(row.InventoryID, row.SiteID);
					row.SiteSequence = itemCache?.SiteSequence ?? 0;
				}

                return MrpGraph.MrpDetailFirstPassRecs.Insert(row);
            }
            catch (Exception e)
            {
                PXTrace.WriteError(e);
                throw new MRPRegenException($"Error Inserting {BuildDataMessage(row)}", e);
            }
        }

        protected virtual string BuildDataMessage(AMRPDetailFP row)
        {
            if (row == null)
            {
                return string.Empty;
            }

            var sb = new System.Text.StringBuilder();

            sb.AppendFormat("MRP Detail FP '{0}' {1} Row: ", MRPPlanningType.GetDescription(row.Type), MRPSDFlag.GetDescription(row.SDFlag));

            InventoryItem inventoryItem = PXSelect<InventoryItem, 
                Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>
                    >.Select(MrpGraph, row.InventoryID);

            if (inventoryItem == null)
            {
                sb.AppendFormat("InventoryID [{0}] ", row.InventoryID);
            }
            else
            {
                sb.AppendFormat("Inventory ID {0} ", inventoryItem.InventoryCD.TrimIfNotNullEmpty());
            }

            if (PXAccess.FeatureInstalled<FeaturesSet.warehouse>())
            {
                INSite inSite = PXSelect<INSite,
                    Where<INSite.siteID, Equal<Required<INSite.siteID>>>
                >.Select(MrpGraph, row.SiteID);

                if (inSite == null)
                {
                    sb.AppendFormat("SiteID [{0}] ", row.SiteID);
                }
                else
                {
                    sb.AppendFormat("Warehouse {0} ", inSite.SiteCD.TrimIfNotNullEmpty());
                }
            }

            if (row.RequiredDate != null)
            {
                sb.AppendFormat("Required {0} ", row.RequiredDate.GetValueOrDefault().ToShortDateString());
            }

            return sb.ToString();
        }

        protected virtual string BuildDataMessage(AMRPDetailPlan row)
        {
            if (row == null)
            {
                return string.Empty;
            }

            var sb = new System.Text.StringBuilder();

            sb.AppendFormat("MRP Detail Plan '{0}' Row: ", row.PlanID);

            InventoryItem inventoryItem = PXSelect<InventoryItem,
                Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>
                    >.Select(MrpGraph, row.InventoryID);

            if (inventoryItem == null)
            {
                sb.AppendFormat("InventoryID [{0}] ", row.InventoryID);
            }
            else
            {
                sb.AppendFormat("Inventory ID {0} ", inventoryItem.InventoryCD.TrimIfNotNullEmpty());
            }

            if (PXAccess.FeatureInstalled<FeaturesSet.warehouse>())
            {
                INSite inSite = PXSelect<INSite,
                    Where<INSite.siteID, Equal<Required<INSite.siteID>>>
                >.Select(MrpGraph, row.SiteID);

                if (inSite == null)
                {
                    sb.AppendFormat("SiteID [{0}] ", row.SiteID);
                }
                else
                {
                    sb.AppendFormat("Warehouse {0} ", inSite.SiteCD.TrimIfNotNullEmpty());
                }
            }

            if (row.RefNoteID != null)
            {
                sb.AppendFormat("RefNoteID [{0}] ", row.RefNoteID);
            }

            if (row.PlanDate != null)
            {
                sb.AppendFormat("PlanDate {0} ", row.PlanDate);
            }

            return sb.ToString();
        }

        protected virtual int LowLevel(InventoryItem inventoryItem)
        {
            return inventoryItem?.GetExtension<InventoryItemExt>()?.AMLowLevel ?? 0;
        }

        /// <summary>
        /// Insert the given Detail Plan record into the cache
        /// </summary>
        /// <param name="row">row to insert</param>
        /// <returns>The inserted row</returns>
        protected virtual AMRPDetailPlan InsertDetailPlan(AMRPDetailPlan row)
        {
            if (row == null || row.Qty.GetValueOrDefault() <= 0)
            {
                return null;
            }

            //Starting in 6.1 a new hidden transit warehouse was added from the installer...
            //  We need to exclude records referencing this warehouse
            if (MrpGraph.InventorySetup != null && MrpGraph.InventorySetup.Current.TransitSiteID == row.SiteID)
            {
                return null;
            }

            try
            {
                return MrpGraph.MrpDetailPlanRecs.Insert(row);
            }
            catch (Exception e)
            {
                throw new MRPRegenException($"Error Inserting {BuildDataMessage(row)}", e);
            }
        }

        /// <summary>
        /// Load INItemPlan values into AMRPDetailPlan Table
        /// </summary>
        public virtual void LoadINItemPlan()
        {
			PXSelectBase<LoadINItemPlanProjection> cmd = new PXSelect<LoadINItemPlanProjection>(MrpGraph);

			List<object> parms = new List<object>();
			if (MrpGraph.TryGetPlanningHorizonDate(out var horizonDate))
			{
				cmd.WhereAnd<Where<LoadINItemPlanProjection.planDate,LessEqual<@P.AsDateTime>>>();
				parms.Add(horizonDate);
			}

			foreach (LoadINItemPlanProjection result in cmd.Select(parms.ToArray()))
            {
                if (result == null || result?.PlanID == null)
                {
                    continue;
                }

				var isSalesOrder = result.Operation != null;
				if (isSalesOrder && (result.IsSupply.GetValueOrDefault() || result.Reverse.GetValueOrDefault())
					&& result.Operation == SOOperation.Receipt && result.AMIncludeSupplyPlan == false)
				{
					continue;
				}

				/* Plan62 is confirmed shipped but not inventory updated. Qty no longer on sales order but not yet pulled from inventory */
				var isShipment = result?.EntityType == typeof(SOShipment).FullName && result.PlanType != INPlanConstants.Plan62;
                if (result?.ParentExcludeFromMRP == true || result?.ExcludeFromMRP == true || INPlanTypeHelper.MrpExcluded(result.PlanType) 
                    || result?.ProdItemCompleted == true || isShipment)
                {
                    continue;
                }

                // Verify location is MRP Location
                if (InventoryHelper.MultiWarehouseLocationFeatureEnabled && result.LocationID != null && result.AMMRPFlag == false)
                {
                    continue;
                }

				// Get the Site extension to check valid plan types to process
				var isProduction = INPlanTypeHelper.IsProduction(result.PlanType);
				if (!result.AMMRPSO.GetValueOrDefault() && INPlanTypeHelper.IsSalesOrder(result.PlanType)
					|| !result.AMMRPPO.GetValueOrDefault() && INPlanTypeHelper.IsPurchase(result.PlanType)
					|| !result.AMMRPShip.GetValueOrDefault() && INPlanTypeHelper.IsShipments(result.PlanType)
					|| !result.AMMRPProd.GetValueOrDefault() && (isProduction || INPlanTypeHelper.IsAssembly(result.PlanType))
					|| PXAccess.FeatureInstalled<FeaturesSet.distributionReqPlan>() && isProduction)
				{
					continue;
				}

				if (INPlanTypeHelper.IsAssembly(result.PlanType) && (!Features.KitFeatureEnabled() || result.PlanDate == null))
				{
					continue;
				}

				if (result.StkItem != true && result.PlanType != INPlanConstants.Plan60)
				{
					continue;
				}

				/* Any IN Related entires would be excluded here if warehouse not checked for MRP Inventory. Ex: IN Transfer-Intransit or IN Receipt */
				if ((result.AMMRPFlagSite ?? true) != true && (result.PlanType == INPlanConstants.Plan42
					|| INPlanTypeHelper.IsInventorySupply(result.PlanType) || INPlanTypeHelper.IsInventoryDemand(result.PlanType)))
				{
					continue;
				}

                var onHoldStatus = GetOnHoldStatus(new INItemPlan { Hold = result.Hold, PlanType = result.PlanType});
                var processed = onHoldStatus == OnHoldStatus.OnHoldExclude;

				string refNbr = null;
				if(result.ProdItemOrderType != null)
				{
					refNbr = string.Join(", ", result.ProdItemOrderType, result.ProdItemProdOrdID);
				}else if(result.ProdMatlOrderType != null)
				{
					refNbr = string.Join(", ", result.ProdMatlOrderType, result.ProdMatlProdOrdID, result.ProdMatlOperationCD);
				}else if(result.SOOrderType != null)
				{
					refNbr = string.Join(", ", result.SOOrderType, result.SOOrderNbr);
				}else if(result.POOrderType  != null)
				{
					refNbr = string.Join(", ", result.POOrderType, result.POOrderNbr);
				}

                var row = new AMRPDetailPlan
                {
                    PlanID = result.PlanID,
                    InventoryID = result.InventoryID,
                    SubItemID = result.SubItemID,
                    SiteID = result.SiteID,
                    PlanDate = result.PlanDate,
                    PlanType = result.PlanType,
                    Qty = result.PlanQty,
                    OnHoldStatus = onHoldStatus,
                    Processed = processed,
                    RefType = GetMRPPlanningType(result.PlanType),
                    SDFlag = result.IsSupply == true ? MRPSDFlag.Supply : MRPSDFlag.Demand,
                    SupplyPlanID = result.SupplyPlanID,
                    DemandPlanID = result.DemandPlanID,
                    BAccountID = result.BAccountID,
                    RefNoteID = result.RefNoteID,
                    LowLevel = result.AMLowLevel ?? 0,
                    ParentInventoryID = result?.ProdItemInventoryID ?? result.InventoryID,
                    ParentSubItemID = result?.ProdItemSubItemID ?? result.SubItemID,
                    ItemClassID = result.ItemClassID,
					BranchID = result.BranchID,
					RefNbr = refNbr
                };

                // When the qty is reverse it needs to have the SD flipped.
                //  For example a return sales order shows as a positive qty value with reverse true but marked as demand. The incoming return is actually supply.
                if (result.Reverse.GetValueOrDefault())
                {
                    row.SDFlag = row.SDFlag == MRPSDFlag.Supply ? MRPSDFlag.Demand : MRPSDFlag.Supply;
                }

                if (result.INDocType == INTranType.Transfer)
                {
                    row.RefType = MRPPlanningType.TransferDemand;
                    row.BAccountID = null;
					row.TransferSiteID = result.DestinationSiteID;

				}

                //In-transit transfer
                if(result.PlanType == INPlanConstants.Plan42)
                {
                    row.RefNoteID = result.OrigNoteID;
                }

                var insertedRow = InsertDetailPlan(row);

				if (insertedRow == null
                || insertedRow.RefType != MRPPlanningType.TransferDemand)
				{
					continue;
				}

                AddTransferSupplyFromTransferDemand(insertedRow, result.DestinationSiteID, result.RequestDate);
            }
        }

        protected virtual int? GetOnHoldStatus(INItemPlan itemPlan)
        {
            if (itemPlan == null)
            {
                throw new ArgumentNullException(nameof(itemPlan));
            }

            if (!itemPlan.Hold.GetValueOrDefault())
            {
                return OnHoldStatus.NotOnHold;
            }

            if(INPlanTypeHelper.IsProduction(itemPlan.PlanType))
            {
                return IncludeOnHoldProductionOrder ? OnHoldStatus.OnHoldInclude : OnHoldStatus.OnHoldExclude;
            }

            if (INPlanTypeHelper.IsSalesOrder(itemPlan.PlanType))
            {
                return IncludeOnHoldSalesOrder ? OnHoldStatus.OnHoldInclude : OnHoldStatus.OnHoldExclude;
            }

            if (INPlanTypeHelper.IsPurchase(itemPlan.PlanType) && !INPlanTypeHelper.IsPurchaseReceipt(itemPlan.PlanType))
            {
                return IncludeOnHoldPurchaseOrder ? OnHoldStatus.OnHoldInclude : OnHoldStatus.OnHoldExclude;
            }

			if (INPlanTypeHelper.IsAssembly(itemPlan.PlanType))
			{
				return IncludeOnHoldKitAssembly ? OnHoldStatus.OnHoldInclude : OnHoldStatus.OnHoldExclude;
			}

			return OnHoldStatus.NotOnHold;
        }

        protected static string GetMRPPlanningType(string itemPlanType)
        {
            if (string.IsNullOrWhiteSpace(itemPlanType))
            {
                return MRPPlanningType.Unknown;
            }

            if (INPlanTypeHelper.IsProductionSupply(itemPlanType))
            {
                return MRPPlanningType.ProductionOrder;
            }

            if (INPlanTypeHelper.IsProductionDemand(itemPlanType))
            {
                return MRPPlanningType.ProductionMaterial;
            }

            if (INPlanTypeHelper.IsSalesOrder(itemPlanType))
            {
                return MRPPlanningType.SalesOrder;
            }

            if (INPlanTypeHelper.IsPurchase(itemPlanType))
            {
                return MRPPlanningType.PurchaseOrder;
            }

            if (INPlanTypeHelper.IsShipments(itemPlanType))
            {
                return MRPPlanningType.Shipment;
            }

            if (INPlanTypeHelper.IsTransferSupply(itemPlanType))
            {
                return MRPPlanningType.TransferSupply;
            }

            if (INPlanTypeHelper.IsInventoryDemand(itemPlanType))
            {
                return MRPPlanningType.InventoryDemand;
            }

            if (INPlanTypeHelper.IsInventorySupply(itemPlanType))
            {
                return MRPPlanningType.InventorySupply;
            }

            if (INPlanTypeHelper.IsFieldService(itemPlanType))
            {
                return MRPPlanningType.FieldService;
            }

            if (INPlanTypeHelper.IsAssemblyDemand(itemPlanType))
            {
                return MRPPlanningType.AssemblyDemand;
            }

            if (INPlanTypeHelper.IsAssemblySupply(itemPlanType))
            {
                return MRPPlanningType.AssemblySupply;
            }

            return MRPPlanningType.Unknown;
        }

        /// <summary>
        /// Load INItemPlan values into AMRPDetailPlan Table
        /// </summary>
        public virtual void LoadConsolidatedItemPlan()
        {
            var currentDetailPlan = new AMRPDetailPlan();
            foreach (AMRPDetailPlan detailPlan in MrpGraph.MrpDetailPlanRecs.Select<AMRPDetailPlan>().ToList().OrderBy(p=>p.InventoryID)
				.ThenBy(p=>p.SiteID).ThenBy(p=>p.SubItemID).ThenBy(p=>p.PlanDate).ThenBy(p=>p.SupplyPlanID)
				.ThenBy(p=>p.DemandPlanID).ThenBy(p=>p.RefNoteID).ThenBy(p=>p.SDFlag))
            {
                if (!(currentDetailPlan.InventoryID == detailPlan.InventoryID &&
                      currentDetailPlan.SiteID == detailPlan.SiteID &&
                      currentDetailPlan.SubItemID == detailPlan.SubItemID &&
                      currentDetailPlan.PlanDate == detailPlan.PlanDate &&
                      currentDetailPlan.SupplyPlanID == detailPlan.SupplyPlanID &&
                      currentDetailPlan.DemandPlanID == detailPlan.DemandPlanID &&
                      currentDetailPlan.RefNoteID == detailPlan.RefNoteID &&
                      currentDetailPlan.SDFlag == detailPlan.SDFlag))
                {
                    InsertConsolidatedAMRPDetailPlan(currentDetailPlan);
                    currentDetailPlan = detailPlan;
                    continue;
                }

                currentDetailPlan.Qty += detailPlan.Qty;
            }
            InsertConsolidatedAMRPDetailPlan(currentDetailPlan);
        }

        protected virtual void InsertConsolidatedAMRPDetailPlan(AMRPDetailPlan detailPlan)
        {
            if (detailPlan?.PlanID == null)
            {
                return;
            }

            InsertDetailFP(new AMRPDetailFP
            {
                PlanID = detailPlan.PlanID,
                PlanType = detailPlan.PlanType,
                InventoryID = detailPlan.InventoryID,
                SubItemID = detailPlan.SubItemID,
                SiteID = detailPlan.SiteID,
                LowLevel = detailPlan.LowLevel,
                PlanDate = detailPlan.PlanDate,
                OriginalQty = detailPlan.Qty,
                Qty = detailPlan.Qty,
                SuppliedQty = detailPlan.SDFlag == MRPSDFlag.Demand || detailPlan.Processed.GetValueOrDefault()
                    ? 0m
                    : detailPlan.Qty.GetValueOrDefault(),
                OnHoldStatus = detailPlan.OnHoldStatus,
                Processed = detailPlan.Processed,
                SupplyPlanID = detailPlan.SupplyPlanID,
                DemandPlanID = detailPlan.DemandPlanID,
                BAccountID = detailPlan.BAccountID,
                RequiredDate = detailPlan.PlanDate,
                RefNbr = detailPlan.RefNbr ?? RefNbrFieldAttribute.GetKeyString(MrpGraph, detailPlan.RefNoteID),
                RefNoteID = detailPlan.RefNoteID,
                SDFlag = detailPlan.SDFlag,
                Type = detailPlan.RefType,
                RefType = detailPlan.RefType,
                ParentInventoryID = detailPlan.ParentInventoryID,
                ParentSubItemID = detailPlan.ParentSubItemID,
                ProductInventoryID = detailPlan.ParentInventoryID,
                ProductSubItemID = detailPlan.ParentSubItemID,
                ItemClassID = detailPlan.ItemClassID,
				BranchID = detailPlan.BranchID
            });
        }

        /// <summary>
        /// Load initial inventory values into MRP (Excludes items not marked for MRP or status is inactive/marked for delete)
        /// </summary>
        /// <param name="mrpItemDictionary"></param>
        public virtual void LoadInventory(MRPProcessCache.MrpItemDictionary mrpItemDictionary)
        {
            PXTrace.WriteInformation("MRP Process: Load inventory");
            foreach (PXResult<INItemSite, InventoryItemMRP, INSite> result in PXSelectJoin<INItemSite,
                LeftJoin<InventoryItemMRP, On<INItemSite.inventoryID, Equal<InventoryItemMRP.inventoryID>>,
                    LeftJoin<INSite, On<INItemSite.siteID, Equal<INSite.siteID>>>>,
                        Where<InventoryItemMRP.stkItem, Equal<True>,
                        And<InventoryItemMRP.itemStatus, NotEqual<INItemStatus.inactive>,
                        And<InventoryItemMRP.itemStatus, NotEqual<INItemStatus.toDelete>,
                        And2<Where<INItemSite.planningMethod, Equal<INPlanningMethod.dRP>,
                                    Or<INItemSite.planningMethod, Equal<INPlanningMethod.mRP>>>,
                            And<Where<INSiteExt.aMMRPFlag, Equal<True>, 
                                    Or<INSiteExt.aMMRPFlag, IsNull>>
                                    >>>>>,
						OrderBy<Asc<INItemSite.siteID, Asc<INItemSite.inventoryID>>>>.Select(MrpGraph))
            {
                var inItemSite = (INItemSite)result;
                var inventoryItem = (InventoryItemMRP)result;
				var inSite = (INSite)result;

                if (inItemSite?.SiteID == null || inventoryItem?.InventoryID == null)
                {
                    continue;
                }

                var lowLevel = inventoryItem.AMLowLevel ?? 0;
                
                //  ************************************************************
                //  Generate Safety Stock
                //  ************************************************************

                //Generate sub item safety stock entries
                if (MrpGraph.SubItemEnabled && (InventoryHelper.FullReplenishmentsEnabled || InventoryHelper.BasicReplenishmentsEnabled))
                {
                    var subitems = CreateSubitemInventory(inItemSite, lowLevel, inSite);
                    if (subitems != null && subitems.Count > 0)
                    {
                        foreach (var row in subitems)
                        {
                            InsertDetailFP(row);
                        }

                        // next item warehouse
                        continue;
                    }
                }

                //Generate non sub item safety stock entries
                var itemCache = mrpItemDictionary.GetCurrentItemCache(inItemSite.InventoryID, inItemSite.SiteID);
                var safetyStockRow = new AMRPDetailFP
                {
                    SiteID = inItemSite.SiteID,
                    InventoryID = inItemSite.InventoryID,
                    SubItemID = inventoryItem.DefaultSubItemID,
                    ParentInventoryID = inItemSite.InventoryID,
                    ParentSubItemID = inventoryItem.DefaultSubItemID,
                    ProductInventoryID = inItemSite.InventoryID,
                    ProductSubItemID = inventoryItem.DefaultSubItemID,
                    PlanDate = MrpGraph.ProcessDateTime.Date,
                    Type = PlanTypeForSafetyStock,
                    SDFlag = MRPSDFlag.Demand,
                    LowLevel = lowLevel,
                    ItemClassID = inventoryItem.ItemClassID,
					BranchID = inSite.BranchID
                };

                if (itemCache == null)
                {
                    safetyStockRow.Qty = MrpGraph.Setup.Current.StockingMethod == AMRPSetup.MRPStockingMethod.SafetyStock
                        ? inItemSite.SafetyStock.GetValueOrDefault()
                        : inItemSite.MinQty.GetValueOrDefault();
                    safetyStockRow.SubItemID = inventoryItem.DefaultSubItemID;
                }
                else
                {
					safetyStockRow.SiteSequence = itemCache.SiteSequence;
                    safetyStockRow.Qty = itemCache.UseSafetyStock ? itemCache.SafetyStock : itemCache.ReorderPoint;
                    safetyStockRow.SubItemID = itemCache.DefaultSubItemID;
                }

                if (safetyStockRow.Qty.GetValueOrDefault() > 0)
                {
                    safetyStockRow.SuppliedQty = safetyStockRow.Qty;
                    safetyStockRow.OriginalQty = safetyStockRow.Qty;
                    InsertDetailFP(safetyStockRow);
                }
            }
        }

        /// <summary>
        /// Generate the related AMRPDetailFP safety stock records related to the given item warehouse record
        /// </summary>
        /// <param name="inItemSite">item warehouse record</param>
        /// <returns>List of subitem safety stock records</returns>
		[Obsolete]
        protected virtual List<AMRPDetailFP> CreateSubitemInventory(INItemSite inItemSite, int lowLevel)
        {
            var list = new List<AMRPDetailFP>();
            foreach (INItemSiteReplenishment replenishment in PXSelect<INItemSiteReplenishment, 
                Where<INItemSiteReplenishment.inventoryID, Equal<Required<INItemSiteReplenishment.inventoryID>>,
                And<INItemSiteReplenishment.siteID, Equal<Required<INItemSiteReplenishment.siteID>>>>
                >.Select(MrpGraph, inItemSite.InventoryID, inItemSite.SiteID))
            {
                if (replenishment == null 
                    || replenishment.InventoryID.GetValueOrDefault() == 0
                    || !(replenishment.ItemStatus == InventoryItemStatus.Active
                                   || replenishment.ItemStatus == InventoryItemStatus.NoSales))
                {
                    continue;
                }

                var ssQty = MrpGraph.Setup.Current.StockingMethod == AMRPSetup.MRPStockingMethod.SafetyStock
                    ? replenishment.SafetyStock.GetValueOrDefault()
                    : replenishment.MinQty.GetValueOrDefault();

                list.Add(new AMRPDetailFP
                {
                    SiteID = inItemSite.SiteID,
                    InventoryID = inItemSite.InventoryID,
                    SubItemID = replenishment.SubItemID,
                    ParentInventoryID = inItemSite.InventoryID,
                    ParentSubItemID = replenishment.SubItemID,
                    ProductInventoryID = inItemSite.InventoryID,
                    ProductSubItemID = replenishment.SubItemID,
                    Qty = ssQty,
                    PlanDate = MrpGraph.ProcessDateTime.Date,
                    Type = PlanTypeForSafetyStock,
                    SDFlag = MRPSDFlag.Demand,
                    SuppliedQty = ssQty,
                    LowLevel = lowLevel,
                    OriginalQty = ssQty
                });
            }
            return list;
        }

        /// <summary>
		/// Generate the related AMRPDetailFP safety stock records related to the given item warehouse record
		/// </summary>
		/// <param name="inItemSite">Item warehouse record</param>
		/// <param name="lowLevel">Lowest bom level value</param>
		/// <param name="inSite">Warehouse record</param>
		/// <returns>List of subitem safety stock records</returns>
		protected virtual List<AMRPDetailFP> CreateSubitemInventory(INItemSite inItemSite, int lowLevel, INSite inSite)
		{
			var list = new List<AMRPDetailFP>();
			foreach (INItemSiteReplenishment replenishment in PXSelect<INItemSiteReplenishment,
				Where<INItemSiteReplenishment.inventoryID, Equal<Required<INItemSiteReplenishment.inventoryID>>,
				And<INItemSiteReplenishment.siteID, Equal<Required<INItemSiteReplenishment.siteID>>>>
				>.Select(MrpGraph, inItemSite.InventoryID, inItemSite.SiteID))
			{
				if (replenishment == null
					|| replenishment.InventoryID.GetValueOrDefault() == 0
					|| !(replenishment.ItemStatus == InventoryItemStatus.Active
								   || replenishment.ItemStatus == InventoryItemStatus.NoSales))
				{
					continue;
				}

				var ssQty = MrpGraph.Setup.Current.StockingMethod == AMRPSetup.MRPStockingMethod.SafetyStock
					? replenishment.SafetyStock.GetValueOrDefault()
					: replenishment.MinQty.GetValueOrDefault();

				list.Add(new AMRPDetailFP
				{
					SiteID = inItemSite.SiteID,
					InventoryID = inItemSite.InventoryID,
					SubItemID = replenishment.SubItemID,
					ParentInventoryID = inItemSite.InventoryID,
					ParentSubItemID = replenishment.SubItemID,
					ProductInventoryID = inItemSite.InventoryID,
					ProductSubItemID = replenishment.SubItemID,
					Qty = ssQty,
					PlanDate = MrpGraph.ProcessDateTime.Date,
					Type = PlanTypeForSafetyStock,
					SDFlag = MRPSDFlag.Demand,
					SuppliedQty = ssQty,
					LowLevel = lowLevel,
					OriginalQty = ssQty,
					BranchID = inSite.BranchID
				});
			}
			return list;
		}

		/// <summary>
        /// Load inventory stock adjustments
        /// </summary>
        public virtual void LoadInventoryStockAdjustments(MRPProcessCache.MrpItemDictionary mrpItemDictionary)
        {
            PXTrace.WriteInformation("MRP Process: Load inventory stock adjustments");
            foreach (PXResult<INLocationStatusByCostCenter, INLocation, InventoryItem, INSite> result in PXSelectJoinGroupBy<INLocationStatusByCostCenter,
                    LeftJoin<INLocation, On<INLocationStatusByCostCenter.siteID, Equal<INLocation.siteID>,
                        And<INLocationStatusByCostCenter.locationID, Equal<INLocation.locationID>>>,
                    LeftJoin<InventoryItem, On<INLocationStatusByCostCenter.inventoryID, Equal<InventoryItem.inventoryID>>,
                    LeftJoin<INSite, On<INLocationStatusByCostCenter.siteID, Equal<INSite.siteID>>>>>,
                    Where2<Where<INLocationExt.aMMRPFlag, IsNull,
                            Or<INLocationExt.aMMRPFlag, Equal<True>>>,
                        And<InventoryItem.stkItem, Equal<True>,
                        And<InventoryItem.itemStatus, NotEqual<INItemStatus.inactive>,
                        And<InventoryItem.itemStatus, NotEqual<INItemStatus.toDelete>,
                        And2<Where<InventoryItem.planningMethod, Equal<INPlanningMethod.dRP>,
                            Or<InventoryItem.planningMethod, Equal<INPlanningMethod.mRP>>>,
                        And<Where<INSiteExt.aMMRPFlag, Equal<True>,
                            Or<INSiteExt.aMMRPFlag, IsNull>>>>>>>>,
                    Aggregate<
                    GroupBy<INLocationStatusByCostCenter.siteID, GroupBy<INLocationStatusByCostCenter.inventoryID, GroupBy<INLocationStatusByCostCenter.subItemID,
                    Sum<INLocationStatusByCostCenter.qtyOnHand>>>>>,
					OrderBy<Asc<INLocationStatusByCostCenter.siteID, Asc<INLocationStatusByCostCenter.inventoryID>>>>.Select(MrpGraph))
            {
                var inLocationStatus = (INLocationStatusByCostCenter)result;
                var inventoryItem = (InventoryItem) result;
				var inSite = (INSite)result;

                if (inventoryItem?.InventoryID == null || inLocationStatus?.InventoryID == null || inLocationStatus?.SiteID == MrpGraph.InventorySetup.Current.TransitSiteID)
                {
                    continue;
                }

                var quantity = inLocationStatus.QtyOnHand.GetValueOrDefault();
                if (quantity != 0)
                {
					var siteSequence = mrpItemDictionary.HasTransferSites
						? mrpItemDictionary.GetCurrentItemCache(
							inventoryItem.InventoryID,
							inLocationStatus.SiteID)?.SiteSequence
						: 0;

					var sdFlag = quantity < 0 ? MRPSDFlag.Demand : MRPSDFlag.Supply;
					var stockAdjustmentRow = new AMRPDetailFP
					{
						SiteID = inLocationStatus.SiteID,
						InventoryID = inLocationStatus.InventoryID,
						SubItemID = inLocationStatus.SubItemID,
						ProductInventoryID = inLocationStatus.InventoryID,
						ProductSubItemID = inLocationStatus.SubItemID,
						ParentInventoryID = inLocationStatus.InventoryID,
						ParentSubItemID = inLocationStatus.SubItemID,
						Qty = Math.Abs(quantity),
						SuppliedQty = Math.Abs(quantity),
						SDFlag = sdFlag,
						Type = MRPPlanningType.StockAdjustment,
						LowLevel = LowLevel(inventoryItem),
						ItemClassID = inventoryItem.ItemClassID,
						BranchID = inSite.BranchID,
						SiteSequence = siteSequence ?? 0
                    };
                    stockAdjustmentRow.PlanDate = stockAdjustmentRow.SDFlag == MRPSDFlag.Demand ? MrpGraph.ProcessDateTime.Date : Common.Dates.BeginOfTimeDate;
                    stockAdjustmentRow.OriginalQty = stockAdjustmentRow.Qty;

                    var mrpInventory = MrpGraph.InsertMRPInventory(
                        InsertDetailFP(stockAdjustmentRow), 
                        mrpItemDictionary);
                    if (mrpInventory != null)
                    {
                        mrpInventory.QtyOnHand = quantity;
                        MrpGraph.MrpInventory.Update(mrpInventory);
                    }
                }
            }
        }

		/// <summary>
        /// Inserts a transfer supply record related to the transfer demand record
        /// </summary>
        protected virtual AMRPDetailPlan AddTransferSupplyFromTransferDemand(AMRPDetailPlan amrpDetailPlan, int? toSiteId, DateTime? planDate)
        {
            if (amrpDetailPlan == null
                || amrpDetailPlan.RefType != MRPPlanningType.TransferDemand
                || toSiteId.GetValueOrDefault() == 0
                || amrpDetailPlan.Qty.GetValueOrDefault() <= 0)
            {
                return null;
            }

            var row = PXCache<AMRPDetailPlan>.CreateCopy(amrpDetailPlan);
            row.PlanID = row.PlanID * -1;
            row.SDFlag = MRPSDFlag.Supply;
            row.RefType = MRPPlanningType.TransferSupply;
            row.SiteID = toSiteId;
			row.TransferSiteID = row.SiteID;
			row.PlanDate = planDate ?? row.PlanDate;
			return InsertDetailPlan(row);
        }

        /// <summary>
        /// Load MPS values into MRP
        /// </summary>
        public virtual void LoadMasterProductionSchedule()
        {
            PXTrace.WriteInformation("MRP Process: Load MPS");
            var mpsTimeFenceDate = MrpGraph.GetMPSTimeFenceDate();
			var usingHorizon = MrpGraph.TryGetPlanningHorizonDate(out var horizonDate);
            var lastMpsPlanDate = mpsTimeFenceDate;
            int? lastItem = -1;
            int? lastSite = -1;

            foreach (PXResult<AMMPS, AMMPSType, InventoryItem, INSite, AMBomItemActiveAggregate> result in PXSelectJoin<AMMPS,
                InnerJoin<AMMPSType, On<AMMPS.mPSTypeID, Equal<AMMPSType.mPSTypeID>>,
                InnerJoin<InventoryItem, On<AMMPS.inventoryID, Equal<InventoryItem.inventoryID>>,
                InnerJoin<INSite, On<AMMPS.siteID, Equal<INSite.siteID>>,
                LeftJoin<AMBomItemActiveAggregate, On<AMMPS.bOMID, Equal<AMBomItemActiveAggregate.bOMID>>>>>>,
                Where<AMMPS.activeFlg, Equal<True>,
                    And2<Where<InventoryItem.planningMethod, Equal<INPlanningMethod.dRP>,
                        Or<InventoryItem.planningMethod, Equal<INPlanningMethod.mRP>>>,
                        And<Where<INSiteExt.aMMRPMPS, IsNull,
                            Or<INSiteExt.aMMRPMPS, Equal<True>>>>>>,
                OrderBy<Asc<AMMPS.inventoryID, Asc<AMMPS.siteID, Asc<AMMPS.planDate>>>>>.Select(MrpGraph))
            {
                var mps = (AMMPS) result;
                var mpsType = (AMMPSType)result;
                var inventoryItem = (InventoryItem) result;
                var bomRevisionId = ((AMBomItemActiveAggregate) result)?.RevisionID;
				var inSite = (INSite)result;

                if (mps?.BOMID == null 
                    || mpsType == null 
                    || mps.PlanDate == null 
                    || mps.InventoryID == null)
                {
                    continue;
                }

                if (DateTime.Compare(mps.PlanDate ?? Common.Dates.BeginOfTimeDate, mpsTimeFenceDate) < 0)
                {
                    mps.ActiveFlg = false;
                    MrpGraph.MPSRecs.Update(mps);
                    continue;
                }

				if (usingHorizon && mps.PlanDate != null && DateTime.Compare(mps.PlanDate.Value, horizonDate) > 0)
				{
					continue;
				}

				if (string.IsNullOrWhiteSpace(bomRevisionId))
                {
                    PXTrace.WriteWarning($"MPS {mps.MPSID} with {PXUIFieldAttribute.GetDisplayName<AMMPS.bOMID>(MrpGraph.Caches<AMMPS>())} {mps.BOMID} does not have an active revision. Skipped planning for this MPS.");
                    continue;
                }

                if (mps.InventoryID != lastItem || mps.SiteID != lastSite)
                {
                    // Sub a day to account for MasterProductionScheduleQtyConsumed looking for start date greater than.... would loose the matching on the time fence date without
                    mpsTimeFenceDate.TryAddDays(-1, out lastMpsPlanDate);
                }

                var qty = mps.BaseQty.GetValueOrDefault() < 0 ? 0 : mps.BaseQty.GetValueOrDefault();
                var row = new AMRPDetailFP
                {
                    Type = MRPPlanningType.MPS,
                    RefType = MRPPlanningType.MPS,
                    SDFlag = MRPSDFlag.Supply,
                    InventoryID = mps.InventoryID,
                    SubItemID = mps.SubItemID,
                    SiteID = mps.SiteID,
                    BOMID = mps.BOMID,
                    RefNbr = $"{mps.MPSTypeID}, {mps.MPSID}",
                    RefNoteID = mps.NoteID,
                    ParentInventoryID = mps.InventoryID,
                    ParentSubItemID = mps.SubItemID,
                    ProductInventoryID = mps.InventoryID,
                    ProductSubItemID = mps.SubItemID,
                    Qty = qty,
                    OriginalQty = qty,
                    SuppliedQty = qty,
                    PlanDate = mps.PlanDate,
                    BOMRevisionID = bomRevisionId,
                    OrderQtyConsumed = mpsType.Dependent.GetValueOrDefault()
                        ? MasterProductionScheduleQtyConsumed(mps, lastMpsPlanDate)
                        : 0,
                    LowLevel = LowLevel(inventoryItem),
                    ItemClassID = inventoryItem.ItemClassID,	
					BranchID = inSite.BranchID
                };

                row.Qty -= row.OrderQtyConsumed.GetValueOrDefault();
                row.SuppliedQty = row.Qty.GetValueOrDefault();

                if (row.Qty.GetValueOrDefault() > 0)
                {
                    row.OriginalQty = row.Qty.GetValueOrDefault();

                    if (InventoryHelper.IsInvalidItemStatus(inventoryItem))
                    {
                        row.OnHoldStatus = OnHoldStatus.InvalidItemStatus;
                        row.Processed = true;
                        row.SuppliedQty = 0;
                    }

                    InsertDetailFP(row);
                }
                if (mpsType.Dependent.GetValueOrDefault())
                {
                    lastMpsPlanDate = row.PlanDate ?? lastMpsPlanDate;
                }
                lastItem = row.InventoryID;
                lastSite = row.SiteID;
            }
        }

		/// <summary>
		/// Get the matching production quantity against the given MPS and date range
		/// </summary>
		/// <param name="mps">MPS to get the consumed qty against</param>
		/// <param name="startDate">date to start considering production orders</param>
		/// <returns>total base quantity of productions orders found</returns>
		public virtual decimal MasterProductionScheduleQtyConsumed(AMMPS mps, DateTime startDate)
        {
            if (mps?.InventoryID == null 
                || mps.SiteID == null 
                || Common.Dates.IsDefaultDate(startDate) 
                || Common.Dates.IsDefaultDate(mps.PlanDate ?? Common.Dates.BeginOfTimeDate))
            {
                return 0;
            }

			return MrpGraph.GetDependentMPSQuantity(mps, startDate);
        }

        /// <summary>
        /// Load Forecast values into MRP
        /// </summary>
        public virtual void LoadForecasts()
        {
			PXTrace.WriteInformation("MRP Process: Load Forecasts");
			var forecastHorizonStartDate = MrpGraph.GetForecastHorizonDate();
			var usingHorizon = MrpGraph.TryGetPlanningHorizonDate(out var forecastHorizonEndDate);

            foreach (PXResult<AMForecast, InventoryItemMRP, INSite> result in PXSelectJoin<
            	AMForecast,
            	InnerJoin<InventoryItemMRP, 
            		On<AMForecast.inventoryID, Equal<InventoryItemMRP.inventoryID>>,
            	InnerJoin<INSite, 
            		On<AMForecast.siteID, Equal<INSite.siteID>>,
            	LeftJoin<INKitSpecHdrLastRevisionDate,
            		On<INKitSpecHdrLastRevisionDate.kitInventoryID.
            		IsEqual<InventoryItemMRP.inventoryID>>>>>,
            	Where<AMForecast.activeFlg, Equal<True>,
            		And2<
            			Where2<
            				Where<InventoryItemMRP.stkItem, Equal<False>, 
            					And<INKitSpecHdrLastRevisionDate.kitInventoryID.IsNotNull>> ,
            				Or<Where<InventoryItemMRP.stkItem, Equal<True>,
            					And<Where<InventoryItemMRP.planningMethod, Equal<INPlanningMethod.dRP>,
            						Or<InventoryItemMRP.planningMethod, Equal<INPlanningMethod.mRP>>>>>>>,
            			And<Where<INSiteExt.aMMRPFcst, IsNull,
            				Or<INSiteExt.aMMRPFcst, Equal<True>>>>>>>
            	.Select(MrpGraph))
            {
                var forecast = (AMForecast) result;
                var inventoryItem = (InventoryItemMRP) result;

                if (forecast == null 
                    || forecast.Qty.GetValueOrDefault() <= 0 
                    || forecast.InventoryID == null 
                    || forecast.BeginDate == null 
                    || forecast.EndDate == null
                    || inventoryItem?.InventoryID == null)
                {
                    continue;
                }

                //if the forecast has past the planning horizon - make the forecast inactive
                if (DateTime.Compare(forecast.EndDate ?? Common.Dates.BeginOfTimeDate, forecastHorizonStartDate) < 0)
                {
                    forecast.ActiveFlg = false;
                    MrpGraph.ForecastRecs.Update(forecast);
                    continue;
                }

				var intervals = Forecast.GetForecastIntervals(forecast);
				foreach (Tuple<DateTime, DateTime> interval in intervals)
				{
					var beginDate = interval.Item1;
					var endDate = interval.Item2;

					if (DateTime.Compare(endDate, forecastHorizonStartDate) <= 0)
					{
						continue;
					}

					if (usingHorizon)
					{
						if (forecast.Interval == ForecastInterval.OneTime &&
							(DateTime.Compare(beginDate, forecastHorizonEndDate) > 0 && DateTime.Compare(endDate, forecastHorizonEndDate) > 0))
						{
							continue;
						}
						else if (forecast.Interval != ForecastInterval.OneTime &&
							(DateTime.Compare(beginDate, forecastHorizonEndDate) > 0 || DateTime.Compare(endDate, forecastHorizonEndDate) > 0))
						{
							continue;
						}
					}

					var qty = forecast.BaseQty.GetValueOrDefault() < 0 ? 0 : forecast.BaseQty.GetValueOrDefault();
					var row = new AMRPDetailFP
					{
						Type = MRPPlanningType.ForecastDemand,
						RefType = MRPPlanningType.ForecastDemand,
						SDFlag = MRPSDFlag.Demand,
						InventoryID = forecast.InventoryID,
						SubItemID = forecast.SubItemID,
						SiteID = forecast.SiteID,
						RefNbr = forecast.ForecastID,
						RefNoteID = forecast.NoteID,
						ParentInventoryID = forecast.InventoryID,
						ParentSubItemID = forecast.SubItemID,
						ProductInventoryID = forecast.InventoryID,
						ProductSubItemID = forecast.SubItemID,
						Qty = qty,
						OriginalQty = qty,
						PlanDate = forecast.BeginDate,
						OrderQtyConsumed = 0,
						LowLevel = inventoryItem.AMLowLevel,
						ItemClassID = inventoryItem.ItemClassID,
						BranchID = forecast.BranchID
					};

					if (forecast.Dependent.GetValueOrDefault())
					{
						row.OrderQtyConsumed = MrpGraph.GetDependentForecastQuantity(forecast, beginDate, endDate);
					}

					row.Qty -= row.OrderQtyConsumed;
					row.PlanDate = beginDate;
					row.OriginalQty = row.Qty;

					if (InventoryHelper.IsInvalidItemStatus(inventoryItem.ItemStatus))
					{
						row.OnHoldStatus = OnHoldStatus.InvalidItemStatus;
						row.Processed = true;
					}

					InsertDetailFP(row);
				}
			}
        }

		/// <summary>
		/// Load Blanket SalesOrders in to MRP
		/// </summary>
		public virtual void LoadBlanketSalesOrders()
		{
			if (!MrpGraph.HasActiveBlanketSalesOrder())
			{
				return;
			}

			foreach (PXResult<SimpleBlanketLineSplit, InventoryItemMRP, INSite> result in SelectFrom<SimpleBlanketLineSplit>
				.InnerJoin<InventoryItemMRP>.On<InventoryItemMRP.inventoryID.IsEqual<SimpleBlanketLineSplit.inventoryID>>
				.InnerJoin<INSite>.On<INSite.siteID.IsEqual<SimpleBlanketLineSplit.siteID>>
				.InnerJoin<SOOrderType>.On<SOOrderType.orderType.IsEqual<SimpleBlanketLineSplit.orderType>>
				.Where<SOOrderType.behavior.IsEqual<SOBehavior.bL>.And<SOOrderType.active.IsEqual<True>>
				.And<InventoryItemMRP.planningMethod.IsEqual<INPlanningMethod.dRP>.Or<InventoryItemMRP.planningMethod.IsEqual<INPlanningMethod.mRP>>>
				.And<INSiteExt.aMMRPSO.IsNull.Or<INSiteExt.aMMRPSO.IsEqual<True>>>
				.And<SimpleBlanketLineSplit.completed.IsEqual<False>
				.And<SimpleBlanketLineSplit.cancelled.IsEqual<False>>>>
				.View.Select(MrpGraph))
			{
				var simpleBlanketLineSplit = (SimpleBlanketLineSplit)result;
				var inventoryItem = (InventoryItemMRP)result;

				if (simpleBlanketLineSplit.OpenOrderQty == 0 || (simpleBlanketLineSplit.Hold == true && MrpGraph?.Setup?.Current?.IncludeOnHoldSalesOrder != true))
				{
					continue;
				}

				if (simpleBlanketLineSplit.IsExpired == true && MrpGraph?.Setup?.Current?.IncludeExpiredBlanketSalesOrders == false)
				{
					InsertBlanketException(simpleBlanketLineSplit,
						new InventoryItem { InventoryID = inventoryItem.InventoryID, ItemClassID = inventoryItem.ItemClassID },
						MRPExceptionType.Delete);
					continue;
				}

				if (simpleBlanketLineSplit.SchedShipDate == null)
				{
					InsertBlanketException(simpleBlanketLineSplit,
						new InventoryItem { InventoryID = inventoryItem.InventoryID, ItemClassID = inventoryItem.ItemClassID },
						MRPExceptionType.OrderWithoutSchedDate);
					continue;
				}

				int onHoldStatus = GetOnHoldStatusForBalnketOrders(simpleBlanketLineSplit);
				var row = new AMRPDetailFP
				{
					Type = MRPPlanningType.BlanketOrder,
					SDFlag = MRPSDFlag.Demand,
					InventoryID = simpleBlanketLineSplit.InventoryID,
					SubItemID = simpleBlanketLineSplit.SubItemID,
					SiteID = simpleBlanketLineSplit.SiteID,
					PlanDate = simpleBlanketLineSplit.SchedShipDate,
					Qty = simpleBlanketLineSplit.OpenOrderQty,
					OriginalQty = simpleBlanketLineSplit.Qty,
					OnHoldStatus = onHoldStatus,
					Processed = onHoldStatus == OnHoldStatus.OnHoldExclude,
					RefType = MRPPlanningType.BlanketOrder,
					RefNbr = RefNbrFieldAttribute.FormatFieldNbr(simpleBlanketLineSplit.OrderType, simpleBlanketLineSplit.OrderNbr),
					RefNoteID = simpleBlanketLineSplit.NoteID,
					LowLevel = inventoryItem.AMLowLevel,
					ItemClassID = inventoryItem.ItemClassID,
					BranchID = simpleBlanketLineSplit.BranchID,
				};

				InsertDetailFP(row);
			}

		}

		private int GetOnHoldStatusForBalnketOrders(SimpleBlanketLineSplit simpleBlanketLineSplit)
		{
			if (simpleBlanketLineSplit.Hold == false || simpleBlanketLineSplit.Status != SOOrderStatus.PendingApproval || simpleBlanketLineSplit.Rejected == false)
			{
				return OnHoldStatus.NotOnHold;
			}

			if (MrpGraph?.Setup?.Current?.IncludeExpiredBlanketSalesOrders == true)
			{
				return OnHoldStatus.OnHoldInclude;
			}

			return OnHoldStatus.OnHoldExclude;
		}

		/// <summary>
		/// Total the sales quantity for the forecast item and given date range.
		/// For use with Dependent forecasts.
		/// </summary>
		/// <param name="forecast"></param>
		/// <param name="beginDate"></param>
		/// <param name="endDate"></param>
		/// <returns>total base order quantity not less than zero</returns>
		[Obsolete("This method has been deprecated and will be removed in Acumatica ERP 2024R2.")]
		public virtual decimal ForecastQtyConsumed(AMForecast forecast, DateTime beginDate, DateTime endDate)
        {
            if (forecast?.InventoryID == null
                || forecast.SiteID == null
                || Common.Dates.IsDefaultDate(beginDate)
                || Common.Dates.IsDefaultDate(endDate)
                || !Common.Dates.StartBeforeEnd(beginDate, endDate))
            {
                return 0m;
            }

            PXSelectBase<SimpleSalesLine> cmd = new PXSelectGroupBy<SimpleSalesLine,
                Where<SimpleSalesLine.siteID, Equal<Required<SimpleSalesLine.siteID>>,
                    And<SimpleSalesLine.inventoryID, Equal<Required<SimpleSalesLine.inventoryID>>,
                    And<SimpleSalesLine.shipDate, Between<Required<SimpleSalesLine.shipDate>, Required<SimpleSalesLine.shipDate>>>>>,
                Aggregate<Sum<SimpleSalesLine.baseOrderQty>>
                >(MrpGraph);

			var parms = new List<object>
            {
                forecast.SiteID, forecast.InventoryID, beginDate, endDate
            };

            if (MrpGraph?.Setup?.Current?.IncludeOnHoldSalesOrder != true)
            {
                // Hold in INItemPlan is true if the Order is on hold or is pending approval so Hold is fine here
                cmd.WhereAnd<Where<SimpleSalesLine.hold, Equal<False>>>();
			}

            if (AM.InventoryHelper.SubItemFeatureEnabled && forecast.SubItemID != null)
            {
                cmd.WhereAnd<Where<SimpleSalesLine.subItemID, Equal<Required<SimpleSalesLine.subItemID>>>>();
                parms.Add(forecast.SubItemID);
			}

            if (forecast.CustomerID != null)
            {
                cmd.WhereAnd<Where<SimpleSalesLine.customerID, Equal<Required<SimpleSalesLine.customerID>>>>();
                parms.Add(forecast.CustomerID);
			}

			SimpleSalesLine soLine = cmd.Select(parms.ToArray());

			return (soLine?.BaseOrderQty ?? 0m).NotLessZero();
		}

		/// <summary>
		/// Total the BL sales quantity for the forecast item and given date range.
		/// For use with Dependent forecasts.
		/// </summary>
		/// <param name="forecast"></param>
		/// <param name="beginDate"></param>
		/// <param name="endDate"></param>
		/// <returns>total open order quantity not less than zero</returns>
		[Obsolete("This method has been deprecated and will be removed in Acumatica ERP 2024R2.")]
		public virtual decimal ForecastBlanketOrderQtyConsumed(AMForecast forecast, DateTime beginDate, DateTime endDate)
		{
			if (forecast?.InventoryID == null
				|| forecast.SiteID == null
				|| Common.Dates.IsDefaultDate(beginDate)
				|| Common.Dates.IsDefaultDate(endDate)
				|| !Common.Dates.StartBeforeEnd(beginDate, endDate))
			{
				return 0m;
			}

			PXSelectBase<SimpleBlanketLineSplit> cmd2 = new PXSelectGroupBy<SimpleBlanketLineSplit,
				Where<SimpleBlanketLineSplit.siteID, Equal<Required<SimpleBlanketLineSplit.siteID>>,
					And<SimpleBlanketLineSplit.inventoryID, Equal<Required<SimpleBlanketLineSplit.inventoryID>>,
					And<SimpleBlanketLineSplit.schedShipDate, Between<Required<SimpleBlanketLineSplit.schedShipDate>, Required<SimpleBlanketLineSplit.schedShipDate>>>>>,
				Aggregate<Sum<SimpleBlanketLineSplit.qty, Sum<SimpleBlanketLineSplit.shippedQty, Sum<SimpleBlanketLineSplit.qtyOnOrders>>>>
				>(MrpGraph);

			var parms = new List<object>
			{
				forecast.SiteID, forecast.InventoryID, beginDate, endDate
			};

			if (MrpGraph?.Setup?.Current?.IncludeOnHoldSalesOrder != true)
			{
				cmd2.WhereAnd<Where<SimpleBlanketLineSplit.hold, Equal<False>>>();
			}

			if (AM.InventoryHelper.SubItemFeatureEnabled && forecast.SubItemID != null)
			{
				cmd2.WhereAnd<Where<SimpleBlanketLineSplit.subItemID, Equal<Required<SimpleBlanketLineSplit.subItemID>>>>();
				parms.Add(forecast.SubItemID);
			}

			if (forecast.CustomerID != null)
			{
				cmd2.WhereAnd<Where<SimpleBlanketLineSplit.customerID, Equal<Required<SimpleBlanketLineSplit.customerID>>>>();
				parms.Add(forecast.CustomerID);
			}

			if (MrpGraph?.Setup?.Current?.IncludeExpiredBlanketSalesOrders != true)
			{
				cmd2.WhereAnd<Where<SimpleBlanketLineSplit.status, NotEqual<SOOrderStatus.expired>>>();
			}

			SimpleBlanketLineSplit blanketSplit = cmd2.Select(parms.ToArray());

			return (blanketSplit?.OpenOrderQty ?? 0m).NotLessZero();
		}

		protected virtual void InsertBlanketException(SimpleBlanketLineSplit simpleBlanketLineSplit, InventoryItem inventoryItem, string mRPExceptionType)
		{
			var except = new AMRPExceptions
			{
				Type = mRPExceptionType,
				Qty = simpleBlanketLineSplit.OpenOrderQty,
				InventoryID = simpleBlanketLineSplit.InventoryID,
				SubItemID = simpleBlanketLineSplit.SubItemID,
				PromiseDate = simpleBlanketLineSplit.SchedShipDate ?? MrpGraph.ProcessDateTime,
				RequiredDate = simpleBlanketLineSplit.SchedOrderDate ?? MrpGraph.ProcessDateTime,
				RefType = MRPPlanningType.BlanketOrder,
				RefNbr = RefNbrFieldAttribute.FormatFieldNbr(simpleBlanketLineSplit.OrderType, simpleBlanketLineSplit.OrderNbr),
				RefNoteID = simpleBlanketLineSplit.NoteID,
				SiteID = simpleBlanketLineSplit.SiteID,
				ItemClassID = inventoryItem.ItemClassID,
				BranchID = simpleBlanketLineSplit.BranchID
			};

			MrpGraph.InsertMrpException(except);
		}

		/// <summary>
		/// Created to avoid numerous sub queries
		/// </summary>
		[PXProjection(typeof(Select2<SOLine,
                InnerJoin<SOOrder,
                    On<SOLine.orderType, Equal<SOOrder.orderType>,
                        And<SOLine.orderNbr, Equal<SOOrder.orderNbr>>>,
                InnerJoin<SOOrderType,
                    On<SOOrder.orderType, Equal<SOOrderType.orderType>>>>,
				Where<SOOrder.cancelled, Equal<False>,
					And<Where2<Where<SOOrderType.iNDocType, Equal<INTranType.invoice>,
							Or<SOOrderType.iNDocType, Equal<INTranType.issue>>>,
						And<Where<SOOrderTypeExt.aMIncludeSupplyPlan, Equal<True>,
							Or<Where<SOOrderTypeExt.aMIncludeSupplyPlan.IfNullThen<False>, Equal<False>,
								And<SOLine.operation, NotEqual<SOOperation.receipt>>>>>>>>>>), Persistent = false)]
        [PXHidden]
        [Serializable]
        public class SimpleSalesLine : PXBqlTable, IBqlTable
        {
            #region InventoryID
            public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
            protected Int32? _InventoryID;
            [PXDBInt(BqlField = typeof(SOLine.inventoryID))]
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
            [PXDBInt(BqlField = typeof(SOLine.subItemID))]
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
            [PXDBInt(BqlField = typeof(SOLine.siteID))]
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
            #region ShipDate
            public abstract class shipDate : PX.Data.BQL.BqlDateTime.Field<shipDate> { }
            protected DateTime? _ShipDate;
            [PXDBDate(BqlField = typeof(SOLine.shipDate))]
            [PXUIField(DisplayName = "Ship Date")]
            public virtual DateTime? ShipDate
            {
                get
                {
                    return this._ShipDate;
                }
                set
                {
                    this._ShipDate = value;
                }
            }
            #endregion
            #region BaseOrderQty
            public abstract class baseOrderQty : PX.Data.BQL.BqlDecimal.Field<baseOrderQty> { }
            protected Decimal? _BaseOrderQty;
            [PXDBDecimal(6, BqlField = typeof(SOLine.baseOrderQty))]
            [PXDefault(TypeCode.Decimal, "0.0")]
            [PXUIField(DisplayName = "Base Order Qty.", Visible = false, Enabled = false)]
            public virtual Decimal? BaseOrderQty
            {
                get
                {
                    return this._BaseOrderQty;
                }
                set
                {
                    this._BaseOrderQty = value;
                }
            }
            #endregion

            #region OrderType
            public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }

            protected String _OrderType;
            [PXDBString(2, IsKey = true, IsFixed = true, InputMask = ">aa", BqlField = typeof(SOOrder.orderType))]
            [PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.SelectorVisible)]
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
            [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(SOOrder.orderNbr))]
            [PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual String OrderNbr
            {
                get
                {
                    return this._OrderNbr;
                }
                set
                {
                    this._OrderNbr = value;
                }
            }
            #endregion
            #region CustomerID
            public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }

            protected Int32? _CustomerID;
            [PXDBInt(BqlField = typeof(SOOrder.customerID))]
            public virtual Int32? CustomerID
            {
                get
                {
                    return this._CustomerID;
                }
                set
                {
                    this._CustomerID = value;
                }
            }
            #endregion
            #region Hold
            public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }

            protected Boolean? _Hold;
            [PXDBBool(BqlField = typeof(SOOrder.hold))]
            [PXUIField(DisplayName = "Hold")]
            public virtual Boolean? Hold
            {
                get
                {
                    return this._Hold;
                }
                set
                {
                    this._Hold = value;
                }
            }
			#endregion
		}

		[PXProjection(typeof(Select2<SOOrder,
                InnerJoin<SOOrderType,
                    On<SOOrder.orderType, Equal<SOOrderType.orderType>>>>), Persistent = false)]
        [PXHidden]
        [Serializable]
        public class SimpleOrder : PXBqlTable, IBqlTable
        {
            #region OrderType
            public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
            protected String _OrderType;
            [PXDBString(2, IsKey = true, IsFixed = true, InputMask = ">aa", BqlField = typeof(SOOrder.orderType))]
            [PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.SelectorVisible)]
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
            [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(SOOrder.orderNbr))]
            [PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual String OrderNbr
            {
                get
                {
                    return this._OrderNbr;
                }
                set
                {
                    this._OrderNbr = value;
                }
            }
            #endregion
            #region CustomerID
            public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
            protected Int32? _CustomerID;
            [PXDBInt(BqlField = typeof(SOOrder.customerID))]
            public virtual Int32? CustomerID
            {
                get
                {
                    return this._CustomerID;
                }
                set
                {
                    this._CustomerID = value;
                }
            }
            #endregion
            #region Hold
            public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
            protected Boolean? _Hold;
            [PXDBBool(BqlField = typeof(SOOrder.hold))]
            [PXUIField(DisplayName = "Hold")]
            public virtual Boolean? Hold
            {
                get
                {
                    return this._Hold;
                }
                set
                {
                    this._Hold = value;
                }
            }
            #endregion
            #region CreditHold
            public abstract class creditHold : PX.Data.BQL.BqlBool.Field<creditHold> { }

            protected Boolean? _CreditHold;
            [PXDBBool(BqlField = typeof(SOOrder.creditHold))]
            [PXDefault(false)]
            [PXUIField(DisplayName = "Credit Hold")]
            public virtual Boolean? CreditHold
            {
                get
                {
                    return this._CreditHold;
                }
                set
                {
                    this._CreditHold = value;
                }
            }
            #endregion
            #region Completed
            public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }

            protected Boolean? _Completed;
            [PXDBBool(BqlField = typeof(SOOrder.completed))]
            [PXDefault(false)]
            [PXUIField(DisplayName = "Completed")]
            public virtual Boolean? Completed
            {
                get
                {
                    return this._Completed;
                }
                set
                {
                    this._Completed = value;
                }
            }
            #endregion
            #region Cancelled
            public abstract class cancelled : PX.Data.BQL.BqlBool.Field<cancelled> { }

            protected Boolean? _Cancelled;
            [PXDBBool(BqlField = typeof(SOOrder.cancelled))]
            [PXDefault(false)]
            [PXUIField(DisplayName = "Canceled")]
            public virtual Boolean? Cancelled
            {
                get
                {
                    return this._Cancelled;
                }
                set
                {
                    this._Cancelled = value;
                }
            }
            #endregion
            #region DestinationSiteID
            public abstract class destinationSiteID : PX.Data.BQL.BqlInt.Field<destinationSiteID> { }

            protected Int32? _DestinationSiteID;
            [PXDBInt(BqlField = typeof(SOOrder.destinationSiteID))]
            [PXUIField(DisplayName = "To Warehouse")]
            public virtual Int32? DestinationSiteID
            {
                get
                {
                    return this._DestinationSiteID;
                }
                set
                {
                    this._DestinationSiteID = value;
                }
            }
            #endregion
            #region INDocType
            public abstract class iNDocType : PX.Data.BQL.BqlString.Field<iNDocType> { }
            protected String _INDocType;
            [PXDBString(3, IsFixed = true, BqlField = typeof(SOOrderType.iNDocType))]
            [INTranType.SOList]
            [PXUIField(DisplayName = "Inventory Transaction Type")]
            public virtual String INDocType
            {
                get { return this._INDocType; }
                set { this._INDocType = value; }
            }
            #endregion
            #region NoteID
            public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }

            protected Guid? _NoteID;
            [PXDBGuidAttribute(BqlField = typeof(SOOrder.noteID))]
            public virtual Guid? NoteID
            {
                get
                {
                    return this._NoteID;
                }
                set
                {
                    this._NoteID = value;
                }
            }
			#endregion
			#region AMIncludeSupplyPlan
			public abstract class aMIncludeSupplyPlan : PX.Data.BQL.BqlBool.Field<aMIncludeSupplyPlan> { }

			protected bool? _AMIncludeSupplyPlan;
			[PXDBBool(BqlField = typeof(SOOrderTypeExt.aMIncludeSupplyPlan))]
			public virtual bool? AMIncludeSupplyPlan
			{
				get
				{
					return this._AMIncludeSupplyPlan;
				}
				set
				{
					this._AMIncludeSupplyPlan = value;
				}
			}
			#endregion
		}

		[PXProjection(typeof(Select2<SOLineSplit,
                InnerJoin<SOLine,
                    On<SOLineSplit.orderType, Equal<SOLine.orderType>,
						And<SOLineSplit.orderNbr, Equal<SOLine.orderNbr>,
							And<SOLineSplit.lineNbr, Equal<SOLine.lineNbr>>>>>>), Persistent = false)]
        [PXHidden]
        [Serializable]
		public class SimpleSalesLineSplit : PXBqlTable, IBqlTable
		{
			#region PlanID
			public abstract class planID : PX.Data.BQL.BqlLong.Field<planID> { }

			[PXDBLong(IsImmutable = true, IsKey = true, BqlField = typeof(SOLineSplit.planID))]
			public virtual Int64? PlanID { get; set; }

			#endregion
			#region RequestDate
			public abstract class requestDate : PX.Data.BQL.BqlDateTime.Field<requestDate> { }

			[PXDBDate(BqlField = typeof(SOLine.requestDate))]
			[PXUIField(DisplayName = "Requested On")]
			public virtual DateTime? RequestDate { get; set; }
			#endregion
			#region Operation
			public abstract class operation : PX.Data.BQL.BqlString.Field<operation> { }
			[PXDBString(1, IsFixed = true, InputMask = ">a", BqlField = typeof(SOLine.operation))]
			[SOOperation.List]
			public virtual String Operation { get; set; }
			#endregion
		}

        /// <summary>
        /// Projection for query to LoadINItemPlan
        /// </summary>
        [PXProjection(typeof(Select2<INItemPlan,
            InnerJoin<InventoryItem,
                On<INItemPlan.inventoryID, Equal<InventoryItem.inventoryID>>,
                LeftJoin<Note,
                    On<INItemPlan.refNoteID, Equal<Note.noteID>>,
                    InnerJoin<INSite,
                        On<INItemPlan.siteID, Equal<INSite.siteID>>,
                        LeftJoin<INItemSite,
							On<INItemPlan.siteID, Equal<INItemSite.siteID>,
								And<INItemPlan.inventoryID, Equal<INItemSite.inventoryID>>>,
                            LeftJoin<INLocation,
                                On<INItemPlan.siteID, Equal<INLocation.siteID>,
                                    And<INItemPlan.locationID, Equal<INLocation.locationID>>>,
                                InnerJoin<INPlanType,
                                    On<INItemPlan.planType, Equal<INPlanType.planType>>,
                                    LeftJoin<AMMrpProdMatl,
                                        On<INItemPlan.planID, Equal<AMMrpProdMatl.planID>>,
                                        LeftJoin<AMProdItem,
                                            On<INItemPlan.refNoteID, Equal<AMProdItem.noteID>>,
                                            LeftJoin<SimpleOrder,
                                                On<INItemPlan.refNoteID, Equal<SimpleOrder.noteID>>,
											    LeftJoin<SimpleSalesLineSplit,
												    On<INItemPlan.planID, Equal<SimpleSalesLineSplit.planID>>,
													    LeftJoin<POOrder,
														    On<POOrder.noteID, Equal<INItemPlan.refNoteID>>,
															    LeftJoin<INKitSpecHdrLastRevisionDate,
																    On<INKitSpecHdrLastRevisionDate.kitInventoryID.IsEqual<InventoryItem.inventoryID>>>>>>>>>>>>>>,
			Where2<Where<InventoryItem.stkItem, Equal<False>,And<INKitSpecHdrLastRevisionDate.kitInventoryID.IsNotNull>>,Or<Where<InventoryItem.stkItem, Equal<True>,
                And<Where<IsNull<INItemSite.planningMethod, InventoryItem.planningMethod>, Equal<INPlanningMethod.dRP>,
                    Or<IsNull<INItemSite.planningMethod, InventoryItem.planningMethod>, Equal<INPlanningMethod.mRP>>>>>>>>), Persistent = false)]
        [PXHidden]
        [Serializable]
        public class LoadINItemPlanProjection : INItemPlan
        {
            #region ItemClassID (InventoryItem)
            /// <summary>
            /// InventoryItem.ItemClassID
            /// </summary>
            public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }

            /// <summary>
            /// InventoryItem.ItemClassID
            /// </summary>
            [PXDBInt(BqlField = typeof(InventoryItem.itemClassID))]
            [PXUIField(DisplayName = "Item Class")]
            public virtual int? ItemClassID { get; set; }
            #endregion
            #region AMLowLevel (InventoryItemExt)
            /// <summary>
            /// InventoryItemExt.AMLowLevel
            /// Non UI field - keeps items lowest bom level value used in calculations
            /// </summary>
            public abstract class aMLowLevel : PX.Data.BQL.BqlInt.Field<aMLowLevel> { }
            /// <summary>
            /// InventoryItemExt.AMLowLevel
            /// Non UI field - keeps items lowest bom level value used in calculations
            /// </summary>
            [PXDBInt(BqlField = typeof(InventoryItemExt.aMLowLevel))]
            [PXUIField(DisplayName = "Low Level")]
            public Int32? AMLowLevel { get; set; }
            #endregion

            #region EntityType (Note)
            /// <summary>
            /// Note.EntityType
            /// </summary>
            public abstract class entityType : PX.Data.BQL.BqlString.Field<entityType> { }

            /// <summary>
            /// Note.EntityType
            /// </summary>
            [PXDBString(BqlField = typeof(Note.entityType))]
            [PXUIField(DisplayName = "Note EntityType")]
            public string EntityType { get; set; }
            #endregion

            #region AMMRPPO (INSiteExt)
            /// <summary>
            /// INSiteExt.AMMRPPO
            /// MRP Include Purchase Orders
            /// </summary>
            public abstract class aMMRPPO : PX.Data.BQL.BqlBool.Field<aMMRPPO> { }
            /// <summary>
            /// INSiteExt.AMMRPPO
            /// MRP Include Purchase Orders
            /// </summary>
            [PXDBBool(BqlField = typeof(INSiteExt.aMMRPPO))]
            [PXUIField(DisplayName = "Site MRP Include Purchase Orders")]
            public Boolean? AMMRPPO { get; set; }
            #endregion
            #region AMMRPProd (INSiteExt)
            /// <summary>
            /// INSiteExt.AMMRPProd
            /// MRP Include Production Orders
            /// </summary>
            public abstract class aMMRPProd : PX.Data.BQL.BqlBool.Field<aMMRPProd> { }
            /// <summary>
            /// INSiteExt.AMMRPProd
            /// MRP Include Production Orders
            /// </summary>
            [PXDBBool(BqlField = typeof(INSiteExt.aMMRPProd))]
            [PXUIField(DisplayName = "Site MRP Include Production Orders")]
            public Boolean? AMMRPProd { get; set; }
            #endregion
            #region AMMRPShip (INSiteExt)
            /// <summary>
            /// INSiteExt.AMMRPShip
            /// MRP Include Shipments
            /// </summary>
            public abstract class aMMRPShip : PX.Data.BQL.BqlBool.Field<aMMRPShip> { }
            /// <summary>
            /// INSiteExt.AMMRPShip
            /// MRP Include Shipments
            /// </summary>
            [PXDBBool(BqlField = typeof(INSiteExt.aMMRPShip))]
            [PXUIField(DisplayName = "Site MRP Include Shipments")]
            public Boolean? AMMRPShip { get; set; }
            #endregion
            #region AMMRPSO (INSiteExt)
            /// <summary>
            /// INSiteExt.AMMRPSO
            /// MRP Include Sales Orders
            /// </summary>
            public abstract class aMMRPSO : PX.Data.BQL.BqlBool.Field<aMMRPSO> { }
            /// <summary>
            /// INSiteExt.AMMRPSO
            /// MRP Include Sales Orders
            /// </summary>
            [PXDBBool(BqlField = typeof(INSiteExt.aMMRPSO))]
            [PXUIField(DisplayName = "Site MRP Include Sales Orders")]
            public Boolean? AMMRPSO { get; set; }
            #endregion
			#region AMMRPFlagSite (INSiteExt.AMMRPFlag)
			/// <summary>
			/// INSiteExt.aMMRPFlag
			/// MRP Include Inventory On Hand (Main flag for warehouse with MRP)
			/// </summary>
			public abstract class aMMRPFlagSite : PX.Data.BQL.BqlBool.Field<aMMRPFlagSite> { }
			/// <summary>
			/// INSiteExt.AMMRPFlag
			/// MRP Include Inventory On Hand (Main flag for warehouse with MRP)
			/// </summary>
			[PXDBBool(BqlField = typeof(INSiteExt.aMMRPFlag))]
			[PXUIField(DisplayName = "Inventory On Hand")]
			public Boolean? AMMRPFlagSite { get; set; }
			#endregion

            #region AMMRPFlag (INLocationExt)
            /// <summary>
            /// INLocationExt.AMMRPFlag
            /// </summary>
            public abstract class aMMRPFlag : PX.Data.BQL.BqlBool.Field<aMMRPFlag> { }

            /// <summary>
            /// INLocationExt.AMMRPFlag
            /// </summary>
            [PXDBBool(BqlField = typeof(INLocationExt.aMMRPFlag))]
            [PXUIField(DisplayName = "Location MRP Enabled")]
            public Boolean? AMMRPFlag { get; set; }
            #endregion

            #region IsSupply (INPlantype)
            /// <summary>
            /// INPlanType.AsSupply
            /// </summary>
            public abstract class isSupply : PX.Data.BQL.BqlBool.Field<isSupply> { }

            /// <summary>
            /// INPlanType.AsSupply
            /// </summary>
            [PXDBBool(BqlField = typeof(INPlanType.isSupply))]
            [PXUIField(DisplayName = "Is Supply")]
            public virtual Boolean? IsSupply { get; set; }
            #endregion

            #region ProdItemInventoryID (AMMrpProdMatl.ProdItemInventoryID = AMProdItem.inventoryID)
            /// <summary>
            /// AMMrpProdMatl.ProdItemInventoryID = AMProdItem.inventoryID
            /// </summary>
            public abstract class prodItemInventoryID : PX.Data.BQL.BqlInt.Field<prodItemInventoryID> { }

            /// <summary>
            /// AMMrpProdMatl.ProdItemInventoryID = AMProdItem.inventoryID
            /// </summary>
            [PXDBInt(BqlField = typeof(AMMrpProdMatl.prodItemInventoryID))]
            [PXUIField(DisplayName = "Production Inventory ID")]
            public virtual Int32? ProdItemInventoryID { get; set; }
            #endregion
            #region ProdItemSubItemID (AMMrpProdMatl.prodItemsubItemID = AMProdItem.subItemID)
            /// <summary>
            /// AMMrpProdMatl.prodItemsubItemID = AMProdItem.subItemID
            /// </summary>
            public abstract class prodItemsubItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }

            /// <summary>
            /// AMMrpProdMatl.prodItemsubItemID = AMProdItem.subItemID
            /// </summary>
            [PXDBInt(BqlField = typeof(AMMrpProdMatl.prodItemsubItemID))]
            public virtual Int32? ProdItemSubItemID { get; set; }
            #endregion
            #region ParentExcludeFromMRP (AMMrpProdMatl.ExcludeFromMRP = AMProdItem.excludeFromMRP)
            /// <summary>
            /// AMMrpProdMatl.excludeFromMRP = AMProdItem.excludeFromMRP
            /// </summary>
            public abstract class parentExcludeFromMRP : PX.Data.BQL.BqlBool.Field<parentExcludeFromMRP> { }

            /// <summary>
            /// AMMrpProdMatl.excludeFromMRP = AMProdItem.excludeFromMRP
            /// </summary>
            [PXDBBool(BqlField = typeof(AMMrpProdMatl.excludeFromMRP))]
            [PXUIField(DisplayName = "Parent Item Exclude from MRP")]
            public virtual Boolean? ParentExcludeFromMRP { get; set; }
            #endregion
			#region ProdItemOrderType
            public abstract class prodItemOrderType : PX.Data.BQL.BqlString.Field<prodItemOrderType> { }
			/// <inheritdoc cref="AMProdItem.OrderType"/>
			[PXDBString(BqlField = typeof(AMProdItem.orderType))]
            public virtual String ProdItemOrderType { get; set; }
            #endregion
			#region ProdItemProdOrdID
            public abstract class prodItemProdOrdID : PX.Data.BQL.BqlString.Field<prodItemProdOrdID> { }
			/// <inheritdoc cref="AMProdItem.ProdOrdID"/>
			[PXDBString(BqlField = typeof(AMProdItem.prodOrdID))]
            public virtual String ProdItemProdOrdID { get; set; }
			#endregion

			#region ProdMatlOrderType
            public abstract class prodMatlOrderType : PX.Data.BQL.BqlString.Field<prodMatlOrderType> { }
			/// <inheritdoc cref="AMMrpProdMatl.OrderType"/>
			[PXDBString(BqlField = typeof(AMMrpProdMatl.orderType))]
            public virtual String ProdMatlOrderType { get; set; }
			#endregion
			#region ProdMatlProdOrdID
			public abstract class prodMatlProdOrdID : PX.Data.BQL.BqlString.Field<prodMatlProdOrdID> { }
			/// <inheritdoc cref="AMMrpProdMatl.ProdOrdID"/>
			[PXDBString(BqlField = typeof(AMMrpProdMatl.prodOrdID))]
            public virtual String ProdMatlProdOrdID { get; set; }
			#endregion
			#region ProdMatlOperationCD
			public abstract class prodMatlOperationCD : PX.Data.BQL.BqlString.Field<prodMatlOperationCD> { }
			/// <inheritdoc cref="AMMrpProdMatl.OperationCD"/>
			[PXDBString(BqlField = typeof(AMMrpProdMatl.operationCD))]
            public virtual String ProdMatlOperationCD { get; set; }
			#endregion

            #region ExcludeFromMRP (AMProdItem.ExcludeFromMRP)
            /// <summary>
            /// AMProdItem.ExcludeFromMRP
            /// </summary>
            public abstract class excludeFromMRP : PX.Data.BQL.BqlBool.Field<excludeFromMRP> { }

            /// <summary>
            /// AMProdItem.ExcludeFromMRP
            /// </summary>
            [PXDBBool(BqlField = typeof(AMProdItem.excludeFromMRP))]
            [PXUIField(DisplayName = "Exclude from MRP")]
            public virtual Boolean? ExcludeFromMRP { get; set; }
            #endregion
            #region StatusID (AMProdItem)
            /// <summary>
            /// AMProdItem.StatusID
            /// </summary>
            public abstract class statusID : PX.Data.BQL.BqlString.Field<statusID> { }

            /// <summary>
            /// AMProdItem.StatusID
            /// </summary>
            [PXDBString(1, IsFixed = true, BqlField = typeof(AMProdItem.statusID))]
            [PXUIField(DisplayName = "Production Order Status")]
            public virtual String StatusID { get; set; }
			#endregion
			#region ProdItemCompleted  (AMProdItem.Completed)
			/// <summary>
			/// AMProdItem.completed
			/// </summary>
			public abstract class prodItemCompleted : PX.Data.BQL.BqlBool.Field<prodItemCompleted> { }
			/// <summary>
			/// AMProdItem.Completed
			/// </summary>
			[PXDBBool(BqlField = typeof(AMProdItem.completed))]
			[PXUIField(DisplayName = "Completed", Visible = false)]
			public virtual Boolean? ProdItemCompleted { get; set; }
			#endregion

			#region DestinationSiteID (SimpleOrder.destinationSiteID = SOOrder.destinationSiteID)
			/// <summary>
			/// SimpleOrder.destinationSiteID = SOOrder.destinationSiteID
			/// </summary>
			public abstract class destinationSiteID : PX.Data.BQL.BqlInt.Field<destinationSiteID> { }

            /// <summary>
            /// SimpleOrder.destinationSiteID = SOOrder.destinationSiteID
            /// </summary>
            [PXDBInt(BqlField = typeof(SimpleOrder.destinationSiteID))]
            [PXUIField(DisplayName = "SO Order To Warehouse")]
            public virtual Int32? DestinationSiteID { get; set; }
            #endregion
            #region INDocType (SimpleOrder.iNDocType = SOOrderType.iNDocType)
            /// <summary>
            /// SimpleOrder.iNDocType = SOOrderType.iNDocType
            /// </summary>
            public abstract class iNDocType : PX.Data.BQL.BqlString.Field<iNDocType> { }

            /// <summary>
            /// SimpleOrder.iNDocType = SOOrderType.iNDocType
            /// </summary>
            [PXDBString(3, IsFixed = true, BqlField = typeof(SimpleOrder.iNDocType))]
            [PXUIField(DisplayName = "SO Order Inventory Transaction Type")]
            public virtual String INDocType { get; set; }
			#endregion
			#region SOOrderType
			public abstract class sOOrderType : PX.Data.BQL.BqlString.Field<sOOrderType> { }
            protected String _SOOrderType;
			/// <inheritdoc cref="SimpleOrder.OrderType"/>
			[PXDBString(2, IsFixed = true, InputMask = ">aa", BqlField = typeof(SimpleOrder.orderType))]
            public virtual String SOOrderType
            {
                get
                {
                    return this._SOOrderType;
                }
                set
                {
                    this._SOOrderType = value;
                }
            }
			#endregion
			#region SOOrderNbr
			public abstract class sOOrderNbr : PX.Data.BQL.BqlString.Field<sOOrderNbr> { }
            protected String _SOOrderNbr;
			/// <inheritdoc cref="SimpleOrder.OrderNbr"/>
			[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(SimpleOrder.orderNbr))]
            public virtual String SOOrderNbr
            {
                get
                {
                    return this._SOOrderNbr;
                }
                set
                {
                    this._SOOrderNbr = value;
                }
            }
			#endregion

			#region RequestDate (SimpleSalesLineSplit - SOLine.RequestDate)
			public abstract class requestDate : PX.Data.BQL.BqlDateTime.Field<requestDate> { }
			/// <summary>
			/// SOLine.RequestDate
			/// </summary>
			[PXDBDate(BqlField = typeof(SimpleSalesLineSplit.requestDate))]
			public virtual DateTime? RequestDate { get; set; }
			#endregion

			#region BranchID
			public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }

			/// <summary>
			/// INSite.BranchID
			/// </summary>
			[PXDBInt(BqlField = typeof(INSite.branchID))]
			public virtual Int32? BranchID { get; set; }

			#endregion

			#region StkItem
			public abstract class stkItem : PX.Data.BQL.BqlBool.Field<stkItem> { }
			/// <summary>
			/// When set to <c>true</c>, indicates that this item is a Stock Item.
			/// </summary>
			[PXDBBool(BqlField =typeof(InventoryItem.stkItem))]
			public virtual Boolean? StkItem { get; set; }
			#endregion
			#region Operation
			public abstract class operation : PX.Data.BQL.BqlString.Field<operation> { }
			[PXDBString(1, IsFixed = true, InputMask = ">a", BqlField = typeof(SimpleSalesLineSplit.operation))]
			[SOOperation.List]
			public virtual String Operation { get; set; }
			#endregion
			#region AMIncludeSupplyPlan
			public abstract class aMIncludeSupplyPlan : PX.Data.BQL.BqlBool.Field<aMIncludeSupplyPlan> { }

			protected bool? _AMIncludeSupplyPlan;
			[PXDBBool(BqlField = typeof(SimpleOrder.aMIncludeSupplyPlan))]
			public virtual bool? AMIncludeSupplyPlan
			{
				get
				{
					return this._AMIncludeSupplyPlan;
				}
				set
				{
					this._AMIncludeSupplyPlan = value;
				}
			}
			#endregion

			#region POOrderType
			public abstract class pOOrderType : PX.Data.BQL.BqlString.Field<pOOrderType> { }
            protected String _POOrderType;
			/// <inheritdoc cref="POOrder.OrderType"/>
			[PXDBString(2, IsFixed = true, BqlField = typeof(POOrder.orderType))]
            public virtual String POOrderType
            {
                get
                {
                    return this._POOrderType;
                }
                set
                {
                    this._POOrderType = value;
                }
            }
			#endregion
			#region POOrderNbr
			public abstract class pOOrderNbr : PX.Data.BQL.BqlString.Field<pOOrderNbr> { }
            protected String _POOrderNbr;
			/// <inheritdoc cref="POOrder.OrderNbr"/>
			[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(POOrder.orderNbr))]
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
		}

		/// <summary>
		/// Projection for query to Simple Blanket Line Split
		/// </summary>
		[PXProjection(typeof(SelectFrom<SOLineSplit>
			.InnerJoin<SOOrder>.On<SOOrder.orderType.IsEqual<SOLineSplit.orderType>
				.And<SOOrder.orderNbr.IsEqual<SOLineSplit.orderNbr>>>
			.InnerJoin<SOOrderType>.On<SOOrderType.orderType.IsEqual<SOLineSplit.orderType>>
			.Where<SOOrderType.behavior.IsEqual<SOBehavior.bL>
				.And<SOOrder.cancelled.IsEqual<False>>.And<SOOrder.completed.IsEqual<False>>>))]
		[PXHidden]
		[Serializable]
		public class SimpleBlanketLineSplit : PXBqlTable, IBqlTable
		{
			#region Keys
			public class PK : PrimaryKeyOf<SimpleBlanketLineSplit>.By<orderType, orderNbr, lineNbr, splitLineNbr, siteID, inventoryID>
			{
				public static SimpleBlanketLineSplit Find(PXGraph graph, string orderType, string orderNbr,int? lineNbr,int? splitLineNbr, int siteID, int inventoryID, PKFindOptions options = PKFindOptions.None)
					=> FindBy(graph, orderType, orderNbr, lineNbr, splitLineNbr, siteID, inventoryID, options);
			}
			public static class FK
			{
				public class Order : SOOrder.PK.ForeignKeyOf<SimpleBlanketLineSplit>.By<orderType, orderNbr> { }
				public class OrderType : SOOrderType.PK.ForeignKeyOf<SimpleBlanketLineSplit>.By<orderType> { }
				public class InventoryItem : IN.InventoryItem.PK.ForeignKeyOf<SimpleBlanketLineSplit>.By<inventoryID> { }
				public class Site : INSite.PK.ForeignKeyOf<SimpleBlanketLineSplit>.By<siteID> { }
			}
			#endregion

			#region Status
			public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
			protected string _Status;
			[PXDBString(BqlField = typeof(SOOrder.status))]
			public virtual String Status
			{
				get
				{
					return this._Status;
				}
				set
				{
					this._Status = value;
				}
			}
			#endregion
			#region OrderType
			public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
			protected String _OrderType;
			[PXDBString(2, IsKey = true, BqlField = typeof(SOOrder.orderType))]
			[PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.SelectorVisible)]
			[PX.Data.EP.PXFieldDescription]
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
			[PXDBString(15, IsKey = true, BqlField = typeof(SOOrder.orderNbr))]
			[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			[PX.Data.EP.PXFieldDescription]
			public virtual String OrderNbr
			{
				get
				{
					return this._OrderNbr;
				}
				set
				{
					this._OrderNbr = value;
				}
			}
			#endregion
			#region BranchID
			public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
			protected Int32? _BranchID;
			[PXDBInt(BqlField = typeof(SOOrder.branchID))]
			[PXUIField(DisplayName = "Branch ID")]
			public virtual Int32? BranchID
			{
				get
				{
					return this._BranchID;
				}
				set
				{
					this._BranchID = value;
				}
			}
			#endregion
			#region Hold
			public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
			protected Boolean? _Hold;
			[PXDBBool(BqlField = typeof(SOOrder.hold))]
			[PXUIField(DisplayName = "Hold")]
			public virtual Boolean? Hold
			{
				get
				{
					return this._Hold;
				}
				set
				{
					this._Hold = value;
				}
			}
			#endregion
			#region CustomerID
			public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
			protected Int32? _CustomerID;
			[PXDBInt(BqlField = typeof(SOOrder.customerID))]
			[PXUIField(DisplayName = "Customer ID")]
			public virtual Int32? CustomerID
			{
				get
				{
					return this._CustomerID;
				}
				set
				{
					this._CustomerID = value;
				}
			}
			#endregion
			#region Qty
			public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
			protected Decimal? _Qty;
			[PXDBQuantity(BqlField = typeof(SOLineSplit.baseQty))]
			[PXUIField(DisplayName = "Quantity")]
			public virtual Decimal? Qty
			{
				get
				{
					return this._Qty;
				}
				set
				{
					this._Qty = value;
				}
			}
			#endregion
			#region ShippedQty
			public abstract class shippedQty : PX.Data.BQL.BqlDecimal.Field<shippedQty> { }
			protected Decimal? _ShippedQty;
			[PXDBQuantity(BqlField = typeof(SOLineSplit.baseShippedQty))]
			[PXUIField(DisplayName = "Qty. On Shipments", Enabled = false)]
			public virtual Decimal? ShippedQty
			{
				get
				{
					return this._ShippedQty;
				}
				set
				{
					this._ShippedQty = value;
				}
			}
			#endregion
			#region OpenOrderQty 
			public abstract class openOrderQty : PX.Data.BQL.BqlDecimal.Field<openOrderQty> { }
			protected Decimal? _OpenOrderQty;
			[PXQuantity]
			[PXDBCalced(typeof(SOLineSplit.baseQty.Subtract<SOLineSplit.baseShippedQty>
				.Subtract<SOLineSplit.qtyOnOrders>), typeof(decimal))]
			[PXUIField(DisplayName = "Open Order Qty.")]
			public virtual Decimal? OpenOrderQty
			{
				get
				{
					return this._OpenOrderQty;
				}
				set
				{
					this._OpenOrderQty = value;
				}
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
			protected Int32? _InventoryID;
			[PXDBInt(BqlField = typeof(SOLineSplit.inventoryID))]
			[PXUIField(DisplayName = "Inventory ID")]
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
			[PXDBInt( BqlField = typeof(SOLineSplit.subItemID))]
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
			[PXDBInt(BqlField =  typeof(SOLineSplit.siteID))]
			[PXUIField(DisplayName = "Site ID")]
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
			#region SchedOrderDate
			public abstract class schedOrderDate : Data.BQL.BqlDateTime.Field<schedOrderDate> { }
			protected DateTime? _SchedOrderDate;
			[PXDBDate(BqlField = typeof(SOLineSplit.schedOrderDate))]
			[PXUIField(DisplayName = "Sched. Order Date")]
			public virtual DateTime? SchedOrderDate
			{
				get
				{
					return this._SchedOrderDate;
				}
				set
				{
					this._SchedOrderDate = value;
				}
			}
			#endregion
			#region SchedShipDate
			public abstract class schedShipDate : Data.BQL.BqlDateTime.Field<schedShipDate> { }
			protected DateTime? _SchedShipDate;
			[PXDBDate(BqlField = typeof(SOLineSplit.schedShipDate))]
			[PXUIField(DisplayName = "Sched. Shipment Date")]
			public virtual DateTime? SchedShipDate
			{
				get
				{
					return this._SchedShipDate;
				}
				set
				{
					this._SchedShipDate = value;
				}
			}
			#endregion
			#region NoteID
			public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
			protected Guid? _NoteID;
			[PXUIField(DisplayName = "Related Document")]
			[PXDBGuid(BqlField = typeof(SOOrder.noteID))]
			public virtual Guid? NoteID
			{
				get
				{
					return this._NoteID;
				}
				set
				{
					this._NoteID = value;
				}
			}
			#endregion
			#region IsExpired
			public abstract class isExpired : Data.BQL.BqlBool.Field<isExpired> { }
			[PXDBBool(BqlField = typeof(SOOrder.isExpired))]
			[PXDefault(false)]
			public virtual bool? IsExpired
			{
				get;
				set;
			}
			#endregion
			#region CreditHold
			public abstract class creditHold : PX.Data.BQL.BqlBool.Field<creditHold> { }
			protected Boolean? _CreditHold;
			[PXDBBool(BqlField = typeof(SOOrder.creditHold))]
			[PXUIField(DisplayName = "Credit Hold")]
			public virtual Boolean? CreditHold
			{
				get
				{
					return this._CreditHold;
				}
				set
				{
					this._CreditHold = value;
				}
			}
			#endregion
			#region OpenDoc
			public abstract class openDoc : PX.Data.BQL.BqlBool.Field<openDoc> { }
			protected Boolean? _OpenDoc;
			[PXDBBool(BqlField = typeof(SOOrder.openDoc))]
			public virtual Boolean? OpenDoc
			{
				get
				{
					return this._OpenDoc;
				}
				set
				{
					this._OpenDoc = value;
				}
			}
			#endregion
			#region POCreate
			public abstract class pOCreate : PX.Data.BQL.BqlBool.Field<pOCreate> { }
			protected Boolean? _POCreate;
			[PXDBBool(BqlField = typeof(SOLineSplit.pOCreate))]
			[PXUIField(DisplayName = "Mark for PO", Visible = true, Enabled = false)]
			public virtual Boolean? POCreate
			{
				get
				{
					return this._POCreate;
				}
				set
				{
					this._POCreate = value ?? false;
				}
			}
			#endregion
			#region Rejected
			public abstract class rejected : PX.Data.BQL.BqlBool.Field<rejected> { }
			protected bool? _Rejected = false;
			[PXDBBool(BqlField = typeof(SOOrder.rejected))]
			public bool? Rejected
			{
				get
				{
					return _Rejected;
				}
				set
				{
					_Rejected = value;
				}
			}
			#endregion
			#region LineNbr
			public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
			protected Int32? _LineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(SOLineSplit.lineNbr))]
			public virtual Int32? LineNbr
			{
				get
				{
					return this._LineNbr;
				}
				set
				{
					this._LineNbr = value;
				}
			}
			#endregion
			#region SplitLineNbr
			public abstract class splitLineNbr : PX.Data.BQL.BqlInt.Field<splitLineNbr> { }
			protected Int32? _SplitLineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(SOLineSplit.splitLineNbr))]
			[PXUIField(DisplayName = "Allocation ID")]
			public virtual Int32? SplitLineNbr
			{
				get
				{
					return this._SplitLineNbr;
				}
				set
				{
					this._SplitLineNbr = value;
				}
			}
			#endregion
			#region QtyOnOrders
			public abstract class qtyOnOrders : Data.BQL.BqlDecimal.Field<qtyOnOrders> { }
			[PXDBQuantity(BqlField = typeof(SOLineSplit.baseQtyOnOrders))]
			[PXUIField(DisplayName = "Qty. On Orders")]
			public virtual decimal? QtyOnOrders
			{
				get;
				set;
			}
			#endregion
			#region Completed
			public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }
			protected Boolean? _Completed;
			[PXDBBool(BqlField = typeof(SOOrder.completed))]
			[PXUIField(DisplayName = "Completed")]
			public virtual Boolean? Completed
			{
				get
				{
					return this._Completed;
				}
				set
				{
					this._Completed = value;
				}
			}
			#endregion
			#region Cancelled
			public abstract class cancelled : PX.Data.BQL.BqlBool.Field<cancelled> { }
			protected Boolean? _Cancelled;
			[PXDBBool(BqlField = typeof(SOOrder.cancelled))]
			[PXUIField(DisplayName = "Canceled")]
			public virtual Boolean? Cancelled
			{
				get
				{
					return this._Cancelled;
				}
				set
				{
					this._Cancelled = value;
				}
			}
			#endregion
		}
	}
}

using CompiledVersion.DAC;
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM.Extensions;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.PO.GraphExtensions.POOrderEntryExt;
using System;
using System.Collections.Generic;
using static PX.Objects.PO.POOrderEntry;
using CommonServiceLocator;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.Common.DAC;
using System.Linq;

namespace CompiledVersion.Graphs
{
    public class POCreateReplaceOriginalMethod : PXGraphExtension<POCreate>
    {
        public static bool IsActive() => true;

        #region Messages
        [PXLocalizable]
        internal static class Messages
        {
            public const string DropShipLineInvalid = "The line cannot be drop-shipped because it is split into multiple lines or allocated in the Line Details dialog box.";
            public const string POOrderCreated = "Purchase Order '{0}' created.";
            public const string SpecialOrderCurrencyDiffNoOverride = "The vendor with the currency that differs from the currency of the sales order is selected and it is not possible to override the currency in purchase orders. The purchase order for the special-order item cannot be created.";
            public const string SpecialOrderCurrencyDiffWillCreate = "The vendor with the currency that differs from the currency of the sales order is selected. A purchase order with the currency of the sales order will be created.";
        }
        #endregion

        #region CreateProc
        public delegate void CreateProcDelegate(List<POFixedDemand> list, Nullable<DateTime> orderDate, Boolean extSort, Nullable<Int32> branchID);
        [PXOverride]
        public void CreateProc(List<POFixedDemand> list, Nullable<DateTime> orderDate, Boolean extSort, Nullable<Int32> branchID, CreateProcDelegate baseMethod)
        {
            //baseMethod(list, orderDate, extSort, branchID);
            PXRedirectRequiredException poredirect = CreatePOOrders(list, orderDate, extSort, branchID);
            if (poredirect != null)
            {
                throw poredirect;
            }
        }
        #endregion

        #region CreatePOOrders
        public virtual PXRedirectRequiredException CreatePOOrders(List<POFixedDemand> list, DateTime? PurchDate, bool extSort, int? branchID = null)
        {
            POOrderEntry docgraph = PXGraph.CreateInstance<POOrderEntry>();
            docgraph.Views.Caches.Add(typeof(POOrderEntry.SOLineSplit3));
            POSetup setup = docgraph.POSetup.Current;
            DocumentList<POOrder> created = new DocumentList<POOrder>(docgraph);
            Dictionary<string, DocumentList<POLine>> orderedByPlantype = new Dictionary<string, DocumentList<POLine>>();
            list = docgraph.SortPOFixDemandList(list);
            POOrder order = null;
            bool hasErrors = false;

            foreach (POFixedDemand demand in list)
            {
                PXProcessing<POFixedDemand>.SetCurrentItem(demand);
                if (demand.FixedSource != "P")
                {
                    continue;
                }
                if (!demand.VendorID.HasValue || !demand.VendorLocationID.HasValue)
                {
                    PXProcessing<POFixedDemand>.SetWarning("Vendor and vendor location should be defined.");
                    continue;
                }
                PXErrorLevel ErrorLevel = PXErrorLevel.RowInfo;
                string ErrorText = string.Empty;
                try
                {
                    string message = GetSpecialOrderCurrencyError(demand, onlyErrors: true);
                    if (!string.IsNullOrEmpty(message))
                    {
                        throw new PXException(message);
                    }
                    SOOrder soorder = PXSelectBase<SOOrder, PXSelect<SOOrder, Where<SOOrder.noteID, Equal<Required<SOOrder.noteID>>>>.Config>.Select(docgraph, demand.RefNoteID);
                    POOrderEntry.SOLineSplit3 soline = PXSelectBase<POOrderEntry.SOLineSplit3, PXSelect<POOrderEntry.SOLineSplit3, Where<POOrderEntry.SOLineSplit3.planID, Equal<Required<POOrderEntry.SOLineSplit3.planID>>>>.Config>.Select(docgraph, demand.PlanID);
                    if (soline != null && soline.POSource.IsIn("D", "L") && soline.IsValidForDropShip != true)
                    {
                        throw new PXException(Messages.DropShipLineInvalid);
                    }
                    bool requireSingleProject = docgraph.apsetup.Current.RequireSingleProjectPerDocument == true;
                    order = FindOrCreatePOOrder(created, order, demand, soorder, soline, requireSingleProject);
                    order.UpdateVendorCost = false;
                    if (order.OrderNbr == null)
                    {
                        docgraph.Clear();
                        order = docgraph.FillPOOrderFromDemand(order, demand, soorder, PurchDate, extSort, branchID);
                    }
                    else if (!docgraph.Document.Cache.ObjectsEqual(docgraph.Document.Current, order))
                    {
                        POOrder pOOrder2 = (docgraph.Document.Current = docgraph.Document.Search<POOrder.orderNbr>(order.OrderNbr, new object[1] { order.OrderType }));
                        order = pOOrder2;
                    }
                    if (!orderedByPlantype.TryGetValue(demand.PlanType, out var ordered))
                    {
                        DocumentList<POLine> documentList2 = (orderedByPlantype[demand.PlanType] = new DocumentList<POLine>(docgraph));
                        ordered = documentList2;
                    }
                    POLine line = FindOrCreatePOLine(docgraph, ordered, order.OrderType, demand, soline);
                    if (line.OrderNbr == null)
                    {
                        docgraph.FillPOLineFromDemand(line, demand, order.OrderType, soline);
                        line = docgraph.Transactions.Insert(line);
                        if (setup.CopyLineNoteSO == true && soline != null)
                        {
                            PXNoteAttribute.SetNote(docgraph.Transactions.Cache, line, PXNoteAttribute.GetNote(docgraph.Caches[typeof(POOrderEntry.SOLineSplit3)], soline));
                        }
                        docgraph.onCopyPOLineFields?.Invoke(demand, line);
                        line = PXCache<POLine>.CreateCopy(line);
                        ordered.Add(line);
                    }
                    else
                    {
                        line = PXSelectBase<POLine, PXSelect<POLine, Where<POLine.orderType, Equal<Current<POOrder.orderType>>, And<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>, And<POLine.lineNbr, Equal<Current<POLine.lineNbr>>>>>>.Config>.SelectSingleBound(docgraph, new object[1] { line });
                        line = PXCache<POLine>.CreateCopy(line);
                        POLine pOLine = line;
                        pOLine.OrderQty += demand.OrderQty;
                    }
                    string replanType = LinkPOLineToBlanket(line, docgraph, demand, soline, ref ErrorLevel, ref ErrorText);
                    line = docgraph.Transactions.Update(line);
                    docgraph.GetExtension<DropShipLinksExt>()?.InsertDropShipLink(line, soline);
                    PXCache cache = docgraph.Caches[typeof(INItemPlan)];
                    CreateSplitDemand(cache, demand);
                    cache.MarkUpdated(demand, assertError: true);
                    demand.SupplyPlanID = line.PlanID;
                    if (replanType != null)
                    {
                        cache.RaiseRowDeleted(demand);
                        demand.PlanType = replanType;
                        cache.RaiseRowInserted(demand);
                    }
                    if (soline != null)
                    {
                        POOrderEntry_Extension graphDocExt = docgraph.GetExtension<POOrderEntry_Extension>();
                        LinkPOLineToSOLineSplit(docgraph, soline, line);
                        graphDocExt.UpdateSOLine(soline, docgraph.Document.Current.VendorID, poCreated: true);
                        docgraph.FixedDemand.Cache.MarkUpdated(soline, assertError: true);
                    }
                    if (!docgraph.Transactions.Cache.IsInsertedUpdatedDeleted)
                    {
                        continue;
                    }
                    using (PXTransactionScope scope = new PXTransactionScope())
                    {
                        docgraph.Save.Press();
                        if (demand.PlanType == "90")
                        {
                            docgraph.Replenihment.Current = docgraph.Replenihment.Search<INReplenishmentOrder.noteID>(demand.RefNoteID, Array.Empty<object>());
                            InsertReplenishmentLine(docgraph, demand, line);
                        }
                        scope.Complete();
                    }

                    if (ErrorLevel == PXErrorLevel.RowInfo)
                    {
                        PXProcessing<POFixedDemand>.SetInfo(PXMessages.LocalizeFormatNoPrefixNLA(Messages.POOrderCreated, docgraph.Document.Current.OrderNbr) + "\r\n" + ErrorText);
                    }
                    else
                    {
                        PXProcessing<POFixedDemand>.SetWarning(PXMessages.LocalizeFormatNoPrefixNLA(Messages.POOrderCreated, docgraph.Document.Current.OrderNbr) + "\r\n" + ErrorText);
                    }
                    if (created.Find(docgraph.Document.Current) == null)
                    {
                        created.Add(docgraph.Document.Current);
                    }
                }
                catch (Exception e)
                {
                    docgraph.Clear();
                    PXProcessing<POFixedDemand>.SetError(e);
                    PXTrace.WriteError(e);
                    hasErrors = true;
                }
            }
            // Copy notes/attachments for all created PO orders after main processing is complete
            foreach (POOrder createdOrder in created)
            {
                try
                {
                    // Reload the order in a fresh graph to avoid cache conflicts
                    var noteGraph = PXGraph.CreateInstance<POOrderEntry>();
                    noteGraph.Document.Current = noteGraph.Document.Search<POOrder.orderNbr>(createdOrder.OrderNbr, new object[] { createdOrder.OrderType });
                    if (noteGraph.Document.Current != null)
                    {
                        CopyNotesFromSOToPO(noteGraph);
                    }
                }
                catch { }
            }

            if (!hasErrors && created.Count == 1)
            {
                using (new PXTimeStampScope(null))
                {
                    docgraph.Clear();
                    docgraph.Document.Current = docgraph.Document.Search<POOrder.orderNbr>(created[0].OrderNbr, new object[1] { created[0].OrderType });
                    return new PXRedirectRequiredException(docgraph, "Purchase Order");
                }
            }
            return null;
        }

        #endregion

        #region FindOrCreatePOLine
        //public delegate POLine FindOrCreatePOLineDelegate(POOrderEntry docgraph, DocumentList<POLine> ordered, string orderType, POFixedDemand demand, POOrderEntry.SOLineSplit3 soline);
        //[PXOverride]
        //public POLine FindOrCreatePOLine(POOrderEntry docgraph, DocumentList<POLine> ordered, string orderType, POFixedDemand demand, POOrderEntry.SOLineSplit3 soline, FindOrCreatePOLineDelegate baseMethod)
        protected virtual POLine FindOrCreatePOLine(POOrderEntry docgraph, DocumentList<POLine> ordered, string orderType, POFixedDemand demand, POOrderEntry.SOLineSplit3 soline)
        {
            POLine line = null;
            POSetup poSetup = docgraph.POSetup.Current;

            // Resolve SPC Code for grouping
            string soOrderType = soline?.OrderType ?? demand.OrderType;
            string soOrderNbr = soline?.OrderNbr ?? demand.OrderNbr;
            int? soLineNbr = soline?.LineNbr ?? demand.LineNbr;

            string spcCode = null;
            if (!string.IsNullOrEmpty(soOrderType) && !string.IsNullOrEmpty(soOrderNbr) && soLineNbr != null)
            {
                SOLine soLine = PXSelect<SOLine,
                    Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                      And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                      And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                    .Select(Base, soOrderType, soOrderNbr, soLineNbr);

                spcCode = soLine?.GetExtension<SOLineExt>()?.UsrSWKSPCCode;
            }

            // Fallback to demand extension (if you write SPC to the demand row earlier)
            if (string.IsNullOrWhiteSpace(spcCode))
            {
                spcCode = demand.GetExtension<POFixedDemandExt>()?.UsrSWKSPCCode;
            }

            if (orderType == POOrderType.RegularOrder && demand.PlanType != INPlanConstants.Plan6B)
            {
                var lineSearchValues = new List<FieldLookup>()
                {
                    new FieldLookup<POLine.vendorID>(demand.VendorID),
                    new FieldLookup<POLine.vendorLocationID>(demand.VendorLocationID),
                    new FieldLookup<POLine.siteID>(demand.POSiteID),
                    new FieldLookup<POLine.inventoryID>(demand.InventoryID),
                    new FieldLookup<POLine.subItemID>(demand.SubItemID),
                    new FieldLookup<POLine.requestedDate>(soline?.ShipDate),
                    new FieldLookup<POLine.projectID>(soline?.ProjectID),
                    new FieldLookup<POLine.taskID>(soline?.TaskID),
                    new FieldLookup<POLine.costCodeID>(soline?.CostCodeID),
                    new FieldLookup<POLine.costCenterID>(demand.CostCenterID),
                };

                // ALWAYS add SPC Code discriminator to ensure lines with different SPC codes (including null) never merge
                // This prevents merging of lines with SPC Cost/Code and lines without SPC Cost/Code
                lineSearchValues.Add(new FieldLookup<POLineExt.usrSWKSPCCode>(spcCode));

                if (poSetup.CopyLineDescrSO == true && soline != null)
                {
                    lineSearchValues.Add(new FieldLookup<POLine.tranDesc>(soline.TranDesc));
                    line = ordered.Find(lineSearchValues.ToArray());
                    if (line != null && poSetup.CopyLineNoteSO == true &&
                        (PXNoteAttribute.GetNote(docgraph.Caches[typeof(POLine)], line) != null ||
                         PXNoteAttribute.GetNote(docgraph.Caches[typeof(POOrderEntry.SOLineSplit3)], soline) != null))
                    {
                        line = null;
                    }
                }
                else
                {
                    line = ordered.Find(lineSearchValues.ToArray());
                }
            }

            return line ?? new POLine();
        }

        #endregion

        #region Replicated Methods
        protected virtual string GetSpecialOrderCurrencyError(POFixedDemand row, bool onlyErrors)
        {
            if (row.CuryID == null)
            {
                return null;
            }
            Vendor vendor = Vendor.PK.Find(Base, row.VendorID);
            string curyID = vendor?.CuryID;
            if (curyID == null)
            {
                curyID = GetBaseCuryID((PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>() ? Base.Filter.Current.BranchID : null) ?? Base.Accessinfo.BranchID);
            }
            string message = null;
            if (curyID != row.CuryID)
            {
                if (vendor == null || vendor.AllowOverrideCury != true)
                {
                    message = Messages.SpecialOrderCurrencyDiffNoOverride;
                }
                else if (!onlyErrors)
                {
                    message = Messages.SpecialOrderCurrencyDiffWillCreate;
                }
            }
            return message;
        }

        protected virtual string GetBaseCuryID(int? branchID)
        {
            return ServiceLocator.Current.GetInstance<Func<PXGraph, IPXCurrencyService>>()(Base).BaseCuryID(branchID);
        }

        protected virtual POOrder FindOrCreatePOOrder(DocumentList<POOrder> created, POOrder previousOrder, POFixedDemand demand, SOOrder soorder, POOrderEntry.SOLineSplit3 soline, bool requireSingleProject)
        {
            string OrderType = (demand.PlanType.IsIn("6D", "6E") ? "DP" : "RO");
            bool linkToBlanket = demand.PlanType == "6B" || demand.PlanType == "6E";
            List<FieldLookup> orderSearchValues = new List<FieldLookup>
        {
            new FieldLookup<POOrder.orderType>(OrderType),
            new FieldLookup<POOrder.vendorID>(demand.VendorID),
            new FieldLookup<POOrder.vendorLocationID>(demand.VendorLocationID),
            new FieldLookup<POOrder.bLOrderNbr>(linkToBlanket ? soline.PONbr : null)
        };
            if (OrderType == "RO")
            {
                if (requireSingleProject)
                {
                    int? project = demand.DemandProjectID ?? ProjectDefaultAttribute.NonProject();
                    orderSearchValues.Add(new FieldLookup<POOrder.projectID>(project));
                }
                if (previousOrder == null || !(previousOrder.ShipDestType == "L") || previousOrder.SiteID.HasValue)
                {
                    orderSearchValues.Add(new FieldLookup<POOrder.siteID>(demand.POSiteID));
                }
            }
            else if (OrderType == "DP")
            {
                orderSearchValues.Add(new FieldLookup<POOrder.sOOrderType>(soline.OrderType));
                orderSearchValues.Add(new FieldLookup<POOrder.sOOrderNbr>(soline.OrderNbr));
            }
            else
            {
                orderSearchValues.Add(new FieldLookup<POOrder.shipToBAccountID>(soorder.CustomerID));
                orderSearchValues.Add(new FieldLookup<POOrder.shipToLocationID>(soorder.CustomerLocationID));
                orderSearchValues.Add(new FieldLookup<POOrder.siteID>(demand.POSiteID));
            }
            if (demand.IsSpecialOrder == true)
            {
                orderSearchValues.Add(new FieldLookup<POOrder.curyID>(demand.CuryID));
            }
            return created.Find(orderSearchValues.ToArray()) ?? new POOrder
            {
                OrderType = OrderType,
                BLType = (linkToBlanket ? "BL" : null),
                BLOrderNbr = (linkToBlanket ? soline.PONbr : null)
            };
        }

        protected virtual string LinkPOLineToBlanket(POLine line, POOrderEntry docgraph, POFixedDemand demand, POOrderEntry.SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref string ErrorText)
        {

            var demandExt = demand.GetExtension<POFixedDemandExt>();
            //demand.VendorID = demandExt?.UsrVendorID ?? demand.VendorID;
            //demand.VendorLocationID = demandExt?.UsrVendorLocationID ?? demand.VendorLocationID;

            SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                  And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                                  .Select(Base, soline?.OrderType, soline?.OrderNbr, soline?.LineNbr);
            // Fallback: for regular special orders soline (split) can be null; use demand keys
            if (soLine == null && demand?.OrderType != null && demand?.OrderNbr != null && demand?.LineNbr != null)
            {
                soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>, And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                .Select(Base, demand.OrderType, demand.OrderNbr, demand.LineNbr);
            }
            SOLineExt soLineExt = soLine?.GetExtension<SOLineExt>();
            POLineExt poLineExt = line?.GetExtension<POLineExt>();
            POOrderExt poOrderExt = docgraph?.CurrentDocument?.Current?.GetExtension<POOrderExt>();
            SOOrder soOrder = SOOrder.PK.Find(Base, soline?.OrderType, soline?.OrderNbr);
            InventoryItem item = InventoryItem.PK.Find(Base, demand?.InventoryID);
            InventoryItemExt itemExt = item?.GetExtension<InventoryItemExt>();
            SOOrderExt soOrderExt = soOrder?.GetExtension<SOOrderExt>();
            if (soLine != null)
            {
                if (poLineExt != null)
                {
                    poLineExt.UsrVendorSpecTerms = soLineExt?.UsrVendorSpecTerms;
                    poLineExt.UsrVendorNotes = soLineExt?.UsrVendorNotes;
                }

                //SOOrder soOrder = SOOrder.PK.Find(Base, soline.OrderType, soline.OrderNbr);

                docgraph.CurrentDocument.Current.FOBPoint = soOrder?.FOBPoint;
                docgraph.CurrentDocument.Current.ShipVia = soOrder?.ShipVia;

                poOrderExt.UsrShipTermsID = soOrder?.ShipTermsID;
                poOrderExt.UsrCustomerAccount = soOrderExt?.UsrCustomerAccount;
            }



            SOOrderType orderType = SOOrderType.PK.Find(Base, soOrder?.OrderType);
            SOOrderTypeExt typeExt = orderType?.GetExtension<SOOrderTypeExt>();
            if (poLineExt != null)
            {
                if (typeExt?.UsrShowVendorID ?? false)
                    poLineExt.UsrVendorID = soLineExt?.UsrVendorID;

                if (typeExt?.UsrShowVendorLocationID ?? false)
                    poLineExt.UsrVendorLocationID = soLineExt?.UsrVendorLocationID;

                if (typeExt?.UsrShowVendorAddress ?? false)
                    poLineExt.UsrVendorAddress = soLineExt?.UsrVendorAddress;

                poLineExt.UsrItemSpecs = soLineExt?.UsrItemSpecs ?? itemExt.UsrItemSpecs;

            }

            //POOrderExt poOrderExt = docgraph.CurrentDocument.Current.GetExtension<POOrderExt>();
            if (poOrderExt != null && demand.PlanType == INPlanConstants.Plan6D)
                poOrderExt.UsrCustomerOrderNbr = soOrder?.CustomerOrderNbr;

            #region orig
            string replanType = null;
            if (demand.PlanType == "6B" || demand.PlanType == "6E")
            {
                replanType = ((demand.PlanType == "6B") ? "66" : "6D");
                demand.FixedSource = "P";
                line.POType = soline.POType;
                line.PONbr = soline.PONbr;
                line.POLineNbr = soline.POLineNbr;
                POLine blanket_line = PXSelectBase<POLine, PXSelect<POLine, Where<POLine.orderType, Equal<Current<POLine.pOType>>, And<POLine.orderNbr, Equal<Current<POLine.pONbr>>, And<POLine.lineNbr, Equal<Current<POLine.pOLineNbr>>>>>>.Config>.SelectSingleBound(docgraph, new object[1] { line });
                if (blanket_line != null)
                {
                    if (demand.PlanQty > blanket_line.BaseOpenQty)
                    {
                        line.OrderQty -= demand.OrderQty;
                        if (string.Equals(line.UOM, blanket_line.UOM))
                        {
                            line.OrderQty += blanket_line.OpenQty;
                        }
                        else
                        {
                            PXDBQuantityAttribute.CalcBaseQty<POLine.orderQty>(docgraph.Transactions.Cache, line);
                            line.BaseOrderQty += blanket_line.BaseOpenQty;
                            PXDBQuantityAttribute.CalcTranQty<POLine.orderQty>(docgraph.Transactions.Cache, line);
                        }
                        ErrorLevel = PXErrorLevel.RowWarning;
                        ErrorText += PXMessages.LocalizeFormatNoPrefixNLA(Messages.POOrderCreated, line.PONbr);
                    }
                    line.CuryUnitCost = blanket_line.CuryUnitCost;
                    line.UnitCost = blanket_line.UnitCost;
                }
            }
            #endregion

            var result = replanType;//baseMethod(line, docgraph, demand, soline, ref ErrorLevel, ref ErrorText);
            // Ensure Unit Cost defaulting is handled only in POLine_CuryUnitCost_FieldDefaulting
            // Do not toggle skipCostDefaulting here.

            // this only runs when the plantype is dropship
            // Set Shipping Instructions from SO Order to PO Order
            if (demand.PlanDate != null && demand.PlanType == INPlanConstants.Plan6D)
                docgraph?.CurrentDocument.Cache.SetValueExt<POOrderExt.usrShippingInstructions>(docgraph.CurrentDocument.Current, soOrderExt?.UsrShippingInstructions);

            // Maintain existing behavior for SPC Code & Ext Cost population (not unit cost)
            try
            {
                POLineExt lineExt = line?.GetExtension<POLineExt>();
                bool assignedCost = false;
                if (lineExt != null) lineExt.UsrUsedVendorPrice = false;

                Func<decimal, decimal> round = v =>
                {
                    int prec = 2;
                    try
                    {
                        var ci = docgraph.FindImplementation<IPXCurrencyHelper>()?.GetCurrencyInfo(docgraph.Document.Current?.CuryInfoID);
                        prec = ci?.GetCM()?.CuryPrecision ?? 2;
                    }
                    catch { }
                    return Math.Round(v, prec, MidpointRounding.AwayFromZero);
                };

                // Priority i: SPC Cost from SO line (highest, override all if >0)
                decimal? spcCostFirst = soLineExt?.UsrSWKSPCCost;
                if (spcCostFirst.HasValue && spcCostFirst.Value > 0m)
                {
                    docgraph.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, round(spcCostFirst.Value));
                    line.CuryUnitCost = round(spcCostFirst.Value);
                    if (lineExt != null) lineExt.UsrUsedVendorPrice = true; // treat as protected from RTH bump
                    assignedCost = true;
                }

                // Priority ii: Active Vendor Price (next if SPC not applied)
                if (!assignedCost)
                {
                    decimal? vendorPrice = null;
                    if (soOrder != null && line.InventoryID != null && line.UOM != null && docgraph.Document.Current?.VendorID != null)
                    {
                        var ci = docgraph.FindImplementation<IPXCurrencyHelper>()?.GetCurrencyInfo(docgraph.Document.Current.CuryInfoID);
                        vendorPrice = APVendorPriceMaint.CalculateUnitCost(
                            docgraph.Transactions.Cache,
                            docgraph.Document.Current.VendorID,
                            docgraph.Document.Current.VendorLocationID,
                            line.InventoryID,
                            line.SiteID,
                            ci?.GetCM(),
                            line.UOM,
                            line.OrderQty,
                            docgraph.Document.Current.OrderDate ?? docgraph.Accessinfo.BusinessDate.GetValueOrDefault(),
                            line.CuryUnitCost);
                    }
                    if (vendorPrice.HasValue && vendorPrice.Value > 0m)
                    {
                        docgraph.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, round(vendorPrice.Value));
                        line.CuryUnitCost = round(vendorPrice.Value);
                        if (lineExt != null) lineExt.UsrUsedVendorPrice = true;
                        assignedCost = true;
                    }
                }

                // Priority iii: RTH Cost (SO line, then item, then demand) if neither SPC nor Vendor applied
                if (!assignedCost)
                {
                    decimal? rthCost = soLineExt?.UsrSWKRTHCost;
                    if (!(rthCost > 0m)) rthCost = itemExt?.UsrSWKRTHCost;
                    if (!(rthCost > 0m)) rthCost = demandExt?.UsrSWKRTHCost;
                    if (rthCost > 0m)
                    {
                        docgraph.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, round(rthCost.Value));
                        line.CuryUnitCost = round(rthCost.Value);
                        if (lineExt != null) lineExt.UsrUsedVendorPrice = false;
                        assignedCost = true;
                    }
                }

                // Priority iv: Last Cost fallback (leave as defaulted earlier if still unassigned)

                // Failsafe: ensure non-negative unit cost
                if ((line.CuryUnitCost ?? 0m) < 0m)
                {
                    docgraph.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, 0m);
                    line.CuryUnitCost = 0m;
                }

                // Recalculate Ext Cost from formula (Unit * Qty)
                decimal qty = line.OrderQty ?? 0m;
                decimal unit = line.CuryUnitCost ?? 0m;
                decimal expectedExt = round(unit * qty);
                decimal currentExt = line.CuryExtCost ?? 0m;
                if (Math.Abs(currentExt - expectedExt) > 0.009m || line.CuryExtCost == null)
                {
                    docgraph.Transactions.Cache.SetValueExt<POLine.curyExtCost>(line, expectedExt);
                    docgraph.Transactions.Cache.SetValueExt<POLine.curyLineAmt>(line, expectedExt);
                }

                // Enforce RTH minimum when vendor/SPC not used
                if (lineExt != null && lineExt.UsrUsedVendorPrice != true)
                {
                    decimal rthUnit = soLineExt?.UsrSWKRTHCost ?? itemExt?.UsrSWKRTHCost ?? demandExt?.UsrSWKRTHCost ?? 0m;
                    if (rthUnit > 0m)
                    {
                        decimal minExt = round(rthUnit * qty);
                        decimal newExt = line.CuryExtCost ?? 0m;
                        if (newExt + 0.009m < minExt)
                        {
                            // Raise warning and bump
                            docgraph.Transactions.Cache.RaiseExceptionHandling<POLine.curyExtCost>(line, newExt,
                                new PXSetPropertyException(POCreateReplaceOriginalMethod.Messages.POOrderCreated, PXErrorLevel.Warning, docgraph.Document.Current?.OrderNbr));
                            docgraph.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, round(rthUnit));
                            line.CuryUnitCost = round(rthUnit);
                            docgraph.Transactions.Cache.SetValueExt<POLine.curyExtCost>(line, minExt);
                            docgraph.Transactions.Cache.SetValueExt<POLine.curyLineAmt>(line, minExt);
                        }
                    }
                }

                // Ensure SPC Code is set on the PO line
                if (soLineExt != null && soLineExt?.UsrSWKSPCCode != null && lineExt != null)
                {
                    docgraph?.Transactions.Cache.SetValueExt<POLineExt.usrSWKSPCCode>(line, soLineExt?.UsrSWKSPCCode);
                }
            }
            catch
            {
                // swallow - non-critical mapping
            }

            return result;
        }

        protected virtual void CreateSplitDemand(PXCache cache, POFixedDemand demand)
        {
            if (!(demand.PlanType != "90") && !(demand.OrderQty == demand.PlanUnitQty))
            {
                INItemPlan orig_demand = PXSelectBase<INItemPlan, PXSelectReadonly<INItemPlan, Where<INItemPlan.planID, Equal<Current<INItemPlan.planID>>>>.Config>.SelectSingleBound(cache.Graph, new object[1] { demand });
                INItemPlan split = PXCache<INItemPlan>.CreateCopy(orig_demand);
                split.PlanID = null;
                split.PlanQty = demand.PlanUnitQty - demand.OrderQty;
                if (demand.UnitMultDiv == "M")
                {
                    split.PlanQty *= demand.UnitRate;
                }
                else
                {
                    split.PlanQty /= demand.UnitRate;
                }
                cache.Insert(split);
                cache.RaiseRowDeleted(demand);
                demand.PlanQty = orig_demand.PlanQty - split.PlanQty;
                cache.RaiseRowInserted(demand);
            }
        }
        protected virtual void LinkPOLineToSOLineSplit(POOrderEntry docgraph, POOrderEntry.SOLineSplit3 soline, POLine line)
        {
            soline.POType = line.OrderType;
            soline.PONbr = line.OrderNbr;
            soline.POLineNbr = line.LineNbr;
            soline.RefNoteID = docgraph.Document.Current.NoteID;
            string targetPOSource = ((soline.POSource == "L") ? "D" : ((soline.POSource == "B") ? "O" : null));
            if (targetPOSource != null)
            {
                soline.POSource = targetPOSource;
                POOrderEntry.SOLine5 origsoline = docgraph.FixedDemandOrigSOLine.Select(soline.OrderType, soline.OrderNbr, soline.LineNbr);
                if (origsoline != null)
                {
                    origsoline.POSource = targetPOSource;
                    docgraph.FixedDemandOrigSOLine.Cache.MarkUpdated(origsoline, assertError: true);
                }
            }
        }


        protected virtual void InsertReplenishmentLine(POOrderEntry docgraph, POFixedDemand demand, POLine line)
        {
            if (docgraph.Replenihment.Current != null)
            {
                INReplenishmentLine rLine = PXCache<INReplenishmentLine>.CreateCopy(docgraph.ReplenishmentLines.Insert(new INReplenishmentLine()));
                rLine.InventoryID = line.InventoryID;
                rLine.SubItemID = line.SubItemID;
                rLine.UOM = line.UOM;
                rLine.VendorID = line.VendorID;
                rLine.VendorLocationID = line.VendorLocationID;
                rLine.Qty = line.OrderQty;
                rLine.POType = line.OrderType;
                rLine.PONbr = docgraph.Document.Current.OrderNbr;
                rLine.POLineNbr = line.LineNbr;
                rLine.SiteID = demand.POSiteID;
                rLine.PlanID = demand.PlanID;
                docgraph.ReplenishmentLines.Update(rLine);
                docgraph.Caches[typeof(INItemPlan)].Delete(demand);
                docgraph.Save.Press();
            }
        }

        private void CopyNotesFromSOToPO(POOrderEntry graph)
        {
            var order = graph.Document.Current;
            if (order == null) return;
            SOSetup sosetup = PXSelect<SOSetup>.Select(graph);
            var setupExt = sosetup?.GetExtension<SOSetupExt>();
            if (setupExt == null) return;
            var orderCache = graph.Caches[typeof(POOrder)];
            var lineCache = graph.Caches[typeof(POLine)];
            var soOrderCache = graph.Caches[typeof(SOOrder)];
            var soLineCache = graph.Caches[typeof(SOLine)];

            // Ensure NoteID exists for header
            PXNoteAttribute.GetNoteID<POOrder.noteID>(orderCache, order);

            // Collect all SO sources for header notes (group by SO Order)
            var soOrdersForHeader = new Dictionary<string, SOOrder>(); // key: OrderType|OrderNbr
            var soLinesForPOLines = new Dictionary<int?, List<(SOLine soLine, SOOrder soOrder)>>(); // key: POLine.LineNbr

            foreach (POLine line in PXSelect<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>, And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>>>
              .Select(graph, order.OrderType, order.OrderNbr).RowCast<POLine>())
            {
                PXNoteAttribute.GetNoteID<POLine.noteID>(lineCache, line);

                SOLine soLine; SOOrder soOrder;
                if (!TryGetLinkedSOLine(graph, line, out soLine, out soOrder) || soOrder == null)
                    continue;

                string soKey = $"{soOrder.OrderType}|{soOrder.OrderNbr}";
                if (!soOrdersForHeader.ContainsKey(soKey))
                    soOrdersForHeader[soKey] = soOrder;

                if (!soLinesForPOLines.ContainsKey(line.LineNbr))
                    soLinesForPOLines[line.LineNbr] = new List<(SOLine, SOOrder)>();
                soLinesForPOLines[line.LineNbr].Add((soLine, soOrder));
            }

            // Process Header Notes
            if (soOrdersForHeader.Count > 0)
            {
                if (setupExt.UsrCopyHeaderNotesToPO == true)
                {
                    var soOrders = soOrdersForHeader.Values.ToList();
                    if (soOrders.Count == 1)
                    {
                        // Single SO Order - copy note directly
                        string noteText = PXNoteAttribute.GetNote(soOrderCache, soOrders[0]);
                        if (!string.IsNullOrEmpty(noteText))
                        {
                            PXNoteAttribute.SetNote(orderCache, order, noteText);
                            orderCache.MarkUpdated(order);
                        }
                    }
                    else
                    {
                        // Multiple SO Orders - concatenate with bullets and order info
                        var combinedNote = new System.Text.StringBuilder();
                        foreach (var soOrder in soOrders)
                        {
                            string noteText = PXNoteAttribute.GetNote(soOrderCache, soOrder);
                            if (!string.IsNullOrEmpty(noteText))
                            {
                                combinedNote.AppendLine($"• SO {soOrder.OrderType} {soOrder.OrderNbr}:");
                                combinedNote.AppendLine(noteText);
                                combinedNote.AppendLine();
                            }
                        }
                        if (combinedNote.Length > 0)
                        {
                            PXNoteAttribute.SetNote(orderCache, order, combinedNote.ToString().TrimEnd());
                            orderCache.MarkUpdated(order);
                        }
                    }
                }

                if (setupExt.UsrCopyHeaderAttachmentsToPO == true)
                {
                    // Copy attachments from all SO orders (always add as separate files)
                    foreach (var soOrder in soOrdersForHeader.Values)
                    {
                        PXNoteAttribute.CopyNoteAndFiles(soOrderCache, soOrder, orderCache, order, false, true);
                        orderCache.MarkUpdated(order);
                    }
                }
            }

            // Process Line Notes
            foreach (var kvp in soLinesForPOLines)
            {
                int? poLineNbr = kvp.Key;
                var soLineSources = kvp.Value;

                POLine poLine = PXSelect<POLine,
          Where<POLine.orderType, Equal<Required<POLine.orderType>>,
                 And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
              And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>
             .Select(graph, order.OrderType, order.OrderNbr, poLineNbr);

                if (poLine == null) continue;

                // Line Notes
                if (setupExt.UsrCopyLineNotesToPO == true)
                {
                    string destNote = PXNoteAttribute.GetNote(lineCache, poLine);
                    if (string.IsNullOrWhiteSpace(destNote))
                    {
                        if (soLineSources.Count == 1)
                        {
                            // Single SO Line - copy note directly
                            var soLine = soLineSources[0].soLine;
                            PXNoteAttribute.CopyNoteAndFiles(soLineCache, soLine, lineCache, poLine, true, false);
                            lineCache.MarkUpdated(poLine);
                        }
                        else
                        {
                            // Multiple SO Lines - concatenate with bullets and order info
                            var combinedNote = new System.Text.StringBuilder();
                            foreach (var (soLine, soOrder) in soLineSources)
                            {
                                string noteText = PXNoteAttribute.GetNote(soLineCache, soLine);
                                if (!string.IsNullOrEmpty(noteText))
                                {
                                    combinedNote.AppendLine($"• SO {soOrder.OrderType} {soOrder.OrderNbr} Line {soLine.LineNbr}:");
                                    combinedNote.AppendLine(noteText);
                                    combinedNote.AppendLine();
                                }
                            }
                            if (combinedNote.Length > 0)
                            {
                                PXNoteAttribute.SetNote(lineCache, poLine, combinedNote.ToString().TrimEnd());
                                lineCache.MarkUpdated(poLine);
                            }
                        }
                    }
                }

                // Line Attachments
                if (setupExt.UsrCopyLineAttachmentsToPO == true)
                {
                    bool lineHasFiles = (PXNoteAttribute.GetFileNotes(lineCache, poLine)?.Any() ?? false);
                    if (!lineHasFiles)
                    {
                        // Copy attachments from all SO lines (always add as separate files)
                        foreach (var (soLine, soOrder) in soLineSources)
                        {
                            PXNoteAttribute.CopyNoteAndFiles(soLineCache, soLine, lineCache, poLine, false, true);
                            lineCache.MarkUpdated(poLine);
                        }
                    }
                }
            }

            // Persist via normal save to ensure proper events run
            try
            {
                graph.Actions.PressSave();
            }
            catch
            {
                // fallback direct persist if PressSave blocked
                orderCache.Persist(PXDBOperation.Update);
                lineCache.Persist(PXDBOperation.Update);
            }
        }

        private bool TryGetLinkedSOLine(POOrderEntry graph, POLine poLine, out SOLine soLine, out SOOrder soOrder)
        {
            soLine = null; soOrder = null;
            if (poLine == null) return false;
            if (POLineType.IsDropShip(poLine.LineType))
            {
                DropShipLink ds = PXSelect<DropShipLink,
                        Where<DropShipLink.pOOrderType, Equal<Required<DropShipLink.pOOrderType>>,
                  And<DropShipLink.pOOrderNbr, Equal<Required<DropShipLink.pOOrderNbr>>,
             And<DropShipLink.pOLineNbr, Equal<Required<DropShipLink.pOLineNbr>>>>>>
                  .Select(graph, poLine.OrderType, poLine.OrderNbr, poLine.LineNbr);
                if (ds != null)
                {
                    soLine = PXSelect<SOLine,
                           Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                         And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                        And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                        .Select(graph, ds.SOOrderType, ds.SOOrderNbr, ds.SOLineNbr);
                    if (soLine != null)
                        soOrder = PXSelect<SOOrder,
                           Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                               And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(graph, soLine.OrderType, soLine.OrderNbr);
                }
            }
            if (soLine == null)
            {
                PXResult<SOLine, SOLineSplit> soRes = (PXResult<SOLine, SOLineSplit>)PXSelectJoin<SOLine, InnerJoin<SOLineSplit,
               On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
              Where<SOLineSplit.pOType, Equal<Required<SOLineSplit.pOType>>,
                        And<SOLineSplit.pONbr, Equal<Required<SOLineSplit.pONbr>>,
              And<SOLineSplit.pOLineNbr, Equal<Required<SOLineSplit.pOLineNbr>>>>>>
                 .Select(graph, poLine.OrderType, poLine.OrderNbr, poLine.LineNbr);
                if (soRes != null)
                {
                    soLine = soRes;
                    if (soLine != null)
                    {
                        soOrder = PXSelect<SOOrder, Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>, And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>.Select(graph, soLine.OrderType, soLine.OrderNbr);
                    }
                }
            }
            return soLine != null && soOrder != null;
        }

        #endregion
    }
}
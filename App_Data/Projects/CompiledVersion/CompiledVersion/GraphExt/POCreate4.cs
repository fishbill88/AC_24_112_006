using CompiledVersion.DAC;
using CompiledVersion.Graphs;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using System.CodeDom;
using System.Collections.Generic;
using static PX.Objects.CA.PXModule;
using static PX.Objects.PO.POOrderEntry;

namespace CompiledVersion
{
    public class POCreateExt : PXGraphExtension<POCreate>
    {
        public PXSelect<BAccountR> BAccount;
        public static bool IsActive() => true;

        #region Event Handlers
        #region LinkPOLineToBlanket


        #region LinkPOLineToBlanketDelegate - unused
        //public delegate String LinkPOLineToBlanketDelegate(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText);
        //[PXOverride]
        //public String LinkPOLineToBlanket(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText, LinkPOLineToBlanketDelegate baseMethod)
        //{
        //    var demandExt = demand.GetExtension<POFixedDemandExt>();
        //    //demand.VendorID = demandExt?.UsrVendorID ?? demand.VendorID;
        //    //demand.VendorLocationID = demandExt?.UsrVendorLocationID ?? demand.VendorLocationID;

        //    SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
        //          And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
        //                          .Select(Base, soline?.OrderType, soline?.OrderNbr, soline?.LineNbr);
        //    SOLineExt soLineExt = soLine?.GetExtension<SOLineExt>();
        //    POLineExt poLineExt = line?.GetExtension<POLineExt>();
        //    POOrderExt poOrderExt = docgraph?.CurrentDocument?.Current?.GetExtension<POOrderExt>();
        //    SOOrder soOrder = SOOrder.PK.Find(Base, soline?.OrderType, soline?.OrderNbr);
        //    InventoryItem item = InventoryItem.PK.Find(Base, demand?.InventoryID);
        //    InventoryItemExt itemExt = item?.GetExtension<InventoryItemExt>();
        //    SOOrderExt soOrderExt = soOrder?.GetExtension<SOOrderExt>();
        //    if (soLine != null)
        //    {
        //        POLineExt lineExt = line?.GetExtension<POLineExt>();
        //        lineExt.UsrVendorSpecTerms = soLineExt?.UsrVendorSpecTerms;
        //        lineExt.UsrVendorNotes = soLineExt?.UsrVendorNotes;

        //        //SOOrder soOrder = SOOrder.PK.Find(Base, soline.OrderType, soline.OrderNbr);

        //        docgraph.CurrentDocument.Current.FOBPoint = soOrder?.FOBPoint;
        //        docgraph.CurrentDocument.Current.ShipVia = soOrder?.ShipVia;


        //        poOrderExt.UsrShipTermsID = soOrder?.ShipTermsID;
        //        poOrderExt.UsrCustomerAccount = soOrderExt?.UsrCustomerAccount;
        //    }



        //    SOOrderType orderType = SOOrderType.PK.Find(Base, soOrder?.OrderType);
        //    SOOrderTypeExt typeExt = orderType?.GetExtension<SOOrderTypeExt>();
        //    if (typeExt?.UsrShowVendorID ?? false)
        //        poLineExt.UsrVendorID = soLineExt?.UsrVendorID;

        //    if (typeExt?.UsrShowVendorLocationID ?? false)
        //        poLineExt.UsrVendorLocationID = soLineExt?.UsrVendorLocationID;

        //    if (typeExt?.UsrShowVendorAddress ?? false)
        //        poLineExt.UsrVendorAddress = soLineExt?.UsrVendorAddress;

        //    poLineExt.UsrItemSpecs = soLineExt?.UsrItemSpecs ?? itemExt.UsrItemSpecs;

        //    //POOrderExt poOrderExt = docgraph.CurrentDocument.Current.GetExtension<POOrderExt>();
        //    poOrderExt.UsrCustomerOrderNbr = soOrder?.CustomerOrderNbr;

        //    var result = baseMethod(line, docgraph, demand, soline, ref ErrorLevel, ref ErrorText);
        //    //POLineExt poLineExt = line?.GetExtension<POLineExt>();
        //    var docGraphExt = docgraph?.GetExtension<POOrderEntry_Extension>();
        //    if (docGraphExt != null)
        //        docGraphExt.skipCostDefaulting = true;

        //    // this only runs when the plantype is dropship
        //    // Set Shipping Instructions from SO Order to PO Order
        //    if(demand.PlanDate != null && demand.PlanType == INPlanConstants.Plan6D)
        //        docgraph?.CurrentDocument.Cache.SetValueExt<POOrderExt.usrShippingInstructions>(docgraph.CurrentDocument.Current, soOrderExt?.UsrShippingInstructions);
        //    // Find the related SO Line
        //    //SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
        //    //    And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
        //    //    .Select(Base, soline?.OrderType, soline?.OrderNbr, soline?.LineNbr);

        //    //SOLineExt soLineExt = soLine?.GetExtension<SOLineExt>();

        //    // Overwrite PO Line Unit Cost if SOLine.usrSWKSPCCost has value
        //    if(soLine != null)
        //    {
        //        if (soLineExt != null && (soLineExt?.UsrSWKSPCCost ?? 0m) > 0m)
        //        {
        //            //line.CuryUnitCost = soLineExt.UsrSWKSPCCost;
        //            docgraph?.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, soLineExt?.UsrSWKSPCCost);
        //        }
        //        else
        //        {
        //            //line.CuryUnitCost = demand.EffPrice;
        //            docgraph?.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, soLine?.CuryUnitCost);
        //            //docgraph?.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, demand.EffPrice);
        //        }
        //    }
        //    else
        //    {
        //        //line.CuryUnitCost = demand.EffPrice;
        //        docgraph?.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, (demandExt.UsrSWKRTHCost ?? demand.EffPrice));
        //    }

        //    if (soLineExt != null && soLineExt?.UsrSWKSPCCode != null && poLineExt != null)
        //    {
        //        //poLineExt.UsrSWKSPCCode = soLineExt.UsrSWKSPCCode;
        //        docgraph?.Transactions.Cache.SetValueExt<POLineExt.usrSWKSPCCode>(line, soLineExt?.UsrSWKSPCCode);
        //    }


        //    return result;
        //}

        #endregion
        #endregion
        // Override the EnumerateAndPrepareFixedDemandRow method to populate Vendor Price
        [PXOverride]
        public virtual void EnumerateAndPrepareFixedDemandRow(PXResult<POFixedDemand> rec,
            System.Action<PXResult<POFixedDemand>> baseMethod)
        {
            // Call the base method first
            baseMethod(rec);

            var demand = (POFixedDemand)rec;
            POFixedDemandExt demandExt = demand.GetExtension<POFixedDemandExt>();

            // Set our custom Vendor Price logic
            decimal? customVendorPrice = CalculateVendorPriceFromPlanType(demand);
            if (customVendorPrice != null)
            {
                demand.EffPrice = customVendorPrice;
            }

            SOLine sOLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                  And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                                  .Select(Base, demand.OrderType, demand.OrderNbr, demand.LineNbr);

            SOLineExt soLineExt = sOLine?.GetExtension<SOLineExt>();
            if(sOLine == null || soLineExt == null)
                return;
            if (soLineExt?.UsrVendorID != null && soLineExt?.UsrVendorLocationID != null)
            {
                demandExt.UsrVendorID = soLineExt?.UsrVendorID;
                demandExt.UsrVendorLocationID = soLineExt?.UsrVendorLocationID;
                demandExt.UsrVendorAddress = soLineExt?.UsrVendorAddress;
                //Base.FixedDemand.Cache.SetValueExt<POFixedDemand.vendorID>(demand, soLineExt?.UsrVendorID);
                //demand.VendorID = soLineExt?.UsrVendorID;
                //demand.VendorLocationID = soLineExt?.UsrVendorLocationID;
            }
            else
            {
                demandExt.UsrVendorID = demand.VendorID;
                demandExt.UsrVendorLocationID = demand.VendorLocationID;
                //demandExt.UsrVendorAddress = demand.VendorAddress;
            }

            if(soLineExt.UsrSWKSPCCode != null)
            {
                var lineExt = demand.GetExtension<POFixedDemandExt>();
                lineExt.UsrSWKSPCCode = soLineExt.UsrSWKSPCCode;
            }   
        }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Vendor Price", Enabled = true)]
        protected virtual void _(Events.CacheAttached<POFixedDemand.effPrice> e) { }

        protected virtual void _(Events.FieldUpdated<POFixedDemand, POFixedDemand.effPrice> e)
        {
            if (e.Row == null) return;

            // Recalculate Extended Cost when Vendor Price is manually changed
            POFixedDemand demand = e.Row;
            if (demand.OrderQty != null && demand.EffPrice != null)
            {
                demand.ExtCost = demand.OrderQty * demand.EffPrice;
                Base.FixedDemand.Cache.RaiseFieldUpdated<POFixedDemand.extCost>(demand, null);
            }
        }

        protected virtual void _(Events.RowSelecting<POFixedDemand> e)
        {
            if (e.Row == null) return;

            using (new PXConnectionScope())
            {
                InventoryItem item = InventoryItem.PK.Find(Base, e.Row.InventoryID);
                POFixedDemandExt ext = e.Row.GetExtension<POFixedDemandExt>();
                InventoryItemExt itemExt = item?.GetExtension<InventoryItemExt>();
                if (ext != null && itemExt != null)
                {
                    // Populate RTH Cost from InventoryItemExt
                    ext.UsrSWKRTHCost = itemExt.UsrSWKRTHCost;
                }
            }
        }
        protected virtual void _(Events.RowSelected<POFixedDemand> e)
        {
            if (e.Row == null) return;

            // Enable EffPrice for the selected row
            PXUIFieldAttribute.SetEnabled<POFixedDemand.effPrice>(e.Cache, e.Row, true);
            PXUIFieldAttribute.SetEnabled<POFixedDemandExt.usrVendorID>(e.Cache, e.Row, true);
            PXUIFieldAttribute.SetEnabled<POFixedDemandExt.usrVendorLocationID>(e.Cache, e.Row, true);
        }


        public delegate POOrder FindOrCreatePOOrderDelegate(DocumentList<POOrder> created, POOrder previousOrder, POFixedDemand demand, SOOrder soorder, SOLineSplit3 soline, Boolean requireSingleProject);
        [PXOverride]
        public POOrder FindOrCreatePOOrder(DocumentList<POOrder> created, POOrder previousOrder, POFixedDemand demand, SOOrder soorder, SOLineSplit3 soline, Boolean requireSingleProject, FindOrCreatePOOrderDelegate baseMethod)
        {

            string OrderType = demand.PlanType.IsIn(INPlanConstants.Plan6D, INPlanConstants.Plan6E) ? POOrderType.DropShip : POOrderType.RegularOrder;
            bool linkToBlanket = demand.PlanType == INPlanConstants.Plan6B || demand.PlanType == INPlanConstants.Plan6E;

            POFixedDemandExt demandExt = demand.GetExtension<POFixedDemandExt>();
            var orderSearchValues = new List<FieldLookup>()
            {
                new FieldLookup<POOrder.orderType>(OrderType),
                new FieldLookup<POOrder.vendorID>(demand.VendorID),
                //new FieldLookup<POOrder.vendorID>(demandExt.UsrVendorID),
                new FieldLookup<POOrder.vendorLocationID>(demand.VendorLocationID),
                //new FieldLookup<POOrder.vendorLocationID>(demandExt.UsrVendorLocationID),
                new FieldLookup<POOrder.bLOrderNbr>(linkToBlanket ? soline.PONbr : null),
            };

            // --- Add this block to separate by SPC Cost if > 0 ---
            if (soline != null)
            {
                SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
              And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                              .Select(Base, soline?.OrderType, soline?.OrderNbr, soline?.LineNbr);

                var solineExt = soLine.GetExtension<SOLineExt>();
                decimal? spcCost = solineExt?.UsrSWKSPCCost;
                if (spcCost.HasValue && spcCost.Value > 0)
                {
                    // Use a dummy field name for grouping, as POOrder does not have this field
                    orderSearchValues.Add(new FieldLookup<SOLineExt.usrSWKSPCCost>(spcCost));
                }
            }
            // -----------------------------------------------------

            // ... existing grouping logic ...

            if (OrderType == POOrderType.RegularOrder)
            {
                if (requireSingleProject)
                {
                    int? project = demand.DemandProjectID ?? PX.Objects.PM.ProjectDefaultAttribute.NonProject();
                    orderSearchValues.Add(new FieldLookup<POOrder.projectID>(project));
                }

                if (previousOrder != null && previousOrder.ShipDestType == POShippingDestination.CompanyLocation && previousOrder.SiteID == null)
                {
                    //When previous order was shipped to Company then we would never find it if we search by POSiteID
                }
                else
                {
                    orderSearchValues.Add(new FieldLookup<POOrder.siteID>(demand.POSiteID));
                }
            }
            else if (OrderType == POOrderType.DropShip)
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
                BLType = linkToBlanket ? POOrderType.Blanket : null,
                BLOrderNbr = linkToBlanket ? soline.PONbr : null
            };
            //return baseMethod(created, previousOrder, demand, soorder, soline, requireSingleProject);
        }
        //    protected virtual POOrder FindOrCreatePOOrder(
        //DocumentList<POOrder> created, POOrder previousOrder, POFixedDemand demand, SOOrder soorder, SOLineSplit3 soline, bool requireSingleProject)
        //    {

        //    }

        #endregion

        #region Helper Methods

        protected virtual decimal? CalculateVendorPriceFromPlanType(POFixedDemand demand)
        {
            if (demand?.PlanType == null || demand.InventoryID == null)
                return null;

            // Check if plan type is SO to Drop-Ship or SO to Purchase
            bool isSOToDropShipOrPurchase = demand.PlanType == INPlanConstants.Plan6D ||
                                           demand.PlanType == INPlanConstants.Plan6E ||
                                           demand.PlanType == INPlanConstants.Plan66;

            if (isSOToDropShipOrPurchase)
            {
                // Use ExtCost value from related SO line to calculate unit cost
                decimal? soUnitCost = GetSOLineUnitCostFromExtCost(demand);
                if (soUnitCost != null)
                {
                    return soUnitCost;
                }
            }



            // For other plan types, get RTH Cost from inventory item
            InventoryItem item = InventoryItem.PK.Find(Base, demand.InventoryID);
            if (item != null)
            {
                var itemExt = item.GetExtension<InventoryItemExt>();
                if (itemExt?.UsrSWKRTHCost != null && itemExt.UsrSWKRTHCost > 0)
                {
                    return itemExt.UsrSWKRTHCost;
                }
            }

            return null;
        }

        protected virtual decimal? GetSOLineUnitCostFromExtCost(POFixedDemand demand)
        {
            if (string.IsNullOrEmpty(demand.OrderType) || string.IsNullOrEmpty(demand.OrderNbr) ||
                demand.LineNbr == null)
                return null;

            // Find the related SO line
            SOLine soLine = PXSelect<SOLine,
                Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                    And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                    And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                .Select(Base, demand.OrderType, demand.OrderNbr, demand.LineNbr);

            if (soLine != null)
            {
                // Set Vendor Price to CuryExtCost directly
                return soLine.CuryExtCost;
            }

            return null;
        }

        #endregion
    }
}
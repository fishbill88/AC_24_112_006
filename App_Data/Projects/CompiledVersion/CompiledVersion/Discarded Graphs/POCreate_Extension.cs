//using CompiledVersion.DAC;
//using PX.Common;
//using PX.Data;
//using PX.Data.ReferentialIntegrity.Attributes;
//using PX.Objects.AP;
//using PX.Objects.CS;
//using PX.Objects.IN;
//using PX.Objects.PO;
//using PX.Objects.SO;
//using System;
//using System.Collections.Generic;
//using static PX.Objects.PO.POCreate;
//using static PX.Objects.SO.SOCreate;
//using SOLineSplit3 = PX.Objects.PO.POOrderEntry.SOLineSplit3;

//namespace CompiledVersion.Graphs
//{
//    public class POCreate_Extension : PXGraphExtension<PX.Objects.PO.POCreate>
//    {
//        public static bool IsActive() => true;

//        #region OVerride Methods

//        public delegate String LinkPOLineToBlanketDelegate(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText);
//        [PXOverride]
//        public String LinkPOLineToBlanket(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText, LinkPOLineToBlanketDelegate baseMethod)
//        {
//            POOrderExt poOrderExt = docgraph.Document.Current?.GetExtension<POOrderExt>();
//            //var docGraphExt = docgraph?.GetExtension<POOrderEntry_Extension>();
//            SOOrder soOrder = SOOrder.PK.Find(Base, soline?.OrderType, soline?.OrderNbr);
//            SOOrderExt soOrderExt = soOrder?.GetExtension<SOOrderExt>();


//            SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
//              And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
//                              .Select(Base, soline?.OrderType, soline?.OrderNbr, soline?.LineNbr);
//            if (soLine != null)
//            {
//                SOLineExt soLineExt = soLine.GetExtension<SOLineExt>();
//                POLineExt poLineExt = line.GetExtension<POLineExt>();

//                poLineExt.UsrVendorSpecTerms = soLineExt.UsrVendorSpecTerms;
//                poLineExt.UsrVendorNotes = soLineExt.UsrVendorNotes;

//                docgraph.CurrentDocument.Current.FOBPoint = soOrder.FOBPoint;
//                docgraph.CurrentDocument.Current.ShipVia = soOrder.ShipVia;

//                poOrderExt.UsrShipTermsID = soOrder.ShipTermsID;
//                poOrderExt.UsrCustomerAccount = soOrderExt.UsrCustomerAccount;



//                poLineExt.UsrItemSpecs = soLineExt?.UsrItemSpecs;

//                poOrderExt.UsrCustomerOrderNbr = soOrder?.CustomerOrderNbr;

//                // Overwrite PO Line Unit Cost if SOLine.usrSWKSPCCost has value
//                if (soLineExt != null && (soLineExt?.UsrSWKSPCCost ?? 0m) > 0m)
//                {
//                    //line.CuryUnitCost = soLineExt.UsrSWKSPCCost;
//                    docgraph?.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, soLineExt?.UsrSWKSPCCost);
//                }
//                else
//                {
//                    //line.CuryUnitCost = demand.EffPrice;
//                    docgraph?.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, soLine?.CuryUnitCost);
//                    //docgraph?.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(line, demand.EffPrice);
//                }

//                if (soLineExt != null && soLineExt?.UsrSWKSPCCode != null && poLineExt != null)
//                {
//                    //poLineExt.UsrSWKSPCCode = soLineExt.UsrSWKSPCCode;
//                    docgraph?.Transactions.Cache.SetValueExt<POLineExt.usrSWKSPCCode>(line, soLineExt?.UsrSWKSPCCode);
//                }
//            }

//            var result = baseMethod(line, docgraph, demand, soline, ref ErrorLevel, ref ErrorText);

//            if (soLine != null)
//            {
//                SOLineExt soLineExt = soLine.GetExtension<SOLineExt>();
//                SOOrderType orderType = SOOrderType.PK.Find(Base, soOrder?.OrderType);
//                SOOrderTypeExt typeExt = orderType?.GetExtension<SOOrderTypeExt>();

//                if (typeExt?.UsrShowVendorID ?? false)
//                    docgraph?.Transactions.Cache.SetValueExt<POLineExt.usrVendorID>(line, soLineExt?.UsrVendorID);
//                //poLineExt.UsrVendorID = soLineExt?.UsrVendorID;

//                if (typeExt?.UsrShowVendorLocationID ?? false && soLineExt?.UsrVendorID != null)
//                    docgraph?.Transactions.Cache.SetValueExt<POLineExt.usrVendorLocationID>(line, soLineExt?.UsrVendorLocationID);
//                //poLineExt.UsrVendorLocationID = soLineExt?.UsrVendorLocationID;

//                if (typeExt?.UsrShowVendorAddress ?? false)
//                    docgraph?.Transactions.Cache.SetValueExt<POLineExt.usrVendorAddress>(line, soLineExt?.UsrVendorAddress);
//                //poLineExt.UsrVendorAddress = soLineExt?.UsrVendorAddress;
//            }

//            docgraph?.Document.Cache.SetValueExt<POOrderExt.usrShippingInstructions>(line, soOrderExt?.UsrShippingInstructions);

//            return result;
//        }


//        [PXOverride]
//        public virtual void EnumerateAndPrepareFixedDemandRow(PXResult<POFixedDemand> rec,
//            System.Action<PXResult<POFixedDemand>> baseMethod)
//        {
//            // Call the base method first
//            baseMethod(rec);

//            var demand = (POFixedDemand)rec;

//            // Set our custom Vendor Price logic
//            decimal? customVendorPrice = CalculateVendorPriceFromPlanType(demand);
//            if (customVendorPrice != null)
//            {
//                demand.EffPrice = customVendorPrice;
//            }

//            SOLine sOLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
//                  And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
//                                  .Select(Base, demand?.OrderType, demand?.OrderNbr, demand?.LineNbr);

//            SOLineExt soLineExt = sOLine?.GetExtension<SOLineExt>();

//            if (soLineExt != null)
//            {
//                bool changed = false;

//                if (soLineExt.UsrVendorID != null && demand.VendorID != soLineExt.UsrVendorID)
//                {
//                    demand.VendorID = soLineExt.UsrVendorID;
//                    changed = true;
//                }

//                if (soLineExt.UsrVendorLocationID != null && demand.VendorLocationID != soLineExt.UsrVendorLocationID)
//                {
//                    demand.VendorLocationID = soLineExt.UsrVendorLocationID;
//                    changed = true;
//                }

//                if (changed)
//                {
//                    // Mark row updated so subsequent logic (grouping / PO creation) sees new vendor values
//                    Base.FixedDemand.Cache.MarkUpdated(demand);
//                }
//            }


//            if (soLineExt?.UsrVendorID != null && soLineExt?.UsrVendorLocationID != null)
//            {
//                demand.VendorID = soLineExt?.UsrVendorID;
//                demand.VendorLocationID = soLineExt?.UsrVendorLocationID;
//            }

//        }


//        public delegate POOrder FindOrCreatePOOrderDelegate(DocumentList<POOrder> created, POOrder previousOrder, POFixedDemand demand, SOOrder soorder, SOLineSplit3 soline, Boolean requireSingleProject);
//        [PXOverride]
//        public POOrder FindOrCreatePOOrder(DocumentList<POOrder> created, POOrder previousOrder, POFixedDemand demand, SOOrder soorder, SOLineSplit3 soline, Boolean requireSingleProject, FindOrCreatePOOrderDelegate baseMethod)
//        {

//            string OrderType = demand.PlanType.IsIn(INPlanConstants.Plan6D, INPlanConstants.Plan6E) ? POOrderType.DropShip : POOrderType.RegularOrder;
//            bool linkToBlanket = demand.PlanType == INPlanConstants.Plan6B || demand.PlanType == INPlanConstants.Plan6E;

//            var orderSearchValues = new List<FieldLookup>()
//            {
//                new FieldLookup<POOrder.orderType>(OrderType),
//                new FieldLookup<POOrder.vendorID>(demand.VendorID),
//                new FieldLookup<POOrder.vendorLocationID>(demand.VendorLocationID),
//                new FieldLookup<POOrder.bLOrderNbr>(linkToBlanket ? soline.PONbr : null),
//            };

//            // --- Add this block to separate by SPC Cost if > 0 ---
//            if (soline != null)
//            {
//                SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
//              And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
//                              .Select(Base, soline?.OrderType, soline?.OrderNbr, soline?.LineNbr);

//                var solineExt = soLine.GetExtension<SOLineExt>();
//                decimal? spcCost = solineExt?.UsrSWKSPCCost;
//                if (spcCost.HasValue && spcCost.Value > 0)
//                {
//                    // Use a dummy field name for grouping, as POOrder does not have this field
//                    orderSearchValues.Add(new FieldLookup<SOLineExt.usrSWKSPCCost>(spcCost));
//                }
//            }
//            // -----------------------------------------------------

//            // ... existing grouping logic ...

//            if (OrderType == POOrderType.RegularOrder)
//            {
//                if (requireSingleProject)
//                {
//                    int? project = demand.DemandProjectID ?? PX.Objects.PM.ProjectDefaultAttribute.NonProject();
//                    orderSearchValues.Add(new FieldLookup<POOrder.projectID>(project));
//                }

//                if (previousOrder != null && previousOrder.ShipDestType == POShippingDestination.CompanyLocation && previousOrder.SiteID == null)
//                {
//                    //When previous order was shipped to Company then we would never find it if we search by POSiteID
//                }
//                else
//                {
//                    orderSearchValues.Add(new FieldLookup<POOrder.siteID>(demand.POSiteID));
//                }
//            }
//            else if (OrderType == POOrderType.DropShip)
//            {
//                orderSearchValues.Add(new FieldLookup<POOrder.sOOrderType>(soline.OrderType));
//                orderSearchValues.Add(new FieldLookup<POOrder.sOOrderNbr>(soline.OrderNbr));
//            }
//            else
//            {
//                orderSearchValues.Add(new FieldLookup<POOrder.shipToBAccountID>(soorder.CustomerID));
//                orderSearchValues.Add(new FieldLookup<POOrder.shipToLocationID>(soorder.CustomerLocationID));
//                orderSearchValues.Add(new FieldLookup<POOrder.siteID>(demand.POSiteID));
//            }

//            if (demand.IsSpecialOrder == true)
//            {
//                orderSearchValues.Add(new FieldLookup<POOrder.curyID>(demand.CuryID));
//            }

//            return created.Find(orderSearchValues.ToArray()) ?? new POOrder
//            {
//                OrderType = OrderType,
//                BLType = linkToBlanket ? POOrderType.Blanket : null,
//                BLOrderNbr = linkToBlanket ? soline.PONbr : null
//            };
//            //return baseMethod(created, previousOrder, demand, soorder, soline, requireSingleProject);
//        }

//        #endregion

//        #region Event Handlers
//        //protected virtual void POFixedDemand_VendorLocationID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
//        //{
//        //    POFixedDemand pOFixedDemand = (POFixedDemand)e.Row;
//        //    if (pOFixedDemand != null)
//        //    {
//        //        SOLine sOLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
//        //          And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
//        //                          .Select(Base, pOFixedDemand?.OrderType, pOFixedDemand?.OrderNbr, pOFixedDemand?.LineNbr);

//        //        SOLineExt soLineExt = sOLine?.GetExtension<SOLineExt>();

//        //        if (soLineExt?.UsrVendorLocationID != null)
//        //        {
//        //            e.NewValue = soLineExt?.UsrVendorLocationID;
//        //            e.Cancel = true;
//        //        }
//        //        else
//        //        {
//        //            e.NewValue = POItemCostManager.FetchLocation(Base, pOFixedDemand.VendorID, pOFixedDemand.InventoryID, pOFixedDemand.SubItemID, pOFixedDemand.SiteID);
//        //            e.Cancel = true;
//        //        }


//        //    }
//        //}

//        protected void POFixedDemand_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
//        {
//            var row = e.Row as POFixedDemand;
//            if (row == null) return;
//            if (sender.GetStatus(row) != PXEntryStatus.Deleted)
//            {
//                UpdateUsrPrice();
//            }
//        }

//        protected void POFixedDemand_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
//        {
//            var row = e.Row as POFixedDemand;
//            if (row == null) return;
//            UpdateUsrPrice();
//        }

//        protected virtual void _(Events.RowSelecting<POFixedDemand> e)
//        {
//            var row = e.Row as POFixedDemand;
//            if (row == null) return;
//            //using (new PXConnectionScope())
//            //{

//                SOLine sOLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
//                      And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
//                                      .Select(Base, row?.OrderType, row?.OrderNbr, row?.LineNbr);

//                SOLineExt soLineExt = sOLine?.GetExtension<SOLineExt>();

//                //if (soLineExt?.UsrVendorID != null && soLineExt?.UsrVendorLocationID != null &&
//                //(row.VendorID != soLineExt?.UsrVendorID || row.VendorLocationID != soLineExt?.UsrVendorLocationID))
//                //{
//                //    row.VendorID = soLineExt?.UsrVendorID;
//                //    row.VendorLocationID = soLineExt?.UsrVendorLocationID;
//                //}

//                if (row.VendorID != null && row.InventoryID != null)
//                {
//                    row.AlternateID = null;
//                    INItemXRef itemXRef = PXSelect<INItemXRef,
//                        Where<INItemXRef.inventoryID, Equal<Required<INItemXRef.inventoryID>>,
//                            And<INItemXRef.alternateType, Equal<INAlternateType.global>>>>.
//                        Select(Base, row.InventoryID);
//                    if (itemXRef != null)
//                    {
//                        row.AlternateID = itemXRef.AlternateID;
//                    }
//                }

//                InventoryItem item = InventoryItem.PK.Find(Base, e.Row.InventoryID);
//                POFixedDemandExt ext = e.Row.GetExtension<POFixedDemandExt>();
//                InventoryItemExt itemExt = item?.GetExtension<InventoryItemExt>();
//                if (ext != null && itemExt != null)
//                {
//                    // Populate RTH Cost from InventoryItemExt
//                    ext.UsrSWKRTHCost = itemExt.UsrSWKRTHCost;
//                }

//            //}

//        }

//        protected virtual void _(Events.FieldUpdated<POFixedDemand, POFixedDemand.effPrice> e)
//        {
//            if (e.Row == null) return;

//            // Recalculate Extended Cost when Vendor Price is manually changed
//            POFixedDemand demand = e.Row;
//            if (demand.OrderQty != null && demand.EffPrice != null)
//            {
//                demand.ExtCost = demand.OrderQty * demand.EffPrice;
//                Base.FixedDemand.Cache.RaiseFieldUpdated<POFixedDemand.extCost>(demand, null);
//            }
//        }

//        protected virtual void _(Events.RowSelected<POFixedDemand> e)
//        {
//            if (e.Row == null) return;

//            // Enable EffPrice for the selected row
//            PXUIFieldAttribute.SetEnabled<POFixedDemand.effPrice>(e.Cache, e.Row, true);
//        }
//        #endregion

//        #region CacheAttached
//        [PXMergeAttributes(Method = MergeMethod.Merge)]
//        [PXUIField(DisplayName = "Vendor Price", Enabled = true)]
//        protected virtual void _(Events.CacheAttached<POFixedDemand.effPrice> e) { }


//        [PXMergeAttributes(Method = MergeMethod.Replace)]
//        [LocationActive(typeof(Where<PX.Objects.CR.Location.bAccountID, Equal<Optional<POFixedDemand.vendorID>>>), DescriptionField = typeof(PX.Objects.CR.Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
//        //[PXDefault(typeof(Coalesce<Search2<Vendor.defLocationID, InnerJoin<PX.Objects.CR.Standalone.Location, On<PX.Objects.CR.Standalone.Location.locationID, Equal<Vendor.defLocationID>, And<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Vendor.bAccountID>>>>, Where<Vendor.bAccountID, Equal<Current<POFixedDemand.vendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>>>>, Search<PX.Objects.CR.Standalone.Location.locationID, Where<PX.Objects.CR.Standalone.Location.bAccountID, Equal<Current<POFixedDemand.vendorID>>, And<PX.Objects.CR.Standalone.Location.isActive, Equal<True>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
//        //[PXForeignReference(typeof(CompositeKey<Field<POFixedDemand.vendorID>.IsRelatedTo<PX.Objects.CR.Location.bAccountID>, Field<POFixedDemand.vendorLocationID>.IsRelatedTo<PX.Objects.CR.Location.locationID>>))]
//        protected virtual void _(Events.CacheAttached<POFixedDemand.vendorLocationID> e) { }
//        #endregion

//        #region Methods

//        private void UpdateUsrPrice()
//        {
//            var filter = Base.Filter.Current;
//            if (filter == null) return;
//            var filterExt = PXCache<POCreateFilter>.GetExtension<POCreateFilterExt>(filter);
//            decimal sumPrice = 0m;
//            foreach (POFixedDemand row in Base.FixedDemand.Select())
//            {
//                if (row.Selected == true && row.EffPrice != null && row.OrderQty != null)
//                {
//                    sumPrice += (row.EffPrice ?? 0m);
//                }
//            }
//            filterExt.UsrPrice = sumPrice;
//            Base.Filter.Cache.SetValueExt<POCreateFilterExt.usrPrice>(filter, sumPrice);
//        }

//        /// <summary>
//        /// Copies header and line notes and attachments from a source document to the POOrder and its lines, based on checkboxes.
//        /// </summary>
//        /// <typeparam name="TSourceOrder">Type of the source order (e.g., SOOrder)</typeparam>
//        /// <typeparam name="TSourceLine">Type of the source line (e.g., SOLine)</typeparam>
//        /// <param name="graph">PXGraph context</param>
//        /// <param name="sourceOrder">Source order object</param>
//        /// <param name="sourceLines">Source lines (IEnumerable)</param>
//        /// <param name="poOrder">Target POOrder</param>
//        /// <param name="poLines">Target POLine collection (IEnumerable)</param>
//        /// <param name="copyHeaderNotes">Copy header notes</param>
//        /// <param name="copyHeaderFiles">Copy header attachments</param>
//        /// <param name="copyLineNotes">Copy line notes</param>
//        /// <param name="copyLineFiles">Copy line attachments</param>
//        public static void CopyNotesAndAttachmentsToPO<TSourceOrder, TSourceLine>(
//            PXGraph pograph,
//            PXGraph sograph,
//            TSourceOrder sourceOrder,
//            System.Collections.Generic.IEnumerable<TSourceLine> sourceLines,
//            POOrder poOrder,
//            System.Collections.Generic.IEnumerable<POLine> poLines,
//            bool copyHeaderNotes,
//            bool copyHeaderFiles,
//            bool copyLineNotes,
//            bool copyLineFiles)
//            where TSourceOrder : class, IBqlTable, new()
//            where TSourceLine : class, IBqlTable, new()
//        {
//            // Copy header notes and attachments if enabled
//            if (copyHeaderNotes || copyHeaderFiles)
//            {
//                PXNoteAttribute.CopyNoteAndFiles(
//                    sograph.Caches[typeof(TSourceOrder)], sourceOrder,
//                    pograph.Caches[typeof(POOrder)], poOrder,
//                    copyNotes: copyHeaderNotes, copyFiles: copyHeaderFiles);
//            }

//            // Copy line notes and attachments if enabled
//            if (copyLineNotes || copyLineFiles)
//            {
//                var sourceLineList = new System.Collections.Generic.List<TSourceLine>(sourceLines);
//                var poLineList = new System.Collections.Generic.List<POLine>(poLines);
//                for (int i = 0; i < sourceLineList.Count && i < poLineList.Count; i++)
//                {
//                    PXNoteAttribute.CopyNoteAndFiles(
//                        pograph.Caches[typeof(TSourceLine)], sourceLineList[i],
//                        pograph.Caches[typeof(POLine)], poLineList[i],
//                        copyNotes: copyLineNotes, copyFiles: copyLineFiles);
//                }
//            }
//        }

//        protected virtual decimal? CalculateVendorPriceFromPlanType(POFixedDemand demand)
//        {
//            if (demand?.PlanType == null || demand.InventoryID == null)
//                return null;

//            // Check if plan type is SO to Drop-Ship or SO to Purchase
//            bool isSOToDropShipOrPurchase = demand.PlanType == INPlanConstants.Plan6D ||
//                                           demand.PlanType == INPlanConstants.Plan6E ||
//                                           demand.PlanType == INPlanConstants.Plan66;

//            if (isSOToDropShipOrPurchase)
//            {
//                // Use ExtCost value from related SO line to calculate unit cost
//                decimal? soUnitCost = GetSOLineUnitCostFromExtCost(demand);
//                if (soUnitCost != null)
//                {
//                    return soUnitCost;
//                }
//            }



//            // For other plan types, get RTH Cost from inventory item
//            InventoryItem item = InventoryItem.PK.Find(Base, demand.InventoryID);
//            if (item != null)
//            {
//                var itemExt = item.GetExtension<InventoryItemExt>();
//                if (itemExt?.UsrSWKRTHCost != null && itemExt.UsrSWKRTHCost > 0)
//                {
//                    return itemExt.UsrSWKRTHCost;
//                }
//            }

//            return null;
//        }

//        protected virtual decimal? GetSOLineUnitCostFromExtCost(POFixedDemand demand)
//        {
//            if (string.IsNullOrEmpty(demand.OrderType) || string.IsNullOrEmpty(demand.OrderNbr) ||
//                demand.LineNbr == null)
//                return null;

//            // Find the related SO line
//            SOLine soLine = PXSelect<SOLine,
//                Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
//                    And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
//                    And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
//                .Select(Base, demand.OrderType, demand.OrderNbr, demand.LineNbr);

//            if (soLine != null)
//            {
//                // Set Vendor Price to CuryExtCost directly
//                return soLine.CuryExtCost;
//            }

//            return null;
//        }
//        #endregion

//    }
//}
using CompiledVersion.DAC;
using CompiledVersion.Helpers;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CN.Compliance.PO.CacheExtensions;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Data.PXQuickProcess;
using static PX.Objects.SO.SOPickingWorksheet.worksheetType;

namespace CompiledVersion.Graphs
{
    public class SOShipmentEntry_Extension : PXGraphExtension<PX.Objects.SO.SOShipmentEntry>
    {
        public static bool IsActive() => true;

        protected virtual void _(Events.FieldUpdated<SOShipment, SOShipment.overrideFreightAmount> e)
        {

            if (e.Row == null) return;
            SOShipment shipment = e.Row;
            if (((bool?)e.NewValue) == true)
                shipment.CuryFreightAmt = 0m; // Reset freight amount if override is enabled
        }

        protected virtual void _(Events.FieldUpdated<SOShipment, SOShipment.curyFreightAmt> e)
        {
            if (e.Row == null) return;
            SOShipment shipment = e.Row;
            decimal? newAmt = (decimal?)e.NewValue;
            SOOrder order = Base.OrderList.Select().FirstOrDefault()?.GetItem<SOOrder>();
            if (order == null) return;
            SOOrderExt orderExt = order.GetExtension<SOOrderExt>();
            SOSetupExt sOSetupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
            if (orderExt == null) return;

            // Only enforce limit if UsrFreightPriceLimit has a value and shipping terms match
            if (orderExt.UsrFreightPriceLimit == null || orderExt.UsrFreightPriceLimit <= 0m || 
                order.ShipTermsID != sOSetupExt.UsrNotToExceed || !(shipment.OverrideFreightAmount ?? false))
            {
                // Clear any previous error
                e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(shipment, newAmt, null);
                return;
            }

            decimal? freightLimit = orderExt.UsrFreightPriceLimit;
            decimal? currentFreight = 0m;

            // Calculate freight from OTHER confirmed/invoiced shipments (exclude current shipment)
            var shipmentlist = PXSelectJoin<SOOrderShipment,
                                    LeftJoin<SOShipment, On<SOShipment.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>,
                                        And<SOShipment.shipmentType, Equal<SOOrderShipment.shipmentType>>>>,
                                        Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                                            And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>,
                                                And<SOShipment.shipmentNbr, NotEqual<Required<SOShipment.shipmentNbr>>,
                                                And<Where<SOShipment.status, Equal<SOShipmentStatus.confirmed>,
                                                    Or<SOShipment.status, Equal<SOShipmentStatus.invoiced>>>>>>>,
                                            OrderBy<Asc<SOOrderShipment.shipmentNbr>>>
                                    .Select(Base, order.OrderType, order.OrderNbr, shipment.ShipmentNbr);
            foreach (var item in shipmentlist)
            {
                SOOrderShipment orderShipment = item.GetItem<SOOrderShipment>();
                SOShipment _shipment = item.GetItem<SOShipment>();
                if (_shipment.ShipmentNbr != shipment.ShipmentNbr && 
                    (_shipment.Status == SOShipmentStatus.Confirmed || _shipment.Status == SOShipmentStatus.Invoiced))
                {
                    currentFreight += (_shipment.CuryFreightAmt ?? 0m);
                }
            }

            // Check if the new freight amount exceeds the limit
            if ((newAmt + currentFreight) > freightLimit)
            {
                decimal? exceedAmt = (newAmt + currentFreight) - freightLimit;
                decimal? adjustedAmt = freightLimit - currentFreight;
                
                e.Cache.SetValue<SOShipment.curyFreightAmt>(shipment, adjustedAmt);
                shipment.CuryFreightAmt = adjustedAmt;

                e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(e.Row, adjustedAmt, 
                    new PXSetPropertyException(Messages.FreightExceedsLimit(exceedAmt), PXErrorLevel.RowError));
            }
            else
            {
                // Clear any previous error if the new amount is valid
                e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(shipment, newAmt, null);
            }

        }

        protected virtual void _(Events.RowSelected<SOShipLine> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);

            SOShipLine line = (SOShipLine)e.Row;
            if (line == null) return;
            if (line.OrigOrderNbr != null)
            {
                SOOrder order = SOOrder.PK.Find(Base, line.OrigOrderType, line.OrigOrderNbr);
                if (order != null)
                {
                    SOOrderExt orderExt = order.GetExtension<SOOrderExt>();

                    SOShipment shipment = Base.CurrentDocument.Current;
                    SOShipmentExt shipmentExt = shipment.GetExtension<SOShipmentExt>();
                    shipmentExt.UsrShippingNotes = orderExt.UsrShippingNotes;
                    e.Cache.SetValueExt<SOOrderExt.usrShippingNotes>(shipment, orderExt.UsrShippingNotes);
                }
            }

        }

        /// <summary>
        /// Event handler to set INTran costs from SOLine when creating INIssue documents.
        /// This is triggered by the Update IN button.
        /// </summary>
        protected virtual void _(Events.RowInserted<INTran> e)
        {
            if (e.Row == null) return;

            INTran tran = e.Row;

            // Only process if this transaction is related to a sales order
            if (string.IsNullOrEmpty(tran.SOOrderType) || string.IsNullOrEmpty(tran.SOOrderNbr))
                return;

            // Find the original SOLine to get the cost
            SOLine soLine = PXSelect<SOLine,
      Where<SOLine.orderType, Equal<Required<INTran.sOOrderType>>,
     And<SOLine.orderNbr, Equal<Required<INTran.sOOrderNbr>>,
    And<SOLine.lineNbr, Equal<Required<INTran.sOOrderLineNbr>>>>>>
      .Select(Base, tran.SOOrderType, tran.SOOrderNbr, tran.SOOrderLineNbr);

            if (soLine == null) return;

            // Override the costs with values from the sales order line
            // Map SOLine.CuryUnitCost to INTran.UnitCost
            if (soLine.CuryUnitCost != null && soLine.CuryUnitCost != 0m)
            {
                e.Cache.SetValueExt<INTran.unitCost>(tran, soLine.CuryUnitCost);
            }

            // Map SOLine.CuryExtCost to INTran.TranCost (Extended Cost)
            if (soLine.CuryExtCost != null && soLine.CuryExtCost != 0m)
            {
                e.Cache.SetValueExt<INTran.tranCost>(tran, soLine.CuryExtCost);
            }
        }

        /// <summary>
        /// Event handler to ensure INTran costs are not recalculated before persisting to database.
        /// This prevents the system from overwriting costs with inventory costs during save.
        /// </summary>
        protected virtual void _(Events.RowPersisting<INTran> e)
        {
            if (e.Row == null) return;
            if (e.Operation != PXDBOperation.Insert && e.Operation != PXDBOperation.Update) return;

            INTran tran = e.Row;

            // Only process if this transaction is related to a sales order
            if (string.IsNullOrEmpty(tran.SOOrderType) || string.IsNullOrEmpty(tran.SOOrderNbr))
                return;

            // Find the original SOLine to get the cost
            SOLine soLine = PXSelect<SOLine,
           Where<SOLine.orderType, Equal<Required<INTran.sOOrderType>>,
         And<SOLine.orderNbr, Equal<Required<INTran.sOOrderNbr>>,
    And<SOLine.lineNbr, Equal<Required<INTran.sOOrderLineNbr>>>>>>
           .Select(Base, tran.SOOrderType, tran.SOOrderNbr, tran.SOOrderLineNbr);

            if (soLine == null) return;

            // Set the override flag FIRST to prevent recalculation
            tran.OverrideUnitCost = true;

            // Force the costs to match SOLine values right before saving
            if (soLine.CuryUnitCost != null && soLine.CuryUnitCost != 0m)
            {
                tran.UnitCost = soLine.CuryUnitCost;
            }

            if (soLine.CuryExtCost != null && soLine.CuryExtCost != 0m)
            {
                tran.TranCost = soLine.CuryExtCost;
            }
        }

        /// <summary>
        /// Event handler to override SOShipLine.UnitCost with SOLine.CuryUnitCost before persisting.
        /// This ensures the shipment line uses the currency-based unit cost from the order.
        /// </summary>
        protected virtual void _(Events.RowPersisting<SOShipLine> e)
        {
            if (e.Row == null) return;
            if (e.Operation != PXDBOperation.Insert && e.Operation != PXDBOperation.Update) return;

            SOShipLine shipLine = e.Row;

            // Only process if this shipment line is linked to an order line
            if (string.IsNullOrEmpty(shipLine.OrigOrderType) || string.IsNullOrEmpty(shipLine.OrigOrderNbr) || shipLine.OrigLineNbr == null)
                return;

            // Find the original SOLine to get the currency-based costs
            SOLine soLine = PXSelect<SOLine,
           Where<SOLine.orderType, Equal<Required<SOShipLine.origOrderType>>,
        And<SOLine.orderNbr, Equal<Required<SOShipLine.origOrderNbr>>,
                And<SOLine.lineNbr, Equal<Required<SOShipLine.origLineNbr>>>>>>
   .Select(Base, shipLine.OrigOrderType, shipLine.OrigOrderNbr, shipLine.OrigLineNbr);

            if (soLine == null) return;

            // Override UnitCost with CuryUnitCost from SOLine
            if (soLine.CuryUnitCost != null)
            {
                shipLine.UnitCost = soLine.CuryUnitCost;
            }

            // Override ExtCost with CuryExtCost from SOLine (proportional to shipped quantity)
            if (soLine.CuryExtCost != null && soLine.OrderQty != null && soLine.OrderQty != 0m)
            {
                // Calculate proportional cost based on shipped quantity
                decimal? proportionalExtCost = (shipLine.ShippedQty / soLine.OrderQty) * soLine.CuryExtCost;
                shipLine.ExtCost = proportionalExtCost;
            }
            else if (soLine.CuryUnitCost != null && shipLine.ShippedQty != null)
            {
                // Fallback: Calculate ExtCost from UnitCost * ShippedQty
                shipLine.ExtCost = soLine.CuryUnitCost * shipLine.ShippedQty;
            }
        }

        /// <summary>
        /// Event handler to set default UnitCost from SOLine.CuryUnitCost when shipment line is created.
        /// </summary>
        protected virtual void _(Events.FieldDefaulting<SOShipLine, SOShipLine.unitCost> e)
        {
            if (e.Row == null) return;

            SOShipLine shipLine = e.Row;

            // Only process if this shipment line is linked to an order line
            if (string.IsNullOrEmpty(shipLine.OrigOrderType) || string.IsNullOrEmpty(shipLine.OrigOrderNbr) || shipLine.OrigLineNbr == null)
                return;

            // Find the original SOLine to get the currency-based unit cost
            SOLine soLine = PXSelect<SOLine,
                Where<SOLine.orderType, Equal<Required<SOShipLine.origOrderType>>,
        And<SOLine.orderNbr, Equal<Required<SOShipLine.origOrderNbr>>,
        And<SOLine.lineNbr, Equal<Required<SOShipLine.origLineNbr>>>>>>
    .Select(Base, shipLine.OrigOrderType, shipLine.OrigOrderNbr, shipLine.OrigLineNbr);

            if (soLine != null && soLine.CuryUnitCost != null)
            {
                e.NewValue = soLine.CuryUnitCost;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Ensure SOShipLine inherits cost from SOLine when the line is created (e.g. Create Shipment from Sales Order).
        /// Keeps SOShipLine cost uniform with SOLine.CuryUnitCost.
        /// </summary>
        protected virtual void _(Events.RowInserted<SOShipLine> e)
        {
            var shipLine = e.Row; if (shipLine == null) return;

            if (string.IsNullOrEmpty(shipLine.OrigOrderType) || string.IsNullOrEmpty(shipLine.OrigOrderNbr) || shipLine.OrigLineNbr == null)
                return;

            SOLine soLine = PXSelect<SOLine,
                Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
            .Select(Base, shipLine.OrigOrderType, shipLine.OrigOrderNbr, shipLine.OrigLineNbr);

            if (soLine?.CuryUnitCost != null && soLine.CuryUnitCost != 0m)
            {
                shipLine.UnitCost = soLine.CuryUnitCost;
                if (shipLine.ShippedQty != null)
                {
                    shipLine.ExtCost = (shipLine.UnitCost ?? 0m) * (shipLine.ShippedQty ?? 0m);
                }
                Base.Transactions.Update(shipLine);
            }
        }

        /// <summary>
        /// Override GetINTranUnitCost to ensure INTran uses the correct cost from SOLine.CuryUnitCost.
        /// This method is called during PostShipment (Update IN) to determine the cost for INTran records.
        /// </summary>
        public delegate decimal? GetINTranUnitCostDelegate(SOLine soline, SOShipLine line, SOShipLineSplit split);
        [PXOverride]
        public virtual decimal? GetINTranUnitCost(SOLine soline, SOShipLine line, SOShipLineSplit split, GetINTranUnitCostDelegate baseMethod)
        {
            // Special handling for receipt operations with specific lot/serial numbers
            if (line.Operation == SOOperation.Receipt && !string.IsNullOrEmpty(line.LotSerialNbr))
            {
                var item = InventoryItem.PK.Find(Base, line.InventoryID);

                if (item?.ValMethod == INValMethod.Specific)
                {
                    // Query all cost records for this invoice line (without filtering by lot/serial in SQL)
                    var allCosts = PXSelectJoin<
          INTranCost,
           InnerJoin<INTran,
                  On<INTranCost.FK.Tran>,
         InnerJoin<ARTran,
          On<ARTran.tranType, Equal<INTran.aRDocType>,
             And<ARTran.refNbr, Equal<INTran.aRRefNbr>,
              And<ARTran.lineNbr, Equal<INTran.aRLineNbr>>>>>>,
            Where<ARTran.tranType, Equal<Required<ARTran.tranType>>,
                  And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>,
                And<ARTran.lineNbr, Equal<Required<ARTran.lineNbr>>>>>>
            .Select(Base,
            soline.InvoiceType,
           soline.InvoiceNbr,
                 soline.InvoiceLineNbr)
               .RowCast<INTranCost>()
            .ToList();

                    // Filter in memory using ordinal string comparison to handle special characters like ®
                    var matchingCosts = allCosts
                .Where(c => string.Equals(
              c.LotSerialNbr,
         line.LotSerialNbr,
                StringComparison.Ordinal))
                         .ToList();

                    if (matchingCosts.Any())
                    {
                        decimal? qtySum = matchingCosts.Sum(c => c.Qty);
                        if (qtySum != null && qtySum != 0m)
                        {
                            return INUnitAttribute.ConvertToBase(
                                  Base.Transactions.Cache,
                                  line.InventoryID,
                           line.UOM,
                            matchingCosts.Sum(c => c.TranCost).Value / qtySum.Value,
                            INPrecision.UNITCOST);
                        }
                    }
                }
            }

            // For all other cases, use SOLine.CuryUnitCost if available
            if (soline != null && soline.CuryUnitCost != null && soline.CuryUnitCost != 0m)
            {
                return soline.CuryUnitCost;
            }

            // Fallback to SOShipLine.UnitCost
            return line.UnitCost;
        }

        protected void SOShipment_RowSelected(PXCache cache, PXRowSelectedEventArgs e, PXRowSelected InvokeBaseHandler)
        {
            if (InvokeBaseHandler != null)
                InvokeBaseHandler(cache, e);

            if (e.Row == null) return;

            var shipment = (SOShipment)e.Row;

            // Manual check based on business rules
            //bool canCreateInvoice = CanCreateInvoice(shipment);
            //bool canCreateDropship = CanCreateDropshipInvoice(shipment);

            //createCombinedInvoice.SetEnabled(canCreateInvoice || canCreateDropship);

            //// Hide the original actions
            //Base.createInvoice.SetVisible(false);
            //Base.createDropshipInvoice.SetVisible(false);
        }

        #region Overrides
        public delegate void ConfirmShipmentDelegate(SOOrderEntry docgraph, SOShipment shiporder);
        [PXOverride]
        public void ConfirmShipment(SOOrderEntry docgraph, SOShipment shiporder, ConfirmShipmentDelegate baseMethod)
        {

            baseMethod(docgraph, shiporder);

            SOOrder order = docgraph.Document.Current;


            SOOrderExt ext = order.GetExtension<SOOrderExt>();
            decimal? totalFreight = 0m;
            var shipmentlist = PXSelectJoin<SOOrderShipment,
           LeftJoin<SOShipment, On<SOShipment.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>,
    And<SOShipment.shipmentType, Equal<SOOrderShipment.shipmentType>>>>,
           Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
         And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>,
     And<Where<SOShipment.status, Equal<SOShipmentStatus.confirmed>,
         Or<SOShipment.status, Equal<SOShipmentStatus.partiallyInvoiced>,
         Or<SOShipment.status, Equal<SOShipmentStatus.invoiced>,
         Or<SOShipment.status, Equal<SOShipmentStatus.completed>>>>>>>>,
  OrderBy<Asc<SOOrderShipment.shipmentNbr>>>
         .Select(Base, order.OrderType, order.OrderNbr);
            foreach (var item in shipmentlist)
            {

                SOOrderShipment orderShipment = item.GetItem<SOOrderShipment>();
                SOShipment _shipment = item.GetItem<SOShipment>();
                if (_shipment.Status == SOShipmentStatus.Confirmed || 
                    _shipment.Status == SOShipmentStatus.PartiallyInvoiced || 
                    _shipment.Status == SOShipmentStatus.Invoiced || 
                    _shipment.Status == SOShipmentStatus.Completed)
                {
                    totalFreight += (_shipment.CuryFreightAmt ?? 0m);
                }
            }

            ext.UsrFreightTotal = totalFreight;

            SOSetupExt sOSetupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
            if (sOSetupExt.UsrNotToExceed == order.ShipTermsID && order.CuryPremiumFreightAmt != totalFreight)
            {
                // Use PXDatabase to update CuryPremiumFreightAmt directly without triggering cache events
                PXDatabase.Update<SOOrder>(
            new PXDataFieldAssign<SOOrder.curyPremiumFreightAmt>(totalFreight),
              new PXDataFieldAssign<SOOrder.premiumFreightAmt>(totalFreight), // Also update base currency amount
             new PXDataFieldRestrict<SOOrder.orderType>(order.OrderType),
            new PXDataFieldRestrict<SOOrder.orderNbr>(order.OrderNbr)
             );
            }
        }

        public delegate IEnumerable CreateInvoiceDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable CreateInvoice(PXAdapter adapter, CreateInvoiceDelegate baseMethod)
        {
            var shipments = adapter.Get<SOShipment>().ToList();
            var adapterSlice = (adapter.MassProcess, adapter.AllowRedirect, adapter.QuickProcessFlow);
            var redirectRequired = !Base.IsImport;
            if (!adapter.Arguments.TryGetValue(nameof(SOShipmentFilter.InvoiceDate), out object invoiceDate) || invoiceDate == null)
                invoiceDate = Base.Accessinfo.BusinessDate;

            Base.Save.Press();

            Helper.StartLongOperation(Base, adapter, delegate ()
            {
                var shipmentEntry = PXGraph.CreateInstance<SOShipmentEntry>();
                var invoiceEntry = PXGraph.CreateInstance<SOInvoiceEntry>();

                InvoiceList createdInvoices = new ShipmentInvoices(shipmentEntry);

                foreach (SOShipment shipment in shipments)
                {
                    try
                    {
                        shipmentEntry.SelectTimeStamp();
                        invoiceEntry.SelectTimeStamp();

                        if (adapterSlice.MassProcess)
                            PXProcessing<SOShipment>.SetCurrentItem(shipment);

                        TryGetNonStockError(shipment.ShipmentNbr, out string errorMessage);
                        if (errorMessage != null)
                        {
                            if (adapterSlice.MassProcess)
                                PXProcessing<SOShipment>.SetError(errorMessage);
                            else
                                throw new PXException(errorMessage);
                        }
                        else
                        {
                            shipmentEntry.InvoiceShipment(invoiceEntry, shipment, (DateTime)invoiceDate, createdInvoices, adapterSlice.QuickProcessFlow);

                            if (adapterSlice.MassProcess) // shipment is updated and saved somewhere in InvoiceShipment method
                            {
                                shipmentEntry.Document.Cache.RestoreCopy(shipment, SOShipment.PK.Find(shipmentEntry, shipment));
                                PXProcessing<SOShipment>.SetProcessed();
                            }
                        }

                    }
                    catch (Exception ex) when (adapterSlice.MassProcess)
                    {
                        PXProcessing<SOShipment>.SetError(ex);
                    }
                }

                invoiceEntry.CompleteProcessingImpl(createdInvoices);

                if (adapterSlice.AllowRedirect && !adapterSlice.MassProcess && redirectRequired && createdInvoices.Count > 0)
                {
                    using (new PXTimeStampScope(null))
                    {
                        ARInvoice firstInvoice = createdInvoices[0];
                        invoiceEntry = PXGraph.CreateInstance<SOInvoiceEntry>();

                        invoiceEntry.Document.Current = invoiceEntry.Document.Search<ARInvoice.docType, ARInvoice.refNbr>(firstInvoice.DocType, firstInvoice.RefNbr, firstInvoice.DocType);
                        throw new PXRedirectRequiredException(invoiceEntry, "Invoice");
                    }
                }
            });

            return shipments;


            //// Validate current shipment for single-item action or the item being processed in mass mode
            //bool hasError = false;
            //foreach (SOShipment shipment in adapter.Get<SOShipment>())
            //{
            //    if (shipment?.ShipmentNbr != null && TryGetNonStockError(shipment.ShipmentNbr, out string errorMessage))
            //    {
            //        // Proper log on Process Shipments page
            //        if (adapter.MassProcess)
            //        {
            //            PXProcessing<SOShipment>.SetError(errorMessage);
            //            yield return shipment; // return current row and stop
            //            yield break;
            //        }
            //        hasError = true;
            //        // In single action, block with an exception
            //        throw new PXException(errorMessage);
            //    }
            //}
            //if (!hasError)
            //    foreach (var r in baseMethod(adapter))
            //        yield return r;
        }

        public delegate void InvoiceShipmentDelegate(SOInvoiceEntry docgraph, SOShipment shiporder, DateTime invoiceDate, InvoiceList list, ActionFlow quickProcessFlow);
        [PXOverride]
        public void InvoiceShipment(SOInvoiceEntry docgraph, SOShipment shiporder, DateTime invoiceDate, InvoiceList list, ActionFlow quickProcessFlow, InvoiceShipmentDelegate baseMethod)
        {
            baseMethod(docgraph, shiporder, invoiceDate, list, quickProcessFlow);

            var printMethod = Base.Document.Cache.GetValueExt(shiporder, "AttributeFORMTYPE");

            docgraph.Document.Cache.SetValueExt(docgraph.Document.Current, "AttributeFORMTYPE", printMethod);
            docgraph.Document.Update(docgraph.Document.Current);
            docgraph.Save.Press();
        }
        #endregion

        #region Helper Methods

        private bool TryGetNonStockError(string shipmentNbr, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(shipmentNbr))
                return false;

            var setupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
            if (setupExt == null) return false;

            var nonStockItems = new List<int?>();
            if (setupExt.UsrNonstock1 != null) nonStockItems.Add(setupExt.UsrNonstock1);
            if (setupExt.UsrNonstock2 != null) nonStockItems.Add(setupExt.UsrNonstock2);
            if (setupExt.UsrNonstock3 != null) nonStockItems.Add(setupExt.UsrNonstock3);
            if (nonStockItems.Count == 0)
                return false;

            var lines = PXSelect<SOShipLine,
                Where<SOShipLine.shipmentNbr, Equal<Required<SOShipLine.shipmentNbr>>>>
                .Select(Base, shipmentNbr)
                .RowCast<SOShipLine>();

            var invalidNonstock = new List<string>();

            foreach (SOShipLine line in lines)
            {
                if (nonStockItems.Contains(line.InventoryID))
                {
                    InventoryItem item = PXSelect<InventoryItem,
                        Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                        .Select(Base, line.InventoryID);

                    if (item != null)
                        invalidNonstock.Add(item.InventoryCD);

                    // Mark the line field so if user opens the shipment they see the problematic lines
                    PXUIFieldAttribute.SetError<SOShipLine.inventoryID>(Base.Transactions.Cache, line,
                        string.Format("You cannot invoice this non-stock item for {0}.", shipmentNbr));
                }
            }

            if (invalidNonstock.Count > 0)
            {
                errorMessage = Messages.CannotInvoiceNonStockItems(string.Join(", ", invalidNonstock));
                return true;
            }

            return false;
        }



        private bool CheckItemsForFlaggedNonStockItem(string shipmentNbr)
        {
            if (string.IsNullOrEmpty(shipmentNbr))
                return false;

            var setupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
            if (setupExt == null) return true;

            var nonStockItems = new List<int?>();
            if (setupExt.UsrNonstock1 != null) nonStockItems.Add(setupExt.UsrNonstock1);
            if (setupExt.UsrNonstock2 != null) nonStockItems.Add(setupExt.UsrNonstock2);
            if (setupExt.UsrNonstock3 != null) nonStockItems.Add(setupExt.UsrNonstock3);

            if (nonStockItems.Count == 0)
                return true;

            var lines = PXSelect<SOShipLine,
                Where<SOShipLine.shipmentNbr, Equal<Required<SOShipLine.shipmentNbr>>>>
                .Select(Base, shipmentNbr)
                .RowCast<SOShipLine>();

            var invalidNonstock = new List<string>();

            foreach (SOShipLine line in lines)
            {
                if (nonStockItems.Contains(line.InventoryID))
                {
                    InventoryItem item = PXSelect<InventoryItem,
                        Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                        .Select(Base, line.InventoryID);
                    if (item != null)
                    {
                        invalidNonstock.Add(item.InventoryCD);
                    }

                    // mark the line in the shipment graph
                    PXUIFieldAttribute.SetError<SOShipLine.inventoryID>(Base.Transactions.Cache, line,
                        "You cannot invoice this non-stock item.");
                }
            }

            if (invalidNonstock.Count > 0)
            {
                throw new PXException(Messages.CannotInvoiceNonStockItems(string.Join(", ", invalidNonstock)));
            }
            return true;
        }

        protected virtual bool IsDropShipShipment(SOShipment shipment)
        {
            // Check if shipment has any drop-ship order shipments
            var hasDropShip = PXSelect<SOOrderShipment,
                Where<SOOrderShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                    And<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>>>>
                .Select(Base, shipment.ShipmentNbr).Count > 0;

            return hasDropShip;
        }
        protected virtual bool CanCreateInvoice(SOShipment shipment)
        {
            // Check if shipment is confirmed and not already invoiced
            if (shipment.Confirmed != true)
                return false;

            // Check if there are uninvoiced order shipments
            var hasUninvoiced = PXSelect<SOOrderShipment,
                Where<SOOrderShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                    And<SOOrderShipment.shipmentType, Equal<Required<SOShipment.shipmentType>>,
                    And<SOOrderShipment.invoiceNbr, IsNull,
                    And<SOOrderShipment.createARDoc, Equal<True>>>>>>
                .Select(Base, shipment.ShipmentNbr, shipment.ShipmentType).Count > 0;

            return hasUninvoiced;
        }

        protected virtual bool CanCreateDropshipInvoice(SOShipment shipment)
        {
            // Similar logic for dropship invoices
            var hasDropShip = PXSelect<SOOrderShipment,
                Where<SOOrderShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                    And<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>,
                    And<SOOrderShipment.invoiceNbr, IsNull,
                    And<SOOrderShipment.createARDoc, Equal<True>>>>>>
                .Select(Base, shipment.ShipmentNbr).Count > 0;

            return hasDropShip;
        }

        #endregion

        #region Actions

        //// New combined action - CORRECTED for mass processing
        //public PXAction<SOShipment> createCombinedInvoice;
        //[PXButton(CommitChanges = true), PXUIField(DisplayName = "Prepare Invoice", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        //public virtual IEnumerable CreateCombinedInvoice(PXAdapter adapter)
        //{
        //    var shipments = adapter.Get<SOShipment>().ToList();
        //    var results = new List<SOShipment>();
        //    if (!adapter.Arguments.TryGetValue("InvoiceDate", out var invoiceDate) || invoiceDate == null)
        //    {
        //        invoiceDate = Base.Accessinfo.BusinessDate;
        //    }
        //    foreach (SOShipment shipment in shipments)
        //    {
        //        // Set current shipment
        //        Base.Document.Current = shipment;


        //        if (IsDropShipShipment(shipment))
        //        {
        //            // Process as drop-ship invoice
        //            //processResult = Base.createDropshipInvoice.Press(adapter);
        //            SOShipmentEntry shipmentEntry = PXGraph.CreateInstance<SOShipmentEntry>();
        //            InvoiceList invoiceList = new ShipmentInvoices(shipmentEntry);
        //            (bool MassProcess, Dictionary<string, object> Arguments) adapterSlice = (MassProcess: adapter.MassProcess, Arguments: adapter.Arguments);
        //            SOShipmentEntry.InvoiceReceipt(adapterSlice.Arguments, shipments, invoiceList, adapterSlice.MassProcess);
        //            shipments.ForEach(delegate (SOShipment sh)
        //            {
        //                shipmentEntry.Document.Cache.RestoreCopy(sh, PrimaryKeyOf<SOShipment>.By<SOShipment.shipmentNbr>.Find(shipmentEntry, shipmentEntry.Document.Current));
        //            });
        //        }
        //        else
        //        {
        //            // Process as regular invoice
        //            //processResult = Base.createInvoice.Press(singleAdapter);
        //            SOShipmentEntry shipmentEntry = PXGraph.CreateInstance<SOShipmentEntry>();
        //            SOInvoiceEntry sOInvoiceEntry = PXGraph.CreateInstance<SOInvoiceEntry>();
        //            (bool MassProcess, bool AllowRedirect, PXQuickProcess.ActionFlow QuickProcessFlow) adapterSlice = (MassProcess: adapter.MassProcess, AllowRedirect: adapter.AllowRedirect, QuickProcessFlow: adapter.QuickProcessFlow);
        //            InvoiceList invoiceList = new ShipmentInvoices(shipmentEntry);

        //            if (adapterSlice.MassProcess)
        //            {
        //                PXProcessing<SOShipment>.SetCurrentItem(shipment);
        //            }
        //            Base.InvoiceShipment(sOInvoiceEntry, shipment, (DateTime)invoiceDate, invoiceList, adapterSlice.QuickProcessFlow);

        //            if (adapterSlice.MassProcess)
        //            {
        //                shipmentEntry.Document.Cache.RestoreCopy(shipment, PrimaryKeyOf<SOShipment>.By<SOShipment.shipmentNbr>.Find(shipmentEntry, shipmentEntry.Document.Current));
        //                PXProcessing<SOShipment>.SetProcessed();
        //            }
        //        }

        //    }

        //    return shipments;
        //    //return results.Count > 0 ? results : shipments;
        //}
        #endregion

        /// <summary>
        /// Keep SOShipLine.UnitCost in sync with SOLine.CuryUnitCost even if Orig* fields are populated after insert (e.g., Create Shipment flow).
        /// </summary>
        protected virtual void _(Events.RowUpdated<SOShipLine> e)
        {
            var row = e.Row as SOShipLine; var old = e.OldRow as SOShipLine;
            if (row == null) return;

            // Only when linked to SO
            if (string.IsNullOrEmpty(row.OrigOrderType) || string.IsNullOrEmpty(row.OrigOrderNbr) || row.OrigLineNbr == null)
                return;

            // Avoid unnecessary work if unchanged and already correct
            bool origJustSet = (old == null) || old.OrigOrderNbr != row.OrigOrderNbr || old.OrigLineNbr != row.OrigLineNbr || old.OrigOrderType != row.OrigOrderType;

            // Always verify and align cost if orig changed or if UnitCost equals inventory cost but differs from SO
            SOLine soLine = PXSelect<SOLine,
            Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
            And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
            And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
            .Select(Base, row.OrigOrderType, row.OrigOrderNbr, row.OrigLineNbr);

            if (soLine == null) return;

            decimal? soCost = soLine.CuryUnitCost;
            if (soCost == null || soCost == 0m) return;

            if (row.UnitCost != soCost)
            {
                row.UnitCost = soCost;
                if (row.ShippedQty != null)
                {
                    row.ExtCost = (row.UnitCost ?? 0m) * (row.ShippedQty ?? 0m);
                }
            }
        }

        public delegate void PostShipmentDelegate(INRegisterEntryBase docgraph, PXResult<SOOrderShipment, SOOrder> sh, DocumentList<INRegister> list, ARInvoice invoice);
        [PXOverride]
        public void PostShipment(INRegisterEntryBase docgraph, PXResult<SOOrderShipment, SOOrder> sh, DocumentList<INRegister> list, ARInvoice invoice, PostShipmentDelegate baseMethod)
        {
            baseMethod(docgraph, sh, list, invoice);
            SOOrderShipment soOrderShipment = (SOOrderShipment)sh;

            foreach (INTran item in docgraph.LSSelectDataMember.Select().RowCast<INTran>().Where(r => r.SOOrderNbr == soOrderShipment.OrderNbr && r.SOOrderType == soOrderShipment.OrderType))
            {
                //get SOShipLine
                SOShipLine shipLine = PXSelect<SOShipLine,
                    Where<SOShipLine.shipmentNbr, Equal<Required<SOShipLine.shipmentNbr>>,
                    And<SOShipLine.lineNbr, Equal<Required<SOShipLine.lineNbr>>,
                        And<SOShipLine.shipmentType, Equal<Required<SOShipLine.shipmentType>>>>>>
                    .Select(Base, item.SOShipmentNbr, item.SOShipmentLineNbr, item.SOShipmentType);

                if (shipLine == null) continue;

                // Use PXDatabase to update INTran directly without triggering cache events
                PXDatabase.Update<INTran>(
                     new PXDataFieldAssign<INTran.unitCost>(shipLine.UnitCost),
                    new PXDataFieldAssign<INTran.tranCost>(shipLine.ExtCost),
                        new PXDataFieldRestrict<INTran.docType>(item.DocType),
                      new PXDataFieldRestrict<INTran.refNbr>(item.RefNbr),
                  new PXDataFieldRestrict<INTran.lineNbr>(item.LineNbr)
                  );

                // CRITICAL: Set OverrideUnitCost in cache so release process respects the cost
                //item.OverrideUnitCost = true;
                //item.UnitCost = shipLine.UnitCost;
                //item.TranCost = shipLine.ExtCost;
                //docgraph.LSSelectDataMember.Update(item);
            }
        }

        #region NTE Freight Validation for SO Shipments
        /// <summary>
        /// Validates that the total freight cost across all shipments for a Sales Order
        /// does not exceed the Not-To-Exceed limit defined on the Sales Order.
        /// </summary>
        protected virtual void _(Events.RowPersisting<SOShipment> e)
        {
            if (e.Row == null) return;
            if (e.Operation == PXDBOperation.Delete) return;

            SOShipment shipment = e.Row;

            // Skip if no freight cost on this shipment
            decimal currentFreightCost = shipment.CuryFreightAmt ?? 0m;
            if (currentFreightCost <= 0m) return;

            // Get linked SO order from the shipment
            var linkedSO = GetLinkedSOForShipment(shipment);
            if (linkedSO == null) return; // No SO link, skip validation

            SOOrderExt soExt = linkedSO.GetExtension<SOOrderExt>();
            SOSetupExt setupExt = Base.sosetup.Current?.GetExtension<SOSetupExt>();

            // Check if this SO has NTE shipping terms
            if (linkedSO.ShipTermsID != setupExt?.UsrNotToExceed) return;

            // Check if SO has NTE limit set
            decimal? nteLimit = soExt?.UsrFreightPriceLimit;
            if (nteLimit == null || nteLimit <= 0m) return;

            // Calculate total freight cost across ALL shipments for this SO
            decimal? totalFreightCost = CalculateTotalFreightCostForSOShipments(linkedSO, shipment, currentFreightCost);

            // Validate against limit
            if (totalFreightCost > nteLimit)
            {
                decimal? exceedAmt = totalFreightCost - nteLimit;
                string errorMsg = Messages.ShipmentFreightExceedsNTE(
                    $"{linkedSO.OrderType}-{linkedSO.OrderNbr}",
                    totalFreightCost,
                    nteLimit,
                    exceedAmt
                );

                // Use setup toggle to determine error level
                PXErrorLevel errorLevel = (setupExt?.UsrEnforcePONTE == true) 
                    ? PXErrorLevel.Error 
                    : PXErrorLevel.Warning;

                e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(
                    shipment, 
                    currentFreightCost, 
                    new PXSetPropertyException(errorMsg, errorLevel)
                );

                // If hard stop mode, throw exception to block save
                if (errorLevel == PXErrorLevel.Error)
                {
                    throw new PXException(errorMsg);
                }
            }
        }

        /// <summary>
        /// Retrieves the linked Sales Order for the shipment.
        /// Uses OrderList to get the first linked SO, or queries SOOrderShipment.
        /// </summary>
        private SOOrder GetLinkedSOForShipment(SOShipment shipment)
        {
            if (shipment == null) return null;

            // First try to get from OrderList view (already loaded)
            SOOrder order = Base.OrderList.Select().FirstOrDefault()?.GetItem<SOOrder>();
            if (order != null) return order;

            // Fallback: Query SOOrderShipment to find linked order
            var orderShipment = PXSelect<SOOrderShipment,
                Where<SOOrderShipment.shipmentType, Equal<Required<SOOrderShipment.shipmentType>>,
                    And<SOOrderShipment.shipmentNbr, Equal<Required<SOOrderShipment.shipmentNbr>>>>>
                .SelectWindowed(Base, 0, 1, shipment.ShipmentType, shipment.ShipmentNbr)
                .RowCast<SOOrderShipment>()
                .FirstOrDefault();

            if (orderShipment != null)
            {
                return PXSelect<SOOrder,
                    Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                        And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                    .Select(Base, orderShipment.OrderType, orderShipment.OrderNbr);
            }

            return null;
        }

        /// <summary>
        /// Calculates the cumulative total freight price from all SOOrderShipment records for a Sales Order.
        /// Uses SOOrderShipment as the single source of truth (same as Shipments grid on SO).
        /// Handles both regular shipments and drop-ship PO receipts.
        /// </summary>
        private decimal? CalculateTotalFreightCostForSOShipments(SOOrder soOrder, SOShipment currentShipment, decimal currentFreightCost)
        {
            if (soOrder == null) return 0m;

            decimal totalFreight = 0m;
            bool currentShipmentIncluded = false;

            // Get all SOOrderShipment records for this Sales Order (same as Shipments grid)
            var orderShipments = PXSelect<SOOrderShipment,
                Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                    And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                .Select(Base, soOrder.OrderType, soOrder.OrderNbr);

            foreach (SOOrderShipment orderShipment in orderShipments.RowCast<SOOrderShipment>())
            {
                if (orderShipment.ShipmentType == SOShipmentType.DropShip)
                {
                    // Drop-Ship: Get freight from PO Receipt via ShippingRefNoteID
                    if (orderShipment.ShippingRefNoteID != null)
                    {
                        POReceipt receipt = PXSelect<POReceipt,
                            Where<POReceipt.noteID, Equal<Required<POReceipt.noteID>>>>
                            .Select(Base, orderShipment.ShippingRefNoteID);

                        if (receipt != null)
                        {
                            POReceiptExt receiptExt = receipt.GetExtension<POReceiptExt>();
                            totalFreight += (receiptExt?.UsrFreightPrice ?? 0m);
                        }
                    }
                }
                else
                {
                    // Regular shipment: Get freight from SOShipment
                    if (!string.IsNullOrEmpty(orderShipment.ShipmentNbr))
                    {
                        SOShipment shipment = PXSelect<SOShipment,
                            Where<SOShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                                And<SOShipment.shipmentType, Equal<Required<SOShipment.shipmentType>>>>>
                            .Select(Base, orderShipment.ShipmentNbr, orderShipment.ShipmentType);

                        if (shipment != null)
                        {
                            // Check if this is the current shipment being saved
                            if (shipment.ShipmentType == currentShipment.ShipmentType && 
                                shipment.ShipmentNbr == currentShipment.ShipmentNbr)
                            {
                                totalFreight += currentFreightCost;
                                currentShipmentIncluded = true;
                            }
                            else
                            {
                                totalFreight += (shipment.CuryFreightAmt ?? 0m);
                            }
                        }
                    }
                }
            }

            // If current shipment wasn't found in SOOrderShipment (new shipment not yet linked), add its freight
            if (!currentShipmentIncluded)
            {
                totalFreight += currentFreightCost;
            }

            return totalFreight;
        }
        #endregion
    }
}
using CompiledVersion.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.Licensing;
using PX.Objects.AR;
using PX.Objects.CR.Standalone;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.CM;
using PX.Objects.SO;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static PX.Data.PXQuickProcess;
using PX.Objects.Common.Extensions;

namespace CompiledVersion.Graphs
{
    public class SOOrderEntry_Extension : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() => true;

        public bool SuppressCodeRequired = false;
        private bool _isDocumentDeleting; // flag to suppress line renumber when deleting entire order

        #region Views
        public SelectFrom<SOCustSalesPeople>.Where<SOCustSalesPeople.orderType.IsEqual<SOOrder.orderType.FromCurrent>
            .And<SOCustSalesPeople.orderNbr.IsEqual<SOOrder.orderNbr.FromCurrent>>>.View SalesPeople;
        #endregion

        #region Helpers
        private decimal RoundCury(SOOrder order, decimal value)
        {
            try
            {
                if (order?.CuryInfoID != null)
                {
                    var ci = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(Base, order.CuryInfoID)
                        .TopFirst;
                    int prec = ci?.CuryPrecision ??2;
                    return Math.Round(value, prec, MidpointRounding.AwayFromZero);
                }
            }
            catch { }
            return Math.Round(value,2, MidpointRounding.AwayFromZero);
        }

        private void RecalculatePrepaymentRequiredAmount(SOOrder order)
        {
            if (order == null) return;

            // Respect manual override
            if (order.OverridePrepayment == true) return;

            var ext = order.GetExtension<SOOrderExt>();
            decimal pct = order.PrepaymentReqPct ??0m;
            decimal baseTotal = (ext?.UsrRTHCuryOrderTotal ??0m);

            // Fallback if custom total not available yet
            if (baseTotal <=0m)
                baseTotal = order.CuryOrderTotal ??0m;

            decimal amt = RoundCury(order, pct * baseTotal /100m);
            Base.Document.Cache.SetValueExt<SOOrder.curyPrepaymentReqAmt>(order, amt);
        }
        #endregion

        #region Overrides

        public delegate IEnumerable PrepareInvoiceDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable PrepareInvoice(PXAdapter adapter, PrepareInvoiceDelegate baseMethod)
        {
            List<SOOrder> list = adapter.Get<SOOrder>().ToList();

            foreach (SOOrder order in list)
            {
                if (Base.Document.Cache.GetStatus(order) != PXEntryStatus.Inserted)
                    Base.Document.Cache.MarkUpdated(order, assertError: true);
            }

            if (!adapter.MassProcess)
            {
                try
                {
                    Base.RecalculateExternalTaxesSync = true;
                    Base.Save.Press();
                }
                finally
                {
                    Base.RecalculateExternalTaxesSync = false;
                }
            }

            Dictionary<string, object> arguments = adapter.Arguments;
            bool massProcess = adapter.MassProcess;
            PXQuickProcess.ActionFlow quickProcessFlow = adapter.QuickProcessFlow;

            PXLongOperation.StartOperation(this, delegate ()
            {
                var graph = PXGraph.CreateInstance<SOOrderEntry>();
                SOOrderEntry_Extension ext = graph.GetExtension<SOOrderEntry_Extension>();
                ext.InvoiceOrders(list, arguments, massProcess, quickProcessFlow);
            });

            // IMPORTANT: Return the list (enumerates SOOrder items). Do NOT 'yield return list;'
            return list;
        }

        protected virtual void InvoiceOrders(List<SOOrder> list, Dictionary<string, object> arguments,
            bool massProcess, PXQuickProcess.ActionFlow quickProcessFlow)
        {
            var shipmentEntry = PXGraph.CreateInstance<SOShipmentEntry>();
            var created = new InvoiceList(shipmentEntry);

            Base.InvoiceOrder(arguments, list, created, massProcess, quickProcessFlow, false);

            if (massProcess) // order is updated and saved somewhere in InvoiceOrder method
                list.ForEach(o => shipmentEntry.soorder.Cache.RestoreCopy(o, SOOrder.PK.Find(shipmentEntry, o)));

            if (!massProcess && created.Count >0)
            {
                using (new PXTimeStampScope(null))
                {
                    SOInvoiceEntry ie = PXGraph.CreateInstance<SOInvoiceEntry>();
                    ie.Document.Current = ie.Document.Search<ARInvoice.docType, ARInvoice.refNbr>(((ARInvoice)created[0]).DocType, ((ARInvoice)created[0]).RefNbr, ((ARInvoice)created[0]).DocType);
                    throw new PXRedirectRequiredException(ie, "Invoice");
                }
            }
        }

        public delegate void InvoiceOrderDelegate(Dictionary<String, Object> parameters, IEnumerable<SOOrder> list, InvoiceList created, Boolean isMassProcess, ActionFlow quickProcessFlow, Boolean groupByCustomerOrderNumber);
        [PXOverride]
        public void InvoiceOrder(Dictionary<String, Object> parameters, IEnumerable<SOOrder> list, InvoiceList created, Boolean isMassProcess, ActionFlow quickProcessFlow, Boolean groupByCustomerOrderNumber, InvoiceOrderDelegate baseMethod)
        {
            bool optimizeExternalTaxCalc = isMassProcess;
            SOShipmentEntry docgraph = PXGraph.CreateInstance<SOShipmentEntry>();
            SOInvoiceEntry invoiceEntry = PXGraph.CreateInstance<SOInvoiceEntry>();

            foreach (SOOrder order in list.OrderBy(o => o.OrderType).ThenBy(o => o.OrderNbr))
            {
                try
                {
                    if (isMassProcess) PXProcessing<SOOrder>.SetCurrentItem(order);

                    TryGetNonStockError(order.OrderType,order.OrderNbr, out string errorMessage);

                    if (errorMessage != null)
                    {
                        if (isMassProcess)
                            PXProcessing<SOOrder>.SetError(errorMessage);
                        else
                            throw new Exception(errorMessage);
                    }
                    else
                    {
                        invoiceEntry.Clear();
                        invoiceEntry.Clear(PXClearOption.ClearQueriesOnly);
                        invoiceEntry.ARSetup.Current.RequireControlTotal = false;

                        List<PXResult<SOOrderShipment>> shipments = new List<PXResult<SOOrderShipment>>();
                        PXResultset<SOShipLine, SOLine> details = null;

                        foreach (PXResult<SOOrderType, SOOrderShipment, SOOrderTypeOperation, CurrencyInfo, SOAddress, SOContact, Customer> res in
                            Base.GetOrderShipments(docgraph, order))
                        {
                            SOOrderShipment shipment = (SOOrderShipment)res;

                            if (((SOOrderType)res).RequireShipping == false || ((SOOrderTypeOperation)res).INDocType == INTranType.NoUpdate)
                            {
                                //if order is created with zero lines, invoiced, and then new line added, this will save us
                                if (shipment.ShipmentNbr == null)
                                {
                                    shipment = SOOrderShipment.FromSalesOrder(order);
                                    shipment.ShipmentType = INTranType.DocType(((SOOrderTypeOperation)res).INDocType);
                                }

                                if (details == null)
                                {
                                    details = new PXResultset<SOShipLine, SOLine>();
                                }

                                foreach (SOLine line in PXSelectJoin<SOLine,
                                    InnerJoin<InventoryItem,
                                        On<SOLine.FK.InventoryItem>>,
                                    Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                                    And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                                    And<SOLine.lineType, NotEqual<SOLineType.miscCharge>>>>>.Select(docgraph, order.OrderType, order.OrderNbr))
                                {
                                    details.Add(new PXResult<SOShipLine, SOLine>(SOShipLine.FromSOLine(line), line));
                                }
                            }
                            else if (HasMiscLinesToInvoice(order) && shipment.ShipmentNbr == null)
                            {
                                shipment = SOOrderShipment.FromSalesOrder(order, miscOnly: true);
                                shipment.ShipmentType = INDocType.Invoice;
                            }

                            if (shipment.ShipmentType == SOShipmentType.DropShip)
                            {
                                details = details ?? new PXResultset<SOShipLine, SOLine>();
                                details.AddRange(docgraph.CollectDropshipDetails(shipment));
                            }

                            if (shipment.ShipmentNbr != null)
                            {
                                shipments.Add(new PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType, SOOrderTypeOperation, Customer>(shipment, order, (CurrencyInfo)res, (SOAddress)res, (SOContact)res, (SOOrderType)res, (SOOrderTypeOperation)res, (Customer)res));
                            }
                        }

                        shipments = new List<PXResult<SOOrderShipment>>(shipments.OrderBy(s => PXResult.Unwrap<SOOrderShipment>(s).Operation == PXResult.Unwrap<SOOrderType>(s).DefaultOperation ? 0 : 1)
                            .ThenBy(s => PXResult.Unwrap<SOOrderShipment>(s).ShipmentNbr));

                        foreach (PXResult<SOOrderShipment, SOOrder, CurrencyInfo, SOAddress, SOContact, SOOrderType, SOOrderTypeOperation> res in shipments)
                        {

                            Base.Clear();
                            var soorder = (SOOrder)res;
                            Base.Document.Current = Base.Document.Search<SOOrder.orderNbr>(soorder.OrderNbr, soorder.OrderType);
                            if (PX.SM.WorkflowAction.HasWorkflowActionEnabled(Base, g => g.prepareInvoice, Base.Document.Current) == false)
                            {
                                throw new PXInvalidOperationException(PX.Objects.SO.Messages.ActionNotAvailableInCurrentState,
                                    Base.prepareInvoice.GetCaption(), Base.Document.Cache.GetRowDescription(Base.Document.Current));
                            }

                            using (var ts = new PXTransactionScope())
                            {
                                invoiceEntry.InvoiceOrder(new InvoiceOrderArgs(res)
                                {
                                    InvoiceDate = invoiceEntry.Accessinfo.BusinessDate.Value,
                                    Customer = Base.customer.Current,
                                    List = created,
                                    Details = details,
                                    QuickProcessFlow = quickProcessFlow,
                                    GroupByDefaultOperation = !isMassProcess,
                                    GroupByCustomerOrderNumber = groupByCustomerOrderNumber,
                                    OptimizeExternalTaxCalc = optimizeExternalTaxCalc
                                });

                                Base.Clear();
                                ts.Complete();
                            }

                            PXProcessing<SOOrder>.SetProcessed();
                        }
                    }
                }
                catch (Exception ex) when (isMassProcess)
                {
                    PXProcessing<SOOrder>.SetError(ex);
                }
            }

            if (optimizeExternalTaxCalc)
            {
                invoiceEntry.CompleteProcessingImpl(created);
            }
        }

        // Ensure all copied lines have costs recalculated after Copy Order finishes
        public delegate void CopyOrderProcDelegate(SOOrder sourceOrder, PX.Objects.SO.CopyParamFilter copyFilter);
        [PXOverride]
        public virtual void CopyOrderProc(SOOrder sourceOrder, PX.Objects.SO.CopyParamFilter copyFilter, CopyOrderProcDelegate baseMethod)
        {
            baseMethod(sourceOrder, copyFilter);

            PXCache lineCache = Base.Transactions.Cache;
            foreach (SOLine line in Base.Transactions.Select().RowCast<SOLine>())
            {
                TryRecalculateUnitCost(lineCache,line);
                RecalculateExtendedCost(lineCache,line);
                lineCache.Update(line);
            }
        }

        public delegate IEnumerable CreateShipmentIssueDelegate(PXAdapter adapter, Nullable<DateTime> shipDate, Nullable<Int32> siteID);
        [PXOverride]
        public IEnumerable CreateShipmentIssue(PXAdapter adapter, Nullable<DateTime> shipDate, Nullable<Int32> siteID, CreateShipmentIssueDelegate baseMethod)
        {
            PXGraph.InstanceCreated.AddHandler<SOShipmentEntry>((graphShipmentEntry) =>
            {
                graphShipmentEntry.RowPersisting.AddHandler<SOShipment>((sender, e) =>
                {
                    var AttributeBILLCOMPLE = Base.Document.Cache.GetValueExt(Base.Document.Current, "AttributeBILLCOMPLE");
                    if (AttributeBILLCOMPLE != null)
                        graphShipmentEntry.Document.Cache.SetValueExt(graphShipmentEntry.Document.Current, "AttributeBILLCOMPLE", AttributeBILLCOMPLE.ToString());
                    var printMethod = Base.Document.Cache.GetValueExt(Base.Document.Current, "AttributeFORMTYPE");
                    if (printMethod != null)
                        graphShipmentEntry.Document.Cache.SetValueExt(graphShipmentEntry.Document.Current, "AttributeFORMTYPE", printMethod.ToString());
                });
            });
            return baseMethod(adapter, shipDate, siteID);
        }
        #endregion

        #region Event Handlers

        #region Events

        protected virtual void _(Events.FieldDefaulting<SOLine, SOLine.curyUnitCost> e)
        {
            if (e.Row == null) return;

            SOLine line = e.Row;

            // Get the inventory item
            InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
            if (item?.InventoryID != null)
            {
                var itemExt = item.GetExtension<InventoryItemExt>();
                if ((itemExt?.UsrSWKRTHCost ??0m) >0m)
                {
                    // Calculate Unit Cost with UOM conversion
                    decimal calculatedCost = CalculateUnitCostWithUOM(line, item, itemExt.UsrSWKRTHCost.Value);
                    e.NewValue = calculatedCost;
                    e.Cancel = true;
                }
            }
        }
        protected virtual void _(Events.FieldSelecting<SOOrder, SOOrderExt.usrFreightTotal> e)
        {
            if (e.Row == null) return;

            SOOrder order = e.Row;
            
            // Calculate freight total from all shipment lines regardless of status
            // Handles different shipment types: Issue, DropShip, Transfer, Invoice
            decimal totalFreight = 0m;

            // Get all SOOrderShipment records (lines under Shipments tab)
            var shipmentLines = PXSelect<SOOrderShipment,
                Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                    And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                .Select(Base, order.OrderType, order.OrderNbr);

            foreach (SOOrderShipment orderShipment in shipmentLines.RowCast<SOOrderShipment>())
            {
                if (orderShipment.ShipmentType == SOShipmentType.DropShip)
                {
                    // DropShip: Get freight from PO Receipt via ShippingRefNoteID
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
                    // Issue, Transfer, or other types: Get freight from SOShipment
                    if (!string.IsNullOrEmpty(orderShipment.ShipmentNbr) && 
                        orderShipment.ShipmentNbr != PX.Objects.SO.Constants.NoShipmentNbr)
                    {
                        SOShipment shipment = PXSelect<SOShipment,
                            Where<SOShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                                And<SOShipment.shipmentType, Equal<Required<SOShipment.shipmentType>>>>>
                            .Select(Base, orderShipment.ShipmentNbr, orderShipment.ShipmentType);

                        if (shipment != null)
                        {
                            totalFreight += (shipment.CuryFreightAmt ?? 0m);
                        }
                    }
                }
            }

            e.ReturnValue = totalFreight;
        }

        protected virtual void _(Events.RowSelected<SOOrder> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            SOOrder order = (SOOrder)e.Row;
            if (order == null) return;

            // Recompute prepayment to reflect custom total in UI
            RecalculatePrepaymentRequiredAmount(order);

            SOOrderType orderType = SOOrderType.PK.Find(Base, order.OrderType);
            SOOrderTypeExt typeExt = orderType.GetExtension<SOOrderTypeExt>();
            bool isEditable = order.Status == SOOrderStatus.Open ||
                                           order.Status == SOOrderStatus.Hold ||
                                           order.Status == SOOrderStatus.CreditHold;
            SalesPeople.View.AllowUpdate = isEditable;
            SalesPeople.View.AllowInsert = isEditable;
            SalesPeople.View.AllowDelete = isEditable;

            PXUIFieldAttribute.SetVisible<SOLineExt.usrVendorID>(Base.Transactions.Cache, null, typeExt?.UsrShowVendorID ?? false);
            PXUIFieldAttribute.SetVisible<SOLineExt.usrVendorLocationID>(Base.Transactions.Cache, null, typeExt?.UsrShowVendorLocationID ?? false);
            PXUIFieldAttribute.SetVisible<SOLineExt.usrVendorAddress>(Base.Transactions.Cache, null, typeExt?.UsrShowVendorAddress ?? false);
            PXUIFieldAttribute.SetVisibility<SOLineExt.usrVendorID>(Base.Transactions.Cache, null,
                (typeExt?.UsrShowVendorID ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible);
            PXUIFieldAttribute.SetVisibility<SOLineExt.usrVendorLocationID>(Base.Transactions.Cache, null,
                (typeExt?.UsrShowVendorLocationID ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible);

            PXUIFieldAttribute.SetVisibility<SOLineExt.usrVendorAddress>(Base.Transactions.Cache, null,
                (typeExt?.UsrShowVendorAddress ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible);

            PXUIFieldAttribute.SetEnabled<SOOrderExt.usrHubspotDealID>(e.Cache, order, false);
            
            // Shipping Instructions should only be editable when status is Hold
            bool isOnHold = order.Status == SOOrderStatus.Hold;
            PXUIFieldAttribute.SetEnabled<SOOrderExt.usrShippingInstructions>(e.Cache, order, isOnHold);
        }

        protected virtual void _(Events.FieldUpdated<SOOrder, SOOrder.prepaymentReqPct> e)
        {
            var order = (SOOrder)e.Row;
            if (order == null) return;
            RecalculatePrepaymentRequiredAmount(order);
        }

        protected virtual void _(Events.FieldUpdated<SOOrderExt.usrRTHCuryOrderTotal> e)
        {
            var order = Base.Document.Current;
            if (order == null) return;
            RecalculatePrepaymentRequiredAmount(order);
        }

        protected virtual void _(Events.RowSelected<SOLine> e)
        {
            if (e.Row == null) return;

            var lineExt = e.Row.GetExtension<SOLineExt>();

            //Enable / disable SPC Code based on SPC Cost
            PXUIFieldAttribute.SetEnabled<SOLineExt.usrSWKSPCCode>(e.Cache, e.Row,
                (lineExt?.UsrSWKSPCCost ?? 0) > 0);

            // Manual Cost checkbox is always disabled (read-only)
            PXUIFieldAttribute.SetEnabled<SOLineExt.usrSWKManualCost>(e.Cache, e.Row, false);
        }

        protected virtual void _(Events.RowInserted<SOLine> e)
        {
            var row = e.Row as SOLine;
            if (row == null) return;
            var ext = PXCache<SOLine>.GetExtension<SOLineExt>(row);
            if (ext == null) return;

            // Only assign if not already set
            if (ext.UsrATAIRTHLineNbr == null)
            {
                var lines = Base.Transactions.Select().RowCast<SOLine>()
                    .Where(l => PXCache<SOLine>.GetExtension<SOLineExt>(l)?.UsrATAIRTHLineNbr != null)
                    .ToList();
                int maxNbr = lines.Count >0 ? lines.Max(l => PXCache<SOLine>.GetExtension<SOLineExt>(l).UsrATAIRTHLineNbr ??0) :0;
                ext.UsrATAIRTHLineNbr = maxNbr +1;
                e.Cache.SetValueExt<SOLineExt.usrATAIRTHLineNbr>(row, ext.UsrATAIRTHLineNbr);
            }

            // Force unit/extended cost recomputation for every inserted line (copy order inserts multiple lines)
            TryRecalculateUnitCost(e.Cache, row);
            RecalculateExtendedCost(e.Cache, row);
        }

        protected virtual void _(Events.RowDeleted<SOLine> e)
        {
            if (e.Row == null) return;
            // Skip renumbering when entire order is being deleted to avoid aggregate validation error.
            if (_isDocumentDeleting || Base.Document.Cache.GetStatus(Base.Document.Current) == PXEntryStatus.Deleted)
                return;

            // Renumber all remaining lines after an individual delete
            var lines = Base.Transactions.Select().RowCast<SOLine>()
                .Where(l => Base.Transactions.Cache.GetStatus(l) != PXEntryStatus.Deleted)
                .OrderBy(l => l.LineNbr)
                .ToList();

            int nbr =1;
            foreach (var line in lines)
            {
                var ext = PXCache<SOLine>.GetExtension<SOLineExt>(line);
                if (ext != null && ext.UsrATAIRTHLineNbr != nbr)
                {
                    Base.Transactions.Cache.SetValueExt<SOLineExt.usrATAIRTHLineNbr>(line, nbr);
                }
                nbr++;
            }

            // Update header mirrors & totals
            var order = Base.Document.Current;
            if (order != null)
            {
                RecalculateRthCuryOrderTotal(order);
                Base.Document.Cache.MarkUpdated(order);
                Base.Document.View.RequestRefresh();
            }
            Base.Transactions.View.RequestRefresh();
        }
        #endregion

        protected virtual void _(Events.FieldUpdated<SOOrder, SOOrder.customerID> e)
        {
            var order = (SOOrder)e.Row;
            if (order == null) return;

            // Get the selected customer record
            Customer customer = PXSelect<Customer,
                Where<Customer.bAccountID, Equal<Required<SOOrder.customerID>>>>
                .Select(Base, order.CustomerID);



            if (customer != null)
            {
                // Get the DAC extension for Customer
                var customerExt = customer.GetExtension<CustomerExt>();
                // Get the DAC extension for SOOrder
                var orderExt = order.GetExtension<SOOrderExt>();

                if (customerExt != null && orderExt != null)
                {
                    orderExt.UsrShippingInstructions = customerExt.UsrShippingInstructions;
                    e.Cache.SetValueExt<SOOrderExt.usrShippingInstructions>(order, customerExt.UsrShippingInstructions);
                }
            }
            UpdateCustomerAccount(e.Cache, e.Row as SOOrder);

            ClearSalesPerson();

            foreach (CustSalesPeople item in SelectFrom<CustSalesPeople>.Where<CustSalesPeople.bAccountID.IsEqual<SOOrder.customerID.FromCurrent>>.View.Select(Base))
            {
                //insert in SOCustSalesPeople
                SOCustSalesPeople newItem = SalesPeople.Insert();
                newItem.SalesPersonID = item.SalesPersonID;
                newItem.IsDefault = item.IsDefault;
                newItem.CommisionPct = item.CommisionPct;
            }
        }


        protected virtual bool HasMiscLinesToInvoice(SOOrder order)
            => (order.OrderQty == 0 || order.OpenLineCntr == 0 && order.IsLegacyMiscBilling == false)
                && (order.CuryUnbilledMiscTot != 0 || order.UnbilledOrderQty > 0);


        public void ClearSalesPerson()
        {
            foreach (var item in SalesPeople.Select())
            {
                SalesPeople.Cache.Delete(item);
                SalesPeople.Delete(item);
            }
        }


        protected virtual void _(Events.FieldUpdated<SOOrder, SOOrder.customerLocationID> e)
        {
            var order = (SOOrder)e.Row;
            if (order == null) return;

            // Get the selected customer record
            Customer customer = PXSelect<Customer,
                Where<Customer.bAccountID, Equal<Required<SOOrder.customerID>>>>
                .Select(Base, order.CustomerID);

            if (customer != null)
            {
                // Get the DAC extension for Customer
                var customerExt = customer.GetExtension<CustomerExt>();
                // Get the DAC extension for SOOrder
                var orderExt = order.GetExtension<SOOrderExt>();

                if (customerExt != null && orderExt != null)
                {
                    orderExt.UsrShippingInstructions = customerExt.UsrShippingInstructions;
                    e.Cache.SetValueExt<SOOrderExt.usrShippingInstructions>(order, customerExt.UsrShippingInstructions);
                }
            }
            UpdateCustomerAccount(e.Cache, e.Row as SOOrder);
        }

        protected void SOOrder_UseCustomerAccount_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            UpdateCustomerAccount(sender, e.Row as SOOrder);
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.inventoryID> e)
        {
            if (e.Row == null) return;
            
            // Copy default vendor to UsrVendorID
            var lineExt = e.Row.GetExtension<SOLineExt>();
            POVendorInventory defaultVendor = null;
            foreach (POVendorInventory defVen in PXSelect<POVendorInventory,
                Where<POVendorInventory.inventoryID, Equal<Required<POVendorInventory.inventoryID>>>>
                .Select(Base, e.Row.InventoryID))
            {
                if (defVen.IsDefault == true)
                {
                    defaultVendor = defVen;
                    break;
                }
            }

            if (defaultVendor != null)
            {
                e.Cache.SetValueExt<SOLineExt.usrVendorID>(e.Row, defaultVendor.VendorID);
                e.Cache.SetValueExt<SOLineExt.usrVendorLocationID>(e.Row, defaultVendor.VendorLocationID);

                Address address = PXSelect<Address,
                    Where<Address.bAccountID, Equal<Required<Address.bAccountID>>>>
                    .Select(Base, defaultVendor.VendorID);

                if (address != null)
                {
                    lineExt.UsrVendorAddress = string.Format("{0}{1}{2}, {3} {4}",
                        address.AddressLine1 ?? "",
                        string.IsNullOrWhiteSpace(address.AddressLine2) ? "" : " " + address.AddressLine2,
                        string.IsNullOrWhiteSpace(address.City) ? "" : ", " + address.City,
                        address.State ?? "",
                        address.PostalCode ?? ""
                    ).Trim();
                }
                else
                {
                    lineExt.UsrVendorAddress = null;
                }
            }

            TryRecalculateUnitCost(e.Cache, e.Row);
            if (lineExt?.UsrSWKSPCCost >0)
            {
                // Automatically check the Manual Cost checkbox when SPC Cost >0
                lineExt.UsrSWKManualCost = true;

                // Force validation of SPC Code field
                try
                {
                    if (!SuppressCodeRequired)
                        e.Cache.RaiseExceptionHandling<SOLineExt.usrSWKSPCCode>(e.Row, lineExt.UsrSWKSPCCode,
                            string.IsNullOrEmpty(lineExt.UsrSWKSPCCode) ?
                            new PXSetPropertyException(e.Row,Messages.SPCCodeRequired, PXErrorLevel.Error) :
                            null);
                }
                catch { }
            }
            else if (lineExt?.UsrSWKSPCCost ==0)
            {
                // Clear SPC Code when SPC Cost is zero
                lineExt.UsrSWKSPCCode = null;
                lineExt.UsrSWKManualCost = false;
            }

            // Recalculate Extended Cost
            RecalculateExtendedCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLineExt.usrSWKSPCCode> e)
        {
            if (e.Row == null) return;

            var lineExt = e.Row.GetExtension<SOLineExt>();
            if (!string.IsNullOrEmpty(lineExt?.UsrSWKSPCCode) && (lineExt.UsrSWKSPCCost ??0) ==0)
            {
                // If SPC Code is entered but SPC Cost is0, clear the code
                lineExt.UsrSWKSPCCode = null;
                if (!SuppressCodeRequired)
                    e.Cache.RaiseExceptionHandling<SOLineExt.usrSWKSPCCode>(e.Row, null,
                    new PXSetPropertyException(e.Row,Messages.SPCCodeOnlyWithCost, PXErrorLevel.Warning));
            }
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.uOM> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLineExt.usrSWKSPCCost> e)
        {
            if (e.Row == null) return;

            var lineExt = e.Row.GetExtension<SOLineExt>();
            // Toggle Manual Cost based on SPC Cost and enforce/clear SPC Code requirement
            if ((lineExt?.UsrSWKSPCCost ??0m) >0m)
            {
                lineExt.UsrSWKManualCost = true;
                try
                {
                    if (!SuppressCodeRequired && !Base.IsCopyPasteContext)
                        e.Cache.RaiseExceptionHandling<SOLineExt.usrSWKSPCCode>(e.Row, lineExt.UsrSWKSPCCode,
                            string.IsNullOrEmpty(lineExt.UsrSWKSPCCode)
                                ? new PXSetPropertyException(e.Row, Messages.SPCCodeRequired, PXErrorLevel.Error)
                                : null);
                }
                catch { }
            }
            else
            {
                // If SPC Cost is zero/null, clear SPC Code and uncheck Manual Cost
                lineExt.UsrSWKManualCost = false;
                lineExt.UsrSWKSPCCode = null;
                try
                {
                    e.Cache.RaiseExceptionHandling<SOLineExt.usrSWKSPCCode>(e.Row, null, null);
                }
                catch { }
            }

            TryRecalculateUnitCost(e.Cache, e.Row);
            RecalculateExtendedCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldVerifying<SOLine, SOLineExt.usrSWKSPCCode> e)
        {
            if (e.Row == null) return;

            // Skip validation if this is an import scenario
            if (e.Cache.Graph.IsImport || Base.IsImport)
                return;

            var lineExt = e.Row.GetExtension<SOLineExt>();
            if (lineExt?.UsrSWKSPCCost >0 && string.IsNullOrEmpty((string)e.NewValue))
            {
                if (!SuppressCodeRequired)
                    throw new PXSetPropertyException(e.Row,Messages.SPCCodeRequired, PXErrorLevel.Error);
            }
        }

        // Recalculate when Warehouse changes
        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.siteID> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        // Recalculate when Subitem changes (often part of cost/stock context)
        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.subItemID> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        // If branch affects pricing context in your setup, keep this. Otherwise you may remove it.
        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.branchID> e)
        {
            if (e.Row == null) return;
            TryRecalculateUnitCost(e.Cache, e.Row);
        }
        protected void SOOrder_ShipVia_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            UpdateCustomerAccount(sender, e.Row as SOOrder);
        }

        protected void SOOrder_CustomerLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            UpdateCustomerAccount(sender, e.Row as SOOrder);
        }

        protected virtual void SOOrder_OrderQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHOrderQty = row.OrderQty;
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.orderQty> e)
        {
            if (e.Row == null) return;

            // Recalculate Extended Cost when quantity changes
            RecalculateExtendedCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldUpdated<SOLine, SOLine.curyUnitCost> e)
        {
            if (e.Row == null) return;

            // Recalculate Extended Cost when unit cost changes
            RecalculateExtendedCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldUpdated<SOLineExt.usrVendorID> e)
        {
            SOLine line = (SOLine)e.Row;
            if (line == null) return;

            SOLineExt lineExt = line.GetExtension<SOLineExt>();
            POVendorInventory vendorInv = PXSelect<POVendorInventory,
                Where<POVendorInventory.inventoryID, Equal<Required<SOLine.inventoryID>>,
                    And<POVendorInventory.vendorID, Equal<Required<SOLine.vendorID>>,
                    And<POVendorInventory.isDefault,Equal<True>>>>> 
                .Select(Base, line.InventoryID, lineExt.UsrVendorID);

            lineExt.UsrVendorLocationID = vendorInv?.VendorLocationID;

            PX.Objects.CR.Location loc = PX.Objects.CR.Location.PK.Find(Base, vendorInv?.VendorID, vendorInv?.VendorLocationID);
            Address address = PXSelect<Address,
                Where<Address.addressID, Equal<Required<Address.addressID>>>>
                .Select(Base, loc?.DefAddressID);

            if (address != null)
            {
                lineExt.UsrVendorAddress = string.Format("{0}{1}{2}, {3} {4}",
                    address.AddressLine1 ?? "",
                    string.IsNullOrWhiteSpace(address.AddressLine2) ? "" : " " + address.AddressLine2,
                    string.IsNullOrWhiteSpace(address.City) ? "" : ", " + address.City,
                    address.State ?? "",
                    address.PostalCode ?? ""
                ).Trim();
            }
            else
            {
                lineExt.UsrVendorAddress = null;
            }
        }

        protected virtual void _(Events.FieldUpdated<SOLineExt.usrVendorLocationID> e)
        {
            SOLine line = (SOLine)e.Row;
            if (line == null) return;
            SOLineExt lineExt = line.GetExtension<SOLineExt>();

            PX.Objects.CR.Location loc = PX.Objects.CR.Location.PK.Find(Base, lineExt?.UsrVendorID, lineExt?.UsrVendorLocationID);
            Address address = PXSelect<Address,
                Where<Address.addressID, Equal<Required<Address.addressID>>>>
                .Select(Base, loc?.DefAddressID);

            if (address != null)
            {
                lineExt.UsrVendorAddress = string.Format("{0}{1}{2}, {3} {4}",
                    address.AddressLine1 ?? "",
                    string.IsNullOrWhiteSpace(address.AddressLine2) ? "" : " " + address.AddressLine2,
                    string.IsNullOrWhiteSpace(address.City) ? "" : ", " + address.City,
                    address.State ?? "",
                    address.PostalCode ?? ""
                ).Trim();
            }
            else
            {
                lineExt.UsrVendorAddress = null;
            }
        }

        protected virtual void SOOrder_CuryDetailExtPriceTotal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryDetailExtPriceTotal = row.CuryDetailExtPriceTotal;
        }

        protected virtual void SOOrder_CuryLineDiscTotal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryLineDiscTotal = row.CuryLineDiscTotal;
        }

        protected virtual void SOOrder_CuryDiscTot_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryDiscTot = row.CuryDiscTot;
        }

        protected virtual void SOOrder_CuryFreightTot_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            if((rowExt.UsrFreightPriceLimit ??0m) <=0m)
                rowExt.UsrRTHCuryFreightTot = row.CuryFreightTot;
            RecalculateRthCuryOrderTotal(row);
        }
        protected virtual void _(Events.FieldUpdated<SOOrderExt.usrFreightPriceLimit> e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryFreightTot = ((decimal?)e.NewValue ??0m);
            RecalculateRthCuryOrderTotal(row);
        }

        protected virtual void SOOrder_CuryTaxTotal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryTaxTotal = row.CuryTaxTotal;
        }

        protected virtual void _(Events.FieldUpdated<SOOrderExt.usrRTHCuryDetailExtPriceTotal> e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            RecalculateRthCuryOrderTotal(row);
        }

        protected virtual void _(Events.FieldUpdated<SOOrderExt.usrRTHcuryLineDiscTotal> e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            RecalculateRthCuryOrderTotal(row);
        }

        protected virtual void _(Events.FieldUpdated<SOOrderExt.usrRTHCuryDiscTot> e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            RecalculateRthCuryOrderTotal(row);
        }

        protected virtual void SOOrder_UsrRTHCuryFreightTot_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            RecalculateRthCuryOrderTotal(row);
        }

        protected virtual void _(Events.RowUpdated<SOLine> e, PXRowUpdated del) 
        {
            del?.Invoke(e.Cache, e.Args);
            SOLine row = (SOLine)e.Row;
            if (row == null) return;
            RecalculateRthCuryOrderTotal(Base.Document.Current);
        }
        protected virtual void _(Events.FieldUpdated<SOOrderExt.usrRTHCuryTaxTotal> e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            RecalculateRthCuryOrderTotal(row);
        }

        public void RecalculateRthCuryOrderTotal(SOOrder row)
        {
            if (row == null) return;

            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            if (rowExt == null) return;

            // Read from SOOrder base (authoritative) totals
            decimal detail = row.CuryDetailExtPriceTotal ??0m;
            decimal lineDisc = row.CuryLineDiscTotal ??0m;
            decimal docDisc = row.CuryDiscTot ??0m;
            decimal freight = row.CuryFreightTot ??0m;
            decimal tax = row.CuryTaxTotal ??0m;

            // Cap/override freight if a limit is provided
            if ((rowExt.UsrFreightPriceLimit ??0m) >0m)
                rowExt.UsrRTHCuryFreightTot = rowExt.UsrFreightPriceLimit;
            else
                rowExt.UsrRTHCuryFreightTot = freight;

            // Keep mirrors in sync (no SetValueExt to avoid recursive events)
            rowExt.UsrRTHCuryDetailExtPriceTotal = detail;
            rowExt.UsrRTHCuryLineDiscTotal = lineDisc;
            rowExt.UsrRTHCuryDiscTot = docDisc;
            rowExt.UsrRTHCuryTaxTotal = tax;

            // Final custom total
            rowExt.UsrRTHCuryOrderTotal =
                (detail - (lineDisc + docDisc))
                + (rowExt.UsrRTHCuryFreightTot ??0m)
                + tax;

            // Mark header dirty to reflect UI changes
            Base.Document.Cache.MarkUpdated(row);
        }

        protected virtual void _(Events.RowPersisting<SOLine> e)
        {
            if (e.Row == null) return;

            // Skip validation if this is an import scenario
            if (e.Cache.Graph.IsImport || Base.IsImport)
                return;

            var lineExt = e.Row.GetExtension<SOLineExt>();

            // Validate SPC Code is provided when SPC Cost >0
            if ((lineExt?.UsrSWKSPCCost ??0) >0 && string.IsNullOrEmpty(lineExt?.UsrSWKSPCCode))
            {
                if (!SuppressCodeRequired)
                    e.Cache.RaiseExceptionHandling<SOLineExt.usrSWKSPCCode>(e.Row, lineExt?.UsrSWKSPCCode,
                    new PXSetPropertyException(e.Row,Messages.SPCCodeRequired, PXErrorLevel.Error));
            }
        }

        // Override the Extended Cost calculation
        [PXMergeAttributes]
        [PXFormula(typeof(Switch<
            Case<Where<SOLineExt.usrSWKSPCCost, Greater<decimal0>>,
                Mult<SOLine.orderQty, SOLineExt.usrSWKSPCCost>>,
            Mult<SOLine.orderQty, SOLine.unitCost>>))]
        protected virtual void _(Events.CacheAttached<SOLine.extCost> e) { }

        // Use RTH custom order total for prepayment required amount
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXRemoveBaseAttribute(typeof(PXFormulaAttribute))]
        [PXFormula(typeof(Switch<
            Case<Where<SOOrder.overridePrepayment, NotEqual<True>>,
                Div<Mult<SOOrder.prepaymentReqPct, SOOrderExt.usrRTHCuryOrderTotal>, decimal100>>,
            SOOrder.curyPrepaymentReqAmt>))]
        protected virtual void _(Events.CacheAttached<SOOrder.curyPrepaymentReqAmt> e) { }

        private void UpdateCustomerAccount(PXCache sender, SOOrder order)
        {
            if (order == null) return;

            var orderExt = order.GetExtension<SOOrderExt>();
            if (orderExt != null)
            {
                // Only proceed if all required fields are set
                if (string.IsNullOrEmpty(order.ShipVia) || order.CustomerID == null || order.CustomerLocationID == null || !(order.UseCustomerAccount ?? false))
                {
                    sender.SetValueExt<SOOrderExt.usrCustomerAccount>(order, null);
                    return;
                }

                Carrier shipVia = PXSelect<Carrier,
                    Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>.Select(Base, order?.ShipVia);

                // Query the CarrierCustomer table for a matching record
                CarrierPluginCustomer carrierCustomer = PXSelect<CarrierPluginCustomer,
                          Where<CarrierPluginCustomer.carrierPluginID, Equal<Required<CarrierPluginCustomer.carrierPluginID>>,
                            And<CarrierPluginCustomer.customerID, Equal<Required<CarrierPluginCustomer.customerID>>,
                            And<CarrierPluginCustomer.customerLocationID, Equal<Required<CarrierPluginCustomer.customerLocationID>>,
                            And<CarrierPluginCustomer.isActive, Equal<True>>>>>>
                             .Select(Base, shipVia?.CarrierPluginID, order?.CustomerID, order?.CustomerLocationID);


                if (carrierCustomer == null)
                {
                    carrierCustomer = PXSelect<CarrierPluginCustomer,
                   Where<CarrierPluginCustomer.carrierPluginID, Equal<Required<CarrierPluginCustomer.carrierPluginID>>,
                     And<CarrierPluginCustomer.customerID, Equal<Required<CarrierPluginCustomer.customerID>>,
                 And<CarrierPluginCustomer.isActive, Equal<True>>>>>
                      .Select(Base, shipVia?.CarrierPluginID, order?.CustomerID);
                }


                if (carrierCustomer != null)
                {
                    orderExt.UsrCustomerAccount = carrierCustomer.CarrierAccount;
                    sender.SetValueExt<SOOrderExt.usrCustomerAccount>(order, orderExt.UsrCustomerAccount);
                }
                else
                {
                    orderExt.UsrCustomerAccount = null;
                    sender.SetValueExt<SOOrderExt.usrCustomerAccount>(order, null);
                }
            }
        }
        private void RecalculateExtendedCost(PXCache cache,SOLine line)
        {
            if (line == null) return;

            var lineExt = line.GetExtension<SOLineExt>();

            // Calculate Extended Cost: 
            // If SPC Cost >0, use SPC Cost * Quantity (no UOM conversion)
            // Otherwise, use standard Unit Cost * Quantity calculation
            decimal extendedCost =0m;

            if ((lineExt?.UsrSWKSPCCost ??0) >0)
            {
                // Use SPC Cost without UOM calculation
                extendedCost = (lineExt.UsrSWKSPCCost ??0) * (line.OrderQty ??0);
            }
            else
            {
                // Use standard calculation: Unit Cost * Quantity
                extendedCost = (line.CuryUnitCost ??0) * (line.OrderQty ??0);
            }

            // Set the Extended Cost
            cache.SetValueExt<SOLine.curyExtCost>(line, extendedCost);
        }
        private bool TryGetNonStockError(string ordertype, string orderNbr, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(orderNbr))
                return false;

            var setupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
            if (setupExt == null) return false;

            var nonStockItems = new List<int?>();
            if (setupExt.UsrNonstock1 != null) nonStockItems.Add(setupExt.UsrNonstock1);
            if (setupExt.UsrNonstock2 != null) nonStockItems.Add(setupExt.UsrNonstock2);
            if (setupExt.UsrNonstock3 != null) nonStockItems.Add(setupExt.UsrNonstock3);
            if (nonStockItems.Count ==0)
                return false;

            var lines = PXSelect<SOLine,
                Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                    And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>>>>
                .Select(Base, ordertype, orderNbr)
                .RowCast<SOLine>();

            var invalidNonstock = new List<string>();


            foreach (SOLine line in lines)
            {
                if (nonStockItems.Contains(line.InventoryID))
                {
                    InventoryItem item = PXSelect<InventoryItem,
                        Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                        .Select(Base, line.InventoryID);

                    if (item != null)
                        invalidNonstock.Add(item.InventoryCD);

                    // Mark the line field so if user opens the shipment they see the problematic lines
                    PXUIFieldAttribute.SetError<SOLine.inventoryID>(Base.Transactions.Cache, line,
                        "You cannot invoice this non-stock item.");
                }
            }

            if (invalidNonstock.Count >0)
            {
                errorMessage = Messages.CannotInvoiceNonStockItems(string.Join(", ", invalidNonstock));
                return true;
            }

            return false;
        }

        public void CheckItemsForFlaggedNonStockItem()
        {
            List<int?> nonStockItems = new List<int?>();
            List<string> invalidNonstock = new List<string>();
            SOSetupExt setupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
            if (setupExt == null) return;
            if (setupExt.UsrNonstock1 != null)
            {
                nonStockItems.Add(setupExt.UsrNonstock1);
            }
            if (setupExt.UsrNonstock2 != null)
            {
                nonStockItems.Add(setupExt.UsrNonstock2);
            }
            if (setupExt.UsrNonstock3 != null)
            {
                nonStockItems.Add(setupExt.UsrNonstock3);
            }

            var lines = PXSelect<SOLine,
                Where<SOLine.origOrderType, Equal<Required<SOLine.orderType>>,
                    And<SOLine.origOrderNbr, Equal<Required<SOLine.orderNbr>>>>>
                .Select(Base, Base.Document.Current.OrderType,Base.Document.Current.OrderNbr)
                .RowCast<SOLine>();

            foreach (SOLine line in lines)
            {
                bool hasError = false;
                //check if the line's inventory ID is in the list of non-stock items, then show a popup stating user cannot invoice the non-stock item
                if (nonStockItems.Contains(line.InventoryID))
                {
                    InventoryItem inventoryItem = PXSelect<InventoryItem,
                        Where<InventoryItem.inventoryID, Equal<Required<SOLine.inventoryID>>>>
                        .Select(Base, line.InventoryID);
                    invalidNonstock.Add(inventoryItem.InventoryCD);

                    PXUIFieldAttribute.SetWarning<SOLine.inventoryID>(Base.Transactions.Cache, line, "You cannot invoice this non-stock item.");
                    hasError = true;
                }
                if (hasError)
                {
                    // Show a popup or message to the user
                    throw new PXException(Messages.CannotInvoiceNonStockItems(string.Join(", ", invalidNonstock)));
                }
            }
        }

        private void TryRecalculateUnitCost(PXCache cache,SOLine line)
        {
            if (line == null || line.InventoryID == null)
                return;

            InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
            if (item?.InventoryID == null)
                return;

            var itemExt = item.GetExtension<InventoryItemExt>();
            var soLineExt = line.GetExtension<SOLineExt>();
            soLineExt.UsrSWKRTHCost = (itemExt?.UsrSWKRTHCost ??0m);
            // If neither RTH nor SPC cost provided, do nothing
            if ((itemExt?.UsrSWKRTHCost ??0m) <=0m && (soLineExt?.UsrSWKSPCCost ??0m) <=0m)
                return;

            decimal rthCost = (itemExt?.UsrSWKRTHCost ??0m);
            decimal calculatedCost = CalculateUnitCostWithUOM(line, item, rthCost);

            // Set CuryUnitCost so ExtCost recalculates via PXFormula
            cache.SetValueExt<SOLine.curyUnitCost>(line, calculatedCost);
        }

        protected virtual decimal CalculateUnitCostWithUOM(SOLine line, InventoryItem item, decimal rthCost)
        {
            SOLineExt soLineExt = line.GetExtension<SOLineExt>();
            if ((soLineExt?.UsrSWKSPCCost ??0m) >0m)
                return (soLineExt?.UsrSWKSPCCost ??0m);

            if (line?.UOM == null || item?.BaseUnit == null)
                return rthCost;

            // If SO line UOM == item base UOM, return RTH Cost as-is
            if (string.Equals(line.UOM, item.BaseUnit, System.StringComparison.OrdinalIgnoreCase))
            {
                return rthCost;
            }

            // If SO line UOM != item base UOM, apply UOM conversion
            // Find the conversion rate from SO line UOM to base UOM
            INUnit conversion = PXSelect<INUnit,
                Where<INUnit.inventoryID, Equal<Required<INUnit.inventoryID>>,
                    And<INUnit.fromUnit, Equal<Required<INUnit.fromUnit>>,
                    And<INUnit.toUnit, Equal<Required<INUnit.toUnit>>>>> >
                .Select(Base, item.InventoryID, line.UOM, item.BaseUnit);

            if (conversion != null && conversion.UnitRate != null && conversion.UnitRate !=0)
            {
                // Apply UOM conversion based on UnitMultDiv
                if (conversion.UnitMultDiv == "M") // Multiply
                {
                    return rthCost * conversion.UnitRate.Value;
                }
                else if (conversion.UnitMultDiv == "D") // Divide
                {
                    return rthCost / conversion.UnitRate.Value;
                }
            }

            // If no conversion found, return original RTH Cost
            return rthCost;
        }

        #endregion
    }
}
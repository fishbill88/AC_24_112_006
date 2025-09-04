using CompiledVersion.DAC;
using PX.Common;
using PX.Data;
using PX.Data.Licensing;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;
using PX.Objects.CR.Standalone;
using PX.Data.BQL.Fluent;

namespace CompiledVersion.Graphs
{
    public class SOOrderEntry_Extension : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() => true;

        public bool SuppressCodeRequired = false;

        #region Views
        public SelectFrom<SOCustSalesPeople>.Where<SOCustSalesPeople.orderType.IsEqual<SOOrder.orderType.FromCurrent>
            .And<SOCustSalesPeople.orderNbr.IsEqual<SOOrder.orderNbr.FromCurrent>>>.View SalesPeople;
        #endregion

        #region Overrides

        public delegate IEnumerable PrepareInvoiceDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable PrepareInvoice(PXAdapter adapter, PrepareInvoiceDelegate baseMethod)
        {
            CheckItemsForFlaggedNonStockItem();
            return baseMethod(adapter);
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
                if ((itemExt?.UsrSWKRTHCost ?? 0m) > 0m)
                {
                    // Calculate Unit Cost with UOM conversion
                    decimal calculatedCost = CalculateUnitCostWithUOM(line, item, itemExt.UsrSWKRTHCost.Value);
                    e.NewValue = calculatedCost;
                    e.Cancel = true;
                }
            }
        }
        protected virtual void _(Events.RowSelected<SOOrder> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            SOOrder order = (SOOrder)e.Row;
            if (order == null) return;

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
        }

        protected virtual void _(Events.RowSelected<SOLine> e)
        {
            if (e.Row == null) return;

            var lineExt = e.Row.GetExtension<SOLineExt>();

            // Enable/disable SPC Code based on SPC Cost
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
                int maxNbr = lines.Count > 0 ? lines.Max(l => PXCache<SOLine>.GetExtension<SOLineExt>(l).UsrATAIRTHLineNbr ?? 0) : 0;
                ext.UsrATAIRTHLineNbr = maxNbr + 1;
                e.Cache.SetValueExt<SOLineExt.usrATAIRTHLineNbr>(row, ext.UsrATAIRTHLineNbr);
            }
        }

        protected virtual void _(Events.RowDeleted<SOLine> e)
        {
            // Renumber all lines after a delete
            var lines = Base.Transactions.Select().RowCast<SOLine>()
                .OrderBy(l => l.LineNbr)
                .ToList();
            int nbr = 1;
            foreach (var line in lines)
            {
                var ext = PXCache<SOLine>.GetExtension<SOLineExt>(line);
                if (ext != null)
                {
                    //e.Cache.SetValueExt<SOLineExt.usrATAIRTHLineNbr>(line, nbr);
                    ext.UsrATAIRTHLineNbr = nbr;
                    e.Cache.Update(line);
                }
                nbr++;
            }
            Base.Transactions.View.RequestRefresh(); // Refresh the view to show updated line numbers
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
            TryRecalculateUnitCost(e.Cache, e.Row);
            var lineExt = e.Row.GetExtension<SOLineExt>();
            if (lineExt?.UsrSWKSPCCost > 0)
            {
                // Automatically check the Manual Cost checkbox when SPC Cost > 0
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
            else if (lineExt?.UsrSWKSPCCost == 0)
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
            if (!string.IsNullOrEmpty(lineExt?.UsrSWKSPCCode) && (lineExt.UsrSWKSPCCost ?? 0) == 0)
            {
                // If SPC Code is entered but SPC Cost is 0, clear the code
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
            TryRecalculateUnitCost(e.Cache, e.Row);
        }

        protected virtual void _(Events.FieldVerifying<SOLine, SOLineExt.usrSWKSPCCode> e)
        {
            if (e.Row == null) return;

            // Skip validation if this is an import scenario
            if (e.Cache.Graph.IsImport || Base.IsImport)
                return;

            var lineExt = e.Row.GetExtension<SOLineExt>();
            if (lineExt?.UsrSWKSPCCost > 0 && string.IsNullOrEmpty((string)e.NewValue))
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

            //PX.Objects.CA.Light.Location location = PXSelect<PX.Objects.CA.Light.Location,
            //    Where<PX.Objects.CA.Light.Location.bAccountID, Equal<Required<PX.Objects.CA.Light.Location.bAccountID>>,
            //        And<PX.Objects.CA.Light.Location.locationID, Equal<Required<PX.Objects.CA.Light.Location.locationID>>>>>
            //    .Select(Base, vendorInv?.VendorID, vendorInv?.VendorLocationID);

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
            if((rowExt.UsrFreightPriceLimit ?? 0m) <= 0m)
                rowExt.UsrRTHCuryFreightTot = row.CuryFreightTot;
            RecalculateRthCuryOrderTotal(row);
        }
        protected virtual void _(Events.FieldUpdated<SOOrderExt.usrFreightPriceLimit> e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryFreightTot = ((decimal?)e.NewValue ?? 0m);
            RecalculateRthCuryOrderTotal(row);
        }

        protected virtual void SOOrder_CuryTaxTotal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryTaxTotal = row.CuryTaxTotal;
        }

        //protected virtual void SOOrder_CuryOrderTotal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        //{
        //    SOOrder row = (SOOrder)e.Row;
        //    if (row == null) return;
        //    SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
        //    //rowExt.UsrRTHCuryOrderTotal = row.CuryOrderTotal;
        //    rowExt.UsrRTHCuryOrderTotal = (rowExt.UsrRTHCuryDetailExtPriceTotal - (rowExt.UsrRTHCuryLineDiscTotal + rowExt.UsrRTHCuryDiscTot)) + rowExt.UsrRTHCuryFreightTot + rowExt.UsrRTHCuryTaxTotal;
        //}

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
        //protected virtual void _(Events.FieldUpdated<SOOrderExt.usrRTHCuryFreightTot> e)
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

            if ((rowExt.UsrFreightPriceLimit ?? 0m) <= 0m)
                rowExt.UsrRTHCuryFreightTot = row.CuryFreightTot;
            else
                rowExt.UsrRTHCuryFreightTot = rowExt.UsrFreightPriceLimit;


            //rowExt.UsrRTHCuryOrderTotal = row.CuryOrderTotal;
            rowExt.UsrRTHCuryOrderTotal = ((rowExt.UsrRTHCuryDetailExtPriceTotal ?? 0m) - ((rowExt.UsrRTHCuryLineDiscTotal ?? 0m) + (rowExt.UsrRTHCuryDiscTot ?? 0m))) +
                                            ((rowExt.UsrFreightPriceLimit ?? 0m) > 0m ? (rowExt.UsrFreightPriceLimit ?? 0m) : (rowExt.UsrRTHCuryFreightTot ?? 0m)) + (rowExt.UsrRTHCuryTaxTotal ?? 0m);
        }

        protected virtual void _(Events.RowPersisting<SOLine> e)
        {
            if (e.Row == null) return;

            // Skip validation if this is an import scenario
            if (e.Cache.Graph.IsImport || Base.IsImport)
                return;

            var lineExt = e.Row.GetExtension<SOLineExt>();

            // Validate SPC Code is provided when SPC Cost > 0
            if ((lineExt?.UsrSWKSPCCost ?? 0) > 0 && string.IsNullOrEmpty(lineExt?.UsrSWKSPCCode))
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
        private void RecalculateExtendedCost(PXCache cache, SOLine line)
        {
            if (line == null) return;

            var lineExt = line.GetExtension<SOLineExt>();

            // Calculate Extended Cost: 
            // If SPC Cost > 0, use SPC Cost * Quantity (no UOM conversion)
            // Otherwise, use standard Unit Cost * Quantity calculation
            decimal extendedCost = 0m;

            if ((lineExt?.UsrSWKSPCCost ?? 0) > 0)
            {
                // Use SPC Cost without UOM calculation
                extendedCost = (lineExt.UsrSWKSPCCost ?? 0) * (line.OrderQty ?? 0);
            }
            else
            {
                // Use standard calculation: Unit Cost * Quantity
                extendedCost = (line.UnitCost ?? 0) * (line.OrderQty ?? 0);
            }

            // Set the Extended Cost
            cache.SetValueExt<SOLine.curyExtCost>(line, extendedCost);
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

            foreach (SOLine line in Base.Transactions.Select())
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

        private void TryRecalculateUnitCost(PXCache cache, SOLine line)
        {
            if (line == null || line.InventoryID == null)
                return;

            InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
            if (item?.InventoryID == null)
                return;

            var itemExt = item.GetExtension<InventoryItemExt>();
            var soLineExt = line.GetExtension<SOLineExt>();
            soLineExt.UsrSWKRTHCost = (itemExt?.UsrSWKRTHCost ?? 0m);
            // If neither RTH nor SPC cost provided, do nothing
            if ((itemExt?.UsrSWKRTHCost ?? 0m) <= 0m && (soLineExt?.UsrSWKSPCCost ?? 0m) <= 0m)
                return;

            decimal rthCost = (itemExt?.UsrSWKRTHCost ?? 0m);
            decimal calculatedCost = CalculateUnitCostWithUOM(line, item, rthCost);

            // Set CuryUnitCost so ExtCost recalculates via PXFormula
            cache.SetValueExt<SOLine.curyUnitCost>(line, calculatedCost);
        }

        protected virtual decimal CalculateUnitCostWithUOM(SOLine line, InventoryItem item, decimal rthCost)
        {
            SOLineExt soLineExt = line.GetExtension<SOLineExt>();
            if ((soLineExt?.UsrSWKSPCCost ?? 0m) > 0m)
                return (soLineExt?.UsrSWKSPCCost ?? 0m);

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
                    And<INUnit.toUnit, Equal<Required<INUnit.toUnit>>>>>>
                .Select(Base, item.InventoryID, line.UOM, item.BaseUnit);

            if (conversion != null && conversion.UnitRate != null && conversion.UnitRate != 0)
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
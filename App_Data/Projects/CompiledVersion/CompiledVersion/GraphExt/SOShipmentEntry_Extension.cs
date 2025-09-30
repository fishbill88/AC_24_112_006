using CompiledVersion.DAC;
using CompiledVersion.Helpers;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.CN.Compliance.PO.CacheExtensions;
using PX.Objects.IN;
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
            // Assuming UsrFreightPriceLimit is a decimal field in SOOrderExt
            decimal? freightLimit = orderExt.UsrFreightPriceLimit ?? 0m;
            decimal? currentFreight = 0m;


            var shipmentlist = PXSelectJoin<SOOrderShipment,
                                    LeftJoin<SOShipment, On<SOShipment.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>,
                                        And<SOShipment.shipmentType, Equal<SOOrderShipment.shipmentType>>>>,
                                        Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                                            And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>,
                                                And<SOShipment.status, Equal<SOShipmentStatus.confirmed>>>>,
                                            OrderBy<Asc<SOOrderShipment.shipmentNbr>>>
                                    .Select(Base, order.OrderType, order.OrderNbr);
            foreach (var item in shipmentlist)
            {

                SOOrderShipment orderShipment = item.GetItem<SOOrderShipment>();
                SOShipment _shipment = item.GetItem<SOShipment>();
                if (_shipment.Status == SOShipmentStatus.Confirmed)
                {
                    currentFreight += (_shipment.CuryFreightAmt ?? 0m);
                }
            }

            // Check if the new freight amount exceeds the limit
            // if newAMt is greater than the limit show an error on the field and replace the value with the limit
            if ((newAmt + currentFreight) > freightLimit && (shipment.OverrideFreightAmount ?? false) && order.ShipTermsID == sOSetupExt.UsrNotToExceed)
            {
                decimal? exceedAmt = (newAmt + currentFreight) - freightLimit;
                //PXUIFieldAttribute.SetError<SOShipment.curyFreightAmt>(e.Cache, shipment, "Freight amount exceeds the limit set in the order.");
                e.Cache.SetValue<SOShipment.curyFreightAmt>(shipment, freightLimit); // Set freight amount to limit
                shipment.CuryFreightAmt = freightLimit - currentFreight; // Update the shipment's freight amount to the limit
                                                                         // Optionally, you can also set the focus back to the field
                                                                         //throw new PXException("Freight amount exceeds the limit set in the order.");

                e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(e.Row, freightLimit, new PXSetPropertyException(e.Row, Messages.FreightExceedsLimit(exceedAmt), PXErrorLevel.RowError));
                //e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(shipment, newAmt, new PXSetPropertyException("Freight amount exceeds the limit set in the order."));
            }
            else
            {
                // Optionally clear any previous error if the new amount is valid
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
                                                And<SOShipment.status, Equal<SOShipmentStatus.confirmed>>>>,
                                            OrderBy<Asc<SOOrderShipment.shipmentNbr>>>
                                    .Select(Base, order.OrderType, order.OrderNbr);
            foreach (var item in shipmentlist)
            {

                SOOrderShipment orderShipment = item.GetItem<SOOrderShipment>();
                SOShipment _shipment = item.GetItem<SOShipment>();
                if (_shipment.Status == SOShipmentStatus.Confirmed)
                {
                    totalFreight += (_shipment.CuryFreightAmt ?? 0m);
                }
            }

            ext.UsrFreightTotal = totalFreight;

            SOSetupExt sOSetupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
            if (sOSetupExt.UsrNotToExceed == order.ShipTermsID && order.CuryPremiumFreightAmt != totalFreight)
            {
                order.CuryPremiumFreightAmt = totalFreight;
            }
            Base.Caches[typeof(SOOrder)].Persist(order, PXDBOperation.Update);
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
    }
}
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static PX.Data.PXQuickProcess;
using static PX.Objects.SO.SOLine.FK;

namespace HubspotCustomization
{
    public class SOShipmentEntry_Extension : PXGraphExtension<SOShipmentEntry>
    {
        public static bool IsActive() => true;
        #region Actions

        // Hide the original actions
        //[PXUIField(Visible = false)]
        //[PXButton]
        //public virtual IEnumerable CreateInvoice(PXAdapter adapter)
        //{
        //    return Base.createInvoice.Press(adapter);
        //}

        //[PXUIField(Visible = false)]
        //[PXButton]
        //public virtual IEnumerable CreateDropshipInvoice(PXAdapter adapter)
        //{
        //    return Base.createDropshipInvoice.Press(adapter);
        //}

        // New combined action - CORRECTED for mass processing
        public PXAction<SOShipment> createCombinedInvoice;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Prepare Invoice", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable CreateCombinedInvoice(PXAdapter adapter)
        {
            var shipments = adapter.Get<SOShipment>().ToList();
            var results = new List<SOShipment>();
            if (!adapter.Arguments.TryGetValue("InvoiceDate", out var invoiceDate) || invoiceDate == null)
            {
                invoiceDate = Base.Accessinfo.BusinessDate;
            }
            foreach (SOShipment shipment in shipments)
            {
                // Set current shipment
                Base.Document.Current = shipment;


                if (IsDropShipShipment(shipment))
                {
                    // Process as drop-ship invoice
                    //processResult = Base.createDropshipInvoice.Press(adapter);
                    SOShipmentEntry shipmentEntry = PXGraph.CreateInstance<SOShipmentEntry>();
                    InvoiceList invoiceList = new ShipmentInvoices(shipmentEntry);
                    (bool MassProcess, Dictionary<string, object> Arguments) adapterSlice = (MassProcess: adapter.MassProcess, Arguments: adapter.Arguments);
                    SOShipmentEntry.InvoiceReceipt(adapterSlice.Arguments, shipments, invoiceList, adapterSlice.MassProcess);
                    shipments.ForEach(delegate (SOShipment sh)
                    {
                        shipmentEntry.Document.Cache.RestoreCopy(sh, PrimaryKeyOf<SOShipment>.By<SOShipment.shipmentNbr>.Find(shipmentEntry, shipmentEntry.Document.Current));
                    });
                }
                else
                {
                    // Process as regular invoice
                    //processResult = Base.createInvoice.Press(singleAdapter);
                    SOShipmentEntry shipmentEntry = PXGraph.CreateInstance<SOShipmentEntry>();
                    SOInvoiceEntry sOInvoiceEntry = PXGraph.CreateInstance<SOInvoiceEntry>();
                    (bool MassProcess, bool AllowRedirect, PXQuickProcess.ActionFlow QuickProcessFlow) adapterSlice = (MassProcess: adapter.MassProcess, AllowRedirect: adapter.AllowRedirect, QuickProcessFlow: adapter.QuickProcessFlow);
                    InvoiceList invoiceList = new ShipmentInvoices(shipmentEntry);

                    if (adapterSlice.MassProcess)
                    {
                        PXProcessing<SOShipment>.SetCurrentItem(shipment);
                    }
                    Base.InvoiceShipment(sOInvoiceEntry, shipment, (DateTime)invoiceDate, invoiceList, adapterSlice.QuickProcessFlow);

                    if (adapterSlice.MassProcess)
                    {
                        shipmentEntry.Document.Cache.RestoreCopy(shipment, PrimaryKeyOf<SOShipment>.By<SOShipment.shipmentNbr>.Find(shipmentEntry, shipmentEntry.Document.Current));
                        PXProcessing<SOShipment>.SetProcessed();
                    }
                }

            }

            return shipments;
            //return results.Count > 0 ? results : shipments;
        }

        #endregion

        #region Helper Methods

        protected virtual bool IsDropShipShipment(SOShipment shipment)
        {
            // Check if shipment has any drop-ship order shipments
            var hasDropShip = PXSelect<SOOrderShipment,
                Where<SOOrderShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                    And<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>>>>
                .Select(Base, shipment.ShipmentNbr).Count > 0;

            return hasDropShip;
        }

        #endregion

        #region Event Handlers

        protected void SOShipment_RowSelected(PXCache cache, PXRowSelectedEventArgs e, PXRowSelected InvokeBaseHandler)
        {
            if (InvokeBaseHandler != null)
                InvokeBaseHandler(cache, e);

            if (e.Row == null) return;

            var shipment = (SOShipment)e.Row;

            // Manual check based on business rules
            bool canCreateInvoice = CanCreateInvoice(shipment);
            bool canCreateDropship = CanCreateDropshipInvoice(shipment);

            createCombinedInvoice.SetEnabled(canCreateInvoice || canCreateDropship);

            // Hide the original actions
            Base.createInvoice.SetVisible(false);
            Base.createDropshipInvoice.SetVisible(false);
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

        #region Overrides
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
    }
}
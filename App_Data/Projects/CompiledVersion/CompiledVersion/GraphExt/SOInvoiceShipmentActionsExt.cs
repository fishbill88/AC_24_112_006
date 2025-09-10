using PX.Data;
using PX.Objects.SO;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using PX.Objects.AR;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using CompiledVersion.DAC;
using PX.Objects.PO;
using PX.Data.BQL;
using System.Collections;

namespace CompiledVersion.Graphs
{
    public class SOInvoiceShipmentActionsExt : PXGraphExtension<SOInvoiceShipment>
    {
        public static bool IsActive() => true;

        // Mapping used to convert sort/filter for dropship (POReceipt) fields when building combined list
        private static readonly Dictionary<string, string> DropshipFieldsMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(SOShipment.ShipmentNbr), nameof(POReceipt.receiptNbr) },
            { nameof(SOShipment.CustomerID), nameof(POReceipt.dropshipCustomerID) },
            { nameof(SOShipment.CustomerLocationID), nameof(POReceipt.dropshipCustomerLocationID) },
            { nameof(SOShipment.CustomerOrderNbr), nameof(POReceipt.dropshipCustomerOrderNbr) },
            { nameof(SOShipment.ShipVia), nameof(POReceipt.dropshipShipVia) },
            { nameof(SOShipment.ShipDate), nameof(POReceipt.receiptDate) },
        };

        #region Event Handlers
        protected virtual void _(Events.RowSelected<SOShipmentFilter> e)
        {
            if (e.Row == null) return;

            var filter = e.Row;

            // Show labels/columns when using combined action
            bool isCombined = filter.Action == SOInvoiceShipmentExt.WellKnownActions.SOShipmentScreen.PrepareInvoice;
            PXUIFieldAttribute.SetVisible<SOShipmentExt.usrReceiptNbr>(Base.Orders.Cache, null, isCombined);
            if (isCombined)
            {
                PXUIFieldAttribute.SetDisplayName<SOShipment.shipmentNbr>(Base.Orders.Cache, "Shipment Nbr");
                PXUIFieldAttribute.SetDisplayName<SOShipmentExt.usrReceiptNbr>(Base.Orders.Cache, "Receipt Nbr");
            }
        }

        protected virtual void _(Events.FieldSelecting<SOShipmentFilter, SOShipmentFilter.action> e)
        {
            if (e.Row == null) return;

            var currentList = e.ReturnValue as List<string>;
            if (currentList != null)
            {
                var actionsToRemove = new[]
                {
                    SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateInvoice,
                    SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateDropshipInvoice
                };

                foreach (var actionToRemove in actionsToRemove)
                    currentList.Remove(actionToRemove);

                e.ReturnValue = currentList;
            }
        }

        protected virtual void _(Events.FieldUpdating<SOShipmentFilter, SOShipmentFilter.action> e)
        {
            if (e.Row == null || e.NewValue == null) return;
            var newAction = e.NewValue.ToString();
            var blockedActions = new[]
            {
                SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateInvoice,
                SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateDropshipInvoice
            };

            if (blockedActions.Contains(newAction))
            {
                e.NewValue = SOInvoiceShipmentExt.WellKnownActions.SOShipmentScreen.PrepareInvoice;
            }
        }

        [PXOverride]
        public virtual PXSelectBase GetShipmentsSelectCommand(SOShipmentFilter filter,
            Func<SOShipmentFilter, PXSelectBase> baseMethod)
        {
            if (filter.Action == SOInvoiceShipmentExt.WellKnownActions.SOShipmentScreen.PrepareInvoice)
            {
                // Return shipments query; receipts will be appended in orders() override
                return new
                    SelectFrom<SOShipment>.
                    InnerJoin<INSite>.On<SOShipment.FK.Site>.
                    InnerJoin<Customer>.On<SOShipment.customerID.IsEqual<Customer.bAccountID>>.SingleTableOnly.
                    LeftJoin<Carrier>.On<SOShipment.FK.Carrier>.
                    Where<
                        SOShipment.confirmed.IsEqual<True>.
                        And<Match<Customer, AccessInfo.userName.FromCurrent>>.
                        And<Match<INSite, AccessInfo.userName.FromCurrent>>.
                        And<Exists<
                            SelectFrom<SOOrderShipment>.
                            Where<
                                SOOrderShipment.shipmentNbr.IsEqual<SOShipment.shipmentNbr>.
                                And<SOOrderShipment.shipmentType.IsEqual<SOShipment.shipmentType>>.
                                And<SOOrderShipment.invoiceNbr.IsNull>.
                                And<SOOrderShipment.createARDoc.IsEqual<True>>>>>>.
                    View(this.Base);
            }

            return baseMethod(filter);
        }

        public delegate System.Collections.IEnumerable OrdersDelegate();
        [PXOverride]
        public virtual System.Collections.IEnumerable orders(OrdersDelegate baseMethod)
        {
            var filter = Base.Filter.Current;
            if (filter?.Action != SOInvoiceShipmentExt.WellKnownActions.SOShipmentScreen.PrepareInvoice)
            {
                return baseMethod();
            }

            var results = new List<object>();

            // First, get the shipments from baseMethod (uses our GetShipmentsSelectCommand override)
            foreach (var res in baseMethod())
            {
                var shipment = PXResult.Unwrap<SOShipment>(res);
                if (shipment != null)
                {
                    var ext = shipment.GetExtension<SOShipmentExt>();
                    if (ext != null) ext.UsrReceiptNbr = null; // regular shipments
                    results.Add(shipment); // ensure homogeneous type
                }
            }

            // Build drop-ship receipts query similar to base CreateDropshipInvoice
            PXSelectBase<POReceipt> rtCmd = new
                SelectFrom<POReceipt>.
                Where<
                    POReceipt.released.IsEqual<True>
                    .And<Exists<SelectFrom<SOOrderShipment>
                        .InnerJoin<SOOrderType>.On<SOOrderShipment.FK.OrderType>
                        .InnerJoin<Customer>.On<SOOrderShipment.customerID.IsEqual<Customer.bAccountID>>.SingleTableOnly
                        .Where<SOOrderShipment.shipmentNbr.IsEqual<POReceipt.receiptNbr>
                            .And<SOOrderShipment.shipmentType.IsEqual<SOShipmentType.dropShip>>
                            .And<SOOrderShipment.invoiceNbr.IsNull>
                            .And<SOOrderShipment.createARDoc.IsEqual<True>>
                            .And<Match<Customer, AccessInfo.userName.FromCurrent>>>>> >
                .View(Base);

            // Apply receipt filters similar to base ApplyReceiptFilters
            rtCmd.WhereAnd<Where<POReceipt.receiptDate.IsLessEqual<SOShipmentFilter.endDate.FromCurrent>>>();
            if (filter.CustomerID != null)
                rtCmd.WhereAnd<Where<POReceipt.dropshipCustomerID.IsEqual<SOShipmentFilter.customerID.FromCurrent>>>();
            if (!string.IsNullOrEmpty(filter.ShipVia))
                rtCmd.WhereAnd<Where<POReceipt.dropshipShipVia.IsEqual<SOShipmentFilter.shipVia.FromCurrent>>>();
            if (!string.IsNullOrEmpty(filter.CarrierPluginID))
            {
                rtCmd.Join<InnerJoin<Carrier, On<Carrier.carrierID.IsEqual<POReceipt.dropshipShipVia>>>>();
                rtCmd.WhereAnd<Where<Carrier.carrierPluginID.IsEqual<SOShipmentFilter.carrierPluginID.FromCurrent>>>();
            }
            if (filter.StartDate != null)
                rtCmd.WhereAnd<Where<POReceipt.receiptDate.IsGreaterEqual<SOShipmentFilter.startDate.FromCurrent>>>();

            string[] sortColumns = AlterDropshipSortColumns(PXView.SortColumns);
            PXFilterRow[] mappedFilters = AlterDropshipFilters(PXView.Filters);

            int startRow = PXView.StartRow;
            int totalRows = 0;
            foreach (object res in rtCmd.View.Select(null, null, PXView.Searches, sortColumns, PXView.Descendings, mappedFilters, ref startRow, PXView.MaximumRows, ref totalRows))
            {
                POReceipt receipt = PXResult.Unwrap<POReceipt>(res);
                if (receipt == null) continue;
                SOShipment shipment = SOShipment.FromDropshipPOReceipt(receipt);

                SOShipmentExt ext = Base.Caches[typeof(SOShipment)].GetExtension<DAC.SOShipmentExt>(shipment);
                //var ext = shipment.GetExtension<SOShipmentExt>();
                if (ext != null)
                {
                    ext.UsrReceiptNbr = receipt.ReceiptNbr;
                }

                SOShipment cached = (SOShipment)Base.Orders.Cache.Locate(shipment);
                if (cached == null)
                {
                    Base.Orders.Cache.SetStatus(shipment, PXEntryStatus.Held);
                }
                else
                {
                    shipment.Selected = cached.Selected;
                    shipment.BillSeparately = cached.BillSeparately;
                }

                results.Add(shipment);
            }
            PXView.StartRow = 0;

            Base.Orders.Cache.IsDirty = false;
            return results;
        }

        private string[] AlterDropshipSortColumns(string[] sortColumns)
        {
            var map = DropshipFieldsMapping;
            return sortColumns?.Select(col => map.ContainsKey(col) ? map[col] : col).ToArray() ?? Array.Empty<string>();
        }

        private PXFilterRow[] AlterDropshipFilters(PXView.PXFilterRowCollection filters)
        {
            var newFilters = new List<PXFilterRow>();
            var fieldsMapping = DropshipFieldsMapping;
            foreach (PXFilterRow filter in filters)
            {
                newFilters.Add(
                    fieldsMapping.ContainsKey(filter.DataField)
                    ? new PXFilterRow(filter) { DataField = fieldsMapping[filter.DataField] }
                    : filter);
            }
            return newFilters.ToArray();
        }
        #endregion
    }
}
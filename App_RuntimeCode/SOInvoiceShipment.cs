using PX.Data;
using PX.Objects.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HubspotCustomization
{
    public class SOInvoiceShipmentExt : PXGraphExtension<SOInvoiceShipment>
    {
        public static bool IsActive() => true;

        // Cache for drop-ship detection to avoid repeated queries
        private static readonly Dictionary<string, bool> _dropShipCache = new Dictionary<string, bool>();

        #region Actions
        public PXAction<SOShipmentFilter> prepareInvoice;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Prepare Invoice", MapEnableRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable PrepareInvoice(PXAdapter adapter)
        {
            // Clear the cache at the start of processing
            _dropShipCache.Clear();

            // Get the selected shipments
            var selectedShipments = Base.Orders.Cache.Cached.Cast<SOShipment>()
                .Where(s => s.Selected == true)
                .ToList();

            if (!selectedShipments.Any())
                return adapter.Get();

            // Optimize drop-ship detection with bulk query
            PreloadDropShipInfo(selectedShipments);

            // Group shipments by type for processing
            var regularShipments = new List<SOShipment>();
            var dropShipShipments = new List<SOShipment>();

            foreach (var shipment in selectedShipments)
            {
                if (IsDropShipShipmentCached(shipment))
                {
                    dropShipShipments.Add(shipment);
                }
                else
                {
                    regularShipments.Add(shipment);
                }
            }

            // Process each group using their respective workflows
            ProcessShipments(regularShipments, dropShipShipments);

            return adapter.Get();
        }

        private void PreloadDropShipInfo(List<SOShipment> shipments)
        {
            if (!shipments.Any()) return;

            var shipmentNumbers = shipments.Select(s => s.ShipmentNbr).ToArray();

            // Single query to get all drop-ship shipments
            var dropShipShipments = PXSelectReadonly<SOOrderShipment,
                Where<SOOrderShipment.shipmentNbr, In<Required<SOOrderShipment.shipmentNbr>>,
                    And<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>>>>
                .Select(Base, shipmentNumbers)
                .Select(r => ((SOOrderShipment)r).ShipmentNbr)
                .ToHashSet();

            // Cache the results
            foreach (var shipment in shipments)
            {
                _dropShipCache[shipment.ShipmentNbr] = dropShipShipments.Contains(shipment.ShipmentNbr);
            }
        }

        private bool IsDropShipShipmentCached(SOShipment shipment)
        {
            return _dropShipCache.ContainsKey(shipment.ShipmentNbr) && _dropShipCache[shipment.ShipmentNbr];
        }

        private void ProcessShipments(List<SOShipment> regularShipments, List<SOShipment> dropShipShipments)
        {
            var parameters = new Dictionary<string, object>
            {
                [nameof(SOShipmentFilter.InvoiceDate)] = Base.Filter.Current?.InvoiceDate ?? Base.Accessinfo.BusinessDate
            };

            // Process regular shipments
            if (regularShipments.Count > 0)
            {
                ProcessShipmentGroup(regularShipments, SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateInvoice, parameters);
            }

            // Process drop-ship shipments
            if (dropShipShipments.Count > 0)
            {
                ProcessShipmentGroup(dropShipShipments, SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateDropshipInvoice, parameters);
            }
        }

        private void ProcessShipmentGroup(List<SOShipment> shipments, string actionName, Dictionary<string, object> parameters)
        {
            // Use PXLongOperation for proper mass processing
            PXLongOperation.StartOperation(Base, delegate ()
            {
                var processingGraph = PXGraph.CreateInstance<SOInvoiceShipment>();

                // Set up the filter
                var filter = processingGraph.Filter.Current ?? processingGraph.Filter.Insert();
                filter.Action = actionName;
                filter.InvoiceDate = (System.DateTime?)parameters[nameof(SOShipmentFilter.InvoiceDate)];
                processingGraph.Filter.Update(filter);

                // Process each shipment individually within the long operation
                foreach (var shipment in shipments)
                {
                    try
                    {
                        // Find the shipment in the processing graph
                        SOShipment shipmentToProcess = processingGraph.Orders.Search<SOShipment.shipmentNbr>(shipment.ShipmentNbr);
                        if (shipmentToProcess != null)
                        {
                            // Set it as selected and current
                            shipmentToProcess.Selected = true;
                            processingGraph.Orders.Update(shipmentToProcess);
                            processingGraph.Orders.Current = shipmentToProcess;

                            // Set up the workflow action
                            processingGraph.Orders.SetProcessWorkflowAction(actionName, parameters);

                            // Process this individual shipment using reflection to call the internal process method
                            var processMethod = typeof(PXProcessingBase<SOShipment>).GetMethod("ProcessItem",
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                            if (processMethod != null)
                            {
                                processMethod.Invoke(processingGraph.Orders, new object[] { shipmentToProcess });
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        PXTrace.WriteError($"Error processing shipment {shipment.ShipmentNbr}: {ex.Message}");
                    }
                }
            });
        }
        #endregion

        #region Event Handlers
        protected virtual void _(Events.RowSelected<SOShipmentFilter> e)
        {
            if (e.Row == null) return;

            var filter = e.Row;

            // Hide the button completely
            prepareInvoice.SetVisible(false);

        }
        #endregion

        // Add the WellKnownActions constant for your custom action - FIXED
        public new class WellKnownActions : SOInvoiceShipment.WellKnownActions
        {
            public new class SOShipmentScreen : SOInvoiceShipment.WellKnownActions.SOShipmentScreen
            {
                public const string PrepareInvoice
                    = ScreenID + "$" + nameof(SOShipmentEntry_Extension.createCombinedInvoice);
            }
        }
    }

    public class SOInvoiceShipmentWellKnownActionsExt : PXGraphExtension<SOInvoiceShipment>
    {
        public static bool IsActive() => true;
        public new class WellKnownActions : SOInvoiceShipment.WellKnownActions
        {
            public new class SOShipmentScreen : SOInvoiceShipment.WellKnownActions.SOShipmentScreen
            {
                public const string PrepareInvoice
                    = ScreenID + "$" + nameof(SOShipmentEntry_Extension.createCombinedInvoice);
            }
        }
    }
}
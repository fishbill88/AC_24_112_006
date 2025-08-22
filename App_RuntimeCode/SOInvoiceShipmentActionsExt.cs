using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.SO;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects.IN;
using PX.Objects.CR;
using PX.Objects.PO;
using PX.Objects.AR;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HubspotCustomization
{
    public class SOInvoiceShipmentActionsExt : PXGraphExtension<SOInvoiceShipment>
    {
        public static bool IsActive() => true;

        #region Event Handlers
        protected virtual void _(Events.RowSelected<SOShipmentFilter> e)
        {
            if (e.Row == null) return;

            var filter = e.Row;

            // Hide the original actions from the dropdown by redirecting them
            if (filter.Action == SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateInvoice ||
                filter.Action == SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateDropshipInvoice)
            {
                // Redirect to our combined action
                filter.Action = SOInvoiceShipmentExt.WellKnownActions.SOShipmentScreen.PrepareInvoice;
            }
        }

        protected virtual void _(Events.FieldSelecting<SOShipmentFilter, SOShipmentFilter.action> e)
        {
            if (e.Row == null) return;

            // Get the current list of actions
            var currentList = e.ReturnValue as List<string>;
            
            if (currentList != null)
            {
                // Remove specific actions from the dropdown
                var actionsToRemove = new[]
                {
                    SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateInvoice,
                    SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateDropshipInvoice
                };

                foreach (var actionToRemove in actionsToRemove)
                {
                    currentList.Remove(actionToRemove);
                }

                e.ReturnValue = currentList;
            }
        }

        protected virtual void _(Events.FieldUpdating<SOShipmentFilter, SOShipmentFilter.action> e)
        {
            if (e.Row == null || e.NewValue == null) return;

            var newAction = e.NewValue.ToString();
            
            // List of actions to block
            var blockedActions = new[]
            {
                SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateInvoice,
                SOInvoiceShipment.WellKnownActions.SOShipmentScreen.CreateDropshipInvoice
            };

            if (blockedActions.Contains(newAction))
            {
                // Redirect to your custom action instead
                e.NewValue = SOInvoiceShipmentExt.WellKnownActions.SOShipmentScreen.PrepareInvoice;
            }
        }

        [PXOverride]
        public virtual PXSelectBase GetShipmentsSelectCommand(SOShipmentFilter filter,
            Func<SOShipmentFilter, PXSelectBase> baseMethod)
        {
            // Handle your combined action - include BOTH regular and drop-ship shipments
            if (filter.Action == SOInvoiceShipmentExt.WellKnownActions.SOShipmentScreen.PrepareInvoice)
            {
                // Return a query that includes both regular and drop-ship shipments
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

            // Use base method for all other actions
            return baseMethod(filter);
        }

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.TX;
using PX.Objects.CS;
using POLine = PX.Objects.PO.POLine;
using POOrder = PX.Objects.PO.POOrder;
using PX.CarrierService;
using PX.Concurrency;
using CRLocation = PX.Objects.CR.Standalone.Location;
using ARRegisterAlias = PX.Objects.AR.Standalone.ARRegisterAlias;
using PX.Objects.AR.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Objects.Common.Extensions;
using PX.CS.Contracts.Interfaces;
using Message = PX.CarrierService.Message;
using PX.Data.DependencyInjection;
using PX.Data.WorkflowAPI;
using PX.LicensePolicy;
using PX.Objects.SO.GraphExtensions.CarrierRates;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;
using PX.Objects.SO.Attributes;
using PX.Objects.Common.Attributes;
using PX.Objects.Common.Bql;
using OrderActions = PX.Objects.SO.SOOrderEntryActionsAttribute;
using PX.Data.BQL.Fluent;
using PX.Objects.IN.InventoryRelease;
using PX.Data.BQL;
using PX.Objects.IN.InventoryRelease.Utility;
using PX.Objects.SO.Standalone;
using PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated;
using PX.Objects.Common.Interfaces;
using PX.Objects;
using PX.Objects.SO;

namespace PX.Objects.SO
{
  public class STSOOrderEntryExtension : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
  {
  
    
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
            var customerExt = customer.GetExtension<STCustomerExt>();
            // Get the DAC extension for SOOrder
            var orderExt = order.GetExtension<STSOOrderExt>();

            if (customerExt != null && orderExt != null)
            {
                orderExt.UsrShippingInstructions = customerExt.UsrShippingInstructions;
                e.Cache.SetValueExt<STSOOrderExt.usrShippingInstructions>(order, customerExt.UsrShippingInstructions);
            }
        }
        UpdateCustomerAccount(e.Cache, e.Row as SOOrder);
      
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
            var customerExt = customer.GetExtension<STCustomerExt>();
            // Get the DAC extension for SOOrder
            var orderExt = order.GetExtension<STSOOrderExt>();

            if (customerExt != null && orderExt != null)
            {
                orderExt.UsrShippingInstructions = customerExt.UsrShippingInstructions;
                e.Cache.SetValueExt<STSOOrderExt.usrShippingInstructions>(order, customerExt.UsrShippingInstructions);
            }
        }
        UpdateCustomerAccount(e.Cache, e.Row as SOOrder);
    }
    
    protected void SOOrder_UseCustomerAccount_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
    {
        UpdateCustomerAccount(sender, e.Row as SOOrder);
    }
      
      
    protected void SOOrder_ShipVia_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
    {
        UpdateCustomerAccount(sender, e.Row as SOOrder);
    }

    protected void SOOrder_CustomerLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
    {
        UpdateCustomerAccount(sender, e.Row as SOOrder);
    }

    private void UpdateCustomerAccount(PXCache sender, SOOrder order)
    {
        if (order == null) return;

        var orderExt = order.GetExtension<STSOOrderExt>();
        if (orderExt != null)
        {
          // Only proceed if all required fields are set
            if (string.IsNullOrEmpty(order.ShipVia) || order.CustomerID == null || order.CustomerLocationID == null || !(order.UseCustomerAccount ?? false))
              {
                sender.SetValueExt<STSOOrderExt.usrCustomerAccount>(order, null);
          return;
              }
               
            Carrier shipVia = PXSelect<Carrier,
                Where<Carrier.carrierID, Equal<Required<Carrier.carrierID>>>>.Select(Base, order?.ShipVia);
    
            // Query the CarrierCustomer table for a matching record
             CarrierPluginCustomer carrierCustomer = PXSelect<CarrierPluginCustomer,
                       Where<CarrierPluginCustomer.carrierPluginID, Equal<Required<CarrierPluginCustomer.carrierPluginID>>,
                         And<CarrierPluginCustomer.customerID, Equal<Required<CarrierPluginCustomer.customerID>>,
                         And<CarrierPluginCustomer.customerLocationID,Equal<Required<CarrierPluginCustomer.customerLocationID>>,
                         And<CarrierPluginCustomer.isActive,Equal<True>>>>>>
                          .Select(Base, shipVia?.CarrierPluginID, order?.CustomerID, order?.CustomerLocationID);
               
               
             if(carrierCustomer == null){  
                            carrierCustomer = PXSelect<CarrierPluginCustomer,
                           Where<CarrierPluginCustomer.carrierPluginID, Equal<Required<CarrierPluginCustomer.carrierPluginID>>,
                             And<CarrierPluginCustomer.customerID, Equal<Required<CarrierPluginCustomer.customerID>>,
                         And<CarrierPluginCustomer.isActive,Equal<True>>>>>
                              .Select(Base, shipVia?.CarrierPluginID, order?.CustomerID);
              }

        
            if (carrierCustomer != null)
            {
                orderExt.UsrCustomerAccount = carrierCustomer.CarrierAccount;
                sender.SetValueExt<STSOOrderExt.usrCustomerAccount>(order, orderExt.UsrCustomerAccount);
            }
            else
            {
                orderExt.UsrCustomerAccount = null;
                sender.SetValueExt<STSOOrderExt.usrCustomerAccount>(order, null);
            }
        }
    } 
  }
}
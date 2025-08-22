using PX.Data;
using PX.Objects.CN.Compliance.PO.CacheExtensions;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightCustomization
{
    public class SOShipmentEntry_Extension : PXGraphExtension<PX.Objects.SO.SOShipmentEntry>
    {
        public static bool IsActive() => true;
        
        public delegate void ConfirmShipmentDelegate(SOOrderEntry docgraph, SOShipment shiporder);
        [PXOverride]
        public void ConfirmShipment(SOOrderEntry docgraph, SOShipment shiporder, ConfirmShipmentDelegate baseMethod)
        {
          
          baseMethod(docgraph,shiporder);
      
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
          if(sOSetupExt.UsrNotToExceed == order.ShipTermsID && order.CuryPremiumFreightAmt != totalFreight)
          {
              order.CuryPremiumFreightAmt = totalFreight;
          }
          Base.Caches[typeof(SOOrder)].Persist(order, PXDBOperation.Update);
        }


        protected virtual void _(Events.FieldUpdated<SOShipment, SOShipment.overrideFreightAmount> e)
        {

            if (e.Row == null) return;
            SOShipment shipment = e.Row;
            if(((bool?)e.NewValue) == true) 
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
                                                And<SOShipment.status,Equal<SOShipmentStatus.confirmed>>>>, 
                                            OrderBy<Asc<SOOrderShipment.shipmentNbr>>>
                                    .Select(Base,order.OrderType,order.OrderNbr);
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

                e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(e.Row, freightLimit, new PXSetPropertyException(e.Row, string.Format("Freight price exceeds limit by {0}.", exceedAmt), PXErrorLevel.RowError));
                //e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(shipment, newAmt, new PXSetPropertyException("Freight amount exceeds the limit set in the order."));
            }
            else
            {
                // Optionally clear any previous error if the new amount is valid
                e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(shipment, newAmt, null);
            }

        }
    }
}
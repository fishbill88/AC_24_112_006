using System;
using PX.Data;
using PX.Objects.SO;

namespace FreightCustomization
{
    public class SOOrderEntry_Extension : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() => true;
        #region Event Handlers

        protected virtual void _(Events.RowSelected<SOOrder> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);

            SOOrder order = (SOOrder)e.Row;
            if (order == null) return;

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
                bool isdirty = Base.IsDirty;
                order.CuryPremiumFreightAmt = totalFreight;
                if(!isdirty)
                {
                    Base.Actions.PressSave();
                }
            }
        }
        #endregion
    }
}
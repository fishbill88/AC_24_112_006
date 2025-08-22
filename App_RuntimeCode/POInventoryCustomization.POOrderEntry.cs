using PX.Data;
using PX.Objects.Common.DAC;
using PX.Objects.PO;
using PX.Objects.SO;

namespace FreightCustomization
{
    public class POOrderEntry_Extension : PXGraphExtension<PX.Objects.PO.POOrderEntry>
    {
        public PXSetup<SOSetup> sosetup;   
        public static bool IsActive() => true;

        [PXCopyPasteHiddenView()]
        public PXSelect<DropShipLink,
      Where<DropShipLink.pOOrderType, Equal<Required<POLine.orderType>>,
        And<DropShipLink.pOOrderNbr, Equal<Required<POLine.orderNbr>>,
        And<DropShipLink.pOLineNbr, Equal<Required<POLine.lineNbr>>>>>> STDropShipLinks;

        public virtual DropShipLink GetDropShipLink(POLine line)
        {
            if (line == null || !POLineType.IsDropShip(line.LineType))
                return null;

            return STDropShipLinks.SelectWindowed(0, 1, line.OrderType, line.OrderNbr, line.LineNbr);
        }



        protected virtual void _(Events.RowSelecting<POLine> e, PXRowSelecting baseMethod)
        {

            POOrder order = Base.CurrentDocument.Current;
            POLine line = (POLine)e.Row;
            if (line == null) return;

            using (new PXConnectionScope())
            {
                POOrderExt poExt = order.GetExtension<POOrderExt>();
                sosetup.Current = sosetup.Select();
                SOSetupExt sOSetupExt = sosetup.Current.GetExtension<SOSetupExt>();
                DropShipLink link = GetDropShipLink(line);

                //show the freight cost and price only if SO shipTermsID = UsrPrepayAndAdd or UsrFreeFreightAllowed
                if (link != null && link.POOrderType == line.OrderType && link.POOrderNbr == line.OrderNbr && link.POLineNbr == line.LineNbr)
                {
                    SOOrder soOrder = PXSelect<SOOrder,
                        Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                            And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                        .Select(Base, link.SOOrderType, link.SOOrderNbr);
                   
                    poExt.UsrShipTermsIDTemp = soOrder.ShipTermsID;
                    poExt.UsrShowFreightCost = sOSetupExt.UsrPrepayAndAdd == soOrder.ShipTermsID ||
                       sOSetupExt.UsrFreeFreightAllowed == soOrder.ShipTermsID;
                    poExt.UsrShowFreightPrice = sOSetupExt.UsrFreeFreightAllowed == soOrder.ShipTermsID;
                }
            }
            baseMethod?.Invoke(e.Cache, e.Args);


        }
        protected virtual void _(Events.RowSelected<POOrder> e, PXRowSelected baseMethod)
        {
            POOrder order = (POOrder)e.Row;
            if (order == null) return;

            SOOrder soOrder = PXSelect<SOOrder,
                        Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                            And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                        .Select(Base, order.SOOrderType, order.SOOrderNbr);
            if (order.SOOrderType == null || order.SOOrderNbr == null) return;
            SOSetupExt sOSetupExt = sosetup.Current.GetExtension<SOSetupExt>();

            POOrderExt poExt = order.GetExtension<POOrderExt>();
            PXUIFieldAttribute.SetVisible<POOrderExt.usrFreightCost>(Base.Document.Cache, Base.CurrentDocument.Current,
                sOSetupExt.UsrPrepayAndAdd == soOrder.ShipTermsID ||
                       sOSetupExt.UsrFreeFreightAllowed == soOrder.ShipTermsID);
            PXUIFieldAttribute.SetVisible<POOrderExt.usrFreightPrice>(Base.Document.Cache, Base.CurrentDocument.Current,
                sOSetupExt.UsrPrepayAndAdd == soOrder.ShipTermsID);
            baseMethod?.Invoke(e.Cache, e.Args);
        }
    }
}
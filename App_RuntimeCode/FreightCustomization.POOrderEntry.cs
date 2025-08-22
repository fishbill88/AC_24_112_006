using PX.Data;
using PX.Objects.Common.DAC;
using PX.Objects.PO;
using PX.Objects.SO;

namespace POInventoryCustomization
{
    public class POOrderEntry_Extension : PXGraphExtension<PX.Objects.PO.POOrderEntry>
    {

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


        protected virtual void _(Events.RowSelected<POLine> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            POLine line = (POLine)e.Row;
            if (line == null) return;


            DropShipLink link = GetDropShipLink(line);
            SOOrderTypeExt typeExt = null;
            if (e.Row != null && link != null)
            {
                SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                 And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                                 .Select(Base, link.SOOrderType, link.SOOrderNbr, link.SOLineNbr);
                if (soLine == null) return;

                SOOrderType orderType = SOOrderType.PK.Find(Base, link.SOOrderType);
                typeExt = orderType.GetExtension<SOOrderTypeExt>();
            }

            PXUIFieldAttribute.SetVisible<POLineExt.usrVendorID>(e.Cache, null, typeExt?.UsrShowVendorID ?? false);
            PXUIFieldAttribute.SetVisible<POLineExt.usrVendorLocationID>(e.Cache, null, typeExt?.UsrShowVendorLocationID ?? false);
            PXUIFieldAttribute.SetVisible<POLineExt.usrVendorAddress>(e.Cache, null, typeExt?.UsrShowVendorAddress ?? false);
            PXUIFieldAttribute.SetVisibility<POLineExt.usrVendorID>(e.Cache, null, ((typeExt?.UsrShowVendorID ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible));
            PXUIFieldAttribute.SetVisibility<POLineExt.usrVendorLocationID>(e.Cache, null, ((typeExt?.UsrShowVendorLocationID ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible));
            PXUIFieldAttribute.SetVisibility<POLineExt.usrVendorAddress>(e.Cache, null, ((typeExt?.UsrShowVendorAddress ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible));

        }

    }
}
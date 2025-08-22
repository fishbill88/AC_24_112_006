using PX.Data;
using PX.Objects.SO;
using SOOrder = PX.Objects.SO.SOOrder;
using PX.Objects.PO;
using PX.Objects.Common.DAC;

namespace NoteAttachmentsCustomization
{
    public class POOrderEntry_Extension : PXGraphExtension<PX.Objects.PO.POOrderEntry>
    {
        public PXSetup<SOSetup> sosetup;
        public PXSelect<SOOrder> allsoorder;

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

        #region Event Handlers
        protected virtual void _(Events.RowUpdated<POLine> e, PXRowUpdated baseMethod)
        {
            POOrder order = Base.CurrentDocument.Current;
            POLine line = (POLine)e.Row;
            if (line == null) return;
            baseMethod?.Invoke(e.Cache, e.Args);
            DropShipLink link = GetDropShipLink(line);
            SOSetup _sosetup = sosetup.Current;
            SOSetupExt _soext = _sosetup.GetExtension<SOSetupExt>();
            SOOrder soOrder = PXSelect<SOOrder,
                Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                    And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                .Select(Base, link?.SOOrderType, link?.SOOrderNbr);

            if ((_soext?.UsrCopyHeaderNotesToPO ?? false) && link != null && link.POOrderType == order.OrderType && link.POOrderNbr == order.OrderNbr)
            {
                // Get the note from the Sales Order
                string noteText = PXNoteAttribute.GetNote(Base.Caches[typeof(SOOrder)], soOrder);

                // Set the note on the Shipment
                PXNoteAttribute.SetNote(Base.CurrentDocument.Cache, order, noteText);
            }
            //copy attachments from SOOrder to POOrder
            if ((_soext?.UsrCopyHeaderAttachmentsToPO ?? false) && link != null && link.POOrderType == order.OrderType && link.POOrderNbr == order.OrderNbr)
            {
                PXNoteAttribute.CopyNoteAndFiles(Base.Caches[typeof(SOOrder)], soOrder, Base.Caches[typeof(POOrder)], order);
            }

            SOLine soLine = PXSelect<SOLine,
                        Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                            And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                            And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                        .Select(Base, link?.SOOrderType, link?.SOOrderNbr, link?.SOLineNbr);
            //get attachments and notes from the SOLine to POLIne
            if ((_soext?.UsrCopyLineNotesToPO ?? false) && link != null && link.POOrderType == line.OrderType && link.POOrderNbr == line.OrderNbr && link.POLineNbr == line.LineNbr)
            {

                // Get the note from the Sales Order Line
                string noteText = PXNoteAttribute.GetNote(Base.Caches[typeof(SOLine)], soLine);
                // Set the note on the Shipment Line
                PXNoteAttribute.SetNote(Base.Caches[typeof(POLine)], line, noteText);
            }
            //get attachments from the SOLine to POLIne
            if ((_soext?.UsrCopyLineAttachmentsToPO ?? false) && link != null && link.POOrderType == line.OrderType && link.POOrderNbr == line.OrderNbr && link.POLineNbr == line.LineNbr)
            {
                PXNoteAttribute.CopyNoteAndFiles(Base.Caches[typeof(SOLine)], soLine, Base.Caches[typeof(POLine)], line);
            }
        }
        #endregion
    }
}
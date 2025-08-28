using CompiledVersion.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.Common.DAC;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using System.Linq;

namespace CompiledVersion.Graphs
{
    public class POOrderEntry_Extension : PXGraphExtension<PX.Objects.PO.POOrderEntry>
    {
        public static bool IsActive() => true;
        public bool skipCostDefaulting = false;

        public PXSetup<SOSetup> sosetup;
        public PXSelect<SOOrder> allsoorder;

        [PXCopyPasteHiddenView()]
        public PXSelect<DropShipLink,
      Where<DropShipLink.pOOrderType, Equal<Required<POLine.orderType>>,
        And<DropShipLink.pOOrderNbr, Equal<Required<POLine.orderNbr>>,
        And<DropShipLink.pOLineNbr, Equal<Required<POLine.lineNbr>>>>>> STDropShipLinks;

        #region Methods
        public virtual DropShipLink GetDropShipLink(POLine line)
        {
            if (line == null || !POLineType.IsDropShip(line.LineType))
                return null;

            return STDropShipLinks.SelectWindowed(0, 1, line.OrderType, line.OrderNbr, line.LineNbr);
        }

        private void RecalculateRTHHeaderTotals()
        {
            var order = Base.Document.Current;
            if (order == null) return;
            var orderExt = order.GetExtension<POOrderExt>();

            var lines = Base.Transactions.Select().RowCast<POLine>()
                .Where(l => l.GetExtension<POLineExt>()?.UsrPrepaymentLine != true);

            orderExt.UsrRTHDetailTotal = lines.Sum(l => l.CuryLineAmt ?? 0m);
            orderExt.UsrRTHLineDiscount = lines.Sum(l => l.CuryDiscAmt ?? 0m);
            // For Doc Discount and Tax, you may need to recalculate or copy as needed. Here, set to 0 for now.
            orderExt.UsrRTHDocDiscount = order.CuryDiscTot;
            orderExt.UsrRTHTaxTotal = order.CuryTaxTotal;
            orderExt.UsrRTHOrderTotal = (orderExt.UsrRTHDetailTotal ?? 0m)
                - (orderExt.UsrRTHLineDiscount ?? 0m)
                - (orderExt.UsrRTHDocDiscount ?? 0m)
                + (orderExt.UsrRTHTaxTotal ?? 0m);

            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHDetailTotal>(order, orderExt.UsrRTHDetailTotal);
            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHLineDiscount>(order, orderExt.UsrRTHLineDiscount);
            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHDocDiscount>(order, orderExt.UsrRTHDocDiscount);
            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHTaxTotal>(order, orderExt.UsrRTHTaxTotal);
            Base.Document.Cache.SetValueExt<POOrderExt.usrRTHOrderTotal>(order, orderExt.UsrRTHOrderTotal);

        }
        #endregion

        #region Events

        #region POLine
        protected virtual void _(Events.FieldDefaulting<POLine, POLineExt.usrSWKSPCCode> e)
        {
            if (e.Row == null) return;

            // Try to find the related SO line to copy SPC Code
            var soLine = PXSelectJoin<SOLine,
                InnerJoin<SOLineSplit, On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                    And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                    And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
                Where<SOLineSplit.pOType, Equal<Current<POLine.orderType>>,
                    And<SOLineSplit.pONbr, Equal<Current<POLine.orderNbr>>,
                    And<SOLineSplit.pOLineNbr, Equal<Current<POLine.lineNbr>>>>>>
                .SelectSingleBound(Base, new object[] { e.Row });

            if (soLine != null)
            {
                var soLineExt = ((SOLine)soLine).GetExtension<SOLineExt>();
                if (!string.IsNullOrEmpty(soLineExt?.UsrSWKSPCCode))
                {
                    e.NewValue = soLineExt.UsrSWKSPCCode;
                    e.Cancel = true;
                }
            }
        }
        protected virtual void _(Events.FieldUpdated<POLine, POLine.inventoryID> e)
        {
            if (e.Row == null) return;
            var poLineExt = e.Row.GetExtension<POLineExt>();
            InventoryItem item = InventoryItem.PK.Find(Base, e.Row.InventoryID);
            InventoryItemExt itemExt = item.GetExtension<InventoryItemExt>();
            poLineExt.UsrSWKRTHCost = (itemExt?.UsrSWKRTHCost ?? 0m);
        }
        protected virtual void POLine_CuryUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (skipCostDefaulting)
            {
                return;
            }

            POLine pOLine = e.Row as POLine;
            POOrder current = Base.Document.Current;
            if (pOLine != null && pOLine.ManualPrice == true)
            {
                e.NewValue = pOLine.CuryUnitCost.GetValueOrDefault();
            }
            else if (pOLine != null && pOLine.InventoryID.HasValue && current != null && current.VendorID.HasValue)
            {
                decimal? num = null;
                if (pOLine.UOM != null)
                {
                    DateTime value = Base.Document.Current.OrderDate.Value;
                    PX.Objects.CM.Extensions.CurrencyInfo currencyInfo = Base.FindImplementation<IPXCurrencyHelper>().GetCurrencyInfo(current.CuryInfoID);
                    num = APVendorPriceMaint.CalculateUnitCost(sender, pOLine.VendorID, current.VendorLocationID, pOLine.InventoryID, pOLine.SiteID, currencyInfo.GetCM(), pOLine.UOM, pOLine.OrderQty, value, pOLine.CuryUnitCost);
                    e.NewValue = num;
                }

                if (!num.HasValue)
                {
                    e.NewValue = POItemCostManager.Fetch<POLine.inventoryID, POLine.curyInfoID>(sender.Graph, pOLine, current.VendorID, current.VendorLocationID, current.OrderDate, current.CuryID, pOLine.InventoryID, pOLine.SubItemID, pOLine.SiteID, pOLine.UOM);
                }

                APVendorPriceMaint.CheckNewUnitCost<POLine, POLine.curyUnitCost>(sender, pOLine, e.NewValue);
            }
        }
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
        protected virtual void _(Events.RowSelecting<POLine> e, PXRowSelecting baseMethod)
        {

            POOrder order = Base.CurrentDocument.Current;
            POLine line = (POLine)e.Row;
            if (line == null) return;

            using (new PXConnectionScope())
            {
                POOrderExt poExt = order.GetExtension<POOrderExt>();
                POLineExt lineExt = line.GetExtension<POLineExt>();
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
                    SOOrderExt sOOrderExt = soOrder.GetExtension<SOOrderExt>();

                    poExt.UsrShipTermsIDTemp = soOrder.ShipTermsID;
                    poExt.UsrShowFreightCost = sOSetupExt.UsrPrepayAndAdd == soOrder.ShipTermsID ||
                       sOSetupExt.UsrFreeFreightAllowed == soOrder.ShipTermsID;
                    poExt.UsrShowFreightPrice = sOSetupExt.UsrFreeFreightAllowed == soOrder.ShipTermsID;
                    poExt.UsrShippingInstructions = sOOrderExt.UsrShippingInstructions;
                }

                if (e.Row != null && link != null)
                {
                    SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                     And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                                     .Select(Base, link.SOOrderType, link.SOOrderNbr, link.SOLineNbr);
                    if (soLine == null) return;
                    lineExt.UsrShippingTerms = soLine?.ShipComplete;
                    e.Cache.SetValueExt<POLineExt.usrShippingTerms>(line, soLine?.ShipComplete);
                }
            }
            baseMethod?.Invoke(e.Cache, e.Args);


        }
        protected virtual void _(Events.RowSelected<POLine> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            POLine line = (POLine)e.Row;
            if (line == null) return;

            PXUIFieldAttribute.SetEnabled<POLineExt.usrShippingTerms>(e.Cache, line, false);

            PXException warningVendorSpecTerms = null;
            PXException warningVendorNotes = null;
            PXException warningShippingTerms = null;

            DropShipLink link = GetDropShipLink(line);

            POLineExt lineExt = line.GetExtension<POLineExt>();
            if (lineExt.UsrVendorSpecTerms != null)
            {
                warningVendorSpecTerms = new PXSetPropertyException(line, Messages.Space, PXErrorLevel.Warning);
            }
            if (lineExt.UsrVendorNotes != null)
            {
                warningVendorNotes = new PXSetPropertyException(line, Messages.Space, PXErrorLevel.Warning);
            }
            if (lineExt.UsrShippingTerms != null)
            {
                warningShippingTerms = new PXSetPropertyException(line, Messages.Space, PXErrorLevel.Warning);
            }
            e.Cache.RaiseExceptionHandling<POLineExt.usrVendorSpecTerms>(e.Row, lineExt.UsrVendorSpecTerms, warningVendorSpecTerms);
            e.Cache.RaiseExceptionHandling<POLineExt.usrVendorNotes>(e.Row, lineExt.UsrVendorNotes, warningVendorNotes);
            e.Cache.RaiseExceptionHandling<POLineExt.usrShippingTerms>(e.Row, lineExt.UsrShippingTerms, warningShippingTerms);

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
            PXUIFieldAttribute.SetEnabled<POLineExt.usrSWKSPCCode>(e.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<POLineExt.usrSWKRTHCost>(e.Cache, null, false);
        }
        protected virtual void POLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            POLine line = (POLine)e.Row;
            if (line == null) return;

            POLineExt lineExt = line.GetExtension<POLineExt>();
            DropShipLink link = GetDropShipLink(line);

            if (e.Row != null && link != null)
            {
                SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                    And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                              .Select(Base, link.SOOrderType, link.SOOrderNbr, link.SOLineNbr);
                if (soLine != null)
                {
                    SOLineExt soLineExt = line.GetExtension<SOLineExt>();
                    lineExt.UsrVendorSpecTerms = soLineExt?.UsrVendorSpecTerms;
                    sender.SetValueExt<POLineExt.usrVendorSpecTerms>(line, soLineExt?.UsrVendorSpecTerms);


                    lineExt.UsrVendorNotes = soLineExt?.UsrVendorNotes;
                    sender.SetValueExt<POLineExt.usrVendorNotes>(line, soLineExt?.UsrVendorNotes);
                }
            }

            RecalculateRTHHeaderTotals();
        }
        protected void _(Events.RowUpdated<POLine> e)
        {
            if (e.Row != null)
                RecalculateRTHHeaderTotals();
        }

        protected void _(Events.RowDeleted<POLine> e)
        {
            if (e.Row != null)
                RecalculateRTHHeaderTotals();
        }
        #endregion

        #region POOrder
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
        #endregion

        #endregion


    }
}
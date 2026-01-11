using CompiledVersion.DAC;
using PX.Data;
using PX.Objects.Common.DAC;
using PX.Objects.PO;
using PX.Objects.SO;
using System.Collections.Generic;
using System.Linq;

namespace CompiledVersion.Graphs
{
    public class POReceiptEntry_Extension : PXGraphExtension<PX.Objects.PO.POReceiptEntry>
    {
        public static bool IsActive() => true;
        
        public PXSetup<SOSetup> sosetup;
        
        public PXSelectReadonly<POOrderReceipt,
        Where<POOrderReceipt.receiptType, Equal<Current<POReceipt.receiptType>>,
            And<POOrderReceipt.receiptNbr, Equal<Current<POReceipt.receiptNbr>>>>> OrderReceipt;

        #region Event Handlers
        public delegate POReceipt CreateEmptyReceiptFromDelegate(POOrder order);
        [PXOverride]
        public POReceipt CreateEmptyReceiptFrom(POOrder order, CreateEmptyReceiptFromDelegate baseMethod)
        {
            POReceipt receipt = baseMethod(order);

            POOrderExt poExt = order.GetExtension<POOrderExt>();

            POReceiptExt receiptExt = receipt.GetExtension<POReceiptExt>();
            receiptExt.UsrFOBPoint = order.FOBPoint;
            receiptExt.UsrShipVia = order.ShipVia;
            receiptExt.UsrShipTermsID = poExt.UsrShipTermsID;
            receiptExt.UsrCarrierAccount = poExt.UsrCustomerAccount;
            receiptExt.UsrHasShippingTab = true;
            return receipt;
        }


        protected virtual void _(Events.RowSelected<POReceipt> e)
        {
            POReceipt receipt = (POReceipt)e.Row;
            if (receipt == null) return;
            POReceiptExt receiptExt = receipt.GetExtension<POReceiptExt>();

            PXUIFieldAttribute.SetVisible<POReceiptExt.usrHasShippingTab>(e.Cache, receipt, false);
            //PXUIFieldAttribute.SetVisible<STPOReceiptExt.usrShipVia>(e.Cache, receipt, showTab );
            //PXUIFieldAttribute.SetVisible<STPOReceiptExt.usrShipTermsID>(e.Cache, receipt, showTab );
            //PXUIFieldAttribute.SetVisible<STPOReceiptExt.usrFreightCost>(e.Cache, receipt, showTab );
            //PXUIFieldAttribute.SetVisible<STPOReceiptExt.usrFreightPrice>(e.Cache, receipt, showTab );
            //PXUIFieldAttribute.SetVisible<STPOReceiptExt.usrCarrierAccount>(e.Cache, receipt, showTab );
            //PXUIFieldAttribute.SetVisible<STPOReceiptExt.usrTrackingNumber>(e.Cache, receipt, showTab );
        }

        #endregion

        #region NTE Freight Validation for Drop-Ship PO Receipts
        /// <summary>
        /// Validates that the total freight cost across all PO receipts for a Drop-Ship PO
        /// does not exceed the Not-To-Exceed limit defined on the linked Sales Order.
        /// </summary>
        protected virtual void _(Events.RowPersisting<POReceipt> e)
        {
            if (e.Row == null) return;
            if (e.Operation == PXDBOperation.Delete) return;

            POReceipt receipt = e.Row;
            POReceiptExt receiptExt = receipt.GetExtension<POReceiptExt>();

            // Skip if no freight price on this receipt
            decimal currentFreightPrice = receiptExt?.UsrFreightPrice ?? 0m;
            if (currentFreightPrice <= 0m) return;

            // Check if this receipt is for a Drop-Ship PO
            var linkedSO = GetLinkedSOForDropShipReceipt(receipt);
            if (linkedSO == null) return; // Not a drop-ship receipt or no SO link

            SOOrderExt soExt = linkedSO.GetExtension<SOOrderExt>();
            SOSetupExt setupExt = sosetup.Current?.GetExtension<SOSetupExt>();

            // Check if this SO has NTE shipping terms
            if (linkedSO.ShipTermsID != setupExt?.UsrNotToExceed) return;

            // Check if SO has NTE limit set
            decimal? nteLimit = soExt?.UsrFreightPriceLimit;
            if (nteLimit == null || nteLimit <= 0m) return;

            // Calculate total freight price across ALL shipments/receipts for this SO
            decimal? totalFreightPrice = CalculateTotalFreightPriceForSO(linkedSO, receipt, currentFreightPrice);

            // Validate against limit
            if (totalFreightPrice > nteLimit)
            {
                decimal? exceedAmt = totalFreightPrice - nteLimit;
                string errorMsg = Messages.POReceiptFreightExceedsNTE(
                    $"{linkedSO.OrderType}-{linkedSO.OrderNbr}",
                    totalFreightPrice,
                    nteLimit,
                    exceedAmt
                );

                // Use setup toggle to determine error level
                PXErrorLevel errorLevel = (setupExt?.UsrEnforcePONTE == true) 
                    ? PXErrorLevel.Error 
                    : PXErrorLevel.Warning;

                e.Cache.RaiseExceptionHandling<POReceiptExt.usrFreightPrice>(
                    receipt, 
                    currentFreightPrice, 
                    new PXSetPropertyException(errorMsg, errorLevel)
                );

                // If hard stop mode, throw exception to block save
                if (errorLevel == PXErrorLevel.Error)
                {
                    throw new PXException(errorMsg);
                }
            }
        }

        /// <summary>
        /// Retrieves the linked Sales Order for NTE validation from Drop-Ship PO Receipt.
        /// </summary>
        private SOOrder GetLinkedSOForDropShipReceipt(POReceipt receipt)
        {
            if (receipt == null) return null;

            // Get receipt lines to find the linked Drop-Ship PO and SO
            var receiptLine = PXSelect<POReceiptLine,
                Where<POReceiptLine.receiptType, Equal<Required<POReceiptLine.receiptType>>,
                    And<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>>>>
                .SelectWindowed(Base, 0, 1, receipt.ReceiptType, receipt.ReceiptNbr)
                .RowCast<POReceiptLine>()
                .FirstOrDefault();

            if (receiptLine == null) return null;

            // Check if linked to a Drop-Ship PO
            if (string.IsNullOrEmpty(receiptLine.POType) || receiptLine.POType != POOrderType.DropShip)
                return null;

            // Get the linked SO Order
            // First try from the receipt line's SO fields
            if (!string.IsNullOrEmpty(receiptLine.SOOrderType) && !string.IsNullOrEmpty(receiptLine.SOOrderNbr))
            {
                return PXSelect<SOOrder,
                    Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                        And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                    .Select(Base, receiptLine.SOOrderType, receiptLine.SOOrderNbr);
            }

            // Fallback: Get from DropShipLink via the PO
            DropShipLink link = PXSelect<DropShipLink,
                Where<DropShipLink.pOOrderType, Equal<Required<POReceiptLine.pOType>>,
                    And<DropShipLink.pOOrderNbr, Equal<Required<POReceiptLine.pONbr>>>>>
                .SelectWindowed(Base, 0, 1, receiptLine.POType, receiptLine.PONbr);

            if (link != null)
            {
                return PXSelect<SOOrder,
                    Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                        And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                    .Select(Base, link.SOOrderType, link.SOOrderNbr);
            }

            return null;
        }

        /// <summary>
        /// Calculates the cumulative total freight price from all SOOrderShipment records for a Sales Order.
        /// Uses SOOrderShipment as the single source of truth (same as Shipments grid on SO).
        /// Handles both regular shipments and drop-ship PO receipts.
        /// </summary>
        private decimal? CalculateTotalFreightPriceForSO(SOOrder soOrder, POReceipt currentReceipt, decimal currentFreightPrice)
        {
            if (soOrder == null) return 0m;

            decimal totalFreight = 0m;
            bool currentReceiptIncluded = false;

            // Get all SOOrderShipment records for this Sales Order (same as Shipments grid)
            var orderShipments = PXSelect<SOOrderShipment,
                Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                    And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                .Select(Base, soOrder.OrderType, soOrder.OrderNbr);

            foreach (SOOrderShipment orderShipment in orderShipments.RowCast<SOOrderShipment>())
            {
                if (orderShipment.ShipmentType == SOShipmentType.DropShip)
                {
                    // Drop-Ship: Get freight from PO Receipt via ShippingRefNoteID
                    if (orderShipment.ShippingRefNoteID != null)
                    {
                        POReceipt receipt = PXSelect<POReceipt,
                            Where<POReceipt.noteID, Equal<Required<POReceipt.noteID>>>>
                            .Select(Base, orderShipment.ShippingRefNoteID);

                        if (receipt != null)
                        {
                            // Check if this is the current receipt being saved
                            if (receipt.ReceiptType == currentReceipt.ReceiptType && 
                                receipt.ReceiptNbr == currentReceipt.ReceiptNbr)
                            {
                                totalFreight += currentFreightPrice;
                                currentReceiptIncluded = true;
                            }
                            else
                            {
                                POReceiptExt receiptExt = receipt.GetExtension<POReceiptExt>();
                                totalFreight += (receiptExt?.UsrFreightPrice ?? 0m);
                            }
                        }
                    }
                }
                else
                {
                    // Regular shipment: Get freight from SOShipment
                    if (!string.IsNullOrEmpty(orderShipment.ShipmentNbr))
                    {
                        SOShipment shipment = PXSelect<SOShipment,
                            Where<SOShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                                And<SOShipment.shipmentType, Equal<Required<SOShipment.shipmentType>>>>>
                            .Select(Base, orderShipment.ShipmentNbr, orderShipment.ShipmentType);

                        if (shipment != null)
                        {
                            totalFreight += (shipment.CuryFreightAmt ?? 0m);
                        }
                    }
                }
            }

            // If current receipt wasn't found in SOOrderShipment (new receipt not yet released/linked), add its freight
            if (!currentReceiptIncluded)
            {
                totalFreight += currentFreightPrice;
            }

            return totalFreight;
        }
        #endregion
    }
}
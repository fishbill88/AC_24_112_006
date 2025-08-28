using CompiledVersion.DAC;
using PX.Data;
using PX.Objects.PO;

namespace CompiledVersion.Graphs
{
    public class POReceiptEntry_Extension : PXGraphExtension<PX.Objects.PO.POReceiptEntry>
    {
        public static bool IsActive() => true;
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
    }
}
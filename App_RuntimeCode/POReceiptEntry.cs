using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.DependencyInjection;
using PX.Data.WorkflowAPI;
using PX.LicensePolicy;
using PX.Objects.AP;
using PX.Objects.AP.MigrationMode;
using PX.Objects.CM.Extensions;
using PX.Objects.Common;
using PX.Objects.Common.Bql;
using PX.Objects.Common.Extensions;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.Extensions.CostAccrual;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.IN.InventoryRelease;
using PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated;
using PX.Objects.IN.Services;
using PX.Objects.PM;
using PX.Objects.PO.GraphExtensions.POReceiptEntryExt;
using PX.Objects.PO.LandedCosts;
using PX.Objects.SO;
using PX.Objects.TX;
using CRLocation = PX.Objects.CR.Standalone.Location;
using ItemLotSerial = PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated.ItemLotSerial;
using SiteLotSerial = PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated.SiteLotSerial;
using SOLine4 = PX.Objects.SO.SOLine4;
using SOOrder = PX.Objects.SO.SOOrder;
using PX.Objects;
using PX.Objects.PO;

namespace PX.Objects.PO
{
  public class STPOReceiptEntry_Extension : PXGraphExtension<PX.Objects.PO.POReceiptEntry>
  {

    public PXSelectReadonly<POOrderReceipt,
    Where<POOrderReceipt.receiptType, Equal<Current<POReceipt.receiptType>>,
        And<POOrderReceipt.receiptNbr,Equal<Current<POReceipt.receiptNbr>>>>> OrderReceipt;

    #region Event Handlers
    public delegate POReceipt CreateEmptyReceiptFromDelegate(POOrder order);
    [PXOverride]
    public POReceipt CreateEmptyReceiptFrom(POOrder order, CreateEmptyReceiptFromDelegate baseMethod)
    {
      POReceipt receipt = baseMethod(order);

      STPOOrderExt poExt = order.GetExtension<STPOOrderExt>();

      STPOReceiptExt receiptExt = receipt.GetExtension<STPOReceiptExt>();
      receiptExt.UsrFOBPoint = order.FOBPoint;
      receiptExt.UsrShipVia = order.ShipVia;
      receiptExt.UsrShipTermsID = poExt.UsrShipTermsID;
      receiptExt.UsrCarrierAccount= poExt.UsrCustomerAccount;
      receiptExt.UsrHasShippingTab = true;
      return receipt;
    }
    
      
    protected virtual void _(Events.RowSelected<POReceipt> e)
    {
       POReceipt receipt = (POReceipt)e.Row;
       if (receipt== null) return;
      STPOReceiptExt receiptExt = receipt.GetExtension<STPOReceiptExt>();

        PXUIFieldAttribute.SetVisible<STPOReceiptExt.usrHasShippingTab>(e.Cache, receipt, false);
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
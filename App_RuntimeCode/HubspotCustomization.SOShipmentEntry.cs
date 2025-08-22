using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Common;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.SM;
using POLineType = PX.Objects.PO.POLineType;
using POReceiptLine = PX.Objects.PO.POReceiptLine;
using PX.CarrierService;
using PX.Data.DependencyInjection;
using PX.LicensePolicy;
using PX.Objects.SO.Services;
using PX.Objects.PO;
using PX.Objects.AR.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Objects.Common.Extensions;
using PX.Common.Collection;
using PX.Objects.SO.GraphExtensions.CarrierRates;
using PX.Objects.SO.GraphExtensions.SOShipmentEntryExt;
using PX.Api;
using ShipmentActions = PX.Objects.SO.SOShipmentEntryActionsAttribute;
using PdfSharp.Pdf.IO;
using PX.Objects.IN.Attributes;
using PX.Concurrency;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;
using PX.Objects.GL.FinPeriods.TableDefinition;
using PX.Objects.GL.FinPeriods;
using PX.Objects.IN.InventoryRelease;
using PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated;
using PX.Objects.Common.Scopes;
using PX.Objects;
using PX.Objects.SO;

namespace PX.Objects.SO
{
  public class STSOShipmentEntry_Extension : PXGraphExtension<PX.Objects.SO.SOShipmentEntry>
  {
    #region Event Handlers
    protected virtual void _(Events.RowSelected<SOShipLine> e, PXRowSelected baseMethod)
    {
      baseMethod?.Invoke(e.Cache, e.Args);

      SOShipLine line = (SOShipLine)e.Row;
        if(line == null) return;
        if(line.OrigOrderNbr != null)
        {
            SOOrder order = SOOrder.PK.Find(Base, line.OrigOrderType,line.OrigOrderNbr);
            if(order != null)
            {
               STSOOrderExt orderExt = order.GetExtension<STSOOrderExt>();
              
               SOShipment shipment = Base.CurrentDocument.Current;
               STSOShipmentExt shipmentExt = shipment.GetExtension<STSOShipmentExt>();
               shipmentExt.UsrShippingNotes = orderExt.UsrShippingNotes;
               e.Cache.SetValueExt<STSOOrderExt.usrShippingNotes>(shipment, orderExt.UsrShippingNotes);
            }
        }

    }
    #endregion
  }
}
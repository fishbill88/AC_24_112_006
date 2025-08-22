using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Objects.Common.DAC;
using PX.Concurrency;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM.Extensions;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.SO;
using SOOrder = PX.Objects.SO.SOOrder;
using SOLine = PX.Objects.SO.SOLine;
using PX.Data.DependencyInjection;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.LicensePolicy;
using PX.Objects.PM;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AP.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects.Common.Bql;
using PX.Objects.Extensions.CostAccrual;
using PX.Objects.DR;
using PX.Data.WorkflowAPI;
using PX.Objects.Common.Scopes;
using PX.Objects.IN.Services;
using PX.Objects.Extensions.MultiCurrency;
using PX.Data.Description;
using PX.Objects.PO.GraphExtensions.POOrderEntryExt;
using PX.Objects.IN.InventoryRelease;
using PX.Objects.Common.Interfaces;
using PX.Objects.PO.DAC.Projections;
using PX.Objects;
using PX.Objects.PO;
using static PX.Objects.PO.POOrderEntry;

namespace PX.Objects.PO
{
  public class STPOOrderEntry_Extension : PXGraphExtension<PX.Objects.PO.POOrderEntry>
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
     
    private void RecalculateRTHHeaderTotals()
    {
        var order = Base.Document.Current;
        if (order == null) return;
        var orderExt = order.GetExtension<STPOOrderExt>();
    
        var lines = Base.Transactions.Select().RowCast<POLine>()
            .Where(l => l.GetExtension<STPOLineExt>()?.UsrPrepaymentLine != true);
    
        orderExt.UsrRTHDetailTotal = lines.Sum(l => l.CuryLineAmt ?? 0m);
        orderExt.UsrRTHLineDiscount = lines.Sum(l => l.CuryDiscAmt ?? 0m);
        // For Doc Discount and Tax, you may need to recalculate or copy as needed. Here, set to 0 for now.
        orderExt.UsrRTHDocDiscount = order.CuryDiscTot;
        orderExt.UsrRTHTaxTotal = order.CuryTaxTotal;
        orderExt.UsrRTHOrderTotal = (orderExt.UsrRTHDetailTotal ?? 0m)
            - (orderExt.UsrRTHLineDiscount ?? 0m)
            - (orderExt.UsrRTHDocDiscount ?? 0m)
            + (orderExt.UsrRTHTaxTotal ?? 0m);
    
        Base.Document.Cache.SetValueExt<STPOOrderExt.usrRTHDetailTotal>(order, orderExt.UsrRTHDetailTotal);
        Base.Document.Cache.SetValueExt<STPOOrderExt.usrRTHLineDiscount>(order, orderExt.UsrRTHLineDiscount);
        Base.Document.Cache.SetValueExt<STPOOrderExt.usrRTHDocDiscount>(order, orderExt.UsrRTHDocDiscount);
        Base.Document.Cache.SetValueExt<STPOOrderExt.usrRTHTaxTotal>(order, orderExt.UsrRTHTaxTotal);
        Base.Document.Cache.SetValueExt<STPOOrderExt.usrRTHOrderTotal>(order, orderExt.UsrRTHOrderTotal);
    
    }
  
  
    protected virtual void _(Events.RowSelected<POLine> e, PXRowSelected baseMethod)
    {
        baseMethod?.Invoke(e.Cache, e.Args);
      POLine line = (POLine)e.Row;
        if(line == null) return;

      PXUIFieldAttribute.SetEnabled<STPOLineExt.usrShippingTerms>(e.Cache, line, false);

      PXException warningVendorSpecTerms = null;
      PXException warningVendorNotes = null;
      PXException warningShippingTerms = null;
      
      STPOLineExt lineExt = line.GetExtension<STPOLineExt>();
      if(lineExt.UsrVendorSpecTerms != null)
      {
        warningVendorSpecTerms = new PXSetPropertyException(line, " ", PXErrorLevel.Warning);
      }
      if(lineExt.UsrVendorNotes != null)
      {
        warningVendorNotes = new PXSetPropertyException(line, " ", PXErrorLevel.Warning);
      }
      if(lineExt.UsrShippingTerms != null)
      {
        warningShippingTerms = new PXSetPropertyException(line, " ", PXErrorLevel.Warning);
      }
      e.Cache.RaiseExceptionHandling<STPOLineExt.usrVendorSpecTerms>(e.Row, lineExt.UsrVendorSpecTerms, warningVendorSpecTerms);
      e.Cache.RaiseExceptionHandling<STPOLineExt.usrVendorNotes>(e.Row, lineExt.UsrVendorNotes, warningVendorNotes);
      e.Cache.RaiseExceptionHandling<STPOLineExt.usrShippingTerms>(e.Row, lineExt.UsrShippingTerms, warningShippingTerms);
    }
    protected virtual void _(Events.RowSelecting<POLine> e, PXRowSelecting baseMethod)
    {
       baseMethod?.Invoke(e.Cache, e.Args);

        POLine line = (POLine)e.Row;
        if(line == null) return;
        STPOLineExt lineExt = line.GetExtension<STPOLineExt>();
        using (new PXConnectionScope())
        {
          DropShipLink link = GetDropShipLink(line);
          
          if (e.Row != null && link != null)
          {
             SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
              And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                              .Select(Base, link.SOOrderType,link.SOOrderNbr,link.SOLineNbr);
                if(soLine == null) return;
                  lineExt.UsrShippingTerms = soLine?.ShipComplete;
                  e.Cache.SetValueExt<STPOLineExt.usrShippingTerms>(line, soLine?.ShipComplete);
          }
        }
    }  
          
    protected virtual void POLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
    {
        POLine line = (POLine)e.Row;
        if (line == null) return;

        STPOLineExt lineExt = line.GetExtension<STPOLineExt>();
        DropShipLink link = GetDropShipLink(line);

        if (e.Row != null && link != null)
        {
            SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                          .Select(Base, link.SOOrderType, link.SOOrderNbr, link.SOLineNbr);
            if (soLine != null)
            {
                STSOLineExt soLineExt = line.GetExtension<STSOLineExt>();
                lineExt.UsrVendorSpecTerms = soLineExt?.UsrVendorSpecTerms;
                sender.SetValueExt<STPOLineExt.usrVendorSpecTerms>(line, soLineExt?.UsrVendorSpecTerms);


                lineExt.UsrVendorNotes = soLineExt?.UsrVendorNotes;
                sender.SetValueExt<STPOLineExt.usrVendorNotes>(line, soLineExt?.UsrVendorNotes);
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
  }
}
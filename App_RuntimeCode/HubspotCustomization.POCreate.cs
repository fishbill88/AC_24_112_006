using System;
using System.Collections;
using System.Collections.Generic;
using CommonServiceLocator;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.AP;
using PX.Objects.AP.MigrationMode;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.DAC;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PO.GraphExtensions.POOrderEntryExt;
using PX.Objects.SO;
using PX.TM;
using CRLocation = PX.Objects.CR.Standalone.Location;
using SOLine5 = PX.Objects.PO.POOrderEntry.SOLine5;
using SOLineSplit3 = PX.Objects.PO.POOrderEntry.SOLineSplit3;
using PX.Objects;
using PX.Objects.PO;

namespace PX.Objects.PO
{
  public class STPOCreate_Extension : PXGraphExtension<PX.Objects.PO.POCreate>
  {
    #region Event Handlers
    public delegate String LinkPOLineToBlanketDelegate(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText);
    [PXOverride]
    public String LinkPOLineToBlanket(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText, LinkPOLineToBlanketDelegate baseMethod)
    {
        SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
              And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                              .Select(Base, soline?.OrderType,soline?.OrderNbr,soline?.LineNbr);
        if(soLine != null)
        {
          STSOLineExt soLineExt = soLine.GetExtension<STSOLineExt>();
          STPOLineExt lineExt = line.GetExtension<STPOLineExt>();
          lineExt.UsrVendorSpecTerms = soLineExt.UsrVendorSpecTerms;
          lineExt.UsrVendorNotes= soLineExt.UsrVendorNotes;
          
          
          SOOrder soOrder = SOOrder.PK.Find(Base,soline.OrderType, soline.OrderNbr);
          STSOOrderExt soOrderExt = soOrder.GetExtension<STSOOrderExt>();
          docgraph.CurrentDocument.Current.FOBPoint = soOrder.FOBPoint;
          docgraph.CurrentDocument.Current.ShipVia = soOrder.ShipVia;
          
          STPOOrderExt poOrderExt = docgraph.CurrentDocument.Current.GetExtension<STPOOrderExt>();
          poOrderExt.UsrShipTermsID = soOrder.ShipTermsID;
          poOrderExt.UsrCustomerAccount = soOrderExt.UsrCustomerAccount;
          
          
        }
      
      return baseMethod(line,docgraph,demand,soline,ref ErrorLevel,ref ErrorText);
    }


    #endregion
  }
}
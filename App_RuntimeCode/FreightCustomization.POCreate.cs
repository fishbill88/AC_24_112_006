using System;
using PX.Data;
using PX.Objects.SO;
using SOLineSplit3 = PX.Objects.PO.POOrderEntry.SOLineSplit3;
using PX.Objects.PO;

namespace POInventoryCustomization
{
  public class POCreate_Extension : PXGraphExtension<PX.Objects.PO.POCreate>
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
          SOLineExt lineExt = soLine.GetExtension<SOLineExt>();
          POLineExt poLineExt = line.GetExtension<POLineExt>();
    
          
    
          SOOrder soOrder = SOOrder.PK.Find(Base,soline.OrderType, soline.OrderNbr);
          
          
          SOOrderType orderType = SOOrderType.PK.Find(Base, soOrder.OrderType);
          SOOrderTypeExt typeExt = orderType.GetExtension<SOOrderTypeExt>();
          if(typeExt.UsrShowVendorID ?? false)
              poLineExt.UsrVendorID = lineExt.UsrVendorID;
    
          if(typeExt.UsrShowVendorLocationID ?? false)
              poLineExt.UsrVendorLocationID = lineExt.UsrVendorLocationID;
    
          if(typeExt.UsrShowVendorAddress ?? false)
              poLineExt.UsrVendorAddress = lineExt.UsrVendorAddress;
    
          POOrderExt poOrderExt = docgraph.CurrentDocument.Current.GetExtension<POOrderExt>();
          poOrderExt.UsrCustomerOrderNbr = soOrder.CustomerOrderNbr;
          
        }
      
      return baseMethod(line,docgraph,demand,soline,ref ErrorLevel,ref ErrorText);
    }


    #endregion
  }
}
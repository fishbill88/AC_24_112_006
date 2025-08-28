//using System;
//using PX.Data;
//using PX.Objects.SO;
//using SOLineSplit3 = PX.Objects.PO.POOrderEntry.SOLineSplit3;
//using CompiledVersion.DAC;

//namespace PX.Objects.PO
//{
//    public class STPOCreate_Extension : PXGraphExtension<PX.Objects.PO.POCreate>
//    {
//        public static bool IsActive() => true;
//        #region Event Handlers
//        public delegate String LinkPOLineToBlanketDelegate(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText);
//        [PXOverride]
//        public String LinkPOLineToBlanket(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText, LinkPOLineToBlanketDelegate baseMethod)
//        {
//            //SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
//            //      And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
//            //                      .Select(Base, soline?.OrderType, soline?.OrderNbr, soline?.LineNbr);
//            //if (soLine != null)
//            //{
//            //    SOLineExt soLineExt = soLine.GetExtension<SOLineExt>();
//            //    POLineExt lineExt = line.GetExtension<POLineExt>();
//            //    lineExt.UsrVendorSpecTerms = soLineExt.UsrVendorSpecTerms;
//            //    lineExt.UsrVendorNotes = soLineExt.UsrVendorNotes;


//            //    SOOrder soOrder = SOOrder.PK.Find(Base, soline.OrderType, soline.OrderNbr);
//            //    SOOrderExt soOrderExt = soOrder.GetExtension<SOOrderExt>();
//            //    docgraph.CurrentDocument.Current.FOBPoint = soOrder.FOBPoint;
//            //    docgraph.CurrentDocument.Current.ShipVia = soOrder.ShipVia;

//            //    POOrderExt poOrderExt = docgraph.CurrentDocument.Current.GetExtension<POOrderExt>();
//            //    poOrderExt.UsrShipTermsID = soOrder.ShipTermsID;
//            //    poOrderExt.UsrCustomerAccount = soOrderExt.UsrCustomerAccount;
//            //}

//            //POFixedDemandExt demandExt = demand.GetExtension<POFixedDemandExt>();
//            //demand.VendorID = demandExt.UsrVendorID;
//            //demand.VendorLocationID = demandExt.UsrVendorLocationID;
//            return baseMethod(line, docgraph, demand, soline, ref ErrorLevel, ref ErrorText);
//        }


//        #endregion
//    }
//}
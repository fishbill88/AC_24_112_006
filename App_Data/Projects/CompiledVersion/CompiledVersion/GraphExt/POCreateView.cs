using CompiledVersion.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.AP;
using PX.Objects.Common;
using PX.Objects.Common.DAC;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.CR;
using PX.Objects.SO;
using System;
using System.Collections;
using static PX.Objects.PO.POCreate;
using static PX.Objects.SO.SOCreate;
using CRLocation = PX.Objects.CR.Standalone.Location;

namespace CompiledVersion.Graphs
{
    public class POCreateView : PXGraphExtension<PX.Objects.PO.POCreate>
    {
        public static bool IsActive() => true;

        public virtual IEnumerable fixedDemand()
        {
            PXResultset<POFixedDemand> fixedDemands = SelectFromFixedDemandViewNew();
            return Base.EnumerateAndPrepareFixedDemands(fixedDemands);
        }

        public virtual PXResultset<POFixedDemand> SelectFromFixedDemandViewNew()
        {

            var Fix2 = new PXSelectJoin<POFixedDemand,
                 LeftJoin<Vendor, On<Vendor.bAccountID, Equal<POFixedDemand.vendorID>>,
                 LeftJoin<POVendorInventory,
                       On<POVendorInventory.recordID, Equal<POFixedDemand.recordID>>,
                 LeftJoin<CRLocation, On<CRLocation.bAccountID, Equal<POFixedDemand.vendorID>, And<CRLocation.locationID, Equal<POFixedDemand.vendorLocationID>>>,
                 LeftJoin<SOOrder, On<SOOrder.noteID, Equal<POFixedDemand.refNoteID>, And<SOOrder.status.IsIn<SOOrderStatus.backOrder, SOOrderStatus.open, SOOrderStatus.shipping>>>,
                 LeftJoin<SOLine, On<SOLine.orderType, Equal<POFixedDemand.orderType>, And<SOLine.orderNbr, Equal<POFixedDemand.orderNbr>, And<SOLine.lineNbr, Equal<POFixedDemand.lineNbr>>>>,
                 LeftJoin<DropShipLink, On<DropShipLink.FK.SOLine>>>>>>>,
                 Where2<Where<POFixedDemand.inventoryID, Equal<Current<POCreateFilter.inventoryID>>, Or<Current<POCreateFilter.inventoryID>, IsNull>>,
                     And2<Where<POFixedDemand.siteID, Equal<Current<POCreateFilter.siteID>>, Or<Current<POCreateFilter.siteID>, IsNull>>,
                     And2<Where<SOOrder.customerID, Equal<Current<POCreateFilter.customerID>>, Or<Current<POCreateFilter.customerID>, IsNull, Or<SOOrder.orderNbr, IsNull>>>,
                     And2<Where<SOOrder.orderType, Equal<Current<POCreateFilter.orderType>>, Or<Current<POCreateFilter.orderType>, IsNull>>,
                     And2<Where<SOOrder.orderNbr, Equal<Current<POCreateFilter.orderNbr>>, Or<Current<POCreateFilter.orderNbr>, IsNull>>,
                     And2<Where<POFixedDemand.planDate, LessEqual<Current<POCreateFilter.requestedOnDate>>, Or<Current<POCreateFilter.requestedOnDate>, IsNull>>,
                     And2<Where<POFixedDemand.orderType, IsNull, Or<POFixedDemand.behavior, NotEqual<SOBehavior.bL>, Or<POFixedDemand.pOCreateDate, LessEqual<Current<POCreateFilter.purchDate>>>>>,
                     And2<Where<POFixedDemand.itemClassCD, Like<Current<POCreateFilter.itemClassCDWildcard>>, Or<Current<POCreateFilter.itemClassCDWildcard>, IsNull>>,
                     And<POFixedDemand.planQty, NotEqual<decimal0>,
                     And<Where<POFixedDemand.planType, NotIn3<INPlanConstants.plan6D, INPlanConstants.plan6E>,
                             Or<POFixedDemand.baseShippedQty, Equal<decimal0>,
                                 And<DropShipLink.sOLineNbr, IsNull,
                                 And<SOLine.isLegacyDropShip, Equal<boolFalse>>>>>>>>>>>>>>>>(Base);


            PXView query = new PXView(Base, true, Fix2.View.BqlSelect);

            var fixedDemands = new PXResultset<POFixedDemand>();
            var startRow = PXView.StartRow;
            var totalRows = 0;
            object[] parameters = null;

            if (PXView.MaximumRows == 1 && PXView.SortColumns != null && PXView.Searches != null)
            {
                int planIDIndex = Array.FindIndex(PXView.SortColumns, s => s.Equals(nameof(POFixedDemand.planID), StringComparison.OrdinalIgnoreCase));
                if (planIDIndex >= 0 && PXView.Searches[planIDIndex] != null)
                {
                    long planID = Convert.ToInt64(PXView.Searches[planIDIndex]);
                    query.WhereAnd<Where<POFixedDemand.planID.IsEqual<@P.AsLong>>>();
                    parameters = new object[] { planID };
                }
            }

            using (new PXFieldScope(query, Base.GetFixedDemandFieldScope()))
            {
                foreach (PXResult<POFixedDemand> demand in query.Select(PXView.Currents, parameters,
                    PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters,
                    ref startRow, PXView.MaximumRows, ref totalRows))
                {
                    POFixedDemand demandRow = demand;
                    SOLine sOLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                    And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                                  .Select(Base, demandRow?.OrderType, demandRow?.OrderNbr, demandRow?.LineNbr);
                    if (sOLine != null)
                    {

                        SOLineExt soLineExt = sOLine?.GetExtension<SOLineExt>();
                        if (soLineExt?.UsrVendorID != null && soLineExt?.UsrVendorLocationID != null)
                        {
                            demandRow.VendorID = soLineExt?.UsrVendorID;
                            Location loc = PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
                                And<Location.locationID, Equal<Required<Location.locationID>>>>>
                                .Select(Base, soLineExt?.UsrVendorID, soLineExt?.UsrVendorLocationID);
                            demandRow.VendorLocationID = loc?.LocationID;
                        }

                        // Map read-only display fields from SOLine to POFixedDemand extension
                        POFixedDemandExt demandExt = demandRow.GetExtension<POFixedDemandExt>();
                        if (demandExt != null)
                        {
                            demandExt.UsrSOSPCCost = soLineExt?.UsrSWKSPCCost;
                            demandExt.UsrSOSPCCode = soLineExt?.UsrSWKSPCCode;
                            demandExt.UsrSOVendorNotes = soLineExt?.UsrVendorNotes;
                            demandExt.UsrSOVendorSpecialTerms = soLineExt?.UsrVendorSpecTerms;
                        }
                    }

                    if (Base.Filter.Current.VendorID != null && demandRow.VendorID != Base.Filter.Current.VendorID)
                        continue;

                    InventoryItem item = InventoryItem.PK.Find(Base, demandRow.InventoryID);
                    INItemXRef iNItemXRef = PXSelect<INItemXRef,
         Where<INItemXRef.inventoryID, Equal<Required<INItemXRef.inventoryID>>,
      And<INItemXRef.alternateType, Equal<INAlternateType.global>>>>.
      Select(Base, demandRow.InventoryID);
      if (item != null && iNItemXRef != null)
         demandRow.AlternateID = iNItemXRef.AlternateID;
        else
demandRow.AlternateID = null;
   //if (demandRow.InventoryID != null && demandRow.VendorID == null && demandRow.VendorLocationID == null)
             //{
     //    //get the default vendor from POVendorInventory
             //InventoryItemMaint maint = PXGraph.CreateInstance<InventoryItemMaint>();
        //    maint.Clear();
         //    InventoryItem item = InventoryItem.PK.Find(Base, demandRow.InventoryID);
           //    maint.Item.Current = item;

             //    maint.VendorItems.Select();
         //    foreach (POVendorInventory vendorInventory in maint.VendorItems.Select())
           //    {
            //   if (vendorInventory.IsDefault == true && vendorInventory.Active == true)
      //      {
       //            demandRow.VendorID = vendorInventory.VendorID;
       //        demandRow.VendorLocationID = vendorInventory.VendorLocationID;
     //            break;
      //   }

    //    }
     //}

      fixedDemands.Add(demand);
      }
     }

PXView.StartRow = 0;

     return fixedDemands;
        }
    }
}

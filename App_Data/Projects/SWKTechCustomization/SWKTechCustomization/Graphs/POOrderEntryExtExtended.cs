using PX.Data;
using PX.Objects.AP;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.IN;
using System;
using ItemRequestCustomization;

namespace SWKTechCustomization
{
    public class POOrderEntryExtExtended : PXGraphExtension<POOrderEntry>
    {

        public bool skipCostDefaulting = false;
        public static bool IsActive() => true;

        #region Event Handlers

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

        protected virtual void _(Events.RowSelected<POLine> e)
        {
            if (e.Row == null) return;

            // Disable the UsrSWKSPCCode field using PXUIFieldAttribute
            PXUIFieldAttribute.SetEnabled<POLineExt.usrSWKSPCCode>(e.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<POLineExt.usrSWKRTHCost>(e.Cache, null, false);
        }

        #endregion
    }
}
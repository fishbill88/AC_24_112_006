//using POInventoryCustomization;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.Licensing;
using PX.Objects.CA.Light;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HubspotCustomization
{
    public class SOOrderEntry_Extension : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() => true;

        #region Overrides

        public delegate IEnumerable PrepareInvoiceDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable PrepareInvoice(PXAdapter adapter, PrepareInvoiceDelegate baseMethod)
        {
            CheckItemsForFlaggedNonStockItem();
            return baseMethod(adapter);
        }


        public delegate IEnumerable CreateShipmentIssueDelegate(PXAdapter adapter, Nullable<DateTime> shipDate, Nullable<Int32> siteID);
        [PXOverride]
        public IEnumerable CreateShipmentIssue(PXAdapter adapter, Nullable<DateTime> shipDate, Nullable<Int32> siteID, CreateShipmentIssueDelegate baseMethod)
        {
            PXGraph.InstanceCreated.AddHandler<SOShipmentEntry>((graphShipmentEntry) =>
            {
                graphShipmentEntry.RowPersisting.AddHandler<SOShipment>((sender, e) =>
                {
                    var AttributeBILLCOMPLE = Base.Document.Cache.GetValueExt(Base.Document.Current, "AttributeBILLCOMPLE");
                    if (AttributeBILLCOMPLE != null)
                        graphShipmentEntry.Document.Cache.SetValueExt(graphShipmentEntry.Document.Current, "AttributeBILLCOMPLE", AttributeBILLCOMPLE.ToString());
                    var printMethod = Base.Document.Cache.GetValueExt(Base.Document.Current, "AttributeFORMTYPE");
                    if (printMethod != null)
                        graphShipmentEntry.Document.Cache.SetValueExt(graphShipmentEntry.Document.Current, "AttributeFORMTYPE", printMethod.ToString());
                });
            });
            return baseMethod(adapter, shipDate, siteID);
        }
        #endregion

        #region Event Handlers

        protected virtual void _(Events.RowSelected<SOOrder> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            SOOrder order = (SOOrder)e.Row;
            if (order == null) return;

            //disable SOOrderExt usrHubspotDealID
            PXUIFieldAttribute.SetEnabled<SOOrderExt.usrHubspotDealID>(e.Cache, order, false);
        }

        public void CheckItemsForFlaggedNonStockItem()
        {
            List<int?> nonStockItems = new List<int?>();
            List<string> invalidNonstock = new List<string>();
            SOSetupExt setupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
            if (setupExt == null) return;
            if (setupExt.UsrNonstock1 != null)
            {
                nonStockItems.Add(setupExt.UsrNonstock1);
            }
            if (setupExt.UsrNonstock2 != null)
            {
                nonStockItems.Add(setupExt.UsrNonstock2);
            }
            if (setupExt.UsrNonstock3 != null)
            {
                nonStockItems.Add(setupExt.UsrNonstock3);
            }

            foreach (SOLine line in Base.Transactions.Select())
            {
                bool hasError = false;
                //check if the line's inventory ID is in the list of non-stock items, then show a popup stating user cannot invoice the non-stock item
                if (nonStockItems.Contains(line.InventoryID))
                {
                    InventoryItem inventoryItem = PXSelect<InventoryItem,
                        Where<InventoryItem.inventoryID, Equal<Required<SOLine.inventoryID>>>>
                        .Select(Base, line.InventoryID);
                    invalidNonstock.Add(inventoryItem.InventoryCD);

                    PXUIFieldAttribute.SetWarning<SOLine.inventoryID>(Base.Transactions.Cache, line, "You cannot invoice this non-stock item.");
                    hasError = true;
                }
                if (hasError)
                {
                    // Show a popup or message to the user
                    throw new PXException("You cannot invoice non-stock items: " + string.Join(", ", invalidNonstock));
                }
            }
        }

        #endregion
    }
}
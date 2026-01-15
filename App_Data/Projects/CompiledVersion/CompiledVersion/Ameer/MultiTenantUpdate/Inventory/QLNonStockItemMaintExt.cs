using PX.Data;
using PX.Objects.IN;
using PX.Objects.GL;
using PX.Objects.CS;
using System;
using System.Collections.Generic;

namespace MTUInventory
{
    public class QLNonStockItemMaintExt : PXGraphExtension<NonStockItemMaint>
    {
        public static bool IsActive() => true;

        public List<string> otherTenantNames;

        public bool IsExtEnabled { get; set; } = true;

        private bool IsMultiTenantEnabled()
        {
            INSetup setup = null;
            foreach (PXResult<INSetup> result in PXSelectReadonly<INSetup>.Select(this.Base))
            {
                setup = result;
                break;
            }
            if (setup == null) return false;

            var setupExt = PXCache<INSetup>.GetExtension<QLINSetupExt>(setup);
            return setupExt?.UsrMultiTenantNonStockItem == true;
        }

        public override void Initialize()
        {
            using (CustomSqlConnection sqlConnection = new CustomSqlConnection(PXDatabase.Provider.GetConnectionString()))
            {
                this.otherTenantNames = sqlConnection.GetOtherCompanyNames(base.Base.Accessinfo.CompanyName);
            }
        }

        protected virtual void _(Events.RowPersisted<InventoryItem> e)
        {
            if (!IsMultiTenantEnabled()) return;
            if (!this.IsExtEnabled) return;
            if (e.Row == null) return;
            
            InventoryItem item = e.Row;
            if (item.StkItem == true) return;

            if (e.TranStatus == PXTranStatus.Completed && (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update))
            {
                string inventoryCD = item.InventoryCD;
                List<string> tenants = new List<string>(this.otherTenantNames);
                
                PXLongOperation.StartOperation(this.Base, delegate()
                {
                    SyncNonStockItem(inventoryCD, tenants);
                });
            }
            else if (e.TranStatus == PXTranStatus.Completed && e.Operation == PXDBOperation.Delete)
            {
                string inventoryCD = item.InventoryCD;
                List<string> tenants = new List<string>(this.otherTenantNames);
                
                PXLongOperation.StartOperation(this.Base, delegate()
                {
                    DeleteNonStockItem(inventoryCD, tenants);
                });
            }
        }

        private static void SyncNonStockItem(string inventoryCD, List<string> tenants)
        {
            // Read source data BEFORE entering any tenant login scope
            NonStockItemMaint sourceGraph = PXGraph.CreateInstance<NonStockItemMaint>();
            InventoryItem sourceItem = sourceGraph.Item.Search<InventoryItem.inventoryCD>(inventoryCD);
            
            if (sourceItem == null)
            {
                PXTrace.WriteWarning($"[Non-Stock Sync] Source item {inventoryCD} not found in source tenant");
                return;
            }

            PXTrace.WriteInformation($"[Non-Stock Sync] Reading source item {inventoryCD} from source tenant");

            // Capture all source lookup CDs before switching tenants
            SourceItemData sourceData = new SourceItemData();
            sourceData.InventoryCD = sourceItem.InventoryCD;
            sourceData.Descr = sourceItem.Descr;
            sourceData.ItemStatus = sourceItem.ItemStatus;
            sourceData.ItemType = sourceItem.ItemType;
            sourceData.BaseUnit = sourceItem.BaseUnit;
            sourceData.SalesUnit = sourceItem.SalesUnit;
            sourceData.PurchaseUnit = sourceItem.PurchaseUnit;
            sourceData.PostClassID = sourceItem.PostClassID;
            sourceData.BasePrice = sourceItem.BasePrice;
            sourceData.BaseWeight = sourceItem.BaseWeight;
            sourceData.BaseVolume = sourceItem.BaseVolume;
            sourceData.WeightUOM = sourceItem.WeightUOM;
            sourceData.VolumeUOM = sourceItem.VolumeUOM;
            sourceData.TaxCategoryID = sourceItem.TaxCategoryID;
            sourceData.Visibility = sourceItem.Visibility;

            // Get Item Class CD
            if (sourceItem.ItemClassID != null)
            {
                INItemClass itemClass = PXSelectReadonly<INItemClass,
                    Where<INItemClass.itemClassID, Equal<Required<INItemClass.itemClassID>>>>.Select(sourceGraph, sourceItem.ItemClassID);
                sourceData.ItemClassCD = itemClass?.ItemClassCD;
            }

            // Get Site CD
            if (sourceItem.DfltSiteID != null)
            {
                INSite site = PXSelectReadonly<INSite,
                    Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(sourceGraph, sourceItem.DfltSiteID);
                sourceData.DfltSiteCD = site?.SiteCD;
            }

            // Get Account CDs
            if (sourceItem.InvtAcctID != null)
            {
                Account acct = PXSelectReadonly<Account,
                    Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(sourceGraph, sourceItem.InvtAcctID);
                sourceData.InvtAcctCD = acct?.AccountCD;
            }
            if (sourceItem.InvtSubID != null)
            {
                Sub sub = PXSelectReadonly<Sub,
                    Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(sourceGraph, sourceItem.InvtSubID);
                sourceData.InvtSubCD = sub?.SubCD;
            }
            if (sourceItem.SalesAcctID != null)
            {
                Account acct = PXSelectReadonly<Account,
                    Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(sourceGraph, sourceItem.SalesAcctID);
                sourceData.SalesAcctCD = acct?.AccountCD;
            }
            if (sourceItem.SalesSubID != null)
            {
                Sub sub = PXSelectReadonly<Sub,
                    Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(sourceGraph, sourceItem.SalesSubID);
                sourceData.SalesSubCD = sub?.SubCD;
            }
            if (sourceItem.COGSAcctID != null)
            {
                Account acct = PXSelectReadonly<Account,
                    Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(sourceGraph, sourceItem.COGSAcctID);
                sourceData.COGSAcctCD = acct?.AccountCD;
            }
            if (sourceItem.COGSSubID != null)
            {
                Sub sub = PXSelectReadonly<Sub,
                    Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(sourceGraph, sourceItem.COGSSubID);
                sourceData.COGSSubCD = sub?.SubCD;
            }

            PXTrace.WriteInformation($"[Non-Stock Sync] Source data captured. Syncing to {tenants.Count} tenants");

            // Now sync to each target tenant
            foreach (string tenantName in tenants)
            {
                try
                {
                    PXTrace.WriteInformation($"[Non-Stock Sync] Syncing {inventoryCD} to tenant {tenantName}");
                    
                    using (PXLoginScope loginScope = new PXLoginScope($"admin@{tenantName}"))
                    {
                        NonStockItemMaint targetGraph = PXGraph.CreateInstance<NonStockItemMaint>();
                        var targetGraphExt = targetGraph.GetExtension<QLNonStockItemMaintExt>();
                        if (targetGraphExt != null)
                        {
                            targetGraphExt.IsExtEnabled = false;
                        }

                        InventoryItem existingItem = null;
                        foreach (PXResult<InventoryItem> result in PXSelectReadonly<InventoryItem,
                            Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(targetGraph, inventoryCD))
                        {
                            existingItem = result;
                            break;
                        }

                        if (existingItem != null)
                        {
                            InventoryItem targetItem = targetGraph.Item.Search<InventoryItem.inventoryCD>(inventoryCD);
                            if (targetItem != null)
                            {
                                UpdateItemFromSourceData(sourceData, targetItem, targetGraph);
                                targetGraph.Actions.PressSave();
                                PXTrace.WriteInformation($"[Non-Stock Sync] Updated {inventoryCD} in tenant {tenantName}");
                            }
                        }
                        else
                        {
                            InventoryItem newItem = targetGraph.Item.Insert();
                            newItem.InventoryCD = inventoryCD;
                            newItem = targetGraph.Item.Update(newItem);
                            UpdateItemFromSourceData(sourceData, newItem, targetGraph);
                            targetGraph.Actions.PressSave();
                            PXTrace.WriteInformation($"[Non-Stock Sync] Created {inventoryCD} in tenant {tenantName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    PXTrace.WriteError($"Error syncing non-stock item {inventoryCD} to tenant {tenantName}: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        // Data class to hold source item values
        private class SourceItemData
        {
            public string InventoryCD;
            public string Descr;
            public string ItemStatus;
            public string ItemType;
            public string BaseUnit;
            public string SalesUnit;
            public string PurchaseUnit;
            public string PostClassID;
            public decimal? BasePrice;
            public decimal? BaseWeight;
            public decimal? BaseVolume;
            public string WeightUOM;
            public string VolumeUOM;
            public string TaxCategoryID;
            public string Visibility;
            public string ItemClassCD;
            public string DfltSiteCD;
            public string InvtAcctCD;
            public string InvtSubCD;
            public string SalesAcctCD;
            public string SalesSubCD;
            public string COGSAcctCD;
            public string COGSSubCD;
        }

        private static void DeleteNonStockItem(string inventoryCD, List<string> tenants)
        {
            foreach (string tenantName in tenants)
            {
                try
                {
                    using (PXLoginScope loginScope = new PXLoginScope($"admin@{tenantName}"))
                    {
                        NonStockItemMaint targetGraph = PXGraph.CreateInstance<NonStockItemMaint>();
                        var targetGraphExt = targetGraph.GetExtension<QLNonStockItemMaintExt>();
                        if (targetGraphExt != null)
                        {
                            targetGraphExt.IsExtEnabled = false;
                        }

                        InventoryItem existingItem = null;
                        foreach (PXResult<InventoryItem> result in PXSelectReadonly<InventoryItem,
                            Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(targetGraph, inventoryCD))
                        {
                            existingItem = result;
                            break;
                        }

                        if (existingItem != null)
                        {
                            InventoryItem itemToDelete = targetGraph.Item.Search<InventoryItem.inventoryCD>(inventoryCD);
                            if (itemToDelete != null)
                            {
                                targetGraph.Item.Delete(itemToDelete);
                                targetGraph.Actions.PressSave();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    PXTrace.WriteError($"Error deleting non-stock item {inventoryCD} from tenant {tenantName}: {ex.Message}");
                }
            }
        }

        private static void UpdateItemFromSourceData(SourceItemData sourceData, InventoryItem targetItem, NonStockItemMaint targetGraph)
        {
            PXTrace.WriteInformation($"[Non-Stock Sync] Applying source data to item: {sourceData.InventoryCD}");
            
            // Core Fields
            targetItem.Descr = sourceData.Descr;
            targetItem.ItemStatus = sourceData.ItemStatus;
            targetItem.ItemType = sourceData.ItemType;
            targetItem.BaseUnit = sourceData.BaseUnit;
            targetItem.SalesUnit = sourceData.SalesUnit;
            targetItem.PurchaseUnit = sourceData.PurchaseUnit;
            targetItem.PostClassID = sourceData.PostClassID;

            // Item Class ID Mapping (lookup by CD in target tenant)
            if (!string.IsNullOrEmpty(sourceData.ItemClassCD))
            {
                INItemClass targetItemClass = PXSelectReadonly<INItemClass,
                    Where<INItemClass.itemClassCD, Equal<Required<INItemClass.itemClassCD>>>>.Select(targetGraph, sourceData.ItemClassCD);
                if (targetItemClass != null)
                {
                    targetItem.ItemClassID = targetItemClass.ItemClassID;
                }
            }

            // Default Warehouse (lookup by CD in target tenant)
            int? targetSiteID = null;
            if (!string.IsNullOrEmpty(sourceData.DfltSiteCD))
            {
                INSite targetSite = PXSelectReadonly<INSite,
                    Where<INSite.siteCD, Equal<Required<INSite.siteCD>>>>.Select(targetGraph, sourceData.DfltSiteCD);
                if (targetSite != null)
                {
                    targetSiteID = targetSite.SiteID;
                }
            }

            // GL Accounts - Expense Account
            if (!string.IsNullOrEmpty(sourceData.InvtAcctCD))
            {
                Account targetAcct = PXSelectReadonly<Account,
                    Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Select(targetGraph, sourceData.InvtAcctCD);
                if (targetAcct != null)
                {
                    targetItem.InvtAcctID = targetAcct.AccountID;
                }
            }

            // GL Accounts - Expense Sub
            if (!string.IsNullOrEmpty(sourceData.InvtSubCD))
            {
                Sub targetSub = PXSelectReadonly<Sub,
                    Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Select(targetGraph, sourceData.InvtSubCD);
                if (targetSub != null)
                {
                    targetItem.InvtSubID = targetSub.SubID;
                }
            }

            // Sales Account
            if (!string.IsNullOrEmpty(sourceData.SalesAcctCD))
            {
                Account targetAcct = PXSelectReadonly<Account,
                    Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Select(targetGraph, sourceData.SalesAcctCD);
                if (targetAcct != null)
                {
                    targetItem.SalesAcctID = targetAcct.AccountID;
                }
            }

            // Sales Sub
            if (!string.IsNullOrEmpty(sourceData.SalesSubCD))
            {
                Sub targetSub = PXSelectReadonly<Sub,
                    Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Select(targetGraph, sourceData.SalesSubCD);
                if (targetSub != null)
                {
                    targetItem.SalesSubID = targetSub.SubID;
                }
            }

            // COGS Account
            if (!string.IsNullOrEmpty(sourceData.COGSAcctCD))
            {
                Account targetAcct = PXSelectReadonly<Account,
                    Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Select(targetGraph, sourceData.COGSAcctCD);
                if (targetAcct != null)
                {
                    targetItem.COGSAcctID = targetAcct.AccountID;
                }
            }

            // COGS Sub
            if (!string.IsNullOrEmpty(sourceData.COGSSubCD))
            {
                Sub targetSub = PXSelectReadonly<Sub,
                    Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Select(targetGraph, sourceData.COGSSubCD);
                if (targetSub != null)
                {
                    targetItem.COGSSubID = targetSub.SubID;
                }
            }

            targetItem = targetGraph.Item.Update(targetItem);

            // Base Price and Physical Properties
            if (sourceData.BasePrice != null)
                targetItem.BasePrice = sourceData.BasePrice;

            if (targetSiteID != null)
                targetItem.DfltSiteID = targetSiteID;
            targetItem.BaseWeight = sourceData.BaseWeight;
            targetItem.BaseVolume = sourceData.BaseVolume;
            targetItem.WeightUOM = sourceData.WeightUOM;
            targetItem.VolumeUOM = sourceData.VolumeUOM;

            // Additional Properties
            targetItem.TaxCategoryID = sourceData.TaxCategoryID;
            targetItem.Visibility = sourceData.Visibility;

            targetItem = targetGraph.Item.Update(targetItem);
            
            PXTrace.WriteInformation($"[Non-Stock Sync] Completed applying data for item: {sourceData.InventoryCD}");
        }
    }
}

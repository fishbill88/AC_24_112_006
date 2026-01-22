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

        private bool IsMultiTenantSendEnabled()
        {
            INSetup setup = null;
            foreach (PXResult<INSetup> result in PXSelectReadonly<INSetup>.Select(this.Base))
            {
                setup = result;
                break;
            }
            if (setup == null) return false;

            var setupExt = PXCache<INSetup>.GetExtension<QLINSetupExt>(setup);
            // Check new Send field first, then fallback to deprecated field for backward compatibility
            return setupExt?.UsrMultiTenantNonStockItemSend == true || setupExt?.UsrMultiTenantNonStockItem == true;
        }

        private static bool IsVerboseLoggingEnabled(PXGraph graph)
        {
            INSetup setup = null;
            foreach (PXResult<INSetup> result in PXSelectReadonly<INSetup>.Select(graph))
            {
                setup = result;
                break;
            }
            if (setup == null) return false;

            var setupExt = PXCache<INSetup>.GetExtension<QLINSetupExt>(setup);
            return setupExt?.UsrEnableVerboseSyncLogging == true;
        }

        public override void Initialize()
        {
            using (CustomSqlConnection sqlConnection = new CustomSqlConnection(PXDatabase.Provider.GetConnectionString()))
            {
                var allTenants = sqlConnection.GetOtherCompanyNames(base.Base.Accessinfo.CompanyName);
                PXTrace.WriteInformation($"[Non-Stock Sync] Found {allTenants.Count} other tenants");
                this.otherTenantNames = MultiTenantSyncCache.GetEnabledTenantsForNonStockItems(allTenants);
                PXTrace.WriteInformation($"[Non-Stock Sync] {this.otherTenantNames.Count} tenants enabled to receive non-stock items");
            }
        }

        protected virtual void _(Events.RowPersisted<InventoryItem> e)
        {
            if (!IsMultiTenantSendEnabled()) return;
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
                SyncResult result = new SyncResult(tenantName, inventoryCD);
                
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

                        bool verboseLogging = IsVerboseLoggingEnabled(targetGraph);

                        InventoryItem existingItem = null;
                        foreach (PXResult<InventoryItem> pxResult in PXSelectReadonly<InventoryItem,
                            Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(targetGraph, inventoryCD))
                        {
                            existingItem = pxResult;
                            break;
                        }

                        if (existingItem != null)
                        {
                            InventoryItem targetItem = targetGraph.Item.Search<InventoryItem.inventoryCD>(inventoryCD);
                            if (targetItem != null)
                            {
                                UpdateItemFromSourceData(sourceData, targetItem, targetGraph, result, verboseLogging);
                                
                                try
                                {
                                    targetGraph.Actions.PressSave();
                                    result.Success = true;
                                    PXTrace.WriteInformation($"[Non-Stock Sync] Updated {inventoryCD} in tenant {tenantName} - {result.GetSummary()}");
                                }
                                catch (Exception saveEx)
                                {
                                    result.Success = false;
                                    result.ErrorMessage = $"Save failed: {saveEx.Message}";
                                    PXTrace.WriteWarning($"[Non-Stock Sync] Save failed for {inventoryCD} in tenant {tenantName}: {saveEx.Message}");
                                }
                            }
                        }
                        else
                        {
                            InventoryItem newItem = targetGraph.Item.Insert();
                            newItem.InventoryCD = inventoryCD;
                            newItem = targetGraph.Item.Update(newItem);
                            UpdateItemFromSourceData(sourceData, newItem, targetGraph, result, verboseLogging);
                            
                            try
                            {
                                targetGraph.Actions.PressSave();
                                result.Success = true;
                                PXTrace.WriteInformation($"[Non-Stock Sync] Created {inventoryCD} in tenant {tenantName} - {result.GetSummary()}");
                            }
                            catch (Exception saveEx)
                            {
                                result.Success = false;
                                result.ErrorMessage = $"Save failed: {saveEx.Message}";
                                PXTrace.WriteWarning($"[Non-Stock Sync] Save failed for {inventoryCD} in tenant {tenantName}: {saveEx.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = ex.Message;
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

        private static void UpdateItemFromSourceData(SourceItemData sourceData, InventoryItem targetItem, NonStockItemMaint targetGraph, SyncResult syncResult, bool verboseLogging)
        {
            if (verboseLogging)
                PXTrace.WriteInformation($"[Non-Stock Sync] Applying source data to item: {sourceData.InventoryCD}");
            
            // Core Fields - Direct assignment (no lookup needed)
            try
            {
                targetItem.Descr = sourceData.Descr;
                targetItem.ItemStatus = sourceData.ItemStatus;
                targetItem.ItemType = sourceData.ItemType;
                targetItem.BaseUnit = sourceData.BaseUnit;
                targetItem.SalesUnit = sourceData.SalesUnit;
                targetItem.PurchaseUnit = sourceData.PurchaseUnit;
                targetItem.PostClassID = sourceData.PostClassID;
                
                syncResult.FieldResults.Add(new FieldSyncResult("CoreFields", FieldSyncStatus.Success, null, null));
            }
            catch (Exception ex)
            {
                syncResult.FieldResults.Add(new FieldSyncResult("CoreFields", FieldSyncStatus.SkippedError, null, $"Error: {ex.Message}"));
                if (verboseLogging)
                    PXTrace.WriteWarning($"[Non-Stock Sync] Core fields error: {ex.Message}");
            }

            // Item Class ID Mapping (lookup by CD in target tenant)
            TrySafeItemClassMapping(sourceData, targetItem, targetGraph, syncResult, verboseLogging);

            // Default Warehouse (lookup by CD in target tenant)
            int? targetSiteID = TrySafeSiteMapping(sourceData, targetGraph, syncResult, verboseLogging);

            // GL Accounts - Expense Account
            TrySafeAccountMapping("InvtAccount", sourceData.InvtAcctCD, 
                (acctID) => targetItem.InvtAcctID = acctID, targetGraph, syncResult, verboseLogging);

            // GL Accounts - Expense Sub
            TrySafeSubMapping("InvtSub", sourceData.InvtSubCD, 
                (subID) => targetItem.InvtSubID = subID, targetGraph, syncResult, verboseLogging);

            // Sales Account
            TrySafeAccountMapping("SalesAccount", sourceData.SalesAcctCD, 
                (acctID) => targetItem.SalesAcctID = acctID, targetGraph, syncResult, verboseLogging);

            // Sales Sub
            TrySafeSubMapping("SalesSub", sourceData.SalesSubCD, 
                (subID) => targetItem.SalesSubID = subID, targetGraph, syncResult, verboseLogging);

            // COGS Account
            TrySafeAccountMapping("COGSAccount", sourceData.COGSAcctCD, 
                (acctID) => targetItem.COGSAcctID = acctID, targetGraph, syncResult, verboseLogging);

            // COGS Sub
            TrySafeSubMapping("COGSSub", sourceData.COGSSubCD, 
                (subID) => targetItem.COGSSubID = subID, targetGraph, syncResult, verboseLogging);

            // First Update call
            try
            {
                targetItem = targetGraph.Item.Update(targetItem);
            }
            catch (Exception ex)
            {
                syncResult.FieldResults.Add(new FieldSyncResult("FirstUpdate", FieldSyncStatus.SkippedError, null, $"Update failed: {ex.Message}"));
                if (verboseLogging)
                    PXTrace.WriteWarning($"[Non-Stock Sync] First Update() failed: {ex.Message}");
            }

            // Base Price and Physical Properties
            try
            {
                if (sourceData.BasePrice != null)
                    targetItem.BasePrice = sourceData.BasePrice;

                if (targetSiteID != null)
                    targetItem.DfltSiteID = targetSiteID;
                    
                targetItem.BaseWeight = sourceData.BaseWeight;
                targetItem.BaseVolume = sourceData.BaseVolume;
                targetItem.WeightUOM = sourceData.WeightUOM;
                targetItem.VolumeUOM = sourceData.VolumeUOM;
                targetItem.TaxCategoryID = sourceData.TaxCategoryID;
                targetItem.Visibility = sourceData.Visibility;
                
                syncResult.FieldResults.Add(new FieldSyncResult("PriceAndProperties", FieldSyncStatus.Success, null, null));
            }
            catch (Exception ex)
            {
                syncResult.FieldResults.Add(new FieldSyncResult("PriceAndProperties", FieldSyncStatus.SkippedError, null, $"Error: {ex.Message}"));
                if (verboseLogging)
                    PXTrace.WriteWarning($"[Non-Stock Sync] Price/Properties error: {ex.Message}");
            }

            // Second Update call
            try
            {
                targetItem = targetGraph.Item.Update(targetItem);
            }
            catch (Exception ex)
            {
                syncResult.FieldResults.Add(new FieldSyncResult("SecondUpdate", FieldSyncStatus.SkippedError, null, $"Update failed: {ex.Message}"));
                if (verboseLogging)
                    PXTrace.WriteWarning($"[Non-Stock Sync] Second Update() failed: {ex.Message}");
            }
            
            if (verboseLogging)
                PXTrace.WriteInformation($"[Non-Stock Sync] Completed applying data for item: {sourceData.InventoryCD}");
        }

        private static void TrySafeItemClassMapping(SourceItemData sourceData, InventoryItem targetItem, NonStockItemMaint targetGraph, SyncResult syncResult, bool verboseLogging)
        {
            if (string.IsNullOrEmpty(sourceData.ItemClassCD))
            {
                syncResult.FieldResults.Add(new FieldSyncResult("ItemClass", FieldSyncStatus.SkippedNull, null, "Source ItemClass is null"));
                return;
            }

            try
            {
                INItemClass targetItemClass = PXSelectReadonly<INItemClass,
                    Where<INItemClass.itemClassCD, Equal<Required<INItemClass.itemClassCD>>>>.Select(targetGraph, sourceData.ItemClassCD);
                
                if (targetItemClass != null)
                {
                    targetItem.ItemClassID = targetItemClass.ItemClassID;
                    syncResult.FieldResults.Add(new FieldSyncResult("ItemClass", FieldSyncStatus.Success, sourceData.ItemClassCD, null));
                }
                else
                {
                    syncResult.FieldResults.Add(new FieldSyncResult("ItemClass", FieldSyncStatus.SkippedNotFound, sourceData.ItemClassCD, 
                        $"Item Class '{sourceData.ItemClassCD}' not found in target tenant"));
                    PXTrace.WriteWarning($"[Non-Stock Sync] Item Class '{sourceData.ItemClassCD}' not found in target tenant");
                }
            }
            catch (Exception ex)
            {
                syncResult.FieldResults.Add(new FieldSyncResult("ItemClass", FieldSyncStatus.SkippedError, sourceData.ItemClassCD, $"Error: {ex.Message}"));
                if (verboseLogging)
                    PXTrace.WriteWarning($"[Non-Stock Sync] ItemClass mapping error: {ex.Message}");
            }
        }

        private static int? TrySafeSiteMapping(SourceItemData sourceData, NonStockItemMaint targetGraph, SyncResult syncResult, bool verboseLogging)
        {
            if (string.IsNullOrEmpty(sourceData.DfltSiteCD))
            {
                syncResult.FieldResults.Add(new FieldSyncResult("DefaultSite", FieldSyncStatus.SkippedNull, null, "Source Site is null"));
                return null;
            }

            try
            {
                INSite targetSite = PXSelectReadonly<INSite,
                    Where<INSite.siteCD, Equal<Required<INSite.siteCD>>>>.Select(targetGraph, sourceData.DfltSiteCD);
                
                if (targetSite != null)
                {
                    syncResult.FieldResults.Add(new FieldSyncResult("DefaultSite", FieldSyncStatus.Success, sourceData.DfltSiteCD, null));
                    return targetSite.SiteID;
                }
                else
                {
                    syncResult.FieldResults.Add(new FieldSyncResult("DefaultSite", FieldSyncStatus.SkippedNotFound, sourceData.DfltSiteCD, 
                        $"Site '{sourceData.DfltSiteCD}' not found in target tenant"));
                    PXTrace.WriteWarning($"[Non-Stock Sync] Site '{sourceData.DfltSiteCD}' not found in target tenant");
                    return null;
                }
            }
            catch (Exception ex)
            {
                syncResult.FieldResults.Add(new FieldSyncResult("DefaultSite", FieldSyncStatus.SkippedError, sourceData.DfltSiteCD, $"Error: {ex.Message}"));
                if (verboseLogging)
                    PXTrace.WriteWarning($"[Non-Stock Sync] Site mapping error: {ex.Message}");
                return null;
            }
        }

        private static void TrySafeAccountMapping(string fieldName, string accountCD, Action<int?> setAction, NonStockItemMaint targetGraph, SyncResult syncResult, bool verboseLogging)
        {
            if (string.IsNullOrEmpty(accountCD))
            {
                syncResult.FieldResults.Add(new FieldSyncResult(fieldName, FieldSyncStatus.SkippedNull, null, $"Source {fieldName} is null"));
                return;
            }

            try
            {
                Account targetAcct = PXSelectReadonly<Account,
                    Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Select(targetGraph, accountCD);
                
                if (targetAcct != null)
                {
                    setAction(targetAcct.AccountID);
                    syncResult.FieldResults.Add(new FieldSyncResult(fieldName, FieldSyncStatus.Success, accountCD, null));
                }
                else
                {
                    syncResult.FieldResults.Add(new FieldSyncResult(fieldName, FieldSyncStatus.SkippedNotFound, accountCD, 
                        $"Account '{accountCD}' not found in target tenant"));
                    PXTrace.WriteWarning($"[Non-Stock Sync] Account '{accountCD}' ({fieldName}) not found in target tenant");
                }
            }
            catch (Exception ex)
            {
                syncResult.FieldResults.Add(new FieldSyncResult(fieldName, FieldSyncStatus.SkippedError, accountCD, $"Error: {ex.Message}"));
                if (verboseLogging)
                    PXTrace.WriteWarning($"[Non-Stock Sync] {fieldName} mapping error: {ex.Message}");
            }
        }

        private static void TrySafeSubMapping(string fieldName, string subCD, Action<int?> setAction, NonStockItemMaint targetGraph, SyncResult syncResult, bool verboseLogging)
        {
            if (string.IsNullOrEmpty(subCD))
            {
                syncResult.FieldResults.Add(new FieldSyncResult(fieldName, FieldSyncStatus.SkippedNull, null, $"Source {fieldName} is null"));
                return;
            }

            try
            {
                Sub targetSub = PXSelectReadonly<Sub,
                    Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Select(targetGraph, subCD);
                
                if (targetSub != null)
                {
                    setAction(targetSub.SubID);
                    syncResult.FieldResults.Add(new FieldSyncResult(fieldName, FieldSyncStatus.Success, subCD, null));
                }
                else
                {
                    syncResult.FieldResults.Add(new FieldSyncResult(fieldName, FieldSyncStatus.SkippedNotFound, subCD, 
                        $"Sub-Account '{subCD}' not found in target tenant"));
                    PXTrace.WriteWarning($"[Non-Stock Sync] Sub-Account '{subCD}' ({fieldName}) not found in target tenant");
                }
            }
            catch (Exception ex)
            {
                syncResult.FieldResults.Add(new FieldSyncResult(fieldName, FieldSyncStatus.SkippedError, subCD, $"Error: {ex.Message}"));
                if (verboseLogging)
                    PXTrace.WriteWarning($"[Non-Stock Sync] {fieldName} mapping error: {ex.Message}");
            }
        }
    }
}

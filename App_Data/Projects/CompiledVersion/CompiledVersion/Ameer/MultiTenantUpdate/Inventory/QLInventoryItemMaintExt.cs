using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.GL;
using PX.Objects.AP;

namespace MTUInventory
{
    public class QLInventoryItemMaintExt : PXGraphExtension<InventoryItemMaint>
    {
        private List<string> targetTenantNames;
        private static bool IsExtEnabled = true;

        protected void _(Events.RowPersisted<InventoryItem> e)
        {
            if (e.TranStatus == PXTranStatus.Open && IsExtEnabled && IsMultiTenantSendEnabled() &&
                (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update))
            {
                InventoryItem item = e.Row;
                PXTrace.WriteInformation($"Processing item {item.InventoryCD}, TranStatus={e.TranStatus}, Operation={e.Operation}, Update={e.Operation == PXDBOperation.Update}");
                Initialize();
                SyncToOtherTenants(item);
            }
        }

        private bool IsMultiTenantSendEnabled()
        {
            INSetup setup = null;
            foreach (PXResult<INSetup> result in PXSelect<INSetup>.Select(this.Base))
            {
                setup = result;
                break;
            }
            if (setup == null) return false;
            QLINSetupExt setupExt = PXCache<INSetup>.GetExtension<QLINSetupExt>(setup);
            // Check new Send field first, fallback to deprecated field for backward compatibility
            return setupExt?.UsrMultiTenantStockItemSend == true || setupExt?.UsrMultiTenantStockItem == true;
        }

        private void Initialize()
        {
            string callerTenantName = base.Base.Accessinfo.CompanyName;
            List<string> allTenants;
            using (CustomSqlConnection conn = new CustomSqlConnection(PX.Data.PXDatabase.Provider.GetConnectionString()))
            {
                allTenants = conn.GetOtherCompanyNames(callerTenantName);
            }
            PXTrace.WriteInformation($"[Stock Sync] Found {allTenants.Count} other tenants");
            targetTenantNames = MultiTenantSyncCache.GetEnabledTenantsForStockItems(allTenants);
            PXTrace.WriteInformation($"[Stock Sync] {targetTenantNames.Count} tenants enabled to receive stock items");
        }

        private void SyncToOtherTenants(InventoryItem item)
        {
            if (targetTenantNames == null || targetTenantNames.Count == 0) return;

            PXTrace.WriteInformation($"Starting sync for {item.InventoryCD} to {targetTenantNames.Count} tenants");

            foreach (string tenantName in targetTenantNames)
            {
                try
                {
                    PXTrace.WriteInformation($"Syncing {item.InventoryCD} to tenant: {tenantName}");
                    using (PXLoginScope loginScope = new PXLoginScope($"admin@{tenantName}"))
                    {
                        InventoryItemMaint targetGraph = PXGraph.CreateInstance<InventoryItemMaint>();

                        // Disable the extension in the target graph
                        QLInventoryItemMaintExt targetExt = targetGraph.GetExtension<QLInventoryItemMaintExt>();
                        if (targetExt != null)
                        {
                            IsExtEnabled = false;
                        }

                        try
                        {
                            InventoryItem targetItem = null;
                            InventoryItem existingItem = null;
                            
                            foreach (PXResult<InventoryItem> result in PXSelect<InventoryItem,
                                Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(targetGraph, item.InventoryCD))
                            {
                                existingItem = result;
                                break;
                            }

                            if (existingItem != null)
                            {
                                targetItem = (InventoryItem)targetGraph.Item.Cache.CreateCopy(existingItem);
                            }
                            else
                            {
                                targetItem = (InventoryItem)targetGraph.Item.Cache.CreateInstance();
                                targetItem.InventoryCD = item.InventoryCD;
                                targetItem.StkItem = item.StkItem;
                            }

                            // Phase 1: Core Fields
                            targetItem.Descr = item.Descr;
                            targetItem.ItemStatus = item.ItemStatus;
                            targetItem.BaseUnit = item.BaseUnit;
                            targetItem.SalesUnit = item.SalesUnit;
                            targetItem.PurchaseUnit = item.PurchaseUnit;
                            targetItem.BasePrice = item.BasePrice;
                            targetItem.PostClassID = item.PostClassID;

                            // Phase 2: Extended Fields
                            targetItem.BaseWeight = item.BaseWeight;
                            targetItem.BaseVolume = item.BaseVolume;
                            targetItem.WeightUOM = item.WeightUOM;
                            targetItem.VolumeUOM = item.VolumeUOM;

                            targetItem.ABCCodeID = item.ABCCodeID;
                            targetItem.ABCCodeIsFixed = item.ABCCodeIsFixed;
                            targetItem.MovementClassID = item.MovementClassID;
                            targetItem.MovementClassIsFixed = item.MovementClassIsFixed;

                            targetItem.LotSerClassID = item.LotSerClassID;
                            targetItem.TaxCategoryID = item.TaxCategoryID;
                            targetItem.ItemType = item.ItemType;
                            targetItem.ValMethod = item.ValMethod;
                            targetItem.Visibility = item.Visibility;

                            // Map fields by CD values
                            UpdateInventoryItemWithGraph(item, targetItem, targetGraph);

                            if (existingItem != null)
                            {
                                targetGraph.Item.Update(targetItem);
                            }
                            else
                            {
                                targetGraph.Item.Insert(targetItem);
                            }

                            targetGraph.Actions.PressSave();
                            PXTrace.WriteInformation($"Successfully synced {item.InventoryCD} to {tenantName}");
                        }
                        finally
                        {
                            IsExtEnabled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    PXTrace.WriteError($"Error syncing inventory item {item.InventoryCD} to tenant {tenantName}: {ex.Message}");
                    PXTrace.WriteError($"Stack: {ex.StackTrace}");
                }
            }
        }

        private void UpdateInventoryItemWithGraph(InventoryItem sourceItem, InventoryItem targetItem, InventoryItemMaint targetGraph)
        {
            try
            {
                PXTrace.WriteInformation("Starting field mappings...");

                // Phase 1: ItemClass Mapping (by ItemClassCD)
                if (sourceItem.ItemClassID != null && sourceItem.ItemClassID.HasValue)
                {
                    PXTrace.WriteInformation($"Mapping ItemClass ID {sourceItem.ItemClassID}");
                    
                    INItemClass sourceItemClass = null;
                    foreach (PXResult<INItemClass> result in PXSelectReadonly<INItemClass,
                        Where<INItemClass.itemClassID, Equal<Required<INItemClass.itemClassID>>>>.Select(this.Base, sourceItem.ItemClassID))
                    {
                        sourceItemClass = result;
                        break;
                    }

                    if (sourceItemClass != null)
                    {
                        PXTrace.WriteInformation($"Source ItemClassCD: {sourceItemClass.ItemClassCD}");

                        INItemClass targetItemClass = null;
                        foreach (PXResult<INItemClass> result in PXSelectReadonly<INItemClass,
                            Where<INItemClass.itemClassCD, Equal<Required<INItemClass.itemClassCD>>>>.Select(targetGraph, sourceItemClass.ItemClassCD))
                        {
                            targetItemClass = result;
                            break;
                        }

                        if (targetItemClass != null)
                        {
                            targetItem.ItemClassID = targetItemClass.ItemClassID;
                            PXTrace.WriteInformation($"ItemClass mapped successfully to ID {targetItemClass.ItemClassID}");
                        }
                        else
                        {
                            PXTrace.WriteInformation($"ItemClass {sourceItemClass.ItemClassCD} not found in target tenant");
                        }
                    }
                }
                else
                {
                    PXTrace.WriteInformation("No ItemClass to map");
                }

                // Phase 1: Default Warehouse Mapping (by SiteCD)
                PXTrace.WriteInformation("Mapping Default Warehouse...");
                int? targetSiteID = null;
                if (sourceItem.DfltSiteID != null)
                {
                    INSite sourceSite = null;
                    foreach (PXResult<INSite> result in PXSelectReadonly<INSite,
                        Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(this.Base, sourceItem.DfltSiteID))
                    {
                        sourceSite = result;
                        break;
                    }

                    if (sourceSite != null)
                    {
                        PXTrace.WriteInformation($"Source SiteCD: {sourceSite.SiteCD}");

                        INSite targetSite = null;
                        foreach (PXResult<INSite> result in PXSelectReadonly<INSite,
                            Where<INSite.siteCD, Equal<Required<INSite.siteCD>>>>.Select(targetGraph, sourceSite.SiteCD))
                        {
                            targetSite = result;
                            break;
                        }

                        if (targetSite != null)
                        {
                            targetSiteID = targetSite.SiteID;
                            PXTrace.WriteInformation($"Warehouse mapped successfully to ID {targetSiteID}");
                        }
                        else
                        {
                            PXTrace.WriteInformation($"Warehouse {sourceSite.SiteCD} not found in target tenant");
                        }
                    }
                }
                else
                {
                    PXTrace.WriteInformation("No Warehouse to map");
                }
                targetItem.DfltSiteID = targetSiteID;

                // Phase 1: Inventory Account Mapping (by AccountCD)
                PXTrace.WriteInformation("Mapping Inventory Account...");
                if (sourceItem.InvtAcctID != null)
                {
                    Account sourceInvtAcct = null;
                    foreach (PXResult<Account> result in PXSelectReadonly<Account,
                        Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this.Base, sourceItem.InvtAcctID))
                    {
                        sourceInvtAcct = result;
                        break;
                    }

                    if (sourceInvtAcct != null)
                    {
                        PXTrace.WriteInformation($"Source Inventory AccountCD: {sourceInvtAcct.AccountCD}");

                        Account targetInvtAcct = null;
                        foreach (PXResult<Account> result in PXSelectReadonly<Account,
                            Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Select(targetGraph, sourceInvtAcct.AccountCD))
                        {
                            targetInvtAcct = result;
                            break;
                        }

                        if (targetInvtAcct != null)
                        {
                            targetItem.InvtAcctID = targetInvtAcct.AccountID;
                            PXTrace.WriteInformation($"Inventory Account mapped successfully");
                        }
                        else
                        {
                            PXTrace.WriteInformation($"Inventory Account {sourceInvtAcct.AccountCD} not found in target");
                        }
                    }
                }
                else
                {
                    PXTrace.WriteInformation("No Inventory Account to map");
                }

                // Phase 1: Inventory Sub-Account Mapping (by SubCD)
                PXTrace.WriteInformation("Mapping Inventory Sub-Account...");
                if (sourceItem.InvtSubID != null)
                {
                    Sub sourceInvtSub = null;
                    foreach (PXResult<Sub> result in PXSelectReadonly<Sub,
                        Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this.Base, sourceItem.InvtSubID))
                    {
                        sourceInvtSub = result;
                        break;
                    }

                    if (sourceInvtSub != null)
                    {
                        PXTrace.WriteInformation($"Source Inventory SubCD: {sourceInvtSub.SubCD}");

                        Sub targetInvtSub = null;
                        foreach (PXResult<Sub> result in PXSelectReadonly<Sub,
                            Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Select(targetGraph, sourceInvtSub.SubCD))
                        {
                            targetInvtSub = result;
                            break;
                        }

                        if (targetInvtSub != null)
                        {
                            targetItem.InvtSubID = targetInvtSub.SubID;
                            PXTrace.WriteInformation($"Inventory Sub mapped successfully");
                        }
                        else
                        {
                            PXTrace.WriteInformation($"Inventory Sub {sourceInvtSub.SubCD} not found in target");
                        }
                    }
                }
                else
                {
                    PXTrace.WriteInformation("No Inventory Sub to map");
                }

                // Phase 1: Sales Account Mapping (by AccountCD)
                PXTrace.WriteInformation("Mapping Sales Account...");
                if (sourceItem.SalesAcctID != null)
                {
                    Account sourceSalesAcct = null;
                    foreach (PXResult<Account> result in PXSelectReadonly<Account,
                        Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this.Base, sourceItem.SalesAcctID))
                    {
                        sourceSalesAcct = result;
                        break;
                    }

                    if (sourceSalesAcct != null)
                    {
                        PXTrace.WriteInformation($"Source Sales AccountCD: {sourceSalesAcct.AccountCD}");

                        Account targetSalesAcct = null;
                        foreach (PXResult<Account> result in PXSelectReadonly<Account,
                            Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Select(targetGraph, sourceSalesAcct.AccountCD))
                        {
                            targetSalesAcct = result;
                            break;
                        }

                        if (targetSalesAcct != null)
                        {
                            targetItem.SalesAcctID = targetSalesAcct.AccountID;
                            PXTrace.WriteInformation($"Sales Account mapped successfully");
                        }
                        else
                        {
                            PXTrace.WriteInformation($"Sales Account {sourceSalesAcct.AccountCD} not found in target");
                        }
                    }
                }
                else
                {
                    PXTrace.WriteInformation("No Sales Account to map");
                }

                // Phase 1: Sales Sub-Account Mapping (by SubCD)
                PXTrace.WriteInformation("Mapping Sales Sub-Account...");
                if (sourceItem.SalesSubID != null)
                {
                    Sub sourceSalesSub = null;
                    foreach (PXResult<Sub> result in PXSelectReadonly<Sub,
                        Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this.Base, sourceItem.SalesSubID))
                    {
                        sourceSalesSub = result;
                        break;
                    }

                    if (sourceSalesSub != null)
                    {
                        PXTrace.WriteInformation($"Source Sales SubCD: {sourceSalesSub.SubCD}");

                        Sub targetSalesSub = null;
                        foreach (PXResult<Sub> result in PXSelectReadonly<Sub,
                            Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Select(targetGraph, sourceSalesSub.SubCD))
                        {
                            targetSalesSub = result;
                            break;
                        }

                        if (targetSalesSub != null)
                        {
                            targetItem.SalesSubID = targetSalesSub.SubID;
                            PXTrace.WriteInformation($"Sales Sub mapped successfully");
                        }
                        else
                        {
                            PXTrace.WriteInformation($"Sales Sub {sourceSalesSub.SubCD} not found in target");
                        }
                    }
                }
                else
                {
                    PXTrace.WriteInformation("No Sales Sub to map");
                }

                // Phase 1: COGS Account Mapping (by AccountCD)
                PXTrace.WriteInformation("Mapping COGS Account...");
                if (sourceItem.COGSAcctID != null)
                {
                    Account sourceCOGSAcct = null;
                    foreach (PXResult<Account> result in PXSelectReadonly<Account,
                        Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this.Base, sourceItem.COGSAcctID))
                    {
                        sourceCOGSAcct = result;
                        break;
                    }

                    if (sourceCOGSAcct != null)
                    {
                        PXTrace.WriteInformation($"Source COGS AccountCD: {sourceCOGSAcct.AccountCD}");

                        Account targetCOGSAcct = null;
                        foreach (PXResult<Account> result in PXSelectReadonly<Account,
                            Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Select(targetGraph, sourceCOGSAcct.AccountCD))
                        {
                            targetCOGSAcct = result;
                            break;
                        }

                        if (targetCOGSAcct != null)
                        {
                            targetItem.COGSAcctID = targetCOGSAcct.AccountID;
                            PXTrace.WriteInformation($"COGS Account mapped successfully");
                        }
                        else
                        {
                            PXTrace.WriteInformation($"COGS Account {sourceCOGSAcct.AccountCD} not found in target");
                        }
                    }
                }
                else
                {
                    PXTrace.WriteInformation("No COGS Account to map");
                }

                // Phase 1: COGS Sub-Account Mapping (by SubCD)
                PXTrace.WriteInformation("Mapping COGS Sub-Account...");
                if (sourceItem.COGSSubID != null)
                {
                    Sub sourceCOGSSub = null;
                    foreach (PXResult<Sub> result in PXSelectReadonly<Sub,
                        Where<Sub.subID, Equal<Required<Sub.subID>>>>.Select(this.Base, sourceItem.COGSSubID))
                    {
                        sourceCOGSSub = result;
                        break;
                    }

                    if (sourceCOGSSub != null)
                    {
                        PXTrace.WriteInformation($"Source COGS SubCD: {sourceCOGSSub.SubCD}");

                        Sub targetCOGSSub = null;
                        foreach (PXResult<Sub> result in PXSelectReadonly<Sub,
                            Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Select(targetGraph, sourceCOGSSub.SubCD))
                        {
                            targetCOGSSub = result;
                            break;
                        }

                        if (targetCOGSSub != null)
                        {
                            targetItem.COGSSubID = targetCOGSSub.SubID;
                            PXTrace.WriteInformation($"COGS Sub mapped successfully");
                        }
                        else
                        {
                            PXTrace.WriteInformation($"COGS Sub {sourceCOGSSub.SubCD} not found in target");
                        }
                    }
                }
                else
                {
                    PXTrace.WriteInformation("No COGS Sub to map");
                }

                // Phase 2: Preferred Vendor Mapping (by AcctCD)
                // Use direct SQL to avoid BAccount cache extension conflicts (Salesforce vs Projects)
                PXTrace.WriteInformation("Mapping Preferred Vendor...");
                if (sourceItem.PreferredVendorID != null)
                {
                    try
                    {
                        using (CustomSqlConnection sqlConn = new CustomSqlConnection(PX.Data.PXDatabase.Provider.GetConnectionString()))
                        {
                            int sourceCompanyID = sqlConn.GetCompanyID(this.Base.Accessinfo.CompanyName);
                            int targetCompanyID = sqlConn.GetCompanyID(targetGraph.Accessinfo.CompanyName);

                            // Get source vendor AcctCD using direct SQL (bypasses cache extensions)
                            string sourceVendorAcctCD = null;
                            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(PX.Data.PXDatabase.Provider.GetConnectionString()))
                            {
                                conn.Open();
                                string sql = "SELECT AcctCD FROM BAccount WHERE BAccountID = @BAccountID AND CompanyID = @CompanyID";
                                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                                {
                                    cmd.Parameters.AddWithValue("@BAccountID", sourceItem.PreferredVendorID);
                                    cmd.Parameters.AddWithValue("@CompanyID", sourceCompanyID);
                                    object result = cmd.ExecuteScalar();
                                    if (result != null && result != DBNull.Value)
                                    {
                                        sourceVendorAcctCD = result.ToString().Trim();
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(sourceVendorAcctCD))
                            {
                                PXTrace.WriteInformation($"Source Vendor AcctCD: {sourceVendorAcctCD}");

                                // Get target vendor BAccountID using direct SQL
                                int? targetVendorID = null;
                                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(PX.Data.PXDatabase.Provider.GetConnectionString()))
                                {
                                    conn.Open();
                                    string sql = "SELECT BAccountID FROM BAccount WHERE AcctCD = @AcctCD AND CompanyID = @CompanyID AND Type IN ('VE', 'VC')";
                                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@AcctCD", sourceVendorAcctCD);
                                        cmd.Parameters.AddWithValue("@CompanyID", targetCompanyID);
                                        object result = cmd.ExecuteScalar();
                                        if (result != null && result != DBNull.Value)
                                        {
                                            targetVendorID = Convert.ToInt32(result);
                                        }
                                    }
                                }

                                if (targetVendorID != null)
                                {
                                    targetItem.PreferredVendorID = targetVendorID;
                                    PXTrace.WriteInformation($"Vendor mapped successfully to ID {targetVendorID}");
                                }
                                else
                                {
                                    PXTrace.WriteInformation($"Vendor {sourceVendorAcctCD} not found in target tenant");
                                }
                            }
                            else
                            {
                                PXTrace.WriteInformation("Source vendor record not found");
                            }
                        }
                    }
                    catch (Exception vendorEx)
                    {
                        PXTrace.WriteWarning($"Vendor mapping failed: {vendorEx.Message}");
                    }
                }
                else
                {
                    PXTrace.WriteInformation("No Vendor to map");
                }

                // Phase 2: Cross-References Sync (Add/Update/Delete)
                SyncCrossReferences(sourceItem, targetItem, targetGraph);

                // Phase 2: Categories Sync (Add/Delete)
                SyncCategories(sourceItem, targetItem, targetGraph);
            }
            catch (Exception ex)
            {
                PXTrace.WriteError($"Error updating inventory item fields: {ex.Message}");
                PXTrace.WriteError($"Stack: {ex.StackTrace}");
                throw;
            }
        }

        private void SyncCrossReferences(InventoryItem sourceItem, InventoryItem targetItem, InventoryItemMaint targetGraph)
        {
            try
            {
                PXTrace.WriteInformation("Starting Cross-References sync...");
                
                // Skip if target item doesn't have an InventoryID (new item not yet saved)
                if (targetItem.InventoryID == null || targetItem.InventoryID <= 0)
                {
                    PXTrace.WriteInformation("Skipping cross-references sync - target item not yet saved");
                    return;
                }

                List<INItemXRef> sourceCrossRefsList = new List<INItemXRef>();
                foreach (PXResult<INItemXRef> result in PXSelectReadonly<INItemXRef,
                    Where<INItemXRef.inventoryID, Equal<Required<INItemXRef.inventoryID>>>>.Select(this.Base, sourceItem.InventoryID))
                {
                    INItemXRef xref = result;
                    if (xref != null) sourceCrossRefsList.Add(xref);
                }
                PXTrace.WriteInformation($"Found {sourceCrossRefsList.Count} source cross-references");

                if (sourceCrossRefsList.Count == 0)
                {
                    PXTrace.WriteInformation("No source cross-references to sync");
                    return;
                }

                // Build composite keys of source cross-refs 
                // PK is: InventoryID, AlternateType, AlternateID, SubItemID, BAccountID (NOT UOM)
                HashSet<string> sourceCompositeKeys = new HashSet<string>();
                foreach (INItemXRef xref in sourceCrossRefsList)
                {
                    if (xref != null)
                    {
                        string key = $"{xref.AlternateID?.Trim()}|{xref.BAccountID ?? 0}|{xref.AlternateType}|{xref.SubItemID ?? 0}";
                        sourceCompositeKeys.Add(key);
                    }
                }

                // Get existing target cross-refs from database (not from view)
                List<INItemXRef> targetCrossRefsList = new List<INItemXRef>();
                foreach (PXResult<INItemXRef> result in PXSelectReadonly<INItemXRef,
                    Where<INItemXRef.inventoryID, Equal<Required<INItemXRef.inventoryID>>>>.Select(targetGraph, targetItem.InventoryID))
                {
                    INItemXRef xref = result;
                    if (xref != null) targetCrossRefsList.Add(xref);
                }
                PXTrace.WriteInformation($"Found {targetCrossRefsList.Count} existing target cross-references");

                // Build set of existing target keys
                HashSet<string> existingTargetKeys = new HashSet<string>();
                foreach (INItemXRef xref in targetCrossRefsList)
                {
                    if (xref != null)
                    {
                        string key = $"{xref.AlternateID?.Trim()}|{xref.BAccountID ?? 0}|{xref.AlternateType}|{xref.SubItemID ?? 0}";
                        existingTargetKeys.Add(key);
                    }
                }

                // Delete cross-refs that exist in target but not in source
                int deleteCount = 0;
                foreach (INItemXRef targetXRef in targetCrossRefsList)
                {
                    if (targetXRef != null)
                    {
                        string targetKey = $"{targetXRef.AlternateID?.Trim()}|{targetXRef.BAccountID ?? 0}|{targetXRef.AlternateType}|{targetXRef.SubItemID ?? 0}";
                        if (!sourceCompositeKeys.Contains(targetKey))
                        {
                            foreach (INItemXRef viewXRef in targetGraph.itemxrefrecords.Select())
                            {
                                if (viewXRef != null && 
                                    viewXRef.InventoryID == targetItem.InventoryID &&
                                    (viewXRef.AlternateID?.Trim() ?? "") == (targetXRef.AlternateID?.Trim() ?? "") &&
                                    (viewXRef.BAccountID ?? 0) == (targetXRef.BAccountID ?? 0) &&
                                    viewXRef.AlternateType == targetXRef.AlternateType &&
                                    (viewXRef.SubItemID ?? 0) == (targetXRef.SubItemID ?? 0))
                                {
                                    targetGraph.itemxrefrecords.Delete(viewXRef);
                                    deleteCount++;
                                    PXTrace.WriteInformation($"Deleted cross-ref: {targetXRef.AlternateID}");
                                    break;
                                }
                            }
                        }
                    }
                }
                if (deleteCount > 0) PXTrace.WriteInformation($"Total deleted: {deleteCount} cross-references");

                // Add or update cross-refs from source
                int insertCount = 0;
                int updateCount = 0;
                int skipCount = 0;
                foreach (INItemXRef sourceXRef in sourceCrossRefsList)
                {
                    if (sourceXRef == null) continue;
                    
                    string sourceKey = $"{sourceXRef.AlternateID?.Trim()}|{sourceXRef.BAccountID ?? 0}|{sourceXRef.AlternateType}|{sourceXRef.SubItemID ?? 0}";
                    
                    // Check if already exists in database
                    if (existingTargetKeys.Contains(sourceKey))
                    {
                        // Find and update in view
                        foreach (INItemXRef viewXRef in targetGraph.itemxrefrecords.Select())
                        {
                            if (viewXRef != null && 
                                viewXRef.InventoryID == targetItem.InventoryID &&
                                (viewXRef.AlternateID?.Trim() ?? "") == (sourceXRef.AlternateID?.Trim() ?? "") &&
                                (viewXRef.BAccountID ?? 0) == (sourceXRef.BAccountID ?? 0) &&
                                viewXRef.AlternateType == sourceXRef.AlternateType &&
                                (viewXRef.SubItemID ?? 0) == (sourceXRef.SubItemID ?? 0))
                            {
                                INItemXRef updateXRef = (INItemXRef)targetGraph.itemxrefrecords.Cache.CreateCopy(viewXRef);
                                updateXRef.Descr = sourceXRef.Descr;
                                updateXRef.UOM = sourceXRef.UOM;
                                targetGraph.itemxrefrecords.Update(updateXRef);
                                updateCount++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Insert new - but double check it doesn't exist in the pending cache
                        bool alreadyPending = false;
                        foreach (INItemXRef pendingXRef in targetGraph.itemxrefrecords.Cache.Inserted)
                        {
                            if (pendingXRef != null &&
                                (pendingXRef.AlternateID?.Trim() ?? "") == (sourceXRef.AlternateID?.Trim() ?? "") &&
                                (pendingXRef.BAccountID ?? 0) == (sourceXRef.BAccountID ?? 0) &&
                                pendingXRef.AlternateType == sourceXRef.AlternateType &&
                                (pendingXRef.SubItemID ?? 0) == (sourceXRef.SubItemID ?? 0))
                            {
                                alreadyPending = true;
                                skipCount++;
                                break;
                            }
                        }
                        
                        if (!alreadyPending)
                        {
                            INItemXRef newXRef = (INItemXRef)targetGraph.itemxrefrecords.Cache.CreateInstance();
                            newXRef.InventoryID = targetItem.InventoryID;
                            newXRef.AlternateID = sourceXRef.AlternateID;
                            newXRef.AlternateType = sourceXRef.AlternateType;
                            newXRef.BAccountID = sourceXRef.BAccountID;
                            newXRef.SubItemID = sourceXRef.SubItemID;
                            newXRef.UOM = sourceXRef.UOM;
                            newXRef.Descr = sourceXRef.Descr;
                            targetGraph.itemxrefrecords.Insert(newXRef);
                            insertCount++;
                            PXTrace.WriteInformation($"Inserting cross-ref: {sourceXRef.AlternateID}");
                        }
                    }
                }
                PXTrace.WriteInformation($"Cross-References sync completed: {insertCount} inserted, {updateCount} updated, {skipCount} skipped");
            }
            catch (Exception ex)
            {
                PXTrace.WriteError($"Error syncing cross-references: {ex.Message}");
                // Don't throw - continue with other syncs
            }
        }

        private void SyncCategories(InventoryItem sourceItem, InventoryItem targetItem, InventoryItemMaint targetGraph)
        {
            try
            {
                PXTrace.WriteInformation("Starting Categories sync...");
                
                List<INItemCategory> sourceCategoriesList = new List<INItemCategory>();
                foreach (PXResult<INItemCategory> result in PXSelectReadonly<INItemCategory,
                    Where<INItemCategory.inventoryID, Equal<Required<INItemCategory.inventoryID>>>>.Select(this.Base, sourceItem.InventoryID))
                {
                    INItemCategory cat = result;
                    if (cat != null) sourceCategoriesList.Add(cat);
                }

                List<INItemCategory> targetCategoriesList = new List<INItemCategory>();
                foreach (PXResult<INItemCategory> result in PXSelectReadonly<INItemCategory,
                    Where<INItemCategory.inventoryID, Equal<Required<INItemCategory.inventoryID>>>>.Select(targetGraph, targetItem.InventoryID))
                {
                    INItemCategory cat = result;
                    if (cat != null) targetCategoriesList.Add(cat);
                }

                // Build lists of CategoryIDs
                List<int?> sourceCategoryIDs = new List<int?>();
                foreach (INItemCategory cat in sourceCategoriesList)
                {
                    if (cat != null)
                    {
                        sourceCategoryIDs.Add(cat.CategoryID);
                    }
                }

                List<int?> targetCategoryIDs = new List<int?>();
                foreach (INItemCategory cat in targetCategoriesList)
                {
                    if (cat != null)
                    {
                        targetCategoryIDs.Add(cat.CategoryID);
                    }
                }
                PXTrace.WriteInformation($"Found {sourceCategoryIDs.Count} source categories, {targetCategoryIDs.Count} target categories");

                // Delete categories that exist in target but not in source
                int deleteCount = 0;
                foreach (INItemCategory targetCat in targetCategoriesList)
                {
                    if (targetCat != null && !sourceCategoryIDs.Contains(targetCat.CategoryID))
                    {
                        // Find the record in the actual view to delete it
                        foreach (INItemCategory viewCat in targetGraph.Category.Select())
                        {
                            if (viewCat != null && viewCat.CategoryID == targetCat.CategoryID &&
                                viewCat.InventoryID == targetItem.InventoryID)
                            {
                                targetGraph.Category.Delete(viewCat);
                                deleteCount++;
                                break;
                            }
                        }
                    }
                }
                if (deleteCount > 0) PXTrace.WriteInformation($"Deleted {deleteCount} categories");

                // Add categories from source that don't exist in target
                int insertCount = 0;
                foreach (INItemCategory sourceCat in sourceCategoriesList)
                {
                    if (sourceCat != null && !targetCategoryIDs.Contains(sourceCat.CategoryID))
                    {
                        INItemCategory newCat = (INItemCategory)targetGraph.Category.Cache.CreateInstance();
                        newCat.InventoryID = targetItem.InventoryID;
                        newCat.CategoryID = sourceCat.CategoryID;
                        targetGraph.Category.Insert(newCat);
                        insertCount++;
                    }
                }
                PXTrace.WriteInformation($"Categories sync completed: {insertCount} inserted");
            }
            catch (Exception ex)
            {
                PXTrace.WriteError($"Error syncing categories: {ex.Message}");
            }
        }
    }
}

using PX.Data;
using PX.Objects.IN;
using System;
using System.Collections.Generic;

namespace MTUInventory
{
    public static class MultiTenantSyncCache
    {
        private static readonly object _lockObject = new object();
        private static Dictionary<string, TenantSyncSettings> _cache = null;
        private static DateTime _cacheExpiration = DateTime.MinValue;
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public class TenantSyncSettings
        {
            public bool AcceptsStockItems { get; set; }
            public bool AcceptsNonStockItems { get; set; }
        }

        public static List<string> GetEnabledTenantsForStockItems(List<string> allTenants)
        {
            RefreshCacheIfNeeded(allTenants);
            
            var enabledTenants = new List<string>();
            lock (_lockObject)
            {
                if (_cache != null)
                {
                    foreach (var tenant in allTenants)
                    {
                        if (_cache.ContainsKey(tenant) && _cache[tenant].AcceptsStockItems)
                        {
                            enabledTenants.Add(tenant);
                        }
                    }
                }
            }
            
            PXTrace.WriteInformation($"MultiTenantSyncCache: Found {enabledTenants.Count} tenants enabled for stock items");
            return enabledTenants;
        }

        public static List<string> GetEnabledTenantsForNonStockItems(List<string> allTenants)
        {
            RefreshCacheIfNeeded(allTenants);
            
            var enabledTenants = new List<string>();
            lock (_lockObject)
            {
                if (_cache != null)
                {
                    foreach (var tenant in allTenants)
                    {
                        if (_cache.ContainsKey(tenant) && _cache[tenant].AcceptsNonStockItems)
                        {
                            enabledTenants.Add(tenant);
                        }
                    }
                }
            }
            
            PXTrace.WriteInformation($"MultiTenantSyncCache: Found {enabledTenants.Count} tenants enabled for non-stock items");
            return enabledTenants;
        }

        private static void RefreshCacheIfNeeded(List<string> allTenants)
        {
            lock (_lockObject)
            {
                if (_cache == null || DateTime.Now >= _cacheExpiration)
                {
                    PXTrace.WriteInformation("MultiTenantSyncCache: Refreshing cache");
                    _cache = new Dictionary<string, TenantSyncSettings>();
                    
                    foreach (var tenantName in allTenants)
                    {
                        try
                        {
                            using (PXLoginScope loginScope = new PXLoginScope($"admin@{tenantName}"))
                            {
                                PXGraph graph = PXGraph.CreateInstance<PXGraph>();
                                INSetup setup = PXSelectReadonly<INSetup>.Select(graph);
                                
                                if (setup != null)
                                {
                                    var setupExt = PXCache<INSetup>.GetExtension<QLINSetupExt>(setup);
                                    
                                    if (setupExt != null)
                                    {
                                        _cache[tenantName] = new TenantSyncSettings
                                        {
                                            AcceptsStockItems = setupExt.UsrMultiTenantStockItemReceive == true,
                                            AcceptsNonStockItems = setupExt.UsrMultiTenantNonStockItemReceive == true
                                        };
                                        
                                        PXTrace.WriteInformation($"MultiTenantSyncCache: Tenant '{tenantName}' - StockItems: {_cache[tenantName].AcceptsStockItems}, NonStockItems: {_cache[tenantName].AcceptsNonStockItems}");
                                    }
                                    else
                                    {
                                        PXTrace.WriteInformation($"MultiTenantSyncCache: QLINSetupExt not found for tenant '{tenantName}'");
                                        _cache[tenantName] = new TenantSyncSettings
                                        {
                                            AcceptsStockItems = false,
                                            AcceptsNonStockItems = false
                                        };
                                    }
                                }
                                else
                                {
                                    PXTrace.WriteInformation($"MultiTenantSyncCache: INSetup not found for tenant '{tenantName}'");
                                    _cache[tenantName] = new TenantSyncSettings
                                    {
                                        AcceptsStockItems = false,
                                        AcceptsNonStockItems = false
                                    };
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            PXTrace.WriteError($"MultiTenantSyncCache: Error checking settings for tenant '{tenantName}': {ex.Message}");
                            _cache[tenantName] = new TenantSyncSettings
                            {
                                AcceptsStockItems = false,
                                AcceptsNonStockItems = false
                            };
                        }
                    }
                    
                    _cacheExpiration = DateTime.Now.Add(_cacheDuration);
                    PXTrace.WriteInformation($"MultiTenantSyncCache: Cache refreshed. Expires at {_cacheExpiration}");
                }
            }
        }

        public static void InvalidateCache()
        {
            lock (_lockObject)
            {
                _cache = null;
                _cacheExpiration = DateTime.MinValue;
                PXTrace.WriteInformation("MultiTenantSyncCache: Cache invalidated");
            }
        }
    }
}

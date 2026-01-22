using System;
using System.Collections.Generic;
using System.Text;

namespace MTUInventory
{
    public enum FieldSyncStatus
    {
        Success,            // Field synced successfully
        SkippedNotFound,    // Lookup failed, CD not found in target
        SkippedNull,        // Source value was null
        SkippedError        // Exception during assignment
    }

    public class FieldSyncResult
    {
        public string FieldName { get; set; }
        public FieldSyncStatus Status { get; set; }
        public string SourceValue { get; set; }
        public string WarningMessage { get; set; }

        public FieldSyncResult(string fieldName, FieldSyncStatus status, string sourceValue, string warningMessage)
        {
            FieldName = fieldName;
            Status = status;
            SourceValue = sourceValue;
            WarningMessage = warningMessage;
        }
    }

    public class SyncResult
    {
        public string TenantName { get; set; }
        public bool Success { get; set; }
        public string InventoryCD { get; set; }
        public List<FieldSyncResult> FieldResults { get; set; }
        public string ErrorMessage { get; set; }

        public int SuccessfulFieldsCount
        {
            get { return FieldResults.FindAll(f => f.Status == FieldSyncStatus.Success).Count; }
        }

        public int SkippedFieldsCount
        {
            get { return FieldResults.FindAll(f => f.Status != FieldSyncStatus.Success).Count; }
        }

        public SyncResult(string tenantName, string inventoryCD)
        {
            TenantName = tenantName;
            InventoryCD = inventoryCD;
            FieldResults = new List<FieldSyncResult>();
        }

        public string GetSummary()
        {
            StringBuilder summary = new StringBuilder();
            summary.AppendFormat("Synced {0} fields successfully", SuccessfulFieldsCount);

            if (SkippedFieldsCount > 0)
            {
                summary.AppendFormat(", {0} fields skipped (", SkippedFieldsCount);
                
                List<string> skippedDetails = new List<string>();
                foreach (var field in FieldResults)
                {
                    if (field.Status != FieldSyncStatus.Success)
                    {
                        string reason = field.Status == FieldSyncStatus.SkippedNotFound ? "not found" :
                                       field.Status == FieldSyncStatus.SkippedNull ? "null" :
                                       "error";
                        skippedDetails.Add(string.Format("{0}: {1}", field.FieldName, reason));
                    }
                }
                summary.Append(string.Join(", ", skippedDetails));
                summary.Append(")");
            }

            return summary.ToString();
        }
    }
}

# NTE Validation - Configuration & Disable Guide

**Feature:** Drop-Ship Purchase Order Not-To-Exceed Validation  
**Project:** CompiledVersion  
**Last Updated:** December 20, 2025

---

## Table of Contents

1. [Quick Disable Options](#quick-disable-options)
2. [Configuration Modes](#configuration-modes)
3. [Selective Disabling](#selective-disabling)
4. [Complete Code Removal](#complete-code-removal)
5. [Troubleshooting](#troubleshooting)

---

## Quick Disable Options

### Option 1: Disable via UI (Recommended)

**No code changes required - Immediate effect**

1. Navigate to: **Sales Orders → Preferences → SO Preferences (SO201000)**
2. Scroll to **Purchase Order Settings** section
3. **Uncheck** the checkbox: `Enforce PO Not-To-Exceed Limit`
4. Click **Save**

**Result:** NTE validation will not run at all. All POs will save normally regardless of limits.

---

### Option 2: Disable for Specific Shipping Terms

**Change which Ship Terms trigger validation**

1. Navigate to: **Sales Orders → Preferences → SO Preferences (SO201000)**
2. Find field: `Not to Exceed` (top section)
3. Change the value to a different Ship Terms ID that you never use
4. Click **Save**

**Result:** Only SOs with the new Ship Terms value will trigger validation.

---

### Option 3: Set All Orders to Warning Mode

**Allow saves but show informational warnings**

1. Navigate to: **Sales Orders → Preferences → SO Preferences (SO201000)**
2. Scroll to **Purchase Order Settings** section
3. **Uncheck** the checkbox: `Enforce PO Not-To-Exceed Limit`
4. Click **Save**

**Result:** Validation still runs but only shows warnings (orange), never blocks saves.

---

## Configuration Modes

### Mode Comparison Table

| Mode | Toggle Setting | Behavior | Use Case |
|------|---------------|----------|----------|
| **Disabled** | Checkbox unchecked | No validation, feature inactive | Testing, emergencies, temporary bypass |
| **Warning** | Checkbox unchecked | Orange warning, allows save | Soft enforcement, trust-based |
| **Hard Stop** | Checkbox checked | Red error, blocks save | Strict compliance, contract enforcement |

### Checking Current Mode

**Via UI:**
```
Sales Orders → Preferences → SO Preferences (SO201000)
→ Purchase Order Settings section
→ "Enforce PO Not-To-Exceed Limit" checkbox
```

**Via Database:**
```sql
SELECT 
    CompanyID,
    UsrEnforcePONTE AS [Is Hard Stop Enabled],
    UsrNotToExceed AS [NTE Ship Terms Code]
FROM SOSetup
```

- `UsrEnforcePONTE = 0` or `NULL` → Warning mode or disabled
- `UsrEnforcePONTE = 1` → Hard stop mode

---

## Selective Disabling

### Disable for Specific Sales Orders

**Method 1: Change Ship Terms**
- Open the Sales Order
- Change `Ship Terms` to anything other than the configured NTE value
- Validation will not apply to this SO

**Method 2: Clear Freight Limit**
- Open the Sales Order
- Set `Freight Limit` = 0 or blank
- Validation will skip this SO (no limit = no enforcement)

### Disable for Specific POs

**Workaround:** Change the linked SO's Ship Terms or Freight Limit before saving the PO.

---

## Complete Code Removal

### If You Want to Remove the Feature Entirely

⚠️ **Warning:** This requires code modification and recompilation.

#### Step 1: Comment Out Validation Event

**File:** `POOrderEntry_Extension.cs`  
**Location:** Lines ~1235-1290

```csharp
// Comment out or delete this entire block:

/*
/// <summary>
/// Validates that drop-ship PO totals do not exceed the Not-To-Exceed limit.
/// </summary>
protected virtual void _(Events.RowPersisting<POOrder> e)
{
    if (e.Row == null) return;
    if (e.Operation == PXDBOperation.Delete) return;
    
    // ... entire validation code ...
}
*/
```

#### Step 2: Comment Out Helper Methods (Optional)

**File:** `POOrderEntry_Extension.cs`  
**Location:** Lines ~1295-1360

```csharp
// Comment out GetLinkedSOForNTEValidation and CalculateTotalPOAmountForSO

/*
private SOOrder GetLinkedSOForNTEValidation(POOrder poOrder)
{
    // ... code ...
}

private decimal? CalculateTotalPOAmountForSO(SOOrder soOrder, POOrder currentPO)
{
    // ... code ...
}
*/
```

#### Step 3: Remove Toggle Field (Optional)

**File:** `SOSetupExt.cs`  
**Location:** Lines 73-78

```csharp
// Delete or comment out:

/*
#region UsrEnforcePONTE
[PXDBBool]
[PXUIField(DisplayName = "Enforce PO Not-To-Exceed Limit")]
[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
public bool? UsrEnforcePONTE { get; set; }
public abstract class usrEnforcePONTE : PX.Data.BQL.BqlBool.Field<usrEnforcePONTE> { }
#endregion
*/
```

#### Step 4: Remove Messages (Optional)

**File:** `Messages.cs`  
**Location:** Lines ~26-28

```csharp
// Delete or comment out:

/*
private const string _poTotalExceedsNTE = "Total PO amount for Sales Order {0} ({1}) exceeds Not-To-Exceed limit ({2}) by {3}.";
public static string POTotalExceedsNTE(string soOrderNbr, decimal? totalPO, decimal? nteLimit, decimal? exceedAmt) =>
    PXLocalizer.LocalizeFormat(_poTotalExceedsNTE, soOrderNbr, totalPO, nteLimit, exceedAmt);
*/
```

#### Step 5: Rebuild and Publish

1. Save all files
2. Build the customization project
3. Publish to Acumatica instance
4. Clear browser cache

#### Step 6: Database Cleanup (Optional)

If you removed the toggle field, optionally remove the database column:

```sql
-- OPTIONAL: Remove database column if field was deleted
USE [YourAcumaticaDB]
GO

IF EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'dbo.SOSetup') 
    AND name = 'UsrEnforcePONTE'
)
BEGIN
    ALTER TABLE dbo.SOSetup
    DROP COLUMN UsrEnforcePONTE
    
    PRINT 'UsrEnforcePONTE column removed.'
END
GO
```

---

## Troubleshooting

### Issue: Validation Still Running After Unchecking Toggle

**Solution:**
1. Verify you saved the SO Preferences screen
2. Clear browser cache (Ctrl+F5)
3. Log out and log back in
4. Check database value: `SELECT UsrEnforcePONTE FROM SOSetup`

### Issue: Getting Warnings Even Though Feature is Disabled

**Possible Causes:**
1. The toggle checkbox might still be checked for warning mode
2. Check if you're using the correct company (multi-company setup)
3. Verify Ship Terms match on SO

**Solution:**
```sql
-- Force disable for all companies
UPDATE SOSetup
SET UsrEnforcePONTE = 0
```

### Issue: Cannot Save PO Even in Warning Mode

**Cause:** Feature is in Hard Stop mode

**Solution:**
1. Uncheck "Enforce PO Not-To-Exceed Limit" in SO Preferences
2. Or increase the `Freight Limit` on the Sales Order
3. Or temporarily change SO Ship Terms to non-NTE value

### Issue: Feature Not Working At All

**Checklist:**
1. ✅ Ship Terms on SO matches `UsrNotToExceed` setup value?
2. ✅ Freight Limit on SO is > 0?
3. ✅ PO is actually a drop-ship PO (linked via DropShipLink)?
4. ✅ Code was compiled and published?
5. ✅ Database column `UsrEnforcePONTE` exists in SOSetup table?

---

## Emergency Disable Procedure

**If validation is blocking critical POs and you need immediate bypass:**

### Quick Fix (1 minute)

1. Open SQL Server Management Studio
2. Run this query:

```sql
-- EMERGENCY: Disable NTE validation company-wide
UPDATE SOSetup
SET UsrEnforcePONTE = 0
WHERE CompanyID = 2  -- Replace with your CompanyID
```

3. Clear browser cache (Ctrl+F5)
4. Retry PO save

### Alternative (UI Method - 2 minutes)

1. Navigate to SO Preferences (SO201000)
2. Uncheck "Enforce PO Not-To-Exceed Limit"
3. Save
4. Clear browser cache
5. Retry PO save

---

## Feature Status Reference

### Files Containing NTE Validation Code

| File | Path | Lines | Purpose |
|------|------|-------|---------|
| **SOSetupExt.cs** | DACExt/ | 73-78 | Toggle field definition |
| **Messages.cs** | Messages/ | 26-28 | Error messages |
| **POOrderEntry_Extension.cs** | GraphExt/ | 1235-1360 | Validation logic |

### Database Objects

| Object | Type | Purpose |
|--------|------|---------|
| `SOSetup.UsrEnforcePONTE` | Column (bit) | Stores toggle setting |

### UI Locations

| Screen | Screen ID | Field/Setting |
|--------|-----------|---------------|
| SO Preferences | SO201000 | "Enforce PO Not-To-Exceed Limit" checkbox |
| SO Preferences | SO201000 | "Not to Exceed" Ship Terms selector |
| Sales Orders | SO301000 | "Freight Limit" field (visible when Ship Terms = NTE) |
| Purchase Orders | PO301000 | Validation triggers on save |

---

## Restore After Disabling

### To Re-enable the Feature

1. Navigate to SO Preferences (SO201000)
2. Check "Enforce PO Not-To-Exceed Limit" (for hard stop mode)
3. Or leave unchecked (for warning mode)
4. Ensure "Not to Exceed" Ship Terms field is set correctly
5. Save

**Or via SQL:**

```sql
-- Re-enable Hard Stop mode
UPDATE SOSetup
SET UsrEnforcePONTE = 1
WHERE CompanyID = 2

-- Enable Warning mode
UPDATE SOSetup
SET UsrEnforcePONTE = 0
WHERE CompanyID = 2
```

---

## Support Contact

For questions about this feature or disabling procedures:

- **Technical Issues:** Contact Development Team
- **Configuration Questions:** Contact System Administrator
- **Business Logic Changes:** Contact Project Manager

---

## Change Log

| Date | Version | Change | Author |
|------|---------|--------|--------|
| 2025-12-20 | 1.0 | Initial documentation | Development Team |

---

**End of Document**

# Migration Guide: Apply Changes from December 29, 2025

**Date:** December 29, 2025  
**Changeset Reference:** 2afdfc2ee3d38e7ba69bbf793a4fe4a3c01a5b70  
**Author:** Biloy

---

## ⚠️ IMPORTANT REMINDERS

### Scope
This migration guide covers changes in the following folders within the CompiledVersion customization project:
- **GraphExt** - Graph Extensions
- **Graphs** - Business Logic Graphs

**Base Path:** `App_Data/Projects/CompiledVersion/CompiledVersion/`

### Customization Package Updates
After implementing these changes, you **MUST** update your customization packages:

1. **Publish Customization Project** - After making code changes in the Customization Project editor
2. **Test in Development Environment** - Thoroughly test all changes before promoting
3. **Export Updated Package** - Create a new customization package export
4. **Version Control** - Tag this as a new version if using source control

---

## 📋 Table of Contents

1. [New Graph Extensions](#1-new-graph-extensions)
2. [GraphExt Changes](#2-graphext-changes)
3. [Graph Changes](#3-graph-changes)
4. [Project File Updates](#4-project-file-updates)
5. [Testing Checklist](#5-testing-checklist)

---

## 1. New Graph Extensions

### 1.1 SOOrderEntry_CreatePaymentExt.cs (NEW)
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/GraphExt/SOOrderEntry_CreatePaymentExt.cs`

**Purpose:** Extends the CreatePaymentExt functionality in SOOrderEntry to use RTH Order Total instead of standard order total when creating payments.

**Action:** Create new file with the following content:

```csharp
using CompiledVersion.DAC;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.SO;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;

namespace CompiledVersion.Graphs
{
    public class SOOrderEntry_CreatePaymentExt : PXGraphExtension<CreatePaymentExt, PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() => true;

        #region Event Handlers - Override to use RTH Order Total

        protected virtual void _(Events.FieldDefaulting<SOQuickPayment, SOQuickPayment.curyOrigDocAmt> e, PXFieldDefaulting baseHandler)
        {
            // Call base handler first
            baseHandler?.Invoke(e.Cache, e.Args);

            // Override with RTH Order Total
            decimal? rthAmount = GetRTHDefaultPaymentAmount(e.Row as SOQuickPayment);
            if (rthAmount != null)
            {
                e.NewValue = rthAmount;
            }
        }

        protected virtual void _(Events.FieldDefaulting<SOQuickPayment, SOQuickPayment.curyRefundAmt> e, PXFieldDefaulting baseHandler)
        {
            // Call base handler first
            baseHandler?.Invoke(e.Cache, e.Args);

            // Override with RTH Order Total
            decimal? rthAmount = GetRTHDefaultPaymentAmount(e.Row as SOQuickPayment);
            if (rthAmount != null)
            {
                e.NewValue = rthAmount;
            }
        }

        protected virtual decimal? GetRTHDefaultPaymentAmount(SOQuickPayment qp)
        {
            if (qp == null || Base.Document.Current == null)
                return null;

            SOOrder document = Base.Document.Current;
            SOOrderExt documentExt = document.GetExtension<SOOrderExt>();

            // Use RTH Order Total instead of standard order total
            decimal? rthOrderTotal = documentExt?.UsrRTHCuryOrderTotal;
            if (rthOrderTotal == null || rthOrderTotal <= 0)
            {
                // Return null to use base calculation
                return null;
            }

            decimal? amt = null;

            if (qp.CuryID == document.CuryID)
            {
                // Calculate unpaid balance using RTH Order Total
                decimal? paidAmount = document.CuryPaymentTotal ?? 0m;
                amt = rthOrderTotal - paidAmount;
            }
            else if (qp.CuryID != null)
            {
                // Convert the unpaid balance based on RTH Order Total
                decimal? paidAmount = document.PaymentTotal ?? 0m;
                decimal baseUnpaidBalance = (rthOrderTotal ?? 0m) - (paidAmount ?? 0m);
                PXCurrencyAttribute.CuryConvCury(Base1.QuickPayment.Cache, qp, baseUnpaidBalance, out decimal curyUnpaidBalance);
                amt = curyUnpaidBalance;
            }

            return amt;
        }

        #endregion
    }
}
```

**Key Features:**
- Extends `CreatePaymentExt` graph extension
- Overrides payment amount defaults to use `UsrRTHCuryOrderTotal` from `SOOrderExt`
- Handles currency conversion when payment currency differs from order currency
- Falls back to base calculation when RTH Order Total is not available

---

## 2. GraphExt Changes

### 2.1 SOOrderEntry_Extension.cs - Enhanced Vendor Field Population
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/GraphExt/SOOrderEntry_Extension.cs`

**Changes:** Added automatic vendor field population when inventory item is selected on sales order line.

**Location:** In the `FieldUpdated<SOLine, SOLine.inventoryID>` event handler

**Action:** Add the following code after the null check and before `TryRecalculateUnitCost`:

```csharp
// Copy default vendor to UsrVendorID
var lineExt = e.Row.GetExtension<SOLineExt>();
POVendorInventory defaultVendor = null;
foreach (POVendorInventory defVen in PXSelect<POVendorInventory,
    Where<POVendorInventory.inventoryID, Equal<Required<POVendorInventory.inventoryID>>>>
    .Select(Base, e.Row.InventoryID))
{
    if (defVen.IsDefault == true)
    {
        defaultVendor = defVen;
        break;
    }
}

if (defaultVendor != null)
{
    e.Cache.SetValueExt<SOLineExt.usrVendorID>(e.Row, defaultVendor.VendorID);
    e.Cache.SetValueExt<SOLineExt.usrVendorLocationID>(e.Row, defaultVendor.VendorLocationID);

    Address address = PXSelect<Address,
        Where<Address.bAccountID, Equal<Required<Address.bAccountID>>>>
        .Select(Base, defaultVendor.VendorID);

    if (address != null)
    {
        lineExt.UsrVendorAddress = string.Format("{0}{1}{2}, {3} {4}",
            address.AddressLine1 ?? "",
            string.IsNullOrWhiteSpace(address.AddressLine2) ? "" : " " + address.AddressLine2,
            string.IsNullOrWhiteSpace(address.City) ? "" : ", " + address.City,
            address.State ?? "",
            address.PostalCode ?? ""
        ).Trim();
    }
    else
    {
        lineExt.UsrVendorAddress = null;
    }
}
```

**Impact:**
- When an inventory item is selected on a sales order line, the default vendor information is automatically populated
- Includes vendor ID, location ID, and formatted address
- Vendor address is constructed from the Address record with proper formatting

---

## 3. Graph Changes

### 3.1 ItemRequestEntry.cs - File Attachment Copy Functionality
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/Graphs/ItemRequestEntry.cs`

**Changes:** Added functionality to copy file attachments from Item Request to newly created Inventory Items.

**Action:** 

1. Add the `System` namespace import at the top:
```csharp
using System;
```

2. In the item creation logic (after the note text section), add the file copy section:

```csharp
#region Copy Files
// Copy files/attachments from Item Request to Inventory Item
Guid[] files = PXNoteAttribute.GetFileNotes(Caches[typeof(InventoryRequest)], request);
if (files != null && files.Length > 0)
{
    PXNoteAttribute.SetFileNotes(itemGraph.Item.Cache, item, files);
}
#endregion
```

**Location:** After the note copy section and before `itemGraph.Save.Press()`

**Impact:**
- When creating an Inventory Item from an Item Request, all attached files are now copied to the new item
- Uses PXNoteAttribute to retrieve and copy file GUIDs
- Ensures file attachments are preserved during the conversion process

---

## 4. Project File Updates

### 4.1 Add SOOrderEntry_CreatePaymentExt.cs to Project Files

**Files to Update:**
- `App_Data/Projects/CompiledVersion/CompiledVersion/CompiledVersion24R1.csproj`
- `App_Data/Projects/CompiledVersion/CompiledVersion/CompiledVersion25R2.csproj`

**Action:** Add the following line after `SOOrderEntryCopyOppFieldsExt.cs`:

```xml
<Compile Include="GraphExt\SOOrderEntry_CreatePaymentExt.cs" />
```

**Note:** This is typically handled automatically by the Customization Project Editor, but verify the file is included in your project references.

---

## 5. Testing Checklist

### 5.1 Payment Creation with RTH Order Total
- [ ] Create a sales order with RTH Order Total configured
- [ ] Verify that "Create Payment" uses RTH Order Total instead of standard order total
- [ ] Test with multi-currency scenarios
- [ ] Verify refund amount calculation
- [ ] Test with partial payments already applied

### 5.2 Vendor Field Auto-Population
- [ ] Create a new sales order
- [ ] Add a line item with a default vendor configured
- [ ] Verify vendor ID, vendor location, and vendor address are automatically populated
- [ ] Test with items that have no default vendor
- [ ] Test with items that have vendor address missing

### 5.3 File Attachment Copy
- [ ] Create an Item Request with file attachments
- [ ] Convert the Item Request to an Inventory Item
- [ ] Verify all attached files are copied to the new Inventory Item
- [ ] Test with multiple file types (images, PDFs, documents)
- [ ] Verify no files are duplicated or lost

### 5.4 General Testing
- [ ] Verify existing SO functionality remains intact
- [ ] Test order creation, modification, and release processes
- [ ] Verify performance is not impacted
- [ ] Check for any console errors or exceptions in the trace log

---

## 📌 Notes

### Dependencies
This migration requires the following custom fields to exist:
- `SOOrderExt.UsrRTHCuryOrderTotal` - RTH Order Total field
- `SOLineExt.UsrVendorID` - Vendor ID field on SO Line
- `SOLineExt.UsrVendorLocationID` - Vendor Location ID field on SO Line  
- `SOLineExt.UsrVendorAddress` - Vendor Address field on SO Line

### Related Functionality
- The RTH Order Total calculation logic should be implemented in SOOrderExt
- Vendor information setup should be maintained in POVendorInventory records
- File attachment functionality relies on PXNoteAttribute infrastructure

### Best Practices
- Test all payment scenarios with varying order totals
- Verify vendor data accuracy before relying on auto-population
- Monitor file attachment sizes to avoid performance issues
- Review trace logs after deployment for any warnings or errors

---

## ✅ Sign-off

**Developer:** _________________  
**Code Reviewer:** _________________  
**QA Tester:** _________________  
**Date Completed:** _________________

---

*This migration guide was generated for changeset 2afdfc2ee3d38e7ba69bbf793a4fe4a3c01a5b70*

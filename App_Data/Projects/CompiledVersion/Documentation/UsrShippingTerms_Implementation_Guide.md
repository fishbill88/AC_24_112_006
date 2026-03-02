# UsrShippingTerms Field Implementation Guide

## Overview
The `UsrShippingTerms` custom field captures special shipping terms at the Sales Order line level and propagates them through to Purchase Order lines during the SO-to-PO creation process. This field provides visibility into non-standard shipping requirements that originate from the sales order.

---

## 1. Field Definition - SOLineExt.cs

### Location
**File**: `App_Data\Projects\CompiledVersion\CompiledVersion\DACExt\SOLineExt.cs`
**DAC Extension**: `SOLineExt` (extends `PX.Objects.SO.SOLine`)

### Field Implementation

```csharp
#region UsrShippingTerms
[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
[PXUIField(DisplayName = "Shipping Terms")]
[PXSelector(typeof(ShipTerms.shipTermsID), 
    DescriptionField = typeof(ShipTerms.description), 
    CacheGlobal = true)]
[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
public string UsrShippingTerms { get; set; }
public abstract class usrShippingTerms : PX.Data.BQL.BqlString.Field<usrShippingTerms> { }
#endregion
```

### Field Characteristics
- **Database Type**: String field, 10 characters maximum
- **Unicode Support**: Enabled
- **Input Mask**: `">aaaaaaaaaa"` - Forces uppercase, alphabetic characters only
- **Selector**: Linked to standard Acumatica `ShipTerms` table
- **Display Name**: "Shipping Terms"
- **Default Value**: None (optional field)
- **Editable**: Yes (on Sales Order lines)

### Purpose
Allows users to specify special shipping terms at the individual SO line level, which may differ from the header-level shipping information.

---

## 2. Field Definition - POLineExt.cs

### Location
**File**: `App_Data\Projects\CompiledVersion\CompiledVersion\DACExt\POLineExt.cs`
**DAC Extension**: `POLineExt` (extends `PX.Objects.PO.POLine`)

### Field Implementation

```csharp
#region UsrShippingTerms
[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
[PXUIField(DisplayName = "Shipping Terms")]
[PXSelector(typeof(ShipTerms.shipTermsID), 
    DescriptionField = typeof(ShipTerms.description), 
    CacheGlobal = true)]
[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
public string UsrShippingTerms { get; set; }
public abstract class usrShippingTerms : PX.Data.BQL.BqlString.Field<usrShippingTerms> { }
#endregion
```

### Field Characteristics
- **Identical structure** to SOLineExt version
- **Display-only** on PO lines (controlled by graph logic, not attributes)
- Serves as a reference/notification field on the purchase order

---

## 3. SO-to-PO Field Copy Logic

### Location
**File**: `App_Data\Projects\CompiledVersion\CompiledVersion\GraphExt\POCreateReplaceOriginalMethod.cs`
**Graph Extension**: PO Create functionality

### Functionality: Copy Shipping Terms from SO to PO

**Trigger**: During Purchase Order creation from Sales Order (Drop Ship or standard SO-to-PO)

**Business Rule**: When multiple SO lines are merged into a single PO line, use the **first non-empty** shipping terms value.

### Implementation Pattern

```csharp
// Get extension objects
SOLineExt soLineExt = soline.GetExtension<SOLineExt>();
POLineExt poLineExt = POline.GetExtension<POLineExt>();

// Copy Shipping Terms - only use the first one when merging multiple SO lines
if (string.IsNullOrWhiteSpace(poLineExt.UsrShippingTerms))
{
    poLineExt.UsrShippingTerms = soLineExt?.UsrShippingTerms;
}
```

### Key Points
- **Null/Empty Check**: Only populate if PO line field is currently empty
- **First-Wins Logic**: Prevents overwriting when processing multiple SO lines
- **Safe Navigation**: Uses null-conditional operator `?.` to avoid null reference exceptions
- **Placement**: Should be in the same area where other SO line fields (Vendor Special Terms, Vendor Notes) are copied to PO lines

### Where to Find This Logic
Look for the method that handles SO line to PO line mapping, typically:
- In a `POCreate` or `POCreateSalesOrder` graph extension
- Near other field copy operations (e.g., vendor notes, special terms)
- Within a loop that processes SO lines and creates/updates PO lines

---

## 4. PO Line Display and Warning Logic

### Location
**File**: `App_Data\Projects\CompiledVersion\CompiledVersion\GraphExt\POOrderEntry_Extension.cs`
**Graph Extension**: `POOrderEntry_Extension` (extends `PX.Objects.PO.POOrderEntry`)

### Functionality A: Make Field Read-Only

**Event**: `RowSelected<POLine>`

```csharp
protected virtual void _(Events.RowSelected<POLine> e, PXRowSelected baseMethod)
{
    baseMethod?.Invoke(e.Cache, e.Args);
    POLine line = (POLine)e.Row;
    if (line == null) return;

    // Disable editing of shipping terms on PO lines
    PXUIFieldAttribute.SetEnabled<POLineExt.usrShippingTerms>(e.Cache, line, false);
    
    // ... rest of method
}
```

**Purpose**: Prevents users from modifying the shipping terms on the PO, as this value should reflect what was specified on the originating SO line.

### Functionality B: Display Visual Warning Indicator

**Event**: Same `RowSelected<POLine>` event

```csharp
protected virtual void _(Events.RowSelected<POLine> e, PXRowSelected baseMethod)
{
    baseMethod?.Invoke(e.Cache, e.Args);
    POLine line = (POLine)e.Row;
    if (line == null) return;

    PXUIFieldAttribute.SetEnabled<POLineExt.usrShippingTerms>(e.Cache, line, false);

    PXException warningShippingTerms = null;
    
    POLineExt lineExt = line.GetExtension<POLineExt>();
    
    // Create warning if field has a value
    if (lineExt.UsrShippingTerms != null)
    {
        warningShippingTerms = new PXSetPropertyException(line, Messages.Space, PXErrorLevel.Warning);
    }
    
    // Raise the warning on the field
    e.Cache.RaiseExceptionHandling<POLineExt.usrShippingTerms>(
        e.Row, 
        lineExt.UsrShippingTerms, 
        warningShippingTerms);
}
```

**Purpose**: 
- Displays a **warning indicator** (typically orange/yellow icon) on the PO line when shipping terms are present
- Alerts the purchaser that special shipping arrangements were requested on the SO
- Does not block saving or processing - informational only

### Implementation Notes
- **Warning Level**: Uses `PXErrorLevel.Warning` (not Error)
- **Visual Indicator**: Shows in the UI but doesn't prevent record operations
- **Message**: Uses `Messages.Space` - displays just an icon/indicator without verbose text
- **Grouped Logic**: Should be implemented alongside similar warning logic for vendor special terms and vendor notes

### Pattern for Similar Fields
This same pattern is used for:
- `UsrVendorSpecTerms` 
- `UsrVendorNotes`

All three fields follow the same logic:
1. Check if field has a value
2. Create a warning-level exception
3. Raise exception handling to display the indicator

---

## 5. Related Fields (Context)

The `UsrShippingTerms` field is part of a group of SO-to-PO informational fields:

| Field Name | Display Name | Purpose |
|------------|--------------|---------|
| `UsrVendorSpecTerms` | Vendor Special Terms | Vendor-specific contract terms from SO |
| `UsrVendorNotes` | Vendor Notes | Additional notes for the vendor from SO |
| `UsrShippingTerms` | Shipping Terms | Special shipping requirements from SO |

All three fields:
- Are defined on both `SOLineExt` and `POLineExt`
- Copy from SO to PO during creation
- Display as read-only with warning indicators on PO
- Use first-wins logic when merging multiple SO lines

---

## 6. Database Schema

### Tables Modified
- **SOLine** - Add custom field `UsrShippingTerms`
- **POLine** - Add custom field `UsrShippingTerms`

### SQL Schema (for reference)
```sql
-- SOLine Extension
ALTER TABLE SOLine ADD UsrShippingTerms NVARCHAR(10) NULL;

-- POLine Extension
ALTER TABLE POLine ADD UsrShippingTerms NVARCHAR(10) NULL;
```

---

## 7. Testing Checklist

### Scenario 1: Single SO Line to Single PO Line
1. Create Sales Order with one line
2. Set `UsrShippingTerms` to a value (e.g., "FOB")
3. Create Purchase Order from SO line
4. **Expected**: PO line shows shipping terms value, read-only, with warning icon

### Scenario 2: Multiple SO Lines Merged to One PO Line
1. Create Sales Order with multiple lines for same item/vendor
2. Set `UsrShippingTerms` on first line to "FOB"
3. Set `UsrShippingTerms` on second line to "CIF"
4. Create Purchase Order (lines merge)
5. **Expected**: PO line shows "FOB" (first non-empty value)

### Scenario 3: SO Line Without Shipping Terms
1. Create Sales Order with line
2. Leave `UsrShippingTerms` empty
3. Create Purchase Order from SO line
4. **Expected**: PO line has empty shipping terms, no warning icon

### Scenario 4: Direct PO Entry (No SO)
1. Create Purchase Order directly (not from SO)
2. **Expected**: `UsrShippingTerms` field is read-only and empty, no warning

---

## 8. Implementation Steps for New Version

### Step 1: Add DAC Extensions
1. Locate or create `SOLineExt.cs`
2. Add the `UsrShippingTerms` field region with all attributes
3. Locate or create `POLineExt.cs`
4. Add the identical `UsrShippingTerms` field region

### Step 2: Implement SO-to-PO Copy Logic
1. Find the graph extension that handles PO creation from SO
   - Search for: `POCreate`, `CreatePOOrders`, or similar methods
   - Look for where `SOLine` is being processed to create `POLine`
2. Locate where other extension fields are copied (VendorSpecTerms, VendorNotes)
3. Add the shipping terms copy logic using first-wins pattern
4. Place it in the same section as related field copies

### Step 3: Implement PO Display Logic
1. Find or create `POOrderEntry` graph extension
2. Locate or create `RowSelected<POLine>` event handler
3. Add logic to set field as disabled
4. Add logic to display warning when field has value
5. Follow the same pattern as VendorSpecTerms and VendorNotes warnings

### Step 4: Publish and Test
1. Publish customization project
2. Verify database schema updates
3. Test all scenarios in testing checklist
4. Verify warning indicators display correctly

---

## 9. UI Placement Recommendations

### Sales Order Screen (SO301000)
- **Tab**: Details (line-level tab)
- **Section**: Typically in a "Vendor Information" or "Special Instructions" section
- **Position**: Near related fields (Vendor Special Terms, Vendor Notes)

### Purchase Order Screen (PO301000)
- **Tab**: Document Details (line-level tab)
- **Section**: Same grouping as Sales Order
- **Display**: Read-only with visual warning indicator when populated

---

## 10. Maintenance Notes

### Future Considerations
- If standard Acumatica adds line-level shipping terms, evaluate migration path
- Consider expanding field length if longer shipping term codes are needed
- Consider adding validation against ShipTerms master table at save time

### Related Customizations
This field is part of a larger SO-to-PO enhancement that includes:
- Vendor selection at SO line level
- Vendor location at SO line level
- Vendor address display
- Special terms and notes
- Item specifications

### Code Dependencies
- Requires `ShipTerms` table/DAC (standard Acumatica)
- Links to `PX.Objects.CS.ShipTerms`
- Should work with standard drop ship and SO-to-PO workflows

---

## 11. Troubleshooting

### Issue: Field Not Copying from SO to PO
**Check**:
- Copy logic is in the correct graph extension
- Graph extension is active (`IsActive()` returns true)
- Copy logic executes during the SO-to-PO process
- No errors in trace for the copy method

### Issue: Warning Not Displaying on PO
**Check**:
- `RowSelected` event handler is implemented
- Warning exception is being created when field has value
- `RaiseExceptionHandling` is called correctly
- Field is visible in the screen layout

### Issue: Field Is Editable on PO
**Check**:
- `SetEnabled` is called with `false` in `RowSelected`
- Graph extension load order (ensure it runs after base)
- No other extension is setting it to enabled

---

## Document Version
- **Created**: 2024
- **Last Updated**: Current implementation
- **Compatible With**: Acumatica 2024 R1+ (adjust for specific version)
- **Customization Project**: CompiledVersion

---

## Quick Reference Code Snippets

### Full DAC Field Definition
```csharp
#region UsrShippingTerms
[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
[PXUIField(DisplayName = "Shipping Terms")]
[PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
public string UsrShippingTerms { get; set; }
public abstract class usrShippingTerms : PX.Data.BQL.BqlString.Field<usrShippingTerms> { }
#endregion
```

### SO-to-PO Copy Pattern
```csharp
SOLineExt soLineExt = soline.GetExtension<SOLineExt>();
POLineExt poLineExt = POline.GetExtension<POLineExt>();

if (string.IsNullOrWhiteSpace(poLineExt.UsrShippingTerms))
{
    poLineExt.UsrShippingTerms = soLineExt?.UsrShippingTerms;
}
```

### PO Warning Display Pattern
```csharp
protected virtual void _(Events.RowSelected<POLine> e, PXRowSelected baseMethod)
{
    baseMethod?.Invoke(e.Cache, e.Args);
    POLine line = e.Row;
    if (line == null) return;

    PXUIFieldAttribute.SetEnabled<POLineExt.usrShippingTerms>(e.Cache, line, false);

    POLineExt lineExt = line.GetExtension<POLineExt>();
    PXException warningShippingTerms = null;
    
    if (lineExt.UsrShippingTerms != null)
    {
        warningShippingTerms = new PXSetPropertyException(line, Messages.Space, PXErrorLevel.Warning);
    }
    
    e.Cache.RaiseExceptionHandling<POLineExt.usrShippingTerms>(e.Row, lineExt.UsrShippingTerms, warningShippingTerms);
}
```

---

## End of Document

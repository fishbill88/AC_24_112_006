# Drop-Ship Header Fields: SO-to-PO Copy Implementation Guide

## Overview

When a Purchase Order is created from a Sales Order via the Drop-Ship workflow, four **header-level** shipping fields must be copied from the SO to the PO. This guide documents the exact behavior of the reference implementation so it can be replicated in a newer Acumatica version.

> **Scope**: This guide covers **header-level** fields only. For line-level fields (e.g., `UsrShippingTerms` on `POLine`), see `UsrShippingTerms_Implementation_Guide.md`.

---

## Field Summary

| # | Field | Source (SO) | Target (PO) | Standard/Custom | Drop-Ship Only? |
|---|-------|-------------|-------------|-----------------|-----------------|
| 1 | **FOB Point** | `SOOrder.FOBPoint` | `POOrder.FOBPoint` | Standard → Standard | No (all PO types) |
| 2 | **Ship Via** | `SOOrder.ShipVia` | `POOrder.ShipVia` | Standard → Standard | No (all PO types) |
| 3 | **Shipping Terms** | `SOOrder.ShipTermsID` | `POOrderExt.UsrShipTermsID` | Standard → **Custom** | **Yes** (Drop-Ship only) |
| 4 | **Carrier Account** | `SOOrderExt.UsrCustomerAccount` | `POOrderExt.UsrCustomerAccount` | **Custom** → **Custom** | **Yes** (Drop-Ship only) |

### Why Shipping Terms uses a custom field on PO

The standard `POOrder` already has a `ShipTermsID` field, but it defaults from the **vendor location** and serves a different business purpose (vendor shipping terms). The custom `POOrderExt.UsrShipTermsID` preserves the **customer's** shipping terms from the originating Sales Order, keeping the two values separate.

---

## 1. DAC Extensions Required

### 1.1 SOOrderExt — Custom field: `UsrCustomerAccount`

**File**: DAC extension on `PX.Objects.SO.SOOrder`

This field stores the customer's carrier/shipping account number on the Sales Order header. It is only visible when the standard `SOOrder.UseCustomerAccount` flag is `true`.

```csharp
#region UsrCustomerAccount
[PXDBString(255, IsUnicode = true)]
[PXUIField(DisplayName = "Customer Account")]
[PXUIVisible(typeof(Where<Current<SOOrder.useCustomerAccount>, Equal<True>>))]
public string UsrCustomerAccount { get; set; }
public abstract class usrCustomerAccount : PX.Data.BQL.BqlString.Field<usrCustomerAccount> { }
#endregion
```

**Key points:**
- `PXDBString(255)` — persisted to database, 255 characters max
- Conditionally visible based on standard `SOOrder.UseCustomerAccount`
- No selector — free-text entry for the account number

### 1.2 POOrderExt — Custom fields: `UsrShipTermsID` and `UsrCustomerAccount`

**File**: DAC extension on `PX.Objects.PO.POOrder`

#### UsrShipTermsID

Stores the SO header's shipping terms on the PO header. Uses the same `ShipTerms` selector as the standard field.

```csharp
#region UsrShipTermsID
[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
[PXUIField(DisplayName = "Shipping Terms")]
[PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
public string UsrShipTermsID { get; set; }
public abstract class usrShipTermsID : PX.Data.BQL.BqlString.Field<usrShipTermsID> { }
#endregion
```

**Key points:**
- Matches the same `PXDBString(10)` length as the standard `ShipTerms.shipTermsID`
- `InputMask = ">aaaaaaaaaa"` — forces uppercase, alpha only (matching SO field)
- `PXSelector` against `ShipTerms` table — provides lookup/validation
- `PXDefault(PersistingCheck = PXPersistingCheck.Nothing)` — optional, no validation on save

#### UsrCustomerAccount

Stores the customer's carrier account number on the PO header (copied from `SOOrderExt.UsrCustomerAccount`).

```csharp
#region UsrCustomerAccount
[PXDBString(255, IsUnicode = true)]
[PXUIField(DisplayName = "Carrier Account")]
[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
public string UsrCustomerAccount { get; set; }
public abstract class usrCustomerAccount : PX.Data.BQL.BqlString.Field<usrCustomerAccount> { }
#endregion
```

**Key points:**
- Display name on PO is **"Carrier Account"** (vs. **"Customer Account"** on SO) — different labels, same data
- `PXDBString(255)` — matches SO field length
- No selector — free-text, copied from SO

---

## 2. Copy Logic — SO-to-PO during PO Creation

### 2.1 Where the copy lives

The copy logic is inside a method that processes each SO line/demand during PO creation. In the reference implementation this method is called `LinkPOLineToBlanket` inside a `POCreate` graph extension. Although the method operates at the line level (called once per SO line being processed), the header-level fields are set on the PO order header within this same method.

### 2.2 Required object references

Before the copy block, the following objects must already be resolved within the method:

```csharp
// The SO line's parent order — resolved via soline (SOLineSplit3) keys
SOOrder soOrder = SOOrder.PK.Find(graph, soline?.OrderType, soline?.OrderNbr);
SOOrderExt soOrderExt = soOrder?.GetExtension<SOOrderExt>();

// The PO order header extension
POOrderExt poOrderExt = docgraph?.CurrentDocument?.Current?.GetExtension<POOrderExt>();

// The demand record (POFixedDemand) — contains PlanType to identify Drop-Ship
POFixedDemand demand = /* already available in the processing loop */;
```

### 2.3 Copy implementation — exact logic

```csharp
// ── HEADER-LEVEL FIELD COPY (inside the block where soLine != null) ──

// 1) FOB Point — standard to standard, ALL PO types
docgraph.CurrentDocument.Current.FOBPoint = soOrder?.FOBPoint;

// 2) Ship Via — standard to standard, ALL PO types
docgraph.CurrentDocument.Current.ShipVia = soOrder?.ShipVia;

// 3) Shipping Terms — standard to CUSTOM, DROP-SHIP ONLY
// 4) Carrier Account — custom to custom, DROP-SHIP ONLY
if (demand.PlanType == INPlanConstants.Plan6D || demand.PlanType == "6D")
{
    poOrderExt.UsrShipTermsID = soOrder?.ShipTermsID;
    poOrderExt.UsrCustomerAccount = soOrderExt?.UsrCustomerAccount;
}
```

### 2.4 Behavior rules

| Rule | Detail |
|------|--------|
| **FOB Point and Ship Via always copy** | These are set for every PO type (regular, drop-ship, blanket-linked). They overwrite the vendor-defaulted values with the SO header values. |
| **Shipping Terms and Carrier Account are drop-ship gated** | They copy **only** when `demand.PlanType` equals `INPlanConstants.Plan6D` (string value `"6D"`). This is the plan type for Drop-Ship demand. |
| **Direct assignment, not SetValueExt** | FOB Point and Ship Via are set by direct property assignment on `docgraph.CurrentDocument.Current`. Shipping Terms and Carrier Account are set by direct property assignment on `poOrderExt`. Neither uses `SetValueExt`, so no field-level events are triggered during the copy. |
| **No first-wins / merge logic for headers** | Unlike line-level fields, header fields are **overwritten** each time the method runs for a new SO line in the same PO. In practice this is fine because all lines on a drop-ship PO originate from the same SO order (Acumatica groups drop-ship POs by SO). |
| **Null-safe** | Uses `?.` operator on `soOrder` and `soOrderExt` to avoid null reference exceptions if the SO cannot be found. |

### 2.5 Plan type reference

| Constant | String Value | Meaning |
|----------|-------------|---------|
| `INPlanConstants.Plan6D` | `"6D"` | SO-to-PO Drop-Ship demand |
| `INPlanConstants.Plan6B` | `"6B"` | Blanket PO demand |
| `INPlanConstants.Plan6E` | `"6E"` | Blanket Drop-Ship demand |

The reference code checks for both `INPlanConstants.Plan6D` and the literal `"6D"` — they are equivalent. Use `INPlanConstants.Plan6D` for clarity.

### 2.6 Placement within the method

The header copy block is placed:
- **After** the line-level custom field copies (UsrVendorSpecTerms, UsrVendorNotes, UsrShippingTerms)
- **Before** the blanket PO link logic and cost hierarchy evaluation
- **Inside** the `if (soLine != null)` guard — ensuring an SO line was found

Pseudo-structure:

```
LinkPOLineToBlanket(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ...)
{
    // Resolve SOLine, SOOrder, extensions...

    if (soLine != null)
    {
        // ── LINE-LEVEL COPIES ──
        // UsrVendorSpecTerms, UsrVendorNotes, UsrShippingTerms (on POLineExt)

        // ── HEADER-LEVEL COPIES (THIS GUIDE) ──
        // FOBPoint, ShipVia (always)
        // UsrShipTermsID, UsrCustomerAccount (drop-ship only)
    }

    // Blanket link logic, cost hierarchy, SPC code, etc.
}
```

---

## 3. PO Display Logic — POOrderEntry Graph Extension

### 3.1 No special RowSelected logic for these header fields

Unlike line-level fields (which are disabled and show warnings), the four header fields documented here do **not** have custom `RowSelected` logic to disable them or show warnings. Specifically:

- **FOB Point** and **Ship Via** — These are standard `POOrder` fields. The base Acumatica framework handles their display/edit behavior. The copy simply overwrites the vendor-defaulted value with the SO value.
- **UsrShipTermsID** and **UsrCustomerAccount** — These are custom header fields placed on the PO form. They are editable by default (no `SetEnabled(false)` call in RowSelected). Users can modify them on the PO if needed.

### 3.2 Header RowSelected handler (context)

The existing `RowSelected<POOrder>` handler in the reference implementation handles freight visibility only and does **not** touch FOB Point, Ship Via, Shipping Terms, or Carrier Account:

```csharp
protected virtual void _(Events.RowSelected<POOrder> e, PXRowSelected baseMethod)
{
    POOrder order = (POOrder)e.Row;
    if (order == null) return;

    if (order.SOOrderType == null || order.SOOrderNbr == null)
    {
        baseMethod?.Invoke(e.Cache, e.Args);
        return;
    }

    SOOrder soOrder = /* lookup by order.SOOrderType, order.SOOrderNbr */;
    SOSetupExt sOSetupExt = /* setup extension */;

    // Freight visibility logic only — not related to these four fields
    PXUIFieldAttribute.SetVisible<POOrderExt.usrFreightCost>(...);
    PXUIFieldAttribute.SetVisible<POOrderExt.usrFreightPrice>(...);

    baseMethod?.Invoke(e.Cache, e.Args);
}
```

### 3.3 RowSelecting handler (context)

The `RowSelecting<POLine>` handler also touches header-level fields, but only for freight-related runtime values. It does **not** affect the four fields in this guide. Relevant for context:

```csharp
protected virtual void _(Events.RowSelecting<POLine> e, PXRowSelecting baseMethod)
{
    // ...
    using (new PXConnectionScope())
    {
        DropShipLink link = GetDropShipLink(line);

        if (link != null && /* matches current line */)
        {
            SOOrder soOrder = /* lookup from DropShipLink */;
            SOOrderExt sOOrderExt = soOrder.GetExtension<SOOrderExt>();

            // These are RUNTIME freight fields, NOT the four header fields
            poExt.UsrShipTermsIDTemp = soOrder.ShipTermsID;  // non-persisted temp field
            poExt.UsrShowFreightCost = /* conditional */;
            poExt.UsrShowFreightPrice = /* conditional */;
            poExt.UsrShippingInstructions = sOOrderExt.UsrShippingInstructions;
        }
    }
    baseMethod?.Invoke(e.Cache, e.Args);
}
```

> **Note**: `UsrShipTermsIDTemp` is a non-persisted (`PXString`, not `PXDBString`) runtime field used only for freight visibility logic. It is separate from the persisted `UsrShipTermsID` field documented in this guide.

---

## 4. Database Schema

### Tables Modified

```sql
-- SOOrder Extension (if UsrCustomerAccount does not already exist)
ALTER TABLE SOOrder ADD UsrCustomerAccount NVARCHAR(255) NULL;

-- POOrder Extension
ALTER TABLE POOrder ADD UsrShipTermsID NVARCHAR(10) NULL;
ALTER TABLE POOrder ADD UsrCustomerAccount NVARCHAR(255) NULL;
```

> **FOB Point** and **Ship Via** are standard `POOrder` columns — no schema changes needed.

---

## 5. Implementation Steps for a New Version

### Step 1: Verify standard fields still exist

Confirm that the following standard fields exist and have the same property names in the target version:

| DAC | Field | Expected Type |
|-----|-------|---------------|
| `SOOrder` | `FOBPoint` | `string`, 15 chars |
| `SOOrder` | `ShipVia` | `string`, 15 chars |
| `SOOrder` | `ShipTermsID` | `string`, 10 chars |
| `SOOrder` | `UseCustomerAccount` | `bool?` |
| `POOrder` | `FOBPoint` | `string`, 15 chars |
| `POOrder` | `ShipVia` | `string`, 15 chars |

Search the base `POOrder.cs` and `SOOrder.cs` DAC files for these properties. If names or types have changed in the new version, adjust accordingly.

### Step 2: Add DAC extensions

1. **SOOrderExt** — Add `UsrCustomerAccount` field if it does not already exist (see Section 1.1)
2. **POOrderExt** — Add `UsrShipTermsID` and `UsrCustomerAccount` fields (see Section 1.2)

### Step 3: Locate the SO-to-PO copy method

Find the method where SO demand is processed to create PO lines. Search for:
- The method that calls `FillPOLineFromDemand` or `FillPOOrderFromDemand`
- References to `POFixedDemand` being iterated
- References to `SOLineSplit3` or `DropShipLink`
- The method where `demand.PlanType` is checked against `INPlanConstants.Plan6D`

In the reference implementation this is `LinkPOLineToBlanket`. In a different version it may be called differently or structured as an override/delegate.

### Step 4: Add the header copy logic

Insert the four-field copy block inside the method identified in Step 3. Place it:
- Inside the guard where the source `SOLine` / `SOOrder` is confirmed non-null
- After any line-level field copies
- Before blanket link logic

Use the exact pattern from Section 2.3:

```csharp
// FOB Point and Ship Via — always (all PO types)
docgraph.CurrentDocument.Current.FOBPoint = soOrder?.FOBPoint;
docgraph.CurrentDocument.Current.ShipVia = soOrder?.ShipVia;

// Shipping Terms and Carrier Account — drop-ship only
if (demand.PlanType == INPlanConstants.Plan6D || demand.PlanType == "6D")
{
    poOrderExt.UsrShipTermsID = soOrder?.ShipTermsID;
    poOrderExt.UsrCustomerAccount = soOrderExt?.UsrCustomerAccount;
}
```

### Step 5: Add custom fields to PO screen layout

Add `UsrShipTermsID` and `UsrCustomerAccount` to the Purchase Order screen (PO301000) in the appropriate section (e.g., Shipping Settings tab or a custom section near the standard FOB Point / Ship Via fields).

### Step 6: Verify and test

Run through the testing checklist in Section 6.

---

## 6. Testing Checklist

### Scenario 1: Drop-Ship PO from SO — All four fields copy

1. Create a Sales Order with:
   - FOB Point set (e.g., "DESTINATION")
   - Ship Via set (e.g., "FEDEX2DAY")
   - Shipping Terms set (e.g., "FOB")
   - Use Customer Account = true, Customer Account = "ACCT-12345"
2. Add a line with Drop-Ship source
3. Create Purchase Order via PO creation process
4. **Expected on PO header**:
   - `FOBPoint` = "DESTINATION" (standard field)
   - `ShipVia` = "FEDEX2DAY" (standard field)
   - `UsrShipTermsID` = "FOB" (custom field)
   - `UsrCustomerAccount` = "ACCT-12345" (custom field)

### Scenario 2: Regular (non-drop-ship) PO from SO — Only FOB Point and Ship Via copy

1. Create a Sales Order with all four fields populated
2. Add a line with **regular** PO source (not drop-ship)
3. Create Purchase Order
4. **Expected on PO header**:
   - `FOBPoint` = value from SO ✓
   - `ShipVia` = value from SO ✓
   - `UsrShipTermsID` = empty/null ✗ (not copied for non-drop-ship)
   - `UsrCustomerAccount` = empty/null ✗ (not copied for non-drop-ship)

### Scenario 3: SO without shipping fields populated

1. Create a Sales Order with FOB Point, Ship Via, Shipping Terms, and Customer Account all empty/null
2. Add a drop-ship line
3. Create Purchase Order
4. **Expected**: PO header fields remain at their defaults (vendor-defaulted for FOBPoint/ShipVia, null for custom fields). No errors.

### Scenario 4: Direct PO entry (no SO)

1. Create a Purchase Order directly (not from SO)
2. **Expected**: Standard FOBPoint and ShipVia default from vendor location as normal. Custom `UsrShipTermsID` and `UsrCustomerAccount` are empty.

### Scenario 5: Multiple SO lines on same drop-ship PO

1. Create a Sales Order with multiple drop-ship lines for the same vendor
2. All lines originate from the same SO header
3. Create Purchase Order
4. **Expected**: Header fields reflect the SO header values. Since all lines share the same SO header, the values are consistent regardless of processing order.

---

## 7. Related Implementations

These items are **not** part of this guide but are copied in the same method and share the same code path:

| Field | Level | Guide |
|-------|-------|-------|
| `UsrShippingTerms` | PO Line | See `UsrShippingTerms_Implementation_Guide.md` |
| `UsrVendorSpecTerms` | PO Line | Same pattern as UsrShippingTerms |
| `UsrVendorNotes` | PO Line | Same pattern, with concatenation logic |
| `UsrShippingInstructions` | PO Header | Copied via `SetValueExt` for drop-ship only |
| `UsrCustomerOrderNbr` | PO Header | Copies `SOOrder.CustomerOrderNbr` for drop-ship |

### UsrShippingInstructions copy (for reference)

This field is also copied in the same method but uses `SetValueExt` instead of direct assignment, and has its own drop-ship gate:

```csharp
if (demand.PlanDate != null && demand.PlanType == INPlanConstants.Plan6D)
{
    docgraph?.CurrentDocument.Cache.SetValueExt<POOrderExt.usrShippingInstructions>(
        docgraph.CurrentDocument.Current, soOrderExt?.UsrShippingInstructions);
}
```

### UsrCustomerOrderNbr copy (for reference)

```csharp
if (poOrderExt != null && demand.PlanType == INPlanConstants.Plan6D)
{
    poOrderExt.UsrCustomerOrderNbr = soOrder?.CustomerOrderNbr;
}
```

---

## 8. Troubleshooting

### FOB Point / Ship Via not copying

- **Check**: The `SOOrder` lookup succeeds. The method resolves the SO via `SOOrder.PK.Find(graph, soline?.OrderType, soline?.OrderNbr)`. If `soline` is null or has null keys, the SO will not be found.
- **Check**: The copy block is inside the `if (soLine != null)` guard. If the SO *line* lookup fails (different from the SO *order* lookup), the entire block is skipped.
- **Check**: The PO is not being created from a blanket order where `soline` may have different keys.

### Shipping Terms / Carrier Account not copying

- **Check**: The `demand.PlanType` equals `"6D"`. Use trace logging to verify the actual plan type value.
- **Check**: The `poOrderExt` is not null — this can happen if `docgraph?.CurrentDocument?.Current` is null.
- **Check**: The `SOOrderExt` exists and `UsrCustomerAccount` is populated on the SO.

### Values getting overwritten by vendor defaults

- **Symptom**: FOB Point or Ship Via on the PO shows the vendor default instead of the SO value.
- **Cause**: The standard `POOrder` field defaulting runs **after** the custom copy, resetting the value.
- **Fix**: Ensure the copy logic runs at the right point in the PO creation process — after `FillPOOrderFromDemand` (which sets vendor defaults) and after the PO header is established. In the reference implementation, `LinkPOLineToBlanket` runs after `FillPOLineFromDemand`, which is after the order header has been finalized.

---

## Document Version
- **Created**: 2025
- **Source Version**: Acumatica 2024 R1 (24.112.006)
- **Customization Project**: CompiledVersion
- **Reference Files**:
  - `DACExt/SOOrderExt.cs` — SOOrderExt DAC extension
  - `DACExt/POOrderExt.cs` — POOrderExt DAC extension
  - `GraphExt/POCreateReplaceOriginalMethod.cs` — `LinkPOLineToBlanket` method (copy logic)
  - `GraphExt/POOrderEntry_Extension.cs` — PO display/event handlers (context only)

# SPC Cost — Full Implementation Analysis

> **Generated:** February 16, 2026  
> **Purpose:** Reference document to compare SPC Cost implementation across versions.

---

## What Is SPC Cost?

**SPC Cost** (`UsrSWKSPCCost`) is a **Special Pricing Cost** — a manually-entered override cost that takes the highest priority in the cost hierarchy, overriding the standard RTH (Rate Table/House) Cost. It is always paired with a required **SPC Code** (`UsrSWKSPCCode`) that identifies the reason/authorization for the special pricing.

---

## Data Fields

| Entity | Fields | Storage | Notes |
|--------|--------|---------|-------|
| **CROpportunityProducts** | `UsrSWKSPCCost` (Decimal), `UsrSWKSPCCode` (String 30) | Persisted (`PXDBDecimal`/`PXDBString`) | Entry point — user enters here first |
| **SOLine** | `UsrSWKSPCCost` (Decimal), `UsrSWKSPCCode` (String 30), `UsrSWKManualCost` (Bool) | Persisted | Carried from Opportunity or entered directly |
| **POFixedDemand** | `UsrSOSPCCost` (Decimal, scalar lookup), `UsrSOSPCCode` (String, scalar lookup), `UsrSWKSPCCode` (String) | Mixed (scalar + persisted) | Read from linked SO line via `PXDBScalar` |
| **POLine** | `UsrSWKSPCCode` (String 30), `UsrUsedVendorPrice` (Bool) | Persisted | SPC Code carried; flag tracks if SPC/Vendor price was used |
| **INItemPlan** | `UsrSWKSPCCode` (String 30) | Persisted | Tracks SPC through inventory planning |

---

## Processes Affected & Behavior

### 1. Opportunity / Quote

**File:** `GraphExt/OpportunityMaintExt.cs`

- When `CuryUnitCost` defaults on an Opportunity Product line, **SPC Cost is checked first**
- If SPC Cost > 0 → Unit Cost = SPC Cost (RTH Cost ignored, no UOM conversion)
- If SPC Cost = 0 → Unit Cost = RTH Cost (with UOM conversion if needed)
- When SPC Cost is updated → Unit Cost recalculates automatically via `FieldUpdated` handler

### 2. Opportunity → Sales Order Conversion

**File:** `GraphExt/SOOrderEntryCopyOppFieldsExt.cs`

- On `SOLine.RowInserted`, copies **RTH Cost, SPC Cost, and SPC Code** from the source `CROpportunityProducts` to the new SO line
- Uses generic `GetValue`/`SetValueExt` approach for cross-project compatibility

### 3. Sales Order Entry

**File:** `GraphExt/SOOrderEntry_Extension.cs`

This is the most complex area. Key behaviors:

#### SPC Cost Updated (`FieldUpdated<SOLine, SOLineExt.usrSWKSPCCost>`)
- If > 0: Sets `ManualCost = true`, **requires SPC Code** (error if missing), recalculates Unit Cost and Extended Cost
- If = 0: Clears SPC Code, sets `ManualCost = false`, clears validation errors

#### SPC Code Validation (`FieldVerifying<SOLine, SOLineExt.usrSWKSPCCode>`)
- Cannot enter SPC Code if SPC Cost = 0 (auto-cleared with warning)
- Cannot clear SPC Code if SPC Cost > 0 (throws error)

#### SPC Code Updated (`FieldUpdated<SOLine, SOLineExt.usrSWKSPCCode>`)
- If SPC Code entered but SPC Cost = 0 → code is auto-cleared with warning message

#### Extended Cost Override (`CacheAttached<SOLine.extCost>`)
- Uses a `PXFormula`:
  ```
  If SPC Cost > 0 → ExtCost = Qty × SPC Cost  (bypasses standard UOM-based calculation)
  Otherwise       → ExtCost = Qty × UnitCost   (standard)
  ```

#### Unit Cost Calculation (`CalculateUnitCostWithUOM`)
- SPC Cost > 0 → returns SPC Cost directly (no UOM conversion needed)
- Otherwise → uses RTH Cost with UOM conversion

#### Row Persisting (`RowPersisting<SOLine>`)
- Final validation — blocks save if SPC Cost > 0 but SPC Code is empty

#### Row Selected (`RowSelected<SOLine>`)
- SPC Code field is **enabled only when SPC Cost > 0**

#### Inventory ID Updated
- When `InventoryID` changes, SPC Cost triggers re-evaluation of unit cost
- If neither RTH nor SPC cost provided, no cost override is applied

### 4. PO Create Process

**File:** `GraphExt/POCreate4.cs`

#### Fixed Demand Preparation (`EnumerateAndPrepareFixedDemandRow`)
- Copies `UsrSWKSPCCode` from SO line to demand extension
- Sets `EffPrice` (Vendor Price) from SO line's `CuryExtCost` for drop-ship/purchase plan types

#### PO Order Grouping (`FindOrCreatePOOrder`)
- **Lines with different SPC Costs create separate PO orders** — an SPC Cost value is added to the `orderSearchValues` criteria so lines with different special pricing won't merge into the same PO

### 5. Purchase Order Entry

**File:** `GraphExt/POOrderEntry_Extension.cs`

This implements the **4-level price hierarchy** in `EnsureExtCostAndUnitCostFailsafe`:

| Priority | Source | Condition | RTH Floor |
|----------|--------|-----------|-----------|
| **1st** | SPC Cost | From linked SO line (`UsrSWKSPCCost > 0`) | **BYPASSED** |
| **2nd** | Vendor Price | From AP Vendor Price tables | **BYPASSED** |
| **3rd** | RTH Cost | From Inventory Item `UsrSWKRTHCost` | Enforced |
| **4th** | Last Cost | Standard Acumatica fallback | Enforced |

#### SPC Cost Lookup — 4 Strategies (priority order)

1. **DropShipLink** — works for existing drop-ship PO lines
2. **SOLineSplit search** — for drop-ship during Insert (when DropShipLink doesn't exist yet)
3. **SOLineSplit reverse lookup** — by PO reference for regular "Mark for PO" lines
4. **SOLineSplit pending lookup** — lines not yet linked to a PO

For merged lines (multiple SO lines → one PO line), **picks the highest SPC Cost**.

#### SPC Code Defaulting (`FieldDefaulting<POLine, POLineExt.usrSWKSPCCode>`)
- Auto-copies SPC Code from the linked SO line
- Field is **read-only** on the PO

#### RTH Floor Bypass
- When `UsrUsedVendorPrice = true` (SPC or Vendor Price was used):
  - RTH floor enforcement is **skipped**
  - Only a non-negative check applies (cost ≥ 0)
- When `UsrUsedVendorPrice = false`:
  - Unit Cost cannot go below RTH Cost
  - Extended Cost cannot go below RTH × Qty

#### Unit Cost Field Defaulting (`POLine_CuryUnitCost_FieldDefaulting`)
- Follows the same 4-level hierarchy as the failsafe method
- Uses the same 4 SO line lookup strategies
- Sets `UsrUsedVendorPrice` flag based on which source was used

---

## Validation Rules Summary

| Rule | Where Enforced | Severity |
|------|----------------|----------|
| SPC Code required when SPC Cost > 0 | SO Line — FieldUpdated, RowPersisting | Error (blocks save) |
| SPC Code auto-cleared when SPC Cost = 0 | SO Line — FieldUpdated | Auto-action |
| SPC Code rejected when SPC Cost = 0 | SO Line — FieldVerifying, FieldUpdated | Warning + auto-clear |
| SPC Code field disabled when SPC Cost = 0 | SO Line — RowSelected | UI disabled |
| SPC Code read-only on PO Line | PO Order Entry — CacheAttached, RowSelected | UI disabled |
| SPC Cost > 0 bypasses RTH floor | PO Order Entry — EnsureExtCostAndUnitCostFailsafe | Logic change |

---

## Messages

| Constant | Text |
|----------|------|
| `SPCCodeRequired` | "SPC Code is required when SPC Cost is greater than zero." |
| `SPCCodeOnlyWithCost` | "SPC Code can only be entered when SPC Cost is greater than zero." |

---

## Flow Diagram (Non-Developer Friendly)

```
┌─────────────────────────────────────────────────────────┐
│                  OPPORTUNITY / QUOTE                     │
│                                                         │
│   User enters SPC Cost + SPC Code on product line       │
│   → Unit Cost automatically set to SPC Cost             │
└────────────────────────┬────────────────────────────────┘
                         │ Convert to Sales Order
                         ▼
┌─────────────────────────────────────────────────────────┐
│                    SALES ORDER                           │
│                                                         │
│   SPC Cost, SPC Code, RTH Cost copied to SO line        │
│                                                         │
│   IF SPC Cost > 0:                                      │
│     ✓ Manual Cost checkbox = ON                         │
│     ✓ SPC Code REQUIRED (cannot save without it)        │
│     ✓ Unit Cost = SPC Cost (no UOM conversion)          │
│     ✓ Extended Cost = SPC Cost × Quantity               │
│                                                         │
│   IF SPC Cost = 0:                                      │
│     ✗ Use RTH Cost (with UOM conversion)                │
│     ✗ SPC Code field disabled and cleared               │
│     ✗ Extended Cost = Unit Cost × Quantity              │
└────────────────────────┬────────────────────────────────┘
                         │ Mark for PO / Drop-Ship
                         ▼
┌─────────────────────────────────────────────────────────┐
│                  PO CREATE PROCESS                       │
│                                                         │
│   SPC Cost & SPC Code passed to purchase demand          │
│                                                         │
│   ⚡ Lines with DIFFERENT SPC Costs go to               │
│      SEPARATE purchase orders (not merged)               │
│                                                         │
│   Vendor Price set from SO line cost                     │
└────────────────────────┬────────────────────────────────┘
                         │ Create PO
                         ▼
┌─────────────────────────────────────────────────────────┐
│                  PURCHASE ORDER                          │
│                                                         │
│   SPC Code shown (read-only) on PO line                 │
│                                                         │
│   PRICE HIERARCHY (checked in order):                   │
│     1️⃣  SPC Cost      ← HIGHEST PRIORITY               │
│     2️⃣  Vendor Price                                    │
│     3️⃣  RTH Cost                                       │
│     4️⃣  Last Cost     ← FALLBACK                       │
│                                                         │
│   IF SPC or Vendor Price used:                          │
│     → RTH minimum floor check is BYPASSED               │
│     → Only checks cost is not negative                  │
│                                                         │
│   IF RTH/Last Cost used:                                │
│     → RTH minimum floor is ENFORCED                     │
│     → Cost cannot drop below RTH Cost                   │
└─────────────────────────────────────────────────────────┘
```

---

## Source Files Reference

| File | What It Does |
|------|-------------|
| `DACExt/CROpportunityProductsExt.cs` | SPC Cost + SPC Code fields on Opportunity Products |
| `DACExt/SOLineExt.cs` | SPC Cost + SPC Code + Manual Cost fields on SO Line |
| `DACExt/POLineExt.cs` | SPC Code + UsedVendorPrice flag on PO Line |
| `DACExt/POFixedDemandExt.cs` | SO SPC Cost/Code scalar lookups + SPC Code on demand/INItemPlan |
| `GraphExt/OpportunityMaintExt.cs` | SPC Cost priority in Opportunity unit cost defaulting |
| `GraphExt/SOOrderEntryCopyOppFieldsExt.cs` | Copy SPC fields from Opportunity to SO on conversion |
| `GraphExt/SOOrderEntry_Extension.cs` | SO line SPC validation, ManualCost toggle, ExtCost formula, unit cost calc |
| `GraphExt/POCreate4.cs` | SPC-based PO grouping, demand preparation, SPC Code copy |
| `GraphExt/POOrderEntry_Extension.cs` | 4-level price hierarchy, SPC lookup strategies, RTH bypass logic |
| `Messages/Messages.cs` | SPCCodeRequired and SPCCodeOnlyWithCost error messages |

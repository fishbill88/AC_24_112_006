# Create Purchase Orders (PO505000) — Vendor Price Implementation Plan

> **Purpose**: This document provides a comprehensive, step-by-step implementation plan for the **Vendor Price** functionality on the **Create Purchase Orders (PO505000)** form. It is designed to be fed to Copilot in a newer Acumatica version to reproduce the behavior in a **separate, dedicated extension file**.
>
> **Source Version**: Acumatica 24R1 (CompiledVersion project)  
> **Target**: Newer Acumatica version (25R2+)  
> **Date Created**: February 13, 2026

---

## Table of Contents

1. [Business Requirements](#1-business-requirements)
2. [Architecture Overview](#2-architecture-overview)
3. [Prerequisites & Dependencies](#3-prerequisites--dependencies)
4. [DAC Extensions Required](#4-dac-extensions-required)
5. [Graph Extension Implementation](#5-graph-extension-implementation)
6. [Detailed Code Specifications](#6-detailed-code-specifications)
7. [Plan Type Constants Reference](#7-plan-type-constants-reference)
8. [Data Flow Diagram](#8-data-flow-diagram)
9. [Testing Checklist](#9-testing-checklist)
10. [Important Notes for Newer Versions](#10-important-notes-for-newer-versions)
11. [PO Order Entry — Cost Hierarchy Re-Evaluation](#11-po-order-entry--cost-hierarchy-re-evaluation-polinecuryunitcost)

---

## 1. Business Requirements

The **Vendor Price** field on the Create Purchase Orders (PO505000) form must behave as follows:

| # | Requirement | Details |
|---|-------------|---------|
| **a** | **Field identity** | Uses the existing `POFixedDemand.EffPrice` field |
| **b** | **Editable** | The field must be user-editable on the grid |
| **c** | **SO to Drop-Ship / SO to Purchase** | When the plan type equals **SO to Drop-Ship** (`Plan6D`, `Plan6E`) or **SO to Purchase** (`Plan66`), populate Vendor Price with the **ExtCost** (`SOLine.CuryExtCost`) value from the linked Sales Orders Details line |
| **d** | **All other plan types** | When the plan type is NOT Drop-Ship/Purchase, populate Vendor Price with the **RTH Cost** (`InventoryItem.UsrSWKRTHCost`) assigned to the selected inventory item |
| **e** | **Native PO line cost** | Acumatica's native functionality writes the Vendor Price field value to `POLine.CuryUnitCost` on the Purchase Orders Details line when the purchase order is created |

---

## 2. Architecture Overview

Create a **single, self-contained graph extension file** that handles all Vendor Price logic for PO505000. This keeps the functionality isolated and easy to maintain.

### File Structure

```
DACExt/
  POFixedDemandExt.cs          ← Must already have UsrSWKRTHCost field (may already exist)
  InventoryItemExt.cs          ← Must already have UsrSWKRTHCost field (may already exist)

GraphExt/
  POCreate_VendorPrice.cs      ← NEW FILE — All Vendor Price logic lives here
```

### Extension Class Name

```csharp
public class POCreate_VendorPrice : PXGraphExtension<POCreate>
```

> **Why a separate extension?** Acumatica supports multiple `PXGraphExtension` classes on the same base graph. By isolating the Vendor Price logic, it avoids conflicts with other POCreate extensions and simplifies maintenance.

---

## 3. Prerequisites & Dependencies

Before implementing, verify these exist in your project:

### 3.1 DAC Extension: `InventoryItemExt`

The `InventoryItem` DAC must have a custom `UsrSWKRTHCost` field:

```csharp
public sealed class InventoryItemExt : PXCacheExtension<PX.Objects.IN.InventoryItem>
{
    public static bool IsActive() => true;

    #region UsrSWKRTHCost
    [PXDBDecimal(2)]
    [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
    [PXUIField(DisplayName = "RTH Cost")]
    public decimal? UsrSWKRTHCost { get; set; }
    public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
    #endregion
}
```

**Database column**: `InventoryItem.UsrSWKRTHCost` (Decimal, 2 precision)

### 3.2 DAC Extension: `POFixedDemandExt`

The `POFixedDemand` DAC must have a custom `UsrSWKRTHCost` unbound field for display purposes:

```csharp
// This field is part of POFixedDemandExt — it is NOT persisted.
// It is populated at runtime from InventoryItemExt.UsrSWKRTHCost
#region UsrSWKRTHCost
[PXDecimal(2)]
[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "RTH Cost", Enabled = false)]
public decimal? UsrSWKRTHCost { get; set; }
public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
#endregion
```

> **Note**: `UsrSWKRTHCost` on `POFixedDemandExt` uses `[PXDecimal]` (NOT `[PXDBDecimal]`) because it is an unbound/virtual field populated at runtime. It does NOT require a database column on the `POFixedDemand` table.

### 3.3 Namespaces Required

```csharp
using PX.Data;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
```

Plus your project's DAC namespace (e.g., `using YourProject.DAC;`)

---

## 4. DAC Extensions Required

If these DAC extensions **do not already exist** in your newer project, create them first. If they **already exist** (from migration), just verify they have the `UsrSWKRTHCost` fields.

### 4.1 InventoryItemExt — Full Definition

```csharp
using PX.Data;
using System;

namespace YourProject.DAC
{
    public sealed class InventoryItemExt : PXCacheExtension<PX.Objects.IN.InventoryItem>
    {
        public static bool IsActive() => true;

        #region UsrSWKRTHCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Cost")]
        public decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion
    }
}
```

### 4.2 POFixedDemandExt — Vendor Price Related Fields Only

```csharp
using PX.Data;
using System;

namespace YourProject.DAC
{
    public sealed class POFixedDemandExt : PXCacheExtension<PX.Objects.PO.POFixedDemand>
    {
        public static bool IsActive() => true;

        #region UsrSWKRTHCost
        // Unbound field — populated at runtime from InventoryItemExt.UsrSWKRTHCost
        [PXDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Cost", Enabled = false)]
        public decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion
    }
}
```

---

## 5. Graph Extension Implementation

### 5.1 New File: `POCreate_VendorPrice.cs`

This is the **complete graph extension** that implements all Vendor Price functionality. Create this as a new file.

```csharp
using PX.Data;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using YourProject.DAC;  // Adjust namespace to your project

namespace YourProject.GraphExt  // Adjust namespace to your project
{
    /// <summary>
    /// POCreate (PO505000) extension for Vendor Price functionality.
    /// 
    /// Business Rules:
    ///   1. POFixedDemand.EffPrice is re-labeled "Vendor Price" and made editable
    ///   2. When plan type = SO to Drop-Ship (6D, 6E) or SO to Purchase (66):
    ///      → Vendor Price = SOLine.CuryExtCost from the linked Sales Order line
    ///   3. When plan type ≠ above:
    ///      → Vendor Price = InventoryItem.UsrSWKRTHCost (RTH Cost)
    ///   4. When user manually edits Vendor Price, ExtCost recalculates (Qty × Price)
    ///   5. Native Acumatica writes EffPrice to POLine.CuryUnitCost when PO is created
    /// </summary>
    public class POCreate_VendorPrice : PXGraphExtension<POCreate>
    {
        public static bool IsActive() => true;

        #region CacheAttached — Make EffPrice Editable and Renamed

        /// <summary>
        /// Re-label the EffPrice field to "Vendor Price" and enable it for editing.
        /// Uses PXMergeAttributes to preserve existing attributes while overriding display/enabled.
        /// </summary>
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Vendor Price", Enabled = true)]
        protected virtual void _(Events.CacheAttached<POFixedDemand.effPrice> e) { }

        #endregion

        #region Event: EnumerateAndPrepareFixedDemandRow Override

        /// <summary>
        /// Override the row preparation to populate Vendor Price (EffPrice) 
        /// based on plan type BEFORE the grid is displayed.
        /// 
        /// This is the MAIN entry point for populating the Vendor Price value.
        /// It fires for each POFixedDemand row when the grid loads.
        /// </summary>
        [PXOverride]
        public virtual void EnumerateAndPrepareFixedDemandRow(
            PXResult<POFixedDemand> rec,
            System.Action<PXResult<POFixedDemand>> baseMethod)
        {
            // IMPORTANT: Always call base method first to let standard logic run
            baseMethod(rec);

            var demand = (POFixedDemand)rec;

            // Calculate the custom Vendor Price based on plan type
            decimal? customVendorPrice = CalculateVendorPriceFromPlanType(demand);

            if (customVendorPrice != null)
            {
                // Overwrite EffPrice with our custom value
                demand.EffPrice = customVendorPrice;
            }
        }

        #endregion

        #region Event: FieldUpdated — Recalculate ExtCost on Manual Edit

        /// <summary>
        /// When the user manually changes Vendor Price (EffPrice),
        /// recalculate ExtCost = OrderQty × EffPrice.
        /// </summary>
        protected virtual void _(Events.FieldUpdated<POFixedDemand, POFixedDemand.effPrice> e)
        {
            if (e.Row == null) return;

            POFixedDemand demand = e.Row;

            if (demand.OrderQty != null && demand.EffPrice != null)
            {
                demand.ExtCost = demand.OrderQty * demand.EffPrice;
                Base.FixedDemand.Cache.RaiseFieldUpdated<POFixedDemand.extCost>(demand, null);
            }
        }

        #endregion

        #region Event: RowSelecting — Populate RTH Cost at Runtime

        /// <summary>
        /// Populate the unbound UsrSWKRTHCost field on POFixedDemandExt
        /// from the InventoryItem's UsrSWKRTHCost value.
        /// This runs inside a PXConnectionScope for DB access during row selection.
        /// </summary>
        protected virtual void _(Events.RowSelecting<POFixedDemand> e)
        {
            if (e.Row == null) return;

            using (new PXConnectionScope())
            {
                InventoryItem item = InventoryItem.PK.Find(Base, e.Row.InventoryID);
                POFixedDemandExt ext = e.Row.GetExtension<POFixedDemandExt>();
                InventoryItemExt itemExt = item?.GetExtension<InventoryItemExt>();

                if (ext != null && itemExt != null)
                {
                    ext.UsrSWKRTHCost = itemExt.UsrSWKRTHCost;
                }
            }
        }

        #endregion

        #region Event: RowSelected — Enable EffPrice for Editing

        /// <summary>
        /// Ensure the EffPrice field is enabled for editing on every row.
        /// </summary>
        protected virtual void _(Events.RowSelected<POFixedDemand> e)
        {
            if (e.Row == null) return;

            PXUIFieldAttribute.SetEnabled<POFixedDemand.effPrice>(e.Cache, e.Row, true);
        }

        #endregion

        #region Helper: CalculateVendorPriceFromPlanType

        /// <summary>
        /// Main calculation method for Vendor Price.
        /// 
        /// Logic:
        ///   IF plan type is SO to Drop-Ship (Plan6D, Plan6E) or SO to Purchase (Plan66):
        ///     → Return SOLine.CuryExtCost from the linked SO line
        ///   ELSE:
        ///     → Return InventoryItem.UsrSWKRTHCost (RTH Cost)
        ///   
        ///   Returns null if no value can be determined (leaves default EffPrice).
        /// </summary>
        protected virtual decimal? CalculateVendorPriceFromPlanType(POFixedDemand demand)
        {
            if (demand?.PlanType == null || demand.InventoryID == null)
                return null;

            // ---------------------------------------------------------------
            // Rule c: Plan type = SO to Drop-Ship or SO to Purchase
            //   INPlanConstants.Plan6D = "6D" → SO to Drop-Ship
            //   INPlanConstants.Plan6E = "6E" → Blanket Drop-Ship
            //   INPlanConstants.Plan66 = "66" → SO to Purchase
            // ---------------------------------------------------------------
            bool isSOToDropShipOrPurchase =
                demand.PlanType == INPlanConstants.Plan6D ||
                demand.PlanType == INPlanConstants.Plan6E ||
                demand.PlanType == INPlanConstants.Plan66;

            if (isSOToDropShipOrPurchase)
            {
                decimal? soExtCost = GetSOLineCuryExtCost(demand);
                if (soExtCost != null)
                {
                    return soExtCost;
                }
            }

            // ---------------------------------------------------------------
            // Rule d: All other plan types → Use RTH Cost from inventory item
            // ---------------------------------------------------------------
            InventoryItem item = InventoryItem.PK.Find(Base, demand.InventoryID);
            if (item != null)
            {
                var itemExt = item.GetExtension<InventoryItemExt>();
                if (itemExt?.UsrSWKRTHCost != null && itemExt.UsrSWKRTHCost > 0)
                {
                    return itemExt.UsrSWKRTHCost;
                }
            }

            return null;
        }

        #endregion

        #region Helper: GetSOLineCuryExtCost

        /// <summary>
        /// Looks up the related SOLine for a given POFixedDemand row
        /// and returns SOLine.CuryExtCost.
        /// 
        /// The demand row carries OrderType, OrderNbr, LineNbr which
        /// identify the originating SO line.
        /// </summary>
        protected virtual decimal? GetSOLineCuryExtCost(POFixedDemand demand)
        {
            if (string.IsNullOrEmpty(demand.OrderType) ||
                string.IsNullOrEmpty(demand.OrderNbr) ||
                demand.LineNbr == null)
                return null;

            SOLine soLine = PXSelect<SOLine,
                Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                    And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                    And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                .Select(Base, demand.OrderType, demand.OrderNbr, demand.LineNbr);

            if (soLine != null)
            {
                return soLine.CuryExtCost;
            }

            return null;
        }

        #endregion
    }
}
```

---

## 6. Detailed Code Specifications

### 6.1 Method: `EnumerateAndPrepareFixedDemandRow` Override

| Aspect | Detail |
|--------|--------|
| **When it fires** | When the PO505000 grid loads / refreshes, once per demand row |
| **Signature** | `void EnumerateAndPrepareFixedDemandRow(PXResult<POFixedDemand> rec, Action<PXResult<POFixedDemand>> baseMethod)` |
| **Must call** | `baseMethod(rec)` FIRST — standard Acumatica logic must run first |
| **Sets** | `demand.EffPrice = customVendorPrice` |
| **Version Note** | This method exists in Acumatica 24R1+. **Verify the method signature in your target version.** If the signature changes, adjust accordingly. Check `PX.Objects.PO.POCreate` for the current override signature. |

### 6.2 Method: `CalculateVendorPriceFromPlanType`

| Input | `POFixedDemand demand` |
|-------|------------------------|
| **Returns** | `decimal?` — the calculated Vendor Price, or `null` to leave default |
| **Plan type check** | `Plan6D` ("6D"), `Plan6E` ("6E"), `Plan66` ("66") → Use SO ExtCost |
| **Fallback** | For all other plan types → Use `InventoryItemExt.UsrSWKRTHCost` |
| **Further fallback** | Return `null` if no RTH Cost is found or is 0 |

### 6.3 Method: `GetSOLineCuryExtCost`

| Input | `POFixedDemand demand` |
|-------|------------------------|
| **Returns** | `decimal?` — `SOLine.CuryExtCost` value |
| **Lookup key** | `demand.OrderType`, `demand.OrderNbr`, `demand.LineNbr` → matches `SOLine` |
| **Returns null if** | SOLine not found or keys are null/empty |

### 6.4 CacheAttached: `POFixedDemand.effPrice`

```csharp
[PXMergeAttributes(Method = MergeMethod.Merge)]
[PXUIField(DisplayName = "Vendor Price", Enabled = true)]
```

- `MergeMethod.Merge` preserves existing attributes and only overrides `PXUIField`
- Changes display name from default to "Vendor Price"
- Sets `Enabled = true` to allow editing

### 6.5 FieldUpdated: `POFixedDemand.effPrice`

- Recalculates: `demand.ExtCost = demand.OrderQty × demand.EffPrice`
- Raises `FieldUpdated` on `extCost` to trigger any dependent recalculations

### 6.6 RowSelecting: `POFixedDemand`

- Runs inside `PXConnectionScope` (required for DB access during RowSelecting)
- Looks up `InventoryItem` by `demand.InventoryID`
- Copies `InventoryItemExt.UsrSWKRTHCost` → `POFixedDemandExt.UsrSWKRTHCost`
- This populates the "RTH Cost" display column on the grid

### 6.7 RowSelected: `POFixedDemand`

- `PXUIFieldAttribute.SetEnabled<POFixedDemand.effPrice>(e.Cache, e.Row, true)`
- Ensures the field stays enabled even after row selection events

---

## 7. Plan Type Constants Reference

These are the `INPlanConstants` values used for plan type identification:

| Constant | String Value | Meaning |
|----------|-------------|---------|
| `INPlanConstants.Plan6D` | `"6D"` | SO to Drop-Ship demand |
| `INPlanConstants.Plan6E` | `"6E"` | Blanket Drop-Ship demand |
| `INPlanConstants.Plan66` | `"66"` | SO to Purchase demand |
| `INPlanConstants.Plan6B` | `"6B"` | Blanket PO demand |

**Decision matrix for Vendor Price source**:

```
Plan Type ∈ {6D, 6E, 66}  →  Vendor Price = SOLine.CuryExtCost
Plan Type ∉ {6D, 6E, 66}  →  Vendor Price = InventoryItem.UsrSWKRTHCost
```

---

## 8. Data Flow Diagram

```
PO505000 Grid Loads
        │
        ▼
EnumerateAndPrepareFixedDemandRow()
        │
        ├── baseMethod(rec)           ← Standard Acumatica logic first
        │
        ▼
CalculateVendorPriceFromPlanType(demand)
        │
        ├── Is PlanType in {6D, 6E, 66}?
        │       │
        │       YES → GetSOLineCuryExtCost(demand)
        │       │       │
        │       │       ├── Lookup SOLine by OrderType/OrderNbr/LineNbr
        │       │       └── Return SOLine.CuryExtCost
        │       │
        │       NO → Lookup InventoryItem by demand.InventoryID
        │               │
        │               └── Return InventoryItemExt.UsrSWKRTHCost
        │
        ▼
demand.EffPrice = calculatedValue
        │
        ▼
Grid displays "Vendor Price" column with value
        │
        ▼ (User creates PO)
        │
Native Acumatica → POLine.CuryUnitCost = demand.EffPrice
```

### Additional Runtime Flow

```
RowSelecting(POFixedDemand)
        │
        └── Populate POFixedDemandExt.UsrSWKRTHCost from InventoryItemExt
            (for "RTH Cost" display column on grid)

RowSelected(POFixedDemand)
        │
        └── Enable EffPrice field for editing

FieldUpdated(POFixedDemand.effPrice)
        │
        └── Recalculate ExtCost = OrderQty × EffPrice
```

---

## 9. Testing Checklist

### 9.1 Basic Functionality

- [ ] **Field Label**: Verify the EffPrice column on PO505000 shows as "Vendor Price"
- [ ] **Field Editable**: Verify the Vendor Price field can be manually edited by the user
- [ ] **ExtCost Recalculation**: After editing Vendor Price, verify ExtCost = Qty × Vendor Price

### 9.2 Plan Type: SO to Drop-Ship (Plan6D)

- [ ] Create a Sales Order with line having `CuryExtCost` = $500
- [ ] Set the SO line to use Drop-Ship mark for replenishment
- [ ] Open PO505000 → Find the demand row
- [ ] **Verify**: Vendor Price = $500 (the SOLine.CuryExtCost value)
- [ ] Create the PO and verify `POLine.CuryUnitCost` = $500

### 9.3 Plan Type: SO to Purchase (Plan66)

- [ ] Create a Sales Order with line having `CuryExtCost` = $300
- [ ] Set the SO line to "Purchase to Order" replenishment
- [ ] Open PO505000 → Find the demand row
- [ ] **Verify**: Vendor Price = $300 (the SOLine.CuryExtCost value)

### 9.4 Plan Type: Other (e.g., Transfer, Replenishment)

- [ ] Set up an inventory item with `UsrSWKRTHCost` = $150
- [ ] Create a demand that is NOT Drop-Ship or SO-to-Purchase
- [ ] Open PO505000 → Find the demand row
- [ ] **Verify**: Vendor Price = $150 (the InventoryItem.UsrSWKRTHCost value)

### 9.5 Edge Cases

- [ ] Inventory item with `UsrSWKRTHCost` = 0 → Vendor Price should keep default value
- [ ] SOLine not found (orphaned demand) → Vendor Price should keep default value
- [ ] Manual edit of Vendor Price → Value persists and flows to PO
- [ ] RTH Cost column displays correctly (read-only) on the grid

---

## 10. Important Notes for Newer Versions

### 10.1 Method Signature Verification

> **CRITICAL**: The `EnumerateAndPrepareFixedDemandRow` method signature may change between versions.
>
> Before implementing, search the Acumatica source code for the current method in `PX.Objects.PO.POCreate`:
>
> ```csharp
> // Search for this in the POCreate graph:
> public virtual void EnumerateAndPrepareFixedDemandRow(...)
> ```
>
> If the method name or parameters have changed, adjust the `[PXOverride]` signature accordingly.

### 10.2 INPlanConstants Changes

- Verify that `INPlanConstants.Plan6D`, `Plan6E`, and `Plan66` still exist and have the same string values
- These have been stable across versions but always confirm

### 10.3 PK Find Pattern

- The code uses `InventoryItem.PK.Find(Base, id)` — this pattern requires the PK class to be defined on the DAC
- If not available in your version, use the BQL `PXSelect` pattern instead:
  ```csharp
  InventoryItem item = PXSelect<InventoryItem,
      Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
      .Select(Base, demand.InventoryID);
  ```

### 10.4 Database Columns

Ensure these database columns exist before deploying:

| Table | Column | Type | Notes |
|-------|--------|------|-------|
| `InventoryItem` | `UsrSWKRTHCost` | `Decimal(19,2)` | Persisted — requires DB column |
| `POFixedDemand` | *(none needed)* | — | `UsrSWKRTHCost` on POFixedDemandExt is unbound (`[PXDecimal]`, not `[PXDBDecimal]`) |

### 10.5 Extension Ordering

If your project has **multiple extensions** on `POCreate`, be aware of execution order:
- Extensions run in the order they are discovered/registered
- If you have a `POCreateReplaceOriginalMethod` extension that overrides `CreateProc`, the Vendor Price calculated by this extension in `EnumerateAndPrepareFixedDemandRow` must populate `EffPrice` **before** `CreateProc` runs
- `EnumerateAndPrepareFixedDemandRow` fires during grid load, which is before the user clicks "Create PO", so ordering should not be an issue

### 10.6 Relationship to POOrderEntry_Extension

The Vendor Price set here (`demand.EffPrice`) is what Acumatica natively uses to set `POLine.CuryUnitCost`. The **POOrderEntry_Extension** then **re-evaluates** the cost when the PO line is inserted/updated using a 4-level hierarchy. These are complementary:

- **PO505000 Vendor Price** (this extension) → sets the **initial** cost value on the demand grid
- **POOrderEntry_Extension** (Section 11 below) → **re-evaluates** the cost on the actual PO line

### 10.7 ExtCost vs CuryExtCost

The code returns `SOLine.CuryExtCost` (currency-specific extended cost). If your system operates in a single currency, `ExtCost` and `CuryExtCost` will be identical. For multi-currency environments, `CuryExtCost` is the correct field as it represents the document currency value.

---

## 11. PO Order Entry — Cost Hierarchy Re-Evaluation (POLine.CuryUnitCost)

> This section covers the **second hierarchy** that runs on the **Purchase Orders (PO301000)** form when a PO line is created or updated. It re-evaluates the Unit Cost on the PO line using a 4-level priority system.

### 11.1 Overview

Once the PO is created from PO505000, Acumatica inserts POLine records. The `POOrderEntry_Extension` intercepts cost defaulting at **two points**:

1. **`POLine_CuryUnitCost_FieldDefaulting`** — fires when a new line is inserted, determines the initial Unit Cost
2. **`EnsureExtCostAndUnitCostFailsafe`** — fires on RowUpdated/RowInserted, re-validates and enforces RTH minimum

Both use the same 4-level hierarchy:

| Priority | Source | Field | Bypasses RTH Floor? | Flag |
|----------|--------|-------|---------------------|------|
| **1st** | SPC Cost | `SOLineExt.UsrSWKSPCCost` (from linked SO line) | **Yes** | `UsrUsedVendorPrice = true` |
| **2nd** | Vendor Price | `APVendorPriceMaint.CalculateUnitCost()` | **Yes** | `UsrUsedVendorPrice = true` |
| **3rd** | RTH Cost | `POLineExt.UsrSWKRTHCost` or `InventoryItemExt.UsrSWKRTHCost` | No (it IS the floor) | `UsrUsedVendorPrice = false` |
| **4th** | Last Cost | `POItemCostManager.Fetch()` | No | `UsrUsedVendorPrice = false` |

### 11.2 Additional DAC Extensions Required

#### POLineExt — Cost-Related Fields

These fields must exist on the `POLine` DAC extension:

```csharp
#region UsrSWKRTHCost
[PXDBDecimal(2)]
[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "RTH Cost", Enabled = false)]
public decimal? UsrSWKRTHCost { get; set; }
public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
#endregion

#region UsrSWKSPCCode
[PXDBString(30, IsUnicode = true)]
[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "SPC Code", Enabled = false)]
public string UsrSWKSPCCode { get; set; }
public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
#endregion

#region UsrUsedVendorPrice
[PXDBBool]
[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "Used Vendor Price", Visible = false, Enabled = false)]
public bool? UsrUsedVendorPrice { get; set; }
public abstract class usrUsedVendorPrice : PX.Data.BQL.BqlBool.Field<usrUsedVendorPrice> { }
#endregion
```

**Database columns required on `POLine` table**:

| Column | Type | Notes |
|--------|------|-------|
| `UsrSWKRTHCost` | `Decimal(19,2)` | RTH Cost copied from InventoryItem on InventoryID change |
| `UsrSWKSPCCode` | `NVarChar(30)` | SPC Code copied from linked SO line |
| `UsrUsedVendorPrice` | `Bit` | Flag: true if SPC or Vendor Price was used (bypasses RTH floor) |

#### SOLineExt — Required Fields

The `SOLine` DAC extension must have:

```csharp
#region UsrSWKSPCCost
[PXDBDecimal(2)]
[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "SPC Cost")]
public decimal? UsrSWKSPCCost { get; set; }
public abstract class usrSWKSPCCost : PX.Data.BQL.BqlDecimal.Field<usrSWKSPCCost> { }
#endregion

#region UsrSWKSPCCode
[PXDBString(30, IsUnicode = true)]
[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "SPC Code")]
public string UsrSWKSPCCode { get; set; }
public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
#endregion
```

### 11.3 File Structure

Create a **separate extension file** for the PO Order Entry cost hierarchy:

```
GraphExt/
  POOrderEntry_CostHierarchy.cs   ← NEW FILE — Cost hierarchy logic for PO301000
```

### 11.4 Graph Extension: `POOrderEntry_CostHierarchy`

```csharp
using PX.Data;
using PX.Objects.AP;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using System;
using System.Linq;
using YourProject.DAC;  // Adjust to your namespace

namespace YourProject.GraphExt  // Adjust to your namespace
{
    /// <summary>
    /// POOrderEntry (PO301000) extension for Unit Cost hierarchy.
    /// 
    /// 4-Level Cost Hierarchy (highest priority first):
    ///   1. SPC Cost      — SOLineExt.UsrSWKSPCCost from linked SO line
    ///   2. Vendor Price   — APVendorPriceMaint.CalculateUnitCost()
    ///   3. RTH Cost       — InventoryItemExt.UsrSWKRTHCost
    ///   4. Last Cost      — POItemCostManager.Fetch()
    /// 
    /// If SPC or Vendor Price is used, RTH floor enforcement is BYPASSED.
    /// </summary>
    public class POOrderEntry_CostHierarchy : PXGraphExtension<PX.Objects.PO.POOrderEntry>
    {
        public static bool IsActive() => true;

        /// <summary>
        /// Skip toggle — set to true to bypass cost hierarchy evaluation.
        /// Used when other code is explicitly setting unit cost (e.g., during LinkPOLineToBlanket).
        /// </summary>
        public bool skipCostDefaulting = false;

        #region Helper: GetDropShipLink

        /// <summary>
        /// Finds the DropShipLink record for a given POLine (if it's a drop-ship line).
        /// </summary>
        public virtual DropShipLink GetDropShipLink(POLine line)
        {
            if (line == null || !POLineType.IsDropShip(line.LineType))
                return null;

            return PXSelect<DropShipLink,
                Where<DropShipLink.pOOrderType, Equal<Required<POLine.orderType>>,
                    And<DropShipLink.pOOrderNbr, Equal<Required<POLine.orderNbr>>,
                    And<DropShipLink.pOLineNbr, Equal<Required<POLine.lineNbr>>>>>>
                .SelectWindowed(Base, 0, 1, line.OrderType, line.OrderNbr, line.LineNbr);
        }

        #endregion

        #region Event: POLine.InventoryID FieldUpdated — Copy RTH Cost to POLine

        /// <summary>
        /// When InventoryID changes on the PO line, copy RTH Cost from InventoryItem to POLineExt.
        /// </summary>
        protected virtual void _(Events.FieldUpdated<POLine, POLine.inventoryID> e)
        {
            if (e.Row == null) return;

            var poLineExt = e.Row.GetExtension<POLineExt>();
            InventoryItem item = InventoryItem.PK.Find(Base, e.Row.InventoryID);
            InventoryItemExt itemExt = item?.GetExtension<InventoryItemExt>();
            poLineExt.UsrSWKRTHCost = itemExt?.UsrSWKRTHCost ?? 0m;
        }

        #endregion

        #region Event: POLine.CuryUnitCost FieldDefaulting — 4-Level Cost Hierarchy

        /// <summary>
        /// Main cost hierarchy for initial unit cost defaulting.
        /// Fires when a new POLine is inserted (during PO creation from PO505000).
        /// 
        /// Priority:
        ///   1. SPC Cost (from linked SO line via multiple lookup strategies)
        ///   2. Vendor Price (APVendorPriceMaint.CalculateUnitCost)
        ///   3. RTH Cost (from POLineExt or InventoryItemExt)
        ///   4. Last Cost (POItemCostManager.Fetch)
        /// </summary>
        protected virtual void POLine_CuryUnitCost_FieldDefaulting(
            PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (skipCostDefaulting) return;

            POLine line = e.Row as POLine;
            POOrder order = Base.Document.Current;
            if (line == null || order == null || !line.InventoryID.HasValue)
                return;

            var lineExt = line.GetExtension<POLineExt>();
            lineExt.UsrUsedVendorPrice = false;

            // ═══════════════════════════════════════════
            // LEVEL 1: SPC Cost (from linked SO line)
            // ═══════════════════════════════════════════
            SOLine soLine = FindLinkedSOLine(line, order);

            if (soLine != null)
            {
                decimal? spc = soLine.GetExtension<SOLineExt>()?.UsrSWKSPCCost;
                if (spc.HasValue && spc.Value > 0m)
                {
                    e.NewValue = spc.Value;
                    e.Cancel = true;
                    lineExt.UsrUsedVendorPrice = true;  // Bypass RTH floor
                    return;
                }
            }

            // ═══════════════════════════════════════════
            // LEVEL 2: Vendor Price
            // ═══════════════════════════════════════════
            if (line.UOM != null && order.VendorID != null && order.CuryInfoID != null)
            {
                try
                {
                    var ci = Base.FindImplementation<IPXCurrencyHelper>()?
                        .GetCurrencyInfo(order.CuryInfoID);
                    decimal? vendCost = APVendorPriceMaint.CalculateUnitCost(
                        sender, order.VendorID, order.VendorLocationID,
                        line.InventoryID, line.SiteID, ci?.GetCM(),
                        line.UOM, line.OrderQty,
                        order.OrderDate ?? Base.Accessinfo.BusinessDate.GetValueOrDefault(),
                        line.CuryUnitCost);

                    if (vendCost.HasValue && vendCost.Value > 0m)
                    {
                        e.NewValue = vendCost.Value;
                        e.Cancel = true;
                        lineExt.UsrUsedVendorPrice = true;  // Bypass RTH floor
                        return;
                    }
                }
                catch { /* Vendor price lookup failure — continue to next level */ }
            }

            // ═══════════════════════════════════════════
            // LEVEL 3: RTH Cost
            // ═══════════════════════════════════════════
            decimal? rth = lineExt?.UsrSWKRTHCost;
            if (!rth.HasValue || rth.Value <= 0m)
            {
                InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
                rth = item?.GetExtension<InventoryItemExt>()?.UsrSWKRTHCost;
            }
            if (rth.HasValue && rth.Value > 0m)
            {
                e.NewValue = rth.Value;
                e.Cancel = true;
                return;
            }

            // ═══════════════════════════════════════════
            // LEVEL 4: Last Cost (Acumatica standard fallback)
            // ═══════════════════════════════════════════
            e.NewValue = POItemCostManager.Fetch<POLine.inventoryID, POLine.curyInfoID>(
                sender.Graph, line,
                order.VendorID, order.VendorLocationID,
                order.OrderDate, order.CuryID,
                line.InventoryID, line.SubItemID, line.SiteID, line.UOM);
            APVendorPriceMaint.CheckNewUnitCost<POLine, POLine.curyUnitCost>(
                sender, line, e.NewValue);
        }

        #endregion

        #region Helper: FindLinkedSOLine — Multi-Strategy SO Line Lookup

        /// <summary>
        /// Attempts to find the linked SOLine using multiple strategies.
        /// When multiple SO lines match (merged lines), picks the one with the highest SPC cost.
        /// 
        /// Strategies (in order):
        ///   1. DropShipLink — direct link for existing drop-ship PO lines
        ///   2. SOLineSplit (Drop-Ship) — match by InventoryID/SiteID/VendorID for drop-ship during insert
        ///   3. SOLineSplit (Linked) — match by PO reference for regular "Mark for PO" lines
        ///   4. SOLineSplit (Pending) — match by criteria for not-yet-linked regular lines
        /// </summary>
        protected virtual SOLine FindLinkedSOLine(POLine line, POOrder order)
        {
            SOLine soLine = null;

            // Strategy 1: DropShipLink (works for existing drop-ship PO lines)
            DropShipLink ds = GetDropShipLink(line);
            if (ds != null)
            {
                soLine = PXSelect<SOLine,
                    Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                        And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                        And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                    .Select(Base, ds.SOOrderType, ds.SOOrderNbr, ds.SOLineNbr);
            }

            // Strategy 2: SOLineSplit for drop-ship during Insert
            if (soLine == null && POLineType.IsDropShip(line.LineType))
            {
                var matches = PXSelectJoin<SOLine,
                    InnerJoin<SOLineSplit,
                        On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                        And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                        And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
                    Where<SOLineSplit.pOCreate, Equal<True>,
                        And<SOLineSplit.inventoryID, Equal<Required<SOLineSplit.inventoryID>>,
                        And<SOLineSplit.siteID, Equal<Required<SOLineSplit.siteID>>,
                        And<SOLineSplit.vendorID, Equal<Required<SOLineSplit.vendorID>>>>>>>
                    .Select(Base, line.InventoryID, line.SiteID, order?.VendorID)
                    .RowCast<SOLine>().ToList();

                if (matches.Any())
                {
                    soLine = matches
                        .OrderByDescending(so => so.GetExtension<SOLineExt>()?.UsrSWKSPCCost ?? 0m)
                        .First();
                }
            }

            // Strategy 3: SOLineSplit for regular "Mark for PO" (linked)
            if (soLine == null && !POLineType.IsDropShip(line.LineType))
            {
                if (line.OrderType != null && line.OrderNbr != null && line.LineNbr != null)
                {
                    var linked = PXSelectJoin<SOLine,
                        InnerJoin<SOLineSplit,
                            On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                            And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                            And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
                        Where<SOLineSplit.pOType, Equal<Required<SOLineSplit.pOType>>,
                            And<SOLineSplit.pONbr, Equal<Required<SOLineSplit.pONbr>>,
                            And<SOLineSplit.pOLineNbr, Equal<Required<SOLineSplit.pOLineNbr>>>>>>
                        .Select(Base, line.OrderType, line.OrderNbr, line.LineNbr)
                        .RowCast<SOLine>().ToList();

                    if (linked.Any())
                    {
                        soLine = linked
                            .OrderByDescending(so => so.GetExtension<SOLineExt>()?.UsrSWKSPCCost ?? 0m)
                            .First();
                    }
                }

                // Strategy 4: SOLineSplit for pending (not yet linked) regular lines
                if (soLine == null && line.InventoryID != null && line.SiteID != null && order?.VendorID != null)
                {
                    var pending = PXSelectJoin<SOLine,
                        InnerJoin<SOLineSplit,
                            On<SOLine.orderType, Equal<SOLineSplit.orderType>,
                            And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
                            And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>,
                        Where<SOLineSplit.pOCreate, Equal<True>,
                            And<SOLineSplit.inventoryID, Equal<Required<SOLineSplit.inventoryID>>,
                            And<SOLineSplit.siteID, Equal<Required<SOLineSplit.siteID>>,
                            And<SOLineSplit.vendorID, Equal<Required<SOLineSplit.vendorID>>,
                            And<SOLineSplit.pONbr, IsNull>>>>>>
                        .Select(Base, line.InventoryID, line.SiteID, order.VendorID)
                        .RowCast<SOLine>().ToList();

                    if (pending.Any())
                    {
                        soLine = pending
                            .OrderByDescending(so => so.GetExtension<SOLineExt>()?.UsrSWKSPCCost ?? 0m)
                            .First();
                    }
                }
            }

            return soLine;
        }

        #endregion

        #region EnsureExtCostAndUnitCostFailsafe — Post-Insert/Update Validation

        /// <summary>
        /// Re-validates unit cost and extended cost after the PO line is inserted or updated.
        /// Uses the same 4-level hierarchy as FieldDefaulting.
        /// 
        /// Additional enforcement:
        ///   - If SPC or Vendor Price was used → only ensure non-negative
        ///   - If RTH or Last Cost was used → enforce RTH minimum floor on both UnitCost and ExtCost
        ///   - Always recalculates ExtCost = UnitCost × Qty (failsafe)
        /// 
        /// Called from RowInserted/RowUpdated events on POLine.
        /// </summary>
        private void EnsureExtCostAndUnitCostFailsafe(PXCache cache, POLine line, bool raiseErrors)
        {
            if (line == null) return;

            var order = Base.Document.Current;
            var qty = line.OrderQty ?? 0m;
            var unitCost = line.CuryUnitCost ?? 0m;
            var ext = line.CuryExtCost ?? 0m;
            var lineExt = line.GetExtension<POLineExt>();
            var rthUnit = lineExt?.UsrSWKRTHCost ?? 0m;

            // ═══════════════════════════════════════════
            // Re-evaluate cost hierarchy (same 4 levels)
            // Only if NOT explicitly skipping cost defaulting
            // ═══════════════════════════════════════════
            if (!skipCostDefaulting)
            {
                decimal? expectedUnitCost = null;
                bool expectedIsVendorOrSPC = false;

                // 1) SPC Cost — same multi-strategy lookup as FieldDefaulting
                try
                {
                    SOLine soLine = FindLinkedSOLine(line, order);
                    if (soLine != null)
                    {
                        var soLineExt = soLine.GetExtension<SOLineExt>();
                        if (soLineExt?.UsrSWKSPCCost > 0m)
                        {
                            expectedUnitCost = soLineExt.UsrSWKSPCCost;
                            expectedIsVendorOrSPC = true;
                        }
                    }
                }
                catch { }

                // 2) Vendor Price
                if (!expectedUnitCost.HasValue && line.UOM != null
                    && order?.VendorID != null && order?.CuryInfoID != null)
                {
                    try
                    {
                        var ci = Base.FindImplementation<IPXCurrencyHelper>()?
                            .GetCurrencyInfo(order.CuryInfoID);
                        var vendCost = APVendorPriceMaint.CalculateUnitCost(
                            cache, order.VendorID, order.VendorLocationID,
                            line.InventoryID, line.SiteID, ci?.GetCM(),
                            line.UOM, qty,
                            order.OrderDate ?? Base.Accessinfo.BusinessDate.GetValueOrDefault(),
                            line.CuryUnitCost);
                        if (vendCost.HasValue && vendCost.Value > 0m)
                        {
                            expectedUnitCost = vendCost.Value;
                            expectedIsVendorOrSPC = true;
                        }
                    }
                    catch { }
                }

                // 3) RTH Cost
                if (!expectedUnitCost.HasValue)
                {
                    decimal rth = lineExt?.UsrSWKRTHCost ?? 0m;
                    if (rth <= 0m && line.InventoryID.HasValue)
                    {
                        InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);
                        rth = item?.GetExtension<InventoryItemExt>()?.UsrSWKRTHCost ?? 0m;
                    }
                    if (rth > 0m)
                    {
                        expectedUnitCost = rth;
                    }
                }

                // 4) Last Cost
                if (!expectedUnitCost.HasValue)
                {
                    expectedUnitCost = POItemCostManager.Fetch<POLine.inventoryID, POLine.curyInfoID>(
                        cache.Graph, line,
                        order?.VendorID, order?.VendorLocationID,
                        order?.OrderDate, order?.CuryID,
                        line.InventoryID, line.SubItemID, line.SiteID, line.UOM);
                }

                // Apply if different
                if (expectedUnitCost.HasValue && Math.Abs(unitCost - expectedUnitCost.Value) > 0.0000001m)
                {
                    cache.SetValueExt<POLine.curyUnitCost>(line, expectedUnitCost.Value);
                    unitCost = expectedUnitCost.Value;
                }

                // Track flag for RTH enforcement
                lineExt.UsrUsedVendorPrice = expectedIsVendorOrSPC;
            }

            // ═══════════════════════════════════════════
            // RTH Floor Enforcement
            // ═══════════════════════════════════════════
            if (lineExt?.UsrUsedVendorPrice == true)
            {
                // SPC/Vendor price used → only ensure non-negative
                if (unitCost < 0m)
                {
                    cache.SetValueExt<POLine.curyUnitCost>(line, 0m);
                    unitCost = 0m;
                }
            }
            else
            {
                // RTH/Last Cost path → enforce RTH minimum on UnitCost
                if (rthUnit > 0m && unitCost < rthUnit)
                {
                    cache.SetValueExt<POLine.curyUnitCost>(line, rthUnit);
                    unitCost = rthUnit;
                }
            }

            // ═══════════════════════════════════════════
            // Failsafe: Recalculate ExtCost = UnitCost × Qty
            // ═══════════════════════════════════════════
            var expectedExt = Math.Round(unitCost * qty, 2, MidpointRounding.AwayFromZero);
            if (Math.Abs(ext - expectedExt) > 0.009m || line.CuryExtCost == null)
            {
                cache.SetValueExt<POLine.curyExtCost>(line, expectedExt);
                ext = expectedExt;
            }

            // Enforce RTH minimum on ExtCost (only if vendor/SPC not used)
            if (lineExt?.UsrUsedVendorPrice != true)
            {
                var rthMin = Math.Round((lineExt?.UsrSWKRTHCost ?? 0m) * qty, 2, MidpointRounding.AwayFromZero);
                if (ext + 0.009m < rthMin)
                {
                    if (raiseErrors)
                    {
                        throw new PXSetPropertyException("Extended Cost cannot be below RTH minimum.");
                    }
                    cache.SetValueExt<POLine.curyExtCost>(line, rthMin);
                }
            }
        }

        #endregion
    }
}
```

### 11.5 SPC Cost Lookup — Multi-Strategy Detail

The SPC Cost lookup uses **4 strategies** in priority order to find the linked SO line. This handles various scenarios: existing drop-ship lines, new inserts, regular "Mark for PO" lines, and merged lines.

```
Strategy 1: DropShipLink
   └── Direct table link: DropShipLink → SOLine
   └── Works for: Existing drop-ship PO lines

Strategy 2: SOLineSplit (Drop-Ship, by criteria)
   └── Match: InventoryID + SiteID + VendorID + POCreate=True
   └── Works for: Drop-ship during Insert (before DropShipLink exists)
   └── Picks: Highest UsrSWKSPCCost (for merged lines)

Strategy 3: SOLineSplit (Linked, by PO reference)
   └── Match: SOLineSplit.POType/PONbr/POLineNbr = current POLine
   └── Works for: Regular "Mark for PO" lines already linked
   └── Picks: Highest UsrSWKSPCCost (for merged lines)

Strategy 4: SOLineSplit (Pending)
   └── Match: InventoryID + SiteID + VendorID + POCreate=True + PONbr IS NULL
   └── Works for: Regular lines not yet linked to any PO
   └── Picks: Highest UsrSWKSPCCost (for merged lines)
```

### 11.6 RTH Floor Enforcement Rules

| Scenario | UnitCost Rule | ExtCost Rule |
|----------|---------------|--------------|
| SPC or Vendor Price used (`UsrUsedVendorPrice = true`) | Must be ≥ 0 (non-negative only) | No RTH floor check |
| RTH or Last Cost used (`UsrUsedVendorPrice = false`) | Must be ≥ `UsrSWKRTHCost` | Must be ≥ `UsrSWKRTHCost × Qty` |

### 11.7 skipCostDefaulting Flag

The `skipCostDefaulting` boolean is used to temporarily disable the cost hierarchy. Set it to `true` before programmatically setting `CuryUnitCost` to prevent the hierarchy from overwriting your value:

```csharp
// Example usage in LinkPOLineToBlanket or other override:
var costExt = docgraph.GetExtension<POOrderEntry_CostHierarchy>();
if (costExt != null)
    costExt.skipCostDefaulting = true;

// ... set unit cost explicitly ...

costExt.skipCostDefaulting = false;  // Re-enable after
```

### 11.8 Data Flow — Combined PO505000 + PO301000

```
┌─────────────────────────────────────────────────────────┐
│  PO505000 (Create Purchase Orders)                       │
│                                                          │
│  EnumerateAndPrepareFixedDemandRow                       │
│    │                                                     │
│    ├── Plan6D/6E/66? → SOLine.CuryExtCost → EffPrice    │
│    └── Other?         → InventoryItem.UsrSWKRTHCost      │
│                         → EffPrice                       │
│                                                          │
│  User edits EffPrice (optional)                          │
│  User clicks "Create PO"                                 │
└────────────────┬────────────────────────────────────────┘
                 │    demand.EffPrice
                 ▼
┌─────────────────────────────────────────────────────────┐
│  PO301000 (Purchase Orders) — POOrderEntry               │
│                                                          │
│  POLine_CuryUnitCost_FieldDefaulting                     │
│    │                                                     │
│    ├── 1) SPC Cost  (SOLineExt.UsrSWKSPCCost)            │
│    ├── 2) Vendor Price (APVendorPriceMaint)               │
│    ├── 3) RTH Cost  (InventoryItemExt.UsrSWKRTHCost)     │
│    └── 4) Last Cost (POItemCostManager.Fetch)             │
│                                                          │
│  EnsureExtCostAndUnitCostFailsafe (post-insert)          │
│    │                                                     │
│    ├── Re-evaluate same 4-level hierarchy                 │
│    ├── Set UsrUsedVendorPrice flag                        │
│    ├── Enforce RTH floors (if not vendor/SPC)             │
│    └── Recalculate ExtCost = UnitCost × Qty              │
└─────────────────────────────────────────────────────────┘
```

---

## Appendix A: Complete File Templates

- **PO505000 extension**: Section [5.1](#51-new-file-pocreate_vendorpricecs) — `POCreate_VendorPrice.cs`
- **PO301000 extension**: Section [11.4](#114-graph-extension-poorderentry_costhierarchy) — `POOrderEntry_CostHierarchy.cs`

Copy each, adjust the namespace, and add to your project as separate files.

## Appendix B: All Database Columns Required

| Table | Column | Type | Persisted? |
|-------|--------|------|------------|
| `InventoryItem` | `UsrSWKRTHCost` | `Decimal(19,2)` | Yes |
| `POLine` | `UsrSWKRTHCost` | `Decimal(19,2)` | Yes |
| `POLine` | `UsrSWKSPCCode` | `NVarChar(30)` | Yes |
| `POLine` | `UsrUsedVendorPrice` | `Bit` | Yes |
| `SOLine` | `UsrSWKSPCCost` | `Decimal(19,2)` | Yes |
| `SOLine` | `UsrSWKSPCCode` | `NVarChar(30)` | Yes |
| `POFixedDemand` | *(none)* | — | No (unbound field only) |

## Appendix C: Original Source Files (24R1 Reference)

| File | Contains |
|------|----------|
| `GraphExt/POCreate4.cs` | `POCreateExt` — `EnumerateAndPrepareFixedDemandRow`, `CalculateVendorPriceFromPlanType`, `GetSOLineUnitCostFromExtCost`, CacheAttached, FieldUpdated, RowSelecting, RowSelected |
| `GraphExt/POOrderEntry_Extension.cs` | `POOrderEntry_Extension` — `EnsureExtCostAndUnitCostFailsafe`, `POLine_CuryUnitCost_FieldDefaulting`, RTH enforcement, SPC/Vendor lookup strategies |
| `DACExt/POFixedDemandExt.cs` | `UsrSWKRTHCost` unbound field, plus Vendor, SPC fields |
| `DACExt/POLineExt.cs` | `UsrSWKRTHCost`, `UsrSWKSPCCode`, `UsrUsedVendorPrice` persisted fields |
| `DACExt/InventoryItemExt.cs` | `UsrSWKRTHCost` persisted field |
| `GraphExt/POCreateReplaceOriginalMethod.cs` | Full `CreateProc` override (PO creation process) |

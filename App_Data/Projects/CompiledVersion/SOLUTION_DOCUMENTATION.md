# Complete Cost Override Solution

## Problem Statement
`INTran.UnitCost` was showing 250 (from inventory standard cost) instead of 111 (from `SOLine.CuryUnitCost`).

## Root Cause Analysis

### Issue 1: INTran.UnitCost Default Attribute
The `INTran.UnitCost` field in the DAC has a `[PXDefault]` attribute that automatically queries `INItemSite.tranUnitCost`:

```csharp
[PXDefault(TypeCode.Decimal, "0.0", typeof(Coalesce<
    Search<INItemSite.tranUnitCost, ...>,
    Search<INItemCost.tranUnitCost, ...>>))]
```

This happens during `FieldDefaulting` event, which fires **before** our `RowInserted` event.

### Issue 2: DefaultUnitCost Method
`INRegisterEntryBase.DefaultUnitCost()` method is called during various field updates and recalculates the cost from inventory settings, overwriting our custom values.

## Complete Solution

The solution requires **THREE** extension files working together:

### 1. SOShipmentEntry_Extension.cs
**Purpose**: Ensures correct costs flow from `SOLine` to `SOShipLine` and then to `INTran`

**Key Methods**:
- `RowPersisting<SOShipLine>` - Sets `SOShipLine.UnitCost` from `SOLine.CuryUnitCost`
- `FieldDefaulting<SOShipLine.unitCost>` - Provides correct default when line is created
- `GetINTranUnitCost()` override - Returns `SOLine.CuryUnitCost` instead of inventory cost
- `RowPersisting<INTran>` - Final safeguard with `OverrideUnitCost = true`

**File Location**: `App_Data\Projects\CompiledVersion\CompiledVersion\GraphExt\SOShipmentEntry_Extension.cs`

### 2. INIssueEntry_Extension.cs (NEW)
**Purpose**: Prevents INIssue documents from recalculating costs for SO-related transactions

**Key Methods**:
- `FieldDefaulted<INTran.unitCost>` - Overrides the default value **immediately after** it's set
- `RowInserted<INTran>` - Sets `OverrideUnitCost = true` and correct costs
- `RowPersisting<INTran>` - Final safeguard before database save
- `DefaultUnitCost()` override - **Critical**: Prevents `INRegisterEntryBase.DefaultUnitCost()` from recalculating

**File Location**: `App_Data\Projects\CompiledVersion\CompiledVersion\GraphExt\INIssueEntry_Extension.cs`

## Data Flow

```
Sales Order
?? SOLine.CuryUnitCost = 111.00 ?
    ?
Shipment Created
?? FieldDefaulting<SOShipLine.unitCost> fires
?   ?? Returns 111.00 (from SOLine.CuryUnitCost) ?
?? RowPersisting<SOShipLine> fires
?   ?? SOShipLine.UnitCost = 111.00 ?
    ?
Update IN Button Clicked
?? PostShipment() called in SOShipmentEntry
?? GetINTranUnitCost() override fires
?   ?? Returns 111.00 (from SOLine.CuryUnitCost) ?
?? newline.UnitCost = 111.00
?? newline = docgraph.Insert(newline)
  ?
INIssueEntry Processing
?? FieldDefaulting<INTran.unitCost> fires
?   ?? Base: Would return 250 from INItemSite ?
?   ?? Override: Returns 111.00 from SOLine ?
?? RowInserted<INTran> fires
?   ?? tran.OverrideUnitCost = true
?   ?? Confirms UnitCost = 111.00 ?
?? DefaultUnitCost() called (from various events)
?   ?? Base: Would recalculate from inventory ?
?   ?? Override: Skips for SO transactions ?
?? RowPersisting<INTran> fires (FINAL SAFEGUARD)
?   ?? tran.OverrideUnitCost = true
?   ?? tran.UnitCost = 111.00 ?
?   ?? tran.TranCost = 111.00 ?
    ?
Database Persist
?? INTran.UnitCost = 111.00 ? **SUCCESS!**
```

## Key Technical Points

### 1. Event Order
```
1. FieldDefaulting  ? INIssueEntry_Extension intercepts HERE
2. RowInserted      ? Sets OverrideUnitCost flag
3. FieldUpdated     ? May call DefaultUnitCost
4. RowPersisting ? Final safeguard
5. Database Save
```

### 2. The OverrideUnitCost Flag
- **Type**: Unbound field (`[PXBool]` not `[PXDBBool]`)
- **Purpose**: Runtime flag to prevent recalculation
- **Limitation**: Not persisted to database, only affects current session
- **Usage**: Set to `true` in all our event handlers

### 3. DefaultUnitCost Override
**Critical**: This prevents the base class method from running:
```csharp
[PXOverride]
public virtual void DefaultUnitCost(PXCache cache, INTran tran, bool setZero, DefaultUnitCostDelegate baseMethod)
{
    if (!string.IsNullOrEmpty(tran.SOOrderType) && !string.IsNullOrEmpty(tran.SOOrderNbr))
        return; // Skip recalculation for SO transactions
    
    baseMethod(cache, tran, setZero); // Normal processing for other transactions
}
```

## Testing Checklist

- [ ] Create Sales Order with `SOLine.CuryUnitCost` = 111.00
- [ ] Create Shipment from order
- [ ] Verify `SOShipLine.UnitCost` = 111.00
- [ ] Click "Update IN" button
- [ ] Open created INIssue document
- [ ] Verify `INTran.UnitCost` = 111.00
- [ ] Release the INIssue document
- [ ] Query database: `SELECT UnitCost FROM INTran WHERE ...`
- [ ] Confirm database value = 111.00

## Troubleshooting

### If cost is still wrong:
1. Check if both extension files are active (`IsActive() => true`)
2. Add breakpoints in all event handlers
3. Use trace to see event order
4. Check if `SOLine.CuryUnitCost` has correct value
5. Verify `OverrideUnitCost` flag is being set

### Debug Queries
```sql
-- Check SOLine cost
SELECT OrderType, OrderNbr, LineNbr, CuryUnitCost 
FROM SOLine 
WHERE OrderType = 'SO' AND OrderNbr = '000123'

-- Check SOShipLine cost
SELECT ShipmentNbr, LineNbr, UnitCost 
FROM SOShipLine 
WHERE ShipmentNbr = '000456'

-- Check INTran cost
SELECT DocType, RefNbr, LineNbr, UnitCost, SOOrderType, SOOrderNbr 
FROM INTran 
WHERE RefNbr = '000789'
```

## Files Modified/Created

1. **Created**: `App_Data\Projects\CompiledVersion\CompiledVersion\GraphExt\INIssueEntry_Extension.cs`
   - New extension to handle INIssue cost overrides

2. **Modified**: `App_Data\Projects\CompiledVersion\CompiledVersion\GraphExt\SOShipmentEntry_Extension.cs`
   - Added `GetINTranUnitCost` override
   - Enhanced `RowPersisting<INTran>` with `OverrideUnitCost` flag

## Version History

- **v1.0**: Initial SOShipmentEntry_Extension with RowPersisting handlers
- **v1.1**: Added GetINTranUnitCost override
- **v1.2**: Added OverrideUnitCost flag usage
- **v2.0**: Created INIssueEntry_Extension with DefaultUnitCost override (FINAL FIX)

## Success Criteria

? INTran.UnitCost = SOLine.CuryUnitCost (111.00)
? No recalculation from inventory cost (250.00)
? Persists correctly to database
? Works for returns with lot/serial numbers
? Doesn't affect non-SO transactions

# Migration Notes - January 6, 2026

## Summary
Relocated and enhanced Not-To-Exceed (NTE) freight validation from PO Order save to PO Receipt and SO Shipment save events.

---

## Changes Made

### 1. POOrderEntry_Extension.cs - REMOVED NTE Validation
**File:** `App_Data\Projects\CompiledVersion\CompiledVersion\GraphExt\POOrderEntry_Extension.cs`

**Removed:**
- `RowPersisting<POOrder>` event handler for NTE validation
- `GetLinkedSOForNTEValidation()` helper method
- `CalculateTotalPOAmountForSO()` helper method
- Entire `#region NTE Validation` section

**Reason:** NTE validation was moved to PO Receipt save for more accurate freight cost tracking at the time of actual receipt.

---

### 2. POReceiptEntry_Extension.cs - ADDED NTE Freight Validation
**File:** `App_Data\Projects\CompiledVersion\CompiledVersion\GraphExt\POReceiptEntry_Extension.cs`

**Added:**
- `RowPersisting<POReceipt>` event handler for NTE freight validation
- `GetLinkedSOForDropShipReceipt()` helper method - retrieves linked SO via receipt line or DropShipLink
- `CalculateTotalFreightCostForSO()` helper method - sums `UsrFreightCost` from all receipts for SO's Drop-Ship POs

**Behavior:**
- Only validates Drop-Ship PO receipts (`POOrderType.DropShip`)
- Checks if receipt has `UsrFreightCost > 0`
- Gets linked Sales Order and checks for NTE ship terms (`SOSetupExt.UsrNotToExceed`)
- Validates total freight against `SOOrderExt.UsrFreightPriceLimit`
- Raises error or warning based on `SOSetupExt.UsrEnforcePONTE` setting

---

### 3. SOShipmentEntry_Extension.cs - ADDED NTE Freight Validation
**File:** `App_Data\Projects\CompiledVersion\CompiledVersion\GraphExt\SOShipmentEntry_Extension.cs`

**Added:**
- `RowPersisting<SOShipment>` event handler for NTE freight validation
- `GetLinkedSOForShipment()` helper method - retrieves linked SO from OrderList or SOOrderShipment
- `CalculateTotalFreightCostForSOShipments()` helper method - sums `CuryFreightAmt` from all finalized shipments

**Behavior:**
- Checks if shipment has `CuryFreightAmt > 0`
- Gets linked Sales Order and checks for NTE ship terms
- Calculates total freight across shipments with statuses: Confirmed, Invoiced, PartiallyInvoiced, Completed
- Validates total against `SOOrderExt.UsrFreightPriceLimit`
- Raises error or warning based on `SOSetupExt.UsrEnforcePONTE` setting

---

### 4. Messages.cs - ADDED New Message Constants
**File:** `App_Data\Projects\CompiledVersion\CompiledVersion\Messages\Messages.cs`

**Added:**
```csharp
// PO Receipt freight validation message
private const string _poReceiptFreightExceedsNTE = "Total freight cost for Sales Order {0} ({1:C}) exceeds Not-To-Exceed limit ({2:C}) by {3:C}.";
public static string POReceiptFreightExceedsNTE(string soOrderNbr, decimal? totalFreight, decimal? nteLimit, decimal? exceedAmt)

// SO Shipment freight validation message
private const string _shipmentFreightExceedsNTE = "Total shipment freight for Sales Order {0} ({1:C}) exceeds Not-To-Exceed limit ({2:C}) by {3:C}.";
public static string ShipmentFreightExceedsNTE(string soOrderNbr, decimal? totalFreight, decimal? nteLimit, decimal? exceedAmt)
```

---

## Configuration Dependencies

| Field | Location | Description |
|-------|----------|-------------|
| `UsrNotToExceed` | SOSetupExt | Ship Terms ID that triggers NTE validation |
| `UsrFreightPriceLimit` | SOOrderExt | NTE freight limit amount on Sales Order |
| `UsrEnforcePONTE` | SOSetupExt | Toggle: `true` = Error (block save), `false` = Warning only |
| `UsrFreightCost` | POReceiptExt | Freight cost field on PO Receipt |

---

## Testing Checklist

- [ ] Create a Sales Order with NTE ship terms and set `UsrFreightPriceLimit`
- [ ] Create Drop-Ship PO from the Sales Order
- [ ] Create PO Receipt with `UsrFreightCost` exceeding the limit - verify error/warning
- [ ] Create multiple PO Receipts and verify cumulative freight validation
- [ ] Create SO Shipment with `CuryFreightAmt` exceeding limit - verify error/warning
- [ ] Test with `UsrEnforcePONTE = true` (hard stop) and `false` (warning only)
- [ ] Verify PO Order save no longer triggers NTE validation

---

## Rollback Instructions

To rollback these changes:
1. Restore the `#region NTE Validation` section in `POOrderEntry_Extension.cs`
2. Remove the `#region NTE Freight Validation for Drop-Ship PO Receipts` section from `POReceiptEntry_Extension.cs`
3. Remove the `#region NTE Freight Validation for SO Shipments` section from `SOShipmentEntry_Extension.cs`
4. Remove `ShipmentFreightExceedsNTE` and `POReceiptFreightExceedsNTE` messages from `Messages.cs`

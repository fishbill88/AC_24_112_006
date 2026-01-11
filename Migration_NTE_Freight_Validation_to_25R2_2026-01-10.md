# Migration Guide: NTE Freight Validation to 25R2

**Source Version:** 24.112.006  
**Target Version:** 2025 R2  
**Date:** January 10, 2026  
**Feature:** Not-To-Exceed (NTE) Freight Validation Enhancement

---

## Executive Summary

This migration relocates NTE freight validation from PO Order save events to PO Receipt and SO Shipment save events for more accurate freight cost tracking at the point of actual transaction. The enhancement provides:

- **Real-time validation** at receipt/shipment rather than order creation
- **Unified freight aggregation** across drop-ship and regular shipments
- **Configurable enforcement** (Error blocks save / Warning allows save)
- **Comprehensive freight tracking** using `SOOrderShipment` as single source of truth

---

## Table of Contents

1. [Overview](#overview)
2. [Files to Modify](#files-to-modify)
3. [Detailed Changes](#detailed-changes)
4. [Configuration Requirements](#configuration-requirements)
5. [Testing Guide](#testing-guide)
6. [Rollback Plan](#rollback-plan)

---

## Overview

### Business Problem
Original NTE validation at PO Order level was premature - actual freight costs are only known when receipts are processed. This caused:
- Inaccurate validations based on estimated amounts
- No validation on regular SO shipments
- Inconsistent freight aggregation logic

### Solution
Move validation to actual transaction events:
- **PO Receipt save**: Validates drop-ship freight costs
- **SO Shipment save**: Validates regular shipment freight costs
- **Unified calculation**: Both use `SOOrderShipment` table as single source

### Benefits
- ✅ Accurate validation at time of actual freight cost
- ✅ Handles mixed scenarios (drop-ship + regular shipments)
- ✅ Consistent freight aggregation across all screens
- ✅ Better user experience with configurable error/warning mode

---

## Files to Modify

| File | Type | Lines Changed | Complexity |
|------|------|---------------|------------|
| [POOrderEntry_Extension.cs](#1-poorderentry_extensioncs) | GraphExt | -136 (removed) | Low |
| [POReceiptEntry_Extension.cs](#2-poreceiptentry_extensioncs) | GraphExt | +188 (added) | High |
| [SOShipmentEntry_Extension.cs](#3-soshipmententry_extensioncs) | GraphExt | +173 (added) | High |
| [SOOrderEntry_Extension.cs](#4-soorderentry_extensioncs) | GraphExt | ~48 (modified) | Medium |
| [Messages.cs](#5-messagescs) | Messages | +25 (added) | Low |

---

## Detailed Changes

### 1. POOrderEntry_Extension.cs

**Action:** **REMOVE** entire NTE validation section

#### Location
Around line 1223, find and **DELETE** the entire `#region NTE Validation` block (approximately 136 lines).

#### Code to Remove
```csharp
#region NTE Validation
/// <summary>
/// Validates that drop-ship PO totals do not exceed the Not-To-Exceed limit defined on the originating Sales Order.
/// </summary>
protected virtual void _(Events.RowPersisting<POOrder> e)
{
    // ... entire method
}

private SOOrder GetLinkedSOForNTEValidation(POOrder poOrder)
{
    // ... entire method
}

private decimal? CalculateTotalPOAmountForSO(SOOrder soOrder, POOrder currentPO)
{
    // ... entire method
}
#endregion
```

#### Why Remove?
This validation was premature - PO orders don't have actual freight costs yet. The validation is now performed at PO Receipt save where actual costs are known.

---

### 2. POReceiptEntry_Extension.cs

**Action:** **ADD** new NTE validation for drop-ship receipts

#### Step 1: Add Using Statements
At the top of the file, after existing using statements:

```csharp
using PX.Objects.Common.DAC;
using PX.Objects.SO;
using System.Collections.Generic;
using System.Linq;
```

#### Step 2: Add SOSetup View
After the `IsActive()` method, add:

```csharp
public PXSetup<SOSetup> sosetup;
```

#### Step 3: Add NTE Validation Region
At the end of the class (before the closing brace), add the entire region:

```csharp
#region NTE Freight Validation for Drop-Ship PO Receipts
/// <summary>
/// Validates that the total freight cost across all PO receipts for a Drop-Ship PO
/// does not exceed the Not-To-Exceed limit defined on the linked Sales Order.
/// </summary>
protected virtual void _(Events.RowPersisting<POReceipt> e)
{
    if (e.Row == null) return;
    if (e.Operation == PXDBOperation.Delete) return;

    POReceipt receipt = e.Row;
    POReceiptExt receiptExt = receipt.GetExtension<POReceiptExt>();

    // Skip if no freight price on this receipt
    decimal currentFreightPrice = receiptExt?.UsrFreightPrice ?? 0m;
    if (currentFreightPrice <= 0m) return;

    // Check if this receipt is for a Drop-Ship PO
    var linkedSO = GetLinkedSOForDropShipReceipt(receipt);
    if (linkedSO == null) return; // Not a drop-ship receipt or no SO link

    SOOrderExt soExt = linkedSO.GetExtension<SOOrderExt>();
    SOSetupExt setupExt = sosetup.Current?.GetExtension<SOSetupExt>();

    // Check if this SO has NTE shipping terms
    if (linkedSO.ShipTermsID != setupExt?.UsrNotToExceed) return;

    // Check if SO has NTE limit set
    decimal? nteLimit = soExt?.UsrFreightPriceLimit;
    if (nteLimit == null || nteLimit <= 0m) return;

    // Calculate total freight price across ALL shipments/receipts for this SO
    decimal? totalFreightPrice = CalculateTotalFreightPriceForSO(linkedSO, receipt, currentFreightPrice);

    // Validate against limit
    if (totalFreightPrice > nteLimit)
    {
        decimal? exceedAmt = totalFreightPrice - nteLimit;
        string errorMsg = Messages.POReceiptFreightExceedsNTE(
            $"{linkedSO.OrderType}-{linkedSO.OrderNbr}",
            totalFreightPrice,
            nteLimit,
            exceedAmt
        );

        // Use setup toggle to determine error level
        PXErrorLevel errorLevel = (setupExt?.UsrEnforcePONTE == true) 
            ? PXErrorLevel.Error 
            : PXErrorLevel.Warning;

        e.Cache.RaiseExceptionHandling<POReceiptExt.usrFreightPrice>(
            receipt, 
            currentFreightPrice, 
            new PXSetPropertyException(errorMsg, errorLevel)
        );

        // If hard stop mode, throw exception to block save
        if (errorLevel == PXErrorLevel.Error)
        {
            throw new PXException(errorMsg);
        }
    }
}

/// <summary>
/// Retrieves the linked Sales Order for NTE validation from Drop-Ship PO Receipt.
/// </summary>
private SOOrder GetLinkedSOForDropShipReceipt(POReceipt receipt)
{
    if (receipt == null) return null;

    // Get receipt lines to find the linked Drop-Ship PO and SO
    var receiptLine = PXSelect<POReceiptLine,
        Where<POReceiptLine.receiptType, Equal<Required<POReceiptLine.receiptType>>,
            And<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>>>>
        .SelectWindowed(Base, 0, 1, receipt.ReceiptType, receipt.ReceiptNbr)
        .RowCast<POReceiptLine>()
        .FirstOrDefault();

    if (receiptLine == null) return null;

    // Check if linked to a Drop-Ship PO
    if (string.IsNullOrEmpty(receiptLine.POType) || receiptLine.POType != POOrderType.DropShip)
        return null;

    // Get the linked SO Order
    // First try from the receipt line's SO fields
    if (!string.IsNullOrEmpty(receiptLine.SOOrderType) && !string.IsNullOrEmpty(receiptLine.SOOrderNbr))
    {
        return PXSelect<SOOrder,
            Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
            .Select(Base, receiptLine.SOOrderType, receiptLine.SOOrderNbr);
    }

    // Fallback: Get from DropShipLink via the PO
    DropShipLink link = PXSelect<DropShipLink,
        Where<DropShipLink.pOOrderType, Equal<Required<POReceiptLine.pOType>>,
            And<DropShipLink.pOOrderNbr, Equal<Required<POReceiptLine.pONbr>>>>>
        .SelectWindowed(Base, 0, 1, receiptLine.POType, receiptLine.PONbr);

    if (link != null)
    {
        return PXSelect<SOOrder,
            Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
            .Select(Base, link.SOOrderType, link.SOOrderNbr);
    }

    return null;
}

/// <summary>
/// Calculates the cumulative total freight price from all SOOrderShipment records for a Sales Order.
/// Uses SOOrderShipment as the single source of truth (same as Shipments grid on SO).
/// Handles both regular shipments and drop-ship PO receipts.
/// </summary>
private decimal? CalculateTotalFreightPriceForSO(SOOrder soOrder, POReceipt currentReceipt, decimal currentFreightPrice)
{
    if (soOrder == null) return 0m;

    decimal totalFreight = 0m;
    bool currentReceiptIncluded = false;

    // Get all SOOrderShipment records for this Sales Order (same as Shipments grid)
    var orderShipments = PXSelect<SOOrderShipment,
        Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
            And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
        .Select(Base, soOrder.OrderType, soOrder.OrderNbr);

    foreach (SOOrderShipment orderShipment in orderShipments.RowCast<SOOrderShipment>())
    {
        if (orderShipment.ShipmentType == SOShipmentType.DropShip)
        {
            // Drop-Ship: Get freight from PO Receipt via ShippingRefNoteID
            if (orderShipment.ShippingRefNoteID != null)
            {
                POReceipt receipt = PXSelect<POReceipt,
                    Where<POReceipt.noteID, Equal<Required<POReceipt.noteID>>>>
                    .Select(Base, orderShipment.ShippingRefNoteID);

                if (receipt != null)
                {
                    // Check if this is the current receipt being saved
                    if (receipt.ReceiptType == currentReceipt.ReceiptType && 
                        receipt.ReceiptNbr == currentReceipt.ReceiptNbr)
                    {
                        totalFreight += currentFreightPrice;
                        currentReceiptIncluded = true;
                    }
                    else
                    {
                        POReceiptExt receiptExt = receipt.GetExtension<POReceiptExt>();
                        totalFreight += (receiptExt?.UsrFreightPrice ?? 0m);
                    }
                }
            }
        }
        else
        {
            // Regular shipment: Get freight from SOShipment
            if (!string.IsNullOrEmpty(orderShipment.ShipmentNbr))
            {
                SOShipment shipment = PXSelect<SOShipment,
                    Where<SOShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                        And<SOShipment.shipmentType, Equal<Required<SOShipment.shipmentType>>>>>
                    .Select(Base, orderShipment.ShipmentNbr, orderShipment.ShipmentType);

                if (shipment != null)
                {
                    totalFreight += (shipment.CuryFreightAmt ?? 0m);
                }
            }
        }
    }

    // If current receipt wasn't found in SOOrderShipment (new receipt not yet released/linked), add its freight
    if (!currentReceiptIncluded)
    {
        totalFreight += currentFreightPrice;
    }

    return totalFreight;
}
#endregion
```

---

### 3. SOShipmentEntry_Extension.cs

**Action:** **ADD** new NTE validation for SO shipments

#### Step 1: Add Using Statement
At the top of the file, after existing using statements, add:

```csharp
using PX.Objects.PO;
```

#### Step 2: Update ConfirmShipment Override
Find the `ConfirmShipment` method and update the shipment status filter.

**Find this code** (around line 400):
```csharp
And<Where<SOShipment.status, Equal<SOShipmentStatus.confirmed>,
    Or<SOShipment.status, Equal<SOShipmentStatus.invoiced>>>>>>,
```

**Replace with**:
```csharp
And<Where<SOShipment.status, Equal<SOShipmentStatus.confirmed>,
    Or<SOShipment.status, Equal<SOShipmentStatus.partiallyInvoiced>,
    Or<SOShipment.status, Equal<SOShipmentStatus.invoiced>,
    Or<SOShipment.status, Equal<SOShipmentStatus.completed>>>>>>>>,
```

Also update the status check in the foreach loop:

**Find**:
```csharp
if (_shipment.Status == SOShipmentStatus.Confirmed || _shipment.Status == SOShipmentStatus.Invoiced)
```

**Replace with**:
```csharp
if (_shipment.Status == SOShipmentStatus.Confirmed || 
    _shipment.Status == SOShipmentStatus.PartiallyInvoiced || 
    _shipment.Status == SOShipmentStatus.Invoiced || 
    _shipment.Status == SOShipmentStatus.Completed)
```

#### Step 3: Add NTE Validation Region
At the end of the class (before the closing brace), add:

```csharp
#region NTE Freight Validation for SO Shipments
/// <summary>
/// Validates that the total freight cost across all shipments for a Sales Order
/// does not exceed the Not-To-Exceed limit defined on the Sales Order.
/// </summary>
protected virtual void _(Events.RowPersisting<SOShipment> e)
{
    if (e.Row == null) return;
    if (e.Operation == PXDBOperation.Delete) return;

    SOShipment shipment = e.Row;

    // Skip if no freight cost on this shipment
    decimal currentFreightCost = shipment.CuryFreightAmt ?? 0m;
    if (currentFreightCost <= 0m) return;

    // Get linked SO order from the shipment
    var linkedSO = GetLinkedSOForShipment(shipment);
    if (linkedSO == null) return; // No SO link, skip validation

    SOOrderExt soExt = linkedSO.GetExtension<SOOrderExt>();
    SOSetupExt setupExt = Base.sosetup.Current?.GetExtension<SOSetupExt>();

    // Check if this SO has NTE shipping terms
    if (linkedSO.ShipTermsID != setupExt?.UsrNotToExceed) return;

    // Check if SO has NTE limit set
    decimal? nteLimit = soExt?.UsrFreightPriceLimit;
    if (nteLimit == null || nteLimit <= 0m) return;

    // Calculate total freight cost across ALL shipments for this SO
    decimal? totalFreightCost = CalculateTotalFreightCostForSOShipments(linkedSO, shipment, currentFreightCost);

    // Validate against limit
    if (totalFreightCost > nteLimit)
    {
        decimal? exceedAmt = totalFreightCost - nteLimit;
        string errorMsg = Messages.ShipmentFreightExceedsNTE(
            $"{linkedSO.OrderType}-{linkedSO.OrderNbr}",
            totalFreightCost,
            nteLimit,
            exceedAmt
        );

        // Use setup toggle to determine error level
        PXErrorLevel errorLevel = (setupExt?.UsrEnforcePONTE == true) 
            ? PXErrorLevel.Error 
            : PXErrorLevel.Warning;

        e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(
            shipment, 
            currentFreightCost, 
            new PXSetPropertyException(errorMsg, errorLevel)
        );

        // If hard stop mode, throw exception to block save
        if (errorLevel == PXErrorLevel.Error)
        {
            throw new PXException(errorMsg);
        }
    }
}

/// <summary>
/// Retrieves the linked Sales Order for the shipment.
/// Uses OrderList to get the first linked SO, or queries SOOrderShipment.
/// </summary>
private SOOrder GetLinkedSOForShipment(SOShipment shipment)
{
    if (shipment == null) return null;

    // First try to get from OrderList view (already loaded)
    SOOrder order = Base.OrderList.Select().FirstOrDefault()?.GetItem<SOOrder>();
    if (order != null) return order;

    // Fallback: Query SOOrderShipment to find linked order
    var orderShipment = PXSelect<SOOrderShipment,
        Where<SOOrderShipment.shipmentType, Equal<Required<SOOrderShipment.shipmentType>>,
            And<SOOrderShipment.shipmentNbr, Equal<Required<SOOrderShipment.shipmentNbr>>>>>
        .SelectWindowed(Base, 0, 1, shipment.ShipmentType, shipment.ShipmentNbr)
        .RowCast<SOOrderShipment>()
        .FirstOrDefault();

    if (orderShipment != null)
    {
        return PXSelect<SOOrder,
            Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
            .Select(Base, orderShipment.OrderType, orderShipment.OrderNbr);
    }

    return null;
}

/// <summary>
/// Calculates the cumulative total freight price from all SOOrderShipment records for a Sales Order.
/// Uses SOOrderShipment as the single source of truth (same as Shipments grid on SO).
/// Handles both regular shipments and drop-ship PO receipts.
/// </summary>
private decimal? CalculateTotalFreightCostForSOShipments(SOOrder soOrder, SOShipment currentShipment, decimal currentFreightCost)
{
    if (soOrder == null) return 0m;

    decimal totalFreight = 0m;
    bool currentShipmentIncluded = false;

    // Get all SOOrderShipment records for this Sales Order (same as Shipments grid)
    var orderShipments = PXSelect<SOOrderShipment,
        Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
            And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
        .Select(Base, soOrder.OrderType, soOrder.OrderNbr);

    foreach (SOOrderShipment orderShipment in orderShipments.RowCast<SOOrderShipment>())
    {
        if (orderShipment.ShipmentType == SOShipmentType.DropShip)
        {
            // Drop-Ship: Get freight from PO Receipt via ShippingRefNoteID
            if (orderShipment.ShippingRefNoteID != null)
            {
                POReceipt receipt = PXSelect<POReceipt,
                    Where<POReceipt.noteID, Equal<Required<POReceipt.noteID>>>>
                    .Select(Base, orderShipment.ShippingRefNoteID);

                if (receipt != null)
                {
                    POReceiptExt receiptExt = receipt.GetExtension<POReceiptExt>();
                    totalFreight += (receiptExt?.UsrFreightPrice ?? 0m);
                }
            }
        }
        else
        {
            // Regular shipment: Get freight from SOShipment
            if (!string.IsNullOrEmpty(orderShipment.ShipmentNbr))
            {
                SOShipment shipment = PXSelect<SOShipment,
                    Where<SOShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                        And<SOShipment.shipmentType, Equal<Required<SOShipment.shipmentType>>>>>
                    .Select(Base, orderShipment.ShipmentNbr, orderShipment.ShipmentType);

                if (shipment != null)
                {
                    // Check if this is the current shipment being saved
                    if (shipment.ShipmentType == currentShipment.ShipmentType && 
                        shipment.ShipmentNbr == currentShipment.ShipmentNbr)
                    {
                        totalFreight += currentFreightCost;
                        currentShipmentIncluded = true;
                    }
                    else
                    {
                        totalFreight += (shipment.CuryFreightAmt ?? 0m);
                    }
                }
            }
        }
    }

    // If current shipment wasn't found in SOOrderShipment (new shipment not yet linked), add its freight
    if (!currentShipmentIncluded)
    {
        totalFreight += currentFreightCost;
    }

    return totalFreight;
}
#endregion
```

---

### 4. SOOrderEntry_Extension.cs

**Action:** **MODIFY** freight total calculation logic

#### Location
Find the `FieldSelecting<SOOrder, SOOrderExt.usrFreightTotal>` event handler (around line 327).

#### Replace Entire Method

**Find**:
```csharp
protected virtual void _(Events.FieldSelecting<SOOrder, SOOrderExt.usrFreightTotal> e)
{
    if (e.Row == null) return;

    SOOrder order = e.Row;
    
    // Calculate freight total from all confirmed and invoiced shipments
    decimal? totalFreight = 0m;
    var shipmentlist = PXSelectJoin<SOOrderShipment,
        LeftJoin<SOShipment, On<SOShipment.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>,
            And<SOShipment.shipmentType, Equal<SOOrderShipment.shipmentType>>>>,
        Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
            And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>,
            And<Where<SOShipment.status, Equal<SOShipmentStatus.confirmed>,
                Or<SOShipment.status, Equal<SOShipmentStatus.invoiced>>>>>>,
        OrderBy<Asc<SOOrderShipment.shipmentNbr>>>
        .Select(Base, order.OrderType, order.OrderNbr);

    foreach (PXResult<SOOrderShipment, SOShipment> item in shipmentlist)
    {
        SOShipment _shipment = item;
        if (_shipment.Status == SOShipmentStatus.Confirmed || _shipment.Status == SOShipmentStatus.Invoiced)
        {
            totalFreight += (_shipment.CuryFreightAmt ?? 0m);
        }
    }

    e.ReturnValue = totalFreight;
}
```

**Replace with**:
```csharp
protected virtual void _(Events.FieldSelecting<SOOrder, SOOrderExt.usrFreightTotal> e)
{
    if (e.Row == null) return;

    SOOrder order = e.Row;

    // Calculate freight total from all shipment lines regardless of status
    // Handles different shipment types: Issue, DropShip, Transfer, Invoice
    decimal totalFreight = 0m;

    // Get all SOOrderShipment records (lines under Shipments tab)
    var shipmentLines = PXSelect<SOOrderShipment,
        Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
            And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
        .Select(Base, order.OrderType, order.OrderNbr);

    foreach (SOOrderShipment orderShipment in shipmentLines.RowCast<SOOrderShipment>())
    {
        if (orderShipment.ShipmentType == SOShipmentType.DropShip)
        {
            // DropShip: Get freight from PO Receipt via ShippingRefNoteID
            if (orderShipment.ShippingRefNoteID != null)
            {
                POReceipt receipt = PXSelect<POReceipt,
                    Where<POReceipt.noteID, Equal<Required<POReceipt.noteID>>>>
                    .Select(Base, orderShipment.ShippingRefNoteID);

                if (receipt != null)
                {
                    POReceiptExt receiptExt = receipt.GetExtension<POReceiptExt>();
                    totalFreight += (receiptExt?.UsrFreightPrice ?? 0m);
                }
            }
        }
        else
        {
            // Issue, Transfer, or other types: Get freight from SOShipment
            if (!string.IsNullOrEmpty(orderShipment.ShipmentNbr) && 
                orderShipment.ShipmentNbr != PX.Objects.SO.Constants.NoShipmentNbr)
            {
                SOShipment shipment = PXSelect<SOShipment,
                    Where<SOShipment.shipmentNbr, Equal<Required<SOShipment.shipmentNbr>>,
                        And<SOShipment.shipmentType, Equal<Required<SOShipment.shipmentType>>>>>
                    .Select(Base, orderShipment.ShipmentNbr, orderShipment.ShipmentType);

                if (shipment != null)
                {
                    totalFreight += (shipment.CuryFreightAmt ?? 0m);
                }
            }
        }
    }

    e.ReturnValue = totalFreight;
}
```

---

### 5. Messages.cs

**Action:** **ADD** new message methods

#### Step 1: Add New Message Constants

Find the location after the existing `FreightExceedsLimit` method and add:

```csharp
private const string _poReceiptFreightExceedsNTE = "Total freight for Sales Order {0} ({1}) exceeds Not-To-Exceed limit ({2}) by {3}.";
public static string POReceiptFreightExceedsNTE(string soOrderNbr, decimal? totalFreight, decimal? nteLimit, decimal? exceedAmt) =>
    PXLocalizer.LocalizeFormat(_poReceiptFreightExceedsNTE, soOrderNbr, FormatCurrency(totalFreight), FormatCurrency(nteLimit), FormatCurrency(exceedAmt));

private const string _shipmentFreightExceedsNTE = "Total freight for Sales Order {0} ({1}) exceeds Not-To-Exceed limit ({2}) by {3}.";
public static string ShipmentFreightExceedsNTE(string soOrderNbr, decimal? totalFreight, decimal? nteLimit, decimal? exceedAmt) =>
    PXLocalizer.LocalizeFormat(_shipmentFreightExceedsNTE, soOrderNbr, FormatCurrency(totalFreight), FormatCurrency(nteLimit), FormatCurrency(exceedAmt));
```

#### Step 2: Add FormatCurrency Helper

Add this private helper method:

```csharp
/// <summary>
/// Formats a decimal value as currency with $ symbol and 2 decimal places.
/// </summary>
private static string FormatCurrency(decimal? value) =>
    string.Format("$ {0:#,##0.00}", value ?? 0m);
```

#### Step 3: Update Existing Messages (Optional but Recommended)

Update the existing `FreightExceedsLimit` method to use the new formatter:

**Find**:
```csharp
private const string _freightExceedsLimit = "Freight price exceeds limit by {0}.";
public static string FreightExceedsLimit(decimal? amount) =>
    PXLocalizer.LocalizeFormat(_freightExceedsLimit, amount);
```

**Replace with**:
```csharp
private const string _freightExceedsLimit = "Freight price exceeds limit by {0}.";
public static string FreightExceedsLimit(decimal? amount) =>
    PXLocalizer.LocalizeFormat(_freightExceedsLimit, FormatCurrency(amount));
```

---

## Configuration Requirements

### Prerequisites - Custom Fields Must Exist

Before applying these changes, verify these custom fields exist in your 25R2 system:

#### SOSetup Extension Fields
```csharp
// File: DACExt/SOSetupExt.cs
public sealed class SOSetupExt : PXCacheExtension<SOSetup>
{
    #region UsrNotToExceed
    [PXDBString(10, IsUnicode = true)]
    [PXUIField(DisplayName = "Not-To-Exceed Ship Terms")]
    [PXSelector(typeof(ShipTerms.shipTermsID))]
    public string UsrNotToExceed { get; set; }
    public abstract class usrNotToExceed : BqlString.Field<usrNotToExceed> { }
    #endregion

    #region UsrEnforcePONTE
    [PXDBBool]
    [PXUIField(DisplayName = "Enforce NTE as Error")]
    [PXDefault(false)]
    public bool? UsrEnforcePONTE { get; set; }
    public abstract class usrEnforcePONTE : BqlBool.Field<usrEnforcePONTE> { }
    #endregion
}
```

#### SOOrder Extension Fields
```csharp
// File: DACExt/SOOrderExt.cs
public sealed class SOOrderExt : PXCacheExtension<SOOrder>
{
    #region UsrFreightPriceLimit
    [PXDBDecimal(2)]
    [PXUIField(DisplayName = "Freight Price Limit (NTE)")]
    public decimal? UsrFreightPriceLimit { get; set; }
    public abstract class usrFreightPriceLimit : BqlDecimal.Field<usrFreightPriceLimit> { }
    #endregion

    #region UsrFreightTotal
    [PXDecimal(2)]
    [PXUIField(DisplayName = "Total Freight", Enabled = false)]
    public decimal? UsrFreightTotal { get; set; }
    public abstract class usrFreightTotal : BqlDecimal.Field<usrFreightTotal> { }
    #endregion
}
```

#### POReceipt Extension Fields
```csharp
// File: DACExt/POReceiptExt.cs
public sealed class POReceiptExt : PXCacheExtension<POReceipt>
{
    #region UsrFreightPrice
    [PXDBDecimal(2)]
    [PXUIField(DisplayName = "Freight Price")]
    public decimal? UsrFreightPrice { get; set; }
    public abstract class usrFreightPrice : BqlDecimal.Field<usrFreightPrice> { }
    #endregion
}
```

### Setup Configuration

#### Step 1: Configure SO Setup (SO201000)
1. Navigate to **Sales Orders → Configuration → Sales Orders Preferences**
2. Go to custom tab/section
3. Set **Not-To-Exceed Ship Terms** = Ship Terms ID that triggers NTE (e.g., "NTE")
4. Set **Enforce NTE as Error**:
   - ☑ Checked = Error (blocks save)
   - ☐ Unchecked = Warning (allows save with message)

#### Step 2: Configure Sales Orders
1. On Sales Orders where NTE applies, set:
   - **Ship Terms** = The value configured in SO Setup
   - **Freight Price Limit (NTE)** = Maximum allowed freight amount

---

## Testing Guide

### Test Case 1: Drop-Ship Receipt Validation (Error Mode)

**Setup:**
1. SO Setup: `UsrEnforcePONTE = true` (Error mode)
2. Create Sales Order:
   - Ship Terms = "NTE"
   - Freight Price Limit = $100.00

**Steps:**
1. Create drop-ship PO from SO
2. Create PO Receipt #1:
   - Set `UsrFreightPrice = $60.00`
   - Save → **Expected: Success**
3. Create PO Receipt #2:
   - Set `UsrFreightPrice = $50.00`
   - Save → **Expected: Error**
   - Error message: "Total freight for Sales Order SO-XXXXXX ($ 110.00) exceeds Not-To-Exceed limit ($ 100.00) by $ 10.00."
   - Save is blocked

**Verify:**
- Sales Order shows `UsrFreightTotal = $60.00` (only first receipt)
- Second receipt cannot be saved until freight is reduced

---

### Test Case 2: Drop-Ship Receipt Validation (Warning Mode)

**Setup:**
1. SO Setup: `UsrEnforcePONTE = false` (Warning mode)
2. Create Sales Order:
   - Ship Terms = "NTE"
   - Freight Price Limit = $100.00

**Steps:**
1. Create drop-ship PO from SO
2. Create PO Receipt:
   - Set `UsrFreightPrice = $120.00`
   - Save → **Expected: Warning**
   - Warning message displayed
   - Save completes successfully

**Verify:**
- Receipt is saved with warning
- Sales Order shows `UsrFreightTotal = $120.00`

---

### Test Case 3: Regular Shipment Validation

**Setup:**
1. SO Setup: `UsrEnforcePONTE = true`
2. Create Sales Order:
   - Ship Terms = "NTE"
   - Freight Price Limit = $150.00

**Steps:**
1. Create Shipment #1:
   - Set `CuryFreightAmt = $80.00`
   - Confirm → **Expected: Success**
2. Create Shipment #2:
   - Set `CuryFreightAmt = $80.00`
   - Confirm → **Expected: Error**
   - Total = $160.00 exceeds limit of $150.00

**Verify:**
- First shipment confirmed successfully
- Second shipment blocked with error

---

### Test Case 4: Mixed Scenario (Drop-Ship + Regular)

**Setup:**
1. Create SO with 2 lines:
   - Line 1: Drop-ship item
   - Line 2: Regular stock item
   - Freight Price Limit = $100.00

**Steps:**
1. Create drop-ship PO for Line 1 → Receipt with `UsrFreightPrice = $50.00`
2. Create regular shipment for Line 2 with `CuryFreightAmt = $60.00`
3. Try to create another receipt/shipment with additional freight

**Verify:**
- SO shows `UsrFreightTotal = $110.00`
- NTE validation includes freight from BOTH sources
- Additional freight is blocked/warned if exceeds limit

---

### Test Case 5: No NTE Configuration

**Setup:**
1. Create Sales Order with:
   - Ship Terms ≠ "NTE" (e.g., "FOB")
   - OR Freight Price Limit = $0 or blank

**Steps:**
1. Create receipts/shipments with any freight amount
2. Save/Confirm

**Expected:**
- No NTE validation occurs
- Save completes without warning/error
- System functions normally

---

### Test Case 6: Multiple Receipts for Same PO

**Setup:**
1. Drop-ship PO from SO with limit = $100.00

**Steps:**
1. Receipt #1: $40.00 → Success
2. Receipt #2: $40.00 → Success (total $80.00)
3. Receipt #3: $30.00 → Warning/Error (total $110.00)

**Verify:**
- Cumulative freight tracked across multiple receipts
- Each receipt validates against total

---

## Rollback Plan

If issues are discovered after deployment, follow these steps to rollback:

### Step 1: Restore POOrderEntry_Extension.cs
Re-add the removed `#region NTE Validation` section to POOrderEntry_Extension.cs

### Step 2: Remove New Code
- Remove NTE validation region from POReceiptEntry_Extension.cs
- Remove NTE validation region from SOShipmentEntry_Extension.cs
- Revert SOOrderEntry_Extension.cs freight calculation changes

### Step 3: Revert Messages.cs
- Remove `POReceiptFreightExceedsNTE` and `ShipmentFreightExceedsNTE` methods
- Remove `FormatCurrency` helper (if not used elsewhere)

### Step 4: Publish and Test
- Republish customization project
- Test that original PO Order validation works
- Verify no compilation errors

---

## Post-Migration Checklist

- [ ] All code changes compiled successfully
- [ ] Customization project published without errors
- [ ] Test Case 1 (Drop-Ship Error Mode) passes
- [ ] Test Case 2 (Drop-Ship Warning Mode) passes
- [ ] Test Case 3 (Regular Shipment) passes
- [ ] Test Case 4 (Mixed Scenario) passes
- [ ] Test Case 5 (No NTE Config) passes
- [ ] Performance acceptable with large datasets
- [ ] No console errors in browser
- [ ] Application event log reviewed
- [ ] User documentation updated
- [ ] Training materials prepared (if needed)

---

## Performance Optimization (Optional)

If performance issues occur with large transaction volumes, consider adding these database indexes:

```sql
-- Drop-Ship Link Index
CREATE NONCLUSTERED INDEX IX_DropShipLink_PO_SO
ON DropShipLink (POOrderType, POOrderNbr, SOOrderType, SOOrderNbr)
INCLUDE (SOLineNbr, POLineNbr);

-- SO Order Shipment Index
CREATE NONCLUSTERED INDEX IX_SOOrderShipment_OrderShip
ON SOOrderShipment (OrderType, OrderNbr, ShipmentType, ShipmentNbr, ShippingRefNoteID)
INCLUDE (InvoiceNbr);
```

---

## Support & Troubleshooting

### Common Issues

**Issue:** Compilation error "POReceipt not found"
- **Solution:** Add `using PX.Objects.PO;` to POReceiptEntry_Extension.cs and SOShipmentEntry_Extension.cs

**Issue:** "UsrFreightPrice field not found"
- **Solution:** Verify POReceiptExt has UsrFreightPrice field defined

**Issue:** "UsrEnforcePONTE field not found"
- **Solution:** Add field to SOSetupExt (see Configuration Requirements)

**Issue:** NTE validation not firing
- **Solution:** Verify ShipTermsID on SO matches UsrNotToExceed value in SO Setup

**Issue:** Freight total always zero
- **Solution:** Check that SOOrderShipment.ShippingRefNoteID is populated for drop-ship lines

---

## Additional Notes

### Currency Considerations
- `FormatCurrency()` method uses format "$ 1,234.56"
- Verify this matches your regional/currency settings
- Modify format string if different currency symbol needed

### Validation Timing
- PO Receipt: Validates on `RowPersisting` (before database save)
- SO Shipment: Validates on `RowPersisting` (before confirm)
- Early validation prevents bad data from being saved

### SOOrderShipment as Source of Truth
The enhancement uses `SOOrderShipment` table as the single source for freight aggregation because:
- It matches the Shipments grid visible to users
- Handles all shipment types (Issue, DropShip, Transfer, Invoice)
- Links drop-ship receipts via `ShippingRefNoteID`
- Provides consistent view across all screens

---

**Document Version:** 1.0  
**Last Updated:** January 10, 2026  
**Prepared By:** Migration Assistant  
**Status:** Ready for Implementation

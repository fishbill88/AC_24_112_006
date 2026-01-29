# RestrictInvoice Customization Package

## Overview
This customization enforces strict 1 PO = 1 Bill = 1 Receipt relationship for AP Bills connected to Purchase Orders. When enabled, it validates that:
- Only one bill can be created per Purchase Order
- Bill amount matches PO amount within configurable tolerance
- Only one receipt is connected per Purchase Order

## Setup Instructions

### 1. Enable the Feature
Navigate to: **AP > Setup > AP Setup (AP101000)**
- Go to the **PO-Bill Restrictions** tab
- Check **Enable PO-Bill Restriction** to activate validation
- Set **Amount Tolerance** (default: 0.01) for allowed rounding differences

### 2. Configuration Options

#### Enable PO-Bill Restriction
- **Default**: Disabled (false)
- **Purpose**: Master switch to enable/disable all validations
- **Effect**: When disabled, existing Acumatica behavior is preserved

#### Amount Tolerance
- **Default**: 0.01
- **Range**: 0.00 to 999.99
- **Purpose**: Maximum allowed difference between Bill and PO amounts
- **Example**: If PO total is $1,000.00 and tolerance is 0.01, bill amount must be between $999.99 and $1,000.01

## Validation Rules

When enabled, the following validations occur during bill release:

### 1. One Bill Per PO
- System checks if another bill already references the same Purchase Order
- **Error**: "Cannot release bill. Purchase Order {PONbr} is already connected to another bill: {TranType} {RefNbr}. Only one bill per PO is allowed per setup configuration."

### 2. Amount Matching
- System compares bill line totals (for each PO) against PO Order Total
- Validates per PO if bill contains multiple PO references
- **Error**: "Cannot release bill. Bill amount for PO {PONbr} ({billTotal}) does not match PO amount ({poTotal}). Difference: {difference}, Allowed tolerance: {tolerance}. Bill and PO amounts must match within configured tolerance."

### 3. One Receipt Per PO
- System ensures only one receipt is connected to each PO
- Checks both within current bill and across other bills
- **Error**: "Cannot release bill. Purchase Order {PONbr} is connected to multiple receipts: {receiptList}. Only one receipt per PO is allowed (1 PO = 1 Bill = 1 Receipt)."

## Files Included

- **APSetupExt.cs**: DAC extension adding two fields to APSetup
- **APSetupMaintExt.cs**: Graph extension for AP101000 setup screen
- **APInvoiceEntryExt.cs**: Graph extension for AP301000 with validation logic

## Technical Details

### Per-PO Validation
If a bill has lines from multiple POs, each PO is validated separately:
- Bill lines are grouped by `POOrderType` + `PONbr`
- Each group is validated independently for one-bill, one-receipt, and amount matching
- One PO violation does not prevent validation of other POs

### Database Fields
Extended fields stored in APSetup table:
- `UsrEnablePOBillRestriction` (bit/boolean)
- `UsrPOBillAmountTolerance` (decimal 2 precision)

### Override Method
- Overrides `APInvoiceEntry.Release()` action via delegate
- Performs validation before calling base method
- Throws `PXException` to block release if validation fails

## Deployment

This customization can be published independently to client instances without conflicts with other customizations.

### Publishing Steps
1. In Acumatica, navigate to **System > Customization > Customization Projects**
2. Import this customization package
3. Publish the customization
4. The setup fields will appear in AP Setup (AP101000) under PO-Bill Restrictions tab

## Client Requirements

The client must:
1. Enable the feature in AP Setup
2. Set appropriate amount tolerance
3. Ensure existing data complies with restrictions before enabling
4. Clean up any existing violations (multiple bills per PO) if present

## Disabling the Feature

To temporarily disable validation without unpublishing:
1. Navigate to AP Setup (AP101000)
2. Uncheck **Enable PO-Bill Restriction**
3. Save

This allows existing bills to be released without validation while keeping the customization published.

## Support

For questions or issues:
- Review error messages for specific violation details
- Check AP Setup configuration
- Verify PO and Bill amounts match within tolerance
- Ensure no duplicate bills exist for the same PO before enabling

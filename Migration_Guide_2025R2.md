# Migration Guide: Apply Changes to 2025 R2 Version

**Source Version:** 24.112.006  
**Target Version:** 2025 R2  
**Date:** December 26, 2025  
**Changeset Reference:** cbd553b790851f2e3380a43177ecda282b647f94 → HEAD

---

## ⚠️ IMPORTANT REMINDERS

### Scope
This migration guide covers changes **ONLY** in the following folders within the CompiledVersion customization project:
- **DAC** - Data Access Classes
- **DACExt** - DAC Extensions  
- **GraphExt** - Graph Extensions
- **Graphs** - Business Logic Graphs
- **Helpers** - Helper classes (no changes in this release)
- **Messages** - Message/error text constants

**Base Path:** `App_Data/Projects/CompiledVersion/CompiledVersion/`

### Customization Package Updates
After implementing these changes, you **MUST** update your customization packages:

1. **Publish Customization Project** - After making code changes in the Customization Project editor
2. **Test in Development Environment** - Thoroughly test all changes before promoting
3. **Export Updated Package** - Create a new customization package export
4. **Version Control** - Tag this as a new version if using source control
5. **Database Schema Changes** - Some DAC field changes may require database updates

---

## 📋 Table of Contents

1. [DAC Changes](#1-dac-changes)
2. [DACExt Changes](#2-dacext-changes)
3. [GraphExt Changes](#3-graphext-changes)
4. [New Graphs](#4-new-graphs)
5. [Helpers Changes](#5-helpers-changes)
6. [Messages Updates](#6-messages-updates)
7. [Testing Checklist](#7-testing-checklist)

---

## 1. DAC Changes

### 1.1 CROpportunityClassStageReason.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/Ameer/DAC/CROpportunityClassStageReason.cs`

**Change:** Add documentation skip comment

```csharp
namespace CompiledVersion.DAC
{
    //skip documentation for this
    [Serializable]
    [PXCacheName("Opportunity Class Stage Reason")]
    public partial class CROpportunityClassStageReason : PXBqlTable, IBqlTable
```

---

## 2. DACExt Changes

### 2.1 Create New File: CROpportunityExt.cs (Ameer)
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/Ameer/Extension/DAC/CROpportunityExt.cs`

**Action:** Create new file with the following content:

```csharp
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using System;

namespace PX.Objects.CR
{
    public sealed class CROpportunityExt2 : PXCacheExtension<CROpportunity>
    {
        public static bool IsActive() => true;

        #region UsrReferralSource
        public abstract class usrReferralSource : BqlString.Field<usrReferralSource> { }

        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Referral Source")]
        public string UsrReferralSource { get; set; }
        #endregion

        #region UsrReleaseHold
        public abstract class usrReleaseHold : BqlDateTime.Field<usrReleaseHold> { }

        [PXDBDate]
        [PXUIField(DisplayName = "Release Hold Date")]
        public DateTime? UsrReleaseHold { get; set; }
        #endregion

        #region UsrServicesEstimate
        public abstract class usrServicesEstimate : BqlDecimal.Field<usrServicesEstimate> { }

        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Services Estimate")]
        public Decimal? UsrServicesEstimate { get; set; }
        #endregion

        #region UsrActivityNote
        public abstract class usrActivityNote : BqlString.Field<usrActivityNote> { }

        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Activity Note")]
        public string UsrActivityNote { get; set; }
        #endregion
    }
}
```

**Database Impact:** Yes - New custom fields will be added to CROpportunity table

---

### 2.2 Update: CROpportunityReasonExt.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/Ameer/Extension/DAC/CROpportunityReasonExt.cs`

**Change:** Add documentation skip comment

```csharp
namespace CompiledVersion.DAC
{
    //skip documentation for this
    public sealed class CROpportunityReasonExt : PXCacheExtension<CROpportunity>
    {
        public static bool IsActive() => true;
```

---

### 2.3 Create New File: StandaloneCROpportunityExt.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/Ameer/Extension/DAC/StandaloneCROpportunityExt.cs`

**Action:** Create new file with the following content:

```csharp
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR.Standalone;
using System;

namespace CompiledVersion.DAC
{
    public sealed class StandaloneCROpportunityAmeerExt : PXCacheExtension<CROpportunity>
    {
        public static bool IsActive() => true;

        #region UsrReferralSource
        public abstract class usrReferralSource : BqlString.Field<usrReferralSource> { }

        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Referral Source")]
        public string UsrReferralSource { get; set; }
        #endregion

        #region UsrReleaseHold
        public abstract class usrReleaseHold : BqlDateTime.Field<usrReleaseHold> { }

        [PXDBDate]
        [PXUIField(DisplayName = "Release Hold Date")]
        public DateTime? UsrReleaseHold { get; set; }
        #endregion

        #region UsrServicesEstimate
        public abstract class usrServicesEstimate : BqlDecimal.Field<usrServicesEstimate> { }

        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Services Estimate")]
        public Decimal? UsrServicesEstimate { get; set; }
        #endregion
    }
}
```

**Database Impact:** Yes - Fields added to Standalone CROpportunity table

---

### 2.4 Update: POLineExt.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/DACExt/POLineExt.cs`

**Change 1:** Reduce UsrVendorNotes field size from 500 to 250 characters

**Find:**
```csharp
#region UsrVendorNotes
[PXDBString(500, IsUnicode = true)]
[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "Vendor Notes")]
public string UsrVendorNotes { get; set; }
public abstract class usrVendorNotes : PX.Data.BQL.BqlString.Field<usrVendorNotes> { }
#endregion
```

**Replace with:**
```csharp
#region UsrVendorNotes
[PXDBString(250, IsUnicode = true)]
[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "Vendor Notes")]
public string UsrVendorNotes { get; set; }
public abstract class usrVendorNotes : PX.Data.BQL.BqlString.Field<usrVendorNotes> { }
#endregion
```

**Change 2:** Update UsrShippingTerms to use ShipTerms selector

**Find:**
```csharp
#region UsrShippingTerms
[PXString(1, IsFixed = true)]
[SOShipComplete.List()]
[PXUIField(DisplayName = "Shipping Rule", Enabled = false)]
public string UsrShippingTerms { get; set; }
public abstract class usrShippingTerms : PX.Data.BQL.BqlString.Field<usrShippingTerms> { }
#endregion
```

**Replace with:**
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

**Database Impact:** Yes - Field definitions changed

---

### 2.5 Update: POOrderExt.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/DACExt/POOrderExt.cs`

**Change 1:** Convert UsrFreightCost to unbound/calculated field

**Find:**
```csharp
#region UsrFreightCost
[PXDBDecimal(2)]
[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "Freight Cost")]
public decimal? UsrFreightCost { get; set; }
public abstract class usrFreightCost : PX.Data.BQL.BqlDecimal.Field<usrFreightCost> { }
#endregion
```

**Replace with:**
```csharp
#region UsrFreightCost
[PXDecimal(2)]
[PXUIField(DisplayName = "Freight Cost", Enabled = false)]
public decimal? UsrFreightCost { get; set; }
public abstract class usrFreightCost : PX.Data.BQL.BqlDecimal.Field<usrFreightCost> { }
#endregion
```

**Change 2:** Convert UsrFreightPrice to unbound/calculated field

**Find:**
```csharp
#region UsrFreightPrice
[PXDBDecimal(2)]
[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
[PXUIField(DisplayName = "Freight Price")]
public decimal? UsrFreightPrice { get; set; }
public abstract class usrFreightPrice : PX.Data.BQL.BqlDecimal.Field<usrFreightPrice> { }
#endregion
```

**Replace with:**
```csharp
#region UsrFreightPrice
[PXDecimal(2)]
[PXUIField(DisplayName = "Freight Price", Enabled = false)]
public decimal? UsrFreightPrice { get; set; }
public abstract class usrFreightPrice : PX.Data.BQL.BqlDecimal.Field<usrFreightPrice> { }
#endregion
```

**Database Impact:** Yes - Fields will be removed from database (now unbound)

---

### 2.6 Update: SOSetupExt.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/DACExt/SOSetupExt.cs`

**Change:** Add new field UsrEnforcePONTE

**Find the location after `UsrCopyLineAttachmentsToPO` region and add:**

```csharp
#region UsrEnforcePONTE
[PXDBBool]
[PXUIField(DisplayName = "Enforce PO Not-To-Exceed Limit")]
[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
public bool? UsrEnforcePONTE { get; set; }
public abstract class usrEnforcePONTE : PX.Data.BQL.BqlBool.Field<usrEnforcePONTE> { }
#endregion
```

**Database Impact:** Yes - New field added to SOSetup

---

## 3. GraphExt Changes

### 3.1 Update Ameer Graph Extensions (Add Documentation Skip)

Add `//skip documentation for this` comment to these files:

1. **CROpportunityClassMaint_StageReasons.cs**
2. **OpportunityMaint_OpenCustomExt.cs**
3. **OpportunityMaint_ReasonSyncExt.cs**
4. **OpportunityMaint_StageReasonExt.cs**

**Pattern:**
```csharp
namespace CompiledVersion.GraphExt  // or CompiledVersion
{
    //skip documentation for this
    public class [ClassName] : PXGraphExtension<[BaseGraph]>
    {
```

---

### 3.2 Update: POCreateReplaceOriginalMethod.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/GraphExt/POCreateReplaceOriginalMethod.cs`

**Change 1:** Update Vendor Notes concatenation logic

**Find the line:**
```csharp
poLineExt.UsrVendorNotes = soLineExt?.UsrVendorNotes;
```

**Replace with:**
```csharp
// Concatenate Vendor Notes when merging lines, up to 250 characters
string newVendorNotes = soLineExt?.UsrVendorNotes;
if (!string.IsNullOrWhiteSpace(newVendorNotes))
{
    if (string.IsNullOrWhiteSpace(poLineExt.UsrVendorNotes))
    {
        // First note - just set it, truncate if needed
        poLineExt.UsrVendorNotes = newVendorNotes.Length > 250 
            ? newVendorNotes.Substring(0, 250) 
            : newVendorNotes;
    }
    else if (poLineExt.UsrVendorNotes.Length < 250)
    {
        // Existing notes - concatenate with separator
        string separator = " | ";
        string combined = poLineExt.UsrVendorNotes + separator + newVendorNotes;
        poLineExt.UsrVendorNotes = combined.Length > 250 
            ? combined.Substring(0, 250) 
            : combined;
    }
    // If already at 250 chars, don't add more
}

// Copy Shipping Terms - only use the first one when merging multiple SO lines
if (string.IsNullOrWhiteSpace(poLineExt.UsrShippingTerms))
{
    poLineExt.UsrShippingTerms = soLineExt?.UsrShippingTerms;
}
```

**Change 2:** Update Ship Terms and Customer Account assignment

**Find:**
```csharp
poOrderExt.UsrShipTermsID = soOrder?.ShipTermsID;
poOrderExt.UsrCustomerAccount = soOrderExt?.UsrCustomerAccount;
```

**Replace with:**
```csharp
// Copy Shipping Terms and Carrier Account only for Drop-Ship orders
if (demand.PlanType == INPlanConstants.Plan6D || demand.PlanType == "6D")
{
    poOrderExt.UsrShipTermsID = soOrder?.ShipTermsID;
    poOrderExt.UsrCustomerAccount = soOrderExt?.UsrCustomerAccount;
}
```

---

### 3.3 Update: POOrderEntry_Extension.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/GraphExt/POOrderEntry_Extension.cs`

**Change 1:** Add using statement at the top

**Find:**
```csharp
using PX.Objects.AP;
```

**Add after:**
```csharp
using PX.Objects.AR;
```

**Change 2:** Comment out shipping terms sync logic

**Find the code block around line 937:**
```csharp
if (e.Row != null && link != null)
{
    SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                     And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                                     .Select(Base, link.SOOrderType, link.SOOrderNbr, link.SOLineNbr);
    if (soLine == null) return;
    lineExt.UsrShippingTerms = soLine?.ShipComplete;
    e.Cache.SetValueExt<POLineExt.usrShippingTerms>(line, soLine?.ShipComplete);
}
```

**Replace with:**
```csharp
//if (e.Row != null && link != null)
//{
//    SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
//                     And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
//                                     .Select(Base, link.SOOrderType, link.SOOrderNbr, link.SOLineNbr);
//    if (soLine == null) return;
//    lineExt.UsrShippingTerms = soLine?.ShipComplete;
//    e.Cache.SetValueExt<POLineExt.usrShippingTerms>(line, soLine?.ShipComplete);
//}
```

**Change 3:** Add freight calculation methods

**Add these methods in the `#region POOrder` section (after line ~1055):**

```csharp
protected virtual void _(Events.FieldSelecting<POOrder, POOrderExt.usrFreightCost> e)
{
    if (e.Row == null) return;
    POOrder order = e.Row;

    // Only calculate for drop-ship orders linked to SO
    if (order.SOOrderType == null || order.SOOrderNbr == null)
    {
        e.ReturnValue = 0m;
        return;
    }

    // Get all SO orders linked to this PO (handles merged SOs)
    var linkedSOOrders = PXSelectJoin<SOOrder,
        InnerJoin<DropShipLink,
            On<DropShipLink.sOOrderType, Equal<SOOrder.orderType>,
            And<DropShipLink.sOOrderNbr, Equal<SOOrder.orderNbr>>>>,
        Where<DropShipLink.pOOrderType, Equal<Required<POOrder.orderType>>,
            And<DropShipLink.pOOrderNbr, Equal<Required<POOrder.orderNbr>>>>>
        .Select(Base, order.OrderType, order.OrderNbr)
        .RowCast<SOOrder>()
        .ToList();

    if (!linkedSOOrders.Any())
    {
        e.ReturnValue = 0m;
        return;
    }

    decimal totalFreightCost = 0m;

    // For each linked SO, find invoices created from shipments
    foreach (SOOrder soOrder in linkedSOOrders)
    {
        // Get all invoices created from this SO's shipments
        var invoices = PXSelectJoin<ARInvoice,
            InnerJoin<SOOrderShipment,
                On<SOOrderShipment.invoiceType, Equal<ARInvoice.docType>,
                And<SOOrderShipment.invoiceNbr, Equal<ARInvoice.refNbr>>>>,
            Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
            .Select(Base, soOrder.OrderType, soOrder.OrderNbr)
            .RowCast<ARInvoice>()
            .ToList();

        foreach (ARInvoice invoice in invoices)
        {
            totalFreightCost += (invoice.CuryFreightCost ?? 0m);
        }
    }

    e.ReturnValue = totalFreightCost;
}

protected virtual void _(Events.FieldSelecting<POOrder, POOrderExt.usrFreightPrice> e)
{
    if (e.Row == null) return;
    POOrder order = e.Row;

    // Only calculate for drop-ship orders linked to SO
    if (order.SOOrderType == null || order.SOOrderNbr == null)
    {
        e.ReturnValue = 0m;
        return;
    }

    // Get all SO orders linked to this PO (handles merged SOs)
    var linkedSOOrders = PXSelectJoin<SOOrder,
        InnerJoin<DropShipLink,
            On<DropShipLink.sOOrderType, Equal<SOOrder.orderType>,
            And<DropShipLink.sOOrderNbr, Equal<SOOrder.orderNbr>>>>,
        Where<DropShipLink.pOOrderType, Equal<Required<POOrder.orderType>>,
            And<DropShipLink.pOOrderNbr, Equal<Required<POOrder.orderNbr>>>>>
        .Select(Base, order.OrderType, order.OrderNbr)
        .RowCast<SOOrder>()
        .ToList();

    if (!linkedSOOrders.Any())
    {
        e.ReturnValue = 0m;
        return;
    }

    decimal totalFreightPrice = 0m;

    // For each linked SO, find invoices created from shipments
    foreach (SOOrder soOrder in linkedSOOrders)
    {
        // Get all invoices created from this SO's shipments
        var invoices = PXSelectJoin<ARInvoice,
            InnerJoin<SOOrderShipment,
                On<SOOrderShipment.invoiceType, Equal<ARInvoice.docType>,
                And<SOOrderShipment.invoiceNbr, Equal<ARInvoice.refNbr>>>>,
            Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
            .Select(Base, soOrder.OrderType, soOrder.OrderNbr)
            .RowCast<ARInvoice>()
            .ToList();

        foreach (ARInvoice invoice in invoices)
        {
            totalFreightPrice += (invoice.CuryFreightAmt ?? 0m);
        }
    }

    e.ReturnValue = totalFreightPrice;
}
```

**Change 4:** Add NTE Validation section

**Add this entire section before the closing of the class (around line 1223):**

```csharp
#region NTE Validation
/// <summary>
/// Validates that drop-ship PO totals do not exceed the Not-To-Exceed limit defined on the originating Sales Order.
/// </summary>
protected virtual void _(Events.RowPersisting<POOrder> e)
{
    if (e.Row == null) return;
    if (e.Operation == PXDBOperation.Delete) return;

    POOrder poOrder = e.Row;

    // Get linked SO order
    var linkedSO = GetLinkedSOForNTEValidation(poOrder);
    if (linkedSO == null) return; // No SO link, skip validation

    SOOrderExt soExt = linkedSO.GetExtension<SOOrderExt>();
    SOSetupExt setupExt = sosetup.Current?.GetExtension<SOSetupExt>();

    // Check if this SO has NTE shipping terms
    if (linkedSO.ShipTermsID != setupExt?.UsrNotToExceed) return;

    // Check if SO has NTE limit set
    decimal? nteLimit = soExt?.UsrFreightPriceLimit;
    if (nteLimit == null || nteLimit <= 0m) return;

    // Calculate total of ALL POs for this SO
    decimal? totalPOAmount = CalculateTotalPOAmountForSO(linkedSO, poOrder);

    // Validate against limit
    if (totalPOAmount > nteLimit)
    {
        decimal? exceedAmt = totalPOAmount - nteLimit;
        string errorMsg = Messages.POTotalExceedsNTE(
            $"{linkedSO.OrderType}-{linkedSO.OrderNbr}",
            totalPOAmount,
            nteLimit,
            exceedAmt
        );

        // Use setup toggle to determine error level
        PXErrorLevel errorLevel = (setupExt?.UsrEnforcePONTE == true) 
            ? PXErrorLevel.Error 
            : PXErrorLevel.Warning;

        e.Cache.RaiseExceptionHandling<POOrder.curyOrderTotal>(
            poOrder, 
            poOrder.CuryOrderTotal, 
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
/// Retrieves the linked Sales Order for NTE validation.
/// Tries header fields first, then falls back to DropShipLink query.
/// </summary>
private SOOrder GetLinkedSOForNTEValidation(POOrder poOrder)
{
    if (poOrder == null) return null;

    // Strategy 1: Use header fields if available (simple 1:1 scenario)
    if (!string.IsNullOrEmpty(poOrder.SOOrderType) && !string.IsNullOrEmpty(poOrder.SOOrderNbr))
    {
        return PXSelect<SOOrder,
            Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
            .Select(Base, poOrder.SOOrderType, poOrder.SOOrderNbr);
    }

    // Strategy 2: Use DropShipLink (handles partial/merged scenarios)
    DropShipLink link = PXSelect<DropShipLink,
        Where<DropShipLink.pOOrderType, Equal<Required<POOrder.orderType>>,
            And<DropShipLink.pOOrderNbr, Equal<Required<POOrder.orderNbr>>>>>
        .SelectWindowed(Base, 0, 1, poOrder.OrderType, poOrder.OrderNbr);

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
/// Calculates the cumulative total of all POs linked to a Sales Order.
/// Includes the current PO being saved.
/// </summary>
private decimal? CalculateTotalPOAmountForSO(SOOrder soOrder, POOrder currentPO)
{
    if (soOrder == null) return 0m;

    // Get all POs linked via DropShipLink
    var linkedPOs = PXSelectJoin<POOrder,
        InnerJoin<DropShipLink,
            On<DropShipLink.pOOrderType, Equal<POOrder.orderType>,
            And<DropShipLink.pOOrderNbr, Equal<POOrder.orderNbr>>>>,
        Where<DropShipLink.sOOrderType, Equal<Required<SOOrder.orderType>>,
            And<DropShipLink.sOOrderNbr, Equal<Required<SOOrder.orderNbr>>>>>
        .Select(Base, soOrder.OrderType, soOrder.OrderNbr)
        .RowCast<POOrder>()
        .ToList();

    decimal? total = 0m;

    foreach (POOrder po in linkedPOs)
    {
        // Use current PO's values if it's the one being saved
        if (po.OrderType == currentPO.OrderType && po.OrderNbr == currentPO.OrderNbr)
        {
            total += (currentPO.CuryOrderTotal ?? 0m);
        }
        else
        {
            total += (po.CuryOrderTotal ?? 0m);
        }
    }

    return total;
}
#endregion
```

**Change 5:** Add field visibility control

**Add this new event handler in the `#region Event Handlers` section:**

```csharp
protected virtual void _(Events.RowSelected<POOrder> e)
{
    if (e.Row == null) return;

    var order = e.Row;
    var orderExt = order.GetExtension<POOrderExt>();

    // Show UsrShipTermsID and UsrCustomerAccount only for Drop-Ship orders
    bool isDropShip = order.OrderType == "DP" || order.OrderType == "PD"; // POOrderType.DropShip or ProjectDropShip

    PXUIFieldAttribute.SetVisible<POOrderExt.usrShipTermsID>(e.Cache, order, isDropShip);
    PXUIFieldAttribute.SetVisible<POOrderExt.usrCustomerAccount>(e.Cache, order, isDropShip);
}
```

---

### 3.4 Update: SOInvoiceEntry_Extension.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/GraphExt/SOInvoiceEntry_Extension.cs`

**Change 1:** Add using statements at the top

```csharp
using PX.Common;
using POInventoryCustomization; // For SOOrderExt with UsrRTHCuryFreightTot
```

**Change 2:** Add freight override logic

**Add this method before the closing brace of the class:**

```csharp
#region Freight Override - Use SOOrder.UsrRTHCuryFreightTot for Invoice Freight

/// <summary>
/// Override ARInvoice freight total right before persisting to database.
/// This ensures we use SOOrder.UsrRTHCuryFreightTot instead of the calculated freight
/// from SOFreightDetail records. Also updates the SOFreightDetail records to match.
/// 
/// Business Rules (as of 2025-12-17):
/// - Freight is not taxed
/// - Only one open shipment per sales order is allowed
/// - Freight should be reversed for returns/credit memos
/// 
/// TODO: FUTURE ENHANCEMENT - Add order type filtering if business decides 
/// this should only apply to specific order types instead of all types.
/// </summary>
protected virtual void _(Events.RowPersisting<ARInvoice> e)
{
    if (e.Row == null || e.Operation == PXDBOperation.Delete)
        return;

    ARInvoice invoice = e.Row;

    // Find all freight details for this invoice
    var freightDetails = PXSelect<SOFreightDetail,
        Where<SOFreightDetail.docType, Equal<Required<ARInvoice.docType>>,
            And<SOFreightDetail.refNbr, Equal<Required<ARInvoice.refNbr>>>>>
        .Select(Base, invoice.DocType, invoice.RefNbr);

    if (freightDetails.Count == 0) return;

    // Get the first detail to find the order
    SOFreightDetail firstDetail = freightDetails.FirstOrDefault();
    if (firstDetail == null) return;

    // Get the sales order
    SOOrder order = SOOrder.PK.Find(Base, firstDetail.OrderType, firstDetail.OrderNbr);
    if (order == null) return;

    // Get the custom freight total from the order extension
    var orderExt = order.GetExtension<SOOrderExt>();
    if (orderExt?.UsrRTHCuryFreightTot == null)
        return;

    // Apply the custom freight amount
    decimal targetFreightTotal = orderExt.UsrRTHCuryFreightTot.Value;

    // Update the invoice header
    invoice.CuryFreightTot = targetFreightTotal;

    // Update SOFreightDetail records to match the new total
    // Strategy: Set the first detail to the full amount, zero out the rest
    bool isFirst = true;
    foreach (SOFreightDetail detail in freightDetails)
    {
        if (isFirst)
        {
            // Put the entire UsrRTHCuryFreightTot into CuryFreightAmt of first detail
            detail.CuryFreightAmt = targetFreightTotal;
            detail.CuryPremiumFreightAmt = 0m; // Zero out premium
            isFirst = false;
        }
        else
        {
            // Zero out any additional freight detail records
            detail.CuryFreightAmt = 0m;
            detail.CuryPremiumFreightAmt = 0m;
        }

        Base.FreightDetails.Update(detail);
    }
}

#endregion
```

---

### 3.5 Update: SOOrderEntry_Extension.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/GraphExt/SOOrderEntry_Extension.cs`

**Change 1:** Add freight total calculation method

**Add this method around line 324 (after existing FieldSelecting handlers):**

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

**Change 2:** Update RowSelected to control Shipping Instructions editability

**Find the RowSelected method and add at the end:**

```csharp
// Shipping Instructions should only be editable when status is Hold
bool isOnHold = order.Status == SOOrderStatus.Hold;
PXUIFieldAttribute.SetEnabled<SOOrderExt.usrShippingInstructions>(e.Cache, order, isOnHold);
```

---

### 3.6 Update: SOShipmentEntry_Extension.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/GraphExt/SOShipmentEntry_Extension.cs`

**Change 1:** Update freight limit validation logic

**Find the entire freight validation block (around line 42-90) and replace with:**

```csharp
SOOrderExt orderExt = order.GetExtension<SOOrderExt>();
SOSetupExt sOSetupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
if (orderExt == null) return;

// Only enforce limit if UsrFreightPriceLimit has a value and shipping terms match
if (orderExt.UsrFreightPriceLimit == null || orderExt.UsrFreightPriceLimit <= 0m ||
    order.ShipTermsID != sOSetupExt.UsrNotToExceed || !(shipment.OverrideFreightAmount ?? false))
{
    // Clear any previous error
    e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(shipment, newAmt, null);
    return;
}

decimal? freightLimit = orderExt.UsrFreightPriceLimit;
decimal? currentFreight = 0m;

// Calculate freight from OTHER confirmed/invoiced shipments (exclude current shipment)
var shipmentlist = PXSelectJoin<SOOrderShipment,
                    LeftJoin<SOShipment, On<SOShipment.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>,
                        And<SOShipment.shipmentType, Equal<SOOrderShipment.shipmentType>>>>,
                    Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
                        And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>,
                        And<SOShipment.shipmentNbr, NotEqual<Required<SOShipment.shipmentNbr>>,
                        And<Where<SOShipment.status, Equal<SOShipmentStatus.confirmed>,
                            Or<SOShipment.status, Equal<SOShipmentStatus.invoiced>>>>>>,
                    OrderBy<Asc<SOOrderShipment.shipmentNbr>>>
                    .Select(Base, order.OrderType, order.OrderNbr, shipment.ShipmentNbr);

foreach (var item in shipmentlist)
{
    SOOrderShipment orderShipment = item.GetItem<SOOrderShipment>();
    SOShipment _shipment = item.GetItem<SOShipment>();
    if (_shipment.ShipmentNbr != shipment.ShipmentNbr && 
        (_shipment.Status == SOShipmentStatus.Confirmed || _shipment.Status == SOShipmentStatus.Invoiced))
    {
        currentFreight += (_shipment.CuryFreightAmt ?? 0m);
    }
}

// Check if the new freight amount exceeds the limit
if ((newAmt + currentFreight) > freightLimit)
{
    decimal? exceedAmt = (newAmt + currentFreight) - freightLimit;
    decimal? adjustedAmt = freightLimit - currentFreight;

    e.Cache.SetValue<SOShipment.curyFreightAmt>(shipment, adjustedAmt);
    shipment.CuryFreightAmt = adjustedAmt;

    e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(e.Row, adjustedAmt,
        new PXSetPropertyException(Messages.FreightExceedsLimit(exceedAmt), PXErrorLevel.RowError));
}
else
{
    // Clear any previous error if the new amount is valid
    e.Cache.RaiseExceptionHandling<SOShipment.curyFreightAmt>(shipment, newAmt, null);
}
```

**Change 2:** Update shipment status filter (around line 400)

**Find:**
```csharp
Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
    And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>,
    And<SOShipment.status, Equal<SOShipmentStatus.confirmed>>>>,
```

**Replace with:**
```csharp
Where<SOOrderShipment.orderType, Equal<Required<SOOrder.orderType>>,
    And<SOOrderShipment.orderNbr, Equal<Required<SOOrder.orderNbr>>,
    And<Where<SOShipment.status, Equal<SOShipmentStatus.confirmed>,
        Or<SOShipment.status, Equal<SOShipmentStatus.invoiced>>>>>>,
```

**Change 3:** Update status check in foreach loop

**Find:**
```csharp
if (_shipment.Status == SOShipmentStatus.Confirmed)
```

**Replace with:**
```csharp
if (_shipment.Status == SOShipmentStatus.Confirmed || _shipment.Status == SOShipmentStatus.Invoiced)
```

---

## 4. New Graphs

### 4.1 Create: SIOpportunityActivityProcess.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/Ameer/Graphs/SIOpportunityActivityProcess.cs`

**Action:** Create new file with the following complete content:

```csharp
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.SM;
using System;
using PX.Objects.AR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.EP;

namespace PX.Objects.CR
{
    public class SIOpportunityActivityProcess : PXGraph<SIOpportunityActivityProcess>
    {
        #region Views

        public PXFilter<OpportunityActivityFilter> Filter;
        public PXCancel<OpportunityActivityFilter> Cancel;

        [PXFilterable]
        [PXViewDetailsButton(typeof(CROpportunity))]
        public PXFilteredProcessing<CROpportunity, OpportunityActivityFilter>
            Items;

        #endregion

        #region Constructor

        public SIOpportunityActivityProcess()
        {
            Items.SetSelected<CROpportunity.selected>();
            Items.SetProcessDelegate<SIOpportunityActivityProcess>(ProcessRecords);

            Items.AllowInsert = false;
            Items.AllowDelete = false;
            Items.AllowUpdate = true;

            // Enable the field by default
            PXUIFieldAttribute.SetEnabled<PX.Objects.CR.CROpportunityExt2.usrActivityNote>(Items.Cache, null, true);

            // Set default sort order
            Items.Cache.AllowSelect = true;
            Items.OrderByNew<OrderBy<Asc<CROpportunity.closeDate>>>();
        }

        protected virtual IEnumerable items()
        {
            var filter = Filter.Current;
            if (filter == null)
                yield break;

            // Parse comma-delimited ClassIDs
            var classIDs = new List<string>();
            if (!string.IsNullOrWhiteSpace(filter.ClassID))
            {
                classIDs = filter.ClassID.Split(',')
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .ToList();
            }

            // Parse comma-delimited Statuses for exclusion
            var excludeStatuses = new List<string>();
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                excludeStatuses = filter.Status.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
            }

            PXSelectBase<CROpportunity> select = new PXSelectJoinOrderBy<CROpportunity,
                LeftJoin<Contact, On<Contact.contactID, Equal<CROpportunity.contactID>>,
                LeftJoin<BAccount, On<BAccount.bAccountID, Equal<CROpportunity.bAccountID>>,
                LeftJoin<BAccountParent, On<BAccountParent.bAccountID, Equal<BAccount.parentBAccountID>>,
                LeftJoin<CRCampaign, On<CRCampaign.campaignID, Equal<CROpportunity.campaignSourceID>>>>>>,
                OrderBy<Asc<CROpportunity.closeDate>>>(this);

            foreach (PXResult<CROpportunity> result in select.Select())
            {
                CROpportunity opp = result;

                // Apply class filter if ClassIDs are specified
                if (classIDs.Count > 0 && !classIDs.Contains(opp.ClassID))
                    continue;

                // Exclude opportunities with matching statuses
                if (excludeStatuses.Count > 0 && excludeStatuses.Contains(opp.Status))
                    continue;

                yield return result;
            }
        }

        #endregion

        #region Event Handlers

        protected virtual void _(Events.RowSelected<OpportunityActivityFilter> e)
        {
            if (e.Row == null) return;

            Items.SetProcessAllCaption("Process All");
            Items.SetProcessCaption("Process");
        }

        protected virtual void _(Events.RowSelected<CROpportunity> e)
        {
            if (e.Row == null) return;

            PXUIFieldAttribute.SetEnabled<PX.Objects.CR.CROpportunityExt2.usrActivityNote>(e.Cache, e.Row, true);
        }

        protected virtual void _(Events.FieldUpdated<CROpportunity, PX.Objects.CR.CROpportunityExt2.usrActivityNote> e)
        {
            if (e.Row == null) return;

            var opportunityExt = e.Row.GetExtension<PX.Objects.CR.CROpportunityExt2>();
            if (opportunityExt != null && !string.IsNullOrWhiteSpace(opportunityExt.UsrActivityNote))
            {
                e.Row.Selected = true;
            }
        }

        #endregion

        #region Processing Methods

        public static void ProcessRecords(SIOpportunityActivityProcess graph, CROpportunity opportunity)
        {
            var opportunityExt = opportunity.GetExtension<PX.Objects.CR.CROpportunityExt2>();
            if (opportunityExt == null)
            {
                throw new PXException("Unable to get opportunity extension.");
            }

            string activityNote = opportunityExt.UsrActivityNote;

            if (string.IsNullOrWhiteSpace(activityNote))
            {
                throw new PXException("Activity Note is required.");
            }

            // Create the activity using CRActivityMaint graph
            var activityGraph = PXGraph.CreateInstance<CRActivityMaint>();

            var activity = (CRActivity)activityGraph.Activities.Cache.CreateInstance();
            activity.ClassID = CRActivityClass.Activity;
            activity.Type = "N"; // Note type
            activity.RefNoteID = opportunity.NoteID;
            activity.OwnerID = opportunity.OwnerID;
            activity.Subject = activityNote;
            activity.StartDate = graph.Accessinfo.BusinessDate;

            activity = activityGraph.Activities.Insert(activity);

            activityGraph.Actions.PressSave();
        }

        #endregion

        #region Filter DAC

        [Serializable]
        [PXHidden]
        public class OpportunityActivityFilter : PX.Data.PXBqlTable, PX.Data.IBqlTable
        {
            #region ClassID
            public abstract class classID : BqlString.Field<classID> { }


            [PXString(255, IsUnicode = true)]
            [PXUIField(DisplayName = "Class")]
            [PXSelector(typeof(Search<CROpportunityClass.cROpportunityClassID>),
                        typeof(CROpportunityClass.cROpportunityClassID),
                DescriptionField = typeof(CROpportunityClass.description),ValidateValue = false)]
            public virtual string ClassID { get; set; }
            #endregion

            #region Status
            public abstract class status : BqlString.Field<status> { }

            [PXString(255, IsUnicode = true)]
            [PXUIField(DisplayName = "Not in Status")]
            [PXSelector(typeof(Search4<CROpportunity.status, 
                Aggregate<GroupBy<CROpportunity.status>>>),
                typeof(CROpportunity.status),
                ValidateValue = false)]
            public virtual string Status { get; set; }
            #endregion
        }

        #endregion
    }
}
```

**Database Impact:** No

**Note:** This graph needs a corresponding screen definition in the sitemap/ASPX page setup.

---

## 5. Helpers Changes

**Location:** `App_Data/Projects/CompiledVersion/CompiledVersion/Helpers/`

**Status:** ✅ No changes detected in this folder for this release.

---

## 6. Messages Updates

### 6.1 Update: Messages.cs
**File:** `App_Data/Projects/CompiledVersion/CompiledVersion/Messages/Messages.cs`

**Change 1:** Add PO NTE message

**Find the location after `FreightExceedsLimit` method and add:**

```csharp
private const string _poTotalExceedsNTE = "Total PO amount for Sales Order {0} ({1}) exceeds Not-To-Exceed limit ({2}) by {3}.";
public static string POTotalExceedsNTE(string soOrderNbr, decimal? totalPO, decimal? nteLimit, decimal? exceedAmt) =>
    PXLocalizer.LocalizeFormat(_poTotalExceedsNTE, soOrderNbr, totalPO, nteLimit, exceedAmt);
```

**Change 2:** Add email process message

**Find a suitable location (e.g., after attribute constants) and add:**

```7. Testing Checklist

### 7c const string RelatedEntityTypeRequired = "Related Entity Type is required for processing.";
#endregion
```

---

## 6. Testing Checklist

### 6.1 DAC Field Testing
- [ ] Verify new CROpportunity extension fields appear on Opportunity screen
- [ ] Test Referral Source field can accept 255 characters
- [ ] Test Release Hold Date field accepts dates
- [ ] Test Services Estimate accepts decimal values
- [ ] Test Activity Note field is unbound and displays correctly
- [ ] Verify POLine.UsrVendorNotes truncates at 250 characters
- [ ] Verify POLine.UsrShippingTerms shows ShipTerms selector
- [ ] Confirm POOrder.UsrFreightCost and UsrFreightPrice are read-only and calculated
- [ ] Verify SOSetup.UsrEnforcePONTE checkbox works

### 7.2 Business Logic Testing
- [ ] Test vendor notes concatenation with " | " separator when merging SO lines to PO
- [ ] Verify shipping terms only copies first value when merging
- [ ] Test freight cost/price calculation from linked SO invoices on PO
- [ ] Test NTE validation with warning mode (UsrEnforcePONTE = false)
- [ ] Test NTE validation with error mode (UsrEnforcePONTE = true)
- [ ] Verify NTE validation only applies to drop-ship orders
- [ ] Test invoice freight override uses UsrRTHCuryFreightTot from SO
- [ ] Verify SOFreightDetail records are updated correctly
- [ ] Test SO freight total calculation includes confirmed and invoiced shipments
- [ ] Verify shipping instructions only editable when SO is on Hold
- [ ] Test shipment freight limit validation excludes current shipment
- [ ] Verify freight limit includes both confirmed and invoiced shipments

### 7.3 New Graph Testing
- [ ] Verify SIOpportunityActivityProcess screen loads
- [ ] Test filtering by opportunity class (comma-delimited)
- [ ] Test filtering by status exclusion (comma-delimited)
- [ ] Verify activity note field is editable in the grid
- [ ] Test auto-selection when activity note is entered
- [ ] Verify activities are created with correct details
- [ ] Test bulk processing of multiple opportunities

### 7.4 Integration Testing
- [ ] Create SO → Generate PO → Verify fields copy correctly
- [ ] Create drop-ship PO → Verify NTE validation fires
- [ ] Create shipment → Confirm → Verify freight limits work
- [ ] Create invoice from shipment → Verify freight override works
- [ ] Test multi-SO to single PO scenario
- [ ] Test single SO to multi-PO scenario with NTE

### 7.5 Performance Testing
- [ ] Test freight calculations with large number of linked SOs/POs
- [ ] Verify no performance degradation on invoice save
- [ ] Test opportunity activity process with 100+ records

---

## 8. Deployment Steps

### 8.1 Pre-Deployment
1. **Backup Current System**
   - Export existing customization package
   - Backup database
   - Document current customization version

2. **Code Migration**
   - Apply all code changes in Customization Project editor
   - Compile and fix any compilation errors
   - Review all warnings

3. **Screen Updates**
   - Add new fields to applicable screens (if not auto-added)
   - Configure field properties and visibility
   - Test tab order and layout

### 8.2 Deployment
1. **Validate Customization Package**
   - Click "Validate" in Customization Project
   - Resolve all errors and warnings
   - Review schema changes

2. **Publish to Development**
   - Publish customization project
   - Monitor for compilation errors
   - Check application event log

3. **Database Updates**
   - Verify new columns created in database
   - Check for any orphaned columns from field type changes
   - Validate indexes and constraints

4. **Smoke Testing**
   - Test each modified screen
   - Verify no errors in browser console
   - Check for performance issues

### 8.3 Post-Deployment
1. **Functional Testing**
   - Execute full testing checklist (Section 6)
   - Test with actual business scenarios
   - Verify data integrity

2. **Documentation**
   - Update user documentation
   - Document configuration changes
   - Create training materials if needed

3. **Rollback Plan**
   - Keep previous customization package ready
   - Document rollback steps
   - Maintain database backup

---

## 9. Known Issues & Considerations

### 9.1 Breaking Changes
- **POLineExt.UsrVendorNotes**: Field size reduced from 500 to 250 characters
  - **Action Required**: Truncate existing data > 250 chars before deployment
  - **SQL Script**:
    ```sql
    UPDATE POLine
    SET UsrVendorNotes = LEFT(UsrVendorNotes, 250)
    WHERE LEN(UsrVendorNotes) > 250
    ```

- **POOrderExt.UsrFreightCost/UsrFreightPrice**: Changed from bound to unbound
  - **Action Required**: Existing data will be lost; fields now calculated dynamically
  - **Mitigation**: Export existing data if historical values needed

- **POLineExt.UsrShippingTerms**: Changed from char(1) to string(10)
  - **Action Required**: Existing SOShipComplete values will be invalid
  - **Mitigation**: Data migration script required to map old values to ShipTerms

### 9.2 Dependencies
- **POInventoryCustomization namespace**: Ensure this namespace/customization package exists
  - Used in SOInvoiceEntry_Extension.cs for SOOrderExt.UsrRTHCuryFreightTot
  - Verify namespace name matches your implementation

- **SOOrderExt.UsrRTHCuryFreightTot**: Must exist in your system
  - Required for invoice freight override functionality
  - If field doesn't exist, comment out related code in SOInvoiceEntry_Extension.cs

### 9.3 Performance Considerations
- Freight calculations query multiple tables (SO, PO, Invoice, Shipment)
- Consider adding indexes on:
  - DropShipLink (POOrderType, POOrderNbr, SOOrderType, SOOrderNbr)
  - SOOrderShipment (OrderType, OrderNbr, ShipmentNbr)

### 9.4 Version-Specific Notes
- Code developed on version 24.112.006
- Target version 2025 R2 may have different APIs
- Test thoroughly for API compatibility
- Check Acumatica release notes for breaking changes between versions

---

## 10. Support & Troubleshooting

### Common Errors

**Error:** "Field UsrRTHCuryFreightTot not found"
- **Solution**: Verify SOOrderExt contains this field or comment out SOInvoiceEntry freight override

**Error:** "Cannot convert PXDBDecimal to PXDecimal"
- **Solution**: Ensure POOrderExt freight fields don't have PXDBDecimal attribute

**Error:** "Namespace POInventoryCustomization not found"
- **Solution**: Verify namespace or update using statement in SOInvoiceEntry_Extension.cs

**Error:** NTE validation not firing
- **Solution**: Check SOSetup.UsrNotToExceed is configured with correct ShipTerms

**Error:** Activity process fails
- **Solution**: Verify CROpportunityExt2 extension is active (IsActive() returns true)

---

## 11. Appendix

### A. Folder Structure Reference

All changes are within the CompiledVersion customization project:

```
App_Data/
└── Projects/
    └── CompiledVersion/
        └── CompiledVersion/
            ├── Ameer/
            │   ├── DAC/
            │   │   └── CROpportunityClassStageReason.cs (Modified)
            │   ├── Extension/
            │   │   ├── DAC/
            │   │   │   ├── CROpportunityExt.cs (NEW)
            │   │   │   ├── CROpportunityReasonExt.cs (Modified)
            │   │   │   └── StandaloneCROpportunityExt.cs (NEW)
            │   │   └── Graphs/
            │   │       ├── CROpportunityClassMaint_StageReasons.cs (Modified)
            │   │       ├── OpportunityMaint_OpenCustomExt.cs (Modified)
            │   │       ├── OpportunityMaint_ReasonSyncExt.cs (Modified)
            │   │       └── OpportunityMaint_StageReasonExt.cs (Modified)
            │   └── Graphs/
            │       └── SIOpportunityActivityProcess.cs (NEW)
            ├── DAC/ (No changes)
            ├── DACExt/
            │   ├── POLineExt.cs (Modified)
            │   ├── POOrderExt.cs (Modified)
            │   ├── SOLineExt.cs (No changes)
     Helpers Files
- [ ] No changes in this folder

#### Messages Files
- [ ] Messages.cs (Modified)

### C       │   ├── POOrderEntry_Extension.cs (Modified - Major)
            │   ├── SOInvoiceEntry_Extension.cs (Modified - Major)
            │   ├── SOOrderEntry_Extension.cs (Modified)
            │   └── SOShipmentEntry_Extension.cs (Modified - Major)
            ├── Graphs/ (No changes in main folder, see Ameer/Graphs)
            ├── Helpers/ (No changes)
            └── Messages/
                └── Messages.cs (Modified)
```

### B. File Checklist
Use this checklist to track migration progress:

#### DAC Files
- [ D CROpportunityClassStageReason.cs (Modified)

#### DACExt Files
- [ ] CROpportunityExt.cs (NEW - Ameer/Extension/DAC)
- [ ] CROpportunityReasonExt.cs (Modified)
- [ ] StandaloneCROpportunityExt.cs (NEW)
- [ ] POLineExt.cs (Modified)
- [ ] POOrderExt.cs (Modified)
- [ ] SOLineExt.cs (No changes)
- [ ] SOSetupExt.cs (Modified)

#### GraphExt Files
- [ ] CROpportunityClassMaint_StageReasons.cs (Modified)
- [ ] OpportunityMaint_OpenCustomExt.cs (Modified)
- [ ] OpportunityMaint_ReasonSyncExt.cs (Modified)
- [ ] OpportunityMaint_StageReasonExt.cs (Modified)
- [ ] POCreateReplaceOriginalMethod.cs (Modified)
- [ ] POOrderEntry_Extension.cs (Modified - Major)
- [ ] SOInvoiceEntry_Extension.cs (Modified - Major)
- [ ] SOOrderEntry_Extension.cs (Modified)
- [ ] SOShipmentEntry_Extension.cs (Modified - Major)

#### Graph Files
- [ ] SIOpportunityActivityProcess.cs (NEW)

#### Other Files
- [ ] Messages.cs (Modified)

### B. SQL Scripts for Data Migration

#### Check POLine Vendor Notes > 250 chars
```sql
SELECT LineNbr, OrderType, OrderNbr, LEN(UsrVendorNotes) as Length, UsrVendorNotes
FROM POLine
WHERE LEN(UsrVendorNotes) > 250
ORDER BY Length DESC
```

#### Truncate Vendor Notes
```sql
UPDATE POLine
SET UsrVendorNotes = LEFT(UsrVendorNotes, 250)
WHERE LEN(UsrVendorNotes) > 250
```

### C. Configuration Required

**SO Setup (SO201000)**
- Configure UsrNotToExceed field with ShipTerms ID for NTE orders
- Set UsrEnforcePONTE to true/false based on business requirement

**Opportunity Classes**
- Verify stages are configured if using OpportunityMaint extensions

**Ship Terms**
- Ensure ShipTerms master data is populated for POLine.UsrShippingTerms selector

---

**END OF MIGRATION GUIDE**

---

## Questions or Issues?
Document any issues encountered during migration and their resolutions for future reference.

**Migration Started:** ______________  
**Migration Completed:** ______________  
**Migrated By:** ______________  
**Verified By:** ______________

# CompiledVersion Project - Complete Documentation
**Generated**: December 17, 2025  
**Acumatica Version**: 24.112.006  
**Project**: CompiledVersion24R1

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Graph Extensions (Modules Affected)](#graph-extensions-modules-affected)
3. [DAC Extensions (Database Fields)](#dac-extensions-database-fields)
4. [Custom DACs (New Tables)](#custom-dacs-new-tables)
5. [Custom Graphs (New Screens)](#custom-graphs-new-screens)
6. [Module Connections & Data Flow](#module-connections--data-flow)
7. [Process Flow Diagram Notes](#process-flow-diagram-notes)

---

## Project Overview

The **CompiledVersion** customization project extends Acumatica ERP with custom functionality across multiple modules including:
- **Sales Orders (SO)** - Freight management, cost tracking, order processing
- **Purchase Orders (PO)** - Drop-ship handling, cost management, automated PO creation
- **Inventory (IN)** - Cost tracking, item requests, custom fields
- **Shipments** - Cost preservation, freight validation, invoice creation
- **CRM** - Opportunity tracking, HubSpot integration, quote management
- **Invoicing (AR)** - Custom freight calculations, email capture

---

## Graph Extensions (Modules Affected)

### Sales Order Module (SO)

#### 1. **SOOrderEntry_Extension**
- **File**: `GraphExt/SOOrderEntry_Extension.cs`
- **Base Graph**: `PX.Objects.SO.SOOrderEntry`
- **Functions**:
  - **Sales People Management**: Custom view for managing sales commission splits
  - **RTH Total Calculations**: Maintains "Ready-to-Hold" totals (RTH) that represent committed/confirmed values
  - **Freight Limit Enforcement**: Validates freight amounts against customer-defined limits
  - **Cost Tracking**: Tracks and updates custom cost fields (RTH costs)
  - **Shipping Instructions**: Copies custom shipping notes to related documents
  - **Line Renumbering**: Custom logic to maintain line order during edits
  - **Document Deletion**: Special handling when entire orders are deleted
- **Key Customizations**:
  - Adds custom sales people grid
  - Calculates RTH totals for freight, discounts, tax, and order totals
  - Enforces freight limits based on shipping terms
  - Preserves cost information throughout the order lifecycle

#### 2. **SOShipmentEntry_Extension**
- **File**: `GraphExt/SOShipmentEntry_Extension.cs`
- **Base Graph**: `PX.Objects.SO.SOShipmentEntry`
- **Functions**:
  - **Cost Preservation**: Ensures SOLine costs are copied to INTran (inventory transactions)
  - **Freight Validation**: Prevents freight from exceeding order-level limits
  - **Invoice Creation Control**: Validates non-stock items before allowing invoice creation
  - **Shipping Notes**: Copies internal shipping notes from order to shipment
  - **Freight Total Update**: Updates order-level freight total when shipment is confirmed
  - **Cost Override**: Forces INTran and SOShipLine to use costs from SOLine (not inventory costs)
  - **Lot/Serial Cost Tracking**: Special handling for specific identification costing
- **Key Event Handlers**:
  - `RowInserted<INTran>`: Sets costs from SOLine
  - `RowPersisting<INTran>`: Prevents cost recalculation before save
  - `RowPersisting<SOShipLine>`: Overrides unit costs
  - `ConfirmShipment`: Updates freight totals on order
  - `CreateInvoice`: Validates non-stock items
  - `InvoiceShipment`: Copies form type attribute
  - `PostShipment`: Updates INTran costs after posting

#### 3. **SOInvoiceEntry_Extension**
- **File**: `GraphExt/SOInvoiceEntry_Extension.cs`
- **Base Graph**: `PX.Objects.SO.SOInvoiceEntry`
- **Functions**:
  - **Custom Freight Calculation**: Uses SOOrder.UsrRTHCuryFreightTot instead of calculated freight
  - **Freight Detail Management**: Updates SOFreightDetail records to match custom freight
  - **Non-Stock Item Validation**: Prevents invoicing of flagged non-stock items
  - **Sales People Grid**: Displays sales commission splits on invoice
- **Key Features**:
  - Overrides freight total right before invoice save
  - Sets first freight detail to full amount, zeros out others
  - Integrates with freight limit enforcement from order

#### 4. **SOCreateShipment_Extension**
- **File**: `GraphExt/SOCreateShipment_Extension.cs`
- **Base Graph**: `PX.Objects.SO.SOCreateShipment`
- **Functions**:
  - Custom shipment creation logic
  - Batch processing enhancements

#### 5. **SOReleaseInvoice_Extension**
- **File**: `GraphExt/SOReleaseInvoice_Extension.cs`
- **Base Graph**: `PX.Objects.SO.SOReleaseInvoice`
- **Functions**:
  - Custom logic during invoice release process

#### 6. **SOSetupMaint_Extension**
- **File**: `GraphExt/SOSetupMaint_Extension.cs`
- **Base Graph**: `PX.Objects.SO.SOSetupMaint`
- **Functions**:
  - Custom SO setup configuration options

#### 7. **SOOrderEntryCopyOppFieldsExt**
- **File**: `GraphExt/SOOrderEntryCopyOppFieldsExt.cs`
- **Base Graph**: `PX.Objects.SO.SOOrderEntry`
- **Functions**:
  - Copies opportunity fields to sales order during conversion

---

### Purchase Order Module (PO)

#### 8. **POOrderEntry_Extension**
- **File**: `GraphExt/POOrderEntry_Extension.cs`
- **Base Graph**: `PX.Objects.PO.POOrderEntry`
- **Functions**:
  - **RTH Cost Tracking**: Calculates "Ready-to-Hold" costs for PO lines
  - **Drop-Ship Integration**: Links PO lines to SO lines for drop-ship orders
  - **Cost Management**: Handles freight costs and pricing from linked sales orders
  - **Prepayment Handling**: Special logic for prepayment lines
  - **SO Field Visibility**: Controls visibility of SO-related fields based on linked orders
- **Key Features**:
  - Recalculates RTH header totals when lines change
  - Manages drop-ship links between PO and SO
  - Copies freight costs from sales orders to purchase orders

#### 9. **POReceiptEntry_Extension**
- **File**: `GraphExt/POReceiptEntry_Extension.cs`
- **Base Graph**: `PX.Objects.PO.POReceiptEntry`
- **Functions**:
  - Custom receipt processing
  - Cost updates during receipt

#### 10. **POCreate Extensions (Multiple)**
- **Files**: 
  - `GraphExt/POCreate_2.cs`
  - `GraphExt/POCreate3.cs`
  - `GraphExt/POCreate4.cs`
  - `GraphExt/POCreateView.cs`
  - `GraphExt/POCreateReplaceOriginalMethod.cs`
- **Base Graph**: `PX.Objects.PO.POCreate`
- **Functions**:
  - **Automated PO Creation**: Custom logic for creating POs from demands
  - **Demand Filtering**: Advanced filtering for purchase requisitions
  - **Vendor Selection**: Custom vendor selection logic
  - **View Customizations**: Custom views and filters
  - **Validation Logic**: Business rules for PO creation

---

### Inventory Module (IN)

#### 11. **InventoryItemMaintExt**
- **File**: `GraphExt/InventoryItemMaintExt.cs`
- **Base Graph**: `PX.Objects.IN.InventoryItemMaint`
- **Functions**:
  - Custom inventory item maintenance
  - Additional field management

#### 12. **NonStockItemMaintExt**
- **File**: `GraphExt/NonStockItemMaintExt.cs`
- **Base Graph**: `PX.Objects.IN.NonStockItemMaint`
- **Functions**:
  - Custom non-stock item maintenance
  - Special handling for service items

---

### CRM Module

#### 13. **OpportunityMaintExt**
- **File**: `GraphExt/OpportunityMaintExt.cs`
- **Base Graph**: `PX.Objects.CR.OpportunityMaint`
- **Functions**:
  - HubSpot integration
  - Custom opportunity field management

#### 14. **QuoteMaint_Extension**
- **File**: `GraphExt/QuoteMaint_Extension.cs`
- **Base Graph**: `PX.Objects.CR.QuoteMaint`
- **Functions**:
  - Custom quote management
  - HubSpot deal integration

#### 15. **QuoteMaintCRCreateSalesOrderExt**
- **File**: `GraphExt/QuoteMaintCRCreateSalesOrderExt.cs`
- **Base Graph**: `PX.Objects.CR.QuoteMaint.CRCreateSalesOrderExt`
- **Functions**:
  - Custom logic for converting quotes to sales orders

---

### Customer Module

#### 16. **TestCustomerMaint_Extension**
- **File**: `GraphExt/TestCustomerMaint_Extension.cs`
- **Functions**:
  - Test/demo customer maintenance extensions

---

## DAC Extensions (Database Fields)

### Sales Order Extensions

#### 1. **SOOrderExt**
- **File**: `DACExt/SOOrderExt.cs`
- **Table Extended**: `PX.Objects.SO.SOOrder`
- **Custom Fields**:
  - **UsrFreightPriceLimit** (Decimal): Maximum freight amount allowed
  - **UsrFreightTotal** (Decimal): Calculated total freight from shipments
  - **UsrShippingInstructions** (String): Customer-facing shipping instructions
  - **UsrShippingNotes** (String): Internal shipping notes
  - **UsrCustomerAccount** (String): Customer's account number
  - **UsrRTHOrderQty** (Decimal): Ready-to-hold ordered quantity
  - **UsrRTHCuryDetailExtPriceTotal** (Decimal): RTH detail price total
  - **UsrRTHCuryLineDiscTotal** (Decimal): RTH line discount total
  - **UsrRTHCuryDiscTot** (Decimal): RTH document discount total
  - **UsrRTHCuryFreightTot** (Decimal): RTH freight total (used for invoicing)
  - **UsrRTHCuryTaxTotal** (Decimal): RTH tax total
  - **UsrRTHCuryOrderTotal** (Decimal): RTH order total
  - **UsrInternalNotes** (String): Internal notes

#### 2. **SOLineExt**
- **File**: `DACExt/SOLineExt.cs`
- **Table Extended**: `PX.Objects.SO.SOLine`
- **Custom Fields**:
  - **UsrSWKRTHCost** (Decimal): RTH cost tracking
  - **UsrSWKSPCCost** (Decimal): Special cost field
  - Custom location and cost fields

#### 3. **SOLineSplit3Ext**
- **File**: `DACExt/SOLineSplit3Ext.cs`
- **Table Extended**: `PX.Objects.SO.SOLineSplit3`

#### 4. **SOShipmentExt**
- **File**: `DACExt/SOShipmentExt.cs`
- **Table Extended**: `PX.Objects.SO.SOShipment`
- **Custom Fields**:
  - **UsrShippingNotes** (String): Internal shipping notes copied from order

#### 5. **SOOrderTypeExt**
- **File**: `DACExt/SOOrderTypeExt.cs`
- **Table Extended**: `PX.Objects.SO.SOOrderType`

#### 6. **SOSetupExt**
- **File**: `DACExt/SOSetupExt.cs`
- **Table Extended**: `PX.Objects.SO.SOSetup`
- **Custom Fields**:
  - **UsrNotToExceed** (String): Shipping terms ID for "not to exceed" freight
  - **UsrPrepayAndAdd** (String): Shipping terms ID for prepay and add
  - **UsrFreeFreightAllowed** (String): Shipping terms ID for free freight
  - **UsrNonstock1, UsrNonstock2, UsrNonstock3** (Int): Flagged non-stock items

---

### Purchase Order Extensions

#### 7. **POOrderExt**
- **File**: `DACExt/POOrderExt.cs`
- **Table Extended**: `PX.Objects.PO.POOrder`
- **Custom Fields**:
  - **UsrRTHOrderQty** (Decimal): RTH ordered quantity
  - **UsrRTHCuryDetailExtPriceTotal** (Decimal): RTH detail total
  - **UsrRTHCuryLineDiscTotal** (Decimal): RTH line discounts
  - **UsrRTHCuryDiscTot** (Decimal): RTH document discounts
  - **UsrRTHCuryFreightTot** (Decimal): RTH freight total
  - **UsrRTHCuryTaxTotal** (Decimal): RTH tax total
  - **UsrRTHCuryOrderTotal** (Decimal): RTH order total
  - **UsrShowFreightCost** (Bool): Show freight cost fields
  - **UsrShowFreightPrice** (Bool): Show freight price fields

#### 8. **POLineExt**
- **File**: `DACExt/POLineExt.cs`
- **Table Extended**: `PX.Objects.PO.POLine`
- **Custom Fields**:
  - **UsrSWKRTHCost** (Decimal): RTH cost
  - **UsrSWKSPCCost** (Decimal): Special cost
  - **UsrPrepaymentLine** (Bool): Identifies prepayment lines
  - Custom location fields

#### 9. **POReceiptExt**
- **File**: `DACExt/POReceiptExt.cs`
- **Table Extended**: `PX.Objects.PO.POReceipt`

#### 10. **POCreateFilterExt**
- **File**: `DACExt/POCreateFilterExt.cs`
- **Table Extended**: `PX.Objects.PO.POCreateFilter`

#### 11. **POFixedDemandExt**
- **File**: `DACExt/POFixedDemandExt.cs`
- **Table Extended**: `PX.Objects.PO.POFixedDemand`

---

### Inventory Extensions

#### 12. **InventoryItemExt**
- **File**: `DACExt/InventoryItemExt.cs`
- **Table Extended**: `PX.Objects.IN.InventoryItem`
- **Custom Fields**:
  - **UsrSWKRTHCost** (Decimal): RTH cost
  - **UsrSWKSPCCost** (Decimal): Special cost
  - **UsrSWKSPCCode** (String): Special code

#### 13. **INSetupExt**
- **File**: `DACExt/INSetupExt.cs`
- **Table Extended**: `PX.Objects.IN.INSetup`
- **Custom Fields**:
  - **UsrDefaultWarehouse** (Int): Default warehouse for item requests
  - **UsrProductBrandAttributeID** (String): Product brand attribute
  - **UsrItemRequestNumberingID** (String): Numbering sequence for item requests

---

### CRM Extensions

#### 14. **CROpportunityExt**
- **File**: `DACExt/CROpportunityExt.cs`
- **Table Extended**: `PX.Objects.CR.CROpportunity`
- **Custom Fields**:
  - **UsrHubspotDealID** (String): HubSpot Deal ID integration

#### 15. **CROpportunityProductsExt**
- **File**: `DACExt/CROpportunityProductsExt.cs`
- **Table Extended**: `PX.Objects.CR.CROpportunityProducts`
- **Custom Fields**:
  - **UsrSWKRTHCost** (Decimal): RTH cost on opportunity products
  - **UsrSWKSPCCost** (Decimal): Special cost
  - **UsrSWKSPCCode** (String): Special code

#### 16. **CRQuoteExt**
- **File**: `DACExt/CRQuoteExt.cs`
- **Table Extended**: `PX.Objects.CR.CRQuote`
- **Custom Fields**:
  - **UsrHubspotDealID** (String): HubSpot Deal ID

---

### AR/Customer Extensions

#### 17. **ARInvoiceExt**
- **File**: `DACExt/ARInvoiceExt.cs`
- **Table Extended**: `PX.Objects.AR.ARInvoice`
- **Custom Fields**:
  - **UsrEmail** (String): Customer email captured from contact

#### 18. **CustomerExt**
- **File**: `DACExt/CustomerExt.cs`
- **Table Extended**: `PX.Objects.AR.Customer`
- **Custom Fields**:
  - **UsrShippingInstructions** (String): Default shipping instructions

---

## Custom DACs (New Tables)

### 1. **InventoryRequest**
- **File**: `DAC/InventoryRequest.cs`
- **Purpose**: Item request/creation workflow
- **Screen**: ST301001
- **Key Fields**:
  - **RefNbr** (String): Reference number (key)
  - **InventoryCD** (String): Proposed inventory ID
  - **InventoryID** (Int): Created inventory item
  - **ItemDescription** (String): Item description
  - **ItemClassID** (Int): Item class
  - **PostClassID** (String): Posting class
  - **TaxCategoryID** (Int): Tax category
  - **DefaultWarehouse** (Int): Default warehouse
  - **PartNumber** (String): Manufacturer part number
  - **ProductBrand** (String): Product brand
  - **Status** (String): Request status
  - **DateSubmitted** (DateTime): Submission date
  - **RequestorName** (String): Requestor
  - **StdUnitOfMeasure** (String): UOM
  - **PreferredVendorID** (Int): Preferred vendor
- **Purpose**: Allows users to request new inventory items with approval workflow

### 2. **SOCustSalesPeople**
- **File**: `DAC/SOCustSalesPeople.cs`
- **Purpose**: Sales commission splits on orders
- **Key Fields**:
  - **OrderType** (String): Order type
  - **OrderNbr** (String): Order number
  - **SalespersonID** (Int): Salesperson
  - **CommissionPct** (Decimal): Commission percentage
- **Purpose**: Tracks multiple salespeople and their commission splits per order

### 3. **CROpportunityClassStageReason** (Ameer folder)
- **File**: `Ameer/DAC/CROpportunityClassStageReason.cs`
- **Purpose**: Stage reasons for opportunity management
- **Related Extensions**:
  - `Ameer/Extension/DAC/CROpportunityReasonExt.cs`
  - `Ameer/Extension/Graphs/CROpportunityClassMaint_StageReasons.cs`
  - `Ameer/Extension/Graphs/OpportunityMaint_ReasonSyncExt.cs`
  - `Ameer/Extension/Graphs/OpportunityMaint_StageReasonExt.cs`
  - `Ameer/Extension/Graphs/OpportunityMaint_OpenCustomExt.cs`
  - `Ameer/Extension/Workflows/OpportunityMaint_WorkflowFormExt.cs`

---

## Custom Graphs (New Screens)

### 1. **ItemRequestEntry**
- **File**: `Graphs/ItemRequestEntry.cs`
- **Screen ID**: ST301001
- **Purpose**: Item Request Management
- **Functions**:
  - Create inventory item requests
  - Validate required fields
  - Auto-numbering support
  - Create stock items from requests
  - Product brand validation
- **Key Actions**:
  - **CreateStockItem**: Validates request and creates inventory item
- **Integration**:
  - Creates `InventoryItem` records
  - Uses `INSetupExt` for default warehouse
  - Validates product brand from item class

---

## Module Connections & Data Flow

### 1. Order-to-Cash Process
```
Sales Order (SOOrderEntry) 
    ↓
    ├── Creates RTH Totals (committed values)
    ├── Sets Freight Limit
    ├── Copies Shipping Instructions
    └── Links to Sales People
    ↓
Shipment (SOShipmentEntry)
    ↓
    ├── Validates Freight vs Limit
    ├── Preserves SOLine Costs to INTran
    ├── Updates Order Freight Total
    └── Validates Non-Stock Items
    ↓
Invoice (SOInvoiceEntry)
    ↓
    ├── Uses SOOrder.UsrRTHCuryFreightTot
    ├── Updates SOFreightDetail records
    ├── Validates Non-Stock Items
    └── Displays Sales People
    ↓
Release (SOReleaseInvoice)
```

### 2. Drop-Ship Process
```
Sales Order (Drop-Ship)
    ↓
    ├── SOOrderEntry creates order
    └── Sets drop-ship line type
    ↓
PO Create (POCreate)
    ↓
    ├── Filters drop-ship demands
    ├── Creates linked PO
    └── Establishes SO-PO link
    ↓
Purchase Order (POOrderEntry)
    ↓
    ├── Shows linked SO fields
    ├── Copies freight costs from SO
    └── Manages drop-ship link
    ↓
PO Receipt (POReceiptEntry)
    ↓
    └── Updates linked SO line
    ↓
Invoice (SOInvoiceEntry)
    ↓
    └── Creates invoice from receipt
```

### 3. Cost Tracking Flow
```
Opportunity (OpportunityMaint)
    ↓ (UsrSWKRTHCost, UsrSWKSPCCost)
Quote (QuoteMaint)
    ↓ (copies costs)
Sales Order (SOOrderEntry)
    ↓ (SOLineExt.UsrSWKRTHCost)
Shipment (SOShipmentEntry)
    ↓ (copies to SOShipLine.UnitCost)
INTran (Inventory Transaction)
    ↓ (preserves cost via RowPersisting)
Invoice (SOInvoiceEntry)
    └── (cost reflected in COGS)
```

### 4. Freight Management Flow
```
SO Setup (SOSetupExt)
    ├── UsrNotToExceed (shipping terms)
    ├── UsrPrepayAndAdd (shipping terms)
    └── UsrFreeFreightAllowed (shipping terms)
    ↓
Sales Order (SOOrderExt)
    ├── UsrFreightPriceLimit (customer limit)
    └── UsrRTHCuryFreightTot (committed freight)
    ↓
Shipment (SOShipmentEntry)
    ├── Validates freight vs limit
    └── Updates order freight total
    ↓
Invoice (SOInvoiceEntry)
    ├── Uses RTH freight total
    └── Overrides calculated freight
```

### 5. Item Request Workflow
```
Item Request Entry (ST301001)
    ↓
    ├── User fills request form
    ├── Selects Item Class
    ├── Enters description, UOM
    ├── Selects warehouse, vendor
    └── Validates product brand
    ↓
Create Stock Item Action
    ↓
    ├── Validates required fields
    ├── Checks auto-numbering
    ├── Creates InventoryItem
    └── Returns new item CD
```

### 6. HubSpot Integration
```
Opportunity (CROpportunityExt)
    ↓ (UsrHubspotDealID)
Quote (CRQuoteExt)
    ↓ (UsrHubspotDealID)
Sales Order
    └── (linked via opportunity)
```

---

## Process Flow Diagram Notes

### Key Process Flows to Diagram:

#### 1. **Order-to-Invoice with Freight Management**
- **Entry Points**: SOOrderEntry
- **Decision Points**: 
  - Freight limit check (shipping terms = NotToExceed?)
  - Non-stock item validation
  - RTH vs actual calculations
- **Data Modifications**:
  - SOOrderExt fields updated
  - SOShipmentEntry updates freight totals
  - SOInvoiceEntry overrides freight
- **Exit Points**: Released AR Invoice

#### 2. **Cost Preservation Flow**
- **Entry Points**: SOOrderEntry (SOLine cost entry)
- **Critical Points**:
  - SOShipmentEntry.RowPersisting<SOShipLine> (cost override)
  - SOShipmentEntry.RowInserted<INTran> (cost copy)
  - SOShipmentEntry.RowPersisting<INTran> (cost lock)
  - SOShipmentEntry.PostShipment (database update)
- **Data Flow**: SOLine → SOShipLine → INTran
- **Exit Points**: Posted inventory transaction

#### 3. **Drop-Ship Procurement**
- **Entry Points**: SOOrderEntry (drop-ship order)
- **Parallel Processes**:
  - SO processing
  - PO creation via POCreate
  - Drop-ship link maintenance
- **Integration Points**:
  - POOrderEntry shows SO fields
  - POReceiptEntry updates SO
  - SOInvoiceEntry creates AR invoice
- **Exit Points**: Invoiced drop-ship sale

#### 4. **RTH Calculations**
- **Triggers**: Any change to SOOrder or SOLine
- **Calculations**:
  - UsrRTHOrderQty (sum of line quantities)
  - UsrRTHCuryDetailExtPriceTotal (goods + misc)
  - UsrRTHCuryLineDiscTotal (line discounts)
  - UsrRTHCuryDiscTot (document discounts)
  - UsrRTHCuryFreightTot (freight + premium)
  - UsrRTHCuryTaxTotal (taxes)
  - UsrRTHCuryOrderTotal (grand total)
- **Purpose**: Snapshot of confirmed/committed amounts
- **Usage**: Invoice creation uses RTH values

#### 5. **Sales Commission Split**
- **Entry Points**: SOOrderEntry
- **DAC**: SOCustSalesPeople
- **Display**: Custom grid in SOOrderEntry
- **Copy**: SOInvoiceEntry shows same splits
- **Purpose**: Track multiple salespeople per order

### Visual Elements for Diagrams:

#### **Swimlanes**:
1. Sales Order Entry
2. Shipment Processing
3. Invoice Creation
4. Purchase Order (for drop-ship)
5. Inventory Management

#### **Decision Nodes**:
- Is freight limit set?
- Does freight exceed limit?
- Are there non-stock items flagged?
- Is this a drop-ship order?
- Is RTH value different from actual?

#### **Data Stores**:
- SOOrder + SOOrderExt
- SOLine + SOLineExt
- SOShipment + SOShipmentExt
- SOShipLine (with cost overrides)
- INTran (with preserved costs)
- ARInvoice + ARInvoiceExt
- SOFreightDetail
- SOCustSalesPeople

#### **Integration Points**:
- SOOrder ↔ POOrder (drop-ship)
- SOLine → SOShipLine → INTran (cost flow)
- SOOrder → ARInvoice (RTH freight)
- Opportunity → Quote → SOOrder (HubSpot ID)

#### **Color Coding Suggestion**:
- 🔵 **Blue**: Standard Acumatica process
- 🟢 **Green**: Custom validation/logic
- 🟡 **Yellow**: Data transformation
- 🔴 **Red**: Critical decision point
- 🟣 **Purple**: Integration point

### Critical Business Rules to Document:

1. **Freight Limit Enforcement**:
   - Only applies when ShipTermsID = UsrNotToExceed
   - Validated during shipment freight entry
   - Sum of all confirmed shipments cannot exceed order limit
   - Error prevents saving if exceeded

2. **Cost Preservation**:
   - SOLine.CuryUnitCost must flow to INTran.UnitCost
   - Override flag must be set to prevent recalculation
   - Special handling for specific identification (lot/serial)
   - Critical for margin reporting accuracy

3. **RTH Values**:
   - Represent committed/confirmed amounts
   - Calculated when freight limit conditions are met
   - Used as source for invoice freight (not calculated freight)
   - Updated on shipment confirmation

4. **Non-Stock Validation**:
   - Three configurable non-stock items in SOSetup
   - Prevents these items from being invoiced
   - Validation occurs in both shipment and invoice screens

5. **Drop-Ship Integration**:
   - SO and PO must remain linked
   - Freight costs copy from SO to PO
   - Receipt updates SO line quantities
   - Invoice created from receipt, not shipment

---

## File Structure Summary

```
CompiledVersion/
├── DAC/                          # Custom Tables
│   ├── InventoryRequest.cs       # Item request management
│   └── SOCustSalesPeople.cs      # Sales commission splits
│
├── DACExt/                       # Database Field Extensions
│   ├── ARInvoiceExt.cs          # Invoice extensions
│   ├── CROpportunityExt.cs      # Opportunity extensions
│   ├── CROpportunityProductsExt.cs
│   ├── CRQuoteExt.cs            # Quote extensions
│   ├── CustomerExt.cs           # Customer extensions
│   ├── INSetupExt.cs            # IN Setup extensions
│   ├── InventoryItemExt.cs      # Item extensions
│   ├── POCreateFilterExt.cs     # PO Create filter
│   ├── POFixedDemandExt.cs      # PO Demand
│   ├── POLineExt.cs             # PO Line extensions
│   ├── POOrderExt.cs            # PO Order extensions
│   ├── POReceiptExt.cs          # PO Receipt extensions
│   ├── SOLineExt.cs             # SO Line extensions
│   ├── SOLineSplit3Ext.cs       # SO Line split
│   ├── SOOrderExt.cs            # SO Order extensions (major)
│   ├── SOOrderTypeExt.cs        # SO Order Type
│   ├── SOSetupExt.cs            # SO Setup extensions
│   └── SOShipmentExt.cs         # SO Shipment extensions
│
├── GraphExt/                     # Screen/Process Extensions
│   ├── InventoryItemMaintExt.cs # Inventory maintenance
│   ├── NonStockItemMaintExt.cs  # Non-stock maintenance
│   ├── OpportunityMaintExt.cs   # Opportunity screen
│   ├── POCreate*.cs             # PO Create (5 files)
│   ├── POOrderEntry_Extension.cs # PO Entry (major)
│   ├── POReceiptEntry_Extension.cs # PO Receipt
│   ├── QuoteMaint*.cs           # Quote maintenance (2 files)
│   ├── SOCreateShipment_Extension.cs # Create Shipment
│   ├── SOInvoiceEntry_Extension.cs # Invoice Entry (major)
│   ├── SOOrderEntryCopyOppFieldsExt.cs
│   ├── SOOrderEntry_Extension.cs # SO Entry (major)
│   ├── SOReleaseInvoice_Extension.cs # Invoice Release
│   ├── SOSetupMaint_Extension.cs # SO Setup
│   ├── SOShipmentEntry_Extension.cs # Shipment Entry (major)
│   └── TestCustomerMaint_Extension.cs
│
├── Graphs/                       # Custom Screens
│   └── ItemRequestEntry.cs      # Item Request (ST301001)
│
├── Helpers/                      # Utility Classes
│   └── Helper.cs                # Long operation helper
│
├── Messages/                     # Custom Messages
│   └── Messages.cs              # Error/info messages
│
└── Ameer/                        # Ameer-specific customizations
    ├── DAC/
    │   └── CROpportunityClassStageReason.cs
    └── Extension/
        ├── DAC/
        ├── Graphs/
        └── Workflows/
```

---

## Dependencies & Integration Map

### Internal Dependencies:
- **SOOrderEntry_Extension** ← uses → **SOOrderExt, SOLineExt, SOSetupExt**
- **SOShipmentEntry_Extension** ← uses → **SOOrderExt, SOShipmentExt, Messages**
- **SOInvoiceEntry_Extension** ← uses → **SOOrderExt, ARInvoiceExt, SOFreightDetail**
- **POOrderEntry_Extension** ← uses → **POOrderExt, POLineExt, SOOrderExt**
- **ItemRequestEntry** ← uses → **InventoryRequest, INSetupExt**

### External System Integration:
- **HubSpot**: Via CROpportunityExt.UsrHubspotDealID, CRQuoteExt.UsrHubspotDealID
- **Email Systems**: Via ARInvoiceExt.UsrEmail

### Data Flow Dependencies:
1. **Cost Flow**: OpportunityProducts → Quote → SOLine → SOShipLine → INTran
2. **Freight Flow**: SOSetup → SOOrder → SOShipment → ARInvoice
3. **Sales People**: SOCustSalesPeople → displayed in SO and AR
4. **Drop-Ship**: SOOrder ↔ POOrder (bidirectional link)

---

## Testing Scenarios

### 1. Freight Limit Enforcement
- Create SO with "Not To Exceed" shipping terms
- Set freight limit to $100
- Create shipment with freight $60
- Confirm shipment
- Create second shipment with freight $50
- **Expected**: Error prevents saving second shipment

### 2. Cost Preservation
- Create SO with custom cost on line
- Create and confirm shipment
- Update IN (create inventory issue)
- View INTran record
- **Expected**: INTran.UnitCost = SOLine.CuryUnitCost

### 3. Custom Freight on Invoice
- Create SO with RTH Freight Total = $50
- Create shipment with calculated freight = $75
- Prepare invoice
- **Expected**: Invoice freight total = $50 (not $75)

### 4. Non-Stock Item Validation
- Configure non-stock item in SO Setup
- Create SO with that item
- Create and confirm shipment
- Attempt to prepare invoice
- **Expected**: Error prevents invoice creation

### 5. Drop-Ship Process
- Create drop-ship SO
- Run PO Create
- **Expected**: PO created and linked to SO
- Verify SO freight costs copied to PO
- Receive PO
- **Expected**: SO line updated
- Create invoice from receipt
- **Expected**: AR invoice created

---

**End of Documentation**

Generated by GitHub Copilot
Date: December 17, 2025
Version: 1.0

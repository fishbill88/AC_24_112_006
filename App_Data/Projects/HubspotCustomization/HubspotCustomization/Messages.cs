using PX.Common;

namespace HubspotCustomization
{
    /// <summary>
    /// Localized messages for HubspotCustomization project
    /// </summary>
    [PXLocalizable]
    public static class Messages
    {
        #region Action Display Names
        public const string PrepareInvoice = "Prepare Invoice";
        public const string CreateCombinedInvoice = "Create Combined Invoice";
        public const string ScreenIDCreateCombinedInvoice = "SO302000$createCombinedInvoice";
        #endregion

        #region Exception Messages
        public const string NoInvoicesSelected = "No invoices selected.";
        public const string NoShipmentsSelected = "No shipments selected.";
        public const string ErrorProcessingInvoice = "Error processing invoice {0}: {1}";
        public const string ErrorProcessingShipment = "Error processing shipment {0}: {1}";
        public const string CannotInvoiceNonStockItems = "You cannot invoice non-stock items: {0}";
        public const string CannotInvoiceNonStockItem = "You cannot invoice this non-stock item.";
        #endregion

        #region Field Display Names
        public const string HubspotDealID = "HubSpot Deal ID";
        public const string Email = "Email";
        public const string VendorID = "Vendor";
        public const string VendorLocationID = "Vendor Location";
        public const string VendorAddress = "Vendor Address";
        public const string NonStock1 = "Non-Stock Item 1";
        public const string NonStock2 = "Non-Stock Item 2";
        public const string NonStock3 = "Non-Stock Item 3";
        public const string HidePrintingMethod = "Hide Printing Method";
        public const string HidePrintingMethod2 = "Hide Printing Method 2";
        #endregion

        #region Processing Messages
        public const string ProcessingInvoices = "Processing invoices...";
        public const string ProcessingShipments = "Processing shipments...";
        public const string InvoiceProcessingComplete = "Invoice processing completed successfully.";
        public const string ShipmentProcessingComplete = "Shipment processing completed successfully.";
        #endregion

        #region Validation Messages
        public const string ShipmentNotConfirmed = "Shipment must be confirmed before creating an invoice.";
        public const string ShipmentAlreadyInvoiced = "This shipment has already been invoiced.";
        public const string InvalidShipmentStatus = "Shipment status must be 'Confirmed' to create an invoice.";
        public const string NoUninvoicedOrderShipments = "No uninvoiced order shipments found for this shipment.";
        #endregion

        #region Information Messages
        public const string EmailSetFromCustomerContact = "Email address has been set from customer contact information.";
        public const string NoEmailFoundForCustomer = "No email address found for customer.";
        public const string CustomFieldsUpdated = "Custom fields have been updated successfully.";
        #endregion

        #region Button/Action Messages
        public const string CreateInvoice = "Create Invoice";
        public const string CreateDropshipInvoice = "Create Dropship Invoice";
        public const string Invoice = "Invoice";
        public const string ProcessSelected = "Process Selected";
        #endregion

        #region Cache Names
        public const string SOOrderExtension = "Sales Order Extension";
        public const string SOLineExtension = "Sales Order Line Extension";
        public const string ARInvoiceExtension = "AR Invoice Extension";
        public const string SOSetupExtension = "Sales Order Setup Extension";
        public const string CROpportunityExtension = "Opportunity Extension";
        public const string CRQuoteExtension = "Quote Extension";
        #endregion

        #region Attribute Names
        public const string AttributeBILLCOMPLE = "BILLCOMPLE";
        public const string AttributeFORMTYPE = "FORMTYPE";
        #endregion

        #region Warning Messages
        public const string NonStockItemInvoiceWarning = "Warning: This line contains a non-stock item that cannot be invoiced.";
        public const string VendorInformationMissing = "Vendor information is incomplete for this item.";
        public const string AddressInformationIncomplete = "Address information is incomplete for the selected vendor.";
        #endregion

        #region Success Messages
        public const string VendorInformationUpdated = "Vendor information has been updated successfully.";
        public const string AddressUpdated = "Vendor address has been updated successfully.";
        public const string EmailAddressUpdated = "Email address has been updated from customer contact.";
        #endregion

        #region Error Messages
        public const string CannotRetrieveCustomerEmail = "Cannot retrieve customer email address.";
        public const string CannotUpdateVendorAddress = "Cannot update vendor address information.";
        public const string FailedToProcessInvoice = "Failed to process invoice: {0}";
        public const string FailedToProcessShipment = "Failed to process shipment: {0}";
        public const string UnexpectedError = "An unexpected error occurred: {0}";
        #endregion

        #region Confirmation Messages
        public const string ConfirmProcessSelectedInvoices = "Are you sure you want to process the selected invoices?";
        public const string ConfirmProcessSelectedShipments = "Are you sure you want to process the selected shipments?";
        public const string ConfirmCreateCombinedInvoice = "This will create invoices for both regular and drop-ship items. Continue?";
        #endregion

        #region Status Messages
        public const string Ready = "Ready";
        public const string Processing = "Processing...";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string InProgress = "In Progress";
        #endregion
    }
}
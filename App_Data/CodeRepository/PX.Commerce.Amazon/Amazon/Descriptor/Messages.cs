/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using PX.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
	[PXLocalizable()]
	public static class AmazonMessages
	{
		public const string StockKitsWithDifferentTrackingNumber = "The {0} shipment could not be exported because the components of the {1} kit item were split into different packages with different tracking numbers.";
		public const string ShippingAccountOrSubMissing = "No account or subaccount (or neither) has been specified to record shipping charges. Make sure that either the shipping account (and the shipping subaccount, if applicable) is specified on the Order Settings tab of the Amazon Stores (BC201020) form or the freight account (and the freight subaccount, if applicable) is specified for the default customer location of the Amazon generic customer on the GL Accounts tab of the Customer Locations (AR303020) form. Then sync the order again.";
		public const string TestConnectionFailed = "The test connection could not be established. Make sure the information specified on the Connection Settings tab is correct and try again. For details, see the trace log: Click Tools > Trace on the form title bar.";
		public const string ItemNotFoundInvoice = "No active item with Seller SKU {0} and ASIN {1} has been found. Create the item in Acumatica ERP and process the invoice again.";
		public const string ItemNotFound = "No active item with Seller SKU {0} and ASIN {1} has been found. Create the item in Acumatica ERP and process the order again.";
		public const string ItemsWithoutBoxes = "The {0} shipment could not be exported because the {1} item is not linked to a box. Please add the item to a box and try again.";
		public const string BoxesWithoutItems = "The {0} shipment could not be exported because the {1} box has no items. Please delete this box or assign an item to it.";
		public const string PIIDisabled = "The Protect Personal Data check box is not selected for the {0} order type on the General tab of the Order Types (SO201000) form. Select the check box, and process the order again.";
		public const string NoDefaultTaxID = "The default tax has not been specified on the Amazon Stores (BC201020) form. Specify the default tax, and process the invoice again.";
		public const string NoGiftWrapItem = "The gift wrapping item could not be found. Configure the mapping of gift wrapping options on the Order Settings tab of the Amazon Stores (BC201020) form.";
		public const string InvoiceStatusDoesNotAllowModification = "The status of the {0} invoice does not allow modifications.";
		public const string AuthorizationRequired = "The connection could not be established because the access token is missing or invalid. Sign in to the Amazon seller account on the Amazon Stores (BC201020) form.";
		public const string RegionRequired = "The connection could not be established because the region is not specified. Specify the region and try again.";
		public const string MarketplaceRequired = "The connection could not be established because the marketplace is not specified. Specify the marketplace and try again.";
		public const string RegionMarketplaceRequired = "The connection could not be established because the region and marketplace are not specified. Specify the region and marketplace and try again.";
		public const string CustomerNotActive = "The {0} customer is not active.";
		public const string OrderDescription = "Amazon order {0} from {1} store";
		public const string InvoiceDecription = "Invoice for order {0} from {1} store";
		public const string NoGenericCustomer = "The customer account to be used for synchronization cannot be found. Make sure that the customer exists and the Guest Customer box is filled in on the Amazon Stores (BC201020) form.";
		public const string OrderIsSkippedBecauseItIsNotShipped = "The {0} order has been skipped because it has not been shipped in the external system.";
		public const string FBMOrderShipped = "The {0} order has been skipped because it is a seller-fulfilled order that has already been shipped.";
		public const string ShippingDiscount = "Shipping Discount";
		public const string PromotionalDiscount = "Promotional Discount";
		public const string NoBranch = "The branch for imported orders has not been specified on the Amazon Stores (BC201020) form. Specify the branch and process the order again.";
		public const string PaymentDescription = "{0} | Order: {1} Gateway: {2}";
		public const string LogPaymentSkippedNotConfigured = "The payment has been skipped because the {0} store order payment method is not mapped on the Amazon Stores (BC201020) form.";
		public const string PaymentForReplacementOrderIsInvalid = "No payment has been created in Acumatica ERP because the {0} replacement order total is $0.";
		public const string ShipmentWithoutBoxes = "The {0} shipment could not be processed because it does not contain any boxes. Add at least one box with items and process the shipment again.";
		public const string ShipmentWithoutDate = "The {0} shipment could not be processed because it does not have a shipment date.";
		public const string ShipmentIsEmpty = "The {0} shipment could not be processed because it does not contain any items. Add at least one item and process the shipment again.";
		public const string InventoryIsEmpty = "No product availability information has been exported for the {0} item.";
		public const string FeedProcessingUnexpectedStatus = "An unexpected error occurred while processing feed documents in Amazon. Feed processing status: {0}";
		public const string ShipmentExternError = "The {0} shipment could not be exported because of the following error: {1}";
		public const string ProductAvailabilityExternError = "The product availability information could not be exported for the {0} item because of the following error: {1}";
		public const string FeedDocumentIllformed = "The shipment document was malformed and could not be processed in Amazon. Process the shipment again.";
		public const string NotSupportedDropshipShipment = "The {0} shipment could not be processed because drop shipments are not supported for seller-fulfilled orders. Update the shipment information in your Amazon seller account manually.";
		public const string NotSupportedInvoiceShipment = "The {0} shipment could not be exported because the order has been invoiced. Update the shipment information in your Amazon seller account manually.";
		public const string ExportedShipmentCannotModify = "The {0} shipment for the Amazon order cannot be corrected because it has already been exported to Amazon.";
		public const string SellingPartnerIdNotProvidedPrepare = "Shipments could not be prepared because the Seller Partner ID is not specified on the Connection Settings tab of the Amazon Stores (BC201020) form. Specify the Seller Partner ID and prepare shipments again.";
		public const string SellingPartnerIdNotProvidedProcess = "Shipments could not be processed because the Seller Partner ID is not specified on the Connection Settings tab of the Amazon Stores (BC201020) form. Specify the Seller Partner ID and process the shipments again.";
		public const string ProductAvailabilityCannotBeProcessedBecauseOfSellingPartnerId = "The product availability information could not be exported for the {0} item because the Seller Partner ID is not specified on the Connection Settings tab of the Amazon Stores (BC201020) form. Add the Seller Partner ID and process the product availability record again.";
		public const string OrderShippingMappingIsMissing = "The order could not be imported because the {0} store shipping method is not mapped to a Ship Via code or the mapping is inactive on the Amazon Stores (BC201020) form.";
		public const string LogPaymentSkippedOrderNotShipped = "The {0} payment could not be processed because the order has not yet been shipped in the Amazon seller account. Ship the order and process the payment again.";
		public const string PaymentFailedFeesNotLinkedToEntryType = "The payment could not be processed because the following fees have not been mapped for the {0} store order payment method: {1}. Map the fees in the Amazon Fees section on the Payment Settings tab of the Amazon Stores (BC201020) form and process the payment again.";
		public const string PaymentNotReadyToBeProcessed = "The payment is not ready to be imported from Amazon. Import the payment after the order is fully shipped in the Amazon seller account.";
		public const string PaymentIsDeferredAndCannotBeImported = "The payment is a deferred payment and cannot be imported until it is released in the Amazon seller account.";
		public const string CannotActivateBecauseofAdvancedSOInvoices = "The Sales Invoice entity cannot be activated because the Advanced SO Invoices feature is disabled on the Enable/Disable Features (CS100000) form. Enable the feature and then activate the entity.";
		public const string SellerPartnerIDmustBeSpecified = "The Seller Partner ID must be specified when the Shipment entity is activated.";
		public const string SellerFulfilledOrderTypeMustBeSpecified = "The Seller-Fulfilled Order Type box must be filled in when the Seller-Fulfilled Order entity is activated.";
		public const string CannotProcessBecauseofAdvancedSOInvoices = "The {0} invoice could not be processed because the Advanced SO Invoices feature is disabled on the Enable/Disable Features (CS100000) form. Enable the feature and process the invoice again.";
		public const string CannotProcessBecauseofSellerFulfilledOrderTypeIsEmpty = "The order could not be processed because the Seller-Fulfilled Order Type box is not filled in on the Amazon Stores (BC201020) form. Fill in the box and process the order again.";
		public const string CannotImportPaymentInvoiceNotSynced = "The sales invoice for the Amazon order {0} has not been imported. Import the sales invoice and process the payment again.";
		public const string MultipleItemsSameCrossRefAmazon = "The seller SKU {0} in your Amazon seller account matches the alternate ID of the following items: {1}. Make sure the seller SKU is used as an alternate ID for only one active item in the ERP system, and process the record again.";

		public const string OrderNotSynced = "The {0} order is not yet synchronized. Synchronize the order and try again.";

		public const string ConnectionEstablished = "The test connection has been successfully established.";
		public const string ServerTimeRetrieved = "Server Time has been successfully retrieved.";

		public const string ActivateFeatureForDiagnostic = "To activate the service, enable the Amazon Connector feature first.";
		public const string DescriptionForDiagnostic = "Amazon Cloud Authorization Service";
		public const string SellerSKUAndASINHaveNoMatch = "The seller SKU {0} and ASIN {1} in your Amazon seller account do not match any item's inventory ID or alternate ID in the ERP system. Make sure an item with this inventory ID or alternate ID exists in the ERP system, and process the record again.";
		public const string SellerSKUOrASINMatchIsInactive = "The seller SKU {0} and ASIN {1} in your Amazon seller account match the inventory ID or alternate ID of the following items: {2}. However, all of these items have the Inactive, Marked for Deletion, or No Sales status. Make sure the seller SKU is used as an inventory ID or alternate ID for one active item in the ERP system, and process the record again.";
		public const string SellerSKUOrASINDuplicateMatches = "The seller SKU {0} and ASIN {1} in your Amazon seller account matches the inventory ID or alternate ID of the following items: {2}. Make sure the seller SKU is used as an inventory ID or alternate ID for one active item in the ERP system, and process the record again.";
		public const string PaymentExternalDescrTemplate = "{0}, Posted Date: {1}";
		public const string TheExternalPaymentIDIsNotCorrect = "The external payment ID must contain the Amazon order ID, followed by a semicolon and the payment number. Example: 123-4567891-2345678;1. The payment number must be an integer between 1 and the number of payments created for the current order.";
		public const string TheExternalPaymentIDIsNotCorrectWithNumberOfShipmentEvents = "The external payment ID must contain the Amazon order ID, followed by a semicolon and the payment number. Example: 123-4567891-2345678;1. The payment number for the {0} order must be an integer between 1 and {1}.";
		public const string InvoiceOrApplicationsAreNotReleased = "The payment could not be imported. Make sure that the {0} invoice and all payments applied to it have been released, and process the payment again.";
		public const string NoShipViaCodeHasBeen = "No Ship Via code has been specified for the {0} shipment. Fill in the Ship Via box for the shipment on the Shipments (SO302000) form, and process the record again.";
		public const string NoCarrierIsMapped =    "No carrier is mapped with the following Ship Via code: {0}. Map the Ship Via code with a carrier in the {1} substitution list on the Substitution Lists (SM206026) form, and process the record again.";
		public const string NoShippingServiceIsMapped = "No shipping service is mapped with the following Ship Via code: {0}. Map the Ship Via code with a shipping service in the {1} substitution list on the Substitution Lists (SM206026) form, and process the record again.";
		public const string LogPaymentSkippedCreatedBeforeSyncOrdersFrom = "The payment for the {0} order has been filtered because the order was created before the earliest order date ({1}).";
		public const string WrongBindingState = "Wrong binding state: {0}";
		public const string WrongBindingCompany = "Wrong company state: {0}";
		public const string ThePaymentMethodMustBeCashCheck = "Specify a payment method that has the Cash/Check means of payment.";

		public const string FBAOrderNotShipped = "The {0} order has been skipped because its status in the external system ({1}) is not supported.";
		public const string CannotProcessBecauseAmazonFulfilledOrderTypeIsEmpty = "The order could not be processed because the Marketplace-Fulfilled Order Type box is not filled in on the Amazon Stores (BC201020) form. Fill in the box and process the order again.";
		public const string ShippingPriceItemDoesNotExist = "The shipping price item does not exist. Make sure that the correct item is specified in the Shipping Price Item box on the Order Settings tab of the Amazon Stores (BC201020) form, and process the record again.";
		public const string TheShippingItemIsNotSpecified = "No shipping price item has been specified. Fill in the Shipping Price Item box on the Order Settings tab of the Amazon Stores (BC201020) form, and process the record again.";
		public const string ShippingPriceItem = "Shipping Price Item";
		public const string GiftWrappingItemDoesNotExist = "The gift wrapping item does not exist. Make sure that the correct item is specified in the Gift Wrapping Item box on the Order Settings tab of the Amazon Stores (BC201020) form, and process the record again.";
		public const string GiftWrappingItemIsNotSpecified = "No gift wrapping item has been specified. Fill in the Gift Wrapping Item box on the Order Settings tab of the Amazon Stores (BC201020) form, and process the record again.";
		public const string FBAHasAlreadyBeenImportedAsSOInvoice = "The {0} order has already been imported from Amazon as a sales invoice.";
		public const string FetchIsNotSupportedForEntity = "The {0} entity has been deprecated. Use the {1} entity to prepare Amazon-fulfilled orders.";

		public const string ClosedFinancialGroupWithoutDate = "The {0} financial group does not have a start date or an end date. Try again later.";
		public const string FinancialGroupIsOpened = "The non-order fees have been skipped because the {0} financial group has the Open status. Make sure the settlement report is generated for the settlement period, and process the record again.";
		public const string FinancialGroupOtherCurrency = "The {0} financial group has been skipped because its currency ({1}) is different from the store currency ({2}).";
		public const string FinancialGroupStartDateMissing = "The {0} financial group has been skipped because it does not have a start date.";
		public const string FinancialGroupEndDateMissing = "The {0} financial group has been skipped because it does not have an end date.";
		public const string FinancialGroupStetementPeriodIsMissing = "No financial group exists for the settlement period specified in the external description. Make sure that there is a settlement report for the specified settlement period and import the non-order fees again.";
		public const string NonOrderFeeDescriptionIsIncorrect = "The external description does not have a valid start date or a valid end date. Delete this record and import the non-order fees again.";
		public const string FinancialGroupStartDateCantBeParsed = "The external description does not have a valid start date. Delete this record and import the non-order fees again.";
		public const string FinancialGroupEndDateCantBeParsed = "The external description does not have a valid end date. Delete this record and import the non-order fees again.";
		public const string FeeMappingIsInactive = "The {0} fee in the amount of {1} has been skipped because its mapping is inactive.";
		public const string FeeMappingIsAbsent = "The {0} fee has been unmapped on the Payment Settings tab of the Amazon Stores (BC201020) form. Map the fee, and process the record again.";
		public const string NonOrderFeeFailedFeesNotLinkedToEntryType = "The non-order fees could not be processed because the following Amazon fees are not mapped with any entry types: {0}. Map the fees in the Amazon Fees section of the Payment Settings tab of the Amazon Stores (BC201020) form, and process the record again.";
		public const string OrderPaymentMethodIsMissing = "No payment has been imported from Amazon or the store payment method mapping is inactive on the Amazon Stores (BC201020) form. Import any payment and then process the non-order fees.";
		public const string FinancialGroupFilterDateIsIncorrect = "The date range for importing non-order fees for a settlement period cannot exceed 365 days.";
		public const string InactiveAllFeeTypes = "All non-order fees have been skipped because there is no active mapping of Amazon fees and entry types on the Payment Settings tab of the Amazon Stores (BC201020) form. Map the fee types and import the record again.";
		public const string CashTransactionDescription = "Transaction for Entry Type {0}";
		public const string FeeIsSkippedDueToZeroAmount = "The fee has a zero amount and will be skipped. Entry Type: {0}, Fee Description: {1}, Fee Date: {2}.";
		public const string NoValidToken = "Authorization in the Amazon service has failed because no valid token has been received.";
		public const string AuthorizationError = "The connection to the Amazon service cannot be established. Try running the operation again or restarting the application. Please contact your Acumatica support provider if the issue is not resolved after several connection attempts.";
		public const string ItemIsSplittedWithinDifferentPackages = "The {0} shipment could not be processed because the {1} item has been added to multiple packages. Add the item to a single package, and process the shipment again.";
	}

	[PXLocalizable()]
	public static class AmazonCaptions
	{
		public const string FBAEntity = "Marketplace-Fulfilled Order";
		public const string FBMOrderType = "Seller-Fulfilled Order Type";
		public const string FBAOrderType = "Marketplace-Fulfilled Order Type";

		public const string SalesInvoicesDiscountDetails = "Sales Invoice Discount Details";
		public const string Payment = "Payment";

		//Amazon Marketplaces
		public const string Canada = "Canada";
		public const string US = "United States of America";
		public const string Mexico = "Mexico";
		public const string Brazil = "Brazil";

		public const string Singapore = "Singapore";
		public const string Australia = "Australia";
		public const string Japan = "Japan";

		public const string Spain = "Spain";
		public const string UnitedKingdom = "United Kingdom";
		public const string France = "France";
		public const string Netherlands = "Netherlands";
		public const string Germany = "Germany";
		public const string Italy = "Italy";
		public const string Sweden = "Sweden";
		public const string Poland = "Poland";
		public const string Egypt = "Egypt";
		public const string Turkey = "Turkey";
		public const string SaudiArabia = "Saudi Arabia";
		public const string UnitedArabEmirates = "United Arab Emirates";
		public const string India = "India";

		//Amazon Seller Region
		public const string NorthAmerica = "North America";
		public const string Europe = "Europe";
		public const string FarEast = "Far East";

		//Store Configuration
		public const string CloudAuth = "Cloud Authorization";
		public const string SelfAuth = "Self-Authorization";
		//Amazon API Object Descriptions
		public const string AmazonOrderId = "Amazon Order Id";
		public const string PurchaseDate = "Purchase Date";
		public const string OrderStatus = "Order Status";
		public const string LatestShipDate = "Latest Shipment Date";
		public const string FulfillmentChannel = "Fulfillment Channel";
		public const string OrderItems = "Order Items";
		public const string BuyerInfo = "Buyer Info";

		public const string Order = "Order";
		public const string ShippingAddress = "Shipping Address";
		public const string BuyerEmail = "Buyer Email";
		public const string BuyerName = "Buyer Name";
		public const string Name = "Name";
		public const string PurchaseOrderNumber = "Purchase Order Number";
		public const string AddressLine1 = "Address Line 1";
		public const string AddressLine2 = "Address Line 2";
		public const string AddressLine3 = "Address Line 3";
		public const string City = "City";
		public const string County = "County";
		public const string District = "District";
		public const string StateOrRegion = "State Or Region";
		public const string PostalCode = "Postal Code";
		public const string Phone = "Phone";
		public const string SellerSKU = "Seller SKU";
		public const string Title = "Title";
		public const string QuantityOrdered = "Quantity Ordered";
		public const string ItemPrice = "Item Price";
		public const string ShippingPrice = "Shipping Price";
		public const string ItemTax = "Item Tax";
		public const string ShippingDiscount = "Shipping Discount";
		public const string PromotionDiscount = "Promotion Discount";
		public const string CODFeeDiscount = "COD Fee Discount";
		public const string IsGift = "Is Gift";
		public const string ScheduledDeliveryStartDate = "Scheduled Delivery Start Date";
		public const string CurrencyCode = "Currency Code";
		public const string Amount = "Amount";
		public const string CurrencyAmount = "Currency Amount";
		public const string PaymentExecutionDetail = "Payment Execution Detail";
		public const string PaymentMethod = "Payment Method";
		public const string SerialNumberRequired = "Serial Number Required";
		public const string ScheduledDeliveryEndDate = "Scheduled Delivery End Date";
		public const string ConditionNote = "Condition Note";
		public const string CODFee = "COD Fee";
		public const string PromotionDiscountTax = "Promotion Discount Tax";
		public const string ShippingTax = "Shipping Tax";
		public const string ASIN = "ASIN";
		public const string BuyerTaxInfo = "Buyer Tax Info";
		public const string BuyerCounty = "Buyer County";
		public const string AddressType = "Address Type";
		public const string FulfillmentInstruction = "Fulfillment Instruction";
		public const string EarliestDeliveryDate = "Earliest Delivery Date";
		public const string EarliestShipDate = "Earliest Ship Date";
		public const string ShipmentServiceLevelCategory = "Shipment Service Level Category";
		public const string MarketplaceId = "Marketplace Id";
		public const string OrderTotal = "Order Total";
		public const string ShipServiceLevel = "Shipment Service Level";
		public const string OrderType = "Order Type";
		public const string OrderChannel = "Order Channel";
		public const string SalesChannel = "Sales Channel";
		public const string SellerOrderId = "Seller Order Id";
		public const string FeeType = "Fee Type";
		public const string FeeAmount = "Fee Amount";
		public const string ChargeType = "Charge Type";
		public const string ChargeAmount = "Charge Amount";
		public const string OrderItemId = "Order Item Id";
		public const string SubAccount = "Subaccount";

		// Listings
		public const string SKU = "SKU";
		public const string Summaries = "Summaries";
		public const string Attributes = "Attributes";
		public const string Issues = "Issues";
		public const string Offers = "Offers";
		public const string FulfillmentAvailability = "Fulfillment Availability";
		public const string Procurement = "Procurement";
		public const string ProductType = "Product Type";
		public const string ConditionType = "Condition Type";
		public const string Status = "Status";
		public const string FnSku = "FnSku";
		public const string ItemName = "Item Name";
		public const string CreatedDate = "Created Date";
		public const string LastUpdatedDate = "Last Updated Date";
		public const string MainImage = "Main Image";
		public const string FulfillmentChannelCode = "Fulfillment Channel Code";
		public const string Quantity = "Fulfillment Channel Code";
		public const string Code = "Code";
		public const string Message = "Message";
		public const string Severity = "Severity";
		public const string AttributeNames = "Attribute Names";
		public const string OfferType = "Offer Type";
		public const string Price = "Price";
		public const string Points = "Points";
		public const string CostPrice = "Cost/Price";

		// Features
		public const string AdvancedSOInvoicesFeature = "Advanced SO Invoices";

		// Amazon Details
		public const string SalesInvoiceLineEntityType = "IL";
		public const string SalesInvoiceLineEntity = "Sales Invoice Line";

		// Non order fee
		public const string NonOrderFee = "Non-Order Fees";
		public const string OrderFeeType = "Order Fee";
		public const string NonOrderFeeType = "Non-Order Fee";
		public const string CashTransaction = "Cash Transaction";
	}
}

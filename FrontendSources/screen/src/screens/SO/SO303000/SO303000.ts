import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo
} from 'client-controls';

import {
	ARInvoice,
	ARInvoiceCurrent,
	ARTran,
	TaxTran,
	ARSalesPerTran,
	SOFreightDetail,
	SOInvoice,
	ARContact,
	ARAddress,
	ARShippingContact,
	ARShippingAddress,
	ARInvoiceDiscountDetail,
	ARAdjust,
	ARAdjust2,
	SOOrderShipment,
	RecalcDiscountsParamFilter,
	SOLineForDirectInvoice,
	ARTranForDirectInvoice,
	AddressLookupFilter,
	SOQuickPayment,
	SOImportExternalTran,
	DuplicateFilter,
	CurrencyInfo
} from './views';

@graphInfo({graphType: 'PX.Objects.SO.SOInvoiceEntry', primaryView: 'Document', udfTypeField: 'DocType', bpEventsIndicator: true, showUDFIndicator: true})
export class SO303000 extends PXScreen {
	AddressLookup: PXActionState;
	ShippingAddressLookup: PXActionState;
	AddShipment: PXActionState;
	AddShipmentCancel: PXActionState;
	RecalcOk: PXActionState;
	AddSOLine: PXActionState;
	AddARTran: PXActionState;
	AddressLookupSelectAction: PXActionState;
	CreatePaymentRefund: PXActionState;
	CreatePaymentCapture: PXActionState;
	CreatePaymentAuthorize: PXActionState;
	CreatePaymentOK: PXActionState;
	ImportDocumentPaymentCreate: PXActionState;
	Approve: PXActionState;
	Reject: PXActionState;

	@viewInfo({containerName: 'Invoice Summary'})
	Document = createSingle(ARInvoice);

	@viewInfo({containerName: 'Invoice Summary'})
	CurrentDocument = createSingle(ARInvoiceCurrent);

	@viewInfo({containerName: 'Currency'})
	CurrencyInfo = createSingle(CurrencyInfo);

	@viewInfo({containerName: 'Details'})
	Transactions = createCollection(ARTran);

	@viewInfo({containerName: 'Taxes'})
	Taxes = createCollection(TaxTran);

	@viewInfo({containerName: 'Commissions'})
	SalesPerTrans = createCollection(ARSalesPerTran);

	@viewInfo({containerName: 'Freight'})
	FreightDetails = createCollection(SOFreightDetail);

	@viewInfo({containerName: 'Payment Information'})
	SODocument = createSingle(SOInvoice);

	@viewInfo({containerName: 'Bill-To Contact'})
	Billing_Contact = createSingle(ARContact);

	@viewInfo({containerName: 'Bill-To Address'})
	Billing_Address = createSingle(ARAddress);

	@viewInfo({containerName: 'Ship-To Contact'})
	Shipping_Contact = createSingle(ARShippingContact);

	@viewInfo({containerName: 'Ship-To Address'})
	Shipping_Address = createSingle(ARShippingAddress);

	@viewInfo({containerName: 'Discounts'})
	ARDiscountDetails = createCollection(ARInvoiceDiscountDetail);

	@viewInfo({containerName: 'Application Invoice'})
	Adjustments = createCollection(ARAdjust);

	@viewInfo({containerName: 'Application Credit Memo'})
	Adjustments_1 = createCollection(ARAdjust2);

	@viewInfo({containerName: 'Add Order'})
	shipmentlist = createCollection(SOOrderShipment);

	@viewInfo({containerName: 'Recalculate Prices'})
	recalcdiscountsfilter = createSingle(RecalcDiscountsParamFilter);

	@viewInfo({containerName: 'Add SO Line'})
	soLineList = createCollection(SOLineForDirectInvoice);

	@viewInfo({containerName: 'Add Return Line'})
	arTranList = createCollection(ARTranForDirectInvoice);

	@viewInfo({containerName: 'Address Lookup'})
	AddressLookupFilter = createSingle(AddressLookupFilter);

	@viewInfo({containerName: 'Create Payment'})
	QuickPayment = createSingle(SOQuickPayment);

	@viewInfo({containerName: 'Import CC Payment'})
	ImportExternalTran = createSingle(SOImportExternalTran);

	@viewInfo({containerName: 'Duplicate Reference Nbr.'})
	duplicatefilter = createSingle(DuplicateFilter);
}
import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection,  createSingle,  PXScreen,  graphInfo,  PXActionState,  viewInfo,
	handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection } from 'client-controls';
import { POOrderHeader, POOrder, POOrderFilter, POLineS, POOrderS, SOLineSplit3,
	POLine, POTaxTran, POContact, POAddress, POContact2, POAddress2,
	POOrderDiscountDetail, POOrderPOReceipt, POOrderAPDoc, POOrderPrepayment,
	PMChangeOrderLine, ComplianceDocument, RecalcDiscountsParamFilter,
	CreateSOOrderFilter, AddressLookupFilter, CurrencyInfo, POOrderChildOrdersReceipts, POOrderChildOrdersAPDocs } from './views';

@graphInfo({graphType: 'PX.Objects.PO.POOrderEntry',  primaryView: 'Document',  udfTypeField: 'OrderType', bpEventsIndicator: true, showUDFIndicator: true})
export class PO301000 extends PXScreen {
	AddressLookup: PXActionState;
	RemitAddressLookup: PXActionState;
	AddSelectedItems: PXActionState;
	RecalcOk: PXActionState;
	AddressLookupSelectAction: PXActionState;
	AppendSelectedProjectItems: PXActionState;
	ViewChangeOrder: PXActionState;
	ViewOrigChangeOrder: PXActionState;

	@viewInfo({containerName: 'Document Summary'})
	Document = createSingle(POOrderHeader);
	@viewInfo({containerName: 'Document'})
	CurrentDocument = createSingle(POOrder);
	@viewInfo({containerName: 'currencyinfo'})
	CurrencyInfo = createSingle(CurrencyInfo);

	@viewInfo({containerName: 'Details'})
	Transactions = createCollection(POLine);

	@viewInfo({containerName: 'Taxes'})
	Taxes = createCollection(POTaxTran);

	@viewInfo({containerName: 'Ship-To Contact'})
	Shipping_Contact = createSingle(POContact);
	@viewInfo({containerName: 'Ship-To Address'})
	Shipping_Address = createSingle(POAddress);
	@viewInfo({containerName: 'Vendor Contact'})
	Remit_Contact = createSingle(POContact2);
	@viewInfo({containerName: 'Vendor Address'})
	Remit_Address = createSingle(POAddress2);

	@viewInfo({containerName: 'Discounts'})
	DiscountDetails = createCollection(POOrderDiscountDetail);

	@viewInfo({containerName: 'PO History'})
	Receipts = createCollection(POOrderPOReceipt);

	@viewInfo({containerName: 'PO History'})
	APDocs = createCollection(POOrderAPDoc);

	@viewInfo({containerName: 'Blanket PO History'})
	ChildOrdersReceipts = createCollection(POOrderChildOrdersReceipts);

	@viewInfo({containerName: 'Blanket PO History'})
	ChildOrdersAPDocs = createCollection(POOrderChildOrdersAPDocs);

	@viewInfo({containerName: 'Prepayments'})
	PrepaymentDocuments = createCollection(POOrderPrepayment);

	@viewInfo({containerName: 'Change Orders'})
	ChangeOrders = createCollection(PMChangeOrderLine);

	@viewInfo({containerName: 'Compliance'})
	ComplianceDocuments = createCollection(ComplianceDocument);

	@viewInfo({containerName: 'Recalculate Prices'})
	recalcdiscountsfilter = createSingle(RecalcDiscountsParamFilter);
	@viewInfo({containerName: 'Create Sales Order'})
	createSOFilter = createSingle(CreateSOOrderFilter);

	@viewInfo({containerName: 'Address Lookup'})
	AddressLookupFilter = createSingle(AddressLookupFilter);

	@viewInfo({containerName: 'PO Selection'})
	filter = createSingle(POOrderFilter);
	@viewInfo({containerName: 'Add Purchase Order Line'})
	poLinesSelection = createCollection(POLineS);

	@viewInfo({containerName: 'Add Purchase Order'})
	openOrders = createCollection(POOrderS);

	@viewInfo({containerName: 'Demand'})
	FixedDemand = createCollection(SOLineSplit3);

	@handleEvent(CustomEventType.RowSelected, { view: 'Transactions' })
	onSOLineChanged(args: RowSelectedHandlerArgs<PXViewCollection<POLine>>) {
		const model = (<any>args.viewModel as POLine); // todo: think about better solution
		const ar = args.viewModel.activeRow;

		if (model.ViewDemand) model.ViewDemand.enabled = !!ar?.ViewDemandEnabled.value;
	}
}

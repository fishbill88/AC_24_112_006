import
{
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	handleEvent,
	CustomEventType,
	RowSelectedHandlerArgs,
	PXViewCollection
} from "client-controls";
import { Messages as SysMessages } from 'client-controls/services/messages';
import
{
	SOOrder,
	SOLine,
	SOLineSplit,
	SOTaxTran,
	SOSalesPerTran,
	SOAdjustments,
	CurrencyInfo,
	SOParamFilter,
	SOShippingContact,
	SOShippingAddress,
	SOBillingContact,
	SOBillingAddress,
	SOOrderShipment,
	SOBlanketOrderDisplayLink,
	OpenBlanketSOLineSplit,
	LineSplittingHeader,
	RecalcDiscountsFilter,
	SOLinePOLink,
	LinkedPOLines,
	AddInvoiceHeader,
	AddInvoiceDetails,
	CopyParamFilter,
	SOQuickPayment,
	Relations,
	ShopForRatesHeader,
	CarrierRates,
	Packages,
	SOOrderHeader,
	ImportExternalTran,
	BlanketTaxZoneOverrideFilter
} from './views';

@graphInfo({graphType: 'PX.Objects.SO.SOOrderEntry', primaryView: 'Document', bpEventsIndicator: true, udfTypeField: 'OrderType', showActivitiesIndicator: true, showUDFIndicator: true })
export class SO301000 extends PXScreen {
	SysMessages = SysMessages;

	AddInvoiceOK: PXActionState;

	OverrideBlanketTaxZone: PXActionState;
	AddRelatedItems: PXActionState;
	PasteLine: PXActionState;
	ShopRates: PXActionState;
	ShippingAddressLookup: PXActionState;
	AddressLookup: PXActionState;
	ViewChildOrder: PXActionState;
	ViewPayment: PXActionState;

	CreatePaymentRefund: PXActionState;
	CreatePaymentCapture: PXActionState;
	CreatePaymentAuthorize: PXActionState;
	DeletePayment: PXActionState;
	DeleteRefund: PXActionState;
	CreatePaymentOK: PXActionState;
	CalculateFreight: PXActionState;
	SOOrderLineSplittingExtension_GenerateNumbers: PXActionState;
	AddSelectedItems: PXActionState;
	AddBlanketLineOK: PXActionState;
	CheckCopyParams: PXActionState;

	RecalcOk: PXActionState;
	ImportDocumentPaymentCreate: PXActionState;

	@viewInfo({containerName: "Order Summary"})
	Document = createSingle(SOOrderHeader);

	@viewInfo({containerName: "Details"})
	Transactions = createCollection(SOLine);

	@viewInfo({containerName: "Line Details"})
	SOOrderLineSplittingExtension_LotSerOptions = createSingle(LineSplittingHeader);

	@viewInfo({containerName: "Line Details"})
	splits = createCollection(SOLineSplit);

	@viewInfo({containerName: "Taxes"})
	Taxes = createCollection(SOTaxTran);

	@viewInfo({containerName: "Commissions"})
	SalesPerTran = createCollection(SOSalesPerTran);

	@viewInfo({containerName: "Commissions"})
	CurrentDocument = createSingle(SOOrder);

	@viewInfo({containerName: "Ship-To Contact"})
	Shipping_Contact = createSingle(SOShippingContact);
	@viewInfo({containerName: "Ship-To Address"})
	Shipping_Address = createSingle(SOShippingAddress);
	@viewInfo({containerName: "Bill-To Contact"})
	Billing_Contact = createSingle(SOBillingContact);
	@viewInfo({containerName: "Bill-To Address"})
	Billing_Address = createSingle(SOBillingAddress);

	@viewInfo({containerName: "Shipments"})
	shipmentlist = createCollection(SOOrderShipment);

	@viewInfo({containerName: "Child Orders"})
	BlanketOrderChildrenDisplayList = createCollection(SOBlanketOrderDisplayLink);

	@viewInfo({containerName: "Add Blanket Sales Order Line"})
	BlanketSplits = createCollection(OpenBlanketSOLineSplit);

	@viewInfo({containerName: "Payments"})
	Adjustments = createCollection(SOAdjustments);

	@viewInfo({containerName: "Relations"})
	Relations = createCollection(Relations);

	@viewInfo({containerName: "Process Order"})
	_SOOrder_CurrencyInfo_ = createSingle(CurrencyInfo);

	@viewInfo({containerName: "Purchasing Details"})
	SOLineDemand = createSingle(SOLinePOLink);

	@viewInfo({containerName: "Purchasing Details"})
	SupplyPOLines = createCollection(LinkedPOLines);

	@viewInfo({containerName: "Legacy Purchasing Details"})
	posupply = createCollection(LinkedPOLines);

	@viewInfo({containerName: "Add Invoice Details"})
	AddInvoiceFilter = createSingle(AddInvoiceHeader);

	@viewInfo({containerName: "Add Invoice Details"})
	invoiceSplits = createCollection(AddInvoiceDetails);

	@viewInfo({containerName: "Specify Shipment Parameters"})
	soparamfilter = createSingle(SOParamFilter);

	@viewInfo({containerName: "Recalculate Prices"})
	recalcdiscountsfilter = createSingle(RecalcDiscountsFilter);

	@viewInfo({containerName: "Copy To"})
	copyparamfilter = createSingle(CopyParamFilter);

	//Header = createSingle(EntryHeader); // TODO: Must be placed in feature extension

	@viewInfo({containerName: "Create Payment"})
	QuickPayment = createSingle(SOQuickPayment);

	@viewInfo({containerName: "Import CC Payment"})
	ImportExternalTran = createSingle(ImportExternalTran);

	@viewInfo({containerName: "Shop For Rates"})
	DocumentProperties = createSingle(ShopForRatesHeader);

	@viewInfo({containerName: "Carrier Rates"})
	CarrierRates = createCollection(CarrierRates);

	@viewInfo({containerName: "Packages"})
	Packages = createCollection(Packages);

	@viewInfo({containerName: "Override Tax Zone"})
	BlanketTaxZoneOverrideFilter = createSingle(BlanketTaxZoneOverrideFilter);

	@handleEvent(CustomEventType.RowSelected, { view: 'Transactions' })
	onSOLineChanged(args: RowSelectedHandlerArgs<PXViewCollection<SOLine>>) {
		const model = (<any>args.viewModel as SOLine); // todo: think about better solution
		const ar = args.viewModel.activeRow;

		if (model.POSupplyOK) model.POSupplyOK.enabled = !!ar?.POCreate.value;
		if (model.ConfigureEntry) model.ConfigureEntry.enabled = !!ar?.IsConfigurable.value;
		if (model.linkProdOrder) model.linkProdOrder.enabled = !!ar?.AMProdCreate.value;

		if (model.ItemAvailability) model.ItemAvailability.enabled = !!ar?.IsStockItem.value;
		if (model.SOOrderLineSplittingExtension_ShowSplits) model.SOOrderLineSplittingExtension_ShowSplits.enabled = !!ar;
	}
}

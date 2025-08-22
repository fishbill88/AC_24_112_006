import
{
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo
} from "client-controls";
import { Messages as SysMessages } from 'client-controls/services/messages';
import { POReceiptHeader, POReceipt, POReceiptLine, POOrderReceipt, INRegister, POReceiptPOOriginal, POReceiptAPDoc, POLandedCostReceipt,
	POReceiptLineS, LineSplittingHeader, POReceiptLineSplit, POOrderFilter, POLine, POOrder, POReceiptReturnFilter, POReceiptReturn, POReceiptLineReturn,
	SOOrderShipment, INTran, CurrencyInfo } from './views';

@graphInfo({ graphType: 'PX.Objects.PO.POReceiptEntry', primaryView: 'Document', bpEventsIndicator: true, udfTypeField: 'ReceiptType', showActivitiesIndicator: true, showUDFIndicator: true })
export class PO302000 extends PXScreen {
	SysMessages = SysMessages;

	AddPOReceiptLine2: PXActionState;
	AddPOOrder2: PXActionState;
	AddPOOrderLine2: PXActionState;
	AddPOReceiptReturn2: PXActionState;
	AddPOReceiptLineReturn2: PXActionState;
	AddTransfer2: PXActionState;
	AddINTran: PXActionState;
	AddINTran2: PXActionState;
	POReceiptLineSplittingExtension_GenerateNumbers: PXActionState;

	@viewInfo({containerName: 'Document Summary'})
	Document = createSingle(POReceiptHeader);
	@viewInfo({containerName: ''})
	CurrentDocument = createSingle(POReceipt);

	@viewInfo({containerName: ''})
	CurrencyInfo = createSingle(CurrencyInfo);

	@viewInfo({containerName: 'Details'})
	transactions = createCollection(POReceiptLine);

	@viewInfo({containerName: 'Line Details'})
	POReceiptLineSplittingExtension_LotSerOptions = createSingle(LineSplittingHeader);
	@viewInfo({containerName: 'Line Details'})
	splits = createCollection(POReceiptLineSplit);

	@viewInfo({containerName: 'Orders'})
	ReceiptOrdersLink = createCollection(POOrderReceipt);

	@viewInfo({containerName: 'Put Away'})
	RelatedTransfers = createCollection(INRegister);

	@viewInfo({containerName: 'History'})
	ReceiptHistory = createCollection(POReceiptPOOriginal);

	@viewInfo({containerName: 'Billing'})
	apDocs = createCollection(POReceiptAPDoc);

	@viewInfo({containerName: 'Landed Costs'})
	landedCosts = createCollection(POLandedCostReceipt);

	@viewInfo({containerName: 'Add Receipt Line'})
	addReceipt = createSingle(POReceiptLineS);

	@viewInfo({containerName: 'PO Selection'})
	filter = createSingle(POOrderFilter);
	@viewInfo({containerName: 'Add Purchase Order Line'})
	poLinesSelection = createCollection(POLine);

	@viewInfo({containerName: 'Add Purchase Order'})
	openOrders = createCollection(POOrder);

	@viewInfo({containerName: 'PO Receipt Selection'})
	returnFilter = createSingle(POReceiptReturnFilter);
	@viewInfo({containerName: 'Add Receipt'})
	poReceiptReturn = createCollection(POReceiptReturn);

	@viewInfo({containerName: 'Add Receipt Line'})
	poReceiptLineReturn = createCollection(POReceiptLineReturn);

	@viewInfo({containerName: 'Add Transfer Order'})
	openTransfers = createCollection(SOOrderShipment);

	@viewInfo({containerName: 'Add Transfer Line'})
	intranSelection = createCollection(INTran);
}

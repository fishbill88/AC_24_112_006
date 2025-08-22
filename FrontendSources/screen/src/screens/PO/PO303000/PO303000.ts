import
{
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo
} from "client-controls";
import { POLandedCostDocHeader, POLandedCostDoc, POLandedCostDetail, POLandedCostReceiptLine,
	POLandedCostTaxTran, CurrencyInfo, POReceiptFilter, POReceipt, POReceiptLineAdd } from './views';

@graphInfo({graphType: 'PX.Objects.PO.POLandedCostDocEntry', primaryView: 'Document', bpEventsIndicator: true, udfTypeField: ''})
export class PO303000 extends PXScreen {
	AddLC: PXActionState;
	AddPOReceipt2: PXActionState;
	AddPOReceiptLine2: PXActionState;

	@viewInfo({containerName: 'Document Summary'})
	Document = createSingle(POLandedCostDocHeader);
	@viewInfo({containerName: 'Billing Settings'})
	CurrentDocument = createSingle(POLandedCostDoc);

	@viewInfo({containerName: 'Landed Costs'})
	Details = createCollection(POLandedCostDetail);

	@viewInfo({containerName: 'Details'})
	ReceiptLines = createCollection(POLandedCostReceiptLine);

	@viewInfo({containerName: 'Taxes'})
	Taxes = createCollection(POLandedCostTaxTran);

	@viewInfo({containerName: ''})
	CurrencyInfo = createSingle(CurrencyInfo);

	@viewInfo({containerName: 'PO Selection'})
	filter = createSingle(POReceiptFilter);
	@viewInfo({containerName: 'Add Receipt'})
	poReceiptSelection = createCollection(POReceipt);
	@viewInfo({containerName: 'Add Receipt Line'})
	poReceiptLinesSelection = createCollection(POReceiptLineAdd);
}

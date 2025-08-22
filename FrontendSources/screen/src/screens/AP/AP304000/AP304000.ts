import { APQuickCheck, APTran, TaxTran, EPApproval, APContact, APAddress, APPaymentChargeTran, ReasonApproveRejectFilter, ReassignApprovalFilter, AddressLookupFilter, CurrencyInfo } from './views';
import { PXActionState, graphInfo, PXScreen, createSingle, createCollection, viewInfo } from "client-controls";

@graphInfo({
	graphType: 'PX.Objects.AP.APQuickCheckEntry',
	primaryView: 'Document',
	bpEventsIndicator: false,
	udfTypeField: "DocType",
	showUDFIndicator: true
})
export class AP304000 extends PXScreen {
	ViewSchedule: PXActionState;
	AddressLookup: PXActionState;
	AddressLookupSelectAction: PXActionState;

	Document = createSingle(APQuickCheck);
	CurrentDocument = createSingle(APQuickCheck);
	Transactions = createCollection(APTran);
	Taxes = createCollection(TaxTran);
	Approval = createCollection(EPApproval);
	Remittance_Contact = createSingle(APContact);
	Remittance_Address = createSingle(APAddress);
	PaymentCharges = createCollection(APPaymentChargeTran);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectFilter);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
	AddressLookupFilter = createSingle(AddressLookupFilter);

	@viewInfo({ containerName: "Currency rate" })
	CurrencyInfo = createSingle(CurrencyInfo);
}

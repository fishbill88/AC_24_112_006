import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Document,
	Project,
	Overflow,
	Details,
	Unbilled,
	DocumentSettings,
	ProgressiveLines,
	TransactionLines,
	Taxes,
	Revisions,
	Approval,
	Billing_Contact,
	Billing_Address,
	Shipping_Contact,
	Shipping_Address,
	CurrencyInfo,
	ReassignApprovalFilter,
	ReasonApproveRejectParams
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.ProformaEntry",
	primaryView: "Document",
	showActivitiesIndicator: true, showUDFIndicator: true
})
export class PM307000 extends PXScreen {
	ViewTranDocument: PXActionState;
	ViewProgressLineTask: PXActionState;
	ViewProgressLineInventory: PXActionState;
	ViewTransactLineTask: PXActionState;
	ViewTransactLineInventory: PXActionState;
	ViewVendor: PXActionState;
	AppendSelected: PXActionState;
	AutoApplyPrepayments: PXActionState;

	Document = createSingle(Document);
	Project = createSingle(Project);
	Overflow = createSingle(Overflow);
	Details = createCollection(Details);
	Unbilled = createCollection(Unbilled);
	DocumentSettings = createSingle(DocumentSettings);
	ProgressiveLines = createCollection(ProgressiveLines);
	TransactionLines = createCollection(TransactionLines);
	Taxes = createCollection(Taxes);
	Revisions = createCollection(Revisions);
	Approval = createCollection(Approval);
	Billing_Contact = createSingle(Billing_Contact);
	Billing_Address = createSingle(Billing_Address);
	Shipping_Contact = createSingle(Shipping_Contact);
	Shipping_Address = createSingle(Shipping_Address);
	CurrencyInfo = createSingle(CurrencyInfo);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
}

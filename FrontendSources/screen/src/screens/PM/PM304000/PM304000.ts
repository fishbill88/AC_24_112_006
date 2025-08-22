import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from "client-controls";

import {
	Document,
	Transactions,
	ComplianceDocuments,
	ProjectCuryInfo,
	BaseCuryInfo,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.RegisterEntry",
	primaryView: "Document", showUDFIndicator: true
})
export class PM304000 extends PXScreen {
	Save: PXActionState;
	ViewProject: PXActionState;
	ViewTask: PXActionState;
	ViewCustomer: PXActionState;
	ViewInventory: PXActionState;
	ComplianceViewProject: PXActionState;
	ComplianceViewCostTask: PXActionState;
	ComplianceViewRevenueTask: PXActionState;
	ComplianceViewCostCode: PXActionState;
	ComplianceViewCustomer: PXActionState;
	ComplianceViewVendor: PXActionState;
	ComplianceDocument$ProjectTransactionID$Link: PXActionState;
	ComplianceDocument$ApCheckID$Link: PXActionState;
	ComplianceDocument$ArPaymentID$Link: PXActionState;
	ComplianceDocument$BillID$Link: PXActionState;
	ComplianceDocument$InvoiceID$Link: PXActionState;
	ComplianceViewJointVendor: PXActionState;
	ComplianceDocument$PurchaseOrder$Link: PXActionState;
	ComplianceDocument$Subcontract$Link: PXActionState;
	ComplianceDocument$ChangeOrderNumber$Link: PXActionState;
	ComplianceViewSecondaryVendor: PXActionState;

	Document = createSingle(Document);
	Transactions = createCollection(Transactions);
	ComplianceDocuments = createCollection(ComplianceDocuments);
	ProjectCuryInfo = createSingle(ProjectCuryInfo);
	BaseCuryInfo = createSingle(BaseCuryInfo);
}


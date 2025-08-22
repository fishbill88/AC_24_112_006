import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from "client-controls";

import {
	Task,
	TaskProperties,
	TaskCampaign,
	BillingItems,
	Activities,
	Answers,
	ComplianceDocuments,
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.ProjectTaskEntry",
	primaryView: "Task", showUDFIndicator: true
})
export class PM302000 extends PXScreen {
	OpenActivityOwner: PXActionState;
	ComplianceViewProject: PXActionState;
	ComplianceViewCostTask: PXActionState;
	ComplianceViewRevenueTask: PXActionState;
	ComplianceViewCostCode: PXActionState;
	ComplianceViewCustomer: PXActionState;
	ComplianceViewVendor: PXActionState;
	ComplianceDocument$BillID$Link: PXActionState;
	ComplianceDocument$ApCheckID$Link: PXActionState;
	ComplianceDocument$ArPaymentID$Link: PXActionState;
	ComplianceDocument$InvoiceID$Link: PXActionState;
	ComplianceViewJointVendor: PXActionState;
	ComplianceDocument$ProjectTransactionID$Link: PXActionState;
	ComplianceDocument$Subcontract$Link: PXActionState;
	ComplianceDocument$ChangeOrderNumber$Link: PXActionState;
	ComplianceDocument$PurchaseOrder$Link: PXActionState;
	ComplianceDocuments_Vendor_ViewDetails: PXActionState;

	Task = createSingle(Task);
	TaskProperties = createSingle(TaskProperties);
	TaskCampaign = createSingle(TaskCampaign);
	BillingItems = createCollection(BillingItems);
	Activities = createCollection(Activities);
	Answers = createCollection(Answers);
	ComplianceDocuments = createCollection(ComplianceDocuments);
}


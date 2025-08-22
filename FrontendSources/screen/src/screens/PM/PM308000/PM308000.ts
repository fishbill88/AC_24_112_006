import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	Document,
	VisibilitySettings,
	AvailableCostBudget,
	AvailableRevenueBudget,
	AvailablePOLineFilter,
	AvailablePOLines,
	AvailableChangeRequests,
	ChangeRequestCostDetails,
	ChangeRequestRevenueDetails,
	ChangeRequestMarkupDetails,
	ReversingChangeOrders,
	DocumentSettings,
	ChangeRequests,
	RevenueBudget,
	CostBudget,
	Details,
	Answers,
	Approval,
	ComplianceDocuments,
	ReasonApproveRejectParams,
	ReassignApprovalFilter
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PM.ChangeOrderEntry",
	primaryView: "Document",
	udfTypeField: "ClassID",
	showActivitiesIndicator: true, showUDFIndicator: true
})
export class PM308000 extends PXScreen {
	ViewReversingChangeOrders: PXActionState;
	ViewChangeOrder: PXActionState;
	ViewChangeRequest: PXActionState;
	ViewCurrentReversingChangeOrder: PXActionState;
	ViewRevenueBudgetTask: PXActionState;
	ViewRevenueBudgetInventory: PXActionState;
	ViewCostBudgetTask: PXActionState;
	ViewCostBudgetInventory: PXActionState;
	ViewCommitmentTask: PXActionState;
	ViewCommitmentInventory: PXActionState;
	ViewCommitments: PXActionState;
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
	ComplianceDocument$PurchaseOrder$Link: PXActionState;
	ComplianceDocument$Subcontract$Link: PXActionState;
	ComplianceDocument$ChangeOrderNumber$Link: PXActionState;
	ComplianceViewSecondaryVendor: PXActionState;
	AppendSelectedCostBudget: PXActionState;
	AppendSelectedRevenueBudget: PXActionState;
	AppendSelectedPOLines: PXActionState;
	AppendSelectedChangeRequests: PXActionState;

	Document = createSingle(Document);
	VisibilitySettings = createSingle(VisibilitySettings);
	AvailableCostBudget = createCollection(AvailableCostBudget);
	AvailableRevenueBudget = createCollection(AvailableRevenueBudget);
	AvailablePOLineFilter = createSingle(AvailablePOLineFilter);
	AvailablePOLines = createCollection(AvailablePOLines);
	AvailableChangeRequests = createCollection(AvailableChangeRequests);
	ChangeRequestCostDetails = createCollection(ChangeRequestCostDetails);
	ChangeRequestRevenueDetails = createCollection(ChangeRequestRevenueDetails);
	ChangeRequestMarkupDetails = createCollection(ChangeRequestMarkupDetails);
	ReversingChangeOrders = createCollection(ReversingChangeOrders);
	DocumentSettings = createSingle(DocumentSettings);
	ChangeRequests = createCollection(ChangeRequests);
	RevenueBudget = createCollection(RevenueBudget);
	CostBudget = createCollection(CostBudget);
	Details = createCollection(Details);
	Answers = createCollection(Answers);
	Approval = createCollection(Approval);
	ComplianceDocuments = createCollection(ComplianceDocuments);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);

	afterConstructor() {
		super.afterConstructor();
		this.screenService.registerViewBinding(this.element, VisibilitySettings.name);
	}
}

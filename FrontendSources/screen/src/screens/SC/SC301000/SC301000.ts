import {
	createCollection,
	createSingle,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	filter,
	poLinesSelection,
	openOrders,
	FixedDemand,
	ReplenishmentLines,
	Document,
	CurrentDocument,
	Transactions,
	Taxes,
	Remit_Contact,
	Remit_Address,
	Approval,
	DiscountDetails,
	APDocs,
	PrepaymentDocuments,
	ChangeOrders,
	Answers,
	ComplianceDocuments,
	ItemFilter,
	ItemInfo,
	recalcdiscountsfilter,
	CurrencyInfo,
	ProjectItemFilter,
	AvailableProjectItems,
	ReassignApprovalFilter
} from "./views";

@graphInfo({
	graphType: "PX.Objects.CN.Subcontracts.SC.Graphs.SubcontractEntry",
	primaryView: "Document"
})
export class SC301000 extends PXScreen {
	ViewChangeOrder: PXActionState;
	ViewOrigChangeOrder: PXActionState;
	ComplianceViewProject: PXActionState;
	ComplianceViewCostTask: PXActionState;
	ComplianceViewRevenueTask: PXActionState;
	ComplianceViewCostCode: PXActionState;
	ComplianceViewVendor: PXActionState;
	ComplianceDocument$PurchaseOrder$Link: PXActionState;
	ComplianceDocument$Subcontract$Link: PXActionState;
	ComplianceDocument$ChangeOrderNumber$Link: PXActionState;
	ComplianceDocument$ApCheckID$Link: PXActionState;
	ComplianceDocument$ArPaymentID$Link: PXActionState;
	ComplianceDocument$BillID$Link: PXActionState;
	ComplianceViewCustomer: PXActionState;
	ComplianceDocument$InvoiceID$Link: PXActionState;
	ComplianceViewJointVendor: PXActionState;
	ComplianceDocument$ProjectTransactionID$Link: PXActionState;
	ComplianceViewSecondaryVendor: PXActionState;
	AddSelectedItems: PXActionState;
	RecalcOk: PXActionState;
	AppendSelectedProjectItems: PXActionState;

	AddressLookup: PXActionState;
	AddressLookupSelectAction: PXActionState;
	RemitAddressLookup: PXActionState;
	ShowMatrixPanel: PXActionState;
	NewVendor: PXActionState;
	EditVendor: PXActionState;
	AddPOOrder: PXActionState;
	AddPOOrderLine: PXActionState;
	CreatePOReceipt: PXActionState;
	CreatePrepayment: PXActionState;
	AddInvBySite: PXActionState;
	ViewDemand: PXActionState;
	ResetOrder: PXActionState;

	Document = createSingle(Document);
	filter = createSingle(filter);
	poLinesSelection = createCollection(poLinesSelection);
	openOrders = createCollection(openOrders);
	FixedDemand = createCollection(FixedDemand);
	ReplenishmentLines = createCollection(ReplenishmentLines);
	CurrentDocument = createSingle(CurrentDocument);
	Transactions = createCollection(Transactions);
	Taxes = createCollection(Taxes);
	Remit_Contact = createSingle(Remit_Contact);
	Remit_Address = createSingle(Remit_Address);
	Approval = createCollection(Approval);
	DiscountDetails = createCollection(DiscountDetails);
	APDocs = createCollection(APDocs);
	PrepaymentDocuments = createCollection(PrepaymentDocuments);
	ChangeOrders = createCollection(ChangeOrders);
	Answers = createCollection(Answers);
	ComplianceDocuments = createCollection(ComplianceDocuments);
	ItemFilter = createSingle(ItemFilter);
	ItemInfo = createCollection(ItemInfo);
	recalcdiscountsfilter = createSingle(recalcdiscountsfilter);
	CurrencyInfo = createSingle(CurrencyInfo);
	ProjectItemFilter = createSingle(ProjectItemFilter);
	AvailableProjectItems = createCollection(AvailableProjectItems);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

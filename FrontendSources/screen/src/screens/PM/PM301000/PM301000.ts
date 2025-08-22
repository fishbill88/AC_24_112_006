import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	handleEvent,
	CustomEventType,
	RowCssHandlerArgs,
	graphInfo
} from 'client-controls';

import {
	Project,
	TaskTotals,
	ProjectProperties,
	ProjectRevenueTotals,
	Billing,
	RetainageSteps,
	Tasks,
	RevenueFilter,
	RevenueBudget,
	CostFilter,
	CostBudget,
	BalanceRecords,
	PurchaseOrders,
	Invoices,
	ChangeOrders,
	ChangeRequests,
	Unions,
	Activities,
	EmployeeContract,
	ContractRates,
	EquipmentRates,
	Site_Address,
	Billing_Contact,
	Billing_Address,
	Accounts,
	Markups,
	Answers,
	Approval,
	NotificationSources,
	NotificationRecipients,
	ComplianceDocuments,
	LienWaiverRecipients,
	ProjectContacts,
	ProjectProdOrders,
	ProjectEstimates,
	CreateProductionOrderFilter,
	TemplateSettings,
	ChangeIDDialog,
	TasksForAddition,
	CopyDialog,
	LoadFromTemplateDialog,
	CurrencyInfo,
	ReasonApproveRejectParams,
	ReassignApprovalFilter,
	DocumentSettings
} from './views';

import {
	PMConstants
} from "../pm-constants";

@graphInfo({
	graphType: 'PX.Objects.PM.ProjectEntry',
	primaryView: 'Project', showUDFIndicator: true
})
export class PM301000 extends PXScreen {
	Save: PXActionState;
	ViewTask: PXActionState;
	ViewRevenueBudgetInventory: PXActionState;
	ViewCostBudgetInventory: PXActionState;
	ViewPurchaseOrder: PXActionState;
	ViewProforma: PXActionState;
	ViewInvoice: PXActionState;
	ViewOrigDocument: PXActionState;
	ViewChangeOrder: PXActionState;
	ViewOrigChangeOrder: PXActionState;
	ViewChangeRequest: PXActionState;
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
	ComplianceDocument$PurchaseOrder$Link: PXActionState;
	ComplianceDocument$Subcontract$Link: PXActionState;
	ComplianceDocument$ChangeOrderNumber$Link: PXActionState;
	ComplianceViewSecondaryVendor: PXActionState;
	Relations_EntityDetails: PXActionState;
	ViewProdOrder: PXActionState;
	SetCurrencyRates: PXActionState;
	ViewAddressOnMap: PXActionState;
	AddTasks: PXActionState;
	LoadFromTemplate: PXActionState;

	Project = createSingle(Project);
	TaskTotals = createSingle(TaskTotals);
	ProjectProperties = createSingle(ProjectProperties);
	ProjectRevenueTotals = createSingle(ProjectRevenueTotals);
	Billing = createSingle(Billing);
	RetainageSteps = createCollection(RetainageSteps);
	Tasks = createCollection(Tasks);
	RevenueFilter = createSingle(RevenueFilter);
	RevenueBudget = createCollection(RevenueBudget);
	CostFilter = createSingle(CostFilter);
	CostBudget = createCollection(CostBudget);
	BalanceRecords = createCollection(BalanceRecords);
	PurchaseOrders = createCollection(PurchaseOrders);
	Invoices = createCollection(Invoices);
	ChangeOrders = createCollection(ChangeOrders);
	ChangeRequests = createCollection(ChangeRequests);
	Unions = createCollection(Unions);
	Activities = createCollection(Activities);
	EmployeeContract = createCollection(EmployeeContract);
	ContractRates = createCollection(ContractRates);
	EquipmentRates = createCollection(EquipmentRates);
	Site_Address = createSingle(Site_Address);
	Billing_Contact = createSingle(Billing_Contact);
	Billing_Address = createSingle(Billing_Address);
	Accounts = createCollection(Accounts);
	Markups = createCollection(Markups);
	Answers = createCollection(Answers);
	Approval = createCollection(Approval);
	NotificationSources = createCollection(NotificationSources);
	NotificationRecipients = createCollection(NotificationRecipients);
	ComplianceDocuments = createCollection(ComplianceDocuments);
	LienWaiverRecipients = createCollection(LienWaiverRecipients);
	ProjectContacts = createCollection(ProjectContacts);
	ProjectProdOrders = createCollection(ProjectProdOrders);
	ProjectEstimates = createCollection(ProjectEstimates);
	CreateProductionOrderFilter = createSingle(CreateProductionOrderFilter);
	TemplateSettings = createSingle(TemplateSettings);
	ChangeIDDialog = createSingle(ChangeIDDialog);
	TasksForAddition = createCollection(TasksForAddition);
	CopyDialog = createSingle(CopyDialog);
	LoadFromTemplateDialog = createSingle(LoadFromTemplateDialog);
	CurrencyInfo = createSingle(CurrencyInfo);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
	DocumentSettings = createSingle(DocumentSettings);

	@handleEvent(CustomEventType.GetRowCss, { view: BalanceRecords.name })
	getItemsRowCss(args: RowCssHandlerArgs) {
		return args?.selector?.row?.RecordID.value < 0
			? PMConstants.BoldRowCssClass
			: undefined;
	}
}


import {
	columnConfig,
	gridConfig,
	headerDescription,
	linkCommand,
	GridColumnShowHideMode,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView,
	GridPreset
} from "client-controls";

export class Document extends PXView {
	RefNbr: PXFieldState;
	Status: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Hidden>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletionDate: PXFieldState;
	DelayDays: PXFieldState;
	@linkCommand("ViewReversingChangeOrders")
	ReversingRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState;
	ExtRefNbr: PXFieldState;
	ProjectNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewChangeOrder")
	OrigRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	RevenueTotal: PXFieldState<PXFieldOptions.Disabled>;
	CommitmentTotal: PXFieldState<PXFieldOptions.Disabled>;
	CostTotal: PXFieldState<PXFieldOptions.Disabled>;
	GrossMarginAmount: PXFieldState<PXFieldOptions.Disabled>;
	GrossMarginPct: PXFieldState<PXFieldOptions.Disabled>;
	ChangeRequestCostTotal: PXFieldState;
	ChangeRequestLineTotal: PXFieldState;
	ChangeRequestMarkupTotal: PXFieldState;
	ChangeRequestPriceTotal: PXFieldState;
	@headerDescription
	FormCaptionDescription: PXFieldState;
}

export class VisibilitySettings extends PXView {
	IsRevenueVisible: PXFieldState;
	IsCostVisible: PXFieldState;
	IsDetailsVisible: PXFieldState;
	IsChangeRequestVisible: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class AvailableCostBudget extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ChangeOrderQty: PXFieldState;
	CuryChangeOrderAmount: PXFieldState;
	CommittedQty: PXFieldState;
	CuryCommittedAmount: PXFieldState;
	CommittedReceivedQty: PXFieldState;
	CommittedInvoicedQty: PXFieldState;
	CuryCommittedInvoicedAmount: PXFieldState;
	CommittedOpenQty: PXFieldState;
	CuryCommittedOpenAmount: PXFieldState;
	ActualQty: PXFieldState;
	CuryActualAmount: PXFieldState;
	CuryActualPlusOpenCommittedAmount: PXFieldState;
	CuryVarianceAmount: PXFieldState;
	Performance: PXFieldState;
	IsProduction: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLastCostToComplete: PXFieldState;
	CuryCostToComplete: PXFieldState;
	LastPercentCompleted: PXFieldState;
	PercentCompleted: PXFieldState;
	CuryLastCostAtCompletion: PXFieldState;
	CuryCostAtCompletion: PXFieldState;
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class AvailableRevenueBudget extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Type: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	LimitQty: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxQty: PXFieldState;
	LimitAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryMaxAmount: PXFieldState;
	ChangeOrderQty: PXFieldState;
	CuryChangeOrderAmount: PXFieldState;
	CommittedQty: PXFieldState;
	CuryCommittedAmount: PXFieldState;
	CommittedReceivedQty: PXFieldState;
	CommittedInvoicedQty: PXFieldState;
	CuryCommittedInvoicedAmount: PXFieldState;
	CommittedOpenQty: PXFieldState;
	CuryCommittedOpenAmount: PXFieldState;
	CuryInvoicedAmount: PXFieldState;
	ActualQty: PXFieldState;
	CuryActualAmount: PXFieldState;
	CuryActualPlusOpenCommittedAmount: PXFieldState;
	CuryVarianceAmount: PXFieldState;
	PrepaymentPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaymentAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaymentInvoiced: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaymentAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletedPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmountToInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	Performance: PXFieldState;
	TaxCategoryID: PXFieldState;
}

export class AvailablePOLineFilter extends PXView {
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	POOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeTo: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeNonOpen: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class AvailablePOLines extends PXView {
	@columnConfig({
		allowCheckAll: true
	})
	Selected: PXFieldState;
	TaskID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	VendorID: PXFieldState;
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	OrderDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	LineNbr: PXFieldState;
	TranDesc: PXFieldState;
	OrderQty: PXFieldState;
	UOM: PXFieldState;
	CuryUnitCost: PXFieldState;
	ReceivedQty: PXFieldState;
	CuryExtCost: PXFieldState;
	PromisedDate: PXFieldState;
	VendorRefNbr: PXFieldState;
	AlternateID: PXFieldState;
	Cancelled: PXFieldState;
	Completed: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class AvailableChangeRequests extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@linkCommand("ViewChangeRequest")
	RefNbr: PXFieldState;
	Date: PXFieldState;
	ExtRefNbr: PXFieldState;
	Description: PXFieldState;
	CostTotal: PXFieldState;
	LineTotal: PXFieldState;
	MarkupTotal: PXFieldState;
	PriceTotal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true
})
export class ChangeRequestCostDetails extends PXView {
	@linkCommand("ViewChangeRequest")
	RefNbr: PXFieldState;
	Description: PXFieldState;
	Qty: PXFieldState;
	UnitCost: PXFieldState;
	ExtCost: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true
})
export class ChangeRequestRevenueDetails extends PXView {
	@linkCommand("ViewChangeRequest")
	RefNbr: PXFieldState;
	Description: PXFieldState;
	Qty: PXFieldState;
	UnitPrice: PXFieldState;
	ExtPrice: PXFieldState;
	LineAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true
})
export class ChangeRequestMarkupDetails extends PXView {
	@linkCommand("ViewChangeRequest")
	RefNbr: PXFieldState;
	Type: PXFieldState;
	Description: PXFieldState;
	Value: PXFieldState;
	MarkupAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class ReversingChangeOrders extends PXView {
	@linkCommand("ViewCurrentReversingChangeOrder")
	RefNbr: PXFieldState;
	Description: PXFieldState;
}

export class DocumentSettings extends PXView {
	Text: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false
})
export class ChangeRequests extends PXView {
	AddChangeRequests: PXActionState;

	@linkCommand("ViewChangeRequest")
	RefNbr: PXFieldState;
	Status: PXFieldState;
	Description: PXFieldState;
	CostTotal: PXFieldState;
	LineTotal: PXFieldState;
	MarkupTotal: PXFieldState;
	PriceTotal: PXFieldState;
	DelayDays: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class RevenueBudget extends PXView {
	ViewChangeRequestRevenueDetails: PXActionState;
	AddRevenueBudget: PXActionState;

	@linkCommand("ViewRevenueBudgetTask")
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewRevenueBudgetInventory")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	ChangeRequestQty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	Rate: PXFieldState<PXFieldOptions.CommitChanges>;
	ChangeRequestAmount: PXFieldState;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	PMBudget__Qty: PXFieldState;
	PMBudget__CuryAmount: PXFieldState;
	PreviouslyApprovedQty: PXFieldState;
	PreviouslyApprovedAmount: PXFieldState;
	RevisedQty: PXFieldState;
	RevisedAmount: PXFieldState;
	PMBudget__CuryInvoicedAmount: PXFieldState;
	PMBudget__ActualQty: PXFieldState;
	PMBudget__CuryActualAmount: PXFieldState;
	PMBudget__CompletedPct: PXFieldState;
	OtherDraftRevisedAmount: PXFieldState;
	TotalPotentialRevisedAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class CostBudget extends PXView {
	ViewChangeRequestCostDetails: PXActionState;
	AddCostBudget: PXActionState;

	@linkCommand("ViewCostBudgetTask")
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewCostBudgetInventory")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	ChangeRequestQty: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	Rate: PXFieldState<PXFieldOptions.CommitChanges>;
	ChangeRequestAmount: PXFieldState;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	PMBudget__Qty: PXFieldState;
	PMBudget__CuryAmount: PXFieldState;
	PreviouslyApprovedQty: PXFieldState;
	PreviouslyApprovedAmount: PXFieldState;
	RevisedQty: PXFieldState;
	RevisedAmount: PXFieldState;
	PMBudget__CommittedQty: PXFieldState;
	PMBudget__CuryCommittedAmount: PXFieldState;
	PMBudget__CommittedReceivedQty: PXFieldState;
	PMBudget__CommittedInvoicedQty: PXFieldState;
	PMBudget__CuryCommittedInvoicedAmount: PXFieldState;
	PMBudget__CommittedOpenQty: PXFieldState;
	PMBudget__CuryCommittedOpenAmount: PXFieldState;
	PMBudget__CommittedCOQty: PXFieldState;
	PMBudget__CuryCommittedCOAmount: PXFieldState;
	PMBudget__ActualQty: PXFieldState;
	PMBudget__CuryActualAmount: PXFieldState;
	CommittedCOQty: PXFieldState;
	CommittedCOAmount: PXFieldState;
	OtherDraftRevisedAmount: PXFieldState;
	TotalPotentialRevisedAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class Details extends PXView {
	AddPOLines: PXActionState;

	@linkCommand("ViewChangeRequest")
	ChangeRequestRefNbr: PXFieldState;
	LineType: PXFieldState;
	@linkCommand("ViewCommitmentTask")
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewCommitmentInventory")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	POOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewCommitments")
	POOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	POLinePM__OrderDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	POLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	POLinePM__TranDesc: PXFieldState;
	POLinePM__OrderQty: PXFieldState;
	POLinePM__CuryLineAmt: PXFieldState;
	POLinePM__CalcOpenQty: PXFieldState;
	POLinePM__CalcCuryOpenAmt: PXFieldState;
	AmountInProjectCury: PXFieldState<PXFieldOptions.CommitChanges>;
	PotentialRevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	PotentialRevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	HasExpiredComplianceDocuments: PXFieldState;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageAmtInProjectCury: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Attributes
})
export class Answers extends PXView {
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	@columnConfig({
		allowSort: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Value: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Approval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class ComplianceDocuments extends PXView {
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true,
		width: 200
	})
	DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;
	CreationDate: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		width: 200
	})
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 110 })
	Required: PXFieldState;
	@columnConfig({ width: 110 })
	Received: PXFieldState;
	ReceivedDate: PXFieldState;
	@columnConfig({ width: 110 })
	IsProcessed: PXFieldState;
	@columnConfig({ width: 110 })
	IsVoided: PXFieldState;
	@columnConfig({ width: 110 })
	IsCreatedAutomatically: PXFieldState;
	SentDate: PXFieldState;
	@linkCommand("ComplianceViewProject")
	ProjectID: PXFieldState;
	@linkCommand("ComplianceViewCostTask")
	CostTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceViewRevenueTask")
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceViewCostCode")
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceViewCustomer")
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerName: PXFieldState;
	@linkCommand("ComplianceViewVendor")
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorName: PXFieldState;
	@linkCommand("ComplianceDocument$BillID$Link")
	BillID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillAmount: PXFieldState;
	AccountID: PXFieldState;
	@linkCommand("ComplianceDocument$ApCheckID$Link")
	ApCheckID: PXFieldState<PXFieldOptions.CommitChanges>;
	CheckNumber: PXFieldState;
	@linkCommand("ComplianceDocument$ArPaymentID$Link")
	ArPaymentID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertificateNumber: PXFieldState;
	CreatedByID: PXFieldState;
	DateIssued: PXFieldState;
	EffectiveDate: PXFieldState;
	InsuranceCompany: PXFieldState;
	InvoiceAmount: PXFieldState;
	@linkCommand("ComplianceDocument$InvoiceID$Link")
	InvoiceID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsExpired: PXFieldState;
	IsRequiredJointCheck: PXFieldState;
	JointAmount: PXFieldState;
	JointRelease: PXFieldState;
	JointReleaseReceived: PXFieldState;
	@linkCommand("ComplianceViewJointVendor")
	JointVendorInternalId: PXFieldState;
	JointVendorExternalName: PXFieldState;
	LastModifiedByID: PXFieldState;
	LienWaiverAmount: PXFieldState;
	Limit: PXFieldState;
	MethodSent: PXFieldState;
	PaymentDate: PXFieldState;
	ArPaymentMethodID: PXFieldState;
	ApPaymentMethodID: PXFieldState;
	Policy: PXFieldState;
	@linkCommand("ComplianceDocument$ProjectTransactionID$Link")
	ProjectTransactionID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceDocument$PurchaseOrder$Link")
	PurchaseOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchaseOrderLineItem: PXFieldState;
	@linkCommand("ComplianceDocument$Subcontract$Link")
	Subcontract: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractLineItem: PXFieldState;
	@linkCommand("ComplianceDocument$ChangeOrderNumber$Link")
	ChangeOrderNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptDate: PXFieldState;
	ReceiveDate: PXFieldState;
	ReceivedBy: PXFieldState;
	@linkCommand("ComplianceViewSecondaryVendor")
	SecondaryVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	SecondaryVendorName: PXFieldState;
	SourceType: PXFieldState;
	SponsorOrganization: PXFieldState;
	ThroughDate: PXFieldState;
	@columnConfig({hideViewLink: true})
	DocumentTypeValue: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

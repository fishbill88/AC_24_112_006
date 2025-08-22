import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	headerDescription,
	gridConfig,
	GridPreset
} from "client-controls";

export class Task extends PXView {
	ProjectID: PXFieldState;
	TaskCD: PXFieldState;
	Type: PXFieldState;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	IsDefault: PXFieldState;
	@headerDescription FormCaptionDescription: PXFieldState;
}

export class TaskProperties extends PXView {
	PlannedStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	PlannedEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletedPctMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletedPercent: PXFieldState;
	ApproverID: PXFieldState;
	BillSeparately: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.Disabled>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	AllocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	RateTableID: PXFieldState;
	BillingOption: PXFieldState;
	WipAccountGroupID: PXFieldState;
	ProgressBillingBase: PXFieldState;
	TaxCategoryID: PXFieldState;
	DefaultSalesAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultSalesSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultAccrualAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultAccrualSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningsAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningsSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PTOExpenseAcctID: PXFieldState;
	PTOExpenseSubID: PXFieldState;
	VisibleInGL: PXFieldState;
	VisibleInAP: PXFieldState;
	VisibleInAR: PXFieldState;
	VisibleInSO: PXFieldState;
	VisibleInPO: PXFieldState;
	VisibleInIN: PXFieldState;
	VisibleInCA: PXFieldState;
	VisibleInCR: PXFieldState;
	VisibleInPROD: PXFieldState;
	VisibleInTA: PXFieldState;
	VisibleInEA: PXFieldState;
}

export class TaskCampaign extends PXView {
	CampaignID: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details
})
export class BillingItems extends PXView {
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Amount: PXFieldState;
	AccountSource: PXFieldState<PXFieldOptions.CommitChanges>;
	SubMask: PXFieldState;
	@columnConfig({hideViewLink: true})
	BranchId: PXFieldState;
	@columnConfig({hideViewLink: true})
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState;
	ResetUsage: PXFieldState;
	Included: PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowDelete: false
})
export class Activities extends PXView {
	NewTask: PXActionState;
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;
	ViewActivity: PXActionState;

	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	IsCompleteIcon: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	PriorityIcon: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	CRReminder__ReminderIcon: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	ClassIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	RefNoteID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ViewActivity")
	Summary: PXFieldState;
	ApprovalStatus: PXFieldState;
	Released: PXFieldState;
	StartDate: PXFieldState;
	CategoryID: PXFieldState;
	IsBillable: PXFieldState;
	TimeSpent: PXFieldState;
	OvertimeSpent: PXFieldState;
	TimeBillable: PXFieldState;
	OvertimeBillable: PXFieldState;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({hideViewLink: true})
	WorkgroupID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	TimeActivityOwner: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	body: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowDelete: false
})
export class Answers extends PXView {
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class ComplianceDocuments extends PXView {
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;
	CreationDate: PXFieldState;
	@columnConfig({hideViewLink: true})
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Required: PXFieldState;
	Received: PXFieldState;
	ReceivedDate: PXFieldState;
	IsProcessed: PXFieldState;
	IsVoided: PXFieldState;
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
	PurchaseOrderLineItem: PXFieldState;
	@linkCommand("ComplianceDocument$Subcontract$Link")
	Subcontract: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractLineItem: PXFieldState;
	@linkCommand("ComplianceDocument$ChangeOrderNumber$Link")
	ChangeOrderNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceDocument$PurchaseOrder$Link")
	PurchaseOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiveDate: PXFieldState;
	ReceivedBy: PXFieldState;
	@linkCommand("ComplianceDocuments_Vendor_ViewDetails")
	SecondaryVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	SecondaryVendorName: PXFieldState;
	SourceType: PXFieldState;
	SponsorOrganization: PXFieldState;
	ThroughDate: PXFieldState;
	DocumentTypeValue: PXFieldState<PXFieldOptions.CommitChanges>;
}


import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnShowHideMode,
	GridColumnDisplayMode,
	linkCommand,
	columnConfig,
	GridPreset,
	gridConfig
} from "client-controls";

export class Project extends PXView {
	ContractCD: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	NonProject: PXFieldState;
}

export class ProjectProperties extends PXView {
	BudgetLevel: PXFieldState<PXFieldOptions.CommitChanges>;
	CostBudgetLevel: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState;
	ApproverID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountingMode: PXFieldState;
	ChangeOrderWorkflow: PXFieldState;
	RestrictToEmployeeList: PXFieldState;
	RestrictToResourceList: PXFieldState;
	BudgetMetricsEnabled: PXFieldState;
	TermsID: PXFieldState;
	AllocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoAllocate: PXFieldState;
	BillingID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultBranchID: PXFieldState;
	RateTableID: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateProforma: PXFieldState<PXFieldOptions.CommitChanges>;
	LimitsEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentDefCode: PXFieldState;
	AutomaticReleaseAR: PXFieldState;
	VisibleInGL: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInAP: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInAR: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInSO: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInPO: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInIN: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInCA: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInCR: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInPROD: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInTA: PXFieldState<PXFieldOptions.CommitChanges>;
	VisibleInEA: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageMode: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeCO: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	SteppedRetainage: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageMaxPct: PXFieldState;
	AIALevel: PXFieldState<PXFieldOptions.CommitChanges>;
	LastProformaNumber: PXFieldState;
	IncludeQtyInAIA: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultSalesAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultSalesSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultAccrualAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultAccrualSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DropshipExpenseAccountSource: PXFieldState;
	DropshipExpenseSubMask: PXFieldState;
	DropshipReceiptProcessing: PXFieldState<PXFieldOptions.CommitChanges>;
	DropshipExpenseRecording: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningsAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningsSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PTOExpenseAcctID: PXFieldState;
	PTOExpenseSubID: PXFieldState;
	ThroughDateSourceConditional: PXFieldState;
	IsActive: PXFieldState;
	ThroughDateSourceUnconditional: PXFieldState;
}

export class Billing extends PXView {
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: true,
	allowDelete: true,
	preset: GridPreset.ShortList
})
export class RetainageSteps extends PXView {
	ThresholdPct: PXFieldState;
	RetainagePct: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true,
	preset: GridPreset.Details
})
export class Tasks extends PXView {
	@linkCommand("ViewTask")
	TaskCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	RateTableID: PXFieldState;
	AllocationID: PXFieldState;
	BillingID: PXFieldState;
	ApproverID: PXFieldState;
	BillingOption: PXFieldState;
	ProgressBillingBase: PXFieldState;
	TaxCategoryID: PXFieldState;
	IsDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	BillSeparately: PXFieldState;
	EarningsAcctID: PXFieldState;
	EarningsSubID: PXFieldState;
	BenefitExpenseAcctID: PXFieldState;
	BenefitExpenseSubID: PXFieldState;
	TaxExpenseAcctID: PXFieldState;
	TaxExpenseSubID: PXFieldState;
	PTOExpenseAcctID: PXFieldState;
	PTOExpenseSubID: PXFieldState;
	DefaultSalesSubID: PXFieldState;
	DefaultExpenseSubID: PXFieldState;
	DefaultSalesAccountID: PXFieldState;
	DefaultExpenseAccountID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true,
	preset: GridPreset.Details
})
export class RevenueBudget extends PXView {
	@columnConfig({hideViewLink: true})
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	LimitQty: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxQty: PXFieldState;
	LimitAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryMaxAmount: PXFieldState;
	PrepaymentPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaymentAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState;
	ProgressBillingBase: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true,
	preset: GridPreset.Details
})
export class CostBudget extends PXView {
	@columnConfig({hideViewLink: true})
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	IsProduction: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductivityTracking: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true,
	preset: GridPreset.Details
})
export class EmployeeContract extends PXView {
	@columnConfig({hideViewLink: true})
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	EPEmployee__AcctName: PXFieldState;
	@columnConfig({hideViewLink: true})
	EPEmployee__DepartmentID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	initNewRow: true,
	preset: GridPreset.Details
})
export class ContractRates extends PXView {
	@columnConfig({hideViewLink: true})
	EarningType: PXFieldState<PXFieldOptions.CommitChanges>;
	EPEarningType__Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	LabourItemID: PXFieldState;
	InventoryItem__BasePrice: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	preset: GridPreset.Details
})
export class EquipmentRates extends PXView {
	IsActive: PXFieldState;
	EquipmentID: PXFieldState;
	EPEquipment__Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	EPEquipment__RunRateItemID: PXFieldState;
	RunRate: PXFieldState;
	@columnConfig({hideViewLink: true})
	EPEquipment__SetupRateItemID: PXFieldState;
	SetupRate: PXFieldState;
	@columnConfig({hideViewLink: true})
	EPEquipment__SuspendRateItemID: PXFieldState;
	SuspendRate: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	preset: GridPreset.ShortList
})
export class Accounts extends PXView {
	@columnConfig({hideViewLink: true})
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	TaskID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	suppressNoteFiles: true,
	allowInsert: true,
	allowDelete: true,
	preset: GridPreset.ShortList
})
export class Markups extends PXView {
	Type: PXFieldState;
	Description: PXFieldState;
	Value: PXFieldState;
	@columnConfig({hideViewLink: true})
	TaskID: PXFieldState;
	@columnConfig({hideViewLink: true})
	AccountGroupID: PXFieldState;
	@columnConfig({hideViewLink: true})
	CostCodeID: PXFieldState;
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class Answers extends PXView {
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False,
		hideViewLink: true,
		displayMode: GridColumnDisplayMode.Text,
		width: 300
	})
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	preset: GridPreset.Details
})
export class LienWaiverRecipients extends PXView {
	AddAllVendorClasses: PXActionState;

	@columnConfig({hideViewLink: true})
	VendorClassId: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({width: 250})
	MinimumCommitmentAmount: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	preset: GridPreset.Details
})
export class NotificationSources extends PXView {
	Active: PXFieldState;
	@columnConfig({hideViewLink: true})
	SetupID: PXFieldState<PXFieldOptions.CommitChanges>;
	NBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	EMailAccountID: PXFieldState;
	@columnConfig({hideViewLink: true})
	ReportID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	NotificationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipientsBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	initNewRow: true,
	preset: GridPreset.Details
})
export class NotificationRecipients extends PXView {
	Active: PXFieldState;
	ContactType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	OriginalContactID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState;
	Email: PXFieldState;
	Format: PXFieldState<PXFieldOptions.CommitChanges>;
	AddTo: PXFieldState;
}

export class CopyDialog extends PXView {
	TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
}


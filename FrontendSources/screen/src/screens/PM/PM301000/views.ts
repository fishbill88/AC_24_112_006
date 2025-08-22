import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	ICurrencyInfo,
	gridConfig,
	GridColumnDisplayMode,
	GridPreset
} from "client-controls";

export class Project extends PXView {
	ContractCD: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	CuryID: PXFieldState;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Hold: PXFieldState<PXFieldOptions.Hidden>;
}

export class TaskTotals extends PXView {
	CuryIncome: PXFieldState<PXFieldOptions.Disabled>;
	CuryExpense: PXFieldState<PXFieldOptions.Disabled>;
	CuryMargin: PXFieldState<PXFieldOptions.Disabled>;
	MarginPct: PXFieldState<PXFieldOptions.Disabled>;
}

export class ProjectProperties extends PXView {
	BudgetLevel: PXFieldState<PXFieldOptions.CommitChanges>;
	CostBudgetLevel: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpireDate: PXFieldState;
	ProjectGroupID: PXFieldState;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ApproverID: PXFieldState<PXFieldOptions.CommitChanges>;
	LastChangeOrderNumber: PXFieldState;
	CuryIDCopy: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	RateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountingMode: PXFieldState;
	ChangeOrderWorkflow: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowNonProjectAccountGroups: PXFieldState;
	RestrictToEmployeeList: PXFieldState<PXFieldOptions.CommitChanges>;
	RestrictToResourceList: PXFieldState<PXFieldOptions.CommitChanges>;
	BudgetMetricsEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	CertifiedJob: PXFieldState;
	BillingCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayrollWorkLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	AllocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoAllocate: PXFieldState;
	BillingID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	RateTableID: PXFieldState<PXFieldOptions.CommitChanges>;
	CreateProforma: PXFieldState<PXFieldOptions.CommitChanges>;
	LimitsEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentDefCode: PXFieldState;
	AutomaticReleaseAR: PXFieldState;
	RetainageMode: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeCO: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	SteppedRetainage: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageMaxPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryCapAmount: PXFieldState<PXFieldOptions.NoLabel>;
	AIALevel: PXFieldState<PXFieldOptions.CommitChanges>;
	LastProformaNumber: PXFieldState;
	IncludeQtyInAIA: PXFieldState<PXFieldOptions.CommitChanges>;
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
	QuoteNbr: PXFieldState<PXFieldOptions.Disabled>;
	CostTaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueTaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
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

export class ProjectRevenueTotals extends PXView {
	CuryAmount: PXFieldState;
	CuryRevisedAmount: PXFieldState;
	ContractCompletedPct: PXFieldState;
	ContractCompletedWithCOPct: PXFieldState;
	CuryTotalRetainedAmount: PXFieldState;
}

export class Billing extends PXView {
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	NextDate: PXFieldState;
	LastDate: PXFieldState;
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
	ViewAddCommonTask: PXActionState;
	ActivateTasks: PXActionState;
	CompleteTasks: PXActionState;

	@linkCommand("ViewTask")
	TaskCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	RateTableID: PXFieldState;
	AllocationID: PXFieldState;
	BillingID: PXFieldState;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletedPercent: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	ApproverID: PXFieldState;
	BillingOption: PXFieldState;
	ProgressBillingBase: PXFieldState;
	TaxCategoryID: PXFieldState;
	IsDefault: PXFieldState<PXFieldOptions.CommitChanges>;
	BillSeparately: PXFieldState;
	DefaultSalesAccountID: PXFieldState;
	DefaultSalesSubID: PXFieldState;
	DefaultExpenseAccountID: PXFieldState;
	DefaultExpenseSubID: PXFieldState;
	PlannedStartDate: PXFieldState;
	PlannedEndDate: PXFieldState;
	EarningsAcctID: PXFieldState;
	EarningsSubID: PXFieldState;
	BenefitExpenseAcctID: PXFieldState;
	BenefitExpenseSubID: PXFieldState;
	TaxExpenseAcctID: PXFieldState;
	TaxExpenseSubID: PXFieldState;
	PTOExpenseAcctID: PXFieldState;
	PTOExpenseSubID: PXFieldState;
	CompletedPctMethod: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class RevenueFilter extends PXView {
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByTask: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmountToInvoiceTotal: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true,
	preset: GridPreset.Details
})
export class RevenueBudget extends PXView {
	ViewRevenueTransactions: PXActionState;
	Refresh: PXActionState;

	@columnConfig({hideViewLink: true})
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewRevenueBudgetInventory")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Type: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	DraftChangeOrderQty: PXFieldState;
	CuryDraftChangeOrderAmount: PXFieldState;
	RevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ChangeOrderQty: PXFieldState;
	CuryChangeOrderAmount: PXFieldState;
	LimitQty: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxQty: PXFieldState;
	LimitAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryMaxAmount: PXFieldState;
	CommittedQty: PXFieldState;
	CuryCommittedAmount: PXFieldState;
	CommittedReceivedQty: PXFieldState;
	CommittedInvoicedQty: PXFieldState;
	CuryCommittedInvoicedAmount: PXFieldState;
	CommittedOpenQty: PXFieldState;
	CuryCommittedOpenAmount: PXFieldState;
	InvoicedQty: PXFieldState;
	CuryInvoicedAmount: PXFieldState;
	ActualQty: PXFieldState;
	CuryActualAmount: PXFieldState;
	ActualAmount: PXFieldState;
	CuryActualPlusOpenCommittedAmount: PXFieldState;
	CuryVarianceAmount: PXFieldState;
	PrepaymentPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaymentAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaymentInvoiced: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaymentAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletedPct: PXFieldState<PXFieldOptions.CommitChanges>;
	QtyToInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmountToInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	Performance: PXFieldState;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState;
	RetainageMaxPct: PXFieldState;
	CuryCapAmount: PXFieldState;
	CuryDraftRetainedAmount: PXFieldState;
	CuryRetainedAmount: PXFieldState;
	CuryTotalRetainedAmount: PXFieldState;
	ProgressBillingBase: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CostFilter extends PXView {
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByTask: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true,
	preset: GridPreset.Details
})
export class CostBudget extends PXView {
	ViewCostCommitments: PXActionState;
	ViewCostTransactions: PXActionState;
	Refresh: PXActionState;

	@columnConfig({hideViewLink: true})
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewCostBudgetInventory")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Type: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	DraftChangeOrderQty: PXFieldState;
	CuryDraftChangeOrderAmount: PXFieldState;
	ChangeOrderQty: PXFieldState;
	CuryChangeOrderAmount: PXFieldState;
	RevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CommittedOrigQty: PXFieldState;
	CuryCommittedOrigAmount: PXFieldState;
	CommittedCOQty: PXFieldState;
	CuryCommittedCOAmount: PXFieldState;
	CommittedQty: PXFieldState;
	CuryCommittedAmount: PXFieldState;
	CommittedReceivedQty: PXFieldState;
	CommittedInvoicedQty: PXFieldState;
	CuryCommittedInvoicedAmount: PXFieldState;
	CommittedOpenQty: PXFieldState;
	CuryCommittedOpenAmount: PXFieldState;
	ActualQty: PXFieldState;
	CuryActualAmount: PXFieldState;
	ActualAmount: PXFieldState;
	CuryActualPlusOpenCommittedAmount: PXFieldState;
	CuryVarianceAmount: PXFieldState;
	Performance: PXFieldState;
	IsProduction: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryCostToComplete: PXFieldState;
	CuryCostAtCompletion: PXFieldState;
	PercentCompleted: PXFieldState;
	CuryLastCostToComplete: PXFieldState;
	CuryLastCostAtCompletion: PXFieldState;
	LastPercentCompleted: PXFieldState;
	CuryCostProjectionCostToComplete: PXFieldState;
	CuryCostProjectionCostAtCompletion: PXFieldState;
	CostProjectionQtyToComplete: PXFieldState;
	CostProjectionQtyAtCompletion: PXFieldState;
	CostProjectionCompletedPct: PXFieldState;
	CuryUnitPrice: PXFieldState;
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductivityTracking: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class BalanceRecords extends PXView {
	ViewBalanceTransactions: PXActionState;
	ViewCommitments: PXActionState;

	@columnConfig({allowShowHide: GridColumnShowHideMode.False})
	RecordID: PXFieldState<PXFieldOptions.Hidden>;
	AccountGroup: PXFieldState;
	Description: PXFieldState;
	CuryAmount: PXFieldState;
	CuryDraftCOAmount: PXFieldState;
	CuryBudgetedCOAmount: PXFieldState;
	CuryRevisedAmount: PXFieldState;
	CuryOriginalCommittedAmount: PXFieldState;
	CuryCommittedCOAmount: PXFieldState;
	CuryCommittedAmount: PXFieldState;
	CuryCommittedInvoicedAmount: PXFieldState;
	CuryActualAmount: PXFieldState;
	ActualAmount: PXFieldState;
	CuryCommittedOpenAmount: PXFieldState;
	CuryActualPlusOpenCommittedAmount: PXFieldState;
	CuryVarianceAmount: PXFieldState;
	Performance: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	CuryInclTaxAmount: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class PurchaseOrders extends PXView {
	CreatePurchaseOrder: PXActionState;
	CreateSubcontract: PXActionState;

	OrderType: PXFieldState;
	@linkCommand("ViewPurchaseOrder")
	OrderNbr: PXFieldState;
	OrderDate: PXFieldState;
	@columnConfig({hideViewLink: true})
	VendorID: PXFieldState;
	VendorID_Vendor_acctName: PXFieldState;
	POLine__CuryExtCost: PXFieldState;
	POLine__OrderQty: PXFieldState;
	OrderQty: PXFieldState;
	CuryOrderTotal: PXFieldState;
	@columnConfig({hideViewLink: true})
	CuryID: PXFieldState;
	Status: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class Invoices extends PXView {
	Aia: PXActionState;
	ViewReleaseRetainage: PXActionState;

	RecordNumber: PXFieldState;
	PMProforma__InvoiceDate: PXFieldState;
	@linkCommand("ViewProforma")
	ProformaRefNbr: PXFieldState;
	PMProforma__ProjectNbr: PXFieldState;
	PMProforma__Description: PXFieldState;
	PMProforma__Status: PXFieldState;
	PMProforma__CuryDocTotal: PXFieldState;
	@columnConfig({hideViewLink: true})
	PMProforma__CuryID: PXFieldState;
	ARDocType: PXFieldState;
	@linkCommand("ViewInvoice")
	ARRefNbr: PXFieldState;
	ARInvoice__DocDate: PXFieldState;
	ARInvoice__DocDesc: PXFieldState;
	ARInvoice__CuryOrigDocAmt: PXFieldState;
	ARInvoice__CuryRetainageTotal: PXFieldState;
	ARInvoice__CuryOrigDocAmtWithRetainageTotal: PXFieldState;
	ARInvoice__CuryDocBal: PXFieldState;
	@columnConfig({hideViewLink: true})
	ARInvoice__CuryID: PXFieldState;
	ARInvoice__Status: PXFieldState;
	ARInvoice__CuryRetainageUnreleasedAmt: PXFieldState;
	ARInvoice__IsRetainageDocument: PXFieldState;
	@linkCommand("ViewOrigDocument")
	ARInvoice__OrigRefNbr: PXFieldState;
	PMProforma__IsMigratedRecord: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class ChangeOrders extends PXView {
	CreateChangeOrder: PXActionState;

	@linkCommand("ViewChangeOrder")
	RefNbr: PXFieldState;
	@columnConfig({hideViewLink: true})
	ClassID: PXFieldState;
	ProjectNbr: PXFieldState;
	Status: PXFieldState;
	Description: PXFieldState;
	Date: PXFieldState;
	CompletionDate: PXFieldState;
	DelayDays: PXFieldState;
	ExtRefNbr: PXFieldState;
	RevenueTotal: PXFieldState;
	CommitmentTotal: PXFieldState;
	CostTotal: PXFieldState;
	ReverseStatus: PXFieldState;
	@linkCommand("ViewOrigChangeOrder")
	OrigRefNbr: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class ChangeRequests extends PXView {
	@linkCommand("ViewChangeRequest")
	RefNbr: PXFieldState;
	Status: PXFieldState;
	Date: PXFieldState;
	Description: PXFieldState;
	CostTotal: PXFieldState;
	LineTotal: PXFieldState;
	MarkupTotal: PXFieldState;
	PriceTotal: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	preset: GridPreset.Details
})
export class Unions extends PXView {
	UnionID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnionID_Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	preset: GridPreset.Details
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
	WorkgroupID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	TimeActivityOwner: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.True
	})
	ProjectID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.True
	})
	ProjectTaskID: PXFieldState<PXFieldOptions.Hidden>;
	body: PXFieldState;
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
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	initNewRow: true,
	preset: GridPreset.Details
})
export class EquipmentRates extends PXView {
	IsActive: PXFieldState;
	EquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
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

export class Site_Address extends PXView {
	AddressLine1: PXFieldState;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryId: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState<PXFieldOptions.CommitChanges>;
	Longitude: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Billing_Contact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class Billing_Address extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	preset: GridPreset.Details
})
export class Accounts extends PXView {
	@columnConfig({hideViewLink: true})
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
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
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class Approval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({
		allowUpdate: false
	})
	Status: PXFieldState;
	@columnConfig({
		allowUpdate: false
	})
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
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

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true,
	preset: GridPreset.Details
})
export class ComplianceDocuments extends PXView {
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;
	CreationDate: PXFieldState;
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
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
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
	ThroughDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentTypeValue: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	preset: GridPreset.ShortList
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
	syncPosition: true,
	adjustPageSize: true,
	preset: GridPreset.Details
})
export class ProjectContacts extends PXView {
	@linkCommand("Relations_EntityDetails")
	BusinessAccountId: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactId: PXFieldState<PXFieldOptions.CommitChanges>;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone: PXFieldState<PXFieldOptions.CommitChanges>;
	Role: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class ProjectProdOrders extends PXView {
	CreateProdOrder: PXActionState;

	OrderType: PXFieldState;
	@linkCommand("ViewProdOrder")
	ProdOrdID: PXFieldState;
	InventoryID: PXFieldState;
	SiteID: PXFieldState;
	LocationID: PXFieldState;
	ProdDate: PXFieldState;
	QtytoProd: PXFieldState;
	QtyComplete: PXFieldState;
	QtyScrapped: PXFieldState;
	QtyRemaining: PXFieldState;
	TaskID: PXFieldState;
	CostCodeID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class ProjectEstimates extends PXView {
	AMEstimateReference__EstimateID: PXFieldState;
	InventoryCD: PXFieldState;
	SiteID: PXFieldState;
	OrderQty: PXFieldState;
	AMEstimateReference__TaskID: PXFieldState;
	AMEstimateReference__CostCodeID: PXFieldState;
}

export class CreateProductionOrderFilter extends PXView {
	OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState;
	ProdDate: PXFieldState;
	QtytoProd: PXFieldState;
	ProjectID: PXFieldState;
	TaskID: PXFieldState;
	CostCodeID: PXFieldState;
}

export class TemplateSettings extends PXView {
	TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ChangeIDDialog extends PXView {
	CD: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
	preset: GridPreset.Details
})
export class TasksForAddition extends PXView {
	@columnConfig({
		allowCheckAll: true
	})
	Selected: PXFieldState;
	TaskCD: PXFieldState;
	Description: PXFieldState;
	ApproverID: PXFieldState;
	PMProject__NonProject: PXFieldState;
}

export class CopyDialog extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class LoadFromTemplateDialog extends PXView {
	Message: PXFieldState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class DocumentSettings extends PXView {
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SLAETA_Date: PXFieldState;
	SLAETA_Time: PXFieldState<PXFieldOptions.NoLabel>;
	AssignedEmpID: PXFieldState;
	ProblemID: PXFieldState;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeBegin_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeBegin_Time: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	ScheduledDateTimeEnd_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeEnd_Time: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	HandleManuallyScheduleTime: PXFieldState<PXFieldOptions.CommitChanges>;
}
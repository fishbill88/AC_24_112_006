import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	GridColumnShowHideMode,
	columnConfig,
	gridConfig
} from "client-controls";

export class Task extends PXView {
	TaskCD: PXFieldState;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
}

export class TaskProperties extends PXView {
	CompletedPctMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	ApproverID: PXFieldState;
	BillSeparately: PXFieldState;
	AllocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultBranchID: PXFieldState;
	RateTableID: PXFieldState;
	BillingOption: PXFieldState;
	WipAccountGroupID: PXFieldState;
	ProgressBillingBase: PXFieldState;
	TaxCategoryID: PXFieldState;
	DefaultSalesAccountID: PXFieldState;
	DefaultSalesSubID: PXFieldState;
	DefaultExpenseAccountID: PXFieldState;
	DefaultExpenseSubID: PXFieldState;
	DefaultAccrualAccountID: PXFieldState;
	DefaultAccrualSubID: PXFieldState;
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
	VisibleInTA: PXFieldState;
	VisibleInEA: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Budget extends PXView {
	PMAccountGroup__Type: PXFieldState;
	@columnConfig({hideViewLink: true})
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState;
	IsProduction: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
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
	wrapToolbar: true,
	adjustPageSize: false,
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


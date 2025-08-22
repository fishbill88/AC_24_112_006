import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig
} from "client-controls";

export class Allocations extends PXView {
	AllocationID: PXFieldState;
	Description: PXFieldState<PXFieldOptions.Multiline>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	suppressNoteFiles: true,
	adjustPageSize: false,
	fastFilterByAllFields: false,
	autoRepaint: ["StepRules", "StepSettings"]
})
export class Steps extends PXView {
	StepID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class StepRules extends PXView {
	Method: PXFieldState<PXFieldOptions.CommitChanges>;
	Post: PXFieldState<PXFieldOptions.CommitChanges>;
	SelectOption: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	RangeStart: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupTo: PXFieldState<PXFieldOptions.CommitChanges>;
	RangeEnd: PXFieldState<PXFieldOptions.CommitChanges>;
	RateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	NoRateOption: PXFieldState<PXFieldOptions.CommitChanges>;
	QtyFormula: PXFieldState<PXFieldOptions.CommitChanges>;
	BillableQtyFormula: PXFieldState<PXFieldOptions.CommitChanges>;
	AmountFormula: PXFieldState<PXFieldOptions.CommitChanges>;
	DescriptionFormula: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class StepSettings extends PXView {
	UpdateGL: PXFieldState<PXFieldOptions.CommitChanges>;
	DateSource: PXFieldState<PXFieldOptions.CommitChanges>;
	AllocateZeroQty: PXFieldState<PXFieldOptions.CommitChanges>;
	AllocateZeroAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	AllocateNonBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	MarkAsNotAllocated: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetBranchOrigin: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	Reverse: PXFieldState<PXFieldOptions.CommitChanges>;
	UseReversalDateFromOriginal: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectOrigin: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskOrigin: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupOrigin: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountOrigin: PXFieldState<PXFieldOptions.CommitChanges>;
	SubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskCD: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetProjectOrigin: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetTaskOrigin: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetAccountGroupOrigin: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetAccountOrigin: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetSubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetTaskCD: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetAccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	OffsetSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByDate: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByEmployee: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByVendor: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByItem: PXFieldState<PXFieldOptions.CommitChanges>;
}


import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	headerDescription,
	gridConfig
} from "client-controls";

export class Billing extends PXView {
	BillingID: PXFieldState;
	@headerDescription Description: PXFieldState;
	IsActive: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	suppressNoteFiles: true,
	adjustPageSize: false,
	fastFilterByAllFields: false,
	autoRepaint: ["BillingRule"]
})
export class BillingRules extends PXView {
	IsActive: PXFieldState;
	StepID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceGroup: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class BillingRule extends PXView {
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	RateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	NoRateOption: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceFormula: PXFieldState<PXFieldOptions.CommitChanges>;
	QtyFormula: PXFieldState<PXFieldOptions.CommitChanges>;
	AmountFormula: PXFieldState<PXFieldOptions.CommitChanges>;
	DescriptionFormula: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchSource: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchSourceBudget: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetBranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountSource: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	SubMaskBudget: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyNotes: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeZeroAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeZeroAmountAndQty: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeNonBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByDate: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByEmployee: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByVendor: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupByItem: PXFieldState<PXFieldOptions.CommitChanges>;
}


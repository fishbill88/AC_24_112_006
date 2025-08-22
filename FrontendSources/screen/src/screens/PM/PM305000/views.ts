import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	columnConfig,
	headerDescription,
	gridConfig
} from "client-controls";

export class Document extends PXView {
	ProjectID: PXFieldState;
	RevisionID: PXFieldState;
	Status: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Hidden>;
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	TotalAmountToComplete: PXFieldState<PXFieldOptions.Disabled>;
	TotalAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalBudgetedAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalProjectedAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalVarianceAmount: PXFieldState<PXFieldOptions.Disabled>;
	@headerDescription FormCaptionDescription: PXFieldState;
}

export class ProjectTotals extends PXView {
	TotalBudgetedRevenueAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalBudgetedCompletedAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalBudgetedAmountToComplete: PXFieldState<PXFieldOptions.Disabled>;
	TotalBudgetedCostAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalBudgetedGrossProfit: PXFieldState<PXFieldOptions.Disabled>;
	TotalBudgetedVarianceAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalProjectedGrossProfit: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true,
	allowInsert: false
})
export class Details extends PXView {
	AddCostBudget: PXActionState;
	ViewCostCommitments: PXActionState;
	ViewCostTransactions: PXActionState;
	Refresh: PXActionState;
	ShowHistory: PXActionState;

	AccountGroupID: PXFieldState;
	TaskID: PXFieldState;
	CostCodeID: PXFieldState;
	InventoryID: PXFieldState;
	Description: PXFieldState;
	UOM: PXFieldState;
	BudgetedQuantity: PXFieldState;
	BudgetedAmount: PXFieldState;
	ActualQuantity: PXFieldState;
	ActualAmount: PXFieldState;
	UnbilledQuantity: PXFieldState;
	UnbilledAmount: PXFieldState;
	CompletedQuantity: PXFieldState;
	CompletedAmount: PXFieldState;
	QuantityToComplete: PXFieldState;
	AmountToComplete: PXFieldState;
	Quantity: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectedQuantity: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	VarianceQuantity: PXFieldState<PXFieldOptions.CommitChanges>;
	VarianceAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletedPct: PXFieldState<PXFieldOptions.CommitChanges>;
	Mode: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
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
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class AvailableCostBudget extends PXView {
	@columnConfig({
		allowCheckAll: true
	})
	Selected: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
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
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class History extends PXView {
	PMCostProjection__RevisionID: PXFieldState;
	PMCostProjection__Date: PXFieldState;
	BudgetedQuantity: PXFieldState;
	BudgetedAmount: PXFieldState;
	ActualQuantity: PXFieldState;
	ActualAmount: PXFieldState;
	UnbilledQuantity: PXFieldState;
	UnbilledAmount: PXFieldState;
	CompletedQuantity: PXFieldState;
	CompletedAmount: PXFieldState;
	QuantityToComplete: PXFieldState;
	AmountToComplete: PXFieldState;
	Quantity: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectedQuantity: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletedPct: PXFieldState<PXFieldOptions.CommitChanges>;
	Mode: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CopyDialog extends PXView {
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefreshBudget: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyNotes: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyFiles: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

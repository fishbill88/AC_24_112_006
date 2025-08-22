import {
	columnConfig,
	gridConfig,
	linkCommand,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Document extends PXView {
	RefNbr: PXFieldState;
	Status: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	CreatedByID: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	LastModifiedByID: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	@linkCommand("ViewDailyFieldReport")
	DailyFieldReportCD: PXFieldState<PXFieldOptions.Disabled>;
}

export class Project extends PXView {
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Details extends PXView {
	LoadTemplate: PXActionState;
	SelectBudgetLines: PXActionState;

	OwnerID: PXFieldState;
	WorkgroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	UOM: PXFieldState;
	PreviouslyCompletedQuantity: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	PriorPeriodQuantity: PXFieldState;
	CurrentPeriodQuantity: PXFieldState;
	TotalCompletedQuantity: PXFieldState;
	CompletedPercentTotalQuantity: PXFieldState;
	TotalBudgetedQuantity: PXFieldState;
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
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

export class costBudgetfilter extends PXView {
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeTo: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class CostBudgets extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProjectTaskID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	RevisedQty: PXFieldState;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

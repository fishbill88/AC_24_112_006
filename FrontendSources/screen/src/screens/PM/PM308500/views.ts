import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnType,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	selectorSettings,
	ICurrencyInfo,
	headerDescription,
	gridConfig
} from "client-controls";

export class Document extends PXView {
	RefNbr: PXFieldState;
	Status: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Hidden>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	DelayDays: PXFieldState;
	ExtRefNbr: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState;
	ProjectIssueID: PXFieldState<PXFieldOptions.CommitChanges>;
	RFIID: PXFieldState<PXFieldOptions.CommitChanges>;
	ChangeOrderNbr: PXFieldState;
	CostChangeOrderNbr: PXFieldState;
	CostTotal: PXFieldState<PXFieldOptions.Disabled>;
	LineTotal: PXFieldState<PXFieldOptions.Disabled>;
	MarkupTotal: PXFieldState<PXFieldOptions.Disabled>;
	GrossMarginPct: PXFieldState<PXFieldOptions.Disabled>;
	PriceTotal: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	@headerDescription FormCaptionDescription: PXFieldState;
}

export class DocumentSettings extends PXView {
	Text: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Details extends PXView {
	CostTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostAccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtCost: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceMarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueAccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	LineMarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	LineAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsCommitment: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Markups extends PXView {
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState;
	MarkupAmount: PXFieldState;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
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

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

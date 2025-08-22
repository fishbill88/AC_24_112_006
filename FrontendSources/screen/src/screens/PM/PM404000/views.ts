import {
	columnConfig,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	DocType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	DateFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTo: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class BudgetControlLines extends PXView {
	DocType: PXFieldState;
	@linkCommand("EditDocument")
	RefNoteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState;
	ProjectDescription: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	TranDescription: PXFieldState;
	BudgetedAmount: PXFieldState;
	ConsumedAmount: PXFieldState;
	AvailableAmount: PXFieldState;
	DocumentAmount: PXFieldState;
	RemainingAmount: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
}

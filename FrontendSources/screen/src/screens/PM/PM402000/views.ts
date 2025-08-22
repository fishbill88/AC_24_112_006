import {
	columnConfig,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	ApproverID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class FilteredItems extends PXView {
	@linkCommand("ViewProject")
	ProjectID: PXFieldState;
	PMProject__Description: PXFieldState;
	@linkCommand("ViewTask")
	TaskCD: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	RateTableID: PXFieldState;
	Status: PXFieldState;
	CompletedPercent: PXFieldState;
	PlannedStartDate: PXFieldState;
	PlannedEndDate: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ApproverID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DefaultSalesSubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DefaultExpenseSubID: PXFieldState;
	VisibleInGL: PXFieldState;
	VisibleInAP: PXFieldState;
	VisibleInAR: PXFieldState;
	VisibleInSO: PXFieldState;
	VisibleInPO: PXFieldState;
	VisibleInTA: PXFieldState;
	VisibleInEA: PXFieldState;
	VisibleInIN: PXFieldState;
	PMTaskTotal__CuryAsset: PXFieldState;
	PMTaskTotal__CuryLiability: PXFieldState;
	PMTaskTotal__CuryIncome: PXFieldState;
	PMTaskTotal__CuryExpense: PXFieldState;
}

import {
	columnConfig,
	gridConfig,
	linkCommand,
	GridColumnShowHideMode,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class DailyFieldReports extends PXView {
	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False
	})
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewEntity")
	DailyFieldReportCd: PXFieldState;
	Status: PXFieldState;
	Date: PXFieldState;
	@linkCommand("ViewEntity")
	ProjectId: PXFieldState;
	@linkCommand("ViewEntity")
	@columnConfig({ width: 150 })
	ProjectManagerId: PXFieldState;
	@linkCommand("ViewEntity")
	CreatedById: PXFieldState;
}

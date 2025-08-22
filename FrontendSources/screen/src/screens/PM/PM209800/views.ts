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
	gridConfig
} from "client-controls";

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	autoRepaint: ["ProjectTaskSources", "LaborItemSources", "CostCodeRanges"]
})
export class Items extends PXView {
	IsActive: PXFieldState;
	WorkCodeID: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class ProjectTaskSources extends PXView {
	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ProjectTaskID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class LaborItemSources extends PXView {
	@columnConfig({ hideViewLink: true })
	LaborItemID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class CostCodeRanges extends PXView {
	@columnConfig({ hideViewLink: true })
	CostCodeFrom: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CostCodeTo: PXFieldState;
}

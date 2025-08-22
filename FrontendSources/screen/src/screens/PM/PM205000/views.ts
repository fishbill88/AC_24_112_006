import {
	columnConfig,
	gridConfig,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	RatetableID: PXFieldState<PXFieldOptions.CommitChanges>;
	RateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class RateDefinitions extends PXView {
	viewRate: PXActionState;
	@columnConfig({ width: 100 })
	Sequence: PXFieldState;
	@columnConfig({ width: 200 })
	Description: PXFieldState;
	@columnConfig({ width: 140 })
	Project: PXFieldState;
	@columnConfig({ width: 140 })
	Task: PXFieldState;
	@columnConfig({ width: 140 })
	AccountGroup: PXFieldState;
	@columnConfig({ width: 140 })
	RateItem: PXFieldState;
	@columnConfig({ width: 140 })
	Employee: PXFieldState;
}

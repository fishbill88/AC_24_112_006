import {
	columnConfig,
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Mapping extends PXView {
	IsActive: PXFieldState;
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 200 })
	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;
	ControlType: PXFieldState;
	Type: PXFieldState;
}

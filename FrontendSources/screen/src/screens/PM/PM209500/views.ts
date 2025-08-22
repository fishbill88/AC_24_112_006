import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig
} from "client-controls";

export class ChangeIDDialog extends PXView {
	CD: PXFieldState;
}

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class Items extends PXView {
	@columnConfig({ hideViewLink: true })
	CostCodeCD: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	IsDefault: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
}
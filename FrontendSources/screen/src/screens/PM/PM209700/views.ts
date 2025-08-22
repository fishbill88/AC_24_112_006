import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnType,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	gridConfig
} from "client-controls";

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class Items extends PXView {
	IsActive: PXFieldState;
	UnionID: PXFieldState;
	Description: PXFieldState;
}


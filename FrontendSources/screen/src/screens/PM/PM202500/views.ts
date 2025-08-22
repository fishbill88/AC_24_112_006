import {
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class ProjectGroups extends PXView {
	ProjectGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	IsActive: PXFieldState;
}

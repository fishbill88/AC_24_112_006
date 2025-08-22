import {
	columnConfig,
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class ProjectGroup extends PXView {
	ProjectGroupID: PXFieldState;
	Description: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.Multiline>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class Groups extends PXView {
	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState<PXFieldOptions.NoLabel>;
	GroupName: PXFieldState;
	Description: PXFieldState;
	Active: PXFieldState<PXFieldOptions.NoLabel>;
	GroupType: PXFieldState<PXFieldOptions.Disabled>;
}

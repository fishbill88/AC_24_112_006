import {
	columnConfig,
	gridConfig,
	PXFieldState,
	PXFieldOptions,
	PXView
} from "client-controls";

export class Group extends PXView {
	GroupName: PXFieldState;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	GroupType: PXFieldState;
	Active: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	allowInsert: true,
	allowDelete: false
})
export class Users extends PXView {
	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;
	Username: PXFieldState;
	FullName: PXFieldState;
	Comment: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class ProjectGroup extends PXView {
	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;
	ProjectGroupID: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	allowInsert: true,
	allowDelete: false
})
export class Project extends PXView {
	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;
	ContractCD: PXFieldState;
	Description: PXFieldState;
	CustomerID: PXFieldState;
	TemplateID: PXFieldState;
	Status: PXFieldState;
}

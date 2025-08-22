import {
	columnConfig,
	gridConfig,
	PXFieldOptions,
	PXFieldState,
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
	allowInsert: true,
	allowDelete: false
})
export class AccountGroup extends PXView {
	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;
	GroupCD: PXFieldState;
	Description: PXFieldState;
}

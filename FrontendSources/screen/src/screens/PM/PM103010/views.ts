import {
	columnConfig,
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class AccountGroup extends PXView {
	GroupCD: PXFieldState;
	Description: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.Multiline>;
	Type: PXFieldState<PXFieldOptions.Disabled>;
	IsActive: PXFieldState;
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

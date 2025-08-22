import {
	columnConfig,
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Project extends PXView {
	ContractCD: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.Multiline>;
	CustomerID: PXFieldState<PXFieldOptions.Disabled>;
	TemplateID: PXFieldState<PXFieldOptions.Disabled>;
	ProjectGroupID: PXFieldState<PXFieldOptions.Disabled>;
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

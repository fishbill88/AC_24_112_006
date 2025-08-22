import {
	columnConfig,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class PhotoLogSetup extends PXView {
	PhotoLogNumberingId: PXFieldState;
	PhotoNumberingId: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class PhotoLogStatuses extends PXView {
	Name: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Attributes extends PXView {
	IsActive: PXFieldState;
	@linkCommand("CRAttribute_ViewDetails")
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState;
	CSAttribute__IsInternal: PXFieldState;
	ControlType: PXFieldState;
	DefaultValue: PXFieldState;
}

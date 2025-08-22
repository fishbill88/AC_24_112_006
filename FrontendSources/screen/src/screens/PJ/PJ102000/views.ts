import {
	gridConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class DrawingLogSetup extends PXView {
	DrawingLogNumberingSequenceId: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class DrawingLogDisciplines extends PXView {
	IsActive: PXFieldState;
	Name: PXFieldState;
	SortOrder: PXFieldState<PXFieldOptions.Hidden>;
	LineNbr: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true
})
export class DrawingLogStatuses extends PXView {
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

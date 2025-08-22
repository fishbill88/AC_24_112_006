import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	GridPreset,
	gridConfig
} from "client-controls";

export class Filter extends PXView {
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskId: PXFieldState<PXFieldOptions.CommitChanges>;
	DateFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTo: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	preset: GridPreset.Inquiry,
	autoRepaint: ["MainPhoto"]
})
export class PhotoLogs extends PXView {
	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False
	})
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("editPhotoLog")
	PhotoLogCd: PXFieldState;
	StatusId: PXFieldState;
	Date: PXFieldState;
	@linkCommand("ViewEntity")
	ProjectId: PXFieldState;
	@linkCommand("ViewEntity")
	ProjectTaskId: PXFieldState;
	Description: PXFieldState;
	@linkCommand("ViewEntity")
	CreatedById: PXFieldState;
}

export class MainPhoto extends PXView {
	ImageUrl: PXFieldState;
}


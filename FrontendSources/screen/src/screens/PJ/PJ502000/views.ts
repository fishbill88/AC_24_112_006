import {
	columnConfig,
	gridConfig,
	linkCommand,
	GridColumnShowHideMode,
	PXFieldState,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class ProjectIssues extends PXView {
	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False,
		width: 35,
	})
	Selected: PXFieldState;
	@linkCommand("ProjectIssues_ViewDetails")
	ProjectIssueCd: PXFieldState;
	Summary: PXFieldState;
	Status: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		width: 100,
	})
	PriorityId: PXFieldState;
	DueDate: PXFieldState;
	ClassId: PXFieldState;
	@columnConfig({
		width: 150,
	})
	OwnerID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState;
	CreationDate: PXFieldState;
}

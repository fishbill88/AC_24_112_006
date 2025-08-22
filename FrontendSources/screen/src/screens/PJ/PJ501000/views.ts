import {
	columnConfig,
	gridConfig,
	linkCommand,
	GridColumnDisplayMode,
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
export class RequestsForInformation extends PXView {
	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False
	})
	Selected: PXFieldState;
	@linkCommand("RequestsForInformation_ViewDetails")
	RequestForInformationCd: PXFieldState;
	Summary: PXFieldState;
	Status: PXFieldState;
	Reason: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PriorityId: PXFieldState;
	DueResponseDate: PXFieldState;
	ClassId: PXFieldState;
	@linkCommand("RequestsForInformation_EntityDetails")
	BusinessAccountId: PXFieldState;
	@linkCommand("RequestsForInformation_ContactDetails")
	@columnConfig({ displayMode: GridColumnDisplayMode.Text })
	ContactId: PXFieldState;
	OwnerId: PXFieldState;
	CreationDate: PXFieldState;
}

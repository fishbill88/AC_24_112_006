import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	headerDescription,
	gridConfig
} from "client-controls";

export class PhotoLog extends PXView {
	PhotoLogCd: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState;
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskId: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	StatusId: PXFieldState<PXFieldOptions.CommitChanges>;
	CreatedById: PXFieldState;
	PhotoLogId: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.Hidden>;
	@headerDescription FormCaptionDescription: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	autoRepaint: ["PhotoImage"]
})
export class Photos extends PXView {
	UploadFromAttachments: PXActionState;

	@linkCommand("viewPhoto")
	PhotoCd: PXFieldState<PXFieldOptions.CommitChanges>;
	Name: PXFieldState;
	Description: PXFieldState;
	UploadedDate: PXFieldState;
	@linkCommand("ViewEntity")
	UploadedById: PXFieldState;
	IsMainPhoto: PXFieldState;
	PhotoLogId: PXFieldState<PXFieldOptions.Hidden>;
	ImageUrl: PXFieldState<PXFieldOptions.Hidden>;
}

export class PhotoImage extends PXView {
	ImageUrl: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Activities extends PXView {
	NewTask: PXActionState;
	NewEvent: PXActionState;
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;

	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	IsCompleteIcon: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	PriorityIcon: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	CRReminder__ReminderIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	RefNoteID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ViewActivity")
	Subject: PXFieldState;
	UIStatus: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState;
	CategoryID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;
	@columnConfig({
		allowUpdate: false
	})
	CreatedByID_Creator_Username: PXFieldState;
	IsBillable: PXFieldState<PXFieldOptions.Hidden>;
	TimeSpent: PXFieldState<PXFieldOptions.Hidden>;
	OvertimeSpent: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowUpdate: false
	})
	TimeBillable: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowUpdate: false
	})
	OvertimeBillable: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.True
	})
	ProjectID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.True
	})
	ProjectTaskID: PXFieldState<PXFieldOptions.Hidden>;
	Released: PXFieldState<PXFieldOptions.Hidden>;
	body: PXFieldState;
}


import {
	columnConfig,
	gridConfig,
	linkCommand,
	GridColumnShowHideMode,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class ProjectIssue extends PXView {
	Summary: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	ProjectIssueCd: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskId: PXFieldState<PXFieldOptions.CommitChanges>;
	ClassId: PXFieldState<PXFieldOptions.CommitChanges>;
	RelatedEntityDescription: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ProjectIssue$ConvertedTo$Link")
	ConvertedTo: PXFieldState<PXFieldOptions.Disabled>;
	DueDate: PXFieldState<PXFieldOptions.CommitChanges>;
	PriorityId: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	CreationDate_Date: PXFieldState;
	CreationDate_Time: PXFieldState;
	CreatedById: PXFieldState;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	ResolvedOn: PXFieldState;
	ProjectIssueTypeId: PXFieldState;
	IsScheduleImpact: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleImpact: PXFieldState;
	IsCostImpact: PXFieldState<PXFieldOptions.CommitChanges>;
	CostImpact: PXFieldState;
}

export class CurrentProjectIssue extends PXView {
	Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Activities extends PXView {
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	IsCompleteIcon: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	PriorityIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	RefNoteID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ViewActivity")
	Subject: PXFieldState;
	CostCodeID: PXFieldState;
	UIStatus: PXFieldState;
	Released: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	CategoryID: PXFieldState;
	IsBillable: PXFieldState;
	TimeSpent: PXFieldState;
	OvertimeSpent: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TimeBillable: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OvertimeBillable: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;
	NoteID: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false
})
export class Attributes extends PXView {
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	@columnConfig({
		allowSort: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Value: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	suppressNoteFiles: true,
	adjustPageSize: false,
	allowInsert: false
})
export class LinkedDrawingLogs extends PXView {
	LinkDrawing: PXActionState;
	UnlinkDrawing: PXActionState;
	ViewAttachments: PXActionState;

	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False
	})
	Selected: PXFieldState;
	@linkCommand("ViewEntity")
	DrawingLogCd: PXFieldState;
	@linkCommand("ViewEntity")
	ProjectId: PXFieldState;
	@linkCommand("ViewEntity")
	ProjectTaskId: PXFieldState;
	@columnConfig({hideViewLink: true})
	DisciplineId: PXFieldState;
	// eslint-disable-next-line id-denylist
	Number: PXFieldState;
	Revision: PXFieldState;
	Sketch: PXFieldState;
	Title: PXFieldState;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	StatusId: PXFieldState;
	DrawingDate: PXFieldState;
	ReceivedDate: PXFieldState;
	@linkCommand("DrawingLog$OriginalDrawingId$Link")
	OriginalDrawingId: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	OwnerId: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	suppressNoteFiles: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false,
	allowUpdate: false
})
export class DrawingLogs extends PXView {
	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False
	})
	Selected: PXFieldState;
	@linkCommand("ViewEntity")
	DrawingLogCd: PXFieldState;
	@linkCommand("ViewEntity")
	ProjectId: PXFieldState;
	@linkCommand("ViewEntity")
	ProjectTaskId: PXFieldState;
	@columnConfig({hideViewLink: true})
	DisciplineId: PXFieldState;
	// eslint-disable-next-line id-denylist
	Number: PXFieldState;
	Revision: PXFieldState;
	Sketch: PXFieldState;
	Title: PXFieldState;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	StatusId: PXFieldState;
	DrawingDate: PXFieldState;
	ReceivedDate: PXFieldState;
	@linkCommand("DrawingLog$OriginalDrawingId$Link")
	OriginalDrawingId: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true})
	OwnerId: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class DrawingLogsAttachments extends PXView {
	@linkCommand("ViewAttachment")
	FileName: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewEntity")
	DrawingLog__DrawingLogCd: PXFieldState;
	UploadFileRevision__Comment: PXFieldState;
}

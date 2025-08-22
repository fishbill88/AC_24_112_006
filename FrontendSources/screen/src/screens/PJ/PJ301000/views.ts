import {
	columnConfig,
	gridConfig,
	linkCommand,
	GridColumnShowHideMode,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView,
} from "client-controls";

export class RequestForInformation extends PXView {
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskId: PXFieldState<PXFieldOptions.CommitChanges>;
	BusinessAccountId: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactId: PXFieldState<PXFieldOptions.CommitChanges>;
	ClassId: PXFieldState<PXFieldOptions.CommitChanges>;
	Summary: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	Incoming: PXFieldState<PXFieldOptions.CommitChanges>;
	IncomingRequestForInformationId: PXFieldState<PXFieldOptions.Disabled>;
	Status: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Disabled>;
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;
	PriorityId: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentationLink: PXFieldState;
	SpecSection: PXFieldState;
	RequestForInformationCd: PXFieldState<PXFieldOptions.CommitChanges>;
	CreationDate: PXFieldState;
	CreatedById: PXFieldState;
	OwnerId: PXFieldState;
	DueResponseDate: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsScheduleImpact: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleImpact: PXFieldState;
	IsCostImpact: PXFieldState<PXFieldOptions.CommitChanges>;
	CostImpact: PXFieldState;
	DesignChange: PXFieldState;
	@linkCommand("RequestForInformation$ConvertedFrom$Link")
	ConvertedFrom: PXFieldState<PXFieldOptions.Disabled>;
	@linkCommand("RequestForInformation$ConvertedTo$Link")
	ConvertedTo: PXFieldState<PXFieldOptions.Disabled>;
}

export class CurrentRequestForInformation extends PXView {
	RequestDetails: PXFieldState;
	LastModifiedRequestAnswer: PXFieldState;
	RequestAnswer: PXFieldState<PXFieldOptions.CommitChanges>;
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
	adjustPageSize: false
})
export class Activities extends PXView {
	NewTask: PXActionState;
	NewEvent: PXActionState;
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;

	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False,
		width: 50
	})
	IsCompleteIcon: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False,
		width: 50
	})
	PriorityIcon: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False,
		width: 50
	})
	CRReminder__ReminderIcon: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	ClassIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	RefNoteID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ViewActivity")
	Subject: PXFieldState;
	IsFinalAnswer: PXFieldState;
	UIStatus: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowUpdate: false })
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CategoryID: PXFieldState;
	WorkgroupID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;
	body: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Relations extends PXView {
	Role: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPrimary: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("Relations_TargetDetails")
	@columnConfig({ width: 200 })
	DocumentNoteId: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("Relations_EntityDetails")
	BusinessAccountId: PXFieldState<PXFieldOptions.CommitChanges>;
	BusinessAccountName: PXFieldState;
	@linkCommand("Relations_ContactDetails")
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactEmail: PXFieldState;
	AddToCC: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false
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
	@columnConfig({ hideViewLink: true })
	DisciplineId: PXFieldState;
	// eslint-disable-next-line id-denylist
	Number: PXFieldState;
	Revision: PXFieldState;
	Sketch: PXFieldState;
	Title: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	StatusId: PXFieldState;
	DrawingDate: PXFieldState;
	ReceivedDate: PXFieldState;
	@linkCommand("DrawingLog$OriginalDrawingId$Link")
	OriginalDrawingId: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	OwnerId: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	suppressNoteFiles: true,
	adjustPageSize: false
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
	@columnConfig({ hideViewLink: true })
	DisciplineId: PXFieldState;
	// eslint-disable-next-line id-denylist
	Number: PXFieldState;
	Revision: PXFieldState;
	Sketch: PXFieldState;
	Title: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	StatusId: PXFieldState;
	DrawingDate: PXFieldState;
	ReceivedDate: PXFieldState;
	@linkCommand("DrawingLog$OriginalDrawingId$Link")
	OriginalDrawingId: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true, width: 200 })
	OwnerId: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class DrawingLogsAttachments extends PXView {
	@linkCommand("ViewAttachment")
	@columnConfig({ width: 300 })
	FileName: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewEntity")
	DrawingLog__DrawingLogCd: PXFieldState;
	UploadFileRevision__Comment: PXFieldState;
}

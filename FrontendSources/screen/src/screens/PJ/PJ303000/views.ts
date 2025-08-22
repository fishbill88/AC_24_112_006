import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	gridConfig
} from "client-controls";

export class DrawingLog extends PXView {
	DrawingLogCd: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskId: PXFieldState<PXFieldOptions.CommitChanges>;
	DisciplineId: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerId: PXFieldState<PXFieldOptions.CommitChanges>;
	// eslint-disable-next-line id-denylist
	Number: PXFieldState<PXFieldOptions.CommitChanges>;
	Revision: PXFieldState<PXFieldOptions.CommitChanges>;
	DrawingDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceivedDate: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusId: PXFieldState<PXFieldOptions.CommitChanges>;
	IsCurrent: PXFieldState<PXFieldOptions.CommitChanges>;
	Title: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	Description: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Multiline>;
	Sketch: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("DrawingLog$OriginalDrawingId$Link")
	OriginalDrawingId: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false
})
export class Drawings extends PXView {
	@linkCommand("ViewAttachment")
	@columnConfig({width: 400})
	UploadFile__FileName: PXFieldState;
	@columnConfig({width: 400})
	Comment: PXFieldState;
	IsDrawingLogCurrentFile: PXFieldState;
	CreatedByID_Creator_Username: PXFieldState;
	CreatedDateTime: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false
})
export class Attributes extends PXView {
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
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
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	ClassIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	RefNoteID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ViewActivity")
	Subject: PXFieldState;
	UIStatus: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowUpdate: false
	})
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CategoryID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;
	body: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	suppressNoteFiles: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Revisions extends PXView {
	@linkCommand("ViewEntity")
	DrawingLogCd: PXFieldState;
	@columnConfig({hideViewLink: true})
	ProjectId: PXFieldState;
	@columnConfig({hideViewLink: true})
	ProjectTaskId: PXFieldState;
	@columnConfig({hideViewLink: true})
	DisciplineId: PXFieldState;
	@columnConfig({hideViewLink: true})
	OwnerId: PXFieldState;
	Title: PXFieldState;
	Description: PXFieldState;
	Revision: PXFieldState;
	Sketch: PXFieldState;
	@linkCommand("DrawingLog$OriginalDrawingId$Link")
	OriginalDrawingId: PXFieldState;
	// eslint-disable-next-line id-denylist
	Number: PXFieldState;
	@columnConfig({hideViewLink: true})
	StatusId: PXFieldState;
	DrawingDate: PXFieldState;
	ReceivedDate: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class LinkedDrawingLogRelations extends PXView {
	Link: PXActionState;
	UnLink: PXActionState;

	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False
	})
	Selected: PXFieldState;
	@linkCommand("LinkedDrawingLogRelation$DocumentId$Link")
	DocumentCd: PXFieldState;
	DocumentType: PXFieldState;
	@linkCommand("ViewEntity")
	ProjectId: PXFieldState;
	@linkCommand("ViewEntity")
	ProjectTaskId: PXFieldState;
	Status: PXFieldState;
	@columnConfig({hideViewLink: true, width: 100})
	PriorityId: PXFieldState;
	@columnConfig({ width: 400})
	Summary: PXFieldState;
	@columnConfig({hideViewLink: true})
	CreatedById: PXFieldState;
	@columnConfig({hideViewLink: true, width: 120})
	OwnerId: PXFieldState;
	DueDate: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class UnlinkedDrawingLogRelations extends PXView {
	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False
	})
	Selected: PXFieldState;
	DocumentCd: PXFieldState;
	DocumentType: PXFieldState;
	ProjectId: PXFieldState;
	ProjectTaskId: PXFieldState;
	Status: PXFieldState;
	PriorityId: PXFieldState;
	Summary: PXFieldState;
	CreatedById: PXFieldState;
	OwnerId: PXFieldState;
	DueDate: PXFieldState;
}


import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnShowHideMode,
	GridColumnDisplayMode,
	linkCommand,
	columnConfig,
	headerDescription,
	gridConfig
} from "client-controls";

export class Submittals extends PXView {
	SubmittalID: PXFieldState;
	RevisionID: PXFieldState;
	Status: PXFieldState;
	Reason: PXFieldState;
	TypeID: PXFieldState;
	Summary: PXFieldState<PXFieldOptions.Multiline>;
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskId: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	SpecificationInfo: PXFieldState;
	SpecificationSection: PXFieldState;
	DateCreated: PXFieldState<PXFieldOptions.CommitChanges>;
	DueDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DateOnSite: PXFieldState;
	DateClosed: PXFieldState;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CurrentWorkflowItemContactID: PXFieldState;
	DaysOverdue: PXFieldState;
	@headerDescription FormCaptionDescription: PXFieldState;
}

export class CurrentSubmittal extends PXView {
	Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	initNewRow: true,
	allowDelete: false,
	actionsConfig: {
		DeleteWorkflowItem: {
			images: {
				normal: "main@RecordDel"
			}
		}
	}
})
export class SubmittalWorkflowItems extends PXView {
	DeleteWorkflowItem: PXActionState;

	EmailTo: PXFieldState;
	@columnConfig({displayMode: GridColumnDisplayMode.Text, width: 120})
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	Contact__FullName: PXFieldState;
	Contact__Salutation: PXFieldState;
	Role: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DaysForReview: PXFieldState<PXFieldOptions.CommitChanges>;
	DueDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletionDate: PXFieldState;
	DateReceived: PXFieldState;
	DateSent: PXFieldState;
	Contact__EMail: PXFieldState;
	Contact__Phone1: PXFieldState;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.False
	})
	CanDelete: PXFieldState<PXFieldOptions.Hidden>;
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
	IsFinalAnswer: PXFieldState;
	UIStatus: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({
		allowUpdate: false
	})
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	CategoryID: PXFieldState;
	WorkgroupID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;
	body: PXFieldState;
}


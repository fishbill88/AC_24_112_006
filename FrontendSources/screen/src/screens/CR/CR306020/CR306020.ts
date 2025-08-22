import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	columnConfig,
	linkCommand,
	GridColumnShowHideMode,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.CRTaskMaint",
	primaryView: "Tasks",
	udfTypeField: "Type",
	showUDFIndicator: true,
})
export class CR306020 extends PXScreen {
	Tasks = createSingle(CRActivity);
	Activities = createCollection(CRActivity2);
	ReferencedTasks = createCollection(CRActivity);
	Reminder = createSingle(CRReminder);
	TimeActivity = createSingle(PMTimeActivity);
}

export class CRActivity extends PXView {
	Subject: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	PercentCompletion: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPrivate: PXFieldState;
	ProvidesCaseSolution: PXFieldState;
	RefNoteIDType: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentNoteID: PXFieldState<PXFieldOptions.Disabled>;
	UIStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	Priority: PXFieldState;
	CategoryID: PXFieldState;
	CompletedDate: PXFieldState<PXFieldOptions.Disabled>;
	Body: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({ 
	preset: GridPreset.Details, 
	allowInsert: false
})
export class CRActivity2 extends PXView {
	NewTask: PXActionState;
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;

	NoteID: PXFieldState<PXFieldOptions.Hidden>;
	TimeSpent: PXFieldState;
	TimeBillable: PXFieldState;
	OvertimeSpent: PXFieldState;
	OvertimeBillable: PXFieldState;
	CostCodeID: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	IsCompleteIcon: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	PriorityIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	RefNoteID: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("ViewActivity")
	Subject: PXFieldState;
	UIStatus: PXFieldState;
	Released: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	CategoryID: PXFieldState;
	IsBillable: PXFieldState;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState;
	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;
}

export class CRReminder extends PXView {
	IsReminderOn: PXFieldState<PXFieldOptions.CommitChanges>;
	ReminderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ReminderDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ReminderDate_Time: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class PMTimeActivity extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeSpent: PXFieldState;
	OvertimeSpent: PXFieldState;
	TimeBillable: PXFieldState;
	OvertimeBillable: PXFieldState;
	ServiceID: PXFieldState;
}

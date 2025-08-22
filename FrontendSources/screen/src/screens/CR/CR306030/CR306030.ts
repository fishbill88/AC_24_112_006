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
	linkCommand,
	GridPreset
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.EP.EPEventMaint",
	primaryView: "Events",
	udfTypeField: "Type",
	showUDFIndicator: true
})
export class CR306030 extends PXScreen {
	Events = createSingle(CRActivity);
	Attendees = createCollection(EPAttendee);
	Activities = createCollection(CRActivity2);
	Reminder = createSingle(CRReminder);
	TimeActivity = createSingle(PMTimeActivity);
}

export class CRActivity extends PXView {
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	NoteID: PXFieldState<PXFieldOptions.Hidden>;
	Subject: PXFieldState;
	Location: PXFieldState;
	TimeZone: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	AllDay: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAsID: PXFieldState;
	IsPrivate: PXFieldState;
	ProvidesCaseSolution: PXFieldState;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNoteIDType: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	UIStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	Priority: PXFieldState;
	CategoryID: PXFieldState;
	Body: PXFieldState;
}

@gridConfig({ 
	preset: GridPreset.Details,
	autoAdjustColumns: true
})
export class EPAttendee extends PXView {
	SendPersonalInvitation: PXActionState;
	SendInvitations: PXActionState;

	@linkCommand("Attendees_Contact__contactID_ViewDetails")
	ContactID: PXFieldState;
	Email: PXFieldState;
	Comment: PXFieldState;
	IsOptional: PXFieldState;
	Invitation: PXFieldState;
}

@gridConfig({ 
	preset: GridPreset.Details,
	allowInsert: false
})
export class CRActivity2 extends PXView {
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;

	@linkCommand("ViewActivity")
	Subject: PXFieldState;
	UIStatus: PXFieldState;
	StartDate: PXFieldState;
	TimeSpent: PXFieldState;
	TimeBillable: PXFieldState;
	OvertimeBillable: PXFieldState;
	CostCodeID: PXFieldState;
	ClassInfo: PXFieldState;
	IsBillable: PXFieldState;
	OvertimeSpent: PXFieldState;
}

export class CRReminder extends PXView {
	IsReminderOn: PXFieldState<PXFieldOptions.CommitChanges>;
	RemindAt: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class PMTimeActivity extends PXView {
	TimeSpent: PXFieldState;
	OvertimeSpent: PXFieldState;
	TimeBillable: PXFieldState;
	OvertimeBillable: PXFieldState;
}

import {
	PXView,
	PXFieldState,
	graphInfo,
	PXScreen,
	createSingle,
	PXFieldOptions,
	viewInfo,
	gridConfig,
	selectorSettings,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.CREmailActivityMaint",
	primaryView: "Message",
})
export class CR306015 extends PXScreen {

	Message = createSingle(CRSMEmail);
	@viewInfo({ containerName: 'TimeActivity' })
	TimeActivity = createSingle(PMTimeActivity);
}

// Views

export class CRSMEmail extends PXView {
	NoteID: PXFieldState<PXFieldOptions.Disabled>;
	@selectorSettings('EmailAccountID', '')
	MailAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	MailFrom: PXFieldState;
	MailTo: PXFieldState;
	BAccount__AcctCD: PXFieldState;
	MailCc: PXFieldState;
	MailBcc: PXFieldState;
	Subject: PXFieldState;
	RecipientNotes: PXFieldState<PXFieldOptions.CommitChanges>;
	EntityDescription: PXFieldState;
	Body: PXFieldState;
	StartDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	IsIncome: PXFieldState<PXFieldOptions.Disabled>;
	IsPrivate: PXFieldState;
	ProvidesCaseSolution: PXFieldState;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ResponseToNoteID: PXFieldState<PXFieldOptions.Disabled>;
	RefNoteIDType: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentNoteID: PXFieldState<PXFieldOptions.Disabled>;
	MPStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	SendAt: PXFieldState;
	Categories: PXFieldState;
	BypassSuppressionChecks: PXFieldState;
	TrackEmailOpens: PXFieldState;
	TrackClicksInEmail: PXFieldState;
}

@gridConfig({ allowInsert: false })
export class SMSendGridRecipient extends PXView {
	Address: PXFieldState;
	Name: PXFieldState;
	Status: PXFieldState;
	OpenedCount: PXFieldState;
	ClickedCount: PXFieldState;
	ReportedAsSpamCount: PXFieldState;
	OptedOutCount: PXFieldState;
	MailServiceReply: PXFieldState;
}

export class PMTimeActivity extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertifiedJob: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState;
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnionID: PXFieldState;
	TrackTime: PXFieldState<PXFieldOptions.CommitChanges>;
	ApprovalStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	ApproverID: PXFieldState<PXFieldOptions.Disabled>;
	EarningTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkCodeID: PXFieldState;
	ShiftID: PXFieldState;
	TimeSpent: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeSpent: PXFieldState<PXFieldOptions.Disabled>;
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	Released: PXFieldState;
	TimeBillable: PXFieldState;
	OvertimeBillable: PXFieldState<PXFieldOptions.Disabled>;
	NoteID: PXFieldState<PXFieldOptions.Hidden>;
}

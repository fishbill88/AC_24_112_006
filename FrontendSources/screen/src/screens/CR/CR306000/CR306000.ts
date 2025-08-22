import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	PXView,
	PXFieldOptions,
	gridConfig,
	PXFieldState,
	columnConfig,
	linkCommand,
	GridColumnShowHideMode,
	disabled,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.CR.CRCaseMaint",
	primaryView: "Case",
	udfTypeField: "CaseClassID",
	showUDFIndicator: true,
})
export class CR306000 extends PXScreen {
	CreateServiceOrderFilter = createSingle(FSCreateServiceOrderOnCaseFilter);
	Case = createSingle(CRCase);
	CaseActivityStatistics = createSingle(CaseActivityStatistics);
	Answers = createCollection(CSAnswers);
	Activities = createCollection(CRActivity);
	CaseRefs = createCollection(CRCaseReference);
	Relations = createCollection(CRRelation);
	SyncRecs = createCollection(SFSyncRecord);
	OwnerUser = createSingle(Users);
}

export class FSCreateServiceOrderOnCaseFilter extends PXView {
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	AssignedEmpID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProblemID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CRCase extends PXView {
	CaseCD: PXFieldState;
	CaseClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	Subject: PXFieldState<PXFieldOptions.Multiline>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Resolution: PXFieldState<PXFieldOptions.CommitChanges>;
	ReportedOnDateTime: PXFieldState;
	Severity: PXFieldState<PXFieldOptions.CommitChanges>;
	Priority: PXFieldState;
	ResolutionDate: PXFieldState;
	HeaderInitialResponseDueDateTime: PXFieldState;
	HeaderResponseDueDateTime: PXFieldState;
	HeaderResolutionDueDateTime: PXFieldState;
	Description: PXFieldState;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContractID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualBillableTimes: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeBillable: PXFieldState;
	OvertimeBillable: PXFieldState;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	InitResponse: PXFieldState;
	TimeSpent: PXFieldState;
	OvertimeSpent: PXFieldState;
	TimeResolution: PXFieldState;
	IsActive: PXFieldState;
	InitialResponseDueDateTime: PXFieldState;
	ResponseDueDateTime: PXFieldState;
	ResolutionDueDateTime: PXFieldState;
	SolutionActivityNoteID: PXFieldState;
	LastActivity: PXFieldState<PXFieldOptions.Disabled>;
	ClosureNotes: PXFieldState;
}

export class CaseActivityStatistics extends PXView {
	LastIncomingActivityDate: PXFieldState;
	LastOutgoingActivityDate: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class CSAnswers extends PXView {
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	Value: PXFieldState;
}

@gridConfig({ allowUpdate: false })
export class CRActivity extends PXView {
	NewTask: PXActionState;
	NewEvent: PXActionState;
	NewMailActivity: PXActionState;
	NewActivity: PXActionState;
	TogglePinActivity: PXActionState;

	IsPinned: PXFieldState;
	IsCompleteIcon: PXFieldState;
	PriorityIcon: PXFieldState;
	CRReminder__ReminderIcon: PXFieldState;
	ClassIcon: PXFieldState;
	ClassInfo: PXFieldState;
	@columnConfig({ allowUpdate: false })
	@linkCommand("ViewActivity")
	Subject: PXFieldState;
	UIStatus: PXFieldState;
	Released: PXFieldState;
	StartDate: PXFieldState;
	CreatedDateTime: PXFieldState;
	TimeSpent: PXFieldState;
	OvertimeSpent: PXFieldState;
	@columnConfig({ allowUpdate: false })
	IsBillable: PXFieldState;
	TimeBillable: PXFieldState;
	OvertimeBillable: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID_Creator_Username: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	@linkCommand("OpenActivityOwner")
	OwnerID: PXFieldState;
	Source: PXFieldState<PXFieldOptions.Hidden>;
	BAccountID: PXFieldState<PXFieldOptions.Hidden>;
	ContactID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectID: PXFieldState<PXFieldOptions.Hidden>;
	ProjectTaskID: PXFieldState<PXFieldOptions.Hidden>;
	Body: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({ 
	preset: GridPreset.Details,
	syncPosition: true,
	adjustPageSize: true
})
export class CRCaseReference extends PXView {
	@linkCommand("CaseRefs_CRCase_ViewDetails")
	ChildCaseCD: PXFieldState;
	RelationType: PXFieldState;
	CRCaseRelated__Subject: PXFieldState;
	CRCaseRelated__Status: PXFieldState;
	CRCaseRelated__OwnerID: PXFieldState;
	CRCaseRelated__WorkgroupID: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class CRRelation extends PXView {
	Role: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPrimary: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetType: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("RelationsViewTargetDetails")
	TargetNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	OwnerID: PXFieldState;
	@linkCommand("RelationsViewEntityDetails")
	EntityID: PXFieldState<PXFieldOptions.CommitChanges>;
	Name: PXFieldState;
	@linkCommand("RelationsViewContactDetails") ContactID: PXFieldState;
	Email: PXFieldState;
	AddToCC: PXFieldState;

	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	initNewRow: true,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class SFSyncRecord extends PXView {
	SyncSalesforce: PXActionState;

	SYProvider__Name: PXFieldState;
	@linkCommand("GoToSalesforce")
	RemoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	Operation: PXFieldState;
	SFEntitySetup__ImportScenario: PXFieldState;
	SFEntitySetup__ExportScenario: PXFieldState;
	LastErrorMessage: PXFieldState;
	LastAttemptTS: PXFieldState;
	AttemptCount: PXFieldState;
}

export class Users extends PXView {
	PKID: PXFieldState;
}

import {
	createSingle,
	PXScreen,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.EP.CRActivityMaint",
	primaryView: "Activities",
	udfTypeField: "Type",
	showUDFIndicator: true,
})
export class CR306010 extends PXScreen {
	Activities = createSingle(CRActivity);
	TimeActivity = createSingle(PMTimeActivity);
}

export class CRActivity extends PXView  {
	Subject: PXFieldState<PXFieldOptions.Multiline>;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPrivate: PXFieldState;
	ProvidesCaseSolution: PXFieldState;
	StartDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNoteIDType: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	NoteID: PXFieldState;
	Body: PXFieldState;
}

export class PMTimeActivity extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertifiedJob: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
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
	TimeBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	ARRefNbr: PXFieldState<PXFieldOptions.Disabled>;
}

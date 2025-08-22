import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState, TextAlign, GridPreset } from "client-controls";

export class EPWeeklyCrewTimeActivity extends PXView {
	WorkgroupID: PXFieldState;
	Week: PXFieldState;
}

export class EPWeeklyCrewTimeActivityFilter extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	Day: PXFieldState<PXFieldOptions.CommitChanges>;
	RegularTime: PXFieldState;
	BillableTime: PXFieldState;
	Overtime: PXFieldState;
	BillableOvertime: PXFieldState;
	TotalTime: PXFieldState;
	TotalBillableTime: PXFieldState;
	TotalWorkgroupMembers: PXFieldState;
	TotalWorkgroupMembersWithActivities: PXFieldState;
	ShowAllMembers: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class PMTimeActivity extends PXView {
	EnterBulkTime: PXActionState;
	CopySelectedActivity: PXActionState;
	LoadLastWeekActivities: PXActionState;

	@columnConfig({ type: GridColumnType.CheckBox })
	Hold: PXFieldState;

	@columnConfig({ width: 200 })
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;

	OwnerID_EPEmployee_AcctName: PXFieldState;
	ApprovalStatus: PXFieldState;

	@columnConfig({ width: 175, hideViewLink: true })
	Date: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	Date_Time: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	EarningTypeID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	ParentTaskNoteID: PXFieldState;

	ContractID: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ type: GridColumnType.CheckBox })
	CertifiedJob: PXFieldState;

	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	UnionID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	WorkCodeID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ShiftID: PXFieldState<PXFieldOptions.CommitChanges>;

	AppointmentID: PXFieldState;
	AppointmentCustomerID: PXFieldState;
	LogLineNbr: PXFieldState;
	ServiceID: PXFieldState;
	TimeSpent: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ type: GridColumnType.CheckBox })
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;

	TimeBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	Summary: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class EPTimeActivitiesSummary extends PXView {
	LoadLastWeekMembers: PXActionState;

	@columnConfig({ hideViewLink: true })
	ContactID: PXFieldState;

	ContactID_EPEmployee_AcctName: PXFieldState;
	Status: PXFieldState;
	MondayTime: PXFieldState;
	TuesdayTime: PXFieldState;
	WednesdayTime: PXFieldState;
	ThursdayTime: PXFieldState;
	FridayTime: PXFieldState;
	SaturdayTime: PXFieldState;
	SundayTime: PXFieldState;
	TotalRegularTime: PXFieldState;
	TotalBillableTime: PXFieldState;
	TotalOvertime: PXFieldState;
	TotalBillableOvertime: PXFieldState;

	@columnConfig({ visible: false, allowShowHide: GridColumnShowHideMode.False })
	IsWithoutActivities: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details
})
export class EPTimeActivitiesSummary2 extends PXView {
	LoadLastWeekMembers: PXActionState;

	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox, allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ContactID: PXFieldState;

	Status: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class PMTimeActivity2 extends PXView {
	@columnConfig({ hideViewLink: true })
	Date: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	Date_Time: PXFieldState;

	@columnConfig({ hideViewLink: true })
	EarningTypeID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ParentTaskNoteID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ContractID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ type: GridColumnType.CheckBox })
	CertifiedJob: PXFieldState;

	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	UnionID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	WorkCodeID: PXFieldState;

	AppointmentID: PXFieldState;
	AppointmentCustomerID: PXFieldState;
	LogLineNbr: PXFieldState;
	ServiceID: PXFieldState;
	TimeSpent: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ type: GridColumnType.CheckBox })
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;

	TimeBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	Summary: PXFieldState<PXFieldOptions.CommitChanges>;
}

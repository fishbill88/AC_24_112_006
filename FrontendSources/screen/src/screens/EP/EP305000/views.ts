import { PXView, PXFieldState, gridConfig, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnType, PXActionState, TextAlign, GridPreset } from "client-controls";

export class EPTimeCard extends PXView  {
	TimeCardCD : PXFieldState;
	Status : PXFieldState<PXFieldOptions.Disabled>;
	@selectorSettings("WeekID", "")
	WeekID : PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings("AcctCD", "")
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
	TimecardType : PXFieldState<PXFieldOptions.Disabled>;
	OrigTimecardCD : PXFieldState<PXFieldOptions.Disabled>;
	TimeSpentCalc : PXFieldState<PXFieldOptions.Disabled>;
	TimeBillableCalc : PXFieldState<PXFieldOptions.Disabled>;
	OvertimeSpentCalc : PXFieldState<PXFieldOptions.Disabled>;
	OvertimeBillableCalc : PXFieldState<PXFieldOptions.Disabled>;
	TotalSpentCalc : PXFieldState<PXFieldOptions.Disabled>;
	TotalBillableCalc : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Primary
})
export class CRActivity extends PXView  {
	@columnConfig({allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	Selected : PXFieldState;
	Subject : PXFieldState;
	ProjectID : PXFieldState;
	ProjectTaskID : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class EPTimeCardSummary extends PXView  {
	PreloadFromTasks: PXActionState;
	PreloadFromPreviousTimecard: PXActionState;
	PreloadHolidays: PXActionState;
	NormalizeTimecard: PXActionState;
	EarningType : PXFieldState;
	ParentNoteID : PXFieldState;
	ProjectID : PXFieldState;
	ProjectDescription : PXFieldState<PXFieldOptions.Hidden>;
	ProjectTaskID : PXFieldState;
	ProjectTaskDescription : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({type: GridColumnType.CheckBox})	CertifiedJob : PXFieldState;
	CostCodeID : PXFieldState;
	CostCodeDescription : PXFieldState<PXFieldOptions.Hidden>;
	UnionID : PXFieldState;
	LabourItemID : PXFieldState;
	WorkCodeID : PXFieldState;
	ShiftID : PXFieldState<PXFieldOptions.CommitChanges>;
	Mon : PXFieldState;
	Tue : PXFieldState;
	Wed : PXFieldState;
	Thu : PXFieldState;
	Fri : PXFieldState;
	Sat : PXFieldState;
	Sun : PXFieldState;
	TimeSpent : PXFieldState;
	@columnConfig({type: GridColumnType.CheckBox})	IsBillable : PXFieldState;
	Description : PXFieldState;
	ApprovalStatus : PXFieldState;
	ApproverID : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	topBarItems: {
		ViewActivity:
		{
			index: 0,
			config:
			{
				commandName: "ViewActivity",
				text: "ViewActivity"
			}
		},
		CreateActivity:
		{
			index: 1,
			config:
			{
				commandName: "CreateActivity",
				text: "Add Activity"
			}
		},
		View:
		{
			index: 2,
			config:
			{
				commandName: "View",
				text: "View"
			}
		},
	}
})
export class PMTimeActivity extends PXView  {
	ViewActivity : PXActionState;
	CreateActivity : PXActionState;
	View : PXActionState;
	Date_Date : PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID : PXFieldState;
	EarningTypeID : PXFieldState;
	ParentTaskNoteID : PXFieldState;
	ProjectID : PXFieldState;
	ProjectTaskID : PXFieldState;
	@columnConfig({ type: GridColumnType.CheckBox })
	CertifiedJob: PXFieldState;
	CostCodeID : PXFieldState;
	UnionID : PXFieldState;
	LabourItemID : PXFieldState;
	WorkCodeID : PXFieldState;
	ShiftID : PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("OpenAppointment")
	AppointmentID : PXFieldState<PXFieldOptions.CommitChanges>;
	LogLineNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceID : PXFieldState;
	ReportedInTimeZoneID : PXFieldState;
	Date_Time : PXFieldState<PXFieldOptions.CommitChanges>;
	TimeSpent : PXFieldState;
	@columnConfig({ type: GridColumnType.CheckBox })
	IsBillable: PXFieldState;
	BillableTimeCalc : PXFieldState;
	BillableOvertimeCalc : PXFieldState;
	Summary : PXFieldState;
	RegularTimeCalc : PXFieldState;
	OvertimeCalc : PXFieldState;
	OvertimeMultiplierCalc : PXFieldState;
	ApprovalStatus : PXFieldState;
	Day : PXFieldState;
	CaseCD : PXFieldState;
	ContractCD : PXFieldState;
	IsActivityExists : PXFieldState;
	AppointmentCustomerID : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class EPTimeCardItem extends PXView  {
	ProjectID : PXFieldState;
	TaskID : PXFieldState;
	CostCodeID : PXFieldState;
	InventoryID : PXFieldState;
	Description : PXFieldState;
	UOM : PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	Mon: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	Tue: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	Wed: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	Thu: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	Fri: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	Sat: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	Sun: PXFieldState;
	@columnConfig({ allowNull: false, textAlign: TextAlign.Right })
	TotalQty: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Primary
})
export class EPApproval extends PXView  {
	ApproverEmployee__AcctCD : PXFieldState;
	ApproverEmployee__AcctName : PXFieldState;
	WorkgroupID : PXFieldState;
	ApprovedByEmployee__AcctCD : PXFieldState;
	ApprovedByEmployee__AcctName : PXFieldState;
	OrigOwnerID : PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate : PXFieldState;
	@columnConfig({ allowUpdate: false, allowNull: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;
	AssignmentMapID : PXFieldState<PXFieldOptions.Hidden>;
	RuleID : PXFieldState<PXFieldOptions.Hidden>;
	StepID : PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime : PXFieldState<PXFieldOptions.Hidden>;
}

export class ReasonApproveRejectFilter extends PXView  {
	Reason : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReassignApprovalFilter extends PXView  {
	NewApprover : PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations : PXFieldState<PXFieldOptions.CommitChanges>;
}

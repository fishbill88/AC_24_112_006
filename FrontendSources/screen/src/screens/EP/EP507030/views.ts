import { PXView, PXFieldState, gridConfig, selectorSettings, PXFieldOptions, columnConfig, GridColumnType, TextAlign, GridPreset } from "client-controls";

export class EPSummaryFilter extends PXView  {
	ApproverID : PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings("WeekID", "")
	FromWeek : PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings("WeekID", "")
	TillWeek : PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID : PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID : PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
	RegularTime : PXFieldState<PXFieldOptions.Disabled>;
	RegularOvertime : PXFieldState<PXFieldOptions.Disabled>;
	RegularTotal : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Primary,
	quickFilterFields: ["EmployeeID", "Description"]
})
export class EPSummaryApprove extends PXView  {
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsApprove: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsReject: PXFieldState;
	WeekID_Description : PXFieldState;
	EmployeeID : PXFieldState;
	EarningType : PXFieldState;
	ParentNoteID : PXFieldState;
	ProjectID : PXFieldState;
	ProjectDescription : PXFieldState;
	ProjectTaskID : PXFieldState;
	ProjectTaskDescription : PXFieldState;
	ShiftID : PXFieldState;
	Mon : PXFieldState;
	Tue : PXFieldState;
	Wed : PXFieldState;
	Thu : PXFieldState;
	Fri : PXFieldState;
	Sat : PXFieldState;
	Sun : PXFieldState;
	TimeSpent : PXFieldState;
	@columnConfig({ type: GridColumnType.CheckBox })
	IsBillable: PXFieldState;
	Description : PXFieldState;
	TimeCardCD : PXFieldState;
}

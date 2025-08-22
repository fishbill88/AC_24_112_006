import {
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	GridColumnType,
	TextAlign,
	GridPreset
} from "client-controls";

export class EPActivityFilter extends PXView  {
	ApproverID : PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate : PXFieldState<PXFieldOptions.CommitChanges>;
	TillDate : PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID : PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID : PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
	RegularTime : PXFieldState<PXFieldOptions.Disabled>;
	BillableTime : PXFieldState<PXFieldOptions.Disabled>;
	RegularOvertime : PXFieldState<PXFieldOptions.Disabled>;
	BillableOvertime : PXFieldState<PXFieldOptions.Disabled>;
	RegularTotal : PXFieldState<PXFieldOptions.Disabled>;
	BillableTotal : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Primary,
	quickFilterFields: ["Owner", "Subject"]
})
export class PMTimeActivity extends PXView  {
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsApproved: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsReject: PXFieldState;
	Date : PXFieldState;
	OwnerID : PXFieldState;
	OwnerID_EPEmployee_AcctName : PXFieldState;
	WorkGroupID : PXFieldState;
	EarningTypeID : PXFieldState;
	ParentTaskNoteID : PXFieldState;
	@linkCommand("ViewContract")
	ContractID : PXFieldState;
	ProjectID : PXFieldState;
	ProjectDescription : PXFieldState;
	ProjectTaskID : PXFieldState;
	ProjectTaskDescription : PXFieldState;
	CostCodeID : PXFieldState;
	CostCodeDescription : PXFieldState;
	ShiftID : PXFieldState;
	TimeSpent : PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsBillable: PXFieldState;
	TimeBillable : PXFieldState;
	@linkCommand("ViewDetails")
	Summary : PXFieldState;
	ApproverID : PXFieldState;
	TimeCardCD : PXFieldState;
	@linkCommand("ViewCase")
	CRCase__CaseCD : PXFieldState;
	@linkCommand("ViewContract")
	ContractEx__ContractCD : PXFieldState;
	OvertimeBillable : PXFieldState;
}

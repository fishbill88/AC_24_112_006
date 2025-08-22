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

export class OwnedFilter extends PXView  {
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate : PXFieldState<PXFieldOptions.CommitChanges>;
	TillDate : PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID : PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID : PXFieldState<PXFieldOptions.CommitChanges>;
	ContractID : PXFieldState<PXFieldOptions.CommitChanges>;
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
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox, allowCheckAll: true })
	Selected: PXFieldState;
	Date: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OwnerID : PXFieldState;
	OwnerID_EPEmployee_AcctName : PXFieldState;
	WorkGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	EarningTypeID : PXFieldState;
	ParentTaskNoteID : PXFieldState;
	@linkCommand("ViewContract")
	ContractID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProjectID : PXFieldState;
	ProjectDescription: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProjectTaskID : PXFieldState;
	ProjectTaskDescription : PXFieldState;
	CostCodeID : PXFieldState;
	CostCodeDescription : PXFieldState;
	ShiftID : PXFieldState;
	TimeSpent : PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsBillable: PXFieldState;
	TimeBillable : PXFieldState;
	Summary : PXFieldState;
	ApproverID : PXFieldState;
	ApprovedDate : PXFieldState;
	@linkCommand("ViewCase")
	CRCase__CaseCD : PXFieldState;
	CRCase__Status : PXFieldState;
	@linkCommand("ViewContract")
	ContractEx__ContractCD : PXFieldState;
	OvertimeBillable : PXFieldState;
}

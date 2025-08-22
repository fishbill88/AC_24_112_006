import {
	PXView,
	PXFieldState,
	gridConfig,
	selectorSettings,
	PXFieldOptions,
	columnConfig,
	GridColumnType,
	PXActionState,
	TextAlign
} from "client-controls";

export class OwnedFilter extends PXView  {
	OwnerID : PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings("WeekID", "")
	FromWeek : PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings("WeekID", "")
	TillWeek : PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID : PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID : PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeReject : PXFieldState<PXFieldOptions.CommitChanges>;
	RegularTime : PXFieldState<PXFieldOptions.Disabled>;
	BillableTime : PXFieldState<PXFieldOptions.Disabled>;
	RegularOvertime : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
	BillableOvertime: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
	RegularTotal : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
	BillableTotal : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true,
	topBarItems: {
		View: {
			index: 0,
			config:	{
				commandName: "View",
				text: "View"
			}
		},
	}
})
export class PMTimeActivity extends PXView  {
	View : PXActionState;
	Date_Date : PXFieldState;
	Date_Time: PXFieldState;
	@columnConfig({	hideViewLink: true })
	EarningTypeID : PXFieldState;
	ParentTaskNoteID : PXFieldState;
	TimeSpent : PXFieldState;
	TimeBillable : PXFieldState;
	OvertimeBillable : PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ProjectID : PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID : PXFieldState<PXFieldOptions.CommitChanges>;
	LogLineNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceID : PXFieldState;
	AppointmentID : PXFieldState<PXFieldOptions.CommitChanges>;
	AppointmentCustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	LabourItemID : PXFieldState;
	ShiftID : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	Hold : PXFieldState;
	ApprovalStatus : PXFieldState;
	WorkGroupID : PXFieldState;
	ContractID : PXFieldState;
	ProjectDescription : PXFieldState;
	ProjectTaskDescription : PXFieldState;
	@columnConfig({type: GridColumnType.CheckBox})	CertifiedJob : PXFieldState;
	CostCodeDescription : PXFieldState;
	UnionID : PXFieldState;
	WorkCodeID : PXFieldState;
	@columnConfig({textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	IsBillable : PXFieldState;
	Summary : PXFieldState;
	ApproverID : PXFieldState;
	TimeCardCD : PXFieldState;
	CRCase__CaseCD : PXFieldState;
	ContractEx__ContractCD : PXFieldState;
	RefNoteID : PXFieldState;
}

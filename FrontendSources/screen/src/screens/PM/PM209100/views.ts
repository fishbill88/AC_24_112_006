import {
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Items extends PXView {
	NoteID: PXFieldState;
	ApprovalStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	Date_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	Date_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ApproverID: PXFieldState<PXFieldOptions.Disabled>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertifiedJob: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	UnionID: PXFieldState;
	Summary: PXFieldState<PXFieldOptions.Multiline>;
	EarningTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkCodeID: PXFieldState;
	TimeSpent: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeSpent: PXFieldState<PXFieldOptions.Disabled>;
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	Released: PXFieldState;
	TimeBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	OvertimeBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeRate: PXFieldState;
}

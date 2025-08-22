import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	columnConfig,
	gridConfig
} from "client-controls";

export class Document extends PXView {
	TimeCardCD: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	WeekID: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	TimecardType: PXFieldState<PXFieldOptions.Disabled>;
	OrigTimecardCD: PXFieldState<PXFieldOptions.Disabled>;
	TimeSetupCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeBillableSetupCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeRunCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeBillableRunCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeSuspendCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeBillableSuspendCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeTotalCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeBillableTotalCalc: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: true,
	allowUpdate: true,
	allowDelete: true
})
export class Summary extends PXView {
	PreloadFromPreviousTimecard: PXActionState;
	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	@columnConfig({ hideViewLink: true })
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	RateType: PXFieldState<PXFieldOptions.CommitChanges>;
	Mon: PXFieldState<PXFieldOptions.CommitChanges>;
	Tue: PXFieldState<PXFieldOptions.CommitChanges>;
	Wed: PXFieldState<PXFieldOptions.CommitChanges>;
	Thu: PXFieldState<PXFieldOptions.CommitChanges>;
	Fri: PXFieldState<PXFieldOptions.CommitChanges>;
	Sat: PXFieldState<PXFieldOptions.CommitChanges>;
	Sun: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeSpent: PXFieldState;
	IsBillable: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Details extends PXView {
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ProjectTaskID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	SetupTime: PXFieldState<PXFieldOptions.CommitChanges>;
	RunTime: PXFieldState<PXFieldOptions.CommitChanges>;
	SuspendTime: PXFieldState<PXFieldOptions.CommitChanges>;
	IsBillable: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Approval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({
		allowUpdate: false
	})
	Status: PXFieldState;
	@columnConfig({
		allowUpdate: false
	})
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

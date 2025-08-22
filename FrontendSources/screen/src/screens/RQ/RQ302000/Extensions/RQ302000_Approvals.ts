import {
	RQ302000
} from '../RQ302000';

import {
	PXView,
	createSingle,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	columnConfig,
	gridConfig,
	GridPreset
} from 'client-controls';


export interface RQ302000_Approvals extends RQ302000 {}
export class RQ302000_Approvals {
	@viewInfo({containerName: "Approvals"})
	Approval = createCollection(EPApproval);

	@viewInfo({containerName: "Enter Reason"})
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectFilter);

   	@viewInfo({containerName: "Reassign Approval"})
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class EPApproval extends PXView  {
	ApproverEmployee__AcctCD : PXFieldState;
	ApproverEmployee__AcctName : PXFieldState;
	@columnConfig({hideViewLink: true}) WorkgroupID : PXFieldState;
	ApprovedByEmployee__AcctCD : PXFieldState;
	ApprovedByEmployee__AcctName : PXFieldState;
	OrigOwnerID : PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate : PXFieldState;
	@columnConfig({allowUpdate: false, allowNull: false})	Status : PXFieldState;
	@columnConfig({allowUpdate: false})	Reason : PXFieldState;
	AssignmentMapID : PXFieldState<PXFieldOptions.Hidden>;
	RuleID : PXFieldState<PXFieldOptions.Hidden>;
	StepID : PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime : PXFieldState<PXFieldOptions.Hidden>;
}

export class ReasonApproveRejectFilter extends PXView  {
	Reason : PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView  {
	NewApprover : PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations : PXFieldState<PXFieldOptions.CommitChanges>;
}

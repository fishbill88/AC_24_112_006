import {
	PO301000
} from '../PO301000';

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


export interface PO301000_Approvals extends PO301000 {}
export class PO301000_Approvals {
	@viewInfo({containerName: 'Approvals'})
	Approval = createCollection(EPApproval);

	@viewInfo({containerName: 'Enter Reason'})
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	@viewInfo({containerName: 'Reassign Approval'})
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

@gridConfig({ preset: GridPreset.Details, allowDelete: false, allowInsert: false, allowUpdate: false })
export class EPApproval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	ApproveDate: PXFieldState;
	Status: PXFieldState;
	Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

export class ReasonApproveRejectParams extends PXView {
	Reason: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Multiline>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

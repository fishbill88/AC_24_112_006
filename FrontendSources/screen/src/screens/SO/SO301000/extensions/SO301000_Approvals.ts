import {
	SO301000
} from '../SO301000';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	columnConfig,
	gridConfig
} from 'client-controls';


export interface SO301000_Approvals extends SO301000 {}
export class SO301000_Approvals {
	@viewInfo({containerName: "Approvals"})
	Approval = createCollection(SOApproval);
}

@gridConfig({
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class SOApproval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	@columnConfig({hideViewLink: true}) WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	ApproveDate: PXFieldState;
	@columnConfig({allowUpdate: false}) Status: PXFieldState;
	@columnConfig({allowUpdate: false}) Reason: PXFieldState;
	AssignmentMapID: PXFieldState;
	RuleID: PXFieldState;
	StepID: PXFieldState;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

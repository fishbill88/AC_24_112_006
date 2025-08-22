import {
	PXView,
	PXFieldState,
	PXFieldOptions,

	createCollection,

	viewInfo,
	gridConfig,
	columnConfig,
	GridPreset
} from 'client-controls';

import { SO303000 } from '../SO303000';

export interface SO303000_Approvals extends SO303000 {}
export class SO303000_Approvals {
	@viewInfo({containerName: "Approvals"})
	Approval = createCollection(SOInvoiceApproval);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
})
export class SOInvoiceApproval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;

	@columnConfig({hideViewLink: true})
	WorkgroupID: PXFieldState;

	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;

	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;

	@columnConfig({allowUpdate: false})
	Status: PXFieldState;

	// @columnConfig({allowUpdate: false})
	// Reason: PXFieldState; // maybe later

	AssignmentMapID: PXFieldState;
	RuleID: PXFieldState;
	StepID: PXFieldState;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

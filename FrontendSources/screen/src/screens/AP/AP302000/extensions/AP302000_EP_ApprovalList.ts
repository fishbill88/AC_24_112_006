import { AP302000 } from '../AP302000';
import { PXView, PXFieldState, PXFieldOptions, createSingle } from 'client-controls';

export interface AP302000_EP_ApprovalList extends AP302000 { }
export class AP302000_EP_ApprovalList {

	// PX.Objects.EP.EPApprovalList

	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);

}

export class ReasonApproveRejectParams extends PXView {

	Reason: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class ReassignApprovalFilter extends PXView {

	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;

}

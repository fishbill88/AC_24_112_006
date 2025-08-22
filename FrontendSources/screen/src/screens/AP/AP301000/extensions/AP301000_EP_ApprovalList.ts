import { AP301000 } from '../AP301000';
import { PXView, PXFieldState, createSingle, PXFieldOptions } from 'client-controls';

export interface AP3010000_EP_ApprovalList extends AP301000 { }

export class AP3010000_EP_ApprovalList {

	// PX.Objects.EP.EPApprovalList

	ReasonApproveRejectParams = createSingle(ReasonApproveRejectFilter); // PX.Objects.EP.EPApprovalList

}

export class ReasonApproveRejectFilter extends PXView {

	Reason: PXFieldState<PXFieldOptions.CommitChanges>;

}

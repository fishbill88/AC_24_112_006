import { EP503010 } from '../EP503010';
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	createSingle,
} from "client-controls";

export interface EP503010_panel_Reassign extends EP503010 { }
export class EP503010_panel_Reassign {
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}
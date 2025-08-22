import { CR304500 } from "../CR304500";
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	gridConfig,
	viewInfo,
	PXActionState,
	columnConfig,
	GridPreset,
} from "client-controls";

export interface CR304500_Approvals extends CR304500 {}
export class CR304500_Approvals {
	@viewInfo({ containerName: "Approvals" })
	Approval = createCollection(EPApproval);
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	fastFilterByAllFields: false,
})
export class EPApproval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({ allowUpdate: false }) Status: PXFieldState;
	@columnConfig({ allowUpdate: false }) Reason: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

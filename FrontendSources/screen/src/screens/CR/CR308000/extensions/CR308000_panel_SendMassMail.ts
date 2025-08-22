import { CR308000 } from "../CR308000";
import { createSingle, PXView, PXFieldState, PXFieldOptions } from "client-controls";

export interface CR308000_panel_SendMassMail extends CR308000 {}
export class CR308000_panel_SendMassMail {
	MassMailPrepare = createSingle(CRMassMailPrepare);
}

export class CRMassMailPrepare extends PXView {
	CampaignUpdateListMembers: PXFieldState<PXFieldOptions.CommitChanges>;
}
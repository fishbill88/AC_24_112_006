import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	viewInfo,
	createSingle,
} from "client-controls";

export interface CR304000_CRMInfo extends CR304000 {}
export class CR304000_CRMInfo {
	@viewInfo({ containerName: "CRM Info" })
	ProbabilityCurrent = createSingle(CROpportunityProbability);

	ActivityStatistics = createSingle(CRActivityStatistics);
}

export class CROpportunityProbability extends PXView {
	Probability: PXFieldState<PXFieldOptions.Disabled>;
}

export class CRActivityStatistics extends PXView {
	LastIncomingActivityDate: PXFieldState;
	LastOutgoingActivityDate: PXFieldState;
}

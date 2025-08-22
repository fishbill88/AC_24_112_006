import { CR301000 } from "../CR301000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	PXActionState,
	viewInfo,
	columnConfig,
	GridPreset,
} from "client-controls";

export interface CR301000_Opportunities extends CR301000 {}
export class CR301000_Opportunities {
	@viewInfo({ containerName: "Opportunities" })
	Opportunities = createCollection(CROpportunity);
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowDelete: false,
	fastFilterByAllFields: false,
})
export class CROpportunity extends PXView {
	convertToOpportunityAll: PXActionState;

	@linkCommand("Opportunities_ViewDetails")
	OpportunityID: PXFieldState;
	Subject: PXFieldState;
	StageID: PXFieldState;
	Status: PXFieldState;
	CuryProductsAmount: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	CloseDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
	@linkCommand("Opportunities_CLassID_ViewDetails")
	ClassID: PXFieldState<PXFieldOptions.Hidden>;
	CROpportunityClass__Description: PXFieldState<PXFieldOptions.Hidden>;
}

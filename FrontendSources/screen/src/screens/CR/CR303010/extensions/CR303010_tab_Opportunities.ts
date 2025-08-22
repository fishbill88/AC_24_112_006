import { CR303010 } from "../CR303010";
import {
	PXView,
	PXFieldState,
	createCollection,
	viewInfo,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	GridPreset
} from "client-controls";

export interface CR303010_Opportunities extends CR303010 {}
export class CR303010_Opportunities {
	@viewInfo({ containerName: "Opportunities" })
	Opportunities = createCollection(CROpportunity);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
})
export class CROpportunity extends PXView {
	@linkCommand("Opportunities_ViewDetails")
	OpportunityID: PXFieldState;
	Subject: PXFieldState;
	StageID: PXFieldState;
	CROpportunityProbability__Probability: PXFieldState<PXFieldOptions.Hidden>;
	Status: PXFieldState;
	CuryProductsAmount: PXFieldState;
	CuryID: PXFieldState;
	CloseDate: PXFieldState;
	@linkCommand("Opportunities_Contact_ViewDetails")
	Contact__DisplayName: PXFieldState<PXFieldOptions.Hidden>;
	WorkgroupID: PXFieldState<PXFieldOptions.Hidden>;
	OwnerID: PXFieldState;
}
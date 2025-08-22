import { CR302000 } from "../CR302000";
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

export interface CR302000_Opportunities extends CR302000 {}
export class CR302000_Opportunities {
	@viewInfo({ containerName: "Opportunities" })
	Opportunities = createCollection(CROpportunity);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	fastFilterByAllFields: false,
	topBarItems: {
		AddOpportunity: {
			index: 2,
			config: {
				commandName: "AddOpportunity",
				images: { normal: "main@RecordAdd" },
			},
		},
	},
})
export class CROpportunity extends PXView {
	AddOpportunity: PXActionState;

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

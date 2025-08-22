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
	GridPreset
} from "client-controls";

export interface CR302000_Cases extends CR302000 {}
export class CR302000_Cases {
	@viewInfo({ containerName: "Cases" })
	Cases = createCollection(CRCases);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	adjustPageSize: true,
	fastFilterByAllFields: false,
	topBarItems: {
		AddCase: {
			index: 2,
			config: {
				commandName: "AddCase",
				images: { normal: "main@RecordAdd" },
			},
		},
	},
})
export class CRCases extends PXView {
	//convertToOpportunityAll: PXActionState;
	AddCase: PXActionState;

	@linkCommand("Cases_ViewDetails") CaseCD: PXFieldState;
	Subject: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CaseClassID: PXFieldState;
	Severity: PXFieldState;
	Status: PXFieldState;
	Resolution: PXFieldState;
	ReportedOnDateTime: PXFieldState;
	TimeEstimated: PXFieldState<PXFieldOptions.Hidden>;
	ResolutionDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	WorkgroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OwnerID: PXFieldState;
}

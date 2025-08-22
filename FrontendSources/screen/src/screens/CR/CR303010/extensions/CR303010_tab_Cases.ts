import { CR303010 } from "../CR303010";
import {
	PXView,
	PXFieldState,
	createCollection,
	viewInfo,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	GridPreset,
} from "client-controls";

export interface CR303010_Cases extends CR303010 {}
export class CR303010_Cases {
	@viewInfo({ containerName: "Cases" })
	Cases = createCollection(CRCase);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
})
export class CRCase extends PXView {
	@linkCommand("Cases_ViewDetails")
	CaseCD: PXFieldState;
	Subject: PXFieldState;
	CaseClassID: PXFieldState;
	Severity: PXFieldState;
	Status: PXFieldState;
	Resolution: PXFieldState;
	ReportedOnDateTime: PXFieldState;
	TimeEstimated: PXFieldState<PXFieldOptions.Hidden>;
	ResolutionDate: PXFieldState;
	WorkgroupID: PXFieldState;
	OwnerID: PXFieldState;
}

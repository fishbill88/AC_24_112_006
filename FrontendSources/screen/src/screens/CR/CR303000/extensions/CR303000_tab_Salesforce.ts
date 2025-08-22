import { CR303000 } from "../CR303000";
import {
	PXView,
	PXFieldState,
	linkCommand,
	createCollection,
	PXFieldOptions,
	gridConfig,
	PXActionState,
	viewInfo,
	GridPagerMode,
	GridPreset,
} from "client-controls";

export interface CR303000_Salesforce extends CR303000 {}
export class CR303000_Salesforce {
	@viewInfo({ containerName: "Salesforce" })
	SyncRecs = createCollection(SFSyncRecord);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	pagerMode: GridPagerMode.InfiniteScroll,
	fastFilterByAllFields: false,
})
export class SFSyncRecord extends PXView {
	SyncSalesforce: PXActionState;

	SYProvider__Name: PXFieldState;
	@linkCommand("GoToSalesforce")
	RemoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	Operation: PXFieldState;
	LastErrorMessage: PXFieldState;
	LastAttemptTS: PXFieldState;
	AttemptCount: PXFieldState;
	SFEntitySetup__ImportScenario: PXFieldState;
	SFEntitySetup__ExportScenario: PXFieldState;
}

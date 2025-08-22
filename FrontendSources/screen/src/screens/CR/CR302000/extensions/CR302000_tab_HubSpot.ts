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
	GridPagerMode,
} from "client-controls";

export interface CR302000_HubSpot extends CR302000 {}
export class CR302000_HubSpot {
	@viewInfo({ containerName: "HubSpot" })
	HubSpotSyncRecs = createCollection(HSSyncRecord);
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	allowUpdate: false,
	adjustPageSize: true,
	pagerMode: GridPagerMode.InfiniteScroll,
	fastFilterByAllFields: false,
})
export class HSSyncRecord extends PXView {
	SyncHubSpot: PXActionState;
	PushToHubSpot: PXActionState;
	PullFromHubSpot: PXActionState;

	SYProvider__Name: PXFieldState;
	@linkCommand("GoToHubSpot")
	RemoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	SyncStatus: PXFieldState;
	LastSource: PXFieldState;
	LastOperation: PXFieldState;
	LastErrorMessageSimplified: PXFieldState;
	LastAttemptTS: PXFieldState;
	RemoteTS: PXFieldState;
	AttemptCount: PXFieldState;
	HSEntitySetup__ImportScenario: PXFieldState;
	HSEntitySetup__ExportScenario: PXFieldState;
	SyncRecordID: PXFieldState<PXFieldOptions.Hidden>;
	LastErrorMessage: PXFieldState<PXFieldOptions.Hidden>;
}

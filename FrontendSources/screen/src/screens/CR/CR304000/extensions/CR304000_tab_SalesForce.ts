import { CR304000 } from "../CR304000";
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

export interface CR304000_SyncStatus extends CR304000 {}
export class CR304000_SyncStatus {
	@viewInfo({ containerName: "Sync Status" })
	SyncRecs = createCollection(SFSyncRecord);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
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

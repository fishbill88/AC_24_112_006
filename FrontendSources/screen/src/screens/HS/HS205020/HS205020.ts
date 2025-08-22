import {
	PXView,
	PXFieldState,
	graphInfo,
	PXScreen,
	createCollection,
	gridConfig,
	PXFieldOptions,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.DataSync.HubSpot.HSSetupMaint",
	primaryView: "EntitySetup",
	bpEventsIndicator: false,
	udfTypeField: "",
})
export class HS205020 extends PXScreen {
	EntitySetup = createCollection(HSEntitySetup);
}

@gridConfig({
	preset: GridPreset.Primary,
	suppressNoteFiles: true,
	fastFilterByAllFields: false,
})
export class HSEntitySetup extends PXView {
	EntityType: PXFieldState<PXFieldOptions.CommitChanges>;
	ImportScenario: PXFieldState;
	ExportScenario: PXFieldState;
	MaxAttemptCount: PXFieldState;
	PoolingPeriod: PXFieldState;
	MasterSource: PXFieldState;
	SyncSortOrder: PXFieldState;
	LastMissedSyncDateTime: PXFieldState;
	LastFullSyncDateTime: PXFieldState;
}

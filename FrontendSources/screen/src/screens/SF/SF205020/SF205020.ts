import {
	graphInfo,
	PXView,
	PXFieldState,
	gridConfig,
	PXScreen,
	columnConfig,
	createCollection,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Salesforce.SFSetupMaint",
	primaryView: "EntitySetup",
})
export class SF205020 extends PXScreen {
	EntitySetup = createCollection(SFEntitySetup);
}

@gridConfig({
	preset: GridPreset.Primary,
	autoAdjustColumns: true,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
})
export class SFEntitySetup extends PXView {
	EntityType: PXFieldState;
	ImportScenario: PXFieldState;
	ExportScenario: PXFieldState;
	MaxAttemptCount: PXFieldState;
	SyncSortOrder: PXFieldState;
	LastMissedSyncDateTime: PXFieldState;
	LastFullSyncDateTime: PXFieldState;
}

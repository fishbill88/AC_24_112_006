import {
	PXView,
	PXFieldState,
	graphInfo,
	PXScreen,
	createCollection,
	createSingle,
	gridConfig,
	PXFieldOptions,
	columnConfig,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.DataSync.HubSpot.HSReSyncMaint",
	primaryView: "Filter",
	bpEventsIndicator: false,
	udfTypeField: "",
})
export class HS205035 extends PXScreen {
	Filter = createSingle(HSSyncFilter);
	RStoLSSyncProc = createCollection(HSEntitySetup);
}

export class HSSyncFilter extends PXView {
	SyncMode: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
	autoAdjustColumns: true,
	fastFilterByAllFields: false,
})
export class HSEntitySetup extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({
		allowCheckAll: true,
		width: 10,
	})
	Selected: PXFieldState;
	EntityType: PXFieldState;
	MasterSource: PXFieldState;
	LastMissedSyncDateTime: PXFieldState;
	LastFullSyncDateTime: PXFieldState;
}

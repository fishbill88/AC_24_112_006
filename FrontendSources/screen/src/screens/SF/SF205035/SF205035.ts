import {
	graphInfo,
	PXView,
	PXFieldState,
	gridConfig,
	PXScreen,
	columnConfig,
	createCollection,
	PXFieldOptions,
	createSingle,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: "PX.Salesforce.SFReSyncMaint", primaryView: "Filter" })
export class SF205035 extends PXScreen {
	RStoLSSyncProc = createCollection(SFEntitySetup);
	Filter = createSingle(SFSyncFilter);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	autoAdjustColumns: true,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
})
export class SFEntitySetup extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowCheckAll: true, width: 10 })
	Selected: PXFieldState;
	EntityType: PXFieldState;
	LastMissedSyncDateTime: PXFieldState;
	LastFullSyncDateTime: PXFieldState;
}

export class SFSyncFilter extends PXView {
	SyncMode: PXFieldState<PXFieldOptions.CommitChanges>;
}

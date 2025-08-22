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
	PXActionState,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.DataSync.HubSpot.HSSyncMaint",
	primaryView: "RStoLSSyncProc",
	bpEventsIndicator: false,
	udfTypeField: "",
})
export class HS205030 extends PXScreen {
	RStoLSSyncProc = createCollection(HSEntitySetup);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	suppressNoteFiles: true,
	allowUpdate: false,
	fastFilterByAllFields: false,
})
export class HSEntitySetup extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	SyncProcessStatus: PXFieldState;
	EntityType: PXFieldState;
	ImportScenario: PXFieldState;
	ExportScenario: PXFieldState;
	MasterSource: PXFieldState;
	LastRealTimeDateTime: PXFieldState;
}

import {
	graphInfo,
	PXView,
	PXFieldState,
	gridConfig,
	PXScreen,
	columnConfig,
	createCollection,
	PXFieldOptions,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.Salesforce.SFSyncMaint",
	primaryView: "RStoLSSyncProc",
})
export class SF205030 extends PXScreen {
	RStoLSSyncProc = createCollection(SFEntitySetup);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	autoAdjustColumns: true,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
})
export class SFEntitySetup extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowCheckAll: true, width: 10 }) Selected: PXFieldState;
	ProcStatus: PXFieldState;
	EntityType: PXFieldState;
	ImportScenario: PXFieldState;
	ExportScenario: PXFieldState;
	LastNotificationTime: PXFieldState;
	LastNotification: PXFieldState;
}

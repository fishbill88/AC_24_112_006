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
	linkCommand,
	PXActionState,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Salesforce.SFRealtimeSyncStateMaint",
	primaryView: "Filter",
})
export class SF205040 extends PXScreen {
	GoToSalesforce: PXActionState;
	ShowEntity: PXActionState;

	Records = createCollection(SFSyncRecord);
	Filter = createSingle(SFSyncFilter);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	allowUpdate: false,
	autoAdjustColumns: true,
	fastFilterByAllFields: false,
})
export class SFSyncRecord extends PXView {
	@columnConfig({ allowCheckAll: true, width: 10 })
	Selected: PXFieldState;
	EntityType: PXFieldState;
	LocalID: PXFieldState;
	@linkCommand("ShowEntity")
	DisplayName: PXFieldState;
	LocalTS: PXFieldState;
	@linkCommand("GoToSalesforce")
	RemoteID: PXFieldState;
	RemoteTS: PXFieldState;
	Operation: PXFieldState;
	Status: PXFieldState;
	LastErrorMessage: PXFieldState;
	LastAttemptTS: PXFieldState;
	AttemptCount: PXFieldState;
}

export class SFSyncFilter extends PXView {
	Entity: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	ErrorsOnly: PXFieldState<PXFieldOptions.CommitChanges>;
}

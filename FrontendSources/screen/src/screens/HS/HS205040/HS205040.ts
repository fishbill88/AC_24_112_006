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
	linkCommand,
	GridPreset,
	GridFilterBarVisibility,
} from "client-controls";

@graphInfo({
	graphType: "PX.DataSync.HubSpot.HSSyncRecordMaint",
	primaryView: "Filter",
	bpEventsIndicator: false,
	udfTypeField: "",
})
export class HS205040 extends PXScreen {
	GoToHubSpot: PXActionState;
	ShowEntity: PXActionState;

	Filter = createSingle(HSSyncRecordFilter);
	Records = createCollection(HSSyncRecord);
}

export class HSSyncRecordFilter extends PXView {
	Entity: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	ErrorsOnly: PXFieldState<PXFieldOptions.CommitChanges>;
	SyncRecordID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	showFilterBar: GridFilterBarVisibility.OnDemand,
	suppressNoteFiles: true,
	allowUpdate: false,
	autoAdjustColumns: true,
	quickFilterFields: [
		"DisplayName",
		"RemoteName",
		"LastErrorMessageSimplified",
	],
})
export class HSSyncRecord extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({
		allowCheckAll: true,
		width: 10,
	})
	Selected: PXFieldState;
	SyncRecordID: PXFieldState<PXFieldOptions.Hidden>;
	EntityType: PXFieldState;
	@linkCommand("ShowEntity") DisplayName: PXFieldState;
	LocalTS: PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand("GoToHubSpot")
	RemoteName: PXFieldState<PXFieldOptions.CommitChanges>;
	RemoteTS: PXFieldState<PXFieldOptions.Hidden>;
	SyncStatus: PXFieldState;
	LastSource: PXFieldState;
	LastOperation: PXFieldState;
	LastErrorMessage: PXFieldState<PXFieldOptions.Hidden>;
	LastErrorMessageSimplified: PXFieldState;
	LastAttemptTS: PXFieldState<PXFieldOptions.Hidden>;
	AttemptCount: PXFieldState<PXFieldOptions.Hidden>;
}

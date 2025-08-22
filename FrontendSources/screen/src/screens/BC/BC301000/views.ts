import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	gridConfig
} from "client-controls";

export class MasterView extends PXView {
	BindingID: PXFieldState<PXFieldOptions.CommitChanges>;
	EntityType: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class DetailsView extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("NavigateEntity")
	SyncID: PXFieldState;
	ConnectorType: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("NavigateEntity")
	EntityType: PXFieldState;
	BindingID: PXFieldState;
	@linkCommand("NavigateLocal")
	LocalID: PXFieldState;
	@linkCommand("NavigateLocal")
	Source: PXFieldState;
	@linkCommand("NavigateExtern")
	ExternID: PXFieldState;
	ExternDescription: PXFieldState;
	@columnConfig({ allowFastFilter: true })
	Status: PXFieldState;
	PendingSync: PXFieldState;
	BCEntity__PrimarySystem: PXFieldState;
	BCEntity__Direction: PXFieldState;
	@columnConfig({ format: "g" })
	LocalTS: PXFieldState;
	@columnConfig({ format: "g" })
	ExternTS: PXFieldState;
	ExternHash: PXFieldState;
	LastErrorMessage: PXFieldState;
	LastOperation: PXFieldState;
	@columnConfig({ format: "g" })
	LastOperationTS: PXFieldState;
	AttemptCount: PXFieldState;
	BCEntity__IsActive: PXFieldState;
}

export class StatusEditPanel extends PXView {
	ConnectorType: PXFieldState<PXFieldOptions.CommitChanges>;
	BindingID: PXFieldState<PXFieldOptions.CommitChanges>;
	EntityType: PXFieldState<PXFieldOptions.CommitChanges>;
	LocalID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternID: PXFieldState<PXFieldOptions.CommitChanges>;
	NeedSync: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	autoAdjustColumns: true
})
export class StatusDetailsPanel extends PXView {
	EntityType: PXFieldState;
	@linkCommand("NavigateLocal")
	LocalID: PXFieldState;
	@linkCommand("NavigateLocal")
	Source: PXFieldState;
	@linkCommand("NavigateExtern")
	ExternID: PXFieldState;
}

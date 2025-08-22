import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	linkCommand,
	gridConfig
} from "client-controls";

export class ProcessFilter extends PXView {
	BindingID: PXFieldState<PXFieldOptions.CommitChanges>;
	EntityType: PXFieldState<PXFieldOptions.CommitChanges>;
	RowsToProcess: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class Statuses extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	ConnectorType: PXFieldState;
	BindingID: PXFieldState;
	@linkCommand("NavigateEntity")
	EntityType: PXFieldState;
	BCEntity__PrimarySystem: PXFieldState;
	BCEntity__Direction: PXFieldState;
	@linkCommand("NavigateLocal")
	Source: PXFieldState;
	@linkCommand("NavigateExtern")
	ExternID: PXFieldState;
	ExternDescription: PXFieldState;
	Status: PXFieldState;
	LastErrorMessage: PXFieldState;
	LastOperation: PXFieldState;
	LocalTS: PXFieldState;
	ExternTS: PXFieldState;
	SyncID: PXFieldState;
	LocalID: PXFieldState;
	LastOperationTS: PXFieldState;
}

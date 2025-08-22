import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, PXActionState, GridColumnGeneration } from "client-controls";

// Views

export class SYImportOperation extends PXView {
	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
	SkipHeaders: PXFieldState;
}

@gridConfig({ adjustPageSize: true, autoAdjustColumns: true, mergeToolbarWith: 'ScreenToolbar', syncPosition: true })
export class SYMapping extends PXView {
	viewHistory: PXActionState;
	viewPreparedData: PXActionState;

	@columnConfig({ allowCheckAll: true, width: 20 }) Selected: PXFieldState;
	Name: PXFieldState;
	ScreenDescription: PXFieldState;
	@columnConfig({ hideViewLink: true }) ProviderID: PXFieldState;
	SyncType: PXFieldState;
	Status: PXFieldState;
	NbrRecords: PXFieldState;
	PreparedOn: PXFieldState;
	CompletedOn: PXFieldState;
}

@gridConfig({ allowUpdate: false, adjustPageSize: true, autoAdjustColumns: true, allowDelete: false, allowInsert: false })
export class SYHistory extends PXView {
	Status: PXFieldState;
	NbrRecords: PXFieldState;
	@columnConfig({width: 200}) ExportTimeStamp: PXFieldState;
	@columnConfig({width: 250}) StatusDate: PXFieldState;
}

@gridConfig({ generateColumns: GridColumnGeneration.AppendDynamic, adjustPageSize: true, allowDelete: false, allowInsert: false })
export class SYData extends PXView {
	LineNbr: PXFieldState;
	IsActive: PXFieldState;
	ErrorMessage: PXFieldState;
	IsProcessed: PXFieldState;
}
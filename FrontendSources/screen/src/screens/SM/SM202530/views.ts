import {
	PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig } from "client-controls";

// Views

export class SynchronizationFilter extends PXView {
	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class UploadFile extends PXView {
	Selected: PXFieldState;
	Name: PXFieldState;
	SourceType: PXFieldState;
	SourceUri: PXFieldState;
	SourceIsFolder: PXFieldState;
	//CreatedDateTime: PXFieldState<PXFieldOptions.Disabled>;
	SourceLastImportDate: PXFieldState;
	LastExportDate: PXFieldState;
	@columnConfig({hideViewLink: true})
	CreatedByID: PXFieldState;
}
import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXFieldState,
	PXFieldOptions,
	PXView,
	gridConfig,
	PXPageLoadBehavior,
	GridPreset
} from 'client-controls';

@graphInfo({
	graphType: 'PX.Objects.IN.INUpdateMCAssignment',
	primaryView: 'UpdateSettings',
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
	hideFilesIndicator: true,
	hideNotesIndicator: true
})
export class IN506100 extends PXScreen {

	@viewInfo({containerName: 'Selection'})
	UpdateSettings = createSingle(UpdateMCAssignmentSettings);
	@viewInfo({containerName: 'Details'})
	ResultPreview = createCollection(UpdateMCAssignmentResult);
}

export class UpdateMCAssignmentSettings extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Year: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.Disabled>;
	EndDate: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	allowUpdate: false,
	batchUpdate: true
})
export class UpdateMCAssignmentResult extends PXView {
	InventoryID: PXFieldState;
	Descr: PXFieldState;
	OldMC: PXFieldState;
	MCFixed: PXFieldState;
	NewMC: PXFieldState;
}
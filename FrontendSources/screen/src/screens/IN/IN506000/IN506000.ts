import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	gridConfig,
	GridPreset
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.IN.INUpdateABCAssignment', primaryView: 'UpdateSettings' })
export class IN506000 extends PXScreen {

	@viewInfo({ containerName: "Update ABC Codes Filter" })
	UpdateSettings = createSingle(UpdateSettings);
	@viewInfo({ containerName: "Items" })
	ResultPreview = createCollection(ResultPreview);
}

export class UpdateSettings extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	batchUpdate: true,
	allowUpdate: false
})
export class ResultPreview extends PXView {
	InventoryID: PXFieldState;
	Descr: PXFieldState;
	OldABCCode: PXFieldState;
	ABCCodeFixed: PXFieldState;
	YtdCost: PXFieldState;
	Ratio: PXFieldState;
	CumulativeRatio: PXFieldState;
	NewABCCode: PXFieldState;
}
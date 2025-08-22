import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	PXFieldOptions,
	viewInfo,
	gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.IN.INClosingProcess', primaryView: 'Filter' })
export class IN509000 extends PXScreen {

	@viewInfo({ containerName: "Close Financial Period Filter" })
	Filter = createSingle(Filter);
	@viewInfo({ containerName: "Financial Periods" })
	FinPeriods = createCollection(FinPeriods);
}

export class Filter extends PXView {
	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	FromYear: PXFieldState<PXFieldOptions.CommitChanges>;
	ToYear: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	mergeToolbarWith: 'ScreenToolbar'
})
export class FinPeriods extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;
	Descr: PXFieldState;
}
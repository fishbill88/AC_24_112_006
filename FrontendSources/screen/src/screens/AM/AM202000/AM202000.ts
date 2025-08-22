import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

@gridConfig({
	preset: GridPreset.Primary,
	initNewRow: true,
})
export class ForecastRecords extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	Interval: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	BeginDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState;
	Dependent: PXFieldState;
	ActiveFlg: PXFieldState;
	ForecastID: PXFieldState;
	CustomerID: PXFieldState;
	CustomerID_description: PXFieldState;
	InventoryID_description: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
	SubItemID: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.Forecast', primaryView: 'ForecastRecords' })
export class AM202000 extends PXScreen {
	ForecastRecords = createCollection(ForecastRecords);
}

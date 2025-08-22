import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.GenerateForecastProcess', primaryView: 'SelectedRecs' })
export class AM502000 extends PXScreen {
	SelectedRecs = createCollection(AMForecastStaging);
	Settings = createSingle(ForecastSettings);
}

@gridConfig({
	preset: GridPreset.Primary,
	adjustPageSize: false,
	initNewRow: true,
})
export class AMForecastStaging extends PXView {
	Selected: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	BeginDate: PXFieldState;
	EndDate: PXFieldState;
	ForecastQty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	ChangeUnits: PXFieldState;
	PercentChange: PXFieldState;
	@columnConfig({ hideViewLink: true }) CustomerID2: PXFieldState;
	Dependent: PXFieldState;
	LastYearSalesQty: PXFieldState;
	LastYearBaseQty: PXFieldState;
	Seasonality: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
}

export class ForecastSettings extends PXView {
	ForecastDate: PXFieldState;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Seasonality: PXFieldState;
	GrowthRate: PXFieldState;
	GrowthFactor: PXFieldState;
	CalculateByMonth: PXFieldState<PXFieldOptions.CommitChanges>;
	Years: PXFieldState;
	Dependent: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessByCustomer: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteId: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
}

import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent,  CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INRepForecastApplicationGraph', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class IN509500 extends PXScreen {
	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(Filter);
	@viewInfo({containerName: 'Items Requiring Replenishment'})
	Records = createCollection(INItemSite);
}

// Views

export class Filter extends PXView  {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplenishmentPolicyID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassCD: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ['ReplenishmentPolicyID'] })
export class INItemSite extends PXView  {
	@columnConfig({allowCheckAll: true}) Selected: PXFieldState;
	@columnConfig({hideViewLink: true}) SiteID: PXFieldState;
	@columnConfig({hideViewLink: true}) InventoryID: PXFieldState;
	PreferredVendorOverride: PXFieldState;
	@columnConfig({hideViewLink: true}) PreferredVendorID: PXFieldState;
	@columnConfig({hideViewLink: true}) PreferredVendorLocationID: PXFieldState;
	@columnConfig({hideViewLink: true}) ReplenishmentClassID: PXFieldState;
	ReplenishmentPolicyOverride: PXFieldState;
	@columnConfig({hideViewLink: true}) ReplenishmentPolicyID: PXFieldState;
	ReplenishmentMethod: PXFieldState;
	ReplenishmentSource: PXFieldState;
	@columnConfig({hideViewLink: true}) ReplenishmentSourceSiteID: PXFieldState;
	MaxShelfLifeOverride: PXFieldState;
	MaxShelfLife: PXFieldState;
	SafetyStockOverride: PXFieldState;
	SafetyStock: PXFieldState;
	SafetyStockSuggested: PXFieldState;
	MinQtyOverride: PXFieldState;
	MinQty: PXFieldState;
	MinQtySuggested: PXFieldState;
	MaxQtyOverride: PXFieldState;
	MaxQty: PXFieldState;
	MaxQtySuggested: PXFieldState;
	SubItemOverride: PXFieldState;
	LastForecastDate: PXFieldState;
	LastFCApplicationDate: PXFieldState;
	DemandPerDayAverage: PXFieldState;
	DemandPerDaySTDEV: PXFieldState;
	LeadTimeAverage: PXFieldState;
	LeadTimeSTDEV: PXFieldState;
}

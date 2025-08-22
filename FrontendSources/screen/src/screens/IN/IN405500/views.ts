import {
	PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions,
	linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState } from "client-controls";

// Views

export class INDeadStockEnqFilter extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SelectBy: PXFieldState<PXFieldOptions.CommitChanges>;
	InStockDays: PXFieldState<PXFieldOptions.CommitChanges>;
	InStockSince: PXFieldState<PXFieldOptions.CommitChanges>;
	NoSalesDays: PXFieldState<PXFieldOptions.CommitChanges>;
	NoSalesSince: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({adjustPageSize: true, mergeToolbarWith: "ScreenToolbar"})
export class INDeadStockEnqResult extends PXView {
	@columnConfig({hideViewLink: true, allowUpdate: false})
	SiteID: PXFieldState;
	@columnConfig({allowUpdate: false})
	InventoryID: PXFieldState;
	@columnConfig({hideViewLink: true, allowUpdate: false})
	SubItemID: PXFieldState;
	@columnConfig({allowUpdate: false})
	InventoryItem__Descr: PXFieldState;
	@columnConfig({allowUpdate: false})
	InStockQty: PXFieldState;
	@columnConfig({allowUpdate: false})
	DeadStockQty: PXFieldState;
	@columnConfig({hideViewLink: true, allowUpdate: false})
	InventoryItem__BaseUnit: PXFieldState;
	@columnConfig({allowUpdate: false})
	InDeadStockDays: PXFieldState;
	@columnConfig({allowUpdate: false})
	LastPurchaseDate: PXFieldState;
	@columnConfig({allowUpdate: false})
	LastSaleDate: PXFieldState;
	@columnConfig({allowUpdate: false})
	LastCost: PXFieldState;
	@columnConfig({allowUpdate: false})
	TotalDeadStockCost: PXFieldState;
	@columnConfig({allowUpdate: false})
	AverageItemCost: PXFieldState;
	@columnConfig({hideViewLink: true, allowUpdate: false})
	BaseCuryID: PXFieldState;
}

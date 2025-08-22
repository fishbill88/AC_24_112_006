import {
	PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions,
	linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState } from "client-controls";

// Views

export class MassConvertStockNonStockFilter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	NonStockItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	NonStockPostClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	StockItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	StockValMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	StockPostClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	StockLotSerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({syncPosition: true, adjustPageSize: true, mergeToolbarWith: "ScreenToolbar"})
export class InventoryItem extends PXView {
	@columnConfig({allowUpdate: false, allowCheckAll: true})
	Selected: PXFieldState;
	@columnConfig({allowUpdate: false})
	InventoryCD: PXFieldState;
	@columnConfig({allowUpdate: false})
	Descr: PXFieldState;
	@columnConfig({allowUpdate: false})
	ItemType: PXFieldState;
	@columnConfig({allowUpdate: false})
	ValMethod: PXFieldState;
	@columnConfig({allowUpdate: false})
	ItemClassID: PXFieldState;
	@columnConfig({allowUpdate: false})
	PostClassID: PXFieldState;
	@columnConfig({allowUpdate: false})
	LotSerClassID: PXFieldState;
	@columnConfig({allowUpdate: false})
	BaseUnit: PXFieldState;
	@columnConfig({allowUpdate: false})
	TaxCategoryID: PXFieldState;
}

@gridConfig({syncPosition: true, adjustPageSize: true})
export class InventoryLinkFilter extends PXView {
	@columnConfig({allowUpdate: false, hideViewLink: true})
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	Descr: PXFieldState;
}

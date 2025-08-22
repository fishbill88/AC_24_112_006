import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent,  CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.Turnover.TurnoverEnq', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class IN407010 extends PXScreen {
	SelectItems: PXActionState;

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(INTurnoverEnqFilter);
	@viewInfo({containerName: 'Turnover Calculation Items'})
	TurnoverCalcItems = createCollection(TurnoverCalcItem);

	@viewInfo({containerName: 'Inventory Item List'})
	SelectedItems = createCollection(InventoryLinkFilter);
}

// Views

export class INTurnoverEnqFilter extends PXView  {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	batchUpdate: true,
	quickFilterFields: ["SiteID"]
})
export class TurnoverCalcItem extends PXView  {
	InventoryID: PXFieldState<PXFieldOptions.Disabled>;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true}) SiteID: PXFieldState;
	@columnConfig({hideViewLink: true}) UOM: PXFieldState;
	BegCost: PXFieldState;
	BegQty: PXFieldState;
	YtdCost: PXFieldState;
	YtdQty: PXFieldState;
	AvgCost: PXFieldState;
	AvgQty: PXFieldState;
	SoldCost: PXFieldState;
	SoldQty: PXFieldState;
	@columnConfig({nullText: '-'})	CostRatio: PXFieldState;
	@columnConfig({nullText: '-'})	QtyRatio: PXFieldState;
	@columnConfig({nullText: '-'})	CostSellDays: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true
})
export class InventoryLinkFilter extends PXView {
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	Descr: PXFieldState;
}

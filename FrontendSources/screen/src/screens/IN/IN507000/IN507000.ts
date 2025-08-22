import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.Turnover.ManageTurnover', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class IN507000 extends PXScreen {
	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(INTurnoverCalcFilter);
	@viewInfo({containerName: 'Details'})
	TurnoverCalcs = createCollection(INTurnoverCalc);
}

// Views

export class INTurnoverCalcFilter extends PXView  {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfPeriods: PXFieldState;
	CalculateBy: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry
})
export class INTurnoverCalc extends PXView  {
	@columnConfig({allowCheckAll: true})	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowNull: false})	BranchID: PXFieldState;
	@columnConfig({allowNull: false})	FromPeriodID: PXFieldState;
	@columnConfig({allowNull: false})	ToPeriodID: PXFieldState;
	SiteID: PXFieldState;
	ItemClassID: PXFieldState;
	InventoryID: PXFieldState;
	CreatedDateTime: PXFieldState;
}

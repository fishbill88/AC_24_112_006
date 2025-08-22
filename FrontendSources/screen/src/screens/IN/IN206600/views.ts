import { PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState } from 'client-controls';

// Views

export class INReplenishmentPolicy extends PXView  {
	ReplenishmentPolicyID: PXFieldState;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	CalendarID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({initNewRow: true, allowImport: true})
export class INReplenishmentSeason extends PXView  {
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowNull: false})
	Factor: PXFieldState;
}

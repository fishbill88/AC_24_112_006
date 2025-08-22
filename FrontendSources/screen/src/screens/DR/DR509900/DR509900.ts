import {
	PXScreen, createSingle, createCollection, graphInfo,
	PXView,
	PXFieldState,
	gridConfig,
	columnConfig,
	PXFieldOptions
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.DR.DRBalanceValidation', primaryView: 'Filter' })
export class DR509900 extends PXScreen {
	Filter = createSingle(DRBalanceValidationFilter);
	Items = createCollection(DRBalanceType);
}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class DRBalanceType extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ allowNull: false })
	AccountType: PXFieldState;
}

export class DRBalanceValidationFilter extends PXView {
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}

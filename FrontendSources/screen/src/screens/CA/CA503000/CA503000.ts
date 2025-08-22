import {
	PXScreen, createSingle, createCollection, graphInfo,
	PXView,
	PXFieldState,
	gridConfig,
	columnConfig,
    PXFieldOptions
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.CABalValidate', primaryView: 'PeriodsFilter' })
export class CA503000 extends PXScreen {
	PeriodsFilter = createSingle(CABalanceValidationPeriodFilter);
	CABalValidateList = createCollection(CashAccount);
}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class CashAccount extends PXView {
	@columnConfig({ allowCheckAll: true, allowNull: false, allowSort: false })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CashAccountCD: PXFieldState;

	Descr: PXFieldState;
}

export class CABalanceValidationPeriodFilter extends PXView {
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}

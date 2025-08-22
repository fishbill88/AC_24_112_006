import {
	PXScreen, PXView,
	PXFieldState, PXFieldOptions,
	PXActionState,
	createCollection, createSingle, gridConfig, graphInfo, columnConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.CCUpdateExpirationDatesProcess', primaryView: 'Filter' })
export class CA208000 extends PXScreen {
	Filter = createSingle(Filter);
	CustomerPaymentMethods = createCollection(CustomerPaymentMethod);
}

export class Filter extends PXView {
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ allowImport: false })
export class CustomerPaymentMethod extends PXView {
	@columnConfig({ allowCheckAll: true, allowSort: false })
	Selected: PXFieldState;

	BAccountID: PXFieldState;

	CashAccountID: PXFieldState;

	Descr: PXFieldState;

	ExpirationDate: PXFieldState;
}

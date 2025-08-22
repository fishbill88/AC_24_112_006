import {
	PXScreen, createSingle, createCollection, graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.PaymentMethodConverter', primaryView: 'filter' })
export class CA207000 extends PXScreen {
	filter = createSingle(Filter);
	CustomerPaymentMethodList = createCollection(CustomerPaymentMethod);
}

export class Filter extends PXView {
	OldPaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	OldCCProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewCCProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessExpiredCards: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CustomerPaymentMethod extends PXView {
	Selected: PXFieldState;
	PaymentMethodID: PXFieldState;
	BAccountID: PXFieldState;
	Descr: PXFieldState;
	IsActive: PXFieldState;
	CCProcessingCenterID: PXFieldState;
}

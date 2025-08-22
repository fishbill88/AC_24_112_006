import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, PXActionState, createCollection, columnConfig, linkCommand, gridConfig, viewInfo
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.CCSynchronizeCards', primaryView: 'Filter' })
export class CA206000 extends PXScreen {
	Filter = createSingle(CreditCardsFilter);
	CustomerCardPaymentData = createCollection(CCSynchronizeCard);

	@viewInfo({containerName: 'Select Payment Method'})
	PMFilter = createSingle(CreditCardsFilter);

	@viewInfo({containerName: 'Multiple Customer Payment Profiles'})
	CustPaymentProfileForDialog = createCollection(CustomerPaymentProfile);

	ViewCustomer: PXActionState;
}

@gridConfig({ syncPosition: true, adjustPageSize: true })
export class CCSynchronizeCard extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@linkCommand('ViewCustomer')
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	BAccountID_Customer_acctName: PXFieldState;

	@columnConfig({ hideViewLink: true })
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentType: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerCCPID: PXFieldState;
	PCCustomerID: PXFieldState;
	PCCustomerDescription: PXFieldState;
	PCCustomerEmail: PXFieldState;
	PaymentCCPID: PXFieldState;
	FirstName: PXFieldState;
	LastName: PXFieldState;
}

export class CreditCardsFilter extends PXView {
	ScheduledServiceSync: PXFieldState<PXFieldOptions.CommitChanges>;
 	ProcessingCenterId: PXFieldState<PXFieldOptions.CommitChanges>;
 	LoadExpiredCards: PXFieldState<PXFieldOptions.CommitChanges>;

	EftPaymentMethodId: PXFieldState<PXFieldOptions.CommitChanges>;
	OverwriteEftPaymentMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	CCPaymentMethodId: PXFieldState<PXFieldOptions.CommitChanges>;
	OverwriteCCPaymentMethod: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CustomerPaymentProfile extends PXView  {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	CustomerCCPID: PXFieldState;
	PCCustomerID: PXFieldState;
	PCCustomerDescription: PXFieldState;
	PCCustomerEmail: PXFieldState;
	PaymentCCPID: PXFieldState;
	PaymentProfileFirstName: PXFieldState;
	PaymentProfileLastName: PXFieldState;
}

import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, gridConfig, viewInfo
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.CA.CCProcessingCenterMaint', primaryView: 'ProcessingCenter', })
export class CA205000 extends PXScreen {

	@viewInfo({containerName: 'Credit Card Processing Center'})
	ProcessingCenter = createSingle(CCProcessingCenter);

	CurrentProcessingCenter = createSingle(CCProcessingCenter2);
	@viewInfo({containerName: 'Plug-In Parameters'})
	Details = createCollection(CCProcessingCenterDetail);

	@viewInfo({containerName: 'Payment Methods'})
	PaymentMethods = createCollection(CCProcessingCenterPmntMethod);

	@viewInfo({containerName: 'Fees'})
	FeeTypes = createCollection(CCProcessingCenterFeeType);

	@viewInfo({containerName: 'Payment Links'})
	ProcCenterBranch = createCollection(CCProcessingCenterBranch);

	@viewInfo({containerName: 'CashAccount'})
	CashAccount = createSingle(CashAccount);
}

// Views

export class CCProcessingCenter extends PXView  {
	ProcessingCenterID: PXFieldState;
	Name: PXFieldState;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingTypeName: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowDirectInput: PXFieldState;
	AllowSaveProfile: PXFieldState;
	SyncronizeDeletion: PXFieldState;
	UseAcceptPaymentForm: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowUnlinkedRefund: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowPayLink: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CCProcessingCenter2 extends PXView  {
	OpenTranTimeout: PXFieldState;
	SyncRetryAttemptsNo: PXFieldState;
	SyncRetryDelayMs: PXFieldState;
	CreateAdditionalCustomerProfiles: PXFieldState<PXFieldOptions.CommitChanges>;
	CreditCardLimit: PXFieldState;
	ReauthRetryNbr: PXFieldState;
	ReauthRetryDelay: PXFieldState;
	ImportSettlementBatches: PXFieldState<PXFieldOptions.CommitChanges>;
	ImportStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	LastSettlementDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DepositAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoCreateBankDeposit: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowPartialPayment: PXFieldState<PXFieldOptions.CommitChanges>;
	WebhookID: PXFieldState;
}

export class CCProcessingCenterDetail extends PXView  {
	DetailID: PXFieldState;
	Descr: PXFieldState;
	Value: PXFieldState;
}

export class CCProcessingCenterPmntMethod extends PXView  {
	PaymentMethodID: PXFieldState;
	IsActive: PXFieldState;
	IsDefault: PXFieldState;
	FundHoldPeriod: PXFieldState;
	ReauthDelay: PXFieldState;
}

export class CCProcessingCenterFeeType extends PXView  {
	FeeType: PXFieldState;
	EntryTypeID: PXFieldState;
}

@gridConfig({syncPosition: true})
export class CCProcessingCenterBranch extends PXView  {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultForBranch: PXFieldState;
	CCPaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CCCashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	EFTPaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	EFTCashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CashAccount extends PXView  {
	CuryID: PXFieldState;
}
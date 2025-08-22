import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnShowHideMode } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.CA.CashAccountMaint', primaryView: 'CashAccount', showUDFIndicator: true })
export class CA202000 extends PXScreen {

	@viewInfo({containerName: 'Specify New ID'})
	ChangeIDDialog = createSingle(ChangeIDParam);

	@viewInfo({containerName: 'Cash Account Summary'})
	CashAccount = createSingle(CashAccount);

	CurrentCashAccount = createSingle(CashAccount2);

	@viewInfo({containerName: 'Payment Methods'})
	Details = createCollection(PaymentMethodAccount);

	@viewInfo({containerName: 'Clearing Accounts'})
	Deposits = createCollection(CashAccountDeposit);

	@viewInfo({containerName: 'Entry Types'})
	ETDetails = createCollection(CashAccountETDetail);

	@viewInfo({containerName: 'Payment Method'})
	PaymentMethodForRemittance = createCollection(PaymentMethodAccount2);

	@viewInfo({containerName: 'Remittance Details'})
	PaymentDetails = createCollection(CashAccountPaymentMethodDetail);
}

// Views

export class ChangeIDParam extends PXView  {
	CD: PXFieldState;
}

export class CashAccount extends PXView  {
	CashAccountCD: PXFieldState;
	Active: PXFieldState;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.Disabled>;
	CuryRateTypeID: PXFieldState;
	ExtRefNbr: PXFieldState;
	Descr: PXFieldState;
	ClearingAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	Reconcile: PXFieldState<PXFieldOptions.CommitChanges>;
	RestrictVisibilityWithBranch: PXFieldState;
	MatchToBatch: PXFieldState;
	UseForCorpCard: PXFieldState;
	ReconNumberingID: PXFieldState;
	ReferenceID: PXFieldState;
	StatementImportTypeName: PXFieldState;
	AcctSettingsAllowed: PXFieldState<PXFieldOptions.Disabled>;
	PTInstancesAllowed: PXFieldState<PXFieldOptions.Disabled>;
}

export class CashAccount2 extends PXView  {
	SignatureDescr: PXFieldState;
	Signature: PXFieldState;
}

@gridConfig({initNewRow: true})
export class PaymentMethodAccount extends PXView  {
	PaymentMethodID: PXFieldState;
	UseForAP: PXFieldState;
	UseForPR: PXFieldState;
	APIsDefault: PXFieldState;
	APAutoNextNbr: PXFieldState;
	APLastRefNbr: PXFieldState;
	APBatchLastRefNbr: PXFieldState;
	APQuickBatchGeneration: PXFieldState;
	UseForAR: PXFieldState;
	ARIsDefault: PXFieldState;
	ARIsDefaultForRefund: PXFieldState;
	ARAutoNextNbr: PXFieldState;
	ARLastRefNbr: PXFieldState;
}

export class CashAccountDeposit extends PXView  {
	DepositAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	ChargeEntryTypeID: PXFieldState;
	ChargeRate: PXFieldState;
}

export class CashAccountETDetail extends PXView  {
	@columnConfig({allowShowHide: GridColumnShowHideMode.False})	CashAccountID: PXFieldState<PXFieldOptions.Hidden>;
	EntryTypeID: PXFieldState;

	CAEntryType__DrCr: PXFieldState;
	CAEntryType__Module: PXFieldState;

	@columnConfig({hideViewLink: true})
	CAEntryType__BranchID: PXFieldState;

	@columnConfig({hideViewLink: true})
	CAEntryType__AccountID: PXFieldState;

	@columnConfig({hideViewLink: true})
	CAEntryType__SubID: PXFieldState;

	@columnConfig({hideViewLink: true})
	CAEntryType__CashAccountID: PXFieldState;

	CAEntryType__ReferenceID: PXFieldState;

	CAEntryType__Descr: PXFieldState;

	@columnConfig({hideViewLink: true})
	CAEntryType__UseToReclassifyPayments: PXFieldState;

	@columnConfig({hideViewLink: true})
	OffsetCashAccountID: PXFieldState;

	@columnConfig({hideViewLink: true})
	OffsetBranchID: PXFieldState;

	@columnConfig({hideViewLink: true})
	OffsetAccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({hideViewLink: true})
	OffsetSubID: PXFieldState;

	@columnConfig({hideViewLink: true})
	TaxZoneID: PXFieldState;
	TaxCalcMode: PXFieldState;
	IsDefault: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	autoRepaint: ["PaymentDetails"],
})
export class PaymentMethodAccount2 extends PXView  {
	@columnConfig({ hideViewLink: true })
	PaymentMethodID: PXFieldState;
}

export class CashAccountPaymentMethodDetail extends PXView  {
	PaymentMethodID: PXFieldState;
	DetailID: PXFieldState;
	PaymentMethodDetail__descr: PXFieldState;

	@columnConfig({allowShowHide: GridColumnShowHideMode.False})
	DetailValue: PXFieldState;
}

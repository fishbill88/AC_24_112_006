import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, selectorSettings, ICurrencyInfo } from "client-controls";

@graphInfo({graphType: "PX.Objects.CA.CAReconEntry", primaryView: "CAReconRecords", showUDFIndicator: true })
export class CA302000 extends PXScreen {

	@viewInfo({containerName: "Quick Transaction"})
	AddFilter = createSingle(AddTrxFilter);
	@viewInfo({containerName: "Reconciliation Summary"})
	CAReconRecords = createSingle(CARecon);
	@viewInfo({containerName: "Reconciliation Details"})
	CAReconTranRecords = createCollection(CATran);
	@viewInfo({containerName: "_AddTrxFilter_CurrencyInfo_"})
	_AddTrxFilter_CurrencyInfo_ = createSingle(CurrencyInfo);
}

// Views

export class AddTrxFilter extends PXView {
	CashAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	EntryTypeID : PXFieldState<PXFieldOptions.CommitChanges>;
	TranDate : PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID : PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	ReferenceID : PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID : PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID : PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID : PXFieldState;
	CuryTranAmt : PXFieldState;
	CuryID : PXFieldState;
	Descr : PXFieldState;
	DrCr : PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	OrigModule : PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	Status : PXFieldState<PXFieldOptions.Disabled>;
	Hold : PXFieldState<PXFieldOptions.CommitChanges>;
	Cleared : PXFieldState;
	AccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	SubID : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CARecon extends PXView {
	CashAccountID : PXFieldState;
	CuryID : PXFieldState<PXFieldOptions.Disabled>;

	@selectorSettings("ReconNbr", "")
	ReconNbr : PXFieldState;

	ReconDate : PXFieldState<PXFieldOptions.CommitChanges>;
	Status : PXFieldState<PXFieldOptions.Disabled>;
	CashAccountID_Description : PXFieldState;
	LastReconDate : PXFieldState<PXFieldOptions.Disabled>;
	LoadDocumentsTill : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryBegBalance : PXFieldState<PXFieldOptions.Disabled>;
	CuryReconciledDebits : PXFieldState<PXFieldOptions.Disabled>;
	CuryReconciledCredits : PXFieldState<PXFieldOptions.Disabled>;
	CuryReconciledBalance : PXFieldState<PXFieldOptions.Disabled>;
	CuryBalance : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDiffBalance : PXFieldState<PXFieldOptions.Disabled>;
	CountDebit : PXFieldState<PXFieldOptions.Disabled>;
	CountCredit : PXFieldState<PXFieldOptions.Disabled>;
	SkipVoided : PXFieldState<PXFieldOptions.Disabled>;
	ShowBatchPayments : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	initNewRow: true,
})
export class CATran extends PXView {

	Reconciled : PXFieldState<PXFieldOptions.CommitChanges>;
	Cleared : PXFieldState;
	ClearDate : PXFieldState;
	CuryEffDebitAmt : PXFieldState;
	CuryEffCreditAmt : PXFieldState;
	ExtRefNbr : PXFieldState;
	OrigModule : PXFieldState;
	OrigTranTypeUI : PXFieldState;
	OrigRefNbr : PXFieldState;

	@columnConfig({allowUpdate: false})
	Status : PXFieldState;

	TranDate : PXFieldState;
	BAccountR__acctCD : PXFieldState;
	BAccountR__AcctName : PXFieldState;
	TranDesc : PXFieldState;

	// Actions:
	ToggleReconciled : PXActionState;
	ToggleCleared : PXActionState;
	ReconcileProcessed : PXActionState;
	CreateAdjustment : PXActionState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID : PXFieldState;
	BaseCuryID : PXFieldState;
	BaseCalc : PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID : PXFieldState;
	CuryRateTypeID : PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision : PXFieldState;
	CuryRate : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate : PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate : PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate : PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate : PXFieldState<PXFieldOptions.CommitChanges>;
}

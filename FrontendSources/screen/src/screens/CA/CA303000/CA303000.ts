import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, ICurrencyInfo, PXFieldOptions, linkCommand } from "client-controls";

@graphInfo({graphType: "PX.Objects.CA.CATranEnq", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues, showUDFIndicator: true })
export class CA303000 extends PXScreen {

	@viewInfo({containerName: "Quick Transaction"})
	AddFilter = createSingle(AddTrxFilter);
	@viewInfo({containerName: "Selection"})
	Filter = createSingle(CAEnqFilter);
	@viewInfo({containerName: "Cash Transactions"})
	CATranListRecords = createCollection(CATran);
	@viewInfo({containerName: "_AddTrxFilter_CurrencyInfo_"})
	_AddTrxFilter_CurrencyInfo_ = createSingle(CurrencyInfo);

	ViewDoc : PXActionState;
	ViewBatch : PXActionState;
	ViewBatchPayment : PXActionState;
	ViewRecon : PXActionState;
}

// Views

export class AddTrxFilter extends PXView  {
	CashAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID : PXFieldState<PXFieldOptions.CommitChanges>;
	EntryTypeID : PXFieldState<PXFieldOptions.CommitChanges>;
	TranDate : PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID : PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	ReferenceID : PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID : PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID : PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID : PXFieldState;
	CuryTranAmt : PXFieldState;
	Descr : PXFieldState;
	DrCr : PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	OrigModule : PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	Status : PXFieldState<PXFieldOptions.Disabled>;
	Hold : PXFieldState<PXFieldOptions.CommitChanges>;
	Cleared : PXFieldState;
	AccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	SubID : PXFieldState<PXFieldOptions.CommitChanges>;

	// Actions :
	releaseTransaction : PXActionState;
}

export class CAEnqFilter extends PXView  {
	AccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate : PXFieldState<PXFieldOptions.CommitChanges>;
	ShowSummary : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleased : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID : PXFieldState<PXFieldOptions.Disabled>;
	BegBal : PXFieldState<PXFieldOptions.Disabled>;
	DebitTotal : PXFieldState<PXFieldOptions.Disabled>;
	CreditTotal : PXFieldState<PXFieldOptions.Disabled>;
	EndBal : PXFieldState<PXFieldOptions.Disabled>;
	BegClearedBal : PXFieldState<PXFieldOptions.Disabled>;
	DebitClearedTotal : PXFieldState<PXFieldOptions.Disabled>;
	CreditClearedTotal : PXFieldState<PXFieldOptions.Disabled>;
	EndClearedBal : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	syncPosition: true
})
export class CATran extends PXView  {
	Selected : PXFieldState;
	TranDate : PXFieldState;
	FinPeriodID : PXFieldState;
	DayDesc : PXFieldState;
	OrigModule : PXFieldState;

	@linkCommand("ViewDoc")
	OrigRefNbr : PXFieldState;

	ExtRefNbr : PXFieldState;
	OrigTranTypeUI : PXFieldState;

	@linkCommand("ViewBatch")
	BatchNbr : PXFieldState;

	Status : PXFieldState;
	CuryDebitAmt : PXFieldState;
	CuryCreditAmt : PXFieldState;
	EndBal : PXFieldState;

	@linkCommand("ViewBatchPayment")
	BatchPaymentRefNbr : PXFieldState;

	Cleared : PXFieldState;
	ClearDate : PXFieldState;
	Reconciled : PXFieldState;
	DepositNbr : PXFieldState;
	TranDesc : PXFieldState;
	BAccountR__AcctCD : PXFieldState;
	BAccountR__AcctName : PXFieldState;

	@linkCommand("ViewRecon")
	ReconNbr : PXFieldState;

	CuryClearedCreditAmt : PXFieldState;
	ReferenceID : PXFieldState;

	// Actions:
	AddDet : PXActionState;
	DoubleClick : PXActionState;
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

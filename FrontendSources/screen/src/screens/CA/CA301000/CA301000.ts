import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, PXView, PXFieldState, gridConfig, linkCommand, ICurrencyInfo, PXFieldOptions, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign } from "client-controls";

@graphInfo({graphType: "PX.Objects.CA.CashTransferEntry", primaryView: "Transfer", showUDFIndicator: true })
export class CA301000 extends PXScreen {

	@viewInfo({containerName: "Transfer summary"})
	Transfer = createSingle(CATransfer);

	@viewInfo({containerName: "Additional Charges"})
	Expenses = createCollection(CAExpense);

	@viewInfo({containerName: "Expense Taxes"})
	ExpenseTaxTrans = createCollection(TaxTran);

	@viewInfo({containerName: "currencyinfoout"})
	CurrencyInfoOut = createSingle(CurrencyInfo);

	@viewInfo({containerName: "currencyinfo"})
	CurrencyInfo = createSingle(CurrencyInfo);

	CAReversingTransfers: PXActionState;

	ViewExpenseTaxes: PXActionState;
	viewExpenseBatch: PXActionState;
	viewDoc: PXActionState;
}

// Views

export class CATransfer extends PXView {

	TransferNbr : PXFieldState;
	Status : PXFieldState<PXFieldOptions.Disabled>;
	OrigTransferNbr : PXFieldState<PXFieldOptions.Disabled>;

	@linkCommand("CAReversingTransfers")
	ReverseCount : PXFieldState<PXFieldOptions.Disabled>;

	Descr : PXFieldState;
	RGOLAmt : PXFieldState<PXFieldOptions.Disabled>;
	OutAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	OutDate : PXFieldState<PXFieldOptions.CommitChanges>;
	OutPeriodID : PXFieldState<PXFieldOptions.CommitChanges>;
	ClearedOut : PXFieldState<PXFieldOptions.CommitChanges>;
	OutExtRefNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTranOut : PXFieldState<PXFieldOptions.CommitChanges>;
	TranIDOut_CATran_batchNbr : PXFieldState<PXFieldOptions.Disabled>;
	ClearDateOut : PXFieldState;
	OutGLBalance : PXFieldState<PXFieldOptions.Disabled>;
	CashBalanceOut : PXFieldState<PXFieldOptions.Disabled>;
	InCuryID: PXFieldState<PXFieldOptions.Disabled>;
	OutCuryID: PXFieldState<PXFieldOptions.Disabled>;
	TranOut : PXFieldState<PXFieldOptions.Disabled>;
	InAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	InDate : PXFieldState<PXFieldOptions.CommitChanges>;
	InPeriodID : PXFieldState<PXFieldOptions.CommitChanges>;
	ClearedIn : PXFieldState<PXFieldOptions.CommitChanges>;
	InExtRefNbr : PXFieldState;
	CuryTranIn : PXFieldState<PXFieldOptions.CommitChanges>;
	TranIDIn_CATran_batchNbr : PXFieldState<PXFieldOptions.Disabled>;
	ClearDateIn : PXFieldState;
	InGLBalance : PXFieldState<PXFieldOptions.Disabled>;
	CashBalanceIn : PXFieldState<PXFieldOptions.Disabled>;
	TranIn : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true
})
export class CAExpense extends PXView {

	@columnConfig({ hideViewLink: true })
	CashAccountID : PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	EntryTypeID : PXFieldState<PXFieldOptions.CommitChanges>;

	TranDesc : PXFieldState;

	CuryTaxableAmt : PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ViewExpenseTaxes")
	CuryTaxTotal : PXFieldState<PXFieldOptions.CommitChanges>;

	CuryTranAmt : PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	CuryID : PXFieldState;

	AdjCuryRate : PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr : PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID : PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	SubID : PXFieldState<PXFieldOptions.CommitChanges>;

	TranDate : PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	FinPeriodID : PXFieldState<PXFieldOptions.CommitChanges>;

	TaxZoneID : PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID : PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode : PXFieldState<PXFieldOptions.CommitChanges>;

	Cleared : PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("viewExpenseBatch")
	batchNbr : PXFieldState;

	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})
	@linkCommand("viewDoc")
	AdjRefNbr : PXFieldState;

	CuryTaxRoundDiff : PXFieldState;
}

export class TaxTran extends PXView {
	TaxID : PXFieldState;
	TaxRate : PXFieldState;
	CuryTaxableAmt : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTaxAmt : PXFieldState<PXFieldOptions.CommitChanges>;
	NonDeductibleTaxRate : PXFieldState;
	CuryExpenseAmt : PXFieldState<PXFieldOptions.CommitChanges>;
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

import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, linkCommand, PXActionState, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AR.ARCustomerBalanceEnq', primaryView: 'Filter' })
export class AR401000 extends PXScreen {

	viewDetails: PXActionState;

	Filter = createSingle(ARHistoryFilter);
	Summary = createSingle(ARHistorySummary);
	History = createCollection(ARHistoryResult);
}

export class ARHistoryFilter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	Period: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ARSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SplitByCurrency: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowWithBalanceOnly: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMasterCalendar: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeChildAccounts: PXFieldState<PXFieldOptions.CommitChanges>;
}


export class ARHistorySummary extends PXView {

	BalanceSummary: PXFieldState;
	DepositsSummary: PXFieldState;
	RevaluedSummary: PXFieldState;
	CuryBalanceSummary: PXFieldState;
	CuryDepositsSummary: PXFieldState;
	BalanceRetainedSummary: PXFieldState;
	CuryBalanceRetainedSummary: PXFieldState;
}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar' })
export class ARHistoryResult extends PXView {

	@linkCommand("viewDetails")
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	FinPeriodID: PXFieldState;
	CuryID: PXFieldState;
	CuryBegBalance: PXFieldState;
	CuryEndBalance: PXFieldState;
	CuryBalance: PXFieldState;
	CuryDepositsBalance: PXFieldState;
	CurySales: PXFieldState;
	CuryPayments: PXFieldState;
	CuryBegRetainedBalance: PXFieldState;
	CuryEndRetainedBalance: PXFieldState;
	CuryRetainageWithheld: PXFieldState;
	CuryRetainageReleased: PXFieldState;
	CuryFinCharges: PXFieldState;
	CuryDiscount: PXFieldState;
	CuryCrAdjustments: PXFieldState;
	CuryDrAdjustments: PXFieldState;
	CuryDeposits: PXFieldState;
	BegBalance: PXFieldState;
	EndBalance: PXFieldState;
	Balance: PXFieldState;
	DepositsBalance: PXFieldState;
	Sales: PXFieldState;
	Payments: PXFieldState;
	BegRetainedBalance: PXFieldState;
	EndRetainedBalance: PXFieldState;
	RetainageWithheld: PXFieldState;
	RetainageReleased: PXFieldState;
	FinCharges: PXFieldState;
	Discount: PXFieldState;
	RGOL: PXFieldState;
	CrAdjustments: PXFieldState;
	DrAdjustments: PXFieldState;
	FinPtdRevaluated: PXFieldState;
	Deposits: PXFieldState;
}

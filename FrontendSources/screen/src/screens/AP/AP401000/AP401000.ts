import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, linkCommand, gridConfig, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APVendorBalanceEnq', primaryView: 'Filter' })
export class AP401000 extends PXScreen {

	viewDetails: PXActionState;

	Filter = createSingle(APHistoryFilter);
	Summary = createSingle(APHistorySummary);
	History = createCollection(APHistoryResult);

}

export class APHistoryFilter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;

	VendorClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SplitByCurrency: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowWithBalanceOnly: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMasterCalendar: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class APHistorySummary extends PXView {

	BalanceSummary: PXFieldState;
	DepositsSummary: PXFieldState;
	CuryBalanceSummary: PXFieldState;
	CuryDepositsSummary: PXFieldState;
	BalanceRetainedSummary: PXFieldState;
	CuryBalanceRetainedSummary: PXFieldState;

}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar', syncPosition: true })
export class APHistoryResult extends PXView {

	@linkCommand("viewDetails")
	AcctCD: PXFieldState;

	AcctName: PXFieldState;
	FinPeriodID: PXFieldState;
	CuryID: PXFieldState;
	CuryBegBalance: PXFieldState;
	CuryEndBalance: PXFieldState;
	CuryDepositsBalance: PXFieldState;
	CuryPurchases: PXFieldState;
	CuryPayments: PXFieldState;
	CuryBegRetainedBalance: PXFieldState;
	CuryEndRetainedBalance: PXFieldState;
	CuryRetainageWithheld: PXFieldState;
	CuryRetainageReleased: PXFieldState;
	CuryDiscount: PXFieldState;
	CuryWhTax: PXFieldState;
	CuryCrAdjustments: PXFieldState;
	CuryDrAdjustments: PXFieldState;
	CuryDeposits: PXFieldState;
	BegBalance: PXFieldState;
	EndBalance: PXFieldState;
	DepositsBalance: PXFieldState;
	Purchases: PXFieldState;
	Payments: PXFieldState;
	BegRetainedBalance: PXFieldState;
	EndRetainedBalance: PXFieldState;
	RetainageWithheld: PXFieldState;
	RetainageReleased: PXFieldState;
	Discount: PXFieldState;
	WhTax: PXFieldState;
	RGOL: PXFieldState;
	CrAdjustments: PXFieldState;
	DrAdjustments: PXFieldState;
	Deposits: PXFieldState;

}

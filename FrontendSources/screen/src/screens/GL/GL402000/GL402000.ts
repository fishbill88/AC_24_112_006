import {
	PXScreen, createSingle, createCollection, graphInfo, localizable, PXActionState, PXView, PXFieldState, PXFieldOptions,
	GridPagerMode, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.AccountHistoryByYearEnq', primaryView: 'Filter' })
export class GL402000 extends PXScreen {
	Filter = createSingle(Filter);

	EnqResult = createCollection(EnqResult);
}

export class Filter extends PXView {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinYear: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowCuryDetail: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMasterCalendar: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar', syncPosition: true })
export class EnqResult extends PXView {
	LastActivityPeriod: PXFieldState;
	SignBegBalance: PXFieldState;
	PtdDebitTotal: PXFieldState;
	PtdCreditTotal: PXFieldState;
	SignEndBalance: PXFieldState;
	CuryID: PXFieldState;
	SignCuryBegBalance: PXFieldState;
	CuryPtdDebitTotal: PXFieldState;
	CuryPtdCreditTotal: PXFieldState;
	SignCuryEndBalance: PXFieldState;
	PtdSaldo: PXFieldState;
	CuryPtdSaldo: PXFieldState;
}

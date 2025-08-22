import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig,
	GridColumnShowHideMode, GridPagerMode, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.AccountHistoryBySubEnq', primaryView: 'Filter' })
export class GL403000 extends PXScreen {
	Filter = createSingle(Filter);

	EnqResult = createCollection(EnqResult);
}

export class Filter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;

	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowCuryDetail: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMasterCalendar: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar', syncPosition: true })
export class EnqResult extends PXView {

	SubCD: PXFieldState;
	SignBegBalance: PXFieldState;
	PtdDebitTotal: PXFieldState;
	PtdCreditTotal: PXFieldState;
	SignEndBalance: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryID: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	SignCuryBegBalance: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryPtdDebitTotal: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryPtdCreditTotal: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	SignCuryEndBalance: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	PtdSaldo: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryPtdSaldo: PXFieldState;

}

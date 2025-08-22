import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig,
	GridColumnShowHideMode, linkCommand, GridPagerMode, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.AccountByPeriodEnq', primaryView: 'Filter' })
export class GL404000 extends PXScreen {
	Filter = createSingle(Filter);

	GLTranEnq = createCollection(GLTranEnq);
}

export class Filter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	EndPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;

	StartDateUI: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodStartDateUI: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDateUI: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodEndDateUI: PXFieldState<PXFieldOptions.CommitChanges>;

	ShowSummary: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnposted: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleased: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeReclassified: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowCuryDetail: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMasterCalendar: PXFieldState<PXFieldOptions.CommitChanges>;

	BegBal: PXFieldState;
	TurnOver: PXFieldState;
	EndBal: PXFieldState;

}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar', syncPosition: true })
export class GLTranEnq extends PXView {

	@columnConfig({ allowCheckAll: true, allowShowHide: GridColumnShowHideMode.Server })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	Module: PXFieldState;

	@linkCommand("ViewBatch")
	BatchNbr: PXFieldState;

	TranDate: PXFieldState;
	FinPeriodID: PXFieldState;
	TranPeriodID: PXFieldState;
	TranDesc: PXFieldState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

	LineNbr: PXFieldState;
	BranchID: PXFieldState;
	AccountID: PXFieldState;
	SubID: PXFieldState;
	SignBegBalance: PXFieldState;
	DebitAmt: PXFieldState;
	CreditAmt: PXFieldState;
	SignEndBalance: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryID: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	SignCuryBegBalance: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryDebitAmt: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryCreditAmt: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	SignCuryEndBalance: PXFieldState;

	InventoryID: PXFieldState;
	ReferenceID: PXFieldState;
	ReferenceID_BaccountR_AcctName: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	@linkCommand("ViewReclassBatch")
	ReclassBatchNbr: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	IncludedInReclassHistory: PXFieldState; // visible=false

	@linkCommand("ViewPMTran")
	PMTranID: PXFieldState;

}

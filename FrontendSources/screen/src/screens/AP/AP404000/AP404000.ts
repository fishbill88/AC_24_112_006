import {
	PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, graphInfo, PXScreen, createSingle, createCollection, PXActionState
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.AP.APChecksToPrintEnq', primaryView: 'Filter' })
export class AP404000 extends PXScreen {

	ViewCashAccountL: PXActionState;

	Filter = createSingle(DocFilter);
	Documents = createCollection(CheckSummary);

}

export class DocFilter extends PXView {
	PayTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Balance: PXFieldState<PXFieldOptions.Disabled>;
	CuryBalance: PXFieldState<PXFieldOptions.Disabled>;
	CuryID: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class CheckSummary extends PXView {
	@columnConfig({ allowUpdate: false })
	BranchID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PayTypeID: PXFieldState;
	@linkCommand('ViewCashAccount')
	@columnConfig({ allowUpdate: false })
	PayAccountID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PayAccountID_CashAccount_Descr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocCount: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryDocBal: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.False })
	DocBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OverdueDocCount: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.False })
	OverdueDocBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OverdueCuryDocBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	MinPayDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	MaxPayDate: PXFieldState;
}

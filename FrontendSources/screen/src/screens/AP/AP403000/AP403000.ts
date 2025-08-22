import {
	PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, graphInfo, PXScreen, createSingle, createCollection, PXActionState
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.AP.APPendingInvoicesEnq', primaryView: 'Filter' })
export class AP403000 extends PXScreen {

	ProcessPayment: PXActionState; // to hide action from toolbars

	Filter = createSingle(PendingInvoiceFilter);
	Documents = createCollection(PendingPaymentSummary);

}

export class PendingInvoiceFilter extends PXView {
	PayAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.Disabled>;
	Balance: PXFieldState<PXFieldOptions.Disabled>;
	CuryBalance: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class PendingPaymentSummary extends PXView {

	@columnConfig({ allowUpdate: false })
	BranchID: PXFieldState;
	@linkCommand('ProcessPayment')
	@columnConfig({ allowUpdate: false })
	PayAccountID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PayAccountID_CashAccount_Descr: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	PayTypeID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocCount: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryDocBal: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.Server })
	DocBal: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.Server })
	CuryDiscBal: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.Server })
	DiscBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OverdueDocCount: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.Server })
	OverdueDocBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OverdueCuryDocBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ValidDiscCount: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.Server })
	ValidDiscBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ValidCuryDiscBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	LostDiscCount: PXFieldState;
	@columnConfig({ allowUpdate: false, allowShowHide: GridColumnShowHideMode.Server })
	LostDiscBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	LostCuryDiscBal: PXFieldState;
	@columnConfig({ allowUpdate: false })
	MinPayDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	MaxPayDate: PXFieldState;
}

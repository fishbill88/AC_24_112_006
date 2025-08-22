import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState,
	graphInfo, viewInfo, gridConfig, columnConfig,
	PXFieldOptions
} from "client-controls";

@graphInfo({graphType: "PX.Objects.CA.CashForecastEntry", primaryView: "filterCashAccounts", showUDFIndicator: true })
export class CA305500 extends PXScreen {
   	@viewInfo({containerName: "Selection"})
	filterCashAccounts = createSingle(CashAccount);

   	@viewInfo({containerName: "Selection"})
	filter = createSingle(Filter);

   	@viewInfo({containerName: "Cash Transactions"})
	cashForecastTrans = createCollection(CashForecastTran);
}

export class CashAccount extends PXView {
	CashAccountCD: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.Disabled>;
}

export class Filter extends PXView {
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true
})
export class CashForecastTran extends PXView {
	TranDate: PXFieldState;

	@columnConfig({ allowNull: false })
	DrCr: PXFieldState;

	TranDesc: PXFieldState;

	@columnConfig({ allowNull: false })
	CuryTranAmt: PXFieldState;
}

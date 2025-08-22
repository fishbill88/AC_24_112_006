import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs,
	PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings,
	PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARSPCommissionProcess", primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.GoLastRecord
})
export class AR505500 extends PXScreen {

	@viewInfo({ containerName: "Commission Period" })
	Filter = createSingle(ARSPCommissionPeriod);

	@viewInfo({ containerName: "Salespersons' Commissions" })
	ToProcess = createCollection(ARSalesPerTran);
}

export class ARSPCommissionPeriod extends PXView {

	CommnPeriodID: PXFieldState<PXFieldOptions.Disabled>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	StartDateUI: PXFieldState<PXFieldOptions.Disabled>;
	EndDateUI: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar"
})
export class ARSalesPerTran extends PXView {

	@columnConfig({ allowSort: false, textAlign: TextAlign.Center, allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SalespersonID: PXFieldState;
	SalespersonID_SalesPerson_descr: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	DocCount: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	CommnblAmt: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	CommnAmt: PXFieldState;

	@columnConfig({ hideViewLink: true, textAlign: TextAlign.Right })
	BaseCuryID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	AveCommnPct: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	MinCommnPct: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	CommnPct: PXFieldState;
}

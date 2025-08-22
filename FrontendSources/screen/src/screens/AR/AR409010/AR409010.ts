import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType,
	RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo,
	disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({ graphType: "ReconciliationTools.ARGLDiscrepancyByAccountEnq", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues })
export class AR409010 extends PXScreen {

	ViewDetails: PXActionState;

	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(DiscrepancyEnqFilter);
	Rows = createCollection(GLTran);
}

export class DiscrepancyEnqFilter extends PXView {

	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodTo: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowOnlyWithDiscrepancy: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalGLAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalXXAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalDiscrepancy: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class GLTran extends PXView {

	AccountID: PXFieldState;
	SubID: PXFieldState;
	FinPeriodID: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	GLTurnover: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	XXTurnover: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	NonXXTrans: PXFieldState;

	@linkCommand("ViewDetails")
	@columnConfig({ textAlign: TextAlign.Right })
	Discrepancy: PXFieldState;
}

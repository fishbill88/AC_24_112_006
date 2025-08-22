import { Messages as SysMessages } from "client-controls/services/messages";
import {
	createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs,
	PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings,
	PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign
} from "client-controls";

@graphInfo({
	graphType: "ReconciliationTools.ARGLDiscrepancyByCustomerEnq", primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues
})
export class AR409020 extends PXScreen {

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(DiscrepancyEnqFilter);
	Rows = createCollection(ARHistoryResult);
}

export class DiscrepancyEnqFilter extends PXView  {

	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodFrom : PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID : PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD : PXFieldState<PXFieldOptions.CommitChanges>;
	ShowOnlyWithDiscrepancy : PXFieldState<PXFieldOptions.CommitChanges>;
	TotalGLAmount : PXFieldState<PXFieldOptions.Disabled>;
	TotalXXAmount : PXFieldState<PXFieldOptions.Disabled>;
	TotalDiscrepancy : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class ARHistoryResult extends PXView  {

	@linkCommand("ViewCustomer")
	AcctCD: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AcctName: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	GLTurnover: PXFieldState;

	@columnConfig({ textAlign: TextAlign.Right })
	XXTurnover: PXFieldState;

	@linkCommand("ViewDetails")
	@columnConfig({ textAlign: TextAlign.Right })
	Discrepancy: PXFieldState;
}

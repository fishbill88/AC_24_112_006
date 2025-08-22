import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, viewInfo, gridConfig, linkCommand,
	PXPageLoadBehavior, PXFieldOptions
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARStatementHistory", primaryView: "Filter", 
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR404000 extends PXScreen {
	ViewDetails: PXActionState;

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(ARStatementHistoryParameters);
	
   	@viewInfo({containerName: "Details"})
	History = createCollection(HistoryResult);
}

export class ARStatementHistoryParameters extends PXView  {
	StatementCycleId : PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeOnDemandStatements : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class HistoryResult extends PXView  {
	@linkCommand("ViewDetails")
	StatementCycleId : PXFieldState;
	
	StatementDate : PXFieldState;
	Descr : PXFieldState;
	NumberOfDocuments : PXFieldState;
	ToPrintCount : PXFieldState;
	PrintedCount : PXFieldState;
	ToEmailCount : PXFieldState;
	EmailedCount : PXFieldState;
	NoActionCount : PXFieldState;
	EmailCompletion : PXFieldState;
	PrintCompletion : PXFieldState;
}

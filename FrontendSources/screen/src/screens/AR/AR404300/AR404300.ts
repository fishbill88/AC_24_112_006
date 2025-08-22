import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, viewInfo, gridConfig, columnConfig, linkCommand,
	GridColumnType, PXFieldOptions, PXPageLoadBehavior
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARStatementDetails", primaryView: "Filter", 
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR404300 extends PXScreen {
	ViewDetails: PXActionState;

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(ARStatementDetailsParameters);
	
   	@viewInfo({containerName: "Details"})
	Details = createCollection(DetailsResult);
}

export class ARStatementDetailsParameters extends PXView  {
	StatementCycleId : PXFieldState<PXFieldOptions.CommitChanges>;
	StatementDate : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class DetailsResult extends PXView  {
	@linkCommand("ViewDetails")	
	@columnConfig({allowUpdate: false})	
	CustomerID : PXFieldState;
	
	CustomerID_BAccountR_acctName : PXFieldState;
	StatementBalance : PXFieldState;
	OverdueBalance: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;

	CuryStatementBalance : PXFieldState;
	CuryOverdueBalance : PXFieldState;
	
	@columnConfig({ type: GridColumnType.CheckBox})	
	UseCurrency : PXFieldState;
	
	@columnConfig({ type: GridColumnType.CheckBox })
	DontPrint: PXFieldState;

	@columnConfig({ type: GridColumnType.CheckBox })
	Printed: PXFieldState;

	@columnConfig({ type: GridColumnType.CheckBox })
	DontEmail: PXFieldState;

	@columnConfig({ type: GridColumnType.CheckBox })
	Emailed: PXFieldState;

	@columnConfig({ type: GridColumnType.CheckBox })
	OnDemand: PXFieldState;
	
	PreparedOn : PXFieldState;
}

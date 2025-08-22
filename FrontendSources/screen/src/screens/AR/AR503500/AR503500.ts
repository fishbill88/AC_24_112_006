import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState,
	graphInfo, viewInfo, gridConfig, columnConfig,
	GridColumnType, PXPageLoadBehavior, PXFieldOptions
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARStatementPrint", primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR503500 extends PXScreen {
   	@viewInfo({containerName: "Selection"})
    Filter = createSingle(PrintParameters);

   	@viewInfo({containerName: "Details"})
	Details = createCollection(DetailsResult);
}

export class PrintParameters extends PXView  {
	Action : PXFieldState<PXFieldOptions.CommitChanges>;
	StatementCycleId : PXFieldState<PXFieldOptions.CommitChanges>;
	StatementDate : PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID : PXFieldState<PXFieldOptions.CommitChanges>;
	OrganizationID : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryStatements : PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAll : PXFieldState<PXFieldOptions.CommitChanges>;
	PrintWithDeviceHub : PXFieldState<PXFieldOptions.CommitChanges>;
	DefinePrinterManually : PXFieldState<PXFieldOptions.CommitChanges>;
	PrinterID : PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCopies : PXFieldState<PXFieldOptions.CommitChanges>;
	StatementMessage: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ["CustomerID", "CustomerID_BAccountR_acctName"]
})
export class DetailsResult extends PXView  {
	@columnConfig({ allowUpdate: false, allowSort: false, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CustomerID: PXFieldState;

	CustomerID_BAccountR_acctName : PXFieldState;
	BranchID: PXFieldState;
	StatementBalance: PXFieldState;
	OverdueBalance: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;

	CuryStatementBalance: PXFieldState;
	CuryOverdueBalance: PXFieldState;

	@columnConfig({ type: GridColumnType.CheckBox })
	UseCurrency: PXFieldState;

	@columnConfig({ type: GridColumnType.CheckBox })
	DontPrint: PXFieldState;

	@columnConfig({ type: GridColumnType.CheckBox })
	Printed: PXFieldState;

	@columnConfig({ type: GridColumnType.CheckBox })
	DontEmail: PXFieldState;

	@columnConfig({ type: GridColumnType.CheckBox })
	Emailed: PXFieldState;
}

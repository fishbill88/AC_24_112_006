import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, viewInfo, gridConfig, columnConfig, linkCommand,
	GridColumnType, PXPageLoadBehavior, PXFieldOptions
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARRetainageRelease", primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR510000 extends PXScreen {
	ViewDocument: PXActionState;

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(ARRetainageFilter);

   	@viewInfo({containerName: "Documents"})
	DocumentList = createCollection(ARInvoice);
}


export class ARRetainageFilter extends PXView {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowBillsWithOpenBalance: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageReleasePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageReleasedAmt: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class ARInvoice extends PXView {
	@columnConfig({ allowUpdate: false, allowSort: false, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	DocType: PXFieldState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;

	ARTranSortOrder: PXFieldState;
	RetainageReleasePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageReleasedAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageUnreleasedCalcAmt: PXFieldState;
	DocDate: PXFieldState;
	CuryOrigDocAmtWithRetainageTotal: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;

	DisplayProjectID: PXFieldState;
	DocDesc: PXFieldState;
	FinPeriodID: PXFieldState;
	InvoiceNbr: PXFieldState;
	ARTranInventoryID: PXFieldState;
	ARTranTaskID: PXFieldState;
	ARTranCostCodeID: PXFieldState;
	ARTranAccountID: PXFieldState;
}

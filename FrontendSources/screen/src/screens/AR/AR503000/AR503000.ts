import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState,
	graphInfo, viewInfo, gridConfig, columnConfig,
	PXPageLoadBehavior, PXFieldOptions, GridColumnType
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARStatementProcess", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR503000 extends PXScreen {
	@viewInfo({ containerName: "Selection" })
	Filter = createSingle(Parameters);

	CyclesList = createCollection(ARStatementCycle);
}

export class Parameters extends PXView {
	StatementDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ["StatementCycleId", "Descr"]
})
export class ARStatementCycle extends PXView {
	@columnConfig({ allowSort: false, allowNull: false, allowCheckAll: true, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

    @columnConfig({ hideViewLink: true })
	StatementCycleId: PXFieldState;

	Descr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LastStmtDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LastFinChrgDate: PXFieldState;

	@columnConfig({ allowNull: false })
	PrepareOn: PXFieldState;

	@columnConfig({ allowUpdate: false })
	NextStmtDate: PXFieldState;
}

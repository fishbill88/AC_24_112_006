import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior,
	PXView, PXFieldState, columnConfig, PXFieldOptions
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.GL.Consolidation.ConsolSourceDataMaint", primaryView: "Filter",
	pageLoadBehavior: PXPageLoadBehavior.GoLastRecord,
	hideFilesIndicator: true,
	hideNotesIndicator: true
})
export class GL509001 extends PXScreen {

	@viewInfo({containerName: "Consolidation Data Parameters"})
	Filter = createSingle(ConsolRecordsFilter);
	@viewInfo({containerName: "Consolidation Data"})
	ConsolRecords = createCollection(GLConsolData);
}

export class ConsolRecordsFilter extends PXView {
	LedgerCD: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchCD: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class GLConsolData extends PXView {

	@columnConfig({ hideViewLink: true })
	AccountCD: PXFieldState;

	MappedValue: PXFieldState;
	MappedValueLength: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	ConsolAmtCredit: PXFieldState;
	ConsolAmtDebit: PXFieldState;
}

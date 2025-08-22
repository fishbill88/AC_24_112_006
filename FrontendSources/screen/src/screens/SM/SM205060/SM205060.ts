import {
	graphInfo,
	PXView,
	PXFieldState,
	gridConfig,
	PXScreen,
	createSingle,
	createCollection,
	PXFieldOptions,
	columnConfig,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: "PX.SM.AUReportProcess", primaryView: "Filter" })
export class SM205060 extends PXScreen {
	Filter = createSingle(Parameters);
	Templates = createCollection(ReportSettings);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	autoAdjustColumns: true,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
})
export class ReportSettings extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;

	Selected: PXFieldState;
	Name: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ScreenID: PXFieldState;
	ScreenID_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	Username: PXFieldState;
	IsDefault: PXFieldState;
	IsShared: PXFieldState;
	MergeReports: PXFieldState<PXFieldOptions.CommitChanges>;
	MergingOrder: PXFieldState;
}

export class Parameters extends PXView {
	ReportID: PXFieldState<PXFieldOptions.CommitChanges>;
	Username: PXFieldState<PXFieldOptions.CommitChanges>;
	My: PXFieldState<PXFieldOptions.CommitChanges>;
}

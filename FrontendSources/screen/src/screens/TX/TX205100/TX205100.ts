import {
	createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnShowHideMode, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.TaxReportMaint', primaryView: 'Report', hideFilesIndicator: true, hideNotesIndicator: true })
export class TX205100 extends PXScreen {

	Report = createSingle(TaxReport);
	ReportLine = createCollection(TaxReportLine);
	Bucket = createCollection(TaxBucket);

}

export class TaxReport extends PXView {

	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	ValidFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowNoTemp: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ initNewRow: true, syncPosition: true })
export class TaxReportLine extends PXView {

	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
	Descr: PXFieldState;
	LineType: PXFieldState;
	LineMult: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	TaxZoneID: PXFieldState;

	TempLine: PXFieldState;
	NetTax: PXFieldState;
	HideReportLine: PXFieldState;
	ReportLineNbr: PXFieldState;
	BucketSum: PXFieldState;

	// Actions
	Up: PXActionState;
	Down: PXActionState;

}

@gridConfig({
	initNewRow: true,
	syncPosition: true,
})
export class TaxBucket extends PXView {

	ViewGroupDetails: PXActionState;

	Name: PXFieldState;
	BucketType: PXFieldState;

}

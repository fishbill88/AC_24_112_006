import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridPreset
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INLotSerClassMaint', primaryView: 'lotserclass'})
export class IN207000 extends PXScreen {

	@viewInfo({containerName: 'Lot/Serial Settings Summary'})
	lotserclass = createSingle(INLotSerClass);
	@viewInfo({containerName: 'Numbering Settings'})
	lotsersegments = createCollection(INLotSerSegment);
}

export class INLotSerClass extends PXView {
	LotSerClassID: PXFieldState;
	Descr: PXFieldState;
	LotSerTrack: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerTrackExpiration: PXFieldState;
	RequiredForDropship: PXFieldState;
	LotSerAssign: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerIssueMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerNumShared: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerNumVal: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoNextNbr: PXFieldState;
	AutoSerialMaxCount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	adjustPageSize: true
})
export class INLotSerSegment extends PXView {
	SegmentID: PXFieldState;
	SegmentType: PXFieldState;
	SegmentValue: PXFieldState;
}
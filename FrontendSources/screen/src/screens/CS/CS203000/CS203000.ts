import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState,
	graphInfo, viewInfo, columnConfig, gridConfig,
	PXFieldOptions, GridColumnShowHideMode
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.CS.SegmentMaint', primaryView: 'Segment' })
export class CS203000 extends PXScreen {
	@viewInfo({containerName: 'Segment Summary'})
	Segment = createSingle(Segment);

	@viewInfo({containerName: 'Possible Values'})
	Values = createCollection(SegmentValue);
}

export class Segment extends PXView {
	DimensionID: PXFieldState;
	SegmentID: PXFieldState;
	Descr: PXFieldState;
}

@gridConfig({ initNewRow: true })
export class SegmentValue extends PXView {
	Value: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	DimensionID: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	SegmentID: PXFieldState<PXFieldOptions.Hidden>;

	Descr: PXFieldState;
	Active: PXFieldState;
	IsConsolidatedValue: PXFieldState;
	MappedSegValue: PXFieldState;
}

import {
	createCollection, createSingle,
	PXScreen, PXActionState, PXView, PXFieldState,
	PXFieldOptions, GridColumnShowHideMode,
	viewInfo, graphInfo, gridConfig, columnConfig, linkCommand
} from 'client-controls';


@graphInfo({graphType: 'PX.Objects.CS.DimensionMaint', primaryView: 'Header', })
export class CS202000 extends PXScreen {
	@viewInfo({containerName: 'Segmented Key Definition'})
	Header = createSingle(Dimension);

	@viewInfo({containerName: 'Segment Definition'})
	Detail = createCollection(Segment);
}

export class Dimension extends PXView {
	DimensionID: PXFieldState;
	ParentDimensionID: PXFieldState<PXFieldOptions.Disabled>;
	LookupMode: PXFieldState<PXFieldOptions.CommitChanges>;
	Validate: PXFieldState;
	SpecificModule: PXFieldState;
	NumberingID: PXFieldState;
	Descr: PXFieldState;
	MaxLength: PXFieldState<PXFieldOptions.Disabled>;
	Length: PXFieldState<PXFieldOptions.Disabled>;
	Segments: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true })
export class Segment extends PXView {
	viewSegment: PXActionState;

	@linkCommand('viewSegment')
	@columnConfig({ allowUpdate: false })
	SegmentID: PXFieldState;

	Descr: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	Inherited: PXFieldState<PXFieldOptions.Hidden>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	IsOverrideForUI: PXFieldState;

	Length: PXFieldState<PXFieldOptions.CommitChanges>;
	Align: PXFieldState;
	EditMask: PXFieldState;
	CaseConvert: PXFieldState;
	Validate: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	IsCosted: PXFieldState;

	AutoNumber: PXFieldState;
	Separator: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.True })
	PromptCharacter: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ConsolOrder: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ConsolNumChar: PXFieldState;
}

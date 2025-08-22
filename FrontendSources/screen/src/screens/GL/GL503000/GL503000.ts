import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, gridConfig, columnConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.FinPeriodStatusProcess', primaryView: 'Filter' })
export class GL503000 extends PXScreen {

	Filter = createSingle(Filter);
	FinPeriods = createCollection(FinPeriods);

}

export class Filter extends PXView {
	OrganizationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	FromYear: PXFieldState<PXFieldOptions.CommitChanges>;
	ToYear: PXFieldState<PXFieldOptions.CommitChanges>;
	ReopenInSubledgers: PXFieldState;
}

@gridConfig({ adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar', syncPosition: true, suppressNoteFiles: true })
export class FinPeriods extends PXView {

	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	FinPeriodID: PXFieldState;
	Descr: PXFieldState;
	Status: PXFieldState;
	APClosed: PXFieldState;
	ARClosed: PXFieldState;
	INClosed: PXFieldState;
	CAClosed: PXFieldState;
	FAClosed: PXFieldState;
}

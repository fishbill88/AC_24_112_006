import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig, TextAlign
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.GLAccessBySub', primaryView: 'Sub', hideFilesIndicator: true, hideNotesIndicator: true })
export class GL104030 extends PXScreen {

	Sub = createSingle(Sub);
	Groups = createCollection(RelationGroup);
}

export class Sub extends PXView {

	SubCD: PXFieldState;
	Description: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ allowDelete: false, allowInsert: false, allowUpdate: false, adjustPageSize: true })
export class RelationGroup extends PXView {

	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Included: PXFieldState;
	@columnConfig({ allowUpdate: false })
	GroupName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Description: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Active: PXFieldState;
	@columnConfig({ allowUpdate: false })
	GroupType: PXFieldState;
}

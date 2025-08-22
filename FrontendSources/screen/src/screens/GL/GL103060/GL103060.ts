import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig, TextAlign
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.GLBranchAccess', primaryView: 'Group' })
export class GL103060 extends PXScreen {

	Group = createSingle(Group);
	Branch = createCollection(Branch);
	Sub = createCollection(Sub);
	SegmentFilter = createSingle(SegmentFilter);
	Segment = createCollection(Segment);
}

@gridConfig({ allowDelete: false })
export class Group extends PXView {
	GroupName: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupType: PXFieldState<PXFieldOptions.CommitChanges>;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ allowDelete: false, allowUpdate: false, syncPosition: true, quickFilterFields: ['SubCD', 'Description'] })
export class Sub extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubCD: PXFieldState;

	Active: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({ allowDelete: false, allowUpdate: false, syncPosition: true, quickFilterFields: ['Value', 'Descr'] })
export class Segment extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Value: PXFieldState;

	Active: PXFieldState;
	Descr: PXFieldState;
}

@gridConfig({ allowDelete: false, allowInsert: false, quickFilterFields: ['BranchCD', 'AcctName'] })
export class Branch extends PXView {

	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchCD: PXFieldState;

	AcctName: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LedgerID: PXFieldState;
}

export class SegmentFilter extends PXView {
	SegmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	ValidCombos: PXFieldState<PXFieldOptions.CommitChanges>;
}

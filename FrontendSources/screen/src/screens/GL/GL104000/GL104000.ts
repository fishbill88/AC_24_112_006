import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig, TextAlign, GridPagerMode
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.GLAccess', primaryView: 'Group', hideFilesIndicator: true, hideNotesIndicator: true })
export class GL104000 extends PXScreen {

	Group = createSingle(RelationGroup);
	SegmentFilter = createSingle(SegmentFilter);
	Users = createCollection(Users);
	Account = createCollection(Account);
	Sub = createCollection(Sub);
	Segment = createCollection(SegmentValue);

}

@gridConfig({ allowDelete: false })
export class RelationGroup extends PXView {
	GroupName: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	GroupType: PXFieldState<PXFieldOptions.CommitChanges>;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SegmentFilter extends PXView {
	SegmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	ValidCombos: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ allowDelete: false, quickFilterFields: ['Username', 'FullName'], allowImport: false })
export class Users extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Username: PXFieldState;

	FullName: PXFieldState;
	Comment: PXFieldState;
}

@gridConfig({ allowDelete: false, quickFilterFields: ['AccountCD', 'Description'], allowImport: false })
export class Account extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountCD: PXFieldState;

	Type: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountClassID: PXFieldState;

	Active: PXFieldState;
	Description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
}

@gridConfig({ allowDelete: false, syncPosition: true, quickFilterFields: ['SubCD', 'Description'], allowImport: false })
export class Sub extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubCD: PXFieldState;

	Active: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({ allowDelete: false, allowUpdate: false, syncPosition: true, quickFilterFields: ['Value', 'Descr'] })
export class SegmentValue extends PXView {

	@columnConfig({ textAlign: TextAlign.Center, allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ hideViewLink: true })
	Value: PXFieldState;

	Active: PXFieldState;
	Descr: PXFieldState;
}

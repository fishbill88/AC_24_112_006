import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, createCollection, columnConfig, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APAccess', primaryView: 'Group' })
export class AP102000 extends PXScreen {

	Group = createSingle(RelationGroup);
	Users = createCollection(Users);
	Vendor = createCollection(Vendor);

}

export class RelationGroup extends PXView {

	GroupName: PXFieldState;
	Description: PXFieldState;

	@columnConfig({ allowNull: false })
	GroupType: PXFieldState;

	Active: PXFieldState;

}

@gridConfig({ suppressNoteFiles: true })
export class Users extends PXView {

	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	Username: PXFieldState;

	@columnConfig({ allowUpdate: false })
	FullName: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Comment: PXFieldState;

}

@gridConfig({ suppressNoteFiles: true })
export class Vendor extends PXView {

	@columnConfig({ allowCheckAll: true })
	Included: PXFieldState;

	AcctCD: PXFieldState;

	@columnConfig({ allowUpdate: false })
	VStatus: PXFieldState;

	@columnConfig({ allowUpdate: false })
	AcctName: PXFieldState;

}

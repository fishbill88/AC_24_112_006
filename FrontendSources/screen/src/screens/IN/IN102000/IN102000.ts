import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	gridConfig,
	PXFieldState,
	columnConfig
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INAccess', primaryView: 'Group'})
export class IN102000 extends PXScreen {

	@viewInfo({containerName: 'Restriction Group'})
	Group = createSingle(RelationGroup);

	@viewInfo({containerName: 'Users'})
	Users = createCollection(Users);

	@viewInfo({containerName: 'Warehouses'})
	Site = createCollection(INSite);
}

export class RelationGroup extends PXView  {
	GroupName: PXFieldState;
	Description: PXFieldState;
	GroupType: PXFieldState;
	Active: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	adjustPageSize: true,
	suppressNoteFiles: true
})
export class Users extends PXView  {
	@columnConfig({allowCheckAll: true})
	Included: PXFieldState;
	@columnConfig({hideViewLink: true})
	Username: PXFieldState;
	FullName: PXFieldState;
	Comment: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	adjustPageSize: true,
	suppressNoteFiles: true
})
export class INSite extends PXView  {
	@columnConfig({allowCheckAll: true})
	Included: PXFieldState;
	@columnConfig({hideViewLink: true})
	SiteCD: PXFieldState;
	Descr: PXFieldState;
}
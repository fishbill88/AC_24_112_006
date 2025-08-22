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

@graphInfo({graphType: 'PX.Objects.IN.INAccessItem', primaryView: 'Group'})
export class IN103000 extends PXScreen {

	@viewInfo({containerName: 'Restriction Group'})
	Group = createSingle(RelationGroup);

	@viewInfo({containerName: 'Users'})
	Users = createCollection(Users);

	@viewInfo({containerName: 'Item Classes'})
	Class = createCollection(INItemClass);

	@viewInfo({containerName: 'Inventory Items'})
	Item = createCollection(InventoryItem);

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
export class INItemClass extends PXView  {
	@columnConfig({allowCheckAll: true})
	Included: PXFieldState;
	@columnConfig({hideViewLink: true})
	ItemClassCD: PXFieldState;
	Descr: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	adjustPageSize: true,
	suppressNoteFiles: true
})
export class InventoryItem extends PXView  {
	@columnConfig({allowCheckAll: true})
	Included: PXFieldState;
	@columnConfig({hideViewLink: true})
	InventoryCD: PXFieldState;
	Descr: PXFieldState;
}
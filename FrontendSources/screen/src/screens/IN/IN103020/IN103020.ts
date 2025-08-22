import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	gridConfig,
	columnConfig
} from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INAccessDetailByItem', primaryView: 'Item'})
export class IN103020 extends PXScreen {

	@viewInfo({containerName: 'Inventory Item'})
	Item = createSingle(InventoryItem);
	@viewInfo({containerName: 'Restriction Groups'})
	Groups = createCollection(RelationGroup);
}

export class InventoryItem extends PXView  {
	InventoryCD: PXFieldState;
	Descr: PXFieldState;
	ItemClassID: PXFieldState;
	ItemStatus: PXFieldState;
	StkItem: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	adjustPageSize: true
})
export class RelationGroup extends PXView  {
	@columnConfig({allowCheckAll: true })
	Included: PXFieldState;
	@columnConfig({ hideViewLink: true })
	GroupName: PXFieldState;
	Description: PXFieldState;
	Active: PXFieldState;
	GroupType: PXFieldState;
}
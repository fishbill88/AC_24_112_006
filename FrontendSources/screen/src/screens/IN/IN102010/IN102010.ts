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

@graphInfo({graphType: 'PX.Objects.IN.INAccessDetail', primaryView: 'Site'})
export class IN102010 extends PXScreen {

	@viewInfo({containerName: 'Warehouse'})
	Site = createSingle(INSite);
	@viewInfo({containerName: 'Restriction Groups'})
	Groups = createCollection(RelationGroup);
}

export class INSite extends PXView  {
	SiteCD: PXFieldState;
	Descr: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false
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
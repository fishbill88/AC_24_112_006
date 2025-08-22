import { createCollection, createSingle, PXScreen, graphInfo,
	PXView, viewInfo, PXFieldState, PXFieldOptions,
	treeConfig, gridConfig, columnConfig, GridPreset } from "client-controls";

@graphInfo({graphType: "PX.SM.WikiAccessRightsMaintenance", primaryView: "RolesRecords", })
export class SM202015 extends PXScreen {
	@viewInfo({containerName: "Role Information"})
	RolesRecords = createSingle(RolesFilter);
	Folders = createCollection(WikiPage);
	Children = createCollection(WikiPage2);
}

export class RolesFilter extends PXView {
	Rolename : PXFieldState<PXFieldOptions.CommitChanges>;
	Descr : PXFieldState;
	Isinherited : PXFieldState;
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'Folders',
	idParent: 'Key',
	idName: 'PageID',
	description: 'Title',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
})
export class WikiPage extends PXView {
	PageID: PXFieldState;
	Title: PXFieldState;
	Key: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false
})
export class WikiPage2 extends PXView {
	@columnConfig({allowUpdate: false, width: 200})	Name : PXFieldState;
	@columnConfig({allowUpdate: false, width: 200})	Title : PXFieldState;
	@columnConfig({width: 200})	AccessRights : PXFieldState;
	@columnConfig({width: 200})	ParentAccessRights : PXFieldState;
}
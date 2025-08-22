import { createCollection, PXScreen, graphInfo, PXActionState, 
	gridConfig, PXView, PXFieldState, TextAlign,
	GridColumnType, columnConfig, treeConfig, GridPreset } from "client-controls";

@graphInfo({graphType: "PX.SM.WikiPageMapMaintenance", primaryView: "Children", })
export class SM202010 extends PXScreen {
	Folders = createCollection(WikiPage);
	Children = createCollection(WikiPage2);
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
export class WikiPage extends PXView  {
	PageID: PXFieldState;
	Title: PXFieldState;
	Key: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	topBarItems: {
		selectAll: { config: {commandName: "selectAll", text: "Select All"} },
		viewArticle: { config: {commandName: "viewArticle", text: "View Article"} },
		RowUp: { config: {commandName: "RowUp", text: "Up", images: { normal: "main@ArrowUp" } } },
		RowDown: { config: {commandName: "RowDown", text: "Down", images: { normal: "main@ArrowDown" } } },
	}
})
export class WikiPage2 extends PXView  {
	selectAll : PXActionState;
	viewArticle : PXActionState;
	RowUp : PXActionState;
	RowDown : PXActionState;
	@columnConfig({width: 60, textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	Selected : PXFieldState;
	@columnConfig({width: 200})	Name : PXFieldState;
	@columnConfig({width: 200})	Title : PXFieldState;
	@columnConfig({width: 60, textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	Folder : PXFieldState;
	@columnConfig({width: 60})	Number : PXFieldState;
	@columnConfig({width: 60})	WikiID : PXFieldState;
}
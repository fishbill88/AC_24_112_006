import { createCollection, createSingle, PXScreen, graphInfo,
	PXActionState, viewInfo, PXView, gridConfig,
	treeConfig, columnConfig, TextAlign, GridColumnShowHideMode,
	PXFieldState, PXFieldOptions, linkCommand, GridColumnType, GridPreset } from "client-controls";

@graphInfo({graphType: "PX.TM.ImportCompanyTreeMaint", primaryView: "Items", })
export class EP204060 extends PXScreen {
	ViewEmployee: PXActionState;
	Left: PXActionState;
	Right: PXActionState;

	@viewInfo({containerName: "Company Tree"})
	Folders = createCollection(EPCompanyTree);
	@viewInfo({containerName: "List of Groups"})
	Items = createCollection(EPCompanyTree2);
	@viewInfo({containerName: "Group Members"})
	Members = createCollection(EPCompanyTreeMember);
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'Folders',
	idParent: 'Key',
	idName: 'WorkGroupID',
	description: 'Description',
	modifiable: false,
	mode: 'single',
	openedLayers: 1,
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
	hideToolbarSearch: true,
	topBarItems: {
		Left: {index: 0, config: {commandName: "Left", images: { normal: "main@ArrowLeft" } } },
		Right: {index: 1, config: {commandName: "Right", images: { normal: "main@ArrowRight" } } },
	}
})
export class EPCompanyTree extends PXView  {
	WorkGroupID: PXFieldState;
	Description: PXFieldState;
	Key: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	fastFilterByAllFields: false,
	autoRepaint: ["Members"],
	topBarItems: {
		Up: {index: 0, config: {commandName: "Up", text: "Up", images: { normal: "main@ArrowUp" }}},
		Down: {index: 1, config: {commandName: "Down", text: "Down", images: { normal: "main@ArrowDown" }}},
	}
})
export class EPCompanyTree2 extends PXView  {
	Up : PXActionState;
	Down : PXActionState;
	@columnConfig({visible: false, textAlign: TextAlign.Right, allowShowHide: GridColumnShowHideMode.False})	WorkGroupID : PXFieldState<PXFieldOptions.Hidden>;
	Description : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	fastFilterByAllFields: false,
})
export class EPCompanyTreeMember extends PXView  {
	ContactID : PXFieldState;
	@linkCommand("ViewEmployee")
	EPEmployee__acctCD : PXFieldState;
	EPEmployee__acctName : PXFieldState;
	EPEmployeePosition__positionID : PXFieldState;
	EPEmployee__departmentID : PXFieldState;
	@columnConfig({textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	IsOwner : PXFieldState;
	@columnConfig({textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	Active : PXFieldState;
}

import {
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,

	createSingle,
	createCollection,

	graphInfo,
	viewInfo,
	gridConfig,
	columnConfig,

	linkCommand,
	treeConfig,
	PXActionState,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.IN.INCategoryMaint', primaryView: 'SelectedFolders' })
export class IN204060 extends PXScreen {
	@viewInfo({ containerName: "Selected Folders" })
	SelectedFolders = createSingle(SelectedFolders);

	@viewInfo({ containerName: "Folders" })
	Folders = createCollection(Folders);

	@viewInfo({ containerName: "Parent Folders" })
	ParentFolders = createCollection(ParentFolders);

	@viewInfo({ containerName: "Current Category" })
	CurrentCategory = createSingle(CurrentCategory);

	@viewInfo({ containerName: "Members" })
	Members = createCollection(Members);

	@viewInfo({ containerName: "Class Info" })
	ClassInfo = createSingle(ClassInfo);
}

export class SelectedFolders extends PXView {
	FolderID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@treeConfig({
	dynamic: true,
	syncPosition: true,
	hideRootNode: true,
	mode: 'single',
	modifiable: false,
	singleClickSelect: true,
	openedLayers: 1,
	dataMember: "Folders",
	idName: "CategoryID",
	idParent: "ParentID",
	description: "Description",
	topBarItems: {
		AddCategory: { config: { commandName: "AddCategory", images: {normal: "main@RecordAdd"} } },
		up: { config: { commandName: 'Up', text: '', images: { normal: 'main@ArrowUp' } } },
		down: { config: { commandName: 'Down', text: '', images: { normal: 'main@ArrowDown' } } },
		DeleteCategory: {config: {commandName: 'DeleteCategory', images: {normal: 'main@RecordDel' } } } }
})
export class Folders extends PXView {
	Up: PXActionState;
	Down: PXActionState;
	AddCategory: PXActionState;
	DeleteCategory: PXActionState;

	CategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentID: PXFieldState;
	Description: PXFieldState;
}

export class ParentFolders extends PXView {
	CategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentID: PXFieldState;
	Description: PXFieldState;
}

export class CurrentCategory extends PXView {
	Description: PXFieldState<PXFieldOptions.CommitChanges>;
	ParentID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowImport: false,
	suppressNoteFiles: true
})
export class Members extends PXView {
	Copy: PXActionState;
	Cut: PXActionState;
	Paste: PXActionState;
	AddItemsbyClass: PXActionState;

	@columnConfig({ allowCheckAll: true })
	CategorySelected: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("viewDetails")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;

	InventoryItem__Descr: PXFieldState<PXFieldOptions.Disabled>;
	InventoryItem__ItemClassID: PXFieldState<PXFieldOptions.Disabled>;
	InventoryItem__ItemStatus: PXFieldState<PXFieldOptions.Disabled>;
}

export class ClassInfo extends PXView {
	AddItemsTypes: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
}

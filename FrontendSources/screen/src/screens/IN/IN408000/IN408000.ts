import {
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,

	createSingle,
	createCollection,

	graphInfo,
	viewInfo,
	gridConfig,
	treeConfig,
	columnConfig,
	linkCommand,
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.IN.INInventoryByItemClassEnq', primaryView: 'ItemClassFilter' })
export class IN408000 extends PXScreen {
	ViewItem: PXActionState;
	ViewClass: PXActionState;
	GoToNodeSelectedInTree: PXActionState;

	@viewInfo({ containerName: "Tree View and Primary View Synchronization Helper" })
	TreeViewAndPrimaryViewSynchronizationHelper = createSingle(TreeViewAndPrimaryViewSynchronizationHelper);

	@viewInfo({ containerName: "Item Class Filter" })
	ItemClassFilter = createSingle(ItemClassFilter);

	@viewInfo({ containerName: "Item Classes" })
	ItemClasses = createCollection(ItemClasses);

	@viewInfo({ containerName: "Inventory Filter" })
	InventoryFilter = createSingle(InventoryFilter);

	@viewInfo({ containerName: "Inventories" })
	Inventories = createCollection(Inventories);
}

export class ItemClassFilter extends PXView {
	ItemClassCD: PXFieldState<PXFieldOptions.CommitChanges>;
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'ItemClasses',
	idParent: 'ParentItemClassID',
	idName: 'ItemClassID',
	description: 'SegmentedClassCD',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	//actionField: 'GoToNodeSelectedInTree',
	syncPosition: true,
	openedLayers: 0,
})
// AllowCollapse="False"
// AutoRepaint="True"
// PreserveExpanded="True"
// PopulateOnDemand="True"
// AutoCallBackCommand="GoToNodeSelectedInTree"
export class ItemClasses extends PXView {
	ParentItemClassID: PXFieldState;
	ItemClassID: PXFieldState;
	SegmentedClassCD: PXFieldState;
	Descr: PXFieldState;
}

export class InventoryFilter extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>; // autocallback GoToNodeSelectedInTree
	ShowItems: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	suppressNoteFiles: true,
	initNewRow: false,
	batchUpdate: true
})
export class Inventories extends PXView {
	Cut: PXActionState; // displayStyle: image (main@Cut), Tooltip: Cut Selected Inventory Items
	Paste: PXActionState; // displayStyle: image (main@Paste), Tooltip: Paste Inventory Items from Buffer

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ViewItem")
	InventoryCD: PXFieldState<PXFieldOptions.Disabled>;

	Descr: PXFieldState<PXFieldOptions.Disabled>;

	@linkCommand("ViewClass")
	ItemClassID: PXFieldState<PXFieldOptions.Disabled>;

	INItemClass__Descr: PXFieldState<PXFieldOptions.Disabled>;

	ItemStatus: PXFieldState<PXFieldOptions.Disabled>;
}

export class TreeViewAndPrimaryViewSynchronizationHelper extends PXView {
	ItemClassCD: PXFieldState;
}

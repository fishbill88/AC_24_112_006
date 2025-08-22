import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	treeConfig,
	GridPreset,
} from 'client-controls';

export class AsBuiltConfigFilter extends PXView {
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrdNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	LevelsToDisplay: PXFieldState<PXFieldOptions.CommitChanges>;
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'Tree',
	idParent: 'ParentID',
	iconField: 'Icon',
	//toolTipField : 'ToolTip',
	idName: 'ParentID, MatlLine, Level',
	description: 'Label',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
	//autoRepaint: ["Filter", "ProdLotSerialRecs"],
})
export class AsBuiltTreeNode extends PXView {
	ParentID: PXFieldState;
	MatlLine: PXFieldState;
	Label: PXFieldState;
	ToolTip: PXFieldState;
	SortOrder: PXFieldState;
	Icon: PXFieldState;
	Level: PXFieldState;
	SelectedValue: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMProdLotSerial extends PXView {
	InventoryID: PXFieldState;
	Descr: PXFieldState;
	LotSerialNbr: PXFieldState;
	ParentInventoryID: PXFieldState;
	ParentDescr: PXFieldState;
	ParentLotSerialNbr: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.AsBuiltConfigInq', primaryView: 'Filter' })
export class AM401700 extends PXScreen {
	Filter = createSingle(AsBuiltConfigFilter);
	Tree = createCollection(AsBuiltTreeNode);
	ProdLotSerialRecs = createCollection(AMProdLotSerial);
}

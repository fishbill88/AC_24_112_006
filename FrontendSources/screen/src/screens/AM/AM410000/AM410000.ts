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

export class BOMCompareFilter extends PXView {
	IDType1: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMID1: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID1: PXFieldState<PXFieldOptions.CommitChanges>;
	ECRID1: PXFieldState<PXFieldOptions.CommitChanges>;
	ECOID1: PXFieldState<PXFieldOptions.CommitChanges>;
	IDType2: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMID2: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID2: PXFieldState<PXFieldOptions.CommitChanges>;
	ECRID2: PXFieldState<PXFieldOptions.CommitChanges>;
	ECOID2: PXFieldState<PXFieldOptions.CommitChanges>;
}

@treeConfig({
	dynamic: true,
	hideRootNode: false,
	dataMember: 'Tree1',
	idParent: 'ParentID',
	iconField: 'Icon',
	//toolTipField : 'ToolTip',
	idName: 'ParentID, LineNbr, CategoryNbr, DetailLineNbr',
	description: 'Label',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
	//autoRepaint: ["BomMatlRecords", "BomStepRecords", "BomToolRecords", "BomOvhdRecords"],
})
export class AMBOMCompareTreeNode extends PXView {
	ParentID: PXFieldState;
	LineNbr: PXFieldState;
	CategoryNbr: PXFieldState;
	DetailLineNbr: PXFieldState;
	Label: PXFieldState;
	ToolTip: PXFieldState;
	SortOrder: PXFieldState;
	Icon: PXFieldState;
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'Tree2',
	idParent: 'ParentID',
	iconField: 'Icon',
	//toolTipField : 'ToolTip',
	idName: 'ParentID, LineNbr, CategoryNbr, DetailLineNbr',
	description: 'Label',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
	//autoRepaint: ["CurrentFeature", "CurrentOption", "Options"],
})
export class AMBOMCompareTreeNode2 extends PXView {
	ParentID: PXFieldState;
	LineNbr: PXFieldState;
	CategoryNbr: PXFieldState;
	DetailLineNbr: PXFieldState;
	Label: PXFieldState;
	ToolTip: PXFieldState;
	SortOrder: PXFieldState;
	Icon: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMBomMatl extends PXView {
	LineID: PXFieldState;
	SortOrder: PXFieldState;
	BOMID: PXFieldState;
	RevisionID: PXFieldState;
	InventoryID: PXFieldState;
	OperationID: PXFieldState;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	QtyReq: PXFieldState;
	BatchSize: PXFieldState;
	UOM: PXFieldState;
	UnitCost: PXFieldState;
	PlanCost: PXFieldState;
	MaterialType: PXFieldState;
	PhantomRouting: PXFieldState;
	BFlush: PXFieldState;
	SiteID: PXFieldState;
	CompBOMID: PXFieldState;
	CompBOMRevisionID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	ScrapFactor: PXFieldState;
	BubbleNbr: PXFieldState;
	EffDate: PXFieldState;
	ExpDate: PXFieldState;
	RowStatus: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false
})
export class AMBomStep extends PXView {
	BOMID: PXFieldState;
	RevisionID: PXFieldState;
	OperationID: PXFieldState;
	Descr: PXFieldState;
	LineID: PXFieldState;
	RowStatus: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMBomTool extends PXView {
	BOMID: PXFieldState;
	RevisionID: PXFieldState;
	OperationID: PXFieldState;
	LineID: PXFieldState;
	ToolID: PXFieldState;
	Descr: PXFieldState;
	QtyReq: PXFieldState;
	UnitCost: PXFieldState;
	RowStatus: PXFieldState;
	AMToolMst__Descr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false
})
export class AMBomOvhd extends PXView {
	BOMID: PXFieldState;
	RevisionID: PXFieldState;
	OperationID: PXFieldState;
	LineID: PXFieldState;
	OvhdID: PXFieldState;
	AMOverhead__Descr: PXFieldState;
	AMOverhead__OvhdType: PXFieldState;
	OFactor: PXFieldState;
	RowStatus: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.BOMCompareInq', primaryView: 'Filter', showActivitiesIndicator: true })
export class AM410000 extends PXScreen {
	Filter = createSingle(BOMCompareFilter);
	Tree1 = createCollection(AMBOMCompareTreeNode);
	Tree2 = createCollection(AMBOMCompareTreeNode2);
	BomMatlRecords = createCollection(AMBomMatl);
	BomStepRecords = createCollection(AMBomStep);
	BomToolRecords = createCollection(AMBomTool);
	BomOvhdRecords = createCollection(AMBomOvhd);
}

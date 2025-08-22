import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	treeConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';

export class AMBomItem extends PXView {
	BOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	EffStartDate: PXFieldState;
	EffEndDate: PXFieldState;
}

@treeConfig({
	dynamic: true,
	hideRootNode: false,
	dataMember: 'BomTree',
	idParent: 'IDParent',
	iconField: 'Icon',
	//toolTipField : 'ToolTip',
	idName: 'IDName',
	description: 'Description',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
})
export class WorkbenchTreeNode extends PXView  {
	IDName: PXFieldState;
	IDNameOriginal: PXFieldState;
	IDParent: PXFieldState;
	Description: PXFieldState;
	Icon: PXFieldState;
	IconColor: PXFieldState;
	Actions: PXFieldState;
	ExtraColumns: PXFieldState;
}

export class AMBomItem2 extends PXView {
	BOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	EffStartDate: PXFieldState;
	EffEndDate: PXFieldState;
}

export class TreeNodeEventResult extends PXView {
	IsOperation: PXFieldState;
	IsSubassembly: PXFieldState;
	OperationCD: PXFieldState;
	WcID: PXFieldState;
	OperationDescription: PXFieldState;
}

export class AMBomMatl extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	QtyReq: PXFieldState;
	UOM: PXFieldState;
	BFlush: PXFieldState;
	BatchSize: PXFieldState;
	ScrapFactor: PXFieldState;
	UnitCost: PXFieldState;
	PlanCost: PXFieldState;
	MaterialType: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractSource: PXFieldState<PXFieldOptions.CommitChanges>;
	PhantomRouting: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState;
	CompBOMID: PXFieldState;
	CompBOMRevisionID: PXFieldState;
	BubbleNbr: PXFieldState;
	EffDate: PXFieldState;
	ExpDate: PXFieldState;
}

export class AMBomItem3 extends PXView {
	BOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	EffStartDate: PXFieldState;
	EffEndDate: PXFieldState;
}

export class AMBomOper extends PXView {
	OperationCD: PXFieldState;
	WcID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	SetupTime: PXFieldState;
	RunUnits: PXFieldState;
	RunUnitTime: PXFieldState;
	MachineUnits: PXFieldState;
	MachineUnitTime: PXFieldState;
	BFlush: PXFieldState;
	ControlPoint: PXFieldState;
	QueueTime: PXFieldState;
	FinishTime: PXFieldState;
	MoveTime: PXFieldState;
	ScrapAction: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
//	topBarItems: {
//		ResetOrder: { index: 0, config: { commandName: "ResetOrder", text: "Reset Lines" } },
//		AddNew: { index: 1, config: { commandName: "AddNew", text: "Insert Row" } },
//		Copy: { index: 2, config: { commandName: "Copy", text: "Cut Row" } },
//		Paste: { index: 3, config: { commandName: "Paste", text: "Insert Cut Row" } },
//	}
})
export class AMBomMatl2 extends PXView {
	PanelRef: PXActionState;
	ResetOrder: PXActionState;
	AddNew: PXActionState;
	Copy: PXActionState;
	Paste: PXActionState;

	LineID: PXFieldState;
	SortOrder: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	QtyReq: PXFieldState<PXFieldOptions.CommitChanges>;
	BatchSize: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitCost: PXFieldState;
	PlanCost: PXFieldState;
	MaterialType: PXFieldState<PXFieldOptions.CommitChanges>;
	PhantomRouting: PXFieldState;
	BFlush: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewCompBomID") CompBOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	CompBOMRevisionID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	ScrapFactor: PXFieldState;
	BubbleNbr: PXFieldState;
	EffDate: PXFieldState;
	ExpDate: PXFieldState;
	SubcontractSource: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class AMBomStep extends PXView {
	Descr: PXFieldState;
	LineID: PXFieldState;
	SortOrder: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class AMBomTool extends PXView {
	LineID: PXFieldState;
	ToolID: PXFieldState;
	Descr: PXFieldState;
	QtyReq: PXFieldState;
	UnitCost: PXFieldState;
	AMToolMst__Descr: PXFieldState;//??
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMBomOvhd extends PXView {
	LineID: PXFieldState;
	OvhdID: PXFieldState;
	AMOverhead__Descr: PXFieldState;
	AMOverhead__OvhdType: PXFieldState;
	OFactor: PXFieldState;
}

export class AMBomOper2 extends PXView {
	OutsideProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	DropShippedToVendor: PXFieldState;
}

export class AMBomOperCury extends PXView {
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMBomRef extends PXView {
	RefDes: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMID: PXFieldState;
	OperationID: PXFieldState;
	LineID: PXFieldState;
	MatlLineID: PXFieldState;
}

export class CopyBomFilter extends PXView {
	FromBOMID: PXFieldState;
	FromRevisionID: PXFieldState;
	FromInventoryID: PXFieldState;
	FromSubItemID: PXFieldState;
	FromSiteID: PXFieldState;
	ToBOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToRevisionID: PXFieldState;
	ToInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToSubItemCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ToSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateMaterialWarehouse: PXFieldState;
	CopyNotesItem: PXFieldState;
	CopyNotesOper: PXFieldState;
	CopyNotesMatl: PXFieldState;
	CopyNotesStep: PXFieldState;
	CopyNotesTool: PXFieldState;
	CopyNotesOvhd: PXFieldState;
}

export class DefaultBomLevels extends PXView {
	Item: PXFieldState;
	Warehouse: PXFieldState;
	SubItem: PXFieldState;
}

export class DefaultBomLevels2 extends PXView {
	Item: PXFieldState;
	Warehouse: PXFieldState;
	SubItem: PXFieldState;
}

export class AMBomCost extends PXView {
	LotSize: PXFieldState;
	MultiLevelProcess: PXFieldState;
	FLaborCost: PXFieldState;
	VLaborCost: PXFieldState;
	FOvdCost: PXFieldState;
	VOvdCost: PXFieldState;
	MachCost: PXFieldState;
	ToolCost: PXFieldState;
	MatlCost: PXFieldState;
	UnitCost: PXFieldState;
	TotalCost: PXFieldState;
}

export class RollupSettings extends PXView {
	LotSize: PXFieldState<PXFieldOptions.CommitChanges>;
	SnglMlti: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.EngineeringWorkbenchMaint', primaryView: 'Documents' })
export class AM208100 extends PXScreen {
	ViewCompBomID: PXActionState;

	Documents = createSingle(AMBomItem);
	BomTree = createCollection(WorkbenchTreeNode);
	SelectedBomItem2 = createSingle(AMBomItem2);
	SelectedTreeNode = createSingle(TreeNodeEventResult);
	SelectedBomMatl = createSingle(AMBomMatl);
	SubassemblyBomItem3 = createSingle(AMBomItem3);
	SelectedBomOper = createSingle(AMBomOper);
	BomMatlRecords = createCollection(AMBomMatl2);
	BomStepRecords = createCollection(AMBomStep);
	BomToolRecords = createCollection(AMBomTool);
	BomOvhdRecords = createCollection(AMBomOvhd);
	OutsideProcessingOperationSelected = createSingle(AMBomOper2);
	CurySettings_AMBomOper = createSingle(AMBomOperCury);

	BomRefRecords = createCollection(AMBomRef);
	copyBomFilter = createSingle(CopyBomFilter);
	DefaultBomLevelsFilter = createSingle(DefaultBomLevels);
	PlanningBomLevelsFilter = createSingle(DefaultBomLevels2);
	BomCostRecs = createSingle(AMBomCost);
	rollsettings = createSingle(RollupSettings);
}

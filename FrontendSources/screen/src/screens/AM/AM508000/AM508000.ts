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
	GridPreset,
	linkCommand,
} from 'client-controls';

export class Settings extends PXView {
	SnglMlti: PXFieldState;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteId: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	BOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState;
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ApplyPend: PXFieldState;
	IncMatScrp: PXFieldState;
	IncFixed: PXFieldState;
	UpdateMaterial: PXFieldState;
	UsePending: PXFieldState;
	IgnoreMinMaxLotSizeValues: PXFieldState;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class BomCostRecs extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	@linkCommand("ViewBOM") BOMID: PXFieldState;
	RevisionID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	UnitCost: PXFieldState;
	MatlManufacturedCost: PXFieldState;
	MatlNonManufacturedCost: PXFieldState;
	MatlCost: PXFieldState;
	FLaborCost: PXFieldState;
	VLaborCost: PXFieldState;
	FOvdCost: PXFieldState;
	VOvdCost: PXFieldState;
	ToolCost: PXFieldState;
	MachCost: PXFieldState;
	SubcontractMaterialCost: PXFieldState;
	ReferenceMaterialCost: PXFieldState;
	LotSize: PXFieldState;
	AMBomItem__Status: PXFieldState;
	AMBomItem__Descr: PXFieldState;
	MultiLevelProcess: PXFieldState;
	Level: PXFieldState;
	IsDefaultBom: PXFieldState;
	FixedLaborTime: PXFieldState;
	VariableLaborTime: PXFieldState;
	MachineTime: PXFieldState;
	@columnConfig({ hideViewLink: true }) ItemClassID: PXFieldState;
	StdCost: PXFieldState;
	PendingStdCost: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.BOMCostRoll', primaryView: 'settings' })
export class AM508000 extends PXScreen {
	// to remove the button from the screen toolbar
	ViewBOM: PXActionState;

	settings = createSingle(Settings);
	BomCostRecs = createCollection(BomCostRecs);
}

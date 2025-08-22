import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';


@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class CostRollHistoryRecords extends PXView {
	@linkCommand("ViewBOM") BOMID: PXFieldState;
	RevisionID: PXFieldState;
	CreatedDateTime: PXFieldState;
	LastModifiedDateTime: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
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
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCost: PXFieldState;
	PendingStdCost: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.CostRollHistory', primaryView: 'CostRollHistoryRecords' })
export class AM408000 extends PXScreen {
	// to remove the button from the screen toolbar
	ViewBOM: PXActionState;

	CostRollHistoryRecords = createCollection(CostRollHistoryRecords);
}

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

@graphInfo({ graphType: 'PX.Objects.AM.ProdDetail', primaryView: 'ProdItemRecords' })
export class AM209000 extends PXScreen {
	MatlLineSplittingExtension_GenerateNumbers: PXActionState;

	ProdItemRecords = createSingle(AMProdItem);
	ProdOperRecords = createCollection(AMProdOper);
	ProdMatlRecords = createCollection(AMProdMatl);
	ProdStepRecords = createCollection(AMProdStep);
	ProdToolRecords = createCollection(AMProdTool);
	ProdOvhdRecords = createCollection(AMProdOvhd);
	ProdOperSelected = createSingle(ProdOperTotal);
	OutsideProcessingOperationSelected = createSingle(AMProdOper);
	MatlLineSplittingExtension_LotSerOptions = createSingle(LineSplittingHeader);
	ProdMatlSplits = createCollection(AMProdMatlSplit);
	currentposupply = createSingle(Currentposupply);
	posupply = createCollection(Posupply);
}

export class AMProdItem extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState;
	ProdDate: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	SiteId: PXFieldState;
	StatusID: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMProdOper extends PXView {
	CreatePurchaseOrder: PXActionState;
	CreateVendorShipment: PXActionState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	OperationCD: PXFieldState;
	WcID: PXFieldState;
	Descr: PXFieldState;
	SetupTime: PXFieldState<PXFieldOptions.CommitChanges>;
	SetupTimeRaw: PXFieldState;
	RunUnits: PXFieldState<PXFieldOptions.CommitChanges>;
	RunUnitTime: PXFieldState<PXFieldOptions.CommitChanges>;
	RunUnitTimeRaw: PXFieldState;
	MachineUnits: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineUnitTime: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineUnitTimeRaw: PXFieldState;
	QueueTime: PXFieldState;
	QueueTimeRaw: PXFieldState;
	FinishTime: PXFieldState;
	FinishTimeRaw: PXFieldState;
	MoveTime: PXFieldState;
	MoveTimeRaw: PXFieldState;
	QtytoProd: PXFieldState;
	QtyComplete: PXFieldState;
	QtyScrapped: PXFieldState;
	QtyRemaining: PXFieldState;
	TotalQty: PXFieldState;
	StatusID: PXFieldState;
	BFlush: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	ActStartDate: PXFieldState;
	ActEndDate: PXFieldState;
	ScrapAction: PXFieldState;
	PhtmBOMID: PXFieldState;
	PhtmBOMRevisionID: PXFieldState;
	PhtmBOMOperationID: PXFieldState;
	PhtmBOMLineRef: PXFieldState;
	PhtmLevel: PXFieldState;
	PhtmMatlBOMID: PXFieldState;
	PhtmMatlRevisionID: PXFieldState;
	PhtmMatlOperationID: PXFieldState;
	PhtmMatlLineRef: PXFieldState;
	PhtmPriorLevelQty: PXFieldState;
	ProdOrdID: PXFieldState;
	OutsideProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	DropShippedToVendor: PXFieldState;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState;
	POOrderNbr: PXFieldState;
	POLineNbr: PXFieldState;
	ShippedQuantity: PXFieldState;
	ShipRemainingQty: PXFieldState;
	AtVendorQuantity: PXFieldState;
	ControlPoint: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoReportQty: PXFieldState;
	StartDate_Date: PXFieldState;
	EndDate_Date: PXFieldState;
	ActStartDate_Date: PXFieldState;
	ActEndDate_Date: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMProdMatl extends PXView {
	ResetOrder: PXActionState;
	MatlLineSplittingExtension_ShowSplits: PXActionState;
	InventoryAllocationDetailInqMatl: PXActionState;
	POSupplyOK: PXActionState;
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	QtyReq: PXFieldState;
	BatchSize: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	UnitCost: PXFieldState;
	StatusID: PXFieldState;
	BFlush: PXFieldState;
	WarehouseOverride: PXFieldState;
	SiteID: PXFieldState;
	@linkCommand("ViewCompBomID") CompBOMID: PXFieldState;
	CompBOMRevisionID: PXFieldState;
	LocationID: PXFieldState;
	ScrapFactor: PXFieldState;
	TotalQtyRequired: PXFieldState;
	PlanCost: PXFieldState;
	QtyActual: PXFieldState;
	QtyRemaining: PXFieldState;
	QtyRoundUp: PXFieldState;
	TotActCost: PXFieldState;
	MaterialType: PXFieldState;
	PhtmBOMID: PXFieldState;
	PhtmBOMLineRef: PXFieldState;
	PhtmBOMOperationID: PXFieldState;
	PhtmLevel: PXFieldState;
	PhtmMatlLineRef: PXFieldState;
	PhtmMatlOperationID: PXFieldState;
	IsByproduct: PXFieldState;
	LineID: PXFieldState;
	SortOrder: PXFieldState;
	POCreate: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdCreate: PXFieldState;
	SubcontractSource: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMProdStep extends PXView {
	Descr: PXFieldState;
	PhtmBOMID: PXFieldState;
	PhtmBOMRevisionID: PXFieldState;
	PhtmBOMOperationID: PXFieldState;
	PhtmBOMLineRef: PXFieldState;
	PhtmLevel: PXFieldState;
	PhtmMatlBOMID: PXFieldState;
	PhtmMatlRevisionID: PXFieldState;
	PhtmMatlOperationID: PXFieldState;
	PhtmMatlLineRef: PXFieldState;
	LineID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	ProdOrdID: PXFieldState;
	SortOrder: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMProdTool extends PXView {
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	LineID: PXFieldState;
	ToolID: PXFieldState;
	Descr: PXFieldState;
	QtyReq: PXFieldState;
	UnitCost: PXFieldState;
	TotActUses: PXFieldState;
	TotActCost: PXFieldState;
	PhtmBOMID: PXFieldState;
	PhtmBOMRevisionID: PXFieldState;
	PhtmBOMOperationID: PXFieldState;
	PhtmBOMLineRef: PXFieldState;
	PhtmLevel: PXFieldState;
	PhtmMatlBOMID: PXFieldState;
	PhtmMatlRevisionID: PXFieldState;
	PhtmMatlOperationID: PXFieldState;
	PhtmMatlLineRef: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMProdOvhd extends PXView {
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	LineID: PXFieldState;
	OvhdID: PXFieldState;
	AMOverhead__Descr: PXFieldState;
	AMOverhead__OvhdType: PXFieldState;
	OFactor: PXFieldState;
	AMOverheadCurySettings__CostRate: PXFieldState;
	TotActCost: PXFieldState;
	WCFlag: PXFieldState;
	PhtmBOMID: PXFieldState;
	PhtmBOMRevisionID: PXFieldState;
	PhtmBOMOperationID: PXFieldState;
	PhtmBOMLineRef: PXFieldState;
	PhtmLevel: PXFieldState;
	PhtmMatlBOMID: PXFieldState;
	PhtmMatlRevisionID: PXFieldState;
	PhtmMatlOperationID: PXFieldState;
	PhtmMatlLineRef: PXFieldState;
}

export class ProdOperTotal extends PXView {
	PlanLaborTime: PXFieldState;
	PlanLaborTimeRaw: PXFieldState;
	PlanLabor: PXFieldState;
	PlanMachine: PXFieldState;
	PlanMaterial: PXFieldState;
	PlanTool: PXFieldState;
	PlanFixedOverhead: PXFieldState;
	PlanVariableOverhead: PXFieldState;
	PlanSubcontract: PXFieldState;
	PlanQtyToProduce: PXFieldState;
	PlanTotal: PXFieldState;
	PlanCostDate: PXFieldState;
	PlanReferenceMaterial: PXFieldState;
	ActualLaborTime: PXFieldState;
	ActualLaborTimeRaw: PXFieldState;
	ActualLabor: PXFieldState;
	ActualMachine: PXFieldState;
	ActualMaterial: PXFieldState;
	ActualTool: PXFieldState;
	ActualFixedOverhead: PXFieldState;
	ActualVariableOverhead: PXFieldState;
	ActualSubcontract: PXFieldState;
	QtyComplete: PXFieldState;
	WIPAdjustment: PXFieldState;
	ScrapAmount: PXFieldState;
	WIPTotal: PXFieldState;
	WIPComp: PXFieldState;
	VarianceLaborTime: PXFieldState;
	VarianceLaborTimeRaw: PXFieldState;
	VarianceLabor: PXFieldState;
	VarianceMachine: PXFieldState;
	VarianceMaterial: PXFieldState;
	VarianceTool: PXFieldState;
	VarianceFixedOverhead: PXFieldState;
	VarianceVariableOverhead: PXFieldState;
	VarianceSubcontract: PXFieldState;
	QtyRemaining: PXFieldState;
	VarianceTotal: PXFieldState;
	WIPBalance: PXFieldState;
}

export class LineSplittingHeader extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	adjustPageSize: true,
	syncPosition: true,
})
export class AMProdMatlSplit extends PXView {
	IsAllocated: PXFieldState;
	SplitLineNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	Qty: PXFieldState;
	QtyReceived: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	POCreate: PXFieldState;
	ProdCreate: PXFieldState;
	@linkCommand("AMProdMatlSplit$RefNoteID$Link") RefNoteID: PXFieldState;
}

export class Currentposupply extends PXView {
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class Posupply extends PXView {
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	VendorRefNbr: PXFieldState;
	LineType: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	VendorID: PXFieldState;
	VendorID_Vendor_AcctName: PXFieldState;
	PromisedDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	OrderQty: PXFieldState;
	OpenQty: PXFieldState;
	TranDesc: PXFieldState;
}

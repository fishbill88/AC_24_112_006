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
} from 'client-controls';

export class AMEstimateOper extends PXView {
	EstimateID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	OperationCD: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	SetupTime: PXFieldState<PXFieldOptions.CommitChanges>;
	SetupTimeRaw: PXFieldState<PXFieldOptions.Hidden>;
	RunUnits: PXFieldState<PXFieldOptions.CommitChanges>;
	RunUnitTime: PXFieldState<PXFieldOptions.CommitChanges>;
	RunUnitTimeRaw: PXFieldState<PXFieldOptions.Hidden>;
	MachineUnits: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineUnitTime: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineUnitTimeRaw: PXFieldState<PXFieldOptions.Hidden>;
	QueueTime: PXFieldState;
	QueueTimeRaw: PXFieldState<PXFieldOptions.Hidden>;
	FinishTime: PXFieldState;
	FinishTimeRaw: PXFieldState<PXFieldOptions.Hidden>;
	MoveTime: PXFieldState;
	MoveTimeRaw: PXFieldState<PXFieldOptions.Hidden>;
	Description: PXFieldState;
	FixedLaborCost: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedLaborOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableLaborCost: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableLaborOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineCost: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MaterialCost: PXFieldState<PXFieldOptions.CommitChanges>;
	MaterialOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ToolCost: PXFieldState<PXFieldOptions.CommitChanges>;
	ToolOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedOverheadCost: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedOverheadOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableOverheadCost: PXFieldState<PXFieldOptions.CommitChanges>;
	VariableOverheadOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractCost: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtCost: PXFieldState<PXFieldOptions.CommitChanges>;
	ReferenceMaterialCost: PXFieldState;
	BackflushLabor: PXFieldState<PXFieldOptions.CommitChanges>;
	ControlPoint: PXFieldState;
	OutsideProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	DropShippedToVendor: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMEstimateMatl extends PXView {
	ResetOrder: PXActionState;

	InventoryCD: PXFieldState;
	SubItemID: PXFieldState;
	ItemDesc: PXFieldState;
	@columnConfig({ hideViewLink: true }) ItemClassID: PXFieldState;
	QtyReq: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	UnitCost: PXFieldState;
	BackFlush: PXFieldState;
	SiteID: PXFieldState;
	ScrapFactor: PXFieldState;
	BatchSize: PXFieldState;
	QtyRoundUp: PXFieldState;
	TotalQtyRequired: PXFieldState;
	MaterialOperCost: PXFieldState;
	IsNonInventory: PXFieldState;
	MaterialType: PXFieldState;
	PhantomRouting: PXFieldState;
	LineID: PXFieldState;
	SortOrder: PXFieldState;
	SubcontractSource: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMEstimateStep extends PXView {
	Description: PXFieldState;
	LineID: PXFieldState;
	OperationID: PXFieldState;
	SortOrder: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMEstimateTool extends PXView {
	ToolID: PXFieldState;
	Description: PXFieldState;
	QtyReq: PXFieldState;
	UnitCost: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMEstimateOvhd extends PXView {
	OvhdID: PXFieldState;
	Description: PXFieldState;
	OvhdType: PXFieldState;
	OverheadCostRate: PXFieldState;
	OFactor: PXFieldState;
	WCFlag: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.EstimateOperMaint', primaryView: 'EstimateOperationRecords' })
export class AM304000 extends PXScreen {
	EstimateOperationRecords = createSingle(AMEstimateOper);
	EstimateOperMatlRecords = createCollection(AMEstimateMatl);
	EstimateOperStepRecords = createCollection(AMEstimateStep);
	EstimateOperToolRecords = createCollection(AMEstimateTool);
	EstimateOperOvhdRecords = createCollection(AMEstimateOvhd);
}

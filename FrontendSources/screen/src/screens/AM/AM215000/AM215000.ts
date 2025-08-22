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

export class AMECRItem extends PXView {
	ECOID: PXFieldState;
	RevisionID: PXFieldState;
	BOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMRevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	RequestDate: PXFieldState;
	EffectiveDate: PXFieldState;
	Requestor: PXFieldState;
	Priority: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	autoRepaint: ["BomMatlRecords", "BomStepRecords", "BomToolRecords", "BomOvhdRecords", "OutsideProcessingOperationSelected"],
})
export class AMBomOper extends PXView {
	OperationCD: PXFieldState;
	WcID: PXFieldState;
	Descr: PXFieldState;
	SetupTime: PXFieldState;
	RunUnits: PXFieldState;
	RunUnitTime: PXFieldState;
	MachineUnits: PXFieldState;
	MachineUnitTime: PXFieldState;
	QueueTime: PXFieldState;
	FinishTime: PXFieldState;
	MoveTime: PXFieldState;
	BFlush: PXFieldState;
	ScrapAction: PXFieldState;
	RowStatus: PXFieldState;
	ControlPoint: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMBomAttribute extends PXView {
	LineNbr: PXFieldState;
	Level: PXFieldState;
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	OperationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Label: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	Enabled: PXFieldState;
	TransactionRequired: PXFieldState;
	Value: PXFieldState;
	OrderFunction: PXFieldState;
	RowStatus: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class EPApproval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrigOwnerID: PXFieldState;
	ApproveDate: PXFieldState;
	Status: PXFieldState;
	Reason: PXFieldState;
	@columnConfig({ hideViewLink: true }) AssignmentMapID: PXFieldState;
	@columnConfig({ hideViewLink: true }) RuleID: PXFieldState;
	@columnConfig({ hideViewLink: true }) StepID: PXFieldState;
	CreatedDateTime: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMBomMatl extends PXView {
	ResetOrder: PXActionState;
	AddNew: PXActionState;
	Copy: PXActionState;
	Paste: PXActionState;

	LineID: PXFieldState;
	SortOrder: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	QtyReq: PXFieldState;
	BatchSize: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	UnitCost: PXFieldState;
	PlanCost: PXFieldState;
	MaterialType: PXFieldState<PXFieldOptions.CommitChanges>;
	PhantomRouting: PXFieldState;
	BFlush: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewCompBomID") CompBOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	CompBOMRevisionID: PXFieldState;
	LocationID: PXFieldState;
	ScrapFactor: PXFieldState;
	BubbleNbr: PXFieldState;
	EffDate: PXFieldState;
	ExpDate: PXFieldState;
	RowStatus: PXFieldState;
	SubcontractSource: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMID: PXFieldState;
	OperationID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMBomStep extends PXView {
	Descr: PXFieldState;
	LineID: PXFieldState;
	RowStatus: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMBomTool extends PXView {
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
	adjustPageSize: true,
})
export class AMBomOvhd extends PXView {
	LineID: PXFieldState;
	OvhdID: PXFieldState;
	AMOverhead__Descr: PXFieldState;
	AMOverhead__OvhdType: PXFieldState;
	OFactor: PXFieldState;
	RowStatus: PXFieldState;
}

export class AMBomOper2 extends PXView {
	OutsideProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	DropShippedToVendor: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMBomRef extends PXView {
	RefDes: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMID: PXFieldState;
	OperationID: PXFieldState;
	LineID: PXFieldState;
	MatlLineID: PXFieldState;
	RowStatus: PXFieldState;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.ECOMaint', primaryView: 'Documents', showActivitiesIndicator: true })
export class AM215000 extends PXScreen {
	Documents = createSingle(AMECRItem);
	BomOperRecords = createCollection(AMBomOper);
	BomAttributes = createCollection(AMBomAttribute);
	Approval = createCollection(EPApproval);
	BomMatlRecords = createCollection(AMBomMatl);
	BomStepRecords = createCollection(AMBomStep);
	BomToolRecords = createCollection(AMBomTool);
	BomOvhdRecords = createCollection(AMBomOvhd);
	OutsideProcessingOperationSelected = createSingle(AMBomOper2);
	BomRefRecords = createCollection(AMBomRef);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

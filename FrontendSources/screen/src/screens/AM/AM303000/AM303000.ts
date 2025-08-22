import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
	ICurrencyInfo,
} from 'client-controls';

export class AMEstimateItem extends PXView {
	EstimateID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsNonInventory: PXFieldState;
	RevisionID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPrimary: PXFieldState;
	InventoryCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemDesc: PXFieldState;
	SubItemID: PXFieldState;
	EstimateClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState;
	EngineerID: PXFieldState;
	RequestDate: PXFieldState;
	PromiseDate: PXFieldState;
	LeadTime: PXFieldState;
	LeadTimeOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimateStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	QuoteSource: PXFieldState;
	RevisionDate: PXFieldState;
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
	ExtCostDisplay: PXFieldState<PXFieldOptions.CommitChanges>;
	ReferenceMaterialCost: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AMEstimateItem2 extends PXView {
	EstimateID: PXFieldState;
	IsNonInventory: PXFieldState;
	RevisionID: PXFieldState;
	ImageUrl: PXFieldState;
	Body: PXFieldState;
	OrderQty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	LaborMarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	MachineMarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	MaterialMarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	ToolMarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	OverheadMarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractMarkupPct: PXFieldState<PXFieldOptions.CommitChanges>;
	MarkupPct: PXFieldState;
	CuryID: PXFieldState;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	syncPosition: true,
})
export class AMEstimateOper extends PXView {
	OperationCD: PXFieldState;
	WorkCenterID: PXFieldState;
	Description: PXFieldState;
	SetupTime: PXFieldState;
	RunUnits: PXFieldState;
	RunUnitTime: PXFieldState;
	MachineUnits: PXFieldState;
	MachineUnitTime: PXFieldState;
	QueueTime: PXFieldState;
	FinishTime: PXFieldState;
	MoveTime: PXFieldState;
	BackFlushLabor: PXFieldState;
	ControlPoint: PXFieldState;
	FixedLaborCost: PXFieldState;
	FixedLaborOverride: PXFieldState;
	VariableLaborCost: PXFieldState;
	VariableLaborOverride: PXFieldState;
	MachineCost: PXFieldState;
	MachineOverride: PXFieldState;
	MaterialCost: PXFieldState;
	MaterialOverride: PXFieldState;
	ToolCost: PXFieldState;
	ToolOverride: PXFieldState;
	FixedOverheadCost: PXFieldState;
	FixedOverheadOverride: PXFieldState;
	VariableOverheadCost: PXFieldState;
	VariableOverheadOverride: PXFieldState;
	SubcontractCost: PXFieldState;
	SubcontractOverride: PXFieldState;
	ReferenceMaterialCost: PXFieldState;
	OutsideProcess: PXFieldState;
}
export class AMEstimateReference extends PXView {
	EstimateID: PXFieldState;
	RevisionID: PXFieldState;
	CuryInfoID: PXFieldState;
	OpportunityID: PXFieldState;
	QuoteType: PXFieldState;
	QuoteNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	QuoteNbrLink: PXFieldState;
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState;
	ExternalRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
})
export class AMEstimateHistory extends PXView {
	EstimateID: PXFieldState;
	LineNbr: PXFieldState;
	RevisionID: PXFieldState;
	CreatedDateTime: PXFieldState;
	CreatedByID: PXFieldState;
	Description: PXFieldState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.EstimateMaint', primaryView: 'Documents' })
export class AM303000 extends PXScreen {
	// to remove the buttons from the screen toolbar
	AddHistory: PXActionState;

	Documents = createSingle(AMEstimateItem);
	EstimateOperRecords = createCollection(AMEstimateOper);
	EstimateReferenceRecord = createSingle(AMEstimateReference);
	EstimateRecordSelected = createSingle(AMEstimateItem2);
	EstimateHistoryRecords = createCollection(AMEstimateHistory);
	_AMEstimateItem_CurrencyInfo_ = createSingle(CurrencyInfo);
}

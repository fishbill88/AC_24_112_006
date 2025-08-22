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
	headerDescription,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.DisassemblyEntry', primaryView: 'Document' })
export class AM301500 extends PXScreen {
	Document = createSingle(AMDisassembleBatch);
	MaterialTransactionRecords = createCollection(AMDisassembleTran);
	TransactionAttributes = createCollection(AMDisassembleBatchAttribute);

	AMDisassembleMasterTranLineSplittingExtension_GenerateNumbers: PXActionState;
	AMDisassembleMasterTranLineSplittingExtension_LotSerOptions = createSingle(AMDisassembleMasterTranLineSplittingExtension_LotSerOptions);
	MasterSplits = createCollection(AMDisassembleBatchSplit);

	CurrentDocument = createSingle(AMDisassembleBatch);

	AMDisassembleMaterialTranLineSplittingExtension_GenerateNumbers: PXActionState;
	AMDisassembleMaterialTranLineSplittingExtension_LotSerOptions = createSingle(AMDisassembleMaterialTranLineSplittingExtension_LotSerOptions);
	MaterialSplits = createCollection(AMDisassembleTranSplit);
}

export class AMDisassembleBatch extends PXView {
	BatchNbr: PXFieldState;
	Status: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@headerDescription Description: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState;
	INDocType: PXFieldState;
	INBatNbr: PXFieldState;
	TranDesc: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMDisassembleTran extends PXView {
	AMDisassembleMaterialTranLineSplittingExtension_ShowSplits: PXActionState;
	copyLine: PXActionState;

	LineNbr: PXFieldState;
	TranOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	IsScrap: PXFieldState<PXFieldOptions.CommitChanges>;
	TranType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	ExpireDate: PXFieldState;
	UnitCost: PXFieldState;
	TranAmt: PXFieldState;
	TranDesc: PXFieldState;
	ReasonCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	MatlLineId: PXFieldState;
	ParentLotSerialNbr: PXFieldState;
	GLBatNbr: PXFieldState;
	GLLineNbr: PXFieldState;
	INDocType: PXFieldState;
	INBatNbr: PXFieldState;
	INLineNbr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMDisassembleBatchAttribute extends PXView {
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	LineNbr: PXFieldState;
	AttributeID: PXFieldState;
	Label: PXFieldState;
	Descr: PXFieldState;
	TransactionRequired: PXFieldState;
	Value: PXFieldState;
}

export class AMDisassembleMasterTranLineSplittingExtension_LotSerOptions extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMDisassembleBatchSplit extends PXView {
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ width: 200 }) ExpireDate: PXFieldState;
	InventoryID: PXFieldState;
}

export class AMDisassembleMaterialTranLineSplittingExtension_LotSerOptions extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMDisassembleTranSplit extends PXView {
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@columnConfig({ width: 200 }) ExpireDate: PXFieldState;
}

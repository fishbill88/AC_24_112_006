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

@graphInfo({ graphType: 'PX.Objects.AM.LaborEntry', primaryView: 'batch' })
export class AM301000 extends PXScreen {
	LineSplittingExtension_GenerateNumbers: PXActionState;

	batch = createSingle(AMBatch);
	transactions = createCollection(AMMTran);
	LineSplittingExtension_LotSerOptions = createSingle(LineSplittingHeader);
	splits = createCollection(AMMTranSplit);
	TransactionAttributes = createCollection(AMMTranAttribute);
}

export class AMBatch extends PXView {
	BatNbr: PXFieldState;
	Status: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	@headerDescription TranDesc: PXFieldState;
	ControlQty: PXFieldState;
	TotalQty: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMMTran extends PXView {
	LineSplittingExtension_ShowSplits: PXActionState;
	ProductionAttributes: PXActionState;
	lateAssignmentEntry: PXActionState;

	LineNbr: PXFieldState;
	LaborType: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LaborCodeID: PXFieldState;
	@columnConfig({ hideViewLink: true }) EmployeeID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ShiftCD: PXFieldState;
	StartTime: PXFieldState;
	EndTime: PXFieldState;
	LaborTime: PXFieldState;
	LaborTimeRaw: PXFieldState;
	LaborRate: PXFieldState;
	ExtCost: PXFieldState;
	IsScrap: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	QtyScrapped: PXFieldState;
	ReasonCodeID: PXFieldState;
	ScrapAction: PXFieldState;
	GLBatNbr: PXFieldState;
	GLLineNbr: PXFieldState;
	INDocType: PXFieldState;
	INBatNbr: PXFieldState;
	INLineNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	ExpireDate: PXFieldState;
	ReceiptNbr: PXFieldState;
	TranDesc: PXFieldState;
}

export class LineSplittingHeader extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMMTranSplit extends PXView {
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@columnConfig({ width: 200 }) ExpireDate: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMMTranAttribute extends PXView {
	DocType: PXFieldState;
	BatNbr: PXFieldState;
	TranLineNbr: PXFieldState;
	LineNbr: PXFieldState;
	AttributeID: PXFieldState;
	Label: PXFieldState;
	Descr: PXFieldState;
	TransactionRequired: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

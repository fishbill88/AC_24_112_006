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

@graphInfo({ graphType: 'PX.Objects.AM.MaterialEntry', primaryView: 'batch' })
export class AM300000 extends PXScreen {
	LineSplittingExtension_GenerateNumbers: PXActionState;

	batch = createSingle(AMBatch);
	transactions = createCollection(AMMTran);
	LineSplittingExtension_LotSerOptions = createSingle(LineSplittingHeader);
	splits = createCollection(AMMTranSplit);
}

export class AMBatch extends PXView {
	BatNbr: PXFieldState;
	Status: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrigBatNbr: PXFieldState;
	OrigDocType: PXFieldState;
	@headerDescription TranDesc: PXFieldState;
	TotalQty: PXFieldState;
	ControlQty: PXFieldState;
	TotalAmount: PXFieldState;
	ControlAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMMTran extends PXView {
	LineSplittingExtension_ShowSplits: PXActionState;

	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	ExpireDate: PXFieldState;
	UnitCost: PXFieldState;
	TranAmt: PXFieldState;
	GLBatNbr: PXFieldState;
	GLLineNbr: PXFieldState;
	INDocType: PXFieldState;
	INBatNbr: PXFieldState;
	INLineNbr: PXFieldState;
	IsByproduct: PXFieldState;
	MatlLineId: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID_description: PXFieldState;
	@columnConfig({ hideViewLink: true }) ParentLotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	LineNbr: PXFieldState;
	TranDesc: PXFieldState;
}

export class LineSplittingHeader extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMMTranSplit extends PXView {
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@columnConfig({ width: 200 }) ExpireDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) ParentLotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

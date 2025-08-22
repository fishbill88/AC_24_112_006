import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
	headerDescription,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.WIPAdjustmentEntry', primaryView: 'batch' })
export class AM308000 extends PXScreen {
	batch = createSingle(AMBatch);
	transactions = createCollection(AMMTran);
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
	ControlAmount: PXFieldState;
	TotalAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class AMMTran extends PXView {
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	TranAmt: PXFieldState;
	ReasonCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) AcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) SubID: PXFieldState;
	TranDesc: PXFieldState;
	LineNbr: PXFieldState;
	GLBatNbr: PXFieldState;
	GLLineNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	INDocType: PXFieldState;
	INBatNbr: PXFieldState;
	INLineNbr: PXFieldState;
	QtyScrapped: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	TranType: PXFieldState;
}

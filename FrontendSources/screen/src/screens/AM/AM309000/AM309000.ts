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

@graphInfo({ graphType: 'PX.Objects.AM.ProductionCostEntry', primaryView: 'batch' })
export class AM309000 extends PXScreen {
	batch = createSingle(AMBatch);
	transactions = createCollection(AMMTran);
}

export class AMBatch extends PXView {
	BatNbr: PXFieldState;
	Status: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDate: PXFieldState;
	FinPeriodID: PXFieldState;
	OrigBatNbr: PXFieldState;
	OrigDocType: PXFieldState;
	@headerDescription TranDesc: PXFieldState;
	ControlAmount: PXFieldState;
	TotalAmount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class AMMTran extends PXView {
	TranType: PXFieldState;
	TranAmt: PXFieldState;
	@columnConfig({ hideViewLink: true }) AcctID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SubID: PXFieldState;
	TranDesc: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	InventoryID: PXFieldState;
	LineNbr: PXFieldState;
	GLBatNbr: PXFieldState;
	GLLineNbr: PXFieldState;
	Qty: PXFieldState;
	ReferenceCostID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
}

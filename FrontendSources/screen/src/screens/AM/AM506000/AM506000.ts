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

@graphInfo({ graphType: 'PX.Objects.AM.CloseOrderProcess', primaryView: 'CompletedOrders' })
export class AM506000 extends PXScreen {
	transactionsByProductionOrderInq: PXActionState;
	CompletedOrders = createCollection(AMProdItem);
	FinancialPeriod = createSingle(FinancialPeriod);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class AMProdItem extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteId: PXFieldState;
	QtytoProd: PXFieldState;
	QtyComplete: PXFieldState;
	WIPBalance: PXFieldState;
	@columnConfig({ hideViewLink: true }) WIPVarianceAcctID: PXFieldState;
	@columnConfig({ hideViewLink: true }) WIPVarianceSubID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ProjectID: PXFieldState;
	@columnConfig({ hideViewLink: true }) TaskID: PXFieldState;
	CostCodeID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
}

export class FinancialPeriod extends PXView {
	FinancialPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}

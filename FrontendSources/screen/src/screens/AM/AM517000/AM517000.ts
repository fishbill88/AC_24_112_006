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
} from 'client-controls';

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class ProductionOrderList extends PXView {
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
	BranchID: PXFieldState;
}

export class Filter extends PXView {
	processAction: PXFieldState<PXFieldOptions.CommitChanges>;
}

@graphInfo({ graphType: 'PX.Objects.AM.LockOrderProcess', primaryView: 'ProductionOrderList' })
export class AM517000 extends PXScreen {
	Filter = createSingle(Filter);
	ProductionOrderList = createCollection(ProductionOrderList);
}

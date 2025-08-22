import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.ReleaseOrd', primaryView: 'PlannedOrds' })
export class AM500000 extends PXScreen {
	PlannedOrds = createCollection(AMProdItem);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class AMProdItem extends PXView {
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteId: PXFieldState;
	QtytoProd: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	ProdDate: PXFieldState;
	OrdTypeRef: PXFieldState;
	OrdNbr: PXFieldState;
	InventoryID_InventoryItem_descr: PXFieldState;
	CustomerID: PXFieldState;
	CustomerID_Customer_acctName: PXFieldState;
	DetailSource: PXFieldState;
	@columnConfig({ hideViewLink: true }) ProjectID: PXFieldState;
	@columnConfig({ hideViewLink: true }) TaskID: PXFieldState;
	@columnConfig({ hideViewLink: true }) CostCodeID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
}

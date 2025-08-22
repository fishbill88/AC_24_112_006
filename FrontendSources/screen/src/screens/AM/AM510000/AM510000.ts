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

@graphInfo({ graphType: 'PX.Objects.AM.CreateProductionOrdersProcess', primaryView: 'Filter' })
export class AM510000 extends PXScreen {
	InventorySummary: PXActionState;
	InventoryAllocationDetails: PXActionState;

	Filter = createSingle(ProductionOrdersCreateFilter);
	FixedDemand = createCollection(AMFixedDemand);
}

export class ProductionOrdersCreateFilter extends PXView {
	CreationOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	CreationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassCD: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestedOnStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestedOnEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class AMFixedDemand extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	PlanType_INPlanType_descr: PXFieldState;
	InventoryID: PXFieldState;
	InventoryID_InventoryItem_descr: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	OrderQty: PXFieldState;
	PlanDate: PXFieldState;
	SOOrder__CustomerID: PXFieldState;
	SOOrder__CustomerID_BAccountR_acctName: PXFieldState;
	@columnConfig({ hideViewLink: true }) SOOrder__CustomerLocationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) SOOrderType: PXFieldState;
	@linkCommand("viewDocument") SOOrderNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) AMOrderType: PXFieldState;
	AMProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) AMOperationID: PXFieldState;
}

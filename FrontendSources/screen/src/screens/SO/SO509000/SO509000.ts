import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	PXFieldOptions,
	viewInfo,
	gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.SO.SOCreate', primaryView: 'Filter' })
export class SO509000 extends PXScreen {

	@viewInfo({ containerName: "Create Transfer Orders Filter" })
	Filter = createSingle(Filter);
	@viewInfo({ containerName: "Orders" })
	FixedDemand = createCollection(FixedDemand);
}

export class Filter extends PXView {
	PurchDate: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassCD: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SourceSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderWeight: PXFieldState;
	OrderVolume: PXFieldState;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	mergeToolbarWith: 'ScreenToolbar',
	quickFilterFields: ["InventoryID", "SOOrder__CustomerID"]
})
export class FixedDemand extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	LocalizedPlanDescr: PXFieldState;
	@columnConfig({ hideViewLink: true })InventoryID: PXFieldState;
	InventoryID_InventoryItem_descr: PXFieldState;
	@columnConfig({ hideViewLink: true })SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })SourceSiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })DemandSiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })UOM: PXFieldState;
	OrderQty: PXFieldState;
	PlanDate: PXFieldState;
	@columnConfig({ hideViewLink: true })SOOrder__CustomerID: PXFieldState;
	SOOrder__CustomerID_BAccountR_acctName: PXFieldState;
	SOOrder__OrderNbr: PXFieldState;
	ExtWeight: PXFieldState;
	ExtVolume: PXFieldState;
}
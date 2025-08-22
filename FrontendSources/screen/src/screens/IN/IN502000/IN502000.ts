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

@graphInfo({ graphType: 'PX.Objects.IN.INUpdateStdCost', primaryView: 'Filter' })
export class IN502000 extends PXScreen {

	@viewInfo({ containerName: "Update Standard Costs Filter" })
	Filter = createSingle(Filter);
	@viewInfo({ containerName: "Items" })
	INItemList = createCollection(INItemList,);
}

export class Filter extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	PendingStdCostDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RevalueInventory: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	mergeToolbarWith: 'ScreenToolbar'
})
export class INItemList extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	InventoryID: PXFieldState;
	SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InvtAcctID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InvtSubID: PXFieldState;
	PendingStdCost: PXFieldState;
	PendingStdCostDate: PXFieldState;
	StdCost: PXFieldState;
	CuryID: PXFieldState;
	StdCostOverride: PXFieldState;
}
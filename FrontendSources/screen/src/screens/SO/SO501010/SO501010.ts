import {
	createCollection,
	createSingle,
	PXScreen, graphInfo,
	PXActionState,
	viewInfo,
	PXPageLoadBehavior,
	PXView,
	PXFieldOptions,
	PXFieldState,
	columnConfig,
	gridConfig,
	GridColumnShowHideMode,
	GridPreset
} from "client-controls";

@graphInfo({graphType: "PX.Objects.SO.ManageSalesAllocations", primaryView: "Filter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class SO501010 extends PXScreen {
	SelectItems: PXActionState;

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(SalesAllocationsFilter);
   	@viewInfo({containerName: "Details"})
	Allocations = createCollection(SalesAllocation);
   	@viewInfo({containerName: "Inventory Item List"})
	SelectedItems = createCollection(InventoryLinkFilter);
}

export class SalesAllocationsFilter extends PXView  {
	Action : PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID : PXFieldState<PXFieldOptions.CommitChanges>;
	SelectBy : PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate : PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate : PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType : PXFieldState<PXFieldOptions.CommitChanges>;
	OrderStatus : PXFieldState<PXFieldOptions.CommitChanges>;
	Priority : PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr : PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID : PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID : PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry
})
export class SalesAllocation extends PXView  {
	@columnConfig({
		allowSort: false,
		allowCheckAll: true
	})
	Selected : PXFieldState<PXFieldOptions.CommitChanges>;
	OrderPriority : PXFieldState;
	@columnConfig({hideViewLink: true})
	OrderType : PXFieldState;
	OrderNbr : PXFieldState;
	OrderStatus : PXFieldState;
	OrderDesc : PXFieldState;
	@columnConfig({hideViewLink: true})
	CustomerID : PXFieldState;
	CustomerName : PXFieldState;
	@columnConfig({hideViewLink: true})
	CustomerClassID : PXFieldState;
	@columnConfig({hideViewLink: true})
	SalesPersonID : PXFieldState;
	@columnConfig({hideViewLink: true})
	CuryID : PXFieldState;
	CuryOrderTotal : PXFieldState;
	LineNbr : PXFieldState;
	@columnConfig({hideViewLink: true})
	LineSiteID : PXFieldState;
	InventoryID : PXFieldState;
	@columnConfig({
		allowSort: false,
		allowFilter: false,
		allowShowHide: GridColumnShowHideMode.Server,
		hideViewLink: true
	})
	BaseUOM : PXFieldState;
	TranDesc : PXFieldState;
	BaseLineQty : PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM : PXFieldState;
	LineQty : PXFieldState;
	CuryLineAmt : PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.Server})
	QtyHardAvail : PXFieldState;
	@columnConfig({hideViewLink: true})
	SplitSiteID : PXFieldState;
	QtyAllocated : PXFieldState;
	QtyUnallocated : PXFieldState;
	QtyToAllocate : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		allowSort: false,
		allowFilter: false,
		allowShowHide: GridColumnShowHideMode.Server
	})
	QtyToDeallocate : PXFieldState<PXFieldOptions.CommitChanges>;
	ShipComplete : PXFieldState;
	RequestDate : PXFieldState;
	ShipDate : PXFieldState;
	OrderDate : PXFieldState;
	CancelDate : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowUpdate: false
})
export class InventoryLinkFilter extends PXView  {
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	Descr: PXFieldState;
}
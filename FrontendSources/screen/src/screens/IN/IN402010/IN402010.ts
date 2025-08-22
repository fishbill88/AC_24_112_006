import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.IntercompanyGoodsInTransitInq', primaryView: 'Filter', pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class IN402010 extends PXScreen {
	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(IntercompanyGoodsInTransitFilter);
	@viewInfo({containerName: 'Inventory Summary'})
	Results = createCollection(IntercompanyGoodsInTransitResult);

}

// Views

export class IntercompanyGoodsInTransitFilter extends PXView  {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShippedBefore: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowOverdueItems: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowItemsWithoutReceipt: PXFieldState<PXFieldOptions.CommitChanges>;
	SellingCompany: PXFieldState<PXFieldOptions.CommitChanges>;
	SellingSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchasingCompany: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchasingSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	batchUpdate: true,
	allowUpdate: false
})
export class IntercompanyGoodsInTransitResult extends PXView  {
	InventoryID: PXFieldState;
	TranDesc: PXFieldState;
	@columnConfig({hideViewLink: true}) SellingBranchID: PXFieldState;
	@columnConfig({hideViewLink: true}) SellingSiteID: PXFieldState;
	ShipmentNbr: PXFieldState;
	ShipDate: PXFieldState;
	ShipmentStatus: PXFieldState;
	RequestDate: PXFieldState;
	ShippedQty: PXFieldState;
	@columnConfig({hideViewLink: true}) UOM: PXFieldState;
	ExtCost: PXFieldState;
	DaysInTransit: PXFieldState;
	DaysOverdue: PXFieldState;
	@columnConfig({hideViewLink: true}) PurchasingBranchID: PXFieldState;
	@columnConfig({hideViewLink: true}) PurchasingSiteID: PXFieldState;
	POReceiptNbr: PXFieldState;
	ReceiptDate: PXFieldState;
}

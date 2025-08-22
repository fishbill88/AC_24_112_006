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
	gridConfig,
	linkCommand,
	PXActionState,
	handleEvent,
	CustomEventType,
	RowCssHandlerArgs,
	GridPreset
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.IN.InventorySummaryEnq', primaryView: 'Filter' })
export class IN401000 extends PXScreen {

	ViewAllocDet: PXActionState;

	@viewInfo({ containerName: "Inventory Summary Filter" })
	Filter = createSingle(Filter);
	@viewInfo({ containerName: "Inventory Summary Details" })
	ISERecords = createCollection(ISERecords);

	@handleEvent(CustomEventType.GetRowCss, { view: 'ISERecords' })
	getTransactionsRowCss(args: RowCssHandlerArgs) {
		if (args?.selector?.row?.IsTotal.value === true) {
			return 'bold-row';
		}
		return undefined;
	}
}

export class Filter extends PXView {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpandByLotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpandByCostLayerType: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemCD: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details
})
export class ISERecords extends PXView {
	@linkCommand('ViewAllocDet')
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState;
	LocationID: PXFieldState;
	CostLayerType: PXFieldState;
	QtyAvail: PXFieldState;
	QtyHardAvail: PXFieldState;
	QtyActual: PXFieldState;
	QtyNotAvail: PXFieldState;
	QtySOPrepared: PXFieldState;
	QtySOBooked: PXFieldState;
	QtySOShipping: PXFieldState;
	QtySOShipped: PXFieldState;
	QtySOBackOrdered: PXFieldState;
	QtyINIssues: PXFieldState;
	QtyINReceipts: PXFieldState;
	QtyInTransit: PXFieldState;
	QtyInTransitToSO: PXFieldState;
	QtyProductionSupplyPrepared: PXFieldState;
	QtyProductionSupply: PXFieldState;
	QtyProdFixedProdOrdersPrepared: PXFieldState;
	QtyProdFixedProdOrders: PXFieldState;
	QtyPOFixedProductionPrepared: PXFieldState;
	QtyPOFixedProductionOrders: PXFieldState;
	QtyProdFixedSalesOrdersPrepared: PXFieldState;
	QtyProdFixedSalesOrders: PXFieldState;
	QtyProductionDemandPrepared: PXFieldState;
	QtyProductionDemand: PXFieldState;
	QtyProductionAllocated: PXFieldState;
	QtyProdFixedProduction: PXFieldState;
	QtyProdFixedPurchase: PXFieldState;
	QtySOFixedProduction: PXFieldState;
	QtyInAssemblyDemand: PXFieldState;
	QtyInAssemblySupply: PXFieldState;
	QtyPOPrepared: PXFieldState;
	QtyPOOrders: PXFieldState;
	QtyPOReceipts: PXFieldState;
	QtyExpired: PXFieldState;
	QtyOnHand: PXFieldState;
	QtySOFixed: PXFieldState;
	QtyPOFixedOrders: PXFieldState;
	QtyPOFixedPrepared: PXFieldState;
	QtyPOFixedReceipts: PXFieldState;
	QtySODropShip: PXFieldState;
	QtyPODropShipOrders: PXFieldState;
	QtyPODropShipPrepared: PXFieldState;
	QtyPODropShipReceipts: PXFieldState;
	QtyFSSrvOrdPrepared: PXFieldState;
	QtyFSSrvOrdBooked: PXFieldState;
	QtyFSSrvOrdAllocated: PXFieldState;
	QtyFixedFSSrvOrd: PXFieldState;
	QtyPOFixedFSSrvOrd: PXFieldState;
	QtyPOFixedFSSrvOrdPrepared: PXFieldState;
	QtyPOFixedFSSrvOrdReceipts: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BaseUnit: PXFieldState;
	UnitCost: PXFieldState;
	TotalCost: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState;
	ExpireDate: PXFieldState;
	IsTotal: PXFieldState<PXFieldOptions.Hidden>;
}
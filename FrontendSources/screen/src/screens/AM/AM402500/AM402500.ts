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

export class Filter extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState;
	LocationID: PXFieldState;
	ProductionStatusID: PXFieldState<PXFieldOptions.CommitChanges>;
	MultiLevel: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class ProductionWhereUsed extends PXView {
	ParentInventoryID: PXFieldState;
	ParentDescr: PXFieldState;
	ParentLotSerialNbr: PXFieldState;
	ComponentInventoryID: PXFieldState;
	ComponentDescr: PXFieldState;
	ComponentLotSerialNbr: PXFieldState;
	Level: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	QtyIssued: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	SalesOrderType: PXFieldState;
	SalesOrderNbr: PXFieldState;
	InventoryID: PXFieldState;
	Descr: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	ProductionStatusID: PXFieldState;
	ComponentOrderType: PXFieldState;
	ComponentProdOrdID: PXFieldState;
	CustomerID: PXFieldState;
	CustomerName: PXFieldState;
	ScheduleStatus: PXFieldState;
	ProdDate: PXFieldState;
	ConstDate: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	ParentQty: PXFieldState;
	ParentQtyComplete: PXFieldState;
	ParentQtyScrapped: PXFieldState;
	ParentQtyRemaining: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.ProductionWhereUsedInq', primaryView: 'Filter' })
export class AM402500 extends PXScreen {
	Filter = createSingle(Filter);
	ProductionWhereUsed = createCollection(ProductionWhereUsed);
}

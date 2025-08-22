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

@graphInfo({ graphType: 'PX.Objects.AM.ProductionOrderPrintProcess', primaryView: 'Filter' })
export class AM511000 extends PXScreen {
	Filter = createSingle(ProductionOrderPrintProcessFilter);
	ProductionOrders = createCollection(PrintProductionOrders);
}

export class ProductionOrderPrintProcessFilter extends PXView {
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassCD: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	Reprint: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintWithDeviceHub: PXFieldState<PXFieldOptions.CommitChanges>;
	DefinePrinterManually: PXFieldState<PXFieldOptions.CommitChanges>;
	PrinterID: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCopies: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class PrintProductionOrders extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	SiteId: PXFieldState;
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
	StatusID: PXFieldState;
	@columnConfig({ hideViewLink: true }) ProjectID: PXFieldState;
	@columnConfig({ hideViewLink: true }) TaskID: PXFieldState;
	@columnConfig({ hideViewLink: true }) CostCodeID: PXFieldState;
	ProductionReportID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
}

import {
	graphInfo,
	linkCommand,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	columnConfig,
	gridConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ConvertItemsToEquipmentProcess', primaryView: 'Filter' })
export class FS500900 extends PXScreen {
	OpenInvoice: PXActionState;
	Filter = createSingle(StockItemsFilter);
	InventoryItems = createCollection(SoldInventoryItem);
}

export class StockItemsFilter extends PXView {
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class SoldInventoryItem extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	InventoryCD: PXFieldState;
	Descr: PXFieldState;
	LotSerialNumber: PXFieldState;
	ItemClassID: PXFieldState;
	@linkCommand("OpenInvoice")	InvoiceRefNbr: PXFieldState;
	DocDate: PXFieldState;
	CustomerID: PXFieldState;
	@columnConfig({ hideViewLink: true }) CustomerLocationID: PXFieldState;
}

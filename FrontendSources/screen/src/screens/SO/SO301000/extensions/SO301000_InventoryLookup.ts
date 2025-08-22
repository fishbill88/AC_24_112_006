import {
	SO301000
} from '../SO301000';

import {
	PXView,
	createCollection,
	createSingle,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	columnConfig,
	localizable,
	gridConfig
} from 'client-controls';

@localizable
export class InventoryLookupPanelHeaders {
	static InventoryLookup = "Inventory Lookup";
}

export interface SO301000_InventoryLookup extends SO301000 {}
export class SO301000_InventoryLookup {
	InventoryLookupPanelHeaders = InventoryLookupPanelHeaders;

	@viewInfo({containerName: "Inventory Lookup"})
	ItemFilter = createSingle(SOSiteStatusFilter);

	@viewInfo({containerName: "Inventory Lookup"})
	ItemInfo = createCollection(SOSiteStatusSelected);
}

export class SOSiteStatusFilter extends PXView {
	Inventory: PXFieldState<PXFieldOptions.CommitChanges>;
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem: PXFieldState<PXFieldOptions.CommitChanges>;

	Mode: PXFieldState<PXFieldOptions.CommitChanges>;
	HistoryDate: PXFieldState<PXFieldOptions.CommitChanges>;
	OnlyAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
	DropShipSales: PXFieldState<PXFieldOptions.CommitChanges>;

	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class SOSiteStatusSelected extends PXView {
	@columnConfig({allowCheckAll: true}) Selected: PXFieldState;
	QtySelected: PXFieldState;
	@columnConfig({hideViewLink: true}) SiteID: PXFieldState;
	@columnConfig({hideViewLink: true}) ItemClassID: PXFieldState;
	ItemClassDescription: PXFieldState;
	@columnConfig({hideViewLink: true}) PriceClassID: PXFieldState;
	PriceClassDescription: PXFieldState;
	@columnConfig({hideViewLink: true}) PreferredVendorID: PXFieldState;
	PreferredVendorDescription: PXFieldState;
	@columnConfig({hideViewLink: true}) InventoryCD: PXFieldState;
	@columnConfig({hideViewLink: true}) SubItemID: PXFieldState;
	Descr: PXFieldState;
	@columnConfig({hideViewLink: true}) SalesUnit: PXFieldState;
	QtyAvailSale: PXFieldState;
	QtyOnHandSale: PXFieldState;
	CuryID: PXFieldState;
	QtyLastSale: PXFieldState;
	CuryUnitPrice: PXFieldState;
	LastSalesDate: PXFieldState;
	DropShipLastQty: PXFieldState;
	DropShipCuryUnitPrice: PXFieldState;
	DropShipLastDate: PXFieldState;
	@columnConfig({
		allowSort: false,
		allowFilter: false
	})
	AlternateID: PXFieldState;
	@columnConfig({
		allowSort: false,
		allowFilter: false
	})
	AlternateType: PXFieldState;
	@columnConfig({
		allowSort: false,
		allowFilter: false
	})
	AlternateDescr: PXFieldState;
}

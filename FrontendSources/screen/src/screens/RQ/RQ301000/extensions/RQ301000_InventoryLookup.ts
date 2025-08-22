import {
	RQ301000
} from '../RQ301000';

import {
	PXView,
	createCollection,
	createSingle,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	columnConfig,
	gridConfig
} from 'client-controls';

export interface RQ301000_InventoryLookup extends RQ301000 {}
export class RQ301000_InventoryLookup {
	
	@viewInfo({containerName: "Inventory Lookup Header"})
	ItemFilter = createSingle(RQSiteStatusFilter);

	@viewInfo({containerName: "Inventory Lookup"})
	ItemInfo = createCollection(RQSiteStatusSelected);
}

export class RQSiteStatusFilter extends PXView  {
	Inventory: PXFieldState<PXFieldOptions.CommitChanges>;
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	OnlyAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	autoAdjustColumns: true,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class RQSiteStatusSelected extends PXView  {
	@columnConfig({allowCheckAll: true}) 
	Selected: PXFieldState;
	QtySelected: PXFieldState;
	@columnConfig({hideViewLink: true})
	SiteID: PXFieldState;
	@columnConfig({hideViewLink: true})
	ItemClassID: PXFieldState;
	ItemClassDescription: PXFieldState;
	@columnConfig({hideViewLink: true})
	PriceClassID: PXFieldState;
	PriceClassDescription: PXFieldState;
	@columnConfig({hideViewLink: true})
	PreferredVendorID: PXFieldState;
	PreferredVendorDescription: PXFieldState;
	@columnConfig({hideViewLink: true})
	InventoryCD: PXFieldState;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	@columnConfig({hideViewLink: true})
	PurchaseUnit: PXFieldState;
	QtyAvailExt: PXFieldState;
	QtyOnHandExt: PXFieldState;
	QtyPOOrdersExt: PXFieldState;
	QtyPOReceiptsExt: PXFieldState;
}
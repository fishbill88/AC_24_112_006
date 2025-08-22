import {
	RQ302000
} from '../RQ302000';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	createSingle,
	gridConfig,
	columnConfig,
	viewInfo,
	GridPreset
} from 'client-controls';

export interface RQ302000_InventoryLookup extends RQ302000 { }
export class RQ302000_InventoryLookup {
	@viewInfo({containerName: "Inventory Lookup"})
	ItemFilter = createSingle(INSiteStatusFilter);
   	@viewInfo({containerName: "Inventory Lookup"})
	ItemInfo = createCollection(RQSiteStatusSelected);
}

export class INSiteStatusFilter extends PXView  {
	Inventory : PXFieldState<PXFieldOptions.CommitChanges>;
	BarCode : PXFieldState<PXFieldOptions.CommitChanges>;
	OnlyAvailable : PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID : PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClass : PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class RQSiteStatusSelected extends PXView  {
	@columnConfig({allowCheckAll: true}) Selected : PXFieldState;
	@columnConfig({allowNull: false}) QtySelected : PXFieldState;
	@columnConfig({hideViewLink: true}) SiteID : PXFieldState;
	@columnConfig({hideViewLink: true}) ItemClassID : PXFieldState;
	ItemClassDescription : PXFieldState;
	@columnConfig({hideViewLink: true}) PriceClassID : PXFieldState;
	PriceClassDescription : PXFieldState;
	@columnConfig({hideViewLink: true}) PreferredVendorID : PXFieldState;
	PreferredVendorDescription : PXFieldState;
	InventoryCD : PXFieldState;
	SubItemID : PXFieldState;
	Descr : PXFieldState;
	@columnConfig({hideViewLink: true}) PurchaseUnit : PXFieldState;
	@columnConfig({allowNull: false}) QtyAvailExt : PXFieldState;
	@columnConfig({allowNull: false}) QtyOnHandExt : PXFieldState;
	@columnConfig({allowNull: false}) QtyPOOrdersExt : PXFieldState;
	@columnConfig({allowNull: false}) QtyPOReceiptsExt : PXFieldState;
}

import {
	PO301000
} from '../PO301000';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	featureInstalled,
	createSingle,
	gridConfig,
	columnConfig,
	viewInfo,
	GridColumnShowHideMode
} from 'client-controls';

export interface PO301000_InventoryLookup extends PO301000 { }
export class PO301000_InventoryLookup {
	@viewInfo({containerName: 'Inventory Lookup'})
	ItemFilter = createSingle(INSiteStatusFilter);
	@viewInfo({containerName: 'Inventory Lookup'})
	ItemInfo = createCollection(POSiteStatusSelected);
}


export class INSiteStatusFilter extends PXView {
	Inventory: PXFieldState<PXFieldOptions.CommitChanges>;
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	OnlyAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ allowDelete: false, allowInsert: false })
export class POSiteStatusSelected extends PXView {
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	QtySelected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	SiteCD: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ hideViewLink: true })
	ItemClassID: PXFieldState;
	ItemClassDescription: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PriceClassID: PXFieldState;
	PriceClassDescription: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PreferredVendorID: PXFieldState;
	PreferredVendorDescription: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryCD: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	SubItemCD: PXFieldState<PXFieldOptions.Hidden>;
	Descr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PurchaseUnit: PXFieldState;
	QtyAvailExt: PXFieldState;
	QtyOnHandExt: PXFieldState;
	QtyPOOrdersExt: PXFieldState;
	QtyPOReceiptsExt: PXFieldState;
	AlternateID: PXFieldState;
	AlternateType: PXFieldState;
	AlternateDescr: PXFieldState;
}

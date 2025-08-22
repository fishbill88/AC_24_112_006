import { createCollection, createSingle, PXScreen, graphInfo,
	PXActionState, viewInfo, PXView, PXFieldState, gridConfig,
	PXFieldOptions, columnConfig, headerDescription, GridPreset } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.PO.POVendorCatalogueMaint', primaryView: 'BAccount' })
export class PO201000 extends PXScreen {
	@viewInfo({containerName: 'Vendor Inventory Summary'})
	BAccount = createSingle(VendorLocation);
	@viewInfo({containerName: 'Vendor Inventory Details'})
	VendorCatalogue = createCollection(POVendorInventory);
}

export class VendorLocation extends PXView {
	@headerDescription
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@headerDescription
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	VSiteID: PXFieldState<PXFieldOptions.Disabled>;
	VLeadTime: PXFieldState<PXFieldOptions.Disabled>;
	CuryID: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class POVendorInventory extends PXView {
	ShowVendorPrices: PXActionState;

	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	IsDefault: PXFieldState;
	AllLocations: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryItem__Descr: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PurchaseUnit: PXFieldState;
	VendorInventoryID: PXFieldState;
	OverrideSettings: PXFieldState<PXFieldOptions.CommitChanges>;
	AddLeadTimeDays: PXFieldState;
	MinOrdFreq: PXFieldState;
	MinOrdQty: PXFieldState;
	MaxOrdQty: PXFieldState;
	LotSize: PXFieldState;
	ERQ: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	LastPrice: PXFieldState;
}

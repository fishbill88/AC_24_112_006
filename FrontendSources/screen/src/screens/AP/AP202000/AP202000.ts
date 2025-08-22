import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APVendorPriceMaint', primaryView: 'Filter' })
export class AP202000 extends PXScreen {

	Filter = createSingle(APVendorPriceFilter);
	Records = createCollection(APVendorPrice);

}

export class APVendorPriceFilter extends PXView {

	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClassCD: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveAsOfDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ syncPosition:true })
export class APVendorPrice extends PXView {

	@columnConfig({ hideViewLink:true })
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;

	VendorID_Vendor_AcctName: PXFieldState;
	AlternateID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID_InventoryItem_Descr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;

	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPromotionalPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	BreakQty: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesPrice: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;

	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpirationDate: PXFieldState;

}

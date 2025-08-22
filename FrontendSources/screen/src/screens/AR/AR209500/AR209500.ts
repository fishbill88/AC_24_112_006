import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, TextAlign } from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARDiscountSequenceMaint", primaryView: "Sequence",
	pageLoadBehavior: PXPageLoadBehavior.GoFirstRecord, showUDFIndicator: true
})
export class AR209500 extends PXScreen {

	@viewInfo({ containerName: "Discount Sequence Summary" })
	Sequence = createSingle(DiscountSequence);

	Discount = createSingle(ARDiscount);

	@viewInfo({ containerName: "Update Discounts" })
	UpdateSettings = createSingle(UpdateSettingsFilter);

	@viewInfo({ containerName: "Discount Breakpoints" })
	Details = createCollection(DiscountDetail);

	@viewInfo({ containerName: "Items" })
	Items = createCollection(DiscountItem);

	@viewInfo({ containerName: "Customers" })
	Customers = createCollection(DiscountCustomer);

	@viewInfo({ containerName: "Customer Price Classes" })
	CustomerPriceClasses = createCollection(DiscountCustomerPriceClass);

	@viewInfo({ containerName: "Item Price Classes" })
	InventoryPriceClasses = createCollection(DiscountInventoryPriceClass);

	@viewInfo({ containerName: "Branches" })
	Branches = createCollection(DiscountBranch);

	@viewInfo({ containerName: "Warehouses" })
	Sites = createCollection(DiscountSite);

	@viewInfo({ containerName: "Free Item" })
	CurrentSequence = createSingle(DiscountSequence);
}

export class DiscountSequence extends PXView {

	DiscountID: PXFieldState;
	DiscountSequenceID: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPromotion: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	DiscountedFor: PXFieldState<PXFieldOptions.CommitChanges>;
	BreakBy: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Prorate: PXFieldState;
	EndDate: PXFieldState;
	ShowFreeItem: PXFieldState;
	FreeItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	PendingFreeItemID: PXFieldState;
	LastFreeItemID: PXFieldState;
	UpdateDate: PXFieldState<PXFieldOptions.Disabled>;
}

export class ARDiscount extends PXView {

	showListOfItems: PXFieldState;
	ShowCustomers: PXFieldState;
	ShowCustomerPriceClass: PXFieldState;
	ShowInventoryPriceClass: PXFieldState;
	ShowBranches: PXFieldState;
	ShowSites: PXFieldState;
}

export class UpdateSettingsFilter extends PXView {

	FilterDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true
})
export class DiscountDetail extends PXView {

	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	Quantity: PXFieldState;
	Amount: PXFieldState;
	Discount: PXFieldState;
	DiscountPercent: PXFieldState;
	FreeItemQty: PXFieldState;
	PendingQuantity: PXFieldState;
	PendingAmount: PXFieldState;
	PendingDiscount: PXFieldState;
	PendingDiscountPercent: PXFieldState;
	PendingFreeItemQty: PXFieldState;
	StartDate: PXFieldState;
	LastQuantity: PXFieldState;
	LastAmount: PXFieldState;
	LastDiscount: PXFieldState;
	LastDiscountPercent: PXFieldState;
	LastFreeItemQty: PXFieldState;
	LastDate: PXFieldState;
}

export class DiscountItem extends PXView {

	@columnConfig({ format: ">CCCCC-CCCCCCCCCCCCCCC" })
	InventoryID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	InventoryItem__Descr: PXFieldState;

	UOM: PXFieldState;
	Amount: PXFieldState;
	Quantity: PXFieldState;
}

export class DiscountCustomer extends PXView {

	@columnConfig({ format: "CCCCCCCCCC", hideViewLink: true })
	CustomerID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Customer__AcctName: PXFieldState;
}

export class DiscountCustomerPriceClass extends PXView {

	@columnConfig({ format: ">aaaaaaaaaa", hideViewLink: true })
	CustomerPriceClassID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	ARPriceClass__Description: PXFieldState;
}

export class DiscountInventoryPriceClass extends PXView {

	@columnConfig({ format: ">aaaaaaaaaa", hideViewLink: true })
	InventoryPriceClassID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	INPriceClass__Description: PXFieldState;
}

export class DiscountBranch extends PXView {

	@columnConfig({ format: ">aaaaaaaaaa", hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Branch__AcctName: PXFieldState;
}

export class DiscountSite extends PXView {

	@columnConfig({ format: ">aaaaaaaaaa", hideViewLink: true })
	SiteID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	INSite__Descr: PXFieldState;
}

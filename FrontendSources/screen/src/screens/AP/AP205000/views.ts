import {
	PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridPreset
} from "client-controls";

// Views

export class VendorDiscountSequence extends PXView {
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountID: PXFieldState;
	DiscountSequenceID: PXFieldState;
	IsActive: PXFieldState;
	IsPromotion: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	DiscountedFor: PXFieldState<PXFieldOptions.CommitChanges>;
	BreakBy: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Prorate: PXFieldState;
	EndDate: PXFieldState;
}

export class APDiscountEx extends PXView {
	showListOfItems: PXFieldState;
	ShowLocations: PXFieldState;
	ShowInventoryPriceClass: PXFieldState;
}

export class UpdateSettingsFilter extends PXView {
	FilterDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ preset: GridPreset.Details, initNewRow: true, adjustPageSize: true })
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
	LastDate: PXFieldState;
	LastQuantity: PXFieldState;
	LastAmount: PXFieldState;
	LastDiscount: PXFieldState;
	LastDiscountPercent: PXFieldState;
	LastFreeItemQty: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details, adjustPageSize: true })
export class DiscountItem extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryItem__Descr: PXFieldState;
	UOM: PXFieldState;
	Amount: PXFieldState;
	Quantity: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details, adjustPageSize: true })
export class APDiscountLocation extends PXView {
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Location__Descr: PXFieldState;
}

@gridConfig({preset: GridPreset.Details,  adjustPageSize: true })
export class DiscountInventoryPriceClass extends PXView {
	InventoryPriceClassID: PXFieldState;
	INPriceClass__Description: PXFieldState;
}

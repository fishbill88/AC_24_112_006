import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	gridConfig,
	GridPagerMode
} from "client-controls";

export class Bindings extends PXView {
	ConnectorType: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	BindingName: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	IsDefault: PXFieldState;
}
export class CurrentBindingAmazon extends PXView {
	ReleaseInvoices: PXFieldState;
	Region: PXFieldState<PXFieldOptions.CommitChanges>;
	Marketplace: PXFieldState<PXFieldOptions.CommitChanges>;
	SellerPartnerId: PXFieldState<PXFieldOptions.CommitChanges>;
	SellerFulfilledOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ShippingAccount: PXFieldState;
	ShippingSubAccount: PXFieldState;
	ShipViaCodesToCarriers: PXFieldState;
	ShipViaCodesToCarrierServices: PXFieldState;
	Warehouse: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultTaxID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrentBinding extends PXView {
	LocaleName: PXFieldState;
	BindingAdministrator: PXFieldState;
	AllowedStores: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	autoAdjustColumns: true,
	allowDelete: false,
	allowInsert: false
})
export class Entities extends PXView {
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({width: 180})
	@linkCommand("Navigate")
	EntityType: PXFieldState;
	Direction: PXFieldState<PXFieldOptions.CommitChanges>;
	PrimarySystem: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxAttemptCount: PXFieldState;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true,
	autoAdjustColumns: true,
	pagerMode: GridPagerMode.InfiniteScroll
})
export class ExportLocations extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	autoAdjustColumns: true,
	autoRepaint: ["FeeMappings"],
	pagerMode: GridPagerMode.InfiniteScroll
})
export class PaymentMethods extends PXView {
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	StorePaymentMethod: PXFieldState;
	@columnConfig({ hideViewLink: true })
	StoreCurrency: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReleasePayments: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowInsert: true,
	allowDelete: true,
	pagerMode: GridPagerMode.InfiniteScroll
})
export class FeeMappings extends PXView {
	FeeType: PXFieldState<PXFieldOptions.CommitChanges>;
	EntryTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	EntryDescription: PXFieldState;
	TransactionType: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	autoAdjustColumns: true,
	pagerMode: GridPagerMode.InfiniteScroll
})
export class ShippingMappings extends PXView {
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	ShippingZone: PXFieldState;
	ShippingMethod: PXFieldState;
	CarrierID: PXFieldState;
	ZoneID: PXFieldState;
	ShipTermsID: PXFieldState;
}
export class CurrentStore extends PXView {
	AvailabilityCalcRule: PXFieldState<PXFieldOptions.CommitChanges>;
	WarehouseMode: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderTimeZone: PXFieldState;
	PostDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	SyncOrdersFrom: PXFieldState;
	GiftWrappingItemID: PXFieldState;
	GuestCustomerID: PXFieldState;
	TaxSynchronization: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultTaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
}

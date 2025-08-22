import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	gridConfig,
	GridPagerMode,
} from "client-controls";

export class Bindings extends PXView {
	ConnectorType: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	BindingName: PXFieldState<PXFieldOptions.CommitChanges>;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	IsDefault: PXFieldState;
}

export class CurrentBindingBigCommerce extends PXView {
	StoreAdminUrl: PXFieldState<PXFieldOptions.CommitChanges>;
	StoreBaseURL: PXFieldState<PXFieldOptions.CommitChanges>;
	StoreXAuthClient: PXFieldState<PXFieldOptions.CommitChanges>;
	StoreXAuthToken: PXFieldState<PXFieldOptions.CommitChanges>;
	StoreWDAVServerUrl: PXFieldState<PXFieldOptions.CommitChanges>;
	StoreWDAVClientUser: PXFieldState<PXFieldOptions.CommitChanges>;
	StoreWDAVClientPass: PXFieldState<PXFieldOptions.CommitChanges>;
}
export class CurrentStore extends PXView {
	DefaultStoreCurrency: PXFieldState;
	SupportedCurrencies: PXFieldState;
	StoreTimeZone: PXFieldState;

	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerTemplate: PXFieldState;
	CustomerNumberingID: PXFieldState;
	LocationTemplate: PXFieldState;
	LocationNumberingID: PXFieldState;
	InventoryNumberingID: PXFieldState;
	InventoryTemplate: PXFieldState;
	GuestCustomerID: PXFieldState;
	MultipleGuestAccounts: PXFieldState;

	StockItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	NonStockItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	StockSalesCategoriesIDs: PXFieldState;
	NonStockSalesCategoriesIDs: PXFieldState;
	RelatedItems: PXFieldState;
	Visibility: PXFieldState<PXFieldOptions.CommitChanges>;
	Availability: PXFieldState<PXFieldOptions.CommitChanges>;
	NotAvailMode: PXFieldState<PXFieldOptions.CommitChanges>;
	AvailabilityCalcRule: PXFieldState<PXFieldOptions.CommitChanges>;
	WarehouseMode: PXFieldState<PXFieldOptions.CommitChanges>;

	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OtherSalesOrderTypes: PXFieldState;
	ReturnOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	RefundAmountItemID: PXFieldState;
	ReasonCode: PXFieldState;
	OrderTimeZone: PXFieldState;
	PostDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	GiftCertificateItemID: PXFieldState;
	GiftWrappingItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SyncOrderNbrToStore: PXFieldState<PXFieldOptions.CommitChanges>;
	SyncOrdersFrom: PXFieldState;
	TaxSynchronization: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultTaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	UseAsPrimaryTaxZone: PXFieldState;
	TaxSubstitutionListID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategorySubstitutionListID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShippingCarrierListID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrentBinding extends PXView {
	LocaleName: PXFieldState;
	BindingAdministrator: PXFieldState;
	AllowedStores: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	autoAdjustColumns: true
})
export class Entities extends PXView {
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("Navigate")
	EntityType: PXFieldState;
	Direction: PXFieldState<PXFieldOptions.CommitChanges>;
	PrimarySystem: PXFieldState<PXFieldOptions.CommitChanges>;
	ImportRealTimeStatus: PXFieldState;
	ExportRealTimeStatus: PXFieldState;
	RealTimeMode: PXFieldState;
	MaxAttemptCount: PXFieldState;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true,
	pagerMode: GridPagerMode.InfiniteScroll
})
export class ExportLocations extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	autoAdjustColumns: true,
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
	StoreOrderPaymentMethod: PXFieldState;
	ProcessRefunds: PXFieldState;
	CreatePaymentfromOrder: PXFieldState;
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

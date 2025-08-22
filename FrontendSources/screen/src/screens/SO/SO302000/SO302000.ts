import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	headerDescription,
	PXFieldOptions,
	columnConfig,
	PXActionState,
	ICurrencyInfo,
	ScreenUpdateParams,
	gridConfig,
	localizable
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.SO.SOShipmentEntry', primaryView: 'Document', udfTypeField: "ShipmentType", bpEventsIndicator: true, showUDFIndicator: true })
export class SO302000 extends PXScreen {

	AddSO: PXActionState;
	AddSOCancel: PXActionState;
	ShopRates: PXActionState;
	AddressLookup: PXActionState;
	SOShipmentLineSplittingExtension_GenerateNumbers: PXActionState;

	@viewInfo({ containerName: "Shipment Summary" })
	Document = createSingle(SOShipmentHeader);

	@viewInfo({ containerName: "Shipment Information" })
	CurrentDocument = createSingle(SOShipment);

	@viewInfo({containerName: "Currency rate"})
	_SOShipment_CurrencyInfo_ = createSingle(CurrencyInfo);

	@viewInfo({ containerName: "Details" })
	Transactions = createCollection(Transactions);

	@viewInfo({ containerName: "Orders" })
	OrderList = createCollection(OrderList);

	@viewInfo({ containerName: "Line Details Header" })
	SOShipmentLineSplittingExtension_LotSerOptions = createSingle(LineSplittingHeader);

	@viewInfo({ containerName: "Line Details" })
	splits = createCollection(SOShipLineSplit);

	@viewInfo({ containerName: "Add Order Header" })
	addsofilter = createSingle(addsofilter);

	@viewInfo({ containerName: "Add Orders" })
	soshipmentplan = createCollection(soshipmentplan);

	@viewInfo({ containerName: "Ship-To Contact" })
	Shipping_Contact = createSingle(Shipping_Contact);

	@viewInfo({ containerName: "Ship-To Address" })
	Shipping_Address = createSingle(Shipping_Address);

	@viewInfo({ containerName: "Carrier Rates" })
	CarrierRates = createCollection(CarrierRates);

	@viewInfo({ containerName: "Packages - Rates" })
	PackagesForRates = createCollection(PackagesForRates);

	@viewInfo({ containerName: "Packages" })
	Packages = createCollection(Packages);

	@viewInfo({ containerName: "Contents of Selected Package" })
	PackageDetailSplit = createCollection(PackageDetailSplit);
}

@localizable
export class NullTextValues {
	static Split = "<SPLIT>";
}

export class SOShipmentHeader extends PXView {
	ShipmentNbr: PXFieldState;
	ShipmentType: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	Operation: PXFieldState;
	ShipDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@headerDescription
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState;
	CurrentWorksheetNbr: PXFieldState;
	ShipmentQty: PXFieldState<PXFieldOptions.Readonly>;
	ControlQty: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipmentWeight: PXFieldState<PXFieldOptions.Readonly>;
	ShipmentVolume: PXFieldState<PXFieldOptions.Readonly>;
	PackageWeight: PXFieldState<PXFieldOptions.Readonly>;
	PackageCount: PXFieldState<PXFieldOptions.Readonly>;
	ShipmentDesc: PXFieldState;
}

export class SOShipment extends PXView {
	BrokerContactID: PXFieldState;

	IntercompanyPOReceiptNbr: PXFieldState;
	ExcludeFromIntercompanyProc: PXFieldState;

	ShipVia: PXFieldState<PXFieldOptions.CommitChanges>;
	WillCall: PXFieldState;
	FreightClass: PXFieldState;
	FOBPoint: PXFieldState;
	ShipTermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsOfSale: PXFieldState;
	DHLBillingRef: PXFieldState;
	SkipAddressVerification: PXFieldState;
	DeliveryConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;
	EndorsementService: PXFieldState<PXFieldOptions.CommitChanges>;
	Resedential: PXFieldState;
	SaturdayDelivery: PXFieldState;
	UseCustomerAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	Insurance: PXFieldState<PXFieldOptions.CommitChanges>;
	GroundCollect: PXFieldState;
	CuryID: PXFieldState;
	CuryFreightCost: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightAmountSource: PXFieldState;
	OverrideFreightAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryFreightAmt: PXFieldState<PXFieldOptions.CommitChanges>;

	ShipmentWeight: PXFieldState<PXFieldOptions.Readonly>;
	PackageWeight: PXFieldState<PXFieldOptions.Readonly>;

	Installed: PXFieldState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false
})
export class Transactions extends PXView {
	SOShipmentLineSplittingExtension_ShowSplits: PXActionState;
	SelectSO: PXActionState;
	InventorySummary: PXActionState;

	Availability: PXFieldState;
	ShipmentNbr: PXFieldState;
	LineNbr: PXFieldState;
	OrigOrderType: PXFieldState;
	OrigOrderNbr: PXFieldState;
	OrigLineNbr: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsFree: PXFieldState;
	SiteID: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	LocationID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	AssociatedOrderLineNbr: PXFieldState;
	GiftMessage: PXFieldState;
	ShippedQty: PXFieldState;
	BaseShippedQty: PXFieldState;
	OriginalShippedQty: PXFieldState;
	OrigOrderQty: PXFieldState;
	OpenOrderQty: PXFieldState;
	UnassignedQty: PXFieldState;
	PickedQty: PXFieldState;
	PackedQty: PXFieldState;
	CompleteQtyMin: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	LotSerialNbr: PXFieldState;
	ShipComplete: PXFieldState;
	ExpireDate: PXFieldState;
	ReasonCode: PXFieldState;
	TranDesc: PXFieldState;
	BlanketNbr: PXFieldState;
}

export class LineSplittingHeader extends PXView {
	UnassignedQty: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState;
	StartNumVal: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	initNewRow: true
})
export class SOShipLineSplit extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	ExpireDate: PXFieldState;
	PickedQty: PXFieldState;
	PackedQty: PXFieldState;
	InventoryID_InventoryItem_descr: PXFieldState;
}

export class addsofilter extends PXView {
	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false
})
export class soshipmentplan extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	SOLine__LineNbr: PXFieldState;
	SOLine__InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SOLineSplit__SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SOLineSplit__UOM: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SOLineSplit__LotSerialNbr: PXFieldState;
	PlanDate: PXFieldState;
	SOLineSplit__Qty: PXFieldState;
	SOLine__TranDesc: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false,
	allowUpdate: false
})
export class OrderList extends PXView {
	ShipmentNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OrderType: PXFieldState<PXFieldOptions.Readonly>;
	OrderNbr: PXFieldState<PXFieldOptions.Readonly>;
	ShipmentQty: PXFieldState;
	ShipmentWeight: PXFieldState;
	ShipmentVolume: PXFieldState;
	InvoiceType: PXFieldState<PXFieldOptions.Readonly>;
	InvoiceNbr: PXFieldState<PXFieldOptions.Readonly>;
	InvtDocType: PXFieldState<PXFieldOptions.Readonly>;
	InvtRefNbr: PXFieldState<PXFieldOptions.Readonly>;
}

export class Shipping_Contact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Shipping_Address extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
	IsValidated: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false
})
export class CarrierRates extends PXView {
	RefreshRates: PXActionState;

	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Method: PXFieldState;
	Description: PXFieldState;
	Amount: PXFieldState;
	DaysInTransit: PXFieldState;
	DeliveryDate: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true
})
export class PackagesForRates extends PXView {
	RecalculatePackages: PXActionState;

	@columnConfig({ hideViewLink: true })
	BoxID: PXFieldState<PXFieldOptions.CommitChanges>;
	BoxDescription: PXFieldState;
	AllowOverrideDimension: PXFieldState;
	Length: PXFieldState;
	Width: PXFieldState;
	Height: PXFieldState;
	LinearUOM: PXFieldState;
	WeightUOM: PXFieldState;
	Weight: PXFieldState;
	BoxWeight: PXFieldState;
	NetWeight: PXFieldState;
	DeclaredValue: PXFieldState;
	COD: PXFieldState;
	StampsAddOns: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	autoRepaint: ['PackageDetailSplit']
})
export class Packages extends PXView {
	RecalculatePackages: PXActionState;

	Confirmed: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BoxID: PXFieldState<PXFieldOptions.CommitChanges>;
	PackageType: PXFieldState;
	Description: PXFieldState;
	AllowOverrideDimension: PXFieldState;
	Length: PXFieldState;
	Width: PXFieldState;
	Height: PXFieldState;
	LinearUOM: PXFieldState;
	Weight: PXFieldState<PXFieldOptions.CommitChanges>;
	WeightUOM: PXFieldState;
	DeclaredValue: PXFieldState;
	COD: PXFieldState;
	StampsAddOns: PXFieldState;
	TrackNumber: PXFieldState;
	TrackUrl: PXFieldState;
	ReturnTrackNumber: PXFieldState;
	CustomRefNbr1: PXFieldState;
	CustomRefNbr2: PXFieldState;
	ContentType: PXFieldState<PXFieldOptions.CommitChanges>;
	EELPFC: PXFieldState;
	ContentTypeDesc: PXFieldState;
	CertificateNumber: PXFieldState;
	InvoiceNumber: PXFieldState;
	LicenseNumber: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	initNewRow: true
})
export class PackageDetailSplit extends PXView {
	@columnConfig({ hideViewLink: true })
	ShipmentSplitLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	PackedQty: PXFieldState;
}

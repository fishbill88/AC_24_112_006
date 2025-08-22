import
{
	PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState, PXJoined, commitChanges, localizable, GridColumnDisplayMode, GridPreset
} from "client-controls";

@localizable
export class NullTextValues {
	static Split = "<SPLIT>";
	static Zero = "0.0";
}

export class SOOrderHeader extends PXView {
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	OrderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpireDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerRefNbr: PXFieldState;
	CuryInfoID: PXFieldState;

	@headerDescription
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DestinationSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderDesc: PXFieldState;

	OrderQty: PXFieldState<PXFieldOptions.Disabled>;
	CuryDetailExtPriceTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineDiscTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscTot: PXFieldState<PXFieldOptions.Disabled>;
	CuryFreightTot: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrderTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryControlTotal: PXFieldState;

	ArePaymentsApplicable: PXFieldState<PXFieldOptions.CommitChanges>;
	IsFSIntegrated: PXFieldState<PXFieldOptions.Disabled>;

	ShowDiscountsTab: PXFieldState;
	ShowShipmentsTab: PXFieldState;
	ShowOrdersTab: PXFieldState;
}

export class SOOrder extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchBaseCuryID: PXFieldState;
	DisableAutomaticTaxCalculation: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideTaxZone: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalTaxExemptionNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	AvalaraCustomerUsageType: PXFieldState;
	BillSeparately: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceNbr: PXFieldState;
	InvoiceDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	DueDate: PXFieldState;
	DiscDate: PXFieldState;
	FinPeriodID: PXFieldState;

	OverridePrepayment: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentReqPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaymentReqAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentReqSatisfied: PXFieldState;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;

	OrigOrderType: PXFieldState<PXFieldOptions.Disabled>;
	OrigOrderNbr: PXFieldState<PXFieldOptions.Disabled>;
	Emailed: PXFieldState<PXFieldOptions.Disabled>;
	Printed: PXFieldState<PXFieldOptions.Disabled>;

	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisableAutomaticDiscountCalculation: PXFieldState<PXFieldOptions.CommitChanges>;

	CuryUnreleasedPaymentAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryCCAuthorizedAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryPaidAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryPaymentTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryBilledPaymentTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTransferredToChildrenPaymentTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnpaidBalance: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnbilledOrderTotal: PXFieldState<PXFieldOptions.Disabled>;
	RiskStatus: PXFieldState<PXFieldOptions.Disabled>;

	ShipVia: PXFieldState<PXFieldOptions.CommitChanges>;
	WillCall: PXFieldState;
	DeliveryConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;
	EndorsementService: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightClass: PXFieldState;
	FOBPoint: PXFieldState;
	Priority: PXFieldState;
	ShipTermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	Resedential: PXFieldState;
	SaturdayDelivery: PXFieldState;
	Insurance: PXFieldState;
	UseCustomerAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	GroundCollect: PXFieldState<PXFieldOptions.CommitChanges>;
	IntercompanyPOType: PXFieldState;
	IntercompanyPONbr: PXFieldState<PXFieldOptions.Disabled>;
	IntercompanyPOReturnNbr: PXFieldState<PXFieldOptions.Disabled>;
	ShipDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipSeparately: PXFieldState;
	ShipComplete: PXFieldState<PXFieldOptions.CommitChanges>;
	CancelDate: PXFieldState;
	Cancelled: PXFieldState<PXFieldOptions.Disabled>;
	DefaultSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	//Freight Info
	OrderWeight: PXFieldState<PXFieldOptions.Disabled>;
	OrderVolume: PXFieldState<PXFieldOptions.Disabled>;
	PackageWeight: PXFieldState<PXFieldOptions.Disabled>;
	CuryFreightCost: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightCostIsValid: PXFieldState;
	OverrideFreightAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightAmountSource: PXFieldState;
	CuryFreightAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPremiumFreightAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightTaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	//VAT Totals
	CuryVatExemptTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	//Order Totals
	AMCuryEstimateTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryGoodsExtPriceTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryMiscExtPriceTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineDiscTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscTot: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	MarginPct: PXFieldState<PXFieldOptions.Disabled>;
	CuryMarginAmt: PXFieldState<PXFieldOptions.Disabled>;
	//Shipment and Invoice Info
	AMEstimateQty: PXFieldState<PXFieldOptions.Disabled>;
	BlanketOpenQty: PXFieldState<PXFieldOptions.Disabled>;
	OpenOrderQty: PXFieldState<PXFieldOptions.Disabled>;
	CuryOpenOrderTotal: PXFieldState<PXFieldOptions.Disabled>;
	UnbilledOrderQty: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnrefundedBalance: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	statusField: "Availability"
})
export class SOLine extends PXView {
	ShowItems: PXActionState;
	ShowMatrixPanel: PXActionState;
	AddInvoice: PXActionState;
	AddBlanketLine: PXActionState;
	SOOrderLineSplittingExtension_ShowSplits: PXActionState;
	POSupplyOK: PXActionState;
	ItemAvailability: PXActionState;
	ConfigureEntry: PXActionState;
	linkProdOrder: PXActionState;

	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Availability: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ExcludedFromExport: PXFieldState;
	IsConfigurable: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	LineNbr: PXFieldState;
	AssociatedOrderLineNbr: PXFieldState;
	GiftMessage: PXFieldState;
	SortOrder: PXFieldState;
	LineType: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	InvoiceNbr: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	Operation: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		type: GridColumnType.Icon,
		allowShowHide: GridColumnShowHideMode.Server,
		allowFilter: false,
		allowSort: false,
		suppressExport: true
	})
	IsStockItem: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	@linkCommand("AddRelatedItems")
	RelatedItems: PXFieldState;
	SubstitutionRequired: PXFieldState;
	IsSpecialOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentAction: PXFieldState<PXFieldOptions.CommitChanges>;
	Comment: PXFieldState;
	SMEquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewEquipmentLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentComponentLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("SOLine$RelatedDocument$Link")
	RelatedDocument: PXFieldState;
	SDSelected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	AutoCreateIssueLine: PXFieldState;
	IsFree: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server,
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	LocationID: PXFieldState;
	TranDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderQty: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseOrderQty: PXFieldState;
	QtyOnOrders: PXFieldState;
	BlanketOpenQty: PXFieldState;
	UnshippedQty: PXFieldState;
	ShippedQty: PXFieldState<PXFieldOptions.Disabled>;
	OpenQty: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnitCost: PXFieldState;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscPct: PXFieldState;
	CuryDiscAmt: PXFieldState;
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountSequenceID: PXFieldState;
	ManualDisc: PXFieldState<PXFieldOptions.CommitChanges>;
	AutomaticDiscountsDisabled: PXFieldState;
	@columnConfig({ nullText: NullTextValues.Zero })
	CuryDiscPrice: PXFieldState;
	SkipLineDiscounts: PXFieldState;
	@columnConfig({ nullText: NullTextValues.Zero })
	AvgCost: PXFieldState;
	CuryLineAmt: PXFieldState;
	MarginPct: PXFieldState;
	CuryMarginAmt: PXFieldState;
	@columnConfig({
		nullText: NullTextValues.Split
	})
	SchedOrderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID_Location_descr: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ShipVia: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	FOBPoint: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ShipTermsID: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ShipZoneID: PXFieldState;
	@columnConfig({
		nullText: NullTextValues.Split
	})
	SchedShipDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	DRTermStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DRTermEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnbilledAmt: PXFieldState<PXFieldOptions.Disabled>;
	RequestDate: PXFieldState;
	ShipDate: PXFieldState;
	ShipComplete: PXFieldState;
	CompleteQtyMin: PXFieldState;
	CompleteQtyMax: PXFieldState;
	Completed: PXFieldState<PXFieldOptions.CommitChanges>;
	POCreate: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPOLinkAllowed: PXFieldState;
	POSource: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		nullText: NullTextValues.Split
	})
	POCreateDate: PXFieldState<PXFieldOptions.CommitChanges>;
	POOrderNbr: PXFieldState;
	POOrderStatus: PXFieldState;
	POLineNbr: PXFieldState;
	POLinkActive: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		hideViewLink: true,
		allowShowHide: GridColumnShowHideMode.Server,
		nullText: NullTextValues.Split
	})
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	ExpireDate: PXFieldState;
	ReasonCode: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SalesPersonID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;
	AvalaraCustomerUsageType: PXFieldState;
	Commissionable: PXFieldState<PXFieldOptions.CommitChanges>;
	BlanketNbr: PXFieldState;
	AlternateID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SalesSubID: PXFieldState;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryUnitPriceDR: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	DiscPctDR: PXFieldState;
	AMProdCreate: PXFieldState<PXFieldOptions.CommitChanges>;
	AMorderType: PXFieldState;
	AMProdOrdID: PXFieldState;
	AMEstimateID: PXFieldState;
	AMEstimateRevisionID: PXFieldState;
	AMParentLineNbr: PXFieldState;
	AMIsSupplemental: PXFieldState;
	AMConfigKeyID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class LineSplittingHeader extends PXView {
	UnassignedQty: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState;
	StartNumVal: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true
})
export class SOLineSplit extends PXView {
	@columnConfig({ hideViewLink: true })
	SplitLineNbr: PXFieldState;
	ParentSplitLineNbr: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	SchedOrderDate: PXFieldState;
	SchedShipDate: PXFieldState;
	CustomerOrderNbr: PXFieldState;
	ShipDate: PXFieldState;
	IsAllocated: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Completed: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	QtyOnOrders: PXFieldState;
	ShippedQty: PXFieldState;
	ReceivedQty: PXFieldState;
	BlanketOpenQty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	ExpireDate: PXFieldState;
	POCreate: PXFieldState<PXFieldOptions.CommitChanges>;
	POCreateDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("SOLineSplit$RefNoteID$Link")
	RefNoteID: PXFieldState;
}

export class SOShippingAddress extends PXView {
	AddressID: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	AddressLine3: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class SOShippingContact extends PXView {
	ContactID: PXFieldState;
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SOBillingAddress extends PXView {
	AddressID: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	AddressLine3: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class SOBillingContact extends PXView {
	ContactID: PXFieldState;
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SOTaxTranTax {
	TaxType: PXFieldState;
	PendingTax: PXFieldState;
	ReverseTax: PXFieldState;
	ExemptTax: PXFieldState;
	StatisticalTax: PXFieldState;
}

export class SOTaxTran extends PXView {
	@columnConfig({allowUpdate: false})
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	TaxRate: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxableAmt: PXFieldState;
	CuryExemptedAmt: PXFieldState;
	TaxUOM: PXFieldState;

	TaxableQty: PXFieldState;
	CuryTaxAmt: PXFieldState;
	Tax: SOTaxTranTax;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class SOSalesPerTran extends PXView {
	@columnConfig({hideViewLink: true})
	SalespersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	CommnPct: PXFieldState;
	@columnConfig({allowUpdate: false})
	CuryCommnAmt: PXFieldState;
	@columnConfig({allowUpdate: false})
	CuryCommnblAmt: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
})
export class SOOrderShipment extends PXView {
	ShipmentType: PXFieldState;
	@columnConfig({allowUpdate: false})
	ShipmentNbr: PXFieldState;
	@linkCommand("SOOrderShipment~DisplayShippingRefNoteID~Link")
	DisplayShippingRefNoteID: PXFieldState;
	SOShipment__StatusIsNull: PXFieldState;
	@columnConfig({allowUpdate: false})
	Operation: PXFieldState;
	@columnConfig({allowUpdate: false})
	OrderType: PXFieldState<PXFieldOptions.Disabled>;
	@columnConfig({allowUpdate: false})
	OrderNbr: PXFieldState<PXFieldOptions.Disabled>;
	ShipDate: PXFieldState;
	ShipmentQty: PXFieldState;
	ShipmentWeight: PXFieldState;
	ShipmentVolume: PXFieldState;
	@columnConfig({allowUpdate: false})
	InvoiceType: PXFieldState;
	@columnConfig({allowUpdate: false})
	InvoiceNbr: PXFieldState;
	@columnConfig({allowUpdate: false})
	InvtDocType: PXFieldState;
	@columnConfig({allowUpdate: false})
	InvtRefNbr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	adjustPageSize: true,
	allowUpdate: false
})
export class SOBlanketOrderDisplayLink extends PXView {
	@columnConfig({hideViewLink: true})
	CustomerLocationID: PXFieldState;
	@columnConfig({allowUpdate: false})
	@linkCommand("ViewChildOrder")
	OrderNbr: PXFieldState;
	@columnConfig({allowUpdate: false})
	OrderDate: PXFieldState;
	OrderStatus: PXFieldState;
	OrderedQty: PXFieldState;
	CuryOrderedAmt: PXFieldState;
	ShipmentType: PXFieldState;
	@columnConfig({allowUpdate: false})
	ShipmentNbr: PXFieldState;
	@linkCommand("SOBlanketOrderDisplayLink~DisplayShippingRefNoteID~Link")
	DisplayShippingRefNoteID: PXFieldState;
	@columnConfig({allowUpdate: false})
	ShipmentDate: PXFieldState;
	ShipmentStatus: PXFieldState;
	@columnConfig({allowUpdate: false})
	ShippedQty: PXFieldState;
	@columnConfig({allowUpdate: false})
	InvoiceType: PXFieldState;
	@columnConfig({allowUpdate: false})
	InvoiceNbr: PXFieldState;
	@columnConfig({allowUpdate: false})
	InvoiceDate: PXFieldState;
	InvoiceStatus: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	adjustPageSize: true
})
export class OpenBlanketSOLineSplit extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	@columnConfig({hideViewLink: true})
	OrderType: PXFieldState<PXFieldOptions.Disabled>;
	@columnConfig({hideViewLink: true})
	OrderNbr: PXFieldState<PXFieldOptions.Disabled>;
	SchedOrderDate: PXFieldState;
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState;
	TranDesc: PXFieldState;
	@columnConfig({hideViewLink: true})
	SiteID: PXFieldState;
	CustomerOrderNbr: PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState;
	BlanketOpenQty: PXFieldState;
	@columnConfig({hideViewLink: true})
	CustomerLocationID: PXFieldState;
	@columnConfig({hideViewLink: true})
	TaxZoneID: PXFieldState;
}

@gridConfig(
	{
		preset: GridPreset.Details,
		initNewRow: true,
		adjustPageSize: true,
		wrapToolbar: true
	}
)
export class SOAdjustments extends PXView {
	CreateDocumentPayment: PXActionState;
	CreateOrderPrepayment: PXActionState;
	CaptureDocumentPayment: PXActionState;
	VoidDocumentPayment: PXActionState;
	ImportDocumentPayment: PXActionState;
	CreateDocumentRefund: PXActionState;

	AdjgDocType: PXFieldState;
	@linkCommand("ViewPayment")
	AdjgRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	BlanketNbr: PXFieldState;
	CuryAdjdAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAdjdBilledAmt: PXFieldState;
	CuryAdjdTransferredToChildrenAmt: PXFieldState;
	@columnConfig({allowUpdate: false})
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	@columnConfig({allowUpdate: false})
	ARPayment__Status: PXFieldState<PXFieldOptions.Disabled>;
	ExtRefNbr: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		allowUpdate: false
	})
	PaymentMethodID: PXFieldState<PXFieldOptions.Disabled>;
	@columnConfig({hideViewLink: true})
	CashAccountID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	@columnConfig({hideViewLink: true})
	ARPayment__CuryID: PXFieldState;
	ExternalTransaction__ProcStatus: PXFieldState;
	CanVoid: PXFieldState;
	CanCapture: PXFieldState;
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

export class SOParamFilter extends PXView {
	ShipDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@selectorSettings("INSite__SiteCD", "INSite__descr")
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class RecalcDiscountsFilter extends PXView {
	RecalcTarget: PXFieldState;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDocGroupDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	CalcDiscountsOnLinesWithDisabledAutomaticDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CopyParamFilter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	AMIncludeEstimate: PXFieldState<PXFieldOptions.CommitChanges>;
	CopyConfigurations: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SOLinePOLink extends PXView {
	POSource: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	POSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	adjustPageSize: true
})
export class LinkedPOLines extends PXView {
	Selected: PXFieldState;
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	VendorRefNbr: PXFieldState;
	LineType: PXFieldState;
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState;
	VendorID: PXFieldState;
	VendorID_Vendor_AcctName: PXFieldState;
	PromisedDate: PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState;
	OrderQty: PXFieldState;
	OpenQty: PXFieldState;
	TranDesc: PXFieldState;
}

export class AddInvoiceHeader extends PXView {
	ARDocType: PXFieldState<PXFieldOptions.CommitChanges>;
	ARRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Expand: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	adjustPageSize: true,
})
export class AddInvoiceDetails extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	@columnConfig({hideViewLink: true})
	InventoryID: PXFieldState;
	TranDesc: PXFieldState;
	@columnConfig({hideViewLink: true}) ComponentID: PXFieldState;
	ComponentDesc: PXFieldState;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState;
	LotSerialNbr: PXFieldState;
	@columnConfig({hideViewLink: true})
	UOM: PXFieldState;
	QtyAvailForReturn: PXFieldState;
	QtyToReturn: PXFieldState;
	Qty: PXFieldState;
	QtyReturned: PXFieldState;
	SOOrderDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) SOOrderType: PXFieldState;
	@columnConfig({ hideViewLink: true }) SOOrderNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) ARTranDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) ARDocType: PXFieldState;
	@columnConfig({ hideViewLink: true }) ARRefNbr: PXFieldState;
	@columnConfig({hideViewLink: true})
	SiteID: PXFieldState;
	@columnConfig({hideViewLink: true})
	LocationID: PXFieldState;
	@columnConfig({hideViewLink: true})
	DropShip: PXFieldState;
}

export class SOQuickPayment extends PXView {
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRefundAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDesc: PXFieldState;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefTranExtNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	NewCard: PXFieldState<PXFieldOptions.CommitChanges>;
	NewAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	SaveCard: PXFieldState<PXFieldOptions.CommitChanges>;
	SaveAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID: PXFieldState;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ImportExternalTran extends PXView {
	TranNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	adjustPageSize: true
})
export class Relations extends PXView {
	Role: PXFieldState<PXFieldOptions.CommitChanges>;
	IsPrimary: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ displayMode: GridColumnDisplayMode.Text })
	@linkCommand("RelationsViewTargetDetails")
	TargetNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	@linkCommand("RelationsViewEntityDetails")
	EntityID: PXFieldState<PXFieldOptions.CommitChanges>;
	Name: PXFieldState;
	@columnConfig({allowUpdate: false})
	@linkCommand("RelationsViewContactDetails")
	ContactID: PXFieldState;
	Email: PXFieldState;
	AddToCC: PXFieldState<PXFieldOptions.CommitChanges>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({hideViewLink: true})
	CreatedByID: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({hideViewLink: true})
	LastModifiedByID: PXFieldState<PXFieldOptions.Hidden>;
}

export class ShopForRatesHeader extends PXView {
	OrderWeight: PXFieldState;
	PackageWeight: PXFieldState;
	IsManualPackage: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
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
	preset: GridPreset.Details,
	adjustPageSize: true,
	autoAdjustColumns: true
})
export class Packages extends PXView {
	RecalculatePackages: PXActionState;
	@columnConfig({hideViewLink: true})
	BoxID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	SiteID: PXFieldState;
	Length: PXFieldState;
	Width: PXFieldState;
	Height: PXFieldState;
	LinearUOM: PXFieldState;
	WeightUOM: PXFieldState;
	Weight: PXFieldState;
	BoxWeight: PXFieldState;
	GrossWeight: PXFieldState;
	DeclaredValue: PXFieldState;
	COD: PXFieldState;
	StampsAddOns: PXFieldState;
}

export class BlanketTaxZoneOverrideFilter extends PXView {
	TaxZoneID: PXFieldState;
}

import {
	PXView,
	PXFieldState,
	gridConfig,
	ICurrencyInfo,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	GridColumnShowHideMode,
	GridColumnType,
	PXActionState,
	localizable
} from 'client-controls';

@localizable
export class NullTextValues {
	static Split = "<SPLIT>";
}

export class ARInvoice extends PXView  {
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceNbr: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDesc: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	DueDate: PXFieldState;
	DiscDate: PXFieldState;
	CuryDetailExtPriceTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineDiscTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscTot: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryFreightTot: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrigDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ARInvoiceCurrent extends PXView  {
	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryCommnblAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryCommnAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryFreightAmt: PXFieldState;
	CuryPremiumFreightAmt: PXFieldState;
	BatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARSubID: PXFieldState;
	OrigRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	CorrectionRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	DontPrint: PXFieldState<PXFieldOptions.CommitChanges>;
	Printed: PXFieldState<PXFieldOptions.Disabled>;
	DontEmail: PXFieldState<PXFieldOptions.CommitChanges>;
	Emailed: PXFieldState<PXFieldOptions.Disabled>;
	DisableAutomaticTaxCalculation: PXFieldState;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalTaxExemptionNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	AvalaraCustomerUsageType: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryVatTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatExemptTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscountedDocTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscountedTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscountedPrice: PXFieldState<PXFieldOptions.Disabled>;
	MultiShipAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeliveryMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnreleasedPaymentAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryCCAuthorizedAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryPaidAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryPaymentTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryBalanceWOTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnpaidBalance: PXFieldState<PXFieldOptions.Disabled>;
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
	initNewRow: true,
	syncPosition: true
})
export class ARTran extends PXView  {
	SelectShipment: PXActionState;
	SelectSOLine: PXActionState;
	SelectARTran: PXActionState;
	ViewSchedule: PXActionState;
	ResetOrder: PXActionState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;
	TranType: PXFieldState;
	RefNbr: PXFieldState;
	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
	@columnConfig({allowUpdate: false})
	LineType: PXFieldState;
	@columnConfig({allowUpdate: false})
	SOShipmentNbr: PXFieldState;
	@columnConfig({allowUpdate: false})
	SOOrderType: PXFieldState;
	@columnConfig({allowUpdate: false})
	SOOrderNbr: PXFieldState;
	AssociatedOrderLineNbr: PXFieldState;
	GiftMessage: PXFieldState;
	@linkCommand('ViewItem')
	InventoryID: PXFieldState;
	@columnConfig({
		type: GridColumnType.Icon,
		allowShowHide: GridColumnShowHideMode.Server,
		allowFilter: false,
		allowSort: false,
		suppressExport: true
	})
	@linkCommand("AddRelatedItems")
	RelatedItems: PXFieldState;
	SubstitutionRequired: PXFieldState;
	@columnConfig({
		hideViewLink: true,
		nullText: NullTextValues.Split
	})
	SubItemID: PXFieldState;
	ReplaceSMEquipmentID: PXFieldState<PXFieldOptions.Hidden>;
	EquipmentAction: PXFieldState<PXFieldOptions.CommitChanges>;
	Comment: PXFieldState<PXFieldOptions.CommitChanges>;
	SMEquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewEquipmentLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentComponentLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand('ARTran$RelatedDocument$Link')
	RelatedDocument: PXFieldState<PXFieldOptions.Hidden>;
	TranDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState;
	@columnConfig({
		nullText: NullTextValues.Split,
		hideViewLink: true
	})
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseQty: PXFieldState;
	@columnConfig({
		nullText: NullTextValues.Split,
		hideViewLink: true
	})
	UOM: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpireDate: PXFieldState;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscPct: PXFieldState;
	CuryDiscAmt: PXFieldState;
	ManualDisc: PXFieldState;
	SkipLineDiscounts: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DiscountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DiscountSequenceID: PXFieldState;
	CuryTranAmt: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;
	AccountID_Account_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ExpenseAccrualAccountID: PXFieldState;
	ExpenseAccrualAccountID_Account_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ExpenseAccrualSubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ExpenseAccountID: PXFieldState;
	ExpenseAccountID_Account_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ExpenseSubID: PXFieldState;
	CostBasisNull: PXFieldState;
	CuryAccruedCost: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SalesPersonID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DeferredCode: PXFieldState<PXFieldOptions.CommitChanges>;
	DRTermStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DRTermEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	DefScheduleID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;
	AvalaraCustomerUsageType: PXFieldState;
	Commissionable: PXFieldState;
	@columnConfig({allowUpdate: false})
	SOOrderLineNbr: PXFieldState;
	@columnConfig({allowUpdate: false})
	OrigInvoiceType: PXFieldState;
	@columnConfig({allowUpdate: false})
	OrigInvoiceNbr: PXFieldState;
	@columnConfig({allowUpdate: false})
	OrigInvoiceLineNbr: PXFieldState;
	@columnConfig({allowUpdate: false})
	InvtDocType: PXFieldState;
	@columnConfig({allowUpdate: false})
	InvtRefNbr: PXFieldState;
	BlanketNbr: PXFieldState;
}

export class TaxTran extends PXView  {
	@columnConfig({
		allowUpdate: false,
		hideViewLink: true
	})
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false})
	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	CuryExemptedAmt: PXFieldState;
	TaxUOM: PXFieldState;
	TaxableQty: PXFieldState;
	CuryTaxAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
	CuryDiscountedTaxableAmt: PXFieldState;
	CuryDiscountedPrice: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false
})
export class ARSalesPerTran extends PXView  {
	@columnConfig({ hideViewLink: true })
	SalespersonID: PXFieldState;
	CommnPct: PXFieldState;
	CuryCommnAmt: PXFieldState;
	CuryCommnblAmt: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false
})
export class SOFreightDetail extends PXView  {
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	@columnConfig({allowUpdate: false})
	ShipmentNbr: PXFieldState;
	ShipmentType: PXFieldState;
	ShipTermsID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ShipZoneID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ShipVia: PXFieldState;
	Weight: PXFieldState;
	Volume: PXFieldState;
	CuryLineTotal: PXFieldState;
	CuryFreightCost: PXFieldState;
	CuryFreightAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPremiumFreightAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTotalFreightAmt: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;
	AccountID_Account_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;
	TaskID: PXFieldState;
	TaxCategoryID: PXFieldState;
}

export class SOInvoice extends PXView  {
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState;
}

export class ARContact extends PXView  {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ARAddress extends PXView  {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState;
}

export class ARShippingContact extends PXView  {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ARShippingAddress extends PXView  {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
	IsValidated: PXFieldState;
}

@gridConfig({
	syncPosition: true
})
export class ARInvoiceDiscountDetail extends PXView  {
	SkipDiscount: PXFieldState;
	LineNbr: PXFieldState;
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountSequenceID: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState;
	IsManual: PXFieldState;
	CuryDiscountableAmt: PXFieldState;
	DiscountableQty: PXFieldState;
	CuryDiscountAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountPct: PXFieldState<PXFieldOptions.CommitChanges>;
	FreeItemID: PXFieldState;
	FreeItemQty: PXFieldState;
	ExtDiscCode: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowUpdate: false
})
export class ARAdjust extends PXView  {
	LoadDocuments: PXActionState;
	AutoApply: PXActionState;
	CreateDocumentPayment: PXActionState;
	CaptureDocumentPayment: PXActionState;
	VoidDocumentPayment: PXActionState;
	ImportDocumentPayment: PXActionState;
	
	Selected: PXFieldState;
	AdjgDocType: PXFieldState;
	@linkCommand('ViewPayment')
	AdjgRefNbr: PXFieldState;
	CuryAdjdAmt: PXFieldState;
	CuryAdjgDiscAmt: PXFieldState;
	CuryAdjdWOAmt: PXFieldState;
	ARPayment__DocDate: PXFieldState;
	CuryDocBal: PXFieldState;
	ARPayment__DocDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ARPayment__CuryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ARPayment__FinPeriodID: PXFieldState;
	ARPayment__ExtRefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	AdjdDocType: PXFieldState;
	AdjdRefNbr: PXFieldState;
	AdjNbr: PXFieldState;
	ARPayment__Status: PXFieldState;
	ExternalTransaction__ProcStatus: PXFieldState;
	CanVoid: PXFieldState;
	CanCapture: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowUpdate: false
})
export class ARAdjust2 extends PXView  {
	CreateDocumentRefund: PXActionState;

	DisplayDocType: PXFieldState;
	@linkCommand('ViewInvoice')
	DisplayRefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DisplayCustomerID: PXFieldState;
	DisplayCuryAmt: PXFieldState;
	DisplayDocDate: PXFieldState;
	CuryDocBal: PXFieldState;
	DisplayDocDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DisplayCuryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DisplayFinPeriodID: PXFieldState;
	ARInvoice__InvoiceNbr: PXFieldState;
	DisplayStatus: PXFieldState;
	DisplayProcStatus: PXFieldState;
}

@gridConfig({
	batchUpdate: true,
	autoAdjustColumns: true
})
export class SOOrderShipment extends PXView  {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	ShipmentNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerLocationID: PXFieldState;
	ShipDate: PXFieldState;
	ShipmentQty: PXFieldState;
}

export class RecalcDiscountsParamFilter extends PXView  {
	RecalcTarget: PXFieldState;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDocGroupDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	batchUpdate: true,
	autoAdjustColumns: true
})
export class SOLineForDirectInvoice extends PXView  {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	Operation: PXFieldState;
	ShipDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	OrderQty: PXFieldState;
	ShippedQty: PXFieldState;
}

@gridConfig({
	batchUpdate: true,
	autoAdjustColumns: true
})
export class ARTranForDirectInvoice extends PXView  {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
	TranType: PXFieldState;
	RefNbr: PXFieldState;
	LineNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	TranDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	Qty: PXFieldState;
	@linkCommand('ARTran$RelatedDocument$Link')
	RelatedDocument: PXFieldState<PXFieldOptions.Hidden>;
}

export class AddressLookupFilter extends PXView  {
	SearchAddress: PXFieldState;
	ViewName: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	AddressLine3: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}

export class SOQuickPayment extends PXView  {
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRefundAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.Disabled>;
	DocDesc: PXFieldState;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefTranExtNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	NewCard: PXFieldState<PXFieldOptions.CommitChanges>;
	SaveCard: PXFieldState<PXFieldOptions.CommitChanges>;
	NewAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	SaveAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SOImportExternalTran extends PXView  {
	TranNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	ProcessingCenterID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class DuplicateFilter extends PXView  {
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}


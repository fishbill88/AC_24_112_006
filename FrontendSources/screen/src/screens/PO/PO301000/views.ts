import
{
	PXView, PXFieldState, gridConfig, ICurrencyInfo, headerDescription,
	PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, PXActionState, GridPreset
} from 'client-controls';

export class POOrderHeader extends PXView {
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Behavior: PXFieldState;
	RequestApproval: PXFieldState;
	OrderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpectedDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderDesc: PXFieldState;

	@headerDescription
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	CuryDetailExtCostTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineDiscTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscTot: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrderTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryControlTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageTotal: PXFieldState<PXFieldOptions.Disabled>;
}

export class POOrder extends PXView {
	ShipDestType: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipToBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipToLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderNbr: PXFieldState;
	FOBPoint: PXFieldState;
	ShipVia: PXFieldState;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	PayToVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentPct: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	RQReqNbr: PXFieldState<PXFieldOptions.Disabled>;
	OriginalPONbr: PXFieldState<PXFieldOptions.Disabled>;
	SuccessorPONbr: PXFieldState<PXFieldOptions.Disabled>;
	OwnerWorkgroupID: PXFieldState;
	DontPrint: PXFieldState<PXFieldOptions.CommitChanges>;
	Printed: PXFieldState<PXFieldOptions.Disabled>;
	DontEmail: PXFieldState<PXFieldOptions.CommitChanges>;
	Emailed: PXFieldState<PXFieldOptions.Disabled>;
	OrderBasedAPBill: PXFieldState;
	RetainageApply: PXFieldState<PXFieldOptions.CommitChanges>;
	DefRetainagePct: PXFieldState;
	CuryVatExemptTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryGoodsExtCostTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryServiceExtCostTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineDiscTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscTot: PXFieldState<PXFieldOptions.Disabled>;
	UnbilledOrderQty: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnbilledOrderTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryPrepaidTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnprepaidTotal: PXFieldState<PXFieldOptions.Disabled>;
	IntercompanySOType: PXFieldState;
	IntercompanySONbr: PXFieldState<PXFieldOptions.Disabled>;
	ExcludeFromIntercompanyProc: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class POLine extends PXView {
	ShowItems: PXActionState;
	ShowMatrixPanel: PXActionState;
	AddProjectItem: PXActionState;
	AddPOOrder: PXActionState;
	AddPOOrderLine: PXActionState;
	ViewDemand: PXActionState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsSpecialOrder: PXFieldState;
	LineType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderQty: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseOrderQty: PXFieldState;
	OrderedQty: PXFieldState;
	NonOrderedQty: PXFieldState;
	ReceivedQty: PXFieldState;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLineAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderNbr: PXFieldState;
	SOOrderStatus: PXFieldState;
	SOLineNbr: PXFieldState;
	SOLinkActive: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscPct: PXFieldState;
	CuryDiscAmt: PXFieldState;
	CuryDiscCost: PXFieldState;
	ManualDisc: PXFieldState;
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountSequenceID: PXFieldState;
	DisplayReqPrepaidQty: PXFieldState;
	CuryReqPrepaidAmt: PXFieldState;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtCost: PXFieldState;
	AlternateID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	LotSerialNbr: PXFieldState;
	RcptQtyMin: PXFieldState;
	RcptQtyMax: PXFieldState;
	RcptQtyThreshold: PXFieldState;
	RcptQtyAction: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAcctID_Account_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ExpenseSubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	POAccrualAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	POAccrualSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	LineNbr: PXFieldState<PXFieldOptions.Hidden>;
	SortOrder: PXFieldState<PXFieldOptions.Hidden>;
	RequestedDate: PXFieldState;
	PromisedDate: PXFieldState;
	DRTermStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DRTermEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CompletePOLine: PXFieldState<PXFieldOptions.Hidden>;
	Completed: PXFieldState;
	Cancelled: PXFieldState;
	Closed: PXFieldState;
	BilledQty: PXFieldState;
	CuryBilledAmt: PXFieldState;
	UnbilledQty: PXFieldState;
	CuryUnbilledAmt: PXFieldState;
	POType: PXFieldState;
	@linkCommand('ViewBlanketOrder')
	PONbr: PXFieldState;
	POAccrualType: PXFieldState;
	HasExpiredComplianceDocuments: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	ViewDemandEnabled: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details
})
export class POTaxTran extends PXView {
	@columnConfig({ hideViewLink: true })
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TaxUOM: PXFieldState;
	TaxableQty: PXFieldState;
	CuryTaxAmt: PXFieldState;
	CuryRetainedTaxableAmt: PXFieldState;
	CuryRetainedTaxAmt: PXFieldState;
	NonDeductibleTaxRate: PXFieldState;
	CuryExpenseAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
}

export class POContact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class POAddress extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class POContact2 extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class POAddress2 extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details
})
export class POOrderDiscountDetail extends PXView {
	SkipDiscount: PXFieldState;
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountSequenceID: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState;
	IsManual: PXFieldState;
	CuryDiscountableAmt: PXFieldState;
	DiscountableQty: PXFieldState;
	CuryDiscountAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainedDiscountAmt: PXFieldState;
	DiscountPct: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtDiscCode: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	statusField: 'StatusText',
	adjustPageSize: true
})
export class POOrderPOReceipt extends PXView {
	ReceiptType: PXFieldState;
	ReceiptNbr: PXFieldState;
	DocDate: PXFieldState;
	Status: PXFieldState;
	TotalQty: PXFieldState;
	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	StatusText: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	statusField: 'StatusText',
	adjustPageSize: true
})
export class POOrderAPDoc extends PXView {
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	DocDate: PXFieldState;
	Status: PXFieldState;
	TotalQty: PXFieldState;
	TotalAmt: PXFieldState;
	TotalPPVAmt: PXFieldState;
	CuryID: PXFieldState;
	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	StatusText: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	statusField: 'StatusText',
	adjustPageSize: true
})
export class POOrderChildOrdersReceipts extends PXView {
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	OrderDate: PXFieldState;
	Status: PXFieldState;
	TotalQty: PXFieldState;
	POBlanketOrderPOReceipt__ReceiptType: PXFieldState;
	POBlanketOrderPOReceipt__ReceiptNbr: PXFieldState;
	POBlanketOrderPOReceipt__ReceiptDate: PXFieldState;
	POBlanketOrderPOReceipt__Status: PXFieldState;
	POBlanketOrderPOReceipt__TotalQty: PXFieldState;
	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	StatusText: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	statusField: 'StatusText',
	adjustPageSize: true
})
export class POOrderChildOrdersAPDocs extends PXView {
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	DocDate: PXFieldState;
	Status: PXFieldState;
	TotalQty: PXFieldState;
	TotalAmt: PXFieldState;
	TotalPPVAmt: PXFieldState;
	CuryID: PXFieldState;
	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	StatusText: PXFieldState;
}


@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	statusField: 'StatusText'
})
export class POOrderPrepayment extends PXView {
	APDocType: PXFieldState;
	APRefNbr: PXFieldState;
	APRegister__DocDate: PXFieldState;
	CuryAppliedAmt: PXFieldState;
	APRegister__CuryDocBal: PXFieldState;
	APRegister__Status: PXFieldState;
	APRegister__CuryID: PXFieldState;
	PayRefNbr: PXFieldState;
	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	StatusText: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false
})
export class PMChangeOrderLine extends PXView {
	@linkCommand('ViewChangeOrder')
	RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PMChangeOrder__ClassID: PXFieldState;
	PMChangeOrder__ProjectNbr: PXFieldState;
	PMChangeOrder__Status: PXFieldState;
	PMChangeOrder__Description: PXFieldState;
	PMChangeOrder__Date: PXFieldState;
	PMChangeOrder__CompletionDate: PXFieldState;
	PMChangeOrder__DelayDays: PXFieldState;
	PMChangeOrder__ReverseStatus: PXFieldState;
	@linkCommand('ViewOrigChangeOrder')
	PMChangeOrder__OrigRefNbr: PXFieldState;
	PMChangeOrder__ExtRefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;
	Description: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	UnitCost: PXFieldState;
	Amount: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class ComplianceDocument extends PXView {
	ExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;
	CreationDate: PXFieldState;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Required: PXFieldState;
	Received: PXFieldState;
	ReceivedDate: PXFieldState;
	IsProcessed: PXFieldState;
	IsVoided: PXFieldState;
	IsCreatedAutomatically: PXFieldState;
	SentDate: PXFieldState;
	@linkCommand('ComplianceViewProject')
	ProjectID: PXFieldState;
	@linkCommand('ComplianceViewCostTask')
	CostTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand('ComplianceViewRevenueTask')
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand('ComplianceViewCostCode')
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorName: PXFieldState;
	@linkCommand('ComplianceDocument$PurchaseOrder$Link')
	PurchaseOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchaseOrderLineItem: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand('ComplianceDocument$Subcontract$Link')
	Subcontract: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractLineItem: PXFieldState;
	@linkCommand('ComplianceDocument$ChangeOrderNumber$Link')
	ChangeOrderNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand('ComplianceViewVendor')
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand('ComplianceDocument$ApCheckID$Link')
	ApCheckID: PXFieldState<PXFieldOptions.CommitChanges>;
	CheckNumber: PXFieldState;
	@linkCommand('ComplianceDocument$ArPaymentID$Link')
	ArPaymentID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillAmount: PXFieldState;
	@linkCommand('ComplianceDocument$BillID$Link')
	BillID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertificateNumber: PXFieldState;
	CreatedByID: PXFieldState;
	@linkCommand('ComplianceViewCustomer')
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerName: PXFieldState;
	DateIssued: PXFieldState;
	DocumentTypeValue: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveDate: PXFieldState;
	InsuranceCompany: PXFieldState;
	InvoiceAmount: PXFieldState;
	@linkCommand('ComplianceDocument$InvoiceID$Link')
	InvoiceID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsExpired: PXFieldState;
	IsRequiredJointCheck: PXFieldState;
	JointAmount: PXFieldState;
	JointLienWaiverAmount: PXFieldState;
	JointReceivedDate: PXFieldState;
	JointRelease: PXFieldState;
	JointReleaseReceived: PXFieldState;
	LienNoticeAmount: PXFieldState;
	IsReceivedFromJointVendor: PXFieldState;
	JointLienNoticeAmount: PXFieldState;
	@linkCommand('ComplianceViewJointVendor')
	JointVendorInternalId: PXFieldState;
	JointVendorExternalName: PXFieldState;
	LastModifiedByID: PXFieldState;
	LienWaiverAmount: PXFieldState;
	Limit: PXFieldState;
	MethodSent: PXFieldState;
	PaymentDate: PXFieldState;
	ArPaymentMethodID: PXFieldState;
	ApPaymentMethodID: PXFieldState;
	Policy: PXFieldState;
	@linkCommand('ComplianceDocument$ProjectTransactionID$Link')
	ProjectTransactionID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptDate: PXFieldState;
	ReceiveDate: PXFieldState;
	ReceivedBy: PXFieldState;
	@linkCommand('ComplianceViewSecondaryVendor')
	SecondaryVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	SecondaryVendorName: PXFieldState;
	SponsorOrganization: PXFieldState;
	ThroughDate: PXFieldState;
	Selected: PXFieldState;
	SourceType: PXFieldState;
}

export class RecalcDiscountsParamFilter extends PXView {
	RecalcTarget: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDocGroupDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CreateSOOrderFilter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AddressLookupFilter extends PXView {
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

export class POOrderFilter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class POLineS extends PXView {
	Selected: PXFieldState;
	LineType: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	OrderQty: PXFieldState;
	OpenQty: PXFieldState;
	TranDesc: PXFieldState;
	RcptQtyMin: PXFieldState;
	RcptQtyMax: PXFieldState;
	RcptQtyAction: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class POOrderS extends PXView {
	Selected: PXFieldState;
	OrderType: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OrderNbr: PXFieldState;
	OrderDate: PXFieldState;
	ExpirationDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryOrderTotal: PXFieldState;
	VendorRefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TermsID: PXFieldState;
	OrderDesc: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OpenOrderQty: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryLeftToReceiveCost: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false
})
export class SOLineSplit3 extends PXView {
	OrderType: PXFieldState;
	OrderNbr: PXFieldState;
	RequestDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	OrderQty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	POUOM: PXFieldState;
	POUOMOrderQty: PXFieldState;
	Active: PXFieldState;
}

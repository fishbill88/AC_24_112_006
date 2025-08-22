import {
	columnConfig,
	gridConfig,
	linkCommand,
	GridColumnShowHideMode,
	ICurrencyInfo,
	PXActionState,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class filter extends PXView {
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class poLinesSelection extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	LineType: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	UOM: PXFieldState;
	OrderQty: PXFieldState;
	OpenQty: PXFieldState;
	TranDesc: PXFieldState;
	RcptQtyMin: PXFieldState;
	RcptQtyMax: PXFieldState;
	RcptQtyAction: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class openOrders extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState;
	OrderDate: PXFieldState;
	ExpirationDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryOrderTotal: PXFieldState;
	VendorRefNbr: PXFieldState;
	TermsID: PXFieldState;
	OrderDesc: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OpenQty: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryLeftToReceiveCost: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class FixedDemand extends PXView {
	OrderNbr: PXFieldState;
	RequestDate: PXFieldState;
	CustomerID: PXFieldState;
	SiteID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	UOM: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OrderQty: PXFieldState;
	@columnConfig({ allowUpdate: false })
	POUOM: PXFieldState;
	@columnConfig({ allowUpdate: false })
	POUOMOrderQty: PXFieldState;
	INItemPlan__Active: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class ReplenishmentLines extends PXView {
	RefNbr: PXFieldState;
	OrderDate: PXFieldState;
	UOM: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Qty: PXFieldState;
}

export class Document extends PXView {
	OrderNbr: PXFieldState;
	Hold: PXFieldState<PXFieldOptions.Hidden>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Behavior: PXFieldState;
	RequestApproval: PXFieldState;
	Approved: PXFieldState;
	OrderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpectedDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpirationDate: PXFieldState;
	OrderDesc: PXFieldState<PXFieldOptions.Multiline>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	VendorRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDetailExtCostTotal: PXFieldState;
	CuryLineDiscTotal: PXFieldState;
	CuryDiscTot: PXFieldState;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrderTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryControlTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageTotal: PXFieldState<PXFieldOptions.Disabled>;
}

export class CurrentDocument extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayToVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageApply: PXFieldState<PXFieldOptions.CommitChanges>;
	DefRetainagePct: PXFieldState;
	OwnerWorkgroupID: PXFieldState;
	UnbilledOrderQty: PXFieldState;
	CuryUnbilledOrderTotal: PXFieldState;
	CuryPrepaidTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnprepaidTotal: PXFieldState<PXFieldOptions.Disabled>;
	DontPrint: PXFieldState;
	Printed: PXFieldState<PXFieldOptions.Disabled>;
	DontEmail: PXFieldState;
	Emailed: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatExemptTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineDiscTotal: PXFieldState;
	CuryDiscTot: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class Transactions extends PXView {
	AddProjectItem: PXActionState;
	AddNew: PXActionState;
	Copy: PXActionState;
	Paste: PXActionState;

	@columnConfig({
		allowShowHide: GridColumnShowHideMode.Server,
		hideViewLink: true
	})
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLineAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscPct: PXFieldState;
	CuryDiscAmt: PXFieldState;
	CuryDiscCost: PXFieldState;
	ManualDisc: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountSequenceID: PXFieldState;
	DisplayReqPrepaidQty: PXFieldState;
	CuryReqPrepaidAmt: PXFieldState;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtCost: PXFieldState;
	AlternateID: PXFieldState;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	LotSerialNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAcctID_Account_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ExpenseSubID: PXFieldState;
	SortOrder: PXFieldState<PXFieldOptions.Hidden>;
	RequestedDate: PXFieldState;
	PromisedDate: PXFieldState;
	DRTermStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DRTermEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceivedQty: PXFieldState<PXFieldOptions.Disabled>;
	CompletePOLine: PXFieldState<PXFieldOptions.Hidden>;
	Completed: PXFieldState;
	Cancelled: PXFieldState;
	Closed: PXFieldState;
	BilledQty: PXFieldState;
	CuryBilledAmt: PXFieldState;
	UnbilledQty: PXFieldState;
	CuryUnbilledAmt: PXFieldState;
	BaseOrderQty: PXFieldState;
	LineNbr: PXFieldState<PXFieldOptions.Hidden>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class Taxes extends PXView {
	@columnConfig({ allowUpdate: false })
	TaxID: PXFieldState<PXFieldOptions.NoLabel>;
	@columnConfig({ allowUpdate: false })
	TaxRate: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.NoLabel>;
	CuryTaxableAmt: PXFieldState<PXFieldOptions.NoLabel>;
	CuryTaxAmt: PXFieldState<PXFieldOptions.NoLabel>;
	CuryRetainedTaxableAmt: PXFieldState;
	CuryRetainedTaxAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
}

export class Remit_Contact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	FullName: PXFieldState;
	Salutation: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Remit_Address extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.NoLabel>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Approval extends PXView {
	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;
	@columnConfig({
		allowUpdate: false
	})
	Status: PXFieldState;
	WorkgroupID: PXFieldState;
	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false
})
export class DiscountDetails extends PXView {
	SkipDiscount: PXFieldState;
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountSequenceID: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState<PXFieldOptions.Disabled>;
	IsManual: PXFieldState;
	CuryDiscountableAmt: PXFieldState;
	DiscountableQty: PXFieldState;
	CuryDiscountAmt: PXFieldState;
	CuryRetainedDiscountAmt: PXFieldState;
	DiscountPct: PXFieldState;
	ExtDiscCode: PXFieldState;
	Description: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class APDocs extends PXView {
	DocType: PXFieldState;
	RefNbr: PXFieldState<PXFieldOptions.NoLabel>;
	DocDate: PXFieldState;
	Status: PXFieldState;
	TotalQty: PXFieldState;
	TotalAmt: PXFieldState;
	TotalPPVAmt: PXFieldState;
	CuryID: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class PrepaymentDocuments extends PXView {
	APDocType: PXFieldState;
	APRefNbr: PXFieldState;
	APRegister__DocDate: PXFieldState;
	CuryAppliedAmt: PXFieldState;
	APRegister__CuryDocBal: PXFieldState;
	APRegister__Status: PXFieldState;
	APRegister__CuryID: PXFieldState;
	PayRefNbr: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class ChangeOrders extends PXView {
	@linkCommand("ViewChangeOrder")
	RefNbr: PXFieldState;
	PMChangeOrder__ClassID: PXFieldState;
	PMChangeOrder__ProjectNbr: PXFieldState;
	PMChangeOrder__Status: PXFieldState;
	PMChangeOrder__Description: PXFieldState;
	PMChangeOrder__Date: PXFieldState;
	PMChangeOrder__CompletionDate: PXFieldState;
	PMChangeOrder__DelayDays: PXFieldState;
	PMChangeOrder__ReverseStatus: PXFieldState;
	@linkCommand("ViewOrigChangeOrder")
	PMChangeOrder__OrigRefNbr: PXFieldState;
	PMChangeOrder__ExtRefNbr: PXFieldState;
	ProjectID: PXFieldState;
	TaskID: PXFieldState;
	InventoryID: PXFieldState;
	CostCodeID: PXFieldState;
	Description: PXFieldState;
	Qty: PXFieldState;
	UOM: PXFieldState;
	UnitCost: PXFieldState;
	Amount: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	allowInsert: false,
	allowDelete: false
})
export class Answers extends PXView {
	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	@columnConfig({
		allowSort: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Value: PXFieldState;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	initNewRow: true
})
export class ComplianceDocuments extends PXView {
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
	@linkCommand("ComplianceViewProject")
	ProjectID: PXFieldState;
	@linkCommand("ComplianceViewCostTask")
	CostTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceViewRevenueTask")
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceViewCostCode")
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceViewVendor")
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorName: PXFieldState;
	@linkCommand("ComplianceDocument$PurchaseOrder$Link")
	PurchaseOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchaseOrderLineItem: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceDocument$Subcontract$Link")
	Subcontract: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractLineItem: PXFieldState;
	@linkCommand("ComplianceDocument$ChangeOrderNumber$Link")
	ChangeOrderNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ComplianceDocument$ApCheckID$Link")
	ApCheckID: PXFieldState<PXFieldOptions.CommitChanges>;
	CheckNumber: PXFieldState;
	@linkCommand("ComplianceDocument$ArPaymentID$Link")
	ArPaymentID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillAmount: PXFieldState;
	@linkCommand("ComplianceDocument$BillID$Link")
	BillID: PXFieldState<PXFieldOptions.CommitChanges>;
	CertificateNumber: PXFieldState;
	CreatedByID: PXFieldState;
	@linkCommand("ComplianceViewCustomer")
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerName: PXFieldState;
	DateIssued: PXFieldState;
	DocumentTypeValue: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveDate: PXFieldState;
	InsuranceCompany: PXFieldState;
	InvoiceAmount: PXFieldState;
	@linkCommand("ComplianceDocument$InvoiceID$Link")
	InvoiceID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsExpired: PXFieldState;
	IsRequiredJointCheck: PXFieldState;
	JointAmount: PXFieldState;
	JointRelease: PXFieldState;
	JointReleaseReceived: PXFieldState;
	@linkCommand("ComplianceViewJointVendor")
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
	@linkCommand("ComplianceDocument$ProjectTransactionID$Link")
	ProjectTransactionID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptDate: PXFieldState;
	ReceiveDate: PXFieldState;
	ReceivedBy: PXFieldState;
	@linkCommand("ComplianceViewSecondaryVendor")
	SecondaryVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	SecondaryVendorName: PXFieldState;
	SponsorOrganization: PXFieldState;
	ThroughDate: PXFieldState;
	SourceType: PXFieldState;
}

export class ItemFilter extends PXView {
	Inventory: PXFieldState<PXFieldOptions.CommitChanges>;
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	OnlyAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class ItemInfo extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	QtySelected: PXFieldState;
	SiteID: PXFieldState;
	ItemClassID: PXFieldState;
	ItemClassDescription: PXFieldState;
	PriceClassID: PXFieldState;
	PriceClassDescription: PXFieldState;
	PreferredVendorID: PXFieldState;
	PreferredVendorDescription: PXFieldState;
	InventoryCD: PXFieldState;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	PurchaseUnit: PXFieldState;
	QtyAvailExt: PXFieldState;
	QtyOnHandExt: PXFieldState;
	QtyPOOrdersExt: PXFieldState;
	QtyPOReceiptsExt: PXFieldState;
	AlternateID: PXFieldState;
	AlternateType: PXFieldState;
	AlternateDescr: PXFieldState;
}

export class recalcdiscountsfilter extends PXView {
	RecalcTarget: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
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

export class ProjectItemFilter extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class AvailableProjectItems extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ChangeOrderQty: PXFieldState;
	CuryChangeOrderAmount: PXFieldState;
	CommittedQty: PXFieldState;
	CuryCommittedAmount: PXFieldState;
	CommittedReceivedQty: PXFieldState;
	CommittedInvoicedQty: PXFieldState;
	CuryCommittedInvoicedAmount: PXFieldState;
	CommittedOpenQty: PXFieldState;
	CuryCommittedOpenAmount: PXFieldState;
	ActualQty: PXFieldState;
	CuryActualAmount: PXFieldState;
	CuryActualPlusOpenCommittedAmount: PXFieldState;
	CuryVarianceAmount: PXFieldState;
	Performance: PXFieldState;
	IsProduction: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLastCostToComplete: PXFieldState;
	CuryCostToComplete: PXFieldState;
	LastPercentCompleted: PXFieldState;
	PercentCompleted: PXFieldState;
	CuryLastCostAtCompletion: PXFieldState;
	CuryCostAtCompletion: PXFieldState;
	RevenueTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	RevenueInventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, gridConfig, columnConfig, linkCommand, PXActionState,
	PXFieldOptions, ICurrencyInfo, GridColumnShowHideMode, viewInfo, GridPreset
} from 'client-controls';

@graphInfo({
	graphType: 'PX.Objects.AR.ARInvoiceEntry',
	primaryView: 'Document',
	showActivitiesIndicator: true,
	bpEventsIndicator: true,
	udfTypeField: "DocType",
	showUDFIndicator: true
})
export class AR301000 extends PXScreen {

	ViewOriginalDocument: PXActionState;
	ViewCorrectionDocument: PXActionState;
	ViewBatch: PXActionState;
	ViewOrigRetainageDocument: PXActionState;
	ViewItem: PXActionState;
	ARTran$RelatedDocument$Link: PXActionState;
	ViewRetainageDocument: PXActionState;
	ViewPayment: PXActionState;
	ViewPPDCrMemo: PXActionState;
	ViewInvoice: PXActionState;

	RecalcOk: PXActionState;
	ViewSchedule: PXActionState;
	CreateSchedule: PXActionState;

	@viewInfo({ containerName: "Invoice Summary" })
	Document = createSingle(ARInvoice);
	CurrentDocument = createSingle(ARInvoice);

	@viewInfo({ containerName: "currencyinfo" })
	CurrencyInfo = createSingle(CurrencyInfo);

	@viewInfo({ containerName: "Financial" })
	dunningLetterDetail = createSingle(ARDunningLetterDetail);

	@viewInfo({ containerName: "Details" })
	Transactions = createCollection(ARTran);

	@viewInfo({ containerName: "Bill-To Contact" })
	Billing_Contact = createSingle(ARContact);

	@viewInfo({ containerName: "Bill-To Address" })
	Billing_Address = createSingle(ARAddress);

	@viewInfo({ containerName: "Ship-To Contact" })
	Shipping_Contact = createSingle(ARShippingContact);

	@viewInfo({ containerName: "Ship-To Address" })
	Shipping_Address = createSingle(ARShippingAddress);

	@viewInfo({ containerName: "Taxes" })
	Taxes = createCollection(ARTaxTran);

	@viewInfo({ containerName: "Commissions" })
	SalesPerTrans = createCollection(ARSalesPerTran);

	@viewInfo({ containerName: "Approvals" })
	Approval = createCollection(EPApproval);

	@viewInfo({ containerName: "Discounts" })
	ARDiscountDetails = createCollection(ARInvoiceDiscountDetail);

	@viewInfo({ containerName: "Retainage" })
	RetainageDocuments = createCollection(ARRetainageInvoice);

	@viewInfo({ containerName: "Applications" })
	Adjustments = createCollection(ARAdjust2);

	@viewInfo({ containerName: "Applications" })
	Adjustments_1 = createCollection(ARAdjust);

	@viewInfo({ containerName: "Payment Links" })
	PayLink = createSingle(CCPayLink);

	@viewInfo({ containerName: "Recalculate Prices" })
	recalcdiscountsfilter = createSingle(RecalcDiscountsParamFilter);

	@viewInfo({ containerName: "Reassign Approval" })
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);

	@viewInfo({ containerName: "Duplicate Reference Nbr." })
	duplicatefilter = createSingle(DuplicateFilter);

}

export class ARInvoice extends PXView {

	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceNbr: PXFieldState;
	DocDesc: PXFieldState<PXFieldOptions.Multiline>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	DueDate: PXFieldState;
	IsRetainageDocument: PXFieldState<PXFieldOptions.Disabled>;
	RetainageApply: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscDate: PXFieldState;
	PaymentsByLinesAllowed: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDetailExtPriceTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineDiscTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscTot: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLineRetainageTotal: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryInitDocBal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRoundDiff: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDocUnpaidBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrigDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;


	@linkCommand("ViewBatch")
	BatchNbr: PXFieldState;
	DisplayCuryInitDocBal: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARSubID: PXFieldState;
	PrepaymentAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentSubID: PXFieldState;
	RetainageAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageSubID: PXFieldState;

	@linkCommand("ViewOriginalDocument")
	OrigRefNbr: PXFieldState;
	@linkCommand("ViewCorrectionDocument")
	CorrectionRefNbr: PXFieldState;

	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	Printed: PXFieldState;
	DontPrint: PXFieldState<PXFieldOptions.CommitChanges>;
	Emailed: PXFieldState;
	DontEmail: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ApplyOverdueCharge: PXFieldState;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	ExternalTaxExemptionNumber: PXFieldState<PXFieldOptions.CommitChanges>;
	AvalaraCustomerUsageType: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryVatTaxableTotal: PXFieldState;
	CuryVatExemptTotal: PXFieldState;

	IsHiddenInIntercompanySales: PXFieldState;
	Revoked: PXFieldState;
	CuryDiscountedDocTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscountedTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscountedPrice: PXFieldState<PXFieldOptions.Disabled>;

	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryCommnblAmt: PXFieldState;
	CuryCommnAmt: PXFieldState;

	DefRetainagePct: PXFieldState;
	CuryOrigDocAmtWithRetainageTotal: PXFieldState;
	CuryRetainageTotal: PXFieldState;
	CuryRetainageUnreleasedAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageReleased: PXFieldState;
	CuryRetainageUnpaidTotal: PXFieldState;
	CuryRetainagePaidTotal: PXFieldState;
	CuryRetainedTaxTotal: PXFieldState;
	CuryRetainedDiscTotal: PXFieldState;
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

@gridConfig({ preset: GridPreset.Details, initNewRow: true })
export class ARTran extends PXView {

	ViewSchedule: PXActionState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrigDocType: PXFieldState;

	@linkCommand("ViewOrigRetainageDocument")
	OrigRefNbr: PXFieldState;
	SortOrder: PXFieldState;

	@linkCommand("ViewItem")
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("ARTran$RelatedDocument$Link")
	RelatedDocument: PXFieldState<PXFieldOptions.Hidden>;
	TranDesc: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseQty: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	TranCost: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscPct: PXFieldState;
	CuryDiscAmt: PXFieldState;
	ManualDisc: PXFieldState<PXFieldOptions.CommitChanges>;
	SkipLineDiscounts: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	DiscountSequenceID: PXFieldState;

	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageBal: PXFieldState;
	CuryRetainedTaxAmt: PXFieldState;
	CuryTranAmt: PXFieldState;
	CuryCashDiscBal: PXFieldState;
	CuryTranBal: PXFieldState;
	CuryOrigTaxAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccrualAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccrualAccountID_Account_description: PXFieldState;
	ExpenseAccrualSubID: PXFieldState;
	ExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccountID_Account_description: PXFieldState;
	ExpenseSubID: PXFieldState;
	CostBasisNull: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAccruedCost: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SalesPersonID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DefScheduleID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DeferredCode: PXFieldState<PXFieldOptions.CommitChanges>;

	DRTermStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DRTermEndDate: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;
	AvalaraCustomerUsageType: PXFieldState;
	Date: PXFieldState;
	Commissionable: PXFieldState<PXFieldOptions.CommitChanges>;
	CaseCD: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryUnitPriceDR: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	DiscPctDR: PXFieldState;

	HasExpiredComplianceDocuments: PXFieldState;
}

export class ARDunningLetterDetail extends PXView {

	ARDunningLetter__DunningLetterDate: PXFieldState;
	DunningLetterLevel: PXFieldState;
}

export class ARContact extends PXView {

	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class ARAddress extends PXView {

	AddressLookup: PXActionState;

	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState<PXFieldOptions.Disabled>;
}

export class ARShippingContact extends PXView {

	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState;
}

export class ARShippingAddress extends PXView {

	ShippingAddressLookup: PXActionState;

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
	preset: GridPreset.Details
})
export class ARTaxTran extends PXView {

	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	CuryExemptedAmt: PXFieldState;
	TaxUOM: PXFieldState;
	TaxableQty: PXFieldState;
	CuryTaxAmt: PXFieldState;
	CuryRetainedTaxableAmt: PXFieldState;
	CuryRetainedTaxAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
	CuryDiscountedTaxableAmt: PXFieldState;
	CuryDiscountedPrice: PXFieldState;
	CuryAdjustedTaxableAmt: PXFieldState;
	CuryAdjustedTaxAmt: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details, allowDelete: false, allowInsert: false, allowUpdate: false })
export class ARSalesPerTran extends PXView {

	SalespersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	CommnPct: PXFieldState;
	CuryCommnAmt: PXFieldState;
	CuryCommnblAmt: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	AdjdDocType: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({ preset: GridPreset.Details, allowDelete: false, allowInsert: false, allowUpdate: false })
export class EPApproval extends PXView {

	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;
	OrigOwnerID: PXFieldState<PXFieldOptions.Hidden>;
	ApproveDate: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	Status: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;

	AssignmentMapID: PXFieldState<PXFieldOptions.Hidden>;
	RuleID: PXFieldState<PXFieldOptions.Hidden>;
	StepID: PXFieldState<PXFieldOptions.Hidden>;
	CreatedDateTime: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({ preset: GridPreset.Details })
export class ARInvoiceDiscountDetail extends PXView {

	SkipDiscount: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountID: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState;
	IsManual: PXFieldState;
	DiscountSequenceID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDiscountableAmt: PXFieldState;
	DiscountableQty: PXFieldState;
	CuryDiscountAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainedDiscountAmt: PXFieldState;
	DiscountPct: PXFieldState<PXFieldOptions.CommitChanges>;
	FreeItemID: PXFieldState;
	FreeItemQty: PXFieldState;
	OrderNbr: PXFieldState;
	ExtDiscCode: PXFieldState;
	Description: PXFieldState;
	OrderType: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class ARRetainageInvoice extends PXView {

	DocType: PXFieldState;

	@linkCommand("ViewRetainageDocument")
	RefNbr: PXFieldState;
	DocDate: PXFieldState;
	FinPeriodID: PXFieldState;
	Status: PXFieldState;
	CuryRetainageReleasedAmt: PXFieldState;
	CuryRetainagePaidAmt: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	PaymentMethodID: PXFieldState;
	InvoiceNbr: PXFieldState;
	DocDesc: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowUpdate: false })
export class ARAdjust2 extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	LoadDocuments: PXActionState;
	AutoApply: PXActionState;

	@columnConfig({ hideViewLink: true })
	AdjgBranchID: PXFieldState;
	AdjgDocType: PXFieldState;

	@linkCommand("ViewPayment")
	AdjgRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;

	CuryAdjdAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAdjdPPDAmt: PXFieldState;
	CuryAdjdWOAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	WriteOffReasonCode: PXFieldState;

	ARPayment__DocDate: PXFieldState;
	CuryDocBal: PXFieldState;
	ARPayment__DocDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ARPayment__FinPeriodID: PXFieldState;

	ARPayment__ExtRefNbr: PXFieldState;

	ARPayment__Status: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ARPayment__CuryID: PXFieldState;

	AdjdDocType: PXFieldState;
	AdjdRefNbr: PXFieldState;
	PendingPPD: PXFieldState;

	@linkCommand("ViewPPDCrMemo")
	PPDCrMemoRefNbr: PXFieldState;
	HasExpiredComplianceDocuments: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowUpdate: false })
export class ARAdjust extends PXView {

	DisplayDocType: PXFieldState;

	@linkCommand("ViewInvoice")
	DisplayRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	DisplayCuryAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayDocDate: PXFieldState;
	CuryDocBal: PXFieldState;
	DisplayCuryID: PXFieldState;
	DisplayFinPeriodID: PXFieldState;
	DisplayStatus: PXFieldState;
	DisplayDocDesc: PXFieldState;
	DisplayBranchID: PXFieldState;
	DisplayCustomerID: PXFieldState;
	ARInvoice__InvoiceNbr: PXFieldState;
}

export class RecalcDiscountsParamFilter extends PXView {

	RecalcTarget: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDocGroupDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CCPayLink extends PXView {
	Url: PXFieldState;
	LinkStatus: PXFieldState;
}
export class APInvoice extends PXView {
	DocumentKey: PXFieldState<PXFieldOptions.Disabled>;
}

//<!--#include file = "~\Pages\Includes\EPReassignApproval.inc"-- >
export class ReassignApprovalFilter extends PXView {
	NewApprover: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreApproversDelegations: PXFieldState<PXFieldOptions.CommitChanges>;
}

//<!--#include file="~\Pages\Includes\DuplicateReferencePanel.inc"-->
export class DuplicateFilter extends PXView {
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

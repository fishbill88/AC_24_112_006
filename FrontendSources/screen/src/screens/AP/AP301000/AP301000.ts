import {
	PXScreen, createSingle, createCollection, graphInfo, PXActionState, PXView, PXFieldState, columnConfig, PXFieldOptions, linkCommand, viewInfo, ICurrencyInfo, gridConfig, GridPreset
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APInvoiceEntry', primaryView: 'Document', udfTypeField: "DocType", showUDFIndicator: true })
export class AP301000 extends PXScreen {

	ViewPurchaseOrder: PXActionState;
	ViewSubcontract: PXActionState;
	ViewPayment: PXActionState;
	ViewRetainageDocument: PXActionState;
	OpenFSDocument: PXActionState;

	VoidInvoice: PXActionState;
	NewTask: PXActionState;
	NewEvent: PXActionState;
	NewActivity: PXActionState;
	NewMailActivity: PXActionState;
	ReverseInvoice: PXActionState;
	ReclassifyBatch: PXActionState;
	VendorRefund: PXActionState;
	VoidDocument: PXActionState;
	ViewBatch: PXActionState;
	ViewOriginalDocument: PXActionState;

	ViewVoucherBatch: PXActionState;
	ViewWorkBook: PXActionState;
	NewVendor: PXActionState;
	EditVendor: PXActionState;
	VendorDocuments: PXActionState;
	AddPOReceipt2: PXActionState;
	AddReceiptLine2: PXActionState;
	AddPOOrder2: PXActionState;
	AddPOOrderLine2: PXActionState;

	AddSubcontract: PXActionState;
	AddSubcontractLine: PXActionState;
	AddLandedCost2: PXActionState;

	ViewPODocument: PXActionState;
	CurrencyView: PXActionState;
	ViewApPayment: PXActionState;
	AdjustJointAmounts: PXActionState;
	AddPostLandedCostTran: PXActionState;
	RecalcOk: PXActionState;
	ReleaseRetainage: PXActionState;
	ViewSourceDocument: PXActionState;


	@viewInfo({ containerName: "Document Summary" })
	Document = createSingle(APInvoice);

	@viewInfo({ containerName: "Details" })
	Transactions = createCollection(APTran);

	Taxes = createCollection(APTaxTran);
	Approval = createCollection(EPApproval);

	@viewInfo({ containerName: "Discounts" })
	DiscountDetails = createCollection(APInvoiceDiscountDetail);

	CurrentDocument = createSingle(APInvoice);
	RetainageDocuments = createCollection(APRetainageInvoice);
	Adjustments = createCollection(APAdjust);
	duplicatefilter = createSingle(DuplicateFilter);
	recalcdiscountsfilter = createSingle(RecalcDiscountsParamFilter);

	@viewInfo({ containerName: "Currency rate" })
	currencyinfo = createSingle(Currencyinfo);

}

export class APInvoice extends PXView {

	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDesc: PXFieldState<PXFieldOptions.Multiline>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	DueDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IsRetainageDocument: PXFieldState<PXFieldOptions.Disabled>;
	RetainageApply: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscDate: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentsByLinesAllowed: PXFieldState<PXFieldOptions.CommitChanges>;
	IsJointPayees: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDetailExtPriceTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryLineDiscTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscTot: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLineRetainageTotal: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState;
	CuryOrigWhTaxAmt: PXFieldState;
	CuryInitDocBal: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRoundDiff: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Disabled>;
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryOrigDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: false })
	BatchNbr: PXFieldState;

	PrebookBatchNbr: PXFieldState;
	VoidBatchNbr: PXFieldState;
	DisplayCuryInitDocBal: PXFieldState;
	BranchID: PXFieldState;
	APAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	APSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrebookAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrebookSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	RetainageSubID: PXFieldState;

	@columnConfig({ hideViewLink: false })
	OrigRefNbr: PXFieldState;

	EmployeeWorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	SeparateCheck: PXFieldState;
	PaySel: PXFieldState<PXFieldOptions.CommitChanges>;
	PayDate: PXFieldState;
	PayLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	UsesManualVAT: PXFieldState;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCostINAdjRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	CuryVatTaxableTotal: PXFieldState;
	CuryVatExemptTotal: PXFieldState;
	SuppliedByVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	SuppliedByVendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	IntercompanyInvoiceNoteID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDiscountedDocTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscountedTaxableTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDiscountedPrice: PXFieldState<PXFieldOptions.Disabled>;
	DefRetainagePct: PXFieldState;
	CuryOrigDocAmtWithRetainageTotal: PXFieldState;
	CuryRetainageTotal: PXFieldState;
	CuryRetainageUnreleasedAmt: PXFieldState;
	CuryRetainageReleased: PXFieldState;
	CuryRetainageUnpaidTotal: PXFieldState;
	CuryRetainagePaidTotal: PXFieldState;
	CuryRetainedTaxTotal: PXFieldState;
	CuryRetainedDiscTotal: PXFieldState;

}

@gridConfig({ initNewRow: true, preset: GridPreset.Details })
export class APTran extends PXView {

	ViewSchedule: PXActionState;
	AddPOReceipt: PXActionState;
	AddReceiptLine: PXActionState;
	AddPOOrder: PXActionState;
	AddSubcontracts: PXActionState;
	AddPOOrderLine: PXActionState;
	AddSubcontractLines: PXActionState;
	AddLandedCost: PXActionState;
	LinkLine: PXActionState;
	ViewItem: PXActionState;


	SubcontractLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;
	LineNbr: PXFieldState;
	SortOrder: PXFieldState;

	@linkCommand("ViewItem")
	InventoryID: PXFieldState;

	SubItemID: PXFieldState;
	TranDesc: PXFieldState;
	Qty: PXFieldState;
	BaseQty: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryLineAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscPct: PXFieldState;
	CuryDiscAmt: PXFieldState;
	CuryDiscCost: PXFieldState;
	ManualDisc: PXFieldState;
	DiscountID: PXFieldState;
	DiscountSequenceID: PXFieldState;
	PrepaymentPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryPrepaymentAmt: PXFieldState;
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryCashDiscBal: PXFieldState;
	CuryRetainageAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRetainageBal: PXFieldState;
	CuryRetainedTaxAmt: PXFieldState;
	CuryTranAmt: PXFieldState;
	CuryTranBal: PXFieldState;
	CuryOrigTaxAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	AccountID_Account_description: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState;

	CostCodeID: PXFieldState;
	NonBillable: PXFieldState;
	Box1099: PXFieldState;
	DeferredCode: PXFieldState;
	DefScheduleID: PXFieldState;
	DRTermStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DRTermEndDate: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;

	Date: PXFieldState;
	POOrderType: PXFieldState;

	@linkCommand("ViewPurchaseOrder")
	PONbr: PXFieldState;

	@linkCommand("ViewSubcontract")
	SubcontractNbr: PXFieldState;

	POLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	LCDocType: PXFieldState;
	LCRefNbr: PXFieldState;
	LCLineNbr: PXFieldState;
	ReceiptType: PXFieldState;
	ReceiptNbr: PXFieldState;
	ReceiptLineNbr: PXFieldState;
	RelatedEntityType: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("OpenFSDocument")
	RelatedDocNoteID: PXFieldState<PXFieldOptions.CommitChanges>;

	PPVDocType: PXFieldState;
	PPVRefNbr: PXFieldState;
	HasExpiredComplianceDocuments: PXFieldState;

}

@gridConfig({ preset: GridPreset.Details })
export class APTaxTran extends PXView {

	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
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
	CuryDiscountedTaxableAmt: PXFieldState;
	CuryDiscountedPrice: PXFieldState;

}

@gridConfig({ allowInsert: false, allowDelete: false, preset: GridPreset.Details })
export class EPApproval extends PXView {

	ApproverEmployee__AcctCD: PXFieldState;
	ApproverEmployee__AcctName: PXFieldState;
	WorkgroupID: PXFieldState;
	ApprovedByEmployee__AcctCD: PXFieldState;
	ApprovedByEmployee__AcctName: PXFieldState;

	ApproveDate: PXFieldState;

	@columnConfig({ allowNull: false, allowUpdate: false })
	Status: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Reason: PXFieldState;

	@columnConfig({ visible: false })
	AssignmentMapID: PXFieldState;

	@columnConfig({ visible: false })
	RuleID: PXFieldState;

	@columnConfig({ visible: false })
	StepID: PXFieldState;

	@columnConfig({ visible: false })
	CreatedDateTime: PXFieldState;

	@columnConfig({ visible: false })
	OrigOwnerID: PXFieldState;
}

@gridConfig({ preset: GridPreset.Details })
export class APInvoiceDiscountDetail extends PXView {

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

@gridConfig({ preset: GridPreset.Details })
export class APRetainageInvoice extends PXView {

	DocType: PXFieldState;

	@linkCommand("ViewRetainageDocument")
	RefNbr: PXFieldState;

	DocDate: PXFieldState;
	FinPeriodID: PXFieldState;
	Status: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	CuryDocBal: PXFieldState;
	APInvoice__PayTypeID: PXFieldState;
	APInvoice__InvoiceNbr: PXFieldState;
	DocDesc: PXFieldState;

}

@gridConfig({ preset: GridPreset.Details })
export class APAdjust extends PXView {

	AutoApply: PXActionState;

	@columnConfig({ hideViewLink: true })
	AdjgBranchID: PXFieldState;

	DisplayDocType: PXFieldState;

	@linkCommand("ViewPayment")
	DisplayRefNbr: PXFieldState;

	CuryAdjdAmt: PXFieldState;
	CuryAdjdPPDAmt: PXFieldState;
	DisplayDocDate: PXFieldState;
	CuryDocBal: PXFieldState;
	DisplayDocDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DisplayFinPeriodID: PXFieldState;

	APPayment__ExtRefNbr: PXFieldState;
	AdjdDocType: PXFieldState;
	AdjdRefNbr: PXFieldState;
	DisplayStatus: PXFieldState;

}

export class DuplicateFilter extends PXView {

	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class RecalcDiscountsParamFilter extends PXView {

	RecalcTarget: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcUnitPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualPrices: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalcDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideManualDocGroupDiscounts: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class Currencyinfo extends PXView implements ICurrencyInfo {

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

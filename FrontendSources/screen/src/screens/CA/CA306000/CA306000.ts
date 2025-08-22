import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState, TextAlign
} from "client-controls";

@graphInfo({ graphType: "PX.Objects.CA.CABankTransactionsMaint", primaryView: "TranFilter", })
export class CA306000 extends PXScreen {

	RefreshAfterRuleCreate: PXActionState;
	ViewInvoice :PXActionState;
	ViewPayment :PXActionState;
	ClearMatch: PXActionState;
	ClearAllMatches: PXActionState;
	Hide: PXActionState;
	UnapplyRule: PXActionState;
	CreateRule: PXActionState;
	ResetMatchSettingsToDefault: PXActionState;

	TranFilter = createSingle(Filter);
	Details = createCollection(CABankTran);

	@viewInfo({ containerName: "Match to Payments" })
	DetailsForPaymentMatching = createSingle(CABankTran2);

	@viewInfo({ containerName: "Match to Payments" })
	DetailMatchesCA = createCollection(CATran);

	@viewInfo({ containerName: "Create Document" })
	DetailsForInvoiceApplication = createSingle(CABankTran3);

	@viewInfo({ containerName: "Matching Invoices" })
	detailMatchingInvoices = createCollection(CABankTranInvoiceMatch);

	@viewInfo({ containerName: "Matching Expense Receipts" })
	ExpenseReceiptDetailMatches = createCollection(CABankTranExpenseDetailMatch);

	@viewInfo({ containerName: "Create Payment" })
	DetailsForPaymentCreation = createSingle(CABankTran4);

	@viewInfo({ containerName: "AP/AR Adjustments" })
	Adjustments = createCollection(CABankTranAdjustment);

	@viewInfo({ containerName: "CA Splits" })
	TranSplit = createCollection(CABankTranDetail);

	@viewInfo({ containerName: "Transaction Matching Settings" })
	cashAccount = createSingle(CashAccount);

	@viewInfo({ containerName: "Create Rule" })
	RuleCreation = createSingle(CreateRuleSettings);

	@viewInfo({ containerName: "Tax Details" })
	CurrentCABankTran = createSingle(CABankTran5);

	@viewInfo({ containerName: "Tax Details" })
	TaxTrans = createCollection(CABankTaxTran);

	@viewInfo({ containerName: "Load Options" })
	loadOpts = createSingle(LoadOptions);

}

export class Filter extends PXView {

	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsCorpCardCashAccount: PXFieldState;

}

@gridConfig({ syncPosition: true, statusField: "MatchStatsInfo" })
export class CABankTran extends PXView {

	Status: PXFieldState;

	@columnConfig({ allowSort: false, allowResize: false, allowFilter: false, allowShowHide: GridColumnShowHideMode.Server })
	SplittedIcon: PXFieldState;

	DocumentMatched: PXFieldState;
	RuleApplied: PXFieldState;
	ApplyRuleEnabled: PXFieldState;
	ExtTranID: PXFieldState;
	ExtRefNbr: PXFieldState;
	TranDate: PXFieldState;

	CuryDebitAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryCreditAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryOrigDebitAmt: PXFieldState;
	CuryOrigCreditAmt: PXFieldState;
	CardNumber: PXFieldState;
	TranDesc: PXFieldState;
	TranCode: PXFieldState;
	TranEntryDate: PXFieldState;
	PayeeName: PXFieldState;
	EntryTypeID1: PXFieldState;
	InvoiceInfo1: PXFieldState;
	PaymentMethodID1: PXFieldState;
	PayeeBAccountID1: PXFieldState;
	AcctName: PXFieldState;
	OrigModule1: PXFieldState;
	PayeeLocationID1: PXFieldState;
	UserDesc: PXFieldState;

}

export class CABankTran2 extends PXView {

	MultipleMatchingToPayments: PXFieldState<PXFieldOptions.CommitChanges>;
	MatchReceiptsAndDisbursements: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTotalAmtDisplay: PXFieldState<PXFieldOptions.Disabled>;
	CuryApplAmtMatchToPayment: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnappliedBalMatchToPayment: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({ initNewRow: true })
export class CATran extends PXView {

	@columnConfig({ allowSort: false })
	IsMatched: PXFieldState<PXFieldOptions.CommitChanges>;

	MatchRelevancePercent: PXFieldState;
	OrigRefNbr: PXFieldState;
	TranDate: PXFieldState;
	ExtRefNbr: PXFieldState;
	OrigModule: PXFieldState;
	OrigTranTypeUI: PXFieldState;
	Status: PXFieldState;
	TranDesc: PXFieldState;
	FinPeriodID: PXFieldState;
	CuryTranAbsAmt: PXFieldState;
	CuryTranAmtCalc: PXFieldState;
	BAccount__AcctCD: PXFieldState;
	BAccount__AcctName: PXFieldState;
}

export class CABankTran3 extends PXView {

	MultipleMatching: PXFieldState<PXFieldOptions.CommitChanges>;
	PayeeBAccountIDCopy: PXFieldState<PXFieldOptions.CommitChanges>;
	PayeeLocationIDCopy: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodIDCopy: PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceIDCopy: PXFieldState<PXFieldOptions.CommitChanges>;
	ChargeTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTotalAmtCopy: PXFieldState<PXFieldOptions.Disabled>;
	CuryApplAmtMatchToInvoice: PXFieldState<PXFieldOptions.Disabled>;
	CuryChargeAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryChargeTaxAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnappliedBalMatchToInvoice: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({ allowDelete: false, allowInsert: false })
export class CABankTranInvoiceMatch extends PXView {

	@columnConfig({ allowSort: false })
	IsMatched: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	MatchRelevancePercent: PXFieldState;

	BranchID: PXFieldState;
	OrigModule: PXFieldState;
	OrigTranType: PXFieldState;
	OrigRefNbr: PXFieldState;
	ExtRefNbr: PXFieldState;
	TranDate: PXFieldState;

	@columnConfig({ allowNull: false })
	CuryTranAmt: PXFieldState;

	CuryDiscAmt: PXFieldState;

	DiscDate: PXFieldState;
	ReferenceID: PXFieldState;
	ReferenceName: PXFieldState;
	TranDesc: PXFieldState;

}

@gridConfig({ allowDelete: false, allowInsert: false })
export class CABankTranExpenseDetailMatch extends PXView {

	@columnConfig({ allowSort: false })
	IsMatched: PXFieldState<PXFieldOptions.CommitChanges>;

	MatchRelevancePercent: PXFieldState;
	RefNbr: PXFieldState;
	TranDesc: PXFieldState;
	DocDate: PXFieldState;
	CuryDocAmt: PXFieldState;
	ClaimCuryID: PXFieldState;
	CuryDocAmtDiff: PXFieldState;
	CardNumber: PXFieldState;
	ReferenceID: PXFieldState;
	ReferenceName: PXFieldState;
	ExtRefNbr: PXFieldState;
	PaidWith: PXFieldState;

}

export class CABankTran4 extends PXView {

	CreateDocument: PXFieldState<PXFieldOptions.CommitChanges>;
	RuleID: PXFieldState<PXFieldOptions.Disabled>;
	OrigModule: PXFieldState<PXFieldOptions.CommitChanges>;
	MatchingPaymentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	MatchingFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	EntryTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayeeBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayeeLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	PMInstanceID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceInfo: PXFieldState;
	UserDesc: PXFieldState;
	CuryDetailsWithTaxesTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryTotalAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryApplAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnappliedBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryWOAmt: PXFieldState<PXFieldOptions.Disabled>;
	CuryApplAmtCA: PXFieldState<PXFieldOptions.Disabled>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryUnappliedBalCA: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({
	initNewRow: true,
	syncPosition: true,
})
export class CABankTranAdjustment extends PXView {

	LoadInvoices: PXActionState;
	AdjdDocType: PXFieldState;
	AdjdRefNbr: PXFieldState;
	ARInvoice__CustomerID: PXFieldState;
	CuryAdjgAmt: PXFieldState;
	CuryAdjgDiscAmt: PXFieldState;
	CuryAdjgWhTaxAmt: PXFieldState;
	AdjdDocDate: PXFieldState;
	AdjdCuryRate: PXFieldState;
	CuryDocBal: PXFieldState;
	CuryDiscBal: PXFieldState;
	CuryWhTaxBal: PXFieldState;

	@columnConfig({ allowNull: false })
	CuryAdjgWOAmt: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowNull: false })
	WriteOffReasonCode: PXFieldState;

	AdjdBranchID: PXFieldState;
	AdjdFinPeriodID: PXFieldState;
	AdjdCuryID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;

}

@gridConfig({ initNewRow: true, syncPosition: true })
export class CABankTranDetail extends PXView {

	BranchID: PXFieldState;
	ProjectID: PXFieldState;
	TaskID: PXFieldState;
	InventoryID: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTranAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState;

	@columnConfig({ allowUpdate: false })
	AccountID_description: PXFieldState;

	CostCodeID: PXFieldState;
	NonBillable: PXFieldState;

}

export class CashAccount extends PXView {

	DisbursementTranDaysBefore: PXFieldState;
	DisbursementTranDaysAfter: PXFieldState;
	AllowMatchingCreditMemo: PXFieldState;
	ReceiptTranDaysBefore: PXFieldState;
	ReceiptTranDaysAfter: PXFieldState;
	InvoiceFilterByCashAccount: PXFieldState;
	InvoiceFilterByDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DaysBeforeInvoiceDiscountDate: PXFieldState;
	DaysBeforeInvoiceDueDate: PXFieldState;
	DaysAfterInvoiceDueDate: PXFieldState;
	SkipVoided: PXFieldState;
	MatchThreshold: PXFieldState;
	RelativeMatchThreshold: PXFieldState;
	RefNbrCompareWeight: PXFieldState<PXFieldOptions.CommitChanges>;
	EmptyRefNbrMatching: PXFieldState<PXFieldOptions.CommitChanges>;
	DateCompareWeight: PXFieldState<PXFieldOptions.CommitChanges>;
	PayeeCompareWeight: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbrComparePercent: PXFieldState<PXFieldOptions.Disabled>;
	DateComparePercent: PXFieldState<PXFieldOptions.Disabled>;
	PayeeComparePercent: PXFieldState<PXFieldOptions.Disabled>;
	AmountWeight: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDiffThreshold: PXFieldState;
	DateMeanOffset: PXFieldState;
	DateSigma: PXFieldState;
	RatioInRelevanceCalculationLabel: PXFieldState<PXFieldOptions.Disabled>;
	InvoiceRefNbrCompareWeight: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceDateCompareWeight: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoicePayeeCompareWeight: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceRefNbrComparePercent: PXFieldState<PXFieldOptions.Disabled>;
	InvoiceDateComparePercent: PXFieldState<PXFieldOptions.Disabled>;
	InvoicePayeeComparePercent: PXFieldState<PXFieldOptions.Disabled>;
	AveragePaymentDelay: PXFieldState;
	InvoiceDateSigma: PXFieldState;

}

export class CreateRuleSettings extends PXView {

	RuleName: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class CABankTran5 extends PXView {

	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class CABankTaxTran extends PXView {

	TaxID: PXFieldState;
	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTaxAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	NonDeductibleTaxRate: PXFieldState;
	CuryExpenseAmt: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class LoadOptions extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState;
	TillDate: PXFieldState;
	MaxDocs: PXFieldState;
	StartRefNbr: PXFieldState;
	EndRefNbr: PXFieldState;
	OrderBy: PXFieldState;

}

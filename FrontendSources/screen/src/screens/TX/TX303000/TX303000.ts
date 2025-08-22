import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, ICurrencyInfo, PXFieldOptions, columnConfig, linkCommand, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.TXInvoiceEntry', primaryView: 'Document', udfTypeField: 'DocType', showUDFIndicator: true })
export class TX303000 extends PXScreen {

	AddInvoicesOK: PXActionState;
	ViewPayment: PXActionState;
	Inquiry: PXActionState;
	Report: PXActionState;
	ReverseInvoice: PXActionState;
	VendorRefund: PXActionState;
	VoidDocument: PXActionState;
	PayInvoice: PXActionState;
	ViewSchedule: PXActionState;
	CreateSchedule: PXActionState;
	ViewBatch: PXActionState;
	NewVendor: PXActionState;
	EditVendor: PXActionState;
	ReclassifyBatch: PXActionState;
	VendorDocuments: PXActionState;
	Approve: PXActionState;
	Reject: PXActionState;
	ReleaseRetainage: PXActionState;
	ViewSourceDocument: PXActionState;
	AddPOReceipt2: PXActionState;
	AddReceiptLine2: PXActionState;
	AddPOOrder2: PXActionState;
	AddPOReceipt: PXActionState;
	AddReceiptLine: PXActionState;
	AddPOOrder: PXActionState;
	AddPOOrderLine: PXActionState;
	AddLandedCost: PXActionState;
	AddLandedCost2: PXActionState;
	LinkLine: PXActionState;
	ViewPODocument: PXActionState;
	CurrencyView: PXActionState;
	LsLCSplits: PXActionState;
	RecalculateDiscountsAction: PXActionState;
	RecalcOk: PXActionState;
	NewTask: PXActionState;
	NewEvent: PXActionState;
	NewActivity: PXActionState;
	NewMailActivity: PXActionState;
	OpenFSDocument: PXActionState;

	Document = createSingle(APInvoice);
	Taxes = createCollection(TaxTran);
	CurrentDocument = createSingle(APInvoice);
	Adjustments = createCollection(APAdjust);
	BillFilter = createSingle(AddBillFilter);
	DocumentList = createCollection(APInvoice3); // for qp-panel with specified columns

	currencyinfo = createSingle(CurrencyInfo);
}

export class APInvoice extends PXView {

	DocType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	TermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	DueDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDesc: PXFieldState<PXFieldOptions.Multiline>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTaxTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	CuryOrigDocAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTaxAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryOrigDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	BatchNbr: PXFieldState<PXFieldOptions.Disabled>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	APAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	APSubID: PXFieldState;
	SeparateCheck: PXFieldState;
	PaySel: PXFieldState<PXFieldOptions.CommitChanges>;
	PayDate: PXFieldState;
	PayLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayAccountID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ initNewRow: true })
export class TaxTran extends PXView {

	OrigTranType: PXFieldState;
	OrigRefNbr: PXFieldState;

	@columnConfig({hideViewLink: true})
	TaxID: PXFieldState;

	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	CuryTaxAmt: PXFieldState;

	@columnConfig({hideViewLink: true})
	TaxZoneID: PXFieldState;

	@columnConfig({hideViewLink: true})
	AccountID: PXFieldState;

	@columnConfig({hideViewLink: true})
	SubID: PXFieldState;

	// Actions:
	AddInvoices: PXActionState;

}

export class APAdjust extends PXView {

	AdjgDocType: PXFieldState;

	@linkCommand('ViewPayment')
	AdjgRefNbr: PXFieldState;

	CuryAdjdAmt: PXFieldState;

	APPayment__DocDate: PXFieldState;
	CuryDocBal: PXFieldState;
	APPayment__DocDesc: PXFieldState;

	@columnConfig({hideViewLink: true})
	APPayment__CuryID: PXFieldState;

	@columnConfig({hideViewLink: true})
	APPayment__FinPeriodID: PXFieldState;

	APPayment__ExtRefNbr: PXFieldState;
	AdjdDocType: PXFieldState;
	AdjdRefNbr: PXFieldState;
	APPayment__Status: PXFieldState;

	// Actions:
	AutoApply: PXActionState;

}

export class AddBillFilter extends PXView {

	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ allowDelete: false, allowInsert: false, adjustPageSize: true })
export class APInvoice3 extends PXView {

	Selected: PXFieldState;
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	InvoiceNbr: PXFieldState;
	DocDate: PXFieldState;
	VendorID: PXFieldState;
	VendorID_description: PXFieldState;
	VendorLocationID: PXFieldState;
	CuryID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	CuryDocBal: PXFieldState;
	DueDate: PXFieldState;
	DocDesc: PXFieldState;

}

export class CurrencyInfo extends PXView implements ICurrencyInfo {

	CuryInfoID : PXFieldState;
	BaseCuryID : PXFieldState;
	BaseCalc : PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID : PXFieldState;
	CuryRateTypeID : PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision : PXFieldState;
	CuryRate : PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate : PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate : PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate : PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate : PXFieldState<PXFieldOptions.CommitChanges>;

}

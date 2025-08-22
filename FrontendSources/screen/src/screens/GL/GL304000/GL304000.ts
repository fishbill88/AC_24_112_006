import { Messages as SysMessages } from "client-controls/services/messages";
import {
	createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection,
	PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions,
	linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState
} from "client-controls";

@graphInfo({graphType: "PX.Objects.GL.JournalWithSubEntry", primaryView: "BatchModule", showUDFIndicator: true })
export class GL304000 extends PXScreen {

	BatchModule = createSingle(GLDocBatch);
	GLTranModuleBatNbr = createCollection(GLTranDoc);
	APPayments = createCollection(GLTranDocAP);
	ARPayments = createCollection(GLTranDocAR);
	APAdjustments = createCollection(APAdjust);
	ARAdjustments = createCollection(ARAdjust);
	GLTransactions = createCollection(GLTran);
	CurrentDocTaxes = createCollection(GLTax);
	_GLDocBatch_CurrencyInfo_ = createSingle(CurrencyInfo);
}

export class GLDocBatch extends PXView {

	BatchNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DateEntered: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	LedgerID: PXFieldState<PXFieldOptions.Disabled>;
	CuryID: PXFieldState;
	CuryInfoID: PXFieldState;
	Description: PXFieldState;
	CuryDebitTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryCreditTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryControlTotal: PXFieldState;
}

@gridConfig({ initNewRow: true, syncPosition: true, allowUpdate: false })
export class GLTranDoc extends PXView {

	ViewDocument: PXActionState;
	ShowTaxes: PXActionState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TranCode: PXFieldState;

	@columnConfig({ allowUpdate: false })
	TranDate: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	BAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	EntryTypeID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DebitAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DebitSubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CreditAccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CreditSubID: PXFieldState;


	ExtRefNbr: PXFieldState;
	CuryTranTotal: PXFieldState;
	CuryTranAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaxZoneID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaxCategoryID: PXFieldState;

	@linkCommand("ShowTaxes")
	CuryTaxAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Split: PXFieldState;

	TranDesc: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	PaymentMethodID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	PMInstanceID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	TermsID: PXFieldState;
	DueDate: PXFieldState;
	DiscDate: PXFieldState;
	CuryDiscAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	LineNbr: PXFieldState<PXFieldOptions.Hidden>;

	CuryInclTaxAmt: PXFieldState;
	CuryDocTotal: PXFieldState;
	DocCreated: PXFieldState;
	Released: PXFieldState;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false })
export class GLTranDocAP extends PXView {

	TranCode: PXFieldState;
	RefNbr: PXFieldState;
	TranDate: PXFieldState;
	BAccountID: PXFieldState;
	LocationID: PXFieldState;
	PaymentMethodID: PXFieldState;
	DebitAccountID: PXFieldState;
	DebitSubID: PXFieldState;
	CreditAccountID: PXFieldState;
	CreditSubID: PXFieldState;
	ExtRefNbr: PXFieldState;
	CuryID: PXFieldState;
	CuryApplAmt: PXFieldState;
	curyUnappliedBal: PXFieldState;
	CuryTranAmt: PXFieldState;
	TranDesc: PXFieldState;
}

export class APAdjust extends PXView {

	AdjdDocType: PXFieldState;
	AdjdRefNbr: PXFieldState;
	CuryAdjgAmt: PXFieldState;
	CuryAdjgDiscAmt: PXFieldState;
	CuryAdjgWhTaxAmt: PXFieldState;
	AdjdDocDate: PXFieldState;
	APInvoice__DueDate: PXFieldState;
	APInvoice__DiscDate: PXFieldState;
	CuryDocBal: PXFieldState;
	CuryDiscBal: PXFieldState;
	CuryWhTaxBal: PXFieldState;
	APInvoice__DocDesc: PXFieldState;
	AdjdCuryID: PXFieldState;
	AdjdFinPeriodID: PXFieldState;
	APInvoice__InvoiceNbr: PXFieldState;
	VendorID: PXFieldState;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false })
export class GLTranDocAR extends PXView {

	TranCode: PXFieldState;
	RefNbr: PXFieldState;
	TranDate: PXFieldState;
	BAccountID: PXFieldState;
	LocationID: PXFieldState;
	PaymentMethodID: PXFieldState;
	DebitAccountID: PXFieldState;
	DebitSubID: PXFieldState;
	CreditAccountID: PXFieldState;
	CreditSubID: PXFieldState;
	ExtRefNbr: PXFieldState;
	CuryID: PXFieldState;
	CuryApplAmt: PXFieldState;
	curyUnappliedBal: PXFieldState;
	CuryTranAmt: PXFieldState;
	TranDesc: PXFieldState;
}

export class ARAdjust extends PXView {

	AdjdDocType: PXFieldState;
	AdjdRefNbr: PXFieldState;
	CuryAdjgAmt: PXFieldState;
	CuryAdjgDiscAmt: PXFieldState;
	CuryAdjgWOAmt: PXFieldState;
	WriteOffReasonCode: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AdjdDocDate: PXFieldState;
	ARInvoice__DueDate: PXFieldState;
	ARInvoice__DiscDate: PXFieldState;
	CuryDocBal: PXFieldState;
	CuryDiscBal: PXFieldState;
	CuryWOBal: PXFieldState;
	ARInvoice__DocDesc: PXFieldState;
	AdjdCuryID: PXFieldState;
	AdjdFinPeriodID: PXFieldState;
	ARInvoice__InvoiceNbr: PXFieldState;
	CustomerID: PXFieldState;
}

export class GLTran extends PXView {
	RefNbr: PXFieldState;
	Module: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BatchNbr: PXFieldState;

	TranDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	Qty: PXFieldState;
	CuryID: PXFieldState;
	CuryDebitAmt: PXFieldState;
	CuryCreditAmt: PXFieldState;
	TranDesc: PXFieldState;
}

@gridConfig({ syncPosition: true, initNewRow: true })
export class GLTax extends PXView {
	TaxID: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	TaxRate: PXFieldState;
	CuryTaxAmt: PXFieldState;
	NonDeductibleTaxRate: PXFieldState;
	CuryExpenseAmt: PXFieldState;
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

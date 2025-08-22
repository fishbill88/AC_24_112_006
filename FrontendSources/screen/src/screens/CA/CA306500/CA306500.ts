import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, PXActionState } from "client-controls";

@graphInfo({ graphType: "PX.Objects.CA.CABankTransactionsImport", primaryView: "Header", showUDFIndicator: true })
export class CA306500 extends PXScreen {

	Header = createSingle(CABankTranHeader);
	Details = createCollection(CABankTran);

}

export class CABankTranHeader extends PXView {

	CashAccountID: PXFieldState;
	RefNbr: PXFieldState;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TranType: PXFieldState;
	StartBalanceDate: PXFieldState;
	EndBalanceDate: PXFieldState;
	CuryBegBalance: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEndBalance: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDetailsEndBalance: PXFieldState;

}

@gridConfig({ initNewRow: true, syncPosition: true})
export class CABankTran extends PXView {

	Unhide: PXActionState;
	Unmatch: PXActionState;
	UnmatchAll: PXActionState;
	ViewDoc: PXActionState;

	DocumentMatched: PXFieldState;
	Processed: PXFieldState;
	Hidden: PXFieldState;
	FullProcessed: PXFieldState;
	FullDocumentMatched: PXFieldState;
	ExtTranID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState;
	TranDate: PXFieldState;
	TranDesc: PXFieldState;
	CuryDebitAmt: PXFieldState;
	CuryCreditAmt: PXFieldState;
	CardNumber: PXFieldState;
	InvoiceInfo: PXFieldState;
	RuleID: PXFieldState;

	EntryTypeID1: PXFieldState;
	PaymentMethodID1: PXFieldState;
	PayeeBAccountID1: PXFieldState;
	AcctName: PXFieldState;
	OrigModule1: PXFieldState;
	PayeeLocationID1: PXFieldState;
	UserDesc: PXFieldState;
	TranID: PXFieldState;
	Splitted: PXFieldState;
	TranCode: PXFieldState;
	CuryDisplayDebitAmt: PXFieldState;
	CuryDisplayCreditAmt: PXFieldState;
	PayeeName: PXFieldState;

}

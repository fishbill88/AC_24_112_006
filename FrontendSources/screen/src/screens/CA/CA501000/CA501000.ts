import {
	PXScreen, graphInfo, PXView, PXFieldState, PXActionState, createCollection, columnConfig, linkCommand, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.CABankTransactionsProcess', primaryView: 'BankAccountSummary' })
export class CA501000 extends PXScreen {
	BankAccountSummary = createCollection(CashAccount);

	viewUnmatchedTrans: PXActionState;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	mergeToolbarWith: 'ScreenToolbar',
	quickFilterFields: ['CashAccountID', 'CashAccount__Descr'],
})
export class CashAccount extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@linkCommand('viewUnmatchedTrans')
	CashAccountCD: PXFieldState;

	Descr: PXFieldState;
	ExtRefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;

	CABankTranByCashAccount__DebitNumber: PXFieldState;
	CABankTranByCashAccount__CreditNumber: PXFieldState;
	CABankTranByCashAccount__UnprocessedNumber: PXFieldState;
	CABankTranByCashAccount__MatchedNumber: PXFieldState;
	CABankTranByCashAccount__CuryDebitAmount: PXFieldState;
	CABankTranByCashAccount__CuryCreditAmount: PXFieldState;
}

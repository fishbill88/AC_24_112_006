import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, columnConfig, linkCommand, gridConfig,
	PXFieldOptions, PXPageLoadBehavior, GridColumnShowHideMode
} from "client-controls";


@graphInfo({graphType: "PX.Objects.CA.CABankTransactionsEnq", primaryView: "TranFilter", pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues})
export class CA402000 extends PXScreen {
	ViewStatement: PXActionState;
	ViewDoc: PXActionState;

	TranFilter = createSingle(Filter);
	Result = createCollection(CABankTranHistory);
}

export class Filter extends PXView {
	CashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TranType: PXFieldState<PXFieldOptions.CommitChanges>;
	HeaderRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
})
export class CABankTranHistory extends PXView {
	@linkCommand("ViewStatement")
	HeaderRefNbr: PXFieldState;

	@columnConfig({ allowSort: false, allowResize: false, allowFilter: false, allowShowHide: GridColumnShowHideMode.Server })
	SplittedIcon: PXFieldState;

	ExtTranID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExtRefNbr: PXFieldState;
	Status: PXFieldState;
	TranDate: PXFieldState;
	TranDesc: PXFieldState;
	TranCode: PXFieldState;
	CuryDebitAmt: PXFieldState;
	CuryCreditAmt: PXFieldState;
	InvoiceInfo: PXFieldState;
	PayeeName: PXFieldState;
	RuleID: PXFieldState;
	MatchedModule: PXFieldState;
	CuryMatchedDebitAmt: PXFieldState;
	CuryMatchedCreditAmt: PXFieldState;
	CuryOrigDebitAmt: PXFieldState;
	CuryOrigCreditAmt: PXFieldState;
	MatchedDocType: PXFieldState;

	@linkCommand("ViewDoc")
	MatchedRefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	MatchedReferenceID: PXFieldState;

	MatchedReferenceID_BAccountR_acctName: PXFieldState;
}

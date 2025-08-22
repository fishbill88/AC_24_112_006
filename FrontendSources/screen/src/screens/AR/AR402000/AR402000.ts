import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, linkCommand, PXActionState, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AR.ARDocumentEnq', primaryView: 'Filter' })
export class AR402000 extends PXScreen {

	ViewDocument: PXActionState;
	ViewOriginalDocument: PXActionState;

	Filter = createSingle(ARDocumentFilter);
	Documents = createCollection(ARDocumentResult);
}

export class ARDocumentFilter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	Period: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAllDocs: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleased: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMasterCalendar: PXFieldState<PXFieldOptions.CommitChanges>;
	BalanceSummary: PXFieldState;
	CustomerBalance: PXFieldState;
	CustomerDepositsBalance: PXFieldState;
	Difference: PXFieldState;
	CustomerRetainedBalance: PXFieldState;
	CuryBalanceSummary: PXFieldState;
	CuryCustomerBalance: PXFieldState;
	CuryCustomerDepositsBalance: PXFieldState;
	CuryDifference: PXFieldState;
	CuryCustomerRetainedBalance: PXFieldState;
	IncludeChildAccounts: PXFieldState<PXFieldOptions.CommitChanges>;
}


@gridConfig(
	{
		syncPosition: true,
		mergeToolbarWith: 'ScreenToolbar',
		quickFilterFields: ["RefNbr", "ExtRefNbr", "DocDesc"],
		actionsConfig:
		{
			refresh: { hidden: true }
		}
	})
export class ARDocumentResult extends PXView {

	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;

	BranchID: PXFieldState;
	DocType: PXFieldState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;
	FinPeriodID: PXFieldState;
	DocDate: PXFieldState;
	DueDate: PXFieldState;
	Status: PXFieldState;
	ARAccountID: PXFieldState;
	ARSubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;

	CuryOrigDocAmt: PXFieldState;
	CuryBegBalance: PXFieldState;
	CuryDocBal: PXFieldState;
	CuryDiscActTaken: PXFieldState;
	CuryWOAmt: PXFieldState;
	CuryRetainageTotal: PXFieldState;
	CuryOrigDocAmtWithRetainageTotal: PXFieldState;
	CuryRetainageUnreleasedAmt: PXFieldState;
	OrigDocAmt: PXFieldState;
	BegBalance: PXFieldState;
	DocBal: PXFieldState;
	DiscActTaken: PXFieldState;
	WOAmt: PXFieldState;
	RetainageTotal: PXFieldState;
	OrigDocAmtWithRetainageTotal: PXFieldState;
	RetainageUnreleasedAmt: PXFieldState;
	IsRetainageDocument: PXFieldState;

	@linkCommand("ViewOriginalDocument")
	OrigRefNbr: PXFieldState;
	RGOLAmt: PXFieldState;
	PaymentMethodID: PXFieldState;
	ExtRefNbr: PXFieldState;
	DocDesc: PXFieldState;
}

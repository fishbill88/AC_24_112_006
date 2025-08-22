import {
	PXScreen, createSingle, createCollection, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig,
	GridColumnShowHideMode, linkCommand, PXActionState, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APDocumentEnq', primaryView: 'Filter' })
export class AP402000 extends PXScreen {

	ViewDocument: PXActionState;
	ViewOriginalDocument: PXActionState;

	Filter = createSingle(APDocumentFilter);
	Documents = createCollection(APDocumentResult);
}

export class APDocumentFilter extends PXView {

	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;

	ShowAllDocs: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleased: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMasterCalendar: PXFieldState<PXFieldOptions.CommitChanges>;

	BalanceSummary: PXFieldState;
	VendorBalance: PXFieldState;
	VendorDepositsBalance: PXFieldState;
	Difference: PXFieldState;
	VendorRetainedBalance: PXFieldState;

	CuryBalanceSummary: PXFieldState;
	CuryVendorBalance: PXFieldState;
	CuryVendorDepositsBalance: PXFieldState;
	CuryDifference: PXFieldState;
	CuryVendorRetainedBalance: PXFieldState;

}

@gridConfig(
	{
		mergeToolbarWith: 'ScreenToolbar',
		actionsConfig:
		{
			refresh: { hidden: true }
		}
	})
export class APDocumentResult extends PXView {

	BranchID: PXFieldState;
	DocType: PXFieldState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

	DocDate: PXFieldState;
	FinPeriodID: PXFieldState;
	Status: PXFieldState;
	CuryID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryBegBalance: PXFieldState;

	CuryDocBal: PXFieldState;
	CuryDiscActTaken: PXFieldState;
	CuryTaxWheld: PXFieldState;
	CuryRetainageTotal: PXFieldState;
	CuryOrigDocAmtWithRetainageTotal: PXFieldState;
	CuryRetainageUnreleasedAmt: PXFieldState;
	OrigDocAmt: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	BegBalance: PXFieldState;

	DocBal: PXFieldState;
	DiscActTaken: PXFieldState;
	TaxWheld: PXFieldState;
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
	SuppliedByVendorID: PXFieldState;

}

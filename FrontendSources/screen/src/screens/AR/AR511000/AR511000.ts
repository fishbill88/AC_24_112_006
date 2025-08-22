import {
	createCollection, createSingle,
	PXScreen, PXView, PXFieldState, PXActionState,
	graphInfo, viewInfo,
	gridConfig, columnConfig,
	PXFieldOptions, GridColumnType
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARChargeInvoices", primaryView: "Filter",
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR511000 extends PXScreen {
	ViewDocument: PXActionState;

   	@viewInfo({containerName: "Selection"})
	Filter = createSingle(PayBillsFilter);

   	@viewInfo({containerName: "Payment Details"})
    ARDocumentList = createCollection(ARInvoice);
}


export class PayBillsFilter extends PXView {
	PayDate: PXFieldState<PXFieldOptions.CommitChanges>;
	PayFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowOverDueFor: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	OverDueFor: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	ShowDueInLessThan: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	DueInLessThan: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	ShowDiscountExparedWithinLast: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	DiscountExparedWithinLast: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	ShowDiscountExpiresInLessThan: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	DiscountExpiresInLessThan: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
}

@gridConfig({
	initNewRow: true,
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class ARInvoice extends PXView {
	@columnConfig({ allowUpdate: false, allowSort: false, allowNull: false, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	DocType: PXFieldState;
	RefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true, format: ">AAAAAAAAAA" })
	CustomerID: PXFieldState;

	CustomerID_BAccountR_acctName: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	DueDate: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	DiscDate: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	InvoiceNbr: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false, hideViewLink: true })
	CuryID: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	CuryOrigDocAmt: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	CuryDocBal: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	CashAccount__CashAccountCD: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	CashAccount__CuryID: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	CustomerPaymentMethod__PaymentMethodID: PXFieldState;

	@columnConfig({ allowUpdate: false, allowNull: false })
	CustomerPaymentMethod__Descr: PXFieldState;

	TermsID: PXFieldState;
	Customer__StatementCycleID: PXFieldState;
}

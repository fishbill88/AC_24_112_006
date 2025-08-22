import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection, columnConfig, linkCommand, PXActionState, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AR.ARPrintInvoices', primaryView: 'Filter' })
export class AR508000 extends PXScreen {

	EditDetail: PXActionState;

	Filter = createSingle(PrintInvoicesFilter);
	ARDocumentList = createCollection(ARInvoice);
}

export class PrintInvoicesFilter extends PXView {

	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	MyWorkGroup: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAll: PXFieldState<PXFieldOptions.CommitChanges>;
	BeginDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintWithDeviceHub: PXFieldState<PXFieldOptions.CommitChanges>;
	DefinePrinterManually: PXFieldState<PXFieldOptions.CommitChanges>;
	PrinterID: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCopies: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class ARInvoice extends PXView {

	@columnConfig({ allowNull: true, allowCheckAll: true, allowSort: false })
	Selected: PXFieldState;
	BranchID: PXFieldState;
	DocType: PXFieldState;

	@linkCommand("EditDetail")
	RefNbr: PXFieldState;
	Status: PXFieldState;
	DocDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;

	CustomerID_BAccountR_acctName: PXFieldState;
	DueDate: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	CuryOrigDiscAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;

	InvoiceNbr: PXFieldState;
	Printed: PXFieldState;
	DontPrint: PXFieldState;
	Emailed: PXFieldState;
	DontEmail: PXFieldState;
}

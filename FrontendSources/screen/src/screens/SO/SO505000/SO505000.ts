import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	linkCommand,
	PXFieldOptions,
	PXActionState,
	viewInfo,
	gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.SO.SOReleaseInvoice', primaryView: 'Filter' })
export class SO505000 extends PXScreen {

	viewDocument: PXActionState;

	@viewInfo({ containerName: "Process Invoices and Memos Filter" })
	Filter = createSingle(Filter);
	@viewInfo({ containerName: "Invoices" })
	SOInvoiceList = createCollection(SOInvoiceList);
}

export class Filter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowPrinted: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintWithDeviceHub: PXFieldState<PXFieldOptions.CommitChanges>;
	DefinePrinterManually: PXFieldState<PXFieldOptions.CommitChanges>;
	PrinterID: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCopies: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	mergeToolbarWith: 'ScreenToolbar',
	batchUpdate: true
})
export class SOInvoiceList extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	DocType: PXFieldState;
	@linkCommand('viewDocument')
	RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })CustomerID: PXFieldState;
	CustomerID_BAccountR_acctName: PXFieldState;
	@columnConfig({ hideViewLink: true })CustomerLocationID: PXFieldState;
	CustomerLocationID_Location_descr: PXFieldState;
	@columnConfig({ hideViewLink: true })InvoiceNbr: PXFieldState;
	Status: PXFieldState;
	CuryPaymentTotal: PXFieldState;
	CuryUnpaidBalance: PXFieldState;
	DocDate: PXFieldState;
	@columnConfig({ hideViewLink: true })FinPeriodID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	@columnConfig({ hideViewLink: true })CuryID: PXFieldState;
	DocDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })TermsID: PXFieldState;
}
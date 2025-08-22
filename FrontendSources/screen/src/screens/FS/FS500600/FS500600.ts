import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	linkCommand,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.CreateInvoiceByServiceOrderPost', primaryView: 'Filter' })
export class FS500600 extends PXScreen {
	ViewPostBatch: PXActionState;
	OpenReviewTemporaryBatch: PXActionState;
	Filter = createSingle(CreateInvoiceFilter);
	PostLines = createCollection(ServiceOrderToPost);
}

export class CreateInvoiceFilter extends PXView {
	PostTo: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingCycleID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	UpToDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IgnoreBillingCycles: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepareInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	ReleaseInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailSalesOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	SOQuickProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	ReleaseBill: PXFieldState<PXFieldOptions.CommitChanges>;
	PayBill: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: 'ScreenToolbar'
})
export class ServiceOrderToPost extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	@columnConfig({ hideViewLink: true }) SrvOrdType: PXFieldState
	RefNbr: PXFieldState;
	BillCustomerID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BillLocationID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BillingCycleCD: PXFieldState;
	CutOffDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchLocationID: PXFieldState;
	Status: PXFieldState;
	DocDesc: PXFieldState;
	@linkCommand("ViewPostBatch") BatchID: PXFieldState;
}

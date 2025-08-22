import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.CreateInvoiceByContractPost', primaryView: 'Filter' })
export class FS501300 extends PXScreen {
	Filter = createSingle(InvoiceContractPeriodFilter);
	Contracts = createCollection(ContractPeriodToPost);
}

export class InvoiceContractPeriodFilter extends PXView {
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceContractID: PXFieldState<PXFieldOptions.CommitChanges>;
	UpToDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class ContractPeriodToPost extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	RefNbr: PXFieldState;
	CustomerContractNbr: PXFieldState;
	BillCustomerID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BillLocationID: PXFieldState;
	BillingPeriod: PXFieldState;
	BranchID: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchLocationID: PXFieldState;
	DocDesc: PXFieldState;
	Status: PXFieldState;
	ContractPostBatchID: PXFieldState;
}

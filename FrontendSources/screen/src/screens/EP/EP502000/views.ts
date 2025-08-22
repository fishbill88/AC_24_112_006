import {
	columnConfig,
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	InvoiceDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InvFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class Customers extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	CustomerClassID: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.Disabled>;
	CustomerID_BAccountR_acctName: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState<PXFieldOptions.Disabled>;
	LocationID_Location_descr: PXFieldState;
}

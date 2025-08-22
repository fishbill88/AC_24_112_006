import {
	columnConfig,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	BeginDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class Items extends PXView {
	@columnConfig({
		allowCheckAll: true,
		allowUpdate: false,
		allowSort: false,
		width: 30
	})
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("viewDocumentProject")
	ProjectID: PXFieldState;
	@linkCommand("viewDocumentRef")
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;
	Status: PXFieldState;
	InvoiceDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	CustomerID_BAccountR_acctName: PXFieldState;
	CuryDocTotal: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	DueDate: PXFieldState;
	DiscDate: PXFieldState;
}

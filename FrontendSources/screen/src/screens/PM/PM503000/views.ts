import {
	columnConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	InvoiceDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InvFinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	StatementCycleId: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	TemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Items extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@linkCommand("viewDocumentProject")
	ContractCD: PXFieldState;
	@columnConfig({ width: 350 })
	Description: PXFieldState<PXFieldOptions.Disabled>;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState<PXFieldOptions.Disabled>;
	CustomerID_Customer_acctName: PXFieldState<PXFieldOptions.Disabled>;
	FromDate: PXFieldState<PXFieldOptions.Disabled>;
	NextDate: PXFieldState<PXFieldOptions.Disabled>;
	LastDate: PXFieldState<PXFieldOptions.Disabled>;
}

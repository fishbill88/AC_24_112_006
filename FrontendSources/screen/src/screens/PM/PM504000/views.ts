import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	linkCommand
} from "client-controls";

export class Filter extends PXView {
	RecalculateProjectBalances: PXFieldState<PXFieldOptions.Disabled>;
	RecalculateUnbilledSummary: PXFieldState<PXFieldOptions.CommitChanges>;
	RebuildCommitments: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculateDraftInvoicesAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculateChangeOrders: PXFieldState<PXFieldOptions.CommitChanges>;
	RecalculateInclusiveTaxes: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class Items extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@linkCommand("ViewProject")
	ContractCD: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;
	Status: PXFieldState;
	StartDate: PXFieldState;
	ExpireDate: PXFieldState;
}

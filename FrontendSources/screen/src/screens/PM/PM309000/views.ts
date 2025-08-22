import {
	columnConfig,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class Items extends PXView {
	@linkCommand("ViewProject")
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewTask")
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges|PXFieldOptions.Hidden>;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	PMProject__CuryID: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	RevisedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRevisedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CommittedQty: PXFieldState;
	CuryCommittedAmount: PXFieldState;
	CommittedOpenQty: PXFieldState;
	CuryCommittedOpenAmount: PXFieldState;
	CommittedReceivedQty: PXFieldState;
	CommittedInvoicedQty: PXFieldState;
	CuryCommittedInvoicedAmount: PXFieldState;
	ActualQty: PXFieldState;
	CuryActualAmount: PXFieldState;
	Type: PXFieldState;
}

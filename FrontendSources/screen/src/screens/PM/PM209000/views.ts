import {
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Commitments extends PXView {
	BranchID: PXFieldState;
	ExtRefNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	ProjectCuryID: PXFieldState;
	OrigAmount: PXFieldState;
	OrigQty: PXFieldState;
	Amount: PXFieldState;
	Qty: PXFieldState;
	OpenAmount: PXFieldState;
	OpenQty: PXFieldState;
	ReceivedQty: PXFieldState;
	InvoicedAmount: PXFieldState;
	InvoicedQty: PXFieldState;
}

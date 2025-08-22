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
	adjustPageSize: true
})
export class AllocationAuditSource extends PXView {
	@linkCommand("ViewAllocationRule")
	PMAllocationSourceTran__AllocationID: PXFieldState;
	PMAllocationSourceTran__StepID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;
	@linkCommand("ViewBatch")
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState;
	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;
	ProjectID: PXFieldState;
	TaskID: PXFieldState;
	AccountGroupID: PXFieldState;
	ResourceID: PXFieldState;
	BAccountID: PXFieldState;
	LocationID: PXFieldState;
	InventoryID: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	Qty: PXFieldState;
	Billable: PXFieldState;
	UseBillableQty: PXFieldState;
	BillableQty: PXFieldState;
	UnitRate: PXFieldState;
	Amount: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OffsetAccountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OffsetSubID: PXFieldState;
	OffsetAccountGroupID: PXFieldState;
	Allocated: PXFieldState;
	Released: PXFieldState;
	BatchNbr: PXFieldState;
	OrigModule: PXFieldState;
	OrigTranType: PXFieldState;
	OrigRefNbr: PXFieldState;
	OrigLineNbr: PXFieldState;
	Billed: PXFieldState;
	BilledDate: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	Reverse: PXFieldState;
	EarningType: PXFieldState;
	OvertimeMultiplier: PXFieldState;
	ARRefNbr: PXFieldState;
	Skip: PXFieldState;
}

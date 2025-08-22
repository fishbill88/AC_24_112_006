import {
	columnConfig,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCode: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DateFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTo: PXFieldState<PXFieldOptions.CommitChanges>;
	ResourceID: PXFieldState<PXFieldOptions.CommitChanges>;
	OnlyAllocation: PXFieldState<PXFieldOptions.CommitChanges>;
	IncludeUnreleased: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ARDocType: PXFieldState<PXFieldOptions.CommitChanges>;
	ARRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	TranID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class Transactions extends PXView {
	TranID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;
	PMRegister__Module: PXFieldState;
	@linkCommand("ViewDocument")
	PMRegister__RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState;
	TaskID: PXFieldState;
	@linkCommand("ViewInventory")
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState;
	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;
	Description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	Qty: PXFieldState;
	BillableQty: PXFieldState;
	TranCuryUnitRate: PXFieldState;
	TranCuryAmount: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TranCuryID: PXFieldState;
	ProjectCuryAmount: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProjectCuryID: PXFieldState;
	Released: PXFieldState;
	Billable: PXFieldState;
	Billed: PXFieldState;
	@linkCommand("ViewProforma")
	ProformaRefNbr: PXFieldState;
	@linkCommand("ViewInvoice")
	ARRefNbr: PXFieldState;
	@linkCommand("ViewCustomer")
	BAccountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ResourceID: PXFieldState;
	BAccount__AcctName: PXFieldState;
	Allocated: PXFieldState;
	PMRegister__OrigDocType: PXFieldState;
	@linkCommand("ViewOrigDocument")
	PMRegister__OrigNoteID: PXFieldState;
	PMRegister__OrigDocNbr: PXFieldState;
	BatchNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AccountGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OffsetAccountGroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OffsetAccountID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OffsetSubID: PXFieldState;
	EarningType: PXFieldState;
	OvertimeMultiplier: PXFieldState;
	ExcludedFromAllocation: PXFieldState;
	ExcludedFromBilling: PXFieldState;
	ExcludedFromBillingReason: PXFieldState;
	ExcludedFromBalance: PXFieldState;
}

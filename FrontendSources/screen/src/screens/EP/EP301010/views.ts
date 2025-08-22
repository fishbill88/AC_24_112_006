import {
	columnConfig,
	GridColumnDisplayMode,
	gridConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

export class Filter extends PXView {
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true,
	allowUpdate: false
})
export class ClaimDetails extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	ClaimDetailCD: PXFieldState;
	ContractID: PXFieldState;
	TaskID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState;
	ExpenseDate: PXFieldState;
	Status: PXFieldState;
	@linkCommand("editDetail")
	TranDesc: PXFieldState;
	ExpenseRefNbr: PXFieldState;
	CuryTranAmtWithTaxes: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	@linkCommand("viewClaim")
	RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true, displayMode: GridColumnDisplayMode.Text })
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	CreatedByID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;
}

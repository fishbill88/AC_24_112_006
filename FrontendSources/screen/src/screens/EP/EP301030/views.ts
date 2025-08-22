import {
	gridConfig,
	columnConfig,
	linkCommand,
	PXFieldOptions,
	PXFieldState,
	PXView,
	GridPreset,
	GridColumnDisplayMode
} from "client-controls";

export class Filter extends PXView {
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry
})
export class Claim extends PXView {
	DocDate: PXFieldState;
	@linkCommand("EditDetail")
	RefNbr: PXFieldState;
	Status: PXFieldState;
	DocDesc: PXFieldState;
	CuryDocBal: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	@columnConfig({ hideViewLink: true, displayMode: GridColumnDisplayMode.Text })
	EmployeeID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CreatedByID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DepartmentID: PXFieldState;
	ApproveDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;
}

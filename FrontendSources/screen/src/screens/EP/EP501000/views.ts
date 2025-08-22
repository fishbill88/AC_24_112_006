import {
	columnConfig,
	gridConfig,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	adjustPageSize: false
})
export class EPDocumentList extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	RefNbr: PXFieldState<PXFieldOptions.Disabled>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DocDate: PXFieldState<PXFieldOptions.Disabled>;
	CuryDocBal: PXFieldState<PXFieldOptions.Disabled>;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState<PXFieldOptions.Disabled>;
	EmployeeID: PXFieldState<PXFieldOptions.Disabled>;
	EmployeeID_description: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DepartmentID: PXFieldState<PXFieldOptions.Disabled>;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.Disabled>;
}

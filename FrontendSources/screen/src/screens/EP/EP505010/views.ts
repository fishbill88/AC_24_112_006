import {
	columnConfig,
	gridConfig,
	GridColumnDisplayMode,
	PXFieldState,
	PXFieldOptions,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: true
})
export class FilteredItems extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	TimeCardCD: PXFieldState;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 150 })
	EmployeeID_description: PXFieldState;
	@columnConfig({
		displayMode: GridColumnDisplayMode.Text,
		hideViewLink: true
	})
	WeekID: PXFieldState;
	TimeSpent: PXFieldState<PXFieldOptions.Disabled>;
	OvertimeSpent: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.NoLabel>;
	TotalTimeSpent: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.NoLabel>;
	TimeBillable: PXFieldState<PXFieldOptions.Disabled>;
	OvertimeBillable: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.NoLabel>;
	TotalTimeBillable: PXFieldState<PXFieldOptions.Disabled|PXFieldOptions.NoLabel>;
	@columnConfig({
		displayMode: GridColumnDisplayMode.Text,
		width: 150
	})
	EPApprovalEx__ApprovedByID: PXFieldState;
	@columnConfig({ width: 150 })
	EPEmployeeEx__AcctName: PXFieldState;
	EPApprovalEx__ApproveDate: PXFieldState;
}

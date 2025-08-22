import {
	columnConfig,
	gridConfig,
	GridColumnDisplayMode,
	PXFieldOptions,
	PXFieldState,
	PXView
} from "client-controls";

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false
})
export class FilteredItems extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	TimeCardCD: PXFieldState;
	EquipmentID: PXFieldState;
	@columnConfig({
		displayMode: GridColumnDisplayMode.Text,
		hideViewLink: true
	})
	WeekID: PXFieldState;
	TimeSetupCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeRunCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeSuspendCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeTotalCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeBillableSetupCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeBillableRunCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeBillableSuspendCalc: PXFieldState<PXFieldOptions.Disabled>;
	TimeBillableTotalCalc: PXFieldState<PXFieldOptions.Disabled>;
	@columnConfig({
		displayMode: GridColumnDisplayMode.Text,
		hideViewLink: true
	})
	EPApproval__ApprovedByID: PXFieldState;
	EPApproval__ApproveDate: PXFieldState;
}

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
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Items extends PXView {
	@columnConfig({ hideViewLink: true })
	EquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	TimecardCD: PXFieldState<PXFieldOptions.Disabled>;
	EPEquipmentTimeCardSpentTotals__SetupTime: PXFieldState<PXFieldOptions.Disabled>;
	EPEquipmentTimeCardSpentTotals__RunTime: PXFieldState<PXFieldOptions.Disabled>;
	EPEquipmentTimeCardSpentTotals__SuspendTime: PXFieldState<PXFieldOptions.Disabled>;
	EPEquipmentTimeCardSpentTotals__TimeTotalCalc: PXFieldState<PXFieldOptions.Disabled>;
	EPEquipmentTimeCardBillableTotals__SetupTime: PXFieldState<PXFieldOptions.Disabled>;
	EPEquipmentTimeCardBillableTotals__RunTime: PXFieldState<PXFieldOptions.Disabled>;
	EPEquipmentTimeCardBillableTotals__SuspendTime: PXFieldState<PXFieldOptions.Disabled>;
	EPEquipmentTimeCardBillableTotals__TimeTotalCalc: PXFieldState<PXFieldOptions.Disabled>;
}

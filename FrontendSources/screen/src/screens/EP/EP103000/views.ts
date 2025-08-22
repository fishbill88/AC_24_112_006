import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnType, TextAlign, GridPreset } from "client-controls";

@gridConfig({
	preset: GridPreset.Details,
	autoRepaint: ["Rates"]
})
export class EPShiftCode extends PXView {
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox }) IsActive: PXFieldState;
	@columnConfig({ width: 150 }) ShiftCD: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ width: 250 }) Description: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class EPShiftCodeRate extends PXView {
	EffectiveDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Percent: PXFieldState<PXFieldOptions.CommitChanges>;
	WageAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	CostingAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	BurdenAmount: PXFieldState;
}

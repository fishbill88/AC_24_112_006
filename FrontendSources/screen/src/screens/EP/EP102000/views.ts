import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnType, TextAlign,  GridPreset } from "client-controls";

@gridConfig({
	preset: GridPreset.Primary
})
export class EPEarningType extends PXView  {
	TypeCD : PXFieldState;
	Description : PXFieldState;
	@columnConfig({textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	isActive : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	isOvertime : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({textAlign: TextAlign.Right})	OvertimeMultiplier : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	isBillable : PXFieldState;
	ProjectID : PXFieldState;
	TaskID : PXFieldState;
}

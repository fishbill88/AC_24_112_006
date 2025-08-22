import { PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig } from "client-controls";

export class TimecardFilter extends PXView  {
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class TimecardWithTotals extends PXView  {
	@columnConfig({ hideViewLink: true })
	WeekID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TimeCardCD: PXFieldState;
	@columnConfig({ hideViewLink: true })
	EmployeeID : PXFieldState;
	Status : PXFieldState;
	TimeSpentCalc : PXFieldState;
	TimeBillableCalc : PXFieldState;
	OvertimeSpentCalc : PXFieldState;
	OvertimeBillableCalc : PXFieldState;
	TotalSpentCalc : PXFieldState;
	TotalBillableCalc : PXFieldState;
	EmployeeID_description : PXFieldState;
	BillingRateCalc : PXFieldState;
	WeekStartDate : PXFieldState;
}

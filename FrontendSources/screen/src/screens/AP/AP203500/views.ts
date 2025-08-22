import {
	PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig
} from "client-controls";

// Views

export class Schedule extends PXView {
	ScheduleID: PXFieldState;
	Active: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	NoEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RunLimit: PXFieldState;
	NoRunLimit: PXFieldState<PXFieldOptions.CommitChanges>;
	FormScheduleType: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleName: PXFieldState;
	LastRunDate: PXFieldState<PXFieldOptions.Disabled>;
	NextRunDate: PXFieldState<PXFieldOptions.Disabled>;
	RunCntr: PXFieldState<PXFieldOptions.Disabled>;
	PeriodDateSel: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodFrequency: PXFieldState;
	Periods: PXFieldState<PXFieldOptions.Disabled>;
	PeriodFixedDay: PXFieldState;
	MonthlyDaySel: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyFrequency: PXFieldState;
	Months: PXFieldState<PXFieldOptions.Disabled>;
	MonthlyOnDay: PXFieldState;
	MonthlyOnWeek: PXFieldState;
	MonthlyOnDayOfWeek: PXFieldState;
	WeeklyFrequency: PXFieldState;
	Weeks: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnDay1: PXFieldState;
	WeeklyOnDay2: PXFieldState;
	WeeklyOnDay3: PXFieldState;
	WeeklyOnDay4: PXFieldState;
	WeeklyOnDay5: PXFieldState;
	WeeklyOnDay6: PXFieldState;
	WeeklyOnDay7: PXFieldState;
	DailyFrequency: PXFieldState;
	Days: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ adjustPageSize: true })
export class APRegister extends PXView {
	@linkCommand('ViewDocument')
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	VendorID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocDate: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	FinPeriodID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryOrigDocAmt: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocDesc: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocType: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorID_Vendor_acctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;

}

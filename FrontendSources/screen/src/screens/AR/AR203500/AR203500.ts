import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, PXFieldOptions, PXActionState } from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.ARScheduleMaint", primaryView: "Schedule_Header", showUDFIndicator: true  })
export class AR203500 extends PXScreen {

	ViewDocument: PXActionState;
	ViewGenDocument: PXActionState;


	@viewInfo({ containerName: "Schedule Summary" })
	Schedule_Header = createSingle(Schedule);

	@viewInfo({ containerName: "Document List" })
	Document_Detail = createCollection(ARRegister);

	@viewInfo({ containerName: "Generated Documents" })
	Document_History = createCollection(ARRegister2);

}

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

export class ARRegister extends PXView {

	RefNbr: PXFieldState;
	CustomerID: PXFieldState;
	DocDate: PXFieldState;
	FinPeriodID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	CuryID: PXFieldState;
	DocDesc: PXFieldState;
	DocType: PXFieldState;
	CustomerID_BAccountR_acctName: PXFieldState;

}

export class ARRegister2 extends PXView {

	RefNbr: PXFieldState;
	CustomerID: PXFieldState;
	DocDate: PXFieldState;
	FinPeriodID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	CuryID: PXFieldState;
	DocDesc: PXFieldState;
	DocType: PXFieldState;
	CustomerID_BAccountR_acctName: PXFieldState;
	Status: PXFieldState;

}

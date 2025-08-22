import {
	graphInfo,
	createSingle,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.StaffContractScheduleEntry', primaryView: 'StaffScheduleRecords' })
export class FS202001 extends PXScreen {
	StaffScheduleRecords = createSingle(FSStaffSchedule);
	StaffScheduleSelected = createSingle(FSStaffSchedule);
}

export class FSStaffSchedule extends PXView {
	RefNbr: PXFieldState;
	EmployeeID: PXFieldState;
	BranchID: PXFieldState;
	BranchLocationID: PXFieldState;
	StaffScheduleDescription: PXFieldState;
	EndDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	EnableExpirationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	StartTime_Time: PXFieldState;
	EndTime_Time: PXFieldState;
	ScheduleType: PXFieldState;
	FrequencyType: PXFieldState<PXFieldOptions.CommitChanges>;
	RecurrenceDescription: PXFieldState<PXFieldOptions.Readonly>;
	DailyFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	DailyLabel: PXFieldState<PXFieldOptions.Readonly>;
	WeeklyFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyLabel: PXFieldState<PXFieldOptions.Readonly>;
	WeeklyOnSun: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnMon: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnTue: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnWed: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnThu: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnFri: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnSat: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyLabel: PXFieldState<PXFieldOptions.Readonly>;
	MonthlyRecurrenceType1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyRecurrenceType2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyRecurrenceType3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyRecurrenceType4: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay4: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek4: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek4: PXFieldState<PXFieldOptions.CommitChanges>;
	Monthly2Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Monthly3Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Monthly4Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	YearlyLabel: PXFieldState<PXFieldOptions.Readonly>;
	AnnualOnJan: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnFeb: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnMar: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnApr: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnMay: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnJun: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnJul: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnAug: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnSep: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnOct: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnNov: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnDec: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualRecurrenceType: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnDay: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnWeek: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnDayOfWeek: PXFieldState<PXFieldOptions.CommitChanges>;
}

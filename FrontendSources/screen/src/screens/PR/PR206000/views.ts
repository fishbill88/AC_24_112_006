import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig
} from "client-controls";

export class FiscalYearSetup extends PXView {
	PayGroupID: PXFieldState;
	FirstFinYear: PXFieldState<PXFieldOptions.CommitChanges>;
	BegFinYear: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodType: PXFieldState<PXFieldOptions.CommitChanges>;
	EndYearDayOfWeek: PXFieldState;
	TranDayOfWeek: PXFieldState;
	TranWeekDiff: PXFieldState;
	PeriodsStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SecondPeriodsStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TransactionsStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SecondTransactionsStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IsSecondWeekOfYear: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriods: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodLength: PXFieldState;
	UserDefined: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ wrapToolbar: true, adjustPageSize: false })
export class Periods extends PXView {
	@columnConfig({ allowUpdate: false })
	PeriodNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	StartDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	EndDateUI: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	TransactionDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
}

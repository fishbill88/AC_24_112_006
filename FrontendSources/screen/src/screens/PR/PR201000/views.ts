import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
	columnConfig
} from "client-controls";

export class PayrollYear extends PXView {
	PayGroupID: PXFieldState;
	Year: PXFieldState;
	StartDate: PXFieldState<PXFieldOptions.Disabled>;
	FinPeriods: PXFieldState;
	OverrideFinPeriods: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class Periods extends PXView {
	PeriodNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;
	StartDate: PXFieldState;
	EndDateUI: PXFieldState<PXFieldOptions.CommitChanges>;
	TransactionDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	FinYear: PXFieldState;
}

export class PeriodCreation extends PXView {
	UseExceptions: PXFieldState<PXFieldOptions.CommitChanges>;
	ExceptionDateBehavior: PXFieldState<PXFieldOptions.CommitChanges>;
}

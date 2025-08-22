import {
	PXScreen, createSingle, graphInfo, PXView, PXFieldState, PXFieldOptions, createCollection
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.GL.FiscalYearSetupMaint', primaryView: 'FiscalYearSetup' })
export class GL101000 extends PXScreen {

	FiscalYearSetup = createSingle(FiscalYearSetup);
	Periods = createCollection(Periods);

}

export class FiscalYearSetup extends PXView {

	FirstFinYear: PXFieldState;
	BegFinYear: PXFieldState<PXFieldOptions.CommitChanges>;
	BelongsToNextYear: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodType: PXFieldState<PXFieldOptions.CommitChanges>;
	EndYearDayOfWeek: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodsStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriods: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodLength: PXFieldState<PXFieldOptions.CommitChanges>;
	UserDefined: PXFieldState<PXFieldOptions.CommitChanges>;
	HasAdjustmentPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	EndYearCalcMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	YearLastDayOfWeek: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class Periods extends PXView {

	PeriodNbr: PXFieldState;
	StartDateUI: PXFieldState;
	EndDateUI: PXFieldState;
	Descr: PXFieldState;

}

import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, gridConfig } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.FA.FABookYearSetupMaint', primaryView: 'FiscalYearSetup', })
export class FA206000 extends PXScreen {

	@viewInfo({ containerName: 'Calendar Summary' })
	FiscalYearSetup = createSingle(FABookYearSetup);

	@viewInfo({ containerName: 'Periods' })
	Periods = createCollection(FABookPeriodSetup);

}

@gridConfig({ allowInsert: false })
export class FABookYearSetup extends PXView {

	BookID: PXFieldState;
	FirstFinYear: PXFieldState<PXFieldOptions.Disabled>;
	BegFinYear: PXFieldState<PXFieldOptions.CommitChanges>;
	BelongsToNextYear: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodType: PXFieldState<PXFieldOptions.CommitChanges>;
	EndYearDayOfWeek: PXFieldState;
	PeriodsStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriods: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodLength: PXFieldState;
	HasAdjustmentPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	UserDefined: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ initNewRow: false })
export class FABookPeriodSetup extends PXView {

	@columnConfig({ allowUpdate: false })
	PeriodNbr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	StartDateUI: PXFieldState;

	@columnConfig({ allowUpdate: false })
	EndDateUI: PXFieldState;

	Descr: PXFieldState;

}

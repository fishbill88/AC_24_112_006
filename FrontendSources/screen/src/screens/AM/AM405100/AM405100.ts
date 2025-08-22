import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';

export class Filter extends PXView {
	WcID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShiftCD: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowAll: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class ScheduleDetail extends PXView {
	WcID: PXFieldState;
	ShiftCD: PXFieldState;
	SchdBlocks: PXFieldState;
	@linkCommand("ViewSchedule") SchdDate: PXFieldState;
	StartTime: PXFieldState;
	EndTime: PXFieldState;
	CrewSize: PXFieldState;
	ShiftCrewSize: PXFieldState;
	CrewSizeShortage: PXFieldState;
	OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	OperationID: PXFieldState;
}


@graphInfo({ graphType: 'PX.Objects.AM.WorkCenterCrewScheduleInq', primaryView: 'Filter' })
export class AM405100 extends PXScreen {
	// to remove the button from the screen toolbar
	ViewSchedule: PXActionState;

	Filter = createSingle(Filter);
	ScheduleDetail = createCollection(ScheduleDetail);
}

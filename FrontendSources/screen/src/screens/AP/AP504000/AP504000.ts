import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, createCollection, createSingle, PXScreen, graphInfo, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APScheduleRun', primaryView: 'Filter', hideFilesIndicator: true, hideNotesIndicator: true })
export class AP504000 extends PXScreen {

	ViewSchedule: PXActionState;

	Filter = createSingle(Parameters);
	Schedule_List = createCollection(Schedule);

}

export class Parameters extends PXView {
	ExecutionDate: PXFieldState<PXFieldOptions.CommitChanges>;
	LimitTypeSel: PXFieldState<PXFieldOptions.CommitChanges>;
	RunLimit: PXFieldState;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class Schedule extends PXView {
	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;
	@linkCommand('ViewSchedule')
	@columnConfig({ allowUpdate: false })
	ScheduleID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ScheduleName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	StartDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	EndDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	RunCntr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	RunLimit: PXFieldState;
	@columnConfig({ allowUpdate: false })
	NextRunDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	LastRunDate: PXFieldState;
}

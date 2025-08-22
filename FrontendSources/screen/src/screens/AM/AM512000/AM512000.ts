import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
} from 'client-controls';

export class Filter extends PXView {
	IsWorkCenterCalendarProcess: PXFieldState;
	IsHistoryCleanupProcess: PXFieldState;
}

export class ProcessingRecords extends PXView {
	WorkCenterCalendarProcessLastRunDateTime: PXFieldState;
	WorkCenterCalendarProcessLastRunByID: PXFieldState;
	BlockSizeSyncProcessLastRunDateTime: PXFieldState;
	BlockSizeSyncProcessLastRunByID: PXFieldState;
	LastBlockSize: PXFieldState;
	CurrentBlockSize: PXFieldState;
	HistoryCleanupProcessLastRunDateTime: PXFieldState;
	HistoryCleanupProcessLastRunByID: PXFieldState;
	WorkCalendarProcessLastRunDateTime: PXFieldState;
	WorkCalendarProcessLastRunByID: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.APSMaintenanceProcess', primaryView: 'ProcessingRecords' })
export class AM512000 extends PXScreen {
	Filter = createSingle(Filter);

	ProcessingRecords = createSingle(ProcessingRecords);
}

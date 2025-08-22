import {
	PXScreen, createSingle, createCollection, graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	gridConfig,
	columnConfig,
	linkCommand
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.DR.DRRecognition', primaryView: 'Filter' })
export class DR501000 extends PXScreen {
	ViewSchedule: PXActionState;

	Filter = createSingle(ScheduleRecognitionFilter);
	Items = createCollection(ScheduledTran);
}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class ScheduledTran extends PXView {
	@columnConfig({ allowCheckAll: true, allowNull: false })
	Selected: PXFieldState;

	@linkCommand("ViewSchedule")
	ScheduleNbr: PXFieldState;

	@columnConfig({ allowNull: false })
	DefCode: PXFieldState;

	DocType: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	ComponentCD: PXFieldState;
	LineNbr: PXFieldState;
	RecDate: PXFieldState;

	@columnConfig({ allowNull: false })
	Amount: PXFieldState;

	BaseCuryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	FinPeriodID: PXFieldState;
}

export class ScheduleRecognitionFilter extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeferredCode: PXFieldState<PXFieldOptions.CommitChanges>;
	RecDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

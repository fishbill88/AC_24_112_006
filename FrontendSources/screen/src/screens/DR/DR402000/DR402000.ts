import {
	PXView, PXFieldState, PXActionState, PXScreen, PXFieldOptions,
	gridConfig, columnConfig, linkCommand, graphInfo,
	createSingle, createCollection
} from 'client-controls';


@graphInfo({ graphType: 'PX.Objects.DR.ScheduleTransInq', primaryView: 'Filter' })
export class DR402000 extends PXScreen {
	ViewSchedule: PXActionState;
	ViewDoc: PXActionState;
	ViewBatch: PXActionState;

	Filter = createSingle(ScheduleTransFilter);
	Records = createCollection(DRScheduleTran);
}

export class ScheduleTransFilter extends PXView {
	AccountType: PXFieldState<PXFieldOptions.CommitChanges>;
	DeferredCode: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	UseMasterCalendar: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false, mergeToolbarWith: 'ScreenToolbar' })
export class DRScheduleTran extends PXView {
	@linkCommand('ViewSchedule')
	ScheduleID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DRScheduleDetail__DefCode: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DRScheduleDetail__BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DRScheduleDetail__ComponentID: PXFieldState;

	RecDate: PXFieldState;
	TranDate: PXFieldState;
	FinPeriodID: PXFieldState;
	Amount: PXFieldState;
	DRSchedule__BaseCuryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	DRScheduleDetail__DocType: PXFieldState;

	@linkCommand('ViewDoc')
	DRScheduleDetail__RefNbr: PXFieldState;

	DRScheduleDetail__LineNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DRScheduleDetail__BAccountID: PXFieldState;

	@linkCommand('ViewBatch')
	BatchNbr: PXFieldState;
}

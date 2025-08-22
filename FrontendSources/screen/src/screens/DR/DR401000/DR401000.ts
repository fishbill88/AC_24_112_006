import {
	PXView, PXFieldState, PXActionState, PXScreen, PXFieldOptions,
	gridConfig, columnConfig, linkCommand, graphInfo, viewInfo,
	createSingle, createCollection
} from 'client-controls';


@graphInfo({ graphType: 'PX.Objects.DR.SchedulesInq', primaryView: 'Filter' })
export class DR401000 extends PXScreen {
	ViewSchedule: PXActionState;
	ViewDocument: PXActionState;

    @viewInfo({containerName: 'Selection'})
	Filter = createSingle(SchedulesFilter);

	@viewInfo({ containerName: 'Deferral Schedules' })
	Records = createCollection(SchedulesInqResult);
}

export class SchedulesFilter extends PXView {
	AccountType: PXFieldState<PXFieldOptions.CommitChanges>;
	DeferredCode: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalScheduled: PXFieldState;
	TotalDeferred: PXFieldState;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false, mergeToolbarWith: 'ScreenToolbar' })
export class SchedulesInqResult extends PXView {
	@linkCommand('ViewSchedule')
	DRSchedule__ScheduleNbr: PXFieldState;

	DocumentType: PXFieldState;

	@linkCommand('ViewDocument')
	RefNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	ComponentCD: PXFieldState;
	LineNbr: PXFieldState;
	DocDate: PXFieldState;
	Status: PXFieldState;
	SignTotalAmt: PXFieldState;
	SignDefAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DefCode: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DefAcctID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DefSubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BAccountID: PXFieldState;

	BAccountID_BAccountR_acctName: PXFieldState;
}

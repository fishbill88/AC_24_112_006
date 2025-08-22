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

@graphInfo({ graphType: 'PX.Objects.DR.DRDraftScheduleProc', primaryView: 'Filter' })
export class DR503000 extends PXScreen {
	ViewSchedule: PXActionState;
	ViewDocument: PXActionState;

	Filter = createSingle(SchedulesFilter);
	Items = createCollection(DRScheduleDetail);
}

@gridConfig({ adjustPageSize: true, syncPosition: true, mergeToolbarWith: 'ScreenToolbar' })
export class DRScheduleDetail extends PXView {
	@columnConfig({ allowCheckAll: true, allowNull: false })
	Selected: PXFieldState;

	@linkCommand("ViewSchedule")
	ScheduleID: PXFieldState;

	DefCode: PXFieldState;
	DocumentType: PXFieldState;

	@linkCommand("ViewDocument")
	RefNbr: PXFieldState;

	@columnConfig({hideViewLink: true })
	BranchID: PXFieldState;

	ComponentID: PXFieldState;
	LineNbr: PXFieldState;

	@columnConfig({ allowNull: false })
	TotalAmt: PXFieldState;

	DRSchedule__BaseCuryID: PXFieldState;

	@columnConfig({ allowNull: false })
	DefAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DefAcctID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DefSubID: PXFieldState;

	@columnConfig({ allowNull: false })
	DocDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	BAccountID: PXFieldState;
}

export class SchedulesFilter extends PXView {
	AccountType: PXFieldState<PXFieldOptions.CommitChanges>;
	DeferredCode: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
}

import {
	createCollection, createSingle,
	PXScreen, PXActionState, PXView, PXFieldState, PXFieldOptions,
	graphInfo, viewInfo, gridConfig, columnConfig, linkCommand
} from 'client-controls';


@graphInfo({graphType: 'PX.Objects.DR.DraftScheduleMaint', primaryView: 'Schedule', })
export class DR201500 extends PXScreen {
	ViewBatch: PXActionState;
	ViewSchedule: PXActionState;

	@viewInfo({containerName: 'Deferral Schedule'})
	Schedule = createSingle(DRSchedule);

	@viewInfo({ containerName: 'Components' })
	Components = createCollection(DRScheduleDetail);

	@viewInfo({ containerName: 'Transactions' })
	Transactions = createCollection(DRScheduleTran);

	@viewInfo({ containerName: 'Components' })
	ReallocationPool = createCollection(ARTran);

	@viewInfo({ containerName: 'Original Schedules' })
	Associated = createCollection(DRScheduleEx);
}


export class DRSchedule extends PXView {
	ScheduleNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsCustom: PXFieldState<PXFieldOptions.Hidden>;
	IsPoolVisible: PXFieldState<PXFieldOptions.Hidden>;
	DocumentType: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	LineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseCuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrigLineAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryNetTranPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountLocID: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaskID: PXFieldState;
	IsOverridden: PXFieldState<PXFieldOptions.CommitChanges>;
	TermStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TermEndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ComponentsTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	DefTotal: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseCuryIDASC606: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, autoRepaint: ["Transactions"] })
export class DRScheduleDetail extends PXView {
	GenerateTransactions: PXActionState;

	@columnConfig({ hideViewLink: true })
	ComponentID: PXFieldState;

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

	TermStartDate: PXFieldState;
	TermEndDate: PXFieldState;
	ProjectID: PXFieldState;
	TaskID: PXFieldState;
	TotalAmt: PXFieldState;
	DefAmt: PXFieldState;
	DefTotal: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState;

	Status: PXFieldState;
}

@gridConfig({ syncPosition: true })
export class DRScheduleTran extends PXView {
	LineNbr: PXFieldState;
	Status: PXFieldState;
	RecDate: PXFieldState;
	TranDate: PXFieldState;
	Amount: PXFieldState;

	@columnConfig({ hideViewLink: true })
	AccountID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubID: PXFieldState;

	FinPeriodID: PXFieldState;

	@linkCommand('ViewBatch')
	BatchNbr: PXFieldState;
}

@gridConfig({ syncPosition: true, allowDelete: false, allowInsert: false, autoRepaint: ["Transactions"] })
export class ARTran extends PXView {
	RefNbr: PXFieldState;
	ARTran__LineNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ARTran__BranchID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ARTran__InventoryID: PXFieldState;


	ARTran__Qty: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ARTran__UOM: PXFieldState;

	ARTran__CuryExtPrice: PXFieldState;
	ARTran__CuryTranAmt: PXFieldState;
	INComponent__ComponentID: PXFieldState;
	INComponent__Qty: PXFieldState;
	INComponent__UOM: PXFieldState;

	@columnConfig({ hideViewLink: true })
	DRScheduleDetail__DefCode: PXFieldState;

	DRScheduleDetail__FairValuePrice: PXFieldState;
	DRScheduleDetail__DiscountPercent: PXFieldState;
	DRScheduleDetail__EffectiveFairValuePrice: PXFieldState;
	DRScheduleDetail__FairValueCuryID: PXFieldState;
	DRScheduleDetail__Percentage: PXFieldState;
	DRScheduleDetail__TotalAmt: PXFieldState;
}

@gridConfig({ allowDelete: false, allowInsert: false })
export class DRScheduleEx extends PXView {
	@linkCommand('ViewSchedule')
	ScheduleNbr: PXFieldState;

	TranDesc: PXFieldState;
	DocumentTypeEx: PXFieldState;
	RefNbr: PXFieldState;
	FinPeriodID: PXFieldState;
}

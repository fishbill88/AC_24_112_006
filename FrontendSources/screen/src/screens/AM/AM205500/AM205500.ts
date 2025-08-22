import {
	PXScreen,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from 'client-controls';

export class Tools extends PXView {
	ToolID: PXFieldState;
	Descr: PXFieldState;
	Active: PXFieldState;
	ActualUses: PXFieldState;
	AcctID: PXFieldState;
	SubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleQty: PXFieldState;
}

export class CurySettings_AMToolMst extends PXView {
	UnitCost: PXFieldState;
	TotalCost: PXFieldState;
	ActualCost: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.ToolMaint', primaryView: 'Tools' })
export class AM205500 extends PXScreen {
	Tools = createSingle(Tools);
	CurySettings_AMToolMst = createSingle(CurySettings_AMToolMst);
}

import {
	createSingle, PXScreen, graphInfo, viewInfo, PXView, PXFieldState, PXFieldOptions} from "client-controls";

@graphInfo({ graphType: "PX.Objects.AR.ARStatementMaint", primaryView: "ARStatementCycleRecord", showActivitiesIndicator: true })
export class AR202800 extends PXScreen {

	@viewInfo({containerName: "Statement Cycle"})
	ARStatementCycleRecord = createSingle(ARStatementCycle);
}


export class ARStatementCycle extends PXView {

	StatementCycleId: PXFieldState;
	Descr: PXFieldState;
	PrepareOn: PXFieldState<PXFieldOptions.CommitChanges>;
	Day00: PXFieldState<PXFieldOptions.CommitChanges>;
	Day01: PXFieldState<PXFieldOptions.CommitChanges>;
	DayOfWeek: PXFieldState<PXFieldOptions.CommitChanges>;
	LastStmtDate: PXFieldState<PXFieldOptions.Disabled>;
	RequirePaymentApplication: PXFieldState;
	PrintEmptyStatements: PXFieldState;
	UseFinPeriodForAging: PXFieldState<PXFieldOptions.CommitChanges>;
	Bucket01LowerInclusiveBound: PXFieldState;
	Bucket02LowerInclusiveBound: PXFieldState;
	Bucket03LowerInclusiveBound: PXFieldState;
	AgeDays00: PXFieldState<PXFieldOptions.CommitChanges>;
	AgeDays01: PXFieldState<PXFieldOptions.CommitChanges>;
	AgeDays02: PXFieldState<PXFieldOptions.CommitChanges>;
	Bucket04LowerExclusiveBound: PXFieldState;
	AgeMsgCurrent: PXFieldState;
	AgeMsg00: PXFieldState;
	AgeMsg01: PXFieldState;
	AgeMsg02: PXFieldState;
	AgeMsg03: PXFieldState;
	AgeBasedOn: PXFieldState;
	FinChargeApply: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireFinChargeProcessing: PXFieldState;
	FinChargeID: PXFieldState;
}

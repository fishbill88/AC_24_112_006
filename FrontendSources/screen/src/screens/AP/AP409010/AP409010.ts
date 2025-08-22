import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, graphInfo, PXScreen, createSingle, createCollection, PXActionState } from "client-controls";

@graphInfo({ graphType: 'ReconciliationTools.APGLDiscrepancyByAccountEnq', primaryView: 'Filter' })
export class AP409010 extends PXScreen {

	ViewDetails: PXActionState;

	Filter = createSingle(DiscrepancyEnqFilter);
	Rows = createCollection(GLTran);

}

export class DiscrepancyEnqFilter extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodTo: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowOnlyWithDiscrepancy: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalGLAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalXXAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalDiscrepancy: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class GLTran extends PXView {
	@columnConfig({ allowUpdate: false })
	AccountID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	SubID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	FinPeriodID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	GLTurnover: PXFieldState;
	@columnConfig({ allowUpdate: false })
	XXTurnover: PXFieldState;
	@columnConfig({ allowUpdate: false })
	NonXXTrans: PXFieldState;
	@linkCommand('ViewDetails')
	@columnConfig({ allowUpdate: false })
	Discrepancy: PXFieldState;
}

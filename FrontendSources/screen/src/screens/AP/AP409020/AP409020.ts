import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState, graphInfo, PXScreen, createSingle, createCollection } from "client-controls";

@graphInfo({ graphType: 'ReconciliationTools.APGLDiscrepancyByVendorEnq', primaryView: 'Filter' })
export class AP409020 extends PXScreen {

	ViewVendor: PXActionState;
	ViewDetails: PXActionState;

	Filter = createSingle(DiscrepancyEnqFilter);
	Rows = createCollection(APHistoryResult);

}

export class DiscrepancyEnqFilter extends PXView {
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	PeriodFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubCD: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowOnlyWithDiscrepancy: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalGLAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalXXAmount: PXFieldState<PXFieldOptions.Disabled>;
	TotalDiscrepancy: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class APHistoryResult extends PXView {
	@linkCommand('ViewVendor')
	@columnConfig({ allowUpdate: false })
	AcctCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	AcctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	GLTurnover: PXFieldState;
	@columnConfig({ allowUpdate: false })
	XXTurnover: PXFieldState;
	@linkCommand('ViewDetails')
	@columnConfig({ allowUpdate: false })
	Discrepancy: PXFieldState;
}

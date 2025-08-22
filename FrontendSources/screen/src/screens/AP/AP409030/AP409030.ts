import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState, graphInfo, PXScreen, createSingle, createCollection } from "client-controls";

@graphInfo({ graphType: 'ReconciliationTools.APGLDiscrepancyByDocumentEnq', primaryView: 'Filter' })
export class AP409030 extends PXScreen {

	ViewDocument: PXActionState;
	ViewBatch: PXActionState;

	Filter = createSingle(DiscrepancyEnqFilter);
	Rows = createCollection(APDocumentResult);

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
export class APDocumentResult extends PXView {
	@columnConfig({ allowUpdate: false })
	DocType: PXFieldState;
	@linkCommand('ViewDocument')
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;
	@columnConfig({ allowUpdate: false })
	OrigDocAmt: PXFieldState;
	@linkCommand('ViewBatch')
	@columnConfig({ allowUpdate: false })
	BatchNbr: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	FinPeriodID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	GlTurnover: PXFieldState;
	@columnConfig({ allowUpdate: false })
	XXTurnover: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Discrepancy: PXFieldState;
}

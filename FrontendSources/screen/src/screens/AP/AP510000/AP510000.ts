import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, createCollection, createSingle, PXScreen, graphInfo, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APRetainageRelease', primaryView: 'Filter' })
export class AP510000 extends PXScreen {

	ViewDocument: PXActionState;

	Filter = createSingle(APRetainageFilter);
	DocumentList = createCollection(APInvoice);

}

export class APRetainageFilter extends PXView {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowBillsWithOpenBalance: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class APInvoice extends PXView {
	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	BranchID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocType: PXFieldState;
	@linkCommand('ViewDocument')
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	VendorID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	LineNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	RetainageReleasePct: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	CuryRetainageReleasedAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	CuryRetainageUnreleasedCalcAmt: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryOrigDocAmtWithRetainageTotal: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CuryID: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	DisplayProjectID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocDesc: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	FinPeriodID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	InvoiceNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTranInventoryID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTranTaskID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTranCostCodeID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	APTranAccountID: PXFieldState;
}

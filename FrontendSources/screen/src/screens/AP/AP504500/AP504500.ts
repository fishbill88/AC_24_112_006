import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APPPDDebitAdjProcess', primaryView: 'Filter', hideFilesIndicator: true, hideNotesIndicator: true })
export class AP504500 extends PXScreen {

	ViewInvoice: PXActionState;
	ViewPayment: PXActionState;

	Filter = createSingle(APPPDVATAdjParameters);
	Applications = createCollection(APAdjust);

}

export class APPPDVATAdjParameters extends PXView {

	ApplicationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	GenerateOnePerVendor: PXFieldState<PXFieldOptions.CommitChanges>;
	DebitAdjDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class APAdjust extends PXView {

	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	AdjdBranchID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	VendorID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	AdjdDocType: PXFieldState;

	@linkCommand('ViewInvoice')
	@columnConfig({ allowUpdate: false })
	AdjdRefNbr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	AdjdDocDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	InvCuryID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	InvCuryOrigDocAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryAdjdPPDAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	InvTermsID: PXFieldState;

	@linkCommand('ViewPayment')
	@columnConfig({ allowUpdate: false })
	AdjgRefNbr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	InvoiceNbr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocDesc: PXFieldState;

}

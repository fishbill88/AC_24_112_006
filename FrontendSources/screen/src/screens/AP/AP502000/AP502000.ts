
import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState } from 'client-controls';
import { createCollection, createSingle, PXScreen, graphInfo } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APApproveBills', primaryView: 'Filter' })
export class AP502000 extends PXScreen {

	viewDocument: PXActionState;

	Filter = createSingle(ApproveBillsFilter);
	APDocumentList = createCollection(APInvoice);

}

export class ApproveBillsFilter extends PXView {
	SelectionDate: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowApprovedForPayment: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowNotApprovedForPayment: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowPayInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	PayInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	Days: PXFieldState<PXFieldOptions.Disabled>;
	ShowDueInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	DueInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowDiscountExpiresInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscountExpiresInLessThan: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryApprovedTotal: PXFieldState<PXFieldOptions.Disabled>;
	CuryDocsTotal: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class APInvoice extends PXView {

	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	PaySel: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	BranchID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocType: PXFieldState;

	@linkCommand('viewDocument')
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocDesc: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	VendorID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	VendorID_Vendor_acctName: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	SuppliedByVendorID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	SeparateCheck: PXFieldState;

	@columnConfig({ allowUpdate: false })
	PayDate: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	DueDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DiscDate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryDocBal: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryDiscBal: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CuryID: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	PayLocationID: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	PayTypeID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	PayAccountID: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowUpdate: false })
	InvoiceNbr: PXFieldState;

}

import {
	PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, createCollection, createSingle, PXScreen, graphInfo, PXActionState
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APPrintChecks', primaryView: 'Filter' })
export class AP505000 extends PXScreen {

	viewDocument: PXActionState;

	Filter = createSingle(PrintChecksFilter);
	APPaymentList = createCollection(APPayment);

}

export class PrintChecksFilter extends PXView {
	PayTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	NextCheckNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	NextPaymentRefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	GLBalance: PXFieldState<PXFieldOptions.Disabled>;
	CashBalance: PXFieldState<PXFieldOptions.Disabled>;
	CurySelTotal: PXFieldState<PXFieldOptions.Disabled>;
	SelCount: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class APPayment extends PXView {
	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false })
	BranchID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ExtRefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	DocType: PXFieldState;
	@linkCommand('viewDocument')
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	VendorID_Vendor_acctName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CuryOrigDocAmt: PXFieldState;
}

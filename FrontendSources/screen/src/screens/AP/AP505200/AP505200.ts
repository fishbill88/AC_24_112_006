import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, createCollection, createSingle, PXScreen, graphInfo, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AP.APReleaseChecks', primaryView: 'Filter' })
export class AP505200 extends PXScreen {

	viewDocument: PXActionState;
	Reprint: PXActionState;
	VoidReprint: PXActionState;
	Release: PXActionState;

	Filter = createSingle(ReleaseChecksFilter);
	APPaymentList = createCollection(APPayment);

}

export class ReleaseChecksFilter extends PXView {
	PayTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PayAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.Disabled>;
	GLBalance: PXFieldState<PXFieldOptions.Disabled>;
	CashBalance: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class APPayment extends PXView {
	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false })
	PrintCheck: PXFieldState;
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

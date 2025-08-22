import { PXView, PXFieldState, gridConfig, linkCommand, columnConfig, PXActionState, createCollection, PXScreen, graphInfo, viewInfo } from "client-controls";

@graphInfo({ graphType: 'PX.Objects.AP.APDocumentRelease', primaryView: 'APDocumentList' })
export class AP501000 extends PXScreen {

	viewDocument: PXActionState;

	APDocumentList = createCollection(APRegister);

}

@gridConfig({ syncPosition: true, adjustPageSize: true, mergeToolbarWith: 'ScreenToolbar' })
export class APRegister extends PXView {

	@columnConfig({ allowUpdate: false, allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ allowUpdate: false })
	BranchID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocType: PXFieldState;

	@linkCommand('viewDocument')
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	VendorID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	VendorID_Vendor_acctName: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	APInvoice__SuppliedByVendorID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	VendorRefNbr: PXFieldState;

	@columnConfig({ allowUpdate: false })
	Status: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocDate: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	FinPeriodID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	CuryOrigDocAmt: PXFieldState;

	@columnConfig({ allowUpdate: false, hideViewLink: true })
	CuryID: PXFieldState;

	@columnConfig({ allowUpdate: false })
	OrigDocAmt: PXFieldState;

	@columnConfig({ allowUpdate: false })
	DocDesc: PXFieldState;

}

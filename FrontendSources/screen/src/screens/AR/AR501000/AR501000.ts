import {
	createCollection,
	PXView, PXFieldState,
	PXScreen, PXActionState,
	graphInfo, viewInfo, gridConfig, columnConfig,
	GridColumnType
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.AR.ARDocumentRelease", primaryView: "ARDocumentList",
	hideFilesIndicator: true, hideNotesIndicator: true
})
export class AR501000 extends PXScreen {
	viewDocument: PXActionState;
	ARDocumentList_refNbr_ViewDetails: PXActionState;

   	@viewInfo({containerName: "AR Documents"})
	ARDocumentList = createCollection(ARRegister);
}

@gridConfig({
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar",
	quickFilterFields: ['RefNbr', 'CustomerID', 'CustomerID_BAccountR_acctName']
})
export class ARRegister extends PXView {
	@columnConfig({ allowSort: false, allowCheckAll: true, type: GridColumnType.CheckBox })
	Selected: PXFieldState;

	BranchID: PXFieldState;
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	PaymentMethodID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CustomerID: PXFieldState;

	CustomerID_BAccountR_acctName: PXFieldState;
	CustomerRefNbr: PXFieldState;
	Status: PXFieldState;
	DocDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FinPeriodID: PXFieldState;

	CuryOrigDocAmt: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;

	OrigDocAmt: PXFieldState;
	DocDesc: PXFieldState;
}

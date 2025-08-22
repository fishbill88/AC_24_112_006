import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.AMDocumentRelease', primaryView: 'AMDocumentList' })
export class AM503000 extends PXScreen {
	AMDocumentList = createCollection(AMBatch);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class AMBatch extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	BatNbr: PXFieldState;
	DocType: PXFieldState;
	Status: PXFieldState;
	TranDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) FinPeriodID: PXFieldState;
	TotalQty: PXFieldState;
	TotalCost: PXFieldState;
	TotalAmount: PXFieldState;
	TranDesc: PXFieldState;
	@columnConfig({ hideViewLink: true }) BranchID: PXFieldState;
}

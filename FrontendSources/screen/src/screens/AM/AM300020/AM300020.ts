import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.MatlWizard2', primaryView: 'ProcessMatl' })
export class AM300020 extends PXScreen {
	ProcessMatl = createCollection(ProcessMatl);
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
})
export class ProcessMatl extends PXView {
	Selected: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	QtyReq: PXFieldState;
	MatlQty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	QtyAvail: PXFieldState;
	SiteID: PXFieldState;
	LocationID: PXFieldState;
	OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	IsByproduct: PXFieldState;
	InventoryID_description: PXFieldState;
	UnreleasedBatchQty: PXFieldState<PXFieldOptions.CommitChanges>;
}

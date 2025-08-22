import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.LateAssignmentMaint', primaryView: 'ProdItemSplits' })
export class AM312000 extends PXScreen {
	ProdItemSplits = createSingle(AMProdItemSplitPreassign);
	MatlAssigned = createCollection(AMProdMatlLotSerialAssigned);
	MatlUnassigned = createCollection(AMProdMatlLotSerialUnassigned);
}

export class AMProdItemSplitPreassign extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusID: PXFieldState;
	InventoryID: PXFieldState;
	SiteId: PXFieldState;
	Qty: PXFieldState;
	QtyComplete: PXFieldState;
	QtyScrapped: PXFieldState;
	QtyRemaining: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
})
export class AMProdMatlLotSerialAssigned extends PXView {
	Unallocate: PXActionState;

	InventoryID: PXFieldState;
	Descr: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	QtyIssued: PXFieldState;
	BaseUnit: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
})
export class AMProdMatlLotSerialUnassigned extends PXView {
	Allocate: PXActionState;

	InventoryID: PXFieldState;
	Descr: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
	QtyIssued: PXFieldState;
	BaseUnit: PXFieldState;
	QtyRequired: PXFieldState;
	QtyToAllocate: PXFieldState;
}


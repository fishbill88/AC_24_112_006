import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
	linkCommand,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.UnreleasedMaterialAllocations', primaryView: 'AMUnrelMaterialAllocationsFilterRecs' })
export class AM305500 extends PXScreen {
	AMUnrelMaterialAllocationsFilterRecs = createSingle(Filter);
	UnrelMaterialAllocationsDetailRecs = createCollection(Detail);
}

export class Filter extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductionOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	PONbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptType: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SubAssyOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	SubAssyProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class Detail extends PXView {
	Selected: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	TranDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	@linkCommand("ViewDetail") OperationID: PXFieldState;
	AMProdItem__StatusID: PXFieldState;
	POOrderNbr: PXFieldState;
	POReceiptNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) VendorID: PXFieldState;
	AMDocType: PXFieldState;
	AMBatNbr: PXFieldState;
	AMOrderType: PXFieldState;
	AMProdOrdID: PXFieldState;
	@columnConfig({ hideViewLink: true }) AMProdItem__BranchID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState;
}

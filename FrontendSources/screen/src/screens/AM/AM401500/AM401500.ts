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
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.AM.ProductionAttributesInq', primaryView: 'Filter' })
export class AM401500 extends PXScreen {
	Filter = createSingle(Filter);
	ProductionAttributes = createCollection(ProductionAttributes);
}

export class Filter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowTransactionAttributes: PXFieldState<PXFieldOptions.CommitChanges>;
	ShowOrderAttributes: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	adjustPageSize: true,
	syncPosition: true,
})
export class ProductionAttributes extends PXView {
	LineNbr: PXFieldState;
	TranLineNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	Level: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState;
	Source: PXFieldState;
	AttributeID: PXFieldState;
	Label: PXFieldState;
	Descr: PXFieldState;
	Enabled: PXFieldState;
	TransactionRequired: PXFieldState;
	Value: PXFieldState;
	DocType: PXFieldState;
	BatNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) TranOperationID: PXFieldState;
	Qty: PXFieldState;
	TranDate: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	InventoryItemDescr: PXFieldState;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState;
}

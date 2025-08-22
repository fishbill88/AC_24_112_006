import {
	SO301000
} from '../SO301000';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	columnConfig,
	createSingle,
	gridConfig
} from 'client-controls';

export interface SO301000_Manufacturing extends SO301000 {}
export class SO301000_Manufacturing {

	@viewInfo({containerName: "Production Orders"})
	AMSOLineRecords = createCollection(AMSOLineRecords);

	@viewInfo({containerName: "Production Details"})
	linkProdOrderSelectFilter = createSingle(linkProdOrderSelectFilter);

	@viewInfo({containerName: "Production Details"})
	AMSOLineLinkRecords = createCollection(AMSOLineLinkRecords);
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false
})
export class AMSOLineRecords extends PXView {
	@columnConfig({allowCheckAll: true}) AMSelected: PXFieldState<PXFieldOptions.CommitChanges>;
	LineNbr: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	AMQtyReadOnly: PXFieldState;
	AMUOMReadOnly: PXFieldState;
	AMOrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	AMProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMProdItem__StatusID: PXFieldState;
	AMProdItem__QtytoProd: PXFieldState;
	AMProdItem__QtyComplete: PXFieldState;
	AMProdItem__UOM: PXFieldState;
	AMConfigurationResults__Completed: PXFieldState;
}


export class linkProdOrderSelectFilter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false
})
export class AMSOLineLinkRecords extends PXView {
	@columnConfig({allowCheckAll: true}) Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderType: PXFieldState;
	ProdOrdID: PXFieldState;
	StatusID: PXFieldState;
	QtytoProd: PXFieldState;
	QtyComplete: PXFieldState;
	UOM: PXFieldState;
}

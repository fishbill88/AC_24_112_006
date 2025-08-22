import {
	RQ302000
} from '../RQ302000';

import {
	PXView,
	createCollection,
	PXFieldState,
	gridConfig,
	columnConfig,
	viewInfo,
	PXActionState,
	GridPreset
} from 'client-controls';

export interface RQ302000_PurchaseOrderLines extends RQ302000 { }
export class RQ302000_PurchaseOrderLines {
	@viewInfo({containerName: "Purchase Order Lines"})
	OrderLines = createCollection(POLine);
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class POLine extends PXView  {
	viewOrderByLine : PXActionState;

	@columnConfig({hideViewLink: true}) POOrder__OrderType : PXFieldState;
	POOrder__OrderDate : PXFieldState;
	POOrder__OrderNbr : PXFieldState;
	@columnConfig({hideViewLink: true}) POOrder__VendorID : PXFieldState;
	@columnConfig({hideViewLink: true}) POOrder__VendorLocationID : PXFieldState;
	@columnConfig({allowUpdate: false, allowNull: false}) POOrder__Status : PXFieldState;
	LineType : PXFieldState;
	@columnConfig({hideViewLink: true}) InventoryID : PXFieldState;
	@columnConfig({hideViewLink: true}) SubItemID : PXFieldState;
	@columnConfig({hideViewLink: true}) UOM : PXFieldState;
	@columnConfig({allowNull: false})	OrderQty : PXFieldState;
	@columnConfig({allowUpdate: false, allowNull: false})	ReceivedQty : PXFieldState;
}

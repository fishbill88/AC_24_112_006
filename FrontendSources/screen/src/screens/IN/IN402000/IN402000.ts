import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	PXPageLoadBehavior,
	PXFieldState,
	PXFieldOptions,
	PXView,
	gridConfig,
	columnConfig,
	handleEvent,
	CustomEventType,
	RowCssHandlerArgs,
	GridPreset
} from 'client-controls';

@graphInfo({
	graphType: 'PX.Objects.IN.InventoryAllocDetEnq',
	primaryView: 'Filter',
	pageLoadBehavior: PXPageLoadBehavior.PopulateSavedValues,
	hideFilesIndicator: true,
	hideNotesIndicator: true
})
export class IN402000 extends PXScreen {

	@viewInfo({ containerName: 'Selection' })
	Filter = createSingle(InventoryAllocDetEnqFilter);

	@viewInfo({ containerName: 'Allocation Details' })
	ResultRecords = createCollection(InventoryAllocDetEnqResult);

	@viewInfo({ containerName: 'Addition' })
	Addition = createCollection(InventoryQtyByPlanType);

	@viewInfo({ containerName: 'Deduction' })
	Deduction = createCollection(InventoryQtyByPlanType2);

	@handleEvent(CustomEventType.GetRowCss, { view: 'Addition' })
	getAdditionRowCss(args: RowCssHandlerArgs) {
		if (args?.selector?.row?.IsTotal.value === true) {
			return 'bold-row';
		}
		return undefined;
	}

	@handleEvent(CustomEventType.GetRowCss, { view: 'Deduction' })
	getDeductionRowCss(args: RowCssHandlerArgs) {
		if (args?.selector?.row?.IsTotal.value === true) {
			return 'bold-row';
		}
		return undefined;
	}
}

export class InventoryAllocDetEnqFilter extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemCD: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseUnit: PXFieldState<PXFieldOptions.Disabled>;
	QtyOnHand: PXFieldState<PXFieldOptions.Disabled>;
	QtyTotalAddition: PXFieldState<PXFieldOptions.Disabled>;
	QtyTotalDeduction: PXFieldState<PXFieldOptions.Disabled>;
	QtyAvail: PXFieldState<PXFieldOptions.Disabled>;
	QtyHardAvail: PXFieldState<PXFieldOptions.Disabled>;
	QtyActual: PXFieldState<PXFieldOptions.Disabled>;
	QtyNotAvail: PXFieldState<PXFieldOptions.Disabled>;
	QtyExpired: PXFieldState<PXFieldOptions.Disabled>;
	QtyPOPrepared: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyPOPrepared: PXFieldState<PXFieldOptions.Disabled>;
	QtyPOOrders: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyPOOrders: PXFieldState<PXFieldOptions.Disabled>;
	QtyPOReceipts: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyPOReceipts: PXFieldState<PXFieldOptions.Disabled>;
	QtyINReceipts: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyINReceipts: PXFieldState<PXFieldOptions.Disabled>;
	QtyInTransit: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyInTransit: PXFieldState<PXFieldOptions.Disabled>;
	QtyInTransitToSO: PXFieldState<PXFieldOptions.Disabled>;
	QtyINAssemblySupply: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyINAssemblySupply: PXFieldState<PXFieldOptions.Disabled>;
	QtyPOFixedPrepared: PXFieldState<PXFieldOptions.Disabled>;
	QtyPOFixedOrders: PXFieldState<PXFieldOptions.Disabled>;
	QtyPOFixedReceipts: PXFieldState<PXFieldOptions.Disabled>;
	QtyPODropShipPrepared: PXFieldState<PXFieldOptions.Disabled>;
	QtyPODropShipOrders: PXFieldState<PXFieldOptions.Disabled>;
	QtyPODropShipReceipts: PXFieldState<PXFieldOptions.Disabled>;
	QtyPOFixedFSSrvOrdPrepared: PXFieldState<PXFieldOptions.Disabled>;
	QtyPOFixedFSSrvOrd: PXFieldState<PXFieldOptions.Disabled>;
	QtyPOFixedFSSrvOrdReceipts: PXFieldState<PXFieldOptions.Disabled>;
	QtySOPrepared: PXFieldState<PXFieldOptions.Disabled>;
	InclQtySOPrepared: PXFieldState<PXFieldOptions.Disabled>;
	QtySOBooked: PXFieldState<PXFieldOptions.Disabled>;
	InclQtySOBooked: PXFieldState<PXFieldOptions.Disabled>;
	QtySOShipping: PXFieldState<PXFieldOptions.Disabled>;
	InclQtySOShipping: PXFieldState<PXFieldOptions.Disabled>;
	QtySOShipped: PXFieldState<PXFieldOptions.Disabled>;
	InclQtySOShipped: PXFieldState<PXFieldOptions.Disabled>;
	QtySOBackOrdered: PXFieldState<PXFieldOptions.Disabled>;
	InclQtySOBackOrdered: PXFieldState<PXFieldOptions.Disabled>;
	QtyINIssues: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyINIssues: PXFieldState<PXFieldOptions.Disabled>;
	QtyINAssemblyDemand: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyINAssemblyDemand: PXFieldState<PXFieldOptions.Disabled>;
	QtyFSSrvOrdPrepared: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyFSSrvOrdPrepared: PXFieldState<PXFieldOptions.Disabled>;
	QtyFSSrvOrdBooked: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyFSSrvOrdBooked: PXFieldState<PXFieldOptions.Disabled>;
	QtyFSSrvOrdAllocated: PXFieldState<PXFieldOptions.Disabled>;
	InclQtyFSSrvOrdAllocated: PXFieldState<PXFieldOptions.Disabled>;
	QtySOFixed: PXFieldState<PXFieldOptions.Disabled>;
	QtySODropShip: PXFieldState<PXFieldOptions.Disabled>;
	QtyFixedFSSrvOrd: PXFieldState<PXFieldOptions.Disabled>;
	Label: PXFieldState<PXFieldOptions.Disabled>;
	Label2: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	allowUpdate: false
})
export class InventoryAllocDetEnqResult extends PXView {
	ViewDocument: PXActionState;

	GridLineNbr: PXFieldState;
	Module: PXFieldState;
	AllocationType: PXFieldState;
	PlanDate: PXFieldState;
	QADocType: PXFieldState;
	RefNbr: PXFieldState;
	@columnConfig({hideViewLink: true})
	SubItemID: PXFieldState;
	@columnConfig({hideViewLink: true})
	SiteID: PXFieldState;
	@columnConfig({hideViewLink: true})
	LocationID: PXFieldState;
	@columnConfig({hideViewLink: true})
	LotSerialNbr: PXFieldState;
	PlanQty: PXFieldState;
	CostLayerType: PXFieldState;
	@columnConfig({hideViewLink: true})
	BAccountID: PXFieldState;
	AcctName: PXFieldState;
	LocNotAvailable: PXFieldState;
	Expired: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true
})
export class InventoryQtyByPlanType extends PXView {
	PlanType: PXFieldState;
	Qty: PXFieldState;
	Included: PXFieldState;
	IsTotal: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true
})
export class InventoryQtyByPlanType2 extends PXView {
	PlanType: PXFieldState;
	Qty: PXFieldState;
	Included: PXFieldState;
	IsTotal: PXFieldState<PXFieldOptions.Hidden>;
}
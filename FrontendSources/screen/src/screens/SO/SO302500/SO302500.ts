import {
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,

	createSingle,
	createCollection,

	graphInfo,
	viewInfo,
	columnConfig,
	gridConfig,
	headerDescription,

	GridPagerMode,
	GridPreset
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.SO.SOPickingWorksheetReview', primaryView: 'worksheet', showUDFIndicator: true })
export class SO302500 extends PXScreen {
	@viewInfo({ containerName: "Picking Worksheet Summary" })
	worksheet = createSingle(SOPickingWorksheetHeader);

	@viewInfo({ containerName: "Details" })
	worksheetLines = createCollection(Lines);

	@viewInfo({ containerName: "Line Details" })
	worksheetLineSplits = createCollection(LineSplits);

	@viewInfo({ containerName: "Shipments" })
	shipmentLinks = createCollection(ShipmentLinks);

	@viewInfo({ containerName: "Shipment Pickers" })
	shipmentPickers = createCollection(ShipmentPickers);

	@viewInfo({ containerName: "Pick List Entries by Shipment" })
	pickerListByShipment = createCollection(PickListEntriesByShipment);

	@viewInfo({ containerName: "Pickers" })
	pickers = createCollection(Pickers);

	@viewInfo({ containerName: "Picker Shipments" })
	pickerShipments = createCollection(PickerShipments);

	@viewInfo({ containerName: "Pick List Entries" })
	pickerList = createCollection(PickListEntries);
}

export class SOPickingWorksheetHeader extends PXView {
	WorksheetNbr: PXFieldState;
	WorksheetType: PXFieldState;
	Status: PXFieldState;
	PickDate: PXFieldState;

	@headerDescription
	SiteID: PXFieldState;

	PickStartDate: PXFieldState;
	PickCompleteDate: PXFieldState;

	Qty: PXFieldState;
	ShipmentWeight: PXFieldState;
	ShipmentVolume: PXFieldState;
}

// ---------------- Details Tab ---------------------------
@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Lines extends PXView {
	ShowSplits: PXActionState;

	WorksheetNbr: PXFieldState;
	LineNbr: PXFieldState;
	SiteID: PXFieldState;

	@columnConfig({
		hideViewLink: true,
		nullText: "<SPLIT>"
	})
	LocationID: PXFieldState;

	InventoryID: PXFieldState;

	@columnConfig({
		hideViewLink: true,
		nullText: "<SPLIT>"
	})
	SubItemID: PXFieldState;

	@columnConfig({
		hideViewLink: true,
		nullText: "<SPLIT>"
	})
	LotSerialNbr: PXFieldState;

	ExpireDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;

	Qty: PXFieldState;
	OrigOrderQty: PXFieldState;
	OpenOrderQty: PXFieldState;
	PickedQty: PXFieldState;
	TranDesc: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false
})
export class LineSplits extends PXView {
	InventoryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState;

	PickedQty: PXFieldState;
	Qty: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;

	ExpireDate: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SortingLocationID: PXFieldState;
}

// ---------------- Shipments Tab -------------------------
@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
})
export class ShipmentLinks extends PXView {
	ShowPickers: PXActionState;

	Picked: PXFieldState;
	ShipmentNbr: PXFieldState;
	PickedQty: PXFieldState;
	ShipmentQty: PXFieldState;
	ShipmentWeight: PXFieldState;
	ShipmentVolume: PXFieldState;
	Status: PXFieldState;
	Unlinked: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false,
})
export class ShipmentPickers extends PXView {
	Confirmed: PXFieldState;
	PickerNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UserID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CartID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SortingLocationID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	autoRepaint: ['pickerListByShipment'],
	pagerMode: GridPagerMode.InfiniteScroll,
})
export class PickerShipments extends PXView {
	ShipmentNbr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false
})
export class PickListEntriesByShipment extends PXView {
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState;

	InventoryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState;

	Qty: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;

	ExpireDate: PXFieldState;
	PickedQty: PXFieldState;

	@columnConfig({ hideViewLink: true })
	ToteID: PXFieldState;
}

// ---------------- Pickers Tab ---------------------------
@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class Pickers extends PXView {
	ShowShipments: PXActionState;
	ShowPickList: PXActionState;

	Confirmed: PXFieldState;
	PickerNbr: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UserID: PXFieldState;

	PathLength: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CartID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SortingLocationID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false
})
export class PickListEntries extends PXView {
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState;

	InventoryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState;

	Qty: PXFieldState;

	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;

	ExpireDate: PXFieldState;
	PickedQty: PXFieldState;
}

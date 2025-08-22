import {
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,

	createSingle,
	createCollection,

	graphInfo,
	viewInfo,
	columnConfig,
	gridConfig,
	linkCommand,
	GridPreset
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.SO.SOPickingWorksheetProcess', primaryView: 'Filter' })
export class SO503050 extends PXScreen {
	SelectItems: PXActionState;
	SelectLocations: PXActionState;

	@viewInfo({ containerName: "Header" })
	Filter = createSingle(FilterHeader);

	@viewInfo({ containerName: "Shipments" })
	Shipments = createCollection(Shipments);
}

export class FilterHeader extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;

	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxNumberOfLinesInShipment: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxQtyInLines: PXFieldState<PXFieldOptions.CommitChanges>;

	NumberOfPickers: PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfTotesPerPicker: PXFieldState<PXFieldOptions.CommitChanges>;
	SendToQueue: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintPickLists: PXFieldState<PXFieldOptions.CommitChanges>;
	AutomaticShipmentConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;

	PrintWithDeviceHub: PXFieldState<PXFieldOptions.CommitChanges>;
	DefinePrinterManually: PXFieldState<PXFieldOptions.CommitChanges>;
	PrinterID: PXFieldState<PXFieldOptions.CommitChanges>;

	// kept for backward compatibility of schedules
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CarrierPluginID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipVia: PXFieldState<PXFieldOptions.CommitChanges>;
	PackagingType: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	suppressNoteFiles: true,
	quickFilterFields: ["ShipmentNbr", "CustomerID", "ShipVia", "CustomerOrderNbr"]
})
export class Shipments extends PXView {
	@columnConfig({ allowSort: false, allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	@linkCommand("viewDocument")
	@columnConfig({ allowFastFilter: true })
	ShipmentNbr: PXFieldState<PXFieldOptions.Disabled>;

	CurrentWorksheetNbr: PXFieldState<PXFieldOptions.Disabled>;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	ShipDate: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	CustomerID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	CustomerID_BAccountR_acctName: PXFieldState<PXFieldOptions.Disabled>;

	CustomerLocationID: PXFieldState<PXFieldOptions.Disabled>;
	CustomerLocationID_Location_descr: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	CustomerOrderNbr: PXFieldState<PXFieldOptions.Disabled>;

	BillSeparately: PXFieldState<PXFieldOptions.Disabled>;

	SiteID: PXFieldState<PXFieldOptions.Disabled>;
	SiteID_INSite_descr: PXFieldState<PXFieldOptions.Disabled>;

	WorkgroupID: PXFieldState<PXFieldOptions.Disabled>;
	OwnerID: PXFieldState<PXFieldOptions.Disabled>;

	ShipmentQty: PXFieldState<PXFieldOptions.Disabled>;

	ShipVia: PXFieldState<PXFieldOptions.Disabled>;
	ShipVia_Carrier_description: PXFieldState<PXFieldOptions.Disabled>;

	ShipmentWeight: PXFieldState<PXFieldOptions.Disabled>;
	ShipmentVolume: PXFieldState<PXFieldOptions.Disabled>;

	LabelsPrinted: PXFieldState<PXFieldOptions.Disabled>;
	Picked: PXFieldState<PXFieldOptions.Disabled>;
	PickListPrinted: PXFieldState<PXFieldOptions.Disabled>;
}
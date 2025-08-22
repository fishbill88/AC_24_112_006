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
	GridPreset,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.SO.SOPickingJobProcess', primaryView: 'Filter' })
export class SO503075 extends PXScreen {
	SelectItems: PXActionState;
	SelectLocations: PXActionState;

	@viewInfo({ containerName: "Header" })
	Filter = createSingle(FilterHeader);

	@viewInfo({ containerName: "Picking Jobs" })
	PickingJobs = createCollection(PickingJobs);
}

export class FilterHeader extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorksheetType: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;

	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxNumberOfLinesInPickList: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxQtyInLines: PXFieldState<PXFieldOptions.CommitChanges>;

	NewPriority: PXFieldState<PXFieldOptions.CommitChanges>;
	AssigneeID: PXFieldState<PXFieldOptions.CommitChanges>;

	// kept for backward compatibility of schedules
	Priority: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Hidden>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Hidden>;
	CarrierPluginID: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Hidden>;
	ShipVia: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	suppressNoteFiles: true,
	quickFilterFields: [
		"SOPickingJob__Priority",
		"Carrier__CarrierID",
		"Carrier__Description"
	]
})
export class PickingJobs extends PXView {
	@columnConfig({ allowSort: false, allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;

	SOPickingWorksheet__WorksheetType: PXFieldState<PXFieldOptions.Disabled>;

	@linkCommand("ShowPickList")
	@columnConfig({ allowFastFilter: true })
	SOPickingJob__PickListNbr: PXFieldState<PXFieldOptions.Disabled>;

	SOPickingJob__Status: PXFieldState<PXFieldOptions.Disabled>;

	SOPickingWorksheet__PickDate: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	SOPickingJob__Priority: PXFieldState<PXFieldOptions.Disabled>;

	SOPickingJob__PreferredAssigneeID: PXFieldState<PXFieldOptions.Disabled>;
	SOPickingJob__ActualAssigneeID: PXFieldState<PXFieldOptions.Disabled>;

	SOPicker__PathLength: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Customer__AcctCD: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Customer__Descr: PXFieldState<PXFieldOptions.Disabled>;

	Location__LocationCD: PXFieldState<PXFieldOptions.Disabled>;
	Location__Descr: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Carrier__CarrierID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Carrier__Description: PXFieldState<PXFieldOptions.Disabled>;

	SOShipment__ShipmentQty: PXFieldState<PXFieldOptions.Disabled>;
	SOShipment__ShipmentWeight: PXFieldState<PXFieldOptions.Disabled>;
	SOShipment__ShipmentVolume: PXFieldState<PXFieldOptions.Disabled>;
}
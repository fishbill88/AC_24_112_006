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

	// handleEvent,
	// CustomEventType,
	// CellCssHandlerArgs
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.SO.SOPickingJobEnq', primaryView: 'Filter' })
export class SO503080 extends PXScreen {
	@viewInfo({ containerName: "Header" })
	Filter = createSingle(FilterHeader);

	@viewInfo({ containerName: "Picking Jobs" })
	PickingJobs = createCollection(PickingJobs);

	@viewInfo({ containerName: "Packing Jobs" })
	PackingJobs = createCollection(PackingJobs);

	// priorityToStyle(priority: number): string | undefined {
	// 	if (priority === 4) {
	// 		return 'qp-connotation-Danger';
	// 	}
	// 	if (priority === 3) {
	// 		return 'qp-connotation-Warning';
	// 	}
	// 	if (priority === 2) {
	// 		return 'qp-connotation-Primary';
	// 	}
	// 	if (priority === 1) {
	// 		return 'qp-connotation-Secondary';
	// 	}

	// 	return undefined;
	// }

	// @handleEvent(CustomEventType.GetCellCss, { view: 'PickingJobs', column: 'Priority' })
	// getPickingJobsPriorityCellCss(args: CellCssHandlerArgs<PickingJobs>): string | undefined {
	// 	return (<SO503080>args.screenModel).priorityToStyle(args?.selector?.row?.Priority.value);
	// }

	// @handleEvent(CustomEventType.GetCellCss, { view: 'PackingJobs', column: 'Priority' })
	// getPackingJobsPriorityCellCss(args: CellCssHandlerArgs<PackingJobs>): string | undefined {
	// 	return (<SO503080>args.screenModel).priorityToStyle(args?.selector?.row?.Priority.value);
	// }
}

export class FilterHeader extends PXView {
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	AssigneeID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorksheetType: PXFieldState<PXFieldOptions.CommitChanges>;

	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CarrierPluginID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipVia: PXFieldState<PXFieldOptions.CommitChanges>;

	ShowPick: PXFieldState<PXFieldOptions.Hidden>;
	ShowPack: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false,
	suppressNoteFiles: true
})
export class PickingJobs extends PXView {
	HoldJob: PXActionState;

	SOPickingWorksheet__WorksheetType: PXFieldState<PXFieldOptions.Disabled>;

	@linkCommand("ShowPickList")
	@columnConfig({ allowFastFilter: true })
	SOPickingJob__PickListNbr: PXFieldState<PXFieldOptions.Disabled>;

	SOPickingJob__Status: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Priority: PXFieldState<PXFieldOptions.CommitChanges>;

	PreferredAssigneeID: PXFieldState<PXFieldOptions.CommitChanges>;
	SOPickingJob__ActualAssigneeID: PXFieldState<PXFieldOptions.Disabled>;
	SOPickingWorksheet__PickDate: PXFieldState<PXFieldOptions.Disabled>;
	SOPickingJob__TimeInQueue: PXFieldState<PXFieldOptions.Disabled>;
	SOPicker__PathLength: PXFieldState<PXFieldOptions.Disabled>;
	SOPickingJob__PickingStartedAt: PXFieldState<PXFieldOptions.Disabled>;
	AutomaticShipmentConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;

	SOShipment__ShipmentQty: PXFieldState<PXFieldOptions.Disabled>;
	SOShipment__ShipmentWeight: PXFieldState<PXFieldOptions.Disabled>;
	SOShipment__ShipmentVolume: PXFieldState<PXFieldOptions.Disabled>;
	SOShipment__ShipDate: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Customer__AcctCD: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Customer__AcctName: PXFieldState<PXFieldOptions.Disabled>;

	Location__LocationCD: PXFieldState<PXFieldOptions.Disabled>;
	Location__Descr: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Carrier__CarrierID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Carrier__Description: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false,
	suppressNoteFiles: true
})
export class PackingJobs extends PXView {
	SOPickingWorksheet__WorksheetType: PXFieldState<PXFieldOptions.Disabled>;

	@linkCommand("ShowPickList")
	@columnConfig({ allowFastFilter: true })
	SOPickingJob__PickListNbr: PXFieldState<PXFieldOptions.Disabled>;

	SOPickingJob__Status: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Priority: PXFieldState<PXFieldOptions.CommitChanges>;

	PreferredAssigneeID: PXFieldState<PXFieldOptions.CommitChanges>;
	SOPickingJob__ActualAssigneeID: PXFieldState<PXFieldOptions.Disabled>;
	SOPickingWorksheet__PickDate: PXFieldState<PXFieldOptions.Disabled>;
	SOPickingJob__TimeInQueue: PXFieldState<PXFieldOptions.Disabled>;
	SOPicker__PathLength: PXFieldState<PXFieldOptions.Disabled>;
	SOPickingJob__PickingStartedAt: PXFieldState<PXFieldOptions.Disabled>;
	SOPickingJob__PickedAt: PXFieldState<PXFieldOptions.Disabled>;
	AutomaticShipmentConfirmation: PXFieldState<PXFieldOptions.CommitChanges>;

	SOShipment__ShipmentQty: PXFieldState<PXFieldOptions.Disabled>;
	SOShipment__ShipmentWeight: PXFieldState<PXFieldOptions.Disabled>;
	SOShipment__ShipmentVolume: PXFieldState<PXFieldOptions.Disabled>;
	SOShipment__ShipDate: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Customer__AcctCD: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Customer__AcctName: PXFieldState<PXFieldOptions.Disabled>;

	Location__LocationCD: PXFieldState<PXFieldOptions.Disabled>;
	Location__Descr: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Carrier__CarrierID: PXFieldState<PXFieldOptions.Disabled>;

	@columnConfig({ allowFastFilter: true })
	Carrier__Description: PXFieldState<PXFieldOptions.Disabled>;
}

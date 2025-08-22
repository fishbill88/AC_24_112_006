import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	linkCommand,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RouteWrkSheetInq', primaryView: 'Filter' })
export class FS403900 extends PXScreen {
	OpenRouteDocument: PXActionState;
	Filter = createSingle(RouteWrkSheetFilter);
	Routes = createCollection(FSRouteDocument);
	DriverRouteSelected = createSingle(FSRouteDocument);
	DriverFilter = createSingle(DriverSelectionFilter);
	DriverRecords = createCollection(EPEmployee);
	VehicleRouteSelected = createSingle(FSRouteDocument);
	VehicleFilter = createSingle(VehicleSelectionFilter);
	VehicleRecords = createCollection(FSVehicle);
}

export class RouteWrkSheetFilter extends PXView {
	FromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
	DriverID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	syncPosition: true,
	mergeToolbarWith: "ScreenToolbar"
})
export class FSRouteDocument extends PXView {
	@linkCommand("OpenRouteDocument") RefNbr: PXFieldState;
	RouteID: PXFieldState;
	FSRoute__RouteShort: PXFieldState;
	TripNbr: PXFieldState;
	Status: PXFieldState;
	Date: PXFieldState;
	TimeBegin_Time: PXFieldState;
	TimeEnd_Time: PXFieldState;
	DriverID: PXFieldState<PXFieldOptions.Readonly>;
	AdditionalDriverID: PXFieldState;
	VehicleID: PXFieldState<PXFieldOptions.Readonly>;
	AdditionalVehicleID1: PXFieldState;
	AdditionalVehicleID2: PXFieldState;
	TotalNumAppointments: PXFieldState;
	TotalServices: PXFieldState;
	TotalDuration: PXFieldState;
	TotalServicesDuration: PXFieldState;
	TotalTravelTime: PXFieldState;
}

export class DriverSelectionFilter extends PXView {
	ShowUnassignedDrivers: PXFieldState;
}

@gridConfig({
	syncPosition: true
})
export class EPEmployee extends PXView {
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	Mem_UnassignedDriver: PXFieldState;
	FSRouteEmployee__PriorityPreference: PXFieldState;
}

export class VehicleSelectionFilter extends PXView {
	ShowUnassignedVehicles: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true
})
export class FSVehicle extends PXView {
	RefNbr: PXFieldState;
	Descr: PXFieldState;
	Mem_UnassignedVehicle: PXFieldState;
}

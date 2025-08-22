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
	PXFieldOptions,
} from "client-controls";
import { create } from "domain";

@graphInfo({ graphType: 'PX.Objects.FS.RouteDocumentMaint', primaryView: 'RouteRecords' })
export class FS304000 extends PXScreen {
	OpenDriverSelector: PXActionState;
	OpenVehicleSelector: PXActionState;
	SelectCurrentRoute: PXActionState;
	OpenRouteSchedule: PXActionState;
	OpenAppointment: PXActionState;
	OpenCustomerLocation: PXActionState;
	ViewStartGPSOnMap: PXActionState;
	ViewCompleteGPSOnMap: PXActionState;
	RouteRecords = createSingle(FSRouteDocument);
	DriverRouteSelected = createSingle(FSRouteDocument);
	DriverFilter = createSingle(DriverSelectionFilter);
	DriverRecords = createCollection(EPEmployee);
	VehicleRouteSelected = createSingle(FSRouteDocument);
	VehicleFilter = createSingle(VehicleSelectionFilter);
	VehicleRecords = createCollection(FSVehicle);
	AppointmentsInRoute = createCollection(FSAppointmentInRoute);
	RouteSelected = createSingle(FSRouteDocument);
	Answers = createCollection(CSAnswers);
	RouteAppAssignmentFilter = createSingle(RouteAppointmentAssignmentFilter);
	RouteAppAssignmentRecords = createCollection(FSRoute);
	ServiceOrderTypeSelector = createSingle(SrvOrderTypeRouteAux);
}

export class FSRouteDocument extends PXView {
	RefNbr: PXFieldState;
	BranchID: PXFieldState;
	RouteID: PXFieldState<PXFieldOptions.CommitChanges>;
	TripNbr: PXFieldState;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeBegin_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	DriverID: PXFieldState<PXFieldOptions.CommitChanges>
	AdditionalDriverID: PXFieldState;
	VehicleID: PXFieldState<PXFieldOptions.CommitChanges>;
	AdditionalVehicleID1: PXFieldState<PXFieldOptions.CommitChanges>;
	AdditionalVehicleID2: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalNumAppointments: PXFieldState;
	TotalServices: PXFieldState;
	TotalDistanceFriendly: PXFieldState;
	TotalServicesDuration: PXFieldState;
	TotalDuration: PXFieldState;
	TotalTravelTime: PXFieldState;
	ApproximateValuesLabel: PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Readonly>;
	ActualStartTime_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ActualStartTime_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	ActualEndTime_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ActualEndTime_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	MemActualDuration: PXFieldState;
	Miles: PXFieldState;
	Weight: PXFieldState;
	FuelQty: PXFieldState;
	FuelType: PXFieldState<PXFieldOptions.CommitChanges>;
	Oil: PXFieldState;
	AntiFreeze: PXFieldState;
	DEF: PXFieldState;
	Propane: PXFieldState;
	GPSLatitudeStart: PXFieldState;
	GPSLongitudeStart: PXFieldState;
	GPSLatitudeComplete: PXFieldState;
	GPSLongitudeComplete: PXFieldState;
	GPSLatitudeLongitude: PXFieldState;
	FSRoute__RouteCD: PXFieldState;
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
	Mem_UnassignedDriver: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class VehicleSelectionFilter extends PXView {
	ShowUnassignedVehicles: PXFieldState;
}

@gridConfig({
	syncPosition: true
})
export class FSVehicle extends PXView {
	RefNbr: PXFieldState;
	Descr: PXFieldState;
	Mem_UnassignedVehicle: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowInsert: false,
	allowDelete: false
})
export class FSAppointmentInRoute extends PXView {
	AddAppointment: PXActionState;
	UnassignAppointment: PXActionState;
	Up: PXActionState;
	Down: PXActionState;
	ReassignAppointment: PXActionState;
	SrvOrdType: PXFieldState;
	ServiceContractID: PXFieldState;
	CustomerContractNbr: PXFieldState;
	@linkCommand("OpenRouteSchedule") ScheduleID: PXFieldState;
	@linkCommand("OpenAppointment") RefNbr: PXFieldState;
	DocDesc: PXFieldState;
	CustomerID: PXFieldState;
	@linkCommand("OpenCustomerLocation") LocationID: PXFieldState;
	Status: PXFieldState;
	ScheduledDateTimeBegin_Date: PXFieldState;
	ScheduledDateTimeBegin_Time: PXFieldState;
	ScheduledDateTimeEnd_Time: PXFieldState;
	EstimatedDurationTotal: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
}

export class CSAnswers extends PXView {
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}

export class RouteAppointmentAssignmentFilter extends PXView {
	AppRefNbr: PXFieldState;
	AppSrvOrdType: PXFieldState;
	CustomerID: PXFieldState;
	LocationID: PXFieldState;
	ScheduledDateTimeBegin: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	State: PXFieldState;
	EstimatedDurationTotal: PXFieldState;
	RouteDate_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	RouteID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSRoute extends PXView {
	RefNbr: PXFieldState;
	RouteID: PXFieldState;
	FSRoute__RouteShort: PXFieldState;
	Date: PXFieldState;
	DriverID: PXFieldState;
	VehicleID: PXFieldState;
	TotalDuration: PXFieldState;
	TotalNumAppointments: PXFieldState;
	TotalServices: PXFieldState;
}

export class SrvOrderTypeRouteAux extends PXView {
	SrvOrdType: PXFieldState;
}

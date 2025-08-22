import {
	graphInfo,
	gridConfig,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	createSingle,
	PXActionState
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RouteClosingMaint', primaryView: 'RouteRecords' })
export class FS304010 extends PXScreen {
	OpenAppointment: PXActionState;
	OpenCustomerLocation: PXActionState;
	OpenRouteSchedule: PXActionState;
	OpenRouteContract: PXActionState;
	RouteRecords = createSingle(FSRouteDocument);
	AppointmentsInRoute = createCollection(FSAppointment);
	RouteDocumentSelected = createSingle(FSRouteDocument);
}

export class FSAppointment extends PXView {
	SrvOrdType: PXFieldState;
	ServiceContractID: PXFieldState;
	FSServiceContract__CustomerContractNbr: PXFieldState;
	ScheduleID: PXFieldState;
	RefNbr: PXFieldState;
	DocDesc: PXFieldState;
	FSServiceOrder__CustomerID: PXFieldState;
	FSServiceOrder__LocationID: PXFieldState;
	Status: PXFieldState;
	ScheduledDateTimeBegin_Date: PXFieldState;
	ScheduledDateTimeBegin_Time: PXFieldState;
	ScheduledDateTimeEnd_Time: PXFieldState;
	FSAddress__AddressLine1: PXFieldState;
	FSAddress__AddressLine2: PXFieldState;
	FSServiceOrder__PostalCode: PXFieldState;
	FSAddress__City: PXFieldState;
	FSAddress__State: PXFieldState;
	FSAddress__PostalCode: PXFieldState;
}

export class FSRouteDocument extends PXView {
	RefNbr: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.Readonly>;
	Date: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Readonly>;
	TimeBegin_Time: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Readonly>;
	TimeEnd_Time: PXFieldState;
	RouteID: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Readonly>;
	Status: PXFieldState<PXFieldOptions.Readonly>;
	TripNbr: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Readonly>;
	DriverID: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Readonly>;
	VehicleID: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Readonly>;
	ActualStartTime_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	ActualEndTime_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	MemActualDuration: PXFieldState<PXFieldOptions.Readonly>;
	Miles: PXFieldState<PXFieldOptions.Readonly>;
	Weight: PXFieldState<PXFieldOptions.Readonly>;
	FuelQty: PXFieldState<PXFieldOptions.Readonly>;
	FuelType: PXFieldState<PXFieldOptions.Readonly>;
	Oil: PXFieldState<PXFieldOptions.Readonly>;
	AntiFreeze: PXFieldState<PXFieldOptions.Readonly>;
	DEF: PXFieldState<PXFieldOptions.Readonly>;
	Propane: PXFieldState<PXFieldOptions.Readonly>;
}

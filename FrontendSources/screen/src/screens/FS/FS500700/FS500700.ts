import {
	graphInfo,
	gridConfig,
	columnConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.CompleteRouteProcess', primaryView: 'Filter' })
export class FS500700 extends PXScreen {
	Filter = createSingle(RouteFilter);
	RouteDocs = createCollection(FSRouteDocument);
}

export class RouteFilter extends PXView {
	Date: PXFieldState;
	ShowCompletedRoutes: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class FSRouteDocument extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	RefNbr: PXFieldState;
	RouteID: PXFieldState;
	FSRoute__RouteShort: PXFieldState;
	Status: PXFieldState;
	Date: PXFieldState;
	TimeBegin_Time: PXFieldState;
	TimeEnd_Time: PXFieldState;
	DriverID: PXFieldState<PXFieldOptions.CommitChanges>;
	AdditionalDriverID: PXFieldState<PXFieldOptions.CommitChanges>;
	VehicleID: PXFieldState<PXFieldOptions.CommitChanges>;
	AdditionalVehicleID1: PXFieldState<PXFieldOptions.CommitChanges>;
	AdditionalVehicleID2: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalNumAppointments: PXFieldState;
	TotalServices: PXFieldState;
	TotalDuration: PXFieldState;
	TotalServicesDuration: PXFieldState;
	TotalTravelTime: PXFieldState;
}

import {
	graphInfo,
	createSingle,
	gridConfig,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RoutePendingInq', primaryView: 'Filter' })
export class FS404000 extends PXScreen {
	OpenRouteClosing: PXActionState;
	Filter = createSingle(RouteWrkSheetFilter);
	Routes = createCollection(RouteWrkSheetFilter);
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class RouteWrkSheetFilter extends PXView {
	RefNbr: PXFieldState;
	RouteID: PXFieldState;
	FSRoute__RouteShort: PXFieldState;
	TripNbr: PXFieldState;
	Status: PXFieldState;
	Date: PXFieldState;
	TimeBegin_Time: PXFieldState;
	TimeEnd_Time: PXFieldState;
	DriverID: PXFieldState<PXFieldOptions.CommitChanges>;
	AdditionalDriverID: PXFieldState<PXFieldOptions.CommitChanges>;
	VehicleID: PXFieldState<PXFieldOptions.CommitChanges>;
	TotalNumAppointments: PXFieldState;
	TotalServices: PXFieldState;
	TotalDuration: PXFieldState;
	TotalServicesDuration: PXFieldState;
	TotalTravelTime: PXFieldState;
}

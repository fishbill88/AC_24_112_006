import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	linkCommand,
	PXScreen,
	PXView,
	PXActionState,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RouteDocumentInq', primaryView: 'Filter' })
export class FS404100 extends PXScreen {
	OpenRouteDocument: PXActionState;
	Filter = createSingle(RouteDocumentFilter);
	RouteDocuments = createCollection(FSRouteDocument);
}

export class RouteDocumentFilter extends PXView {
	RouteID: PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusOpen: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusInProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusCanceled: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusCompleted: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusClosed: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	allowStoredFilters: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class FSRouteDocument extends PXView {
	@linkCommand("OpenRouteDocument") RefNbr: PXFieldState;
	RouteID: PXFieldState;
	TripNbr: PXFieldState;
	Status: PXFieldState;
	Date: PXFieldState;
	TimeBegin_Time: PXFieldState;
	DriverID: PXFieldState;
	AdditionalDriverID: PXFieldState;
	VehicleID: PXFieldState;
	AdditionalVehicleID1: PXFieldState;
	AdditionalVehicleID2: PXFieldState;
	TotalNumAppointments: PXFieldState;
	TotalServices: PXFieldState;
	TotalTravelTime: PXFieldState;
	TotalServicesDuration: PXFieldState;
	TotalDuration: PXFieldState;
}

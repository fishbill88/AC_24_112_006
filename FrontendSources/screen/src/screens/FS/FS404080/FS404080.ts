import {
	graphInfo,
	gridConfig,
	linkCommand,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RouteAppointmentGPSLocationInq', primaryView: 'RouteAppointmentGPSLocationFilter' })
export class FS404080 extends PXScreen {
	EditAppointment: PXActionState;
	OpenLocationScreen: PXActionState;
	RouteAppointmentGPSLocationFilter = createSingle(RouteAppointmentGPSLocationFilter);
	RouteAppointmentGPSLocationRecords = createCollection(AppointmentData);
}

export class RouteAppointmentGPSLocationFilter extends PXView {
	RouteID: PXFieldState<PXFieldOptions.CommitChanges>;
	RouteDocumentID: PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	DateFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTo: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	allowStoredFilters: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class AppointmentData extends PXView {
	RouteID: PXFieldState;
	RouteDocumentID: PXFieldState;
	@linkCommand("EditAppointment") RefNbr: PXFieldState;
	CustomerID: PXFieldState;
	@linkCommand("OpenLocationScreen") LocationID: PXFieldState;
	ActualDateTimeBegin: PXFieldState;
	Address: PXFieldState;
	GPSStartCoordinate: PXFieldState;
	GPSStartAddress: PXFieldState;
	GPSCompleteCoordinate: PXFieldState;
	GPSCompleteAddress: PXFieldState;
}

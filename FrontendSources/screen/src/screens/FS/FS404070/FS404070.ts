import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RouteAppointmentForecastingInq', primaryView: 'RouteAppointmentForecastingFilter' })
export class FS404070 extends PXScreen {
	RouteAppointmentForecastingFilter = createSingle(RouteAppointmentForecastingFilter);
	RouteAppointmentForecastingRecords = createCollection(FSRouteAppointmentForecasting);
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	mergeToolbarWith: "ScreenToolbar"
})
export class FSRouteAppointmentForecasting extends PXView {
	StartDate: PXFieldState;
	RouteID: PXFieldState;
	FSSchedule__RefNbr: PXFieldState;
	ServiceContractID: PXFieldState;
	FSServiceContract__CustomerContractNbr: PXFieldState;
	CustomerID: PXFieldState;
	@columnConfig({ hideViewLink: true }) CustomerLocationID: PXFieldState;
}

export class RouteAppointmentForecastingFilter extends PXView {
	RouteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	DateBegin: PXFieldState<PXFieldOptions.CommitChanges>;
	DateEnd: PXFieldState<PXFieldOptions.CommitChanges>;
}

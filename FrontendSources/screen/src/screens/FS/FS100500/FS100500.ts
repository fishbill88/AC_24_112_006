import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	columnConfig,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.CalendarComponentSetupMaint', primaryView: 'SetupRecord' })
export class FS100500 extends PXScreen {
	StatusColorSelected = createSingle(FSAppointmentStatusColor);
	StatusColorRecords = createCollection(FSAppointmentStatusColor);
	AppointmentBoxFields = createCollection(AppointmentBoxComponentField);
	ServiceOrderFields = createCollection(ServiceOrderComponentField);
	UnassignedAppointmentFields = createCollection(UnassignedAppComponentField);
}

@gridConfig({
	syncPosition: true,
	autoRepaint: ['StatusColorSelected'],
	allowImport: true
})
export class FSAppointmentStatusColor extends PXView {
	StatusID: PXFieldState<PXFieldOptions.CommitChanges>;
	StatusLabel: PXFieldState;
	BackgroundColor: PXFieldState;
	TextColor: PXFieldState;
	BandColor: PXFieldState;
	IsVisible: PXFieldState;
}

@gridConfig({
	allowImport: true
})
export class AppointmentBoxComponentField extends PXView {
	ImageUrl: PXFieldState
	@columnConfig({ fullState: true }) ObjectName: PXFieldState<PXFieldOptions.CommitChanges>;
	FieldName: PXFieldState;
	IsActive: PXFieldState;
}

@gridConfig({
	allowImport: true
})
export class ServiceOrderComponentField extends PXView {
	@columnConfig({ fullState: true }) ObjectName: PXFieldState<PXFieldOptions.CommitChanges>;
	FieldName: PXFieldState;
	IsActive: PXFieldState;
}

@gridConfig({
	allowImport: true
})
export class UnassignedAppComponentField extends PXView {
	@columnConfig({ fullState: true }) ObjectName: PXFieldState<PXFieldOptions.CommitChanges>;
	FieldName: PXFieldState;
	IsActive: PXFieldState;
}

import {
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	PXActionState,
	columnConfig
} from 'client-controls';

// Views

export class PushNotificationsHook extends PXView  {
	Name: PXFieldState;
	HeaderName: PXFieldState;
	Active: PXFieldState;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	HeaderValue: PXFieldState;
	Address: PXFieldState;
}

@gridConfig({initNewRow: true, syncPosition: true, adjustPageSize: true, autoRepaint: ["TrackingFieldsGI"]})
export class PushNotificationsSourceGI extends PXView  {
	ViewInquiry: PXActionState;

	Active: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DesignID: PXFieldState<PXFieldOptions.CommitChanges>;
	TrackAllFields: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({initNewRow: true, syncPosition: true, adjustPageSize: true})
export class PushNotificationsTrackingFieldGI extends PXView  {
	TableName: PXFieldState<PXFieldOptions.CommitChanges>;
	FieldName: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({initNewRow: true, syncPosition: true, adjustPageSize: true, autoRepaint: ["TrackingFieldsIC"]})
export class PushNotificationsSourceIC extends PXView  {
	Active: PXFieldState;
	InCodeClass: PXFieldState;
	TrackAllFields: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({initNewRow: true, syncPosition: true, adjustPageSize: true})
export class PushNotificationsTrackingFieldIC extends PXView  {
	TableName: PXFieldState<PXFieldOptions.CommitChanges>;
	FieldName: PXFieldState<PXFieldOptions.CommitChanges>;
}

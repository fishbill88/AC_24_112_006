import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState, GridPagerMode } from "client-controls";

// Views

@gridConfig({ syncPosition: true, pagerMode: GridPagerMode.InfiniteScroll, autoAdjustColumns: true, allowDelete: false, allowInsert: false, fastFilterByAllFields: false,
	mergeToolbarWith: 'ScreenToolbar', autoRepaint: ["DispatcherStatisticsPerHour", "DispatcherStatistics", "SlowLogs", "CurrentDispatcherSettings"] })
export class DispatchersStatus extends PXView  {
	QueueType : PXFieldState;
	@columnConfig({width: 20}) Status : PXFieldState;
	QueueCount : PXFieldState;
	QueueSize : PXFieldState;
	QueueName : PXFieldState;
}

export class DispatcherSettings extends PXView  {
	KeepStatisticsForPeriod : PXFieldState<PXFieldOptions.CommitChanges>;
	LogDetails : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class DispatcherSettings2 extends PXView  {
	LogMaxLength : PXFieldState<PXFieldOptions.CommitChanges>;
	LongProcessingThreshold : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, autoAdjustColumns: true, allowDelete: false, allowInsert: false, fastFilterByAllFields: false })
export class DispatcherStatisticsPerMinut extends PXView  {
	clearStatistics: PXActionState;

	@linkCommand("viewPerMinuteStatisticDetails")
	@columnConfig({format: "g"}) CreatedDateTime : PXFieldState;
	Queued : PXFieldState;
	Processed : PXFieldState;
	QueueSize : PXFieldState;
	AverageProcessingTime : PXFieldState;
	MaxProcessingTime : PXFieldState;
}

@gridConfig({ syncPosition: true, adjustPageSize: true, autoAdjustColumns: true, allowDelete: false, allowInsert: false, fastFilterByAllFields: false })
export class DispatcherStatisticsPerHour extends PXView  {
	clearStatistics: PXActionState;

	@linkCommand("viewPerHourStatisticDetails")
	@columnConfig({format: "g"}) CreatedDateTime : PXFieldState;
	Queued : PXFieldState;
	Processed : PXFieldState;
	QueueSize : PXFieldState;
	AverageProcessingTime : PXFieldState;
	MaxProcessingTime : PXFieldState;
}

@gridConfig({ syncPosition: true,  allowDelete: false, allowInsert: false, fastFilterByAllFields: false, adjustPageSize: true, autoAdjustColumns: true })
export class DispatcherSlowLog extends PXView  {
	clearLog: PXActionState;

	@linkCommand("viewLog")
	@columnConfig({format: "g"})
	CreatedDateTime : PXFieldState;
	ProcessingTime : PXFieldState;
	Queries : PXFieldState;
}

export class QueueDispatcherLogBase extends PXView  {
	ProcessingTime : PXFieldState;
	Log : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
}

export class StatisticDetailsFilter extends PXView  {
	From_Date : PXFieldState<PXFieldOptions.CommitChanges>;
	From_Time : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	To_Date : PXFieldState<PXFieldOptions.CommitChanges>;
	To_Time : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	QueueType : PXFieldState;
}

@gridConfig({ syncPosition: true, allowInsert: false, allowDelete: false, adjustPageSize: true, autoAdjustColumns: true })
export class PushNotificationsErrors extends PXView  {
	clearErrors: PXActionState;
	showSourceData: PXActionState;

	HookId : PXFieldState;
	@columnConfig({width: 150})	Source : PXFieldState;
	SourceEvent : PXFieldState;
	ErrorMessage : PXFieldState;
	TimeStamp : PXFieldState;
}

export class CurrentPushNotificationsError extends PXView  {
	SourceData : PXFieldState;
}

export class QueueNotificationSettings extends PXView  {
	FillThreshold : PXFieldState<PXFieldOptions.CommitChanges>;
	IsEmailActive : PXFieldState<PXFieldOptions.CommitChanges>;
	EmailNotificationID : PXFieldState<PXFieldOptions.CommitChanges>;
	IsMobileSmsActive : PXFieldState<PXFieldOptions.CommitChanges>;
	MobileSmsNotificationID : PXFieldState<PXFieldOptions.CommitChanges>;
	IsMobilePushActive : PXFieldState<PXFieldOptions.CommitChanges>;
	MobilePushNotificationID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({ syncPosition: true, allowInsert: false, allowDelete: false, adjustPageSize: true, autoAdjustColumns: true  })
export class DispatcherStatisticQueryDetail extends PXView  {
	Query : PXFieldState;
	Field : PXFieldState;
	Count : PXFieldState;
}

@gridConfig({ syncPosition: true, allowInsert: false, allowDelete: false, adjustPageSize: true, autoAdjustColumns: true  })
export class DispatcherStatisticSourceDetail extends PXView  {
	ScreenID : PXFieldState;
	TableName : PXFieldState;
	Count : PXFieldState;
}

@gridConfig({ syncPosition: true, allowInsert: false, allowDelete: false, adjustPageSize: true, autoAdjustColumns: true  })
export class DispatcherStatisticEventDetail extends PXView  {
	BusinessEventName : PXFieldState;
	Count : PXFieldState;
}

@gridConfig({ syncPosition: true, allowInsert: false, allowDelete: false, adjustPageSize: true, autoAdjustColumns: true  })
export class DispatcherStatisticCommerceDetail extends PXView  {
	Connector : PXFieldState;
	Direction : PXFieldState;
	Count : PXFieldState;
}
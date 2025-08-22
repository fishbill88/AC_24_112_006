import { PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState, TextAlign } from "client-controls";

// Views

export class TaskManagerRunningProcessFilter extends PXView  {
	ActionGC: PXActionState;
	ActionUpdateData: PXActionState;
	ShowAllUsers : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	LoginType : PXFieldState<PXFieldOptions.CommitChanges>;
	GCTotalMemory : PXFieldState;
	WorkingSet : PXFieldState;
	GCCollection : PXFieldState;
	CurrentUtilization : PXFieldState;
	UpTime : PXFieldState;
	ActiveRequests : PXFieldState;
	RequestsSumLastMinute : PXFieldState;
	Source : PXFieldState<PXFieldOptions.CommitChanges>;
	Level : PXFieldState<PXFieldOptions.CommitChanges>;
	FromDate : PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
})
export class RowTaskInfo extends PXView  {
	actionStop : PXActionState;
	actionShow : PXActionState;
	actionStackTrace : PXActionState;
	actionCreateMemoryDump : PXActionState;
	@columnConfig({width: 200})	User : PXFieldState;
	Screen : PXFieldState;
	@columnConfig({width: 150})	Title : PXFieldState;
	@columnConfig({width: 100, textAlign: TextAlign.Right})	Processed : PXFieldState;
	@columnConfig({width: 100, textAlign: TextAlign.Right})	Total : PXFieldState;
	@columnConfig({width: 200, textAlign: TextAlign.Right})	Errors : PXFieldState;
	@columnConfig({width: 100})	WorkTime : PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	topBarItems: {
		actionViewUser: {index: 0, config: {commandName: "actionViewUser", text: "VIEW USER" }}
	}
})
export class RowActiveUserInfo extends PXView  {
	actionViewUser : PXActionState;
	@columnConfig({width: 200})	User : PXFieldState;
	@columnConfig({width: 200})	Company : PXFieldState;
	@columnConfig({width: 200})	LoginType : PXFieldState;
	@columnConfig({width: 200})	LastActivity : PXFieldState;
	@columnConfig({width: 200})	LoginTimeSpan : PXFieldState;
}

@gridConfig({
	syncPosition: true,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	autoRepaint: ["CurrentSystemEvent"]
})
export class SystemEvent extends PXView  {
	@columnConfig({width: 100})	Level : PXFieldState;
	@columnConfig({width: 150})	Source : PXFieldState;
	@columnConfig({width: 450})	EventID : PXFieldState;
	@columnConfig({width: 100})	ScreenId : PXFieldState;
	@columnConfig({width: 150})	LinkToEntity : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({width: 170, textAlign: TextAlign.Right})	Date : PXFieldState;
	TenantName : PXFieldState;
	@columnConfig({width: 200})	User : PXFieldState;
	@columnConfig({width: 200})	Details : PXFieldState;
}

export class SystemEvent2 extends PXView  {
	FormattedProperties : PXFieldState<PXFieldOptions.NoLabel>;
}

@gridConfig({
	syncPosition: true,
	autoAdjustColumns: true,
	topBarItems: {
		actionViewSql: { index: 0, config: { commandName: "actionViewSql", text: "VIEW SQL" } },
		actionViewTrace: { index: 1, config: { commandName: "actionViewTrace", text: "VIEW EVENT LOG" } }
	}
})
export class SMPerformanceInfo extends PXView  {
	ActionViewSql: PXActionState;
	ActionViewExceptions: PXActionState;
	ActionViewTrace: PXActionState

	RequestStartTime : PXFieldState;
	UserId : PXFieldState;
	@columnConfig({width: 170, type: GridColumnType.HyperLink})	ScreenId : PXFieldState;
	@columnConfig({type: GridColumnType.HyperLink})	UrlToScreen : PXFieldState;
	RequestType : PXFieldState;
	CommandTarget : PXFieldState;
	CommandName : PXFieldState;
	RequestTimeMs : PXFieldState;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.True})	SelectTimeMs : PXFieldState<PXFieldOptions.Hidden>;
	SqlTimeMs : PXFieldState;
	RequestCpuTimeMs : PXFieldState;
	@linkCommand('actionViewSql') SqlCounter : PXFieldState;
	SqlRows : PXFieldState;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.True})	SelectCounter : PXFieldState<PXFieldOptions.Hidden>;
	@linkCommand('actionViewExceptions') ExceptionCounter : PXFieldState;
	@linkCommand('actionViewTrace') EventCounter : PXFieldState;
	MemBeforeMb : PXFieldState;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.True})	MemoryWorkingSet : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.True})	ProcessingItems : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.True})	SessionLoadTimeMs : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.True})	Headers : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.True})	TenantId : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.True})	InstallationId : PXFieldState<PXFieldOptions.Hidden>;
}

export class SMPerformanceFilterRow extends PXView  {
	CurrentThreads : PXFieldState<PXFieldOptions.NoLabel>;
}

export class SMPerformanceInfoSQL extends PXView  {
	@columnConfig({width: 300})	TableList : PXFieldState;
	SQLWithStackTrace : PXFieldState;
	SqlId : PXFieldState;
	NRows : PXFieldState;
	RequestStartTime : PXFieldState;
	SqlTimeMs : PXFieldState;
	@columnConfig({width: 250})	ShortParams : PXFieldState;
	@columnConfig({textAlign: TextAlign.Center, type: GridColumnType.CheckBox})	QueryCache : PXFieldState;
}

export class SMPerformanceInfoTraceEvents extends PXView  {
	MessageWithStackTrace : PXFieldState;
	@columnConfig({width: 50})	RequestStartTime : PXFieldState;
	@columnConfig({width: 60})	Source : PXFieldState;
	@columnConfig({width: 60})	TraceType : PXFieldState;
	@columnConfig({width: 250})	ShortMessage : PXFieldState;
}

export class SMPerformanceInfoTraceEvents2 extends PXView  {
	MessageWithStackTrace : PXFieldState;
	@columnConfig({width: 50})	RequestStartTime : PXFieldState;
	@columnConfig({width: 60})	Source : PXFieldState;
	@columnConfig({width: 60})	TraceType : PXFieldState;
	@columnConfig({width: 250})	ShortMessage : PXFieldState;
}

export class RowMemoryDumpOptions extends PXView  {
	DumpContentType: PXFieldState<PXFieldOptions.CommitChanges>;
	InformationMessage : PXFieldState;
	WarningMessage : PXFieldState;
}

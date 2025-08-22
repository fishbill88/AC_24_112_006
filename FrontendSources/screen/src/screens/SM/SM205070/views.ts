import { PXView,
	PXFieldState,
	gridConfig,
	headerDescription,
	ICurrencyInfo,
	disabled,
	selectorSettings,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	GridColumnShowHideMode,
	GridColumnType,
	PXActionState,
	GridColumnGeneration } from 'client-controls';

// Views

export class SMPerformanceFilterRow extends PXView {
	DefaultLogging: PXFieldState<PXFieldOptions.CommitChanges>;
	ProfilerEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeLimit: PXFieldState<PXFieldOptions.CommitChanges>;
	SqlCounterLimit: PXFieldState<PXFieldOptions.CommitChanges>;
	ScreenId: PXFieldState<PXFieldOptions.CommitChanges>;
	UserId: PXFieldState<PXFieldOptions.CommitChanges>;
	SqlProfiler: PXFieldState<PXFieldOptions.CommitChanges>;
	SqlRowCounterLimit: PXFieldState<PXFieldOptions.CommitChanges>;
	SqlTimeLimit: PXFieldState<PXFieldOptions.CommitChanges>;
	SqlMethodFilter: PXFieldState<PXFieldOptions.CommitChanges>;
	SqlProfilerIncludeQueryCache: PXFieldState<PXFieldOptions.CommitChanges>;
	TraceExceptionsEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	TraceEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	LogLevelFilter: PXFieldState<PXFieldOptions.CommitChanges>;
	LogCategoryFilter: PXFieldState<PXFieldOptions.CommitChanges>;
	SqlProfilerShowStackTrace: PXFieldState;
}

@gridConfig({adjustPageSize: true, autoAdjustColumns: true, syncPosition: true, generateColumns: GridColumnGeneration.Append, suppressNoteFiles: true})
export class SMPerformanceInfo extends PXView {
	ActionViewSql: PXActionState;
	ActionViewTrace: PXActionState;
	ActionViewScreen: PXActionState;
	ActionPinRows: PXActionState;

	@columnConfig({allowFilter: false})
	IsPinned: PXFieldState;
	@columnConfig({format: "dd MMM HH:mm:ss"})
	RequestStartTime: PXFieldState;
	UserId: PXFieldState;
	@linkCommand('actionOpenUrl')
	ScreenId: PXFieldState;
	@linkCommand('actionViewScreen')
	InternalScreenId: PXFieldState;
	RequestType: PXFieldState;
	Status: PXFieldState;
	CommandTarget: PXFieldState;
	CommandName: PXFieldState;
	ScriptTimeMs: PXFieldState;
	RequestTimeMs: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	SelectTimeMs: PXFieldState<PXFieldOptions.Hidden>;
	SqlTimeMs: PXFieldState;
	RequestCpuTimeMs: PXFieldState;
	SqlCounter: PXFieldState;
	@linkCommand('actionViewSql')
	LoggedSqlCounter: PXFieldState;
	SqlRows: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	SelectCounter: PXFieldState<PXFieldOptions.Hidden>;
	ExceptionCounter: PXFieldState;
	@linkCommand('actionViewExceptions')
	LoggedExceptionCounter: PXFieldState;
	EventCounter: PXFieldState;
	@linkCommand('actionViewTrace')
	LoggedEventCounter: PXFieldState;
	MemBeforeMb: PXFieldState;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	MemDeltaMb: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	MemoryWorkingSet: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	ProcessingItems: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	SessionLoadTimeMs: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	SessionSaveTimeMs: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	Headers: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	TenantId: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	InstallationId: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowShowHide: GridColumnShowHideMode.True})
	SqlDigest: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({adjustPageSize: true, autoAdjustColumns: true, syncPosition: true})
export class SMPerformanceInfoSQLSummary extends PXView {
	@linkCommand('actionViewSqlSummaryRows')
	RecordId: PXFieldState;
	TableList: PXFieldState;
	SqlText: PXFieldState;
	QueryHash: PXFieldState;
	TotalSQLTime: PXFieldState;
	TotalExecutions: PXFieldState;
	TotalRows: PXFieldState;
}

@gridConfig({adjustPageSize: true, autoAdjustColumns: true, syncPosition: true})
export class SMPerformanceInfoExceptionSummary extends PXView {
	ActionViewExceptionDetails: PXActionState;

	Tenant: PXFieldState;
	ExceptionType: PXFieldState;
	ExceptionMessage: PXFieldState;
	Count: PXFieldState;
	@columnConfig({format: "dd MMM HH:mm:ss"})
	LastOccured: PXFieldState;
	LastUrl: PXFieldState;
	LastCommandTarget: PXFieldState;
	LastCommandName: PXFieldState;
	LastStackTrace: PXFieldState;
}

@gridConfig({adjustPageSize: true, autoAdjustColumns: true, syncPosition: true})
export class SMPerformanceInfoTraceEvents extends PXView {
	ActionViewEventDetails: PXActionState;

	@columnConfig({format: "dd MMM HH:mm:ss", width: 130})
	EventDateTime: PXFieldState;
	TraceType: PXFieldState;
	SMPerformanceInfo__UserId: PXFieldState;
	SMPerformanceInfo__TenantId: PXFieldState;
	Source: PXFieldState;
	SMPerformanceInfo__InternalScreenId: PXFieldState;
	ShortMessage: PXFieldState;
	StackTrace: PXFieldState;
	@linkCommand('actionOpenUrl')
	SMPerformanceInfo__ScreenId: PXFieldState;
	SMPerformanceInfo__CommandTarget: PXFieldState;
	SMPerformanceInfo__CommandName: PXFieldState;
}

@gridConfig({allowInsert: false, allowDelete: false, adjustPageSize: true, autoAdjustColumns: true})
export class SMPerformanceInfoSQL extends PXView {
	TableList: PXFieldState;
	SQLWithStackTrace: PXFieldState<PXFieldOptions.Hidden>;
	QueryOrderID: PXFieldState<PXFieldOptions.Hidden>;
	SqlId: PXFieldState<PXFieldOptions.Hidden>;
	SQLHash: PXFieldState;
	NRows: PXFieldState;
	RequestStartTime: PXFieldState;
	SqlTimeMs: PXFieldState;
	ShortParams: PXFieldState;
	QueryCache: PXFieldState;
}

@gridConfig({allowInsert: false, allowDelete: false, adjustPageSize: true, autoAdjustColumns: true})
export class SMPerformanceInfoTraceEvents2 extends PXView {
	MessageWithStackTrace: PXFieldState<PXFieldOptions.Hidden>;
	RequestStartTime: PXFieldState;
	Source: PXFieldState;
	ExceptionType: PXFieldState;
	MessageText: PXFieldState;
}

@gridConfig({allowInsert: false, allowDelete: false, adjustPageSize: true, autoAdjustColumns: true})
export class SMPerformanceInfoTraceEvents3 extends PXView {
	MessageWithStackTrace: PXFieldState<PXFieldOptions.Hidden>;
	RequestStartTime: PXFieldState;
	Source: PXFieldState;
	TraceType: PXFieldState;
	ShortMessage: PXFieldState;
}

export class SMPerformanceInfoSQLSummary2 extends PXView {
	RecordId: PXFieldState<PXFieldOptions.Disabled>;
	TotalSQLTime: PXFieldState;
	TotalExecutions: PXFieldState;
	TotalRows: PXFieldState;
}

@gridConfig({allowInsert: false, allowDelete: false, adjustPageSize: true, autoAdjustColumns: true, syncPosition: true,
	autoRepaint: ['SqlSummaryRowsPreview']})
export class SMPerformanceInfoSQL2 extends PXView {
	TableList: PXFieldState<PXFieldOptions.Hidden>;
	SQLWithStackTrace: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({format: "dd MMM HH:mm:ss"})
	RequestDateTime: PXFieldState;
	SQLParams: PXFieldState;
	SqlTimeMs: PXFieldState;
	NRows: PXFieldState;
	SMPerformanceInfo__ScreenId: PXFieldState;
	SMPerformanceInfo__CommandTarget: PXFieldState;
	SMPerformanceInfo__CommandName: PXFieldState;
	StackTrace: PXFieldState;
}

export class SMPerformanceInfoSQL3 extends PXView {
	SQLWithStackTrace: PXFieldState;
}

export class SMPerformanceInfoTraceEvents4 extends PXView {
	EventDateTime: PXFieldState<PXFieldOptions.Disabled>;
	TraceType: PXFieldState<PXFieldOptions.Disabled>;
	EventDetails: PXFieldState<PXFieldOptions.Disabled>;
	StackTrace: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({adjustPageSize: true, autoAdjustColumns: true})
export class SMPerformanceInfoTraceEvents5 extends PXView {
	MessageWithStackTrace: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({format: "dd MMM HH:mm:ss"})
	EventDateTime: PXFieldState;
	SMPerformanceInfo__InternalScreenId: PXFieldState;
	SMPerformanceInfo__ScreenId: PXFieldState;
	SMPerformanceInfo__CommandTarget: PXFieldState;
	SMPerformanceInfo__CommandName: PXFieldState;
	StackTrace: PXFieldState;
}

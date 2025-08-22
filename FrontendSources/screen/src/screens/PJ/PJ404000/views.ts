import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	GridColumnType,
	GridColumnShowHideMode,
	linkCommand,
	columnConfig,
	selectorSettings,
	ICurrencyInfo,
	gridConfig
} from "client-controls";

export class Filter extends PXView {
	ProjectId: PXFieldState<PXFieldOptions.CommitChanges>;
	WeatherApiService: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestDateFrom: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestDateTo: PXFieldState<PXFieldOptions.CommitChanges>;
	IsShowErrorsOnly: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: true,
	suppressNoteFiles: true
})
export class WeatherProcessingLogs extends PXView {
	@linkCommand("ViewEntity")
	DailyFieldReport__DailyFieldReportCd: PXFieldState;
	DailyFieldReport__Status: PXFieldState;
	DailyFieldReport__Date: PXFieldState;
	@linkCommand("ViewEntity")
	DailyFieldReport__ProjectId: PXFieldState;
	@linkCommand("ViewEntity")
	@columnConfig({ width: 150 })
	DailyFieldReport__ProjectManagerId: PXFieldState;
	@linkCommand("ViewEntity")
	@columnConfig({ width: 150 })
	DailyFieldReport__CreatedById: PXFieldState;
	WeatherService: PXFieldState;
	RequestTime: PXFieldState;
	RequestBody: PXFieldState;
	RequestStatusIcon: PXFieldState;
	ResponseTime: PXFieldState;
	ResponseBody: PXFieldState;
}


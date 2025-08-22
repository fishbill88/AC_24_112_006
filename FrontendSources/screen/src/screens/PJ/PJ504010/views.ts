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

@gridConfig({
	mergeToolbarWith: 'ScreenToolbar',
	wrapToolbar: true,
	syncPosition: true,
	adjustPageSize: false,
	allowInsert: false,
	allowUpdate: false,
	allowDelete: false
})
export class WeatherProcessingLogs extends PXView {
	@columnConfig({
		allowCheckAll: true,
		allowShowHide: GridColumnShowHideMode.False
	})
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewEntity")
	DailyFieldReport__DailyFieldReportCd: PXFieldState;
	DailyFieldReport__Status: PXFieldState;
	DailyFieldReport__Date: PXFieldState;
	@linkCommand("ViewEntity")
	DailyFieldReport__ProjectId: PXFieldState;
	@linkCommand("ViewEntity")
	DailyFieldReport__ProjectManagerId: PXFieldState;
	@linkCommand("ViewEntity")
	DailyFieldReport__CreatedById: PXFieldState;
	WeatherService: PXFieldState;
	RequestTime: PXFieldState;
	RequestBody: PXFieldState;
	RequestStatusIcon: PXFieldState;
	ResponseTime: PXFieldState;
	ResponseBody: PXFieldState;
}


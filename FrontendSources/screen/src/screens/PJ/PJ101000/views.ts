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

export class ProjectManagementSetup extends PXView {
	AnswerDaysCalculationType: PXFieldState<PXFieldOptions.CommitChanges>;
	CalendarId: PXFieldState;
	RequestForInformationNumberingId: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultEmailNotification: PXFieldState<PXFieldOptions.CommitChanges>;
	RequestForInformationAssignmentMapId: PXFieldState;
	DailyFieldReportNumberingId: PXFieldState<PXFieldOptions.CommitChanges>;
	DailyFieldReportApprovalMapId: PXFieldState;
	PendingApprovalNotification: PXFieldState;
	IsHistoryLogEnabled: PXFieldState;
	ProjectIssueNumberingId: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectIssueAssignmentMapId: PXFieldState;
	SubmittalNumberingId: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	initNewRow: true
})
export class ProjectIssueTypes extends PXView {
	TypeName: PXFieldState;
	Description: PXFieldState;
}

export class DailyFieldReportCopyConfiguration extends PXView {
	IsConfigurationEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	Notes: PXFieldState<PXFieldOptions.CommitChanges>;
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectManager: PXFieldState<PXFieldOptions.CommitChanges>;
	Employee: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeEarningType: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeProjectTask: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeCostCode: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeTime: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeTimeSpent: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeIsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeBillableTime: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeDescription: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeTask: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeCertifiedJob: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeUnionLocal: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeLaborItem: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeWccCode: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeContract: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractorVendorId: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractorProjectTask: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractorCostCode: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractorNumberOfWorkers: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractorTimeArrived: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractorTimeDeparted: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractorWorkingHours: PXFieldState<PXFieldOptions.CommitChanges>;
	SubcontractorDescription: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentId: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentProjectTask: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentCostCode: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentIsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentSetupTime: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentRunTime: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentSuspendTime: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentDescription: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class WeatherIntegrationSetup extends PXView {
	TestConnection: PXActionState;
	IsConfigurationEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	WeatherApiService: PXFieldState<PXFieldOptions.CommitChanges>;
	WeatherApiKey: PXFieldState;
	UnitOfMeasureFormat: PXFieldState;
	RequestParametersType: PXFieldState;
	IsWeatherProcessingLogEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	WeatherProcessingLogTerm: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	wrapToolbar: true,
	adjustPageSize: false,
	initNewRow: true
})
export class SubmittalTypes extends PXView {
	TypeName: PXFieldState;
	Description: PXFieldState;
}


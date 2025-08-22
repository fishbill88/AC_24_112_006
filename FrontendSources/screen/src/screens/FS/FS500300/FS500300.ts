import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	linkCommand,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	columnConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ServiceContractInq', primaryView: 'Filter' })
export class FS500300 extends PXScreen {
	OpenServiceContractScreenBySchedules: PXActionState;
	OpenScheduleScreenBySchedules: PXActionState;
	OpenServiceContractScreenByGenerationLogError: PXActionState;
	OpenScheduleScreenByGenerationLogError: PXActionState;
	Filter = createSingle(ServiceContractFilter);
	ServiceContractSchedules = createCollection(FSContractSchedule);
	ContractHistoryRecords = createCollection(FSContractGenerationHistory);
	ErrorMessageRecords = createCollection(FSGenerationLogError);
}

export class ServiceContractFilter extends PXView {
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class FSContractSchedule extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) FSServiceContract__BranchID: PXFieldState;
	@columnConfig({ hideViewLink: true }) FSServiceContract__BranchLocationID: PXFieldState;
	CustomerID: PXFieldState;
	@columnConfig({ hideViewLink: true }) FSServiceContract__CustomerLocationID: PXFieldState;
	@linkCommand("OpenServiceContractScreenBySchedules") FSServiceContract__RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) FSServiceContract__CustomerContractNbr: PXFieldState;
	FSServiceContract__DocDesc: PXFieldState;
	@linkCommand("OpenScheduleScreenBySchedules") RefNbr: PXFieldState;
	CustomerLocationID: PXFieldState;
	RecurrenceDescription: PXFieldState;
	ScheduleGenType: PXFieldState;
	StartDate: PXFieldState;
	EndDate: PXFieldState;
	LastGeneratedElementDate: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class FSContractGenerationHistory extends PXView {
	GenerationID: PXFieldState;
	LastProcessedDate: PXFieldState;
	LastModifiedDateTime: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class FSGenerationLogError extends PXView {
	ClearAll: PXActionState;
	GenerationID: PXFieldState;
	FSServiceContract__CustomerID: PXFieldState;
	@linkCommand("OpenServiceContractScreenByGenerationLogError") FSServiceContract__RefNbr: PXFieldState;
	@linkCommand("OpenScheduleScreenByGenerationLogError") FSSchedule__RefNbr: PXFieldState;
	ErrorDate: PXFieldState;
	ErrorMessage: PXFieldState;
}

import {
	graphInfo,
	gridConfig,
	columnConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.StaffContractScheduleProcess', primaryView: 'Filter' })
export class FS500400 extends PXScreen {
	Filter = createSingle(StaffScheduleFilter);
	StaffSchedules = createCollection(FSStaffSchedule);
	ContractHistoryRecords = createCollection(FSContractGenerationHistory);
}

export class StaffScheduleFilter extends PXView {
	BAccountID: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ToDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class FSStaffSchedule extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState;
	StaffScheduleDescription: PXFieldState;
	EmployeeID: PXFieldState;
	BAccount__AcctName: PXFieldState;
	ScheduleType: PXFieldState;
	StartDate_Date: PXFieldState;
	EndDate_Date: PXFieldState;
	StartTime_Time: PXFieldState;
	EndTime_Time: PXFieldState;
	RecurrenceDescription: PXFieldState;
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

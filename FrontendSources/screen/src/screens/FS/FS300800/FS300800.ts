import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	linkCommand,
	PXActionState
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RouteServiceContractEntry', primaryView: 'ServiceContractRecords' })
export class FS300800 extends PXScreen {

	OpenRouteScheduleScreen: PXActionState;
	ActivatePeriod: PXActionState;
	OpenRouteScheduleScreenByScheduleDetService: PXActionState;
	OpenRouteScheduleScreenByScheduleDetPart: PXActionState;
	FSBillHistory$ChildDocLink$Link: PXActionState;
	openPostBatch: PXActionState;
	ServiceContractRecords = createSingle(FSServiceContract);
	ContractSchedules = createCollection(FSContractSchedule);
	ContractPeriodFilter = createSingle(FSContractPeriodFilter);
	ContractPeriodDetRecords = createCollection(FSContractPeriodDet);
	InvoiceRecords = createCollection(FSBillHistory);
	SalesPriceLines = createCollection(InventoryItem);
	ContractHistoryItems = createCollection(FSContractAction);
	Answers = createCollection(CSAnswers);
	ActiveScheduleRecords = createCollection(ActiveSchedule);
	ActivationContractFilter = createSingle(FSActivationContractFilter);
	TerminateContractFilter = createSingle(FSTerminateContractFilter);
	SuspendContractFilter = createSingle(FSSuspendContractFilter);
	CopyContractFilter = createSingle(FSCopyContractFilter);
	ForecastFilter = createSingle(FSContractForecastFilter);
}

export class FSServiceContract extends PXView {
	RefNbr: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerContractNbr: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	MasterContractID: PXFieldState;
	Status: PXFieldState;
	StatusEffectiveFromDate: PXFieldState;
	UpcomingStatus: PXFieldState;
	StatusEffectiveUntilDate: PXFieldState;
	DocDesc: PXFieldState;
	Mem_ShowPriceTab: PXFieldState;
	Mem_ShowScheduleTab: PXFieldState;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpirationType: PXFieldState<PXFieldOptions.CommitChanges>;
	Duration: PXFieldState<PXFieldOptions.CommitChanges>;
	DurationType: PXFieldState<PXFieldOptions.CommitChanges>;
	RenewalDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleGenType: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState;
	CustomerContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	Commissionable: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingType: PXFieldState<PXFieldOptions.CommitChanges>;
	BillTo: PXFieldState<PXFieldOptions.CommitChanges>;
	BillCustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillLocationID: PXFieldState;
	UsageBillingCycleID: PXFieldState;
	BillingPeriod: PXFieldState<PXFieldOptions.CommitChanges>;
	LastBillingInvoiceDate: PXFieldState;
	NextBillingInvoiceDate: PXFieldState;
	SourcePrice: PXFieldState;
	DfltProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltCostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class FSContractSchedule extends PXView {
	AddSchedule: PXActionState;
	@linkCommand("OpenRouteScheduleScreen") RefNbr: PXFieldState;
	ScheduleGenType: PXFieldState;
	SrvOrdType: PXFieldState;
	CustomerLocationID: PXFieldState;
	Active: PXFieldState;
	RecurrenceDescription: PXFieldState;
}

export class FSContractPeriodFilter extends PXView {
	Actions: PXFieldState<PXFieldOptions.CommitChanges>;
	ContractPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	PostDocRefNbr: PXFieldState;
	StandardizedBillingTotal: PXFieldState;
}

export class FSContractPeriodDet extends PXView {
	ActivatePeriod: PXActionState;
	LineType: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SMequipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingRule: PXFieldState<PXFieldOptions.CommitChanges>;
	Amount: PXFieldState<PXFieldOptions.CommitChanges>;
	Time: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	RegularPrice: PXFieldState;
	RecurringUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	RecurringTotalPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	OverageItemPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	RemainingAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	RemainingTime: PXFieldState<PXFieldOptions.CommitChanges>;
	RemainingQty: PXFieldState<PXFieldOptions.CommitChanges>;
	UsedAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	UsedTime: PXFieldState<PXFieldOptions.CommitChanges>;
	UsedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledTime: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledQty: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState;
	DeferredCode: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class FSBillHistory extends PXView {
	@linkCommand("openPostBatch") BatchID: PXFieldState;
	ServiceContractPeriodID: PXFieldState;
	ContractPeriodStatus: PXFieldState;
	ChildEntityType: PXFieldState;
	@linkCommand("FSBillHistory$ChildDocLink$Link") ChildDocLink: PXFieldState;
	ChildDocDesc: PXFieldState;
	ChildDocDate: PXFieldState;
	ChildDocStatus: PXFieldState;
	ChildAmount: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class InventoryItem extends PXView {
	InventoryItem__InventoryCD: PXFieldState;
	LineType: PXFieldState;
	Mem_UnitPrice: PXFieldState;
	UOM: PXFieldState;
	CuryID: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class FSContractAction extends PXView {
	Type: PXFieldState;
	Action: PXFieldState;
	ActionBusinessDate: PXFieldState;
	EffectiveDate: PXFieldState;
	ScheduleRefNbr: PXFieldState;
	ScheduleChangeRecurrence: PXFieldState;
	ScheduleNextExecutionDate: PXFieldState;
	ScheduleRecurrenceDescr: PXFieldState;
	OrigServiceContractRefNbr: PXFieldState;
	OrigSceduleRefNbr: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class CSAnswers extends PXView {
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}

export class FSActivationContractFilter extends PXView {
	ActivationDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class ActiveSchedule extends PXView {
	RefNbr: PXFieldState;
	RecurrenceDescription: PXFieldState;
	ChangeRecurrence: PXFieldState<PXFieldOptions.CommitChanges>;
	EffectiveRecurrenceStartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	NextExecution: PXFieldState;
}

export class FSTerminateContractFilter extends PXView {
	CancelationDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSSuspendContractFilter extends PXView {
	SuspensionDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSCopyContractFilter extends PXView {
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState;
}

export class FSContractForecastFilter extends PXView {
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	columnConfig,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ServiceContractScheduleEntry', primaryView: 'ContractScheduleRecords' })
export class FS305100 extends PXScreen {
	AddSelectedItems: PXActionState;
	ContractScheduleRecords = createSingle(FSContractSchedule);
	ItemFilter = createSingle(FSSiteStatusFilter);
	ItemInfo = createCollection(FSSiteStatusSelected);
	CurrentServiceContract = createSingle(FSServiceContract);
	ScheduleDetails = createCollection(FSScheduleDet);
	ContractScheduleSelected = createSingle(FSContractSchedule);
	Answers = createCollection(CRAttributeList);
	FromToFilter = createSingle(FromToFilter);
	ScheduleProjectionRecords = createCollection(ScheduleProjection);
}

export class FSContractSchedule extends PXView {
	EntityID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState;
	Active: PXFieldState;
	CustomerID: PXFieldState;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleGenType: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Hidden>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Hidden>;
	ScheduleStartTime_Time: PXFieldState;
	ScheduleDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	NextExecutionDate: PXFieldState<PXFieldOptions.CommitChanges>;
	LastGeneratedElementDate: PXFieldState<PXFieldOptions.Readonly>;
	VendorID: PXFieldState;
	FrequencyType: PXFieldState<PXFieldOptions.CommitChanges>;
	RecurrenceDescription: PXFieldState<PXFieldOptions.Readonly>;
	DailyFrequency: PXFieldState;
	DailyLabel: PXFieldState<PXFieldOptions.Readonly>;
	WeeklyFrequency: PXFieldState;
	WeeklyLabel: PXFieldState<PXFieldOptions.Readonly>;
	WeeklyOnSun: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnMon: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnTue: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnWed: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnThu: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnFri: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyOnSat: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyFrequency: PXFieldState;
	MonthlyLabel: PXFieldState<PXFieldOptions.Readonly>;
	MonthlyRecurrenceType1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek1: PXFieldState<PXFieldOptions.CommitChanges>;
	Monthly2Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyRecurrenceType2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek2: PXFieldState<PXFieldOptions.CommitChanges>;
	Monthly3Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyRecurrenceType3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek3: PXFieldState<PXFieldOptions.CommitChanges>;
	Monthly4Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyRecurrenceType4: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay4: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek4: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek4: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualFrequency: PXFieldState;
	YearlyLabel: PXFieldState<PXFieldOptions.Readonly>;
	AnnualOnJan: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnFeb: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnMar: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnApr: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnMay: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnJun: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnJul: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnAug: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnSep: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnOct: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnNov: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnDec: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualRecurrenceType: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnDay: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnWeek: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnDayOfWeek: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowImport: true
})
export class FSScheduleDet extends PXView {
	ShowItems: PXActionState;
	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
	LineType: PXFieldState;
	InventoryID: PXFieldState;
	BillingRule: PXFieldState;
	ServiceTemplateID: PXFieldState;
	EstimatedDuration: PXFieldState;
	Qty: PXFieldState;
	SMEquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	ComponentID: PXFieldState;
	EquipmentLineRef: PXFieldState;
	TranDesc: PXFieldState;
	ProjectTaskID: PXFieldState;
	EquipmentAction: PXFieldState;
	CostCodeID: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class ScheduleProjection extends PXView {
	Date: PXFieldState<PXFieldOptions.CommitChanges>;
	DayOfWeek: PXFieldState;
	WeekOfYear: PXFieldState;
	BeginDateOfWeek: PXFieldState;
}

export class FromToFilter extends PXView {
	DateBegin: PXFieldState<PXFieldOptions.CommitChanges>;
	DateEnd: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class CRAttributeList extends PXView {
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}

export class FSServiceContract extends PXView {
	CustomerContractNbr: PXFieldState;
}

export class FSSiteStatusFilter extends PXView {
	LineType: PXFieldState<PXFieldOptions.CommitChanges>;
	Inventory: PXFieldState;
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem: PXFieldState<PXFieldOptions.CommitChanges>;
	Mode: PXFieldState<PXFieldOptions.CommitChanges>;
	OnlyAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
	HistoryDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class FSSiteStatusSelected extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	DurationSelected: PXFieldState;
	SiteID: PXFieldState;
	ItemClassID: PXFieldState;
	ItemType: PXFieldState;
	ItemClassDescription: PXFieldState;
	PriceClassID: PXFieldState;
	PriceClassDescription: PXFieldState;
	PreferredVendorID: PXFieldState;
	PreferredVendorDescription: PXFieldState;
	InventoryCD: PXFieldState;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	EstimatedDuration: PXFieldState;
	BillingRule: PXFieldState;
	SalesUnit: PXFieldState;
	QtyAvailSale: PXFieldState;
	QtyOnHandSale: PXFieldState;
	CuryID: PXFieldState;
	QtyLastSale: PXFieldState;
	CuryUnitPrice: PXFieldState;
	LastSalesDate: PXFieldState;
	AlternateID: PXFieldState;
	AlternateType: PXFieldState;
	AlternateDescr: PXFieldState;
}

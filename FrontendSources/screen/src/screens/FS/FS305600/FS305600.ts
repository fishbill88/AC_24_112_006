import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	columnConfig,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RouteServiceContractScheduleEntry', primaryView: 'ContractScheduleRecords' })
export class FS305600 extends PXScreen {
	AddSelectedItems: PXActionState;

	ForecastFilter = createSingle(FSContractForecastFilter);
	ItemFilter = createSingle(FSSiteStatusFilter);
	ItemInfo = createCollection(FSSiteStatusSelected);
	ContractScheduleRecords = createSingle(Customer);
	CurrentServiceContract = createSingle(FSServiceContract);
	ScheduleDetails = createCollection(ScheduleDetOrdered);
	ContractScheduleSelected = createSingle(FSRouteContractSchedule);
	Answers = createCollection(FSAttributeList);
	ScheduleRoutes = createSingle(FSScheduleRoute);
	WeekCodeFilter = createSingle(WeekCodeFilter);
	WeekCodeDateRecords = createCollection(FSWeekCodeDate);
	FromToFilter = createSingle(FromToFilter);
	ScheduleProjectionRecords = createCollection(ScheduleProjection);
}

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

export class FSWeekCodeDate extends PXView {
	WeekCodeDate: PXFieldState<PXFieldOptions.CommitChanges>;
	WeekCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Mem_DayOfWeek: PXFieldState;
	Mem_WeekOfYear: PXFieldState;
	BeginDateOfWeek: PXFieldState;
}

export class WeekCodeFilter extends PXView {
	DateBegin: PXFieldState<PXFieldOptions.CommitChanges>;
	DateEnd: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSScheduleRoute extends PXView {
	DfltRouteID: PXFieldState<PXFieldOptions.CommitChanges>;
	GlobalSequence: PXFieldState;
	DeliveryNotes: PXFieldState;
	InternalNotes: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class FSAttributeList extends PXView {
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}

export class FSRouteContractSchedule extends PXView {
	FrequencyType: PXFieldState<PXFieldOptions.CommitChanges>;
	RecurrenceDescription: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Readonly>;
	DailyFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	DailyLabel: PXFieldState<PXFieldOptions.Readonly>;
	WeeklyFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	WeeklyLabel: PXFieldState<PXFieldOptions.Readonly>;
	MonthlyFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyLabel: PXFieldState<PXFieldOptions.Readonly>;
	AnnualFrequency: PXFieldState<PXFieldOptions.CommitChanges>;
	YearlyLabel: PXFieldState<PXFieldOptions.Readonly>;
	WeeklyOnSun: PXFieldState;
	WeeklyOnMon: PXFieldState;
	WeeklyOnTue: PXFieldState;
	WeeklyOnWed: PXFieldState;
	WeeklyOnThu: PXFieldState;
	WeeklyOnFri: PXFieldState;
	WeeklyOnSat: PXFieldState;
	MonthlyRecurrenceType1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyRecurrenceType2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyRecurrenceType3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyRecurrenceType4: PXFieldState<PXFieldOptions.CommitChanges>;
	Monthly2Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Monthly3Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Monthly4Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek1: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek2: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek3: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDay4: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnWeek4: PXFieldState<PXFieldOptions.CommitChanges>;
	MonthlyOnDayOfWeek4: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnDay: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnWeek: PXFieldState<PXFieldOptions.CommitChanges>;
	AnnualOnDayOfWeek: PXFieldState<PXFieldOptions.CommitChanges>;
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
}

export class ScheduleDetOrdered extends PXView {
	ShowItems: PXActionState;
	LineType: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingRule: PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceTemplateID: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimatedDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	SMEquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState;
	SortOrder: PXFieldState;
	LineNbr: PXFieldState;
}

export class FSServiceContract extends PXView {
	CustomerContractNbr: PXFieldState;
}

export class Customer extends PXView {
	EntityID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState;
	Active: PXFieldState;
	CustomerID: PXFieldState;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleGenType: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
	NextExecutionDate: PXFieldState;
	LastGeneratedElementDate: PXFieldState;
	WeekCode: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState;
	FrequencyType: PXFieldState;
	RecurrenceDescription: PXFieldState;
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class FSSiteStatusSelected extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	QtySelected: PXFieldState;
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

export class FSContractForecastFilter extends PXView {
	StartDate: PXFieldState<PXFieldOptions.CommitChanges>;
	EndDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

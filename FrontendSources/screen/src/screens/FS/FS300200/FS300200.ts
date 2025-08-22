import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	linkCommand,
	columnConfig,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	ICurrencyInfo,
	GridPagerMode
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.AppointmentEntry', primaryView: 'AppointmentRecords', showUDFIndicator: true })
export class FS300200 extends PXScreen {
	QuickProcessOk: PXActionState;
	AddSelectedItems: PXActionState;
	AddressLookup: PXActionState;

	//Commands
	AddReceipt: PXActionState;
	AddBill: PXActionState;
	Refresh: PXActionState;
	ViewDirectionOnMap: PXActionState;
	ViewStartGPSOnMap: PXActionState;
	ViewCompleteGPSOnMap: PXActionState;
	AddInvSelBySite: PXActionState;

	//LinkCommands
	ViewPOVendor: PXActionState;
	ViewPOVendorLocation: PXActionState;
	viewLinkedDoc: PXActionState;
	ViewEmployee: PXActionState;
	openPostBatch: PXActionState;
	FSBillHistory$ChildDocLink$Link: PXActionState;
	OpenSourceDocument: PXActionState;
	OpenScheduleScreen: PXActionState;
	FSApptLineSplit$RefNoteID$Link: PXActionState;

	AppointmentRecords = createSingle(FSAppointment);
	ServiceOrderRelated = createSingle(FSServiceOrder);
	AppointmentSelected = createSingle(FSAppointment);
	ServiceOrder_Contact = createSingle(FSContact);
	ServiceOrder_Address = createSingle(FSAddress);
	AppointmentDetails = createCollection(FSAppointmentDetail);
	Taxes = createCollection(FSAppointmentTaxTran);
	AppointmentServiceEmployees = createCollection(FSAppointmentEmployee);
	AppointmentResources = createCollection(FSAppointmentResource);
	LogRecords = createCollection(FSAppointmentLog);
	ProfitabilityRecords = createCollection(FSProfitability);
	Answers = createCollection(FSAppointmentAnswer);
	Adjustments = createCollection(ARPayment);
	InvoiceRecords = createCollection(FSBillHistory);
	ScheduleRecord = createSingle(FSSchedule);
	Splits = createCollection(FSApptLineSplit);
	QuickProcessParameters = createSingle(FSAppQuickProcessParams);
	StaffSelectorFilter = createSingle(StaffSelectionFilter);
	SkillGridFilter = createCollection(SkillGridFilter);
	LicenseTypeGridFilter = createCollection(LicenseTypeGridFilter);
	StaffRecords = createCollection(BAccountStaffMember);
	LogActionStartFilter = createSingle(FSLogActionStartFilter);
	LogActionStaffDistinctRecords = createCollection(FSAppointmentStaffDistinct);
	LogActionPCRFilter = createSingle(FSLogActionPCRFilter);
	LogActionLogRecords = createCollection(FSAppointmentLogExtItemLine);
	LogActionStartServiceFilter = createSingle(FSLogActionStartServiceFilter);
	ServicesLogAction = createCollection(FSDetailFSLogAction);
	LogActionStartStaffFilter = createSingle(FSLogActionStartStaffFilter);
	LogActionStaffRecords = createCollection(FSAppointmentStaffExtItemLine);
	ItemFilter = createSingle(FSSiteStatusFilter);
	ItemInfo = createCollection(FSSiteStatusSelected);
	currencyinfo = createSingle(CurrencyInfo);
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
	SiteCD: PXFieldState;
	ItemClassID: PXFieldState;
	ItemType: PXFieldState;
	ItemClassDescription: PXFieldState;
	PriceClassID: PXFieldState;
	PriceClassDescription: PXFieldState;
	PreferredVendorID: PXFieldState;
	PreferredVendorDescription: PXFieldState;
	InventoryCD: PXFieldState;
	SubItemCD: PXFieldState;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	EstimatedDuration: PXFieldState;
	BillingRule: PXFieldState;
	SalesUnit: PXFieldState;
	QtyAvailSale: PXFieldState;
	QtyOnHandSale: PXFieldState;
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

export class FSAppointmentStaffExtItemLine extends PXView {
	@columnConfig({ allowCheckAll: true }) Selected: PXFieldState;
	LineRef: PXFieldState;
	BAccountID: PXFieldState;
	InventoryID: PXFieldState;
	Descr: PXFieldState;
	EstimatedDuration: PXFieldState;
}

export class FSLogActionStartStaffFilter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Me: PXFieldState<PXFieldOptions.CommitChanges>;
	LogDateTime_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	LogDateTime_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	DetLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSDetailFSLogAction extends PXView {
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	LineRef: PXFieldState;
	InventoryID: PXFieldState;
	Descr: PXFieldState;
	EstimatedDuration: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSLogActionStartServiceFilter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Me: PXFieldState<PXFieldOptions.CommitChanges>;
	LogDateTime_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	LogDateTime_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	DetLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSAppointmentLogExtItemLine extends PXView {
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LineRef: PXFieldState;
	Status: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	Travel: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTimeBegin_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTimeBegin_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTimeEnd_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTimeEnd_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeDuration: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSLogActionPCRFilter extends PXView {
	Action: PXFieldState<PXFieldOptions.CommitChanges>;
	Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Me: PXFieldState<PXFieldOptions.CommitChanges>;
	LogDateTime_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	LogDateTime_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	DetLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSAppointmentStaffDistinct extends PXView {
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSLogActionStartFilter extends PXView {
	Action: PXFieldState;
	Type: PXFieldState;
	Me: PXFieldState;
	LogDateTime_Date: PXFieldState;
	LogDateTime_Time: PXFieldState;
	DetLineRef: PXFieldState;
}

@gridConfig({
	pagerMode: GridPagerMode.InfiniteScroll,
	allowInsert: false,
	allowDelete: false,
	suppressNoteFiles: true
})
export class BAccountStaffMember extends PXView {
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ visible: false })
	Type: PXFieldState;
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
}

@gridConfig({
	pagerMode: GridPagerMode.InfiniteScroll,
	allowInsert: false,
	allowDelete: false,
	suppressNoteFiles: true
})
export class LicenseTypeGridFilter extends PXView {
	Mem_Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ visible: false })
	LicenseTypeCD: PXFieldState;
	Descr: PXFieldState;
}

@gridConfig({
	pagerMode: GridPagerMode.InfiniteScroll,
	allowInsert: false,
	allowDelete: false,
	suppressNoteFiles: true
})
export class SkillGridFilter extends PXView {
	Mem_Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ visible: false })
	SkillCD: PXFieldState;
	Descr: PXFieldState;
	Mem_ServicesList: PXFieldState;
}

export class StaffSelectionFilter extends PXView {
	ServiceLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
	Postalcode: PXFieldState;
	GeoZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSAppQuickProcessParams extends PXView {
	CloseAppointment: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailSignedAppointment: PXFieldState<PXFieldOptions.CommitChanges>;
	GenerateInvoiceFromAppointment: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepareInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	SOQuickProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	EMailSalesOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	ReleaseInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSApptLineSplit extends PXView {
	SubItemID: PXFieldState;
	SiteID: PXFieldState;
	LocationID: PXFieldState;
	LotSerialNbr: PXFieldState;
	Qty: PXFieldState;
	UOM: PXFieldState;
	ExpireDate: PXFieldState;
	@linkCommand("FSApptLineSplit$RefNoteID$Link")
	RefNoteID: PXFieldState;
	InventoryID_InventoryItem_descr: PXFieldState;
}

export class FSSchedule extends PXView {
	RecurrenceDescription: PXFieldState<PXFieldOptions.Readonly>;
}

export class FSBillHistory extends PXView {
	@linkCommand("openPostBatch")
	BatchID: PXFieldState;
	ChildEntityType: PXFieldState;
	@linkCommand("FSBillHistory$ChildDocLink$Link")
	ChildDocLink: PXFieldState;
	ChildDocDesc: PXFieldState;
	ChildDocDate: PXFieldState;
	ChildDocStatus: PXFieldState;
}

export class ARPayment extends PXView {
	CreatePrepayment: PXActionState;
	ViewPayment: PXActionState;
	DocType: PXFieldState;
	@linkCommand("ViewPayment")
	RefNbr: PXFieldState;
	Status: PXFieldState;
	AdjDate: PXFieldState;
	ExtRefNbr: PXFieldState;
	PaymentMethodID: PXFieldState;
	CashAccountID: PXFieldState;
	CuryOrigDocAmt: PXFieldState;
	CurySOApplAmt: PXFieldState;
	CuryUnappliedBal: PXFieldState;
	CuryID: PXFieldState;
	FSAdjust__AdjdAppRefNbr: PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false
})
export class FSAppointmentAnswer extends PXView {
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}

export class FSProfitability extends PXView {
	LineRef: PXFieldState;
	LineType: PXFieldState;
	ItemID: PXFieldState;
	Descr: PXFieldState;
	EmployeeID: PXFieldState;
	CuryUnitPrice: PXFieldState;
	CuryUnitCost: PXFieldState;
	EstimatedQty: PXFieldState;
	CuryEstimatedAmount: PXFieldState;
	CuryEstimatedCost: PXFieldState;
	ActualDuration: PXFieldState;
	ActualQty: PXFieldState;
	CuryActualAmount: PXFieldState;
	CuryExtCost: PXFieldState;
	BillableQty: PXFieldState;
	CuryBillableAmount: PXFieldState;
	CuryProfit: PXFieldState;
	ProfitPercent: PXFieldState;
	ProfitMarginPercent: PXFieldState;
}

@gridConfig({
	allowImport: true,
	syncPosition: true
})
export class FSAppointmentLog extends PXView {
	DocType: PXFieldState;
	DocRefNbr: PXFieldState;
	LineRef: PXFieldState;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkGroupID: PXFieldState;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Travel: PXFieldState<PXFieldOptions.CommitChanges>;
	DetLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
	FSAppointmentDet__InventoryID: PXFieldState;
	Descr: PXFieldState;
	DateTimeBegin_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTimeBegin_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTimeEnd_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	DateTimeEnd_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	TimeDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	TrackOnService: PXFieldState<PXFieldOptions.CommitChanges>;
	TrackTime: PXFieldState<PXFieldOptions.CommitChanges>;
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	BillableTimeDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryBillableTranAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningType: PXFieldState<PXFieldOptions.CommitChanges>;
	LaborItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState;
	TimeCardCD: PXFieldState;
	ApprovedTime: PXFieldState;
	KeepDateTimes: PXFieldState;
}

export class FSAppointmentResource extends PXView {
	SrvOrdType: PXFieldState;
	RefNbr: PXFieldState;
	SMEquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	FSEquipment__Descr: PXFieldState;
	Comment: PXFieldState;
}

@gridConfig({
	syncPosition: true
})
export class FSAppointmentEmployee extends PXView {
	OpenStaffSelectorFromStaffTab: PXActionState;
	StartStaff: PXActionState;
	PauseStaff: PXActionState;
	ResumeStaff: PXActionState;
	CompleteStaff: PXActionState;
	DepartStaff: PXActionState;
	ArriveStaff: PXActionState;
	LineNbr: PXFieldState;
	LineRef: PXFieldState;
	@linkCommand("ViewEmployee")
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrimaryDriver: PXFieldState;
	ServiceLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
	FSAppointmentServiceEmployee__InventoryID: PXFieldState;
	FSAppointmentServiceEmployee__TranDesc: PXFieldState;
	TrackTime: PXFieldState;
	EarningType: PXFieldState<PXFieldOptions.CommitChanges>;
	LaborItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState;
	IsDriver: PXFieldState;
	Type: PXFieldState;
}

export class FSAppointmentTaxTran extends PXView {
	TaxID: PXFieldState;
	TaxRate: PXFieldState;
	CuryTaxableAmt: PXFieldState;
	TaxUOM: PXFieldState;
	TaxableQty: PXFieldState;
	CuryTaxAmt: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__PendingTax: PXFieldState;
	Tax__ReverseTax: PXFieldState;
	Tax__ExemptTax: PXFieldState;
	Tax__StatisticalTax: PXFieldState;
}

export class FSContact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Attention: PXFieldState;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.NoLabel>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSAddress extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1: PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine2: PXFieldState<PXFieldOptions.CommitChanges>;
	City: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}

export class FSServiceOrder extends PXView {
	SourceType: PXFieldState;
	@linkCommand("OpenSourceDocument")
	SourceReferenceNbr: PXFieldState<PXFieldOptions.Readonly>;
	SOPrepaymentReceived: PXFieldState;
	SOPrepaymentApplied: PXFieldState;
	SOPrepaymentRemaining: PXFieldState;
	SOCuryBillableUnpaidBalanace: PXFieldState;
	CuryDocTotal: PXFieldState;
	CuryEffectiveBillableDocTotal: PXFieldState;
	SOCuryUnpaidBalanace: PXFieldState;
	BillingBy: PXFieldState;
	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	Commissionable: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	RoomID: PXFieldState;
	CustPORefNbr: PXFieldState;
	CustWorkOrderRefNbr: PXFieldState;
	Severity: PXFieldState;
	Priority: PXFieldState;
	AssignedEmpID: PXFieldState;
	ProblemID: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillCustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowImport: true,
	syncPosition: true,
	statusField: "Availability"
})
export class FSAppointmentDetail extends PXView {
	ShowItems: PXActionState;
	FSAppointmentLineSplittingExtension_ShowSplits: PXActionState;
	OpenStaffSelectorFromServiceTab: PXActionState;
	AddReceipt: PXActionState;
	AddBill: PXActionState;
	Availability: PXFieldState<PXFieldOptions.Hidden>;
	StartItemLine: PXActionState;
	PauseItemLine: PXActionState;
	ResumeItemLine: PXActionState;
	CompleteItemLine: PXActionState;
	CancelItemLine: PXActionState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	LineRef: PXFieldState;
	LineNbr: PXFieldState;
	SODetID: PXFieldState<PXFieldOptions.CommitChanges>;
	UIStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	LineType: PXFieldState<PXFieldOptions.CommitChanges>;
	PickupDeliveryAppLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingRule: PXFieldState;
	TranDesc: PXFieldState;
	EquipmentAction: PXFieldState<PXFieldOptions.CommitChanges>;
	SMEquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewTargetEquipmentLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
	StaffID: PXFieldState<PXFieldOptions.CommitChanges>;
	Warranty: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimatedDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimatedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEstimatedTranAmt: PXFieldState;
	ActualDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	ActualQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryTranAmt: PXFieldState;
	IsFree: PXFieldState<PXFieldOptions.CommitChanges>;
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	BillableQty: PXFieldState;
	CuryBillableExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtCost: PXFieldState;
	DiscPct: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualDisc: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryBillableTranAmt: PXFieldState;
	TaxCategoryID: PXFieldState;
	ProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	CostCodeID: PXFieldState;
	AcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	CoveredQty: PXFieldState;
	ExtraUsageQty: PXFieldState;
	CuryExtraUsageUnitPrice: PXFieldState;
	EnablePO: PXFieldState<PXFieldOptions.CommitChanges>;
	POSource: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewPOVendor")
	POVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewPOVendorLocation")
	POVendorLocationID: PXFieldState;
	PONbr: PXFieldState;
	POStatus: PXFieldState;
	POCompleted: PXFieldState;
	IsPrepaid: PXFieldState;
	LinkedEntityType: PXFieldState;
	@linkCommand("viewLinkedDoc")
	LinkedDisplayRefNbr: PXFieldState;
	ContractRelated: PXFieldState;
	SubID: PXFieldState;
	ServiceType: PXFieldState;
	PickupDeliveryServiceID: PXFieldState<PXFieldOptions.CommitChanges>;
	Comment: PXFieldState;
}

export class FSAppointment extends PXView {
	CuryID: PXFieldState;
	LongDescr: PXFieldState;
	FullNameSignature: PXFieldState;
	RouteID: PXFieldState;
	RouteDocumentID: PXFieldState;
	MapLatitude: PXFieldState;
	MapLongitude: PXFieldState;
	GPSLatitudeStart: PXFieldState<PXFieldOptions.CommitChanges>;
	GPSLongitudeStart: PXFieldState<PXFieldOptions.CommitChanges>;
	GPSLatitudeComplete: PXFieldState<PXFieldOptions.CommitChanges>;
	GPSLongitudeComplete: PXFieldState<PXFieldOptions.CommitChanges>;
	GPSLatitudeLongitude: PXFieldState;
	ServiceContractID: PXFieldState;
	@linkCommand("OpenScheduleScreen")
	ScheduleID: PXFieldState;
	ProfitPercent: PXFieldState;
	ProfitMarginPercent: PXFieldState;
	CuryEstimatedLineTotal: PXFieldState;
	CuryEstimatedCostTotal: PXFieldState;
	CuryLineTotal: PXFieldState;
	CuryBillableLineTotal: PXFieldState;
	CuryLogBillableTranAmountTotal: PXFieldState;
	CuryTaxTotal: PXFieldState;
	CuryDocTotal: PXFieldState;
	CuryVatExemptTotal: PXFieldState;
	CuryVatTaxableTotal: PXFieldState;
	AppCompletedBillableTotal: PXFieldState;
	CuryCostTotal: PXFieldState;
	DeliveryNotes: PXFieldState;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	SrvOrdType: PXFieldState;
	RefNbr: PXFieldState;
	SORefNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	WFStageID: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeBegin_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeBegin_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeEnd_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDateTimeEnd_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	ExecutionDate: PXFieldState<PXFieldOptions.CommitChanges>;
	IsRouteAppoinment: PXFieldState;
	IsPrepaymentEnable: PXFieldState;
	DocDesc: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimatedDurationTotal: PXFieldState;
	ActualDurationTotal: PXFieldState;
	CuryActualBillableTotal: PXFieldState;
	TimeRegistered: PXFieldState<PXFieldOptions.CommitChanges>;
	WaitingForParts: PXFieldState;
	HandleManuallyScheduleTime: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduledDuration: PXFieldState;
	ROOptimizationStatus: PXFieldState;
	Confirmed: PXFieldState<PXFieldOptions.CommitChanges>;
	ValidatedByDispatcher: PXFieldState;
	ActualDateTimeBegin_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	ActualDateTimeEnd_Date: PXFieldState<PXFieldOptions.CommitChanges>;
	ActualDateTimeEnd_Time: PXFieldState<PXFieldOptions.CommitChanges>;
	HandleManuallyActualTime: PXFieldState<PXFieldOptions.CommitChanges>;
	ActualDuration: PXFieldState;
	Finished: PXFieldState<PXFieldOptions.CommitChanges>;
	UnreachedCustomer: PXFieldState<PXFieldOptions.CommitChanges>;
	BillServiceContractID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillContractPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	RecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleCuryRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleRecipRate: PXFieldState<PXFieldOptions.CommitChanges>;
}

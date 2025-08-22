import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	linkCommand,
	columnConfig,
	GridColumnShowHideMode,
	ICurrencyInfo,
	GridPagerMode
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.ServiceOrderEntry', primaryView: 'ServiceOrderRecords', showUDFIndicator: true })
export class FS300100 extends PXScreen {
	AddressLookup: PXActionState;
	QuickProcessOk: PXActionState;
	AddSelectedItems: PXActionState;

	//Commands
	Refresh: PXActionState;
	OpenSource: PXActionState;
	OpenScheduleScreen: PXActionState;
	FSServiceOrderLineSplittingExtension_GenerateNumbers: PXActionState;

	//LinkCommands
	ViewEmployee: PXActionState;
	OpenServiceOrderScreen: PXActionState;
	openPostBatch: PXActionState;
	FSBillHistory$ChildDocLink$Link: PXActionState;
	FSSODetSplit$RefNoteID$Link: PXActionState;
	ViewPOVendor: PXActionState;
	ViewPOVendorLocation: PXActionState;
	viewLinkedDoc: PXActionState;
	AddInvSelBySite: PXActionState;

	ServiceOrderRecords = createSingle(FSServiceOrder);
	CurrentServiceOrder = createSingle(FSServiceOrder);
	ServiceOrder_Contact = createSingle(FSContact);
	ServiceOrder_Address = createSingle(FSAddress);
	ServiceOrderDetails = createCollection(FSSODet);
	Taxes = createCollection(FSServiceOrderTaxTran);
	ServiceOrderAppointments = createCollection(FSAppointment);
	ProfitabilityRecords = createCollection(FSProfitability);
	ServiceOrderEmployees = createCollection(FSSOEmployee);
	ServiceOrderEquipment = createCollection(FSSOResource);
	Answers = createCollection(FSAttributeList);
	RelatedServiceOrders = createCollection(RelatedServiceOrder);
	Adjustments = createCollection(ARPayment);
	InvoiceRecords = createCollection(FSBillHistory);
	ScheduleRecord = createSingle(FSSchedule);
	QuickProcessParameters = createSingle(FSSrvOrdQuickProcessParams);
	StaffSelectorFilter = createSingle(StaffSelectionFilter);
	SkillGridFilter = createCollection(SkillGridFilter);
	LicenseTypeGridFilter = createCollection(LicenseTypeGridFilter);
	StaffRecords = createCollection(BAccountStaffMember);
	ServiceOrderTypeSelector = createSingle(SrvOrderTypeAux);
	ItemFilter = createSingle(FSSiteStatusFilter);
	ItemInfo = createCollection(FSSiteStatusLookup);
	Splits = createCollection(FSSODetSplit);
	FSServiceOrderLineSplittingExtension_LotSerOptions = createSingle(LotSerOptions);
	currencyinfo = createSingle(CurrencyInfo);
}

export class LotSerOptions extends PXView {
	GenerateNumbers: PXActionState;
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
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
	syncPosition: true,
	adjustPageSize: true,
	allowInsert: false,
	allowDelete: false
})
export class FSSiteStatusLookup extends PXView {
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
	Inventory: PXFieldState<PXFieldOptions.CommitChanges>;
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItem: PXFieldState<PXFieldOptions.CommitChanges>;
	Mode: PXFieldState<PXFieldOptions.CommitChanges>;
	OnlyAvailable: PXFieldState<PXFieldOptions.CommitChanges>;
	HistoryDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class SrvOrderTypeAux extends PXView {
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
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

export class StaffSelectionFilter extends PXView {
	ServiceLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode: PXFieldState;
	GeoZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
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

export class FSSrvOrdQuickProcessParams extends PXView {
	AllowInvoiceServiceOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	CompleteServiceOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	CloseServiceOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	GenerateInvoiceFromServiceOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepareInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	SOQuickProcess: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailSalesOrder: PXFieldState<PXFieldOptions.CommitChanges>;
	ReleaseInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	EmailInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
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
	CurySOAppltAmt: PXFieldState;
	CuryUnappliedBal: PXFieldState;
	CuryID: PXFieldState;
	FSAdjust__AdjdAppRefNbr: PXFieldState;
}

export class RelatedServiceOrder extends PXView {
	SrvOrdType: PXFieldState;
	RefNbr: PXFieldState;
	DocDesc: PXFieldState;
	Status: PXFieldState;
	OrderDate: PXFieldState;
	CuryID: PXFieldState;
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

export class FSSOResource extends PXView {
	SrvOrdType: PXFieldState;
	RefNbr: PXFieldState;
	SMEquipmentID: PXFieldState;
	FSEquipment__Descr: PXFieldState;
	Comment: PXFieldState;
}

export class FSSOEmployee extends PXView {
	OpenStaffSelectorFromStaffTab: PXActionState;
	@linkCommand("ViewEmployee")
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccount__AcctName: PXFieldState;
	Type: PXFieldState;
	ServiceLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
	FSSODetEmployee__InventoryID: PXFieldState;
	FSSODetEmployee_TranDesc: PXFieldState;
	Comment: PXFieldState;
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
	CuryCostTotal: PXFieldState;
	CuryProfit: PXFieldState;
	ProfitPercent: PXFieldState;
	ProfitMarginPercent: PXFieldState;
}

export class FSAppointment extends PXView {
	RefNbr: PXFieldState;
	Confirmed: PXFieldState;
	Status: PXFieldState;
	ScheduledDateTimeBegin_Date: PXFieldState;
	ScheduledDateTimeBegin_Time: PXFieldState;
	ScheduledDateTimeEnd_Date: PXFieldState;
	ScheduledDateTimeEnd_Time: PXFieldState;
	CuryBillableLineTotal: PXFieldState;
	CuryTaxTotal: PXFieldState;
	AppCompletedBillableTotal: PXFieldState;
	CuryCostTotal: PXFieldState;
}

export class FSServiceOrderTaxTran extends PXView {
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

@gridConfig({
	allowImport: true,
	syncPosition: true,
	statusField: "Availability"
})
export class FSSODet extends PXView {
	ShowItems: PXActionState;
	openStaffSelectorFromServiceTab: PXActionState;
	FSServiceOrderLineSplittingExtension_ShowSplits: PXActionState;
	AddReceipt: PXActionState;
	AddBill: PXActionState;
	Availability: PXFieldState<PXFieldOptions.Hidden>;
	SrvOrdType: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	RefNbr: PXFieldState;
	LineRef: PXFieldState;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	LineType: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingRule: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentAction: PXFieldState<PXFieldOptions.CommitChanges>;
	SMEquipmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	NewTargetEquipmentLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentLineRef: PXFieldState<PXFieldOptions.CommitChanges>;
	StaffID: PXFieldState<PXFieldOptions.CommitChanges>;
	Warranty: PXFieldState;
	IsPrepaid: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	SiteLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	LotSerialNbr: PXFieldState;
	UOM: PXFieldState;
	EstimatedDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	EstimatedQty: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEstimatedTranAmt: PXFieldState;
	ContractRelated: PXFieldState;
	CoveredQty: PXFieldState;
	ExtraUsageQty: PXFieldState;
	CuryExtraUsageUnitPrice: PXFieldState;
	ApptEstimatedDuration: PXFieldState;
	ApptDuration: PXFieldState;
	ApptQty: PXFieldState;
	CuryApptTranAmt: PXFieldState;
	ApptCntr: PXFieldState;
	IsFree: PXFieldState<PXFieldOptions.CommitChanges>;
	IsBillable: PXFieldState<PXFieldOptions.CommitChanges>;
	BillableQty: PXFieldState;
	CuryBillableExtPrice: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtCost: PXFieldState<PXFieldOptions.CommitChanges>;
	DiscPct: PXFieldState<PXFieldOptions.CommitChanges>;
	ManualDisc: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryDiscAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryBillableTranAmt: PXFieldState;
	EnablePO: PXFieldState<PXFieldOptions.CommitChanges>;
	POSource: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewPOVendor") POVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	@linkCommand("ViewPOVendorLocation") POVendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	PONbr: PXFieldState;
	POStatus: PXFieldState;
	POCompleted: PXFieldState;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectTaskID: PXFieldState;
	CostCodeID: PXFieldState;
	AcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubID: PXFieldState;
	Mem_LastReferencedBy: PXFieldState;
	Comment: PXFieldState;
	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
	LinkedEntityType: PXFieldState;
	@linkCommand("viewLinkedDoc") LinkedDisplayRefNbr: PXFieldState;
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

export class FSContact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Attention: PXFieldState;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.NoLabel>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class FSServiceOrder extends PXView {
	RoomID: PXFieldState;
	ContactID: PXFieldState<PXFieldOptions.CommitChanges>;
	SrvOrdType: PXFieldState;
	RefNbr: PXFieldState;
	Status: PXFieldState;
	WFStageID: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CustPORefNbr: PXFieldState;
	CustWorkOrderRefNbr: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillServiceContractID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillContractPeriodID: PXFieldState;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltProjectTaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	DocDesc: PXFieldState;
	EstimatedDurationTotal: PXFieldState;
	CuryEstimatedBillableTotal: PXFieldState;
	CuryTaxTotal: PXFieldState;
	CuryDocTotal: PXFieldState;
	CuryEffectiveBillableDocTotal: PXFieldState;
	WaitingForParts: PXFieldState;
	AppointmentsNeeded: PXFieldState;
	IsPrepaymentEnable: PXFieldState;
	SLAETA_Date: PXFieldState;
	SLAETA_Time: PXFieldState;
	Severity: PXFieldState;
	Priority: PXFieldState;
	AssignedEmpID: PXFieldState;
	ProblemID: PXFieldState;
	BillCustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	BillLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState<PXFieldOptions.CommitChanges>;
	BillingBy: PXFieldState;
	SalesPersonID: PXFieldState<PXFieldOptions.CommitChanges>;
	Commissionable: PXFieldState;
	CuryEstimatedOrderTotal: PXFieldState;
	CuryBillableOrderTotal: PXFieldState;
	CuryVatExemptTotal: PXFieldState;
	CuryVatTaxableTotal: PXFieldState;
	CuryEffectiveCostTotal: PXFieldState;
	ApptDurationTotal: PXFieldState;
	CuryApptOrderTotal: PXFieldState;
	CuryAppointmentTaxTotal: PXFieldState;
	CuryAppointmentDocTotal: PXFieldState;
	ProfitPercent: PXFieldState;
	ProfitMarginPercent: PXFieldState;
	SOPrepaymentReceived: PXFieldState;
	SOPrepaymentApplied: PXFieldState;
	SOPrepaymentRemaining: PXFieldState;
	SOCuryUnpaidBalanace: PXFieldState;
	SOCuryBillableUnpaidBalanace: PXFieldState;
	SourceType: PXFieldState;
	@linkCommand("OpenSource") SourceReferenceNbr: PXFieldState<PXFieldOptions.Readonly>;
	ServiceContractID: PXFieldState;
	@linkCommand("OpenScheduleScreen") ScheduleID: PXFieldState;
	AllowInvoice: PXFieldState;
	Mem_Invoiced: PXFieldState;
	LongDescr: PXFieldState;
}

export class FSSODetSplit extends PXView {
	SplitLineNbr: PXFieldState;
	ParentSplitLineNbr: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	ShipDate: PXFieldState;
	IsAllocated: PXFieldState;
	SiteID: PXFieldState;
	Completed: PXFieldState;
	LocationID: PXFieldState;
	LotSerialNbr: PXFieldState;
	Qty: PXFieldState;
	ShippedQty: PXFieldState;
	ReceivedQty: PXFieldState;
	UOM: PXFieldState;
	ExpireDate: PXFieldState;
	POCreate: PXFieldState;
	@linkCommand("FSSODetSplit$RefNoteID$Link")
	RefNoteID: PXFieldState;
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

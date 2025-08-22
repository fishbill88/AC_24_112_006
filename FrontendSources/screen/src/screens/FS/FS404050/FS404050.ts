import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.CustomerLocationInq', primaryView: 'LocationRecords' })
export class FS404050 extends PXScreen {
	OpenRouteServiceContract: PXActionState;
	OpenRouteSchedule: PXActionState;
	OpenAppointmentByPickUpDeliveryItem: PXActionState;
	OpenAppointmentByService: PXActionState;
	OpenDocumentByPickUpDeliveryItem: PXActionState;
	OpenDocumentByService: PXActionState;
	LocationRecords = createSingle(BAccountLocation);
	LocationSelected = createSingle(Location);
	CustomerRecords = createSingle(Customer);
	PriceClass1 = createSingle(ARPriceClass);
	RouteContractSchedules = createCollection(FSRouteContractScheduleFSServiceContract);
	PickUpDeliveryItems = createCollection(FSAppointmentDet1);
	Services = createCollection(FSAppointmentDet2);
}

export class FSRouteContractScheduleFSServiceContract extends PXView {
	ServiceContractRefNbr: PXFieldState;
	FSServiceContract__CustomerContractNbr: PXFieldState;
	FSServiceContract__Status: PXFieldState;
	RefNbr: PXFieldState;
	Active: PXFieldState;
	FSScheduleRoute__GlobalSequence: PXFieldState;
	FSRoute__RouteCD: PXFieldState;
	FSRoute__RouteShort: PXFieldState;
	WeekCode: PXFieldState;
	RecurrenceDescription: PXFieldState;
	LastGeneratedElementDate: PXFieldState;
}

export class Customer extends PXView {
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	Status: PXFieldState;
}

export class ARPriceClass extends PXView {
	Description: PXFieldState;
}

export class Location extends PXView {
	Descr: PXFieldState<PXFieldOptions.Readonly>;
	IsActive: PXFieldState<PXFieldOptions.Readonly>;
	Contact__FullName: PXFieldState;
	Contact__Attention: PXFieldState;
	Contact__EMail: PXFieldState;
	Contact__Website: PXFieldState;
	Contact__Phone1: PXFieldState;
	Contact__Phone2: PXFieldState;
	Contact__Fax: PXFieldState;
	Address__AddressLine1: PXFieldState;
	Address__AddressLine2: PXFieldState;
	Address__City: PXFieldState;
	Address__CountryID: PXFieldState;
	Address__State: PXFieldState;
	Address__PostalCode: PXFieldState;
	Address__IsValidated: PXFieldState;
	FSBillingCycle__BillingCycleCD: PXFieldState;
	FSBillingCycle__Descr: PXFieldState;
	TaxRegistrationID: PXFieldState;
	CTaxZoneID: PXFieldState;
	CAvalaraExemptionNumber: PXFieldState;
	CAvalaraCustomerUsageType: PXFieldState;
	CBranchID: PXFieldState;
	CPriceClassID: PXFieldState;
	CDefProjectID: PXFieldState;
}

export class BAccountLocation extends PXView {
	CustomerID: PXFieldState;
	LocationID: PXFieldState;
}

export class FSAppointmentDet1 extends PXView {
	FSAppointment__RefNbr: PXFieldState;
	FSAppointment__Status: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
	TranDesc: PXFieldState;
	SiteID: PXFieldState;
	UOM: PXFieldState;
	Qty: PXFieldState;
	UnitPrice: PXFieldState;
	TranAmt: PXFieldState;
	ProjectTaskID: PXFieldState;
	FSPostBatch__BatchNbr: PXFieldState;
	FSPostDet__Mem_PostedIn: PXFieldState;
	FSPostDet__Mem_DocType: PXFieldState;
	FSPostDet__Mem_DocNbr: PXFieldState;
	SODetID: PXFieldState;
	ServiceType: PXFieldState;
	PickupDeliveryServiceID: PXFieldState;
}

export class FSAppointmentDet2 extends PXView {
	FSAppointment__RefNbr: PXFieldState;
	FSAppointment__Status: PXFieldState;
	FSAppointment__ExecutionDate: PXFieldState;
	Status: PXFieldState;
	LineType: PXFieldState;
	InventoryID: PXFieldState;
	FSSODet__BillingRule: PXFieldState;
	TranDesc: PXFieldState;
	IsBillable: PXFieldState;
	EstimatedDuration: PXFieldState;
	ActualDuration: PXFieldState;
	Qty: PXFieldState;
	UnitPrice: PXFieldState;
	TranAmt: PXFieldState;
	ProjectTaskID: PXFieldState;
	FSPostBatch__BatchNbr: PXFieldState;
	FSPostDet__Mem_PostedIn: PXFieldState;
	FSPostDet__Mem_DocType: PXFieldState;
	FSPostDet__Mem_DocNbr: PXFieldState;
	SODetID: PXFieldState;
}

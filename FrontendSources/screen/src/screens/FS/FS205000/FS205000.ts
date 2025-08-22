import {
	graphInfo,
	createSingle,
	createCollection,
	linkCommand,
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,
	PXActionState,
	gridConfig
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.SMEquipmentMaint', primaryView: 'EquipmentRecords', showUDFIndicator: true })
export class FS205000 extends PXScreen {
	OpenSource: PXActionState;
	EquipmentRecords = createSingle(FSEquipment);
	EquipmentSelected = createSingle(FSEquipmentSelected);
	EquipmentWarranties = createCollection(FSEquipmentComponent);
	Answers = createCollection(CSAnswers);
	ComponentSelected = createCollection(FSEquipmentComponentPanel);
	ReplaceComponentInfo = createSingle(RComponent);
}

export class FSEquipment extends PXView {
	RefNbr: PXFieldState;
	EquipmentTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	SerialNumber: PXFieldState;
	Descr: PXFieldState;
	OwnerType: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsVehicle: PXFieldState;
	RequireMaintenance: PXFieldState<PXFieldOptions.CommitChanges>;
	ResourceEquipment: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationType: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ImageUrl: PXFieldState;
}

export class FSEquipmentSelected extends PXView {
	RegisteredDate: PXFieldState;
	RegistrationNbr: PXFieldState;
	Barcode: PXFieldState;
	TagNbr: PXFieldState;
	SalesDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Color: PXFieldState;
	DateInstalled: PXFieldState<PXFieldOptions.CommitChanges>;
	InstServiceOrderRefNbr: PXFieldState;
	InstAppointmentRefNbr: PXFieldState;
	DisposalDate: PXFieldState;
	ReplaceEquipmentID: PXFieldState;
	DispServiceOrderRefNbr: PXFieldState;
	DispAppointmentRefNbr: PXFieldState;
	ManufacturerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ManufacturerModelID: PXFieldState;
	ManufacturingYear: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState;
	INSerialNumber: PXFieldState;
	PropertyType: PXFieldState;
	VendorID: PXFieldState;
	PurchDate: PXFieldState;
	PurchPONumber: PXFieldState;
	PurchAmount: PXFieldState;
	CpnyWarrantyValue: PXFieldState<PXFieldOptions.CommitChanges>;
	CpnyWarrantyType: PXFieldState<PXFieldOptions.CommitChanges>;
	CpnyWarrantyEndDate: PXFieldState;
	VendorWarrantyValue: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorWarrantyType: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorWarrantyEndDate: PXFieldState;
	SourceType: PXFieldState;
	@linkCommand("OpenSource") SourceRefNbr: PXFieldState;
	SalesOrderNbr: PXFieldState;
	EquipmentReplacedID: PXFieldState;
	VehicleTypeID: PXFieldState;
	EngineNo: PXFieldState;
	Axles: PXFieldState;
	MaxMiles: PXFieldState;
	TareWeight: PXFieldState;
	WeightCapacity: PXFieldState;
	GrossVehicleWeight: PXFieldState;
	FuelType: PXFieldState;
	FuelTank1: PXFieldState;
	FuelTank2: PXFieldState;
}

export class FSEquipmentComponent extends PXView {
	ReplaceComponent: PXActionState;
	LineRef: PXFieldState;
	ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	LongDescr: PXFieldState;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SerialNumber: PXFieldState;
	CpnyWarrantyDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	CpnyWarrantyType: PXFieldState<PXFieldOptions.CommitChanges>;
	CpnyWarrantyEndDate: PXFieldState;
	VendorWarrantyDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorWarrantyType: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorWarrantyEndDate: PXFieldState;
	VendorID: PXFieldState;
	Comment: PXFieldState;
	InstServiceOrderRefNbr: PXFieldState;
	InstAppointmentRefNbr: PXFieldState;
	ComponentReplaced: PXFieldState;
	InstallationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceRefNbr: PXFieldState;
	SalesDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesOrderNbr: PXFieldState;
}

export class FSEquipmentComponentPanel extends PXView {
	LineRef: PXFieldState;
	ComponentID: PXFieldState;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	LongDescr: PXFieldState;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SerialNumber: PXFieldState;
	CpnyWarrantyDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	CpnyWarrantyType: PXFieldState<PXFieldOptions.CommitChanges>;
	CpnyWarrantyEndDate: PXFieldState;
	VendorWarrantyDuration: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorWarrantyType: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorWarrantyEndDate: PXFieldState;
	VendorID: PXFieldState;
	Comment: PXFieldState;
	InstServiceOrderRefNbr: PXFieldState;
	InstAppointmentRefNbr: PXFieldState;
	ComponentReplaced: PXFieldState;
	InstallationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceRefNbr: PXFieldState;
	SalesDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesOrderNbr: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class CSAnswers extends PXView {
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	Value: PXFieldState;
}

export class RComponent extends PXView {
	InstallationDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesDate: PXFieldState<PXFieldOptions.CommitChanges>;
	ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

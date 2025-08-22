import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
	localizable,
} from 'client-controls';

@localizable
export class NullTextValues {
	static Split = "<SPLIT>";
}

@graphInfo({ graphType: 'PX.Objects.AM.VendorShipmentEntry', primaryView: 'Document', showUDFIndicator: true })
export class AM310000 extends PXScreen {
	AMVendorShipmentLineSplittingExtension_GenerateNumbers: PXActionState;
	AddShipLines: PXActionState;
	AddShipLinesClose: PXActionState;

	Document = createSingle(AMVendorShipment);
	Transactions = createCollection(AMVendorShipLine);
	ShippingContact = createSingle(AMVendorShipmentContact);
	ShippingAddress = createSingle(AMVendorShipmentAddress);
	CurrentDocument = createSingle(AMVendorShipment2);
	AMVendorShipmentLineSplittingExtension_LotSerOptions = createSingle(LotSerOptions);
	Splits = createCollection(AMVendorShipLineSplit);
	ShipmentProdOrders = createCollection(AMProdOper);
}

export class AMVendorShipment extends PXView {
	ShipmentNbr: PXFieldState;
	ShipmentType: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState;
	ShipmentDate: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	EmployeeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipmentQty: PXFieldState<PXFieldOptions.Disabled>;
	ControlQty: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class AMVendorShipLine extends PXView {
	AMVendorShipmentLineSplittingExtension_ShowSplits: PXActionState;
	AddProductionOrders: PXActionState;

	LineNbr: PXFieldState;
	LineType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	ProdOrdID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	MatlLineID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true, nullText: NullTextValues.Split }) LotSerialNbr: PXFieldState;
	ExpireDate: PXFieldState;
	InventoryID_description: PXFieldState;
	TranDesc: PXFieldState;
	POOrderNbr: PXFieldState;
	POLineNbr: PXFieldState;
	Released: PXFieldState;
}

export class AMVendorShipmentContact extends PXView {
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState;
	Attention: PXFieldState;
	Phone1: PXFieldState;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AMVendorShipmentAddress extends PXView {
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AMVendorShipment2 extends PXView {
	ShipVia: PXFieldState<PXFieldOptions.CommitChanges>;
	FOBPoint: PXFieldState;
	ShipTermsID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	Residential: PXFieldState;
	SaturdayDelivery: PXFieldState;
	Insurance: PXFieldState;
	GroundCollect: PXFieldState;
	CuryFreightCost: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideFreightAmount: PXFieldState<PXFieldOptions.CommitChanges>;
	FreightAmountSource: PXFieldState;
	CuryFreightAmt: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class LotSerOptions extends PXView {
	UnassignedQty: PXFieldState;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
})
export class AMVendorShipLineSplit extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true }) LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	UOM: PXFieldState;
	@columnConfig({ width: 200 }) ExpireDate: PXFieldState;
	InventoryID_InventoryItem_descr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: false,
})
export class AMProdOper extends PXView {
	Selected: PXFieldState;
	AMProdItem__OrderType: PXFieldState;
	AMProdItem__ProdOrdID: PXFieldState;
	OperationCD: PXFieldState;
	AMProdItem__InventoryID: PXFieldState;
	AMProdItem__SubItemID: PXFieldState;
	AMProdItem__SiteID: PXFieldState;
	AMProdItem__UOM: PXFieldState;
	AMProdItem__Descr: PXFieldState;
	QtytoProd: PXFieldState;
	ShippedQuantity: PXFieldState;
	ShipRemainingQty: PXFieldState;
}

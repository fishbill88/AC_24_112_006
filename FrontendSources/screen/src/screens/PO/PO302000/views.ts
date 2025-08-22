import {
	PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, PXFieldOptions,
	columnConfig, PXActionState, GridPreset, GridColumnShowHideMode } from "client-controls";

// Views

export class POReceiptHeader extends PXView {
	ReceiptType: PXFieldState;
	ReceiptNbr: PXFieldState;
	Status: PXFieldState<PXFieldOptions.Disabled>;
	ReceiptDate: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState;
	@headerDescription
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoCreateInvoice: PXFieldState<PXFieldOptions.CommitChanges>;
	ReturnInventoryCostMode: PXFieldState<PXFieldOptions.CommitChanges>;
	InvoiceNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	WorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	OwnerID: PXFieldState;
	OrderQty: PXFieldState;
	ControlQty: PXFieldState;
	UnbilledQty: PXFieldState;
	CuryOrderTotal: PXFieldState;

	ShowPurchaseOrdersTab: PXFieldState;
	ShowPutAwayHistoryTab: PXFieldState;
	ShowLandedCostsTab: PXFieldState;
}

export class POReceipt extends PXView {
	InventoryRefNbr: PXFieldState<PXFieldOptions.Disabled>;
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchBaseCuryID: PXFieldState;
	InvoiceDate: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderNbr: PXFieldState<PXFieldOptions.Disabled>;
	IntercompanyShipmentNbr: PXFieldState<PXFieldOptions.Disabled>;
	IntercompanySOType: PXFieldState;
	IntercompanySONbr: PXFieldState<PXFieldOptions.Disabled>;
	ExcludeFromIntercompanyProc: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	adjustPageSize: true,
	statusField: 'Availability'
})
export class POReceiptLine extends PXView {
	POReceiptLineSplittingExtension_ShowSplits: PXActionState;
	AddPOReceiptLine: PXActionState;
	AddPOOrder: PXActionState;
	AddPOOrderLine: PXActionState;
	AddPOReceiptReturn: PXActionState;
	AddPOReceiptLineReturn: PXActionState;
	AddTransfer: PXActionState;
	ViewPOOrder: PXActionState;

	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Availability: PXFieldState;
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	BranchID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	LineType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true, nullText: '<SPLIT>' })
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true, nullText: '<SPLIT>' })
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	TranDesc: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	OrigOrderQty: PXFieldState;
	OpenOrderQty: PXFieldState;
	ReceiptQty: PXFieldState<PXFieldOptions.CommitChanges>;
	BaseReceiptQty: PXFieldState;
	UnassignedQty: PXFieldState;
	OrigReceiptNbr: PXFieldState;
	OrigReceiptLineNbr: PXFieldState;
	ExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAcctID_Account_description: PXFieldState;
	ExpenseSubID: PXFieldState;
	POAccrualAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualSubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	TaskID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	CostCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SpecialOrderCostCenterID: PXFieldState;
	ExpireDate: PXFieldState;
	LineNbr: PXFieldState;
	SortOrder: PXFieldState;
	@columnConfig({ hideViewLink: true, nullText: '<SPLIT>' })
	LotSerialNbr: PXFieldState;
	POType: PXFieldState;
	PONbr: PXFieldState;
	POLineNbr: PXFieldState;
	SOOrderType: PXFieldState;
	SOOrderNbr: PXFieldState;
	SOOrderLineNbr: PXFieldState;
	SOShipmentNbr: PXFieldState;
	AllowComplete: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowOpen: PXFieldState<PXFieldOptions.CommitChanges>;
	ReturnedQty: PXFieldState;
	ReasonCode: PXFieldState<PXFieldOptions.CommitChanges>;
	AllowEditUnitCost: PXFieldState;
	CuryUnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryExtCost: PXFieldState<PXFieldOptions.CommitChanges>;
	TranCost: PXFieldState;
	TranCostFinal: PXFieldState;
}

export class LineSplittingHeader extends PXView {
	UnassignedQty: PXFieldState<PXFieldOptions.Disabled>;
	Qty: PXFieldState;
	StartNumVal: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class POReceiptLineSplit extends PXView {
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LocationID: PXFieldState;
	Qty: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LotSerialNbr: PXFieldState;
	ExpireDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true
})
export class POOrderReceipt extends PXView {
	POType: PXFieldState;
	PONbr: PXFieldState;
	CuryID: PXFieldState;
	TaxZoneID: PXFieldState;
	TaxCalcMode: PXFieldState;
	PayToVendorID: PXFieldState;
	Status: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true
})
export class INRegister extends PXView {
	RefNbr: PXFieldState;
	Status: PXFieldState;
	TransferType: PXFieldState;
	TranDate: PXFieldState;
	FinPeriodID: PXFieldState;
	SiteID: PXFieldState;
	TotalQty: PXFieldState;
	BatchNbr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true
})
export class POReceiptPOOriginal extends PXView {
	ReceiptType: PXFieldState;
	ReceiptNbr: PXFieldState;
	DocDate: PXFieldState;
	FinPeriodID: PXFieldState;
	Status: PXFieldState;
	TotalQty: PXFieldState;
	InvtDocType: PXFieldState;
	InvtRefNbr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true,
	statusField: 'StatusText'
})
export class POReceiptAPDoc extends PXView {
	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	StatusText: PXFieldState;
	DocType: PXFieldState;
	RefNbr: PXFieldState;
	DocDate: PXFieldState;
	Status: PXFieldState;
	TotalQty: PXFieldState;
	TotalAmt: PXFieldState;
	AccruedQty: PXFieldState;
	AccruedAmt: PXFieldState;
	TotalPPVAmt: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true
})
export class POLandedCostReceipt extends PXView {
	LCDocType: PXFieldState;
	LCRefNbr: PXFieldState;
	DocDate: PXFieldState;
	Status: PXFieldState;
	@columnConfig({ hideViewLink: true })
	VendorID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	LandedCostCodeID: PXFieldState;
	CuryLineAmt: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	APRefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	INRefNbr: PXFieldState;
}

export class POReceiptLineS extends PXView {
	BarCode: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerialNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpireDate: PXFieldState;
	ReceiptQty: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID: PXFieldState;
	POType: PXFieldState<PXFieldOptions.CommitChanges>;
	PONbr: PXFieldState<PXFieldOptions.Disabled>;
	POLineNbr: PXFieldState<PXFieldOptions.Disabled>;
	ShipFromSiteID: PXFieldState<PXFieldOptions.Disabled>;
	SOOrderType: PXFieldState<PXFieldOptions.Disabled>;
	SOOrderNbr: PXFieldState<PXFieldOptions.Disabled>;
	SOOrderLineNbr: PXFieldState<PXFieldOptions.Disabled>;
	SOShipmentNbr: PXFieldState<PXFieldOptions.Disabled>;
	UnitCost: PXFieldState<PXFieldOptions.CommitChanges>;
	ByOne: PXFieldState<PXFieldOptions.CommitChanges>;
	AutoAddLine: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
}

export class POOrderFilter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipToBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipToLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ShipFromSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	SOOrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class POLine extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OrderNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	VendorID: PXFieldState;
	LineType: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	OrderQty: PXFieldState;
	ReceivedQty: PXFieldState;
	LeftToReceiveQty: PXFieldState;
	TranDesc: PXFieldState;
	PromisedDate: PXFieldState;
	RcptQtyMin: PXFieldState;
	RcptQtyMax: PXFieldState;
	RcptQtyAction: PXFieldState;
	LineNbr: PXFieldState<PXFieldOptions.Hidden>;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class POOrder extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	OrderType: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OrderNbr: PXFieldState;
	OrderDate: PXFieldState;
	Status: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	CuryOrderTotal: PXFieldState;
	VendorRefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	TermsID: PXFieldState;
	OrderDesc: PXFieldState;
	ReceivedQty: PXFieldState;
	LeftToReceiveQty: PXFieldState;
}

export class POReceiptReturnFilter extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
	OrderNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	ReceiptNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class POReceiptReturn extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ReceiptNbr: PXFieldState;
	ReceiptType: PXFieldState;
	@columnConfig({ hideViewLink: true })
	VendorID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	VendorLocationID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	ReceiptDate: PXFieldState;
	OrderQty: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class POReceiptLineReturn extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PONbr: PXFieldState;
	POType: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ReceiptNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	InvoiceNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	ReceiptQty: PXFieldState;
	ReturnedQty: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class SOOrderShipment extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OrderType: PXFieldState;
	@columnConfig({ hideViewLink: true })
	OrderNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ShipmentNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	INRegister__SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	INRegister__ToSiteID: PXFieldState;
	INRegister__TranDate: PXFieldState;
	SOOrder__OrderDesc: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry
})
export class INTran extends PXView {
	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	@columnConfig({ hideViewLink: true })
	RefNbr: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	Qty: PXFieldState;
	TranDesc: PXFieldState;
	LineNbr: PXFieldState;
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

import { PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState, TextAlign, GridPreset } from "client-controls";

export class RQRequisition extends PXView  {
	RecalculatePricesAction: PXActionState;

	ReqNbr : PXFieldState;
	Status : PXFieldState<PXFieldOptions.Disabled>;
	OrderDate : PXFieldState;
	Quoted : PXFieldState<PXFieldOptions.CommitChanges>;
	Description : PXFieldState;
	Priority : PXFieldState;
	EmployeeID : PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerID : PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID : PXFieldState;
	CuryEstExtCostTotal : PXFieldState<PXFieldOptions.Disabled>;
}

export class RQRequisition2 extends PXView  {
	ShipDestType : PXFieldState<PXFieldOptions.CommitChanges>;
	ShipToBAccountID : PXFieldState<PXFieldOptions.CommitChanges>;
	SiteID : PXFieldState<PXFieldOptions.CommitChanges>;
	ShipToLocationID : PXFieldState;
	FOBPoint : PXFieldState;
	ShipVia : PXFieldState;
	VendorID : PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID : PXFieldState<PXFieldOptions.CommitChanges>;
	VendorRefNbr : PXFieldState;
	TermsID : PXFieldState<PXFieldOptions.CommitChanges>;
	POType : PXFieldState<PXFieldOptions.CommitChanges>;
	Splittable : PXFieldState;
	WorkgroupID : PXFieldState;
	OwnerID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true,
	statusField: "Availability"
})
export class RQRequisitionLine extends PXView  {
	viewDetails : PXActionState;
	ShowItems : PXActionState;
	addRequestLine : PXActionState;
	transfer : PXActionState;
	merge : PXActionState;
	viewLineDetails : PXActionState;

	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Availability : PXFieldState;
	@columnConfig({allowCheckAll: true}) Selected : PXFieldState;
	@columnConfig({visible: false})	LineNbr : PXFieldState<PXFieldOptions.Hidden>;
	InventoryID : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true}) SubItemID : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowUpdate: false, allowNull: false})	LineSource : PXFieldState;
	@columnConfig({allowNull: false}) LineType : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true}) SiteID : PXFieldState<PXFieldOptions.CommitChanges>;
	Description : PXFieldState;
	@columnConfig({hideViewLink: true}) UOM : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowNull: false}) OrderQty : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowNull: false}) CuryEstUnitCost : PXFieldState<PXFieldOptions.CommitChanges>;
	ManualPrice : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowNull: false}) CuryEstExtCost : PXFieldState;
	@columnConfig({hideViewLink: true}) ExpenseAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({hideViewLink: true}) ExpenseSubID : PXFieldState;
	AlternateID : PXFieldState;
	IsUseMarkup : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({allowNull: false, textAlign: TextAlign.Right})	MarkupPct : PXFieldState;
	RcptQtyMin : PXFieldState;
	RcptQtyMax : PXFieldState;
	RcptQtyThreshold : PXFieldState;
	RcptQtyAction : PXFieldState;
	RequestedDate : PXFieldState;
	PromisedDate : PXFieldState;
	Cancelled : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class POContact2 extends PXView  {
	OverrideContact : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	FullName : PXFieldState;
	Attention : PXFieldState;
	Phone1 : PXFieldState;
	Email : PXFieldState;
}

export class POAddress2 extends PXView  {
	OverrideAddress : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.CommitChanges>;
	AddressLine1 : PXFieldState;
	AddressLine2 : PXFieldState;
	City : PXFieldState;
	CountryID : PXFieldState<PXFieldOptions.CommitChanges>;
	State : PXFieldState;
	PostalCode : PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class RQBiddingVendor extends PXView  {
	vendorInfo : PXActionState;
	responseVendor : PXActionState;
	chooseVendor : PXActionState;
	sendRequestToCurrentVendor : PXActionState;

	VendorID : PXFieldState<PXFieldOptions.CommitChanges>;
	VendorID_Vendor_AcctName : PXFieldState;
	VendorLocationID : PXFieldState<PXFieldOptions.CommitChanges>;
	VendorLocationID_Location_Descr : PXFieldState;
	@columnConfig({textField: "CuryID"})
	CuryInfoID : PXFieldState;
	Location__VShipTermsID : PXFieldState;
	FOBPoint : PXFieldState;
	Location__VLeadTime : PXFieldState;
	ShipVia : PXFieldState;
	ExpireDate : PXFieldState;
	PromisedDate : PXFieldState;
	Status : PXFieldState;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.False}) RemitContactID : PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({visible: false, allowShowHide: GridColumnShowHideMode.False}) RemitAddressID : PXFieldState<PXFieldOptions.Hidden>;
}

export class POContact3 extends PXView  {
	OverrideContact : PXFieldState<PXFieldOptions.CommitChanges>;
	FullName : PXFieldState;
	Attention : PXFieldState;
	Phone1 : PXFieldState;
	Email : PXFieldState;
}

export class POAddress3 extends PXView  {
	OverrideAddress : PXFieldState<PXFieldOptions.CommitChanges>;
	AddressLine1 : PXFieldState;
	AddressLine2 : PXFieldState;
	City : PXFieldState;
	CountryID : PXFieldState<PXFieldOptions.CommitChanges>;
	State : PXFieldState<PXFieldOptions.CommitChanges>;
	PostalCode : PXFieldState<PXFieldOptions.CommitChanges>;
	IsValidated : PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class POOrder extends PXView  {
	viewPOOrder: PXActionState;
	CreatePOOrder : PXActionState;

	@columnConfig({hideViewLink: true}) OrderType : PXFieldState;
	@linkCommand("viewPOOrder")
	OrderNbr : PXFieldState;
	@columnConfig({allowNull: false})	Status : PXFieldState;
	OrderDate : PXFieldState;
	@columnConfig({hideViewLink: true}) VendorID : PXFieldState;
	@columnConfig({hideViewLink: true}) VendorLocationID : PXFieldState;
	VendorRefNbr : PXFieldState;
	@columnConfig({hideViewLink: true}) CuryID : PXFieldState;
	@columnConfig({allowNull: false})	CuryLineTotal : PXFieldState;
	@columnConfig({allowNull: false})	CuryTaxTotal : PXFieldState;
	@columnConfig({allowNull: false})	CuryOrderTotal : PXFieldState;
	@columnConfig({hideViewLink: true,  textAlign: TextAlign.Left}) OwnerId : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class SOOrder extends PXView  {
	ViewSOOrder : PXActionState;
	CreateQTOrder : PXActionState;

	@columnConfig({allowNull: false, hideViewLink: true})	OrderType : PXFieldState;
	@linkCommand("viewSOOrder")
	OrderNbr : PXFieldState;
	OrderDate : PXFieldState;
	Status : PXFieldState;
	@columnConfig({hideViewLink: true}) CustomerID : PXFieldState;
	@columnConfig({hideViewLink: true}) CustomerLocationID : PXFieldState;
	@columnConfig({hideViewLink: true}) CuryID : PXFieldState;
	CuryLineTotal : PXFieldState;
	CuryTaxTotal : PXFieldState;
	CuryOrderTotal : PXFieldState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID : PXFieldState;
	BaseCuryID : PXFieldState;
	BaseCalc : PXFieldState;
	DisplayCuryID : PXFieldState;
	CuryRateTypeID : PXFieldState;
	BasePrecision : PXFieldState;
	CuryRate : PXFieldState;
	CuryEffDate : PXFieldState;
	RecipRate : PXFieldState;
	SampleCuryRate : PXFieldState;
	SampleRecipRate : PXFieldState;
	CuryID : PXFieldState;
}

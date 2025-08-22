import { PXView, PXFieldState, gridConfig, PXFieldOptions, linkCommand, columnConfig, PXActionState, GridColumnShowHideMode, GridPreset } from "client-controls";

export class ChangeIDParam extends PXView  {
	CD : PXFieldState;
}

export class InventoryItem extends PXView  {
	InventoryCD : PXFieldState;
	Descr : PXFieldState<PXFieldOptions.CommitChanges>;
	StkItem : PXFieldState<PXFieldOptions.CommitChanges>;
}

export class InventoryItem2 extends PXView  {
	ItemStatus : PXFieldState;
	ItemClassID : PXFieldState<PXFieldOptions.CommitChanges>;
	ItemType : PXFieldState;
	ValMethod : PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID : PXFieldState<PXFieldOptions.CommitChanges>;
	PostClassID : PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerClassID : PXFieldState<PXFieldOptions.CommitChanges>;
	CountryOfOrigin : PXFieldState;
	NonStockReceipt : PXFieldState<PXFieldOptions.CommitChanges>;
	NonStockReceiptAsService : PXFieldState<PXFieldOptions.CommitChanges>;
	NonStockShip : PXFieldState;
	CompletePOLine : PXFieldState;
	EstimatedDuration : PXFieldState;
	DefaultSubItemID : PXFieldState;
	DefaultSubItemOnEntry : PXFieldState;
	BaseUnit : PXFieldState<PXFieldOptions.CommitChanges>;
	DecimalBaseUnit : PXFieldState<PXFieldOptions.CommitChanges>;
	SalesUnit : PXFieldState<PXFieldOptions.CommitChanges>;
	DecimalSalesUnit : PXFieldState<PXFieldOptions.CommitChanges>;
	PurchaseUnit : PXFieldState<PXFieldOptions.CommitChanges>;
	DecimalPurchaseUnit : PXFieldState<PXFieldOptions.CommitChanges>;
	CycleID : PXFieldState<PXFieldOptions.CommitChanges>;
	ABCCodeID : PXFieldState<PXFieldOptions.CommitChanges>;
	ABCCodeIsFixed : PXFieldState<PXFieldOptions.NoLabel>;
	MovementClassID : PXFieldState<PXFieldOptions.CommitChanges>;
	MovementClassIsFixed : PXFieldState<PXFieldOptions.NoLabel>;
	BaseItemWeight : PXFieldState;
	WeightUOM : PXFieldState;
	BaseItemVolume : PXFieldState;
	VolumeUOM : PXFieldState;
	HSTariffCode : PXFieldState;
	UndershipThreshold : PXFieldState;
	OvershipThreshold : PXFieldState;
	PackageOption : PXFieldState<PXFieldOptions.CommitChanges>;
	PackSeparately : PXFieldState;
	PriceClassID : PXFieldState;
	PriceWorkgroupID : PXFieldState<PXFieldOptions.CommitChanges>;
	PriceManagerID : PXFieldState<PXFieldOptions.CommitChanges>;
	Commisionable : PXFieldState<PXFieldOptions.NoLabel>;
	MinGrossProfitPct : PXFieldState;
	MarkupPct : PXFieldState;
	PostToExpenseAccount : PXFieldState<PXFieldOptions.CommitChanges>;
	CostBasis : PXFieldState<PXFieldOptions.CommitChanges>;
	PercentOfSalesPrice : PXFieldState;
	DfltEarningType : PXFieldState;
	InvtAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccrualAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	InvtSubID : PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAccrualSubID : PXFieldState<PXFieldOptions.CommitChanges>;
	ReasonCodeSubID : PXFieldState;
	SalesAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID : PXFieldState;
	COGSAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	COGSSubID : PXFieldState;
	ExpenseSubID : PXFieldState;
	StdCstVarAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstVarSubID : PXFieldState;
	StdCstRevAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstRevSubID : PXFieldState;
	POAccrualAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualSubID : PXFieldState;
	PPVAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	PPVSubID : PXFieldState;
	LCVarianceAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	LCVarianceSubID : PXFieldState;
	DeferralAcctID : PXFieldState<PXFieldOptions.CommitChanges>;
	DeferralSubID : PXFieldState;
	Body : PXFieldState;
	DefaultColumnMatrixAttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultRowMatrixAttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
	ImageUrl : PXFieldState;
	UpdateOnlySelected : PXFieldState;
	ExportToExternal : PXFieldState<PXFieldOptions.CommitChanges>;
	Visibility : PXFieldState;
	Availability : PXFieldState<PXFieldOptions.CommitChanges>;
	NotAvailMode : PXFieldState;
	CustomURL : PXFieldState<PXFieldOptions.CommitChanges>;
	PageTitle : PXFieldState;
	ShortDescription : PXFieldState;
	SearchKeywords : PXFieldState;
	MetaKeywords : PXFieldState;
	MetaDescription : PXFieldState;
}

export class InventoryItemCurySettings extends PXView  {
	DfltSiteID : PXFieldState<PXFieldOptions.CommitChanges>;
	DfltShipLocationID : PXFieldState<PXFieldOptions.CommitChanges>;
	DfltReceiptLocationID : PXFieldState<PXFieldOptions.CommitChanges>;
	RecPrice_Label : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
	RecPrice : PXFieldState<PXFieldOptions.NoLabel>;
	BasePrice_Label : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
	BasePrice : PXFieldState<PXFieldOptions.NoLabel>;
	PendingStdCost_Label : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
	PendingStdCost : PXFieldState<PXFieldOptions.NoLabel>;
	PendingStdCostDate : PXFieldState;
	StdCost_Label : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
	StdCost : PXFieldState<PXFieldOptions.NoLabel>;
	StdCostDate : PXFieldState<PXFieldOptions.Disabled>;
	LastStdCost_Label : PXFieldState<PXFieldOptions.NoLabel | PXFieldOptions.Disabled>;
	LastStdCost : PXFieldState<PXFieldOptions.NoLabel>;
}

@gridConfig({
	preset: GridPreset.ShortList,
	initNewRow: true
})
export class INUnit extends PXView {
	UnitType: PXFieldState;
	@columnConfig({visible: false})
	ItemClassID: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.Hidden>;
	FromUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitMultDiv: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleToUnit: PXFieldState;
	PriceAdjustmentMultiplier: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.ShortList
})
export class INItemCategory extends PXView  {
	CategoryID : PXFieldState;
}

@gridConfig({
	preset: GridPreset.ShortList
})
export class INItemBox extends PXView  {
	BoxID : PXFieldState<PXFieldOptions.CommitChanges>;
	Description : PXFieldState;
	UOM : PXFieldState<PXFieldOptions.CommitChanges>;
	Qty : PXFieldState;
	MaxWeight : PXFieldState;
	MaxVolume : PXFieldState;
	MaxQty : PXFieldState;
}


@gridConfig({
	preset: GridPreset.Details
})
export class VendorItems extends PXView {
	Active: PXFieldState;
	IsDefault: PXFieldState;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	Vendor__AcctName: PXFieldState;
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Location__VSiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PurchaseUnit: PXFieldState;
	Location__VLeadTime: PXFieldState;
	OverrideSettings: PXFieldState;
	AddLeadTimeDays: PXFieldState;
	MinOrdFreq: PXFieldState;
	MinOrdQty: PXFieldState;
	MaxOrdQty: PXFieldState;
	LotSize: PXFieldState;
	ERQ: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	LastPrice: PXFieldState;
	PrepaymentPct: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Attributes,
	syncPosition: true,
	autoAdjustColumns: true
})
export class Attributes extends PXView {
	@columnConfig({ hideViewLink: true })
	AttributeID: PXFieldState;
	isRequired: PXFieldState;
	AttributeCategory: PXFieldState;
	Value: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	statusField: "Sample"
})
export class INMatrixGenerationRule extends PXView  {
	IdRowUp: PXActionState;
	IdRowDown: PXActionState;

	SegmentType : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
	Constant : PXFieldState<PXFieldOptions.CommitChanges>;
	NumberingID : PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCharacters : PXFieldState<PXFieldOptions.CommitChanges>;
	UseSpaceAsSeparator : PXFieldState<PXFieldOptions.CommitChanges>;
	Separator : PXFieldState<PXFieldOptions.CommitChanges>;
	AddSpaces : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Sample : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	statusField: "Sample"
})
export class DescriptionGenerationRules extends PXView  {
	DescriptionRowUp: PXActionState;
	DescriptionRowDown: PXActionState;

	SegmentType : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	AttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
	Constant : PXFieldState<PXFieldOptions.CommitChanges>;
	NumberingID : PXFieldState<PXFieldOptions.CommitChanges>;
	NumberOfCharacters : PXFieldState<PXFieldOptions.CommitChanges>;
	UseSpaceAsSeparator : PXFieldState<PXFieldOptions.CommitChanges>;
	Separator : PXFieldState<PXFieldOptions.CommitChanges>;
	AddSpaces : PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({
		visible: false,
		allowShowHide: GridColumnShowHideMode.False
	})
	Sample : PXFieldState;
}

export class EntryHeader extends PXView  {
	ColAttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
	RowAttributeID : PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowDelete: false,
	allowInsert: false
})
export class AdditionalAttributes extends PXView  {
}

@gridConfig({
	preset: GridPreset.Details, //to review when restoring matrix items functionality
	allowDelete: false,
	allowInsert: false
})
export class EntryMatrix extends PXView  {
}

@gridConfig({
	preset: GridPreset.Details, //to review when restoring matrix items functionality
	allowDelete: false,
	allowInsert: false
})
export class MatrixItemsForCreation extends PXView  {
	Selected : PXFieldState;
	InventoryCD : PXFieldState;
	Descr : PXFieldState;
	StkItem : PXFieldState;
	ItemClassID : PXFieldState;
	ItemType : PXFieldState;
	ValMethod : PXFieldState;
	LotSerClassID : PXFieldState;
	DfltSiteID : PXFieldState;
	TaxCategoryID : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details //to review when restoring matrix items functionality
})
export class INMatrixExcludedData extends PXView  {
	TableName : PXFieldState<PXFieldOptions.CommitChanges>;
	FieldName : PXFieldState;
	@columnConfig({allowCheckAll: true})
	IsActive : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details //to review when restoring matrix items functionality
})
export class AttributesExcludedFromUpdate extends PXView  {
	FieldName : PXFieldState;
	CSAnswers__IsRequired : PXFieldState;
	CSAnswers__AttributeCategory : PXFieldState;
	CSAnswers__Value : PXFieldState;
	@columnConfig({allowCheckAll: true})
	IsActive : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false
})
export class MatrixItems extends PXView  {
	DeleteItems: PXActionState;

	@columnConfig({allowCheckAll: true})
	Selected : PXFieldState;
	@linkCommand("ViewMatrixItem")
	InventoryID : PXFieldState;
	Descr : PXFieldState;
	DfltSiteID : PXFieldState;
	AttributeValue0 : PXFieldState;
	ItemClassID : PXFieldState;
	TaxCategoryID : PXFieldState;
	RecPrice : PXFieldState;
	LastCost : PXFieldState;
	BasePrice : PXFieldState;
	StkItem : PXFieldState;
}

@gridConfig({
	preset: GridPreset.ShortList
})
export class BCInventoryFileUrls extends PXView  {
	FileURL : PXFieldState<PXFieldOptions.CommitChanges>;
	FileType : PXFieldState;
}

export class POVendorPriceUpdate extends PXView  {
	PendingDate : PXFieldState;
}

export class INItemClass extends PXView  {
	Mem_RouteService : PXFieldState<PXFieldOptions.Disabled>;
}
import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
	PXView,
	PXFieldState,
	headerDescription,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	PXActionState,
	linkCommand,
	localizable,
	GridPreset
} from "client-controls";

@localizable
export class EquipmentItemClassOptions {
	static PartOrOtherInventory = "Part or Other Inventory";
	static ModelEquipment = "Model Equipment";
	static Component = "Component";
	static Consumable = "Consumable";
}

@graphInfo({ graphType: 'PX.Objects.IN.InventoryItemMaint', primaryView: 'Item' })
export class IN202500 extends PXScreen {
	EquipmentItemClassMode = EquipmentItemClassOptions;

	@viewInfo({ containerName: 'Stock Item Summary' })
	Item = createSingle(InventoryItem);

	@viewInfo({ containerName: "Inventory Item Settings" })
	ItemSettings = createSingle(ItemSettings);

	@viewInfo({ containerName: "Inventory Item Currency-Specific Settings" })
	ItemCurySettings = createSingle(ItemCurySettings);

	@viewInfo({ containerName: "Inventory Item Costs" })
	itemcosts = createSingle(ItemCosts);

	@viewInfo({ containerName: "Subitem Segment Values" })
	SegmentValues = createCollection(SubitemSegmentValues);

	@viewInfo({ containerName: "Item UOM Settings" })
	itemunits = createCollection(ItemUnits);

	@viewInfo({ containerName: "Warehouses" })
	itemsiterecords = createCollection(ItemSiteRecords);

	@viewInfo({ containerName: "Vendors" })
	VendorItems = createCollection(VendorItems);

	@viewInfo({ containerName: "Attributes" })
	Answers = createCollection(Attributes);

	@viewInfo({ containerName: "Sales Categories" })
	Category = createCollection(Category);

	@viewInfo({ containerName: "Boxes" })
	Boxes = createCollection(Boxes);

	@viewInfo({ containerName: "Cross-Reference" })
	itemxrefrecords = createCollection(ItemXRefRecords);

	@viewInfo({ containerName: "Iventory Planning Settings" })
	inventoryPlanningSettings = createSingle(InventoryPlanningSettings);

	@viewInfo({ containerName: "Production Order Default Settings" })
	productionOrderDefaultSettings = createSingle(ProductionOrderDefaultSettings);

	@viewInfo({ containerName: "Replenishment" })
	replenishment = createCollection(Replenishment);

	@viewInfo({ containerName: "Subitem Replenishment" })
	subreplenishment = createCollection(SubReplenishment);

	@viewInfo({ containerName: "Revenue Components" })
	Components = createCollection(RevenueComponents);

	@viewInfo({ containerName: "Restriction Groups" })
	Groups = createCollection(RestrictionGroups);

	@viewInfo({ containerName: "Service Management" })
	ModelComponents = createCollection(ModelComponents);

	@viewInfo({ containerName: "eCommerce" })
	SyncRecs = createCollection(EcommerceSyncRecs);

	@viewInfo({ containerName: "Media URLs" })
	InventoryFileUrls = createCollection(InventoryFileUrls);

	@viewInfo({ containerName: "Specify New ID" })
	ChangeIDDialog = createSingle(ChangeIDDialog);

	@viewInfo({ containerName: "Update Effective Vendor Prices" })
	VendorInventory$UpdatePrice = createSingle(UpdatePrice);
}

export class InventoryItem extends PXView {
	InventoryCD: PXFieldState;
	ItemStatus: PXFieldState;
	@headerDescription Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductWorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductManagerID: PXFieldState;
	ChkEquipmentManagement: PXFieldState;
	Body: PXFieldState;
	ImageUrl: PXFieldState;
}

export class ItemSettings extends PXView {
	//General tab
	TemplateItemID: PXFieldState;
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemType: PXFieldState;
	KitItem: PXFieldState;
	ValMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	PlanningMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	PostClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerNumVal: PXFieldState<PXFieldOptions.CommitChanges>;
	CountryOfOrigin: PXFieldState;
	DefaultSubItemID: PXFieldState;
	DefaultSubItemOnEntry: PXFieldState;
	IsSpecialOrderItem: PXFieldState;
	BaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	DecimalBaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	DecimalSalesUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	DecimalPurchaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	WeightItem: PXFieldState;
	CycleID: PXFieldState<PXFieldOptions.CommitChanges>;
	ABCCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ABCCodeIsFixed: PXFieldState;
	MovementClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	MovementClassIsFixed: PXFieldState;

	//Price-Cost tab
	PriceClassID: PXFieldState;
	PriceWorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceManagerID: PXFieldState<PXFieldOptions.CommitChanges>;
	Commisionable: PXFieldState;
	MinGrossProfitPct: PXFieldState;
	MarkupPct: PXFieldState;

	//Packaging tab
	BaseItemWeight: PXFieldState;
	WeightUOM: PXFieldState;
	BaseItemVolume: PXFieldState;
	VolumeUOM: PXFieldState;
	CommodityCodeType: PXFieldState;
	HSTariffCode: PXFieldState;
	UndershipThreshold: PXFieldState;
	OvershipThreshold: PXFieldState;
	PackageOption: PXFieldState<PXFieldOptions.CommitChanges>;
	PackSeparately: PXFieldState;

	//Deferral tab
	DeferredCode: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultTerm: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultTermUOM: PXFieldState;
	UseParentSubID: PXFieldState;
	TotalPercentage: PXFieldState;

	//GL Accounts tab
	InvtAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvtSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReasonCodeSubID: PXFieldState;
	SalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID: PXFieldState;
	COGSAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSSubID: PXFieldState;
	StdCstVarAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstVarSubID: PXFieldState;
	StdCstRevAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCstRevSubID: PXFieldState;
	POAccrualAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualSubID: PXFieldState;
	PPVAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PPVSubID: PXFieldState;
	LCVarianceAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	LCVarianceSubID: PXFieldState;
	DeferralAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeferralSubID: PXFieldState;
	AMWIPAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMWIPSubID: PXFieldState;
	AMWIPVarianceAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMWIPVarianceSubID: PXFieldState;

	//Service management tab
	EquipmentItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	ManufacturerID: PXFieldState<PXFieldOptions.CommitChanges>;
	ManufacturerModelID: PXFieldState<PXFieldOptions.CommitChanges>;
	EquipmentTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Mem_ShowComponent: PXFieldState;
	CpnyWarrantyValue: PXFieldState;
	CpnyWarrantyType: PXFieldState;
	VendorWarrantyValue: PXFieldState;
	VendorWarrantyType: PXFieldState;

	//eCommerce tab
	ExportToExternal: PXFieldState<PXFieldOptions.CommitChanges>;
	Visibility: PXFieldState;
	Availability: PXFieldState<PXFieldOptions.CommitChanges>;
	AvailabilityAdjustment: PXFieldState<PXFieldOptions.CommitChanges>;
	NotAvailMode: PXFieldState;
	CustomURL: PXFieldState<PXFieldOptions.CommitChanges>;
	PageTitle: PXFieldState;
	ShortDescription: PXFieldState;
	SearchKeywords: PXFieldState;
	MetaKeywords: PXFieldState;
	MetaDescription: PXFieldState;
}

export class ItemCurySettings extends PXView {
	//General tab
	DfltSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltShipLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltReceiptLocationID: PXFieldState<PXFieldOptions.CommitChanges>;

	//Price-Cost tab
	RecPrice_Label: PXFieldState;
	RecPrice: PXFieldState;
	BasePrice_Label: PXFieldState;
	BasePrice: PXFieldState;

	PendingStdCost_Label: PXFieldState;
	PendingStdCost: PXFieldState;
	PendingStdCostDate: PXFieldState;
	StdCost_Label: PXFieldState;
	StdCost: PXFieldState;
	StdCostDate: PXFieldState;
	LastStdCost_Label: PXFieldState;
	LastStdCost: PXFieldState;
}

export class ItemCosts extends PXView {
	LastCost_Label: PXFieldState;
	LastCost: PXFieldState;
	AvgCost_Label: PXFieldState;
	AvgCost: PXFieldState;
	MinCost_Label: PXFieldState;
	MinCost: PXFieldState;
	MaxCost_Label: PXFieldState;
	MaxCost: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class SubitemSegmentValues extends PXView {
	SegmentID: PXFieldState<PXFieldOptions.CommitChanges>;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.ShortList
})
export class ItemUnits extends PXView {
	UnitType: PXFieldState;
	ItemClassID: PXFieldState;
	InventoryID: PXFieldState;
	FromUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitMultDiv: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleToUnit: PXFieldState;
	PriceAdjustmentMultiplier: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details
})
export class ItemSiteRecords extends PXView {
	AddWarehouseDetail: PXActionState;

	IsDefault: PXFieldState;
	SiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DfltReceiptLocationID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DfltShipLocationID: PXFieldState;
	SiteStatus: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InvtAcctID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	InvtSubID: PXFieldState;
	ProductManagerOverride: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProductWorkgroupID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ProductManagerID: PXFieldState;
	StdCostOverride: PXFieldState;
	BasePriceOverride: PXFieldState;
	INSiteStatusSummary__QtyOnHand: PXFieldState;
	PreferredVendorOverride: PXFieldState;
	PreferredVendorID: PXFieldState;
	ReplenishmentPolicyOverride: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ReplenishmentPolicyID: PXFieldState;
	ReplenishmentSource: PXFieldState;
	@columnConfig({ hideViewLink: true })
	ReplenishmentSourceSiteID: PXFieldState;
	ServiceLevelOverride: PXFieldState;
	ServiceLevelPct: PXFieldState;
	LastForecastDate: PXFieldState;
	DemandPerDayAverage: PXFieldState;
	DemandPerDaySTDEV: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class VendorItems extends PXView {
	Active: PXFieldState;
	IsDefault: PXFieldState;
	@linkCommand("ViewVendorEmployee")
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	Vendor__AcctName: PXFieldState;
	@linkCommand("ViewVendorLocation")
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Location__VSiteID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	PurchaseUnit: PXFieldState;
	VendorInventoryID: PXFieldState;
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
	AttributeID: PXFieldState;
	AttributeCategory: PXFieldState;
	Value: PXFieldState;
	isRequired: PXFieldState;
}

@gridConfig({
	preset: GridPreset.ShortList
})
export class Category extends PXView {
	@columnConfig({ hideViewLink: true })
	CategoryID: PXFieldState;
}

@gridConfig({
	preset: GridPreset.ShortList,
	suppressNoteFiles: true
})
export class Boxes extends PXView {
	@columnConfig({ hideViewLink: true })
	BoxID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	Qty: PXFieldState;
	MaxWeight: PXFieldState;
	MaxVolume: PXFieldState;
	MaxQty: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class ItemXRefRecords extends PXView {
	@columnConfig({ hideViewLink: true })
	SubItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	AlternateType: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AlternateID: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
}

export class InventoryPlanningSettings extends PXView {
	ReplenishmentSource: PXFieldState<PXFieldOptions.CommitChanges>;
	AMSourceSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMSafetyStock: PXFieldState;
	AMMinQty: PXFieldState;
	AMMinOrdQty: PXFieldState;
	AMMaxOrdQty: PXFieldState;
	AMLotSize: PXFieldState;
	AMMFGLeadTime: PXFieldState;
	CategoryID: PXFieldState;
	AMGroupWindow: PXFieldState;
	AMGroupWindowOverride: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ProductionOrderDefaultSettings extends PXView {
	ReplenishmentSource: PXFieldState<PXFieldOptions.CommitChanges>;
	AMSourceSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMMinOrdQty: PXFieldState;
	AMMaxOrdQty: PXFieldState;
	AMLotSize: PXFieldState;
	AMMFGLeadTime: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class Replenishment extends PXView {
	ReplenishmentClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplenishmentPolicyID: PXFieldState;
	ReplenishmentSource: PXFieldState;
	ReplenishmentMethod: PXFieldState;
	ReplenishmentSourceSiteID: PXFieldState;
	MaxShelfLife: PXFieldState;
	LaunchDate: PXFieldState;
	TerminationDate: PXFieldState;
	ServiceLevelPct: PXFieldState;
	SafetyStock: PXFieldState;
	MinQty: PXFieldState;
	MaxQty: PXFieldState;
	TransferERQ: PXFieldState;
	ForecastModelType: PXFieldState;
	ForecastPeriodType: PXFieldState;
	HistoryDepth: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class SubReplenishment extends PXView {
	GenerateSubitems: PXActionState;
	UpdateReplenishment: PXActionState;

	InventoryID: PXFieldState;
	ReplenishmentClassID: PXFieldState;
	SubItemID: PXFieldState;
	SafetyStock: PXFieldState;
	MinQty: PXFieldState;
	MaxQty: PXFieldState;
	TransferERQ: PXFieldState;
	ItemStatus: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class RevenueComponents extends PXView {
	ComponentID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	SalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	SalesSubID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	UOM: PXFieldState;
	Qty: PXFieldState;
	DeferredCode: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultTerm: PXFieldState;
	@columnConfig({ hideViewLink: true })
	DefaultTermUOM: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideDefaultTerm: PXFieldState<PXFieldOptions.CommitChanges>;
	AmtOption: PXFieldState<PXFieldOptions.CommitChanges>;
	AmtOptionASC606: PXFieldState<PXFieldOptions.CommitChanges>;
	FixedAmt: PXFieldState;
	Percentage: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowInsert: false,
	allowDelete: false
})
export class RestrictionGroups extends PXView {
	ViewGroupDetails: PXActionState;

	Included: PXFieldState;
	GroupName: PXFieldState;
	SpecificType: PXFieldState;
	Description: PXFieldState;
	Active: PXFieldState;
	GroupType: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class ModelComponents extends PXView {
	ComponentID: PXFieldState<PXFieldOptions.CommitChanges>;
	Active: PXFieldState;
	Optional: PXFieldState;
	Qty: PXFieldState;
	Descr: PXFieldState;
	ClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	RequireSerial: PXFieldState;
	CpnyWarrantyValue: PXFieldState;
	CpnyWarrantyType: PXFieldState;
	VendorWarrantyValue: PXFieldState;
	VendorWarrantyType: PXFieldState;
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true
})
export class EcommerceSyncRecs extends PXView {
	SYProvider__Name: PXFieldState;
	@linkCommand("GoToSalesforce")
	RemoteID: PXFieldState;
	Status: PXFieldState;
	Operation: PXFieldState;
	LastErrorMessage: PXFieldState;
	LastAttemptTS: PXFieldState;
	AttemptCount: PXFieldState;
	SFEntitySetup__ImportScenario: PXFieldState;
	SFEntitySetup__ExportScenario: PXFieldState;
}

@gridConfig({
	preset: GridPreset.ShortList,
	suppressNoteFiles: true
})
export class InventoryFileUrls extends PXView {
	FileURL: PXFieldState<PXFieldOptions.CommitChanges>;
	FileType: PXFieldState;
}

export class ChangeIDDialog extends PXView {
	CD: PXFieldState;
}

export class UpdatePrice extends PXView {
	PendingDate: PXFieldState;
}
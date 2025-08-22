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
	linkCommand,
	localizable,
	GridPreset
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.IN.NonStockItemMaint', primaryView: 'Item' })
export class IN202000 extends PXScreen {
	@viewInfo({ containerName: 'Stock Item Summary' })
	Item = createSingle(InventoryItem);

	@viewInfo({ containerName: "Inventory Item Settings" })
	ItemSettings = createSingle(ItemSettings);

	@viewInfo({ containerName: "Item Class" })
	ItemClass = createCollection(ItemClass);

	@viewInfo({ containerName: "Inventory Item Currency-Specific Settings" })
	ItemCurySettings = createSingle(ItemCurySettings);

	@viewInfo({ containerName: "Item UOM Settings" })
	itemunits = createCollection(ItemUnits);

	@viewInfo({ containerName: "Vendors" })
	VendorItems = createCollection(VendorItems);

	@viewInfo({ containerName: "Attributes" })
	Answers = createCollection(Attributes);

	@viewInfo({ containerName: "Sales Categories" })
	Category = createCollection(Category);

	@viewInfo({ containerName: "Cross-Reference" })
	itemxrefrecords = createCollection(ItemXRefRecords);

	@viewInfo({ containerName: "Revenue Components" })
	Components = createCollection(RevenueComponents);

	@viewInfo({ containerName: "Service Skills" })
	ServiceSkills = createCollection(ServiceSkills);

	@viewInfo({ containerName: "Service License Types" })
	ServiceLicenseTypes = createCollection(ServiceLicenseTypes);

	@viewInfo({ containerName: "Resource Equipment Types" })
	ServiceEquipmentTypes = createCollection(ServiceEquipmentTypes);

	@viewInfo({ containerName: "Pickup/Delivery Item" })
	ServiceInventoryItems = createCollection(ServiceInventoryItems);

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
	ChkServiceManagement: PXFieldState;
	Body: PXFieldState;
}

export class ItemSettings extends PXView {
	//General tab
	TemplateItemID: PXFieldState;
	//Item Defaults
	ItemClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ItemType: PXFieldState;
	PostClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	KitItem: PXFieldState;
	IsTravelItem: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCalcMode: PXFieldState;
	NonStockReceipt: PXFieldState<PXFieldOptions.CommitChanges>;
	NonStockReceiptAsService: PXFieldState<PXFieldOptions.CommitChanges>;
	NonStockShip: PXFieldState;
	CompletePOLine: PXFieldState;
	AMDefaultMarkFor: PXFieldState;
	//Field Service Defaults
	EstimatedDuration: PXFieldState;
	//ItemClass.Mem_RouteService: PXFieldState; How to define?

	//Unit of Measure
	BaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	DecimalBaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	DecimalSalesUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	DecimalPurchaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;

	//Price-Cost tab
	PriceClassID: PXFieldState;
	PriceWorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	PriceManagerID: PXFieldState<PXFieldOptions.CommitChanges>;
	Commisionable: PXFieldState;
	MinGrossProfitPct: PXFieldState;
	MarkupPct: PXFieldState;

	//Posting of Item Cost
	PostToExpenseAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	CostBasis: PXFieldState<PXFieldOptions.CommitChanges>;
	PercentOfSalesPrice: PXFieldState;

	//Field Service Defaults
	DfltEarningType: PXFieldState;
	BillingRule: PXFieldState;

	//Packaging tab
	BaseItemWeight: PXFieldState;
	WeightUOM: PXFieldState;
	BaseItemVolume: PXFieldState;
	VolumeUOM: PXFieldState;
	UndershipThreshold: PXFieldState;
	OvershipThreshold: PXFieldState;
	CommodityCodeType: PXFieldState;
	HSTariffCode: PXFieldState;

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
	COGSAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	COGSSubID: PXFieldState;
	POAccrualAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	POAccrualSubID: PXFieldState;
	SalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID: PXFieldState;
	PPVAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PPVSubID: PXFieldState;
	DeferralAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeferralSubID: PXFieldState;
	EarningsAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	EarningsSubID: PXFieldState;
	BenefitExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	BenefitExpenseSubID: PXFieldState;
	TaxExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxExpenseSubID: PXFieldState;
	PTOExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PTOExpenseSubID: PXFieldState;

	//Pickup/Delivery Item tab
	ActionType: PXFieldState<PXFieldOptions.CommitChanges>;

	//eCommerce tab
	ExportToExternal: PXFieldState<PXFieldOptions.CommitChanges>;
	Visibility: PXFieldState;
	Availability: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomURL: PXFieldState<PXFieldOptions.CommitChanges>;
	PageTitle: PXFieldState;
	ShortDescription: PXFieldState;
	SearchKeywords: PXFieldState;
	MetaKeywords: PXFieldState;
	MetaDescription: PXFieldState;

	//Attributes tab
	ImageUrl: PXFieldState;
}

export class ItemClass extends PXView {
	Mem_RouteService: PXFieldState;
}

export class ItemCurySettings extends PXView {
	//General tab
	DfltSiteID: PXFieldState<PXFieldOptions.CommitChanges>;

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
export class VendorItems extends PXView {
	Active: PXFieldState;
	IsDefault: PXFieldState;
	@linkCommand("ViewVendorEmployee")
	VendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	Vendor__AcctName: PXFieldState;
	@columnConfig({ hideViewLink: true })
	VendorLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true })
	PurchaseUnit: PXFieldState;
	VendorInventoryID: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	LastPrice: PXFieldState;
	PrepaymentPct: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Attributes,
	syncPosition: true
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
	preset: GridPreset.Details
})
export class ServiceSkills extends PXView {
	SkillID: PXFieldState<PXFieldOptions.CommitChanges>;
	FSSkill__Descr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class ServiceLicenseTypes extends PXView {
	LicenseTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	FSLicenseType__Descr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class ServiceEquipmentTypes extends PXView {
	EquipmentTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	FSEquipmentType__Descr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details
})
export class ServiceInventoryItems extends PXView {
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryItem__Descr: PXFieldState;
}


@gridConfig({
	preset: GridPreset.Details
})
export class ItemXRefRecords extends PXView {
	AlternateType: PXFieldState<PXFieldOptions.CommitChanges>;
	BAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AlternateID: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
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
import {
	PXScreen,
	PXView,
	PXFieldState,
	PXFieldOptions,

	createSingle,
	createCollection,

	graphInfo,
	viewInfo,
	gridConfig,
	columnConfig,

	linkCommand,
	treeConfig,
	PXActionState,
} from "client-controls";

import { EquipmentItemClassOptions } from '../IN202500/IN202500';

@graphInfo({ graphType: 'PX.Objects.IN.INItemClassMaint', primaryView: 'itemclass' })
export class IN201000 extends PXScreen {
	EquipmentItemClassMode = EquipmentItemClassOptions;

	@viewInfo({ containerName: "Item Class" })
	itemclass = createSingle(ItemClass);

	@viewInfo({ containerName: "Item Class Settings" })
	itemclasssettings = createSingle(ItemClassSettings);

	@viewInfo({ containerName: "Item Class Cury Settings" })
	itemclasscurysettings = createSingle(ItemClassCurySettings);

	@viewInfo({ containerName: "Tree View and Primary View Synchronization Helper" })
	TreeViewAndPrimaryViewSynchronizationHelper = createSingle(TreeViewAndPrimaryViewSynchronizationHelper);

	@viewInfo({ containerName: "Item Class Nodes" })
	ItemClassNodes = createCollection(ItemClassNodes);

	@viewInfo({ containerName: "Production Order Default Settings" })
	productionOrderDefaultSettings = createSingle(ProductionOrderDefaultSettings);

	@viewInfo({ containerName: "Item Class UOM Settings" })
	classunits = createCollection(ItemClassUnits);

	@viewInfo({ containerName: "Replenishment" })
	replenishment = createCollection(Replenishment);

	@viewInfo({ containerName: "Iventory Planning Settings" })
	inventoryPlanningSettings = createSingle(InventoryPlanningSettings);

	@viewInfo({ containerName: "Restriction Groups" })
	Groups = createCollection(RestrictionGroups);

	@viewInfo({ containerName: "Mapping" })
	Mapping = createCollection(Mapping);

	@viewInfo({ containerName: "Model Template Component Records" })
	ModelTemplateComponentRecords = createCollection(ModelTemplateComponentRecords);


	@viewInfo({ containerName: "Specify New ID" })
	ChangeIDDialog = createSingle(ChangeIDDialog);
}


export class ItemClass extends PXView {
	ItemClassCD: PXFieldState;
	Descr: PXFieldState;
	ChkServiceManagement: PXFieldState;
}

export class ItemClassSettings extends PXView {
	StkItem: PXFieldState<PXFieldOptions.CommitChanges>;
	NegQty: PXFieldState;
	ExportToExternal: PXFieldState;
	ItemType: PXFieldState<PXFieldOptions.CommitChanges>;
	ValMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	PlanningMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCategoryID: PXFieldState;
	TaxCalcMode: PXFieldState;
	PostClassID: PXFieldState;
	PostToExpenseAccount: PXFieldState<PXFieldOptions.CommitChanges>;
	LotSerClassID: PXFieldState;
	PriceClassID: PXFieldState;
	AvailabilitySchemeID: PXFieldState;
	CountryOfOrigin: PXFieldState;
	CommodityCodeType: PXFieldState<PXFieldOptions.CommitChanges>;
	HSTariffCode: PXFieldState;
	UndershipThreshold: PXFieldState;
	OvershipThreshold: PXFieldState;

	BaseUnit: PXFieldState;
	DecimalBaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesUnit: PXFieldState;
	DecimalSalesUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	PurchaseUnit: PXFieldState;
	DecimalPurchaseUnit: PXFieldState<PXFieldOptions.CommitChanges>;

	PriceWorkgroupID: PXFieldState;
	PriceManagerID: PXFieldState;
	MinGrossProfitPct: PXFieldState;
	MarkupPct: PXFieldState;

	DefaultRowMatrixAttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultColumnMatrixAttributeID : PXFieldState<PXFieldOptions.CommitChanges>;

	DemandCalculation: PXFieldState;

	DefaultBillingRule: PXFieldState;
	RequireRoute: PXFieldState;
	EquipmentItemClass: PXFieldState<PXFieldOptions.CommitChanges>;
	Mem_ShowComponent: PXFieldState;
}

export class ItemClassCurySettings extends PXView {
	DfltSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class TreeViewAndPrimaryViewSynchronizationHelper extends PXView {
	Descr: PXFieldState<PXFieldOptions.Hidden | PXFieldOptions.Disabled>;
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'ItemClassNodes',
	idParent: 'ParentItemClassID',
	idName: 'ItemClassID',
	description: 'SegmentedClassCD',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	//actionField: 'GoToNodeSelectedInTree',
	syncPosition: true,
	openedLayers: 1,
})
// AllowCollapse="False"
// AutoRepaint="True"
// PreserveExpanded="True"
// PopulateOnDemand="True"
// AutoCallBackCommand="GoToNodeSelectedInTree"
export class ItemClassNodes extends PXView {
	ParentItemClassID: PXFieldState;
	ItemClassID: PXFieldState;
	SegmentedClassCD: PXFieldState;
	Descr: PXFieldState;
}

@gridConfig({
	autoAdjustColumns: true
})
export class ItemClassUnits extends PXView {
	UnitType: PXFieldState;
	ItemClassID: PXFieldState;
	InventoryID: PXFieldState;
	FromUnit: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitMultDiv: PXFieldState<PXFieldOptions.CommitChanges>;
	UnitRate: PXFieldState<PXFieldOptions.CommitChanges>;
	SampleToUnit: PXFieldState;
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
	autoAdjustColumns: true,
	initNewRow: true
})
export class Replenishment extends PXView {
	ReplenishmentClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplenishmentPolicyID: PXFieldState;
	ReplenishmentSource: PXFieldState;
	ReplenishmentMethod: PXFieldState;
	ReplenishmentSourceSiteID: PXFieldState;

	TransferLeadTime: PXFieldState;
	TransferERQ: PXFieldState;
	ForecastModelType: PXFieldState;
	ForecastPeriodType: PXFieldState;
	HistoryDepth: PXFieldState;
	LaunchDate: PXFieldState;
	TerminationDate: PXFieldState;
	ServiceLevelPct: PXFieldState;
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
	AMDaysSupply: PXFieldState;
}

@gridConfig({
	syncPosition: true,
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
})
export class Mapping extends PXView {
	IsActive: PXFieldState;

	@linkCommand('CRAttribute_ViewDetails')
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;

	Description: PXFieldState;
	SortOrder: PXFieldState;
	Required: PXFieldState<PXFieldOptions.CommitChanges>;
	ControlType: PXFieldState;
	AttributeCategory: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
})
export class ModelTemplateComponentRecords extends PXView {
	ComponentCD: PXFieldState;
	Active: PXFieldState;
	Optional: PXFieldState;
	Qty: PXFieldState;
	Descr: PXFieldState;
	ClassID: PXFieldState;
}

export class ChangeIDDialog extends PXView {
	CD: PXFieldState;
}

import {
	PXView,
	PXFieldState,
	gridConfig,
	PXFieldOptions,
	PXActionState,
	GridPreset
} from 'client-controls';

export class INItemSite extends PXView  {
	InventoryID: PXFieldState;
	SiteID: PXFieldState;
	SiteStatus: PXFieldState;
	ProductManagerOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductWorkgroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProductManagerID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class INItemSiteGeneral extends PXView  {
	DfltShipLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	DfltReceiptLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	ABCCodeOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ABCCodeID: PXFieldState<PXFieldOptions.CommitChanges>;
	ABCCodeIsFixed: PXFieldState;
	MovementClassOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MovementClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	MovementClassIsFixed: PXFieldState;
	CountryOfOrigin: PXFieldState;
	OverrideInvtAcctSub: PXFieldState<PXFieldOptions.CommitChanges>;
	InvtAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	InvtSubID: PXFieldState;
	PlanningMethod: PXFieldState<PXFieldOptions.Disabled>;
	SubItemOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	StdCostOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	LastStdCost_Label: PXFieldState;
	LastStdCost: PXFieldState<PXFieldOptions.Disabled>;
	StdCost_Label: PXFieldState;
	StdCost: PXFieldState<PXFieldOptions.Disabled>;
	StdCostDate: PXFieldState<PXFieldOptions.Disabled>;
	PendingStdCost_Label: PXFieldState;
	PendingStdCost: PXFieldState;
	PendingStdCostDate: PXFieldState;
	PriceWorkgroupID: PXFieldState;
	PriceManagerID: PXFieldState;
	Commissionable: PXFieldState;
	MarkupPctOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MarkupPct: PXFieldState;
	RecPriceOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	RecPrice_Label: PXFieldState;
	RecPrice: PXFieldState;
	LastCost_Label: PXFieldState;
	LastCost: PXFieldState;
	AvgCost_Label: PXFieldState;
	AvgCost: PXFieldState;
	MinCost_Label: PXFieldState;
	MinCost: PXFieldState;
	MaxCost_Label: PXFieldState;
	MaxCost: PXFieldState;
}

export class INItemSiteReplenishmentSettings extends PXView  {
	ReplenishmentClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplenishmentPolicyOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplenishmentPolicyID: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplenishmentSource: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplenishmentMethod: PXFieldState<PXFieldOptions.CommitChanges>;
	ReplenishmentSourceSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxShelfLife: PXFieldState;
	MaxShelfLifeOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	LaunchDate: PXFieldState;
	LaunchDateOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	TerminationDate: PXFieldState;
	TerminationDateOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceLevelPct: PXFieldState;
	ServiceLevelOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	SafetyStock: PXFieldState;
	SafetyStockOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MinQty: PXFieldState;
	MinQtyOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	MaxQty: PXFieldState;
	MaxQtyOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	TransferERQ: PXFieldState;
	TransferERQOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	DemandPerDayAverage: PXFieldState<PXFieldOptions.Disabled>;
	DemandPerDaySTDEV: PXFieldState<PXFieldOptions.Disabled>;
	LeadTimeAverage: PXFieldState<PXFieldOptions.Disabled>;
	LeadTimeSTDEV: PXFieldState<PXFieldOptions.Disabled>;
	SafetyStockSuggested: PXFieldState<PXFieldOptions.Disabled>;
	MinQtySuggested: PXFieldState<PXFieldOptions.Disabled>;
	LastForecastDate: PXFieldState<PXFieldOptions.Disabled>;
}

export class INItemSiteInventoryPlanning extends PXView  {
	AMReplenishmentSource: PXFieldState<PXFieldOptions.CommitChanges>;
	AMReplenishmentSourceOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMSourceSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMSourceSiteIDOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMSafetyStock: PXFieldState<PXFieldOptions.CommitChanges>;
	AMSafetyStockOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMMinQty: PXFieldState<PXFieldOptions.CommitChanges>;
	AMMinQtyOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMMinOrdQty: PXFieldState;
	AMMinOrdQtyOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMMaxOrdQty: PXFieldState;
	AMMaxOrdQtyOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMLotSize: PXFieldState;
	AMLotSizeOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMMFGLeadTime: PXFieldState;
	AMMFGLeadTimeOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMTransferLeadTime: PXFieldState;
	AMTransferLeadTimeOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMGroupWindow: PXFieldState;
	AMGroupWindowOverride: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class INItemSitePreferredVendor extends PXView  {
	PreferredVendorOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	PreferredVendorID: PXFieldState<PXFieldOptions.CommitChanges>;
	PreferredVendorLocationID: PXFieldState;
	InventoryItem__DefaultSubItemID: PXFieldState;
}

export class POVendorInventory extends PXView  {
	VLeadTime: PXFieldState<PXFieldOptions.Disabled>;
	AddLeadTimeDays: PXFieldState<PXFieldOptions.Disabled>;
	MinOrdFreq: PXFieldState<PXFieldOptions.Disabled>;
	MinOrdQty: PXFieldState<PXFieldOptions.Disabled>;
	MaxOrdQty: PXFieldState<PXFieldOptions.Disabled>;
	LotSize: PXFieldState<PXFieldOptions.Disabled>;
	ERQ: PXFieldState<PXFieldOptions.Disabled>;
}

export class INItemSiteProductionOrder extends PXView  {
	AMReplenishmentSource: PXFieldState<PXFieldOptions.CommitChanges>;
	AMReplenishmentSourceOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMSourceSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMSourceSiteIDOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMMinOrdQty: PXFieldState;
	AMMinOrdQtyOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMMaxOrdQty: PXFieldState;
	AMMaxOrdQtyOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMLotSize: PXFieldState;
	AMLotSizeOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMMFGLeadTime: PXFieldState;
	AMMFGLeadTimeOverride: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	initNewRow: true
})
export class SubitemReplenishment extends PXView  {
	UpdateReplenishment: PXActionState;

	SubItemID: PXFieldState;
	SafetyStock: PXFieldState;
	MinQty: PXFieldState;
	MaxQty: PXFieldState;
	TransferERQ: PXFieldState;
	SafetyStockSuggested: PXFieldState;
	MinQtySuggested: PXFieldState;
	MaxQtySuggested: PXFieldState;
	DemandPerDayAverage: PXFieldState;
	DemandPerDaySTDEV: PXFieldState;
	ItemStatus: PXFieldState;
}

export class INItemSiteManufacturing extends PXView  {
	AMBOMID: PXFieldState;
	AMPlanningBOMID: PXFieldState;
	AMConfigurationID: PXFieldState;
	ReplenishmentSource: PXFieldState<PXFieldOptions.Disabled>;
	AMMinOrdQty: PXFieldState<PXFieldOptions.Disabled>;
	AMMaxOrdQty: PXFieldState<PXFieldOptions.Disabled>;
	AMLotSize: PXFieldState<PXFieldOptions.Disabled>;
	AMMFGLeadTime: PXFieldState<PXFieldOptions.Disabled>;
	AMScrapOverride: PXFieldState<PXFieldOptions.CommitChanges>;
	AMScrapSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	AMScrapLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Attributes,
	syncPosition: true
})
export class AMSubItemDefault extends PXView  {
	SubItemID: PXFieldState;
	BOMID: PXFieldState;
	PlanningBOMID: PXFieldState;
}
import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	columnConfig,
	gridConfig,
	GridPreset,
	localizable,
	ICurrencyInfo,
} from 'client-controls';

@localizable
class Labels {
	static ShowAll = "Show All";
}

export class AMConfigurationResults extends PXView {
	ConfigResultsID: PXFieldState<PXFieldOptions.CommitChanges>;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	ConfigurationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Revision: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Completed: PXFieldState;
	IsConfigurationTesting: PXFieldState;
	OrdTypeRef: PXFieldState;
	OrdNbrRef: PXFieldState;
	OrdLineRef: PXFieldState;
	OpportunityQuoteID: PXFieldState;
	OpportunityLineNbr: PXFieldState;
	ProdOrderType: PXFieldState;
	ProdOrderNbr: PXFieldState;
	CuryID: PXFieldState;
	DisplayPrice: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
	CustomerLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class AMConfigurationResults2 extends PXView {
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMConfigResultsFeature extends PXView {
	AMConfigurationFeature__Label: PXFieldState;
	MinSelection: PXFieldState;
	MaxSelection: PXFieldState;
	MinQty: PXFieldState;
	MaxQty: PXFieldState;
	LotQty: PXFieldState;
	TotalQty: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMConfigResultsAttribute extends PXView {
	ConfigResultsID: PXFieldState;
	ConfigurationID: PXFieldState;
	Revision: PXFieldState;
	AttributeLineNbr: PXFieldState;
	AMConfigurationAttribute__Label: PXFieldState;
	AMConfigurationAttribute__Descr: PXFieldState;
	Required: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Details,
	topBarItems: {
		ShowAll: { index: 0, config: { commandName: "ShowAll", text: Labels.ShowAll } },
	}
})
export class AMConfigResultsOption extends PXView {
	ConfigResultsID: PXFieldState;
	FeatureLineNbr: PXFieldState;
	OptionLineNbr: PXFieldState;
	IsRemovable: PXFieldState;
	Included: PXFieldState<PXFieldOptions.CommitChanges>;
	AMConfigurationOption__Label: PXFieldState;
	AMConfigurationOption__Descr: PXFieldState;
	Qty: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) UOM: PXFieldState;
	InventoryID: PXFieldState;
	SubItemID: PXFieldState;
}

export class CurrencyInfo extends PXView implements ICurrencyInfo {
	CuryInfoID: PXFieldState;
	BaseCuryID: PXFieldState;
	BaseCalc: PXFieldState;
	DisplayCuryID: PXFieldState;
	CuryRateTypeID: PXFieldState;
	BasePrecision: PXFieldState;
	CuryRate: PXFieldState;
	CuryEffDate: PXFieldState;
	RecipRate: PXFieldState;
	SampleCuryRate: PXFieldState;
	SampleRecipRate: PXFieldState;
	CuryID: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.ConfigurationEntryForAPI', primaryView: 'Results' })
export class AM306010 extends PXScreen {
	Results = createSingle(AMConfigurationResults);
	CurrentResults = createSingle(AMConfigurationResults2);
	CurrentFeatures = createCollection(AMConfigResultsFeature);
	Attributes = createCollection(AMConfigResultsAttribute);
	Options = createCollection(AMConfigResultsOption);
	_AMConfigurationResults_CurrencyInfo_ = createSingle(CurrencyInfo);
}

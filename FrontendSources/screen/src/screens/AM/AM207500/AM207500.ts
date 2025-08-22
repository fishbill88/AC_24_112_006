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
} from 'client-controls';

export class AMConfiguration extends PXView {
	ConfigurationID: PXFieldState<PXFieldOptions.CommitChanges>;
	Revision: PXFieldState<PXFieldOptions.CommitChanges>;
	Status: PXFieldState<PXFieldOptions.CommitChanges>;
	Descr: PXFieldState;
	BOMID: PXFieldState<PXFieldOptions.CommitChanges>;
	BOMRevisionID: PXFieldState;
	InventoryID: PXFieldState;
	IsCompletionRequired: PXFieldState;
	KeyFormat: PXFieldState<PXFieldOptions.CommitChanges>;
	KeyNumberingID: PXFieldState;
	KeyEquation: PXFieldState;
	KeyDescription: PXFieldState;
	TranDescription: PXFieldState;
	OnTheFlySubItems: PXFieldState;
	PriceRollup: PXFieldState;
	PriceCalc: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	autoRepaint: ["FeatureOptions", "FeatureRules"],
})
export class AMConfigurationFeature extends PXView {
	ConfigurationID: PXFieldState;
	Revision: PXFieldState;
	LineNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) FeatureID: PXFieldState<PXFieldOptions.CommitChanges>;
	Label: PXFieldState;
	Descr: PXFieldState;
	SortOrder: PXFieldState;
	MinSelection: PXFieldState;
	MaxSelection: PXFieldState;
	MinQty: PXFieldState;
	MaxQty: PXFieldState;
	LotQty: PXFieldState;
	Visible: PXFieldState;
	ResultsCopy: PXFieldState;
	PrintResults: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMConfigurationOption extends PXView {
	ConfigurationID: PXFieldState;
	Revision: PXFieldState;
	ConfigFeatureLineNbr: PXFieldState;
	LineNbr: PXFieldState;
	Label: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	FixedInclude: PXFieldState;
	QtyEnabled: PXFieldState;
	QtyRequired: PXFieldState<PXFieldOptions.CommitChanges>;
	UOM: PXFieldState;
	@columnConfig({ hideViewLink: true }) OperationID: PXFieldState<PXFieldOptions.CommitChanges>;
	MinQty: PXFieldState;
	MaxQty: PXFieldState;
	LotQty: PXFieldState;
	ScrapFactor: PXFieldState;
	BFlush: PXFieldState;
	MaterialType: PXFieldState<PXFieldOptions.CommitChanges>;
	PhantomRouting: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	LocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	SortOrder: PXFieldState;
	PriceFactor: PXFieldState;
	ResultsCopy: PXFieldState;
	QtyRoundUp: PXFieldState;
	BatchSize: PXFieldState;
	SubcontractSource: PXFieldState<PXFieldOptions.CommitChanges>;
	PrintResults: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMConfigurationFeatureRule extends PXView {
	RuleType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) SourceOptionLineNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) TargetFeatureLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ hideViewLink: true }) TargetOptionLineNbr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	autoRepaint: ["AttributeRules"]
})
export class AMConfigurationAttribute extends PXView {
	ConfigurationID: PXFieldState;
	Revision: PXFieldState;
	LineNbr: PXFieldState;
	AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsFormula: PXFieldState;
	Label: PXFieldState;
	Variable: PXFieldState;
	Descr: PXFieldState;
	Enabled: PXFieldState;
	Required: PXFieldState;
	Visible: PXFieldState;
	Value: PXFieldState;
	SortOrder: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
})
export class AMConfigurationAttributeRule extends PXView {
	RuleType: PXFieldState<PXFieldOptions.CommitChanges>;
	Condition: PXFieldState<PXFieldOptions.CommitChanges>;
	Value1: PXFieldState;
	Value2: PXFieldState;
	TargetFeatureLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetOptionLineNbr: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.ConfigurationMaint', primaryView: 'Documents' })
export class AM207500 extends PXScreen {
	Documents = createSingle(AMConfiguration);
	ConfigurationFeatures = createCollection(AMConfigurationFeature);
	FeatureOptions = createCollection(AMConfigurationOption);
	FeatureRules = createCollection(AMConfigurationFeatureRule);
	ConfigurationAttributes = createCollection(AMConfigurationAttribute);
	AttributeRules = createCollection(AMConfigurationAttributeRule);
}

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
	treeConfig,
	GridPreset,
	localizable,
	ICurrencyInfo,
} from 'client-controls';

@localizable
class Labels {
	static ShowAll = "Show All";
}

@treeConfig({
	dynamic: true,
	hideRootNode: true,
	dataMember: 'Features',
	//idParent: 'Key',
	iconField: 'Icon',
	//toolTipField : 'ToolTip',
	idName: 'LineNbr, OptionLineNbr',
	description: 'Label',
	modifiable: false,
	mode: 'single',
	singleClickSelect: true,
	selectFirstNode: true,
	syncPosition: true,
	//autoRepaint: ["CurrentFeature", "CurrentOption", "Options"],
})
export class AMConfigTreeNode extends PXView {
	LineNbr: PXFieldState;
	OptionLineNbr: PXFieldState;
	Label: PXFieldState;
	ToolTip: PXFieldState;
	SortOrder: PXFieldState;
	Icon: PXFieldState;
}

export class AMConfigurationResults extends PXView {
	InventoryID: PXFieldState;
	SiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	Completed: PXFieldState;
	IsConfigurationTesting: PXFieldState;
	CuryID: PXFieldState; //??
	DisplayPrice: PXFieldState;
	CustomerID: PXFieldState<PXFieldOptions.CommitChanges>;
}

//export class AMConfigurationResults2 extends PXView {
//}

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

export class AMConfigResultsFeature extends PXView {
	MinMaxSelection : PXFieldState;
	MinLotMaxQty : PXFieldState;
	TotalQty : PXFieldState;
}

export class AMConfigResultsOption extends PXView {
	MinLotMaxQty : PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	topBarItems: {
		ShowAll: { index: 0, config: { commandName: "ShowAll", text: Labels.ShowAll } },
	}
})
export class AMConfigResultsOption2 extends PXView {
	ShowAll : PXActionState;

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

@graphInfo({ graphType: 'PX.Objects.AM.ConfigurationEntry', primaryView: 'Results' })
export class AM306000 extends PXScreen {
	Features = createCollection(AMConfigTreeNode);
	Results = createSingle(AMConfigurationResults);
	//CurrentResults = createSingle(AMConfigurationResults2);
	Attributes = createCollection(AMConfigResultsAttribute);
	CurrentFeature = createSingle(AMConfigResultsFeature);
	CurrentOption = createSingle(AMConfigResultsOption);
	Options = createCollection(AMConfigResultsOption2);
	_AMConfigurationResults_CurrencyInfo_ = createSingle(CurrencyInfo);
}

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

export class Features extends PXView {
	FeatureID: PXFieldState;
	Descr: PXFieldState;
	ActiveFlg: PXFieldState;
	AllowNonInventoryOptions: PXFieldState<PXFieldOptions.CommitChanges>;
	DisplayOptionAttributes: PXFieldState;
	PrintResults: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
})
export class FeatureOptions extends PXView {
	FeatureID: PXFieldState;
	LineNbr: PXFieldState;
	Label: PXFieldState;
	InventoryID: PXFieldState<PXFieldOptions.CommitChanges>;
	SubItemID: PXFieldState;
	Descr: PXFieldState;
	FixedInclude: PXFieldState;
	QtyEnabled: PXFieldState;
	QtyRequired: PXFieldState;
	UOM: PXFieldState;
	MinQty: PXFieldState;
	MaxQty: PXFieldState;
	LotQty: PXFieldState;
	ScrapFactor: PXFieldState;
	BFlush: PXFieldState;
	MaterialType: PXFieldState;
	PhantomRouting: PXFieldState;
	PriceFactor: PXFieldState;
	ResultsCopy: PXFieldState;
	QtyRoundUp: PXFieldState;
	BatchSize: PXFieldState;
	SubcontractSource: PXFieldState;
	PrintResults: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	initNewRow: true,
})
export class FeatureAttributes extends PXView {
	FeatureID: PXFieldState;
	LineNbr: PXFieldState;
	@columnConfig({ hideViewLink: true }) AttributeID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsFormula: PXFieldState;
	Label: PXFieldState;
	Variable: PXFieldState;
	Descr: PXFieldState;
	Enabled: PXFieldState;
	Required: PXFieldState;
	Visible: PXFieldState;
	Value: PXFieldState;
}

@graphInfo({ graphType: 'PX.Objects.AM.FeatureMaint', primaryView: 'Features' })
export class AM203500 extends PXScreen {
	Features = createSingle(Features);
	FeatureOptions = createCollection(FeatureOptions);
	FeatureAttributes = createCollection(FeatureAttributes);
}

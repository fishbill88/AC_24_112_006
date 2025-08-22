import { createCollection, createSingle, PXScreen, graphInfo, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior, PXView, PXFieldState, gridConfig, headerDescription, ICurrencyInfo, disabled, selectorSettings, PXFieldOptions, linkCommand, columnConfig, GridColumnShowHideMode, GridColumnType, PXActionState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.FA.AssetClassMaint', primaryView: 'AssetClass', })
export class FA201000 extends PXScreen {

	@viewInfo({ containerName: 'Asset Class Summary' })
	AssetClass = createSingle(FixedAsset);

	CurrentAssetClass = createSingle(FixedAsset);

	@viewInfo({ containerName: 'Depreciation' })
	DepreciationSettings = createCollection(FABookSettings);

}

export class FixedAsset extends PXView {

	AssetCD: PXFieldState;
	ParentAssetID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	Active: PXFieldState;
	HoldEntry: PXFieldState;
	AssetTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	IsTangible: PXFieldState<PXFieldOptions.Disabled>;
	Depreciable: PXFieldState<PXFieldOptions.CommitChanges>;
	UnderConstruction: PXFieldState<PXFieldOptions.CommitChanges>;
	UsefulLife: PXFieldState<PXFieldOptions.CommitChanges>;
	AcceleratedDepreciation: PXFieldState;
	UsageScheduleID: PXFieldState;
	ServiceScheduleID: PXFieldState;
	ConstructionAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	ConstructionSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	FAAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	FASubID: PXFieldState<PXFieldOptions.CommitChanges>;
	FASubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	AccumulatedDepreciationAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	AccumulatedDepreciationSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	UseFASubMask: PXFieldState<PXFieldOptions.CommitChanges>;
	AccumDeprSubMask: PXFieldState;
	DepreciatedExpenseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DepreciatedExpenseSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	DeprExpenceSubMask: PXFieldState;
	DisposalAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DisposalSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProceedsSubMask: PXFieldState;
	GainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	GainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	LossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	LossSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	GainLossSubMask: PXFieldState;
	RentAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	RentSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	LeaseAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	LeaseSubID: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class FABookSettings extends PXView {

	BookID: PXFieldState;
	Depreciate: PXFieldState;
	AveragingConvention: PXFieldState;
	DepreciationMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	Bonus: PXFieldState;
	Sect179: PXFieldState;
	UsefulLife: PXFieldState;
	MidMonthType: PXFieldState;
	MidMonthDay: PXFieldState;
	UpdateGL: PXFieldState;
	PercentPerYear: PXFieldState<PXFieldOptions.CommitChanges>;

}

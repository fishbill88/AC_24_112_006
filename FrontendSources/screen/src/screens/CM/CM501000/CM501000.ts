import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, gridConfig, PXFieldOptions, columnConfig, GridColumnShowHideMode } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.TranslationProcess', primaryView: 'TranslationParamsFilter', })
export class CM501000 extends PXScreen {

	TranslationParamsFilter = createSingle(TranslationParams);
	TranslationCurrencyRateRecords = createCollection(CurrencyRate);

}

export class TranslationParams extends PXView {

	TranslDefId: PXFieldState<PXFieldOptions.CommitChanges>;
	FinPeriodID: PXFieldState<PXFieldOptions.CommitChanges>;
	LastFinPeriodID: PXFieldState<PXFieldOptions.Disabled>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	BranchID: PXFieldState<PXFieldOptions.Disabled>;
	SourceLedgerId: PXFieldState<PXFieldOptions.Disabled>;
	DestLedgerId: PXFieldState<PXFieldOptions.Disabled>;
	SourceCuryID: PXFieldState<PXFieldOptions.Disabled>;
	DestCuryID: PXFieldState<PXFieldOptions.Disabled>;

}

@gridConfig({mergeToolbarWith: 'ScreenToolbar'})
export class CurrencyRate extends PXView {

	FromCuryID: PXFieldState;
	ToCuryID: PXFieldState;
	CuryRateType_CurrencyRateType_Descr: PXFieldState;
	CuryRateType: PXFieldState;
	CuryEffDate: PXFieldState;
	CuryRate: PXFieldState;
	RateReciprocal: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	CuryRateID: PXFieldState<PXFieldOptions.Hidden>;

	CuryMultDiv: PXFieldState;

}

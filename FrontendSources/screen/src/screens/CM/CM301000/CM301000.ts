import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, GridColumnShowHideMode } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.CuryRateMaint', primaryView: 'Filter', })
export class CM301000 extends PXScreen {

	Filter = createSingle(CuryRateFilter);
	CuryRateRecordsEntry = createCollection(CurrencyRate);
	CuryRateRecordsEffDate = createCollection(CurrencyRate2);

}

export class CuryRateFilter extends PXView {
	ToCurrency: PXFieldState<PXFieldOptions.CommitChanges>;
	EffDate: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class CurrencyRate extends PXView {
	FromCuryID: PXFieldState;
	CuryRateType: PXFieldState;
	CuryEffDate: PXFieldState;
	CuryRate: PXFieldState;

	@columnConfig({ allowUpdate: false })
	RateReciprocal: PXFieldState;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	CuryRateID: PXFieldState<PXFieldOptions.Hidden>;

	CuryMultDiv: PXFieldState;
}

export class CurrencyRate2 extends PXView {

	@columnConfig({hideViewLink: true})
	FromCuryID: PXFieldState;

	@columnConfig({hideViewLink: true})
	CuryRateType: PXFieldState;

	CuryEffDate: PXFieldState;
	CuryRate: PXFieldState;
	CuryMultDiv: PXFieldState;
	RateReciprocal: PXFieldState;
}

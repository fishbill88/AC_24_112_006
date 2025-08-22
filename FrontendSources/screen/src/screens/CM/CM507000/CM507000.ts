import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions, gridConfig, columnConfig } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.RefreshCurrencyRates', primaryView: 'Filter', })
export class CM507000 extends PXScreen {

	Filter = createSingle(RefreshFilter);
	CurrencyRateList = createCollection(RefreshRate);

}

export class RefreshFilter extends PXView {

	CuryID: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryEffDate: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryRateTypeID: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar' })
export class RefreshRate extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;

	@columnConfig({ hideViewLink: true })
	FromCuryID: PXFieldState;

	@columnConfig({ hideViewLink: true })
	CuryRateType: PXFieldState;

	CuryRate: PXFieldState;

}

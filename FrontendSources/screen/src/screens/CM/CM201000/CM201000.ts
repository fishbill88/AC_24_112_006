import { createCollection, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions, gridConfig } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.CurrencyRateTypeMaint', primaryView: 'CuryRateTypeRecords' })
export class CM201000 extends PXScreen {

	CuryRateTypeRecords = createCollection(CurrencyRateType);

}

@gridConfig({ mergeToolbarWith: 'ScreenToolbar' })
export class CurrencyRateType extends PXView {
	CuryRateTypeID: PXFieldState;
	Descr: PXFieldState;
	RateEffDays: PXFieldState;
	RefreshOnline: PXFieldState<PXFieldOptions.CommitChanges>;
	OnlineRateAdjustment: PXFieldState;
}

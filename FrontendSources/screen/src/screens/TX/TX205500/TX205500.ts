import { createCollection, createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions, columnConfig, GridColumnShowHideMode, gridConfig } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.TaxCategoryMaint', primaryView: 'TxCategory', showUDFIndicator: true })
export class TX205500 extends PXScreen {

	TxCategory = createSingle(TaxCategory);
	Details = createCollection(TaxCategoryDet);

}

export class TaxCategory extends PXView {

	TaxCategoryID: PXFieldState;
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	Active: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxCatFlag: PXFieldState<PXFieldOptions.CommitChanges>;
	Exempt: PXFieldState;

}

@gridConfig({mergeToolbarWith: 'ScreenToolbar'})
export class TaxCategoryDet extends PXView {

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	TaxID: PXFieldState;
	Tax__Descr: PXFieldState;
	Tax__TaxType: PXFieldState;
	Tax__TaxCalcRule: PXFieldState;
	Tax__TaxApplyTermsDisc: PXFieldState;
	Tax__DirectTax: PXFieldState;

}

import { autoinject } from 'aurelia-framework';
import {
	createCollection,
	graphInfo,
	PXFieldOptions,
	PXFieldState,
	PXScreen,
	PXView,
	gridConfig,
	GridPagerMode,
	columnConfig,
	GridColumnShowHideMode
} from "client-controls";

@graphInfo({
	graphType: 'PX.Objects.CA.CAExternalTaxCalc',
	primaryView: 'Items'
})
@autoinject
export class CA501600 extends PXScreen {

	Items = createCollection(Items);
}

@gridConfig({
	adjustPageSize: true,
	syncPosition: true,
	batchUpdate: true,
	pagerMode: GridPagerMode.NextPrevFirstLast,
	quickFilterFields: ['AdjRefNbr', 'TranDesc'],
	mergeToolbarWith: 'ScreenToolbar'
})
export class Items extends PXView {
	@columnConfig({ allowCheckAll: true, allowShowHide: GridColumnShowHideMode.False }) Selected: PXFieldState;
	AdjTranType: PXFieldState;
	@columnConfig({ hideViewLink: true }) AdjRefNbr: PXFieldState;
	Status: PXFieldState;
	DrCr: PXFieldState;
	TranDate: PXFieldState;
	@columnConfig({ hideViewLink: true }) FinPeriodID: PXFieldState;
	CuryTranAmt: PXFieldState;
	@columnConfig({ hideViewLink: true }) CuryID: PXFieldState;
	TranDesc: PXFieldState;
}
